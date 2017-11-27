using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;
using System.Html.Data.Files;

namespace wwtlib
{

    public class VoTableLayer : Layer
    {
        protected bool isLongIndex = false;
        protected int shapeVertexCount;


        protected bool lines = false;
        protected int latColumn = -1;
        protected float fixedSize = 1;
        protected float decay = 0;
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

            switch (node.Attributes.GetNamedItem("AltType").Value)
            {
                case "Depth":
                    AltType = AltTypes.Depth;
                    break;
                case "Altitude":
                    AltType = AltTypes.Altitude;
                    break;
                case "Distance":
                    AltType = AltTypes.Distance;
                    break;
                case "SeaLevel":
                    AltType = AltTypes.SeaLevel;
                    break;
                case "Terrain":
                    AltType = AltTypes.Terrain;
                    break;
                default:
                    break;
            }



            MarkerMix = MarkerMixes.Same_For_All;

            switch (node.Attributes.GetNamedItem("ColorMap").Value)
            {
                case "Same_For_All":
                    ColorMap = ColorMaps.Same_For_All;
                    break;
                case "Group_by_Values":
                    ColorMap = ColorMaps.Group_by_Values;
                    break;
                case "Per_Column_Literal":
                    ColorMap = ColorMaps.Per_Column_Literal;
                    break;
                default:
                    break;
            }


            MarkerColumn = int.Parse(node.Attributes.GetNamedItem("MarkerColumn").Value);
            ColorMapColumn = int.Parse(node.Attributes.GetNamedItem("ColorMapColumn").Value);

            switch (node.Attributes.GetNamedItem("PlotType").Value)
            {
                case "Gaussian":
                    PlotType = PlotTypes.Gaussian;
                    break;
                case "Point":
                    PlotType = PlotTypes.Point;
                    break;
                case "Circle":
                    PlotType = PlotTypes.Circle;
                    break;
                case "PushPin":
                    PlotType = PlotTypes.PushPin;
                    break;
                default:
                    break;
            }

            MarkerIndex = int.Parse(node.Attributes.GetNamedItem("MarkerIndex").Value);

            switch (node.Attributes.GetNamedItem("MarkerScale").Value)
            {
                case "Screen":
                    MarkerScale = MarkerScales.Screen;
                    break;
                case "World":
                    MarkerScale = MarkerScales.World;
                    break;
                default:
                    break;
            }

            switch (node.Attributes.GetNamedItem("AltUnit").Value)
            {
                case "Meters":
                    AltUnit = AltUnits.Meters;
                    break;
                case "Feet":
                    AltUnit = AltUnits.Feet;
                    break;
                case "Inches":
                    AltUnit = AltUnits.Inches;
                    break;
                case "Miles":
                    AltUnit = AltUnits.Miles;
                    break;
                case "Kilometers":
                    AltUnit = AltUnits.Kilometers;
                    break;
                case "AstronomicalUnits":
                    AltUnit = AltUnits.AstronomicalUnits;
                    break;
                case "LightYears":
                    AltUnit = AltUnits.LightYears;
                    break;
                case "Parsecs":
                    AltUnit = AltUnits.Parsecs;
                    break;
                case "MegaParsecs":
                    AltUnit = AltUnits.MegaParsecs;
                    break;
                case "Custom":
                    AltUnit = AltUnits.Custom;
                    break;
                default:
                    break;
            }

            AltColumn = int.Parse(node.Attributes.GetNamedItem("AltColumn").Value);
            StartDateColumn = int.Parse(node.Attributes.GetNamedItem("StartDateColumn").Value);
            EndDateColumn = int.Parse(node.Attributes.GetNamedItem("EndDateColumn").Value);
            SizeColumn = int.Parse(node.Attributes.GetNamedItem("SizeColumn").Value);
            HyperlinkFormat = node.Attributes.GetNamedItem("HyperlinkFormat").Value;
            HyperlinkColumn = int.Parse(node.Attributes.GetNamedItem("HyperlinkColumn").Value);
            ScaleFactor = Single.Parse(node.Attributes.GetNamedItem("ScaleFactor").Value);

            switch (node.Attributes.GetNamedItem("PointScaleType").Value)
            {
                case "Linear":
                    PointScaleType = PointScaleTypes.Linear;
                    break;
                case "Power":
                    PointScaleType = PointScaleTypes.Power;
                    break;
                case "Log":
                    PointScaleType = PointScaleTypes.Log;
                    break;
                case "Constant":
                    PointScaleType = PointScaleTypes.Constant;
                    break;
                case "StellarMagnitude":
                    PointScaleType = PointScaleTypes.StellarMagnitude;
                    break;
                default:
                    break;
            }


