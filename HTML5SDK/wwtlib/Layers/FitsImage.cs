using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Net;
using System.Html.Data.Files;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public enum DataTypes { ByteT=0, Int16T=1, Int32T=2, FloatT=3, DoubleT=4, None=5 };
    public enum ScaleTypes { Linear=0, Log=1, Power=2, SquareRoot=3, HistogramEqualization=4 };


    public delegate void WcsLoaded(WcsImage wcsImage);

    public class FitsImage : WcsImage
    {
        Dictionary<String, String> header = new Dictionary<string, string>();
        public static FitsImage Last = null;

        private WcsLoaded callBack;
        public FitsImage(string file, Blob blob, WcsLoaded callMeBack)
        {
            Last = this;
            callBack = callMeBack;
            filename = file;
            if (blob != null)
            {
                ReadFromBlob(blob);
            }
            else
            {
                GetFile(file);
            }
        }

        WebFile webFile;

        public void GetFile(string url)
        {
            webFile = new WebFile(url);
            webFile.ResponseType = "blob";
            webFile.OnStateChange = FileStateChange;
            webFile.Send();
        }

        public void FileStateChange()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                System.Html.Data.Files.Blob mainBlob = (System.Html.Data.Files.Blob)webFile.GetBlob();
                ReadFromBlob(mainBlob);
            }
        }

        public Blob sourceBlob = null;

        private void ReadFromBlob(Blob blob)
        {
            sourceBlob = blob;
            FileReader chunck = new FileReader();
            chunck.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent e)
            {
                ReadFromBin(new BinaryReader(new Uint8Array(chunck.Result)));
                if (callBack != null)
                {
                    callBack.Invoke(this);
                }
            };
            chunck.ReadAsArrayBuffer(blob);
        }

        private void ReadFromBin(BinaryReader br)
        {
            ParseHeader(br);
        }


        public int[] Histogram;
        public int HistogramMaxCount;
        public int Width = 0;
        public int Height = 0;
        public int NumAxis = 0;
        public double BZero = 0;
        public int[] AxisSize;
        public object DataBuffer;
        public DataTypes DataType = DataTypes.None;
        public bool ContainsBlanks = false;
        public double BlankValue = double.MinValue;
        public double MaxVal = int.MinValue;
        public double MinVal = int.MaxValue;

        public int lastMin = 0;
        public int lastMax = 255;
        bool color = false;

        public static bool IsGzip(BinaryReader br)
        {

            byte[] line = br.ReadBytes(2);
            br.Seek(0);
            if (line[0] == 31 && line[1] == 139)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ParseHeader(BinaryReader br)
        {
            bool foundEnd = false;



            while (!foundEnd)
            {
                for (int i = 0; i < 36; i++)
                {
                    string data = br.ReadByteString(80);

                    if (!foundEnd)
                    {
                        string keyword = data.Substring(0, 8).TrimEnd();
                        string[] values = data.Substring(10).Split("/");
                        if (keyword.ToUpperCase() == "END")
                        {
                            foundEnd = true;
                            // Check for XTENSION
                            i++;
                            data = br.ReadByteString(80);
                            while (String.IsNullOrWhiteSpace(data))
                            {
                                i++;
                                data = br.ReadByteString(80);
                            }
                            keyword = data.Substring(0, 8).TrimEnd();
                            values = data.Substring(10).Split("/");
                            if (keyword.ToUpperCase() == "XTENSION")
                            {
                                // We have additional headers
                                foundEnd = false;
                            }
                            else
                            {
                                // Rewind these 80 bytes which could be data
                                br.SeekRelative(-80);
                            }
                        }
                        else
                        {
                            AddKeyword(keyword, values);
                        }
                    }

                }
            }


            NumAxis = Int32.Parse(header["NAXIS"]);

            ContainsBlanks = header.ContainsKey("BLANK");

            if (ContainsBlanks)
            {
                BlankValue = Double.Parse(header["BLANK"]);
            }

            if (header.ContainsKey("BZERO"))
            {
                BZero = Double.Parse(header["BZERO"]);
            }

            AxisSize = new int[NumAxis];

            for (int axis = 0; axis < NumAxis; axis++)
            {
                AxisSize[axis] = Int32.Parse(header[string.Format("NAXIS{0}", axis + 1)]);
                BufferSize *= AxisSize[axis];
            }

            int bitsPix = Int32.Parse(header["BITPIX"]);


            switch (bitsPix)
            {
                case 8:
                    this.DataType = DataTypes.ByteT;
                    InitDataBytes(br);
                    break;
                case 16:
                    this.DataType = DataTypes.Int16T;
                    InitDataShort(br);
                    break;
                case 32:
                    this.DataType = DataTypes.Int32T;
                    InitDataInt(br);
                    break;
                case -32:
                    this.DataType = DataTypes.FloatT;
                    InitDataFloat(br);
                    break;
                case -64:
                    this.DataType = DataTypes.DoubleT;
                    InitDataDouble(br);
                    break;
                default:
                    this.DataType = DataTypes.None;
                    break;
            }

            if (NumAxis > 1)
            {
                if (NumAxis == 3)
                {
                    if (AxisSize[2] == 3)
                    {
                        color = true;
                    }
                }
                sizeX = Width = AxisSize[0];
                sizeY = Height = AxisSize[1];
                ComputeWcs();
                Histogram = ComputeHistogram(256);
                HistogramMaxCount = Histogram[256];
            }
        }



        private void AddKeyword(string keyword, string[] values)
        {
            if (keyword != "CONTINUE" && keyword != "COMMENT" && keyword != "HISTORY" && !String.IsNullOrEmpty(keyword))
            {
                try
                {
                    if (header.ContainsKey(keyword))
                    {
                        header[keyword] = values[0].Trim();
                    }
                    else
                    {
                        header[keyword.ToUpperCase()] = values[0].Trim();
                    }

                }
                catch
                {
                }
            }
        }

        private void ComputeWcs()
        {
            if (header.ContainsKey("CROTA2"))
            {
                rotation = double.Parse(header["CROTA2"].Trim());
                hasRotation = true;

            }

            if (header.ContainsKey("CDELT1"))
            {
                scaleX = double.Parse(header["CDELT1"].Trim());

                if (header.ContainsKey("CDELT2"))
                {
                    scaleY = double.Parse(header["CDELT2"].Trim());
                    hasScale = true;
                }
            }

            if (header.ContainsKey("CRPIX1"))
            {
                referenceX = double.Parse(header["CRPIX1"].Trim()) - 1;

                if (header.ContainsKey("CRPIX2"))
                {
                    referenceY = double.Parse(header["CRPIX2"].Trim()) - 1;
                    hasPixel = true;
                }
            }
            bool galactic = false;
            bool tan = false;

            if (header.ContainsKey("CTYPE1"))
            {
                if (header["CTYPE1"].IndexOf("GLON-") > -1)
                {
                    galactic = true;
                    tan = true;
                }
                if (header["CTYPE2"].IndexOf("GLAT-") > -1)
                {
                    galactic = true;
                    tan = true;
                }

                if (header["CTYPE1"].IndexOf("-TAN") > -1)
                {
                    tan = true;
                }
                if (header["CTYPE1"].IndexOf("-SIN") > -1)
                {
                    tan = true;
                }
            }

            if (!tan)
            {
                throw new System.Exception("Only TAN projected images are supported: ");
            }

            hasSize = true;

            if (header.ContainsKey("CRVAL1"))
            {
                centerX = Double.Parse(header["CRVAL1"].Trim());

                if (header.ContainsKey("CRVAL2"))
                {
                    centerY = double.Parse(header["CRVAL2"].Trim());
                    hasLocation = true;
                }
            }

            if (galactic)
            {
                double[] result = Coordinates.GalactictoJ2000(centerX, centerY);
                centerX = result[0];
                centerY = result[1];
            }

            if (header.ContainsKey("CD1_1") && header.ContainsKey("CD1_2")
                && header.ContainsKey("CD2_1") && header.ContainsKey("CD2_2"))
            {
                cd1_1 = double.Parse(header["CD1_1"].Trim());
                cd1_2 = double.Parse(header["CD1_2"].Trim());
                cd2_1 = double.Parse(header["CD2_1"].Trim());
                cd2_2 = double.Parse(header["CD2_2"].Trim());
                if (!hasRotation)
                {
                    CalculateRotationFromCD();
                }
                if (!hasScale)
                {
                    CalculateScaleFromCD();
                }
                hasScale = true;
                hasRotation = true;
            }


            ValidWcs = hasScale && hasRotation && hasPixel && hasLocation;
        }

        public Bitmap GetHistogramBitmap(int max)
        {
            Bitmap bmp = Bitmap.Create(Histogram.Length, 150);
            //Graphics g = Graphics.FromImage(bmp);
            //g.Clear(Color.FromArgb(68, 82, 105));
            //Pen pen = new Pen(Color.FromArgb(127, 137, 157));
            //double logMax = Math.Log(HistogramMaxCount);
            //for (int i = 0; i < Histogram.Length; i++)
            //{
            //    double height = Math.Log(Histogram[i]) / logMax;
            //    if (height < 0)
            //    {
            //        height = 0;
            //    }


            //    g.DrawLine(Pens.White, new Point(i, 150), new Point(i, (int)(150 - (height * 150))));
            //}
            //pen.Dispose();
            //g.Flush();
            //g.Dispose();

            return bmp;
        }

        public void DrawHistogram(CanvasContext2D ctx)
        {
            ctx.ClearRect(0, 0, 255, 150);
            ctx.BeginPath();
            ctx.StrokeStyle = "rgba(255,255,255,255)";
            double logMax = Math.Log(HistogramMaxCount);
            for (int i = 0; i < Histogram.Length; i++)
            {
                double height = Math.Log(Histogram[i]) / logMax;
                if (height < 0)
                {
                    height = 0;
                }

                ctx.MoveTo(i, 150);
                ctx.LineTo(i, 150 - (height * 150));
                ctx.Stroke();
            }
           
        }

        public int[] ComputeHistogram(int count)
        {
            int[] histogram = new int[count+1];

            for(int i = 0; i < count+1; i++)
            {
                histogram[i] = 0;
            }

            switch (DataType)
            {
                case DataTypes.ByteT:
                    ComputeHistogramByte(histogram);
                    break;
                case DataTypes.Int16T:
                    ComputeHistogramInt16(histogram);
                    break;
                case DataTypes.Int32T:
                    ComputeHistogramInt32(histogram);
                    break;
                case DataTypes.FloatT:
                    ComputeHistogramFloat(histogram);
                    break;
                case DataTypes.DoubleT:
                    ComputeHistogramDouble(histogram);
                    break;
                case DataTypes.None:
                default:
                    break;
            }
            int maxCounter = 1;
            foreach (int val in histogram)
            {
                if (val > maxCounter)
                {
                    maxCounter = val;
                }
            }
            histogram[count] = maxCounter;
            return histogram;
        }

        private void ComputeHistogramDouble(int[] histogram)
        {
            int buckets = histogram.Length;
            double[] buf = (double[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (double val in buf)
            {
                if (!(val == double.NaN))
                {
                    histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
                }
            }
        }
        const float NaN = 0f / 0f;

        private void ComputeHistogramFloat(int[] histogram)
        {
            int buckets = histogram.Length;
            float[] buf = (float[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (float val in buf)
            {
                if (!(val == NaN))
                {
                    histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
                }
            }
        }

        private void ComputeHistogramInt32(int[] histogram)
        {
            int buckets = histogram.Length;
            Int32[] buf = (Int32[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Int32 val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }



        private void ComputeHistogramInt16(int[] histogram)
        {
            int buckets = histogram.Length;
            short[] buf = (short[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Int16 val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }

        private void ComputeHistogramByte(int[] histogram)
        {
            int buckets = histogram.Length;
            Byte[] buf = (Byte[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Byte val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }


        int BufferSize = 1;

        private void InitDataBytes(BinaryReader br)
        {
            byte[] buffer = new byte[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = br.ReadByte();
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataShort(BinaryReader br)
        {
            short[] buffer = new Int16[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (short)((br.ReadSByte() * 256) + (short)br.ReadByte());
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataUnsignedShort(BinaryReader br)
        {
            int[] buffer = new int[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (int)((((short)br.ReadSByte() * 256) + (short)br.ReadByte()) + 32768);
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataInt(BinaryReader br)
        {
            int[] buffer = new int[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (br.ReadSByte() << 24) + (br.ReadSByte() << 16) + (br.ReadSByte() << 8) + br.ReadByte();
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataFloat(BinaryReader br)
        {
            float[] buffer = new float[BufferSize];
            DataBuffer = buffer;
            Uint8Array part = new Uint8Array(4);
            for (int i = 0; i < BufferSize; i++)
            {
                part[3] = br.ReadByte();
                part[2] = br.ReadByte();
                part[1] = br.ReadByte();
                part[0] = br.ReadByte();

                buffer[i] = (new Float32Array(part.buffer, 0, 1))[0];

                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataDouble(BinaryReader br)
        {
            //double[] buffer = new double[BufferSize];
            //Uint8Array part = new Uint8Array(4);
            //DataBuffer = buffer;
            //for (int i = 0; i < BufferSize; i++)
            //{
            //    part[7] = br.ReadByte();
            //    part[6] = br.ReadByte();
            //    part[5] = br.ReadByte();
            //    part[4] = br.ReadByte();
            //    part[3] = br.ReadByte();
            //    part[2] = br.ReadByte();
            //    part[1] = br.ReadByte();
            //    part[0] = br.ReadByte();
            //    buffer[i] = buffer[i] = (new Float64Array(part.buffer, 0, 1))[0];

            //    if (MinVal > (double)buffer[i])
            //    {
            //        MinVal = (double)buffer[i];
            //    }
            //    if (MaxVal < (double)buffer[i])
            //    {
            //        MaxVal = (double)buffer[i];
            //    }
            //}
        }
        public ScaleTypes lastScale = ScaleTypes.Linear;
        public double lastBitmapMin = 0;
        public double lastBitmapMax = 0;
        override public Bitmap GetBitmap()
        {
            if (lastBitmapMax == 0 && lastBitmapMin == 0)
            {
                lastBitmapMin = MinVal;
                lastBitmapMax = MaxVal;
            }


            return GetScaledBitmap(lastBitmapMin, lastBitmapMax, lastScale);
        }

        public Bitmap GetScaledBitmap(double min, double max, ScaleTypes scaleType)
        {
            ScaleMap scale;
            lastScale = scaleType;
            lastBitmapMin = min;
            lastBitmapMax = max;

            switch (scaleType)
            {
                default:
                case ScaleTypes.Linear:
                    scale = new ScaleLinear(min, max);
                    break;
                case ScaleTypes.Log:
                    scale = new ScaleLog(min, max);
                    break;
                case ScaleTypes.Power:
                    scale = new ScalePow(min, max);
                    break;
                case ScaleTypes.SquareRoot:
                    scale = new ScaleSqrt(min, max);
                    break;
                case ScaleTypes.HistogramEqualization:
                    scale = new HistogramEqualization(this, min, max);
                    break;
            }

            try
            {
                switch (DataType)
                {
                    case DataTypes.ByteT:
                        return GetBitmapByte(min, max, scale);
                    case DataTypes.Int16T:
                        return GetBitmapShort(min, max, scale);
                    case DataTypes.Int32T:
                        return GetBitmapInt(min, max, scale);
                    case DataTypes.FloatT:
                        return GetBitmapFloat(min, max, scale);
                    case DataTypes.DoubleT:
                        return GetBitmapDouble(min, max, scale);
                    case DataTypes.None:
                    default:
                        return  Bitmap.Create(100, 100);
                }
            }
            catch
            {
                return Bitmap.Create(10, 10);
            }
        }

        private Bitmap GetBitmapByte(double min, double max, ScaleMap scale)
        {
            byte[] buf = (byte[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = Bitmap.Create(AxisSize[0], AxisSize[1]);

            for (int y = 0; y < AxisSize[1]; y++)
            {
                int indexY = ((AxisSize[1] - 1) - y);

                for (int x = 0; x < AxisSize[0]; x++)
                {
                    if (color)
                    {
                        int datR = buf[(x + indexY * stride)];
                        int datG = buf[(x + indexY * stride) + page];
                        int datB = buf[(x + indexY * stride) + page * 2];
                        if (ContainsBlanks && (double)datR == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            int r = scale.Map(datR);
                            int g = scale.Map(datG);
                            int b = scale.Map(datB);
                            bmp.SetPixel(x, y, r, g, b, 255);
                        }
                    }
                    else
                    {
                        int dataValue = buf[x + indexY * stride];
                        if (ContainsBlanks && (double)dataValue == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            Byte val = scale.Map(dataValue);

                            bmp.SetPixel(x, y, val, val, val, 255);
                        }
                    }
                }
            }
            return bmp;
        }

        private Bitmap GetBitmapDouble(double min, double max, ScaleMap scale)
        {
            double[] buf = (double[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = Bitmap.Create(AxisSize[0], AxisSize[1]);

            for (int y = 0; y < AxisSize[1]; y++)
            {
                int indexY = ((AxisSize[1] - 1) - y);
                for (int x = 0; x < AxisSize[0]; x++)
                {
                    if (color)
                    {
                        double datR = buf[(x + indexY * stride)];
                        double datG = buf[(x + indexY * stride) + page];
                        double datB = buf[(x + indexY * stride) + page * 2];
                        if (ContainsBlanks && (double)datR == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            int r = scale.Map(datR);
                            int g = scale.Map(datG);
                            int b = scale.Map(datB);
                            bmp.SetPixel(x, y, r, g, b, 255);
                        }
                    }
                    else
                    {
                        double dataValue = buf[x + indexY * stride];
                        if (ContainsBlanks && (double)dataValue == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            Byte val = scale.Map(dataValue);
                            bmp.SetPixel(x, y, val, val, val, 255);
                        }
                    }
                }
            }
            return bmp;
        }

        private Bitmap GetBitmapFloat(double min, double max, ScaleMap scale)
        {
            float[] buf = (float[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = Bitmap.Create(AxisSize[0], AxisSize[1]);
            for (int y = 0; y < AxisSize[1]; y++)
            {
                int indexY = ((AxisSize[1] - 1) - y);
                for (int x = 0; x < AxisSize[0]; x++)
                {
                    if (color)
                    {
                        double datR = buf[(x + indexY * stride)];
                        double datG = buf[(x + indexY * stride) + page];
                        double datB = buf[(x + indexY * stride) + page * 2];
                        if (ContainsBlanks && (double)datR == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            int r = scale.Map(datR);
                            int g = scale.Map(datG);
                            int b = scale.Map(datB);
                            bmp.SetPixel(x, y, r, g, b, 255);
                        }
                    }
                    else
                    {
                        double dataValue = buf[x + indexY * stride];
                        if (ContainsBlanks && (double)dataValue == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            Byte val = scale.Map(dataValue);
                            bmp.SetPixel(x, y, val, val, val, 255);
                        }
                    }
                }
            }
            return bmp;
        }

        private Bitmap GetBitmapInt(double min, double max, ScaleMap scale)
        {
            int[] buf = (int[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = Bitmap.Create(AxisSize[0], AxisSize[1]);

            for (int y = 0; y < AxisSize[1]; y++)
            {
                int indexY = ((AxisSize[1] - 1) - y);
                for (int x = 0; x < AxisSize[0]; x++)
                {
                    if (color)
                    {
                        int datR = buf[(x + indexY * stride)];
                        int datG = buf[(x + indexY * stride) + page];
                        int datB = buf[(x + indexY * stride) + page * 2];
                        if (ContainsBlanks && (double)datR == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            int r = scale.Map(datR);
                            int g = scale.Map(datG);
                            int b = scale.Map(datB);
                            bmp.SetPixel(x, y, r, g, b, 255);
                        }
                    }
                    else
                    {
                        int dataValue = buf[x + indexY * stride];
                        if (ContainsBlanks && (double)dataValue == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            Byte val = scale.Map(dataValue);
                            bmp.SetPixel(x, y, val, val, val, 255);
                        }
                    }
                }
            }

            return bmp;
        }
        public Bitmap GetBitmapShort(double min, double max, ScaleMap scale)
        {
            short[] buf = (short[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = Bitmap.Create(AxisSize[0], AxisSize[1]);

            for (int y = 0; y < AxisSize[1]; y++)
            {
                int indexY = ((AxisSize[1] - 1) - y);
              
                for (int x = 0; x < AxisSize[0]; x++)
                {
                    if (color)
                    {
                        int datR = buf[(x + indexY * stride)];
                        int datG = buf[(x + indexY * stride) + page];
                        int datB = buf[(x + indexY * stride) + page * 2];
                        if (ContainsBlanks && (double)datR == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            int r = scale.Map(datR);
                            int g = scale.Map(datG);
                            int b = scale.Map(datB);
                            bmp.SetPixel(x, y, r, g, b, 255);
                        }
                    }
                    else
                    {
                        int dataValue = buf[x + indexY * stride];
                        if (ContainsBlanks && (double)dataValue == BlankValue)
                        {
                            bmp.SetPixel(x, y, 0, 0, 0, 0);
                        }
                        else
                        {
                            Byte val = scale.Map(dataValue);
                            bmp.SetPixel(x, y, val, val, val, 255);
                        }
                    }

                }
            }
            return bmp;
        }
    }


    public abstract class ScaleMap
    {
        public abstract byte Map(double val);
    }

    public class ScaleLinear : ScaleMap
    {
        double min;
        double max;
        double factor;
        double logFactor;
        public ScaleLinear(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)(val - min) / factor * 255)));
        }
    }

    public class ScaleLog : ScaleMap
    {
        double min;
        double max;
        double factor;
        double logFactor;
        public ScaleLog(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            logFactor = 255 / Math.Log(255);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Log((val - min) / factor * 255) * logFactor)));
        }
    }

    public class ScalePow : ScaleMap
    {
        double min;
        double max;
        double factor;
        double powFactor;
        public ScalePow(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            powFactor = 255 / Math.Pow(255, 2);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Pow((val - min) / factor * 255, 2) * powFactor)));
        }
    }

    public class ScaleSqrt : ScaleMap
    {
        double min;
        double max;
        double factor;
        double sqrtFactor;
        public ScaleSqrt(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            sqrtFactor = 255 / Math.Sqrt(255);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Sqrt((val - min) / factor * 255) * sqrtFactor)));
        }
    }

    public class HistogramEqualization : ScaleMap
    {
        double min;
        double max;
        double factor;
        int[] Histogram;
        int maxHistogramValue = 1;
        Byte[] lookup;
        const int buckets = 10000;
        public HistogramEqualization(FitsImage image, double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            Histogram = image.ComputeHistogram(buckets);
            maxHistogramValue = Histogram[buckets];
            lookup = new Byte[buckets];
            int totalCounts = image.Width * image.Height;
            int sum = 0;
            for (int i = 0; i < buckets; i++)
            {
                sum += Histogram[i];
                lookup[i] = (Byte)(Math.Min(255, ((sum * 255.0)) / totalCounts) + .5);
            }
        }

        public override byte Map(double val)
        {
            return (Byte)lookup[Math.Min(buckets - 1, Math.Max(0, (int)((double)(val - min) / factor * (buckets - 1.0))))];
        }
    }
}
