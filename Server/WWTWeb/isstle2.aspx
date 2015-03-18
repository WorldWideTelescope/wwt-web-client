<%@ Page Language="C#" ContentType="text/plain"  AutoEventWireup="true" CodeFile="isstle.aspx.cs" Inherits="isstle" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
    string returnString = "Erorr: Request Failed";
    string url = "";
 
    {
	//string data1 = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
	//data1 += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
	//data1 += "Cached during government shutdown";

	//Response.Write(data1);
	//return;


        url = "http://spaceflight.nasa.gov/realdata/sightings/SSapplications/Post/JavaSSOP/orbit/ISS/SVPOST.html";

        try
        {
            string reply = (string)HttpContext.Current.Cache["WWTISSTLE"];
            DateTime date = DateTime.Now;
            TimeSpan ts = new TimeSpan(100, 0, 0, 0);
            if (HttpContext.Current.Cache["WWTISSTLDATE"] != null)
            {
                date  = (DateTime)HttpContext.Current.Cache["WWTISSTLDATE"];
                ts = DateTime.Now - date;
            }
            
            if (String.IsNullOrEmpty(reply) || ts.TotalDays > .5 || Request.Params["refresh"] != null)
            {
                using (WebClient wc = new WebClient())
                {
                    string data = wc.DownloadString(url);

                    string[] lines = data.Split(new char[] {'\n','\r'});
                    string line1 = "";
                    string line2 = "";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].Trim();
                        if (lines[i].Length == 69 && IsTLECheckSumGood(lines[i]))
                        {
                            if (line1.Length == 0 && lines[i].Substring(0, 1) == "1" )
                            {
                                line1 = lines[i];
                            }
                            if (line2.Length == 0 && lines[i].Substring(0, 1) == "2")
                            {
                                line2 = lines[i];
                            }                 
                        }
                    }
		    if (line1 == "" || line2 == "")
		    {			
			reply  = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
			reply  += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
			reply  += "Cached during government shutdown";

		    }
		    else
	            {

                    	reply = line1 + "\n" + line2 + "\nLaste Updated:" + DateTime.Now;
		    }   
   
                    HttpContext.Current.Cache["WWTISSTLE"] = reply;
                    HttpContext.Current.Cache["WWTISSTLDATE"] = DateTime.Now;
                }
            }
            Response.Write(reply);
        }
        catch (System.Exception e)
        {
		string reply  = "1 25544U 98067A   13274.85334491  .00007046  00000-0  12878-3 0  7167\n";
		reply  += "2 25544  51.6486 299.7368 0003212  97.7461 254.0523 15.50562392851247\n";
		reply  += "Cached during NASA Downtime";


           // returnString = e.Message;
            Response.Write (reply);
        }
    }

%>