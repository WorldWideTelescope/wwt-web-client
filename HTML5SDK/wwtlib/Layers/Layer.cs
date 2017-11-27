using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{

    public enum AltUnits { Meters=1, Feet=2, Inches=3, Miles=4, Kilometers=5, AstronomicalUnits=6, LightYears=7, Parsecs=8, MegaParsecs=9, Custom=10 };
    public enum FadeType { FadeIn=1, FadeOut=2, Both=3, None=4 };
    public abstract class Layer 
    {
        public virtual LayerUI GetPrimaryUI()
        {
            return null;
        }

        public Guid ID = Guid.NewGuid();

        public bool LoadedFromTour = false;

        public TourDocument tourDocument = null;

        public string GetFileStreamUrl(string filename)
        {
            if (tourDocument != null)
            {
                return tourDocument.GetFileStream(filename);
            }
            return null;
        }

        protected float opacity = 1.0f;
        
        public virtual float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                if (opacity != value)
                {
                    version++;
                    opacity = value;
                }

            }
        }

        public bool opened = false;
        public virtual bool Opened
        {
            get
            {
                return opened;
            }
            set
            {
                if (opened != value)
                {
                    version++;
                    opened = value;
                }
            }
        }

        private Date startTime = Date.Parse("01/01/1900");
        
        public Date StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    version++;
                    startTime = value;
                }
            }
        }
        private Date endTime = Date.Parse("01/01/2100");

        
        
        public Date EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    version++;
                    endTime = value;
                }
            }
        }

        private double fadeSpan = 0;

        
        public double FadeSpan
        {
            get { return fadeSpan; }
            set
            {
                version++;
                fadeSpan = value;
            }
        }

        private FadeType fadeType = FadeType.None;

        
        public FadeType FadeType
        {
            get { return fadeType; }
            set {
                if (fadeType != value)
                {
                    Version++;
                    fadeType = value;
                }
            }
        }
        protected int version = 0;
    
        
        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public virtual Place FindClosest(Coordinates target, float distance, Place closestPlace, bool astronomical)
        {
            return closestPlace;
        }

        public virtual bool HoverCheckScreenSpace(Vector2d cursor)
        {
            return false;
        }

        public virtual bool ClickCheckScreenSpace(Vector2d cursor)
        {
            return false;
        }


        public virtual bool Draw(RenderContext renderContext, float opacity, bool flat)
        {

            return true;
        }

        public virtual bool PreDraw(RenderContext renderContext, float opacity)
        {
            return true;
        }

        public virtual bool UpadteData(object data, bool purgeOld, bool purgeAll, bool hasHeader)
        {

            return true;
        }

        public virtual bool CanCopyToClipboard()
        {
            return false;
        }

        public virtual void CopyToClipboard()
        {
            return;
        }

        public virtual double[] GetParams()
        {
            return new double[0];
        }

        public virtual void SetParams(double[] paramList)
        {
        }

        public virtual string[] GetParamNames()
        {

            return new string[0];
        }

        public virtual void CleanUp()
        {
        }

        private string name;
        
        public virtual string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    version++;
                    name = value;
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        protected string referenceFrame;

        public string ReferenceFrame
        {
            get { return referenceFrame; }
            set { referenceFrame = value; }
        }

        //public bool SetProp(string name, string value)
        //{
        //    Type thisType = this.GetType();
        //    PropertyInfo pi = thisType.GetProperty(name);
        //    bool safeToSet = false;
        //    Type layerPropType = typeof(LayerProperty);
        //    object[] attributes = pi.GetCustomAttributes(false);
        //    foreach (object var in attributes)
        //    {
        //        if (var.GetType() == layerPropType)
        //        {
        //            safeToSet = true;
        //            break;
        //        }
        //    }

        //    if (safeToSet)
        //    {
        //        //Convert.ChangeType(
        //        if (pi.PropertyType.BaseType == typeof(Enum))
        //        {
        //            pi.SetValue(this, Enum.Parse(pi.PropertyType, value, true), null);
        //        }
        //        else if (pi.PropertyType == typeof(TimeSpan))
        //        {
        //            pi.SetValue(this, TimeSpan.Parse(value), null);
        //        }
        //        else if (pi.PropertyType == typeof(Vector3d))
        //        {
        //            pi.SetValue(this, Vector3d.Parse(value), null);
        //        }   
        //        else
        //        {
        //            // todo fix this 
        //         //   pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType), null);
        //        }
        //    }


        //    return safeToSet;
        //}

        //public bool SetProps(string xml)
        //{
        //    XDocument doc = XDocument.Parse(xml);
            
         

        //    XElement root = doc.Element("LayerApi");

        //    XElement LayerNode = root.Element("Layer");
        //    foreach (XAttribute attrib in LayerNode.Attributes())
        //    {
        //        if (attrib.Name == "Class")
        //        {
        //            continue;
        //        }
        //        if (!SetProp(attrib.Name.ToString(), attrib.Value))
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public string GetProp(string name)
        //{
        //    Type thisType = this.GetType();
        //    PropertyInfo pi = thisType.GetProperty(name);
        //    bool safeToGet = false;
        //    Type layerPropType = typeof(LayerProperty);
        //    object[] attributes = pi.GetCustomAttributes(false);
        //    foreach (object var in attributes)
        //    {
        //        if (var.GetType() == layerPropType)
        //        {
        //            safeToGet = true;
        //            break;
        //        }
        //    }

        //    if (safeToGet)
        //    {
        //        return pi.GetValue(this, null).ToString();
        //    }


        //    return null;
        //}

        public string GetProps()
        {
            //MemoryStream ms = new MemoryStream();
            //using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
            //{
            //    xmlWriter.Formatting = Formatting.Indented;
            //    xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            //    xmlWriter.WriteStartElement("LayerApi");
            //    xmlWriter.WriteElementString("Status", "Success");
            //    xmlWriter.WriteStartElement("Layer");
            //    xmlWriter.WriteAttributeString("Class", this.GetType().ToString().Replace("TerraViewer.",""));


            //    Type thisType = this.GetType();
            //    PropertyInfo[] properties = thisType.GetProperties();

            //    Type layerPropType = typeof(LayerProperty);

            //    foreach (PropertyInfo pi in properties)
            //    {
            //        bool safeToGet = false;

            //        object[] attributes = pi.GetCustomAttributes(false);
            //        foreach (object var in attributes)
            //        {
            //            if (var.GetType() == layerPropType)
            //            {
            //                safeToGet = true;
            //                break;
            //            }
            //        }

            //        if (safeToGet)
            //        {
            //            xmlWriter.WriteAttributeString(pi.Name, pi.GetValue(this, null).ToString());
            //        }

            //    }
            //    xmlWriter.WriteEndElement();
            //    xmlWriter.WriteFullEndElement();
            //    xmlWriter.Close();

            //}
            //byte[] data = ms.GetBuffer();
            //return Encoding.UTF8.GetString(data);
            return "";
        }

        protected Color color = Colors.White;

        public virtual Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    version++;
                    CleanUp();
                    //todo should this invalidate and cleanup
                }
 
            }
        }

        
        public virtual string ColorValue
        {
            get
            {
                return Color.ToString();
            }
            set
            {
                Color = Color.FromName(value);
            }
        }

        private bool enabled = true;

        
        public virtual bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    version++;
                    enabled = value;
                }
                //todo notify of change
            }
        }

        protected bool astronomical = false;

        
        public bool Astronomical
        {
            get { return astronomical; }
            set
            {
                if (astronomical != value)
                {
                    version++;
                    astronomical = value;
                }
            }
        }

        //
        //public virtual string[] Header
        //{
        //    get
        //    {
        //        return null;
        //    }

        //}
        /* Save Load support
         * 
         */
        public virtual string GetTypeName()
        {
            return "TerraViewer.Layer";
        }


        public virtual void SaveToXml(XmlTextWriter xmlWriter)
        {
            //todo write
            xmlWriter.WriteStartElement("Layer");
            xmlWriter.WriteAttributeString("Id", ID.ToString());
            xmlWriter.WriteAttributeString("Type", GetTypeName());
            xmlWriter.WriteAttributeString("Name", Name);
            xmlWriter.WriteAttributeString("ReferenceFrame", referenceFrame);
            xmlWriter.WriteAttributeString("Color", color.Save());
            xmlWriter.WriteAttributeString("Opacity", opacity.ToString());
            xmlWriter.WriteAttributeString("StartTime", Util.XMLDate(StartTime));
            xmlWriter.WriteAttributeString("EndTime", Util.XMLDate(EndTime));
            xmlWriter.WriteAttributeString("FadeSpan", FadeSpan.ToString());
            xmlWriter.WriteAttributeString("FadeType", FadeType.ToString());

            WriteLayerProperties(xmlWriter);

            xmlWriter.WriteEndElement();
        }

        public virtual void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            return;
        }

        public virtual void InitializeFromXml(XmlNode node)
        {

        }

        public static Layer FromXml(XmlNode layerNode, bool someFlag)
        {
            string layerClassName = layerNode.Attributes.GetNamedItem("Type").Value.ToString();

            string overLayType = layerClassName.Replace("TerraViewer.","");
            if (overLayType == null)
            {
                return null;
            }
 
            Layer newLayer = null;

            switch (overLayType)
            {
                case "SpreadSheetLayer":
                    newLayer = new SpreadSheetLayer();
                    break;
                case "GreatCirlceRouteLayer":
                    newLayer = new GreatCirlceRouteLayer();
                    break;
                case "ImageSetLayer":
                    newLayer = new ImageSetLayer();
                    break;
                default:
                    return null;
            }

            //Force inheritance.
            // TODO: Understand why this breaks in SS .8
            //Script.Literal("for(var method in this){\n /*if (({}).toString.call(this[method]).match(/\\s([a-zA-Z]+)/)[1].toLowerCase() == 'function'){\n*/ newLayer[method] = this[method];/*\n}*/\n}");
            
            newLayer.InitFromXml(layerNode);
            
            return newLayer;
        }

        public virtual void InitFromXml(XmlNode node)
        {
            ID = Guid.FromString(node.Attributes.GetNamedItem("Id").Value);
            Name = node.Attributes.GetNamedItem("Name").Value;
            referenceFrame = node.Attributes.GetNamedItem("ReferenceFrame").Value;
            color = Color.Load(node.Attributes.GetNamedItem("Color").Value);
            opacity = Single.Parse(node.Attributes.GetNamedItem("Opacity").Value);

            if (node.Attributes.GetNamedItem("StartTime") != null)
            {
                StartTime = new Date(node.Attributes.GetNamedItem("StartTime").Value);
            }

            if (node.Attributes.GetNamedItem("EndTime") != null)
            {
                EndTime = new Date(node.Attributes.GetNamedItem("EndTime").Value);
            }

            if (node.Attributes.GetNamedItem("FadeSpan") != null)
            {
                FadeSpan = Util.ParseTimeSpan((node.Attributes.GetNamedItem("FadeSpan").Value));
            }

            if (node.Attributes.GetNamedItem("FadeType") != null)
            {
                 switch (node.Attributes.GetNamedItem("FadeType").Value)
                {
                    case "In":
                        FadeType = FadeType.FadeIn;
                        break;
                    case "Out":
                        FadeType = FadeType.FadeOut;
                        break;
                    case "Both":
                        FadeType = FadeType.Both;
                        break;
                    case "None":
                        FadeType = FadeType.None;
                        break;
                    default:
                        break;
                }
            }

            InitializeFromXml(node);
        }



        public virtual void LoadData(TourDocument doc, string filename)
        {
            return;
        }
        
        public virtual void AddFilesToCabinet(FileCabinet fc)
        {
            return;
        }

        public void GetStringFromGzipBlob(System.Html.Data.Files.Blob blob, GzipStringReady dataReady)
        {
            FileReader reader = new FileReader();
            reader.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent e)
            {
                string result = (string)Script.Literal("pako.inflate(e.target.result, { to: 'string' })");
                dataReady(result);
            };
            reader. ReadAsArrayBuffer(blob);
        }


    }

    public delegate void GzipStringReady(string data);

    class LayerCollection : Layer
    {
        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {
            return base.Draw(renderContext, opacity, false);
        }

        
     
    }
    //public class MarkerPlot
    //{
    //    public Texture Texture;
    //    public VertexBuffer VertexBuffer = null;
    //    public int PointCount = 0;

    //    public MarkerPlot()
    //    {
    //    }

    //    public MarkerPlot(Texture texture, VertexBuffer vertexBuffer, int pointCount)
    //    {
    //        Texture = texture;
    //        VertexBuffer = vertexBuffer;
    //        PointCount = pointCount;
    //    }

    //}

    public class DomainValue
    {

        public DomainValue(string text, int markerIndex)
        {
            Text = text;
            MarkerIndex = markerIndex;
        }

        public string Text;
        public int MarkerIndex = 4;
        public object CustomMarker = null;

    }

   
}

