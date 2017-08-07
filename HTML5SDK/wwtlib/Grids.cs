using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;



namespace wwtlib
{
    public class Grids
    {

        static PositionTextureVertexBuffer galaxyImageVertexBuffer;
        static WebGLBuffer galaxyImageIndexBuffer = null;
        static int galaxyImageTriangleCount = 0;
        static Texture milkyWayImage = null;

        private static void CreateGalaxyImage(RenderContext renderContext)
        {
            if (milkyWayImage == null)
            {
                milkyWayImage = Planets.LoadPlanetTexture("http://cdn.worldwidetelescope.org/webclient/images/milkywaybar.jpg");
            }

  
            int subdivs = 50;
            
            double lat, lng;

            int index = 0;
            double latMin = 64;
            double latMax = -64;
            double lngMin = -64;
            double lngMax = 64;

            //// Create a vertex buffer 
            galaxyImageVertexBuffer = new PositionTextureVertexBuffer((subdivs + 1) * (subdivs + 1));
            PositionTexture[] verts = (PositionTexture[])galaxyImageVertexBuffer.Lock();

            int x1, y1;
            double latDegrees = latMax - latMin;
            double lngDegrees = lngMax - lngMin;
            double scaleFactor = 60800000.0;
            double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            Vector3d point;

            double textureStepX = 1.0f / subdivs;
            double textureStepY = 1.0f / subdivs;
            for (y1 = 0; y1 <= subdivs; y1++)
            {

                if (y1 != subdivs)
                {
                    lat = latMax - (textureStepY * latDegrees * (double)y1);
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subdivs; x1++)
                {
                    if (x1 != subdivs)
                    {
                        lng = lngMin + (textureStepX * lngDegrees * (double)x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = y1 * (subdivs + 1) + x1;
                    point = Vector3d.Create(lng * scaleFactor, 0, (lat - 28) * scaleFactor);
                    point.RotateY(213.0 / 180 * Math.PI);
                    point.RotateZ((-62.87175) / 180 * Math.PI);
                    point.RotateY((-192.8595083) / 180 * Math.PI);
                    point.RotateX(ecliptic);
                    verts[index] = PositionTexture.CreatePosRaw(point, (float)(1f - x1 * textureStepX), (float)(/*1f - */(y1 * textureStepY)));
                    //verts[index].Position = point;
                    //verts[index].Tu = (float)(1f - x1 * textureStepX);
                    //verts[index].Tv = (float)(/*1f - */(y1 * textureStepY));
                }
            }
            galaxyImageVertexBuffer.Unlock();
            galaxyImageTriangleCount = (subdivs) * (subdivs) * 2;
            Uint16Array ui16array = new Uint16Array(subdivs * subdivs * 6);

            UInt16[] indexArray = (UInt16[])(object)ui16array;

            for (y1 = 0; y1 < subdivs; y1++)
            {
                for (x1 = 0; x1 < subdivs; x1++)
                {
                    index = (y1 * subdivs * 6) + 6 * x1;
                    // First triangle in quad
                    indexArray[index] = (ushort)(y1 * (subdivs + 1) + x1);
                    indexArray[index + 2] = (ushort)((y1 + 1) * (subdivs + 1) + x1);
                    indexArray[index + 1] = (ushort)(y1 * (subdivs + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexArray[index + 3] = (ushort)(y1 * (subdivs + 1) + (x1 + 1));
                    indexArray[index + 5] = (ushort)((y1 + 1) * (subdivs + 1) + x1);
                    indexArray[index + 4] = (ushort)((y1 + 1) * (subdivs + 1) + (x1 + 1));
                }
            }
            galaxyImageIndexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, galaxyImageIndexBuffer);
            Tile.PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, ui16array, GL.STATIC_DRAW);
        }

        public static void DrawGalaxyImage(RenderContext renderContext, float opacity)
        {
            if (galaxyImageIndexBuffer == null)
            {
                CreateGalaxyImage(renderContext);
            }

 //           renderContext.setRasterizerState(TriangleCullMode.Off);
//            renderContext.BlendMode = BlendMode.Additive;

            double zoom = renderContext.ViewCamera.Zoom;
            double log = Math.Log(Math.Max(1, zoom)) / Math.Log(4);
            double distAlpha = ((log) - 14) * 128;

            int alpha = (int)(Math.Min(255, (int)Math.Max(0, distAlpha)) * opacity);

            //           renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, milkyWayImage.ResourceView);
            //renderContext.SetupBasicEffect(BasicEffect.TextureColorOpacity, opacity, Color.FromArgb(alpha, alpha, alpha, alpha));
            //if (galaxyImageInputLayout == null)
            //{
            //    galaxyImageInputLayout = new SharpDX.Direct3D11.InputLayout(device, renderContext.Shader.InputSignature, new[]
            //    {
            //        new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
            //        new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       12, 0),
            //    });
            //}

            //renderContext.Device.ImmediateContext.InputAssembler.InputLayout = galaxyImageInputLayout;
            //renderContext.SetVertexBuffer(galaxyImageVertexBuffer);
            //renderContext.SetIndexBuffer(galaxyImageIndexBuffer);
            //device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            //renderContext.PreDraw();
            //device.ImmediateContext.DrawIndexed(galaxyImageTriangleCount * 3, 0, 0);


            TileShader.Use(renderContext, galaxyImageVertexBuffer.VertexBuffer, galaxyImageIndexBuffer, milkyWayImage.Texture2d, (float)opacity, true);
            renderContext.gl.drawElements(GL.TRIANGLES, galaxyImageTriangleCount * 3, GL.UNSIGNED_SHORT, 0);
        }

        public static void DrawStars3D(RenderContext renderContext, float opacity)
        {
            double zoom = renderContext.ViewCamera.Zoom;
 
            double distAlpha = Math.Max(Math.Min(255, (Math.Log(zoom) - 15.5) * 40.8), 0);

            int alpha = Math.Min(255, Math.Max(0, (int)distAlpha));
            if (alpha > 254)
            {
                return;
            }

    
            alpha = (int)((255 - alpha) * opacity);

            if (starSprites == null)
            {
                InitStarVertexBuffer(renderContext);
            }

            if (starSprites != null)
            {
                starSprites.Draw(renderContext, alpha / 255.0f, false);
            }
        }

        static PointList starSprites = null;

        static int starCount = 0;
        static bool starsDownloading = false;

        public static void InitStarVertexBuffer(RenderContext renderContext)
        {
            if (!starsDownloading)
            {
                GetStarFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=hipparcos");
                starsDownloading = true;
            }

            if (starSprites == null && starCount > 0)
            {
                double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                int count = stars.Count;
                starCount = count;

                starSprites = new PointList(renderContext);
                starSprites.DepthBuffered = false;
                foreach (Star star in stars)
                {
                    Vector3d pos = Coordinates.RADecTo3dAu(star.RA, star.Dec, star.Distance);
                    pos.RotateX(ecliptic);
                    star.Position = pos;
                    double radDec = (1200000) / Math.Pow(1.6, star.AbsoluteMagnitude);
                    starSprites.AddPoint(pos, star.Col, new Dates(0, 1), (float)radDec*100);      
                }
            }
        }

        static List<Star> stars = null;
        static Dictionary<int, Star> hipparcosIndex = new Dictionary<int, Star>();
        static double limitingMagnitude = 16;
        static public void InitializeStarDB(string text)
        {

            if (stars == null)
            {
                if (stars == null)
                {
                    stars = new List<Star>();
                    string[] rows = text.Split("\r\n");
                    Star star;
                    foreach (string row in rows)
                    {
                        string line = row;

                        star = new Star(line);
                        if (star.Magnitude < limitingMagnitude && star.Par > .001)
                        {
                            stars.Add(star);
                            hipparcosIndex[star.ID] =  star;
                        }
                    }

                    //// Write Binary file
                    //DumpStarBinaryFile(@"c:\hip.bin");
                    starCount = stars.Count;
                }
            }
        }

        static WebFile webFileStar;

        public static void GetStarFile(string url)
        {
            webFileStar = new WebFile(url);
            webFileStar.OnStateChange = StarFileStateChange;
            webFileStar.Send();
        }

        public static void StarFileStateChange()
        {
            if (webFileStar.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFileStar.Message);
            }
            else if (webFileStar.State == StateType.Received)
            {
                InitializeStarDB(webFileStar.GetText());   
            }

        }

        static WebFile webFileGalaxy;

        public static void GetGalaxyFile(string url)
        {
            webFileGalaxy = new WebFile(url);
            webFileGalaxy.ResponseType = "blob";
            webFileGalaxy.OnStateChange = GalaxyFileStateChange;
            webFileGalaxy.Send();
        }

        public static void GalaxyFileStateChange()
        {
            if (webFileGalaxy.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFileGalaxy.Message);
            }
            else if (webFileGalaxy.State == StateType.Received)
            {
                System.Html.Data.Files.Blob mainBlob = (System.Html.Data.Files.Blob)webFileGalaxy.GetBlob();
                FileReader chunck = new FileReader();
                chunck.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent e)
                {
                    BinaryReader br = new BinaryReader(new Uint8Array(chunck.Result));


                    InitializeCosmos(br);
                };
                chunck.ReadAsArrayBuffer(mainBlob);
            }
        }


