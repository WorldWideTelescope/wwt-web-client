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

public partial class TileImage : System.Web.UI.Page
{
    static MD5 md5Hash = MD5.Create();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public Bitmap DownloadBitmap(string dataset, int level, int x, int y)
    {
        //todo fix this
        string DSSTileCache = "";//; Util.GetCurrentConfigShare("DSSTileCache", true);
        string id = "1738422189";
        string type = ".png";
        switch (dataset)
        {
            case "mars_base_map":
                id = "1738422189";
                break;
            case "mars_terrain_color":
                id = "220581050";
                break;
            case "mars_hirise":
                id = "109459728";
                type = ".auto";
                break;
            case "mars_moc":
                id = "252927426";
                break;
            case "mars_historic_green":
                id = "1194136815";
                break;
            case "mars_historic_schiaparelli":
                id = "1113282550";
                break;
            case "mars_historic_lowell":
                id = "675790761";
                break;
            case "mars_historic_antoniadi":
                id = "1648157275";
                break;
            case "mars_historic_mec1":
                id = "2141096698";
                break;

        }


        string filename = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}\\{1}_{2}.png", level, x, y, id);
        string path = String.Format(DSSTileCache + "\\wwtcache\\mars\\{3}\\{0}\\{2}", level, x, y, id);


        if (!File.Exists(filename))
        {
            return null;
        }

        return new Bitmap(filename);
    }

    public void TileBitmap(Bitmap bmp, string ID)
    {
        string baseDirectory = Util.GetCurrentConfigShare("DSSTileCache", true) + "\\imagesTiler\\";
        int width = bmp.Width;
        int height = bmp.Height;
        double aspect = (double)width / (double)height;

	    string testFile = String.Format(@"{0}\{1}\0\0\0_0.png", baseDirectory, ID);

	    if (File.Exists(testFile))
	    {	
		    return;
	    }

        //narrower
        int levels = 1;
        int maxHeight = 256;
        int maxWidth = 512;
        int xOffset = 0;
        int yOffset = 0;
        do
        {
            if (aspect < 2)
            {
                if (maxHeight >= height)
                {
                    break;
                }
            }
            else
            {
                if (maxWidth >= width)
                {
                    break;
                }
            }
            levels++;
            maxHeight *= 2;
            maxWidth *= 2;
        } while (true);

        xOffset = (maxWidth - width) / 2;
        yOffset = (maxHeight - height) / 2;

        if (Directory.Exists(String.Format(@"{0}\{1})", baseDirectory, ID)))
        {
            Directory.Delete(String.Format(@"{0}\{1})", baseDirectory, ID), true);
        }
        Directory.CreateDirectory(String.Format(@"{0}\{1}", baseDirectory, ID));

        int l = levels;

        int gridX = 256;
        int gridY = 256;
        int currentLevel = 0;
        while (l > 0)
        {
            l--;
            currentLevel = l;
            string levelDir = String.Format(@"{0}\{1}\{2}", baseDirectory, ID, l.ToString());
            Directory.CreateDirectory(levelDir);

            int tilesX = 2 * (int)Math.Pow(2, l);
            int tilesY = (int)Math.Pow(2, l);

            for (int y = 0; y < tilesY; y++)
            {
                string dirName = levelDir + @"\" + y.ToString();
                Directory.CreateDirectory(dirName);

                for (int x = 0; x < tilesX; x++)
                {
                    if ((((x + 1) * gridX) > xOffset) && (((y + 1) * gridX) > yOffset) && (((x) * gridX) < (xOffset + width)) && (((y) * gridX) < (yOffset + height)))
                    {
                        Bitmap bmpTile = new Bitmap(256, 256);
                        Graphics gfx = Graphics.FromImage(bmpTile);
                        gfx.DrawImage(bmp, new Rectangle(0, 0, 256, 256), new Rectangle((x * gridX) - xOffset, (y * gridX) - yOffset, gridX, gridX), GraphicsUnit.Pixel);
                        //gfx.Flush(FlushIntention.Sync);
                        gfx.Dispose();
                        string fileOut = dirName + @"\" + y.ToString() + "_" + x.ToString() + ".png";
                        bmpTile.Save(fileOut, System.Drawing.Imaging.ImageFormat.Png);
                        bmpTile.Dispose();
                    }
                }
            }

            gridX *= 2;
            gridY *= 2;
        }
    }

    public static void MakeThumbnail(Bitmap imgOrig, string hashID)
    {
        string path = Util.GetCurrentConfigShare("DSSTileCache", true) + "\\imagesTiler\\thumbnails\\";

        string filename = path + hashID + ".jpg";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (!File.Exists(filename))
        {
            try
            {
                Bitmap bmpThumb = new Bitmap(96, 45);

                Graphics g = Graphics.FromImage(bmpThumb);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                double imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                double clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                int cw = bmpThumb.Width;
                int ch = bmpThumb.Height;

                if (imageAspect < clientAspect)
                {
                    ch = (int)((double)cw / imageAspect);
                }
                else
                {
                    cw = (int)((double)ch * imageAspect);
                }

                int cx = (bmpThumb.Width - cw) / 2;
                int cy = ((bmpThumb.Height - ch) / 2);// - 1;
                Rectangle destRect = new Rectangle(cx, cy, cw, ch);//+ 1);

                Rectangle srcRect = new Rectangle(0, 0, imgOrig.Width, imgOrig.Height);
                g.DrawImage(imgOrig, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();
                GC.SuppressFinalize(g);
                bmpThumb.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpThumb.Dispose();
            }
            catch
            {
                Bitmap bmp = new Bitmap(96, 45);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Blue);


                bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmp.Dispose();
            }
        }
    }

    public int CalcMaxLevels(int SizeX, int SizeY)
    {
        int levels = 0;
        int maxHeight = 256;
        int maxWidth = 512;
        double aspect = (double)SizeX / (double)SizeY;

        do
        {
            if (aspect < 2)
            {
                if (maxHeight >= SizeY)
                {
                    break;
                }
            }
            else
            {
                if (maxWidth >= SizeX)
                {
                    break;
                }
            }
            levels++;
            maxHeight *= 2;
            maxWidth *= 2;
        } while (true);

        return levels;
    }
}

