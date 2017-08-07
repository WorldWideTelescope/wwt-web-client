using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;

namespace wwtlib
{
    public abstract class Tile
    {
        public static int CurrentRenderGeneration = 0;
        internal List<RenderTriangle>[] RenderTriangleLists = new List<RenderTriangle>[4];

        internal WebGLBuffer[] IndexBuffers = new WebGLBuffer[4];
        internal WebGLBuffer VertexBuffer;

        public virtual WebGLBuffer GetIndexBuffer(int index, int accomidation)
        {
            return IndexBuffers[index];
        }


        public static int TileTargetX = -1;
        public static int TileTargetY = -1;
        public static int TileTargetLevel = -1;

        public int Level;
       
        public int tileX;

        public int tileY;
        public ImageElement texture = null;
        public WebGLTexture texture2d = null;

        public bool ReadyToRender = false;
        public bool InViewFrustum = true;
        public static int TilesInView = 0;
        public static int TrianglesRendered = 0;
        public static int TilesTouched = 0;
        public static List<Tile> frustumList = null;

        public static GL PrepDevice = null;

        protected Vector3d TopLeft;
        protected Vector3d BottomRight;
        protected Vector3d TopRight;
        protected Vector3d BottomLeft;

        public static double uvMultiple = 256;
  
        public static List<Tile> GetFrustumList()
        {
            try
            {
                return frustumList;
            }
            catch
            {
               // return "";
                return null;
            }
        }

        public virtual bool IsPointInTile(double lat, double lng)
        {
            return false;
        }


        public virtual double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {
            return 0;
        }

        protected Tile[] children = new Tile[4] { null, null, null, null };
        public Tile Parent = null;

        public Vector3d localCenter = new Vector3d();
        public int RenderedAtOrBelowGeneration = 0;
 
        public void MakeTexture()
        {
            if (PrepDevice != null)
            {
           //     PrepDevice.pixelStorei(GL.UNPACK_FLIP_Y_WEBGL, 1);

                try
                {
                    texture2d = PrepDevice.createTexture();

                    PrepDevice.bindTexture(GL.TEXTURE_2D, texture2d);
                    PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_EDGE);
                    PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_EDGE);
                    PrepDevice.texImage2D(GL.TEXTURE_2D, 0, GL.RGBA, GL.RGBA, GL.UNSIGNED_BYTE, texture);
                    PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_NEAREST);
                    PrepDevice.generateMipmap(GL.TEXTURE_2D);

