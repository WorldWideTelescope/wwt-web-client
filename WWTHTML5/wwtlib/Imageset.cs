using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Html;

namespace wwtlib
{
    public class Imageset :  IThumbnail
    {


        public static string GetTileKey(Imageset imageset, int level, int x, int y)
        {
            return imageset.ImageSetID.ToString() + @"\" + level.ToString() + @"\" + y.ToString() + "_" + x.ToString();
        }

        public static Tile GetNewTile(Imageset imageset, int level, int x, int y, Tile parent)
        {

            switch (imageset.Projection)
            {
                case ProjectionType.Mercator:
                    {
                        MercatorTile newTile = MercatorTile.Create(level, x, y, imageset, parent);
                        return newTile;
                    }
                case ProjectionType.Equirectangular:
                    {
                        return EquirectangularTile.Create(level, x, y, imageset, parent);
                    }
                //case ProjectionType.Spherical:
                //    {
                //        return new SphericalTile(level, x, y, imageset, parent);
                //    }
                default:
                case ProjectionType.Toast:
                    {
                        return ToastTile.Create(level, x, y, imageset, parent);
                    }
                case ProjectionType.SkyImage:
                    {
                        return SkyImageTile.Create(level, x, y, imageset, parent);
                    }
                case ProjectionType.Plotted:
                    {
                        return PlotTile.Create(level, x, y, imageset, parent);
                    }

                case ProjectionType.Tangent:
                    {
                        TangentTile newTile = TangentTile.Create(level, x, y, imageset, parent);
                        return newTile;
                    }
            }

        }

        ProjectionType projection;

        public ProjectionType Projection
        {
            get { return projection; }
            set { projection = value; }
        }
    
        private string referenceFrame;

        public string ReferenceFrame
        {
            get
            {
                return referenceFrame;
            }
            set
            {
                referenceFrame = value;
            }
        }

        int imageSetID;
        public int ImageSetID
        {
            get
            {
                return imageSetID;
            }
            set
            {
                imageSetID = value;
            }
        }

        double baseTileDegrees;
        public double BaseTileDegrees
        {
            get
            {
                return baseTileDegrees;
            }
            set
            {
                baseTileDegrees = value;
            }
        }

        double widthFactor = 1;

        public double WidthFactor
        {
            get { return widthFactor; }
            set { widthFactor = value; }
        }

        public int GetHashCode()
        {
            
            return Util.GetHashCode(Url);
        }

        protected string url;
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        protected string demUrl = "";
        public string DemUrl
        {
            get
            {
                if (String.IsNullOrEmpty(demUrl) && projection == ProjectionType.Mercator)
                {
                    return "http://www.worldwidetelescope.org/wwtweb/BingDemTile.aspx?Q={0},{1},{2}";      
                   // return "http://ecn.t{S}.tiles.virtualearth.net/tiles/d{Q}.elv?g=1&n=z";
                }
                return demUrl;
            }
            set
            {
                demUrl = value;
            }
        }

        string extension;
        public string Extension
        {
            get
            {
                return extension;
            }
            set
            {
                extension = value;
            }
        }

        int levels;
        public int Levels
        {
            get
            {
                return levels;
            }
            set
            {
                levels = value;
            }
        }
        bool mercator;
        bool bottomsUp;

        public bool BottomsUp
        {
            get { return bottomsUp; }
            set { bottomsUp = value; }
        }

        public bool Mercator
        {
            get { return mercator; }
            set { mercator = value; }
        }
        //private int tileSize = 256;

        //public int TileSize
        //{
        //    get { return tileSize; }
        //    set { tileSize = value; }
        //}
        int baseLevel = 1;

        public int BaseLevel
        {
            get { return baseLevel; }
            set { baseLevel = value; }
        }

        string quadTreeTileMap = "0123";

        public string QuadTreeTileMap
        {
            get { return quadTreeTileMap; }
            set { quadTreeTileMap = value; }
        }
        double centerX = 0;

        public double CenterX
        {
            get { return centerX; }
            set
            {
                if (centerX != value)
                {
                    centerX = value;
                    ComputeMatrix();
                }
            }
        }
        double centerY = 0;