public abstract class WcsImage
{
    public static WcsImage FromFile(string fileName)
    {
        string extension = Path.GetExtension(fileName);

        switch (extension.ToLower())
        {
            case ".fits":
            case ".fit":
            case ".gz":
                return null;
             //   return new FitsImage(fileName);
            default:
                return new VampWCSImageReader(fileName);

        }
    }

    protected string copyright;

    public string Copyright
    {
        get { return copyright; }
        set { copyright = value; }
    }
    protected string creditsUrl;

    public string CreditsUrl
    {
        get { return creditsUrl; }
        set { creditsUrl = value; }
    }

    private bool validWcs = false;

    public bool ValidWcs
    {
        get { return validWcs; }
        set { validWcs = value; }
    }
    protected List<string> keywords = new List<string>();

    public List<string> Keywords
    {
        get
        {
            if (keywords.Count == 0)
            {
                keywords.Add("Image File");
            }
            return keywords;
        }
        set { keywords = value; }
    }
    protected string description;

    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    protected double scaleX = 1;

    public double ScaleX
    {
        get { return scaleX; }
        set { scaleX = value; }
    }
    protected double scaleY = 1;

    public double ScaleY
    {
        get { return scaleY; }
        set { scaleY = value; }
    }
    protected double centerX;

    public double CenterX
    {
        get { return centerX; }
        set { centerX = value; }
    }
    protected double centerY;

    public double CenterY
    {
        get { return centerY; }
        set { centerY = value; }
    }
    protected double rotation;

    public double Rotation
    {
        get { return rotation; }
        set { rotation = value; }
    }
    protected double referenceX;

    public double ReferenceX
    {
        get { return referenceX; }
        set { referenceX = value; }
    }
    protected double referenceY;

    public double ReferenceY
    {
        get { return referenceY; }
        set { referenceY = value; }
    }
    protected double sizeX;

    public double SizeX
    {
        get { return sizeX; }
        set { sizeX = value; }
    }
    protected double sizeY;

    public double SizeY
    {
        get { return sizeY; }
        set { sizeY = value; }
    }
    protected double cd1_1;

    public double Cd1_1
    {
        get { return cd1_1; }
        set { cd1_1 = value; }
    }
    protected double cd1_2;

    public double Cd1_2
    {
        get { return cd1_2; }
        set { cd1_2 = value; }
    }
    protected double cd2_1;

    public double Cd2_1
    {
        get { return cd2_1; }
        set { cd2_1 = value; }
    }
    protected double cd2_2;

