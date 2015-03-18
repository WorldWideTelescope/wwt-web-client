<%@ Page Language="C#" ContentType="image/jpeg" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
        string query = "";

        if (Request.Params["Q"] != null)
        {
            query = Request.Params["Q"];
        }
        else
        {
            Response.Write("No image");
            Response.End();
            return;
        }
    
        string veKey = query;
       
        int level = 0;
        int tileX = 0;
        int tileY = 0;

        level = Util.GetTileAddressFromVEKey(veKey, out tileX, out tileY);
    
    
        string filename;
        string path;

	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);
        filename = String.Format(DSSTileCache + "\\VE\\level{0}\\{2}\\{1}_{2}.jpg", level, tileX, tileY);
        path = String.Format(DSSTileCache +"\\VE\\level{0}\\{2}", level, tileX, tileY);


        if (level > 20)
        {
            Response.Write("No image");
            Response.Close();
            return;
        }
    

        if (!File.Exists(filename))
        {
            if (level == 8 || level == 13)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Util.DownloadVeTile(level, tileX, tileY, true);
                
                float[][] ptsArray ={ 
									new float[] {1, 0, 0, 0, 0},
									new float[] {0, 1, 0, 0, 0},
									new float[] {0, 0, 1, 0, 0},
									new float[] {0, 0, 0, 0.5f, 0}, 
									new float[] {0, 0, 0, 0, 1}};
                ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
                ImageAttributes imgAttributes = new ImageAttributes();
                imgAttributes.SetColorMatrix(clrMatrix,
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);
                Bitmap bmp = new Bitmap(Util.DownloadVeTile(level, tileX, tileY, true));
                Graphics g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                for (int y = 0; y <  2; y++)
                {
                    for (int x = 0; x < 2; x++)
                    {
                        string tempName = Util.DownloadVeTile(level + 1, tileX * 2 + x, tileY * 2 + y, false);
                        FileInfo fi = new FileInfo(tempName);
                        if (fi.Length != 0 && fi.Length != 1033)
                        {
                            Bitmap temp = new Bitmap(tempName);

                            g.DrawImage(temp, new Rectangle(x * 128, y * 128, 128, 128), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
                        }
                    }
                }
                g.Dispose();
                bmp.Save(filename,ImageFormat.Jpeg);
                bmp.Dispose();
            }
            else
            {
                Util.DownloadVeTile(level, tileX, tileY, false);
            }
            

        }
        try
        {
            Response.WriteFile(filename);
        }
        catch
        {
        }  
       Response.End();
	
	%>