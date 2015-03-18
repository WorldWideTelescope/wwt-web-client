<%@ Page Language="C#" ContentType="text/plain" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
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
			id = "1738422189";
			break;
		case "mars_terrain_color":
			id = "220581050";
			break;
		case "mars_hirise":
			id = "109459728";
            type = ".auto";
			break;
		case "mars_moc":
			id = "252927426";
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
        try
        {
            if (!Directory.Exists(filename))
            {
                Directory.CreateDirectory(path);
            }
            
            WebClient webclient = new WebClient();

            string url = string.Format("http://wwt.nasa.gov/wwt/p/{0}/{1}/{2}/{3}{4}", dataset, level, tileX, tileY, type);

            webclient.DownloadFile(url, filename);
        }
        catch
        {
            Response.StatusCode = 404;
		Response.Write("Not Found");
            return;
        }
    }
    Response.Write("OK");	
    
    //Response.WriteFile(filename);
    
%>