    public double Cd2_2
    {
        get { return cd2_2; }
        set { cd2_2 = value; }
    }
    protected bool hasRotation = false;
    protected bool hasSize = false;
    protected bool hasScale = false;
    protected bool hasLocation = false;
    protected bool hasPixel = false;

    public void AdjustScale(double width, double height)
    {
        //adjusts the actual height vs the reported height.
        if (width != sizeX)
        {
            scaleX *= (sizeX / width);
            referenceX /= (sizeX / width);
            sizeX = width;
        }

        if (height != sizeY)
        {
            scaleY *= (sizeY / height);
            referenceY /= (sizeY / height);
            sizeY = height;
        }
    }

    protected void CalculateScaleFromCD()
    {
        scaleX = Math.Sqrt(cd1_1 * cd1_1 + cd2_1 * cd2_1) * Math.Sign(cd1_1 * cd2_2 - cd1_2 * cd2_1);
        scaleY = Math.Sqrt(cd1_2 * cd1_2 + cd2_2 * cd2_2);
    }

    protected void CalculateRotationFromCD()
    {
        double sign = Math.Sign(cd1_1 * cd2_2 - cd1_2 * cd2_1);
        double rot2 = Math.Atan2((-sign * cd1_2), cd2_2);

        rotation = rot2 / Math.PI * 180;
    }
    protected string filename;

    public string Filename
    {
        get { return filename; }
        set { filename = value; }
    }

    private System.Drawing.Color color = System.Drawing.Color.White;

    public System.Drawing.Color Color
    {
        get { return color; }
        set { color = value; }
    }


    private bool colorCombine = false;

    public bool ColorCombine
    {
        get { return colorCombine; }
        set { colorCombine = value; }
    }


    abstract public System.Drawing.Bitmap GetBitmap();
}

class VampWCSImageReader : WcsImage
{
    public static string ExtractXMPFromFile(string filename)
    {
        char contents;
        string xmpStartSigniture = "<rdf:RDF";
        string xmpEndSigniture = "</rdf:RDF>";
        string data = string.Empty;
        bool reading = false;
        bool grepping = false;
        int collectionCount = 0;

        using (System.IO.StreamReader sr = new System.IO.StreamReader(filename))
        {
            while (!sr.EndOfStream)
            {
                contents = (char)sr.Read();

                if (!grepping && !reading && contents == '<')
                {
                    grepping = true;
                }

                if (grepping)
                {
                    data += contents;

                    if (data.Contains(xmpStartSigniture))
                    {
                        //found the begin element we can stop matching and start collecting
                        grepping = false;
                        reading = true;
                    }
                    else if (contents == xmpStartSigniture[collectionCount++])
                    {
                        //we are still looking, but on track to start collecting
                        continue;
                    }
                    else
                    {
                        //false start reset everything
                        data = string.Empty;
                        grepping = false;
                        reading = false;
                        collectionCount = 0;
                    }

                }
                else if (reading)
                {
                    data += contents;

                    if (data.Contains(xmpEndSigniture))
                    {
                        //we are finished found the end of the XMP data
                        break;
                    }
                }
            }

        }
        return data;
    }
    string filename;
    public VampWCSImageReader(string filename)
    {
        this.filename = filename;
        string data = VampWCSImageReader.ExtractXMPFromFile(filename);
        ValidWcs = ExtractXMPParameters(data);
    }


    

    public override Bitmap GetBitmap()
    {
        return new Bitmap(filename) ;
    }

    int Rating;