        public double CenterY
        {
            get { return centerY; }
            set
            {
                if (centerY != value)
                {
                    centerY = value;
                    ComputeMatrix();
                }
            }
        }
        double rotation = 0;

        public double Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    ComputeMatrix();
                }
            }
        }

        private double meanRadius;

        public double MeanRadius
        {
            get { return meanRadius; }
            set { meanRadius = value; }
        }


        ImageSetType dataSetType = ImageSetType.Earth;
        BandPass bandPass = BandPass.Visible;

        public BandPass BandPass
        {
            get { return bandPass; }
            set { bandPass = value; }
        }

        public ImageSetType DataSetType
        {
            get { return dataSetType; }
            set { dataSetType = value; }
        }
        /*
                             node.Attributes.GetNamedItem("").Value,
                            Convert.ToDouble(node.Attributes.GetNamedItem("").Value),
                            Convert.ToInt32(node.Attributes.GetNamedItem("").Value),
 
 
         * */
        string altUrl = "";

        public string AltUrl
        {
            get { return altUrl; }
            set { altUrl = value; }
        }
        bool singleImage = false;

        public bool SingleImage
        {
            get { return singleImage; }
            set { singleImage = value; }
        }


        public static Imageset FromXMLNode(XmlNode node)
        {
            try
            {
                ImageSetType type = ImageSetType.Sky;



                ProjectionType projection = ProjectionType.Tangent;

                if (node.Attributes.GetNamedItem("DataSetType") != null)
                {
                    switch (node.Attributes.GetNamedItem("DataSetType").Value.ToLowerCase())
                    {
                        case "earth":
                            type = ImageSetType.Earth;
                            break;
                        case "planet":
                            type = ImageSetType.Planet;
                            break;
                        case "sky":
                            type = ImageSetType.Sky;
                            break;
                        case "panorama":
                            type = ImageSetType.Panorama;
                            break;
                        case "solarsystem":
                            type = ImageSetType.SolarSystem;
                            break;
                    }
                }

                BandPass bandPass = BandPass.Visible;



                switch (node.Attributes.GetNamedItem("BandPass").Value)
                {
                    case "Gamma":
                        bandPass = BandPass.Gamma;
                        break;
                    case "XRay":
                        bandPass = BandPass.XRay;
                        break;
                    case "Ultraviolet":
                        bandPass = BandPass.Ultraviolet;
                        break;
                    case "Visible":
                        bandPass = BandPass.Visible;
                        break;
                    case "HydrogenAlpha":
                        bandPass = BandPass.HydrogenAlpha;
                        break;
                    case "IR":
                        bandPass = BandPass.IR;
                        break;
                    case "Microwave":
                        bandPass = BandPass.Microwave;
                        break;
                    case "Radio":
                        bandPass = BandPass.Radio;
                        break;
                    case "VisibleNight":
                        bandPass = BandPass.VisibleNight;
                        break;
                    default:
                        break;
                }


                int wf = 1;
                if (node.Attributes.GetNamedItem("WidthFactor") != null)
                {
                    wf = int.Parse(node.Attributes.GetNamedItem("WidthFactor").Value);
                }

                if (node.Attributes.GetNamedItem("Generic") == null || !bool.Parse(node.Attributes.GetNamedItem("Generic").Value.ToString()))
                {

                    switch (node.Attributes.GetNamedItem("Projection").Value.ToString().ToLowerCase())
                    {
                        case "tan":
                        case "tangent":
                            projection = ProjectionType.Tangent;
                            break;
                        case "mercator":
                            projection = ProjectionType.Mercator;
                            break;
                        case "equirectangular":
                            projection = ProjectionType.Equirectangular;
                            break;
                        case "toast":
                            projection = ProjectionType.Toast;
                            break;
                        case "spherical":
                            projection = ProjectionType.Spherical;
                            break;
                        case "plotted":
                            projection = ProjectionType.Plotted;
                            break;
                        case "skyimage":
                            projection = ProjectionType.SkyImage;
                            break;
                    }

                    string fileType = node.Attributes.GetNamedItem("FileType").Value.ToString();
                    if (!fileType.StartsWith("."))
                    {
                        fileType = "." + fileType;
                    }


                    string thumbnailUrl = "";

                    XmlNode thumbUrl = Util.SelectSingleNode(node, "ThumbnailUrl");
                    if (thumbUrl != null)
                    {
                        if (string.IsNullOrEmpty(thumbUrl.InnerText))
                        {
                            ChromeNode cn = (ChromeNode)(object)thumbUrl;
                            thumbnailUrl = cn.TextContent;
                        }
                        else
                        {
                            thumbnailUrl = thumbUrl.InnerText;
                        }
                    }

                    bool stockSet = false;
                    bool elevationModel = false;

                    if (node.Attributes.GetNamedItem("StockSet") != null)
                    {
                        stockSet = bool.Parse(node.Attributes.GetNamedItem("StockSet").Value.ToString());
                    }

                    if (node.Attributes.GetNamedItem("ElevationModel") != null)
                    {
                        elevationModel = bool.Parse(node.Attributes.GetNamedItem("ElevationModel").Value.ToString());
                    }

                    string demUrl = "";
                    if (node.Attributes.GetNamedItem("DemUrl") != null)
                    {
                        demUrl = node.Attributes.GetNamedItem("DemUrl").Value.ToString();
                    }

                    string alturl = "";

                    if (node.Attributes.GetNamedItem("AltUrl") != null)
                    {
                        alturl = node.Attributes.GetNamedItem("AltUrl").Value.ToString();
                    }


                    double offsetX = 0;

                    if (node.Attributes.GetNamedItem("OffsetX") != null)
                    {
                        offsetX = double.Parse(node.Attributes.GetNamedItem("OffsetX").Value.ToString());
                    }

                    double offsetY = 0;

                    if (node.Attributes.GetNamedItem("OffsetY") != null)
                    {
                        offsetY = double.Parse(node.Attributes.GetNamedItem("OffsetY").Value.ToString());
                    }

                    string creditText = "";

                    XmlNode credits = Util.SelectSingleNode(node, "Credits");

                    if (credits != null)
                    {
                        creditText = credits.InnerText;
                    }

                    string creditsUrl = "";

                    credits = Util.SelectSingleNode(node, "CreditsUrl");

                    if (credits != null)
                    {
                        creditsUrl = credits.InnerText;
                    }

                    double meanRadius = 0;

                    if (node.Attributes.GetNamedItem("MeanRadius") != null)
                    {
                        meanRadius = double.Parse(node.Attributes.GetNamedItem("MeanRadius").Value.ToString());
                    }

                    string referenceFrame = null;
                    if (node.Attributes.GetNamedItem("ReferenceFrame") != null)
                    {
                        referenceFrame = node.Attributes.GetNamedItem("ReferenceFrame").Value;
                    }

                    string name = "";
                    if (node.Attributes.GetNamedItem("Name") != null)
                    {
                        name = node.Attributes.GetNamedItem("Name").Value.ToString();
                    }

                    string url = "";
                    if (node.Attributes.GetNamedItem("Url") != null)
                    {
                        url = node.Attributes.GetNamedItem("Url").Value.ToString();
                    }

                    int baseTileLevel = 0;
                    if (node.Attributes.GetNamedItem("BaseTileLevel") != null)
                    {
                        baseTileLevel = int.Parse(node.Attributes.GetNamedItem("BaseTileLevel").Value.ToString());
                    }

                    int tileLevels = 0;
                    if (node.Attributes.GetNamedItem("TileLevels") != null)
                    {
                        tileLevels = int.Parse(node.Attributes.GetNamedItem("TileLevels").Value.ToString());
                    }

                    double baseDegreesPerTile = 0;

                    if (node.Attributes.GetNamedItem("BaseDegreesPerTile") != null)
                    {
                        baseDegreesPerTile = double.Parse(node.Attributes.GetNamedItem("BaseDegreesPerTile").Value.ToString());
                    }


                    bool bottomsUp = false;


                    if (node.Attributes.GetNamedItem("BottomsUp") != null)
                    {
                        bottomsUp = bool.Parse(node.Attributes.GetNamedItem("BottomsUp").Value.ToString());
                    }

                    string quadTreeMap = "";
                    if (node.Attributes.GetNamedItem("QuadTreeMap") != null)
                    {
                        quadTreeMap = node.Attributes.GetNamedItem("QuadTreeMap").Value.ToString();
                    }

                    double centerX = 0;

                    if (node.Attributes.GetNamedItem("CenterX") != null)
                    {
                        centerX = double.Parse(node.Attributes.GetNamedItem("CenterX").Value.ToString());
                    }

                    double centerY = 0;

                    if (node.Attributes.GetNamedItem("CenterY") != null)
                    {
                        centerY = double.Parse(node.Attributes.GetNamedItem("CenterY").Value.ToString());
                    }

                    double rotation = 0;

                    if (node.Attributes.GetNamedItem("Rotation") != null)
                    {
                        rotation = double.Parse(node.Attributes.GetNamedItem("Rotation").Value.ToString());
                    }

                    bool sparse = false;

                    if (node.Attributes.GetNamedItem("Sparse") != null)
                    {
                        sparse = bool.Parse(node.Attributes.GetNamedItem("Sparse").Value.ToString());
                    }

                    return Imageset.Create(name, url,
                        type, bandPass, projection, Math.Abs(Util.GetHashCode(url)),
                        baseTileLevel, tileLevels,
                        256, baseDegreesPerTile, fileType,
                        bottomsUp, quadTreeMap,
                        centerX, centerY,
                        rotation, sparse,
                        thumbnailUrl, stockSet, elevationModel, wf, offsetX, offsetY, creditText, creditsUrl, demUrl, alturl, meanRadius, referenceFrame);
                }
                else
                {
                    return Imageset.CreateGeneric(type, bandPass);
                }

            }
            catch
            {
                return null;
            }
        }

        //public static void SaveToXml(System.Xml.XmlTextWriter xmlWriter, Imageset imageset, string alternateUrl)
        //{
        //    xmlWriter.WriteStartElement("ImageSet");

        //    xmlWriter.WriteAttributeString("Generic", imageset.Generic.ToString());
        //    xmlWriter.WriteAttributeString("DataSetType", imageset.DataSetType.ToString());
        //    xmlWriter.WriteAttributeString("BandPass", imageset.BandPass.ToString());
        //    if (!imageset.Generic)
        //    {
        //        xmlWriter.WriteAttributeString("Name", imageset.Name);

        //        if (String.IsNullOrEmpty(alternateUrl))
        //        {
        //            xmlWriter.WriteAttributeString("Url",  imageset.Url);
        //        }
        //        else
        //        {
        //            xmlWriter.WriteAttributeString("Url",  alternateUrl);
        //        }
        //        xmlWriter.WriteAttributeString("DemUrl", imageset.DemUrl);
        //        xmlWriter.WriteAttributeString("BaseTileLevel", imageset.BaseLevel.ToString());
        //        xmlWriter.WriteAttributeString("TileLevels", imageset.Levels.ToString());
        //        xmlWriter.WriteAttributeString("BaseDegreesPerTile", imageset.BaseTileDegrees.ToString());
        //        xmlWriter.WriteAttributeString("FileType", imageset.Extension);
        //        xmlWriter.WriteAttributeString("BottomsUp", imageset.BottomsUp.ToString());
        //        xmlWriter.WriteAttributeString("Projection", imageset.Projection.ToString());
        //        xmlWriter.WriteAttributeString("QuadTreeMap", imageset.QuadTreeTileMap);
        //        xmlWriter.WriteAttributeString("CenterX", imageset.CenterX.ToString());
        //        xmlWriter.WriteAttributeString("CenterY", imageset.CenterY.ToString());
        //        xmlWriter.WriteAttributeString("OffsetX", imageset.OffsetX.ToString());
        //        xmlWriter.WriteAttributeString("OffsetY", imageset.OffsetY.ToString());
        //        xmlWriter.WriteAttributeString("Rotation", imageset.Rotation.ToString());
        //        xmlWriter.WriteAttributeString("Sparse", imageset.Sparse.ToString());
        //        xmlWriter.WriteAttributeString("ElevationModel", imageset.ElevationModel.ToString());
        //        xmlWriter.WriteAttributeString("StockSet", imageset.DefaultSet.ToString());
        //        xmlWriter.WriteAttributeString("WidthFactor", imageset.WidthFactor.ToString());
        //        xmlWriter.WriteAttributeString("MeanRadius", imageset.MeanRadius.ToString());
        //        xmlWriter.WriteAttributeString("ReferenceFrame", imageset.ReferenceFrame);
        //        if (String.IsNullOrEmpty(alternateUrl))
        //        {
        //            xmlWriter.WriteElementString("ThumbnailUrl", imageset.ThumbnailUrl);
        //        }
        //        else
        //        {
        //            xmlWriter.WriteElementString("ThumbnailUrl", imageset.Url);
        //        }
        //    }
        //    xmlWriter.WriteEndElement();
        //}

        public override string ToString()
        {
            if (DefaultSet)
            {
                return name + " *";
            }
            else
            {
                return name;
            }
        }

        //todo figure out the place for this...
        public Imageset StockImageSet
        {
            get
            {
                if (generic || !defaultSet)
                {
                    return this;
                }
                else
                {
                    return Imageset.CreateGeneric(this.DataSetType, this.BandPass);
                }
            }
        }

        //public static bool operator ==(ImageSet left, ImageSet right)
        //{
        //    if (left == right )
        //    {
        //        return true;
        //    }
        //    if (left == null ^ right == null)
        //    {
        //        return false;
        //    }      
        //    return (left.Url.GetHashCode() == right.Url.GetHashCode());   
        //}

        //public static bool operator !=(ImageSet left, ImageSet right)
        //{
        //    if (left == right )
        //    {
        //        return false;
        //    }       
        //    if ( left == null ^ right == null)
        //    {
        //        return true;
        //    }

        //    return (left.Url.GetHashCode() != right.Url.GetHashCode());
        //}

        //public static bool operator ==(ImageSet o1, ImageSet o2)
        //{
        //    return (Object)o1 == null ? (Object)o2 == null : o1.Equals(o2);
        //}
        //public static bool operator !=(ImageSet o1, ImageSet o2)
        //{
        //    return (Object)o1 != null ? (Object)o2 != null : !o1.Equals(o2);
        //}



        public bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is Imageset))
            {
                return false;
            }
            Imageset b = (Imageset)obj;

            return (Util.GetHashCode(b.Url) == Util.GetHashCode(this.Url) && b.DataSetType == this.DataSetType && b.BandPass == this.BandPass && b.Generic == this.Generic);
            
        }

        private Matrix3d matrix;

        public Matrix3d Matrix
        {
            get
            {
                if (!matrixComputed)
                {
                    ComputeMatrix();
                }
                return matrix;
            }
            set { matrix = value; }
        }
        bool matrixComputed = false;
        private void ComputeMatrix()
        {
            matrixComputed = true;
            matrix = Matrix3d.Identity;
            matrix.Multiply(Matrix3d.RotationX((((Rotation)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - CenterX) ) / 180f * Math.PI)));
        }

        private string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private bool sparse = false;

        public bool Sparse
        {
            get { return sparse; }
            set { sparse = value; }
        }
        private string thumbnailUrl = "";

        public string ThumbnailUrl
        {
            get { return thumbnailUrl; }
            set { thumbnailUrl = value; }
        }
        private bool generic;

        public bool Generic
        {
            get { return generic; }
            set { generic = value; }
        }

        public Imageset()
        {
        }

        public static Imageset CreateGeneric(ImageSetType dataSetType, BandPass bandPass)
        {
            Imageset temp = new Imageset();
            temp.generic = true;
            temp.name = "Generic";
            temp.sparse = false;
            temp.dataSetType = dataSetType;
            temp.bandPass = bandPass;
            temp.quadTreeTileMap = "";
            temp.url = "";
            temp.levels = 0;
            temp.baseTileDegrees = 0;
            temp.imageSetID = 0;
            temp.extension = "";
            temp.projection = ProjectionType.Equirectangular;
            temp.bottomsUp = false;
            temp.baseLevel = 0;
            temp.mercator = (temp.projection == ProjectionType.Mercator);
            temp.centerX = 0;
            temp.centerY = 0;
            temp.rotation = 0;
            //todo add scale
            temp.thumbnailUrl = "";
      
            temp.matrix = Matrix3d.Identity;
            temp.matrix.Multiply(Matrix3d.RotationX((((temp.Rotation)) / 180f * Math.PI)));
            temp.matrix.Multiply(Matrix3d.RotationZ(((temp.CenterY) / 180f * Math.PI)));
            temp.matrix.Multiply(Matrix3d.RotationY((((360 - temp.CenterX) + 180) / 180f * Math.PI)));

            return temp;
        }

        bool defaultSet = false;
        bool elevationModel = false;

        public bool ElevationModel
        {
            get { return elevationModel; }
            set { elevationModel = value; }
        }
        public bool DefaultSet
        {
            get { return defaultSet; }
            set { defaultSet = value; }
        }

        double offsetX = 0;

        public double OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }


        double offsetY = 0;

        public double OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }


        string creditsText;

        public string CreditsText
        {
            get { return creditsText; }
            set { creditsText = value; }
        }
        string creditsUrl;

        public string CreditsUrl
        {
            get { return creditsUrl; }
            set { creditsUrl = value; }
        }

        public bool IsMandelbrot
        {
            get
            {
                return false;
            }
        }


        public static Imageset Create(string name, string url, ImageSetType dataSetType, BandPass bandPass, ProjectionType projection, int imageSetID, int baseLevel, int levels, int tileSize, double baseTileDegrees, string extension, bool bottomsUp, string quadTreeMap, double centerX, double centerY, double rotation, bool sparse, string thumbnailUrl, bool defaultSet, bool elevationModel, int wf, double offsetX, double offsetY, string credits, string creditsUrl, string demUrlIn, string alturl, double meanRadius, string referenceFrame)
        {
            Imageset temp = new Imageset();

            temp.ReferenceFrame = referenceFrame;
            temp.MeanRadius = meanRadius;
            temp.altUrl = alturl;
            temp.demUrl = demUrlIn;
            temp.creditsText = credits;
            temp.creditsUrl = creditsUrl;
            temp.offsetY = offsetY;
            temp.offsetX = offsetX;
            temp.widthFactor = wf;
            temp.elevationModel = elevationModel;
            temp.defaultSet = defaultSet;
            temp.name = name;
            temp.sparse = sparse;
            temp.dataSetType = dataSetType;
            temp.bandPass = bandPass;
            temp.quadTreeTileMap = quadTreeMap;
            temp.url = url;
            temp.levels = levels;
            temp.baseTileDegrees = baseTileDegrees;
            temp.imageSetID = imageSetID;
            temp.extension = extension;
            temp.projection = projection;
            temp.bottomsUp = bottomsUp;
            temp.baseLevel = baseLevel;
            temp.mercator = (projection == ProjectionType.Mercator);
            temp.centerX = centerX;
            temp.centerY = centerY;
            temp.rotation = rotation;
            temp.thumbnailUrl = thumbnailUrl;
            temp.ComputeMatrix();

            return temp;
        }



        // URL parameters
            //{0} ImageSetID
            //{1} level
            //{2} x tile id
            //{3} y tile id
            //{4} quadtree address (VE style)
            //{5} quadtree address (Google maps style)
            //{6} top left corner RA
            //{7} top left corner Dec
            //{8} bottom right corner RA
            //{9} bottom right corner dec
            //{10} bottom left corner RA
            //{11} bottom left corner dec
            //{12} top right corner RA
            //{13} top right corner dec






        #region IThumbnail Members

        ImageElement thumbnail;
        public System.Html.ImageElement Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
            }
        }
        Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        public bool IsImage
        {
            get { return true; }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public bool IsCloudCommunityItem
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get { return false; }
        }

        public List<IThumbnail> Children
        {
            get { return new List<IThumbnail>(); }
        }

        #endregion
    }
}
