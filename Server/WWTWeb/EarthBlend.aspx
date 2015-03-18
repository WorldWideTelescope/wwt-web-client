<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="OctSetTest" %>
<%@ Import Namespace="DpossTile" %>
<%@ Import Namespace="PlateTile" %>
<%
    
    	string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
    	
   	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);
    
    
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);


	string filename = String.Format(DSSTileCache + "\\EarthBlend\\level{0}\\{2}\\{1}_{2}.jpg", level, tileX, tileY);	
	string path = String.Format(DSSTileCache +"\\EarthBlend\\level{0}\\{2}", level, tileX, tileY);

        int octsetlevel = level;
       
    
        string filename2;


        if (level > 20)
        {
            Response.Write("No image");
            Response.Close();
            return;
        }

        if (level < 8)
        {
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\BmngMerBase.plate", level, tileX, tileY);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }	
	else if (level == 8)
	{
	    int L = level;
	    int X = tileX;
	    int Y = tileY;
	    string mime = "png";	
	    int powLev5Diff = (int) Math.Pow(2, L - 2);
            int X32 = X / powLev5Diff;
            int Y32 = Y / powLev5Diff;
            filename = string.Format(wwtTilesDir + @"\BmngMerL2X{1}Y{2}.plate", mime, X32, Y32);
           
            int L5 = L - 2;
            int X5 = X % powLev5Diff; 
            int Y5 = Y % powLev5Diff;
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(filename, L5, X5, Y5);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
	
	}
	else if (level == 9)
	{
	 if (!File.Exists(filename))

	  {
		if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }	
	    int L = level;
	    int X = tileX;
	    int Y = tileY;
	    string mime = "png";	
	    int powLev5Diff = (int) Math.Pow(2, L - 2);
            int X32 = X / powLev5Diff;
            int Y32 = Y / powLev5Diff;
            string platefilename = string.Format(wwtTilesDir + @"\BmngMerL2X{1}Y{2}.plate", mime, X32, Y32);
           
            int L5 = L - 2;
            int X5 = X % powLev5Diff; 
            int Y5 = Y % powLev5Diff;

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
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(platefilename, L5, X5, Y5);
	
	    Bitmap bmp = new Bitmap(s);
	    Graphics g = Graphics.FromImage(bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                


                string tempName = Util.DownloadVeTile(level, tileX, tileY, false);
                FileInfo fi = new FileInfo(tempName);
                if (fi.Length != 0 && fi.Length != 1033)
                {
                   Bitmap temp = new Bitmap(tempName);
			
                   g.DrawImage(temp, new Rectangle(0, 0, 256, 256), 0, 0, 256, 256, GraphicsUnit.Pixel, imgAttributes);
		   temp.Dispose();
                }

                    
                g.Dispose();
                bmp.Save(filename,ImageFormat.Jpeg);
                bmp.Dispose();
		
          }
          
	  Response.WriteFile(filename);
	  return;
	
	}
	
	
    	System.Net.WebClient client = new System.Net.WebClient();


        string url = String.Format("http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15", Util.GetServerID(tileX, tileY), Util.GetTileID(tileX, tileY, level, false));

        byte[] dat =client.DownloadData(url);
                

        client.Dispose();

        Response.OutputStream.Write(dat, 0, dat.Length);


    
	
	%>