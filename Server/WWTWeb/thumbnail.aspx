<%@ Page Language="C#" ContentType="image/jpeg" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="WWTThumbnails" %>
<%
        string name = Request.Params["name"];
        string type = Request.Params["class"];
    	string wwtWebDir = ConfigurationManager.AppSettings["WWTWEBDIR"];

	Stream s = WWTThmbnail.GetThumbnailStream(name);
        if (s == null && type != null)
        {
            s = WWTThmbnail.GetThumbnailStream(type);
        }


        if (s == null)
        {
		if (File.Exists(wwtWebDir + "\\thumbnails\\"+name+".jpg"))
		{
	 	   Response.WriteFile(wwtWebDir + "\\thumbnails\\"+name+".jpg");
		    Response.End();
		}
            s = WWTThmbnail.GetThumbnailStream("Star");
        }
    
        int length = (int)s.Length;
        byte[] data = new byte[length];
        s.Read(data, 0, length);
        Response.OutputStream.Write(data, 0, length);
        Response.Flush();
        Response.End();
    
	%>