        static PointList[] cosmosSprites;
        static Texture[] galaxyTextures = null;
        static int[] galaxyVertexCounts = null;
        static bool largeSet = true;
        static bool cosmosReady = false;
        public static void DrawCosmos3D(RenderContext renderContext, float opacity)
        {
            GL device = renderContext.gl;
            double zoom = renderContext.ViewCamera.Zoom;
            double distAlpha = ((Math.Log(Math.Max(1, zoom))/Math.Log(4)) - 15.5) * 90;

            int alpha = Math.Min(255, Math.Max(0, (int)distAlpha));

            if (alpha < 3)
            {
                return;
            }

            InitCosmosVertexBuffer();

            if (galaxyTextures == null)
            {
                if (largeSet)
                {
                    galaxyTextures = new Texture[256];
                    for (int i = 0; i < 256; i++)
                    {
                        string num = i.ToString();

                        while (num.Length < 4)
                        {
                            num = "0" + num;
                        }


                        string name = string.Format("http://cdn.worldwidetelescope.org/webclient/images/gal_{0}.jpg", num);

                        galaxyTextures[i] = Planets.LoadPlanetTexture(name);

                    }
                }
            }

            if (cosmosReady)
            {
                int count = 256;
                for (int i = 0; i < count; i++)
                {

                    //cosmosSprites[i].MinPointSize = 1;
                    cosmosSprites[i].DrawTextured(renderContext, galaxyTextures[i], (alpha * opacity) / 255.0f);
                   // cosmosSprites[i].Draw(renderContext,  (alpha * opacity) / 255.0f, false);
                }
            }

        }