                    PrepDevice.bindTexture(GL.TEXTURE_2D, null);
                }
                catch
                {
                    errored = true;

                }
            }
        }

        public int AddVertex(float[] buffer, int index, PositionTexture p)
        {
            buffer[index++] = (float)p.Position.X;
            buffer[index++] = (float)p.Position.Y;
            buffer[index++] = (float)p.Position.Z;
            buffer[index++] = (float)p.Tu;
            buffer[index++] = (float)p.Tv;

            return index;
        }

        protected Vector3d GeoTo3dWithAlt(double lat, double lng, bool useLocalCenter, bool rev)
        {
            lat = Math.Max(Math.Min(90, lat), -90);
            lng = Math.Max(Math.Min(180, lng), -180);
            if (!DemEnabled || DemData == null)
            {
                return GeoTo3d(lat, lng, useLocalCenter);
            }
            if (rev)
            {
                lng -= 180;
            }
            double altitude = DemData[demIndex];
            Vector3d retVal = GeoTo3dWithAltitude(lat, lng, altitude, useLocalCenter);
            return retVal;
        }

        public Vector3d GeoTo3dWithAltitude(double lat, double lng, double altitude, bool useLocalCenter)
        {

            double radius = 1 + (altitude / DemScaleFactor);
            Vector3d retVal = (Vector3d.Create((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius)));
            if (useLocalCenter)
            {
                retVal.Subtract(localCenter);
            }
            return retVal;
        }


        private double demScaleFactor = 6371000;

        internal double DemScaleFactor
        {
            get { return demScaleFactor; }
            set
            {
                demScaleFactor = value;// / Properties.Settings.Default.TerrainScaling;
            }
        }

        public virtual void RequestImage()
        {
            if (!Downloading && !ReadyToRender)
            {
                Downloading = true;
                texture = (ImageElement)Document.CreateElement("img");
                CrossDomainImage xdomimg = (CrossDomainImage)(object)texture;

                texture.AddEventListener("load", delegate(ElementEvent e)
                {
                    texReady = true;
                    Downloading = false;
                    errored = false;
                    ReadyToRender = texReady && (DemReady || !demTile);
                    RequestPending = false;
                    TileCache.RemoveFromQueue(this.Key, true);
                    MakeTexture();
                }, false);

                texture.AddEventListener("error", delegate(ElementEvent e)
                {
                    if (!texture.HasAttribute("proxyattempt"))
                    {
                        texture.Src = Util.GetProxiedUrl(URL);
                        texture.SetAttribute("proxyattempt", true);
                    }
                    else
                    {
                        Downloading = false;
                        ReadyToRender = false;
                        errored = true;
                        RequestPending = false;
                        TileCache.RemoveFromQueue(this.Key, true);
                    }
                    
                }, false);

                xdomimg.crossOrigin = "anonymous";
                texture.Src = this.URL.Replace("cdn.","www.");
                //texture.Src = "dss.png";
                
                // TODO add event listener for failed!
            }
        }

        public Float32Array demFile;
        public int demIndex = 0;
        public float[] DemData;
        public float demAverage;
        public bool DemReady = false;
        public bool texReady = false;
        public bool demTile = false;
        public bool demDownloading = false;
        public static int callCount = 0;
        //public Vector3d localCenter = new Vector3d();

        public virtual bool CreateDemFromParent()
        {
            return false;
        }

        private bool LoadDemData()
        {
            if (demFile == null)
            {

                return CreateDemFromParent();
            }
            DemData = (float[])(object)demFile;


            if (demFile.length != 1089 && demFile.length != 513)
            {
                return CreateDemFromParent();
            }

            float total = 0;
            foreach (float fv in DemData)
            {
                total += fv;
            }
            demAverage /= DemData.Length;

            return true;
        }


        public void RequestDem()
        {
            if (!ReadyToRender && !demDownloading)
            {
                demTile = true;
                demDownloading = true;

                callCount++;
                XMLHttpRequest2 xhr = new XMLHttpRequest2();
                xhr.AddEventListener("load", delegate(ElementEvent e)
                {
                    DemReady = true;
                    demDownloading = false;
                    ReadyToRender = texReady && (DemReady || !demTile);
                    RequestPending = false;
                    try
                    {
                        demFile = new Float32Array(xhr.Response);
                    }
                    catch
                    {
                    }
                    
                    TileCache.RemoveFromQueue(this.Key, true);
                }, false);

                xhr.AddEventListener("error", delegate(ElementEvent e)
                {
                    demDownloading = false;
                    DemReady = false;
                    ReadyToRender = false;
                    errored = true;
                    RequestPending = false;
                    TileCache.RemoveFromQueue(this.Key, true);
                }, false);

                xhr.Open(HttpVerb.Get, DemURL/*.Replace("cdn.", "www.")*/, true);
                xhr.ResponseType = "arraybuffer";
                xhr.Send();
                // TODO add event listener for failed!
            }
        }

        public virtual bool Draw3D(RenderContext  renderContext, double opacity)
        {
   //         CanvasContext2D device = renderContext.Device;

            

            RenderedGeneration = CurrentRenderGeneration;
            TilesTouched++;
            AccessCount = TileCache.AccessID++;
 
            if (errored)
            {
                return false;
            }
 
            int xMax = 2;
 
            InViewFrustum = true;

            if (!ReadyToRender)
            {
                TileCache.AddTileToQueue(this);
                //RequestImage();
                return false;
            }

            bool transitioning = false;

            int childIndex = 0;

            int yOffset = 0;
            if (dataset.Mercator || dataset.BottomsUp )
            {
                yOffset = 1;
            }
            int xOffset = 0;

      //      try
            {
                bool anythingToRender = false;
                bool childRendered = false;
                for (int y1 = 0; y1 < 2; y1++)
                {
                    for (int x1 = 0; x1 < xMax; x1++)
                    {
                        //  if (level < (demEnabled ? 12 : dataset.Levels))
                        if (Level < dataset.Levels)
                        {
                            // make children 
                            if (children[childIndex] == null)
                            {
                                children[childIndex] = TileCache.GetTile(Level + 1, tileX * 2 + ((x1 + xOffset) % 2), tileY * 2 + ((y1 + yOffset) % 2), dataset, this);
                            }

                            if (children[childIndex].IsTileInFrustum(renderContext.Frustum))
                            {
                                InViewFrustum = true;
                                if (children[childIndex].IsTileBigEnough(renderContext))
                                {
                                    renderChildPart[childIndex].TargetState = !children[childIndex].Draw3D(renderContext, opacity);
                                    if (renderChildPart[childIndex].TargetState)
                                    {
                                        childRendered = true;
                                    }
                                }
                                else
                                {
                                    renderChildPart[childIndex].TargetState = true;
                                }
                            }
                            else
                            {
                                renderChildPart[childIndex].TargetState = renderChildPart[childIndex].State = false;
                            }

                            //if (renderChildPart[childIndex].TargetState == true || !blendMode)
                            //{
                            //    renderChildPart[childIndex].State = renderChildPart[childIndex].TargetState;
                            //}
                            if (renderChildPart[childIndex].TargetState != renderChildPart[childIndex].State)
                            {
                                transitioning = true;
                            }
                        }
                        else
                        {
                            renderChildPart[childIndex].State = true;
                        }

                        if (renderChildPart[childIndex].State == true)
                        {
                            anythingToRender = true;
                        }

                        childIndex++;
                    }
                }

                if (childRendered || anythingToRender)
                {
                    RenderedAtOrBelowGeneration = CurrentRenderGeneration;
                    if (Parent != null)
                    {
                        Parent.RenderedAtOrBelowGeneration = RenderedAtOrBelowGeneration;
                    }
                }

                if (!anythingToRender)
                {
                    return true;
                }

                if (!CreateGeometry(renderContext))
                {
                    return false;
                }

                TilesInView++;


                accomidation = ComputeAccomidation();
                for (int i = 0; i < 4; i++)
                {
                    if (renderChildPart[i].TargetState)
                    {
                        RenderPart(renderContext, i, (opacity / 100), false);
                    }
                }
            }
   //         catch
            {
            }
            return true;
        }

        public int RenderedGeneration = 0;
        public int accomidation = 0;
        public static bool useAccomidation = true;
        private int ComputeAccomidation()
        {
            int accVal = 0;

            if (!useAccomidation)
            {
                return 0;
            }




            //Bottom
            Tile top = TileCache.GetCachedTile(Level, tileX, tileY + 1, dataset, this);
            if (top == null || top.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 1;
            }

            //right
            Tile right = TileCache.GetCachedTile(Level, tileX + 1, tileY, dataset, this);
            if (right == null || right.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 2;
            }

            //top
            Tile bottom = TileCache.GetCachedTile(Level, tileX, tileY - 1, dataset, this);
            if (bottom == null || bottom.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 4;
            }
            //left
            Tile left = TileCache.GetCachedTile(Level, tileX - 1, tileY, dataset, this);
            if (left == null || left.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 8;
            }

            return accVal;
        }
        public virtual void RenderPart(RenderContext renderContext, int part, double opacity, bool combine)
        {
            
            if (PrepDevice == null)
            {
                bool lighting = renderContext.Lighting && renderContext.SunPosition != null;


                foreach (RenderTriangle tri in RenderTriangleLists[part])
                {
                    tri.Opacity = opacity;
                    if (lighting)
                    {
                        // tranform normal by WV
                        Vector3d norm = tri.Normal.Copy();
                        renderContext.World.MultiplyVector(norm);
                        norm.Normalize();

                        // Dot product from sun angle
                        double light = Vector3d.Dot(norm, renderContext.SunPosition);
                        if (light < 0)
                        {
                            light = 0;
                        }
                        else
                        {
                            light = Math.Min(1.0f, (light * 1) );
                        }

                        // set lighting
                        tri.lighting = (float)light;
                    }
                    else
                    {
                        tri.lighting = 1.0f;
                    }



                    tri.Draw(renderContext.Device, renderContext.WVP);
                }
            }
            else
            {
                TileShader.Use(renderContext, VertexBuffer, GetIndexBuffer(part, accomidation), texture2d, (float)opacity, false);
                renderContext.gl.drawElements(GL.TRIANGLES, TriangleCount * 3 , GL.UNSIGNED_SHORT, 0);
            }
        }
 
        public virtual void CleanUp(bool removeFromParent)
        {
            
            ReadyToRender = false;
            DemData = null;
            demFile = null;
            demDownloading = false;
            texReady = false;
            DemReady = false;
            errored = false;
            if (this.texture != null)
            {
                this.texture = null;

            }

            RenderTriangleLists = new List<RenderTriangle>[4];
            GeometryCreated = false;
            if (removeFromParent && Parent != null )
            {
                Parent.RemoveChild(this);
                Parent = null;
            }

            if (PrepDevice != null)
            {
                foreach (WebGLBuffer buf in IndexBuffers)
                {
                    PrepDevice.deleteBuffer(buf);
                }
                IndexBuffers = new WebGLBuffer[4];

                if (VertexBuffer != null)
                {
                    PrepDevice.deleteBuffer(VertexBuffer);
                    VertexBuffer = null;
                }

                if (texture2d != null)
                {
                    PrepDevice.deleteTexture(texture2d);
      
                    texture2d = null;
                }
            }
        }
  
        public void RemoveChild(Tile child)
        {
            for (int i = 0; i < 4; i++)
            {
                if (children[i] == child)
                {
                    children[i] = null;
                    return;
                }
            }
        }

