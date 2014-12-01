using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;

namespace wwtlib
{
    public class Texture
    {
        public ImageElement ImageElement = null;
        public WebGLTexture Texture2d = null;

        public Texture()
        {

        }

        bool Downloading = false;
        bool Ready = false;
        bool Errored = false;

        public string URL = "";
        public void Load(string url)
        {
            URL = url;
            if (!Downloading)
            {
                Downloading = true;
                ImageElement = (ImageElement)Document.CreateElement("img");
                CrossDomainImage xdomimg = (CrossDomainImage)(object)ImageElement;

                ImageElement.AddEventListener("load", delegate(ElementEvent e)
                {
                    Ready = true;
                    Downloading = false;
                    Errored = false;
                    MakeTexture();
                }, false);

                ImageElement.AddEventListener("error", delegate(ElementEvent e)
                {
                    Downloading = false;
                    Ready = false;
                    Errored = true;
                }, false);

                xdomimg.crossOrigin = "anonymous";
  //              texture.Src = this.URL.Replace("cdn.", "www.");

                ImageElement.Src = URL;
            }

        }

        public void MakeTexture()
        {
            if ( Tile.PrepDevice != null)
            {
                //     PrepDevice.pixelStorei(GL.UNPACK_FLIP_Y_WEBGL, 1);

                try
                {
                    Texture2d = Tile.PrepDevice.createTexture();

                    Tile.PrepDevice.bindTexture(GL.TEXTURE_2D, Texture2d);

                    ImageElement image = ImageElement;

                    // Before we bind resize to a power of two if nessesary so we can MIPMAP
                    if (!IsPowerOfTwo(ImageElement.Height) | !IsPowerOfTwo(ImageElement.Width))
                    {
                        CanvasElement temp = (CanvasElement)Document.CreateElement("canvas");
                        temp.Height = FitPowerOfTwo(image.Height);
                        temp.Width = FitPowerOfTwo(image.Width);
                        CanvasContext2D ctx = (CanvasContext2D)temp.GetContext(Rendering.Render2D);
                        ctx.DrawImage(image, 0, 0, image.Width, image.Height);
                        //Substitute the resized image
                        image = (ImageElement)(Element)temp;
                    }


                    Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_EDGE);
                    Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_EDGE);
                    Tile.PrepDevice.texImage2D(GL.TEXTURE_2D, 0, GL.RGBA, GL.RGBA, GL.UNSIGNED_BYTE, image);
                    Tile.PrepDevice.texParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR_MIPMAP_NEAREST);
                    Tile.PrepDevice.generateMipmap(GL.TEXTURE_2D);

                    Tile.PrepDevice.bindTexture(GL.TEXTURE_2D, null);
                }
                catch
                {
                    Errored = true;

                }
            }
        }

        private bool IsPowerOfTwo(int val)
        {
            return (val & (val - 1)) == 0;
        }

        private int FitPowerOfTwo(int val)
        {
            val--;

            for(int i = 1; i<32;i<<=1)
            {
                val = val | val >> i;
            }
            return val + 1;
        }

    }
}
