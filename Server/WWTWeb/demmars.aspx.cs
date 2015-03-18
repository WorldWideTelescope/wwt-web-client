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
using System.Drawing;
using System.Security.Cryptography;

public partial class DemMars : System.Web.UI.Page
{
    static MD5 md5Hash = MD5.Create();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public UInt32 ComputeHash(int level, int x, int y)
    {
        return PlateFile2.DirectoryEntry.ComputeHash(level + 128, x, y);
    }

    
}
