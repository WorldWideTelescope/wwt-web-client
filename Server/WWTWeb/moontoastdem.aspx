<%@ Page Language="C#" ContentType="application/octet-stream" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="PlateTile" %>
<%


    	string wwtDemDir = ConfigurationManager.AppSettings["WWTDEMDir"];   
    
    
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);

        int octsetlevel = level;
        string filename;
        string path;
    
        string filename2;


        if (level > 10)
        {
            Response.Write("No image");
            Response.Close();
            return;
        }

        if (level < 7)
        {
            Response.ContentType = "application/octet-stream";
            Stream s = PlateTilePyramid.GetFileStream(wwtDemDir + @"\toast\lola\L0X0Y0.plate", level, tileX, tileY);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }	
	else
	{
	    int L = level;
	    int X = tileX;
	    int Y = tileY;
	    string mime = "png";	
	    int powLev5Diff = (int) Math.Pow(2, L - 3);
            int X32 = X / powLev5Diff;
            int Y32 = Y / powLev5Diff;
            filename = string.Format(wwtDemDir + @"\toast\lola\L3x{0}y{1}.plate", X32, Y32);
           
            int L5 = L - 3;
            int X5 = X % powLev5Diff; 
            int Y5 = Y % powLev5Diff;
            Response.ContentType = "application/octet-stream";
            Stream s = PlateTilePyramid.GetFileStream(filename, L5, X5, Y5);
            int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
	
	}


    
	
	%>