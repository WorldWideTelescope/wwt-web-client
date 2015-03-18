using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using WebServices;
using System.Drawing;
using PlateFile2;

public partial class G360 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    public Stream LoadImage(int level, int tileX, int tileY, int id)
    {
        UInt32 index = ComputeHash(level, tileX, tileY) % 16;

        
        return PlateFile2.PlateFile2.GetFileStream(String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index), id, level, tileX, tileY);

        
    }

    public UInt32 ComputeHash(int level, int x, int y)
    {
        return PlateFile2.DirectoryEntry.ComputeHash(level + 128, x, y);
    }

    
}
