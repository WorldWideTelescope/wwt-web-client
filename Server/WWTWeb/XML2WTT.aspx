<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XML2WTT.aspx.cs" Inherits="WWTWeb_XML2WTT" %>

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
    string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];

    Response.ClearHeaders();
    Response.Clear();
    Response.ContentType="application/x-wtt";

    Response.WriteFile(MakeTourFromXML(Request.InputStream, tourcache + "\\temp\\"));
    
    //Response.OutputStream


    
%>