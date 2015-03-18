<%@ Page Language="C#" ContentType="application/x-wtml" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
    if (Request.Cookies["alphakey"] != null && Request.Params["wtml"] == null)
    {
	Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?wtml="+HttpUtility.UrlEncode(Request.Url.ToString()+"&wtml=true"));
	return;
    }

    string name = Request.Params["object"];
    double ra = 0;
    if (Request.Params["ra"] != null)
    {
        ra = Math.Max(0,Math.Min(24.0,Convert.ToDouble(Request.Params["ra"])));
    }
    double dec = 0;
    if (Request.Params["dec"] != null)
    {
        dec = Math.Max(-90,Math.Min(90,Convert.ToDouble(Request.Params["dec"])));
    }
    double zoom = .25;
    if (Request.Params["zoom"] != null)
    {
        zoom = Math.Max(0.001373291015625,Math.Min(360,Convert.ToDouble(Request.Params["zoom"])));
    }

    //if (Request.UserAgent.Contains("WWTClient"))
    if (true)
    {

        string data;
        string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Group=\"Goto\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\"/>\n</Folder>", name, ra, dec, zoom);
        Response.Write(xml);
    }
    else
    {
        Response.ClearHeaders();
        Response.Redirect("http://www.worldwidetelescope.org/wwt.html");
    }
%>