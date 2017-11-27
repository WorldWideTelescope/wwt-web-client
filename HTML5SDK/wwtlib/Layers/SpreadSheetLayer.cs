using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;
using System.Net;

namespace wwtlib
{
     // public enum PointScaleTypes { Linear=0, Power=1, Log=2, Constant=3, StellarMagnitude=4 }; 
    
    
    class SpreadSheetLayer : Layer
    {
        public SpreadSheetLayer()
        {
        }

        public override string GetTypeName()
        {
            return "TerraViewer.SpreadSheetLayer";
        }

        public List<string> Header
        {
            get { return table.Header; }
        }

        //public SpreadSheetLayer(string filename)
        //{
        //    //todo file reading
        //    //string data = File.ReadAllText(filename);
        //    //LoadFromString(data, false, false, false, true);
        //    //ComputeDateDomainRange(-1, -1);
        //}

        public override bool CanCopyToClipboard()
        {

            return true;
        }

        public override void CopyToClipboard()
        {
            //todo copy binary format


       //     Clipboard.SetText(table.ToString());

        }




        bool dataDirty = false;
        //public SpreadSheetLayer(string data, bool something)
        //{
        //    LoadFromString(data, false, false, false, true);
        //    ComputeDateDomainRange(-1, -1);
        //}

        public bool DynamicUpdate()
        {
            string data = GetDatafromFeed(DataSourceUrl);
            if (data != null)
            {
                UpadteData(data, false, true, true);
                GuessHeaderAssignments();
                return true;
            }
            return false;
        }

        private static string GetDatafromFeed(string url)
        {
            //string xml = ExecuteQuery(url);

            //if (xml == null)
            //{
            //    return null;
            //}

            //try
            //{

            //    XmlDocument xmlDoc = new XmlDocument();
            //    XmlNamespaceManager xmlNsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            //    xmlNsMgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
            //    xmlNsMgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            //    xmlNsMgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");

            //    xmlDoc.LoadXml(xml);
            //    XmlNodeList elements = xmlDoc.DocumentElement.SelectNodes("./atom:entry", xmlNsMgr);
            //    StringBuilder sb = new StringBuilder();

            //    if (elements != null && elements.Count > 0)
            //    {
            //        // Add ODATA properties as first row
            //        XmlNodeList properties = elements[0].SelectSingleNode("./atom:content/m:properties", xmlNsMgr).ChildNodes;
            //        int columnCount = 1;
            //        foreach (XmlNode property in properties)
            //        {
            //            if (columnCount != 1)
            //            {
            //                sb.Append("\t");
            //            }

            //            sb.Append(property.Name.Substring(property.Name.IndexOf(":") + 1, property.Name.Length - property.Name.IndexOf(":") - 1));
            //            columnCount++;
            //        }

            //        sb.AppendLine(string.Empty);

            //        // Add ODATA property values from second row onwards
            //        foreach (XmlNode element in elements)
            //        {
            //            XmlNodeList propertyValues = element.SelectSingleNode("./atom:content/m:properties", xmlNsMgr).ChildNodes;
            //            // Reset Column Count
            //            columnCount = 1;
            //            foreach (XmlNode propertyValue in propertyValues)
            //            {
            //                if (columnCount != 1)
            //                {
            //                    sb.Append("\t");
            //                }

            //                sb.Append(propertyValue.InnerText);
            //                columnCount++;
            //            }

            //            sb.AppendLine(string.Empty);
            //        }
            //    }

            //    return sb.ToString();
            //}
            //catch
            //{
            //    return xml;
            //}

            return "";
        }

        

        private static string ExecuteQuery(string url)
        {
            //try
            //{

            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            //    request.Method = "GET";
            //    request.Accept = "application/atom+xml, text/plain";
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    {
            //        using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
            //        {
            //            return readStream.ReadToEnd();
            //        }
            //    }
            //}
            //catch
            //{
            //    return null;
            //}
            return "";
        }

        public override bool UpadteData(object data, bool purgeOld, bool purgeAll, bool hasHeader)
        {

            LoadFromString(data as string, true, purgeOld, purgeAll, hasHeader);
            ComputeDateDomainRange(-1, -1);
            dataDirty = true;
            return true;
        }

        public override void LoadData(TourDocument tourDoc, string filename)
        {
            table = new Table();
            System.Html.Data.Files.Blob blob = tourDoc.GetFileBlob(filename);
            this.GetStringFromGzipBlob(blob, delegate (string data)
            {
                table.LoadFromString(data, false, true, true);
                ComputeDateDomainRange(-1, -1);

                if (DynamicData && AutoUpdate)
                {
                    DynamicUpdate();
                }
                dataDirty = true;
                dirty = true;
            });
        }


        string fileName;


        public override void AddFilesToCabinet(FileCabinet fc)
        {
 
            fileName = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID, this.ID.ToString());

            string dir = fileName.Substring(0, fileName.LastIndexOf("\\"));
           
            string data = table.Save();

            System.Html.Data.Files.Blob blob = new System.Html.Data.Files.Blob(new object[] { data });
            
            fc.AddFile(fileName, blob);

            base.AddFilesToCabinet(fc);
        }

        public void GuessHeaderAssignments()
        {
            int index = 0;
            foreach (string headerName in table.Header)
            {
                string name = headerName.ToLowerCase();

                if (name.IndexOf("lat") > -1 && latColumn == -1)
                {
                    latColumn = index;
                }

                if ((name.IndexOf("lon") > -1 || name.IndexOf("lng") > -1) && lngColumn == -1)
                {
                    lngColumn = index;
                }

                if (name.IndexOf("dec") > -1 && latColumn == -1)
                {
                    latColumn = index;
                    astronomical = true;
                }

                if ((name.IndexOf("ra") > -1 || name.IndexOf("ascen") > -1) && lngColumn == -1)
                {
                    lngColumn = index;
                    astronomical = true;
                    pointScaleType = PointScaleTypes.StellarMagnitude;
                }

                if ((name.IndexOf("mag") > -1 || name.IndexOf("size") > -1) && sizeColumn == -1)
                {
                    sizeColumn = index;
                }

                if ((name.IndexOf("date") > -1  || name.IndexOf("time") > -1  || name.IndexOf("dt") > -1  || name.IndexOf("tm") > -1))
                {
                    if (name.IndexOf("end") > -1  && endDateColumn == -1)
                    {
                        endDateColumn = index;
                    }
                    else if (startDateColumn == -1)
                    {
                        startDateColumn = index;
                    }
                }


                if ((name.IndexOf("altitude") > -1  || name.IndexOf("alt") > -1 ) && altColumn == -1)
                {
                    altColumn = index;
                    AltType = AltTypes.Altitude;
                    AltUnit = AltUnits.Meters;
                }

                if (name.IndexOf("depth") > -1  && altColumn == -1)
                {
                    altColumn = index;
                    AltType = AltTypes.Depth;
                    AltUnit = AltUnits.Kilometers;
                }

                if (name.StartsWith("x") && XAxisColumn == -1)
                {
                    XAxisColumn = index;
                }

                if (name.StartsWith("y") && YAxisColumn == -1)
                {
                    YAxisColumn = index;
                }

                if (name.StartsWith("z") && ZAxisColumn == -1)
                {
                    ZAxisColumn = index;
                }

                if (name.IndexOf("color") > -1 && ColorMapColumn == -1)
                {
                    ColorMapColumn = index;
                }

                if ((name.IndexOf("geometry") > -1 || name.IndexOf("geography") > -1) && geometryColumn == -1)
                {
                    geometryColumn = index;
                }
                index++;
            }

            if (table.Header.Count > 0)
            {
                nameColumn = 0;
            }
        }


