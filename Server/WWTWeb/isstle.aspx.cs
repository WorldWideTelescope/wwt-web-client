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
using System.IO;
using System.Xml;
using WebServices;
using OctSetTest;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;

public partial class isstle : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    	
    public static bool IsTLECheckSumGood(string line)
        {
            if (line.Length != 69)
            {
                return false;
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++ )
            {
                switch (line[i])
                {
                    case '1':
                        checksum += 1;
                        break;
                    case '2':
                        checksum += 2;
                        break;
                    case '3':
                        checksum += 3;
                        break;
                    case '4':
                        checksum += 4;
                        break;
                    case '5':
                        checksum += 5;
                        break;
                    case '6':
                        checksum += 6;
                        break;
                    case '7':
                        checksum += 7;
                        break;
                    case '8':
                        checksum += 8;
                        break;
                    case '9':
                        checksum += 9;
                        break;
                    case '-':
                        checksum += 1;

                        break;
                }
            }
            return ('0'+(char)(checksum % 10)) == line[68];

        }

  
}

