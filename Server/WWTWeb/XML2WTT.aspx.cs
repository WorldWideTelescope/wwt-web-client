using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Text;
using System.Net;

public partial class WWTWeb_XML2WTT : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected string MakeTourFromXML(Stream InputStream, string baseDir)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(InputStream);

       



        string tourStart = "";

        tourStart += "<?xml version='1.0' encoding='UTF-8'?>";
        tourStart += "<Tour ID=\"{TourGuid}\" Title=\"{Title}\" Descirption=\"{Description}\" RunTime=\"20\" Author=\"{AuthorName}\" AuthorEmail=\"{AuthorEmail}\" OrganizationUrl=\"{OrganizationUrl}\" OrganizationName=\"{OrganizationName}\" Keywords=\"{Keywords}\" UserLevel=\"Beginner\" Classification=\"0\" Taxonomy=\"C.5\">";
        tourStart += "<TourStops>";
        string tourEnd = "";
        tourEnd += "</TourStops>"; 
        tourEnd += "</Tour>";

        string master = "";
        master += "<TourStop Id=\"{Guid}\" Name=\"\" Description=\"{SlideTitle}\" Thumbnail=\"\" Duration=\"{Duration}\" Master=\"True\" Transition=\"Slew\" HasLocation=\"True\" LocationAltitude=\"100\" LocationLat=\"47.64222\" LocationLng=\"-122.142\" HasTime=\"True\" StartTime=\"7/8/2009 4:09:04 PM\" EndTime=\"7/8/2009 4:08:16 PM\" ActualPlanetScale=\"True\" ShowClouds=\"False\" ShowConstellationBoundries=\"True\" ShowConstellationFigures=\"True\" ShowConstellationSelection=\"True\" ShowEcliptic=\"False\" ShowElevationModel=\"False\" ShowFieldOfView=\"False\" ShowGrid=\"False\" ShowHorizon=\"False\" ShowHorizonPanorama=\"False\" ShowMoonsAsPointSource=\"False\" ShowSolarSystem=\"True\" FovTelescope=\"0\" FovEyepiece=\"0\" FovCamera=\"0\" LocalHorizonMode=\"False\" FadeInOverlays=\"False\" SolarSystemStars=\"True\" SolarSystemMilkyWay=\"True\" SolarSystemCosmos=\"True\" SolarSystemOrbits=\"True\" SolarSystemOverlays=\"True\" SolarSystemLighting=\"True\" SolarSystemScale=\"100\" SolarSystemMultiRes=\"True\">";
        master += "<Place Name=\"Current Screen\" DataSetType=\"Sky\" RA=\"{RA}\" Dec=\"{Dec}\" Constellation=\"CVN\" Classification=\"Unidentified\" Magnitude=\"0\" Distance=\"0\" AngularSize=\"60\" ZoomLevel=\"{ZoomLevel}\" Rotation=\"0.0\" Angle=\"0\" Opacity=\"100\" Target=\"Sun\">";
        master += "<Description><![CDATA[]]></Description>";
        master += "<BackgroundImageSet>";
        master += "<ImageSet Generic=\"False\" DataSetType=\"Sky\" BandPass=\"Visible\" Name=\"SDSS: Sloan Digital Sky Survey (Optical)\" Url=\"http://www.worldwidetelescope.org/wwtweb/sdsstoast.aspx?q={1},{2},{3}\" DemUrl=\"\" BaseTileLevel=\"0\" TileLevels=\"13\" BaseDegreesPerTile=\"180\" FileType=\".png\" BottomsUp=\"False\" Projection=\"Toast\" QuadTreeMap=\"\" CenterX=\"0\" CenterY=\"0\" OffsetX=\"0\" OffsetY=\"0\" Rotation=\"0\" Sparse=\"False\" ElevationModel=\"False\" StockSet=\"False\" WidthFactor=\"1\">";
        master += "<ThumbnailUrl>http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=sloan</ThumbnailUrl>";
        master += "</ImageSet>";
        master += "</BackgroundImageSet>";
        master += "</Place>";
        master += "<Overlays />";
        master += "<MusicTrack>";
        master += "<Overlay Id=\"e3dbf1aa-0e04-4ee8-bd1c-e9d14dd1c780\" Type=\"TerraViewer.AudioOverlay\" Name=\"Music\" X=\"0\" Y=\"0\" Width=\"0\" Height=\"0\" Rotation=\"0\" Color=\"NamedColor:White\" Url=\"\" Animate=\"False\">";
        master += "<Audio Filename=\"music.wma\" Volume=\"100\" Mute=\"False\" TrackType=\"Music\" />";
        master += "</Overlay>";
        master += "</MusicTrack>";
        master += "</TourStop>";


        string titleSlide = "";
        titleSlide += "<TourStop Id=\"9d25fcf1-47a1-4036-84e1-2e4e70647a4b\" Name=\"\" Description=\"Title Slide\" Thumbnail=\"\" Duration=\"00:00:10\" Master=\"False\" Transition=\"Slew\" HasLocation=\"True\" LocationAltitude=\"100\" LocationLat=\"47.64222\" LocationLng=\"-122.142\" HasTime=\"True\" StartTime=\"7/8/2009 4:09:04 PM\" EndTime=\"7/8/2009 4:08:16 PM\" ActualPlanetScale=\"True\" ShowClouds=\"False\" ShowConstellationBoundries=\"True\" ShowConstellationFigures=\"True\" ShowConstellationSelection=\"True\" ShowEcliptic=\"False\" ShowElevationModel=\"False\" ShowFieldOfView=\"False\" ShowGrid=\"False\" ShowHorizon=\"False\" ShowHorizonPanorama=\"False\" ShowMoonsAsPointSource=\"False\" ShowSolarSystem=\"True\" FovTelescope=\"0\" FovEyepiece=\"0\" FovCamera=\"0\" LocalHorizonMode=\"False\" FadeInOverlays=\"False\" SolarSystemStars=\"True\" SolarSystemMilkyWay=\"True\" SolarSystemCosmos=\"True\" SolarSystemOrbits=\"True\" SolarSystemOverlays=\"True\" SolarSystemLighting=\"True\" SolarSystemScale=\"100\" SolarSystemMultiRes=\"True\">";
        titleSlide += "<Place Name=\"Current Screen\" DataSetType=\"Sky\" RA=\"15.2873183407284\" Dec=\"21.5907089633017\" Constellation=\"SER1\" Classification=\"Unidentified\" Magnitude=\"0\" Distance=\"0\" AngularSize=\"60\" ZoomLevel=\"188.893869642374\" Rotation=\"0.0\" Angle=\"0\" Opacity=\"100\" Target=\"Sun\">";
        titleSlide += "<Description><![CDATA[]]></Description>";
        titleSlide += "<BackgroundImageSet>";
        titleSlide += "<ImageSet Generic=\"False\" DataSetType=\"Sky\" BandPass=\"Visible\" Name=\"SDSS: Sloan Digital Sky Survey (Optical)\" Url=\"http://www.worldwidetelescope.org/wwtweb/sdsstoast.aspx?q={1},{2},{3}\" DemUrl=\"\" BaseTileLevel=\"0\" TileLevels=\"13\" BaseDegreesPerTile=\"180\" FileType=\".png\" BottomsUp=\"False\" Projection=\"Toast\" QuadTreeMap=\"\" CenterX=\"0\" CenterY=\"0\" OffsetX=\"0\" OffsetY=\"0\" Rotation=\"0\" Sparse=\"False\" ElevationModel=\"False\" StockSet=\"False\" WidthFactor=\"1\">";
        titleSlide += "<ThumbnailUrl>http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=sloan</ThumbnailUrl>";
        titleSlide += "</ImageSet>";
        titleSlide += "</BackgroundImageSet>";
        titleSlide += "</Place>";
        titleSlide += "<EndTarget Name=\"End Place\" DataSetType=\"Sky\" RA=\"15.2873183407284\" Dec=\"21.5907089633017\" Constellation=\"SER1\" Classification=\"Unidentified\" Magnitude=\"0\" Distance=\"0\" AngularSize=\"60\" ZoomLevel=\"0.367349417666093\" Rotation=\"0\" Angle=\"0\" Opacity=\"0\" Target=\"Custom\">";
        titleSlide += "<Description><![CDATA[]]></Description>";
        titleSlide += "</EndTarget>";
        titleSlide += "<Overlays>";
        titleSlide += "<Overlay Id=\"2e811eba-14cc-4c4b-89d5-47180c36f8f0\" Type=\"TerraViewer.BitmapOverlay\" Name=\"WWT-gz.png\" X=\"956.5342\" Y=\"290.3851\" Width=\"1240\" Height=\"173\" Rotation=\"0\" Color=\"ARGBColor:255:255:255:255\" Url=\"http://www.galaxyzoo.org\" Animate=\"False\">";
        titleSlide += "<Bitmap Filename=\"zoologo.png\" />";
        titleSlide += "</Overlay>";
        titleSlide += "<Overlay Id=\"6c94ab77-95a3-4e46-8cb5-4ba39b2c8937\" Type=\"TerraViewer.TextOverlay\" Name=\"A tour of your favourites\" X=\"901.0809\" Y=\"453.2796\" Width=\"792.3697\" Height=\"42.89062\" Rotation=\"0\" Color=\"ARGBColor:255:255:255:255\" Url=\"\" Animate=\"False\">";
        titleSlide += "<Text>";
        titleSlide += "<TextObject Bold=\"False\" Italic=\"False\" Underline=\"False\" FontSize=\"24\" FontName=\"Verdana\" ForgroundColor=\"NamedColor:White\" BackgroundColor=\"NamedColor:Black\" BorderStyle=\"None\">A Worldwide Telescope Tour of Your Favourites</TextObject>";
        titleSlide += "</Text>";
        titleSlide += "</Overlay>";
        titleSlide += "</Overlays>";
        titleSlide += "</TourStop>";


        string tourstop = "";
        tourstop +="<TourStop Id=\"{Guid}\" Name=\"\" Description=\"{SlideTitle}\" Thumbnail=\"\" Duration=\"00:00:10\" Master=\"False\" Transition=\"Slew\" HasLocation=\"True\" LocationAltitude=\"100\" LocationLat=\"47.64222\" LocationLng=\"-122.142\" HasTime=\"True\" StartTime=\"7/8/2009 4:09:17 PM\" EndTime=\"7/8/2009 4:09:17 PM\" ActualPlanetScale=\"True\" ShowClouds=\"False\" ShowConstellationBoundries=\"True\" ShowConstellationFigures=\"True\" ShowConstellationSelection=\"True\" ShowEcliptic=\"False\" ShowElevationModel=\"False\" ShowFieldOfView=\"False\" ShowGrid=\"False\" ShowHorizon=\"False\" ShowHorizonPanorama=\"False\" ShowMoonsAsPointSource=\"False\" ShowSolarSystem=\"True\" FovTelescope=\"0\" FovEyepiece=\"0\" FovCamera=\"0\" LocalHorizonMode=\"False\" FadeInOverlays=\"False\" SolarSystemStars=\"True\" SolarSystemMilkyWay=\"True\" SolarSystemCosmos=\"True\" SolarSystemOrbits=\"True\" SolarSystemOverlays=\"True\" SolarSystemLighting=\"True\" SolarSystemScale=\"100\" SolarSystemMultiRes=\"True\">";
        tourstop +="<Place Name=\"Current Screen\" DataSetType=\"Sky\" RA=\"{RA}\" Dec=\"{Dec}\" Constellation=\"\" Classification=\"Unidentified\" Magnitude=\"0\" Distance=\"0\" AngularSize=\"60\" ZoomLevel=\"{ZoomLevel}\" Rotation=\"0\" Angle=\"0\" Opacity=\"100\" Target=\"Sun\">";
        tourstop +="<Description><![CDATA[]]></Description>";
        tourstop +="<BackgroundImageSet>";
        tourstop +="<ImageSet Generic=\"False\" DataSetType=\"Sky\" BandPass=\"Visible\" Name=\"SDSS: Sloan Digital Sky Survey (Optical)\" Url=\"http://www.worldwidetelescope.org/wwtweb/sdsstoast.aspx?q={1},{2},{3}\" DemUrl=\"\" BaseTileLevel=\"0\" TileLevels=\"13\" BaseDegreesPerTile=\"180\" FileType=\".png\" BottomsUp=\"False\" Projection=\"Toast\" QuadTreeMap=\"\" CenterX=\"0\" CenterY=\"0\" OffsetX=\"0\" OffsetY=\"0\" Rotation=\"0\" Sparse=\"False\" ElevationModel=\"False\" StockSet=\"False\" WidthFactor=\"1\">";
        tourstop +="<ThumbnailUrl>http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=sloan</ThumbnailUrl>";
        tourstop +="</ImageSet>";
        tourstop +="</BackgroundImageSet>";
        tourstop +="</Place>";
        tourstop +="<Overlays />";
        tourstop +="</TourStop>";


        string Title = "";
        string Description = "";
        string Author = "";
        string AuthorEmail = "";
        string OrgUrl = "";
        string OrgName = "";
        StringBuilder sb = new StringBuilder();
        
        
        Guid id = Guid.NewGuid();
        string tourGuid = id.ToString();
        string dir = baseDir;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        string outputfilename = dir + id.ToString() + ".wtt";
        FileCabinet cab = new FileCabinet(outputfilename, dir);
        string page = Request.PhysicalPath.Substring(0, Request.PhysicalPath.LastIndexOf('\\'));
        page = page.Substring(0,page.LastIndexOf('\\'));
        List<string> thumbs = new List<string>();
        thumbs.Add(tourGuid + "\\9d25fcf1-47a1-4036-84e1-2e4e70647a4b.thumb.png");

        string musicUrl=null;
        string musicPath=null;
        string voiceUrl=null;
        string voicePath=null;
        try
        {
            XmlNode Tour = doc["Tour"];

            Title = Tour["Title"].InnerText;
            Description = Tour["Description"].InnerText;
            Author = Tour["Author"].InnerText;
            AuthorEmail = Tour["Email"].InnerText;
            OrgUrl = Tour["OrganizationURL"].InnerText;
            OrgName = Tour["OrganizationName"].InnerText;
            sb.Append(tourStart.Replace("{TourGuid}", tourGuid).Replace("{Title}", Title).Replace("{Description}", Description).Replace("{AuthorName}", Author).Replace("{AuthorEmail}", AuthorEmail).Replace("{OrganizationUrl}", OrgUrl).Replace("{OrganizationName}", OrgName).Replace("{Keywords}", ""));

            sb.Append(titleSlide);

            XmlNode Music = Tour["MusicTrack"];
            musicUrl = Music["Filename"].InnerText; 
            XmlNode Voice = Tour["VoiceTrack"];
            voiceUrl = Music["Filename"].InnerText; 


            string stopString = master;
            XmlNode TourStops = Tour["TourStops"];
            foreach (XmlNode child in TourStops.ChildNodes)
            {
                double RA = Convert.ToDouble(child["RA"].InnerText);
                double Dec = Convert.ToDouble(child["Dec"].InnerText);
                double Zoom = Convert.ToDouble(child["ZoomLevel"].InnerText);
                Guid stopID = Guid.NewGuid();
                string Duration = child["Duration"].InnerText;
                stopString = stopString.Replace("{Duration}", Duration).Replace("{Guid}", stopID.ToString()).Replace("{RA}", RA.ToString()).Replace("{Dec}", Dec.ToString()).Replace("{ZoomLevel}", Zoom.ToString());
                sb.Append(stopString);
                thumbs.Add(tourGuid + "\\" + stopID.ToString() + ".thumb.png");
                stopString = tourstop;
            }

            sb.Append(tourEnd);

        }
        catch
        {
        }


 

        //if (!string.IsNullOrEmpty(voiceUrl))
        //{
        //    voicePath = dir + (Math.Abs(voiceUrl.GetHashCode()).ToString());
        //    if (!File.Exists(voicePath))
        //    {
        //        client.DownloadFile(voiceUrl, voicePath);
        //    }
        //    cab.AddFile(voicePath, true, tourGuid + "\\voice.wma");
        //}



        string tourfilename = dir + id.ToString() + ".wttxml";
        File.WriteAllText(tourfilename, sb.ToString(), Encoding.UTF8);


        cab.AddFile(tourfilename, false, "");
        cab.AddFile(page + "\\images\\zoologo.png", true, tourGuid + "\\zoologo.png");
        
       WebClient client = new WebClient();

        if (!string.IsNullOrEmpty(musicUrl))
        {
            musicPath = dir + (Math.Abs(musicUrl.GetHashCode()).ToString());
            if (!File.Exists(musicPath))
            {
                client.DownloadFile(musicUrl, musicPath);
            }
            cab.AddFile(musicPath, true, tourGuid + "\\music.wma");
        

        }

        foreach(string thumbFile in thumbs)
        {
            cab.AddFile(page + "\\images\\zoo.thumb.png", true, thumbFile );
        }
        cab.Package();
        File.Delete(tourfilename);

        return outputfilename;
    }

}

