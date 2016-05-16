using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Net;

namespace wwtlib
{
 
    public enum UserLevel { Beginner=0, Intermediate=1, Advanced=2, Educator=3, Professional=4 };
    public class TourDocument
    {
        int tourDirty = 0;
        public bool TourDirty
        {
            get { return tourDirty > 0; }
            set 
            {
                if (value)
                {
                    tourDirty++;
                }
                else
                {
                    tourDirty = 0;
                }

            }
        }

        public TourDocument()
        {
            id = Guid.NewGuid().ToString();
        }
        string workingDirectory = "";
        public string WorkingDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = TourDocument.BaseWorkingDirectory + id.ToString() + @"\";
                }

                //if (!Directory.Exists(workingDirectory))
                //{
                //    Directory.CreateDirectory(workingDirectory);
                //}

                return workingDirectory;
            }
            set
            {
                workingDirectory = value;
            }
        }
        public static string BaseWorkingDirectory
        {
            get
            {
                //return Settings.Active.CahceDirectory + @"Temp\";
                return  @"";
            }

        }

        public string Url = "";
        private WebFile webFile;
        private Action callMe;
        public static TourDocument FromUrl(string url, Action callMe)
        {
            
            TourDocument temp = new TourDocument();
            temp.Url = url;
            temp.callMe = callMe;

            temp.webFile = new WebFile(Util.GetTourComponent(url, "master"));
            temp.webFile.OnStateChange = temp.LoadXmlDocument;
            temp.webFile.Send();

            return temp;
        }

        private void LoadXmlDocument()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                FromXml(webFile.GetXml());
                callMe();
            }
        }
        
   
        public void FromXml(XmlDocument doc)
        {

            XmlNode root = Util.SelectSingleNode(doc, "Tour");


            id = root.Attributes.GetNamedItem("ID").Value.ToString();
            Title = root.Attributes.GetNamedItem("Title").Value.ToString();
            Author = root.Attributes.GetNamedItem("Author").Value.ToString();

            if (root.Attributes.GetNamedItem("Descirption") != null)
            {
                Description = root.Attributes.GetNamedItem("Descirption").Value;
            }

            if (root.Attributes.GetNamedItem("AuthorEmail") != null)
            {
                authorEmail = root.Attributes.GetNamedItem("AuthorEmail").Value;
            }

            if (root.Attributes.GetNamedItem("Keywords") != null)
            {
                Keywords = root.Attributes.GetNamedItem("Keywords").Value;
            }

            if (root.Attributes.GetNamedItem("OrganizationName") != null)
            {
                OrgName = root.Attributes.GetNamedItem("OrganizationName").Value;
            }

            

            organizationUrl = root.Attributes.GetNamedItem("OrganizationUrl").Value;
 
            switch (root.Attributes.GetNamedItem("UserLevel").Value)
            {

                case "Beginner":
                    level = UserLevel.Beginner;
                    break;
                case "Intermediate":
                    level = UserLevel.Intermediate;
                    break;
                case "Advanced":
                    level = UserLevel.Advanced;
                    break;
                case "Educator":
                    level = UserLevel.Educator;
                    break;
                case "Professional":
                    level = UserLevel.Professional;
                    break;
                default:
                    break;
            }


            switch (root.Attributes.GetNamedItem("Classification").Value)
            {

                case "Star":
                    type = Classification.Star;
                    break;
                case "Supernova":
                    type = Classification.Supernova;
                    break;
                case "BlackHole":
                    type = Classification.BlackHole;
                    break;
                case "NeutronStar":
                    type = Classification.NeutronStar;
                    break;
                case "DoubleStar":
                    type = Classification.DoubleStar;
                    break;
                case "MultipleStars":
                    type = Classification.MultipleStars;
                    break;
                case "Asterism":
                    type = Classification.Asterism;
                    break;
                case "Constellation":
                    type = Classification.Constellation;
                    break;
                case "OpenCluster":
                    type = Classification.OpenCluster;
                    break;
                case "GlobularCluster":
                    type = Classification.GlobularCluster;
                    break;
                case "NebulousCluster":
                    type = Classification.NebulousCluster;
                    break;
                case "Nebula":
                    type = Classification.Nebula;
                    break;
                case "EmissionNebula":
                    type = Classification.EmissionNebula;
                    break;
                case "PlanetaryNebula":
                    type = Classification.PlanetaryNebula;
                    break;
                case "ReflectionNebula":
                    type = Classification.ReflectionNebula;
                    break;
                case "DarkNebula":
                    type = Classification.DarkNebula;
                    break;
                case "GiantMolecularCloud":
                    type = Classification.GiantMolecularCloud;
                    break;
                case "SupernovaRemnant":
                    type = Classification.SupernovaRemnant;
                    break;
                case "InterstellarDust":
                    type = Classification.InterstellarDust;
                    break;
                case "Quasar":
                    type = Classification.Quasar;
                    break;
                case "Galaxy":
                    type = Classification.Galaxy;
                    break;
                case "SpiralGalaxy":
                    type = Classification.SpiralGalaxy;
                    break;
                case "IrregularGalaxy":
                    type = Classification.IrregularGalaxy;
                    break;
                case "EllipticalGalaxy":
                    type = Classification.EllipticalGalaxy;
                    break;
                case "Knot":
                    type = Classification.Knot;
                    break;
                case "PlateDefect":
                    type = Classification.PlateDefect;
                    break;
                case "ClusterOfGalaxies":
                    type = Classification.ClusterOfGalaxies;
                    break;
                case "OtherNGC":
                    type = Classification.OtherNGC;
                    break;
                case "Unidentified":
                    type = Classification.Unidentified;
                    break;
                case "SolarSystem":
                    type = Classification.SolarSystem;
                    break;
                case "Unfiltered":
                    type = Classification.Unfiltered;
                    break;
                case "Stellar":
                    type = Classification.Stellar;
                    break;
                case "StellarGroupings":
                    type = Classification.StellarGroupings;
                    break;
                case "Nebulae":
                    type = Classification.Nebulae;
                    break;
                case "Galactic":
                    type = Classification.Galactic;
                    break;
                case "Other":
                    type = Classification.Other;
                    break;
                default:
                    break;
            }

            taxonomy = root.Attributes.GetNamedItem("Taxonomy").Value.ToString();
            XmlNode TourStops = Util.SelectSingleNode(root, "TourStops");
            foreach (XmlNode tourStop in TourStops.ChildNodes)
            {
                if (tourStop.Name == "TourStop")
                {
                    AddTourStop(TourStop.FromXml(this, tourStop));
                }
            }

            XmlNode Frames = Util.SelectSingleNode(root, "ReferenceFrames");

            if (Frames != null)
            {
                foreach (XmlNode frame in Frames.ChildNodes)
                {
                    if (frame.Name == "ReferenceFrame")
                    {
                        ReferenceFrame newFrame = new ReferenceFrame();
                        newFrame.InitializeFromXml(frame);
                        if (!LayerManager.AllMaps.ContainsKey(newFrame.Name))
                        {
                            LayerMap map = new LayerMap(newFrame.Name, ReferenceFrames.Custom);
                            map.Frame = newFrame;
                            map.LoadedFromTour = true;
                            LayerManager.AllMaps[newFrame.Name] = map;
                        }
                    }
                }
                LayerManager.ConnectAllChildren();
                LayerManager.LoadTree();
            }

            XmlNode Layers = Util.SelectSingleNode(root, "Layers");

            if (Layers != null)
            {
                foreach (XmlNode layer in Layers.ChildNodes)
                {
                    if (layer.Name == "Layer")
                    {
                        
                        Layer newLayer = Layer.FromXml(layer,true);//.Layer.FromXml(layer, true);
                        if (newLayer != null)
                        {
                            string fileName = string.Format("{0}.txt", newLayer.ID.ToString());
                            if (LayerManager.LayerList.ContainsKey(newLayer.ID)) // && newLayer.ID != ISSLayer.ISSGuid)
                            {
                                //if (!CollisionChecked)
                                //{
                                //    if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                //    {
                                //        OverWrite = true;
                                //    }
                                //    else
                                //    {
                                //        OverWrite = false;

                                //    }
                                //    CollisionChecked = true;
                                //}

                                //if (OverWrite)
                                //{
                                LayerManager.DeleteLayerByID(newLayer.ID, true, false);
                                //}
                            }
                            try
                            {
                                newLayer.LoadedFromTour = true;
                                newLayer.LoadData(GetFileStream(fileName));
                                LayerManager.Add(newLayer, false);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                LayerManager.LoadTree();
            }

            //todo author
            //if (File.Exists(WorkingDirectory + "Author.png"))
            //{
            //    authorImage = UiTools.LoadBitmap(WorkingDirectory + "Author.png");
            //}

            tourDirty = 0;

        }
       
        string tagId;

        public string TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }




        public string AuthorThumbnailFilename
        {
            get
            {
                return "Author.Png";
            }
        }

        private int representativeThumbnailTourstop = 0;

        public string TourThumbnailFilename
        {
            get
            {
                if (representativeThumbnailTourstop < tourStops.Count)
                {
                    return tourStops[representativeThumbnailTourstop].TourStopThumbnailFilename;
                }
                else
                {
                    return null;
                }
            }
        }        

      

        string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                TourDirty = true;
            }
        }

        int runTime = 0;
        int lastDirtyCheck = 0;

        public int RunTime
        {
            get
            {
                //todo this is in ms right now... 
                if (runTime == 0 || lastDirtyCheck != tourDirty)
                {
                    runTime = CalculateRunTime();
                    lastDirtyCheck = tourDirty;
                }
                return runTime;
            }
        }

        string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                TourDirty = true;
            }
        }
        string attributesAndCredits;

        public string AttributesAndCredits
        {
            get { return attributesAndCredits; }
            set
            {
                attributesAndCredits = value;
                TourDirty = true;
            }
        } 
        
        string authorEmailOther;

        public string AuthorEmailOther
        {
            get { return authorEmailOther; }
            set
            {
                authorEmailOther = value;
                TourDirty = true;
            }
        } 

        string authorEmail;

        public string AuthorEmail
        {
            get { return authorEmail; }
            set
            {
                authorEmail = value;
                TourDirty = true;
            }
        } 

        string authorUrl;

        public string AuthorUrl
        {
            get { return authorUrl; }
            set
            {
                authorUrl = value;
                TourDirty = true;
            }
        } 

        string authorPhone;

        public string AuthorPhone
        {
            get { return authorPhone; }
            set
            {
                authorPhone = value;
                TourDirty = true;
            }
        }

        string authorContactText;

        public string AuthorContactText
        {
            get { return authorContactText; }
            set
            {
                authorContactText = value;
                TourDirty = true;
            }
        }

        string orgName = "None";

        public string OrgName
        {
            get { return orgName; }
            set
            {
                orgName = value;
                TourDirty = true;
            }
        }

        string orgUrl;

        public string OrgUrl
        {
            get { return orgUrl; }
            set
            {
                orgUrl = value;
                TourDirty = true;
            }
        }

        string author;

        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                TourDirty = true;
            }
        }
        string authorImageUrl;

        public string AuthorImageUrl
        {
            get { return authorImageUrl; }
            set
            {
                authorImageUrl = value;
                TourDirty = true;
            }
        }
        
        ImageElement authorImage;

        public ImageElement AuthorImage
        {
            get { return authorImage; }
            set
            {
                authorImage = value;
                TourDirty = true;
            }
        }

        string organizationUrl;

        public string OrganizationUrl
        {
            get { return organizationUrl; }
            set
            {
                organizationUrl = value;
                TourDirty = true;
            }
        }


        String filename;

        public String FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        UserLevel level;

        public UserLevel Level
        {
            get { return level; }
            set
            {
                level = value;
                TourDirty = true;
            }
        }

        Classification type;

        public Classification Type
        {
            get { return type; }
            set
            {
                type = value;
                TourDirty = true;
            }
        }

        string taxonomy = "";

        public string Taxonomy
        {
            get { return taxonomy; }
            set
            {
                taxonomy = value;
                TourDirty = true;
            }
        }
        string keywords = "";

        public string Keywords
        {
            get { return keywords; }
            set
            {
                keywords = value;
                TourDirty = true;
            }
        }
        string objects = "";

        public string Objects
        {
            get { return objects; }
            set
            {
                objects = value;
                TourDirty = true;
            }
        }
        bool editMode = false;

        public bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
            }
        }

        public List<String> ExplicitTourLinks = new List<string>();
        public List<String> ImplicitTourLinks = new List<string>();

        List<TourStop> tourStops = new List<TourStop>();

        public List<TourStop> TourStops
        {
            get { return tourStops; }
            set { tourStops = value; }
        }

        int currentTourstopIndex = -1;

        public int CurrentTourstopIndex
        {
            get { return currentTourstopIndex; }
            set { currentTourstopIndex = value; }
        }

        public void AddTourStop(TourStop ts)
        {
            ts.Owner = this;

            TourStops.Add(ts);
            currentTourstopIndex = tourStops.Count - 1;

            TourDirty = true;
        }

        public void InsertTourStop(TourStop ts)
        {
            ts.Owner = this;
            if (currentTourstopIndex > -1)
            {
                TourStops.Insert(currentTourstopIndex, ts);
            }
            else
            {
                TourStops.Add(ts);
                currentTourstopIndex = tourStops.Count - 1;
            }
            TourDirty = true;
        }

        public void InsertAfterTourStop(TourStop ts)
        {
            ts.Owner = this;
            if (currentTourstopIndex > -1 || currentTourstopIndex < TourStops.Count)
            {
                TourStops.Insert(currentTourstopIndex+1, ts);
            }
            else
            {
                TourStops.Add(ts);
                currentTourstopIndex = tourStops.Count - 1;
            }
            TourDirty = true;
        }

        public void RemoveTourStop(TourStop ts)
        {
            tourStops.Remove(ts);
            if (currentTourstopIndex > tourStops.Count - 1)
            {
                currentTourstopIndex--;
            }
            TourDirty = true;
        }

        private int CalculateRunTime()
        {
            double totalTime = 0.0;
            for (int i = 0; i < tourStops.Count; i++)
            {
                totalTime += (double)(tourStops[i].Duration);
                if (i > 0)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            if (tourStops[i].Target.BackgroundImageset == null || (tourStops[i - 1].Target.BackgroundImageset.DataSetType == tourStops[i].Target.BackgroundImageset.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageset.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target))))
                            {
                                CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                                ViewMoverSlew slew = ViewMoverSlew.Create(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime*1000;
                            }
                            break;
                        case TransitionType.Instant:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeToBlack:
                            break;
                        default:
                            break;
                    }
                }
            }
            return (int)totalTime;
        }

        public double ElapsedTimeTillTourstop(int index)
        {
            if (index == 0 && index >= tourStops.Count)
            {
                return 0;
            }
            double totalTime = 0.0;
            for (int i = 0; i < index; i++)
            {
                totalTime += (double)(tourStops[i].Duration);
                if (i > 0)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                            if (tourStops[i - 1].Target.BackgroundImageset.DataSetType == tourStops[i].Target.BackgroundImageset.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageset.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target)))
                            {
                                ViewMoverSlew slew = ViewMoverSlew.Create(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime*1000;
                            }
                            break;
                        case TransitionType.Instant:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeToBlack:
                            break;
                        default:
                            break;
                    }
                }
            }
            return totalTime/1000;
        }

        public MasterTime ElapsedTimeSinceLastMaster(int index)
        {
            TourStop masterOut = null;
            if (index == 0 && index >= tourStops.Count)
            {
                return null;
            }
            double totalTime = 0.0;
            for (int i = 0; i < index; i++)
            {
                if (tourStops[i].MasterSlide)
                {
                    totalTime = 0;
                    masterOut = tourStops[i];
                }

                totalTime += (double)(tourStops[i].Duration);
                if (i > 0)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                            if (tourStops[i - 1].Target.BackgroundImageset.DataSetType == tourStops[i].Target.BackgroundImageset.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageset.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target)))
                            {
                                ViewMoverSlew slew = ViewMoverSlew.Create(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime*1000;
                            }
                            break;
                        case TransitionType.Instant:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeToBlack:
                            break;
                        default:
                            break;
                    }
                }
            }


            return new MasterTime(masterOut, totalTime / 1000);
        }

        public TourStop GetMasterSlideForIndex(int index)
        {
            int master = -1;
            for (int i = 0; i < index; i++)
            {
                if (tourStops[i].MasterSlide)
                {
                    master = i;
                }
            }
            if (master == -1)
            {
                return null; 
            }

            return tourStops[master];
        }

        public int GetTourStopIndexByID(string id)
        {
            if (id == "" || id == "Next")
            {
                return currentTourstopIndex++;
            }

            int index = 0;
            foreach (TourStop stop in tourStops)
            {
                if (stop.Id == id)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public void CleanUp()
        {
            foreach (TourStop stop in TourStops)
            {
                stop.CleanUp();
            }
            if (textureList != null)
            {
               
                textureList.Clear();
            }
           
        }
        private Dictionary<string, ImageElement> textureList;
        public ImageElement GetCachedTexture(string filename, Action callMe)
        {

            if (textureList == null)
            {
                textureList = new Dictionary<string, ImageElement>();
            }

            if (textureList.ContainsKey(filename))
            {
                callMe();
                return textureList[filename];
            }
            ImageElement texture = (ImageElement)Document.CreateElement("img");

            texture.Src = GetFileStream(filename);
            texture.AddEventListener("load", delegate {callMe();}, false);
            
            textureList[filename] = texture;

            return texture;

        }

        public string GetFileStream(string filename)
        {
            return Util.GetTourComponent(Url, filename);
        }


        public TourStop CurrentTourStop
        {
            get
            {
                if (currentTourstopIndex > -1)
                {
                    return TourStops[currentTourstopIndex];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int i = 0;
                foreach (TourStop stop in TourStops)
	            {
                    if (stop == value)
                    {
                        if (currentTourstopIndex > -1)
                        {
                            TourStops[currentTourstopIndex].CleanUp();
                        }
                        currentTourstopIndex = i;
                        break;
                    }
            		i++;
	            }
            }
        }
        public bool DontCleanUpTempFiles = false;
        public void ClearTempFiles()
        {
           
        }
    }
  
}
