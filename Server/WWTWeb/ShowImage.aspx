<%@ Page Language="C#" ContentType="application/x-wtml" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<% 
    //if (Request.Cookies["alphakey"] != null && Request.Params["wtml"] == null)
    if (Request.Cookies["fullclient"] == null && Request.Params["wtml"] == null)
    {
	Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?wtml="+HttpUtility.UrlEncode(Request.Url.ToString().Replace(",","-") +"&wtml=true"));
	return;
    }



    bool debug= false;
    if (Request.Params["debug"] != null)
    {
         Response.ClearHeaders();
	 Response.ContentType = "text/plain";

    }	

    string name = Request.Params["name"]; 
    name = name.Replace(",","");
	
    double ra = 0;
    if (Request.Params["ra"] != null)
    {
        ra = Math.Max(0,Math.Min(360.0,Convert.ToDouble(Request.Params["ra"])));
    }

    double dec = 0;
    if (Request.Params["dec"] != null)
    {
        dec = Math.Max(-90,Math.Min(90,Convert.ToDouble(Request.Params["dec"])));
    }

    double x = 0;
    if (Request.Params["x"] != null)
    {
        x = Convert.ToDouble(Request.Params["x"]);
    }

    double y = 0;
    if (Request.Params["y"] != null)
    {
        y = Convert.ToDouble(Request.Params["y"]);
    }

    double scale = 1.0;
    if (Request.Params["scale"] != null)
    {
        scale = Convert.ToDouble(Request.Params["scale"]);
    }

	

    double rotation = 1.0;
    if (Request.Params["rotation"] != null)
    {
        rotation = Convert.ToDouble(Request.Params["rotation"])-180;
    }

    string url = "";
    if (Request.Params["imageurl"] != null)
    {
        url = Request.Params["imageurl"];
    }

    string thumb = "";
    if (Request.Params["thumb"] != null)
    {
        thumb = Request.Params["thumb"];
    }

    string credits = "";
    if (Request.Params["credits"] != null)
    {
        credits = Request.Params["credits"];
    }

    string creditsUrl = "";
    if (Request.Params["creditsUrl"] != null)
    {
        creditsUrl = Request.Params["creditsUrl"];
    }

    bool reverseparity = false;
    if (Request.Params["reverseparity"] != null)
    {
        reverseparity = Convert.ToBoolean(Request.Params["reverseparity"]);
    }

    bool bgoto = false;
    if (Request.Params["goto"] != null)
    {
        bgoto = Convert.ToBoolean(Request.Params["goto"]);
    }

    double zoom = scale*y/360;
    scale = scale / 3600.0;
    bgoto = true;
    //if (Request.UserAgent.Contains("WWTClient"))
    if (true)
    {

        string data;
        //string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Group=\"Goto\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\"/>\n</Folder>", name, ra, dec, zoom);
        string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Name=\"{0}\" Group=\"{14}\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\" Opacity=\"100\" Thumbnail=\"{10}\" Constellation=\"\">\n <ForegroundImageSet>\n <ImageSet DataSetType=\"Sky\" BandPass=\"Visible\" Url=\"{8}\" TileLevels=\"0\" WidthFactor=\"2\" Rotation=\"{5}\" Projection=\"SkyImage\" FileType=\".tif\" CenterY=\"{2}\" CenterX=\"{9}\" BottomsUp=\"{13}\" OffsetX=\"{6}\" OffsetY=\"{7}\" BaseTileLevel=\"0\" BaseDegreesPerTile=\"{4}\">\n<Credits>{11}</Credits>\n<CreditsUrl>{12}</CreditsUrl>\n</ImageSet>\n</ForegroundImageSet>\n</Place>\n</Folder>", name, ra/15, dec, zoom, scale, rotation, x, y, url, ra, thumb,credits, creditsUrl, reverseparity, bgoto ? "Goto" : "Search");
  
       Response.Write(xml);
    }
    else
    {
        Response.ClearHeaders();
        Response.Redirect("http://www.worldwidetelescope.org/wwt.html");
    }
%>