        public void ComputeDateDomainRange(int columnStart, int columnEnd)
        {
            if (columnStart == -1)
            {
                columnStart = startDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = endDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = columnStart;
            }

            BeginRange = new Date("12/31/2100");
            EndRange = new Date("12/31/1890");

            foreach (string[] row in table.Rows)
            {
                try
                {
                    if (columnStart > -1)
                    {
                        bool sucsess = false;
                        Date dateTimeStart = new Date("12/31/2100");
                        try
                        {
                            dateTimeStart = new Date(row[columnStart]);
                            if (dateTimeStart < BeginRange)
                            {
                                BeginRange = dateTimeStart;
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            Date dateTimeEnd = new Date("12/31/1890");



                            if (columnEnd > -1)
                            {
                                dateTimeEnd = new Date(row[columnEnd]);
                                if (sucsess && dateTimeEnd > EndRange)
                                {
                                    EndRange = dateTimeEnd;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }


        //      public struct ColumnStats
        //{
        //    int TargetColumn = -1;
        //    int FilterColumn = -1;
        //    string FilterValue = null;
        //    double Min = 0;
        //    double Max = 0;
        //    double Average = 0;
        //    double Median = 0;
        //    double Sum = 0;
        //    double Count = 0;
        //    int[] Histogram = null;
        //    int Buckets=256;
        //    double BucketWidth =0; 
        //    bool Computed = false;

        //}

      

        public void CheckState()
        {

        }
      





        public double GetMaxValue(int column)
        {

            double max = 0;
            table.Lock();
            foreach (string[] row in table.Rows)
            {
                try
                {
                    if (column > -1)
                    {
                        bool sucsess = false;
                        try
                        {
                            double val = double.Parse(row[column]);

                            if (sucsess && val > max)
                            {
                                max = val;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
            table.Unlock();
            return max;
        }

        public List<string> GetDomainValues(int column)
        {
            List<string> domainValues = new List<string>();
            table.Lock();
            foreach (string[] row in table.Rows)
            {
                try
                {
                    if (column > -1)
                    {
                        if (!domainValues.Contains(row[column]))
                        {
                            domainValues.Add(row[column]);
                        }
                    }
                }
                catch
                {
                }
            }
            domainValues.Sort();
            table.Unlock();
            return domainValues;
        }


        int barChartBitmask = 0;

        public int BarChartBitmask
        {
            get { return barChartBitmask; }
            set { barChartBitmask = value; }
        }
        double barScaleFactor = 20;

     

        private double meanRadius = 6371000;
        
        protected bool PrepVertexBuffer(RenderContext renderContext, float opacity)
        {
            table.Lock();
            if (lineList != null)
            {
                lineList.Clear();
            }

            if (lineList2d != null)
            {
                lineList2d.Clear();
            }

            if (triangleList != null)
            {
                triangleList.Clear();
            }

            if (pointList != null)
            {
                pointList.Clear();
            }
            

            if (triangleList2d != null)
            {
                triangleList2d.Clear();
            }

            if (lineList == null)
            {
                lineList = new LineList();
            }

            if (pointList == null)
            {
                pointList = new PointList(renderContext);
            }

            lineList.TimeSeries = this.timeSeries;

            if (lineList2d == null)
            {
                lineList2d = new LineList();
                lineList2d.DepthBuffered = false;

            }

            lineList.TimeSeries = this.timeSeries;



            if (triangleList == null)
            {
                triangleList = new TriangleList();
            }

            if (triangleList2d == null)
            {
                triangleList2d = new TriangleList();
                triangleList2d.DepthBuffered = false;
            }

          

            
            positions.Clear();
            UInt32 currentIndex = 0;
         //   device.RenderState.FillMode = FillMode.WireFrame;
            Color colorLocal = Color;

           // colorLocal.A = (byte)(Color.A * Opacity);

            // for space 3d
            double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

            

            Dictionary<string, bool> selectDomain = new Dictionary<string, bool>();


            double mr = 0;

        //    double mr = LayerManager.AllMaps[ReferenceFrame].Frame.MeanRadius;
            if (mr != 0)
            {
                meanRadius = mr;
            }

            Vector3d position = new Vector3d();
            float pointSize = .0002f;
            Color pointColor = Colors.White ;
            float pointStartTime = 0;
            float pointEndTime = 0;
            foreach (string[] row in table.Rows)
            {
                try
                {
                    bool selected = false;

                    if (geometryColumn > -1 || (this.CoordinatesType == CoordinatesTypes.Spherical && (lngColumn > -1 && latColumn > -1)) || ((this.CoordinatesType == CoordinatesTypes.Rectangular) && (XAxisColumn > -1 && YAxisColumn > -1)))
                    {
                        double Xcoord = 0;
                        double Ycoord = 0;
                        double Zcoord = 0;

                        double alt = 1;
                        double altitude = 0;
                        double distParces = 0;
                        double factor = GetScaleFactor(AltUnit, 1);
                        if (altColumn == -1 || AltType == AltTypes.SeaLevel || bufferIsFlat)
                        {
                            alt = 1;
                            if (astronomical & !bufferIsFlat)
                            {
                                alt = UiTools.AuPerLightYear * 100;
                            }
                        }
                        else
                        {
                            if (AltType == AltTypes.Depth)
                            {
                                factor = -factor;
                            }

                            alt = 0;
                            try
                            {
                                alt = double.Parse(row[altColumn]);
                            }
                            catch
                            {
                            }

                            if (astronomical)
                            {
                                factor = factor / (1000 * UiTools.KilometersPerAu);
                                distParces = (alt * factor) / UiTools.AuPerParsec;

                                altitude = (factor * alt);
                                alt = (factor * alt);
                            }
                            else if (AltType == AltTypes.Distance)
                            {
                                altitude = (factor * alt);
                                alt = (factor * alt / meanRadius);
                            }
                            else
                            {
                                altitude = (factor * alt);
                                alt = 1 + (factor * alt / meanRadius);
                            }
                        }

                        //todo remove hack when alt is fixed
                        //alt = 1;

                        if (CoordinatesType == CoordinatesTypes.Spherical && lngColumn > -1 && latColumn > -1)
                        {
                            //Xcoord = Coordinates.Parse(row[lngColumn]);
                            //Ycoord = Coordinates.Parse(row[latColumn]);
                            Xcoord = double.Parse(row[lngColumn]);
                            Ycoord = double.Parse(row[latColumn]);

                            if (astronomical)
                            {
                                if (RaUnits == RAUnits.Hours)
                                {
                                    Xcoord *= 015;
                                }
                                if (bufferIsFlat)
                                {
                                 //   Xcoord += 180;
                                }
                                
                            }
                            double offset = 0; //todo EGM96Geoid.Height(Ycoord, Xcoord);
                            //   if (altitude != 0)
                            {
                                //altitude += offset;
                                //alt += offset / meanRadius;
                            }
                            Vector3d pos = Coordinates.GeoTo3dDoubleRad(Ycoord, Xcoord, alt);

                            if (astronomical && !bufferIsFlat)
                            {
                                pos.RotateX(ecliptic);
                            }

                            position = pos;

                            positions.Add(position);

                        }
                        else if (this.CoordinatesType == CoordinatesTypes.Rectangular)
                        {
                            double xyzScale = GetScaleFactor(CartesianScale, CartesianCustomScale) / meanRadius;

                            if (ZAxisColumn > -1)
                            {
                                Zcoord = double.Parse(row[ZAxisColumn]);
                            }

                            Xcoord = double.Parse(row[XAxisColumn]);
                            Ycoord = double.Parse(row[YAxisColumn]);

                            if (XAxisReverse)
                            {
                                Xcoord = -Xcoord;
                            }
                            if (YAxisReverse)
                            {
                                Ycoord = -Ycoord;
                            }
                            if (ZAxisReverse)
                            {
                                Zcoord = -Zcoord;
                            }


                            position = Vector3d.Create((Xcoord * xyzScale), (Zcoord * xyzScale), (Ycoord * xyzScale));
                            positions.Add(position);
                        }

                        // SqlGeometry pntGeo = SqlGeometry.Point(Xcoord,Ycoord, 4326);


                        //// SqlGeometry pntGeo = SqlGeometry.Point(new SqlChars(String.Format("Point ({0} {1})", Xcoord,Ycoord).ToCharArray()), 4326);


                        // if (!geo.STContains(pntGeo))
                        // {
                        //     continue;
                        // }

                        switch (ColorMap)
                        {
                            case ColorMaps.Same_For_All:
                                pointColor = colorLocal;
                                break;
                            case ColorMaps.Per_Column_Literal:
                                if (ColorMapColumn > -1)
                                {
                                    pointColor = ParseColor(row[ColorMapColumn], colorLocal);
                                }
                                else
                                {
                                    pointColor = colorLocal;
                                }
                                break;
                            //case ColorMaps.Group_by_Range:
                            //    break;
                            //case ColorMaps.Gradients_by_Range:
                            //    break;       
                            //case ColorMaps.Group_by_Values:
                            //    pointColor = ColorDomainValues[row[ColorMapColumn]].MarkerIndex;
                            //    break;

                            default:
                                break;
                        }


                        if (sizeColumn > -1)
                        {
                            switch (pointScaleType)
                            {
                                case PointScaleTypes.Linear:
                                    pointSize = Single.Parse(row[sizeColumn]);
                                    break;
                                case PointScaleTypes.Log:
                                    pointSize = (float)Math.Log(Single.Parse(row[sizeColumn]));
                                    break;
                                case PointScaleTypes.Power:
                                    {
                                        double size = 0; 

                                        try
                                        {
                                            pointSize = (float)double.Parse(row[sizeColumn]);
                                            pointSize = Math.Pow(2, pointSize);
                                        }
                                        catch
                                        {
                                            pointSize = 0;
                                        }
                                    }
                                    break;
                                case PointScaleTypes.StellarMagnitude:
                                    {
                                        double size = 0;
                                        try
                                        {
                                            size = double.Parse(row[sizeColumn]);

                                            if (!bufferIsFlat)
                                            {
                                                size = size - 5 * ((Util.LogN(distParces, 10) - 1));
                                                pointSize = (float)(120000000 / Math.Pow(1.6, size));
                                            }
                                            else
                                            {
                                                pointSize = (float)(40 / Math.Pow(1.6, size));
                                            }

                                        }
                                        catch
                                        {
                                            pointSize = 0;
                                        }

                                    }
                                    break;

                                case PointScaleTypes.Constant:
                                    pointSize = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            pointSize = (float)1;
                        }
                        if (PlotType == PlotTypes.Point)
                        {
                            pointSize = (float)1;
                        }

                        if (astronomical & !bufferIsFlat)
                        {
                            //  lastItem.PointSize *= 1000000000000000000000000000f;
                        }


                        if (startDateColumn > -1)
                        {
                            Date dateTime = new Date(row[startDateColumn]);
                            pointStartTime = (float)(SpaceTimeController.UtcToJulian(dateTime) - SpaceTimeController.UtcToJulian(baseDate));

                            if (endDateColumn > -1)
                            {
                                dateTime = new Date(row[endDateColumn]);
                                //dateTime = DateTime.Parse(row[endDateColumn]);
                                pointEndTime = (float)(SpaceTimeController.UtcToJulian(dateTime) - SpaceTimeController.UtcToJulian(baseDate));
                            }
                            else
                            {
                                pointEndTime = pointStartTime;
                            }
                        }

                        pointList.AddPoint(position, pointColor, new Dates(pointStartTime, pointEndTime), pointSize);
                       

                        if (geometryColumn > -1)
                        {
                            ParseGeometry(row[geometryColumn], pointColor, pointColor, altitude, new Dates(pointStartTime, pointEndTime));
                        }

                        //if (barChartBitmask != 0)
                        //{
                        //    MakeBarChart(device, row, Ycoord, Xcoord, pointSize, factor, Color.FromArgb(lastItem.Color), selected, new Dates(pointStartTime, pointEndTime));
                        //}


                        currentIndex++;
                    }
                }
                catch
                {
                }
                lines = false;
            }


            table.Unlock();
            dataDirty = false;
            dirty = false;
            return false;
        }

        private void ParseGeometry(string gs, Color lineColor, Color polyColor, double alt, Dates date)
        {


            gs = gs.Trim().ToLowerCase();

            int index = gs.IndexOf('(');

            if (index < 0)
            {
                return;
            }

            if (!gs.EndsWith(")"))
            {
                return;
            }
            string commandPart = gs.Substring(0, index).Trim();

            string parens = gs.Substr(index);

            string[] parts = commandPart.Split(" ");

            string command = null;
            string mods = null;
            if (parts.Length > 0)
            {
                foreach (string item in parts)
                {
                    if (string.IsNullOrEmpty(command))
                    {
                        command = item;
                    }
                    else if (string.IsNullOrEmpty(mods))
                    {
                        mods = item;
                    }
                }
            }

            switch (command)
            {
                case "multipolygon":
                case "polygon":
                    {
                        ParsePolygon(parens, mods, lineColor, polyColor, alt, date);

                    }
                    break;
                case "multilinestring":
                    {
                        ParseLineString(parens, mods, lineColor, alt, false, date);
                    }
                    break;
                case "linestring":
                    {
                        ParseLineString(parens, mods, lineColor, alt, true, date);
                    }
                    break;
                case "geometrycollection":
                    {
                        parens = parens.Substring(1, parens.Length - 2);
                        List<string> shapes = UiTools.SplitString(parens, ",");
                        foreach (string shape in shapes)
                        {
                            ParseGeometry(shape, lineColor, polyColor, alt, date);
                        }
                    }
                    break;
                default:
                    break;
            }

        }



        private void ParsePolygon(string parens, string mods, Color lineColor, Color polyColor, double alt, Dates date)
        {

            if (!parens.StartsWith("(") && parens.EndsWith(")"))
            {
                return;
            }
            // string the top level of parens
            parens = parens.Substring(1, parens.Length - 2);

            List<string> shapes = UiTools.SplitString(parens, ",");
            foreach (string shape in shapes)
            {
                KmlLineList lineList = new KmlLineList();
                lineList.Astronomical = astronomical;
                lineList.MeanRadius = meanRadius;
                lineList.ParseWkt(shape, mods, alt, date);
                if (alt == 0)
                {
                    AddPolygonFlat(false, lineList, 1, polyColor, lineColor, true, true, date);
                }
                else
                {
                    AddPolygon(false, lineList, 1, polyColor, lineColor, true, true, date);
                }
            }
        }

        private void ParseLineString(string parens, string mods, Color lineColor, double alt, bool single, Dates date)
        {

            if (!parens.StartsWith("(") && parens.EndsWith(")"))
            {
                return;
            }
            if (!single)
            {
                // string the top level of parens
                parens = parens.Substring(1, parens.Length - 2);
            }
            List<string> shapes = UiTools.SplitString(parens, ",");
            foreach (string shape in shapes)
            {
                KmlLineList lineList = new KmlLineList();
                lineList.Astronomical = astronomical;
                lineList.MeanRadius = meanRadius;

                lineList.ParseWkt(shape, mods, alt, date);
                AddPolygon(false, lineList, 1, Colors.White, lineColor, false, false, date);
            }


        }

        private List<string> SplitShapes(string shapes)
        {
            List<string> shapeList = new List<string>();

            int nesting = 0;

            int current = 0;
            while (current < shapes.Length)
            {
                if (shapes.Substr(current,1) == "(")
                {
                    nesting++;
                }
            }

            return shapeList;
        }

       

        private void AddPolygon(bool sky, KmlLineList geo, float lineWidth, Color polyColor, Color lineColor, bool extrude, bool fill, Dates date)
        {

            //todo can we save this work for later?
            List<Vector3d> vertexList = new List<Vector3d>();
            List<Vector3d> vertexListGround = new List<Vector3d>();

            //todo list 
            // We need to Wrap Around for complete polygone
            // we aldo need to do intereor
            //todo space? using RA/DEC
            for (int i = 0; i < (geo.PointList.Count); i++)
            {
                vertexList.Add(Coordinates.GeoTo3dDoubleRad(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / meanRadius)));
                vertexListGround.Add(Coordinates.GeoTo3dDoubleRad(geo.PointList[i].Lat, geo.PointList[i].Lng, 1));
            }


            for (int i = 0; i < (geo.PointList.Count - 1); i++)
            {
                if (sky)
                {
                    //tdo reenable this
                    //this.lineList2d.AddLine
                    //    (Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15 + 12, geo.PointList[i].Lat, 1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15 + 12, geo.PointList[i + 1].Lat, 1), lineColor, date);
                }
                else
                {
                    if (extrude)
                    {

                        this.triangleList.AddQuad(vertexList[i], vertexList[i + 1], vertexListGround[i], vertexListGround[i + 1], polyColor, date);

                    }
                    if (lineWidth > 0)
                    {
                        if (extrude)
                        {
                            this.lineList.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                        }
                        else
                        {
                            this.lineList2d.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                        }
                        if (extrude)
                        {
                            this.lineList.AddLine(vertexListGround[i], vertexListGround[i + 1], lineColor, date);
                            this.lineList.AddLine(vertexList[i], vertexListGround[i], lineColor, date);
                            this.lineList.AddLine(vertexList[i + 1], vertexListGround[i + 1], lineColor, date);
                        }
                    }
                }
            }
            if (fill)
            {
                List<int> indexes = Tessellator.TesselateSimplePoly(vertexList);

                for (int i = 0; i < indexes.Count; i += 3)
                {
                    this.triangleList.AddTriangle(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], polyColor, date);
                }
            }
        }

        private void AddPolygonFlat(bool sky, KmlLineList geo, float lineWidth, Color polyColor, Color lineColor, bool extrude, bool fill, Dates date)
        {

            //todo can we save this work for later?
            List<Vector3d> vertexList = new List<Vector3d>();

            for (int i = 0; i < (geo.PointList.Count); i++)
            {
                vertexList.Add(Coordinates.GeoTo3dDoubleRad(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / meanRadius)));
            }


            for (int i = 0; i < (geo.PointList.Count - 1); i++)
            {
                if (sky)
                {
                    //this.lineList2d.AddLine
                    //    (Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15 + 12, geo.PointList[i].Lat, 1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15 + 12, geo.PointList[i + 1].Lat, 1), lineColor, date);
                }
                else
                {
                    if (lineWidth > 0)
                    {
                        this.lineList2d.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                    }
                }
            }
            if (fill)
            {
                List<int> indexes = Tessellator.TesselateSimplePoly(vertexList);

                for (int i = 0; i < indexes.Count; i += 3)
                {
                    this.triangleList2d.AddSubdividedTriangles(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], polyColor, date, 2);
                }
            }
        }

        // todo fix this mess
        private Color ParseColor(string colorText, Color defaultColor)
        {
            return Color.Load(colorText);


        }

        public static Date ParseDate(string date)
        {
            Date dt = Date.Now;
            try
            {
                dt = new Date(date);
            }
            catch
            {
                try
                {
                    return ExeclToDateTime(double.Parse(date));
                }
                catch
                {
                }

            }

            return dt;
        }

        

        public static Date ExeclToDateTime(double excelDate)
        {
            if (excelDate > 59)
            {
                excelDate -= 1;
            }
            if (excelDate > 730000)
            {
                excelDate = 730000;
            }
            Date es = new Date(1899, 12, 31);
            return new Date(es.GetDate() + (int)(excelDate * 24 * 60 * 60 * 1000));
        }

        public double GetScaleFactor(AltUnits AltUnit, double custom)
        {
            double factor = 1;

            switch (AltUnit)
            {
                case AltUnits.Meters:
                    factor = 1;
                    break;
                case AltUnits.Feet:
                    factor = 1 * 0.3048;
                    break;
                case AltUnits.Inches:
                    factor = (1.0 / 12.0) * 0.3048;
                    break;
                case AltUnits.Miles:
                    factor = 5280 * 0.3048;
                    break;
                case AltUnits.Kilometers:
                    factor = 1000;
                    break;
                case AltUnits.AstronomicalUnits:
                    factor = 1000 * UiTools.KilometersPerAu;
                    break;
                case AltUnits.LightYears:
                    factor = 1000 * UiTools.KilometersPerAu * UiTools.AuPerLightYear;
                    break;
                case AltUnits.Parsecs:
                    factor = 1000 * UiTools.KilometersPerAu * UiTools.AuPerParsec;
                    break;
                case AltUnits.MegaParsecs:
                    factor = 1000 * UiTools.KilometersPerAu * UiTools.AuPerParsec * 1000000;
                    break;
                case AltUnits.Custom:
                    factor = custom;
                    break;
                default:
                    break;
            }
            return factor;
        }

    //    public override Place FindClosest(Coordinates target, float distance, IPlace defaultPlace, bool astronomical)
     //   {
            //Vector3 searchPoint = Coordinates.GeoTo3d(target.Lat, target.Lng);

            ////searchPoint = -searchPoint;
            //Vector3 dist;
            //if (defaultPlace != null)
            //{
            //    Vector3 testPoint = Coordinates.RADecTo3d(defaultPlace.RA, -defaultPlace.Dec, -1.0).Vector3;
            //    dist = searchPoint - testPoint;
            //    distance = dist.Length();
            //}

            //int closestItem = -1;
            //int index = 0;
            //foreach (Vector3 point in positions)
            //{
            //    dist = searchPoint - point;
            //    if (dist.Length() < distance)
            //    {
            //        distance = dist.Length();
            //        closestItem = index;
            //    }
            //    index++;
            //}


            //if (closestItem == -1)
            //{
            //    return defaultPlace;
            //}

            //Coordinates pnt = Coordinates.CartesianToSpherical2(positions[closestItem]);

            //string name = table.Rows[closestItem][this.nameColumn];
            //if (nameColumn == startDateColumn || nameColumn == endDateColumn)
            //{
            //    name = ParseDate(name).ToString("u");
            //}

            //if (String.IsNullOrEmpty(name))
            //{
            //    name = string.Format("RA={0}, Dec={1}", Coordinates.FormatHMS(pnt.RA), Coordinates.FormatDMS(pnt.Dec));
            //}
            //TourPlace place = new TourPlace(name, pnt.Lat, pnt.Lng, Classification.Unidentified, "", ImageSetType.Earth, -1);

            //Dictionary<String, String> rowData = new Dictionary<string, string>();
            //for (int i = 0; i < table.Header.GetLength(0); i++)
            //{
            //    string colValue = table.Rows[closestItem][i];
            //    if (i == startDateColumn || i == endDateColumn)
            //    {
            //        colValue = ParseDate(colValue).ToString("u");
            //    }

            //    if (!rowData.ContainsKey(table.Header[i]) && !string.IsNullOrEmpty(table.Header[i]))
            //    {
            //        rowData.Add(table.Header[i], colValue);
            //    }
            //    else
            //    {
            //        rowData.Add("Column" + i.ToString(), colValue);
            //    }
            //}
            //place.Tag = rowData;

            //return place;
     //   }




        Table table = new Table();

        internal Table Table
        {
            get { return table; }
            set { table = value; }
        }

        public void LoadFromString(string data, bool isUpdate, bool purgeOld, bool purgeAll, bool hasHeader)
        {

            if (!isUpdate)
            {
                table = new Table();
            }
            table.Lock();
            table.LoadFromString(data, isUpdate, purgeAll, hasHeader);
            if (!isUpdate)
            {
                GuessHeaderAssignments();
            }

            if (astronomical && lngColumn > -1)
            {
                double max = GetMaxValue(lngColumn);
                if (max > 24)
                {
                    RaUnits = RAUnits.Degrees;
                }
            }


            if (purgeOld)
            {
                PurgeByTime();
            }
            table.Unlock();
        }

        public void PurgeByTime()
        {
            if (startDateColumn < 0)
            {
                return;
            }
            int columnToUse = startDateColumn;
            if (endDateColumn > -1)
            {
                columnToUse = endDateColumn;
            }

            Date threasholdTime = SpaceTimeController.Now;
            int ts = (int)decay*24*60*60*1000;
            threasholdTime = new Date(threasholdTime.GetDate() - ts);

            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    List<string> row = table.Rows[i];
                    Date colDate = new Date(row[columnToUse]);
                    if (colDate < threasholdTime)
                    {
                        table.Rows.RemoveAt(i);
                        count--;
                        i--;
                    }
                }
                catch
                {
                }
            }
        }

        public override void CleanUp()
        {
            CleanUpBase();
            table.Lock();
            base.CleanUp();
            table.Unlock();

            dirty = true;
        }

        //public override void InitFromXml(XmlNode node)
        //{
        //    base.InitFromXml(node);
        //}

        public override void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("TimeSeries", TimeSeries.ToString());
            xmlWriter.WriteAttributeString("BeginRange", Util.XMLDate(BeginRange));
            xmlWriter.WriteAttributeString("EndRange", Util.XMLDate(EndRange));
            xmlWriter.WriteAttributeString("Decay", Decay.ToString());
            xmlWriter.WriteAttributeString("CoordinatesType", Enums.ToXml("CoordinatesTypes",(int)CoordinatesType));
            xmlWriter.WriteAttributeString("LatColumn", LatColumn.ToString());
            xmlWriter.WriteAttributeString("LngColumn", LngColumn.ToString());
            xmlWriter.WriteAttributeString("GeometryColumn", GeometryColumn.ToString());
            xmlWriter.WriteAttributeString("AltType", Enums.ToXml("AltTypes", (int)AltType));
            xmlWriter.WriteAttributeString("MarkerMix", Enums.ToXml("MarkerMixes", (int)MarkerMix));
            xmlWriter.WriteAttributeString("ColorMap", Enums.ToXml("ColorMaps", (int)ColorMap));
            xmlWriter.WriteAttributeString("MarkerColumn", MarkerColumn.ToString());
            xmlWriter.WriteAttributeString("ColorMapColumn", ColorMapColumn.ToString());
            xmlWriter.WriteAttributeString("PlotType", Enums.ToXml("PlotTypes", (int)PlotType));
            xmlWriter.WriteAttributeString("MarkerIndex", MarkerIndex.ToString());
            xmlWriter.WriteAttributeString("MarkerScale", Enums.ToXml("MarkerScales",(int)MarkerScale));
            xmlWriter.WriteAttributeString("AltUnit", AltUnit.ToString());
            xmlWriter.WriteAttributeString("AltColumn", AltColumn.ToString());
            xmlWriter.WriteAttributeString("StartDateColumn", StartDateColumn.ToString());
            xmlWriter.WriteAttributeString("EndDateColumn", EndDateColumn.ToString());
            xmlWriter.WriteAttributeString("SizeColumn", SizeColumn.ToString());
            xmlWriter.WriteAttributeString("HyperlinkFormat", HyperlinkFormat.ToString());
            xmlWriter.WriteAttributeString("HyperlinkColumn", HyperlinkColumn.ToString());
            xmlWriter.WriteAttributeString("ScaleFactor", ScaleFactor.ToString());
            xmlWriter.WriteAttributeString("PointScaleType", Enums.ToXml("PointScaleTypes", (int)PointScaleType));
            xmlWriter.WriteAttributeString("ShowFarSide", ShowFarSide.ToString());
            xmlWriter.WriteAttributeString("RaUnits", Enums.ToXml("RAUnits", (int)RaUnits));
            xmlWriter.WriteAttributeString("HoverTextColumn", NameColumn.ToString());
            xmlWriter.WriteAttributeString("XAxisColumn", XAxisColumn.ToString());
            xmlWriter.WriteAttributeString("XAxisReverse", XAxisReverse.ToString());
            xmlWriter.WriteAttributeString("YAxisColumn", YAxisColumn.ToString());
            xmlWriter.WriteAttributeString("YAxisReverse", YAxisReverse.ToString());
            xmlWriter.WriteAttributeString("ZAxisColumn", ZAxisColumn.ToString());
            xmlWriter.WriteAttributeString("ZAxisReverse", ZAxisReverse.ToString());
            xmlWriter.WriteAttributeString("CartesianScale", Enums.ToXml("AltUnits", (int)CartesianScale));
            xmlWriter.WriteAttributeString("CartesianCustomScale", CartesianCustomScale.ToString());
            xmlWriter.WriteAttributeString("DynamicData", DynamicData.ToString());
            xmlWriter.WriteAttributeString("AutoUpdate", AutoUpdate.ToString());
            xmlWriter.WriteAttributeString("DataSourceUrl", DataSourceUrl.ToString());

        }

       

        protected bool isLongIndex = false;
        protected int shapeVertexCount;


        protected bool lines = false;
        protected int latColumn = -1;
        protected float fixedSize = 1;
        protected float decay = 16;
        protected bool timeSeries = false;

        private bool dynamicData = false;


        public bool DynamicData
        {
            get { return dynamicData; }
            set { dynamicData = value; }
        }

        private bool autoUpdate = false;


        public bool AutoUpdate
        {
            get { return autoUpdate; }
            set { autoUpdate = value; }
        }

        string dataSourceUrl = "";

        public string DataSourceUrl
        {
            get { return dataSourceUrl; }
            set { dataSourceUrl = value; }
        }





        public bool TimeSeries
        {
            get { return timeSeries; }
            set
            {
                if (timeSeries != value)
                {
                    version++;
                    timeSeries = value;
                }
            }
        }


        Date beginRange = new Date("1/1/2100");


        public Date BeginRange
        {
            get { return beginRange; }
            set
            {
                if (beginRange != value)
                {
                    version++;
                    beginRange = value;
                }
            }
        }
        Date endRange = new Date("01/01/1800");

        public Date EndRange
        {
            get { return endRange; }
            set
            {
                if (endRange != value)
                {
                    version++;
                    endRange = value;
                }
            }
        }


        public override void InitializeFromXml(XmlNode node)
        {
            TimeSeries = bool.Parse(node.Attributes.GetNamedItem("TimeSeries").Value);
            BeginRange = new Date(node.Attributes.GetNamedItem("BeginRange").Value);
            EndRange = new Date(node.Attributes.GetNamedItem("EndRange").Value);
            Decay = Single.Parse(node.Attributes.GetNamedItem("Decay").Value);
            CoordinatesType = (CoordinatesTypes)Enums.Parse("CoordinatesTypes", node.Attributes.GetNamedItem("CoordinatesType").Value);
           

            if ((int)CoordinatesType < 0)
            {
                CoordinatesType = CoordinatesTypes.Spherical;
            }
            LatColumn = int.Parse(node.Attributes.GetNamedItem("LatColumn").Value);
            LngColumn = int.Parse(node.Attributes.GetNamedItem("LngColumn").Value);
            if (node.Attributes.GetNamedItem("GeometryColumn") != null)
            {
                GeometryColumn = int.Parse(node.Attributes.GetNamedItem("GeometryColumn").Value);
            }

            AltType = (AltTypes)Enums.Parse("AltTypes", node.Attributes.GetNamedItem("AltType").Value);
            
            MarkerMix = MarkerMixes.Same_For_All;
            ColorMap = (ColorMaps)Enums.Parse("ColorMaps",node.Attributes.GetNamedItem("ColorMap").Value);

            MarkerColumn = int.Parse(node.Attributes.GetNamedItem("MarkerColumn").Value);
            ColorMapColumn = int.Parse(node.Attributes.GetNamedItem("ColorMapColumn").Value);
            PlotType = (PlotTypes)Enums.Parse("PlotTypes",node.Attributes.GetNamedItem("PlotType").Value);
            
            MarkerIndex = int.Parse(node.Attributes.GetNamedItem("MarkerIndex").Value);
            MarkerScale = (MarkerScales)Enums.Parse("MarkerScales", node.Attributes.GetNamedItem("MarkerScale").Value);
            AltUnit = (AltUnits)Enums.Parse("AltUnits", node.Attributes.GetNamedItem("AltUnit").Value);
            AltColumn = int.Parse(node.Attributes.GetNamedItem("AltColumn").Value);
            StartDateColumn = int.Parse(node.Attributes.GetNamedItem("StartDateColumn").Value);
            EndDateColumn = int.Parse(node.Attributes.GetNamedItem("EndDateColumn").Value);
            SizeColumn = int.Parse(node.Attributes.GetNamedItem("SizeColumn").Value);
            HyperlinkFormat = node.Attributes.GetNamedItem("HyperlinkFormat").Value;
            HyperlinkColumn = int.Parse(node.Attributes.GetNamedItem("HyperlinkColumn").Value);
            ScaleFactor = Single.Parse(node.Attributes.GetNamedItem("ScaleFactor").Value);
            PointScaleType = (PointScaleTypes)Enums.Parse("PointScaleTypes", node.Attributes.GetNamedItem("PointScaleType").Value);
          
            if (node.Attributes.GetNamedItem("ShowFarSide") != null)
            {
                ShowFarSide = Boolean.Parse(node.Attributes.GetNamedItem("ShowFarSide").Value);
            }

            if (node.Attributes.GetNamedItem("RaUnits") != null)
            {
                RaUnits = (RAUnits)Enums.Parse("RAUnits", node.Attributes.GetNamedItem("RaUnits").Value);
            }

            if (node.Attributes.GetNamedItem("HoverTextColumn") != null)
            {
                NameColumn = int.Parse(node.Attributes.GetNamedItem("HoverTextColumn").Value);
            }

            if (node.Attributes.GetNamedItem("XAxisColumn") != null)
            {
                XAxisColumn = int.Parse(node.Attributes.GetNamedItem("XAxisColumn").Value);
                XAxisReverse = bool.Parse(node.Attributes.GetNamedItem("XAxisReverse").Value);
                YAxisColumn = int.Parse(node.Attributes.GetNamedItem("YAxisColumn").Value);
                YAxisReverse = bool.Parse(node.Attributes.GetNamedItem("YAxisReverse").Value);
                ZAxisColumn = int.Parse(node.Attributes.GetNamedItem("ZAxisColumn").Value);
                ZAxisReverse = bool.Parse(node.Attributes.GetNamedItem("ZAxisReverse").Value);
                CartesianScale = (AltUnits)Enums.Parse("AltUnits",node.Attributes.GetNamedItem("CartesianScale").Value);
                CartesianCustomScale = double.Parse(node.Attributes.GetNamedItem("CartesianCustomScale").Value);
            }

            if (node.Attributes.GetNamedItem("DynamicData") != null)
            {
                DynamicData = bool.Parse(node.Attributes.GetNamedItem("DynamicData").Value);
                AutoUpdate = bool.Parse(node.Attributes.GetNamedItem("AutoUpdate").Value);
                DataSourceUrl = node.Attributes.GetNamedItem("DataSourceUrl").Value;
            }
        }


        public Dictionary<string, DomainValue> MarkerDomainValues = new Dictionary<string, DomainValue>();
        public Dictionary<string, DomainValue> ColorDomainValues = new Dictionary<string, DomainValue>();




        public float Decay
        {
            get { return decay; }
            set
            {
                if (decay != value)
                {
                    version++;
                    decay = value;
                }
            }
        }


        private CoordinatesTypes coordinatesType = CoordinatesTypes.Spherical;


        public CoordinatesTypes CoordinatesType
        {
            get { return coordinatesType; }
            set
            {
                if (coordinatesType != value)
                {
                    version++;
                    coordinatesType = value;
                }
            }
        }


        public int LatColumn
        {
            get { return latColumn; }
            set
            {
                if (latColumn != value)
                {
                    version++;
                    latColumn = value;
                }
            }
        }
        protected int lngColumn = -1;


        public int LngColumn
        {
            get { return lngColumn; }
            set
            {
                if (lngColumn != value)
                {
                    version++;
                    lngColumn = value;
                }
            }
        }

        protected int geometryColumn = -1;


        public int GeometryColumn
        {
            get { return geometryColumn; }
            set
            {
                if (geometryColumn != value)
                {
                    version++;
                    geometryColumn = value;
                }
            }
        }

        private int xAxisColumn = -1;


        public int XAxisColumn
        {
            get { return xAxisColumn; }
            set
            {
                if (xAxisColumn != value)
                {
                    version++;
                    xAxisColumn = value;
                }
            }
        }
        private int yAxisColumn = -1;


        public int YAxisColumn
        {
            get { return yAxisColumn; }
            set
            {
                if (yAxisColumn != value)
                {
                    version++;
                    yAxisColumn = value;
                }
            }
        }
        private int zAxisColumn = -1;


        public int ZAxisColumn
        {
            get { return zAxisColumn; }
            set
            {
                if (zAxisColumn != value)
                {
                    version++;
                    zAxisColumn = value;
                }
            }
        }

        private bool xAxisReverse = false;


        public bool XAxisReverse
        {
            get { return xAxisReverse; }
            set
            {
                if (xAxisReverse != value)
                {
                    version++;
                    xAxisReverse = value;
                }
            }
        }
        private bool yAxisReverse = false;


        public bool YAxisReverse
        {
            get { return yAxisReverse; }
            set
            {
                if (yAxisReverse != value)
                {
                    version++;
                    yAxisReverse = value;
                }
            }
        }
        private bool zAxisReverse = false;


        public bool ZAxisReverse
        {
            get { return zAxisReverse; }
            set
            {
                if (zAxisReverse != value)
                {
                    version++;
                    zAxisReverse = value;
                }
            }
        }

        private AltTypes altType = AltTypes.SeaLevel;


        public AltTypes AltType
        {
            get { return altType; }
            set
            {
                if (altType != value)
                {
                    version++;
                    altType = value;
                }
            }
        }


        private MarkerMixes markerMix = MarkerMixes.Same_For_All;


        public MarkerMixes MarkerMix
        {
            get { return markerMix; }
            set
            {
                if (markerMix != value)
                {
                    version++;
                    markerMix = value;
                }
            }
        }

        RAUnits raUnits = RAUnits.Hours;

        public RAUnits RaUnits
        {
            get { return raUnits; }
            set
            {

                if (raUnits != value)
                {
                    version++;
                    raUnits = value;
                }
            }
        }

        private ColorMaps colorMap = ColorMaps.Per_Column_Literal;


        internal ColorMaps ColorMap
        {
            get { return colorMap; }
            set
            {
                if (colorMap != value)
                {
                    version++;
                    colorMap = value;
                }
            }
        }


        private int markerColumn = -1;


        public int MarkerColumn
        {
            get { return markerColumn; }
            set
            {
                if (markerColumn != value)
                {
                    version++;
                    markerColumn = value;
                }
            }
        }

        private int colorMapColumn = -1;


        public int ColorMapColumn
        {
            get { return colorMapColumn; }
            set
            {
                if (colorMapColumn != value)
                {
                    version++;
                    colorMapColumn = value;
                }
            }
        }

        private PlotTypes plotType = PlotTypes.Gaussian;


        public PlotTypes PlotType
        {
            get { return plotType; }
            set
            {
                if (plotType != value)
                {
                    version++;
                    plotType = value;
                }

            }
        }

        private int markerIndex = 0;


        public int MarkerIndex
        {
            get { return markerIndex; }
            set
            {
                if (markerIndex != value)
                {
                    version++;
                    markerIndex = value;
                }
            }
        }

        private bool showFarSide = false;


        public bool ShowFarSide
        {
            get { return showFarSide; }
            set
            {
                if (showFarSide != value)
                {
                    version++;
                    showFarSide = value;
                }
            }
        }


        private MarkerScales markerScale = MarkerScales.World;


        public MarkerScales MarkerScale
        {
            get { return markerScale; }
            set
            {
                if (markerScale != value)
                {
                    version++;
                    markerScale = value;
                }
            }
        }


        private AltUnits altUnit = AltUnits.Meters;


        public AltUnits AltUnit
        {
            get { return altUnit; }
            set
            {
                if (altUnit != value)
                {
                    version++;
                    altUnit = value;
                }
            }
        }
        private AltUnits cartesianScale = AltUnits.Meters;


        public AltUnits CartesianScale
        {
            get { return cartesianScale; }
            set
            {
                if (cartesianScale != value)
                {
                    version++;
                    cartesianScale = value;
                }
            }
        }

        private double cartesianCustomScale = 1;


        public double CartesianCustomScale
        {
            get { return cartesianCustomScale; }
            set
            {
                if (cartesianCustomScale != value)
                {
                    version++;
                    cartesianCustomScale = value;
                }
            }
        }

        protected int altColumn = -1;

        public int AltColumn
        {
            get { return altColumn; }
            set
            {
                if (altColumn != value)
                {
                    version++;
                    altColumn = value;
                }
            }
        }

        protected int startDateColumn = -1;


        public int StartDateColumn
        {
            get { return startDateColumn; }
            set
            {
                if (startDateColumn != value)
                {
                    version++;
                    startDateColumn = value;
                }
            }
        }
        protected int endDateColumn = -1;


        public int EndDateColumn
        {
            get { return endDateColumn; }
            set
            {
                if (endDateColumn != value)
                {
                    version++;
                    endDateColumn = value;
                }
            }
        }

        protected int sizeColumn = -1;


        public int SizeColumn
        {
            get { return sizeColumn; }
            set
            {
                if (sizeColumn != value)
                {
                    version++;
                    sizeColumn = value;
                }
            }
        }
        protected int nameColumn = 0;


        public int NameColumn
        {
            get { return nameColumn; }
            set
            {
                if (nameColumn != value)
                {
                    version++;
                    nameColumn = value;
                }
            }
        }
        private string hyperlinkFormat = "";


        public string HyperlinkFormat
        {
            get { return hyperlinkFormat; }
            set
            {
                if (hyperlinkFormat != value)
                {
                    version++; hyperlinkFormat = value;
                }
            }
        }

        private int hyperlinkColumn = -1;


        public int HyperlinkColumn
        {
            get { return hyperlinkColumn; }
            set
            {
                if (hyperlinkColumn != value)
                {
                    version++;
                    hyperlinkColumn = value;
                }
            }
        }


        protected float scaleFactor = 1.0f;


        public float ScaleFactor
        {
            get { return scaleFactor; }
            set
            {
                if (scaleFactor != value)
                {
                    version++;

                    scaleFactor = value;
                }
            }
        }

        protected PointScaleTypes pointScaleType = PointScaleTypes.Power;


        public PointScaleTypes PointScaleType
        {
            get { return pointScaleType; }
            set
            {
                if (pointScaleType != value)
                {
                    version++;
                    pointScaleType = value;
                }
            }
        }



        protected List<Vector3d> positions = new List<Vector3d>();

        protected LineList lineList;
        protected LineList lineList2d;
        protected TriangleList triangleList;
        protected TriangleList triangleList2d;

        protected PointList pointList;

        protected bool bufferIsFlat = false;

        protected Date baseDate = new Date(2010, 0, 1, 12, 00, 00);

        static ImageElement circleTexture = null;

        static ImageElement CircleTexture
        {
            get
            {
                //if (circleTexture == null)
                //{
                //    circleTexture = UiTools.LoadTextureFromBmp(Tile.prepDevice, Properties.Resources.circle, 0);
                //}

                return circleTexture;
            }
        }
        public bool dirty = true;


        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {

            RenderContext device = renderContext;

            //if (shaderA == null)
            //{
            //    MakeVertexShaderA(device);
            //}

            if (bufferIsFlat != flat)
            {
                CleanUp();
                bufferIsFlat = flat;
            }

            if (dirty)
            {
                PrepVertexBuffer(device, opacity);
            }

            double jNow = SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate);

            float adjustedScale = scaleFactor;

            if (flat && astronomical && (markerScale == MarkerScales.World))
            {
                adjustedScale = (float)(scaleFactor / (renderContext.ViewCamera.Zoom / 360));
            }

            if (triangleList2d != null)
            {
                triangleList2d.Decay = decay;
                triangleList2d.Sky = this.Astronomical;
                triangleList2d.TimeSeries = timeSeries;
                triangleList2d.JNow = jNow;
                triangleList2d.Draw(renderContext, opacity * Opacity, CullMode.Clockwise);
            }

            if (triangleList != null)
            {

                triangleList.Decay = decay;
                triangleList.Sky = this.Astronomical;
                triangleList.TimeSeries = timeSeries;
                triangleList.JNow = jNow;
                triangleList.Draw(renderContext, opacity * Opacity, CullMode.Clockwise);
            }


            if (pointList != null)
            {
                pointList.DepthBuffered = false;
                pointList.Decay = decay;
                pointList.Sky = this.Astronomical;
                pointList.TimeSeries = timeSeries;
                pointList.JNow = jNow;
                pointList.scale = (markerScale == MarkerScales.World) ? (float)adjustedScale : -(float)adjustedScale;
                pointList.Draw(renderContext, opacity * Opacity, false);
            }

            if (lineList != null)
            {
                lineList.Sky = this.Astronomical;
                lineList.Decay = decay;
                lineList.TimeSeries = timeSeries;
                lineList.JNow = jNow;
                lineList.DrawLines(renderContext, opacity * Opacity);
            }

            if (lineList2d != null)
            {
                lineList2d.Sky = this.Astronomical;
                lineList2d.Decay = decay;
                lineList2d.TimeSeries = timeSeries;
                lineList2d.ShowFarSide = ShowFarSide;
                lineList2d.JNow = jNow;
                lineList2d.DrawLines(renderContext, opacity * Opacity);
            }

            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
            //device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;
            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;

            //TextureOperation oldTexOp = device.TextureState[0].ColorOperation;

            //bool zBufferEnabled = device.RenderState.ZBufferEnable;

            //if (astronomical && !bufferIsFlat)
            //{
            //    device.RenderState.ZBufferEnable = true;
            //}
            //else
            //{
            //    device.RenderState.ZBufferEnable = false;
            //}
            //device.TextureState[0].ColorOperation = TextureOperation.Disable;

            //FillMode oldMode = device.RenderState.FillMode;
            //DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);
            //device.RenderState.FillMode = FillMode.Solid;
            //device.SetTexture(0, null);
            //device.Indices = shapeFileIndex;
            //device.VertexShader = shaderA;
            //// Vector3 cam = Vector3d.TransformCoordinate(Earth3d.cameraPosition, Matrix3d.Invert(renderContext.World)).Vector3;
            //Vector3 cam = Vector3.TransformCoordinate(renderContext.CameraPosition.Vector3, Matrix.Invert(renderContext.Device.Transform.World));
            //constantTableA.SetValue(device, cameraHandleA, new Vector4(cam.X, cam.Y, cam.Z, 1));
            //constantTableA.SetValue(device, jNowHandleA, (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate)));
            //constantTableA.SetValue(device, decayHandleA, timeSeries ? decay : 0f);

            //float adjustedScale = scaleFactor;

            //if (flat && astronomical && (markerScale == MarkerScales.World))
            //{
            //    adjustedScale = (float)(scaleFactor / (Earth3d.MainWindow.ZoomFactor / 360));
            //}
            //constantTableA.SetValue(device, scaleHandleA, (markerScale == MarkerScales.World) ? (float)adjustedScale : -(float)adjustedScale);
            //constantTableA.SetValue(device, skyHandleA, astronomical ? -1 : 1);
            //constantTableA.SetValue(device, opacityHandleA, opacity * this.Opacity);
            //constantTableA.SetValue(device, showFarSideHandleA, ShowFarSide ? 1f : 0f);

            //// Matrix matrixWVP = Earth3d.WorldMatrix * Earth3d.ViewMatrix * Earth3d.ProjMatrix;
            ////Matrix matrixWVP = device.Transform.World * device.Transform.View * device.Transform.Projection;
            //Matrix3d matrixWVP = renderContext.World * renderContext.View * renderContext.Projection;

            //constantTableA.SetValue(device, worldViewHandleA, matrixWVP.Matrix);

            //device.SetStreamSource(0, shapeFileVertex, 0);
            ////device.VertexFormat = VertexFormats.None;
            ////device.VertexDeclaration = vertexDeclA;
            //device.VertexFormat = PointVertex.Format;

            //device.RenderState.PointSpriteEnable = plotType != PlotTypes.Point;

            //device.RenderState.PointScaleEnable = (markerScale == MarkerScales.World && plotType != PlotTypes.Point) ? true : false;
            //device.RenderState.PointSize = 0;
            //device.RenderState.PointScaleA = 0;
            //device.RenderState.PointScaleB = 0;

            //device.RenderState.PointScaleC = 10000000f;

            //switch (plotType)
            //{
            //    case PlotTypes.Gaussian:
            //        device.SetTexture(0, Grids.StarProfile);
            //        break;
            //    case PlotTypes.Circle:
            //        device.SetTexture(0, CircleTexture);
            //        break;
            //    case PlotTypes.Point:
            //        device.SetTexture(0, null);
            //        break;
            //    //case PlotTypes.Square:
            //    //    device.SetTexture(0, null);
            //    //    break;
            //    //case PlotTypes.Custom:
            //    //    break;  
            //    case PlotTypes.PushPin:
            //        device.SetTexture(0, PushPin.GetPushPinTexture(markerIndex));
            //        break;

            //    default:
            //        break;
            //}



            //device.RenderState.CullMode = Cull.None;
            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
            //if (plotType == PlotTypes.Gaussian)
            //{
            //    device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.One;
            //}
            //else
            //{
            //    device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;
            //}


            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
            //device.TextureState[0].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
            //device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

            //device.TextureState[1].ColorOperation = TextureOperation.Disable;
            //device.TextureState[1].ColorArgument1 = TextureArgument.Current;
            //device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
            //device.TextureState[1].AlphaOperation = TextureOperation.Disable;
            //device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
            //device.TextureState[1].AlphaArgument2 = TextureArgument.Constant;

            //device.TextureState[1].ConstantColor = Color.FromArgb(255, 255, 255, 255);
            ////                device.TextureState[1].ConstantColor = Color.FromArgb(0, 0, 0, 0);



            //device.DrawPrimitives(PrimitiveType.PointList, 0, shapeVertexCount);
            //device.RenderState.PointSpriteEnable = false;


            ////device.DrawUserPrimitives(PrimitiveType.LineList, segments, points);

            //device.RenderState.FillMode = oldMode;
            //device.TextureState[0].ColorOperation = oldTexOp;
            //device.VertexShader = null;

            //device.RenderState.ZBufferEnable = zBufferEnabled;
            //device.RenderState.AlphaBlendEnable = false;
            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlue;
            return true;
        }


        //protected static EffectHandle worldViewHandleA = null;
        //protected static EffectHandle cameraHandleA = null;
        //protected static EffectHandle jNowHandleA = null;
        //protected static EffectHandle decayHandleA = null;
        //protected static EffectHandle scaleHandleA = null;
        //protected static EffectHandle skyHandleA = null;
        //protected static EffectHandle opacityHandleA = null;
        //protected static EffectHandle showFarSideHandleA = null;
        //protected static ConstantTable constantTableA = null;
        //protected static VertexShader shaderA = null;
        //protected static VertexDeclaration vertexDeclA = null;

        //protected static void MakeVertexShaderA(Device device)
        //{
        //    // Create the vertex shader and declaration
        //    VertexElement[] elements = new VertexElement[]
        //        {
        //            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        //            new VertexElement(0, 0, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.PointSize, 0),
        //            new VertexElement(0, 0, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
        //            new VertexElement(0, 0, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
        //            VertexElement.VertexDeclarationEnd
        //        };

        //    vertexDeclA = new VertexDeclaration(device, elements);

        //    ShaderFlags shaderFlags = 0;

        //    string errors;

        //    string shaderText =

        //            " float4x4 matWVP;              " +
        //            " float4 camPos : POSITION;                               " +
        //            " float1 jNow;                               " +
        //            " float1 decay;                               " +
        //            " float1 scale;                               " +
        //            " float1 opacity;                               " +
        //            " float1 sky;                         " +
        //            " float1 showFarSide;                         " +
        //            " struct VS_IN                                 " +
        //            " {                                            " +
        //            "     float4 ObjPos   : POSITION;              " + // Object space position 
        //            "     float1 PointSize   : PSIZE;              " + // Object Point size 
        //            "     float4 Color    : COLOR;                 " + // Vertex color                 
        //            "     float2 Time   : TEXCOORD0;              " + // Object Point size 
        //            " };                                           " +
        //            "                                              " +
        //            " struct VS_OUT                                " +
        //            " {                                            " +
        //            "     float4 ProjPos  : POSITION;              " + // Projected space position 
        //            "     float1 PointSize   : PSIZE;              " + // Object Point size 
        //            "     float4 Color    : COLOR;                 " +
        //            "     float2 Time   : TEXCOORD0;              " + // Object Point size 
        //            " };                                           " +
        //            "                                              " +
        //            " VS_OUT main( VS_IN In )                      " +
        //            " {                                            " +
        //            "     float dotCam = dot((camPos.xyz - In.ObjPos.xyz), In.ObjPos.xyz);   " +
        //            "     float dist = distance(In.ObjPos, camPos.xyz);   " +
        //            "     VS_OUT Out;                              " +
        //            "     float dAlpha = 1;                         " +
        //            "     if ( decay > 0)                           " +
        //            "     {                                        " +
        //            "          dAlpha = 1 - ((jNow - In.Time.y) / decay);          " +
        //            "          if (dAlpha > 1 )           " +
        //            "          {                                     " +
        //            "               dAlpha = 1;                     " +
        //            "          }                                    " +
        //            "                                               " +
        //            "     }                                        " +
        //            "     Out.ProjPos = mul(In.ObjPos,  matWVP );  " + // Transform vertex into
        //            "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))   " +
        //            "     {                                        " +
        //            "        Out.Color.a = 0;                     " +
        //            "     }                                        " +
        //            "     else                                     " +
        //            "     {                                        " +
        //            "        Out.Color.a = In.Color.a * dAlpha * opacity;    " +
        //            "     }                                        " +
        //            "     Out.Color.r = In.Color.r;              " +
        //            "     Out.Color.g = In.Color.g;              " +
        //            "     Out.Color.b = In.Color.b;              " +
        //            "     Out.Time.x = 0;                        " +
        //            "     Out.Time.y = 0;                        " +
        //            "     if ( scale > 0)                           " +
        //            "     {                                        " +
        //            "       Out.PointSize = scale * (In.PointSize )/ dist;" +
        //            "     }                                        " +
        //            "     else                                     " +
        //            "     {                                        " +
        //            "       Out.PointSize = -scale *In.PointSize;" +
        //            "     }                                        " +
        //             " if (Out.PointSize > 256)                     " +
        //             " {                                            " +
        //             "      Out.PointSize = 256;                                        " +
        //             " }                                            " +
        //             "     return Out;                              " + // Transfer color
        //            " }                                            ";

        //    using (GraphicsStream code = ShaderLoader.CompileShader(shaderText, "main", null, null,
        //              "vs_2_0", shaderFlags, out errors, out constantTableA))
        //    {

        //        // We will store these constants in an effect handle here for performance reasons.
        //        // You could simply use the string value (i.e., "worldViewProj") in the SetValue call
        //        // and it would work just as well, but that actually requires an allocation to be made
        //        // and can actually slow your performance down.  It's much more efficient to simply
        //        // cache these handles for use later
        //        worldViewHandleA = constantTableA.GetConstant(null, "matWVP");
        //        cameraHandleA = constantTableA.GetConstant(null, "camPos");
        //        jNowHandleA = constantTableA.GetConstant(null, "jNow");
        //        decayHandleA = constantTableA.GetConstant(null, "decay");
        //        scaleHandleA = constantTableA.GetConstant(null, "scale");
        //        skyHandleA = constantTableA.GetConstant(null, "sky");
        //        opacityHandleA = constantTableA.GetConstant(null, "opacity");
        //        showFarSideHandleA = constantTableA.GetConstant(null, "showFarSide");

        //        // Create the shader
        //        shaderA = new VertexShader(device, code);
        //    }
        //}

  

        public void CleanUpBase()
        {


            if (lineList != null)
            {
                lineList.Clear();
            }
            if (lineList2d != null)
            {
                lineList2d.Clear();
            }

            if (triangleList2d != null)
            {
                triangleList2d.Clear();
            }

            if (pointList != null)
            {
                pointList.Clear();
            }

            if (triangleList != null)
            {
                triangleList.Clear();
            }
        }
    }





    //public struct PointVertex
    //{
    //    public Vector3 Position;
    //    public float PointSize;
    //    public int Color;
    //    public float Tu;
    //    public float Tv;
    //    public static readonly VertexFormats Format = VertexFormats.Position | VertexFormats.PointSize | VertexFormats.Texture1 | VertexFormats.Diffuse;
    //    public PointVertex(Vector3 position, float size, float time, int color)
    //    {
    //        Position = position;
    //        PointSize = size;
    //        Tu = time;
    //        Tv = 0;
    //        Color = color;
    //    }
    //}
    //public struct TimeSeriesLineVertex
    //{
    //    public Vector3 Position;
    //    public Vector3 Normal;
    //    public int Color;
    //    public float Tu;
    //    public float Tv;
    //    public static readonly VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | VertexFormats.Texture1 | VertexFormats.Diffuse;
    //    public TimeSeriesLineVertex(Vector3 position, Vector3 normal, float time, int color)
    //    {
    //        Position = position;
    //        Normal = normal;
    //        Tu = time;
    //        Tv = 0;
    //        Color = color;
    //    }
    //}

    public class KmlCoordinate
    {
        public double Lat;
        public double Lng;
        public double Alt;
        public Dates Date;
    }

    public class KmlLineList
    {
        public bool extrude;
        public bool Astronomical = false;

        public double MeanRadius = 6371000;

        public List<KmlCoordinate> PointList = new List<KmlCoordinate>();

        public void ParseWkt(string geoText, string option, double alt, Dates date)
        {
            //todo fix the WKT parser
            List<string> parts = UiTools.Split(geoText, "(,)");
            foreach (string part in parts)
            {
                string[] coordinates = part.Trim().Split(" ");
                if (coordinates.Length > 1)
                {
                    KmlCoordinate pnt = new KmlCoordinate();
                    pnt.Lng = double.Parse(coordinates[0]);
                    if (Astronomical)
                    {
                        pnt.Lng -= 180;
                    }
                    pnt.Lat = double.Parse(coordinates[1]);
                    if (coordinates.Length > 2 && alt == 0)
                    {
                        pnt.Alt = double.Parse(coordinates[2]);
                    }
                    else
                    {
                        pnt.Alt = alt;
                    }
                    pnt.Date = date;
                    PointList.Add(pnt);
                }
            }
        }


        public KmlCoordinate GetCenterPoint()
        {
            KmlCoordinate point = new KmlCoordinate();
            point.Lat = 0;
            point.Lng = 0;
            point.Alt = 0;


            foreach (KmlCoordinate pnt in PointList)
            {
                point.Lat += pnt.Lat;
                point.Lng += pnt.Lng;
                point.Alt += pnt.Alt;
            }
            point.Lat /= PointList.Count;
            point.Lng /= PointList.Count;
            point.Alt /= PointList.Count;

            return point;
        }
    }
}
