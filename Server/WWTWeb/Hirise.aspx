<%@ Page Language="C#" ContentType="image/png" CodeFile="HiRise.aspx.cs" Inherits="HiRise" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Security.Cryptography" %>  
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="PlateFile2" %>
<%

    string query = Request.Params["Q"];
    string[] values = query.Split(',');   
    int level = Convert.ToInt32(values[0]);
    int tileX = Convert.ToInt32(values[1]);
    int tileY = Convert.ToInt32(values[2]);

    if (level < 19)
    {
        Response.ContentType = "image/png";

        UInt32 index = ComputeHash(level, tileX, tileY) % 300;

        
        Stream s = PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index), -1, level, tileX, tileY);
        
        if (s == null || (int)s.Length == 0)
        {
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.Write("No image");
            Response.End();
            return;
        }
        int length = (int)s.Length;
        byte[] data = new byte[length];
        s.Read(data, 0, length);
        Response.OutputStream.Write(data, 0, length);
        Response.Flush();
        Response.End();
        return;
    }

%>