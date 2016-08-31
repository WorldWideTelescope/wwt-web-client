using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public enum OverlayAnchor { Sky = 0, Screen = 1 };
    public enum AudioType { Music = 0, Voice = 1 };

    public abstract class Overlay
    {
        public static OverlayAnchor DefaultAnchor = OverlayAnchor.Screen;

        public const string ClipboardFormat = "WorldWideTelescope.Overlay";
        public bool isDynamic = false;
        protected bool isDesignTimeOnly = false;
        string name = "";
        public static int NextId = 11231;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public const double RC = 3.1415927 / 180;
        //todo no Guid in jscript.. we should only be reading and not creating so we are ok for now
        public string Id = (NextId++).ToString();//Guid.NewGuid().ToString();

        TourStop owner = null;

        public TourStop Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public int ZOrder
        {
            get
            {
                int index = 0;
                foreach (Overlay item in owner.Overlays)
                {
                    if (item == this)
                    {
                        break;
                    }
                    index++;
                }
                return index;
            }
        }

        private string url = "";

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string linkID = "";

        public string LinkID
        {
            get { return linkID; }
            set { linkID = value; }
        }

        virtual public void Play()
        {
        }


        virtual public void Pause()
        {
        }


        virtual public void Stop()
        {
        }


        virtual public void Seek(double time)
        {
        }

        Matrix3d domeMatrix = Matrix3d.Identity;

        double domeMatX = 0;
        double domeMatY = 0;
        double domeAngle = 0;

        public Vector3d MakePosition(double centerX, double centerY, double offsetX, double offsetY, double angle)
        {
            centerX -= 960;
            centerY -= 558;

            Vector3d point = Vector3d.Create(centerX + offsetX, centerY + offsetY, 1347);

            if (domeMatX != 0 || domeMatY != 0 || domeAngle != angle)
            {
                domeMatX = centerX;
                domeMatY = centerY;
                domeMatrix = Matrix3d.Translation(Vector3d.Create(-centerX, -centerY, 0));
                domeMatrix.Multiply(Matrix3d.RotationZ((double)(angle / 180 * Math.PI)));
                domeMatrix.Multiply(Matrix3d.Translation(Vector3d.Create(centerX, centerY, 0)));
            }
            point = Vector3d.TransformCoordinate(point, domeMatrix);

            return point;

        }

        virtual public void Draw3D(RenderContext renderContext, bool designTime)
        {
            if (RenderContext.UseGl)
            {
                if (texture == null || isDynamic)
                {
                    InitializeTexture();
                }

                if (!isDesignTimeOnly || designTime)
                {
                    InitiaizeGeometry();

                    UpdateRotation();

                    //todo call proper drawing for this
                    // Sprite2d.Draw(renderContext, points, points.Length, texture, TriangleStrip ? SharpDX.Direct3D.PrimitiveTopology.TriangleStrip : SharpDX.Direct3D.PrimitiveTopology.TriangleList, transparancy);
                }
            }
            else
            {
             
            }
        }

        virtual public void CleanUp()
        {
            if (texture != null)
            {
                texture = null;
            }
            texture2d = null;
        }

        virtual public void InitializeTexture()
        {
        }

        protected PositionColoredTextured[] points = null;

        virtual public void CleanUpGeometry()
        {
            currentRotation = 0;
            points = null;
        }

        virtual public void InitiaizeGeometry()
        {
            if (points == null)
            {

                currentRotation = 0;
                points = new PositionColoredTextured[4];
                points[0] = new PositionColoredTextured();
                points[0].Position = MakePosition(X, Y, -Width / 2, -Height / 2, RotationAngle);
                points[0].Tu = 0;
                points[0].Tv = 0;
                points[0].Color = Color;

                points[1] = new PositionColoredTextured();
                points[1].Position = MakePosition(X, Y, Width / 2, -Height / 2, RotationAngle);
                points[1].Tu = 1;
                points[1].Tv = 0;
                points[1].Color = Color;

                points[2] = new PositionColoredTextured();
                points[2].Position = MakePosition(X, Y, -Width / 2, Height / 2, RotationAngle);
                points[2].Tu = 0;
                points[2].Tv = 1;
                points[2].Color = Color;

                points[3] = new PositionColoredTextured();
                points[3].Position = MakePosition(X, Y, Width / 2, Height / 2, RotationAngle);
                points[3].Tu = 1;
                points[3].Tv = 1;
                points[3].Color = Color;
            }
        }

        virtual public void UpdateRotation()
        {


        }
        // Animation Support
        bool animate;

        public bool Animate
        {
            get { return animate; }
            set
            {
                if (animate != value)
                {
                    animate = value;

                    if (animate)
                    {
                        endX = x;
                        endY = y;
                        endRotationAngle = rotationAngle;
                        endColor = color;
                        endWidth = width;
                        endHeight = height;
                        CleanUpGeometry();
                    }
                    else
                    {
                        endX = x = X;
                        endY = y = Y;
                        endRotationAngle = rotationAngle = RotationAngle;
                        endColor = color = Color;
                        endWidth = width = Width;
                        endHeight = height = Height;
                        CleanUpGeometry();
                        tweenFactor = 0;
                    }
                }
            }
        }
        double tweenFactor = 0;

        public double TweenFactor
        {
            get
            {
                return tweenFactor;
            }
            set
            {
                if (!animate)
                {
                    tweenFactor = 0;
                }
                else
                {
                    if (tweenFactor != value)
                    {
                        tweenFactor = value;
                        CleanUpGeometry();
                    }
                }
            }
        }

        double endX;
        double endY;
        double endOpacity;
        Color endColor = new Color();
        double endWidth;
        double endHeight;
        double endRotationAngle;



        // End Animation Support


        OverlayAnchor anchor = OverlayAnchor.Screen;

        public OverlayAnchor Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        public Vector2d Position
        {
            get
            {
                return Vector2d.Create(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        private double x;



        public double X
        {
            get { return (x * (1 - tweenFactor)) + (endX * tweenFactor); }
            set
            {
                if (tweenFactor < .5f)
                {
                    if (x != value)
                    {
                        x = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endX != value)
                    {
                        endX = value;
                        CleanUpGeometry();
                    }
                }
            }
        }
        private double y;


        public double Y
        {
            get { return (y * (1 - tweenFactor)) + (endY * tweenFactor); }
            set
            {
                if (tweenFactor < .5f)
                {
                    if (y != value)
                    {
                        y = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endY != value)
                    {
                        endY = value;
                        CleanUpGeometry();
                    }
                }
            }
        }
        private double width;


        public double Width
        {
            get { return (width * (1 - tweenFactor)) + (endWidth * tweenFactor); }
            set
            {
                if (value < 5 && value != 0)
                {
                    value = 5;
                }

                if (tweenFactor < .5f)
                {
                    if (width != value)
                    {
                        width = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endWidth != value)
                    {
                        endWidth = value;
                        CleanUpGeometry();
                    }
                }
            }
        }

        private double height;


        public double Height
        {
            get { return (height * (1 - tweenFactor)) + (endHeight * tweenFactor); }
            set
            {
                if (value < 5 && value != 0)
                {
                    value = 5;
                }

                if (tweenFactor < .5f)
                {
                    if (height != value)
                    {
                        height = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endHeight != value)
                    {
                        endHeight = value;
                        CleanUpGeometry();
                    }
                }
            }
        }

        private Color color = Colors.White;


        public virtual Color Color
        {
            get
            {
                int red = (int)(((double)color.R * (1f - tweenFactor)) + ((double)endColor.R * tweenFactor));
                int green = (int)(((double)color.G * (1f - tweenFactor)) + ((double)endColor.G * tweenFactor));
                int blue = (int)(((double)color.B * (1f - tweenFactor)) + ((double)endColor.B * tweenFactor));
                int alpha = (int)(((double)color.A * (1f - tweenFactor)) + ((double)endColor.A * tweenFactor));
                return Color.FromArgb((byte)Math.Max(0, Math.Min(255, alpha)), (byte)Math.Max(0, Math.Min(255, red)), (byte)Math.Max(0, Math.Min(255, green)), (byte)Math.Max(0, Math.Min(255, blue)));
            }
            set
            {
                if (tweenFactor < .5f)
                {
                    if (color != value)
                    {
                        color = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endColor != value)
                    {
                        endColor = value;
                        CleanUpGeometry();
                    }
                }
            }
        }

        private double opacity = .5f;


        public double Opacity
        {
            get
            {
                return (double)Color.A / 255.0f;
            }
            set
            {
                Color col = Color;
                this.Color = Color.FromArgb((byte)Math.Min(255, (int)(value * 255f)), col.R, col.G, col.B);
                opacity = value;
            }
        }

        double rotationAngle = 0;
        protected double currentRotation = 0;


        public double RotationAngle
        {
            get { return (rotationAngle * (1 - tweenFactor)) + (endRotationAngle * tweenFactor); }
            set
            {
                if (tweenFactor < .5f)
                {
                    if (rotationAngle != value)
                    {
                        rotationAngle = value;
                        CleanUpGeometry();
                    }
                }
                else
                {
                    if (endRotationAngle != value)
                    {
                        endRotationAngle = value;
                        CleanUpGeometry();
                    }
                }
            }
        }

        protected ImageElement texture = null;
        protected Texture texture2d = null;

        virtual public bool HitTest(Vector2d pntTest)
        {
            Vector2d[] tempPoints = new Vector2d[1];
            tempPoints[0] = Vector2d.Create(pntTest.X, pntTest.Y);

            Matrix2d mat = Matrix2d.RotateAt(-RotationAngle / 180 * Math.PI, Vector2d.Create(X, Y));
            mat.TransformPoints(tempPoints);

            Rectangle rect = Rectangle.Create((X - (Width / 2)), (Y - (Height / 2)), Width, Height);

            return rect.Contains(tempPoints[0]);





            //todo this needs to be translated to script#

            //Matrix3d mat = new Matrix3d();
            //mat.RotateAt(new Quaternion(new Vector3D(0,0,1), -RotationAngle), new Point3D(X , Y , 0 ));

            //Point3D tempPoint = new Point3D(pntTest.X, pntTest.Y, 0);

            //tempPoint  = mat.Transform(tempPoint);

            //Rect rect = new Rect((X-(Width/2)), (Y-(Height/2)), Width, Height);
            //if (rect.Contains(new Point(tempPoint.X,tempPoint.Y)))
            //{
            //    return true;
            //}
            //return false;
        }


        Rectangle bounds;

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        private InterpolationType interpolationType = InterpolationType.DefaultV;

        public InterpolationType InterpolationType
        {
            get { return interpolationType; }
            set { interpolationType = value; }
        }


        public virtual void SaveToXml(XmlTextWriter xmlWriter, bool saveKeys)
        {
            xmlWriter.WriteStartElement("Overlay");
            xmlWriter.WriteAttributeString("Id", Id);
            xmlWriter.WriteAttributeString("Type", GetTypeName());
            xmlWriter.WriteAttributeString("Name", Name);
            xmlWriter.WriteAttributeString("X", x.ToString());
            xmlWriter.WriteAttributeString("Y", y.ToString());
            xmlWriter.WriteAttributeString("Width", width.ToString());
            xmlWriter.WriteAttributeString("Height", height.ToString());
            xmlWriter.WriteAttributeString("Rotation", rotationAngle.ToString());
            xmlWriter.WriteAttributeString("Color", color.Save());
            xmlWriter.WriteAttributeString("Url", url);
            xmlWriter.WriteAttributeString("LinkID", linkID);
            xmlWriter.WriteAttributeString("Animate", animate.ToString());
            if (animate)
            {
                xmlWriter.WriteAttributeString("EndX", endX.ToString());
                xmlWriter.WriteAttributeString("EndY", endY.ToString());
                xmlWriter.WriteAttributeString("EndWidth", endWidth.ToString());
                xmlWriter.WriteAttributeString("EndHeight", endHeight.ToString());
                xmlWriter.WriteAttributeString("EndRotation", endRotationAngle.ToString());
                xmlWriter.WriteAttributeString("EndColor", endColor.Save());
                xmlWriter.WriteAttributeString("InterpolationType", Enums.ToXml("InterpolationType", (int)interpolationType));
            }
            xmlWriter.WriteAttributeString("Anchor", Enums.ToXml("OverlayAnchor", (int)anchor));


            this.WriteOverlayProperties(xmlWriter);

            // todo add back for timeline tours
            //if (AnimationTarget != null && saveKeys)
            //{
            //    AnimationTarget.SaveToXml(xmlWriter);
            //}

            xmlWriter.WriteEndElement();
        }

        public virtual string GetTypeName()
        {
            return "TerraViewer.Overlay";
        }

        public virtual void AddFilesToCabinet(FileCabinet fc)
        {
            
        }

        public virtual void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            
        }


        internal static Overlay FromXml(TourStop owner, XmlNode overlay)
        {
            if (overlay.Attributes == null)
            {
                return null;
            }

            if (overlay.Attributes.GetNamedItem("Type") == null)
            {
                return null;
            }
            string overlayClassName = overlay.Attributes.GetNamedItem("Type").Value.ToString();

            string overLayType = overlayClassName.Replace("TerraViewer.", "");
            Overlay newOverlay = null;

            switch (overLayType)
            {
                case "AudioOverlay":
                    newOverlay = new AudioOverlay();
                    break;
                case "BitmapOverlay":
                    newOverlay = new BitmapOverlay();
                    break;
                case "FlipBookOverlay":
                    newOverlay = new FlipbookOverlay();
                    break;
                case "ShapeOverlay":
                    newOverlay = new ShapeOverlay();
                    break;
                case "TextOverlay":
                    newOverlay = new TextOverlay();
                    break;
                default:
                    return null;
            }

            newOverlay.owner = owner;
            newOverlay.InitOverlayFromXml(overlay);
            return newOverlay;
        }

        private void InitOverlayFromXml(XmlNode node)
        {
            Id = node.Attributes.GetNamedItem("Id").Value;
            Name = node.Attributes.GetNamedItem("Name").Value;
            x = double.Parse(node.Attributes.GetNamedItem("X").Value);
            y = double.Parse(node.Attributes.GetNamedItem("Y").Value);
            width = double.Parse(node.Attributes.GetNamedItem("Width").Value);
            height = double.Parse(node.Attributes.GetNamedItem("Height").Value);
            rotationAngle = double.Parse(node.Attributes.GetNamedItem("Rotation").Value);
            color = Color.Load(node.Attributes.GetNamedItem("Color").Value);
            if (node.Attributes.GetNamedItem("Url") != null)
            {
                Url = node.Attributes.GetNamedItem("Url").Value;
            }

            if (node.Attributes.GetNamedItem("LinkID") != null)
            {
                LinkID = node.Attributes.GetNamedItem("LinkID").Value;
            }

            if (node.Attributes.GetNamedItem("Animate") != null)
            {
                animate = bool.Parse(node.Attributes.GetNamedItem("Animate").Value);
                if (animate)
                {
                    endX = double.Parse(node.Attributes.GetNamedItem("EndX").Value);
                    endY = double.Parse(node.Attributes.GetNamedItem("EndY").Value);
                    endColor = Color.Load(node.Attributes.GetNamedItem("EndColor").Value);
                    endWidth = double.Parse(node.Attributes.GetNamedItem("EndWidth").Value);
                    endHeight = double.Parse(node.Attributes.GetNamedItem("EndHeight").Value);
                    endRotationAngle = double.Parse(node.Attributes.GetNamedItem("EndRotation").Value);
                    if (node.Attributes.GetNamedItem("InterpolationType") != null)
                    {
                        InterpolationType = (InterpolationType)Enums.Parse("InterpolationType", node.Attributes.GetNamedItem("InterpolationType").Value);
                    } 
                }
            }

            InitializeFromXml(node);
        }

        public virtual void InitializeFromXml(XmlNode node)
        {

        }

        public override string ToString()
        {
            return this.Name;
        }

    }
    public class BitmapOverlay : Overlay
    {

        public override string GetTypeName()
        {
            return "TerraViewer.BitmapOverlay";
        }

        string filename;

        public BitmapOverlay()
        {

        }

        public static BitmapOverlay Create(TourStop owner, System.Html.Data.Files.File file )
        {
            //todo figure out how to load local files into clound and to cabinet
            BitmapOverlay temp = new BitmapOverlay();

            temp.Owner = owner;
            // to make directory and guid filename in tour temp dir.
            temp.filename = file.Name;

            temp.Name = owner.GetNextDefaultName("Image");
            temp.X = 0;
            temp.Y = 0;
            owner.Owner.AddCachedFile(file.Name, file);

            return temp;
        }

        public BitmapOverlay Copy(TourStop owner)
        {
            BitmapOverlay newBmpOverlay = new BitmapOverlay();
            newBmpOverlay.Owner = owner;
            newBmpOverlay.filename = this.filename;
            newBmpOverlay.X = this.X;
            newBmpOverlay.Y = this.Y;
            newBmpOverlay.Width = this.Width;
            newBmpOverlay.Height = this.Height;
            newBmpOverlay.Color = this.Color;
            newBmpOverlay.Opacity = this.Opacity;
            newBmpOverlay.RotationAngle = this.RotationAngle;
            newBmpOverlay.Name = this.Name + " - Copy";

            return newBmpOverlay;
        }

        public override void CleanUp()
        {
            texture = null;
            if (texture2d != null)
            {
                texture2d.CleanUp();
                texture2d = null;
            }

        }

        bool textureReady = false;
        public override void InitializeTexture()
        {
            try
            {
                if (RenderContext.UseGl)
                {
                    texture2d = Owner.Owner.GetCachedTexture2d(filename);
                    textureReady = true;                 
                }
                else
                {
                    texture = Owner.Owner.GetCachedTexture(filename, delegate { textureReady = true; });

                }
            }
            catch
            {

            }
        }

        private ImageElement imageBrush;
        private Sprite2d sprite = new Sprite2d();

        public override void Draw3D(RenderContext renderContext, bool designTime)
        {
            if (RenderContext.UseGl)
            {
                if (texture2d == null)
                {
                    InitializeTexture();
                }

                if (Width == 0 && Height == 0)
                {
                    Width = texture2d.ImageElement.Width;
                    Height = texture2d.ImageElement.Height;
                }


                InitiaizeGeometry();

                UpdateRotation();

                    //todo call proper drawing for this
                sprite.Draw(renderContext, points, points.Length, texture2d, true, 1);
            }
            else
            {

                if (texture == null)
                {
                    InitializeTexture();
                }

                if (!textureReady)
                {
                    return;
                }

                if (Width == 0 && Height == 0)
                {
                    Width = texture.Width;
                    Height = texture.Height;
                }

                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();

                ctx.Translate(X, Y);
                ctx.Rotate(RotationAngle * RC);
                ctx.Alpha = Opacity;
                ctx.DrawImage(texture, -Width / 2, -Height / 2, Width, Height);
                ctx.Restore();
            }
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            fc.AddFile(Owner.Owner.WorkingDirectory + filename, Owner.Owner.GetFileBlob(filename));
        }

        public override void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Bitmap");
            xmlWriter.WriteAttributeString("Filename", filename);
            xmlWriter.WriteEndElement();
        }

        public override void InitializeFromXml(XmlNode node)
        {
            XmlNode bitmap = Util.SelectSingleNode(node, "Bitmap");
            filename = bitmap.Attributes.GetNamedItem("Filename").Value;
        }
    }

    public class TextOverlay : Overlay
    {
        public override string GetTypeName()
        {
            return "TerraViewer.TextOverlay";
        }

        public TextObject TextObject;
        public override Color Color
        {
            get
            {
                return base.Color;
                //return TextObject.ForgroundColor;
            }
            set
            {
                if (TextObject.ForegroundColor != value)
                {
                    TextObject.ForegroundColor = value;
                    base.Color = value;
                    CleanUp();
                }
            }
        }

        public TextOverlay()
        {
        }

        public static TextOverlay Create(TextObject textObject)
        {
            TextOverlay to = new TextOverlay();
            to.TextObject = textObject;
            to.CalculateTextSize();
            return to;
        }

        //public static TextOverlay(Canvas canvas, TextObject textObject)
        //{
        //    this.canvas = canvas;
        //    this.TextObject = textObject;
        //    this.Name = textObject.Text.Split(new char[] { '\r', '\n' })[0];
        //    X = 0;
        //    Y = 0;

        //}

        Sprite2d sprite = new Sprite2d();

        public override void Draw3D(RenderContext renderContext, bool designTime)
        {
            if (RenderContext.UseGl)
            {
                InitializeTexture();
                InitiaizeGeometry();

                UpdateRotation();

                //todo call proper drawing for this
                sprite.Draw(renderContext, points, points.Length, texture2d, true, 1);
            }
            else
            {
                //TextBlock textBlock = new TextBlock();
                //textBlock.Width = this.Width;
                //textBlock.Height = this.Height;
                //textBlock.Foreground = new SolidColorBrush(TextObject.ForgroundColor);
                //textBlock.Text = TextObject.Text;
                //textBlock.FontWeight = TextObject.Bold ? FontWeights.Bold : FontWeights.Normal;
                //textBlock.FontSize = TextObject.FontSize * 1.2;
                //textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                //TranslateTransform tt = new TranslateTransform();
                //tt.X = this.X - (Width / 2);
                //tt.Y = this.Y - (Height / 2);
                //textBlock.RenderTransform = tt;
                //canvas.Children.Add(textBlock);
                //textBlock.Opacity = this.Opacity;

                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();

                ctx.Translate(X, Y);
                ctx.Rotate(RotationAngle * RC);
                ctx.Alpha = Opacity;
                DrawCanvasText(ctx);

                ctx.Restore();
            }
        }

        private void DrawCanvasText(CanvasContext2D ctx)
        {
            ctx.FillStyle = TextObject.ForegroundColor.ToString();
            ctx.Font = (TextObject.Italic ? "italic" : "normal") + " " + (TextObject.Bold ? "bold" : "normal") + " " + Math.Round(TextObject.FontSize * 1.2).ToString() + "px " + TextObject.FontName;
            ctx.TextBaseline = TextBaseline.Top;

            String text = TextObject.Text;

            if (text.IndexOf("{$") > -1)
            {
                if (text.IndexOf("{$DATE}") > -1)
                {
                    string date = String.Format("{0:yyyy/MM/dd}", SpaceTimeController.Now);
                    text = text.Replace("{$DATE}", date);
                }

                if (text.IndexOf("{$TIME}") > -1)
                {
                    string time = String.Format("{0:HH:mm:ss}", SpaceTimeController.Now);
                    text = text.Replace("{$TIME}", time);
                }


                text = text.Replace("{$DIST}", UiTools.FormatDistance(WWTControl.Singleton.RenderContext.SolarSystemCameraDistance));
                text = text.Replace("{$LAT}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Lat));
                text = text.Replace("{$LNG}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Lat));
                text = text.Replace("{$RA}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.RA));
                text = text.Replace("{$DEC}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Dec));
                text = text.Replace("{$FOV}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.FovAngle));
            }

            string[] lines = text.Split("\n");

            double baseline = -(Height / 2);
            double lineSpace = TextObject.FontSize * 1.7;

            foreach (string line in lines)
            {
                List<string> parts = Util.GetWrappedText(ctx, line, Width);
                foreach (string part in parts)
                {
                    ctx.FillText(part, -Width / 2, baseline);
                    baseline += lineSpace;
                }
            }
        }

        private void CalculateTextSize()
        {
             
            if (ctx == null || ce == null)
            {
                ce = (CanvasElement)Document.CreateElement("canvas");
                ce.Height = (int)100;
                ce.Width = (int)100;
                ctx = (CanvasContext2D)ce.GetContext(Rendering.Render2D);
            }
            ctx.FillStyle = TextObject.ForegroundColor.ToString();
            ctx.Font = (TextObject.Italic ? "italic" : "normal") + " " + (TextObject.Bold ? "bold" : "normal") + " " + Math.Round(TextObject.FontSize * 1.2).ToString() + "px " + TextObject.FontName;
            ctx.TextBaseline = TextBaseline.Top;

            String text = TextObject.Text;

            if (text.IndexOf("{$") > -1)
            {
                if (text.IndexOf("{$DATE}") > -1)
                {
                    string date = String.Format("{0:yyyy/MM/dd}", SpaceTimeController.Now);
                    text = text.Replace("{$DATE}", date);
                }

                if (text.IndexOf("{$TIME}") > -1)
                {
                    string time = String.Format("{0:HH:mm:ss}", SpaceTimeController.Now);
                    text = text.Replace("{$TIME}", time);
                }


                text = text.Replace("{$DIST}", UiTools.FormatDistance(WWTControl.Singleton.RenderContext.SolarSystemCameraDistance));
                text = text.Replace("{$LAT}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Lat));
                text = text.Replace("{$LNG}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Lat));
                text = text.Replace("{$RA}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.RA));
                text = text.Replace("{$DEC}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.ViewCamera.Dec));
                text = text.Replace("{$FOV}", Coordinates.FormatDMS(WWTControl.Singleton.RenderContext.FovAngle));
            }

            string[] lines = text.Split("\n");

            double baseline = 0;
            double lineSpace = TextObject.FontSize * 1.7;
            double maxWidth = 0;
            foreach (string line in lines)
            {
                double width = ctx.MeasureText(line).Width;
                maxWidth = Math.Max(width, maxWidth);
                baseline += lineSpace;
            }
            //Width + fudge factor
            Width = maxWidth*1.01;
            Height = baseline;
            ce = null;
            ctx = null;
        }




        CanvasContext2D ctx = null;
        CanvasElement ce = null;

        public override void InitializeTexture()
        {
            if (texture2d == null || (TextObject.Text.IndexOf("{$") > -1))
            {
                if (ctx == null || ce == null)
                {
                    ce = (CanvasElement) Document.CreateElement("canvas")  ;
                    ce.Height = (int)Height;
                    ce.Width = (int)Width;
                    ctx = (CanvasContext2D)ce.GetContext(Rendering.Render2D);
                }
                ctx.Translate(Width/2, Height/2);
                ctx.ClearRect(0, 0, Width, Height);
                DrawCanvasText(ctx);

                texture2d = new Texture();

                texture2d.ImageElement = (ImageElement)(Element)ce;
                texture2d.MakeTexture();
                ce = null;
                ctx = null;
            }


            //System.Drawing.Font font = TextObject.Font;
            //StringFormat sf = new StringFormat();
            //sf.Alignment = StringAlignment.Near;

            //Bitmap bmp = new Bitmap(20, 20);
            //Graphics g = Graphics.FromImage(bmp);
            //SizeF size = g.MeasureString(TextObject.Text, font);
            //g.Dispose();
            //bmp.Dispose();

            //double border =0;

            //switch (TextObject.BorderStyle)
            //{
            //    case TextBorderStyle.None:
            //    case TextBorderStyle.Tight:
            //        border = 0;
            //        break;
            //    case TextBorderStyle.Small:
            //        border = 10;
            //        break;
            //    case TextBorderStyle.Medium:
            //        border = 15;
            //        break;
            //    case TextBorderStyle.Large:
            //        border = 20;
            //        break;
            //    default:
            //        break;
            //}
            //if (size.Width == 0 || size.Height == 0)
            //{
            //    size = new SizeF(1, 1);
            //}
            //bmp = new Bitmap((int)(size.Width + (border * 2)), (int)(size.Height + (border * 2)));
            //g = Graphics.FromImage(bmp);
            //if (TextObject.BorderStyle != TextBorderStyle.None)
            //{
            //    g.Clear(TextObject.BackgroundColor);
            //}

            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            ////g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Brush textBrush = new SolidBrush(TextObject.ForgroundColor);

            //g.DrawString(TextObject.Text, font, textBrush, border, border, sf);
            //textBrush.Dispose();
            //g.Dispose();
            //texture = UiTools.LoadTextureFromBmp(device, bmp);
            //bmp.Dispose();
            //font.Dispose();
            //if (Width == 0 && Height == 0)
            //{
            //    Width = size.Width + (border * 2);
            //    Height = size.Height + (border * 2);
            //}
        }

        public override void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Text");
            TextObject.SaveToXml(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        public override void InitializeFromXml(XmlNode node)
        {
            XmlNode text = Util.SelectSingleNode(node, "Text");

            TextObject = TextObject.FromXml(Util.SelectSingleNode(text, "TextObject"));

        }


        override public void InitiaizeGeometry()
        {
            if (RenderContext.UseGl)
            {
                base.InitiaizeGeometry();
            }
        }
    }

    public enum ShapeType { Circle = 0, Rectagle = 1, Star = 2, Donut = 3, Arrow = 4, Line = 5, OpenRectagle = 6 };
    public class ShapeOverlay : Overlay
    {

        public override string GetTypeName()
        {
            return "TerraViewer.ShapeOverlay";
        }

        ShapeType shapeType = ShapeType.Rectagle;

        public ShapeOverlay()
        {
        }


        public ShapeType ShapeType
        {
            get { return shapeType; }
            set
            {
                shapeType = value;
                CleanUpGeometry();
            }
        }

        //public ShapeOverlay(RenderContext renderContext, TourStop owner, ShapeType shapeType)
        //{
        //    ShapeType = shapeType;
        //    this.Owner = owner;
        //    this.Name = owner.GetNextDefaultName(shapeType.ToString());
        //}

        Sprite2d sprite = new Sprite2d();

        public override void Draw3D(RenderContext renderContext, bool designTime)
        {
            if (RenderContext.UseGl)
            {
                InitiaizeGeometry();
                sprite.Draw(renderContext, points, points.Length, null, TriangleStrip, Opacity);
            }
            else
            {

                switch (shapeType)
                {
                    case ShapeType.Circle:
                        DrawCircleGeometry(renderContext);
                        break;
                    case ShapeType.Rectagle:
                        DrawRectGeometry(renderContext);
                        break;
                    case ShapeType.OpenRectagle:
                        DrawOpenRectGeometry(renderContext);
                        break;
                    case ShapeType.Star:
                        DrawStarGeometry(renderContext);
                        break;
                    case ShapeType.Donut:
                        DrawDonutGeometry(renderContext);
                        break;
                    case ShapeType.Arrow:
                        DrawArrowGeometry(renderContext);
                        break;
                    case ShapeType.Line:
                        DrawLineGeometry(renderContext);
                        break;
                    default:
                        break;
                }

            }
        }
        public override void InitiaizeGeometry()
        {
            if (points == null)
            {
                switch (shapeType)
                {
                    case ShapeType.Circle:
                        {
                            CreateCircleGeometry();

                        }
                        break;
                    case ShapeType.Rectagle:
                        base.InitiaizeGeometry();
                        break;
                    case ShapeType.OpenRectagle:
                        CreateOpenRectGeometry();
                        break;
                    case ShapeType.Star:
                        CreateStarGeometry();
                        break;
                    case ShapeType.Donut:
                        CreateDonutGeometry();
                        break;
                    case ShapeType.Arrow:
                        CreateArrowGeometry();
                        break;
                    case ShapeType.Line:
                        CreateLineGeometry();
                        break;
                    default:
                        break;
                }
            }
        }
        private void CreateLineGeometry()
        {
            double centerX = X;
            double centerY = Y;
            double radius = Width / 2;

            //float length = (float)Math.Sqrt(Width * Width + Height * Height);
            double length = Width;
            int segments = (int)(length / 12f) + 1;
            float radiansPerSegment = ((float)Math.PI * 2) / segments;
            if (points == null)
            {
                points = new PositionColoredTextured[segments * 2 + 2];
            }

            for (int j = 0; j <= segments; j++)
            {
                int i = j * 2;
                points[i] = new PositionColoredTextured();
                points[i].Position = MakePosition(X, Y, (((double)j / (double)segments) * (Width) - (Width / 2)), 6f, RotationAngle);
                points[i].Tu = ((j) % 2);
                points[i].Tv = 0;
                points[i].Color = Color;

                points[i + 1] = new PositionColoredTextured();
                points[i + 1].Position = MakePosition(X, Y, (((double)j / (double)segments) * (Width) - (Width / 2)), -6f, RotationAngle);
                points[i + 1].Tu = (j % 2);
                points[i + 1].Tv = 1;
                points[i + 1].Color = Color;

            }
        }
        private void CreateOpenRectGeometry()
        {
            double centerX = X;
            double centerY = Y;
            double radius = Width / 2;

            double length = Width;
            int segments = (int)(length / 12f) + 1;
            int segmentsHigh = (int)(Height / 12f) + 1;

            int totalPoints = (((segments + 1) * 2) + ((segmentsHigh + 1) * 2)) * 2;
            if (points == null)
            {
                points = new PositionColoredTextured[totalPoints];
            }
            for (int j = 0; j <= segments; j++)
            {
                int i = j * 2;
                points[i] = new PositionColoredTextured();
                points[i].Position = MakePosition(centerX, centerY,
                    ((double)j / (double)segments) * (Width) - (Width / 2),
                    ((Height / 2)), RotationAngle);
                points[i].Tu = ((j) % 2);
                points[i].Tv = 0;
                points[i].Color = Color;

                points[i + 1] = new PositionColoredTextured();
                points[i + 1].Position = MakePosition(centerX, centerY,
                    ((double)j / (double)segments) * (Width) - (Width / 2),
                    ((Height / 2) - 12f), RotationAngle);
                points[i + 1].Tu = (j % 2);
                points[i + 1].Tv = 1;
                points[i + 1].Color = Color;

                int k = (((segments + 1) * 4) + ((segmentsHigh + 1) * 2) - 2) - i;

                points[k] = new PositionColoredTextured();
                points[k].Position = MakePosition(centerX, centerY,
                    ((double)j / (double)segments) * (Width) - (Width / 2),
                    (-(Height / 2)) + 12f, RotationAngle);

                points[k].Tu = ((j) % 2);
                points[k].Tv = 0;
                points[k].Color = Color;

                points[k + 1] = new PositionColoredTextured();
                points[k + 1].Position = MakePosition(centerX, centerY,
                    ((double)j / (double)segments) * (Width) - (Width / 2),
                    (-(Height / 2)), RotationAngle);
                points[k + 1].Tu = (j % 2);
                points[k + 1].Tv = 1;
                points[k + 1].Color = Color;

            }
            int offset = ((segments + 1) * 2);
            for (int j = 0; j <= segmentsHigh; j++)
            {
                int top = ((segmentsHigh + 1) * 2) + offset - 2;
                int i = j * 2;
                points[top - i] = new PositionColoredTextured();
                points[top - i].Position = MakePosition(centerX, centerY, (float)(Width / 2), (float)(((double)j / (double)segmentsHigh) * (Height) - (Height / 2)), RotationAngle);

                points[top - i].Tu = ((j) % 2);
                points[top - i].Tv = 0;
                points[top - i].Color = Color;

                points[top - i + 1] = new PositionColoredTextured();
                points[top - i + 1].Position = MakePosition(centerX, centerY,
                    ((Width / 2) - 12f),
                    (((double)j / (double)segmentsHigh) * Height - ((Height / 2))), RotationAngle);

                points[top - i + 1].Tu = (j % 2);
                points[top - i + 1].Tv = 1;
                points[top - i + 1].Color = Color;

                int k = i + ((segments + 1) * 4) + ((segmentsHigh + 1) * 2);
                points[k] = new PositionColoredTextured();
                points[k].Position = MakePosition(centerX, centerY,
                                (-(Width / 2) + 12),
                                (((double)j / (double)segmentsHigh) * (Height) - (Height / 2)), RotationAngle);
                points[k].Tu = ((j) % 2);
                points[k].Tv = 0;
                points[k].Color = Color;

                points[k + 1] = new PositionColoredTextured();
                points[k + 1].Position = MakePosition(centerX, centerY,
                               (-(Width / 2)),
                               (((double)j / (double)segmentsHigh) * Height - ((Height / 2))), RotationAngle);
                points[k + 1].Tu = (j % 2);
                points[k + 1].Tv = 1;
                points[k + 1].Color = Color;

            }
        }
        PositionColoredTextured[] pnts;
        private void CreateStarGeometry()
        {
            double centerX = X;
            double centerY = Y;
            double radius = Width / 2;

            double radiansPerSegment = (Math.PI * 2) / 5;
            if (points == null)
            {
                points = new PositionColoredTextured[12];
            }

            if (pnts == null)
            {
                pnts = new PositionColoredTextured[10];
            }

            for (int i = 0; i < 5; i++)
            {
                double rads = i * radiansPerSegment - (Math.PI / 2);
                pnts[i] = new PositionColoredTextured();
                pnts[i].Position = MakePosition(centerX, centerY, (Math.Cos(rads) * (Width / 2)), (Math.Sin(rads) * (Height / 2)), RotationAngle);
                pnts[i].Tu = 0;
                pnts[i].Tv = 0;
                pnts[i].Color = Color;
            }

            for (int i = 5; i < 10; i++)
            {
                double rads = i * radiansPerSegment + (radiansPerSegment / 2) - (Math.PI / 2);
                pnts[i] = new PositionColoredTextured();
                pnts[i].Position = MakePosition(centerX, centerY, (Math.Cos(rads) * (Width / 5.3)), (Math.Sin(rads) * (Height / 5.3)), RotationAngle);

                pnts[i].Tu = 0;
                pnts[i].Tv = 0;
                pnts[i].Color = Color;
            }

            points[0] = pnts[0];
            points[1] = pnts[5];
            points[2] = pnts[9];
            points[3] = pnts[1];
            points[4] = pnts[7];
            points[5] = pnts[4];
            points[6] = pnts[6];
            points[7] = pnts[2];
            points[8] = pnts[7];
            points[9] = pnts[7];
            points[10] = pnts[3];
            points[11] = pnts[8];
            TriangleStrip = false;
        }
        private void CreateArrowGeometry()
        {
            if (points == null)
            {
                points = new PositionColoredTextured[9];
            }

            points[0] = new PositionColoredTextured();
            points[0].Position = MakePosition(X, Y, -Width / 2, -Height / 4, RotationAngle);
            points[0].Tu = 0;
            points[0].Tv = 0;
            points[0].Color = Color;

            points[1] = new PositionColoredTextured();
            points[1].Position = MakePosition(X, Y, Width / 4, -Height / 4, RotationAngle);
            points[1].Tu = 1;
            points[1].Tv = 0;
            points[1].Color = Color;


            points[2] = new PositionColoredTextured();
            points[2].Position = MakePosition(X, Y, -Width / 2, Height / 4, RotationAngle);
            points[2].Tu = 0;
            points[2].Tv = 1;
            points[2].Color = Color;

            points[3] = new PositionColoredTextured();
            points[3].Position = MakePosition(X, Y, Width / 4, -Height / 4, RotationAngle);
            points[3].Tu = 1;
            points[3].Tv = 0;
            points[3].Color = Color;


            points[4] = new PositionColoredTextured();
            points[4].Position = MakePosition(X, Y, -Width / 2, Height / 4, RotationAngle);
            points[4].Tu = 0;
            points[4].Tv = 1;
            points[4].Color = Color;


            points[5] = new PositionColoredTextured();
            points[5].Position = MakePosition(X, Y, Width / 4, Height / 4, RotationAngle);
            points[5].Tu = 1;
            points[5].Tv = 1;
            points[5].Color = Color;

            // Point

            points[6] = new PositionColoredTextured();
            points[6].Position = MakePosition(X, Y, Width / 4, -Height / 2, RotationAngle);
            points[6].Tu = 1;
            points[6].Tv = 1;
            points[6].Color = Color;

            points[7] = new PositionColoredTextured();
            points[7].Position = MakePosition(X, Y, Width / 2, 0, RotationAngle);
            points[7].Tu = 1;
            points[7].Tv = .5f;
            points[7].Color = Color;

            points[8] = new PositionColoredTextured();
            points[8].Position = MakePosition(X, Y, Width / 4, Height / 2, RotationAngle);
            points[8].Tu = 1;
            points[8].Tv = 1;
            points[8].Color = Color;

            TriangleStrip = false;
        }
        private void CreateDonutGeometry()
        {
            double centerX = X;
            double centerY = Y;
            double radius = Width / 2;

            double circumference = Math.PI * 2.0f * radius;
            int segments = (int)(circumference / 12) + 1;
            double radiansPerSegment = (Math.PI * 2) / segments;
            if (points == null)
            {
                points = new PositionColoredTextured[segments * 2 + 2];
            }

            for (int j = 0; j <= segments; j++)
            {
                int i = j * 2;
                points[i] = new PositionColoredTextured();
                points[i].Position = MakePosition(centerX, centerY, (Math.Cos(j * radiansPerSegment) * (Width / 2)), (Math.Sin(j * radiansPerSegment) * (Height / 2)), RotationAngle);
                points[i].Tu = ((j) % 2);
                points[i].Tv = 0;
                points[i].Color = Color;

                points[i + 1] = new PositionColoredTextured();
                points[i + 1].Position = MakePosition(centerX, centerY, (Math.Cos(j * radiansPerSegment) * ((Width / 2) - 10)), (Math.Sin(j * radiansPerSegment) * ((Height / 2) - 10)), RotationAngle);
                points[i + 1].Tu = (j % 2);
                points[i + 1].Tv = 1;
                points[i + 1].Color = Color;

            }
        }

        bool TriangleStrip = true;

        private void CreateCircleGeometry()
        {
            double centerX = X;
            double centerY = Y;
            double radius = Width / 2;

            double circumference = Math.PI * 2.0f * radius;
            int segments = (int)(circumference / 12) + 1;
            double radiansPerSegment = (Math.PI * 2) / segments;
            if (points == null)
            {
                points = new PositionColoredTextured[segments * 2 + 2];
            }
            for (int j = 0; j <= segments; j++)
            {
                int i = j * 2;
                points[i] = new PositionColoredTextured();
                points[i].Position = MakePosition(centerX, centerY, (Math.Cos(j * radiansPerSegment) * (Width / 2)), (Math.Sin(j * radiansPerSegment) * (Height / 2)), RotationAngle);
                points[i].Tu = ((j) % 2);
                points[i].Tv = 0;
                points[i].Color = Color;


                points[i + 1] = new PositionColoredTextured();
                points[i + 1].Position = MakePosition(centerX, centerY, 0, 0, RotationAngle);
                points[i + 1].Tu = (j % 2);
                points[i + 1].Tv = 1;
                points[i + 1].Color = Color;

            }
        }
        public override void InitializeTexture()
        {
            switch (ShapeType)
            {
                case ShapeType.Line:
                case ShapeType.Donut:
                case ShapeType.OpenRectagle:
                    {
                        //Bitmap bmp = new Bitmap(13, 10);
                        //Graphics g = Graphics.FromImage(bmp);
                        //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        //Brush brush = new SolidBrush(Color);
                        //g.FillEllipse(brush, 1, 0, 10, 9);
                        //g.Dispose();
                        //texture = Texture11.FromBitmap(bmp);
                        //bmp.Dispose();
                    }
                    break;
                case ShapeType.Circle:

                case ShapeType.Rectagle:
                case ShapeType.Star:

                case ShapeType.Arrow:
                default:
                    {
                        texture = null;
                    }
                    break;
            }
        }

        private void DrawLineGeometry(RenderContext renderContext)
        {

            //todo this needs to be Dashed rounded lines..

            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            double radius = Width / 2;
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle*RC);

            ctx.MoveTo(-radius, 0);
            ctx.LineTo(radius, 0);
            ctx.LineWidth = 9;
            ctx.StrokeStyle = Color.ToString();
            ctx.Alpha = Opacity;
            ctx.Stroke();
            ctx.Restore();


        }
        private void DrawOpenRectGeometry(RenderContext renderContext)
        {
            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle*RC);
 
            ctx.BeginPath();
            ctx.MoveTo(-Width / 2, -Height / 2);
            ctx.LineTo(Width / 2, -Height / 2);
            ctx.LineTo(Width / 2, Height / 2);
            ctx.LineTo(-Width / 2, Height / 2);
            ctx.ClosePath();
            ctx.LineWidth = 9;
            ctx.StrokeStyle = Color.ToString();

           ctx.Alpha = Opacity;
            ctx.Stroke();
            ctx.Restore();
        }

        private void DrawRectGeometry(RenderContext renderContext)
        {
            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle * RC);

            ctx.BeginPath();
            ctx.MoveTo(-Width / 2, -Height / 2);
            ctx.LineTo(Width / 2, -Height / 2);
            ctx.LineTo(Width / 2, +Height / 2);
            ctx.LineTo(-Width / 2, +Height / 2);
            ctx.ClosePath();
            ctx.LineWidth = 0;
            ctx.FillStyle = Color.ToString();

            ctx.Alpha = Opacity;
            ctx.Fill();
            ctx.Restore();
        }
        private void DrawStarGeometry(RenderContext renderContext)
        {

            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle * RC);
            ctx.BeginPath();

            double centerX = 0;
            double centerY = 0;
            double radius = Width / 2;

            double radiansPerSegment = ((double)Math.PI * 2) / 5;

            bool first = true;

            for (int i = 0; i < 5; i++)
            {
                double rads = i * radiansPerSegment - (Math.PI / 2);
                if (first)
                {
                    first = false;
                    ctx.MoveTo(centerX + Math.Cos(rads) * (Width / 2), centerY + Math.Sin(rads) * (Height / 2));
                }
                else
                {
                    ctx.LineTo(centerX + Math.Cos(rads) * (Width / 2), centerY + Math.Sin(rads) * (Height / 2));
                }

                double rads2 = i * radiansPerSegment + (radiansPerSegment / 2) - (Math.PI / 2);
                ctx.LineTo(centerX + Math.Cos(rads2) * (Width / 5.3), centerY + Math.Sin(rads2) * (Height / 5.3));

            }

            ctx.ClosePath();
            ctx.LineWidth = 0;
            ctx.FillStyle = Color.ToString();


            ctx.Alpha = Opacity;
            ctx.Fill();
            ctx.Restore();
        }
        private void DrawArrowGeometry(RenderContext renderContext)
        {
            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle * RC);
 
            ctx.BeginPath();
            ctx.MoveTo((-(Width / 2)), (-(Height / 4)));
            ctx.LineTo(((Width / 4)), (-(Height / 4)));
            ctx.LineTo(((Width / 4)), (-(Height / 2)));
            ctx.LineTo(((Width / 2)), 0);
            ctx.LineTo(((Width / 4)), ((Height / 2)));
            ctx.LineTo(((Width / 4)), ((Height / 4)));
            ctx.LineTo((-(Width / 2)), ((Height / 4)));
            ctx.ClosePath();
            ctx.LineWidth = 0;
            ctx.FillStyle = Color.ToString();

           ctx.Alpha = Opacity;
            ctx.Fill();
            ctx.Restore();
        }

        private void DrawDonutGeometry(RenderContext renderContext)
        {
            //todo move to dashed lines in future
            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Translate(X, Y);
            ctx.Scale(1, Height / Width);
            ctx.Rotate(RotationAngle * RC);
            ctx.BeginPath();
            ctx.Arc(0, 0, Width / 2, 0, Math.PI * 2, false);

            ctx.ClosePath();
            ctx.LineWidth = 9;
            ctx.StrokeStyle = Color.ToString();
            ctx.Alpha = Opacity;
            ctx.Stroke();
            ctx.Restore();


        }

        private void DrawCircleGeometry(RenderContext renderContext)
        {
            CanvasContext2D ctx = renderContext.Device;
            ctx.Save();
            ctx.Scale(1, Width / Height);
            ctx.Translate(X, Y);
            ctx.Rotate(RotationAngle * RC);
            ctx.BeginPath();
            ctx.Arc(0, 0, Width, 0, Math.PI * 2, false);
            ctx.ClosePath();
            ctx.LineWidth = 0;
            ctx.FillStyle = Color.ToString();
            ctx.Alpha = Opacity;
            ctx.Fill();
            ctx.Restore();
        }

  
        public override void CleanUpGeometry()
        {
            base.CleanUpGeometry();
            CleanUp();
        }

        public override void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Shape");
            xmlWriter.WriteAttributeString("ShapeType", Enums.ToXml("ShapeType",(int)shapeType));
            xmlWriter.WriteEndElement();
        }

        public override void InitializeFromXml(XmlNode node)
        {
            XmlNode shape = Util.SelectSingleNode(node, "Shape");
            shapeType = (ShapeType)Enums.Parse("ShapeType", shape.Attributes.GetNamedItem("ShapeType").Value);
        }

        internal static ShapeOverlay Create(TourStop currentTourStop, ShapeType shapeType)
        {
            ShapeOverlay overlay = new ShapeOverlay();
            overlay.shapeType = shapeType;
            overlay.Owner = currentTourStop;
            

            return overlay;
        }
    }
    public class AudioOverlay : Overlay
    {
        public override string GetTypeName()
        {
            return "TerraViewer.AudioOverlay";
        }
        string filename;
        AudioElement audio = null;
        int volume = 100;

        bool mute = false;

        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                Volume = Volume;
            }
        }

        public int Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (audio != null)
                {
                    audio.Volume = this.mute ? 0 : (float)(volume / 100.0);
                }
            }
        }

        public AudioOverlay()
        {
            isDesignTimeOnly = true;

        }

        //void audio_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        //{
        //    audio.Stop();
        //}

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            fc.AddFile(Owner.Owner.WorkingDirectory + filename, Owner.Owner.GetFileBlob(this.filename));
        }


        public override void Play()
        {
            if (audio == null)
            {
                InitializeTexture();
            }
            if (audio != null && audioReady)
            {
                audio.Play();
                Volume = Volume;
                audio.CurrentTime = position;
            }
        }


        public override void Pause()
        {
            if (audio == null)
            {
                InitializeTexture();
            }
            if (audio != null && audioReady)
            {
                audio.Pause();
            }
        }


        public override void Stop()
        {
            if (audio == null)
            {
                InitializeTexture();
            }
            if (audio != null && audioReady)
            {
                audio.Pause();
            }
        }

        double position = 0;
        public override void Seek(double time)
        {
            position = time;
            if (audio == null)
            {
                InitializeTexture();
            }
            //todo double check time

            if (audioReady)
            {
                if (audio.Duration < time)
                {
                    audio.Pause();
                }
                else
                {
                    audio.CurrentTime = position;
                }
            }
        }
        bool audioReady = false;
        //public AudioOverlay(RenderContext renderContext, TourStop owner, string filename)
        //{
        //    isDesignTimeOnly = true;
        //    X = 0;
        //    Y = 0;
        //    this.filename = Guid.NewGuid().ToString() + filename.Substr(filename.LastIndexOf("."));
        //    this.Owner = owner;
        //    this.Name = owner.GetNextDefaultName("Audio");
        //    // File.Copy(filename, Owner.Owner.WorkingDirectory + this.filename);
        //}

        public override void InitializeTexture()
        {
            if (audio == null)
            {
                audio = (AudioElement)Document.CreateElement("audio");
                //audio.AutoPlay = true;
                //audio.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(audio_MediaFailed);
                //audio.MediaOpened += new RoutedEventHandler(audio_MediaOpened);
                //Viewer.MasterView.audio.Children.Add(audio);
                audio.Src = Owner.Owner.GetFileStream(this.filename);
                audio.AddEventListener("canplaythrough", delegate
                {
                    if (!audioReady)
                    {
                        audioReady = true;
                        audio_MediaOpened();
                        audio.Play();
                    }
                }, false);


            }

        }

        public override void CleanUp()
        {
            base.CleanUp();

            if (audio != null)
            {
                audio.Pause();
                audio.Src = null;
                audio = null;
            }

        }

        void audio_MediaOpened()
        {
            audio.CurrentTime = position;

            audio.Volume = this.mute ? 0 : (float)(volume / 100.0);

        }

        AudioType trackType = AudioType.Music;

        public AudioType TrackType
        {
            get { return trackType; }
            set { trackType = value; }
        }

        public override void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Audio");
            xmlWriter.WriteAttributeString("Filename", filename);
            xmlWriter.WriteAttributeString("Volume", volume.ToString());
            xmlWriter.WriteAttributeString("Mute", mute.ToString());
            xmlWriter.WriteAttributeString("TrackType", Enums.ToXml("AudioType",(int)trackType));
          //  xmlWriter.WriteAttributeString("Begin", begin.ToString());
          //  xmlWriter.WriteAttributeString("End", end.ToString());
           // xmlWriter.WriteAttributeString("FadeIn", fadeIn.ToString());
           // xmlWriter.WriteAttributeString("FadeOut", fadeOut.ToString());
          //  xmlWriter.WriteAttributeString("Loop", loop.ToString());
            xmlWriter.WriteEndElement();
        }

        public override void InitializeFromXml(XmlNode node)
        {
            XmlNode audio = Util.SelectSingleNode(node, "Audio");
            filename = audio.Attributes.GetNamedItem("Filename").Value;
            if (audio.Attributes.GetNamedItem("Volume") != null)
            {
                volume = int.Parse(audio.Attributes.GetNamedItem("Volume").Value);
            }

            if (audio.Attributes.GetNamedItem("Mute") != null)
            {
                mute = bool.Parse(audio.Attributes.GetNamedItem("Mute").Value);
            }

            if (audio.Attributes.GetNamedItem("TrackType") != null)
            {
                trackType = (AudioType)Enums.Parse("AudioType", audio.Attributes.GetNamedItem("TrackType").Value);
            }
        }

        public static AudioOverlay Create(TourStop currentTourStop, System.Html.Data.Files.File file)
        {
            AudioOverlay ao = new AudioOverlay();
            ao.Owner = currentTourStop;
            ao.filename = file.Name;
            ao.Owner.Owner.AddCachedFile(file.Name, file);
            return ao;
        }
    }

    public enum LoopTypes { Loop = 0, UpDown = 1, Down = 2, UpDownOnce = 3, Once = 4, Begin = 5, End = 6 };

    public class FlipbookOverlay : Overlay
    {

        public override string GetTypeName()
        {
            return "TerraViewer.FlipbookOverlay";
        }

        string filename;

        LoopTypes loopType = LoopTypes.UpDown;

        public LoopTypes LoopType
        {
            get { return loopType; }
            set { loopType = value; }
        }

        int startFrame = 0;

        public int StartFrame
        {
            get { return startFrame; }
            set { startFrame = value; }
        }
        List<int> framesList = new List<int>();
        string frameSequence;

        public string FrameSequence
        {
            get { return frameSequence; }
            set
            {
                if (frameSequence != value)
                {
                    frameSequence = value;
                    framesList = new List<int>();
                    if (!string.IsNullOrEmpty(frameSequence))
                    {
                        try
                        {
                            string[] parts = frameSequence.Split(",");
                            foreach (string part in parts)
                            {
                                int x = int.Parse(part.Trim());
                                framesList.Add(x);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        int frames = 1;

        public int Frames
        {
            get { return frames; }
            set
            {
                frames = value;
            }
        }

        int framesX = 8;

        public int FramesX
        {
            get { return framesX; }
            set { framesX = value; }
        }
        int framesY = 8;

        public int FramesY
        {
            get { return framesY; }
            set { framesY = value; }
        }



        public FlipbookOverlay()
        {

        }

        //public FlipbookOverlay(RenderContext renderContext, TourStop owner, string filename)
        //{
        //    this.Owner = owner;


        //    string extension = filename.Substr(filename.LastIndexOf("."));

        //    this.filename = Guid.NewGuid().ToString() + extension;

        //    this.Name = filename.Substr(filename.LastIndexOf('\\'));
        //    //File.Copy(filename, Owner.Owner.WorkingDirectory + this.filename);

        //    //Bitmap bmp = new Bitmap(Owner.Owner.WorkingDirectory + this.filename);
        //    Width = 256;
        //    Height = 256;
        //    //bmp.Dispose();
        //    //bmp = null;
        //    X = 0;
        //    Y = 0;
        //}


        //public FlipbookOverlay(RenderContext renderContext, TourStop owner, Image image)
        //{
        //    this.Owner = owner;
        //    this.canvas = canvas;
        //    // to make directory and guid filename in tour temp dir.
        //    this.filename = Guid.NewGuid().ToString() + ".png";

        //    this.Name = owner.GetNextDefaultName("Image");
        //    X = 0;
        //    Y = 0;
        //    //image.Save(Owner.Owner.WorkingDirectory + filename, ImageFormat.Png);
        //    Width = 256;
        //    Height = 256;
        //}

        public FlipbookOverlay Copy(TourStop owner)
        {
            //todo fix this
            FlipbookOverlay newFlipbookOverlay = new FlipbookOverlay();
            newFlipbookOverlay.Owner = owner;
            newFlipbookOverlay.filename = this.filename;
            newFlipbookOverlay.X = this.X;
            newFlipbookOverlay.Y = this.Y;
            newFlipbookOverlay.Width = this.Width;
            newFlipbookOverlay.Height = this.Height;
            newFlipbookOverlay.Color = this.Color;
            newFlipbookOverlay.Opacity = this.Opacity;
            newFlipbookOverlay.RotationAngle = this.RotationAngle;
            newFlipbookOverlay.Name = this.Name + " - Copy";
            newFlipbookOverlay.StartFrame = this.StartFrame;
            newFlipbookOverlay.Frames = this.Frames;
            newFlipbookOverlay.LoopType = this.LoopType;
            newFlipbookOverlay.FrameSequence = this.FrameSequence;
            newFlipbookOverlay.FramesX = this.FramesX;
            newFlipbookOverlay.FramesY = this.FramesY;

            return newFlipbookOverlay;
        }

        public override void CleanUp()
        {
            texture = null;
        }
        bool textureReady = false;
        public override void InitializeTexture()
        {
            try
            {
                bool colorKey = filename.ToLowerCase().EndsWith(".jpg");
                texture = Owner.Owner.GetCachedTexture( filename, delegate { textureReady = true; });

                //texture = UiTools.LoadTextureFromBmp(device, Owner.Owner.WorkingDirectory + filename);

                //      texture = TextureLoader.FromFile(device, Owner.Owner.WorkingDirectory + filename);

                //if (Width == 0 && Height == 0)
                //{
                //    Width = sd.Width;
                //    Height = sd.Height;

                //}
            }
            catch
            {
                //texture = UiTools.LoadTextureFromBmp(device, (Bitmap)global::TerraViewer.Properties.Resources.BadImage);
                //SurfaceDescription sd = texture.GetLevelDescription(0);

                //{
                //    Width = sd.Width;
                //    Height = sd.Height;

                //}
            }
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            fc.AddFile(Owner.Owner.WorkingDirectory + filename, Owner.Owner.GetFileBlob(filename));
        }

        public override void WriteOverlayProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Flipbook");
            xmlWriter.WriteAttributeString("Filename", filename);
            xmlWriter.WriteAttributeString("Frames", frames.ToString());
            xmlWriter.WriteAttributeString("Loop", Enums.ToXml("LoopTypes", (int)loopType));
            xmlWriter.WriteAttributeString("FramesX", framesX.ToString());
            xmlWriter.WriteAttributeString("FramesY", framesY.ToString());
            xmlWriter.WriteAttributeString("StartFrame", startFrame.ToString());
            if (!string.IsNullOrEmpty(frameSequence))
            {
                xmlWriter.WriteAttributeString("FrameSequence", frameSequence.ToString());
            }
            xmlWriter.WriteEndElement();
        }

        public override void InitializeFromXml(XmlNode node)
        {
            XmlNode flipbook = Util.SelectSingleNode(node, "Flipbook");
            filename = flipbook.Attributes.GetNamedItem("Filename").Value;
            frames = int.Parse(flipbook.Attributes.GetNamedItem("Frames").Value);

            loopType = (LoopTypes)Enums.Parse("LoopTypes", flipbook.Attributes.GetNamedItem("Loop").Value);
            
            if (flipbook.Attributes.GetNamedItem("FramesX") != null)
            {
                FramesX = int.Parse(flipbook.Attributes.GetNamedItem("FramesX").Value);
            }
            if (flipbook.Attributes.GetNamedItem("FramesY") != null)
            {
                FramesY = int.Parse(flipbook.Attributes.GetNamedItem("FramesY").Value);
            }
            if (flipbook.Attributes.GetNamedItem("StartFrame") != null)
            {
                StartFrame = int.Parse(flipbook.Attributes.GetNamedItem("StartFrame").Value);
            }
            if (flipbook.Attributes.GetNamedItem("FrameSequence") != null)
            {
                FrameSequence = flipbook.Attributes.GetNamedItem("FrameSequence").Value;
            }
        }

        int currentFrame = 0;
        //int widthCount = 8;
        //int heightCount = 8;
        int cellHeight = 256;
        int cellWidth = 256;
        Date timeStart = Date.Now;
        bool playing = true;
        public override void Play()
        {
            playing = true;
            timeStart = Date.Now;
        }
        public override void Pause()
        {
            playing = false;
        }
        public override void Stop()
        {
            playing = false;
            currentFrame = 0;
        }

        override public void InitiaizeGeometry()
        {
            int frameCount = frames;
            if (!String.IsNullOrEmpty(frameSequence))
            {
                frameCount = framesList.Count;
            }

            if (playing)
            {
                // todo allow play backwards loop to point.
                int ts = Date.Now - timeStart;
                switch (loopType)
                {
                    case LoopTypes.Loop:
                        currentFrame = (int)((ts / 1000.0 * 24.0) % frameCount) + startFrame;
                        break;
                    case LoopTypes.UpDown:
                        currentFrame = Math.Abs((int)((ts / 1000.0 * 24.0 + frameCount) % (frameCount * 2 - 1)) - (frameCount - 1)) + startFrame;
                        if (currentFrame < 0 || currentFrame > frameCount - 1)
                        {
                            int p = 0;
                        }
                        break;
                    case LoopTypes.Down:
                        currentFrame = Math.Max(0, frameCount - (int)((ts / 1000.0 * 24.0) % frameCount)) + startFrame;
                        break;
                    case LoopTypes.UpDownOnce:
                        int temp = (int)Math.Min(ts / 1000.0 * 24.0, frameCount * 2 + 1) + frameCount;
                        currentFrame = Math.Abs((int)((temp) % (frameCount * 2 - 1)) - (frameCount - 1)) + startFrame;
                        break;
                    case LoopTypes.Once:
                        currentFrame = Math.Min(frameCount - 1, (int)((ts / 1000.0 * 24.0)));
                        break;
                    case LoopTypes.Begin:
                        currentFrame = startFrame;
                        break;
                    case LoopTypes.End:
                        currentFrame = (frameCount - 1) + startFrame;
                        break;
                    default:
                        currentFrame = startFrame;
                        break;
                }

            }
            if (!String.IsNullOrEmpty(frameSequence))
            {
                if (currentFrame < framesList.Count && currentFrame > -1)
                {
                    currentFrame = framesList[currentFrame];
                }
                else
                {
                    currentFrame = 0;
                }
            }

            currentRotation = 0;
        }
    }
}