    public bool ExtractXMPParameters(string xmpXmlDoc)
    {
        XmlDocument doc = new XmlDocument();

        bool hasRotation = false;
        bool hasSize = false;
        bool hasScale = false;
        bool hasLocation = false;
        bool hasPixel = false;
        try
        {
            doc.LoadXml(xmpXmlDoc);
        }
        catch
        {
            return false;
        }

        try
        {

            XmlNamespaceManager NamespaceManager = new XmlNamespaceManager(doc.NameTable);
            NamespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            NamespaceManager.AddNamespace("exif", "http://ns.adobe.com/exif/1.0/");
            NamespaceManager.AddNamespace("x", "adobe:ns:meta/");
            NamespaceManager.AddNamespace("xap", "http://ns.adobe.com/xap/1.0/");
            NamespaceManager.AddNamespace("tiff", "http://ns.adobe.com/tiff/1.0/");
            NamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            NamespaceManager.AddNamespace("avm", "http://www.communicatingastronomy.org/avm/1.0/");
            NamespaceManager.AddNamespace("ps", "http://ns.adobe.com/photoshop/1.0/");
            // get ratings
            XmlNode xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/xap:Rating", NamespaceManager);

            // Alternatively, there is a common form of RDF shorthand that writes simple properties as
            // attributes of the rdf:Description element.
            if (xmlNode == null)
            {
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", NamespaceManager);
                xmlNode = xmlNode.Attributes["xap:Rating"];
            }

            if (xmlNode != null)
            {
                this.Rating = Convert.ToInt32(xmlNode.InnerText);
            }

            // get keywords
            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:subject/rdf:Bag", NamespaceManager);

            if (xmlNode != null)
            {

                foreach (XmlNode li in xmlNode)
                {
                    keywords.Add(li.InnerText);
                }
            }

            // get description
            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:description/rdf:Alt", NamespaceManager);

            if (xmlNode != null)
            {
                this.description = xmlNode.ChildNodes[0].InnerText;
            }

            // get Credits
            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/ps:Credit", NamespaceManager);

            if (xmlNode != null)
            {
                this.copyright = xmlNode.ChildNodes[0].InnerText;
            }

            // get credut url
            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:ReferenceURL", NamespaceManager);

            if (xmlNode != null)
            {
                this.creditsUrl = xmlNode.ChildNodes[0].InnerText;
            }


            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.Rotation", NamespaceManager);
            if (xmlNode != null)
            {
                rotation = -Convert.ToDouble(xmlNode.InnerText);
                hasRotation = true;
            }
            else
            {
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.Attributes["avm:Spatial.Rotation"];
                    if (xmlNode != null)
                    {
                        rotation = -Convert.ToDouble(xmlNode.InnerText);
                        hasRotation = true;
                    }
                }
            }

            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.Scale/rdf:Seq", NamespaceManager);
            if (xmlNode != null)
            {
                xmlNode = xmlNode.FirstChild;
                scaleX = Convert.ToDouble(xmlNode.InnerText);
                scaleX = -Math.Abs(scaleX);
                xmlNode = xmlNode.NextSibling;
                scaleY = Convert.ToDouble(xmlNode.InnerText);
                hasScale = true;
            }

            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferencePixel/rdf:Seq", NamespaceManager);
            if (xmlNode != null)
            {
                xmlNode = xmlNode.FirstChild;
                referenceX = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                referenceY = Convert.ToDouble(xmlNode.InnerText);
                hasPixel = true;
            }

            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferenceDimension/rdf:Seq", NamespaceManager);
            if (xmlNode != null)
            {
                xmlNode = xmlNode.FirstChild;
                sizeX = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                sizeY = Convert.ToDouble(xmlNode.InnerText);
                hasSize = true;
            }

            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferenceValue/rdf:Seq", NamespaceManager);
            if (xmlNode != null)
            {
                xmlNode = xmlNode.FirstChild;
                centerX = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                centerY = Convert.ToDouble(xmlNode.InnerText);
                hasLocation = true;
            }
            //- <avm:Spatial.CDMatrix>
            //- <rdf:Seq>
            //  <rdf:li>-2.39806701404E-08</rdf:li> 
            //  <rdf:li>-2.76656202414E-05</rdf:li> 
            //  <rdf:li>-2.76656202414E-05</rdf:li> 
            //  <rdf:li>2.39806701404E-08</rdf:li> 
            //  </rdf:Seq>

            xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.CDMatrix/rdf:Seq", NamespaceManager);
            if (xmlNode != null)
            {
                xmlNode = xmlNode.FirstChild;
                cd1_1 = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                cd1_2 = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                cd2_1 = Convert.ToDouble(xmlNode.InnerText);
                xmlNode = xmlNode.NextSibling;
                cd2_2 = Convert.ToDouble(xmlNode.InnerText);

                //TODO if Rotation was not found calculate it
                if (!hasRotation)
                {
                    CalculateRotationFromCD();
                }
                if (!hasScale)
                {
                    CalculateScaleFromCD();
                }
                hasScale = true;
                hasRotation = true;
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message);
        }
        finally
        {
            doc = null;
        }

        return hasRotation && hasSize && hasScale && hasLocation && hasPixel;
    }

}