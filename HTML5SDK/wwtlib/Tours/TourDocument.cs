using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Net;
using System.Html.Data.Files;

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

        FileCabinet cabinet;

        public static TourDocument FromUrl(string url, Action callMe)
        {         
            TourDocument temp = new TourDocument();
            temp.Url = url;
            temp.callMe = callMe;

            temp.cabinet = FileCabinet.FromUrl(url, temp.LoadXmlDocument);
            return temp;
        }

        private void LoadXmlDocument()
        {
            string master = cabinet.MasterFile;

            FileReader doc = new FileReader();
            doc.OnLoadEnd = delegate (FileProgressEvent ee)
            {
                string data = doc.Result as string;
                XmlDocumentParser xParser = new XmlDocumentParser();
                FromXml(xParser.ParseFromString(data, "text/xml"));
                callMe();
            };
            doc.ReadAsText(cabinet.GetFileBlob(master));
           
        }

        public string Url = "";
        //private WebFile webFile;
        private Action callMe;
        //public static TourDocument FromUrl(string url, Action callMe)
        //{

        //    TourDocument temp = new TourDocument();
        //    temp.Url = url;
        //    temp.callMe = callMe;

        //    temp.webFile = new WebFile(Util.GetTourComponent(url, "master"));
        //    temp.webFile.OnStateChange = temp.LoadXmlDocument;
        //    temp.webFile.Send();

        //    return temp;
        //}

        //private void LoadXmlDocument()
        //{
        //    if (webFile.State == StateType.Error)
        //    {
        //        Script.Literal("alert({0})", webFile.Message);
        //    }
        //    else if (webFile.State == StateType.Received)
        //    {
        //        FromXml(webFile.GetXml());
        //        callMe();
        //    }
        //}


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

            level = (UserLevel)Enums.Parse("UserLevel",root.Attributes.GetNamedItem("UserLevel").Value);
            
            type = (Classification)Enums.Parse("Classification", root.Attributes.GetNamedItem("Classification").Value);
          
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
                                newLayer.LoadData(this, fileName);
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

        public string SaveToDataUrl()
        {
            return  (string)Script.Literal("URL.createObjectURL({0});", SaveToBlob());
        }

        public Blob SaveToBlob()
        {
            bool excludeAudio = false;

            CleanUp();

            string tourXml = GetTourXML();
            
            FileCabinet fc = new FileCabinet();
            fc.PackageID = this.Id;
           

            fc.AddFile("Tour.wwtxml", new Blob(new object[] { tourXml }));

            if (authorImage != null)
            {
                //todo add author image pipeline
             //   fc.AddFile(WorkingDirectory + "Author.Png");
            }

            foreach (TourStop stop in TourStops)
            {
                stop.AddFilesToCabinet(fc, excludeAudio);
            }

            List<Guid> masterList = CreateLayerMasterList();

            foreach (Guid id in masterList)
            {
                if (LayerManager.LayerList.ContainsKey(id))
                {
                    LayerManager.LayerList[id].AddFilesToCabinet(fc);
                }
            }
                        
            TourDirty = false;
            return fc.PackageFiles();
        }

        public string GetTourXML()
        { 
            XmlTextWriter xmlWriter = new XmlTextWriter();
            
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("Tour");

            xmlWriter.WriteAttributeString("ID", this.id);
            xmlWriter.WriteAttributeString("Title", this.title);
            xmlWriter.WriteAttributeString("Descirption", this.Description);
            xmlWriter.WriteAttributeString("Description", this.Description);
            xmlWriter.WriteAttributeString("RunTime", ((double)this.RunTime / 1000.0).ToString());
            xmlWriter.WriteAttributeString("Author", this.author);
            xmlWriter.WriteAttributeString("AuthorEmail", this.authorEmail);
            xmlWriter.WriteAttributeString("OrganizationUrl", this.organizationUrl);
            xmlWriter.WriteAttributeString("OrganizationName", this.OrgName);
            xmlWriter.WriteAttributeString("Keywords", this.Keywords);
            xmlWriter.WriteAttributeString("UserLevel", Enums.ToXml("UserLevel",(int)level));
            xmlWriter.WriteAttributeString("Classification", Enums.ToXml("Classification", (int)type));
            xmlWriter.WriteAttributeString("Taxonomy", taxonomy.ToString());
            // xmlWriter.WriteAttributeString("DomeMode", DomeMode.ToString());
            bool timeLineTour = IsTimelineTour();
            xmlWriter.WriteAttributeString("TimeLineTour", timeLineTour.ToString());

            
            xmlWriter.WriteStartElement("TourStops");
            foreach (TourStop stop in TourStops)
            {
                stop.SaveToXml(xmlWriter, true);
            }
            xmlWriter.WriteEndElement();
            

            List<Guid> masterList = CreateLayerMasterList();

            // This will now save and sync emtpy frames...
            List<ReferenceFrame> referencedFrames = GetReferenceFrameList();

            xmlWriter.WriteStartElement("ReferenceFrames");
            foreach (ReferenceFrame item in referencedFrames)
            {
                item.SaveToXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();


            xmlWriter.WriteStartElement("Layers");
            foreach (Guid id in masterList)
            {
                if (LayerManager.LayerList.ContainsKey(id))
                {
                    LayerManager.LayerList[id].SaveToXml(xmlWriter);
                }
            }
            xmlWriter.WriteEndElement();


            xmlWriter.WriteFullEndElement();
            xmlWriter.Close();
                
            return xmlWriter.Body;
        }

        private List<ReferenceFrame> GetReferenceFrameList()
        {
            List<ReferenceFrame> list = new List<ReferenceFrame>();

            foreach (string key in LayerManager.AllMaps.Keys)
            {
                LayerMap lm = LayerManager.AllMaps[key];
                if ((lm.Frame.Reference == ReferenceFrames.Custom || lm.Frame.Reference == ReferenceFrames.Identity) && !list.Contains(lm.Frame) && lm.Frame.SystemGenerated == false)
                {
                    list.Add(lm.Frame);
                }
            }

            return list;
        }

        private List<Guid> CreateLayerMasterList()
        {
            List<Guid> masterList = new List<Guid>();
            foreach (TourStop stop in TourStops)
            {
                foreach (Guid id in stop.Layers.Keys)
                {
                    if (!masterList.Contains(id))
                    {
                        if (LayerManager.LayerList.ContainsKey(id))
                        {
                            masterList.Add(id);
                        }
                    }
                }
            }
            return masterList;
        }

        private bool IsTimelineTour()
        {
            return false;
        }

        string tagId="";

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

      

        string id="";

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        string title="";

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

        string description="";

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                TourDirty = true;
            }
        }
        string attributesAndCredits="";

        public string AttributesAndCredits
        {
            get { return attributesAndCredits; }
            set
            {
                attributesAndCredits = value;
                TourDirty = true;
            }
        } 
        
        string authorEmailOther="";

        public string AuthorEmailOther
        {
            get { return authorEmailOther; }
            set
            {
                authorEmailOther = value;
                TourDirty = true;
            }
        } 

        string authorEmail="";

        public string AuthorEmail
        {
            get { return authorEmail; }
            set
            {
                authorEmail = value;
                TourDirty = true;
            }
        } 

        string authorUrl="";

        public string AuthorUrl
        {
            get { return authorUrl; }
            set
            {
                authorUrl = value;
                TourDirty = true;
            }
        } 

        string authorPhone="";

        public string AuthorPhone
        {
            get { return authorPhone; }
            set
            {
                authorPhone = value;
                TourDirty = true;
            }
        }

        string authorContactText = "";

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

        string orgUrl = "";

        public string OrgUrl
        {
            get { return orgUrl; }
            set
            {
                orgUrl = value;
                TourDirty = true;
            }
        }

        string author = "";

        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                TourDirty = true;
            }
        }
        string authorImageUrl = "";

        public string AuthorImageUrl
        {
            get { return authorImageUrl; }
            set
            {
                authorImageUrl = value;
                TourDirty = true;
            }
        }
        
        ImageElement authorImage = null;

        public ImageElement AuthorImage
        {
            get { return authorImage; }
            set
            {
                authorImage = value;
                TourDirty = true;
            }
        }

        string organizationUrl = "";

        public string OrganizationUrl
        {
            get { return organizationUrl; }
            set
            {
                organizationUrl = value;
                TourDirty = true;
            }
        }


        String filename = "";

        public String FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        UserLevel level = UserLevel.Beginner;

        public UserLevel Level
        {
            get { return level; }
            set
            {
                level = value;
                TourDirty = true;
            }
        }

        Classification type = Classification.Unidentified;

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
        //todo need change notifications.
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
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
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
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
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
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
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
            //foreach (TourStop stop in TourStops)
            //{
            //    stop.CleanUp();
            //}
            //if (textureList != null)
            //{
               
            //    textureList.Clear();
            //}
           
        }
        private Dictionary<string, ImageElement> textureList = new Dictionary<string, ImageElement>();
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
            string url =  GetFileStream(filename);

            if (!string.IsNullOrWhiteSpace(url))
            {
                ImageElement texture = (ImageElement)Document.CreateElement("img");

                texture.Src = GetFileStream(filename);
                texture.AddEventListener("load", delegate { callMe(); }, false);

                textureList[filename] = texture;

                return texture;
            }
            else
            {
                return null;
            }
        }

        private Dictionary<string, Texture> textureList2d = new Dictionary<string, Texture>();
        public Texture GetCachedTexture2d(string filename)
        {

            if (textureList2d == null)
            {
                textureList2d = new Dictionary<string, Texture>();
            }

            if (textureList2d.ContainsKey(filename))
            {
                return textureList2d[filename];
            }

            Texture texture = new Texture();

            texture.Load(GetFileStream(filename));
            textureList2d[filename] = texture;

            return texture;

        }

        // This handles new files added while editing a tour
        private Dictionary<string, Blob> fileCache = new Dictionary<string, Blob>();

        public void AddCachedFile( string filename, Blob file)
        {
            fileCache[filename] = file;

            //Clean up old Cached Textures if they are based on this file.
            if (textureList2d.ContainsKey(filename))
            {
                textureList2d.Remove(filename);
            }

            if (textureList.ContainsKey(filename))
            {
                textureList.Remove(filename);
            }
        }

        public string GetFileStream(string filename)
        {
            Blob blob = GetFileBlob(filename);
            if (blob == null)
            {
                return null;
            }

            return (string)Script.Literal("URL.createObjectURL({0});", blob); 
        }

        public Blob GetFileBlob(string filename)
        {
            if (fileCache.ContainsKey(filename))
            {
                return fileCache[filename];
            }
            else if (cabinet != null)
            {
                return cabinet.GetFileBlob(WorkingDirectory + filename);
            }
            else
            {
                return null;
            }
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
                        //    TourStops[currentTourstopIndex].CleanUp();
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
