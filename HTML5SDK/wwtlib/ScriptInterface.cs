using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;
namespace wwtlib
{
    public class ScriptInterface
    {
        bool missedReady = false;
        public event EventHandler<EventArgs> Ready;
        internal void FireReady()
        {
            
            if (Ready != null)
            {
                Ready.Invoke(this, new EventArgs());
            }
            else
            {
                missedReady = true;
            }

            // This is to 
            //TestVoTable();
        }

        public void TestVoTable()
        {
            // This is to 
            table = VoTable.LoadFromUrl("http://casjobs.sdss.org/vo/dr5cone/sdssConeSearch.asmx/ConeSearch?ra=202.507695905339&dec=47.2148314989668&sr=0.26563787460365", CallMeMaybe);

        }

        VoTable table = null;

        private void CallMeMaybe()
        {
            //table is loaded now

            string name = table.Column[0].Name;
        }

        public event EventHandler<CollectionLoadedEventArgs> CollectionLoaded;
        
        internal void FireCollectionLoaded(string url)
        {
            if (CollectionLoaded != null)
            {
                CollectionLoaded.Invoke(this, new CollectionLoadedEventArgs(url));
            }

           
        }
       
        // public  EventHandler<ArrivedEventArgs> Arrived;
        public event EventHandler<ArrivedEventArgs> Arrived;
        public event EventHandler<ArrivedEventArgs> Clicked;
        public event EventHandler<AnnotationClickEventArgs> AnnotationClicked;
        
        public event EventHandler<EventArgs> ImageryLoaded;

		public event EventHandler<EventArgs> TourReady;

		public event EventHandler<EventArgs> TourPaused;

		public event EventHandler<EventArgs> TourResumed;

        public event EventHandler<EventArgs> TourEnded;

        public event EventHandler<SlideChangedEventArgs> SlideChanged;

		internal void FireTourReady()
		{
			if (TourReady != null)
			{
				TourReady.Invoke(this, new EventArgs());
			}
		}
		internal void FireTourPaused()
		{
			if (TourPaused != null)
			{
				TourPaused.Invoke(this, new EventArgs());
			}
		}
		internal void FireTourResume()
		{
			if (TourResumed != null)
			{
				TourResumed.Invoke(this, new EventArgs());
			}
		}

        internal void FireTourEnded()
        {
            if (TourEnded != null)
            {
                TourEnded.Invoke(this, new EventArgs());
            }
        }

        internal void FireImageryLoaded()
        {
            if (ImageryLoaded != null)
            {
                ImageryLoaded.Invoke(this, new EventArgs());
            }

        }

