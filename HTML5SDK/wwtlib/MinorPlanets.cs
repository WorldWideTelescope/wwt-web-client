using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;
using System.Html.Data.Files;

namespace wwtlib
{
    class MinorPlanets
    {
        public static List<EOE> MPCList = new List<EOE>();
      
        static WebFile webMpcFile;

        public static void GetMpcFile(string url)
        {
            webMpcFile = new WebFile(url);
            webMpcFile.ResponseType = "blob";
            webMpcFile.OnStateChange = StarFileStateChange;
            webMpcFile.Send();
        }

        public static void StarFileStateChange()
        {
            if (webMpcFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webMpcFile.Message);
            }
            else if (webMpcFile.State == StateType.Received)
            {
                System.Html.Data.Files.Blob mainBlob = (System.Html.Data.Files.Blob)webMpcFile.GetBlob();
                FileReader chunck = new FileReader();
                chunck.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent e)
                {
                    ReadFromBin(new BinaryReader(new Uint8Array(chunck.Result)));
                    InitMPCVertexBuffer();
                };
                chunck.ReadAsArrayBuffer(mainBlob);
            }
        }

        private static void ReadFromBin(BinaryReader br)
        {
            MPCList = new List<EOE>();
            int len = (int)br.Length;
            EOE ee;
            try
            {
                while (br.Position < len)
                {
                    ee = EOE.Create(br);
                    MPCList.Add(ee);
                }
            }
            catch
            {
            }
            br.Close();
        }

        static bool initBegun = false;

        static BlendState[] mpcBlendStates = new BlendState[7];

        public static Texture starTexture = null;
        // ** Begin 
        public static void DrawMPC3D(RenderContext renderContext, float opacity, Vector3d centerPoint)
        {
            double zoom = renderContext.ViewCamera.Zoom;
            double distAlpha = ((Math.Log(Math.Max(1, zoom)) / Math.Log(4)) - 15.5) * 90;

            int alpha = Math.Min(255, Math.Max(0, (int)distAlpha));

        

            if (alpha > 254)
            {
                return;
            }


            if (mpcVertexBuffer == null)
            {
                if (starTexture == null)
                {
                    starTexture = Planets.LoadPlanetTexture("/images/starProfile.png");
                }
                for (int i = 0; i < 7; i++)
                {
                    mpcBlendStates[i] = BlendState.Create(false, 1000);
                }

                if (!initBegun)
                {
                    StartInit();
                    initBegun = true;
                }
                return;
            }

            Matrix3d offset = Matrix3d.Translation(Vector3d.Negate(centerPoint));
            Matrix3d world = Matrix3d.MultiplyMatrix(renderContext.World, offset);
            Matrix3d matrixWVP = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(world, renderContext.View), renderContext.Projection);

            matrixWVP.Transpose();


            Vector3d cam = Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.InvertMatrix(renderContext.World));

            //todo star profile texture

            if (mpcVertexBuffer != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    // mpcBlendStates[i].TargetState = ((Properties.Settings.Default.MinorPlanetsFilter & (int)Math.Pow(2, i)) != 0);
                    mpcBlendStates[i].TargetState = true;

                    if (mpcBlendStates[i].State)
                    {

                        KeplerPointSpriteShader.Use(renderContext, mpcVertexBuffer[i].VertexBuffer, starTexture.Texture2d, Colors.White,
                            opacity * mpcBlendStates[i].Opacity, false,
                            (float)(SpaceTimeController.JNow - KeplerVertex.baseDate), 0, renderContext.CameraPosition, 200f, .5f);
                     
                        renderContext.gl.drawArrays(GL.POINTS, 0, mpcVertexBuffer[i].Count);
                    }
                }
            }
        }          
 

    
        private static void StartInit()
        {
            GetMpcFile("http://cdn.worldwidetelescope.org/wwtweb/catalog.aspx?Q=mpcbin");
        }

        public static void InitMPCVertexBuffer()
        {
            try
            {
                if (mpcVertexBuffer == null)
                {
                    KeplerVertexBuffer[] mpcVertexBufferTemp = new KeplerVertexBuffer[7];

                    mpcCount = MinorPlanets.MPCList.Count;
                    //KeplerVertexBuffer11 temp = new KeplerVertexBuffer11(mpcCount, RenderContext11.PrepDevice);

                    List<KeplerVertex>[] lists = new List<KeplerVertex>[7];
                    for (int i = 0; i < 7; i++)
                    {
                        lists[i] = new List<KeplerVertex>();
                    }

                    foreach (EOE ee in MinorPlanets.MPCList)
                    {
                        int listID = 0;
                        if (ee.a < 2.5)
                        {
                            listID = 0;
                        }
                        else if (ee.a < 2.83)
                        {
                            listID = 1;
                        }
                        else if (ee.a < 2.96)
                        {
                            listID = 2;
                        }
                        else if (ee.a < 3.3)
                        {
                            listID = 3;
                        }
                        else if (ee.a < 5)
                        {
                            listID = 4;
                        }
                        else if (ee.a < 10)
                        {
                            listID = 5;
                        }
                        else
                        {
                            listID = 6;
                        }

                        KeplerVertex vert = new KeplerVertex();
                        vert.Fill(ee);

                        lists[listID].Add(vert);
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        mpcVertexBufferTemp[i] = KeplerVertexBuffer.Create(lists[i]);
                        mpcVertexBufferTemp[i].Unlock();
                    }

                    mpcVertexBuffer = mpcVertexBufferTemp;
                }
            }
            finally
            {
    
            }
        }

        static KeplerVertexBuffer[] mpcVertexBuffer = null;
        static int mpcCount = 0;
    }

}
