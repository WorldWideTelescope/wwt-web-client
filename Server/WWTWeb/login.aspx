<%@ Page Language="C#" ContentType="text/plain"  AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="LoginUser" %>
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
		string wwt2dir = ConfigurationManager.AppSettings["WWT2DIR"];
		Response.AddHeader("Cache-Control", "no-cache");
                Response.Expires = -1;
                Response.CacheControl = "no-cache";
  		Response.AddHeader("etag", "1-2-3-4-5");

		if (Request.Params["Equinox"] != null)
		{
                Response.WriteFile(wwt2dir + @"\EqClientVersion.txt");
		Response.Write("\n");
		}
		else
		{
		Response.Write("ClientVersion:");
                Response.WriteFile(wwt2dir + @"\ClientVersion.txt");
		Response.Write("\n");
                Response.WriteFile(wwt2dir + @"\dataversion.txt");
		Response.Write("\nMessage:");
                Response.WriteFile(wwt2dir + @"\message.txt");
		Response.Write("\nWarnVersion:");
                Response.WriteFile(wwt2dir + @"\warnver.txt");
		Response.Write("\nMinVersion:");
                Response.WriteFile(wwt2dir + @"\minver.txt");
		Response.Write("\nUpdateUrl:");
                Response.WriteFile(wwt2dir + @"\updateurl.txt");
		}
		Response.Flush();
		
try
{
		if (Convert.ToBoolean(ConfigurationManager.AppSettings["LoginTracking"]))
		{
		    String guid = Request.Params["user"];
		    String con = ConfigurationManager.AppSettings["LoggingConn"];
		    String ver = Request.Params["version"];
		    SqlConnection myConn = GetConnectionLogging(con);

		    PostLogin( myConn, guid, 1, ver );
        	}
}
catch
{
}


%>