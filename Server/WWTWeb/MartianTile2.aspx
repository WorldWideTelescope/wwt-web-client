<%@ Page Language="C#" ContentType="image/png" CodeFile="HiRise.aspx.cs" Inherits="HiRise" %>

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
    string dataset = values[3];
    string id = "nothing";
    string type = ".png";
	
  	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);

	switch(dataset)
	{
		case "mars_base_map":
			if (level < 18)
    {
       // Response.ContentType = "image/png";
       
        Stream s = PlateFile2.GetFileStream(@"\\wwt-mars\marsroot\MARSBASEMAP\marsbasemap.plate", -1, level, tileX, tileY);
        
        if (s == null || (int)s.Length == 0)
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
        Response.OutputStream.Write(data, 0, length);
        Response.Flush();
        Response.End();
        return;
    }
			break;
		case "mars_terrain_color":
			id = "220581050";
			break;
		case "mars_hirise":
			if (level < 19)
    {
        Response.ContentType = "image/png";

        UInt32 index = ComputeHash(level, tileX, tileY) % 300;

        
        Stream s = PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index), -1, level, tileX, tileY);
        
        if (s == null || (int)s.Length == 0)
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
        Response.OutputStream.Write(data, 0, length);
        Response.Flush();
        Response.End();
        return;
    }

			break;
		case "mars_moc":
			if (level < 18)
    {
        Response.ContentType = "image/png";

        UInt32 index = ComputeHash(level, tileX, tileY) % 400;
        
        Stream s = PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\moc\mocv5_{0}.plate", index), -1, level, tileX, tileY);
        
        if (s == null || (int)s.Length == 0)
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
        Response.OutputStream.Write(data, 0, length);
        Response.Flush();
        Response.End();
        return;
    }
			break;
		case "mars_historic_green":
			id = "1194136815";
			break;
		case "mars_historic_schiaparelli":
			id = "1113282550";
			break;
		case "mars_historic_lowell":
			id = "675790761";
			break;
		case "mars_historic_antoniadi":
			id = "1648157275";
			break;
		case "mars_historic_mec1":
			id = "2141096698";
			break;
		
	}


    string filename = String.Format( DSSTileCache  + "\\wwtcache\\mars\\{3}\\{0}\\{2}\\{1}_{2}.png", level, tileX, tileY, id);
    string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}", level, tileX, tileY, id);


    if (!File.Exists(filename))
    {
        //try
        //{
        //    if (!Directory.Exists(filename))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
            
        //    WebClient webclient = new WebClient();

        //    string url = string.Format("http://wwt.nasa.gov/wwt/p/{0}/{1}/{2}/{3}{4}", dataset, level, tileX, tileY, type);

        //    webclient.DownloadFile(url, filename);
        //}
        //catch
       // {
            Response.StatusCode = 404;
            return;
       // }
    }
    
    Response.WriteFile(filename);
    
%>