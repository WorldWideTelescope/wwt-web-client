using System;
using System.Collections.Generic;
using System.Html;


namespace wwtlib
{
    class PositionVertexBuffer
    {
        public int Count = 0;

        public PositionVertexBuffer(int count)
        {
            Count = count;
        }

        Vector3d[] verts = null;

        public Vector3d[] Lock()
        {
            verts = new Vector3d[Count];
            return verts;
        }

        public WebGLBuffer VertexBuffer;
        public void Unlock()
        {

            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 3);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (Vector3d pt in verts)
            {
                buffer[index++] = (float)pt.X;
                buffer[index++] = (float)pt.Y;
                buffer[index++] = (float)pt.Z;

            }
   
            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }
    class PositionTextureVertexBuffer
    {
        public int Count = 0;

        public PositionTextureVertexBuffer(int count)
        {
            Count = count;
        }

        PositionTexture[] verts = null;

        public PositionTexture[] Lock()
        {
            verts = new PositionTexture[Count];
            return verts;
        }

        public WebGLBuffer VertexBuffer;
        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 5);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionTexture pt in verts)
            {
                buffer[index++] = (float)pt.Position.X;
                buffer[index++] = (float)pt.Position.Y;
                buffer[index++] = (float)pt.Position.Z;
                buffer[index++] = (float)pt.Tu;
                buffer[index++] = (float)pt.Tv;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }


    class TimeSeriesLineVertexBuffer
    {
        public int Count = 0;

        public TimeSeriesLineVertexBuffer(int count)
        {
            Count = count;
        }

        TimeSeriesLineVertex[] verts = null;

        public TimeSeriesLineVertex[] Lock()
        {
            verts = new TimeSeriesLineVertex[Count];
            return verts;
        }

        public WebGLBuffer VertexBuffer;
        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 9);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (TimeSeriesLineVertex pt in verts)
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

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }
    
    class TimeSeriesPointVertexBuffer
    {
        public int Count = 0;

        public TimeSeriesPointVertexBuffer(int count)
        {
            Count = count;
        }

        TimeSeriesPointVertex[] verts = null;

        public TimeSeriesPointVertex[] Lock()
        {
            verts = new TimeSeriesPointVertex[Count];
            return verts;
        }

        public WebGLBuffer VertexBuffer;
        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 10);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (TimeSeriesPointVertex pt in verts)
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
                buffer[index++] = (float)pt.PointSize;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }
}
