<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="PlateTile" %>
<%
	Response.Write("OK");	
return;
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);
	
        string file = "mars";
	string wwtTilesDir = ConfigurationManager.AppSettings["WWTTilesDir"];
  	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);

        string filename = String.Format( DSSTileCache  + "\\wwtcache\\mars\\dem\\{0}\\{2}\\{1}_{2}.dem", level, tileX, tileY);
        string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\dem\\{0}\\{2}", level, tileX, tileY);


	if (!Directory.Exists(path))
        {
        	Directory.CreateDirectory(path);
        }
	//Response.Write("len = "+Request.ContentLength.ToString());
	if (Request.ContentLength > 100)
	{
		if (!File.Exists(filename))
		{
			Request.Files[0].SaveAs(filename);
		}

		Response.Write("OK");	
	}
	else
	{
		Response.Write("No Write");
	}
        return;
        
%>