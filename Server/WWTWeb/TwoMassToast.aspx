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
   
    
    
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);

        int octsetlevel = level;
        string filename;
        string path;
    
        string filename2;
	string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];

        if (level > 7)
        {
		Response.Clear();
		Response.ContentType = "text/plain";
                Response.Write("No image");
                Response.End();
                return;
        }

        if (level < 9)
        {
            try
            {
                Response.ContentType = "image/png";
                Stream s = PlateTilePyramid.GetFileStream(wwtTilesDir + "\\2MassToast0to7.plate", level, tileX, tileY);
                int length = (int)s.Length;
                byte[] data = new byte[length];
                s.Read(data, 0, length);
                Response.OutputStream.Write(data, 0, length);
                Response.Flush();
                Response.End();
                return;
            }
            catch
            {
		Response.Clear();
		Response.ContentType = "text/plain";
                Response.Write("No image");
                Response.End();
                return;
            }

        }	
	else
	{
        try
        {
            int L = level;
            int X = tileX;
            int Y = tileY;
            string mime = "png";
            int powLev3Diff = (int)Math.Pow(2, L - 3);
            int X8 = X / powLev3Diff;
            int Y8 = Y / powLev3Diff;
            filename = string.Format(wwtTilesDir + @"\\Galex4Far_L3to10_x{0}_y{1}.plate", X8, Y8);

            int L3 = L - 3;
            int X3 = X % powLev3Diff;
            int Y3 = Y % powLev3Diff;
            Response.ContentType = "image/png";
            Stream s = PlateTilePyramid.GetFileStream(filename, L3, X3, Y3);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }
        catch
        {
		Response.Clear();
		Response.ContentType = "text/plain";
                Response.Write("No image");
                Response.End();
                return;
        }
	
	}


    
	
	%>