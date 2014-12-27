using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class TourPlayer : IUiController
    {
        public TourPlayer()
        {
        }
        BlendState overlayBlend = BlendState.Create(false, 1000);
        public void Render(RenderContext renderContext)
        {
            //window.SetupMatricesOverlays();
            if (tour == null || tour.CurrentTourStop == null || !playing)
            {
                return;
            }

            renderContext.Save();

            UpdateSlideStates();

            if (!onTarget)
            {
                slideStartTime = Date.Now;
                if (renderContext.OnTarget(Tour.CurrentTourStop.Target))
                {
                    
                    onTarget = true;
                    overlayBlend.State = !Tour.CurrentTourStop.FadeInOverlays;
                    overlayBlend.TargetState = true;
                    if (tour.CurrentTourStop.MusicTrack != null)
                    {
                        tour.CurrentTourStop.MusicTrack.Play();
                    }

                    if (tour.CurrentTourStop.VoiceTrack != null)
                    {
                        tour.CurrentTourStop.VoiceTrack.Play();
                    }
                    string caption = "";
                    foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                    {
                        
                        if (overlay.Name.ToLowerCase() == "caption")
                        {
                            TextOverlay text = overlay as TextOverlay;
                            if (text != null)
                            {
                                caption = text.TextObject.Text;
                            }
                        }
                        overlay.Play();
                    }

                    if (tour.CurrentTourStop.EndTarget != null && tour.CurrentTourStop.EndTarget.ZoomLevel != -1)
                    {
                        if (tour.CurrentTourStop.Target.Type == ImageSetType.SolarSystem)
                        {
                            // TODO fix this when Planets are implenented
                            //tour.CurrentTourStop.Target.UpdatePlanetLocation(SpaceTimeController.UtcToJulian(tour.CurrentTourStop.StartTime));
                            //tour.CurrentTourStop.EndTarget.UpdatePlanetLocation(SpaceTimeController.UtcToJulian(tour.CurrentTourStop.EndTime));
                        }
                        renderContext.ViewMover = new ViewMoverKenBurnsStyle(tour.CurrentTourStop.Target.CamParams, tour.CurrentTourStop.EndTarget.CamParams, tour.CurrentTourStop.Duration / 1000.0, tour.CurrentTourStop.StartTime, tour.CurrentTourStop.EndTime, tour.CurrentTourStop.InterpolationType);

                    }
                    Settings.TourSettings = tour.CurrentTourStop;
                    SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                    SpaceTimeController.SyncToClock = false;

                    WWTControl.scriptInterface.FireSlideChanged(caption);
                }
            }

            //todo implement gl based tour rendering
            if (renderContext.gl != null)
            {
                return;
            }

            renderContext.Device.Scale(renderContext.Height / 1116, renderContext.Height / 1116);

            double aspectOrig = 1920 / 1116;

            double aspectNow = renderContext.Width / renderContext.Height;

            renderContext.Device.Translate(-((1920 - (aspectNow * 1116)) / 2), 0);

            //todo Factor opacity in somehow ??
            //view.overlays.Opacity = overlayBlend.Opacity;


            if (currentMasterSlide != null)
            {
                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.TweenFactor = 1f;           
                    overlay.Draw3D(renderContext, false);
                }
            }

            if (onTarget)
            {
                foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                {
                    if (overlay.Name.ToLowerCase() != "caption" || WWTControl.scriptInterface.ShowCaptions)
                    {
                        overlay.TweenFactor = (float)CameraParameters.EaseCurve(tour.CurrentTourStop.TweenPosition, overlay.InterpolationType == InterpolationType.DefaultV ? tour.CurrentTourStop.InterpolationType : overlay.InterpolationType);
                        overlay.Draw3D(renderContext, false);
                    }
                }
            }
            else
            {
                int i = 0;
            }

            renderContext.Restore();
            
            DrawPlayerControls(renderContext);
        }

        BlendState PlayerState = BlendState.Create(false, 2000);

        bool middleHover = false;
        bool leftHover = false;
        bool rightHover = false;

        bool middleDown = false;
        bool leftDown = false;
        bool rightDown = false;  
        
        double top = 1;
        double center =1;
        
        void DrawPlayerControls(RenderContext renderContext)
        {
            LoadImages();

            if (!imagesLoaded)
            {
                
                return;
            }

            if (PlayerState.State)
            {
                int span = Date.Now - lastHit;

                if (span > 7000)
                {
                    PlayerState.TargetState = false;
                }


                CanvasContext2D ctx = renderContext.Device;

                ctx.Save();
                ctx.Alpha = PlayerState.Opacity;
                top = renderContext.Height - 60;
                center = renderContext.Width / 2;

                ImageElement left = leftDown ? buttonPreviousPressed : (leftHover ? buttonPreviousHover : buttonPreviousNormal);
                ImageElement middle = Playing ? (middleDown ? buttonPausePressed : (middleHover ? buttonPauseHover : buttonPauseNormal)) :
                                                 (middleDown ? buttonPlayPressed : (middleHover ? buttonPlayHover : buttonPlayNormal));
                ImageElement right = rightDown ? buttonNextPressed : (rightHover ? buttonNextHover : buttonNextNormal);

                ctx.DrawImage(left, center - 110, top);
                ctx.DrawImage(right, center, top);
                ctx.DrawImage(middle, center - 32, top - 4);

                ctx.Restore();
            }
        }
        Date lastHit = Date.Now;

        bool HitTextPlayerControls(Vector2d point, bool click, bool act)
        {


            if (click)
            {
                leftDown = false;
                rightDown = false;
                middleDown = false;
            }
            else
            {
                leftHover = false;
                rightHover = false;
                middleHover = false;

            }

            if (point.Y < (top - 2))
            {
                return false;
            }

            if (point.X < (center - 32) && point.X > (center - 105))
            {
                if (click)
                {

                    leftDown = true;
                }
                else
                {
                    leftHover = true;
                }
                if (act)
                {
                    PlayPreviousSlide();
                    lastHit = Date.Now;
                }
                return true;
            }

            if (point.X < (center + 105) && point.X > (center + 32))
            {
                if (click)
                {

                    rightDown = true;
                }
                else
                {
                    rightHover = true;
                }
                if (act)
                {
                    PlayNextSlide();
                    lastHit = Date.Now;
                }
                return true;
            }

            if (point.X < (center + 32) && point.X > (center - 32))
            {
                if (click)
                {

                    middleDown = true;
                }
                else
                {
                    middleHover = true;
                } 
                if (act)
                {
                    PauseTour();
                    lastHit = Date.Now;
                }
                return true;
            }

            return false;
        }


        ImageElement buttonNextDisabled;
        ImageElement buttonNextHover;
        ImageElement buttonNextNormal;
        ImageElement buttonNextPressed;
        ImageElement buttonPauseDisabled;
        ImageElement buttonPauseHover;
        ImageElement buttonPauseNormal;
        ImageElement buttonPausePressed;
        ImageElement buttonPlayDisabled;
        ImageElement buttonPlayHover;
        ImageElement buttonPlayNormal;
        ImageElement buttonPlayPressed;
        ImageElement buttonPreviousDisabled;
        ImageElement buttonPreviousHover;
        ImageElement buttonPreviousNormal;
        ImageElement buttonPreviousPressed;

        void LoadImages()
        {
            if (!imagesLoaded && !downloading)
            {
                buttonNextDisabled = LoadImageElement("images/button_next_disabled.png");
                buttonNextHover = LoadImageElement("images/button_next_hover.png");
                buttonNextNormal = LoadImageElement("images/button_next_normal.png");
                buttonNextPressed = LoadImageElement("images/button_next_pressed.png");
                buttonPauseDisabled = LoadImageElement("images/button_pause_disabled.png");
                buttonPauseHover = LoadImageElement("images/button_pause_hover.png");
                buttonPauseNormal = LoadImageElement("images/button_pause_normal.png");
                buttonPausePressed = LoadImageElement("images/button_pause_pressed.png");
                buttonPlayDisabled = LoadImageElement("images/button_play_disabled.png");
                buttonPlayHover = LoadImageElement("images/button_play_hover.png");
                buttonPlayNormal = LoadImageElement("images/button_play_normal.png");
                buttonPlayPressed = LoadImageElement("images/button_play_pressed.png");
                buttonPreviousDisabled = LoadImageElement("images/button_previous_disabled.png");
                buttonPreviousHover = LoadImageElement("images/button_previous_hover.png");
                buttonPreviousNormal = LoadImageElement("images/button_previous_normal.png");
                buttonPreviousPressed = LoadImageElement("images/button_previous_pressed.png");
            }
        }


        int imageCount = 0;
        int imageLoadCount = 0;
        bool imagesLoaded = false;
        bool downloading = false;
        ImageElement LoadImageElement(string url)
        {
            imageCount++;
            imagesLoaded = false;
            downloading = true;
            ImageElement temp = (ImageElement)Document.CreateElement("img");
            temp.Src = url;
            temp.AddEventListener("load", delegate(ElementEvent e)
            {
                imageLoadCount++;
                if (imageLoadCount == imageCount)
                {
                    downloading = false;
                    imagesLoaded = true;
                    // Refresh();
                }
            }, false);

            return temp;
        }

        TourDocument tour = null;

        public TourDocument Tour
        {
            get { return tour; }
            set { tour = value; }
        }

        static bool playing = false;

        static public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }
        bool onTarget = false;
        Date slideStartTime;
        TourStop currentMasterSlide = null;
        public void NextSlide()
        {
            if (tour.CurrentTourStop != null)
            {
                if (!tour.CurrentTourStop.MasterSlide)
                {
                    if (tour.CurrentTourStop.MusicTrack != null)
                    {
                        tour.CurrentTourStop.MusicTrack.Stop();
                    }

                    if (tour.CurrentTourStop.VoiceTrack != null)
                    {
                        tour.CurrentTourStop.VoiceTrack.Stop();
                    }

                    foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                    {
                        overlay.Stop();
                    }
                }
                else
                {
                    currentMasterSlide = tour.CurrentTourStop;
                }
            }

            if (tour.CurrentTourstopIndex < (tour.TourStops.Count - 1)) 
            {
                if (tour.CurrentTourStop.EndTarget != null)
                {
                    WWTControl.Singleton.GotoTargetFull(false, true, tour.CurrentTourStop.EndTarget.CamParams, tour.CurrentTourStop.Target.StudyImageset, tour.CurrentTourStop.Target.BackgroundImageset);
                    WWTControl.Singleton.Mover = null;
                }
                onTarget = false;
                if (tour.CurrentTourStop.IsLinked)
                {
                    switch (tour.CurrentTourStop.NextSlide)
                    {
                        case "Return":
                            if (callStack.Count > 0)
                            {
                                PlayFromTourstop(tour.TourStops[callStack.Pop()]);
                            }
                            else
                            {
                                tour.CurrentTourstopIndex = tour.TourStops.Count - 1;
                            }
                            break;
                        default:
                            PlayFromTourstop(tour.TourStops[tour.GetTourStopIndexByID(tour.CurrentTourStop.NextSlide)]);

                            //tour.CurrentTourstopIndex = tour.GetTourStopIndexByID(tour.CurrentTourStop.NextSlide);
                            //PlayMasterForCurrent();
                            break;
                    }

                }
                else
                {
                    tour.CurrentTourstopIndex++;
                }

                if (currentMasterSlide != null && tour.CurrentTourStop.MasterSlide)
                {
                    if (currentMasterSlide.MusicTrack != null)
                    {
                        currentMasterSlide.MusicTrack.Stop();
                    }

                    if (currentMasterSlide.VoiceTrack != null)
                    {
                        currentMasterSlide.VoiceTrack.Stop();
                    }

                    foreach (Overlay overlay in currentMasterSlide.Overlays)
                    {
                        overlay.Stop();
                    }
                    currentMasterSlide = null;
                }
                WWTControl.Singleton.GotoTarget(tour.CurrentTourStop.Target, false, false, false);

                slideStartTime = Date.Now;
                // Move to new settings
                Settings.TourSettings = tour.CurrentTourStop;
                SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                SpaceTimeController.SyncToClock = false;


            }
            else
            {
                StopMaster();
                playing = false;
                if (Settings.Current.AutoRepeatTour)
                {
                    tour.CurrentTourstopIndex = -1;
                    Play();
                }
                else
                {
                    WWTControl.Singleton.FreezeView();
                    if (TourEnded != null)
                    {
                        TourEnded.Invoke(this, new EventArgs());
                    }

                    ShowEndTourPopup();
                    WWTControl.Singleton.HideUI(false);
                    WWTControl.scriptInterface.FireTourEnded();
                }
            }

        }

        private void StopMaster()
        {
            if (currentMasterSlide != null)
            {
                if (currentMasterSlide.MusicTrack != null)
                {
                    currentMasterSlide.MusicTrack.Stop();
                }

                if (currentMasterSlide.VoiceTrack != null)
                {
                    currentMasterSlide.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.Stop();
                }
                currentMasterSlide = null;
            }
        }

        public void ShowEndTourPopup()
        {
            //FolderBrowser.Explorer.TourPopupContainer.IsOpen = true;
            //FolderBrowser.Explorer.TourPopupContents.ShowEndTourPopup(tour);

            //FolderBrowser.Explorer.TourPopupContainer.HorizontalOffset = Viewer.MasterView.ActualWidth / 2 - 300;
            //FolderBrowser.Explorer.TourPopupContainer.VerticalOffset = Viewer.MasterView.ActualHeight/2-200;
        }

        static public event EventHandler TourEnded;
        static bool switchedToFullScreen = false;
        Stack<int> callStack = new Stack<int>();

        public void Play()
        {
            if (tour == null)
            {
                return;
            }

            if (playing)
            {
                Stop(true);
            }
            else
            {
                playing = true;
                //switchedToFullScreen = !Viewer.MasterView.FullScreen;
                //if (switchedToFullScreen)
                //{
                //    Viewer.MasterView.ShowFullScreen(true);

                //}
            }
            WWTControl.Singleton.HideUI(true);

            playing = true;


            if (tour.TourStops.Count > 0)
            {
                onTarget = false;
                if (tour.CurrentTourstopIndex == -1)
                {
                    tour.CurrentTourStop = tour.TourStops[0];
                }

                if (tour.CurrentTourstopIndex > 0)
                {
                    PlayMasterForCurrent();

                }


                WWTControl.Singleton.GotoTarget(tour.CurrentTourStop.Target, false, true, false);

            }

            slideStartTime = Date.Now;
            playing = true;

        }

        private void PlayMasterForCurrent()
        {
            if (!tour.CurrentTourStop.MasterSlide)
            {
                MasterTime currentMaster =  tour.ElapsedTimeSinceLastMaster(tour.CurrentTourstopIndex);

                if (currentMaster != null && currentMasterSlide != null)
                {
                    double elapsed = currentMaster.Durration;
                    currentMasterSlide = currentMaster.Master;

                    if (currentMasterSlide.MusicTrack != null)
                    {
                        currentMasterSlide.MusicTrack.Play();
                        currentMasterSlide.MusicTrack.Seek(elapsed);
                    }

                    if (currentMasterSlide.VoiceTrack != null)
                    {
                        currentMasterSlide.VoiceTrack.Play();
                        currentMasterSlide.VoiceTrack.Seek(elapsed);
                    }

                    foreach (Overlay overlay in currentMasterSlide.Overlays)
                    {
                        overlay.Play();
                        overlay.Seek(elapsed);
                    }
                }
            }
        }

        public static bool NoRestoreUIOnStop;

        public void Stop(bool noSwitchBackFullScreen)
        {


            if (switchedToFullScreen && !noSwitchBackFullScreen)
            {
               // Viewer.MasterView.ShowFullScreen(false);
            }

            Settings.TourSettings = null;
            playing = false;
            if (tour.CurrentTourStop != null)
            {
                if (tour.CurrentTourStop.MusicTrack != null)
                {
                    tour.CurrentTourStop.MusicTrack.Stop();
                }

                if (tour.CurrentTourStop.VoiceTrack != null)
                {
                    tour.CurrentTourStop.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                {
                    overlay.Stop();
                }
            }
            if (currentMasterSlide != null)
            {
                if (currentMasterSlide.MusicTrack != null)
                {
                    currentMasterSlide.MusicTrack.Stop();
                }

                if (currentMasterSlide.VoiceTrack != null)
                {
                    currentMasterSlide.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.Stop();
                }
            }

            WWTControl.Singleton.HideUI(NoRestoreUIOnStop);
			WWTControl.scriptInterface.FireTourEnded();
        }

        public void UpdateSlideStates()
        {
            int slideElapsedTime = Date.Now - slideStartTime;

            if (slideElapsedTime > tour.CurrentTourStop.Duration && playing)
            {
                NextSlide();
            }
            slideElapsedTime = Date.Now - slideStartTime;

            tour.CurrentTourStop.TweenPosition = (float)(slideElapsedTime / tour.CurrentTourStop.Duration);

        }

        public float UpdateTweenPosition(float tween)
        {
            float slideElapsedTime = Date.Now - slideStartTime;

            if (tween > -1)
            {
                return tour.CurrentTourStop.TweenPosition = Math.Min(1, tween);
            }
            else
            {
                return tour.CurrentTourStop.TweenPosition = Math.Min(1, (float)(slideElapsedTime / tour.CurrentTourStop.Duration));
            }
        }

        public void Close()
        {
            if (tour != null)
            {
                if (Playing)
                {
                    Stop(switchedToFullScreen);
                }
                // todo check for changes
                tour = null;
            }
        }

        public bool MouseDown(object sender, ElementEvent e)
        {
            // todo enable links
            Vector2d location;


            location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));

            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
            {
                if (tour.CurrentTourStop.Overlays[i].HitTest(location))
                {
                    if (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].Url))
                    {
                        Overlay linkItem = tour.CurrentTourStop.Overlays[i];
                        Util.OpenUrl(linkItem.Url, true);
                        return true;
                    }
                    if (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].LinkID))
                    {
                        callStack.Push(tour.CurrentTourstopIndex);
                        PlayFromTourstop(tour.TourStops[tour.GetTourStopIndexByID(tour.CurrentTourStop.Overlays[i].LinkID)]);
                        return true;
                    }

                }
            }
            if (PlayerState.State)
            {
                return HitTextPlayerControls(Vector2d.Create(e.OffsetX, e.OffsetY), true, true);
            }
            else
            {
                PlayerState.TargetState = true;
                lastHit = Date.Now;
            }
           

            return false;
        }

        public bool MouseUp(object sender, ElementEvent e)
        {
            if (leftDown || rightDown || middleDown)
            {
                leftDown = false;
                rightDown = false;
                middleDown = false;
                return true;
            }

          
            return false;
        }

        public bool MouseMove(object sender, ElementEvent e)
        {
            // todo enable links
            Vector2d location;

            try
            {
                location = PointToView(Vector2d.Create(e.OffsetX, e.OffsetY));
            }
            catch
            {
                return false;
            }

            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
            {
                if (tour.CurrentTourStop.Overlays[i].HitTest(location) && (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].Url) || !string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].LinkID)))
                {
                    //todo change cursor to hand 
                    return true;
                }
            }
            //todo set cursor to default
            //Viewer.MasterView.Cursor = null;

            if (PlayerState.State)
            {
                return HitTextPlayerControls(Vector2d.Create(e.OffsetX, e.OffsetY), false, false);
            }


            return false;
        }

        public bool MouseClick(object sender, ElementEvent e)
        {
            //if (PlayerState.State)
            //{
            //    return HitTextPlayerControls(Vector2d.Create(e.OffsetX, e.OffsetY), true, true);
            //}
            //else
            //{
               
            //    PlayerState.TargetState = true;
            //}

            return false;
        }

        public bool Click(object sender, ElementEvent e)
        {
            return false;
        }

        public bool MouseDoubleClick(object sender, ElementEvent e)
        {

            return false;
        }

        public bool KeyDown(object sender, ElementEvent e)
        {

            switch (e.KeyCode)
            {
                case 27:
                    Stop(switchedToFullScreen);
                    WWTControl.Singleton.CloseTour();
                    return true;

                case 32:

                    PauseTour();
                    return true;
                case 39:
                    PlayNextSlide();
                    return true;
                case 37:
                    PlayPreviousSlide();

                    return true;
                case 35:
                    if (tour.TourStops.Count > 0)
                    {
                        PlayFromTourstop(tour.TourStops[tour.TourStops.Count - 1]);
                    }

                    return true;
                case 36:
                    if (tour.TourStops.Count > 0)
                    {
                        PlayFromTourstop(tour.TourStops[0]);

                    }
                    return true;
            }

            return false;
        }

        private void PlayNextSlide()
       {
            if ((tour.CurrentTourstopIndex < tour.TourStops.Count - 1) && tour.TourStops.Count > 0)
            {
                PlayFromTourstop(tour.TourStops[tour.CurrentTourstopIndex + 1]);
            }
        }

        private void PlayPreviousSlide()
        {
            if (tour.CurrentTourstopIndex > 0)
            {
                PlayFromTourstop(tour.TourStops[tour.CurrentTourstopIndex - 1]);
            }
        }

        private void PlayFromTourstop(TourStop tourStop)
        {
            Stop(true);
            tour.CurrentTourStop = tourStop;
            WWTControl.Singleton.GotoTarget(tour.CurrentTourStop.Target, false, true, false);
            SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
            SpaceTimeController.SyncToClock = false;
            Play();
        }

        public void PauseTour()
        {
            if (playing)
            {
                Stop(switchedToFullScreen);
                WWTControl.Singleton.FreezeView();
				WWTControl.scriptInterface.FireTourPaused();
            }
            else
            {
                Play();
				WWTControl.scriptInterface.FireTourResume();
            }
        }

        public bool KeyUp(object sender, ElementEvent e)
        {
            return false;
        }


        public bool Hover(Vector2d pnt)
        {
            if (playing)
            {
                return true;
            }
            return false;
        }

        public Vector2d PointToView(Vector2d pnt)
        {
            double clientHeight = WWTControl.Singleton.Canvas.Height;
            double clientWidth = WWTControl.Singleton.Canvas.Width;
            double viewWidth = (clientWidth / clientHeight) * 1116f;
            double x = (((double)pnt.X) / ((double)clientWidth) * viewWidth) - ((viewWidth - 1920) / 2);
            double y = ((double)pnt.Y) / clientHeight * 1116;

            return Vector2d.Create(x, y);
        }
    }

    public class MasterTime
    {
        public TourStop Master;
        public double Durration;

        public MasterTime( TourStop master, double durration)
        {
            Master = master;
            Durration = durration;
        }
    }
}
