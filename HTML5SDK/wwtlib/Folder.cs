using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Net;


namespace wwtlib
{

    public partial class Folder : IThumbnail
    {
        public override string ToString()
        {
            return nameField;
        }

        public Folder Parent = null;

        public bool IsProxy = false;

        private bool versionDependent = false;

        public bool VersionDependent
        {
            get { return versionDependent; }
            set
            {
                versionDependent = value;
                foreach (Folder folder in folders)
                {
                    folder.VersionDependent = versionDependent;
                }
            }
        }

        bool readOnly = true;

        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        bool dirty = false;


        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        WebFile webFile;
        Action onComplete;
        public void LoadFromUrl(string url, Action complete)
        {
            onComplete = complete;

            webFile = new WebFile(Util.GetProxiedUrl(url));
            webFile.OnStateChange = LoadData;
            webFile.Send();

        }

        private void LoadData()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                XmlNode node = Util.SelectSingleNode(webFile.GetXml(), "Folder");

                if (node == null)
                {
                    XmlDocument doc = webFile.GetXml();
                    if (doc != null)
                    {
                        node = Util.SelectSingleNode(doc, "Folder");
                    }
                }

                if (node != null)
                {
                    ClearChildren();
                    ParseXML(node);
                }
                if (onComplete != null)
                {
                    onComplete();
                }
            }
        }

        private void ClearChildren()
        {
            folders.Clear();
            tours.Clear();
            places.Clear();
            Imagesets.Clear();
        }


        //public static Folder LoadFromXML(XmlNode node)
        //{
        //    Folder temp = new Folder();

        //    temp.ParseXML(node);

        //    return temp;
        //}

        private void ParseXML(XmlNode node)
        {
            if (node.Attributes.GetNamedItem("Name") != null)
            {
                nameField = node.Attributes.GetNamedItem("Name").Value;
            }
            else
            {
                nameField = "";
            }
            if (node.Attributes.GetNamedItem("Url") != null)
            {
                urlField = node.Attributes.GetNamedItem("Url").Value;
            }

            if (node.Attributes.GetNamedItem("Thumbnail") != null)
            {
                thumbnailUrlField = node.Attributes.GetNamedItem("Thumbnail").Value;
            }
           
            // load Children

            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "Folder":
                        Folder temp = new Folder();
                        temp.Parent = this;
                        //if (Parent != null && IsProxy)
                        //{
                        //    temp.Parent = Parent.Parent;
                        //}
                        temp.ParseXML(child);
                        folders.Add(temp);
                        break;
                    case "Place":
                        places.Add(Place.FromXml(child));
                        break;
                    case "ImageSet":
                        Imagesets.Add(Imageset.FromXMLNode(child));
                        break;
                    case "Tour":
                        Tours.Add(Tour.FromXml(child));
                        break;
                }
            }
     

        //bool Browseable { get; set; }
        //System.Collections.Generic.List<Folder> Folders { get; set; }
        //FolderGroup Group { get; set; }
        //System.Collections.Generic.List<Imageset> Imagesets { get; set; }
        //long MSRCommunityId { get; set; }
        //long MSRComponentId { get; set; }
        //string Name { get; set; }
        //long Permission { get; set; }
        //System.Collections.Generic.List<Place> Places { get; set; }
        //bool ReadOnly { get; set; }
        //string RefreshInterval { get; set; }
        //FolderRefreshType RefreshType { get; set; }
        //bool RefreshTypeSpecified { get; set; }
        //bool Searchable { get; set; }
        //string SubType { get; set; }
        //string ThumbnailUrl { get; set; }
        //System.Collections.Generic.List<Tour> Tours { get; set; }
        //FolderType Type { get; set; }
        //string Url { get; set; }
        }

        public void AddChildFolder(Folder child)
        {
            folders.Add(child);
            dirty = true;
        }

        public void RemoveChildFolder(Folder child)
        {
            folders.Remove(child);
            dirty = true;
        }

        public void AddChildPlace(Place child)
        {
            places.Add(child);
            dirty = true;
        }

        public void RemoveChildPlace(Place child)
        {
            places.Remove(child);
            dirty = true;
        }

        ImageElement thumbnail = null;

        public ImageElement Thumbnail
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
            get { return false; }
        }


        public bool IsTour
        {
            get { return false; }
        }


        public bool IsFolder
        {
            get { return true; }
        }


        public bool IsCloudCommunityItem
        {
            get
            {
                return communityIdField != 0 || this.permissionField > 0;
            }
        }
        private Folder proxyFolder = null;

        public void Refresh()
        {
            if (proxyFolder == null)
            {
                proxyFolder = new Folder();
                proxyFolder.IsProxy = true;
                proxyFolder.Parent = Parent;
            }

            proxyFolder.LoadFromUrl(urlField, childReadyCallback);
            childReadyCallback = null;
        }

        private Date lastUpdate = new Date();

        Action childReadyCallback;

        public void ChildLoadCallback(Action callback)
        {
            childReadyCallback = callback;

            List<IThumbnail> temp = Children;

            if (proxyFolder == null)
            {
                callback();
            }
            

        }
        List<IThumbnail> childList = new List<IThumbnail>();
        public List<IThumbnail> Children
        {
            get
            {
                if (String.IsNullOrEmpty(urlField))
                {
                    childList.Clear();

                    if (Parent != null)
                    {
                        FolderUp folderUp = new FolderUp();
                        //if (this.IsProxy)
                        //{
                        //    folderUp.Parent = Parent.Parent;
                        //}
                        //else
                        {
                            folderUp.Parent = Parent;
                        }
                        childList.Add(folderUp);
                    }

                    if (Folders != null)
                    {
                        foreach (Folder folder in Folders)
                        {
                            childList.Add(folder);
                        }
                    }
                    if (Imagesets != null)
                    {
                        foreach (Imageset imset in Imagesets)
                        {
                            childList.Add(imset);
                        }
                    }
                    if (Places != null)
                    {
                        foreach (Place place in Places)
                        {
                            childList.Add(place);
                        }
                    }
                    if (Tours != null)
                    {
                        foreach (Tour tour in Tours)
                        {
                            childList.Add(tour);
                        }
                    }


                    return childList;
                }
                else
                {
                    int ts = (lastUpdate - Date.Now)/1000;
                    // TOdo add add Move Complete Auto Update
                    // todo add URL formating for Ambient Parameters
                    // TODO remove true when perth fixes refresh type on server
                    if (true || RefreshType == FolderRefreshType.ConditionalGet || proxyFolder == null ||
                        (this.RefreshType == FolderRefreshType.Interval && (int.Parse(refreshIntervalField) < ts)))
                    {
                        Refresh();
                    }

                    if (proxyFolder != null)
                    {
                        return proxyFolder.Children;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
        }



        private List<Place> itemsField = new List<Place>();
        private List<Imageset> imagesets = new List<Imageset>();
        private List<Tour> tours = new List<Tour>();
        private List<Folder> folders = new List<Folder>();
        private List<Place> places = new List<Place>();
      
        
        private string nameField;

        private FolderGroup groupField;

        private string urlField;

        private string thumbnailUrlField;

        private FolderRefreshType refreshTypeField;

        private bool refreshTypeFieldSpecified;

        private string refreshIntervalField;

        private bool browseableField = true;

        private bool browseableFieldSpecified;

        private bool searchableField = false;

        private FolderType typeField;

        private string subTypeField;


        /// <remarks/>
        /// 

        long communityIdField = 0;

        public long MSRCommunityId
        {
            get { return communityIdField; }
            set { communityIdField = value; }
        }

        long componentIdField = 0;

        public long MSRComponentId
        {
            get { return componentIdField; }
            set { componentIdField = value; }
        }

        long permissionField = 0;

        public long Permission
        {
            get { return permissionField; }
            set { permissionField = value; }
        }


        public List<Folder> Folders
        {
            get
            {
                return folders;
            }
            set
            {
                folders = value;
            }
        }


        public List<Place> Places
        {
            get
            {
                return places;
            }
            set
            {
                places = value;
            }
        }


        public List<Imageset> Imagesets
        {
            get
            {
                return imagesets;
            }
            set
            {
                imagesets = value;
            }
        }



        public List<Tour> Tours
        {
            get
            {
                return this.tours;
            }
            set
            {
                this.tours = value;
            }
        }

        /// <remarks/>

        public string Name
        {
            get
            {
                if (this.nameField == null)
                {
                    return "";
                }
                else
                {
                    return this.nameField;
                }
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>

        public FolderGroup Group
        {
            get
            {
                return this.groupField;
            }
            set
            {
                this.groupField = value;
            }
        }

        public string Url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        public string ThumbnailUrl
        {
            get
            {
                if (string.IsNullOrEmpty(thumbnailUrlField))
                {

                    return "//cdn.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=folder";
                }

                return this.thumbnailUrlField;
            }
            set
            {
                this.thumbnailUrlField = value;
            }
        }

        /// <remarks/>

        public FolderRefreshType RefreshType
        {
            get
            {
                return this.refreshTypeField;
            }
            set
            {
                this.refreshTypeField = value;
                RefreshTypeSpecified = true;
            }
        }


        public bool RefreshTypeSpecified
        {
            get
            {
                return this.refreshTypeFieldSpecified;
            }
            set
            {
                this.refreshTypeFieldSpecified = value;
            }
        }


        public string RefreshInterval
        {
            get
            {
                return this.refreshIntervalField;
            }
            set
            {
                this.refreshIntervalField = value;
            }
        }

        /// <remarks/>

        public bool Browseable
        {
            get
            {
                return this.browseableField;
            }
            set
            {
                this.browseableField = value;
                browseableFieldSpecified = true;
            }
        }


        public bool BrowseableSpecified
        {
            get
            {
                return this.browseableFieldSpecified;
            }
            set
            {
                this.browseableFieldSpecified = value;
            }
        }

        /// <remarks/>

        public bool Searchable
        {
            get
            {
                return this.searchableField;
            }
            set
            {
                this.searchableField = value;
            }
        }

        /// <remarks/>

        public FolderType Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>

        public string SubType
        {
            get
            {
                return this.subTypeField;
            }
            set
            {
                this.subTypeField = value;
            }
        }
    }



    public enum FolderGroup
    {

        /// <remarks/>
        Explorer = 0,

        /// <remarks/>
        Tour = 1,

        /// <remarks/>
        Search = 2,

        /// <remarks/>
        Constellation = 3,

        /// <remarks/>
        View = 4,

        /// <remarks/>
        GoTo = 5,

        /// <remarks/>
        Community = 6,

        /// <remarks/>
        Context = 7,

        /// <remarks/>
        VoTable = 8,

        /// <remarks/>
        ImageStack = 9
    }


    public enum FolderRefreshType
    {

        /// <remarks/>
        Interval = 0,

        /// <remarks/>
        ConditionalGet = 1,

        /// <remarks/>
        ViewChange = 2,
    }



    public enum FolderType
    {

        /// <remarks/>
        Earth = 0,

        /// <remarks/>
        Planet = 1,

        /// <remarks/>
        Sky = 2,

        /// <remarks/>
        Panorama = 3,
    }
}
