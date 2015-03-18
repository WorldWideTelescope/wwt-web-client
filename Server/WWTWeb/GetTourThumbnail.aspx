<%@ Page Language="C#" ContentType="image/png" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
        string guid;
        if (Request.Params["GUID"] != null)
        {
            guid = Request.Params["GUID"];
        }
        else
        {
            Response.End();
            return;
        }
	string tourcache = ConfigurationManager.AppSettings["WWTTOURCACHE"];
        string localDir = tourcache;
        string filename = Util.GetCurrentConfigShare("WWTToursTourFileUNC", true) + String.Format(@"\{0}_TourThumb.bin", guid);
 	string localfilename = localDir  +String.Format(@"\{0}_TourThumb.bin", guid);

        if (!File.Exists(localfilename))
        {
            try
            {


            	if(File.Exists(filename))
		{
	    	    if (!Directory.Exists(localDir))
		    {
			Directory.CreateDirectory(localDir);
		    }		
		    File.Copy(filename,localfilename);
		}
            	
            }
            catch
            {
            }
        }
	
        if (File.Exists(localfilename))
        {
            try
            {
            	Response.ContentType = "image/png";
                Response.WriteFile(localfilename);
                return;
            }
            catch
            {
            }
        }
	else
	{
	    Response.Status = "404 Not Found";
	}
  	
%>