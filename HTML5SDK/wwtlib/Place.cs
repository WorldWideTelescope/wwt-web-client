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
                        return "http://cdn.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=star";
                    }
                    return "http://cdn.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + name.ToLowerCase();
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



        //internal void SaveToXml(System.Xml.XmlTextWriter xmlWriter, string elementName)
        //{

        //    xmlWriter.WriteStartElement(elementName);
        //    xmlWriter.WriteAttributeString("Name", name);
        //    xmlWriter.WriteAttributeString("DataSetType", this.Type.ToString());
        //    if (this.Type == ImageSetType.Sky)
        //    {
        //        xmlWriter.WriteAttributeString("RA", camParams.RA.ToString());
        //        xmlWriter.WriteAttributeString("Dec", camParams.Dec.ToString());
        //    }
        //    else
        //    {
        //        xmlWriter.WriteAttributeString("Lat", Lat.ToString());
        //        xmlWriter.WriteAttributeString("Lng", Lng.ToString());
        //    }

        //    xmlWriter.WriteAttributeString("Constellation", constellation);
        //    xmlWriter.WriteAttributeString("Classification", Classification.ToString());
        //    xmlWriter.WriteAttributeString("Magnitude", magnitude.ToString());
        //    xmlWriter.WriteAttributeString("Distance", distnace.ToString());
        //    xmlWriter.WriteAttributeString("AngularSize", AngularSize.ToString());
        //    xmlWriter.WriteAttributeString("ZoomLevel", ZoomLevel.ToString());
        //    xmlWriter.WriteAttributeString("Rotation", camParams.Rotation.ToString());
        //    xmlWriter.WriteAttributeString("Angle", camParams.Angle.ToString());
        //    xmlWriter.WriteAttributeString("Opacity", camParams.Opacity.ToString());
        //    xmlWriter.WriteAttributeString("Target", Target.ToString());
        //    xmlWriter.WriteAttributeString("ViewTarget", camParams.ViewTarget.ToString());
        //    xmlWriter.WriteAttributeString("TargetReferenceFrame", camParams.TargetReferenceFrame);
        //    xmlWriter.WriteStartElement("Description");
        //    xmlWriter.WriteCData(HtmlDescription);
        //    xmlWriter.WriteEndElement();


        //    if (backgroundImageSet != null)
        //    {
        //        xmlWriter.WriteStartElement("BackgroundImageSet");
        //        ImageSetHelper.SaveToXml(xmlWriter, backgroundImageSet, "");
        //        xmlWriter.WriteEndElement();
        //    }

        //    if (studyImageset != null)
        //    {
        //        ImageSetHelper.SaveToXml(xmlWriter, studyImageset, "");
        //    }
        //    xmlWriter.WriteEndElement();
        //}

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
                switch (place.Attributes.GetNamedItem("DataSetType").Value.ToLowerCase())
                {
                    case "earth":
                        newPlace.type = ImageSetType.Earth;
                        break;
                    case "planet":
                        newPlace.type = ImageSetType.Planet;
                        break;
                    case "sky":
                        newPlace.type = ImageSetType.Sky;
                        break;
                    case "panorama":
                        newPlace.type = ImageSetType.Panorama;
                        break;
                    case "solarsystem":
                        newPlace.type = ImageSetType.SolarSystem;
                        break;
                }
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
            //todo change to switch/case
            if (place.Attributes.GetNamedItem("Classification") != null)
            {
                switch (place.Attributes.GetNamedItem("Classification").Value)
                {

                    case "Star":
                        newPlace.classification = Classification.Star;
                        break;
                    case "Supernova":
                        newPlace.classification = Classification.Supernova;
                        break;
                    case "BlackHole":
                        newPlace.classification = Classification.BlackHole;
                        break;
                    case "NeutronStar":
                        newPlace.classification = Classification.NeutronStar;
                        break;
                    case "DoubleStar":
                        newPlace.classification = Classification.DoubleStar;
                        break;
                    case "MultipleStars":
                        newPlace.classification = Classification.MultipleStars;
                        break;
                    case "Asterism":
                        newPlace.classification = Classification.Asterism;
                        break;
                    case "Constellation":
                        newPlace.classification = Classification.Constellation;
                        break;
                    case "OpenCluster":
                        newPlace.classification = Classification.OpenCluster;
                        break;
                    case "GlobularCluster":
                        newPlace.classification = Classification.GlobularCluster;
                        break;
                    case "NebulousCluster":
                        newPlace.classification = Classification.NebulousCluster;
                        break;
                    case "Nebula":
                        newPlace.classification = Classification.Nebula;
                        break;
                    case "EmissionNebula":
                        newPlace.classification = Classification.EmissionNebula;
                        break;
                    case "PlanetaryNebula":
                        newPlace.classification = Classification.PlanetaryNebula;
                        break;
                    case "ReflectionNebula":
                        newPlace.classification = Classification.ReflectionNebula;
                        break;
                    case "DarkNebula":
                        newPlace.classification = Classification.DarkNebula;
                        break;
                    case "GiantMolecularCloud":
                        newPlace.classification = Classification.GiantMolecularCloud;
                        break;
                    case "SupernovaRemnant":
                        newPlace.classification = Classification.SupernovaRemnant;
                        break;
                    case "InterstellarDust":
                        newPlace.classification = Classification.InterstellarDust;
                        break;
                    case "Quasar":
                        newPlace.classification = Classification.Quasar;
                        break;
                    case "Galaxy":
                        newPlace.classification = Classification.Galaxy;
                        break;
                    case "SpiralGalaxy":
                        newPlace.classification = Classification.SpiralGalaxy;
                        break;
                    case "IrregularGalaxy":
                        newPlace.classification = Classification.IrregularGalaxy;
                        break;
                    case "EllipticalGalaxy":
                        newPlace.classification = Classification.EllipticalGalaxy;
                        break;
                    case "Knot":
                        newPlace.classification = Classification.Knot;
                        break;
                    case "PlateDefect":
                        newPlace.classification = Classification.PlateDefect;
                        break;
                    case "ClusterOfGalaxies":
                        newPlace.classification = Classification.ClusterOfGalaxies;
                        break;
                    case "OtherNGC":
                        newPlace.classification = Classification.OtherNGC;
                        break;
                    case "Unidentified":
                        newPlace.classification = Classification.Unidentified;
                        break;
                    case "SolarSystem":
                        newPlace.classification = Classification.SolarSystem;
                        break;
                    case "Unfiltered":
                        newPlace.classification = Classification.Unfiltered;
                        break;
                    case "Stellar":
                        newPlace.classification = Classification.Stellar;
                        break;
                    case "StellarGroupings":
                        newPlace.classification = Classification.StellarGroupings;
                        break;
                    case "Nebulae":
                        newPlace.classification = Classification.Nebulae;
                        break;
                    case "Galactic":
                        newPlace.classification = Classification.Galactic;
                        break;
                    case "Other":
                        newPlace.classification = Classification.Other;
                        break;

                    default:
                        break;
                }
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

                switch (place.Attributes.GetNamedItem("Target").Value)
                {
                    case "Sun":
                        newPlace.Target = SolarSystemObjects.Sun;
                        break;
                    case "Mercury":
                        newPlace.Target = SolarSystemObjects.Mercury;
                        break;
                    case "Venus":
                        newPlace.Target = SolarSystemObjects.Venus;
                        break;
                    case "Mars":
                        newPlace.Target = SolarSystemObjects.Mars;
                        break;
                    case "Jupiter":
                        newPlace.Target = SolarSystemObjects.Jupiter;
                        break;
                    case "Saturn":
                        newPlace.Target = SolarSystemObjects.Saturn;
                        break;
                    case "Uranus":
                        newPlace.Target = SolarSystemObjects.Uranus;
                        break;
                    case "Neptune":
                        newPlace.Target = SolarSystemObjects.Neptune;
                        break;
                    case "Pluto":
                        newPlace.Target = SolarSystemObjects.Pluto;
                        break;
                    case "Moon":
                        newPlace.Target = SolarSystemObjects.Moon;
                        break;
                    case "Io":
                        newPlace.Target = SolarSystemObjects.Io;
                        break;
                    case "Europa":
                        newPlace.Target = SolarSystemObjects.Europa;
                        break;
                    case "Ganymede":
                        newPlace.Target = SolarSystemObjects.Ganymede;
                        break;
                    case "Callisto":
                        newPlace.Target = SolarSystemObjects.Callisto;
                        break;
                    case "IoShadow":
                        newPlace.Target = SolarSystemObjects.IoShadow;
                        break;
                    case "EuropaShadow":
                        newPlace.Target = SolarSystemObjects.EuropaShadow;
                        break;
                    case "GanymedeShadow":
                        newPlace.Target = SolarSystemObjects.GanymedeShadow;
                        break;
                    case "CallistoShadow":
                        newPlace.Target = SolarSystemObjects.CallistoShadow;
                        break;
                    case "SunEclipsed":
                        newPlace.Target = SolarSystemObjects.SunEclipsed;
                        break;
                    case "Earth":
                        newPlace.Target = SolarSystemObjects.Earth;
                        break;
                    case "Custom":
                        newPlace.Target = SolarSystemObjects.Custom;
                        break;
                    case "Undefined":
                        newPlace.Target = SolarSystemObjects.Undefined;
                        break;

                    default:
                        
                        break;
                }
            }

            if (place.Attributes.GetNamedItem("ViewTarget") != null)
            {
                newPlace.camParams.ViewTarget = Vector3d.Parse(place.Attributes.GetNamedItem("ViewTarget").Value);
            }

            //if (place.Attributes.GetNamedItem("TargetReferenceFrame") != null)
            //{
            //    newPlace.camParams.TargetReferenceFrame = place.Attributes.GetNamedItem("TargetReferenceFrame").Value;
            //}

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
