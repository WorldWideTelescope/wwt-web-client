using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;

namespace wwtlib
{
    public class Place :  IThumbnail, IPlace
    {
        public Place()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private ImageElement thumbnail;

        public ImageElement Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }

        private string name;

        public string Name
        {
            get { return Names[0]; }
            //         set { name = value; }
        }
        public string[] Names
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ("").Split(";");
                }
                return name.Split(";");
            }
            set
            {
                name = UiTools.GetNamesStringFromArray(value);
            }
        }

        private CameraParameters camParams = CameraParameters.Create(0.0, 0.0, -1.0, 0, 0, 100);

        public CameraParameters CamParams
        {
            get
            {
                if (Classification == Classification.SolarSystem && camParams.Target != SolarSystemObjects.Custom)
                {
                    // todo wire this up when astrocalc is moved
                    AstroRaDec raDec = Planets.GetPlanetLocation(Name);
                    camParams.RA = raDec.RA;
                    camParams.Dec = raDec.Dec;
                    this.distnace = raDec.Distance;
                }

                return camParams;
            }
            set { camParams = value; }
        }

        public void UpdatePlanetLocation(double jNow)
        {
            camParams.ViewTarget = Planets.GetPlanet3dLocationJD(Target, jNow);
            if (Target != SolarSystemObjects.Undefined && Target != SolarSystemObjects.Custom)
            {
                //todo add back when planets are added
                camParams.ViewTarget = Planets.GetPlanetTargetPoint(Target, Lat, Lng, jNow);
            }
        }

        Vector3d location3d = Vector3d.Create(0, 0, 0);

        public Vector3d Location3d
        {
            get
            {
                if (Classification == Classification.SolarSystem || (location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                {
                    location3d = Coordinates.RADecTo3d(this.RA, this.Dec);
                }
                return location3d;
            }
        }

        public double Lat
        {
            get { return CamParams.Lat; }
            set { camParams.Lat = value; }
        }

        public double Lng
        {
            get { return CamParams.Lng; }
            set { camParams.Lng = value; }
        }

        public double Opacity
        {
            get { return CamParams.Opacity; }
            set { camParams.Opacity = value; }
        }
        public string HtmlDescription = "";

        private string constellation = "";

        public string Constellation
        {
            get { return constellation; }
            set { constellation = value; }
        }
        private Classification classification = Classification.Galaxy;

        public Classification Classification
        {
            get { return classification; }
            set { classification = value; }
        }
        private ImageSetType type = ImageSetType.Sky;

        public ImageSetType Type
        {
            get { return type; }
            set { type = value; }
        }
        private double magnitude = 0;

        public double Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }
        private double distnace = 0;

        public double Distance
        {
            get { return distnace; }
            set { distnace = value; }
        }
        /// <summary>
        /// Angular Size in Arc Seconds
        /// </summary>
        public double AngularSize = 60;


        public double ZoomLevel
        {
            get { return CamParams.Zoom; }
            set { camParams.Zoom = value; }
        }

        ImageElement thumbNail = null;

        private Imageset studyImageset = null;

        public Imageset StudyImageset
        {
            get { return studyImageset; }
            set { studyImageset = value; }
        }


        private Imageset backgroundImageSet = null;

        public Imageset BackgroundImageset
        {
            get { return backgroundImageSet; }
            set
            {
                if (value != null)
                {
                    Type = value.DataSetType;
                }
                backgroundImageSet = value;
            }
        }

        private double searchDistance = 0;

        public double SearchDistance
        {
            get { return searchDistance; }
            set { searchDistance = value; }
        }

        double elevation = 50;

        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        private string thumbnailField;

        public string ThumbnailUrl
        {
            get
            {
                if (string.IsNullOrEmpty(this.thumbnailField))
                {
                    if (studyImageset != null && !string.IsNullOrEmpty(studyImageset.ThumbnailUrl))
                    {
                        return studyImageset.ThumbnailUrl;
                    }
                    if (backgroundImageSet != null && !string.IsNullOrEmpty(backgroundImageSet.ThumbnailUrl))
                    {
                        return backgroundImageSet.ThumbnailUrl;
                    }       
                    string name = this.Name;

                    if (name.IndexOf(";") > -1)
                    {
                        name = name.Substr(0, name.IndexOf(";"));
                    }
                    if (Classification == Classification.Star)
                    {
                        return "//worldwidetelescope.org/wwtweb/thumbnail.aspx?name=star";
                    }
                    return "//worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + name.ToLowerCase();
                }
                return this.thumbnailField;
            }
            set
            {
                this.thumbnailField = value;
            }
        }


        public double RA
        {
            get
            {
                return CamParams.RA;
            }
            set
            {
                camParams.RA = value;
            }
        }


        public double Dec
        {
            get { return CamParams.Dec; }
            set { camParams.Dec = value; }
        }

        //public Place(string name, double lat, double lng)
        //{
        //    this.name = name;
        //    Lat = lat;
        //    Lng = lng;
        //    Type = DataSetType.Geo;
        //    ZoomLevel = -1;
        //}

        public static Place Create(string name, double lat, double lng, Classification classification, string constellation, ImageSetType type, double zoomFactor)
        {
            Place temp = new Place();
            temp.ZoomLevel = zoomFactor;
            temp.constellation = constellation;
            temp.name = name;
            if (type == ImageSetType.Sky || type == ImageSetType.SolarSystem)
            {
                temp.camParams.RA = lng;
            }
            else
            {
                temp.Lng = lng;
            }
            temp.Lat = lat;
            temp.Classification = classification;
            temp.Type = type;

            return temp;
        }

        public static Place CreateCameraParams(string name, CameraParameters camParams, Classification classification, string constellation, ImageSetType type, SolarSystemObjects target)
        {
            Place temp = new Place();

            temp.constellation = constellation;
            temp.name = name;
            temp.Classification = classification;
            temp.camParams = camParams;
            temp.Type = type;
            temp.Target = target;

            return temp;
        }

        //public TourPlace(string input, bool sky)
        //{

        //    string[] sa = input.Split('\t');

        //    if (sky)
        //    {
        //        name = sa[0];


        //        Lat = Convert.ToDouble(sa[3]);
        //        if (sky)
        //        {
        //            camParams.RA = Convert.ToDouble(sa[2]) / 15;
        //            Type = ImageSetType.Sky;
        //        }
        //        else
        //        {
        //            Lng = 180 - ((Convert.ToDouble(sa[2]) / 24.0 * 360) - 180);
        //            Type = ImageSetType.Earth;
        //        }
        //        string type = sa[1];
        //        type = type.Replace(" ", "");
        //        type = type.Replace("StarCluster", "Cluster");
        //        type = type.Replace("TripleStar", "MultipleStars");
        //        type = type.Replace("HubbleImage", "Unidentified");
        //        Classification = (Classification)Enum.Parse(typeof(Classification), type);
        //        if (sa.Length > 4)
        //        {
        //            try
        //            {
        //                if (sa[4].ToUpper() != "NULL" && sa[4] != "")
        //                {
        //                    magnitude = Convert.ToDouble(sa[4]);
        //                }
        //            }
        //            catch
        //            {
        //            }
        //        }
        //        if (sa.Length > 5)
        //        {
        //            constellation = sa[5].ToUpper();
        //        }
        //        if (sa.Length > 6)
        //        {
        //            try
        //            {
        //                ZoomLevel = Convert.ToDouble(sa[6]);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //        if (sa.Length > 7)
        //        {
        //            try
        //            {
        //                distnace = Convert.ToDouble(sa[7]);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //    else
        //    {
        //        name = sa[0];
        //        Lat = (float)Convert.ToDouble(sa[1]);
        //        Lng = (float)Convert.ToDouble(sa[2]);
        //        Type = ImageSetType.Earth;
        //        if (sa.Length > 3)
        //        {
        //            elevation = Convert.ToDouble(sa[3]);
        //        }
        //    }
        //}
        public override string ToString()
        {
            return name;
        }

        internal void SaveToXml(XmlTextWriter xmlWriter, string elementName)
        {

            xmlWriter.WriteStartElement(elementName);
            xmlWriter.WriteAttributeString("Name", name);
            xmlWriter.WriteAttributeString("DataSetType", Enums.ToXml("ImageSetType", (int) type));
            if (this.Type == ImageSetType.Sky)
            {
                xmlWriter.WriteAttributeString("RA", camParams.RA.ToString());
                xmlWriter.WriteAttributeString("Dec", camParams.Dec.ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("Lat", Lat.ToString());
                xmlWriter.WriteAttributeString("Lng", Lng.ToString());
            }

            xmlWriter.WriteAttributeString("Constellation", constellation);
            xmlWriter.WriteAttributeString("Classification", Enums.ToXml("Classification", (int)classification));
            xmlWriter.WriteAttributeString("Magnitude", magnitude.ToString());
            xmlWriter.WriteAttributeString("Distance", distnace.ToString());
            xmlWriter.WriteAttributeString("AngularSize", AngularSize.ToString());
            xmlWriter.WriteAttributeString("ZoomLevel", ZoomLevel.ToString());
            xmlWriter.WriteAttributeString("Rotation", camParams.Rotation.ToString());
            xmlWriter.WriteAttributeString("Angle", camParams.Angle.ToString());
            xmlWriter.WriteAttributeString("Opacity", camParams.Opacity.ToString());
            xmlWriter.WriteAttributeString("Target", Enums.ToXml("SolarSystemObjects", (int)Target));
            xmlWriter.WriteAttributeString("ViewTarget", camParams.ViewTarget.ToString());
            xmlWriter.WriteAttributeString("TargetReferenceFrame", camParams.TargetReferenceFrame);
            //todo what do we do with full dome?
            // xmlWriter.WriteAttributeString("DomeAlt", camParams.DomeAlt.ToString());
           // xmlWriter.WriteAttributeString("DomeAz", camParams.DomeAz.ToString());
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteCData(HtmlDescription);
            xmlWriter.WriteEndElement();


            if (backgroundImageSet != null)
            {
                xmlWriter.WriteStartElement("BackgroundImageSet");
                Imageset.SaveToXml(xmlWriter, backgroundImageSet, "");
                
                xmlWriter.WriteEndElement();
            }

            if (studyImageset != null)
            {
                Imageset.SaveToXml(xmlWriter, studyImageset, "");
            }
            xmlWriter.WriteEndElement();
        }

        internal static Place FromXml(XmlNode place)
        {
            Place newPlace = new Place();

            newPlace.name = place.Attributes.GetNamedItem("Name").Value;

            if (place.Attributes.GetNamedItem("MSRComponentId") != null && place.Attributes.GetNamedItem("Permission") != null && place.Attributes.GetNamedItem("Url") != null)
            {
                //communities item
                newPlace.Url = place.Attributes.GetNamedItem("Url").Value;
                newPlace.ThumbnailUrl = place.Attributes.GetNamedItem("Thumbnail").Value;
                return newPlace;
            }

            if (place.Attributes.GetNamedItem("DataSetType") != null)
            {
                newPlace.type = (ImageSetType)Enums.Parse("ImageSetType", place.Attributes.GetNamedItem("DataSetType").Value);
            }

            if (newPlace.Type == ImageSetType.Sky)
            {
                newPlace.camParams.RA = double.Parse(place.Attributes.GetNamedItem("RA").Value);
                newPlace.camParams.Dec = double.Parse(place.Attributes.GetNamedItem("Dec").Value);
            }
            else
            {
                newPlace.Lat = double.Parse(place.Attributes.GetNamedItem("Lat").Value);
                newPlace.Lng = double.Parse(place.Attributes.GetNamedItem("Lng").Value);
            }

            if (place.Attributes.GetNamedItem("Constellation") != null)
            {
                newPlace.constellation = place.Attributes.GetNamedItem("Constellation").Value;
            }
        
            if (place.Attributes.GetNamedItem("Classification") != null)
            {
                newPlace.classification = (Classification)Enums.Parse("Classification", place.Attributes.GetNamedItem("Classification").Value);
          
            }

            if (place.Attributes.GetNamedItem("Magnitude") != null)
            {
                newPlace.magnitude = double.Parse(place.Attributes.GetNamedItem("Magnitude").Value);
            }

            if (place.Attributes.GetNamedItem("AngularSize") != null)
            {
                newPlace.AngularSize = double.Parse(place.Attributes.GetNamedItem("AngularSize").Value);
            }

            if (place.Attributes.GetNamedItem("ZoomLevel") != null)
            {

                newPlace.ZoomLevel = double.Parse(place.Attributes.GetNamedItem("ZoomLevel").Value);
            }

            if (place.Attributes.GetNamedItem("Rotation") != null)
            {
                newPlace.camParams.Rotation = double.Parse(place.Attributes.GetNamedItem("Rotation").Value);
            }

            if (place.Attributes.GetNamedItem("Angle") != null)
            {
                newPlace.camParams.Angle = double.Parse(place.Attributes.GetNamedItem("Angle").Value);
            }

            if (place.Attributes.GetNamedItem("Opacity") != null)
            {
                newPlace.camParams.Opacity = Single.Parse(place.Attributes.GetNamedItem("Opacity").Value);
            }
            else
            {
                newPlace.camParams.Opacity = 100;
            }

            newPlace.Target = SolarSystemObjects.Undefined;

            if (place.Attributes.GetNamedItem("Target") != null)
            {
                newPlace.Target = (SolarSystemObjects)Enums.Parse("SolarSystemObjects", place.Attributes.GetNamedItem("Target").Value);
            }

            if (place.Attributes.GetNamedItem("ViewTarget") != null)
            {
                newPlace.camParams.ViewTarget = Vector3d.Parse(place.Attributes.GetNamedItem("ViewTarget").Value);
            }

            if (place.Attributes.GetNamedItem("TargetReferenceFrame") != null)
            {
                newPlace.camParams.TargetReferenceFrame = place.Attributes.GetNamedItem("TargetReferenceFrame").Value;
            }

            XmlNode descriptionNode = Util.SelectSingleNode(place, "Description");
            if (descriptionNode != null)
            {
                newPlace.HtmlDescription = descriptionNode.Value;
            }

            XmlNode backgroundImageSet = Util.SelectSingleNode(place, "BackgroundImageSet");
            if (backgroundImageSet != null)
            {
                XmlNode imageSet = Util.SelectSingleNode(backgroundImageSet, "ImageSet");

                newPlace.backgroundImageSet = Imageset.FromXMLNode(imageSet);

            }
            XmlNode study = Util.SelectSingleNode(place, "ForegroundImageSet");
            if (study != null)
            {
                XmlNode imageSet = Util.SelectSingleNode(study, "ImageSet");

                newPlace.studyImageset = Imageset.FromXMLNode(imageSet);

            }
            study = Util.SelectSingleNode(place, "ImageSet");
            if (study != null)
            {

                newPlace.studyImageset = Imageset.FromXMLNode(study);

            }
            return newPlace;
        }
        //internal static TourPlace FromAstroObjectsRow(AstroObjectsDataset.spGetAstroObjectsRow row)
        //{
        //    TourPlace newPlace = new TourPlace();

        //    string seperator = "";

        //    string name = "";

        //    if (!row.IsPopularName1Null() && !String.IsNullOrEmpty(row.PopularName1))
        //    {
        //        name = ProperCaps(row.PopularName1);
        //        seperator = ";";
        //    }

        //    if (!row.IsMessierNameNull() && !String.IsNullOrEmpty(row.MessierName))
        //    {
        //        name = name + seperator + row.MessierName;
        //        seperator = ";";
        //    }

        //    if (!row.IsNGCNameNull() && !String.IsNullOrEmpty(row.NGCName))
        //    {
        //        name = name + seperator + row.NGCName;
        //        seperator = ";";
        //    }

        //    newPlace.name = name;
        //    newPlace.Type = ImageSetType.Sky;
        //    newPlace.Lat = row.Dec2000;
        //    newPlace.Lng = row.Ra2000 / 15;
        //    newPlace.constellation = Constellations.Abbreviation(row.ConstellationName);
        //    newPlace.Classification = Classification.Galaxy; //(Classification)Enum.Parse(typeof(Classification), place.Attributes["Classification"].Value);
        //    newPlace.magnitude = row.IsVisualMagnitudeNull() ? row.VisualMagnitude : 0;
        //    newPlace.AngularSize = 0; // todo fix this
        //    newPlace.ZoomLevel = .00009;
        //    return newPlace;
        //}
        static string ProperCaps(string name)
        {
            string[] list = name.Split(" ");

            string ProperName = "";

            foreach (string part in list)
            {
                ProperName = ProperName + part.Substr(0,1).ToUpperCase() + (part.Length > 1 ? part.Substr(1).ToLowerCase() : "") + " ";
            }

            return ProperName.Trim();

        }

        #region IThumbnail Members

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

        #endregion
        public bool IsImage
        {
            get
            {
                return studyImageset != null || backgroundImageSet != null;
            }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public List<IThumbnail> Children
        {
            get { return new List<IThumbnail>(); }
        }

        #region IThumbnail Members


        public bool ReadOnly
        {
            get { return true; }
        }

        #endregion

        #region IPlace Members

        public SolarSystemObjects Target
        {
            get
            {
                return camParams.Target;
            }
            set
            {
                camParams.Target = value;
            }
        }

        #endregion

        #region IThumbnail Members


        public bool IsCloudCommunityItem
        {
            get { return false; }
        }

        #endregion
    }
}