        public static void InitCosmosVertexBuffer()
        {

            if (cosmosSprites == null)
            {
                DownloadCosmosFile();
            }

        }

        private static void CreateCosmosVertexBuffer(RenderContext renderContext)
        {
            GL device = Tile.PrepDevice;

            int bucketCount = 256;

            if (cosmosSprites != null)
            {
                for (int ij = 0; ij < bucketCount; ij++)
                {
                    if (cosmosSprites[ij] != null)
                    {
                       cosmosSprites[ij] = null;
                    }
                }
            }
            cosmosSprites = null;
            double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            cosmosSprites = new PointList[bucketCount];

            int[] indexList = new int[bucketCount];
            for (int i = 0; i < bucketCount; i++)
            {
                int count = galaxyVertexCounts[i];
                cosmosSprites[i] = new PointList(renderContext);
                cosmosSprites[i].DepthBuffered = false;
                indexList[i] = 0;
            }

            foreach (Galaxy galaxy in cosmos)
            {
                int bucket = galaxy.eTypeBucket;
                int index = indexList[bucket];

                Vector3d pos = Coordinates.RADecTo3dAu(galaxy.RA, galaxy.Dec, (galaxy.Distance * UiTools.AuPerParsec * 1000000.0) / .73);
                pos.RotateX(ecliptic);
                galaxy.Position = pos;
                cosmosSprites[bucket].AddPoint(pos, Colors.White, new Dates(0, 1), (float)(1000000000f * galaxy.Size*100));
                indexList[bucket]++;
            }

            cosmosReady = true;
        }