        internal void FireClick(double ra, double dec)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this, new ArrivedEventArgs(ra, dec, WWTControl.Singleton.RenderContext.ViewCamera.Zoom));
            }
        }

        internal void FireArrived(double ra, double dec, double zoom)
        {
            if (Arrived != null)
            {
                Arrived.Invoke(this, new ArrivedEventArgs(ra, dec, zoom));
            }
        }


        internal void FireAnnotationclicked(double RA, double Dec, string id)
        {
            try
            {
                if (AnnotationClicked != null)
                {
                    AnnotationClicked.Invoke(this, new AnnotationClickEventArgs(RA, Dec, id));
                }

            }
            catch
            {
            }
        }

        internal void FireSlideChanged(string caption)
        {
            try
            {
                if (SlideChanged != null)
                {
                    SlideChanged.Invoke(this, new SlideChangedEventArgs(caption));
                }
            }
            catch
            {
            }
        }

        public void EndInit()
        {
            if (missedReady)
            {
                FireReady();
            }
        }

        public void GotoRaDecZoom(double ra, double dec, double zoom, bool instant)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.GotoRADecZoom(ra / 15, dec, zoom * 6, instant);
            }
        }

        public void SetBackgroundImageByName(string name)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.SetBackgroundImageByName(name);
            }
        }

        // Call this to add a VOTable to layers
        public void AddVoTableLayer(VoTable table)
        {
            LayerManager.AddVoTableLayer(table, "Vo Table");
        }

        public void SetForegroundImageByName(string name)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.SetForegroundImageByName(name);
                WWTControl.Singleton.RenderContext.ViewCamera.Opacity = 100;
            }
        }

        public void SetForegroundOpacity(double opacity)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.RenderContext.ViewCamera.Opacity = opacity;
            }
        }

        public void HideUI(bool hide)
        {
            //todo enable
            ////App.NoUI = hide;
            //this.ContextPanel.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            //this.Explorer.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            //this.TabMenus.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            //Viewer.MasterView.SetRenderNeeded(true, false);
        }
        public void LoadTour(string url)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.PlayTour(url);
            }

        }

        public void LoadFits(string url)
        {
            FitsImage img = new FitsImage(url, null, OnWcsLoad);
        }

        private void OnWcsLoad(WcsImage wcsImage)
        {
            int width = (int)wcsImage.SizeX;
            int height = (int)wcsImage.SizeY;

            WWTControl.Singleton.RenderContext.ForegroundImageset = Imageset.Create(
                        wcsImage.Description,
                        "m51",
                        ImageSetType.Sky,
                        BandPass.Visible,
                        ProjectionType.SkyImage,
                        54123,
                        0,
                        0,
                        256,
                        wcsImage.ScaleY,
                        ".tif",
                        wcsImage.ScaleX > 0,
                        "",
                        wcsImage.CenterX,
                        wcsImage.CenterY,
                        wcsImage.Rotation,
                        false,
                        "",
                        false,
                        false,
                        1,
                        wcsImage.ReferenceX,
                        wcsImage.ReferenceY,
                        wcsImage.Copyright,
                        wcsImage.CreditsUrl,
                        "",
                        "",
                        0,
                        ""
                        );
            WWTControl.Singleton.RenderContext.ForegroundImageset.WcsImage = wcsImage;
            WWTControl.Singleton.RenderContext.ViewCamera.Opacity = 100;
        }

        public bool hideTourFeedback = false;
        public bool HideTourFeedback
        {
            get
            {
                return hideTourFeedback;
            }
            set
            {
                hideTourFeedback = value;
            }
        }

        public void PlayTour()
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.PlayCurrentTour();
            }

        }

        public void StopTour()
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.StopCurrentTour();
            }
        }

        private Folder imageFolder;
        private string imageUrl;
        public void LoadImageCollection(string url)
        {

            imageUrl = url;
            imageFolder = new Folder();
            imageFolder.LoadFromUrl(url, delegate { Wtml.LoadImagesets(imageFolder); FireCollectionLoaded(url); });

        }

        private void ImageFileLoaded()
        {
            FireCollectionLoaded(imageUrl);
        }

        //public void CollectionReady()
        //{
        //    Folder wtml = Folder.LoadFromFileStream(e.Stream, true);
        //    foreach (object child in wtml.Children)
        //    {
        //        ImageSet imageset = null;


        //        if (child is ImageSet)
        //        {
        //            imageset = (ImageSet)child;
        //            Viewer.ImageSets.Add(imageset);
        //        }
        //        if (child is Place)
        //        {
        //            Place place = (Place)child;
        //            if (place.ForegroundImageSet != null)
        //            {
        //                Viewer.ImageSets.Add(place.ForegroundImageSet.ImageSet);
        //            }
        //            else if (place.BackgroundImageSet != null)
        //            {
        //                Viewer.ImageSets.Add(place.BackgroundImageSet.ImageSet);
        //            }
        //        }
        //    }
        //    FireCollectionLoaded(afc.Url);
        //}

        public void Zoom(double factor)
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.Zoom(factor);
            }
            return ;
        }

        public double GetRA()
        {
            if (WWTControl.Singleton != null)
            {
                return WWTControl.Singleton.RenderContext.RA;
            }
            return 0;
        }

        public double GetDec()
        {
            if (WWTControl.Singleton != null)
            {
                return WWTControl.Singleton.RenderContext.Dec;
            }
            return 0;
        }

        public Folder CreateFolder()
        {
            Folder folder = new Folder();
            return folder;
        }

        public Poly CreatePolygon(bool fill)
        {
            Poly p = new Poly();
            p.Fill = fill;

            return p;
        }

        public PolyLine CreatePolyLine(bool fill)
        {
            return new PolyLine();
        }

        public Circle CreateCircle(bool fill)
        {
            Circle c = new Circle();
            c.Fill = fill;
            return c;
        }

        public void AddAnnotation(Annotation annotation)
        {
            if (annotation != null && annotation is Annotation)
            {
                if (WWTControl.Singleton != null)
                {
                    WWTControl.Singleton.AddAnnotation(annotation);
                }
            }
        }

        public void RemoveAnnotation(Annotation annotation)
        {
            if (annotation != null)
            {
                if (WWTControl.Singleton != null)
                {
                    WWTControl.Singleton.RemoveAnnotation(annotation);
                }
            }
        }

        public void ClearAnnotations()
        {
            if (WWTControl.Singleton != null)
            {
                WWTControl.Singleton.ClearAnnotations();
            }
        }
        bool smoothAnimation = false;
        public bool SmoothAnimation
        {
            get
            {
                return smoothAnimation;
            }
            set
            {
                smoothAnimation = value;
            }
        }

        bool showCaptions = true;

        public bool ShowCaptions
        {
            get { return showCaptions; }
            set { showCaptions = value; }
        }
        

        public void LoadVOTable(string url, bool useCurrentView)
        {
            //if (VoTableView == null)
            //{
            //    VoTableView = new VoTableViewer();
            //    VoTableView.Width = double.NaN;
            //    bottomStuff.Children.Add(VoTableView);
            //}

            //if (!useCurrentView)
            //{
            //    VoTableView.Load(url);
            //}
            //else
            //{
            //    VoTableView.Load(url + string.Format("&ra={0}&dec={1}&sr={2}", Viewer.MasterView.RA * 15, Viewer.MasterView.Dec, Viewer.MasterView.FovAngle * 1.3));
            //}
        }
        public double Fov
        {
            get
            {
                if (WWTControl.Singleton != null)
                {
                    return WWTControl.Singleton.RenderContext.ViewCamera.Zoom / 6;
                }
                return 60;
            }
        }

        public Settings Settings;
       
    }


    public class SlideChangedEventArgs : EventArgs
    {
        private string caption;

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        public SlideChangedEventArgs(string caption)
        {
            Caption = caption;
        }
    }


    public class ArrivedEventArgs : EventArgs
    {
        private double ra;

        public double RA
        {
            get { return ra; }
            set { ra = value; }
        }
        private double dec;

        public double Dec
        {
            get { return dec; }
            set { dec = value; }
        }
        private double zoom;

        public double Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }
        public ArrivedEventArgs(double ra, double dec, double zoom)
        {
            RA = ra * 15;
            Dec = dec;
            Zoom = zoom / 6;
        }


    }

    public class AnnotationClickEventArgs : EventArgs
    {
        private double ra;

        public double RA
        {
            get { return ra; }
            set { ra = value; }
        }
        private double dec;

        public double Dec
        {
            get { return dec; }
            set { dec = value; }
        }
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public AnnotationClickEventArgs(double ra, double dec, string id)
        {
            RA = ra * 15;
            Dec = dec;
            Id = id;
        }


    }


    public class CollectionLoadedEventArgs : EventArgs
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public CollectionLoadedEventArgs(string url)
        {
            this.url = url;
        }
    }
}
