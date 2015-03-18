<%@ Page Language="C#" ContentType="image/png"  AutoEventWireup="true" CodeFile="WMSTOast.aspx.cs" Inherits="WMSToast" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>

<%@ Import Namespace="PlateTile" %>
<%
        string query = Request.Params["Q"];
	bool debug =false;
	if (Request.Params["debug"] != null)
	{
		debug = true;
	}
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);
	string wmsUrl = values[3];
	string dirPart = Math.Abs(wmsUrl.GetHashCode()).ToString();
        string filename;
        string path;
    
        string filename2;

     	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);
        filename = String.Format(DSSTileCache + "\\\WMS\\{3}\\{0}\\{2}\\{2}_{1}.png", level, tileX, tileY, dirPart);
        path = String.Format(DSSTileCache + "\\WMS\\{3}\\{0}\\{2}", level, tileX, tileY, dirPart);

        if (level > 15)
        {
            Response.Write("No image");
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

  
           
            ToastTileMap map = new ToastTileMap(level, tileX, tileY);

            Int32 sqSide = 256;

            
              Bitmap bmpOutput = new Bitmap(sqSide,sqSide);
                FastBitmap bmpOutputFast = new FastBitmap(bmpOutput);
                WMSImage sdim = new WMSImage( map.raMin, map.decMax, map.raMax, map.decMin);
		if (debug)
{
	Response.Clear();
 	Response.ContentType = "text/plain";  
	Response.Write( sdim.LoadImage(wmsUrl,true));
	Response.End();
return;
 }
                sdim.LoadImage(wmsUrl,false);
                sdim.Lock();
     
                bmpOutputFast.LockBitmap();
                // Fill up bmp from sdim
     // Response.Clear();
   //  Response.ContentType = "text/plain";         
                Vector2d vxy,vradec;
                unsafe
                {
                    PixelData* pPixel;
                    for (int y = 0; y < sqSide; y++)
                    {
                        pPixel = bmpOutputFast[0, y];
                        vxy.Y = (y/255.0);
                        for (int x = 0; x < sqSide; x++)
                        {
                            vxy.X = (x/255.0);
                            vradec = map.PointToRaDec(vxy.X, vxy.Y);
                            *pPixel = sdim.GetPixelDataAtRaDec(vradec);

	//Response.Write(sdim.GetPixelDataAtRaDecString(vradec));
	//Response.Write("\n");
 				

                            pPixel++;
                        }
                    }
                }
  //   Response.End();

              	//sdim.Unlock();
                bmpOutputFast.UnlockBitmap();
                
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                
                bmpOutput.Save(filename,ImageFormat.Png);
                bmpOutput.Dispose();
                try
                {
                    Response.WriteFile(filename);
                }
                catch
                {
                }
           
       }
  
       Response.End();
	
	%>