public class FileEntry
{
    public string Filename;
    public string Fullpath;
    public int Size;
    public int Offset;
    public FileEntry(string filename, int size)
    {
        Filename = filename;
        Size = size;
    }
    public override string ToString()
    {
        return Filename;
    }
}

public class FileCabinet
{
    protected List<FileEntry> FileList;
    Dictionary<string, FileEntry> FileDirectory;
    public string Filename;
    public string TempDirectory;
    private int currentOffset = 0;
    private string packageID = "";

    public string PackageID
    {
        get { return packageID; }
        set { packageID = value; }
    }

    public FileCabinet(string filename, string directory)
    {
        ClearFileList();
        Filename = filename;
        TempDirectory = directory;
    }

    //public FileCabinet(string filename)
    //{
    //    ClearFileList();
    //    Filename = filename;
    //    TempDirectory = "";
    //}

    public void AddFile(string filename, bool rename, string newName)
    {

        //  try
        {
            FileInfo fi = new FileInfo(filename);
            string filePart;
            if (rename)
            {
                filePart = newName;
            } 
            else
            if (filename.ToLower().StartsWith(TempDirectory.ToLower()))
            {
                filePart = filename.Substring(TempDirectory.Length);
            }
            else
            {
                filePart = filename.Substring(filename.LastIndexOf(@"\") + (rename ? 0 : 1));
            }

            if (!FileDirectory.ContainsKey(filePart))
            {
                FileEntry fe = new FileEntry(filePart, (int)fi.Length);
                fe.Fullpath = filename;
                fe.Offset = currentOffset;
                FileList.Add(fe);
                FileDirectory.Add(filePart, fe);
                currentOffset += fe.Size;
            }
        }
        //   catch
        {
        }
    }

    public void ClearFileList()
    {
        if (FileList == null)
        {
            FileList = new List<FileEntry>();
        }
        if (FileDirectory == null)
        {
            FileDirectory = new Dictionary<string, FileEntry>();
        }
        FileList.Clear();
        FileDirectory.Clear();
        currentOffset = 0;
    }

    public void Package()
    {
        StringWriter sw = new StringWriter();
        using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
        {
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("FileCabinet");
            xmlWriter.WriteAttributeString("HeaderSize", "0x0BADFOOD");

            xmlWriter.WriteStartElement("Files");
            foreach (FileEntry entry in FileList)
            {
                xmlWriter.WriteStartElement("File");
                xmlWriter.WriteAttributeString("Name", entry.Filename);
                xmlWriter.WriteAttributeString("Size", entry.Size.ToString());
                xmlWriter.WriteAttributeString("Offset", entry.Offset.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteFullEndElement();
            xmlWriter.Close();

        }

        string data = sw.ToString();

        byte[] header = Encoding.UTF8.GetBytes(data);

        string sizeText = String.Format("0x{0:x8}", header.Length);

        data = data.Replace("0x0BADFOOD", sizeText);

        // Yeah this looks redundant, but we needed the data with the replaced size
        header = Encoding.UTF8.GetBytes(data);

        FileStream output = new FileStream(Filename, FileMode.Create);

        // Write Header
        output.Write(header, 0, header.Length);



        // Write each file
        foreach (FileEntry entry in FileList)
        {
         //   using (FileStream fs = new FileStream(TempDirectory + "\\" + entry.Filename, FileMode.Open, FileAccess.Read))
            using (FileStream fs = new FileStream(entry.Fullpath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[entry.Size];
                if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                {
                    throw new SystemException( "One of the files in the collection is missing, corrupt or inaccessable");
                }
                output.Write(buffer, 0, entry.Size);
                fs.Close();
            }
        }

        output.Close();

    }

    public void Extract()
    {
        Extract(true, null);

    }

    public void Extract(bool overwrite, string targetDirectory)
    {
        string data;
        XmlDocument doc = new XmlDocument();
        int headerSize = 0;
        using (FileStream fs = File.OpenRead(Filename))
        {

            byte[] buffer = new byte[256];
            fs.Read(buffer, 0, 255);
            data = Encoding.UTF8.GetString(buffer);

            int start = data.IndexOf("0x");
            if (start == -1)
            {
                throw new SystemException("Invalid File Format");
            }
            headerSize = Convert.ToInt32(data.Substring(start, 10), 16);

            fs.Seek(0, SeekOrigin.Begin);


            buffer = new byte[headerSize];
            fs.Read(buffer, 0, headerSize);
            data = Encoding.UTF8.GetString(buffer);
            doc.LoadXml(data);

            XmlNode cab = doc["FileCabinet"];
            XmlNode files = cab["Files"];

            int offset = headerSize;
            FileList.Clear();
            foreach (XmlNode child in files.ChildNodes)
            {
                FileEntry fe = new FileEntry(child.Attributes["Name"].Value.ToString(), Convert.ToInt32(child.Attributes["Size"].Value));
                fe.Offset = offset;
                offset += fe.Size;
                FileList.Add(fe);
            }

            foreach (FileEntry entry in FileList)
            {
                while (entry.Filename.StartsWith("\\"))
                {
                    entry.Filename = entry.Filename.Substring(1);
                }
                string fullPath = TempDirectory + @"\" + entry.Filename;
                string dir = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                string file = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);

                if (!string.IsNullOrEmpty(targetDirectory) && entry.Filename.Contains("\\"))
                {
                    fullPath = targetDirectory + "\\" + file;
                    dir = targetDirectory;
                }

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (overwrite || !File.Exists(fullPath))
                {
                    FileStream fileOut = new FileStream(fullPath, FileMode.Create);
                    buffer = new byte[entry.Size];
                    fs.Seek(entry.Offset, SeekOrigin.Begin);
                    if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                    {
                        throw new SystemException("One of the files in the collection is missing, corrupt or inaccessable");
                    }
                    fileOut.Write(buffer, 0, entry.Size);
                    fileOut.Close();
                }
            }


            int x = offset;
            fs.Close();
        }

    }

    public string MasterFile
    {
        get
        {
            if (FileList.Count > 0)
            {
                return TempDirectory + FileList[0].Filename.Substring(FileList[0].Filename.LastIndexOf("\\") + 1);
            }
            else
            {
                return null;
            }
        }
    }
    public void ClearTempFiles()
    {
        foreach (FileEntry entry in FileList)
        {
            File.Delete(TempDirectory + "\\" + entry.Filename);
        }
    }

}
