<%@ Page Language="C#" ContentType="text/plain" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
    string etag = Request.Headers["If-None-Match"];
    string filename = "";
    string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"]+"\\data\\";

    if (Request.Params["Q"] != null)
    {
        string query = Request.Params["Q"];

        query = query.Replace("..", "");
        query = query.Replace("\\", "");
        query = query.Replace("/", "");
        filename = webDir  + query + ".txt";

    }
    else if (Request.Params["X"] != null)
    {
        string query = Request.Params["X"];

        query = query.Replace("..", "");
        query = query.Replace("\\", "");
        query = query.Replace("/", "");
        filename = webDir  + query + ".xml";
    }
    else if (Request.Params["W"] != null)
    {
	//Response.Clear();
	//Response.ContentType = "application/x-wtml";

        string query = Request.Params["W"];

        query = query.Replace("..", "");
        query = query.Replace("\\", "");
        query = query.Replace("/", "");
        filename = webDir  + query + ".wtml";
    }

    if (!string.IsNullOrEmpty(filename))
    {
        FileInfo fi = new FileInfo(filename);
        fi.LastWriteTimeUtc.ToString();

        string newEtag = fi.LastWriteTimeUtc.ToString();

        if (newEtag != etag)
        {
            Response.AddHeader("etag", fi.LastWriteTimeUtc.ToString());
            Response.AddHeader("Cache-Control", "no-cache");
            Response.WriteFile(filename);
        }
        else
        {
            Response.Status = "304 Not Modified";
        }
    }	
%>