        static List<Galaxy> cosmos = null;
        public static void InitializeCosmos(BinaryReader br)
        {
            int max = (int)Math.Pow(100, 2.849485002);

            if (cosmos == null)
            {
                galaxyVertexCounts = new int[largeSet ? 256 : 20];
                if (cosmos == null)
                {
                    cosmos = new List<Galaxy>();

                    Galaxy galaxy;
                    try
                    {
                        int count = 0;
                        while (br.Position < br.Length )
                        {
                            galaxy = new Galaxy(br);
                            cosmos.Add(galaxy);
                            galaxyVertexCounts[galaxy.eTypeBucket]++;
                            count++;
                        }
                    }
                    catch
                    {
                    }
                    br.Close();
                }

                CreateCosmosVertexBuffer(WWTControl.Singleton.RenderContext);
            }
        }

        static bool downloadingGalaxy = false;

        internal static bool DownloadCosmosFile()
        {
            if (!downloadingGalaxy)
            {
                GetGalaxyFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=cosmosnewbin");
                downloadingGalaxy = true;
            }
            return false;
        }


        static SimpleLineList equLineList;
        public static bool DrawEquitorialGrid(RenderContext renderContext, float opacity, Color drawColor)
        {
            if (equLineList == null)
            {
                equLineList = new SimpleLineList();
                equLineList.DepthBuffered = false;

                for (double hour = 0; hour < 24; hour++)
                {
                    for (double dec = -80; dec < 80; dec += 2)
                    {
                        equLineList.AddLine(Coordinates.RADecTo3dAu(hour, dec, 1), Coordinates.RADecTo3dAu(hour, dec + 2, 1));
                    }
                }


                for (double dec = -80; dec <= 80; dec += 10)
                {
                    for (double hour = 0; hour < 23.8; hour += .2)
                    {

                        equLineList.AddLine(Coordinates.RADecTo3dAu(hour, dec, 1), Coordinates.RADecTo3dAu(hour + .2, dec, 1));
                        //todo fix for color bright
                    }
                }


                int counter = 0;
                for (double ra = 0; ra < 24; ra += .25)
                {
                    double dec = 0.5;

                    switch (counter % 4)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 3:
                        case 1:
                            dec = .25;
                            break;
                    }
                    counter++;

                    equLineList.AddLine(Coordinates.RADecTo3dAu(ra, dec, 1), Coordinates.RADecTo3dAu(ra, -dec, 1));
                }
                counter = 0;
                for (double ra = 0; ra < 24; ra += 3)
                {
                    counter = 0;
                    for (double dec = -80; dec <= 80; dec += 1)
                    {
                        double width = 0.5 / 30;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5 / 15;
                                break;
                        }

                        counter++;

                        equLineList.AddLine(Coordinates.RADecTo3dAu(ra + width, dec, 1), Coordinates.RADecTo3dAu(ra - width, dec, 1));
                    }
                }
            }

            equLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }


        static Text3dBatch EquTextBatch;
        public static bool DrawEquitorialGridText(RenderContext renderContext, float opacity, Color drawColor)
        {
            MakeEquitorialGridText();

            EquTextBatch.Draw(renderContext, opacity, drawColor);
            return true;
        }

        private static void MakeEquitorialGridText()
        {
            if (EquTextBatch == null)
            {
                EquTextBatch = new Text3dBatch(30);
                int index = 0;

                for (int ra = 0; ra < 24; ra++)
                {
                    string text = ra.ToString() + " hr";
                    if (ra < 10)
                    {
                        text = "  " + ra.ToString() + " hr";
                    }

                    EquTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(ra + 0.005, 0.4, 1), Coordinates.RADecTo3dAu(ra + 0.005, 0.5, 1), text, 45, .00018));
                }

                index = 0;
                for (double ra = 0; ra < 24; ra += 3)
                {

                    for (double dec = -80; dec <= 80; dec += 10)
                    {
                        if (dec == 0)
                        {
                            continue;
                        }
                        string text = dec.ToString();
                        if (dec > 0)
                        {
                            text = "  +" + dec.ToString();
                            EquTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(ra, dec - .4, 1), Coordinates.RADecTo3dAu(ra, dec - .3, 1), text, 45, .00018));
                        }
                        else
                        {
                            text = "  - " + text.Substr(1);
                            EquTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(ra, dec + .4, 1), Coordinates.RADecTo3dAu(ra, dec + .5, 1), text, 45, .00018));
                        }

                        index++;
                    }
                }
            }
        }


        static int EclipticCount = 0;
        static int EclipticYear = 0;

        static double[] monthDays = new double[] { 31, 28.2421, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        static SimpleLineList eclipticOverviewLineList;


        public static bool DrawEcliptic(RenderContext renderContext, float opacity, Color drawColor)
        {
            Color col = drawColor;

            int year = SpaceTimeController.Now.GetUTCFullYear();

            if (eclipticOverviewLineList == null || year != EclipticYear)
            {


                if (eclipticOverviewLineList != null)
                {
                    eclipticOverviewLineList.Clear();
                    eclipticOverviewLineList = null;
                }

                EclipticYear = year;
                double obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
                Matrix3d mat = Matrix3d.RotationX((-obliquity / 360.0 * (Math.PI * 2)));


                double daysPerYear = 365.25;



                if (DT.IsLeap(year, true))
                {
                    monthDays[1] = 29;

                    daysPerYear = 366;
                }
                else
                {
                    monthDays[1] = 28;
                    daysPerYear = 365;
                }
                int count = 2 * (int)daysPerYear;
                EclipticCount = (int)daysPerYear;
                double jYear = SpaceTimeController.UtcToJulian(new Date(year, 0, 1, 12, 0, 0));


                int index = 0;
                double d = 0;

                eclipticOverviewLineList = new SimpleLineList();
                eclipticOverviewLineList.DepthBuffered = false;
                for (int m = 0; m < 12; m++)
                {
                    int daysThisMonth = (int)monthDays[m];
                    for (int i = 0; i < daysThisMonth; i++)
                    {
                        AstroRaDec sunRaDec = Planets.GetPlanetLocationJD("Sun", jYear);

                        COR sunEcliptic = CT.Eq2Ec(sunRaDec.RA, sunRaDec.Dec, obliquity);

                        d = sunEcliptic.X;

                        double width = .005f;
                        if (i == 0)
                        {
                            width = .01f;
                        }
                        double dd = d;// +180;

                        eclipticOverviewLineList.AddLine(
                                    Vector3d.TransformCoordinate( Vector3d.Create((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                    width,
                                    (Math.Sin((dd * Math.PI * 2.0) / 360))), mat),
                                    Vector3d.TransformCoordinate( Vector3d.Create((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                    -width,
                                    (Math.Sin((dd * Math.PI * 2.0) / 360))), mat)
                                                         );


                        index++;
                        jYear += 1;
                    }
                    d += monthDays[m];
                }

            }


            eclipticOverviewLineList.DrawLines(renderContext, opacity, drawColor);
            return true;
        }

        static int EclipticTextYear = 0;
        static Text3dBatch EclipOvTextBatch;
        public static bool DrawEclipticText(RenderContext renderContext, float opacity, Color drawColor)
        {
            MakeEclipticText();

            EclipOvTextBatch.Draw(renderContext, opacity, drawColor);

            return true;
        }

        private static void MakeEclipticText()
        {
            int year = SpaceTimeController.Now.GetUTCFullYear();

            if (EclipOvTextBatch == null)
            {
                EclipOvTextBatch = new Text3dBatch(80);

                EclipticTextYear = year;
                double obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
                Matrix3d mat = Matrix3d.RotationX((-obliquity / 360.0 * (Math.PI * 2)));

                double daysPerYear = 365.25;

                if (DT.IsLeap(year,true))
                {
                    monthDays[1] = 29;

                    daysPerYear = 366;
                }
                else
                {
                    monthDays[1] = 28;
                    daysPerYear = 365;
                }
                int count = 2 * (int)daysPerYear;
                EclipticCount = (int)daysPerYear;
                double jYear = SpaceTimeController.UtcToJulian(new Date(year, 0, 1, 12, 0, 0));


                int index = 0;
                double d = 0;

                for (int m = 0; m < 12; m++)
                {
                    int daysThisMonth = (int)monthDays[m];
                    for (int i = 0; i < daysThisMonth; i++)
                    {
                        AstroRaDec sunRaDec = Planets.GetPlanetLocationJD("Sun", jYear);

                        COR sunEcliptic = CT.Eq2Ec(sunRaDec.RA, sunRaDec.Dec, obliquity);

                        d = sunEcliptic.X;

                        double dd = d;// +180;

                        if (i == Math.Floor(daysThisMonth / 2.0))
                        {
                            Vector3d center = Vector3d.TransformCoordinate( Vector3d.Create((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                                         .025f,
                                                         (Math.Sin((dd * Math.PI * 2.0) / 360))), mat);
                            Vector3d up = Vector3d.TransformCoordinate( Vector3d.Create((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                                         .045f,
                                                         (Math.Sin((dd * Math.PI * 2.0) / 360))), mat);
                            up.Subtract(center);

                            up.Normalize();
                            EclipOvTextBatch.Add(new Text3d(center, up, monthNames[m], 80, .000159375));

                        }


                        index++;

                        index++;
                        jYear += 1;
                    }
                    d += monthDays[m];
                }
            }
        }


        static SimpleLineList precLineList;

        static Text3dBatch PrecTextBatch;
        public static bool DrawPrecessionChart(RenderContext renderContext, float opacity, Color drawColor)
        {
            MakePrecessionChart();

            PrecTextBatch.Draw(renderContext, opacity, drawColor);

            precLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }

        private static void MakePrecessionChart()
        {
            double obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
            Matrix3d mat = Matrix3d.RotationX((obliquity / 360.0 * (Math.PI * 2)));
            Color col = Colors.White;
            if (precLineList == null)
            {
                precLineList = new SimpleLineList();
                precLineList.DepthBuffered = false;

                for (double l = 0; l < 360; l++)
                {
                    double b = 90 - obliquity;
                    precLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu((l + 1) / 15, b, 1), mat));
                }

                for (double l = -12000; l < 13000; l += 2000)
                {

                    double b = 90 - obliquity;
                    double p = -((l - 2000) / 25772 * 24) - 6;
                    precLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(p, b - .5, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(p, b + .5, 1), mat));
                }
            }
            if (PrecTextBatch == null)
            {
                PrecTextBatch = new Text3dBatch(50);

                int index = 0;
                for (double l = -12000; l < 13000; l += 2000)
                {
                    double b = 90 - obliquity + 3;

                    double p = -((l - 2000) / 25772 * 24) - 6;
                    string text = l.ToString();

                    if (l == 0)
                    {
                        b = 90 - obliquity + 2;
                        text = "1 CE";
                    }
                    else if (l < 0)
                    {

                        text = "  " + (Math.Abs(l).ToString()) + " BCE";
                    }
                    else
                    {
                        text = (Math.Abs(l).ToString()) + " CE";
                    }

                    if (text.Length == 9)
                    {
                        text = "   " + text;
                    }

                    PrecTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(p, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(p + .01, b, 1), mat), text, 75, .00015));
                }
            }
            return;
        }

        static SimpleLineList altAzLineList;
        public static bool DrawAltAzGrid(RenderContext renderContext, float opacity, Color drawColor)
        {


            Coordinates zenithAltAz = new Coordinates(0, 0);
            Coordinates zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);

            double raPart = -((zenith.RA + 6) / 24.0 * (Math.PI * 2));
            double decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
            string raText = Coordinates.FormatDMS(zenith.RA);
            Matrix3d mat = Matrix3d.RotationY((float)-raPart);
            mat.Multiply(Matrix3d.RotationX((float)decPart));
            mat.Invert();

            if (altAzLineList == null)
            {
                altAzLineList = new SimpleLineList();
                altAzLineList.DepthBuffered = false;

                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        altAzLineList.AddLine(Coordinates.RADecTo3dAu(l / 15 , b, 1), Coordinates.RADecTo3dAu(l / 15 , b + 2, 1));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        altAzLineList.AddLine(Coordinates.RADecTo3dAu(l / 15 , b, 1), Coordinates.RADecTo3dAu((l + 5) / 15 , b, 1));
                    }
                }

                int counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    double b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    altAzLineList.AddLine(Coordinates.RADecTo3dAu(l / 15 , b, 1), Coordinates.RADecTo3dAu(l / 15, -b, 1));
                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        double width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        altAzLineList.AddLine(Coordinates.RADecTo3dAu((l + width) / 15 , b, 1), Coordinates.RADecTo3dAu((l - width) / 15 , b, 1));
                    }
                }
            }

            Matrix3d matOldWorld = renderContext.World.Clone();
            Matrix3d matOldWorldBase = renderContext.WorldBase.Clone();
            renderContext.WorldBase = Matrix3d.MultiplyMatrix(mat, renderContext.World);
            renderContext.World = renderContext.WorldBase.Clone();
            renderContext.MakeFrustum();

            altAzLineList.ViewTransform = Matrix3d.InvertMatrix(mat);
 
            altAzLineList.DrawLines(renderContext, opacity, drawColor);

            renderContext.WorldBase = matOldWorldBase;
            renderContext.World = matOldWorld;
            renderContext.MakeFrustum();
            return true;
        }

        static Text3dBatch AltAzTextBatch;
        public static bool DrawAltAzGridText(RenderContext renderContext, float opacity, Color drawColor)
        {
            Coordinates zenithAltAz = new Coordinates(0, 0);
            Coordinates zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);

            double raPart = -((zenith.RA -6) / 24.0 * (Math.PI * 2));
            double decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
            string raText = Coordinates.FormatDMS(zenith.RA);
            Matrix3d mat = Matrix3d.RotationY((float)-raPart - Math.PI);
            mat.Multiply(Matrix3d.RotationX((float)decPart));
            mat.Invert();

            MakeAltAzGridText();


            Matrix3d matOldWorld = renderContext.World.Clone();
            Matrix3d matOldWorldBase = renderContext.WorldBase.Clone();

            renderContext.WorldBase = Matrix3d.MultiplyMatrix(mat, renderContext.World);
            renderContext.World = renderContext.WorldBase.Clone();
            renderContext.MakeFrustum();

            AltAzTextBatch.ViewTransform = Matrix3d.InvertMatrix(mat);
            AltAzTextBatch.Draw(renderContext, opacity, drawColor);

            renderContext.WorldBase = matOldWorldBase;
            renderContext.World = matOldWorld;
            renderContext.MakeFrustum();
            return true;
        }

        private static void MakeAltAzGridText()
        {
            Color drawColor = Colors.White;

            int index = 0;
            if (AltAzTextBatch == null)
            {
                AltAzTextBatch = new Text3dBatch(30);
                for (double l = 0; l < 360; l += 10)
                {
                    string text = "       " + l.ToString();
                    if (l < 10)
                    {
                        text = "   " + l.ToString();
                    }
                    else if (l < 100)
                    {
                        text = "     " + l.ToString();
                    }
                    double lc = 360 - l;
                    AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(lc / 15 - 6, .4, 1), Coordinates.RADecTo3dAu(lc / 15 - 6, .5, 1), text, 75, .00018));
                }

                index = 0;
                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        string text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b.ToString();
                            AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(l / 15, b - .4, 1), Coordinates.RADecTo3dAu(l / 15, b - .3, 1), text, 75, .00018));
                        }
                        else
                        {
                            text = "  - " + text.Substr(1);
                            AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3dAu(l / 15, b + .4, 1), Coordinates.RADecTo3dAu(l / 15, b + .5, 1), text, 75, .00018));
                        }
                        index++;
                    }
                }
            }
            return;
        }

        static SimpleLineList eclipticLineList;

        public static bool DrawEclipticGrid(RenderContext renderContext, float opacity, Color drawColor)
        {
            if (eclipticLineList == null)
            {
                eclipticLineList = new SimpleLineList();
                eclipticLineList.DepthBuffered = false;

                double obliquity = Coordinates.MeanObliquityOfEcliptic(2451545);
                Matrix3d mat = Matrix3d.RotationX((-obliquity / 360.0 * (Math.PI * 2)));

        
                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b + 2, 1), mat));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu((l + 5) / 15, b, 1), mat));
                    }
                }

                int counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    double b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, -b, 1), mat));

                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        double width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu((l + width) / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu((l - width) / 15, b, 1), mat));
                    }
                }
            }

            eclipticLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }
        static Text3dBatch EclipticTextBatch;
        public static bool DrawEclipticGridText(RenderContext renderContext, float opacity, Color drawColor)
        {
            MakeEclipticGridText();

            EclipticTextBatch.Draw(renderContext, opacity, drawColor);

            return true;
        }

        private static void MakeEclipticGridText()
        {
            Color drawColor = Colors.White;
            double obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
            Matrix3d mat = Matrix3d.RotationX((float)(-obliquity / 360.0 * (Math.PI * 2)));

            if (EclipticTextBatch == null)
            {
                EclipticTextBatch = new Text3dBatch(30);
                for (double l = 0; l < 360; l += 10)
                {
                    string text = "       " + l.ToString();
                    if (l < 10)
                    {
                        text = "   " + l.ToString();
                    }
                    else if (l < 100)
                    {
                        text = "     " + l.ToString();
                    }
                    EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, .4, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, .5, 1), mat), text, 75, .00018));
                }

                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        string text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b.ToString();
                            EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b - .4, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b - .3, 1), mat), text, 75, .00018));
                        }
                        else
                        {
                            text = "  - " + text.Substr(1);
                            EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b + .4, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3dAu(l / 15, b + .5, 1), mat), text, 75, .00018));
                        }
                    }
                }
            }
            return;
        }

        static SimpleLineList galLineList;

        public static bool DrawGalacticGrid(RenderContext renderContext, float opacity, Color drawColor)
        {
            if (galLineList == null)
            {
                galLineList = new SimpleLineList();
                galLineList.DepthBuffered = false;

                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l, b + 2));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l + 5, b));
                    }
                }

                int counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    double b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l, -b));
                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        double width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l + width, b), Coordinates.GalacticTo3dDouble(l - width, b));
                    }
                }
            }

            galLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }
        static Text3dBatch GalTextBatch;
        public static bool DrawGalacticGridText(RenderContext renderContext, float opacity, Color drawColor)
        {
            MakeGalacticGridText();

            GalTextBatch.Draw(renderContext, opacity, drawColor);
            return true;
        }

        private static void MakeGalacticGridText()
        {
            if (GalTextBatch == null)
            {

                GalTextBatch = new Text3dBatch(30);
                for (int l = 0; l < 360; l += 10)
                {
                    string text = "       " + l.ToString();
                    if (l < 10)
                    {
                        text = "   " + l.ToString();
                    }
                    else if (l < 100)
                    {
                        text = "     " + l.ToString();
                    }
                    GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, 0.4), Coordinates.GalacticTo3dDouble(l, 0.5), text, 75, .00018));
                }

                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        string text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b.ToString();
                            GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, b - .4), Coordinates.GalacticTo3dDouble(l, b - .3), text, 75, .00018));
                        }
                        else
                        {
                            text = "  - " + text.Substr(1);
                            GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, b + .4), Coordinates.GalacticTo3dDouble(l, b + .5), text, 75, .00018));
                        }
                    }
                }
            }
        }
    }
}
