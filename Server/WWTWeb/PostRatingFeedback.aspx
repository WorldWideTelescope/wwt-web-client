<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PostRatingFeedback.aspx.cs" Inherits="PostRatingFeedback" %>
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
    Response.ClearHeaders();
    Response.Clear();
    Response.ContentType="text/xml";
    try
    {
        string query = Request.Params["Q"];
        string[] values = query.Split(',');
        string tour = values[0];
        string user = values[1];
        int rating = Convert.ToInt32(values[2]);
        if (rating > -1 && rating < 6)
        {
            PostFeedback(tour, user, rating);
        }
    }
    catch
    {
    }
    Response.End();
     
%>