            if (node.Attributes.GetNamedItem("ShowFarSide") != null)
            {
                ShowFarSide = Boolean.Parse(node.Attributes.GetNamedItem("ShowFarSide").Value);
            }

            if (node.Attributes.GetNamedItem("RaUnits") != null)
            {
                // RaUnits = (RAUnits)Enum.Parse(typeof(RAUnits), node.Attributes["RaUnits"].Value);

                switch (node.Attributes.GetNamedItem("RaUnits").Value)
                {
                    case "Hours":
                        RaUnits = RAUnits.Hours;
                        break;
                    case "Degrees":
                        RaUnits = RAUnits.Degrees;
                        break;
                }
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

                switch (node.Attributes.GetNamedItem("CartesianScale").Value)
                {
                    case "Meters":
                        CartesianScale = AltUnits.Meters;
                        break;
                    case "Feet":
                        CartesianScale = AltUnits.Feet;
                        break;
                    case "Inches":
                        CartesianScale = AltUnits.Inches;
                        break;
                    case "Miles":
                        CartesianScale = AltUnits.Miles;
                        break;
                    case "Kilometers":
                        CartesianScale = AltUnits.Kilometers;
                        break;
                    case "AstronomicalUnits":
                        CartesianScale = AltUnits.AstronomicalUnits;
                        break;
                    case "LightYears":
                        CartesianScale = AltUnits.LightYears;
                        break;
                    case "Parsecs":
                        CartesianScale = AltUnits.Parsecs;
                        break;
                    case "MegaParsecs":
                        CartesianScale = AltUnits.MegaParsecs;
                        break;
                    case "Custom":
                        CartesianScale = AltUnits.Custom;
                        break;
                    default:
                        break;
                }


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
                PrepVertexBuffer(renderContext, opacity);
                dirty = false;
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

        public override void InitFromXml(XmlNode node)
        {
            base.InitFromXml(node);
        }

        public override void CleanUp()
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

        public virtual bool DynamicUpdate()
        {
            return false;
        }

        //start of VoTable Cutomizations
      //  public VOTableViewer Viewer = null;
        public VoTableLayer()
        {
            this.table = null;
            this.filename = "";

            PlotType = PlotTypes.Circle;
        }
        public static VoTableLayer Create(VoTable table)
        {
            VoTableLayer layer = new VoTableLayer();

            layer.table = table;
            layer.filename = table.LoadFilename;
            layer.LngColumn = table.GetRAColumn().Index;
            layer.LatColumn = table.GetDecColumn().Index;
            layer.PlotType = PlotTypes.Circle;

            return layer;
        }
        public override void AddFilesToCabinet(FileCabinet fc)
        {
            string fName = filename;

            bool copy = true;

            string fileName = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID, this.ID.ToString());
            string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);
            string path2 = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);

            //if (copy)
            //{
            //    if (!Directory.Exists(path2))
            //    {
            //        Directory.CreateDirectory(path2);
            //    }
            //    if (File.Exists(fName) && !File.Exists(fileName))
            //    {
            //        File.Copy(fName, fileName);
            //    }


            //}

