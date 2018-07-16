using System;
using System.Collections.Generic;
using System.Html;


namespace wwtlib
{
    public class ShortIndexBuffer
    {
        public WebGLBuffer Buffer;

        public ShortIndexBuffer(Uint16Array indexes)
        {
            Buffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, Buffer);
            Tile.PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, indexes, GL.STATIC_DRAW);
        }
    }

    public class IndexBuffer : IDisposable
    {
        public WebGLBuffer Buffer;

        public IndexBuffer(Uint32Array indexes)
        {
            Buffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, Buffer);
            Tile.PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, indexes, GL.STATIC_DRAW);
        }
        
        public void Dispose()
        {
            Tile.PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
            Tile.PrepDevice.deleteBuffer(Buffer);
            Buffer = null;
        }

    }
    
    public class VertexBufferBase : IDisposable
    {
        public WebGLBuffer VertexBuffer;

        public void Dispose()
        {
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, null);
            Tile.PrepDevice.deleteBuffer(VertexBuffer);
            VertexBuffer = null;
        }
    }   
            


    public class PositionVertexBuffer : VertexBufferBase
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

    class PositionTextureVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public PositionTextureVertexBuffer(int count)
        {
            Count = count;
        }

        public static PositionTextureVertexBuffer Create(PositionTexture[] data)
        {
            PositionTextureVertexBuffer buffer = new PositionTextureVertexBuffer(data.Length);
            buffer.verts = data;
            buffer.Unlock();
            return buffer;
        }

        PositionTexture[] verts = null;

        public PositionTexture[] Lock()
        {
            verts = new PositionTexture[Count];
            return verts;
        }

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

    public class PositionNormalTexturedVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public PositionNormalTexturedVertexBuffer(int count)
        {
            Count = count;
        }

        public static PositionNormalTexturedVertexBuffer Create(PositionNormalTextured[] data)
        {
            PositionNormalTexturedVertexBuffer buffer = new PositionNormalTexturedVertexBuffer(data.Length);
            buffer.verts = data;
            buffer.Unlock();
            return buffer;
        }

        PositionNormalTextured[] verts = null;

        public PositionNormalTextured[] Lock()
        {
            verts = new PositionNormalTextured[Count];
            return verts;
        }


        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 8);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionNormalTextured pt in verts)
            {
                buffer[index++] = (float)pt.X;
                buffer[index++] = (float)pt.Y;
                buffer[index++] = (float)pt.Z;
                buffer[index++] = (float)pt.Nx;
                buffer[index++] = (float)pt.Ny;
                buffer[index++] = (float)pt.Nz;
                buffer[index++] = (float)pt.Tu;
                buffer[index++] = (float)pt.Tv;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }
    public class PositionNormalTexturedTangentVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public PositionNormalTexturedTangentVertexBuffer(int count)
        {
            Count = count;
        }

        public static PositionNormalTexturedTangentVertexBuffer Create(PositionNormalTexturedTangent[] data)
        {
            PositionNormalTexturedTangentVertexBuffer buffer = new PositionNormalTexturedTangentVertexBuffer(data.Length);
            buffer.verts = data;
            buffer.Unlock();
            return buffer;
        }

        PositionNormalTexturedTangent[] verts = null;

        public PositionNormalTexturedTangent[] Lock()
        {
            verts = new PositionNormalTexturedTangent[Count];
            return verts;
        }

        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 11);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionNormalTexturedTangent pt in verts)
            {
                buffer[index++] = (float)pt.X;
                buffer[index++] = (float)pt.Y;
                buffer[index++] = (float)pt.Z;
                buffer[index++] = (float)pt.Nx;
                buffer[index++] = (float)pt.Ny;
                buffer[index++] = (float)pt.Nz;
                buffer[index++] = (float)pt.Tanx;
                buffer[index++] = (float)pt.Tany;
                buffer[index++] = (float)pt.Tanz;
                buffer[index++] = (float)pt.Tu;
                buffer[index++] = (float)pt.Tv;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }

    class KeplerVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public KeplerVertexBuffer(int count)
        {
            Count = count;
        }

        KeplerVertex[] verts = null;

        public KeplerVertex[] Lock()
        {
            verts = new KeplerVertex[Count];
            return verts;
        }

        public static KeplerVertexBuffer Create(List<KeplerVertex> items)
        {
            KeplerVertexBuffer tmp = new KeplerVertexBuffer(items.Count);
            tmp.verts = (KeplerVertex[])(object)items;
            return tmp;
        }

        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 19);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (KeplerVertex pt in verts)
            {
                buffer[index++] = (float)pt.ABC.X;
                buffer[index++] = (float)pt.ABC.Y;
                buffer[index++] = (float)pt.ABC.Z;
                buffer[index++] = (float)pt.abc1.X;
                buffer[index++] = (float)pt.abc1.Y;
                buffer[index++] = (float)pt.abc1.Z;
                buffer[index++] = (float)pt.PointSize;
                buffer[index++] = (float)pt.Color.R/255;
                buffer[index++] = (float)pt.Color.G/255;
                buffer[index++] = (float)pt.Color.B/255;
                buffer[index++] = (float)pt.Color.A/255;
                buffer[index++] = (float)pt.w;
                buffer[index++] = (float)pt.e;
                buffer[index++] = (float)pt.n;
                buffer[index++] = (float)pt.T;
                buffer[index++] = (float)pt.a;
                buffer[index++] = (float)pt.z;
                buffer[index++] = (float)pt.orbitPos;
                buffer[index++] = (float)pt.orbits;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }

    class TimeSeriesLineVertexBuffer : VertexBufferBase
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
    
    class TimeSeriesPointVertexBuffer : VertexBufferBase
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

        public void Dispose()
        {
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, null);
            Tile.PrepDevice.deleteBuffer(VertexBuffer);
            VertexBuffer = null;
        }
    }

    class PositionColoredVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public PositionColoredVertexBuffer(int count)
        {
            Count = count;
        }

        PositionColored[] verts = null;

        public PositionColored[] Lock()
        {
            verts = new PositionColored[Count];
            return verts;
        }

        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 7);
            float[] buffer = (float[])(object)f32array;
            int index = 0;
            foreach (PositionColored pt in verts)
            {
                buffer[index++] = (float)pt.Position.X;
                buffer[index++] = (float)pt.Position.Y;
                buffer[index++] = (float)pt.Position.Z;
                buffer[index++] = (float)pt.Color.R / 255.0f;
                buffer[index++] = (float)pt.Color.G / 255.0f;
                buffer[index++] = (float)pt.Color.B / 255.0f;
                buffer[index++] = (float)pt.Color.A / 255.0f;
            }

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }

    class PositionColoredTexturedVertexBuffer : VertexBufferBase
    {
        public int Count = 0;

        public PositionColoredTexturedVertexBuffer(int count)
        {
            Count = count;
        }

        PositionColoredTextured[] verts = null;

        public PositionColoredTextured[] Lock()
        {
            verts = new PositionColoredTextured[Count];
            return verts;
        }

        public void Unlock()
        {
            VertexBuffer = Tile.PrepDevice.createBuffer();
            Tile.PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
            Float32Array f32array = new Float32Array(Count * 9);
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

            Tile.PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);
        }
    }
}
