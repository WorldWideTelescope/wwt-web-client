<%@ Page Language="C#" ContentType="text/plain" CodeFile="Hirisedem2.aspx.cs" Inherits="HiriseDem2" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="PlateFile2" %>
<%
    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    int level = Convert.ToInt32(values[0]);
    int tileX = Convert.ToInt32(values[1]);
    int tileY = Convert.ToInt32(values[2]);
	
    string file = "marsToastDem";
    string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
    string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);

DSSTileCache = @"\\wwt-mars\marsroot";

    string filename = String.Format( DSSTileCache  + "\\dem\\Merged4\\{0}\\{1}\\DL{0}X{1}Y{2}.dem", level, tileX, tileY);

    string path = String.Format(DSSTileCache + "\\dem\\Merged4\\{0}\\{1}\\", level, tileX, tileY);



    if (File.Exists(filename))
    {
         Stream stream = File.OpenRead(filename);
            Stream s = HiriseDem2.MergeMolaDemTileStream( level, tileX, tileY, stream);

            int length = (int)s.Length;
	    if(length == 0)
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
    
    {



            Stream ss =  HiriseDem2.GetMolaDemTileStream( level, tileX, tileY);

            int len = (int)ss.Length;
	    if(len == 0)
	    {

		Response.Clear();
           	Response.ContentType = "text/plain";
		Response.Write("No image");
            	Response.End();
            	return;
	    }
            byte[] data = new byte[len];
            ss.Read(data, 0, len);
            Response.OutputStream.Write(data, 0, len);
            Response.Flush();
            Response.End();
            return;
}

    
%>