using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Data.Files;

namespace wwtlib
{
    enum Draging { None = 0, X = 1, Y = 2, Z = 3, HP = 4, PR = 5, RH = 6, HP1 = 7, PR1 = 8, RH1 = 9, Scale = 10 };
    public class Object3dLayer : Layer, IUiController
    {

        Object3dLayerUI primaryUI = null;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new Object3dLayerUI(this);
            }

            return primaryUI;
        }


        public Object3d object3d;

        double heading = 0;
        bool flipV = true;

        //   //[LayerProperty]
        public bool FlipV
        {
            get { return flipV; }
            set
            {
                if (flipV != value)
                {
                    flipV = value;
                    if (object3d != null)
                    {
                        object3d.FlipV = flipV;
                        object3d.Reload();
                    }
                    version++;
                }
            }
        }

        bool flipHandedness = false;

        //[LayerProperty]
        public bool FlipHandedness
        {
            get { return flipHandedness; }
            set
            {
                if (flipHandedness != value)
                {
                    flipHandedness = value;
                    if (object3d != null)
                    {
                        object3d.FlipHandedness = flipHandedness;
                        object3d.Reload();
                    }
                    version++;
                }
            }
        }




        bool smooth = true;

        //[LayerProperty]
        public bool Smooth
        {
            get { return smooth; }
            set
            {
                if (smooth != value)
                {
                    smooth = value;
                    if (object3d != null)
                    {
                        object3d.Smooth = smooth;
                        object3d.Reload();
                    }
                    version++;
                }
            }
        }

        bool twoSidedGeometry = false;

        //[LayerProperty]
        public bool TwoSidedGeometry
        {
            get { return twoSidedGeometry; }
            set
            {
                if (twoSidedGeometry != value)
                {
                    twoSidedGeometry = value;
                    version++;
                }
            }
        }

        //[LayerProperty]
        public double Heading
        {
            get { return heading; }
            set
            {
                if (heading != value)
                {
                    version++;
                    heading = value;
                }
            }
        }
        double pitch = 0;

        //[LayerProperty]
        public double Pitch
        {
            get { return pitch; }
            set
            {
                if (pitch != value)
                {
                    version++;
                    pitch = value;
                }
            }
        }
        double roll = 0;

        //[LayerProperty]
        public double Roll
        {
            get { return roll; }
            set
            {
                if (roll != value)
                {
                    version++;
                    roll = value;
                }
            }
        }
        Vector3d scale = Vector3d.Create(1, 1, 1);

        //[LayerProperty]
        public Vector3d Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    version++;
                    scale = value;
                }
            }
        }
        Vector3d translate = Vector3d.Create(0, 0, 0);

        //[LayerProperty]
        public Vector3d Translate
        {
            get { return translate; }
            set
            {
                if (translate != value)
                {
                    version++;
                    translate = value;
                }
            }
        }


        public Object3dLayer()
        {
        }

        int lightID = 0;

        //[LayerProperty]
        public int LightID
        {
            get { return lightID; }
            set { lightID = value; }
        }



        bool dirty = false;
        public override void CleanUp()
        {
            //if (object3d != null)
            //{
            //    object3d.Dispose();
            //}
            //object3d = null;
            dirty = true;
        }

        public override void ColorChanged()
        {
            if (object3d != null)
            {
                object3d.Color = Color;
                //object3d.Reload();
            }
        }

        public bool ObjType = false;

        public override void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("FlipV", FlipV.ToString());
            xmlWriter.WriteAttributeString("FlipHandedness", FlipHandedness.ToString());
            xmlWriter.WriteAttributeString("Smooth", Smooth.ToString());
            xmlWriter.WriteAttributeString("TwoSidedGeometry", TwoSidedGeometry.ToString());
            xmlWriter.WriteAttributeString("Heading", Heading.ToString());
            xmlWriter.WriteAttributeString("Pitch", Pitch.ToString());
            xmlWriter.WriteAttributeString("Roll", Roll.ToString());
            xmlWriter.WriteAttributeString("Scale", Scale.ToString());
            xmlWriter.WriteAttributeString("Translate", Translate.ToString());
            xmlWriter.WriteAttributeString("LightID", LightID.ToString());
            xmlWriter.WriteAttributeString("Obj", ObjType.ToString());

        }

        public override double[] GetParams()
        {
            double[] paramList = new double[14];
            paramList[0] = heading;
            paramList[1] = pitch;
            paramList[2] = roll;
            paramList[3] = scale.X;
            paramList[4] = scale.Y;
            paramList[5] = scale.Z;
            paramList[6] = translate.X;
            paramList[7] = translate.Y;
            paramList[8] = translate.Z;
            paramList[9] = Color.R / 255;
            paramList[10] = Color.G / 255;
            paramList[11] = Color.B / 255;
            paramList[12] = Color.A / 255;
            paramList[13] = Opacity;

            return paramList;
        }

        public override string[] GetParamNames()
        {
            return new string[] { "Heading", "Pitch", "Roll", "Scale.X", "Scale.Y", "Scale.Z", "Translate.X", "Translate.Y", "Translate.Z", "Colors.Red", "Colors.Green", "Colors.Blue", "Colors.Alpha", "Opacity" };
        }

        //public override BaseTweenType[] GetParamTypes()
        //{
        //    return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Power, BaseTweenType.Power, BaseTweenType.Power, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear };
        //}

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length == 14)
            {
                heading = paramList[0];
                pitch = paramList[1];
                roll = paramList[2];
                scale.X = paramList[3];
                scale.Y = paramList[4];
                scale.Z = paramList[5];
                translate.X = paramList[6];
                translate.Y = paramList[7];
                translate.Z = paramList[8];

                Opacity = (float)paramList[13];
                Color color = Color.FromArgb((int)(paramList[12] * 255), (int)(paramList[9] * 255), (int)(paramList[10] * 255), (int)(paramList[11] * 255));
                Color = color;

            }

        }

        public event EventHandler PropertiesChanged;

        public void FireChanged()
        {
            if (PropertiesChanged != null)
            {
                PropertiesChanged.Invoke(this, new EventArgs());
            }
        }

        public override object GetEditUI()
        {
            return this as IUiController;
        }

        public override void InitializeFromXml(XmlNode node)
        {
            FlipV = Boolean.Parse(node.Attributes.GetNamedItem("FlipV").Value);

            if (node.Attributes.GetNamedItem("FlipHandedness") != null)
            {
                FlipHandedness = Boolean.Parse(node.Attributes.GetNamedItem("FlipHandedness").Value);
            }
            else
            {
                FlipHandedness = false;
            }

            if (node.Attributes.GetNamedItem("Smooth") != null)
            {
                Smooth = Boolean.Parse(node.Attributes.GetNamedItem("Smooth").Value);
            }
            else
            {
                Smooth = true;
            }

            if (node.Attributes.GetNamedItem("TwoSidedGeometry") != null)
            {
                TwoSidedGeometry = Boolean.Parse(node.Attributes.GetNamedItem("TwoSidedGeometry").Value);
            }
            else
            {
                TwoSidedGeometry = false;
            }

            if (node.Attributes.GetNamedItem("Obj") != null)
            {
                ObjType = Boolean.Parse(node.Attributes.GetNamedItem("Obj").Value);
            }
            else
            {
                ObjType = false;
            }

            Heading = double.Parse(node.Attributes.GetNamedItem("Heading").Value);
            Pitch = double.Parse(node.Attributes.GetNamedItem("Pitch").Value);
            Roll = double.Parse(node.Attributes.GetNamedItem("Roll").Value);
            Scale = Vector3d.Parse(node.Attributes.GetNamedItem("Scale").Value);
            Translate = Vector3d.Parse(node.Attributes.GetNamedItem("Translate").Value);

            if (node.Attributes.GetNamedItem("LightID") != null)
            {
                LightID = int.Parse(node.Attributes.GetNamedItem("LightID").Value);
            }
        }

        static TriangleList TranslateUI = null;
        static LineList TranslateUILines = null;

        static TriangleList ScaleUI = null;

        //public static SimpleLineList11 sketch = null;
        static void InitTranslateUI()
        {
            TranslateUILines = new LineList();
            TranslateUILines.TimeSeries = false;
            TranslateUILines.DepthBuffered = false;
            TranslateUILines.ShowFarSide = true;

            TranslateUI = new TriangleList();
            TranslateUI.DepthBuffered = false;
            TranslateUI.TimeSeries = false;
            TranslateUI.WriteZbuffer = false;

            double twoPi = Math.PI * 2;
            double step = twoPi / 45;
            double rad = .05;

            // X

            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(1 - rad * 4, 0, 0);
                Vector3d pnt2 = Vector3d.Create(1 - rad * 4, Math.Cos(a) * rad, Math.Sin(a) * rad);
                Vector3d pnt3 = Vector3d.Create(1 - rad * 4, Math.Cos(a + step) * rad, Math.Sin(a + step) * rad);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Colors.Red, Dates.Empty());
            }
            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(1, 0, 0);
                Vector3d pnt3 = Vector3d.Create(1 - rad * 4, Math.Cos(a) * rad, Math.Sin(a) * rad);
                Vector3d pnt2 = Vector3d.Create(1 - rad * 4, Math.Cos(a + step) * rad, Math.Sin(a + step) * rad);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Color.FromArgb(255, 255, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
            }

            TranslateUILines.AddLine(Vector3d.Create(0, 0, 0), Vector3d.Create(1, 0, 0), Colors.Red, Dates.Empty());

            // Y
            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(0, 1 - rad * 4, 0);
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a) * rad, 1 - rad * 4, Math.Sin(a) * rad);
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a + step) * rad, 1 - rad * 4, Math.Sin(a + step) * rad);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Colors.Green, Dates.Empty());
            }

            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(0, 1, 0);
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a) * rad, 1 - rad * 4, Math.Sin(a) * rad);
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step) * rad, 1 - rad * 4, Math.Sin(a + step) * rad);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Color.FromArgb(255, Math.Max(0, (int)(Math.Sin(a) * 128)), 255, Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
            }

            TranslateUILines.AddLine(Vector3d.Create(0, 0, 0), Vector3d.Create(0, 1, 0), Colors.Green, Dates.Empty());

            // Z
            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(0, 0, 1 - rad * 4);
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a) * rad, Math.Sin(a) * rad, 1 - rad * 4);
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, 1 - rad * 4);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Colors.Blue, Dates.Empty());
            }

            for (double a = 0; a < twoPi; a += step)
            {
                Vector3d pnt1 = Vector3d.Create(0, 0, 1);
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a) * rad, Math.Sin(a) * rad, 1 - rad * 4);
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, 1 - rad * 4);
                TranslateUI.AddTriangle(pnt1, pnt2, pnt3, Color.FromArgb(255, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128)), 255), Dates.Empty());
            }

            TranslateUILines.AddLine(Vector3d.Create(0, 0, 0), Vector3d.Create(0, 0, 1), Colors.Blue, Dates.Empty());
            InitRotateUI();
            InitScaleUI();
        }

        static void InitScaleUI()
        {
            ScaleUI = new TriangleList();
            ScaleUI.DepthBuffered = false;
            ScaleUI.TimeSeries = false;
            ScaleUI.WriteZbuffer = false;

            double twoPi = Math.PI * 2;
            double step = twoPi / 45;
            double rad = .05;

            // X

            MakeCube(ScaleUI, Vector3d.Create(1 - rad * 2, 0, 0), rad * 2, Colors.Red);
            MakeCube(ScaleUI, Vector3d.Create(0, 1 - rad * 2, 0), rad * 2, Colors.Green);
            MakeCube(ScaleUI, Vector3d.Create(0, 0, 1 - rad * 2), rad * 2, Colors.Blue);

        }

        static void MakeCube(TriangleList tl, Vector3d center, double size, Color color)
        {

            Color dark = Color.FromArgb(255, (int)(color.R * .6), (color.G), (int)(color.B * .6));
            Color med = Color.FromArgb(255, (int)(color.R * .8), (int)(color.G * .8), (int)(color.B * .8));


            tl.AddQuad(
                Vector3d.Create(center.X + size, center.Y + size, center.Z + size),
                Vector3d.Create(center.X + size, center.Y + size, center.Z - size),
                Vector3d.Create(center.X - size, center.Y + size, center.Z + size),
                Vector3d.Create(center.X - size, center.Y + size, center.Z - size),
                color, Dates.Empty());

            tl.AddQuad(
                Vector3d.Create(center.X + size, center.Y - size, center.Z + size),
                Vector3d.Create(center.X - size, center.Y - size, center.Z + size),
                Vector3d.Create(center.X + size, center.Y - size, center.Z - size),
                Vector3d.Create(center.X - size, center.Y - size, center.Z - size),
                color, Dates.Empty());


            tl.AddQuad(
               Vector3d.Create(center.X - size, center.Y + size, center.Z + size),
               Vector3d.Create(center.X - size, center.Y + size, center.Z - size),
               Vector3d.Create(center.X - size, center.Y - size, center.Z + size),
               Vector3d.Create(center.X - size, center.Y - size, center.Z - size),
               dark, Dates.Empty());

            tl.AddQuad(
                Vector3d.Create(center.X + size, center.Y + size, center.Z + size),
                Vector3d.Create(center.X + size, center.Y - size, center.Z + size),
                Vector3d.Create(center.X + size, center.Y + size, center.Z - size),
                Vector3d.Create(center.X + size, center.Y - size, center.Z - size),
                dark, Dates.Empty());

            tl.AddQuad(
              Vector3d.Create(center.X + size, center.Y + size, center.Z + size),
              Vector3d.Create(center.X - size, center.Y + size, center.Z + size),
              Vector3d.Create(center.X + size, center.Y - size, center.Z + size),
              Vector3d.Create(center.X - size, center.Y - size, center.Z + size),
              med, Dates.Empty());

            tl.AddQuad(
              Vector3d.Create(center.X + size, center.Y + size, center.Z - size),
              Vector3d.Create(center.X + size, center.Y - size, center.Z - size),
              Vector3d.Create(center.X - size, center.Y + size, center.Z - size),
              Vector3d.Create(center.X - size, center.Y - size, center.Z - size),
              med, Dates.Empty());

        }

        static TriangleList RotateUi = null;

        static void InitRotateUI()
        {
            RotateUi = new TriangleList();
            RotateUi.DepthBuffered = false;
            RotateUi.TimeSeries = false;
            RotateUi.WriteZbuffer = false;

            double twoPi = Math.PI * 2;
            double step = twoPi / 40;
            double rad = .05;
            int index = 0;

            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(rad * (start ? 0 : (end ? 1.5 : 1)), Math.Cos(a), Math.Sin(a));
                Vector3d pnt2 = Vector3d.Create(-rad * (start ? 0 : (end ? 1.5 : 1)), Math.Cos(a), Math.Sin(a));
                Vector3d pnt3 = Vector3d.Create(rad * (start ? 1.5 : (end ? 0 : 1)), Math.Cos(a + step), Math.Sin(a + step));
                Vector3d pnt4 = Vector3d.Create(-rad * (start ? 1.5 : (end ? 0 : 1)), Math.Cos(a + step), Math.Sin(a + step));
                RotateUi.AddQuad(pnt1, pnt3, pnt2, pnt4, Color.FromArgbColor(192, Colors.Red), Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,192, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }

            index = 0;
            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(Math.Cos(a), Math.Sin(a), rad * (start ? 0 : (end ? 1.5 : 1)));
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a), Math.Sin(a), -rad * (start ? 0 : (end ? 1.5 : 1)));
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step), Math.Sin(a + step), rad * (start ? 1.5 : (end ? 0 : 1)));
                Vector3d pnt4 = Vector3d.Create(Math.Cos(a + step), Math.Sin(a + step), -rad * (start ? 1.5 : (end ? 0 : 1)));
                RotateUi.AddQuad(pnt1, pnt3, pnt2, pnt4, Color.FromArgbColor(192, Colors.Blue), Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,192, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }

            index = 0;
            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(Math.Cos(a), rad * (start ? 0 : (end ? 1.5 : 1)), Math.Sin(a));
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a), -rad * (start ? 0 : (end ? 1.5 : 1)), Math.Sin(a));
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step), rad * (start ? 1.5 : (end ? 0 : 1)), Math.Sin(a + step));
                Vector3d pnt4 = Vector3d.Create(Math.Cos(a + step), -rad * (start ? 1.5 : (end ? 0 : 1)), Math.Sin(a + step));
                RotateUi.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgbColor(192, Colors.Green), Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,192, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }

            // X
            index = 0;
            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(-rad * (start ? 0 : (end ? 1.5 : 1)), Math.Cos(a), Math.Sin(a));
                Vector3d pnt2 = Vector3d.Create(rad * (start ? 0 : (end ? 1.5 : 1)), Math.Cos(a), Math.Sin(a));
                Vector3d pnt3 = Vector3d.Create(-rad * (start ? 1.5 : (end ? 0 : 1)), Math.Cos(a + step), Math.Sin(a + step));
                Vector3d pnt4 = Vector3d.Create(rad * (start ? 1.5 : (end ? 0 : 1)), Math.Cos(a + step), Math.Sin(a + step));
                RotateUi.AddQuad(pnt1, pnt3, pnt2, pnt4, Colors.Red, Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,255, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }



            //Y
            index = 0;
            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(Math.Cos(a), Math.Sin(a), -rad * (start ? 0 : (end ? 1.5 : 1)));
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a), Math.Sin(a), rad * (start ? 0 : (end ? 1.5 : 1)));
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step), Math.Sin(a + step), -rad * (start ? 1.5 : (end ? 0 : 1)));
                Vector3d pnt4 = Vector3d.Create(Math.Cos(a + step), Math.Sin(a + step), rad * (start ? 1.5 : (end ? 0 : 1)));
                RotateUi.AddQuad(pnt1, pnt3, pnt2, pnt4, Colors.Blue, Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,255, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }



            //Z
            index = 0;
            for (double a = 0; a < twoPi; a += step)
            {
                bool start = (index % 10) == 0;
                bool end = ((index + 1) % 10) == 0;
                Vector3d pnt1 = Vector3d.Create(Math.Cos(a), -rad * (start ? 0 : (end ? 1.5 : 1)), Math.Sin(a));
                Vector3d pnt2 = Vector3d.Create(Math.Cos(a), rad * (start ? 0 : (end ? 1.5 : 1)), Math.Sin(a));
                Vector3d pnt3 = Vector3d.Create(Math.Cos(a + step), -rad * (start ? 1.5 : (end ? 0 : 1)), Math.Sin(a + step));
                Vector3d pnt4 = Vector3d.Create(Math.Cos(a + step), rad * (start ? 1.5 : (end ? 0 : 1)), Math.Sin(a + step));
                RotateUi.AddQuad(pnt1, pnt2, pnt3, pnt4, Colors.Green, Dates.Empty());
                //TranslateUI.AddQuad(pnt1, pnt2, pnt3, pnt4, Color.FromArgb(255,255, Math.Max(0, (int)(Math.Sin(a) * 128)), Math.Max(0, (int)(Math.Sin(a) * 128))), Dates.Empty());
                index++;
            }
        }


        Vector2d xHandle = new Vector2d();
        Vector2d yHandle = new Vector2d();
        Vector2d zHandle = new Vector2d();

        Vector2d[] hprHandles = new Vector2d[6];

        double uiScale = 1;

        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {

            Matrix3d oldWorld = renderContext.World;
            Matrix3d rotation = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.RotationZ(-roll / 180f * Math.PI), Matrix3d.RotationX(-pitch / 180f * Math.PI)), Matrix3d.RotationY(heading / 180f * Math.PI));

            renderContext.World = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(rotation, Matrix3d.Scaling(scale.X, scale.Y, scale.Z)), Matrix3d.Translation(translate)), oldWorld);
            renderContext.TwoSidedLighting = TwoSidedGeometry;
            //todo           renderContext.setRasterizerState(TwoSidedGeometry ? TriangleCullMode.Off : TriangleCullMode.CullCounterClockwise);
            if (lightID > 0)
            {
                //draw light

                //   Planets.DrawPointPlanet(renderContext, Vector3d.Create(), 1, Color, false, 1.0f);
            }
            else
            {
                if (object3d != null)
                {
                    object3d.Color = Color;
                    object3d.Render(renderContext, opacity * Opacity);
                }
            }
            //todo             renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
            renderContext.TwoSidedLighting = false;


            //todo enable edit UI

            //if (showEditUi)
            //{
            //    if (lightID > 0)
            //    {
            //        //draw light

            //        Planets.DrawPointPlanet(renderContext, new Vector3d(), 1, Color, false, 1.0f);
            //    }

            //    DepthStencilMode oldDepthMode = renderContext.DepthStencilMode = DepthStencilMode.Off;
            //    renderContext.World = Matrix3d.Translation(translate) * oldWorld;

            //    Matrix3d wvp = renderContext.World * renderContext.View * renderContext.Projection;

            //    Vector3d vc = Vector3d.Create(0, 0, 0);
            //    Vector3d vc1 = Vector3d.Create(.001, 0, 0);
            //    Vector3d vc2 = Vector3d.Create(0, .001, 0);
            //    Vector3d vc3 = Vector3d.Create(0, 0, .001);
            //    Vector3d vs = Vector3d.TransformCoordinate(vc, wvp);
            //    Vector3d vs1 = Vector3d.TransformCoordinate(vc1, wvp);
            //    Vector3d vs2 = Vector3d.TransformCoordinate(vc2, wvp);
            //    Vector3d vs3 = Vector3d.TransformCoordinate(vc3, wvp);

            //    Vector2d vsa = Vector2d.Create(vs.X, vs.Y);
            //    Vector2d vsa1 = Vector2d.Subtract(Vector2d.Create(vs1.X, vs1.Y),vsa);
            //    Vector2d vsa2 = Vector2d.Subtract(Vector2d.Create(vs2.X, vs2.Y),vsa);
            //    Vector2d vsa3 = Vector2d.Subtract(Vector2d.Create(vs3.X, vs3.Y),vsa);

            //    uiScale = .0003 / Math.Sqrt((vsa1.Length * vsa1.Length + vsa2.Length * vsa2.Length + vsa3.Length * vsa3.Length));

            //    Matrix3d matUIScale = Matrix3d.Scaling(uiScale, uiScale, uiScale);

            //    renderContext.World = matUIScale * renderContext.World;

            //    wvp = renderContext.World * renderContext.View * renderContext.Projection;


            //    vc1 = Vector3d.Create(.9, 0, 0);
            //    vc2 = Vector3d.Create(0, .9, 0);
            //    vc3 = Vector3d.Create(0, 0, .9);
            //    vs = Vector3d.TransformCoordinate(vc, wvp);
            //    vs1 = Vector3d.TransformCoordinate(vc1, wvp);
            //    vs2 = Vector3d.TransformCoordinate(vc2, wvp);
            //    vs3 = Vector3d.TransformCoordinate(vc3, wvp);

            //    double h = renderContext.ViewPort.Height;
            //    double w = renderContext.ViewPort.Width;

            //    xHandle = Vector2d.Create((vs1.X + 1) * w / 2, h - ((vs1.Y + 1) * h / 2));
            //    yHandle = Vector2d.Create((vs2.X + 1) * w / 2, h - ((vs2.Y + 1) * h / 2));
            //    zHandle = Vector2d.Create((vs3.X + 1) * w / 2, h - ((vs3.Y + 1) * h / 2));


            //    // draw UI
            //    if (TranslateUI == null)
            //    {
            //        InitTranslateUI();

            //    }

            //    bool showTranslate = Control.ModifierKeys != Keys.Control && Control.ModifierKeys != Keys.Shift;
            //    bool showRotate = Control.ModifierKeys == Keys.Control;
            //    bool showScale = Control.ModifierKeys == Keys.Shift;

            //    if (showTranslate)
            //    {
            //        TranslateUILines.DrawLines(renderContext, 1.0f);

            //        TranslateUI.Draw(renderContext, 1.0f, TriangleList.CullMode.Clockwise);
            //    }
            //    else
            //    {
            //        if (showScale)
            //        {
            //            TranslateUILines.DrawLines(renderContext, 1.0f);
            //            ScaleUI.Draw(renderContext, 1.0f, TriangleList.CullMode.Clockwise);
            //        }
            //        else
            //        {
            //            xHandle = Vector2d.Create(-1000, 0);
            //            yHandle = Vector2d.Create(-1000, 0);
            //            zHandle = Vector2d.Create(-1000, 0);
            //        }
            //    }

            //    renderContext.World = rotation * renderContext.World;

            //    if (showRotate)
            //    {
            //        wvp = renderContext.World * renderContext.View * renderContext.Projection;

            //        Vector3d[] hprPoints = new Vector3d[]
            //                        {
            //                            Vector3d.Create(0,0,1),
            //                            Vector3d.Create(0,0,-1),
            //                            Vector3d.Create(0,1,0),
            //                            Vector3d.Create(0,-1,0),
            //                            Vector3d.Create(-1,0,0),
            //                            Vector3d.Create(1,0,0)
            //                        };
            //        hprHandles = new Vector2d[6];
            //        for (int i = 0; i < 6; i++)
            //        {
            //            Vector3d vt = Vector3d.TransformCoordinate(hprPoints[i], wvp);
            //            hprHandles[i] = Vector2d.Create((vt.X + 1) * w / 2, h - ((vt.Y + 1) * h / 2));
            //        }

            //        RotateUi.Draw(renderContext, 1.0f, TriangleList.CullMode.Clockwise);
            //    }
            //    else
            //    {
            //        hprHandles = new Vector2d[0];
            //    }




            //    oldDepthMode = renderContext.DepthStencilMode = oldDepthMode;

            //    //restore matrix
            //    renderContext.World = oldWorld;
            //    showEditUi = false;
            //}
            renderContext.World = oldWorld;

            return true;
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            // Add files to cabinet
            //if (object3d != null)
            //{
            //    string fName = object3d.Filename;

            //    bool copy = true;
            //    //bool copy = !fName.Contains(ID.ToString());
            //    string ext = ObjType ? "obj" : "3ds";
            //    string fileName = fc.TempDirectory + string.Format("{0}\\{1}.{2}", fc.PackageID, this.ID.ToString(), ext);
            //    string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);
            //    string path2 = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);

            //    if (copy)
            //    {
            //        if (!Directory.Exists(path2))
            //        {
            //            Directory.CreateDirectory(path2);
            //        }
            //        if (File.Exists(fName) && !File.Exists(fileName))
            //        {
            //            File.Copy(fName, fileName);
            //        }

            //        foreach (string meshfile in object3d.meshFilenames)
            //        {
            //            if (!String.IsNullOrEmpty(meshfile))
            //            {
            //                string textureFilename = fc.TempDirectory + string.Format("{0}\\{1}", fc.PackageID, meshfile);
            //                string mFilename = path + "\\" + meshfile;
            //                string newfilename = Object3d.FindFile(mFilename);
            //                if (string.IsNullOrEmpty(newfilename))
            //                {
            //                    newfilename = Object3d.FindFileFuzzy(mFilename);
            //                }



            //                if (File.Exists(newfilename) && !File.Exists(textureFilename))
            //                {
            //                    File.Copy(newfilename, textureFilename);
            //                }
            //            }
            //        }
            //    }

            //    if (File.Exists(fileName))
            //    {
            //        fc.AddFile(fileName);
            //    }

            //    foreach (string meshfile in object3d.meshFilenames)
            //    {
            //        if (!string.IsNullOrEmpty(meshfile))
            //        {
            //            string textureFilename = fc.TempDirectory + string.Format("{0}\\{1}", fc.PackageID, meshfile);
            //            fc.AddFile(textureFilename);
            //        }
            //    }
            //}
        }

        public override void LoadData(TourDocument doc, string filename)
        {
            // ObjType = true;

            if (filename.ToLowerCase().EndsWith(".obj"))
            {
                ObjType = true;
            }

            if (lightID == 0)
            {
                if (ObjType)
                {
                    object3d = new Object3d(doc, filename.Replace(".txt", ".obj"), FlipV, flipHandedness, true, Color);
                }
                else
                {
                    object3d = new Object3d(doc, filename.Replace(".txt", ".3ds"), FlipV, flipHandedness, true, Color);
                }
            }
        }

        public Vector2d PointToView(Vector2d pnt)
        {
            double clientHeight = WWTControl.Singleton.RenderContext.Height;
            double clientWidth = WWTControl.Singleton.RenderContext.Width;
            double viewWidth = (WWTControl.Singleton.RenderContext.Width / WWTControl.Singleton.RenderContext.Height) * 1116f;
            double x = (((double)pnt.X) / ((double)clientWidth) * viewWidth) - ((viewWidth - 1920) / 2);
            double y = ((double)pnt.Y) / clientHeight * 1116;

            return Vector2d.Create(x, y);
        }

        public void Render(RenderContext renderEngine)
        {
            showEditUi = true;
            return;
        }
        bool showEditUi = false;
        public void PreRender(RenderContext renderEngine)
        {
            showEditUi = true;
            return;
        }



        Draging dragMode = Draging.None;

        Vector2d pntDown = new Vector2d();
        double valueOnDown = 0;
        double valueOnDown2 = 0;

        double hitDist = 20;

        public bool MouseDown(object sender, ElementEvent e)
        {
            Vector2d location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));

            pntDown = location;

            Vector2d pnt = location;

            if (e.ShiftKey)
            {
                if ((Vector2d.Subtract(pnt, xHandle)).Length < hitDist)
                {
                    dragMode = Draging.Scale;
                    valueOnDown = this.scale.X;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, yHandle)).Length < hitDist)
                {
                    dragMode = Draging.Scale;
                    valueOnDown = this.scale.Y;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, zHandle)).Length < hitDist)
                {
                    dragMode = Draging.Scale;
                    valueOnDown = this.scale.Z;
                    return true;
                }
            }
            else
            {
                if ((Vector2d.Subtract(pnt, xHandle)).Length < hitDist)
                {
                    dragMode = Draging.X;
                    valueOnDown = this.translate.X;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, yHandle)).Length < hitDist)
                {
                    dragMode = Draging.Y;
                    valueOnDown = this.translate.Y;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, zHandle)).Length < hitDist)
                {
                    dragMode = Draging.Z;
                    valueOnDown = this.translate.Z;
                    return true;
                }
            }

            for (int i = 0; i < hprHandles.Length; i++)
            {
                if ((Vector2d.Subtract(pnt, hprHandles[i])).Length < hitDist)
                {
                    switch (i)
                    {
                        case 0:
                            dragMode = Draging.HP;
                            valueOnDown = this.heading;
                            valueOnDown2 = this.pitch;
                            return true;
                        case 1:
                            dragMode = Draging.HP1;
                            valueOnDown = this.heading;
                            valueOnDown2 = this.pitch;
                            return true;
                        case 2:
                            dragMode = Draging.PR;
                            valueOnDown = this.pitch;
                            valueOnDown2 = this.roll;
                            return true;
                        case 3:
                            dragMode = Draging.PR1;
                            valueOnDown = this.pitch;
                            valueOnDown2 = this.roll;
                            return true;
                        case 4:
                            dragMode = Draging.RH;
                            valueOnDown = this.roll;
                            valueOnDown2 = this.heading;
                            return true;
                        case 5:
                            dragMode = Draging.RH1;
                            valueOnDown = this.roll;
                            valueOnDown2 = this.heading;
                            return true;
                        default:
                            break;
                    }
                }
            }

            return false;
        }

        public bool MouseUp(object sender, ElementEvent e)
        {
            if (dragMode != Draging.None)
            {
                dragMode = Draging.None;
                lockPreferedAxis = false;
                return true;
            }
            return false;
        }

        bool lockPreferedAxis = false;
        bool preferY = false;

        public bool MouseMove(object sender, ElementEvent e)
        {

            Vector2d location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));

            if (dragMode != Draging.None)
            {
                double dist = 0;
                double distX = location.X - pntDown.X;
                double distY = -(location.Y - pntDown.Y);

                if (lockPreferedAxis)
                {
                    if (preferY)
                    {
                        dist = distY;
                        preferY = true;
                        Cursor.Current = Cursors.SizeNS;
                    }
                    else
                    {
                        dist = distX;
                        preferY = false;
                        Cursor.Current = Cursors.SizeWE;
                    }
                }
                else
                {
                    if (Math.Abs(distX) > Math.Abs(distY))
                    {
                        dist = distX;
                        preferY = false;
                    }
                    else
                    {
                        dist = distY;
                        preferY = true;
                    }
                    if (dist > 5)
                    {
                        lockPreferedAxis = true;
                    }
                }

                switch (dragMode)
                {
                    case Draging.None:
                        break;
                    case Draging.X:
                        this.translate.X = valueOnDown + (12 * uiScale * (dist / WWTControl.Singleton.RenderContext.Width));
                        break;
                    case Draging.Y:
                        this.translate.Y = valueOnDown + (12 * uiScale * (dist / WWTControl.Singleton.RenderContext.Width));
                        break;
                    case Draging.Z:
                        this.translate.Z = valueOnDown + (12 * uiScale * (dist / WWTControl.Singleton.RenderContext.Width));
                        break;
                    case Draging.HP:
                        this.heading = valueOnDown - distX / 4;
                        this.pitch = valueOnDown2 + distY / 4;
                        break;
                    case Draging.PR:
                        this.pitch = valueOnDown + distY / 4;
                        this.roll = valueOnDown2 - distX / 4;
                        break;
                    case Draging.RH:
                        this.roll = valueOnDown + distY / 4;
                        this.heading = valueOnDown2 - distX / 4;
                        break;
                    case Draging.HP1:
                        this.heading = valueOnDown - distX / 4;
                        this.pitch = valueOnDown2 - distY / 4;
                        break;
                    case Draging.PR1:
                        this.pitch = valueOnDown + distY / 4;
                        this.roll = valueOnDown2 + distX / 4;
                        break;
                    case Draging.RH1:
                        this.roll = valueOnDown - distY / 4;
                        this.heading = valueOnDown2 - distX / 4;
                        break;
                    case Draging.Scale:
                        this.scale.X = this.scale.Y = this.scale.Z = valueOnDown * Math.Pow(2, (dist / 100));
                        break;
                    default:
                        break;
                }
                FireChanged();
                return true;
            }
            else
            {
                Vector2d pnt = location;


                if ((Vector2d.Subtract(pnt, xHandle)).Length < hitDist)
                {
                    Cursor.Current = Cursors.SizeAll;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, yHandle)).Length < hitDist)
                {
                    Cursor.Current = Cursors.SizeAll;
                    return true;
                }

                if ((Vector2d.Subtract(pnt, zHandle)).Length < hitDist)
                {
                    Cursor.Current = Cursors.SizeAll;
                    return true;
                }

                for (int i = 0; i < hprHandles.Length; i++)
                {
                    if ((Vector2d.Subtract(pnt, hprHandles[i])).Length < hitDist)
                    {
                        Cursor.Current = Cursors.SizeAll;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool MouseClick(object sender, ElementEvent e)
        {


            return false;
        }

        public bool Click(object sender, ElementEvent e)
        {
            return false;
        }

        public bool MouseDoubleClick(object sender, ElementEvent e)
        {
            return false;
        }

        public bool KeyDown(object sender, ElementEvent e)
        {
            return false;
        }

        public bool KeyUp(object sender, ElementEvent e)
        {
            return false;
        }

        public bool Hover(Vector2d pnt)
        {
            return false;
        }
    }

    public class Group
    {
        public int startIndex;
        public int indexCount;
        public int materialIndex;
    }

    public class Mesh : IDisposable
    {
        public void Dispose()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                vertexBuffer = null;
            }

            if (tangentVertexBuffer != null)
            {
                tangentVertexBuffer.Dispose();
                tangentVertexBuffer = null;
            }

            if (indexBuffer != null)
            {
                indexBuffer.Dispose();
                indexBuffer = null;
            }
        }

        public Mesh()
        {

        }

        public static Mesh Create(PositionNormalTextured[] vertices, int[] indices)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.indices = indices;

            Vector3d[] points = new Vector3d[vertices.Length];
            for (int i = 0; i < vertices.Length; ++i)
                points[i] = vertices[i].Position;
            mesh.BoundingSphere = ConvexHull.FindEnclosingSphere(points);
            return mesh;
        }

        //public Mesh(PositionNormalTextured[] vertices, int[] indices)
        //{
        //    this.vertices = vertices;

        //    this.indices = new uint[indices.Length];
        //    for (int c = 0; c < indices.Length; c++)
        //    {
        //        this.indices[c] = (uint)indices[c];
        //    }

        //    Vector3d[] points = new Vector3d[vertices.Length];
        //    for (int i = 0; i < vertices.Length; ++i)
        //        points[i] = vertices[i].Position;
        //    boundingSphere = ConvexHull.FindEnclosingSphere(points);
        //}

        // Create a mesh from vertices with tangents, for use with a normal map
        public static Mesh CreateTangent(PositionNormalTexturedTangent[] vertices, int[] indices)
        {
            Mesh mesh = new Mesh();

            mesh.tangentVertices = vertices;
            mesh.indices = indices;

            Vector3d[] points = new Vector3d[mesh.tangentVertices.Length];
            for (int i = 0; i < mesh.tangentVertices.Length; ++i)
                points[i] = mesh.tangentVertices[i].Position;
            mesh.BoundingSphere = ConvexHull.FindEnclosingSphere(points);

            return mesh;
        }

        //public void setMaterialGroups(Group[] groups)
        //{
        //    attributeGroups = groups;
        //}

        public void setObjects(List<ObjectNode> objects)
        {
            this.objects = objects;
        }

        // Convert the vertex data to a GPU vertex buffer
        public void commitToDevice()
        {
            if (vertices != null)
            {
                vertexBuffer = PositionNormalTexturedVertexBuffer.Create(vertices);
            }
            else if (tangentVertices != null)
            {
                tangentVertexBuffer = PositionNormalTexturedTangentVertexBuffer.Create(tangentVertices);
            }

            indexBuffer = new IndexBuffer(new Uint32Array((object)indices));
        }

        public void beginDrawing(RenderContext renderContext)
        {
            if (vertexBuffer != null)
            {
                renderContext.SetVertexBuffer(vertexBuffer);
            }
            else if (tangentVertexBuffer != null)
            {
                renderContext.SetVertexBuffer(tangentVertexBuffer);
            }

            if (indexBuffer != null)
            {
                renderContext.SetIndexBuffer(indexBuffer);
            }

            //           SharpDX.Direct3D11.DeviceContext devContext = renderContext.Device.ImmediateContext;
            //           devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
        }

        public void drawSubset(RenderContext renderContext, int materialIndex)
        {
            if (indexBuffer == null || objects == null)
            {
                return;
            }

            //var vertexLayout = PlanetShader.StandardVertexLayout.PositionNormalTex;
            //if (tangentVertexBuffer != null)
            //{
            //    vertexLayout = PlanetShader.StandardVertexLayout.PositionNormalTexTangent;
            //}

            //SharpDX.Direct3D11.DeviceContext devContext = renderContext.Device.ImmediateContext;
            //devContext.InputAssembler.InputLayout = renderContext.Shader.inputLayout(vertexLayout);


            DrawHierarchy(objects, materialIndex, renderContext, 0);
        }

        public void DrawHierarchy(List<ObjectNode> nodes, int materialIndex, RenderContext renderContext, int depth)
        {
            if (depth > 1212)
            {
                return;
            }
            foreach (ObjectNode node in nodes)
            {
                if (node.DrawGroup != null && node.Enabled)
                {
                    foreach (Group group in node.DrawGroup)
                    {
                        if (group.materialIndex == materialIndex)
                        {

                            renderContext.gl.drawElements(GL.TRIANGLES, group.indexCount, GL.UNSIGNED_INT, group.startIndex*4);
                        }
                    }
                }
                DrawHierarchy(node.Children, materialIndex, renderContext, depth + 1);
            }
        }

        public PositionNormalTexturedVertexBuffer vertexBuffer;
        public PositionNormalTexturedTangentVertexBuffer tangentVertexBuffer;
        public IndexBuffer indexBuffer;

        // Only one of these two will be non-null
        public PositionNormalTextured[] vertices;
        public PositionNormalTexturedTangent[] tangentVertices;
        public int[] indices;

        public SphereHull BoundingSphere = new SphereHull();

        //Group[] attributeGroups;
        List<ObjectNode> objects;

        public List<ObjectNode> Objects
        {
            get { return objects; }
            set { objects = value; }
        }
    }




    class VertexPosition
    {
        public Vector3d position;
        public uint index;
    };

    public class Object3d
    {
        public bool FlipHandedness = false;
        public bool FlipV = true;
        public bool Smooth = true;
        public string Filename;
        Mesh mesh = null; // Our mesh object in sysmem
        List<Material> meshMaterials = new List<Material>(); // Materials for our mesh
        List<Texture> meshTextures = new List<Texture>(); // Textures for our mesh
        List<Texture> meshSpecularTextures = new List<Texture>(); // Specular textures for our mesh
        List<Texture> meshNormalMaps = new List<Texture>(); // Normal maps for our mesh
        public List<String> meshFilenames = new List<string>(); // filenames for meshes

        public Color Color = Colors.White;


        //public static Object3d LoadFromModelFileFromUrl(string url)
        //{
        //    int hash = url.GetHashCode();

        //    Object3d model = null;

        //    string path = Properties.Settings.Default.CahceDirectory + @"mdl\" + hash.ToString() + @"\";
        //    string filename = path + hash + ".mdl";
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    if (!File.Exists(filename))
        //    {
        //        DataSetManager.DownloadFile(url, filename, false, true);
        //    }
        //    string objFile = "";

        //    using (Stream s = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        ZipArchive zip = new ZipArchive(s);
        //        foreach (ZipEntry zFile in zip.Files)
        //        {
        //            Stream output = File.Open(path + zFile.Filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        //            Stream input = zFile.GetFileStream();
        //            CopyStream(input, output);
        //            input.Close();
        //            output.Close();
        //            input.Dispose();
        //            output.Dispose();
        //            if (zFile.Filename.ToLower().EndsWith(".obj"))
        //            {
        //                objFile = path + zFile.Filename;
        //            }
        //        }
        //    }

        //    if (File.Exists(objFile))
        //    {
        //        Object3d o3d = new Object3d(objFile, true, false, true, Colors.White);
        //        if (o3d != null)
        //        {
        //            model = o3d;
        //        }
        //    }

        //    return model;
        //}

        //public static void CopyStream(Stream input, Stream output)
        //{
        //    byte[] buffer = new byte[8192];
        //    int len;
        //    while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
        //    {
        //        output.Write(buffer, 0, len);
        //    }
        //}

        public Object3d(TourDocument tourDoc, string filename, bool flipV, bool flipHandedness, bool smooth, Color color)
        {
            Color = color;
            Smooth = smooth;
            FlipV = flipV;
            FlipHandedness = flipHandedness;
            Filename = filename;
            if (Filename.ToLowerCase().EndsWith(".obj"))
            {
                LoadMeshFromObj(tourDoc,Filename);
            }
            else
            {
                LoadMeshFrom3ds(tourDoc, Filename, 1.0f);
            }
        }

        internal void Reload()
        {
            if (!ISSLayer)
            {
                Dispose();
                if (Filename.ToLowerCase().EndsWith(".obj"))
                {
                    LoadMeshFromObj(tourDocument, Filename);
                }

                else
                {
                    LoadMeshFrom3ds(tourDocument, Filename, 1.0f);
                }
            }
        }

        private static int CompareVector3(Vector3d v0, Vector3d v1)
        {
            if (v0.X < v1.X)
            {
                return -1;
            }
            else if (v0.X > v1.X)
            {
                return 1;
            }
            else if (v0.Y < v1.Y)
            {
                return -1;
            }
            else if (v0.Y > v1.Y)
            {
                return 1;
            }
            else if (v0.Z < v1.Z)
            {
                return -1;
            }
            else if (v0.Z > v1.Z)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private static int CompareVector(Vector2d v0, Vector2d v1)
        {
            if (v0.X < v1.X)
            {
                return -1;
            }
            else if (v0.X > v1.X)
            {
                return 1;
            }
            else if (v0.Y < v1.Y)
            {
                return -1;
            }
            else if (v0.Y > v1.Y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        // Calculate per-vertex normals by averaging face normals. Normals of adjacent faces with an
        // angle of greater than crease angle are not included in the average. CalculateVertexNormalsMerged
        // is slower than the other normal generation method, CalculateVertexNormals, but it produces better
        // results. Vertices with identical positions (bot possibly different texture coordinates) are treated
        // as the same vertex for purposes of normal calculation. This allows smooth normals across texture
        // wrap seams.
        //
        // This method returns an array of vertex normals, one for each index in the index list
        private Vector3d[] CalculateVertexNormalsMerged(List<PositionNormalTextured> vertexList, List<int> indexList, double creaseAngleRad)
        {
            if (vertexList.Count == 0)
            {
                return null;
            }

            int vertexCount = vertexList.Count;
            int triangleCount = Math.Floor(indexList.Count / 3);

            // Create a list of vertices sorted by their positions. This will be used to
            // produce a list of vertices with unique positions.
            List<VertexPosition> vertexPositions = new List<VertexPosition>();
            for (int vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                VertexPosition vp = new VertexPosition();
                //todo11 this should be native..
                vp.position = vertexList[vertexIndex].Position;
                vp.index = (uint)vertexIndex;
                vertexPositions.Add(vp);
            }

            vertexPositions.Sort(delegate (VertexPosition v0, VertexPosition v1) { return CompareVector3(v0.position, v1.position); });
            // vertexMap will map a vertex index to the index of a vertex with a unique position
            int[] vertexMap = new int[vertexPositions.Count];
            int uniqueVertexCount = 0;
            for (int vertexIndex = 0; vertexIndex < vertexPositions.Count; vertexIndex++)
            {
                if (vertexIndex == 0 || CompareVector3(vertexPositions[vertexIndex].position, vertexPositions[vertexIndex - 1].position) != 0)
                {
                    ++uniqueVertexCount;
                }
                vertexMap[vertexPositions[vertexIndex].index] = uniqueVertexCount - 1;
            }

            int[] vertexInstanceCounts = new int[uniqueVertexCount];
            for(int i=0; i< uniqueVertexCount; i++ )
            {
                vertexInstanceCounts[i] = 0;
            }

            foreach (int vertexIndex in indexList)
            {
                int uniqueIndex = vertexMap[vertexIndex];
                vertexInstanceCounts[uniqueIndex]++;
            }

            // vertexInstances contains the list of faces each vertex is referenced in
            int[][] vertexInstances = new int[uniqueVertexCount][];

            for (int i = 0; i < uniqueVertexCount; ++i)
            {
                int count = vertexInstanceCounts[i];
                if (count > 0)
                {
                    vertexInstances[i] = new int[count];
                    for(int j =0; j< count; j++)
                    {
                        vertexInstances[i][j] = 0;
                    }
                }
            }

            // For each vertex, record all faces which include it
            for (int i = 0; i < indexList.Count; ++i)
            {
                int faceIndex = Math.Floor(i / 3);
                int uniqueIndex = vertexMap[indexList[i]];
                vertexInstances[uniqueIndex][--vertexInstanceCounts[uniqueIndex]] = faceIndex;
            }

            // At this point, vertexInstanceCounts should contain nothing but zeroes

            // Compute normals for all faces
            Vector3d[] faceNormals = new Vector3d[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
            {
                // The face normal is just the cross product of the two edge vectors
                int i0 = indexList[i * 3 + 0];
                int i1 = indexList[i * 3 + 1];
                int i2 = indexList[i * 3 + 2];
                Vector3d edge0 = Vector3d.SubtractVectors(vertexList[i1].Position, vertexList[i0].Position);
                Vector3d edge1 = Vector3d.SubtractVectors(vertexList[i2].Position, vertexList[i1].Position);
                faceNormals[i] = Vector3d.Cross(edge0, edge1);

                faceNormals[i].Normalize();
            }

            // Finally, average the face normals
            int newVertexCount = triangleCount * 3;
            Vector3d[] vertexNormals = new Vector3d[newVertexCount];
            float cosCreaseAngle = Math.Min(0.9999f, (float)Math.Cos(creaseAngleRad));
            for (int i = 0; i < newVertexCount; ++i)
            {
                int vertexIndex = indexList[i];
                int uniqueIndex = vertexMap[vertexIndex];
                Vector3d faceNormal = faceNormals[Math.Floor(i / 3)];

                Vector3d sum = new Vector3d();
                foreach (int faceIndex in vertexInstances[uniqueIndex])
                {
                    Vector3d n = faceNormals[faceIndex];
                    if (Vector3d.Dot(faceNormal, n) > cosCreaseAngle)
                    {
                        sum.Add(n);
                    }
                }

                vertexNormals[i] = sum;
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }


        // Calculate tangent vectors at each vertex. The 'face tangent' is a direction in the plane of the
        // triangle and parallel to the direction of increasing tex coord u, i.e. the partial derivative
        // with respect to u of the triangle's plane equation expressed in terms of the texture coordinate
        // (u, v). Partial derivatives of the triangles containing a vertex are averaged to compute the
        // vertex tangent. Faces are not included in the when the angle formed with the test face is
        // greater than the crease angle, or when the texture texture coordinates are not continuous.
        //
        // This method returns an array of vertex normals, one for each index in the index list
        private Vector3d[] CalculateVertexTangents(List<PositionNormalTextured> vertexList, List<uint> indexList, float creaseAngleRad)
        {
            if (vertexList.Count == 0)
            {
                return null;
            }

            int vertexCount = vertexList.Count;
            int triangleCount = Math.Floor(indexList.Count / 3);

            // Create a list of vertices sorted by their positions. This will be used to
            // produce a list of vertices with unique positions.
            List<VertexPosition> vertexPositions = new List<VertexPosition>();
            for (int vertexIndex = 0; vertexIndex < vertexList.Count; ++vertexIndex)
            {
                VertexPosition vp = new VertexPosition();
                vp.position = vertexList[vertexIndex].Position;
                vp.index = (uint)vertexIndex;
                vertexPositions.Add(vp);
            }

            vertexPositions.Sort(delegate (VertexPosition v0, VertexPosition v1) { return CompareVector3(v0.position, v1.position); });

            // vertexMap will map a vertex index to the index of a vertex with a unique position
            uint[] vertexMap = new uint[vertexPositions.Count];
            int uniqueVertexCount = 0;
            for (int vertexIndex = 0; vertexIndex < vertexPositions.Count; vertexIndex++)
            {
                if (vertexIndex == 0 || CompareVector3(vertexPositions[vertexIndex].position, vertexPositions[vertexIndex - 1].position) != 0)
                {
                    ++uniqueVertexCount;
                }
                vertexMap[vertexPositions[vertexIndex].index] = (uint)(uniqueVertexCount - 1);
            }

            int[] vertexInstanceCounts = new int[uniqueVertexCount];
            for (int i = 0; i < uniqueVertexCount; i++)
            {
                vertexInstanceCounts[i] = 0;
            }

            foreach (int vertexIndex in indexList)
            {
                uint uniqueIndex = vertexMap[vertexIndex];
                vertexInstanceCounts[uniqueIndex]++;
            }

            // vertexInstances contains the list of faces each vertex is referenced in
            int[][] vertexInstances = new int[uniqueVertexCount][];
            for (int i = 0; i < uniqueVertexCount; ++i)
            {
                int count = vertexInstanceCounts[i];
                if (count > 0)
                {
                    vertexInstances[i] = new int[count];
                    for (int j = 0; j < count; j++)
                    {
                        vertexInstances[i][j] = 0;
                    }
                }
            }

            // For each vertex, record all faces which include it
            for (int i = 0; i < indexList.Count; ++i)
            {
                int faceIndex = Math.Floor(i / 3);
                uint uniqueIndex = vertexMap[indexList[i]];
                vertexInstances[uniqueIndex][--vertexInstanceCounts[uniqueIndex]] = faceIndex;
            }

            // At this point, vertexInstanceCounts should contain nothing but zeroes

            // Compute partial derivatives for all faces
            Vector3d[] partials = new Vector3d[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
            {
                PositionNormalTextured v0 = vertexList[(int)indexList[i * 3 + 0]];
                PositionNormalTextured v1 = vertexList[(int)indexList[i * 3 + 1]];
                PositionNormalTextured v2 = vertexList[(int)indexList[i * 3 + 2]];
                Vector3d edge0 = Vector3d.SubtractVectors(v1.Position, v0.Position);
                Vector3d edge1 = Vector3d.SubtractVectors(v2.Position, v0.Position);
                double m00 = v1.Tu - v0.Tu;
                double m01 = v1.Tv - v0.Tv;
                double m10 = v2.Tu - v0.Tu;
                double m11 = v2.Tv - v0.Tv;

                double determinant = m00 * m11 - m01 * m10;
                if (Math.Abs(determinant) < 1.0e-6f)
                {
                    // No unique vector; just select one of the edges
                    if (edge0.LengthSq() > 0.0f)
                    {
                        partials[i] = edge0;
                        partials[i].Normalize();
                    }
                    else
                    {
                        // Degenerate edge; just use the unit x vector
                        partials[i] = Vector3d.Create(1.0f, 0.0f, 0.0f);
                    }
                }
                else
                {
                    // Matrix n is the inverse of m
                    double invDeterminant = 1.0f / determinant;
                    double n00 = m11 * invDeterminant;
                    double n01 = -m01 * invDeterminant;
                    double n10 = -m10 * invDeterminant;
                    double n11 = m00 * invDeterminant;

                    partials[i] = Vector3d.AddVectors(Vector3d.MultiplyScalar(edge0, n00), Vector3d.MultiplyScalar(edge1, n01));
                    partials[i].Normalize();
                }
            }

            // Finally, average the partial derivatives
            int newVertexCount = triangleCount * 3;
            Vector3d[] tangents = new Vector3d[newVertexCount];
            float cosCreaseAngle = Math.Min(0.9999f, (float)Math.Cos(creaseAngleRad));
            for (int i = 0; i < newVertexCount; ++i)
            {
                uint vertexIndex = indexList[i];
                uint uniqueIndex = vertexMap[(int)vertexIndex];
                Vector3d du = partials[Math.Floor(i / 3)];

                Vector3d sum = new Vector3d();
                foreach (int faceIndex in vertexInstances[uniqueIndex])
                {
                    Vector3d T = partials[faceIndex];
                    if (Vector3d.Dot(du, T) > cosCreaseAngle)
                    {
                        sum.Add(T);
                    }
                }

                Vector3d N = vertexList[(int)vertexIndex].Normal;

                // Make the tangent orthogonal to the vertex normal
                tangents[i] = Vector3d.SubtractVectors(sum, Vector3d.MultiplyScalar(N, Vector3d.Dot(N, sum)));
                tangents[i].Normalize();
            }

            return tangents;
        }


        // Calculate per-vertex normals by averaging face normals. Normals of adjacent faces with an
        // angle of greater than crease angle are not included in the average.
        //
        // This method returns an array of vertex normals, one for each index in the index list
        private Vector3d[] CalculateVertexNormals(List<PositionNormalTextured> vertexList, List<int> indexList, float creaseAngleRad)
        {
            int vertexCount = vertexList.Count;
            int triangleCount = Math.Floor(indexList.Count / 3);

            // vertexInstanceCounts contains the number of times each vertex is referenced in the mesh 
            int[] vertexInstanceCounts = new int[vertexCount];
            foreach (int vertexIndex in indexList)
            {
                vertexInstanceCounts[vertexIndex]++;
            }

            // vertexInstances contains the list of faces each vertex is referenced in
            int[][] vertexInstances = new int[vertexCount][];
            for (int i = 0; i < vertexCount; ++i)
            {
                int count = vertexInstanceCounts[i];
                if (count > 0)
                {
                    vertexInstances[i] = new int[count];
                }
            }

            // For each vertex, record all faces which include it
            for (int i = 0; i < indexList.Count; ++i)
            {
                int faceIndex = Math.Floor(i / 3);
                int vertexIndex = indexList[i];
                vertexInstances[vertexIndex][--vertexInstanceCounts[vertexIndex]] = faceIndex;
            }

            // At this point, vertexInstanceCounts should contain nothing but zeroes

            // Compute normals for all faces
            Vector3d[] faceNormals = new Vector3d[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
            {
                // The face normal is just the cross product of the two edge vectors
                int i0 = indexList[i * 3 + 0];
                int i1 = indexList[i * 3 + 1];
                int i2 = indexList[i * 3 + 2];
                Vector3d edge0 = Vector3d.SubtractVectors(vertexList[i1].Position, vertexList[i0].Position);
                Vector3d edge1 = Vector3d.SubtractVectors(vertexList[i2].Position, vertexList[i1].Position);
                faceNormals[i] = Vector3d.Cross(edge0, edge1);

                faceNormals[i].Normalize();
            }

            // Finally, average the face normals
            int newVertexCount = triangleCount * 3;
            Vector3d[] vertexNormals = new Vector3d[newVertexCount];
            float cosCreaseAngle = Math.Min(0.9999f, (float)Math.Cos(creaseAngleRad));
            for (int i = 0; i < newVertexCount; ++i)
            {
                int vertexIndex = indexList[i];
                Vector3d faceNormal = faceNormals[Math.Floor(i / 3)];

                Vector3d sum = new Vector3d();
                foreach (int faceIndex in vertexInstances[vertexIndex])
                {
                    Vector3d n = faceNormals[faceIndex];
                    if (Vector3d.Dot(faceNormal, n) > cosCreaseAngle)
                    {
                        sum.Add(n);
                    }
                }

                vertexNormals[i] = sum;
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        // Add textures to ensure that we have as many textures as 
        private void addMaterial(Material material)
        {
            meshMaterials.Add(material);
            while (meshTextures.Count < meshMaterials.Count)
            {
                meshTextures.Add(null);
            }

            while (meshSpecularTextures.Count < meshMaterials.Count)
            {
                meshSpecularTextures.Add(null);
            }

            while (meshNormalMaps.Count < meshMaterials.Count)
            {
                meshNormalMaps.Add(null);
            }
        }

        // Load a color chunk from a 3ds file
        // Colors may be stored in a 3ds file either as 3 floats or 3 bytes
        private Color LoadColorChunk(BinaryReader br)
        {
            ushort chunkID = br.ReadUInt16();
            uint chunkLength = br.ReadUInt32();
            Color color = Colors.Black;

            if ((chunkID == 0x0010 || chunkID == 0x0013) && chunkLength == 18)
            {
                // Need to guard against values outside of [0, 1], otherwise Colors.FromArgb
                // will throw an exception.
                float r = Math.Max(0.0f, Math.Min(1.0f, br.ReadSingle()));
                float g = Math.Max(0.0f, Math.Min(1.0f, br.ReadSingle()));
                float b = Math.Max(0.0f, Math.Min(1.0f, br.ReadSingle()));
                color = Color.FromArgb(255, (int)(255.0f * r), (int)(255.0f * g), (int)(255.0f * b));
            }
            else if ((chunkID == 0x0011 || chunkID == 0x0012) && chunkLength == 9)
            {
                color = Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
            }
            else
            {
                // Unknown color block; ignore it
                br.ReadBytes((int)chunkLength - 6);
            }

            return color;
        }


        // Load a percentage chunk from a 3ds file
        // A percentage may be stored as either a float or a 16-bit integer
        private float LoadPercentageChunk(BinaryReader br)
        {
            ushort chunkID = br.ReadUInt16();
            uint chunkLength = br.ReadUInt32();
            float percentage = 0.0f;

            if (chunkID == 0x0030 && chunkLength == 8)
            {
                percentage = br.ReadUInt16();
            }
            else if (chunkID == 0x0031 && chunkLength == 10)
            {
                percentage = br.ReadSingle();
            }
            else
            {
                // Unknown percentage block; ignore it
                br.ReadBytes((int)chunkLength - 6);
            }

            return percentage;
        }



        Dictionary<string, Texture> TextureCache = new Dictionary<string, Texture>();

        string[] matFiles = new string[0];
        int matFileIndex = 0;
        private void LoadMeshFromObj(TourDocument doc, string filename)
        {
            this.Filename = filename;
            tourDocument = doc;
            

            Blob blob = doc.GetFileBlob(filename);
            FileReader chunck = new FileReader();
            chunck.OnLoadEnd = delegate (FileProgressEvent e)
            {
                matFiles = ReadObjMaterialsFromBin(chunck.Result as string);
                matFileIndex = 0;
                // pass data to LoadMatLib. It will chain load all the material files, then load the obj from this data - hack for having no synchronous blob reading in javascript
                LoadMatLib(chunck.Result as string);

                //this is delay loaded when mat files are all read.
                //ReadObjFromBin(chunck.Result as string);

            };
            chunck.ReadAsText(blob);
        }

        private string[] ReadObjMaterialsFromBin(string data)
        {
            List<string> matFiles = new List<string>();

            string[] lines = data.Split("\n");
            {

                foreach (string lineraw in lines)
                {
                    string line = lineraw.Replace("  ", " ");

                    string[] parts = line.Trim().Split(" ");

                    if (parts.Length > 0)
                    {
                        switch (parts[0])
                        {
                            case "mtllib":
                                {
                                    string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                    string matFile = path + parts[1];
                                    matFiles.Add(matFile);

                                    //LoadMatLib(matFile);
                                }
                                break;
                        }
                    }
                }
            }

            return matFiles;
        }

        private void ReadObjFromBin(string data)
        {
            bool objectFound = false;
            //Dictionary<string, ObjectNode> objectTable = new Dictionary<string, ObjectNode>();
            List<ObjectNode> objects = new List<ObjectNode>();
            ObjectNode currentObject = new ObjectNode();
            currentObject.Name = "Default";

            int triangleCount = 0;
            int vertexCount = 0;

            //           List<Mesh.Group> matGroups = new List<Mesh.Group>();

            List<PositionNormalTextured> vertexList = new List<PositionNormalTextured>();
            List<Vector3d> vertList = new List<Vector3d>();
            List<Vector3d> normList = new List<Vector3d>();
            List<Vector2d> uvList = new List<Vector2d>();

            vertList.Add(new Vector3d());
            normList.Add(new Vector3d());
            uvList.Add(new Vector2d());


            List<int> indexList = new List<int>();
            List<int> attribList = new List<int>();
            List<int[]> applyLists = new List<int[]>();
            List<int> applyListsIndex = new List<int>();
            List<string> materialNames = new List<string>();
            int currentMaterialIndex = -1;
            Material currentMaterial = new Material();
            Group currentGroup = new Group();


            int currentIndex = 0;


            //initialize the default material

            currentMaterial = new Material();
            currentMaterial.Diffuse = Color;
            currentMaterial.Ambient = Color;
            currentMaterial.Specular = Colors.White;
            currentMaterial.SpecularSharpness = 30.0f;
            currentMaterial.Opacity = 1.0f;
            currentMaterial.IsDefault = true;

            //initialize the group
            currentGroup.startIndex = 0;
            currentGroup.indexCount = 0;
            currentGroup.materialIndex = 0;

            string[] lines = data.Split("\n");
            {
                
                foreach(string lineraw in lines)
                {
                    string line = lineraw.Replace("  ", " ");

                    string[] parts = line.Trim().Split(" ");

                    if (parts.Length > 0)
                    {
                        switch (parts[0])
                        {
                            case "mtllib":
                                // We have to pre-load these now in JavaScript, since we can't synchronously load the file and we need file contents to interpret the rest of this file
                                //{
                                //    string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                //    string matFile = path + parts[1];
                                //    LoadMatLib(matFile);
                                //}
                                break;
                            case "usemtl":
                                string materialName = parts[1];
                                if (matLib.ContainsKey(materialName))
                                {
                                    if (currentMaterialIndex == -1 && currentIndex > 0)
                                    {
                                        addMaterial(currentMaterial);
                                        currentMaterialIndex++;
                                    }

                                    if (currentMaterialIndex > -1)
                                    {
                                        currentGroup.indexCount = currentIndex - currentGroup.startIndex;
                                        //      matGroups.Add(currentGroup);
                                        currentObject.DrawGroup.Add(currentGroup);
                                    }

                                    currentMaterialIndex++;

                                    if (matLib.ContainsKey(materialName))
                                    {
                                        currentMaterial = matLib[materialName];


                                        if (textureLib.ContainsKey(materialName))
                                        {
                                            try
                                            {
                                                if (!TextureCache.ContainsKey(textureLib[materialName]))
                                                {
                                                    string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);

                                                    Texture tex = tourDocument.GetCachedTexture2d(path + textureLib[materialName]);
                                                    if (tex != null)
                                                    {
                                                        meshFilenames.Add(textureLib[materialName]);
                                                        TextureCache[textureLib[materialName]] = tex;
                                                    }
                                                }
                                                meshTextures.Add(TextureCache[textureLib[materialName]]);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        addMaterial(currentMaterial);

                                        currentGroup = new Group();
                                        currentGroup.startIndex = currentIndex;
                                        currentGroup.indexCount = 0;
                                        currentGroup.materialIndex = currentMaterialIndex;
                                    }
                                }

                                break;
                            case "v":
                                vertexCount++;
                                if (FlipHandedness)
                                {
                                    vertList.Add(Vector3d.Create(-float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                                }
                                else
                                {
                                    vertList.Add(Vector3d.Create(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                                }
                                break;
                            case "vn":
                                if (FlipHandedness)
                                {
                                    normList.Add(Vector3d.Create(-float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                                }
                                else
                                {
                                    normList.Add(Vector3d.Create(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                                }
                                break;
                            case "vt":
                                uvList.Add(Vector2d.Create(float.Parse(parts[1]), FlipV ? (1 - float.Parse(parts[2])) : float.Parse(parts[2])));
                                break;
                            case "g":
                            case "o":
                                if (objectFound)
                                {
                                    if (currentMaterialIndex > -1)
                                    {
                                        currentGroup.indexCount = currentIndex - currentGroup.startIndex;
                                        //                 matGroups.Add(currentGroup);
                                        currentObject.DrawGroup.Add(currentGroup);
                                        currentGroup = new Group();
                                        currentGroup.startIndex = currentIndex;
                                        currentGroup.indexCount = 0;
                                        currentGroup.materialIndex = currentMaterialIndex;
                                    }
                                    currentObject = new ObjectNode();
                                }

                                objectFound = true;
                                if (parts.Length > 1)
                                {
                                    currentObject.Name = parts[1];
                                }
                                else
                                {
                                    currentObject.Name = "Unnamed";
                                }
                                objects.Add(currentObject);
                                //if (!objectTable.ContainsKey(currentObject.Name))
                                //{
                                //    objectTable.Add(currentObject.Name, currentObject);
                                //}
                                break;
                            case "f":
                                int[] indexiesA = GetIndexies(parts[1]);
                                int[] indexiesB = GetIndexies(parts[2]);
                                int[] indexiesC = GetIndexies(parts[3]);

                                vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesA[0]], normList[indexiesA[2]], uvList[indexiesA[1]]));
                                vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesB[0]], normList[indexiesB[2]], uvList[indexiesB[1]]));
                                vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesC[0]], normList[indexiesC[2]], uvList[indexiesC[1]]));

                                if (FlipHandedness)
                                {
                                    indexList.Add(currentIndex);
                                    indexList.Add(currentIndex + 2);
                                    indexList.Add(currentIndex + 1);
                                }
                                else
                                {
                                    indexList.Add(currentIndex);
                                    indexList.Add(currentIndex + 1);
                                    indexList.Add(currentIndex + 2);

                                }

                                triangleCount++;
                                currentIndex += 3;
                                //bool flip = true;
                                if (parts.Length > 4)
                                {
                                    int partIndex = 4;

                                    while (partIndex < (parts.Length))
                                    {
                                        if (FlipHandedness)
                                        {
                                            indexiesA = GetIndexies(parts[1]);
                                            indexiesC = GetIndexies(parts[partIndex]);
                                            indexiesB = GetIndexies(parts[partIndex - 1]);
                                        }
                                        else
                                        {
                                            indexiesA = GetIndexies(parts[1]);
                                            indexiesB = GetIndexies(parts[partIndex - 1]);
                                            indexiesC = GetIndexies(parts[partIndex]);
                                        }
                                        vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesA[0]], normList[indexiesA[2]], uvList[indexiesA[1]]));
                                        vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesB[0]], normList[indexiesB[2]], uvList[indexiesB[1]]));
                                        vertexList.Add(PositionNormalTextured.CreateUV(vertList[indexiesC[0]], normList[indexiesC[2]], uvList[indexiesC[1]]));


                                        indexList.Add(currentIndex);
                                        indexList.Add(currentIndex + 1);
                                        indexList.Add(currentIndex + 2);
                                        triangleCount++;

                                        currentIndex += 3;
                                        partIndex++;
                                    }
                                }
                                break;
                        }
                    }
                }
            }

            if (!objectFound)
            {
                // add the default object
                objects.Add(currentObject);
            }

            if (currentMaterialIndex == -1 && currentIndex > 0)
            {
                addMaterial(currentMaterial);
                currentMaterialIndex++;
            }

            if (currentMaterialIndex > -1)
            {
                currentGroup.indexCount = (int)(currentIndex - currentGroup.startIndex);
                currentObject.DrawGroup.Add(currentGroup);
            }

            if (normList.Count < 2)
            {
                double degtorag = Math.PI / 180;
                double  creaseAngleRad = (Smooth ? 170.0f * degtorag : 45.0f * degtorag);

                Vector3d[] vertexNormals = CalculateVertexNormalsMerged(vertexList, indexList, creaseAngleRad);
                List<PositionNormalTextured> newVertexList = new List<PositionNormalTextured>();
                int newVertexCount = indexList.Count;

                for (int vertexIndex = 0; vertexIndex < newVertexCount; ++vertexIndex)
                {
                    PositionNormalTextured v = vertexList[indexList[vertexIndex]];
                    v.Normal = vertexNormals[vertexIndex];
                    newVertexList.Add(v);
                }

                vertexList = newVertexList;
            }

            mesh = Mesh.Create(vertexList, indexList);
            ObjectNode rootDummy = new ObjectNode();
            rootDummy.Name = "Root";
            rootDummy.Parent = null;
            rootDummy.Level = -1;
            rootDummy.DrawGroup = null;
            rootDummy.Children = objects;

            Objects = new List<ObjectNode>();
            Objects.Add(rootDummy);


            mesh.setObjects(Objects);

            //List<ObjectNode> objects = new List<ObjectNode>();

            //ObjectNode node = new ObjectNode();
            //node.Name = "Default";
            //node.DrawGroup = matGroups;
            //objects.Add(node);
            //mesh.setObjects(objects);
            //Objects = objects;


            mesh.commitToDevice();

            dirty = false;
            readyToRender = true;
        }

        //private Texture LoadTexture(string filename)
        //{
        //    if (!File.Exists(filename))
        //    {
        //        string newfilename = FindFile(filename);
        //        if (string.IsNullOrEmpty(newfilename))
        //        {
        //            newfilename = FindFileFuzzy(filename);
        //        }

        //        filename = newfilename;
        //    }

        //    if (string.IsNullOrEmpty(filename))
        //    {
        //        return null;
        //    }

        //    return Texture.FromFile(RenderContext.PrepDevice, filename);
        //}

        //public static string FindFileFuzzy(string filename)
        //{
        //    filename = filename.ToLower();
        //    string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
        //    string file = filename.Substring(filename.LastIndexOf("\\") + 1);
        //    if (file.Contains("."))
        //    {
        //        file = file.Substring(0, file.LastIndexOf('.') );
        //    }

        //    string ext = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();

        //    foreach (string f in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        //    {
        //        string fb = f.Substring(f.LastIndexOf("\\") + 1).ToLower();
        //        string fe = ""; 
        //        string ff = "";
        //        if (f.Contains("."))
        //        {
        //            fe = fb.Substring(fb.LastIndexOf(".") + 1);
        //            ff = fb.Substring(0, fb.LastIndexOf("."));
        //        }
        //        if (string.Compare(file, 0, ff, 0, file.Length) == 0 && ((fe == ext) || (fe == "png") || (fe == "tif") || (fe == "tiff") || (fe == "bmp") || (fe == "jpg") || (fe == "jpeg") || (fe == "tga") || (fe == "jfif") || (fe == "rla")))
        //        {
        //            return f;
        //        }
        //    }


        //    //foreach (string dir in Directory.GetDirectories(path))
        //    //{
        //    //    string child = dir + @"\" + file;
        //    //    if (File.Exists(child))
        //    //    {
        //    //        return child;
        //    //    }
        //    //    FindFileFuzzy(child);
        //    //}

        //    return null;
        //}

        //public static string FindFile(string filename)
        //{
        //    string path = filename.Substring(0, filename.LastIndexOf('\\') + 1);
        //    string file = filename.Substring(filename.LastIndexOf("\\") + 1);

        //    foreach (string dir in Directory.GetDirectories(path))
        //    {
        //        string child = dir + @"\" + file;
        //        if (File.Exists(child))
        //        {
        //            return child;
        //        }
        //        FindFile(child);
        //    }

        //    return null;
        //}

        public List<ObjectNode> Objects = new List<ObjectNode>();

        Dictionary<string, Material> matLib = new Dictionary<string, Material>();
        Dictionary<string, string> textureLib = new Dictionary<string, string>();
        void LoadMatLib(string data)
        {
            if (matFileIndex < matFiles.Length)
            {

                string filename = matFiles[matFileIndex++];

                Blob blob = tourDocument.GetFileBlob(filename);
                FileReader chunck = new FileReader();
                chunck.OnLoadEnd = delegate (FileProgressEvent e)
                    {
                        ReadMatLibFromBin(chunck.Result as string);

                        LoadMatLib(data);
                    };
                chunck.ReadAsText(blob);
            }
            else
            {
                ReadObjFromBin(data);
            }
        }

        private void ReadMatLibFromBin(string data)
        {

            

            try
            {
                Material currentMaterial = new Material();
                string materialName = "";

                matLib = new Dictionary<string, Material>();
                textureLib = new Dictionary<string, string>();


                string[] lines = data.Split("\n");
                {

                    foreach (string lineraw in lines)
                    {
                        string line = lineraw;

                        string[] parts = line.Trim().Split(" ");

                        if (parts.Length > 0)
                        {
                            switch (parts[0])
                            {
                                case "newmtl":
                                    if (!string.IsNullOrEmpty(materialName))
                                    {
                                        matLib[materialName] = currentMaterial;
                                    }

                                    currentMaterial = new Material();
                                    currentMaterial.Diffuse = Colors.White;
                                    currentMaterial.Ambient = Colors.White;
                                    currentMaterial.Specular = Colors.Black;
                                    currentMaterial.SpecularSharpness = 30.0f;
                                    currentMaterial.Opacity = 1.0f;
                                    materialName = parts[1];
                                    break;
                                case "Ka":
                                    currentMaterial.Ambient = Color.FromArgb(255, (int)(Math.Min(float.Parse(parts[1]) * 255, 255)), (int)(Math.Min(float.Parse(parts[2]) * 255, 255)), (int)(Math.Min(float.Parse(parts[3]) * 255, 255)));
                                    break;
                                case "map_Kd":
                                    //ENDURE TEXTURES ARE NOT BLACK!    
                                    currentMaterial.Diffuse = Colors.White;

                                    string textureFilename = parts[1];
                                    for (int i = 2; i < parts.Length; i++)
                                    {
                                        textureFilename += " " + parts[i];
                                    }
                                    string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);

                                    textureFilename = textureFilename.Replace("/", "\\");
                                    if (textureFilename.IndexOf("\\") != -1)
                                    {
                                        textureFilename = textureFilename.Substring(textureFilename.LastIndexOf("\\") + 1);
                                    }

                                    //if (File.Exists(path + "\\" + textureFilename))
                                    {
                                        //textureLib.Add(materialName, path + "\\" + textureFilename);
                                        textureLib[materialName] = textureFilename;
                                    }
                                    break;
                                case "Kd":
                                    currentMaterial.Diffuse = Color.FromArgb(255, (int)(Math.Min(float.Parse(parts[1]) * 255, 255)), (int)(Math.Min(float.Parse(parts[2]) * 255, 255)), (int)(Math.Min(float.Parse(parts[3]) * 255, 255)));
                                    break;
                                case "Ks":
                                    currentMaterial.Specular = Color.FromArgb(255, (int)(Math.Min(float.Parse(parts[1]) * 255, 255)), (int)(Math.Min(float.Parse(parts[2]) * 255, 255)), (int)(Math.Min(float.Parse(parts[3]) * 255, 255)));
                                    break;
                                case "d":
                                    // Where does this map?
                                    currentMaterial.Opacity = float.Parse(parts[1]);
                                    break;
                                case "Tr":
                                    // Where does this map?
                                    currentMaterial.Opacity = 1 - float.Parse(parts[1]);
                                    break;

                                case "illum":
                                    // Where does this map?
                                    int illuminationMode = int.Parse(parts[1]);
                                    break;

                                case "sharpness":
                                    currentMaterial.SpecularSharpness = float.Parse(parts[1]);
                                    break;
                                case "Ns":
                                    currentMaterial.SpecularSharpness = 1.0f + 2 * float.Parse(parts[1]);
                                    currentMaterial.SpecularSharpness = Math.Max(10.0f, currentMaterial.SpecularSharpness);
                                    break;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(materialName))
                {
                    matLib[materialName] = currentMaterial;
                }
            }
            catch
            {
            }
        }

        int[] GetIndexies(string data)
        {
            string[] parts = data.Trim().Split('/');
            int[] indecies = new int[3];

            if (string.IsNullOrEmpty(data))
            {
                return indecies;
            }

            if (parts.Length > 0)
            {
                indecies[0] = int.Parse(parts[0]);
            }
            if (parts.Length > 1)
            {
                if (string.IsNullOrEmpty(parts[1]))
                {
                    indecies[1] = 0;
                }
                else
                {
                    indecies[1] = int.Parse(parts[1]);
                }
            }
            if (parts.Length > 2)
            {
                indecies[2] = int.Parse(parts[2]);
            }

            return indecies;
        }

        TourDocument tourDocument = null;

        private void LoadMeshFrom3ds(TourDocument doc, string filename, float scale)
        {
            tourDocument = doc;

            Blob blob = doc.GetFileBlob(filename);
            FileReader chunck = new FileReader();
            chunck.OnLoadEnd = delegate (FileProgressEvent e)
            {
                Read3dsFromBin(new BinaryReader(new Uint8Array(chunck.Result)), scale);

            };
            chunck.ReadAsArrayBuffer(blob);
        }
        private void Read3dsFromBin(BinaryReader br, float scale)
        {
            // Force garbage collection to ensure that unmanaged resources are released.
            // Temporary workaround until unmanaged resource leak is identified

            int i;

            ushort sectionID;
            uint sectionLength;

            string name = "";
            string material = "";
            int triangleCount = 0;
            int vertexCount = 0;
            List<PositionNormalTextured> vertexList = new List<PositionNormalTextured>();
            List<int> indexList = new List<int>();
            List<int> attribList = new List<int>();
            //List<int[]> applyLists = new List<int[]>();
            //List<int> applyListsIndex = new List<int>();
            List<string> materialNames = new List<string>();
            int currentMaterialIndex = -1;
            Material currentMaterial = new Material();
            int attributeID = 0;

            int count = 0;
            UInt16 lastID = 0;
            bool exit = false;
            bool normalMapFound = false;

            float offsetX = 0;
            float offsetY = 0;
            float offsetZ = 0;
            List<ObjectNode> objects = new List<ObjectNode>();
            ObjectNode currentObject = null;
            List<int> objHierarchy = new List<int>();
            List<string> objNames = new List<string>();
            Dictionary<string, ObjectNode> objectTable = new Dictionary<string, ObjectNode>();

            int dummyCount = 0;

            //using (Stream fs = new FileStream(filename, FileMode.Open))
            {
                //   BinaryReader br = new BinaryReader(fs);
                long length = br.Length - 1;


                int startMapIndex = 0;
                int startTriangleIndex = 0;
                while (br.Position < length && !exit) //Loop to scan the whole file
                {
                    sectionID = br.ReadUInt16();
                    sectionLength = br.ReadUInt32();


                    switch (sectionID)
                    {

                        //This section the begining of the file
                        case 0x4d4d:
                            break;

                        // This section marks the edit section containing the 3d models (3d3d get it? very punny!)
                        case 0x3d3d:
                            break;

                        // Object section contains meshes, etc.
                        case 0x4000:
                            name = "";
                            Byte b;
                            do
                            {
                                b = br.ReadByte();
                                if (b > 0)
                                {
                                    name += string.FromCharCode(b); 
                                }

                            } while (b != 0);

                            currentObject = new ObjectNode();
                            currentObject.Name = name;
                            objects.Add(currentObject);
                            if (!objectTable.ContainsKey(currentObject.Name))
                            {
                                objectTable[currentObject.Name] = currentObject;
                            }

                            break;

                        // Marks the start of a mesh section
                        case 0x4100:
                            startMapIndex = vertexList.Count;
                            startTriangleIndex = Math.Floor(indexList.Count / 3);
                            break;

                        // This section has the vertex list.. Maps to Vertext buffer in Direct3d
                        case 0x4110:
                            vertexCount = br.ReadUInt16();

                            for (i = 0; i < vertexCount; i++)
                            {
                                float x = br.ReadSingle() - offsetX;
                                float y = br.ReadSingle() - offsetY;
                                float z = br.ReadSingle() - offsetZ;

                                PositionNormalTextured vert = PositionNormalTextured.Create(x * scale, z * scale, y * scale, 0, 0, 0, 0, 0);
                                vertexList.Add(vert);
                            }
                            break;

                        // This section is a tiangle index list. Maps to Index Buffer in Direct3d
                        case 0x4120:
                            {
                                int triCount = br.ReadUInt16();
                                triangleCount += triCount;

                                for (i = 0; i < triCount; i++)
                                {
                                    int aa = br.ReadUInt16() + startMapIndex;
                                    int bb = br.ReadUInt16() + startMapIndex;
                                    int cc = br.ReadUInt16() + startMapIndex;
                                    indexList.Add(cc);
                                    indexList.Add(bb);
                                    indexList.Add(aa);
                                    UInt16 flags = br.ReadUInt16();
                                }
                            }
                            break;

                        // Material for face from start face to triCount
                        case 0x4130:
                            {
                                material = "";
                                i = 0;
                                byte b1;
                                do
                                {
                                    b1 = br.ReadByte();
                                    if (b1 > 0)
                                    {
                                        material += string.FromCharCode(b1);
                                    }

                                    i++;
                                } while (b1 != 0);
                                int triCount = br.ReadUInt16();
                                int[] applyList = new int[triCount];

                                attributeID = GetMaterialID(material, materialNames);

                                for (i = 0; i < triCount; i++)
                                {
                                    applyList[i] = br.ReadUInt16() + startTriangleIndex;
                                }
                                currentObject.ApplyLists.Add(applyList);
                                currentObject.ApplyListsIndex.Add(attributeID);

                            }
                            break;

                        // Section for UV texture maps
                        case 0x4140:
                            count = br.ReadUInt16();
                            for (i = 0; i < count; i++)
                            {
                                PositionNormalTextured vert = vertexList[startMapIndex + i];
                                Vector2d texCoord = Vector2d.Create(br.ReadSingle(), FlipV ? (1.0f - br.ReadSingle()) : (br.ReadSingle()));
                                vertexList[startMapIndex + i] = PositionNormalTextured.CreateUV(vert.Position, new Vector3d(), texCoord);
                            }
                            break;


                        // Section for Smoothing Groups
                        //case 0x4150:
                        //    count = br.ReadUInt16();
                        //    for (i = 0; i < count; i++)
                        //    {
                        //        CustomVertex.PositionNormalTextured vert = vertexList[startMapIndex + i];
                        //        vertexList[startMapIndex + i] = new CustomVertex.PositionNormalTextured(vert.Position, Vector3d.Create(0,0,0), br.ReadSingle(), FlipV ? (1.0f -  br.ReadSingle() ) : (br.ReadSingle()));
                        //    }
                        //    break;
                        case 0x4160:
                            float[] mat = new float[12];
                            for (i = 0; i < 12; i++)
                            {
                                mat[i] = br.ReadSingle();
                            }
                            //offsetX = mat[9];
                            //offsetY = mat[11];
                            //offsetZ = mat[10];

                            if (objectTable.ContainsKey(name))
                            {
                                objectTable[name].LocalMat = Matrix3d.Create(
                                    mat[0], mat[1], mat[2], 0,
                                    mat[3], mat[4], mat[5], 0,
                                    mat[6], mat[7], mat[8], 0,
                                    mat[9], mat[10], mat[11], 1);

                                objectTable[name].LocalMat.Invert();

                                //objectTable[name].PivotPoint = Vector3d.Create(mat[9]*mat[0],mat[10]*mat[1],mat[11]*mat[2]);
                            }

                            break;
                        // Materials library section
                        case 0xAFFF:
                            break;
                        // Material Name
                        case 0xA000:
                            {
                                string matName = "";
                                i = 0;
                                byte b2;
                                do
                                {
                                    b2 = br.ReadByte();
                                    if (b2 > 0)
                                    {
                                        
                                        matName += string.FromCharCode(b2);
                                    }

                                    i++;
                                } while (b2 != 0);
                                materialNames.Add(matName);

                                if (currentMaterialIndex > -1)
                                {
                                    addMaterial(currentMaterial);
                                }

                                currentMaterialIndex++;

                                currentMaterial = new Material();
                                currentMaterial.Diffuse = Colors.White;
                                currentMaterial.Ambient = Colors.White;
                                currentMaterial.Specular = Colors.Black;
                                currentMaterial.SpecularSharpness = 30.0f;
                                currentMaterial.Opacity = 1.0f;
                            }
                            break;

                        // Ambient color
                        case 0xA010:
                            currentMaterial.Ambient = LoadColorChunk(br);
                            break;

                        // Diffuse color
                        case 0xA020:
                            currentMaterial.Diffuse = LoadColorChunk(br);
                            break;

                        // Specular color
                        case 0xA030:
                            currentMaterial.Specular = LoadColorChunk(br);
                            break;

                        // Specular power
                        case 0xA040:
                            // This is just a reasonable guess at the mapping from percentage to 
                            // specular exponent used by 3D Studio.
                            currentMaterial.SpecularSharpness = 1.0f + 2 * LoadPercentageChunk(br);

                            // Minimum sharpness of 10 enforced here because of bad specular exponents
                            // in ISS model.
                            // TODO: Fix ISS and permit lower specular exponents here
                            currentMaterial.SpecularSharpness = Math.Max(10.0f, currentMaterial.SpecularSharpness);
                            break;

                        //Texture map file 
                        case 0xA200:
                            break;

                        // Texture file name
                        case 0xA300:
                            {
                                string textureFilename = "";
                                i = 0;
                                byte b2;
                                do
                                {
                                    b2 = br.ReadByte();
                                    if (b2 > 0)
                                    {
                                        textureFilename += string.FromCharCode(b2);
                                    }

                                    i++;
                                } while (b2 != 0);
                                string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                try
                                {
                                    Texture tex = tourDocument.GetCachedTexture2d(path + textureFilename);

                                    if (tex != null)
                                    {
                                        meshTextures.Add(tex);
                                        meshFilenames.Add(textureFilename);

                                        // The ISS model has black for the diffuse color; to work around this
                                        // we'll set the diffuse color to white when there's a texture present.
                                        // The correct fix is to modify the 3ds model of ISS.
                                        currentMaterial.Diffuse = Colors.White;
                                    }
                                    else
                                    {
                                        meshTextures.Add(null);
                                    }
                                }
                                catch
                                {
                                    meshTextures.Add(null);
                                }
                            }
                            break;


                        // Bump map
                        case 0xA230:
                            {
                                float percentage = LoadPercentageChunk(br);

                                int nameId = br.ReadUInt16();
                                uint nameBlockLength = br.ReadUInt32();
                                string textureFilename = "";
                                i = 0;
                                byte b2;
                                do
                                {
                                    b2 = br.ReadByte();
                                    if (b2 > 0)
                                    {
                                        textureFilename += string.FromCharCode(b2);
                                    }

                                    i++;
                                } while (b2 != 0);

                                string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                try
                                {
                                    Texture tex = tourDocument.GetCachedTexture2d(path + textureFilename);

                                    if (tex != null)
                                    {
                                        meshNormalMaps.Add(tex);
                                        meshFilenames.Add(textureFilename);

                                        // Indicate that we have a normal map so that we know to generate tangent vectors for the mesh
                                        normalMapFound = true;
                                    }
                                    else
                                    {
                                        meshNormalMaps.Add(null);
                                    }

                                }
                                catch
                                {
                                    meshNormalMaps.Add(null);
                                }
                            }

                            break;

                        // Specular map
                        case 0xA204:
                            {
                                float strength = LoadPercentageChunk(br);

                                int nameId = br.ReadUInt16();
                                uint nameBlockLength = br.ReadUInt32();
                                string textureFilename = "";
                                i = 0;
                                byte b2;
                                do
                                {
                                    b2 = br.ReadByte();
                                    if (b2 > 0)
                                    {
                                        textureFilename += string.FromCharCode(b2);
                                    }

                                    i++;
                                } while (b2 != 0);

                                string path = Filename.Substring(0, Filename.LastIndexOf('\\') + 1);
                                try
                                {
                                    Texture tex = tourDocument.GetCachedTexture2d(path + textureFilename);
                                    if (tex != null)
                                    {
                                        meshSpecularTextures.Add(tex);
                                        meshFilenames.Add(textureFilename);

                                        // Set the current specular color from the specular texture strength
                                        int gray = (int)(255.99f * strength / 100.0f);
                                        currentMaterial.Specular = Color.FromArgb(255, gray, gray, gray);
                                    }
                                    else
                                    {
                                        meshSpecularTextures.Add(null);
                                    }
                                }
                                catch
                                {
                                    meshSpecularTextures.Add(null);
                                }
                            }

                            break;
                        case 0xB000:
                            break;
                        case 0xB002:
                            break;
                        case 0xB010:
                            {
                                name = "";
                                i = 0;
                                byte b1;
                                do
                                {
                                    b1 = br.ReadByte();
                                    if (b1 > 0)
                                    {
                                        name += string.FromCharCode(b1);
                                    }

                                    i++;
                                } while (b1 != 0);
                                int dum1 = (int)br.ReadUInt16();
                                int dum2 = (int)br.ReadUInt16();
                                int level = (int)br.ReadUInt16();

                                if (level == 65535)
                                {
                                    level = -1;
                                }
                                if (name.StartsWith("$"))
                                {
                                    dummyCount++;

                                }
                                else
                                {
                                    objNames.Add(name);
                                }
                                objHierarchy.Add(level);

                                if (objectTable.ContainsKey(name))
                                {

                                    objectTable[name].Level = level;
                                }
                            }
                            break;
                        case 0xB011:
                            {
                                name = "";
                                i = 0;
                                byte b1;
                                do
                                {
                                    b1 = br.ReadByte();
                                    if (b1 > 0)
                                    {
                                        name += string.FromCharCode(b1);
                                    }

                                    i++;
                                } while (b1 != 0);
                                objNames.Add("$$$" + name);
                            }
                            break;
                        case 0xB013:
                            //pivot point
                            float[] points = new float[3];
                            for (i = 0; i < 3; i++)
                            {
                                points[i] = br.ReadSingle();
                            }


                            if (objectTable.ContainsKey(name))
                            {
                                objectTable[name].PivotPoint = Vector3d.Create(-points[0], -points[1], -points[2]);
                            }
                            break;
                        case 0xB020:
                            {
                                float[] pos = new float[8];
                                for (i = 0; i < 8; i++)
                                {
                                    pos[i] = br.ReadSingle();
                                }

                            }
                            break;

                        // If we don't recognize a section then jump over it. Subract the header from the section length
                        default:
                            br.SeekRelative((int)(sectionLength - 6));
                            break;
                    }
                    lastID = sectionID;
                }
                br.Close();
                if (currentMaterialIndex > -1)
                {
                    addMaterial(currentMaterial);
                }
            }

            ////debug

            //for ( i = 0; i < 99; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine(objNames[i]);
            //}

            //foreach (ObjectNode node in objects)
            //{
            //    System.Diagnostics.Debug.WriteLine(node.Name);
            //}

            ////debug




            // Generate vertex normals

            // Vertex normals are computed by averaging the normals of all faces
            // with an angle between them less than the crease angle. By setting
            // the crease angle to 0 degrees, the model will have a faceted appearance.
            // Right now, the smooth flag selects between one of two crease angles,
            // but some smoothing is always applied.
            double degtorag = Math.PI / 180;
            double creaseAngleRad = (Smooth ? 70.0f * degtorag : 45.0f * degtorag);

            Vector3d[] vertexNormals = CalculateVertexNormalsMerged(vertexList, indexList, creaseAngleRad);
            List<PositionNormalTextured> newVertexList = new List<PositionNormalTextured>();
            int newVertexCount = triangleCount * 3;

            for (int vertexIndex = 0; vertexIndex < newVertexCount; ++vertexIndex)
            {
                PositionNormalTextured v = vertexList[indexList[vertexIndex]];
                v.Normal = vertexNormals[vertexIndex];
                newVertexList.Add(v);
            }


            // Use the triangle mesh and material assignments to create a single
            // index list for the mesh.
            List<int> newIndexList = new List<int>();

            foreach (ObjectNode node in objects)
            {
                List<Group> materialGroups = new List<Group>();
                for (i = 0; i < node.ApplyLists.Count; i++)
                {
                    int matId = node.ApplyListsIndex[i];
                    int startIndex = newIndexList.Count;
                    foreach (int triangleIndex in node.ApplyLists[i])
                    {
                        newIndexList.Add((int)(triangleIndex * 3));
                        newIndexList.Add((int)(triangleIndex * 3 + 1));
                        newIndexList.Add((int)(triangleIndex * 3 + 2));
                    }

                    Group group = new Group();
                    group.startIndex = startIndex;
                    group.indexCount = node.ApplyLists[i].Length * 3;
                    group.materialIndex = matId;
                    materialGroups.Add(group);
                }
                node.DrawGroup = materialGroups;
            }

            // Turn objects into tree
            Stack<ObjectNode> nodeStack = new Stack<ObjectNode>();

            List<ObjectNode> nodeTreeRoot = new List<ObjectNode>();

            ObjectNode rootDummy = new ObjectNode();
            rootDummy.Name = "Root";
            rootDummy.Parent = null;
            rootDummy.Level = -1;
            rootDummy.DrawGroup = null;

            int currentLevel = -1;

            nodeStack.Push(rootDummy);
            nodeTreeRoot.Add(rootDummy);

            for (i = 0; i < objHierarchy.Count; i++)
            {
                int level = objHierarchy[i];

                if (level <= currentLevel)
                {
                    // pop out all the nodes to intended parent
                    while (level <= nodeStack.Peek().Level && nodeStack.Count > 1)
                    {
                        nodeStack.Pop();
                    }
                    currentLevel = level;

                }

                if (objNames[i].StartsWith("$$$"))
                {
                    ObjectNode dummy = new ObjectNode();
                    dummy.Name = objNames[i].Replace("$$$", "");
                    dummy.Parent = nodeStack.Peek();
                    dummy.Parent.Children.Add(dummy);
                    dummy.Level = currentLevel = level;
                    dummy.DrawGroup = null;
                    nodeStack.Push(dummy);
                }
                else
                {
                    objectTable[objNames[i]].Level = currentLevel = level;
                    objectTable[objNames[i]].Parent = nodeStack.Peek();
                    objectTable[objNames[i]].Parent.Children.Add(objectTable[objNames[i]]);
                    nodeStack.Push(objectTable[objNames[i]]);
                }
            }

            if (objHierarchy.Count == 0)
            {
                foreach (ObjectNode node in objects)
                {
                    rootDummy.Children.Add(node);
                    node.Parent = rootDummy;
                }
            }


            if (normalMapFound)
            {
                // If we've got a normal map, we want to generate tangent vectors for the mesh

                // Mapping of vertices to geometry is extremely straightforward now, but this could
                // change when a mesh optimization step is introduced.
                List<uint> tangentIndexList = new List<uint>();
                for (uint tangentIndex = 0; tangentIndex < newVertexCount; ++tangentIndex)
                {
                    tangentIndexList.Add(tangentIndex);
                }

                Vector3d[] tangents = CalculateVertexTangents(newVertexList, tangentIndexList, (float)creaseAngleRad);

                // Copy the tangents in the vertex data list
                PositionNormalTexturedTangent[] vertices = new PositionNormalTexturedTangent[newVertexList.Count];
                int vertexIndex = 0;
                foreach (PositionNormalTextured v in newVertexList)
                {
                    PositionNormalTexturedTangent tvertex = new PositionNormalTexturedTangent(v.Position, v.Normal, Vector2d.Create(v.Tu, v.Tv), tangents[vertexIndex]);
                    vertices[vertexIndex] = tvertex;
                    ++vertexIndex;
                }
                mesh = Mesh.CreateTangent(vertices, newIndexList);
            }
            else
            {
                mesh = Mesh.Create(newVertexList, newIndexList);
            }

            Objects = nodeTreeRoot;
            mesh.setObjects(nodeTreeRoot);
            mesh.commitToDevice();

            dirty = false;
            readyToRender = true;
        }

        private void OffsetObjects(List<PositionNormalTextured> vertList, List<ObjectNode> objects, Matrix3d offsetMat, Vector3d offsetPoint)
        {
            foreach (ObjectNode node in objects)
            {
                Matrix3d matLoc = node.LocalMat; //offsetMat *;

                OffsetObjects(vertList, node.Children, matLoc, Vector3d.AddVectors(node.PivotPoint, offsetPoint));

                foreach (Group group in node.DrawGroup)
                {
                    int end = group.startIndex + group.indexCount;
                    for (int i = group.startIndex; i < end; i++)
                    {
                        PositionNormalTextured vert = vertList[i];
                        vert.Position = Vector3d.AddVectors(vert.Position, Vector3d.AddVectors(node.PivotPoint, offsetPoint));
                        vertList[i] = vert;
                    }
                }
            }
        }


        private static int GetMaterialID(string material, List<string> materialNames)
        {
            int index = 0;
            foreach (string mat in materialNames)
            {
                if (mat == material)
                {
                    return index;
                }
                index++;
            }

            return -1;
        }


        // Set up lighting state to account for:
        //   - Light reflected from a nearby planet
        //   - Shadows cast by nearby planets
        public void SetupLighting(RenderContext renderContext)
        {
            Vector3d objPosition = Vector3d.Create(renderContext.World.OffsetX, renderContext.World.OffsetY, renderContext.World.OffsetZ);
            Vector3d objToLight = Vector3d.SubtractVectors(objPosition, renderContext.ReflectedLightPosition);
            Vector3d sunPosition = Vector3d.SubtractVectors(renderContext.SunPosition, renderContext.ReflectedLightPosition);
            double cosPhaseAngle = sunPosition.Length() <= 0.0 ? 1.0 : Vector3d.Dot(objToLight, sunPosition) / (objToLight.Length() * sunPosition.Length());
            float reflectedLightFactor = (float)Math.Max(0.0, cosPhaseAngle);
            reflectedLightFactor = (float)Math.Sqrt(reflectedLightFactor); // Tweak falloff of reflected light
            float hemiLightFactor = 0.0f;

            // 1. Reduce the amount of sunlight when the object is in the shadow of a planet
            // 2. Introduce some lighting due to scattering by the planet's atmosphere if it's
            //    close to the surface.
            double sunlightFactor = 1.0;
            if (renderContext.OccludingPlanetRadius > 0.0)
            {
                double objAltitude = Vector3d.SubtractVectors(objPosition, renderContext.OccludingPlanetPosition).Length() - renderContext.OccludingPlanetRadius;
                hemiLightFactor = (float)Math.Max(0.0, Math.Min(1.0, 1.0 - (objAltitude / renderContext.OccludingPlanetRadius) * 300));
                reflectedLightFactor *= (1.0f - hemiLightFactor);

                // Compute the distance from the center of the object to the line between the sun and occluding planet
                // We're assuming that the radius of the object is very small relative to Earth;
                // for large objects the amount of shadow will vary, and we should use circular
                // eclipse shadows.
                Vector3d sunToPlanet = Vector3d.SubtractVectors(renderContext.OccludingPlanetPosition, renderContext.SunPosition);
                Vector3d objToPlanet = Vector3d.SubtractVectors(renderContext.OccludingPlanetPosition, objPosition);

                Vector3d hemiLightDirection = Vector3d.Create(-objToPlanet.X, -objToPlanet.Y, -objToPlanet.Z);
                hemiLightDirection.Normalize();
                renderContext.HemisphereLightUp = hemiLightDirection;

                Vector3d objToSun = Vector3d.SubtractVectors(renderContext.SunPosition, objPosition);
                double sunPlanetDistance = sunToPlanet.Length();
                double t = -Vector3d.Dot(objToSun, sunToPlanet) / (sunPlanetDistance * sunPlanetDistance);
                if (t > 1.0)
                {
                    // Object is on the side of the planet opposite the sun, so a shadow is possible

                    // Compute the position of the object projected onto the shadow axis
                    Vector3d shadowAxisPoint = Vector3d.AddVectors(renderContext.SunPosition, Vector3d.MultiplyScalar(sunToPlanet, t));

                    // d is the distance to the shadow axis
                    double d = Vector3d.SubtractVectors(shadowAxisPoint, objPosition).Length();

                    // s is the distance from the sun along the shadow axis
                    double s = Vector3d.SubtractVectors(shadowAxisPoint, renderContext.SunPosition).Length();

                    // Use the sun's radius to accurately compute the penumbra and umbra cones
                    const double solarRadius = 0.004645784; // AU
                    double penumbraRadius = renderContext.OccludingPlanetRadius + (t - 1.0) * (renderContext.OccludingPlanetRadius + solarRadius);
                    double umbraRadius = renderContext.OccludingPlanetRadius + (t - 1.0) * (renderContext.OccludingPlanetRadius - solarRadius);

                    if (d < penumbraRadius)
                    {
                        // The object is inside the penumbra, so it is at least partly shadowed
                        double minimumShadow = 0.0;
                        if (umbraRadius < 0.0)
                        {
                            // No umbra at this point; degree of shadowing is limited because the
                            // planet doesn't completely cover the sun even when the object is positioned
                            // exactly on the shadow axis.
                            double occlusion = Math.Pow(1.0 / (1.0 - umbraRadius), 2.0);
                            umbraRadius = 0.0;
                            minimumShadow = 1.0 - occlusion;
                        }

                        // Approximate the amount of shadow with linear interpolation. The accurate
                        // calculation involves computing the area of the intersection of two circles.
                        double u = Math.Max(0.0, umbraRadius);
                        sunlightFactor = Math.Max(minimumShadow, (d - u) / (penumbraRadius - u));

                        int gray = (int)(255.99f * sunlightFactor);
                        renderContext.SunlightColor = Color.FromArgb(255, gray, gray, gray);

                        // Reduce sky-scattered light as well
                        hemiLightFactor *= (float)sunlightFactor;
                    }
                }
            }

            renderContext.ReflectedLightColor = Color.FromArgb(255, (int)(renderContext.ReflectedLightColor.R * reflectedLightFactor),
                                                                               (int)(renderContext.ReflectedLightColor.G * reflectedLightFactor),
                                                                               (int)(renderContext.ReflectedLightColor.B * reflectedLightFactor));
            renderContext.HemisphereLightColor = Color.FromArgb(255, (int)(renderContext.HemisphereLightColor.R * hemiLightFactor),
                                                                               (int)(renderContext.HemisphereLightColor.G * hemiLightFactor),
                                                                               (int)(renderContext.HemisphereLightColor.B * hemiLightFactor));
        }

        public const int MAX_VERTICES = 8000;
        public const int MAX_POLYGONS = 8000;



        public bool ISSLayer = false;
        bool readyToRender = false;
        public void Render(RenderContext renderContext, float opacity)
        {
            if (!readyToRender)
            {
                return;
            }

            if (dirty && !(ISSLayer))
            {
                Reload();
            }
            Matrix3d oldWorld = renderContext.World;

            Vector3d offset = mesh.BoundingSphere.Center;
            double unitScale = 1.0f;
            if (mesh.BoundingSphere.Radius > 0.0f)
            {
                unitScale = 1.0f / mesh.BoundingSphere.Radius;
            }
            renderContext.World = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.RotationY(Math.PI),Matrix3d.Translation(Vector3d.Create(-offset.X, -offset.Y, -offset.Z))), Matrix3d.Scaling(unitScale, unitScale, unitScale)), oldWorld);

            Matrix3d worldView = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
            Vector3d v = worldView.Transform(Vector3d.Empty);
            double scaleFactor = Math.Sqrt(worldView.M11 * worldView.M11 + worldView.M22 * worldView.M22 + worldView.M33 * worldView.M33) / unitScale;
            double dist = v.Length();
            double radius = scaleFactor;

            // Calculate pixelsPerUnit which is the number of pixels covered
            // by an object 1 AU at the distance of the planet center from
            // the camera. This calculation works regardless of the projection
            // type.
            int viewportHeight = (int)renderContext.Height;
            double p11 = renderContext.Projection.M11;
            double p34 = renderContext.Projection.M34;
            double p44 = renderContext.Projection.M44;


            double w = Math.Abs(p34) * dist + p44;
            float pixelsPerUnit = (float)(p11 / w) * viewportHeight;
            float radiusInPixels = (float)(radius * pixelsPerUnit);
            if (radiusInPixels < 0.5f)
            {
                // Too small to be visible; skip rendering
             //todo   return;
            }

            // These colors can be modified by shadows, distance from planet, etc. Restore
            // the original values after rendering.
            Color savedSunlightColor = renderContext.SunlightColor;
            Color savedReflectedColor = renderContext.ReflectedLightColor;
            Color savedHemiColor = renderContext.HemisphereLightColor;

            if (Settings.Current.SolarSystemLighting)
            {
                SetupLighting(renderContext);
                if (!UseCurrentAmbient)
                {
                    renderContext.AmbientLightColor = Color.FromArgb(255, 11, 11, 11);
                }
            }
            else
            {
                // No lighting: set ambient light to white and turn off all other light sources
                renderContext.SunlightColor = Colors.Black;
                renderContext.ReflectedLightColor = Colors.Black;
                renderContext.HemisphereLightColor = Colors.Black;
                renderContext.AmbientLightColor = Colors.White;
            }


            if (mesh == null)
            {
                return;
            }

            ModelShader.MinLightingBrightness = .1f;

            //renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
            //renderContext.BlendMode = BlendMode.Alpha;

            int count = meshMaterials.Count;

            mesh.beginDrawing(renderContext);
            if (count > 0)
            {
                for (int i = 0; i < meshMaterials.Count; i++)
                {
                    if (meshMaterials[i].IsDefault)
                    {
                        Material mat = meshMaterials[i];
                        mat.Diffuse = Color;
                        mat.Ambient = Color;
                        meshMaterials[i] = mat;
                    }
                    // Set the material and texture for this subset
                    renderContext.SetMaterial(meshMaterials[i], meshTextures[i], meshSpecularTextures[i], meshNormalMaps[i], opacity);
                    if (mesh.vertexBuffer != null)
                    {
                        ModelShader.Use(renderContext, mesh.vertexBuffer.VertexBuffer, mesh.indexBuffer.Buffer, meshTextures[i]!= null ? meshTextures[i].Texture2d : null, opacity, false,32);
                    }
                    else
                    {
                        ModelShader.Use(renderContext, mesh.tangentVertexBuffer.VertexBuffer, mesh.indexBuffer.Buffer, meshTextures[i] != null ? meshTextures[i].Texture2d : null, opacity, false,44);

                    }
                    renderContext.PreDraw();
                    //todo                   renderContext.setSamplerState(0, renderContext.WrapSampler);
                    mesh.drawSubset(renderContext, i);
                }
            }
            else
            {
                renderContext.PreDraw();
                for (int i = 0; i < meshTextures.Count; i++)
                {
                    //todo
                    //var key = new PlanetShaderKey(PlanetSurfaceStyle.Diffuse, false, 0);
                    //renderContext.SetupPlanetSurfaceEffect(key, 1.0f);
                    if (meshTextures[i] != null)
                    {
                        renderContext.MainTexture = meshTextures[i];
                        if (mesh.vertexBuffer != null)
                        {
                            ModelShader.Use(renderContext, mesh.vertexBuffer.VertexBuffer, mesh.indexBuffer.Buffer, meshTextures[i] != null ? meshTextures[i].Texture2d : null, opacity, false, 32);
                        }
                        else
                        {
                            ModelShader.Use(renderContext, mesh.tangentVertexBuffer.VertexBuffer, mesh.indexBuffer.Buffer, meshTextures[i] != null ? meshTextures[i].Texture2d : null, opacity, false, 44);

                        }
                    }
                    renderContext.PreDraw();
                    mesh.drawSubset(renderContext, i);
                }
            }




            //todo
            //renderContext.setSamplerState(0, renderContext.ClampSampler);

            //renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
            renderContext.World = oldWorld;



            renderContext.SunlightColor = savedSunlightColor;
            renderContext.ReflectedLightColor = savedReflectedColor;
            renderContext.HemisphereLightColor = savedHemiColor;
            renderContext.AmbientLightColor = Colors.Black;




        }

        public bool UseCurrentAmbient = false;

        bool dirty = true;

        static void DisposeTextureList(List<Texture> textures)
        {
            if (textures != null)
            {
                for (int i = 0; i < textures.Count; ++i)
                {
                    if (textures[i] != null)
                    {
                        textures[i].Dispose();
                        textures[i] = null;
                    }
                }

                textures.Clear();
            }
        }

        public void Dispose()
        {
            if (mesh != null)
            {
                mesh.Dispose();
                mesh = null;
            }

            foreach (string key in TextureCache.Keys)
            {
                Texture tex = TextureCache[key];
                if (tex != null)
                {
                    tex.Dispose();
                }
            }
            TextureCache.Clear();

            DisposeTextureList(meshTextures);
            DisposeTextureList(meshSpecularTextures);
            DisposeTextureList(meshNormalMaps);

            meshMaterials.Clear();
            dirty = true;
        }
    }

    public class ObjectNode
    {
        public string Name;
        public int Level = -1;
        public List<ObjectNode> Children = new List<ObjectNode>();
        public ObjectNode Parent;
        public bool Enabled = true;
        public Vector3d PivotPoint;
        public Matrix3d LocalMat;
        public List<Group> DrawGroup = new List<Group>();
        public List<int[]> ApplyLists = new List<int[]>();
        public List<int> ApplyListsIndex = new List<int>();
        public ObjectNode()
        {
        }

    }


    public class Object3dLayerUI : LayerUI
    {
        Object3dLayer layer = null;
        bool opened = true;

        public Object3dLayerUI(Object3dLayer layer)
        {
            this.layer = layer;
        }
        IUIServicesCallbacks callbacks = null;

        public override void SetUICallbacks(IUIServicesCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            List<LayerUITreeNode> nodes = new List<LayerUITreeNode>();
            if (layer.object3d.Objects.Count > 0 && layer.object3d.Objects[0].Children != null)
            {
                LoadTree(nodes, layer.object3d.Objects[0].Children);
            }
            return nodes;
        }

        void LoadTree(List<LayerUITreeNode> nodes, List<ObjectNode> children)
        {
            foreach (ObjectNode child in children)
            {
                LayerUITreeNode node = new LayerUITreeNode();
                node.Name = child.Name;
                node.Tag = child;
                node.Checked = child.Enabled;
                node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                node.NodeChecked += new LayerUITreeNodeCheckedDelegate(node_NodeChecked);
                nodes.Add(node);
                LoadTree(node.Nodes, child.Children);
            }
        }


        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
            ObjectNode child = (ObjectNode)node.Tag;

            if (child != null)
            {
                child.Enabled = newState;
            }
        }



        void node_NodeSelected(LayerUITreeNode node)
        {
            if (callbacks != null)
            {
                ObjectNode child = (ObjectNode)node.Tag;

                Dictionary<String, String> rowData = new Dictionary<string, string>();

                rowData["Name"] = child.Name;
                rowData["Pivot.X"] = child.PivotPoint.X.ToString();
                rowData["Pivot.Y"] = child.PivotPoint.Y.ToString();
                rowData["Pivot.Z"] = child.PivotPoint.Z.ToString();
                callbacks.ShowRowData(rowData);

                //Object3dLayer.sketch.Clear();
                //Object3dLayer.sketch.AddLine(Vector3d.Create(0, 0, 0), Vector3d.Create(child.PivotPoint.X,-child.PivotPoint.Z,child.PivotPoint.Y));
            }
        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return base.GetNodeContextMenu(node);
        }
    }
}