            //if (File.Exists(fileName))
            //{
            //    fc.AddFile(fileName);
            //}
        }
        string filename = "";
     

        public override void LoadData(TourDocument tourDoc, string filename)
        {
            System.Html.Data.Files.Blob blob = tourDoc.GetFileBlob(filename);

            FileReader doc = new FileReader();
            doc.OnLoadEnd = delegate (FileProgressEvent ee)
            {
                string data = doc.Result as string;

                table = VoTable.LoadFromString(data);
                LngColumn = table.GetRAColumn().Index;
                LatColumn = table.GetDecColumn().Index;
            };
            doc.ReadAsText(blob);    
        }

        public override bool CanCopyToClipboard()
        {
            return true;
        }

        public override void CopyToClipboard()
        {
         //   System.Windows.Clipboard.SetText(table.ToString());
        }

        public override Place FindClosest(Coordinates target, float distance, Place defaultPlace, bool astronomical)
        {
            Vector3d searchPoint = Coordinates.GeoTo3dDouble(target.Lat, target.Lng);

            //searchPoint = -searchPoint;
            Vector3d dist;
            if (defaultPlace != null)
            {
                Vector3d testPoint = Coordinates.RADecTo3dAu(defaultPlace.RA, -defaultPlace.Dec, -1.0);
                dist = Vector3d.SubtractVectors(searchPoint, testPoint);
                distance = (float)dist.Length();
            }

            int closestItem = -1;
            int index = 0;
            foreach (Vector3d point in positions)
            {
                dist = Vector3d.SubtractVectors(searchPoint, point);
                if (dist.Length() < distance)
                {
                    distance = (float)dist.Length();
                    closestItem = index;
                }
                index++;
            }


            if (closestItem == -1)
            {
                return defaultPlace;
            }

            Coordinates pnt = Coordinates.CartesianToSpherical2(positions[closestItem]);

            string name = table.Rows[closestItem].ColumnData[this.nameColumn].ToString();
            if (nameColumn == startDateColumn || nameColumn == endDateColumn)
            {
                //todo fix data output
                name = SpreadSheetLayer.ParseDate(name).ToString();
               //   name = SpreadSheetLayer.ParseDate(name).ToString("u");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = string.Format("RA={0}, Dec={1}", Coordinates.FormatHMS(pnt.RA), Coordinates.FormatDMS(pnt.Dec));
            }
            Place place = Place.Create(name, pnt.Lat, pnt.RA, Classification.Unidentified, "", ImageSetType.Sky, -1);

            Dictionary<String, String> rowData = new Dictionary<string, string>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string colValue = table.Rows[closestItem][i].ToString();
                if (i == startDateColumn || i == endDateColumn)
                {
                    //todo fix data output
                   // colValue = SpreadSheetLayer.ParseDate(colValue).ToString("u");
                    colValue = SpreadSheetLayer.ParseDate(colValue).ToString();
                }

                if (!rowData.ContainsKey(table.Column[i].Name) && !string.IsNullOrEmpty(table.Column[i].Name))
                {
                    rowData[table.Column[i].Name] = colValue;
                }
                else
                {
                    rowData["Column" + i.ToString()] = colValue;
                }
            }
            place.Tag = rowData;
            //todo fix event handling
            //if (Viewer != null)
            //{
            //    Viewer.LabelClicked(closestItem);
            //}
            return place;
        }

        protected bool PrepVertexBuffer(RenderContext renderContext, float opacity)
        {
            VoColumn col = table.GetColumnByUcd("meta.id");
            if (col == null)
            {
                col = table.Column[0];
            }

         //   if (shapeFileVertex == null)
            {
                bool siapSet = IsSiapResultSet();

                if (pointList == null)
                {
                    pointList = new PointList(renderContext);
                }

                if (lineList2d == null)
                {
                    lineList2d = new LineList();
                }
                lineList2d.Clear();

                VoColumn stcsCol = table.GetColumnByUcd("phys.area;obs.field");

                if (stcsCol == null && table.Columns.ContainsKey("regionSTCS"))
                {
                    stcsCol = table.Columns["regionSTCS"];
                }

                if (PlotType == PlotTypes.Gaussian)
                {
                    MarkerScale = MarkerScales.World;
                }
                else
                {
                    MarkerScale = MarkerScales.Screen;
                }

                List<TimeSeriesPointVertex> vertList = new List<TimeSeriesPointVertex>();
                List<UInt32> indexList = new List<UInt32>();
                TimeSeriesPointVertex lastItem = new TimeSeriesPointVertex();
                positions.Clear();
                UInt32 currentIndex = 0;
                Color color = Color.FromArgb((int)(opacity * (float)Color.A), Color.R, Color.G, Color.B);

                pointScaleType = PointScaleTypes.StellarMagnitude;

                foreach (VoRow row in table.Rows)
                {
                    try
                    {
                        if (lngColumn > -1 && latColumn > -1)
                        {
                            double Xcoord = Coordinates.ParseRA(row[this.LngColumn].ToString(), true) * 15;
                            double Ycoord = Coordinates.ParseDec(row[this.LatColumn].ToString());
                            lastItem.Position = Coordinates.GeoTo3dDouble(Ycoord, Xcoord);
                            positions.Add(lastItem.Position);
                            lastItem.Color = color;
                            if (sizeColumn > -1)
                            {

                                try
                                {
                                    if (MarkerScale == MarkerScales.Screen)
                                    {
                                        lastItem.PointSize = 20f;
                                    }
                                    else
                                    {
                                        switch (pointScaleType)
                                        {
                                            case PointScaleTypes.Linear:
                                                lastItem.PointSize = float.Parse(row[sizeColumn].ToString());
                                                break;
                                            case PointScaleTypes.Log:
                                                lastItem.PointSize = (float)Math.Log(float.Parse(row[sizeColumn].ToString()));
                                                break;
                                            case PointScaleTypes.Power:
                                                lastItem.PointSize = (float)Math.Pow(2, float.Parse(row[sizeColumn].ToString()));
                                                break;
                                            case PointScaleTypes.StellarMagnitude:
                                                {
                                                    double size = Single.Parse(row[sizeColumn].ToString());
                                                    lastItem.PointSize = (float)(40 / Math.Pow(1.6, size)) * 10;
                                                }
                                                break;
                                            case PointScaleTypes.Constant:
                                                lastItem.PointSize = 1;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                catch
                                {
                                    lastItem.PointSize = .01f;
                                }
                            }
                            else
                            {
                                if (MarkerScale == MarkerScales.Screen)
                                {
                                    lastItem.PointSize = 20;
                                }
                                else
                                {
                                    lastItem.PointSize = (float)Math.Pow(2, 1) * 100;
                                }
                            }


                            if (startDateColumn > -1)
                            {
                                Date dateTime = Date.Parse(row[startDateColumn].ToString());
                                lastItem.Tu = (float)SpaceTimeController.UtcToJulian(dateTime);
                                lastItem.Tv = 0;
                            }

                            
                            vertList.Add(lastItem);
                            pointList.AddPoint(lastItem.Position, lastItem.color, new Dates(lastItem.Tu, lastItem.Tv), lastItem.PointSize );
                            currentIndex++;
                        }


                        if (siapSet && stcsCol!= null)
                        {
                            AddSiapStcRow(stcsCol.Name, row, row == table.SelectedRow);
                        }
                    }

                    catch
                    {
                    }
                    lines = false;
                }

                if (siapSet && stcsCol != null)
                {
                    AddSiapStcRow(stcsCol.Name, table.SelectedRow, true);
                }
    
            }
            return true;
        }

        private void AddSiapStcRow(string stcsColName, VoRow row, bool selected)
        {
            string stcs = row.GetColumnData(stcsColName).ToString().Replace("  ", " ");
            Color col = Color.FromArgb(120, 255, 255, 255);

            if (selected)
            {
                col = Colors.Yellow;
            }

            if (stcs.StartsWith("Polygon J2000"))
            {
                string[] parts = stcs.Split(' ');

                int len = parts.Length;
                int index = 0;
                while (index < len)
                {
                    if (parts[index] == "Polygon")
                    {
                        index += 2;
                        Vector3d lastPoint = new Vector3d();
                        Vector3d firstPoint = new Vector3d();
                        bool start = true;
                        for (int i = index; i < len; i += 2)
                        {
                            if (parts[i] == "Polygon")
                            {
                                start = true;
                                break;
                            }
                            else
                            {
                                double Xcoord = Coordinates.ParseRA(parts[i], true) * 15 + 180;
                                double Ycoord = Coordinates.ParseDec(parts[i + 1]);
         

                                Vector3d pnt = Coordinates.GeoTo3dDouble(Ycoord, Xcoord);

                                if (!start)
                                {
                                    lineList2d.AddLine(lastPoint, pnt, col, new Dates(0,0));
                                }
                                else
                                {
                                    firstPoint = pnt;
                                    start = false;
                                }
                                lastPoint = pnt;
                            }
                            index += 2;
                        }
                        if (len > 4)
                        {
                            lineList2d.AddLine(firstPoint, lastPoint, col, new Dates(0,0));
                        }

                        

                    }
                }
            }
        }

        public bool IsSiapResultSet()
        {
            return table.GetColumnByUcd("vox:image.title") != null && table.GetColumnByUcd("VOX:Image.AccessReference") != null;
 
        }

        public string[] Header
        {
            get
            {
                string[] header = new string[table.Columns.Count];
                int index = 0;
                foreach (VoColumn col in table.Column)
                {
                    header[index++] = col.Name;
                }

                return header;
            }

        }


        private VoTable table;

        public VoTable Table
        {
            get { return table; }
            set { table = value; }
        }

    }
 
}
