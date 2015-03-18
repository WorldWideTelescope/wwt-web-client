<%@ Page Language="C#" ContentType="image/jpeg" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
        string name = Request.Params["name"];
        string type = Request.Params["class"];
  	string path = Util.GetCurrentConfigShare("DSSTileCache", true) + "\\imagesTiler\\thumbnails\\";

        string filename = path + name + ".jpg";
	if (File.Exists(filename ))
	{
	 	   Response.WriteFile(filename);
		   Response.End();
	}
    
%>