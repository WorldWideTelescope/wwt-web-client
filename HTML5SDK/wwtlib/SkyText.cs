using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class Text3dBatch
    {
        public int Height = 128;
        //public Text3dBatch()
        //{
        //}

        public Text3dBatch(int height)
        {
            Height = (int)(height * 3f);
        }

        //public Text3dBatch(GlyphCache glyphCache)
        //{
        //    Height = glyphCache.Height;

        //}

        public List<Text3d> Items = new List<Text3d>();

        public void Add(Text3d newItem)
        {
            Items.Add(newItem);
        }
        int glyphVersion = -1;

        public Matrix3d ViewTransform = Matrix3d.Identity;

        public void Draw(RenderContext renderContext, float opacity, Color color)
        {
          

            if (renderContext.gl == null)
            {
                Vector3d viewPoint = Vector3d.TransformCoordinate(renderContext.ViewPoint, ViewTransform);

                double drawHeight = (Height / renderContext.FovAngle) * renderContext.Height / 180;

                foreach (Text3d t3d in Items)
                {
                    Vector3d screenSpacePnt = renderContext.WVP.Transform(t3d.center);
                    if (screenSpacePnt.Z < 0)
                    {
                        continue;
                    }

                    if (Vector3d.Dot((Vector3d)viewPoint, (Vector3d)t3d.center) < .55)
                    {
                        continue;
                    }

                    Vector3d screenSpaceTop = renderContext.WVP.Transform(t3d.top);

                    double rotation = Math.Atan2(screenSpacePnt.X - screenSpaceTop.X, screenSpacePnt.Y - screenSpaceTop.Y);


                    CanvasContext2D ctx = renderContext.Device;
                    ctx.Save();

                    ctx.Translate(screenSpacePnt.X, screenSpacePnt.Y);
                    ctx.Rotate(-rotation); // todo update with up vector
                    ctx.Alpha = opacity;
                    ctx.FillStyle = color.ToString();
                    ctx.Font = "normal" + " " + (false ? "bold" : "normal") + " " + Math.Round(drawHeight * 1.2).ToString() + "px " + "Arial";
                    ctx.TextBaseline = TextBaseline.Top;

                    TextMetrics tm = ctx.MeasureText(t3d.Text);

                    ctx.FillText(t3d.Text, -tm.Width / 2, -drawHeight / 2);
                    ctx.Restore();
                }
            }
            else
            {
                // gl version
                if (glyphCache == null || glyphCache.Version > glyphVersion)
                {
                    PrepareBatch();
                }
                if (glyphCache.Ready == false)
                {
                    return;
                }


                TextShader.Use(renderContext, vertexBuffer.VertexBuffer, glyphCache.Texture.Texture2d);
 
                renderContext.gl.drawArrays(GL.TRIANGLES, 0, vertexBuffer.Count);



            }
        }
       
        GlyphCache glyphCache;

        TextObject TextObject = new TextObject();
        PositionTextureVertexBuffer vertexBuffer;
        public void PrepareBatch()
        {
            if (glyphCache == null)
            {
                glyphCache = GlyphCache.GetCache(Height);
            }

            if (glyphCache.Ready == false)
            {
                return;
            }

            // Add All Glyphs

            //foreach (Text3d t3d in Items)
            //{
            //    foreach (char c in t3d.Text)
            //    {
            //        glyphCache.AddGlyph(c);
            //    }
            //}

            //// Calculate Metrics

            TextObject.Text = "";
            TextObject.FontSize = (float)Height * .50f;

            //System.Drawing.Font font = TextObject.Font;
            //StringFormat sf = new StringFormat();
            //sf.Alignment = StringAlignment.Near;

            //Bitmap bmp = new Bitmap(20, 20);
            //Graphics g = Graphics.FromImage(bmp);
            //// Create Index Buffers

            List<PositionTexture> verts = new List<PositionTexture>();
            foreach (Text3d t3d in Items)
            {
                String text = t3d.Text;
                float left = 0;
                float top = 0;
                float fntAdjust = TextObject.FontSize / 128f;
                float factor = .6666f;
                float width = 0;
                float height = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    GlyphItem item = glyphCache.GetGlyphItem(text.Substr(i, 1));
                    if (item != null)
                    {
                        width += (float)(item.Extents.X);
                        height = Math.Max(item.Extents.Y, height);
                    }
                }

                Vector2d size = Vector2d.Create(width, height); 
 
                t3d.width = size.X * (float)t3d.scale * factor * fntAdjust;
                t3d.height = size.Y * (float)t3d.scale * factor * fntAdjust;


                int charsLeft = text.Length;

                for (int i = 0; i < charsLeft; i++)
                {
                    GlyphItem item = glyphCache.GetGlyphItem(text.Substr(i, 1));
                    if (item != null)
                    {
                        Rectangle position = Rectangle.Create(left * (float)t3d.scale * factor, 0 * (float)t3d.scale * factor, item.Extents.X * fntAdjust * (float)t3d.scale * factor, item.Extents.Y * fntAdjust * (float)t3d.scale * factor);
                        left += (float)(item.Extents.X * fntAdjust);

                        //System.Diagnostics.Debug.WriteLine((position.Width/position1.Width).ToString() + ", " + (position.Height / position1.Height).ToString());

                        t3d.AddGlyphPoints(verts, item.Size, position, item.UVRect);
                    }
                }
            }


            vertCount = verts.Count;
            vertexBuffer = new PositionTextureVertexBuffer(vertCount);

            PositionTexture[] vertBuf = (PositionTexture[])vertexBuffer.Lock(); // Lock the buffer (which will return our structs)

            for (int i = 0; i < vertCount; i++)
            {
                vertBuf[i] = verts[i];
            }

            vertexBuffer.Unlock();

            glyphVersion = glyphCache.Version;
        }
        int vertCount = 0;

        public void CleanUp()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer = null;
            }
            //if (glyphCache != null)
            //{
            //    glyphCache.CleanUp();
            //    glyphCache = null;
            //}

            Items.Clear();
        }

    }


    public class GlyphItem
    {

        public GlyphItem(string glyph)
        {
            Glyph = glyph;
            UVRect = new Rectangle();
            Size = new Vector2d();
            ReferenceCount = 1;
        }

        public static GlyphItem Create(string glyph, Rectangle uv, Vector2d size, Vector2d extents)
        {
            GlyphItem temp = new GlyphItem(glyph);
            temp.Glyph = glyph;
            temp.UVRect = uv;
            temp.Size = size;
            temp.Extents = extents;
            temp.ReferenceCount = 1;

            return temp;
        }

        public void AddRef()
        {
            ReferenceCount++;
        }

        public void Release()
        {
            ReferenceCount--;
        }

        public string Glyph;
        public Rectangle UVRect;
        public Vector2d Size;
        public Vector2d Extents;
        public int ReferenceCount = 0;


        internal static GlyphItem FromXML(XmlNode node)
        {
            string glyph = node.Attributes.GetNamedItem("Glyph").Value;

            GlyphItem item = new GlyphItem(glyph);
            item.UVRect = Rectangle.Create(
                double.Parse(node.Attributes.GetNamedItem("UVLeft").Value),
                double.Parse(node.Attributes.GetNamedItem("UVTop").Value),
                double.Parse(node.Attributes.GetNamedItem("UVWidth").Value),
                double.Parse(node.Attributes.GetNamedItem("UVHeight").Value)
                );

            item.Size = Vector2d.Create(
                double.Parse(node.Attributes.GetNamedItem("SizeWidth").Value),
                double.Parse(node.Attributes.GetNamedItem("SizeHeight").Value));

            item.Extents = Vector2d.Create(
              double.Parse(node.Attributes.GetNamedItem("ExtentsWidth").Value),
              double.Parse(node.Attributes.GetNamedItem("ExtentsHeight").Value));

            return item;
        }
    }

    public class GlyphCache : IDisposable
    {
        static Dictionary<int, GlyphCache> caches = new Dictionary<int, GlyphCache>();

        static public GlyphCache GetCache(int height)
        {
            if (!caches.ContainsKey(height))
            {
                caches[height] = new GlyphCache(height);
            }
            return caches[height];
        }

        static public void CleanUpAll()
        {
            //foreach (GlyphCache cache in caches.Values)
            //{
            //    cache.CleanUp();
            //}

            caches.Clear();
        }

        Texture texture;

        int cellHeight = 128;

        public int Height
        {
            get
            {
                return cellHeight;
            }
        }
        int gridSize = 8;

        //public GlyphCache()
        //{

        //}

        private WebFile webFile;

        private GlyphCache(int height)
        {
            cellHeight = height;
            texture = Planets.LoadPlanetTexture("/webclient/images/glyphs1.png");

            webFile = new WebFile("/webclient/images/glyphs1.xml");
            webFile.OnStateChange = GlyphXmlReady;
            webFile.Send();
        }

        private void GlyphXmlReady()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                LoadXmlGlyph(webFile.GetXml());

            }
        }

        public bool Ready = false;
        private void LoadXmlGlyph(XmlDocument xml)
        {
            XmlNode nodes = Util.SelectSingleNode(xml, "GlyphItems");

            foreach (XmlNode glyphItem in nodes.ChildNodes)
            {
                if (glyphItem.Name == "GlyphItem")
                {
                    GlyphItem item = GlyphItem.FromXML(glyphItem);

                    GlyphItems[item.Glyph] = item;
                    allGlyphs = allGlyphs + item.Glyph;
                }
            }
            Ready = true;

        }

        public Texture Texture
        {
            get
            {
              

                return texture;
            }
        }

        private void MakeTexture()
        {
            CalcOrMake(true);
        }

        Dictionary<string, GlyphItem> GlyphItems = new Dictionary<string, GlyphItem>();

        public GlyphItem GetGlyphItem(string glyph)
        {
            if (dirty)
            {
                CalculateGlyphDetails();
            }
            return GlyphItems[glyph];
        }

        public TextObject TextObject = new TextObject();

        private void CalculateGlyphDetails()
        {
            CalcOrMake(false);
        }

        private void CalcOrMake(bool makeTexture)
        {

            //gridSize = 1;

            //while ((gridSize * gridSize) < GlyphItems.Count)
            //{
            //    gridSize *= 2;
            //}

            //int cellSize = 2;

            //while (cellSize < cellHeight)
            //{
            //    cellSize *= 2;
            //}
            //cellHeight = cellSize;

            //int textureSize = cellHeight * gridSize;

            //TextObject.Text = "";
            //TextObject.FontSize = (float)cellHeight * .50f;


            //System.Drawing.Font font = TextObject.Font;
            //StringFormat sf = new StringFormat();
            //sf.Alignment = StringAlignment.Near;

            //Bitmap bmp;
            //if (makeTexture)
            //{
            //    bmp = new Bitmap(textureSize, textureSize);
            //}
            //else
            //{
            //    bmp = new Bitmap(20, 20);
            //}

            //Graphics g = Graphics.FromImage(bmp);

            //int count = 0;

            ////g.Clear(Color.Blue);

            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


            //CharacterRange[] ranges = new CharacterRange[1];
            //ranges[0] = new CharacterRange(0, 1);

            //sf.SetMeasurableCharacterRanges(ranges);



            //foreach (GlyphItem item in GlyphItems.Values)
            //{
            //    int x = (int)(count % gridSize) * cellHeight;
            //    int y = (int)(count / gridSize) * cellHeight;
            //    string text = new string(item.Glyph, 1);
            //    item.Size = g.MeasureString(text, font);
            //    Region[] reg = g.MeasureCharacterRanges(text, font, new RectangleF(new PointF(0, 0), item.Size), sf);
            //    RectangleF rectf = reg[0].GetBounds(g);

            //    float div = textureSize;
            //    item.UVRect = new RectangleF(x / div, y / div, item.Size.Width / div, item.Size.Height / div);
            //    item.UVRect = new RectangleF((x + rectf.X) / div, (y + rectf.Y) / div, rectf.Width / div, rectf.Height / div);
            //    if (makeTexture)
            //    {
            //        g.DrawString(text, font, Brushes.White, x, y, sf);
            //    }
            //    count++;
            //}

            //g.Dispose();
            //GC.SuppressFinalize(g);
            //if (makeTexture)
            //{
            //    if (texture != null)
            //    {
            //        texture.Dispose();
            //        GC.SuppressFinalize(texture);
            //        texture = null;
            //    }
            //    texture = Texture11.FromBitmap(RenderContext11.PrepDevice, bmp);
            //    textureDirty = false;
            //    //Earth3d.MainWindow.ShowDebugBitmap(bmp);
            //}
            //else
            //{
            //    textureDirty = true;
            //}
            ////bmp.Dispose();
            //GC.SuppressFinalize(bmp);
            //dirty = false;

        }



        bool dirty = true;
        bool textureDirty = true;
        int version = 0;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        static string allGlyphs = "";

        public void AddGlyph(string glyph)
        {
            if (!GlyphItems.ContainsKey(glyph))
            {
                GlyphItem item = new GlyphItem(glyph);
                GlyphItems[glyph] = item;
                dirty = true;
                textureDirty = true;
                version++;
                allGlyphs = allGlyphs + glyph.ToString();
            }
            else
            {
                GlyphItems[glyph].AddRef();
            }
        }





        public void CleanUp()
        {
            dirty = true;
            texture = null;
        }



        #region IDisposable Members

        public void Dispose()
        {
            CleanUp();
        }

        #endregion

        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
            }
        }
    }

    public enum Alignment
    {
        Center = 0,
        Left = 1
    }

    public class Text3d
    {
        //public Text3d()
        //{

        //}


        public Text3d(Vector3d center, Vector3d up, string text, float fontsize, double scale)
        {
            Text = text;
            this.up = up;
            this.center = center;
            this.scale = scale;
            this.top = Vector3d.AddVectors(center,Vector3d.Scale(up, scale));

            if (fontsize < 0)
            {
                sky = false;
            }
        }

        public double Rotation = 0;
        public double Tilt = 0;
        public double Bank = 0;
        Matrix3d rtbMat;
        bool matInit = false;

        public Color Color = Colors.White;
        public bool sky = true;
        public Vector3d center;
        public Vector3d up;
        public Vector3d top;
        public double scale;
        public float Opacity = 1.0f;
        public string Text = "";

        public double width = 1;
        public double height = 1;

        public Alignment alignment = Alignment.Center;

        public void AddGlyphPoints(List<PositionTexture> pointList, Vector2d size, Rectangle position, Rectangle uv)
        {
            PositionTexture[] points = new PositionTexture[6];

            for(int i=0; i<6;i++)
            {
                points[i] = new PositionTexture();
            }



            Vector3d left = Vector3d.Cross(center, up);
            Vector3d right = Vector3d.Cross(up, center);

            left.Normalize();
            right.Normalize();
            up.Normalize();

            Vector3d upTan = Vector3d.Cross(center, right);

            upTan.Normalize();

            if (alignment == Alignment.Center)
            {
                left.Multiply(width - position.Left * 2);
                right.Multiply(width - ((width * 2) - position.Right * 2));
            }
            else if (alignment == Alignment.Left)
            {
                left.Multiply(-position.Left * 2);
                right.Multiply(position.Right * 2);
            }

            Vector3d top = upTan.Copy();
            Vector3d bottom = Vector3d.SubtractVectors(Vector3d.Empty,upTan);

            top.Multiply(height - position.Top * 2);
            bottom.Multiply(height - ((height * 2) - position.Bottom * 2));
            Vector3d ul = center.Copy();
            ul.Add(top);
            if (sky)
            {
                ul.Add(left);
            }
            else
            {
                ul.Subtract(left);
            }
            Vector3d ur = center.Copy();
            ur.Add(top);
            if (sky)
            {
                ur.Add(right);
            }
            else
            {
                ur.Subtract(right);
            }
            Vector3d ll = center.Copy();
            if (sky)
            {
                ll.Add(left);
            }
            else
            {
                ll.Subtract(left);
            }

            ll.Add(bottom);

            Vector3d lr = center.Copy();
            if (sky)
            {
                lr.Add(right);
            }
            else
            {
                lr.Subtract(right);
            }
            lr.Add(bottom);

            points[0].Position = ul.Copy();
            points[0].Tu = uv.Left;
            points[0].Tv = uv.Top;
       //     points[0].Color = Color;

            points[2].Tu = uv.Left;
            points[2].Tv = uv.Bottom;
            points[2].Position = ll.Copy();
      //      points[2].Color = Color;

            points[1].Tu = uv.Right;
            points[1].Tv = uv.Top;
            points[1].Position = ur.Copy();
      //      points[1].Color = Color;

            points[3].Tu = uv.Right;
            points[3].Tv = uv.Bottom;
            points[3].Position = lr.Copy();
      //      points[3].Color = Color;

            points[5].Tu = uv.Right;
            points[5].Tv = uv.Top;
            points[5].Position = ur.Copy();
       //     points[5].Color = Color;

            points[4].Tu = uv.Left;
            points[4].Tv = uv.Bottom;
            points[4].Position = ll.Copy();
       //     points[4].Color = Color;

            if (Rotation != 0 || Tilt != 0 || Bank != 0)
            {
                if (!matInit)
                {
                    Matrix3d lookAt = Matrix3d.LookAtLH(center, new Vector3d(), up);
                    Matrix3d lookAtInv = lookAt.Clone();
                    lookAtInv.Invert();

                    rtbMat =  Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix( Matrix3d.MultiplyMatrix(lookAt, Matrix3d.RotationZ(-Rotation / 180 * Math.PI)),  Matrix3d.RotationX(-Tilt / 180 * Math.PI)),  Matrix3d.RotationY(-Bank / 180 * Math.PI)), lookAtInv);
                    //todo make this true after debug
                    matInit = true;
                }
                for (int i = 0; i < 6; i++)
                {
                    points[i].Position = Vector3d.TransformCoordinate(points[i].Position, rtbMat);
                }
            }

            foreach (PositionTexture pnt in points)
            {
                pointList.Add(pnt);
            }
        }

    }

}
