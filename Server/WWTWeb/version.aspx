<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
            	Response.AddHeader("Cache-Control", "no-cache");
		string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"];
                Response.WriteFile(webDir + @"\wwt2\version.txt");
	
%>