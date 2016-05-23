using System;
using System.Collections.Generic;
using System.Html;

namespace wwtlib
{

    class Sprite2d
    {
        public static void Draw(RenderContext renderContext, PositionColoredTextured[] points, int count,
            Texture texture, bool triangle, double opacity)
        {
            if (VertexBuffer == null)
            {
                Create(points);
            }
            else
            {
                Update(points);
            }

            SpriteShader.Use(renderContext, VertexBuffer, texture.Texture2d);

            renderContext.gl.drawArrays(GL.TRIANGLE_STRIP,0, points.Length);

        }

        public static WebGLBuffer VertexBuffer;
        public static void Create(PositionColoredTextured[] verts)
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(verts.Length * 9);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionColoredTextured pt in verts)
            {
                buffer[index++] = (float)pt.Position.X;
                buffer[index++] = (float)pt.Position.Y;
                buffer[index++] = (float)pt.Position.Z;
                buffer[index++] = (float)pt.Color.R / 255.0f;
                buffer[index++] = (float)pt.Color.G / 255.0f;
                buffer[index++] = (float)pt.Color.B / 255.0f;
                buffer[index++] = (float)pt.Color.A / 255.0f;
                buffer[index++] = (float)pt.Tu;
                buffer[index++] = (float)pt.Tv;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.DYNAMIC_DRAW);
        }
        public static void Update(PositionColoredTextured[] verts)
        {
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(verts.Length * 9);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionColoredTextured pt in verts)
            {
                buffer[index++] = (float)pt.Position.X;
                buffer[index++] = (float)pt.Position.Y;
                buffer[index++] = (float)pt.Position.Z;
                buffer[index++] = (float)pt.Color.R / 255.0f;
                buffer[index++] = (float)pt.Color.G / 255.0f;
                buffer[index++] = (float)pt.Color.B / 255.0f;
                buffer[index++] = (float)pt.Color.A / 255.0f;
                buffer[index++] = (float)pt.Tu;
                buffer[index++] = (float)pt.Tv;
            }

            Tile.PrepDevice.bufferSubData(GL.ARRAY_BUFFER, 0, (WebGLArray)(Object)f32array );
        }
    }
}
