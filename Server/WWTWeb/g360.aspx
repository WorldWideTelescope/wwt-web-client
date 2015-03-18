<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="PlateFile2" %>
<%
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);
 
	string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];

	Response.AddHeader("Cache-Control", "public, max-age=31536000");
	Response.AddHeader("Expires", "Thu, 31 Dec 2009 16:00:00 GMT");
	Response.AddHeader("ETag", "155");
	Response.AddHeader("Last-Modified", "Tue, 20 May 2008 22:32:37 GMT");
	//Response.AddHeader("Cache-Control", "max-age=36000");

	
        if (level < 12)
        {
            Response.ContentType = "image/png";

	    UInt32 index = DirectoryEntry.ComputeHash(level + 128, tileX, tileY) % 16;
    

            Stream s = PlateFile2.GetFileStream(String.Format(wwtTilesDir + "\\g360-{0}.plate", index.ToString()), -1, level, tileX, tileY);


            
	    if( s == null || s.Length == 0)
	    {

		Response.Clear();
           	Response.ContentType = "text/plain";
		Response.Write("No image");
            	Response.End();
            	return;
	    }

	    int length = (int)s.Length;
            byte[] data = new byte[length];
            s.Read(data, 0, length);
	    s.Close();
            Response.OutputStream.Write(data, 0, length);
            Response.Flush();
            Response.End();
            return;
        }
	%>