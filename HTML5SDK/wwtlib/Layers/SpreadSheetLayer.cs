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
    
    
    class SpreadSheetLayer : TimeSeriesLayer
    {
        public SpreadSheetLayer()
        {
        }


        public override List<string> Header
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

        public override bool DynamicUpdate()
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

        public override void LoadData(string path)
        {
            table = new Table();

            XmlHttpRequest xhr = new XmlHttpRequest();
            xhr.Open(HttpVerb.Get, path);
            xhr.OnReadyStateChange = delegate()
            {
                if (xhr.ReadyState == ReadyState.Loaded)
                {
                    table.LoadFromString(xhr.ResponseText, false, true, true);
                    ComputeDateDomainRange(-1, -1);

                    if (DynamicData && AutoUpdate)
                    {
                        DynamicUpdate();
                    }
                    dataDirty = true;
                    dirty = true;
                }
            };
            xhr.Send();

            

    
           
        }


        string fileName;
      

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


        public override void ComputeDateDomainRange(int columnStart, int columnEnd)
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

        public override List<string> GetDomainValues(int column)
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
        
        protected override bool PrepVertexBuffer(RenderContext renderContext, float opacity)
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
                                            pointSize = (float)double.Parse(row[altColumn]);
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

        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {
           // table.Lock();
            bool bVal = base.Draw(renderContext, opacity, flat);
          //  table.Unlock();
            return bVal;
        }

        public override void CleanUp()
        {
            table.Lock();
            base.CleanUp();
            table.Unlock();

            dirty = true;
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
