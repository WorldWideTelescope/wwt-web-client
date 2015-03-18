<%@ Page Language="C#" ContentType="image/png" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
    string returnString = "Erorr: No URL Specified";
    string url = "";
    
    string path = Server.MapPath(@"TourCache");

    try
    {
        if (Request.Params["targeturl"] != null && Request.Params["filename"] != null)
        {
            url = Request.Params["targeturl"];

            string targetfile = Request.Params["filename"];
            string filename = path + "\\" + Math.Abs(url.GetHashCode()) + ".wtt";

            if (!File.Exists(filename))
            {
                if (url.ToLower().StartsWith("http"))
                {
                    using (WebClient wc = new WebClient())
                    {
                        byte[] data = wc.DownloadData(url);

                        //Response.ContentType = wc.ResponseHeaders["Content-type"].ToString();
                        int length = data.Length;

                        File.WriteAllBytes(filename, data);
                        //Response.OutputStream.Write(data, 0, length);
                    }
                }
            }


            FileCabinet.Extract(filename, targetfile, Response);
        }
    }
    catch (System.Exception e)
    {
        returnString = e.Message;
        Response.Write(returnString);
    }
%>