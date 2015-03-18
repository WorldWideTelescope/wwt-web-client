<%@ Page Language="C#" ContentType="application/x-wtml" CodeFile="TileImage.aspx.cs"
    Inherits="TileImage" %>

<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<% 
    {
        //if (Request.Cookies["alphakey"] != null && Request.Params["wtml"] == null)
        // if (Request.Cookies["fullclient"] == null && Request.Params["wtml"] == null)
        // {
        //Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx?wtml="+HttpUtility.UrlEncode(Request.Url.ToString().Replace(",","-") +"&wtml=true"));
        //return;
        //}



       if (Request.Params["debug"] != null)
        {
            Response.ClearHeaders();
            Response.ContentType = "text/plain";

        }

        string url = "";
        bool bgoto = false;
        bool reverseparity = false;
        string creditsUrl = "";
        string credits = "";
        string thumb = "";
        double rotation = 1.0;

        double scale = 1.0;
        double y = 0;
        double x = 0;
        double dec = 0;
        double ra = 0;
        string name = "";
	int maxLevels =1;

        if (Request.Params["imageurl"] != null)
        {
            url = Request.Params["imageurl"];
        }

        if (String.IsNullOrEmpty(url))
        {
            url = "http://www.spitzer.caltech.edu/uploaded_files/images/0009/0848/sig12-011.jpg";
        }

        int hashID = Math.Abs(url.GetHashCode());

        //hashID = 12345;
        string path = Util.GetCurrentConfigShare("DSSTileCache", true) + "\\imagesTiler\\dowloadImages\\";
       
        string filename = path + hashID + ".png";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
      
        
        bool tileIt = false;
        if (!File.Exists(filename))
        {
            WebClient client = new WebClient();
            client.DownloadFile(url, filename);
            tileIt = true;
        }


        WcsImage wcsImage = WcsImage.FromFile(filename);

        if (wcsImage != null)
        {
            bool hasAvm = wcsImage.ValidWcs;


            Bitmap bmp = wcsImage.GetBitmap();
            wcsImage.AdjustScale(bmp.Width, bmp.Height);

            MakeThumbnail(bmp, hashID.ToString());
            
            name = wcsImage.Keywords[0];
            reverseparity = false;
            creditsUrl = wcsImage.CreditsUrl;
            credits = wcsImage.Copyright;
            thumb = "http://www.worldwidetelescope.org/wwtweb/tilethumb.aspx?name=" + hashID;
            rotation = wcsImage.Rotation;

            maxLevels = CalcMaxLevels((int)wcsImage.SizeX, (int)wcsImage.SizeY);
            scale = wcsImage.ScaleY * Math.Pow(2, maxLevels) * 256;
            y = 0;
            x = 0;
            dec = wcsImage.CenterY;
            ra = wcsImage.CenterX;

            //if (tileIt)
            {
                TileBitmap(bmp, hashID.ToString());
            }

            // todo make thumbnail
            //pl.ThumbNail = UiTools.MakeThumbnail(bmp);


            bmp.Dispose();
            GC.SuppressFinalize(bmp);
            bmp = null;

            if (Request.Params["debug"] != null)
            {
                name = Request.Params["name"];
                name = name.Replace(",", "");
            }


            if (Request.Params["ra"] != null)
            {
                ra = Math.Max(0, Math.Min(360.0, Convert.ToDouble(Request.Params["ra"])));
            }


            if (Request.Params["dec"] != null)
            {
                dec = Math.Max(-90, Math.Min(90, Convert.ToDouble(Request.Params["dec"])));
            }


            if (Request.Params["x"] != null)
            {
                x = Convert.ToDouble(Request.Params["x"]);
            }


            if (Request.Params["y"] != null)
            {
                y = Convert.ToDouble(Request.Params["y"]);
            }


            if (Request.Params["scale"] != null)
            {
                scale = Convert.ToDouble(Request.Params["scale"]) * Math.Pow(2, maxLevels) * 256;
            }


            if (Request.Params["rotation"] != null)
            {
                rotation = Convert.ToDouble(Request.Params["rotation"]) - 180;
            }


            if (Request.Params["thumb"] != null)
            {
                thumb = Request.Params["thumb"];
            }


            if (Request.Params["credits"] != null)
            {
                credits = Request.Params["credits"];
            }


            if (Request.Params["creditsUrl"] != null)
            {
                creditsUrl = Request.Params["creditsUrl"];
            }


            if (Request.Params["reverseparity"] != null)
            {
                reverseparity = Convert.ToBoolean(Request.Params["reverseparity"]);
            }


            if (Request.Params["goto"] != null)
            {
                bgoto = Convert.ToBoolean(Request.Params["goto"]);
            }

	    if (scale == 0)
		{
		scale = .1;
		}
            double zoom = scale*4;

            //scale = scale / 3600.0;
            //bgoto = true;

            string xml = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Folder Name=\"{0}\" Group=\"{14}\">\n<Place Name=\"{0}\" RA=\"{1}\" Dec=\"{2}\" ZoomLevel=\"{3}\" DataSetType=\"Sky\" Opacity=\"100\" Thumbnail=\"{10}\" Constellation=\"\">\n <ForegroundImageSet>\n <ImageSet DataSetType=\"Sky\" Name=\"{0}\" BandPass=\"Visible\" Url=\"http://www.worldwidetelescope.org/wwtweb/GetTile.aspx?q={{1}},{{2}},{{3}},{8}\" TileLevels=\"{15}\" WidthFactor=\"1\" Rotation=\"{5}\" Projection=\"Tan\" FileType=\".png\" CenterY=\"{2}\" CenterX=\"{9}\" BottomsUp=\"{13}\" OffsetX=\"{6}\" OffsetY=\"{7}\" BaseTileLevel=\"0\" BaseDegreesPerTile=\"{4}\">\n<Credits>{11}</Credits>\n<CreditsUrl>{12}</CreditsUrl>\n<ThumbnailUrl>{10}</ThumbnailUrl>\n</ImageSet>\n</ForegroundImageSet>\n</Place>\n</Folder>", name, ra / 15, dec, zoom, scale, rotation, x, y, hashID, ra, thumb, credits, creditsUrl, reverseparity, "Explorer", maxLevels);

            Response.Write(xml);
        }
    }
%>