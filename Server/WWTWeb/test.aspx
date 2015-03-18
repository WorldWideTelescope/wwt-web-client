<%@ Page Language="C#" ContentType="text/html" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
       
  	 string primary = ConfigurationManager.AppSettings["PrimaryFileserver"].ToLower();
	Response.Write ("primary: "+primary+"<BR />");
            string backup = ConfigurationManager.AppSettings["BackupFileserver"].ToLower();
	Response.Write ("Secondary: "+backup+"<BR />");
            string current = (string)HttpContext.Current.Cache.Get("CurrentFileServer");
	Response.Write ("Current: "+current+"<BR />");

            if (true || string.IsNullOrEmpty(current))
            {
                DateTime lastCheck = DateTime.Now.AddDays(-1);

                if (!string.IsNullOrEmpty(current) && HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime") != null)
                {
                    lastCheck = (DateTime)HttpContext.Current.Cache.Get("LastFileserverUpdateDateTime");
                }

                TimeSpan ts = DateTime.Now - lastCheck;

                if (ts.TotalMinutes > 1)
                {
                    HttpContext.Current.Cache.Remove("LastFileserverUpdateDateTime");
                    HttpContext.Current.Cache.Add("LastFileserverUpdateDateTime", System.DateTime.Now, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);


                    if (string.IsNullOrEmpty(current) || !Directory.Exists(@"\\" + current + @"\wwttours"))
                    {
                        bool primaryUp = false;

                        try
                        {
                            primaryUp = Directory.Exists(@"\\" + primary + @"\wwttours");

				Response.Write ("  primary: "+primary+@"\wwttours      "+"<BR />");
				Response.Write("Is primary up: "+primaryUp+"<BR />");
                        }
                        catch
                        {
                        }

                        if (primaryUp)
                        {
                            current = primary;
                            HttpContext.Current.Cache.Remove("CurrentFileServer");
                            HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);

                        }
                        else
                        {
                            current = backup;
                            HttpContext.Current.Cache.Remove("CurrentFileServer");
                            HttpContext.Current.Cache.Add("CurrentFileServer", current, null, DateTime.MaxValue, new TimeSpan(24, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                        }
                    }
                }
            }



            String baseName = ConfigurationManager.AppSettings["WWTToursTourFileUNC"].ToLower();


                
	Response.Write(baseName.Replace(primary, current));


	
	%>