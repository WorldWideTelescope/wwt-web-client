<%@ Page Language="C#" ContentType="application/octet-stream" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);
        string type = values[3];
        int demSize = 513 * 2;
	string wwtDemDir = ConfigurationManager.AppSettings["WWTDEMDir"];
        string filename = String.Format(wwtDemDir+@"\toast\mars\Chunks\{0}\{1}.chunk", level, tileY);

        if (File.Exists(filename))
        {
            
                byte[] data = new byte[demSize];
                FileStream fs = File.OpenRead(filename);
                fs.Seek((long)(demSize * tileX), SeekOrigin.Begin);

                fs.Read(data, 0, demSize);
                fs.Close();
                Response.OutputStream.Write(data, 0, demSize);
                Response.OutputStream.Flush();

            
       }
       else
        {
            
                byte[] data = new byte[demSize];
            
                Response.OutputStream.Write(data, 0, demSize);
                Response.OutputStream.Flush();

            
       }
	
       Response.End();
	
	%>