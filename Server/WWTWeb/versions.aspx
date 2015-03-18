<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
		string wwt2dir = ConfigurationManager.AppSettings["WWT2DIR"];
		Response.AddHeader("Cache-Control", "no-cache");

		Response.Write("ClientVersion:");
                Response.WriteFile(wwt2dir + @"\version.txt");
		Response.Write("\n");
                Response.WriteFile(wwt2dir + @"\dataversion.txt");
		Response.Write("\nMessage:");
                Response.WriteFile(wwt2dir + @"\message.txt");
	
%>