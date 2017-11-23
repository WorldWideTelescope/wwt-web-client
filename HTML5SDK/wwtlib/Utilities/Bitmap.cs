using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;

namespace wwtlib
{
    public class Bitmap
    {
        public int Width;
        public int Height;

        public Bitmap()
        {

        }

        Uint8Array buffer;

        public static Bitmap Create(int width, int height)
        {
            height = Texture.FitPowerOfTwo(height);
            width = Texture.FitPowerOfTwo(width);

            Bitmap bmp = new Bitmap();
            bmp.Height = height;
            bmp.Width = width;

            bmp.buffer = new Uint8Array(width * height * 4);
            return bmp;
        }

        public void SetPixel(int x, int y, int r, int g, int b, int a)
        {
            int index = (x + y * Width) * 4;

            buffer[index++] = (Byte)r;
            buffer[index++] = (Byte)g;
            buffer[index++] = (Byte)b;
            buffer[index++] = (Byte)a;
        }

        public WebGLTexture GetTexture()
        {
            WebGLTexture tex = Tile.PrepDevice.createTexture();
            Tile.PrepDevice.bindTexture(GL.TEXTURE_2D, tex);
            Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_EDGE);
            Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_EDGE);
            Tile.PrepDevice.texImage2D(GL.TEXTURE_2D, 0, GL.RGBA, Height, Width, 0, GL.RGBA, GL.UNSIGNED_BYTE, (WebGLArray)(object)buffer);
            Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_NEAREST);
            Tile.PrepDevice.generateMipmap(GL.TEXTURE_2D);
            Tile.PrepDevice.bindTexture(GL.TEXTURE_2D, null);
            return tex;
        }

        //function textureFromPixelArray(gl, dataArray, type, width, height)
        //{
        //    var dataTypedArray = new Uint8Array(dataArray); // Don't need to do this if the data is already in a typed array
        //    var texture = gl.createTexture();
        //    gl.bindTexture(gl.TEXTURE_2D, texture);
        //    gl.texImage2D(gl.TEXTURE_2D, 0, type, width, height, 0, type, gl.UNSIGNED_BYTE, dataTypedArray);
        //    // Other texture setup here, like filter modes and mipmap generation
        //    return texture;
        //}

    }
}
