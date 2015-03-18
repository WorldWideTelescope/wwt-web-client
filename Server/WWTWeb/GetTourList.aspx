<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GetTourList.aspx.cs" Inherits="GetTours" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml.Serialization" %>

<%	
    string etag = Request.Headers["If-None-Match"];
  

    Response.ClearHeaders();
    Response.Clear();
    Response.ContentType="application/x-wtml";

    string toursXML = null;
    UpdateCacheEx();
    toursXML = (string)HttpContext.Current.Cache["WWTXMLTours"];

    if (toursXML != null)
    {
        int version = (int)HttpContext.Current.Cache.Get("Version");
        string newEtag = version.ToString();

        if (newEtag != etag)
        {
            Response.AddHeader("etag", newEtag);
            Response.AddHeader("Cache-Control", "no-cache");
            Response.Write(toursXML);
        }
        else
        {
            Response.Status = "304 Not Modified";
        }
    }
    Response.End();


    
%>
