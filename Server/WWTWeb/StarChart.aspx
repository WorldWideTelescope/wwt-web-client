<%@ Page Language="C#" ContentType="image/png" CodeFile="StarChart.aspx.cs" Inherits="StarChart" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="PlateFile2" %>
<%

    double lat = double.Parse(Request.Params["lat"]);
    double lng = double.Parse(Request.Params["lng"]);
    double ra = double.Parse(Request.Params["ra"]);
    double dec = double.Parse(Request.Params["dec"]);
    double time = 0;
    int width = int.Parse(Request.Params["width"]);
    int height = int.Parse(Request.Params["height"]);

    if (Request.Params["jtime"] != null)
    {
        time = double.Parse(Request.Params["jtime"]);	
    }
    else
    {
	if (Request.Params["time"] != null)
    	{
	    time = Calc.ToJulian(DateTime.Parse(Request.Params["time"]));
	}
	else
	{
	    time = Calc.ToJulian(DateTime.Now.ToUniversalTime());
	}
    }


    Bitmap chart = GetChart(lat,lng, time, ra, dec,width,height);
    chart.Save(Response.OutputStream, ImageFormat.Png);
    chart.Dispose();
    
%>