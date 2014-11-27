using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;
namespace wwtlib
{
    public class Orbit
    {
        private EOE elements = null;

        Color orbitColor = Colors.White;
        float scale;
        public Orbit(EOE elements, int segments, Color color, float thickness, float scale)
        {
            this.elements = elements;
            this.segmentCount = segments;
            this.orbitColor = color;
            this.scale = scale;
        }
        public void CleanUp()
        {
            //if (orbitVertexBuffer != null)
            //{
            //    orbitVertexBuffer.Dispose();
            //    GC.SuppressFinalize(orbitVertexBuffer);
            //    orbitVertexBuffer = null;
            //}
        }

        public void InitVertexBuffer(RenderContext renderContext)
        {
            //try
            //{
            //    if (orbitVertexBuffer == null)
            //    {

            //        VertexBuffer temp = new VertexBuffer(typeof(KeplerVertex), segmentCount, device, Usage.WriteOnly, KeplerVertex.Format, Tile.PoolToUse);
            //        KeplerVertex[] points = (KeplerVertex[])temp.Lock(0, 0); // Lock the buffer (which will return our structs)
            //        for (int i = 0; i < segmentCount; i++)
            //        {
            //            points[i].Fill(elements);
            //            points[i].orbitPos = (float)i / (float)(segmentCount - 1);
            //            points[i].a /= scale;
            //            points[i].Col = orbitColor.ToArgb();
            //        }

            //        temp.Unlock();
            //        orbitVertexBuffer = temp;
            //    }
            //}
            //finally
            //{
            //}
        }

        static bool initBegun = false;
        // ** Begin 
        public void Draw3D(RenderContext renderContext, float opacity, Vector3d centerPoint)
        {



        //    Device device = renderContext.Device;
        //    double zoom = Earth3d.MainWindow.ZoomFactor;
        //    double distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;
        //    //double distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 14) * 30 + 24;

        //    int alpha = Math.Min(255, Math.Max(0, (int)distAlpha));


        //    if (alpha > 254)
        //    {
        //        return;
        //    }


        //    if (orbitVertexBuffer == null)
        //    {
        //        KeplerShader.MakeVertexShader(device);
        //        InitVertexBuffer(renderContext.Device);
        //        return;
        //    }
        //    //device.DrawUserPrimitives(PrimitiveType.LineList, segments, points);


        //    //       Matrix savedWorld = device.Transform.World;

        //    //        Matrix offset = Matrix.Translation(-centerPoint.Vector3);

        //    //  device.Transform.World = device.Transform.World * offset;
        //    Vector3 cam = Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector3;


        //    double M = elements.n * (SpaceTimeController.JNow - elements.T) * 0.01745329251994;
        //    double F = 1;
        //    if (M < 0)
        //    {
        //        F = -1;
        //    }
        //    M = Math.Abs(M) / (2 * Math.PI);
        //    M = (M - (int)(M)) * 2 * Math.PI * F;


        //    KeplerShader.UseShader(device, SpaceTimeController.JNow, cam, opacity, (float)M);

        //    device.RenderState.PointSpriteEnable = true;
        //    device.RenderState.PointScaleEnable = true;

        //    device.RenderState.PointScaleA = 0;
        //    device.RenderState.PointScaleB = 0;
        //    device.RenderState.PointScaleC = 100f;
        //    device.RenderState.ZBufferEnable = true;
        //    device.RenderState.ZBufferWriteEnable = false;
        //    device.SetTexture(0, null);

        //    device.SetStreamSource(0, orbitVertexBuffer, 0);
        //    device.VertexFormat = KeplerVertex.Format;

        //    device.RenderState.CullMode = Cull.None;
        //    device.RenderState.AlphaBlendEnable = true;
        //    device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
        //    device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;

        //    //device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.One;




        //    device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
        //    device.TextureState[0].ColorOperation = TextureOperation.Modulate;
        //    device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
        //    device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
        //    device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
        //    device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
        //    device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

        //    device.TextureState[1].ColorOperation = TextureOperation.SelectArg1;
        //    device.TextureState[1].ColorArgument1 = TextureArgument.Current;
        //    device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
        //    device.TextureState[1].AlphaOperation = TextureOperation.Modulate;
        //    device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
        //    device.TextureState[1].AlphaArgument2 = TextureArgument.Constant;

        //    device.TextureState[2].ColorOperation = TextureOperation.Disable;
        //    device.TextureState[2].AlphaOperation = TextureOperation.Disable;

        //    device.TextureState[1].ConstantColor = Color.FromArgb(255 - alpha, 255 - alpha, 255 - alpha, 255 - alpha);

        //    device.DrawPrimitives(PrimitiveType.LineStrip, 0, segmentCount - 1);
        //    device.VertexShader = null;
        //    device.RenderState.ZBufferWriteEnable = true;
        //    device.RenderState.PointSpriteEnable = false;
        //    device.RenderState.PointScaleEnable = false;
        //    // device.Transform.World = savedWorld;

        }



        //VertexBuffer orbitVertexBuffer = null;
        int segmentCount = 0;
    }

}