//        bool blendMode = false;

        
        public int AccessCount = 0;
        public bool Downloading = false;
        public bool GeometryCreated = false;
        public static bool DemEnabled = false;

        public virtual bool CreateGeometry(RenderContext renderContext)
        {
            if (DemEnabled && DemReady && DemData == null)
            {
                if (!LoadDemData())
                {
                    return false;
                }
            }

            if (DemEnabled && DemData == null)
            {
                return false;
            }

            ReadyToRender = true;
            return true;
        }

        //private bool LoadDemData()
        //{
        //    DemData = (float[])(object)demFile;

        //    //todo get dem from parent
        //    return true;
        //}


        protected void CalcSphere()
        {
            Vector3d[] corners = new Vector3d[4];
            corners[0] = TopLeft;
            corners[1] = BottomRight;
            corners[2] = TopRight;
            corners[3] = BottomLeft;
            SphereHull result = ConvexHull.FindEnclosingSphere(corners);

            sphereCenter = result.Center;
            sphereRadius = result.Radius;
        }

        internal bool isHdTile = false;
        protected int demSize = 33 * 33;
     


 //       public static Viewport Viewport;
        public static int maxLevel = 20;
       


        Vector3d topLeftScreen = new Vector3d();
        Vector3d bottomRightScreen = new Vector3d();
        Vector3d topRightScreen = new Vector3d();
        Vector3d bottomLeftScreen = new Vector3d();

        virtual public bool IsTileBigEnough(RenderContext renderContext)
        {
            Matrix3d wvp = renderContext.WVP;


            //// Test for tile scale in view..
            //topLeftScreen = wvp.ProjectToScreen(TopLeft, renderContext.Width, renderContext.Height);
            //bottomRightScreen = wvp.ProjectToScreen(BottomRight, renderContext.Width, renderContext.Height);
            //topRightScreen = wvp.ProjectToScreen(TopRight, renderContext.Width, renderContext.Height);
            //bottomLeftScreen = wvp.ProjectToScreen(BottomLeft, renderContext.Width, renderContext.Height);

            //Vector3d top = topLeftScreen;
            //top.Subtract(topRightScreen);
            //double topLength = top.Length();

            //Vector3d bottom = bottomLeftScreen;
            //bottom.Subtract(bottomRightScreen);
            //double bottomLength = bottom.Length();

            //Vector3d left = bottomLeftScreen;
            //left.Subtract(topLeftScreen);
            //double leftLength = left.Length();

            //Vector3d right = bottomRightScreen;
            //right.Subtract(topRightScreen);
            //double rightLength = right.Length();

            // Test for tile scale in view..
            wvp.TransformTo(TopLeft, topLeftScreen);
            wvp.TransformTo(BottomRight, bottomRightScreen);
            wvp.TransformTo(TopRight, topRightScreen);
            wvp.TransformTo(BottomLeft, bottomLeftScreen);
        
            Vector3d top = topLeftScreen;
            top.Subtract(topRightScreen);
            double topLength = top.Length();

            Vector3d bottom = bottomLeftScreen;
            bottom.Subtract(bottomRightScreen);
            double bottomLength = bottom.Length();

            Vector3d left = bottomLeftScreen;
            left.Subtract(topLeftScreen);
            double leftLength = left.Length();

            Vector3d right = bottomRightScreen;
            right.Subtract(topRightScreen);
            double rightLength = right.Length();


            float lengthMax = Math.Max(Math.Max(rightLength, leftLength), Math.Max(bottomLength, topLength));
            if (lengthMax < 300) // was 220
            {
                return false;
            }
            else
            {
                deepestLevel = Level > deepestLevel ? Level : deepestLevel;
            }


            return true;
        }
        
 //virtual public bool IsTileBigEnough(Device device)
 //       {
 //           //if (!IsTileInFrustum(frustum))
 //           //{
 //           //    outOfView = true;
 //           //    return false;
 //           //}

 //           if (level > 1)
 //           {
 //               if (this.Level < 25)
 //               {
 //                   deepestLevel = level > deepestLevel ? level : deepestLevel; 
 //                   return true;
 //                   bool a = true;
 //               }

 //               // Test for tile scale in view..
 //               topLeftScreen = TopLeft.Vector3;
 //               topLeftScreen.Project(Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);

 //               bottomRightScreen = BottomRight.Vector3;
 //               bottomRightScreen.Project(Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);

 //               topRightScreen = TopRight.Vector3;
 //               topRightScreen.Project(Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);

 //               bottomLeftScreen = BottomLeft.Vector3;
 //               bottomLeftScreen.Project(Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);

 //               Vector3 top = topLeftScreen;
 //               top.Subtract(topRightScreen);
 //               float topLength = top.Length();

 //               Vector3 bottom = bottomLeftScreen;
 //               bottom.Subtract(bottomRightScreen);
 //               float bottomLength = bottom.Length();

 //               Vector3 left = bottomLeftScreen;
 //               left.Subtract(topLeftScreen);
 //               float leftLength = left.Length();

 //               Vector3 right = bottomRightScreen;
 //               right.Subtract(topRightScreen);
 //               float rightLength = right.Length();




 //               float lengthMax = Math.Max(Math.Max(rightLength, leftLength), Math.Max(bottomLength, topLength));

 //               if (lengthMax < (400 - Tile.imageQuality)) // was 220
 //               {
 //                   //if (level > lastDeepestLevel || dataset.DataSetType == ImageSetType.Planet ||dataset.DataSetType == ImageSetType.Earth)
 //                   //if (!Earth3d.MainWindow.SolarSystemMode  && this.DemEnabled && level <= lastDeepestLevel && this is ToastTile)
 //                   //{
 //                   //    return true;
 //                   //}
 //                   //else
 //                   {
 //                       return false;
 //                   }
 //               }
 //               else
 //               {
 //                   deepestLevel = level > deepestLevel ? level : deepestLevel;
 //               }
 //           }
 //           return true;
 //       }

        public static int meshComplexity = 50;
        public static int imageQuality = 50;
        public static int lastDeepestLevel = 0;
        public static int deepestLevel = 0;
        //virtual public bool IsTileInFrustum(PlaneD[]frustum)
        //{
        //    InViewFrustum = false;
        //    Vector3d center = sphereCenter;

        //    if (this.Level < 2 && dataset.Projection == ProjectionType.Mercator)
        //    {
        //        return true;
        //    }

        //    Vector4d centerV4 = new Vector4d(center.X , center.Y , center.Z , 1f);
        //    Vector3d length = Vector3d.Create(sphereRadius, 0, 0);

        //    double rad = length.Length();
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (frustum[i].Dot(centerV4) + rad < 0)
        //        {
        //            return false;
        //        }
        //    }
        //    InViewFrustum = true;

        //    return true;  
        //}
        virtual public bool IsTileInFrustum(PlaneD[] frustum)
        {

            if (this.Level < 2 && (dataset.Projection == ProjectionType.Mercator || dataset.Projection == ProjectionType.Toast))
            {
            //    return true;
            }
            InViewFrustum = false;

            Vector4d centerV4 = new Vector4d(sphereCenter.X, sphereCenter.Y, sphereCenter.Z, 1f);

            for (int i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(centerV4)  < -sphereRadius)
                {
                    return false;
                }
            }
            InViewFrustum = true;

            return true;
        } 


        protected double sphereRadius;

        public double SphereRadius
        {
            get { return sphereRadius; }
        }

        protected Vector3d localOrigin;

        protected Vector3d sphereCenter = new Vector3d();

        public Vector3d SphereCenter
        {
            get { return sphereCenter; }
        }
 
        protected const double RC = (3.1415927 / 180.0);
        protected float radius = 1;
        protected Vector3d GeoTo3d(double lat, double lng, bool useLocalCenter)
        {

            if (dataset.DataSetType == ImageSetType.Panorama )
            {
                Vector3d retVal = Vector3d.Create(-(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

                return retVal;

            }
            else
            {
                lng -= 180;
                Vector3d retVal = Vector3d.Create((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

                return retVal;
            }

        }
     
        static protected int SubDivisions
        {
            get { return 32; /*DemEnabled ? 32 : 16; */ }
        }
        //protected static int subDivisions = 4;


        protected int TriangleCount = 0;
        //{
        //    get { return SubDivisions * SubDivisions * 2; }
        //}

        public virtual void OnCreateVertexBuffer(object sender, EventArgs e)
        {
 
        }

        public int RequestHits;
        public bool RequestPending = false;
        public bool errored = false;
        protected Imageset dataset;
        public Imageset Dataset
        {
            get
            {
                return dataset;
            }
            set
            {
                dataset = value;
            }
        }

   
        public String Key
        {
            get
            {
                return dataset.ImageSetID.ToString() + @"\" + Level.ToString() + @"\" + tileY.ToString() + "_" + tileX.ToString();
            }

        }
  
    
        // URL parameters
        //{0} ImageSetID
        //{1} level
        //{2} x tile id
        //{3} y tile id
        //{4} quadtree address (VE style)
        //{5} quadtree address (Google maps style)
        //{6} top left corner RA
        //{7} top left corner Dec
        //{8} bottom right corner RA
        //{9} bottom right corner dec
        //{10} bottom left corner RA
        //{11} bottom left corner dec
        //{12} top right corner RA
        //{13} top right corner dec

        //{X} - Tile X value
        //{Y} - Tile Y value
        //{L} - Tile Level
        //{Q} - Quad Key ID
        //{S} - Last Digit of Quadkey
        //
        public String URL
        {
            get
            {
            string returnUrl = dataset.Url;  
                
                if (dataset.Url.IndexOf("{1}") > -1)
                {
                    // Old style URL
                    if (dataset.Projection == ProjectionType.Mercator && !string.IsNullOrEmpty(dataset.QuadTreeTileMap))
                    {
                        returnUrl = String.Format(dataset.Url, this.GetServerID(), GetTileID());
                        if (returnUrl.IndexOf("virtualearth.net") > -1)
                        {
                            returnUrl += "&n=z";
                        }
                        return returnUrl;
                    }
                    else
                    {
                        return String.Format(dataset.Url, dataset.ImageSetID, Level, tileX, tileY);
                    }
                }
                returnUrl = returnUrl.Replace("{X}", this.tileX.ToString());
                returnUrl = returnUrl.Replace("{Y}", this.tileY.ToString());
                returnUrl = returnUrl.Replace("{L}", this.Level.ToString());
                int hash = 0;
                if (returnUrl.IndexOf("{S:0}") > -1)
                {
                    hash = 0;
                    returnUrl = returnUrl.Replace("{S:0}", "{S}");
                }
                if (returnUrl.IndexOf("{S:1}") > -1)
                {
                    hash = 1;
                    returnUrl = returnUrl.Replace("{S:1}", "{S}");
                }
                if (returnUrl.IndexOf("{S:2}") > -1)
                {
                    hash = 2;
                    returnUrl = returnUrl.Replace("{S:2}", "{S}");
                }
                if (returnUrl.IndexOf("{S:3}") > -1)
                {
                    hash = 3;
                    returnUrl = returnUrl.Replace("{S:3}", "{S}");
                }

                if (returnUrl.IndexOf("a{S}") > -1)
                {
                    returnUrl = returnUrl.Replace("a{S}", "r{S}");
                }

                if (returnUrl.IndexOf("h{S}") > -1)
                {
                    returnUrl = returnUrl.Replace("h{S}", "r{S}");
                }


                if (returnUrl.IndexOf("http://r{S}.ortho.tiles.virtualearth.net") > -1)
                {
                    returnUrl = returnUrl.Replace("http://r{S}.ortho.tiles.virtualearth.net", "http://ecn.t{S}.tiles.virtualearth.net");
                }

                
                string id = this.GetTileID();
                string server = "";

                if (id.Length == 0)
                {
                    server = hash.ToString();
                }
                else
                {
                    server = id.Substr(id.Length-1, 1).ToString();
                }

                //if (returnUrl == "http://r{S}.ortho.tiles.virtualearth.net/tiles/wtl00{Q}?g=121&band=wwt_rgb" && id.Length > 7 && (id.StartsWith("2") || id.StartsWith("3")))
                //{
                //    returnUrl = "http://r{S}.ortho.tiles.virtualearth.net/tiles/wtl00{Q}?g=120&band=wwt_jpg";
                //}


                returnUrl = returnUrl.Replace("{Q}", id);
                
                returnUrl = returnUrl.Replace("{S}", server);
                if (returnUrl.IndexOf("virtualearth.net") > -1)
                {
                    returnUrl += "&n=z";
                }
                return returnUrl;
            }
        }

        public String DemURL
        {
            get
            {
                if (dataset.Projection == ProjectionType.Mercator)
                {
                    string baseUrl = "http://cdn.worldwidetelescope.org/wwtweb/demtile.aspx?q={0},{1},{2},M";
                    if (!String.IsNullOrEmpty(dataset.DemUrl))
                    {
                        baseUrl = dataset.DemUrl;
                    }

                    //return string.Format(baseUrl, level.ToString(), x.ToString(), y.ToString());
                }


                if (dataset.DemUrl.IndexOf("{1}") > -1)
                {
                    return String.Format(dataset.DemUrl + "&new", Level, tileX, tileY);
                }

                string returnUrl = dataset.DemUrl;

                returnUrl = returnUrl.Replace("{X}", tileX.ToString());
                returnUrl = returnUrl.Replace("{Y}", tileY.ToString());
                returnUrl = returnUrl.Replace("{L}", Level.ToString());
                int hash = 0;
                if (returnUrl.IndexOf("{S:0}") > -1)
                {
                    hash = 0;
                    returnUrl = returnUrl.Replace("{S:0}", "{S}");
                }
                if (returnUrl.IndexOf("{S:1}") > -1)
                {
                    hash = 1;
                    returnUrl = returnUrl.Replace("{S:1}", "{S}");
                }
                if (returnUrl.IndexOf("{S:2}") > -1)
                {
                    hash = 2;
                    returnUrl = returnUrl.Replace("{S:2}", "{S}");
                }
                if (returnUrl.IndexOf("{S:3}") > -1)
                {
                    hash = 3;
                    returnUrl = returnUrl.Replace("{S:3}", "{S}");
                }


                string id = GetTileID();
                string server = "";

                if (id.Length == 0)
                {
                    server = hash.ToString();
                }
                else
                {
                    server = id.Substr(id.Length - 1, 1).ToString();
                }


                returnUrl = returnUrl.Replace("{Q}", id);

                returnUrl = returnUrl.Replace("{S}", server);

                return returnUrl;
            }
        }

        public int GetServerID()
        {
            int server = (this.tileX & 1) + ((this.tileY & 1) << 1);

            return (server);
        }

        string tileId = null;
        public string GetTileID()
        {
            if (tileId != null)
            {
                return tileId;
            }

            int netLevel = Level;
            int netX = tileX;
            int netY = tileY;

            if (dataset.Projection == ProjectionType.Equirectangular)
            {
                netLevel++;
                //netX = x*2;
                //netY = y*2;
            }

            string tileMap = this.dataset.QuadTreeTileMap;

            if (!string.IsNullOrEmpty(tileMap))
            {
                StringBuilder sb = new StringBuilder();

                for (int i = netLevel; i > 0; --i)
                {
                    int mask = 1 << (i - 1);
                    int val = 0;

                    if ((netX & mask) != 0)
                        val = 1;

                    if ((netY & mask) != 0)
                        val += 2;

                    sb.Append(tileMap.Substr(val,1));

                }
                tileId = sb.ToString();
                return tileId;
            }
            else
            {
                tileId = "0";
                return tileId;
            }
        }

        private int vertexCount;

        protected int VertexCount
        {
            get 
            {

                return vertexCount;
            }
            set
            {
                vertexCount = value;
            }
        }
        BlendState[] renderChildPart = null;
      
        public Tile()
        {
            renderChildPart = new BlendState[4];
            for (int i = 0; i < 4; i++ )
            {
                renderChildPart[i] = BlendState.Create(false, 500);
            }
        }
    }

//    public class Quad
//    {
//        public PointF TopLeft;
//        public PointF TopRight;
//        public PointF BottomLeft;
//        public PointF BottomRight;
//        public Quad()
//        {
//        }
//        public Quad(PointF topLeft, PointF topRight, PointF bttomLeft, PointF bottomRight)
//        {
//            this.TopLeft = topLeft;
//            this.TopRight = topRight;
//            this.BottomLeft = bttomLeft;
//            this.BottomRight = bottomRight;
//        }

//        public bool IntersectsRectangle(RectangleF rect)
//        {
//            if (rect.Contains(TopLeft))
//            {
//                return true;
//            }
//            if (rect.Contains(TopRight))
//            {
//                return true;
//            }
//            if (rect.Contains(BottomLeft))
//            {
//                return true;
//            }
//            if (rect.Contains(BottomRight))
//            {
//                return true;
//            }



//            // check all four sides
//            if (LineSegmentInresectsRectangle(rect, TopLeft, TopRight))
//            {
//                return true;
//            }

//            if (LineSegmentInresectsRectangle(rect, TopRight, BottomRight))
//            {
//                return true;
//            }

//            if (LineSegmentInresectsRectangle(rect, BottomRight, BottomLeft))
//            {
//                return true;
//            }

//            if (LineSegmentInresectsRectangle(rect, BottomLeft, TopLeft))
//            {
//                return true;
//            }
//            // check for full containment of rect inside quad

//            if (PointInQuad(new PointF(rect.Right, rect.Top)) &&
//                PointInQuad(new PointF(rect.Right, rect.Bottom)) &&
//                PointInQuad(new PointF(rect.Left, rect.Top)) &&
//                PointInQuad(new PointF(rect.Left, rect.Bottom)))
//            {
//                return true;
//            }   
            
//            return false;
//        }

//        public bool PointInQuad(PointF point)
//        {
//            if (!IsSameSideOfLine(TopLeft, TopRight, BottomRight, point))
//            {
//                return false;
//            }
//            if (!IsSameSideOfLine(TopRight, BottomRight, BottomLeft, point))
//            {
//                return false;
//            }
//            if (!IsSameSideOfLine(BottomRight, BottomLeft, TopLeft, point))
//            {
//                return false;
//            }
//            if (!IsSameSideOfLine(BottomLeft, TopLeft, TopRight, point))
//            {
//                return false;
//            }

//            return true;
//        }

//        public static bool LineSegmentInresectsRectangle(RectangleF rect, PointF seg1, PointF seg2)
//        {
//            if (LineSegmentCross(seg1, seg2, new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top)))
//            {
//                return true;
//            }
//            if (LineSegmentCross(seg1, seg2, new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom)))
//            {
//                return true;
//            }
//            if (LineSegmentCross(seg1, seg2, new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom)))
//            {
//                return true;
//            }
//            if (LineSegmentCross(seg1, seg2, new PointF(rect.Left, rect.Bottom), new PointF(rect.Left, rect.Top)))
//            {
//                return true;
//            }
//            return false;
//        }
//        /* (x1,y1),(x2,y2) defines a line.
//* If the points (ax,ay) and (bx,by) are on 
//* the same side of the line as each other, the 
//* function returns 1. Otherwise it returns 0.
//*/

//        //public static int IsSameSideOfLine(float x1, float y1, float x2, float y2, float ax, float ay, float bx, float by)
//        //{
//        //    return ((x1 - x2) * (ay - y2) - (y1 - y2) * (ax - x2)) * ((x1 - x2) * (by - y2) - (y1 - y2) * (bx - x2)) >= 0;
//        //}
//        public static bool IsSameSideOfLine(PointF seg1, PointF seg2, PointF a, PointF b)
//        {
//            return ((seg1.X - seg2.X) * (a.Y - seg2.Y) - (seg1.Y - seg2.Y) * (a.X - seg2.X)) * ((seg1.X - seg2.X) * (b.Y - seg2.Y) - (seg1.Y - seg2.Y) * (b.X - seg2.X)) >= 0;
//        }
//        /* This is wonderful for collision detection, for example :/*(ax1,ay1),(ax2,ay2) define a line segment A.
//     *(bx1,by1),(bx2,by2) define a line segment B.
//     * If the two line segments intercept, the function returns 1, else 0
//     */

//        //public static int LineSegmentCross(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
//        //{
//        //    return (!same_side_of_line(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2) && !same_side_of_line(bx1, by1, bx2, by2, ax1, ay1, ax2, ay2));
//        //}
//        public static bool LineSegmentCross(PointF a1, PointF a2, PointF b1, PointF b2)
//        {
//            return (!IsSameSideOfLine(a1, a2, b1, b2) && !IsSameSideOfLine(b1, b2, a1, a2));
//        }
//    }

    
}
