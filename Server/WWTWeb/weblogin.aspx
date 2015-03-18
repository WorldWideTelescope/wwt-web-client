<%@ Page Language="C#" ContentType="text/plain"  AutoEventWireup="true" CodeFile="weblogin.aspx.cs" Inherits="LoginWebUser" %>
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
		Response.AddHeader("Cache-Control", "no-cache");
                Response.Expires = -1;
                Response.CacheControl = "no-cache";

	string key = ConfigurationManager.AppSettings["webkey"];
	string testkey = Request.Params["webkey"];
	if (key == testkey)
	{
		Response.Write("Key:Authorized");
		if (Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]))
		{	
			byte platform = 2;
            if (Request.Params["Version"] != null)
            {
                if (Request.Params["Version"] == "BING")
			{
				platform = 4;
			}
                
            

                	if (Request.Params["Version"] == "HTML5")
			{
				platform = 8;
			}
                
		}
            
			if (Request.Params["platform"] != null)
			{
				if (Request.Params["platform"]== "MAC")
				{
					platform +=1;
				}
			}

			PostFeedback( Request.Params["user"], platform );
		}
	}
	else
	{
		Response.Write("Key:Failed");
	}


%>