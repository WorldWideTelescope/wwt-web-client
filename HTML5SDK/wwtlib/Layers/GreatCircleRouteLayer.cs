using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class GreatCirlceRouteLayer : Layer
    {

        public override string GetTypeName()
        {
            return "TerraViewer.GreatCirlceRouteLayer";
        }

        TriangleList triangleList = null;
        public override void CleanUp()
        {
            if (triangleList != null)
            {
                triangleList.Clear();
            }
            triangleList = null;
            base.CleanUp();
        }

        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {

            if (triangleList == null)
            {
                InitializeRoute(renderContext);
            }
            triangleList.JNow = percentComplete / 100;
            triangleList.Draw(renderContext, opacity * this.Opacity, CullMode.CounterClockwise );


            return true;
        }

        private void InitializeRoute(RenderContext renderContext)
        {
            triangleList = new TriangleList();
            triangleList.Decay = 1000;
            triangleList.Sky = this.Astronomical;
            triangleList.TimeSeries = true;
            triangleList.DepthBuffered = false;
            triangleList.AutoTime = false;

            int steps = 500;

            Vector3d start = Coordinates.GeoTo3dDouble(latStart, lngStart);
            Vector3d end = Coordinates.GeoTo3dDouble(latEnd, lngEnd);
            Vector3d dir = Vector3d.SubtractVectors(end, start);
            dir.Normalize();

            Vector3d startNormal = start;
            startNormal.Normalize();

            Vector3d left = Vector3d.Cross(startNormal, dir);
            Vector3d right = Vector3d.Cross(dir, startNormal);
            left.Normalize();
            right.Normalize();

            left.Multiply(.001 * width);
            right.Multiply(.001 * width);

            Vector3d lastLeft = new Vector3d();
            Vector3d lastRight = new Vector3d();
            bool firstTime = true;
            for (int i = 0; i <= steps; i++)
            {
                Vector3d v = Vector3d.Lerp(start, end, i / (float)steps);
                v.Normalize();
               // v.Multiply(1.1);
                Vector3d cl = v;
                Vector3d cr = v;

                cl.Add(left);
                cr.Add(right);

                if (!firstTime)
                {
                    triangleList.AddQuad(lastRight, lastLeft, cr, cl, Color, new Dates(i / (float)steps, 2));
                }
                else
                {
                    firstTime = false;
                }

                lastLeft = cl;
                lastRight = cr;


            }

        }

        public override double[] GetParams()
        {
            return new double[] { percentComplete };

        }

        public override string[] GetParamNames()
        {
            return new string[] { "Percentage" };
        }

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length > 0)
            {
                percentComplete = paramList[0];
            }
        }

        private double latStart = 0;

        
        public double LatStart
        {
            get { return latStart; }
            set
            {
                if (latStart != value)
                {
                    latStart = value;
                    version++;
                }
            }
        }
        private double lngStart = 0;

        
        public double LngStart
        {
            get { return lngStart; }
            set
            {
                if (lngStart != value)
                {
                    lngStart = value;
                    version++;
                }
            }
        }
        private double latEnd = 0;

        
        public double LatEnd
        {
            get { return latEnd; }
            set
            {
                if (latEnd != value)
                {
                    latEnd = value;
                    version++;
                }
            }
        }
        private double lngEnd = 0;

        
        public double LngEnd
        {
            get { return lngEnd; }
            set
            {
                if (lngEnd != value)
                {
                    lngEnd = value;
                    version++;
                }
            }
        }

        private double width = 4;

        
        public double Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    version++;
                }
            }
        }

        private double percentComplete = 100;

        
        public double PercentComplete
        {
            get { return percentComplete; }
            set
            {
                if (percentComplete != value)
                {
                    percentComplete = value;
                    version++;
                }
            }
        }

        public override void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("LatStart", LatStart.ToString());
            xmlWriter.WriteAttributeString("LngStart", LngStart.ToString());
            xmlWriter.WriteAttributeString("LatEnd", LatEnd.ToString());
            xmlWriter.WriteAttributeString("LngEnd", LngEnd.ToString());
            xmlWriter.WriteAttributeString("Width", Width.ToString());
            xmlWriter.WriteAttributeString("PercentComplete", PercentComplete.ToString());

        }

        public override void InitializeFromXml(XmlNode node)
        {
            latStart = double.Parse(node.Attributes.GetNamedItem("LatStart").Value);
            lngStart = double.Parse(node.Attributes.GetNamedItem("LngStart").Value);
            latEnd = double.Parse(node.Attributes.GetNamedItem("LatEnd").Value);
            lngEnd = double.Parse(node.Attributes.GetNamedItem("LngEnd").Value);
            width = double.Parse(node.Attributes.GetNamedItem("Width").Value);
            percentComplete = double.Parse(node.Attributes.GetNamedItem("PercentComplete").Value);
        }
    }
}