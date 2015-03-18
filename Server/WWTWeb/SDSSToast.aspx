<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="OctSetTest" %>
<%@ Import Namespace="PlateTile" %>
<%@ Import Namespace="System.Threading" %> 

<%
    
        if (Request.UserAgent.ToLower().Contains("wget"))
        {

            Response.Write("You are not allowed to bulk download imagery thru the tile service. Please contact wwtpage@microsoft.com for more information.");
            Response.End();
            return;
        }



        string query = Request.Params["Q"];
        string[] values = query.Split(',');
        //++
        // 2014-09-26 security fix.
        //
        int level = 0;
        int tileX = 0;
        int tileY = 0;
        try
        {
            level = Convert.ToInt32(values[0]);
            tileX = Convert.ToInt32(values[1]);
            tileY = Convert.ToInt32(values[2]);
        }
        catch
        {
            Response.Write("Invalid query string.");
            Response.End();
            return;
        }

        string filename;
        string path;

        string filename2;
        string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
        string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);




        filename = String.Format(DSSTileCache + "\\SDSSToast\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY);
        path = String.Format(DSSTileCache + "\\SDSSToast\\{0}\\{2}", level, tileX, tileY);

        if (level > 14)
        {
            Response.Write("No image");
            Response.End();
            return;
        }

        if (level < 9)
        {
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\sdss_8.plate", level, tileX, tileY);
            int length = (int)s.Length;
            if (length == 0)
            {

                Response.Clear();
                Response.ContentType = "text/plain";
                Response.Write("No image");
                Response.End();
                return;
            }
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }


        if (File.Exists(filename))
        {
            try
            {
                Response.WriteFile(filename);
                return;
            }
            catch
            {
            }
        }
        else
        {
            OctTileMap map = new OctTileMap(level, tileX, tileY);

            Int32 sqSide = 256;



            // SDSS boundaries
            // RA: 105 deg <-> 270 deg
            // DEC: -3 deg <-> + 75 deg

            if (!(map.raMin > 270 | map.decMax < -3 | map.raMax < 105 | map.decMin > 75))
            {
                Bitmap bmpOutput = new Bitmap(sqSide, sqSide);
                FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
                SdssImage sdim = new SdssImage(map.raMin, map.decMax, map.raMax, map.decMin);
                sdim.LoadImage();
                sdim.Lock();

                bmpOutputFast.LockBitmap();
                // Fill up bmp from sdim

                Vector2d vxy, vradec;
                unsafe
                {
                    PixelData* pPixel;
                    for (int y = 0; y < sqSide; y++)
                    {
                        pPixel = bmpOutputFast[0, y];
                        vxy.Y = (y / 255.0);
                        for (int x = 0; x < sqSide; x++)
                        {
                            vxy.X = (x / 255.0);
                            vradec = map.PointToRaDec(vxy);
                            *pPixel = sdim.GetPixelDataAtRaDec(vradec);

                            pPixel++;
                        }
                    }
                }

                sdim.Unlock();
                sdim.image.Dispose();

                bmpOutputFast.UnlockBitmap();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                bmpOutput.Save(filename, ImageFormat.Png);
                bmpOutput.Dispose();
                try
                {
                    Response.WriteFile(filename);
                }
                catch
                {
                }
            }
            else
            {
                Response.Write("No Image");
            }
        }

        Response.End();
    
	%>