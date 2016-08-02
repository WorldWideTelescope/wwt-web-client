using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;

namespace wwtlib
{
    enum TransitionType { Slew=0, Instant=1, CrossFade=2, FadeToBlack=3 };
    public class TourStop : ISettings
    {
        public const string ClipboardFormat = "WorldWideTelescope.Slide";

        ImageSetType tourStopType;

         private bool keyFramed = false;

         public bool KeyFramed
         {
             get { return keyFramed; }
         }

        public ImageSetType TourStopType
        {
            get
            {
                if (target.BackgroundImageset != null)
                {
                    return target.BackgroundImageset.DataSetType;
                }
                else
                {
                    return tourStopType;
                }
            }
            set
            {
                if (target.BackgroundImageset != null)
                {
                    if (target.BackgroundImageset.DataSetType != value)
                    {
                        target.BackgroundImageset = null;
                    }
                }
                tourStopType = value;
            }
        }

        private float tweenPosition = 0;

        public float TweenPosition
        {
            get { return tweenPosition; }
            set
            {
                if (tweenPosition != value)
                {
                    tweenPosition = Math.Max(0, Math.Min(1, value));
                    UpdateTweenPosition();
                }

            }
        }


        public void UpdateTweenPosition()
        {
            if (KeyFramed)
            {
                //KeyFrameMover.CurrentDateTime = this.StartTime;
                //KeyFrameMover.ReferenceFrame = this.Target.CamParams.TargetReferenceFrame;
                //KeyFrameMover.MoveTime = (double)(Duration.TotalMilliseconds / 1000.0);
                ////update key framed elements
                //foreach (AnimationTarget target in AnimationTargets)
                //{
                //    target.Tween(tweenPosition);
                //}
                //Earth3d.MainWindow.UpdateMover(KeyFrameMover);
                //SpaceTimeController.Now = KeyFrameMover.CurrentDateTime;
            }
        }


        public TourStop()
        {
            id = Guid.NewGuid().ToString();

        }

        public static TourStop Create(Place target)
        {
            TourStop ts = new TourStop();
            
            ts.target = target;

            return ts;
        }

        TourDocument owner = null;

        public TourDocument Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        TransitionType transition = TransitionType.Slew;

        internal TransitionType Transition
        {
            get { return transition; }
            set
            {
                if (transition != value)
                {
                    transition = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        private double transitionTime = 2;

        internal double TransitionTime
        {
            get { return transitionTime; }
            set
            {
                if (transitionTime != value)
                {
                    transitionTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        private double transitionHoldTime = 4;

        internal double TransitionHoldTime
        {
            get { return transitionHoldTime; }
            set
            {
                if (transitionHoldTime != value)
                {
                    transitionHoldTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }


        private double transitionOutTime = 2;

        internal double TransitionOutTime
        {
            get { return transitionOutTime; }
            set
            {
                if (transitionOutTime != value)
                {
                    transitionOutTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        string nextSlide = "Next";

        public string NextSlide
        {
            get { return nextSlide; }
            set { nextSlide = value; }
        }

        public bool IsLinked
        {
            get
            {
                if (nextSlide == null || nextSlide == "Next" || nextSlide == "")
                {
                    return false;
                }
                return true;
            }
        }

        bool fadeInOverlays = false;

        public bool FadeInOverlays
        {
            get { return fadeInOverlays; }
            set { fadeInOverlays = value; }
        }

        bool masterSlide = false;

        public bool MasterSlide
        {
            get { return masterSlide; }
            set
            {
                if (masterSlide != value)
                {
                    masterSlide = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                if (owner != null) { owner.TourDirty = true; }
            }
        }

        public override string ToString()
        {
            if (target != null)
            {
                return Target.Name;
            }
            else
            {
                return description;
            }
        }

        string description;

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        private string name;

        public string Name
        {
            get
            {
                if (target != null)
                {
                    return target.Name;
                }
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        int duration = 10000;

        public int Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        Place target;

        public Place Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        Place endTarget;

        public Place EndTarget
        {
            get { return endTarget; }
            set
            {
                if (endTarget != value)
                {
                    endTarget = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        InterpolationType interpolationType = InterpolationType.Linear;

        public InterpolationType InterpolationType
        {
            get { return interpolationType; }
            set { interpolationType = value; }
        }


        // Settings

        bool hasLocation = true;

        public bool HasLocation
        {
            get { return hasTime; }
            set
            {
                if (hasLocation != value)
                {
                    hasLocation = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        bool hasTime = true;

        public bool HasTime
        {
            get { return hasTime; }
            set
            {
                if (hasTime != value)
                {
                    hasTime = hasLocation = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        Date startTime = SpaceTimeController.Now;

        public Date StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                if (startTime != value)
                {
                    startTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        Date endTime = SpaceTimeController.Now;

        public Date EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }


        bool actualPlanetScale = Settings.Current.ActualPlanetScale;
        double locationAltitude = Settings.Current.LocationAltitude;
        double locationLat = Settings.Current.LocationLat;
        double locationLng = Settings.Current.LocationLng;
        bool showClouds = Settings.Current.ShowClouds;
        bool showConstellationBoundries = Settings.Current.ShowConstellationBoundries;
        bool showConstellationFigures = Settings.Current.ShowConstellationFigures;
        bool showConstellationSelection = Settings.Current.ShowConstellationSelection;
        bool showEcliptic = Settings.Current.ShowEcliptic;
        bool showElevationModel = Settings.Current.ShowElevationModel;
        bool showFieldOfView = Settings.Current.ShowFieldOfView;
        bool showGrid = Settings.Current.ShowGrid;
        bool showHorizon = Settings.Current.ShowHorizon;
        bool showHorizonPanorama = Settings.Current.ShowHorizonPanorama;
        bool showMoonsAsPointSource = Settings.Current.ShowMoonsAsPointSource;
        bool showSolarSystem = Settings.Current.ShowSolarSystem;
        int fovTelescope = Settings.Current.FovTelescope;
        int fovEyepiece = Settings.Current.FovEyepiece;
        int fovCamera = Settings.Current.FovCamera;
        bool localHorizonMode = Settings.Current.LocalHorizonMode;
        bool galacticMode = Settings.Current.GalacticMode;

        bool solarSystemStars = Settings.Current.SolarSystemStars;
        bool solarSystemMilkyWay = Settings.Current.SolarSystemMilkyWay;
        bool solarSystemCosmos = Settings.Current.SolarSystemCosmos;
        bool solarSystemOrbits = Settings.Current.SolarSystemOrbits;
        bool solarSystemOverlays = Settings.Current.SolarSystemOverlays;
        bool solarSystemLighting = Settings.Current.SolarSystemLighting;
        int solarSystemScale = Settings.Current.SolarSystemScale;
        bool solarSystemMultiRes = Settings.Current.SolarSystemMultiRes;

        //new 
        bool showEquatorialGridText = Settings.Current.ShowEquatorialGridText;
        bool showGalacticGrid = Settings.Current.ShowGalacticGrid;
        bool showGalacticGridText = Settings.Current.ShowGalacticGridText;
        bool showEclipticGrid = Settings.Current.ShowEclipticGrid;
        bool showEclipticGridText = Settings.Current.ShowEclipticGridText;
        private bool showEclipticOverviewText = Settings.Current.ShowEclipticOverviewText;
        private bool showAltAzGrid = Settings.Current.ShowAltAzGrid;
        private bool showAltAzGridText = Settings.Current.ShowAltAzGridText;
        private bool showPrecessionChart = Settings.Current.ShowPrecessionChart;
        private bool showConstellationPictures = Settings.Current.ShowConstellationPictures;
        private bool showConstellationLabels = Settings.Current.ShowConstellationLabels;
        bool solarSystemCMB = Settings.Current.SolarSystemCMB;
        bool solarSystemMinorPlanets = Settings.Current.SolarSystemMinorPlanets;
        bool solarSystemPlanets = Settings.Current.SolarSystemPlanets;
        bool showEarthSky = Settings.Current.ShowEarthSky;
        bool solarSystemMinorOrbits = Settings.Current.SolarSystemMinorOrbits;
        string constellationsEnabled = "";
        ConstellationFilter constellationFiguresFilter = Settings.Current.ConstellationFiguresFilter.Clone();
        ConstellationFilter constellationBoundariesFilter = Settings.Current.ConstellationBoundariesFilter.Clone();
        ConstellationFilter constellationNamesFilter = Settings.Current.ConstellationNamesFilter.Clone();
        ConstellationFilter constellationArtFilter = Settings.Current.ConstellationArtFilter.Clone();
        bool showSkyOverlays = Settings.Current.ShowSkyOverlays;
        bool showConstellations = Settings.Current.ShowConstellations;
        bool showSkyNode = Settings.Current.ShowSkyNode;
        bool showSkyGrids = Settings.Current.ShowSkyGrids;
        bool showSkyOverlaysIn3d = Settings.Current.ShowSkyOverlaysIn3d;
        bool earthCutawayView = Settings.Current.EarthCutawayView;
        bool showISSModel = Settings.Current.ShowISSModel;
        bool milkyWayModel = Settings.Current.MilkyWayModel;
        int minorPlanetsFilter = Settings.Current.MinorPlanetsFilter;
        int planetOrbitsFilter = Settings.Current.PlanetOrbitsFilter;



        public void CaptureSettings()
        {
            startTime = SpaceTimeController.Now;
            actualPlanetScale = Settings.Current.ActualPlanetScale;
            locationAltitude = Settings.Current.LocationAltitude;
            locationLat = Settings.Current.LocationLat;
            locationLng = Settings.Current.LocationLng;
            showClouds = Settings.Current.ShowClouds;
            showConstellationBoundries = Settings.Current.ShowConstellationBoundries;
            showConstellationFigures = Settings.Current.ShowConstellationFigures;
            showConstellationSelection = Settings.Current.ShowConstellationSelection;
            showEcliptic = Settings.Current.ShowEcliptic;
            showElevationModel = Settings.Current.ShowElevationModel;
            showFieldOfView = Settings.Current.ShowFieldOfView;
            showGrid = Settings.Current.ShowGrid;
            showHorizon = Settings.Current.ShowHorizon;
            showHorizonPanorama = Settings.Current.ShowHorizonPanorama;
            showMoonsAsPointSource = Settings.Current.ShowMoonsAsPointSource;
            showSolarSystem = Settings.Current.ShowSolarSystem;
            fovTelescope = Settings.Current.FovTelescope;
            fovEyepiece = Settings.Current.FovEyepiece;
            fovCamera = Settings.Current.FovCamera;
            localHorizonMode = Settings.Current.LocalHorizonMode;
            galacticMode = Settings.Current.GalacticMode;
            solarSystemStars = Settings.Current.SolarSystemStars;
            solarSystemMilkyWay = Settings.Current.SolarSystemMilkyWay;
            solarSystemCosmos = Settings.Current.SolarSystemCosmos;
            solarSystemOrbits = Settings.Current.SolarSystemOrbits;
            solarSystemOverlays = Settings.Current.SolarSystemOverlays;
            solarSystemLighting = Settings.Current.SolarSystemLighting;
            solarSystemScale = Settings.Current.SolarSystemScale;
            solarSystemMultiRes = Settings.Current.SolarSystemMultiRes;
            //new

            showEquatorialGridText = Settings.Current.ShowEquatorialGridText;
            showGalacticGrid = Settings.Current.ShowGalacticGrid;
            showGalacticGridText = Settings.Current.ShowGalacticGridText;
            showEclipticGrid = Settings.Current.ShowEclipticGrid;
            showEclipticGridText = Settings.Current.ShowEclipticGridText;
            showEclipticOverviewText = Settings.Current.ShowEclipticOverviewText;
            showAltAzGrid = Settings.Current.ShowAltAzGrid;
            showAltAzGridText = Settings.Current.ShowAltAzGridText;
            showPrecessionChart = Settings.Current.ShowPrecessionChart;
            showConstellationPictures = Settings.Current.ShowConstellationPictures;
            showConstellationLabels = Settings.Current.ShowConstellationLabels;
            solarSystemCMB = Settings.Current.SolarSystemCMB;
            solarSystemMinorPlanets = Settings.Current.SolarSystemMinorPlanets;
            solarSystemPlanets = Settings.Current.SolarSystemPlanets;
            showEarthSky = Settings.Current.ShowEarthSky;
            solarSystemMinorOrbits = Settings.Current.SolarSystemMinorOrbits;
            constellationFiguresFilter = Settings.Current.ConstellationFiguresFilter.Clone();
            constellationBoundariesFilter = Settings.Current.ConstellationBoundariesFilter.Clone();
            constellationNamesFilter = Settings.Current.ConstellationNamesFilter.Clone();
            constellationArtFilter = Settings.Current.ConstellationArtFilter.Clone();
            showSkyOverlays = Settings.Current.ShowSkyOverlays;
            showConstellations = Settings.Current.ShowConstellations;
            showSkyNode = Settings.Current.ShowSkyNode;
            showSkyGrids = Settings.Current.ShowSkyGrids;
            showSkyOverlaysIn3d = Settings.Current.ShowSkyOverlaysIn3d;
            earthCutawayView = Settings.Current.EarthCutawayView;
            showISSModel = Settings.Current.ShowISSModel;
            milkyWayModel = Settings.Current.MilkyWayModel;
            minorPlanetsFilter = Settings.Current.MinorPlanetsFilter;
            planetOrbitsFilter = Settings.Current.PlanetOrbitsFilter;
        }

        public static string GetXmlText(TourStop ts)
        {

            XmlTextWriter writer = new XmlTextWriter();
            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

            ts.SaveToXml(writer, true);
            writer.Close();
            return writer.Body;
        }

        internal void SaveToXml(XmlTextWriter xmlWriter, bool saveContent)
        {
            if (saveContent)
            {
                if (thumbnail != null)
                {
                    //todo how do we save this?
                    //thumbnail.Save(TourStopThumbnailFilename, System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            xmlWriter.WriteStartElement("TourStop");
            xmlWriter.WriteAttributeString("Id", id);
            xmlWriter.WriteAttributeString("Name", name);
            xmlWriter.WriteAttributeString("Description", description);
            xmlWriter.WriteAttributeString("Thumbnail", thumbnailString);
            xmlWriter.WriteAttributeString("Duration", duration.ToString());
            xmlWriter.WriteAttributeString("Master", masterSlide.ToString());
            xmlWriter.WriteAttributeString("TransitionType", transition.ToString());
            xmlWriter.WriteAttributeString("TransitionTime", transitionTime.ToString());
            xmlWriter.WriteAttributeString("TransitionOutTime", transitionOutTime.ToString());
            xmlWriter.WriteAttributeString("TransitionHoldTime", transitionHoldTime.ToString());
            xmlWriter.WriteAttributeString("NextSlide", nextSlide.ToString());
            xmlWriter.WriteAttributeString("InterpolationType", interpolationType.ToString());

            xmlWriter.WriteAttributeString("HasLocation", hasLocation.ToString());
            if (hasLocation)
            {
                xmlWriter.WriteAttributeString("LocationAltitude", locationAltitude.ToString());
                xmlWriter.WriteAttributeString("LocationLat", locationLat.ToString());
                xmlWriter.WriteAttributeString("LocationLng", locationLng.ToString());
            }
            xmlWriter.WriteAttributeString("HasTime", hasTime.ToString());
            if (hasTime)
            {
                xmlWriter.WriteAttributeString("StartTime", startTime.ToString());
                xmlWriter.WriteAttributeString("EndTime", endTime.ToString());
            }
            xmlWriter.WriteAttributeString("ActualPlanetScale", actualPlanetScale.ToString());
            xmlWriter.WriteAttributeString("ShowClouds", showClouds.ToString());
            xmlWriter.WriteAttributeString("EarthCutawayView", earthCutawayView.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationBoundries", showConstellationBoundries.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationFigures", showConstellationFigures.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationSelection", showConstellationSelection.ToString());
            xmlWriter.WriteAttributeString("ShowEcliptic", showEcliptic.ToString());
            xmlWriter.WriteAttributeString("ShowElevationModel", showElevationModel.ToString());
            showFieldOfView = false;
            xmlWriter.WriteAttributeString("ShowFieldOfView", showFieldOfView.ToString());
            xmlWriter.WriteAttributeString("ShowGrid", showGrid.ToString());
            xmlWriter.WriteAttributeString("ShowHorizon", showHorizon.ToString());
            xmlWriter.WriteAttributeString("ShowHorizonPanorama", showHorizonPanorama.ToString());
            xmlWriter.WriteAttributeString("ShowMoonsAsPointSource", showMoonsAsPointSource.ToString());
            xmlWriter.WriteAttributeString("ShowSolarSystem", showSolarSystem.ToString());
            xmlWriter.WriteAttributeString("FovTelescope", fovTelescope.ToString());
            xmlWriter.WriteAttributeString("FovEyepiece", fovEyepiece.ToString());
            xmlWriter.WriteAttributeString("FovCamera", fovCamera.ToString());
            xmlWriter.WriteAttributeString("LocalHorizonMode", localHorizonMode.ToString());
            //xmlWriter.WriteAttributeString("MilkyWayModel", milkyWayModel.ToString());
            xmlWriter.WriteAttributeString("GalacticMode", galacticMode.ToString());
            xmlWriter.WriteAttributeString("FadeInOverlays", fadeInOverlays.ToString());
            xmlWriter.WriteAttributeString("SolarSystemStars", solarSystemStars.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMilkyWay", solarSystemMilkyWay.ToString());
            xmlWriter.WriteAttributeString("SolarSystemCosmos", solarSystemCosmos.ToString());
            xmlWriter.WriteAttributeString("SolarSystemCMB", solarSystemCMB.ToString());
            xmlWriter.WriteAttributeString("SolarSystemOrbits", solarSystemOrbits.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMinorOrbits", solarSystemMinorOrbits.ToString());
            xmlWriter.WriteAttributeString("SolarSystemOverlays", solarSystemOverlays.ToString());
            xmlWriter.WriteAttributeString("SolarSystemLighting", solarSystemLighting.ToString());
            xmlWriter.WriteAttributeString("ShowISSModel", showISSModel.ToString());
            xmlWriter.WriteAttributeString("SolarSystemScale", solarSystemScale.ToString());
            xmlWriter.WriteAttributeString("MinorPlanetsFilter", minorPlanetsFilter.ToString());
            xmlWriter.WriteAttributeString("PlanetOrbitsFilter", planetOrbitsFilter.ToString());

            xmlWriter.WriteAttributeString("SolarSystemMultiRes", solarSystemMultiRes.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMinorPlanets", solarSystemMinorPlanets.ToString());
            xmlWriter.WriteAttributeString("SolarSystemPlanets", solarSystemPlanets.ToString());
            xmlWriter.WriteAttributeString("ShowEarthSky", showEarthSky.ToString());

            xmlWriter.WriteAttributeString("ShowEquatorialGridText", ShowEquatorialGridText.ToString());
            xmlWriter.WriteAttributeString("ShowGalacticGrid", ShowGalacticGrid.ToString());
            xmlWriter.WriteAttributeString("ShowGalacticGridText", ShowGalacticGridText.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticGrid", ShowEclipticGrid.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticGridText", ShowEclipticGridText.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticOverviewText", ShowEclipticOverviewText.ToString());
            xmlWriter.WriteAttributeString("ShowAltAzGrid", ShowAltAzGrid.ToString());
            xmlWriter.WriteAttributeString("ShowAltAzGridText", ShowAltAzGridText.ToString());
            xmlWriter.WriteAttributeString("ShowPrecessionChart", ShowPrecessionChart.ToString());
            xmlWriter.WriteAttributeString("ConstellationPictures", ShowConstellationPictures.ToString());
            xmlWriter.WriteAttributeString("ConstellationsEnabled", ConstellationsEnabled);
            xmlWriter.WriteAttributeString("ShowConstellationLabels", ShowConstellationLabels.ToString());
            xmlWriter.WriteAttributeString("ShowSkyOverlays", ShowSkyOverlays.ToString());
            xmlWriter.WriteAttributeString("ShowConstellations", ShowConstellations.ToString());
            xmlWriter.WriteAttributeString("ShowSkyNode", ShowSkyNode.ToString());
            xmlWriter.WriteAttributeString("ShowSkyGrids", ShowSkyGrids.ToString());
            xmlWriter.WriteAttributeString("SkyOverlaysIn3d", ShowSkyOverlaysIn3d.ToString());
            xmlWriter.WriteAttributeString("ConstellationFiguresFilter", constellationFiguresFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationBoundariesFilter", constellationBoundariesFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationNamesFilter", constellationNamesFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationArtFilter", constellationArtFilter.ToString());



            target.SaveToXml(xmlWriter, "Place");
            if (endTarget != null)
            {
                endTarget.SaveToXml(xmlWriter, "EndTarget");
            }

            xmlWriter.WriteStartElement("Overlays");

            foreach (Overlay overlay in overlays)
            {
                overlay.SaveToXml(xmlWriter, false);
            }
            xmlWriter.WriteEndElement();

            if (musicTrack != null)
            {
                xmlWriter.WriteStartElement("MusicTrack");

                musicTrack.SaveToXml(xmlWriter, false);

                xmlWriter.WriteEndElement();
            }

            if (voiceTrack != null)
            {
                xmlWriter.WriteStartElement("VoiceTrack");

                voiceTrack.SaveToXml(xmlWriter, false);

                xmlWriter.WriteEndElement();
            }

            //xmlWriter.WriteElementString("Credits", Credits);
            WriteLayerList(xmlWriter);

            //if (KeyFramed)
            //{
            //    xmlWriter.WriteStartElement("AnimationTargets");
            //    foreach (AnimationTarget aniTarget in AnimationTargets)
            //    {
            //        aniTarget.SaveToXml(xmlWriter);
            //    }
            //    xmlWriter.WriteEndElement();
            //}

            xmlWriter.WriteEndElement();
        }

        private void WriteLayerList(XmlTextWriter xmlWriter)
        {
            if (Layers.Count > 0)
            {
                xmlWriter.WriteStartElement("VisibleLayers");
                foreach(Guid key in Layers.Keys)
                {
                    LayerInfo info = Layers[key];
                
                    xmlWriter.WriteStartElement("Layer");
                    xmlWriter.WriteAttributeString("StartOpacity", info.StartOpacity.ToString());
                    xmlWriter.WriteAttributeString("EndOpacity", info.EndOpacity.ToString());
                    int len = info.StartParams.Length;

                    xmlWriter.WriteAttributeString("ParamCount", len.ToString());
                    for (int i = 0; i < len; i++)
                    {
                        xmlWriter.WriteAttributeString(string.Format("StartParam{0}", i), info.StartParams[i].ToString());
                        xmlWriter.WriteAttributeString(string.Format("EndParam{0}", i), info.EndParams[i].ToString());
                    }
                    xmlWriter.WriteValue(info.ID.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }

        public void SyncSettings()
        {
            //Earth3d.ignoreChanges = true;
            //LayerManager.ProcessingUpdate = true;

            Settings.GlobalSettings.ActualPlanetScale = actualPlanetScale;
            Settings.GlobalSettings.LocationAltitude = locationAltitude;
            Settings.GlobalSettings.LocationLat = locationLat;
            Settings.GlobalSettings.LocationLng = locationLng;
         //   Settings.GlobalSettings.ShowClouds.TargetState = showClouds;
            Settings.GlobalSettings.EarthCutawayView = earthCutawayView;
            Settings.GlobalSettings.ShowConstellationBoundries = showConstellationBoundries;
            Settings.GlobalSettings.ShowConstellationFigures = showConstellationFigures;
            Settings.GlobalSettings.ShowConstellationSelection = showConstellationSelection;
            Settings.GlobalSettings.ShowEcliptic = showEcliptic;
            Settings.GlobalSettings.ShowElevationModel = showElevationModel;
           // Settings.GlobalSettings.ShowFieldOfView = showFieldOfView;
            Settings.GlobalSettings.ShowGrid = showGrid;
            Settings.GlobalSettings.ShowHorizon = showHorizon;
         //   Settings.GlobalSettings.ShowHorizonPanorama = showHorizonPanorama;
         //   Settings.GlobalSettings.ShowMoonsAsPointSource = showMoonsAsPointSource;
            Settings.GlobalSettings.ShowSolarSystem = showSolarSystem;
        //    Settings.GlobalSettings.FovTelescope = fovTelescope;
        //    Settings.GlobalSettings.FovEyepiece = fovEyepiece;
        //    Settings.GlobalSettings.FovCamera = fovCamera;
            Settings.GlobalSettings.LocalHorizonMode = localHorizonMode;         
            Settings.GlobalSettings.GalacticMode = galacticMode;
            Settings.GlobalSettings.SolarSystemStars = solarSystemStars;
            Settings.GlobalSettings.SolarSystemMilkyWay = solarSystemMilkyWay;
            Settings.GlobalSettings.SolarSystemCosmos = solarSystemCosmos;
            Settings.GlobalSettings.SolarSystemCMB = solarSystemCMB;
            Settings.GlobalSettings.SolarSystemOrbits = solarSystemOrbits;
            Settings.GlobalSettings.SolarSystemMinorOrbits = solarSystemMinorOrbits;
            Settings.GlobalSettings.SolarSystemMinorPlanets = solarSystemMinorPlanets;
            Settings.GlobalSettings.SolarSystemOverlays = solarSystemOverlays;
            Settings.GlobalSettings.SolarSystemLighting = solarSystemLighting;
            Settings.GlobalSettings.ShowISSModel = showISSModel;
            Settings.GlobalSettings.SolarSystemScale = solarSystemScale;
            Settings.GlobalSettings.SolarSystemMultiRes = solarSystemMultiRes;
            Settings.GlobalSettings.ShowEarthSky = showEarthSky;
            Settings.GlobalSettings.MinorPlanetsFilter = minorPlanetsFilter;
            Settings.GlobalSettings.PlanetOrbitsFilter = planetOrbitsFilter;
            Settings.GlobalSettings.ShowEquatorialGridText = showEquatorialGridText;
            Settings.GlobalSettings.ShowGalacticGrid = showGalacticGrid;
            Settings.GlobalSettings.ShowGalacticGridText = showGalacticGridText;
            Settings.GlobalSettings.ShowEclipticGrid = showEclipticGrid;
            Settings.GlobalSettings.ShowEclipticGridText = showEclipticGridText;
            Settings.GlobalSettings.ShowEclipticOverviewText = showEclipticOverviewText;
            Settings.GlobalSettings.ShowAltAzGrid = showAltAzGrid;
            Settings.GlobalSettings.ShowAltAzGridText = showAltAzGridText;
            Settings.GlobalSettings.ShowPrecessionChart = showPrecessionChart;
            Settings.GlobalSettings.ShowConstellationPictures = showConstellationPictures;
            Settings.GlobalSettings.ConstellationsEnabled = constellationsEnabled;
            Settings.GlobalSettings.ShowSkyOverlays = showSkyOverlays;
            Settings.GlobalSettings.Constellations = showConstellations;
            Settings.GlobalSettings.ShowSkyNode = showSkyNode;
            Settings.GlobalSettings.ShowSkyGrids = showSkyGrids;
           // Settings.GlobalSettings.ShowSkyOverlaysIn3d = skyOverlaysIn3d;
            Settings.GlobalSettings.ConstellationFiguresFilter = constellationFiguresFilter.Clone();
            Settings.GlobalSettings.ConstellationBoundariesFilter = constellationBoundariesFilter.Clone();
            Settings.GlobalSettings.ConstellationNamesFilter=constellationNamesFilter.Clone();
            Settings.GlobalSettings.ConstellationArtFilter=constellationArtFilter.Clone();
           // Earth3d.ignoreChanges = false;
          //  LayerManager.ProcessingUpdate = false;
            //Settings.Default.PulseMeForUpdate = !Properties.Settings.Default.PulseMeForUpdate;
        }

        #region ISettings Members

        public bool SolarSystemStars
        {
            get { return solarSystemStars; }
        }
        public bool SolarSystemMultiRes
        {
            get { return solarSystemMultiRes; }
        }
        public bool SolarSystemMilkyWay
        {
            get { return solarSystemMilkyWay; }
        }

        public bool SolarSystemCosmos
        {
            get { return solarSystemCosmos; }
        }

        public bool SolarSystemOrbits
        {
            get { return solarSystemOrbits; }
        }

        public bool SolarSystemOverlays
        {
            get { return solarSystemOverlays; }
        }

        public bool SolarSystemLighting
        {
            get { return solarSystemLighting; }
        }

        public int SolarSystemScale
        {
            get { return solarSystemScale; }
        }



        public bool ActualPlanetScale
        {
            get { return actualPlanetScale; }
        }

        public int FovCamera
        {
            get { return fovCamera; }
        }

        public int FovEyepiece
        {
            get { return fovEyepiece; }
        }

        public int FovTelescope
        {
            get { return fovTelescope; }
        }

        public double LocationAltitude
        {
            get
            {
                if (hasTime)
                {
                    return locationAltitude;
                }
                else
                {
                    return Settings.Current.LocationAltitude;
                }
            }
        }

        public double LocationLat
        {
            get
            {
                if (hasTime)
                {
                    return locationLat;
                }
                else
                {
                    return Settings.Current.LocationLat;
                }
            }
        }

        public double LocationLng
        {
            get
            {
                if (hasTime)
                {
                    return locationLng;
                }
                else
                {
                    return Settings.Current.LocationLng;
                }
            }
        }

        public bool ShowClouds
        {
            get
            {
                return showClouds;
            }
        }

        public bool ShowConstellationBoundries
        {
            get
            {
                return showConstellationBoundries;
            }
        }

        public bool ShowConstellationFigures
        {
            get { return showConstellationFigures; }
        }

        public bool ShowConstellationSelection
        {
            get { return showConstellationSelection; }
        }

        public bool ShowEcliptic
        {
            get { return showEcliptic; }
        }

        public bool ShowElevationModel
        {
            get { return showElevationModel; }
        }

        public bool ShowFieldOfView
        {
            get { return showFieldOfView; }
        }

        public bool ShowGrid
        {
            get { return showGrid; }
        }

        public bool ShowHorizon
        {
            get { return showHorizon; }
        }

        public bool ShowHorizonPanorama
        {
            get { return showHorizonPanorama; }
        }

        public bool ShowMoonsAsPointSource
        {
            get { return showMoonsAsPointSource; }
        }

        public bool ShowSolarSystem
        {
            get { return showSolarSystem; }
        }

        public bool LocalHorizonMode
        {
            get { return localHorizonMode; }
        }
        public bool GalacticMode
        {
            get { return galacticMode; }
        }
        #endregion
        // End Settings
        string thumbnailString = "";
        ImageElement thumbnail = null;

        public ImageElement Thumbnail
        {
            get
            {
                if (target != null && thumbnail == null)
                {
                    //todo create bitmap from cab file and return it
                    return null;
                    //return target.ThumbNail;
                }
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                if (owner != null) { owner.TourDirty = true; }
            }
        }

        public Dictionary<Guid, LayerInfo> Layers = new Dictionary<Guid, LayerInfo>();


        List<Overlay> overlays = new List<Overlay>();

        public List<Overlay> Overlays
        {
            get { return overlays; }
        }

        AudioOverlay musicTrack = null;

        public AudioOverlay MusicTrack
        {
            get { return musicTrack; }
            set
            {
                if (musicTrack != value)
                {
                    musicTrack = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        AudioOverlay voiceTrack = null;

        public AudioOverlay VoiceTrack
        {
            get { return voiceTrack; }
            set
            {
                if (voiceTrack != value)
                {
                    voiceTrack = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        public void AddOverlay(Overlay overlay)
        {
            if (overlay == null)
            {
                return;
            }

            overlay.Owner = this;
            overlays.Add(overlay);
            if (owner != null) { owner.TourDirty = true; }

        }

        public void RemoveOverlay(Overlay overlay)
        {
            //todo clean up temp disk
            overlays.Remove(overlay);
            if (owner != null) { owner.TourDirty = true; }
        }

        public void CleanUp()
        {
            foreach (Overlay overlay in Overlays)
            {
                overlay.CleanUp();
            }

            if (voiceTrack != null)
            {
                voiceTrack.CleanUp();
            }

            if (musicTrack != null)
            {
                musicTrack.CleanUp();
            }
        }

        public void SendToBack(Overlay target)
        {
            overlays.Remove(target);
            overlays.Insert(0, target);
            if (owner != null) { owner.TourDirty = true; }
        }

        public void BringToFront(Overlay target)
        {
            overlays.Remove(target);
            overlays.Add(target);
            if (owner != null) { owner.TourDirty = true; }
        }

        public void BringForward(Overlay target)
        {
            //int index = overlays.FindIndex(delegate(Overlay overlay) { return target == overlay; });
            int index = overlays.IndexOf(target);
            if (index < overlays.Count - 1)
            {
                overlays.Remove(target);
                overlays.Insert(index + 1, target);
            }
            if (owner != null) { owner.TourDirty = true; }
        }

        public void SendBackward(Overlay target)
        {
            int index = overlays.IndexOf(target);
            if (index > 0)
            {
                overlays.Remove(target);
                overlays.Insert(index - 1, target);
            }
            if (owner != null) { owner.TourDirty = true; }
        }

        public Overlay GetNextOverlay(Overlay current)
        {
            if (current == null)
            {
                if (overlays.Count > 0)
                {
                    return overlays[0];
                }
                else
                {
                    return null;
                }
            }

            int index = overlays.IndexOf(current);
            if (index < overlays.Count - 1)
            {
                return overlays[index + 1];
            }
            else
            {
                return overlays[0];
            }
        }

        public Overlay GetPerviousOverlay(Overlay current)
        {
            if (current == null)
            {
                if (overlays.Count > 0)
                {
                    return overlays[0];
                }
                else
                {
                    return null;
                }
            }
            int index = overlays.IndexOf(current);
            if (index > 0)
            {
                return overlays[index - 1];
            }
            else
            {
                return overlays[overlays.Count - 1];
            }
        }

        public Overlay GetOverlayById(string id)
        {
            foreach (Overlay ol in overlays)
            {
                if (ol.Id == id)
                {
                    return ol;
                }
            }
            return null;
        }

        public string TourStopThumbnailFilename
        {
            get
            {
                return string.Format("{0}.thumb.png", id);
            }
        }


        internal static TourStop FromXml(TourDocument owner, XmlNode tourStop)
        {

            TourStop newTourStop = new TourStop();
            newTourStop.owner = owner;

            newTourStop.Id = tourStop.Attributes.GetNamedItem("Id").Value.ToString();
            newTourStop.Name = tourStop.Attributes.GetNamedItem("Name").Value.ToString();
            newTourStop.Description = tourStop.Attributes.GetNamedItem("Description").Value.ToString();
            newTourStop.thumbnailString = tourStop.Attributes.GetNamedItem("Thumbnail").Value.ToString();
            newTourStop.duration = Util.ParseTimeSpan(tourStop.Attributes.GetNamedItem("Duration").Value.ToString());

            if (tourStop.Attributes.GetNamedItem("Master") != null)
            {
                newTourStop.masterSlide = bool.Parse(tourStop.Attributes.GetNamedItem("Master").Value);
            }

            if (tourStop.Attributes.GetNamedItem("NextSlide") != null)
            {
                newTourStop.nextSlide = tourStop.Attributes.GetNamedItem("NextSlide").Value;
            }

            if (tourStop.Attributes.GetNamedItem("InterpolationType") != null)
            {
                switch (tourStop.Attributes.GetNamedItem("InterpolationType").Value)
                {
                    case "Linear":
                        newTourStop.InterpolationType = InterpolationType.Linear;
                        break;
                    case "EaseIn":
                        newTourStop.InterpolationType = InterpolationType.EaseIn;
                        break;
                    case "EaseOut":
                        newTourStop.InterpolationType = InterpolationType.EaseOut;
                        break;
                    case "EaseInOut":
                        newTourStop.InterpolationType = InterpolationType.EaseInOut;
                        break;
                    case "Exponential":
                        newTourStop.InterpolationType = InterpolationType.Exponential;
                        break;
                    case "Default":
                        newTourStop.InterpolationType = InterpolationType.DefaultV;
                        break;
                    default:
                        newTourStop.InterpolationType = InterpolationType.Linear;
                        break;
                }
            }

            newTourStop.fadeInOverlays = true;

            if (tourStop.Attributes.GetNamedItem("FadeInOverlays") != null)
            {
                newTourStop.fadeInOverlays = bool.Parse(tourStop.Attributes.GetNamedItem("FadeInOverlays").Value);
            }

            if (tourStop.Attributes.GetNamedItem("Transition") != null)
            {
                //newTourStop.transition = (TransitionType)Enum.Parse(typeof(TransitionType), tourStop.Attributes.GetNamedItem("Transition").Value, true);

                switch (tourStop.Attributes.GetNamedItem("Transition").Value)
                {
                    case "Slew":
                        newTourStop.transition = TransitionType.Slew;
                        break;
                    case "Instant":

                        newTourStop.transition = TransitionType.Instant;
                        break;
                    case "CrossFade":

                        newTourStop.transition = TransitionType.CrossFade;
                        break;
                    case "FadeToBlack":
                        newTourStop.transition = TransitionType.FadeToBlack;
                        break;
                    default:
                        break;
                }
            }

            if (tourStop.Attributes.GetNamedItem("HasLocation") != null)
            {
                newTourStop.hasLocation = bool.Parse(tourStop.Attributes.GetNamedItem("HasLocation").Value);
            }

            if (newTourStop.hasLocation)
            {
                if (tourStop.Attributes.GetNamedItem("LocationAltitude") != null)
                {
                    newTourStop.locationAltitude = double.Parse(tourStop.Attributes.GetNamedItem("LocationAltitude").Value);
                }
                if (tourStop.Attributes.GetNamedItem("LocationLat") != null)
                {
                    newTourStop.locationLat = double.Parse(tourStop.Attributes.GetNamedItem("LocationLat").Value);
                }
                if (tourStop.Attributes.GetNamedItem("LocationLng") != null)
                {
                    newTourStop.locationLng = double.Parse(tourStop.Attributes.GetNamedItem("LocationLng").Value);
                }
            }

            if (tourStop.Attributes.GetNamedItem("HasTime") != null)
            {
                newTourStop.hasTime = bool.Parse(tourStop.Attributes.GetNamedItem("HasTime").Value);

                if (newTourStop.hasTime)
                {                
                    if (tourStop.Attributes.GetNamedItem("StartTime") != null)
                    {
                        newTourStop.startTime = Date.Parse(tourStop.Attributes.GetNamedItem("StartTime").Value);
                    }
                    if (tourStop.Attributes.GetNamedItem("EndTime") != null)
                    {
                        newTourStop.endTime = Date.Parse(tourStop.Attributes.GetNamedItem("EndTime").Value);
                    }
                }
            }

            if (tourStop.Attributes.GetNamedItem("ActualPlanetScale") != null)
            {
                newTourStop.actualPlanetScale = bool.Parse(tourStop.Attributes.GetNamedItem("ActualPlanetScale").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowClouds") != null)
            {
                newTourStop.showClouds = bool.Parse(tourStop.Attributes.GetNamedItem("ShowClouds").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellationBoundries") != null)
            {
                newTourStop.showConstellationBoundries = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellationBoundries").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellationFigures") != null)
            {
                newTourStop.showConstellationFigures = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellationFigures").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellationSelection") != null)
            {
                newTourStop.showConstellationSelection = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellationSelection").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowEcliptic") != null)
            {
                newTourStop.showEcliptic = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEcliptic").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowElevationModel") != null)
            {
                newTourStop.showElevationModel = bool.Parse(tourStop.Attributes.GetNamedItem("ShowElevationModel").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowFieldOfView") != null)
            {
                newTourStop.showFieldOfView = bool.Parse(tourStop.Attributes.GetNamedItem("ShowFieldOfView").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowGrid") != null)
            {
                newTourStop.showGrid = bool.Parse(tourStop.Attributes.GetNamedItem("ShowGrid").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowHorizon") != null)
            {
                newTourStop.showHorizon = bool.Parse(tourStop.Attributes.GetNamedItem("ShowHorizon").Value);
            }


            if (tourStop.Attributes.GetNamedItem("ShowHorizonPanorama") != null)
            {
                newTourStop.showHorizonPanorama = bool.Parse(tourStop.Attributes.GetNamedItem("ShowHorizonPanorama").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowMoonsAsPointSource") != null)
            {
                newTourStop.showMoonsAsPointSource = bool.Parse(tourStop.Attributes.GetNamedItem("ShowMoonsAsPointSource").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowSolarSystem") != null)
            {
                newTourStop.showSolarSystem = bool.Parse(tourStop.Attributes.GetNamedItem("ShowSolarSystem").Value);
            }

            if (tourStop.Attributes.GetNamedItem("FovTelescope") != null)
            {
                newTourStop.fovTelescope = int.Parse(tourStop.Attributes.GetNamedItem("FovTelescope").Value);
            }

            if (tourStop.Attributes.GetNamedItem("FovEyepiece") != null)
            {
                newTourStop.fovEyepiece = int.Parse(tourStop.Attributes.GetNamedItem("FovEyepiece").Value);
            }

            if (tourStop.Attributes.GetNamedItem("FovCamera") != null)
            {
                newTourStop.fovCamera = int.Parse(tourStop.Attributes.GetNamedItem("FovCamera").Value);
            }

            if (tourStop.Attributes.GetNamedItem("LocalHorizonMode") != null)
            {
                newTourStop.localHorizonMode = bool.Parse(tourStop.Attributes.GetNamedItem("LocalHorizonMode").Value);
            }

            if (tourStop.Attributes.GetNamedItem("GalacticMode") != null)
            {
                newTourStop.galacticMode = bool.Parse(tourStop.Attributes.GetNamedItem("GalacticMode").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemStars") != null)
            {
                newTourStop.solarSystemStars = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemStars").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemMilkyWay") != null)
            {
                newTourStop.solarSystemMilkyWay = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemMilkyWay").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemCosmos") != null)
            {
                newTourStop.solarSystemCosmos = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemCosmos").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemOrbits") != null)
            {
                newTourStop.solarSystemOrbits = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemOrbits").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemOverlays") != null)
            {
                newTourStop.solarSystemOverlays = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemOverlays").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemLighting") != null)
            {
                newTourStop.solarSystemLighting = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemLighting").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemScale") != null)
            {
                newTourStop.solarSystemScale = int.Parse(tourStop.Attributes.GetNamedItem("SolarSystemScale").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemMultiRes") != null)
            {
                newTourStop.solarSystemMultiRes = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemMultiRes").Value);
            }

            //new 
            if (tourStop.Attributes.GetNamedItem("ShowEquatorialGridText") != null)
            {
                newTourStop.showEquatorialGridText = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEquatorialGridText").Value);
            }
            if (tourStop.Attributes.GetNamedItem("ShowGalacticGrid") != null)
            {
                newTourStop.showGalacticGrid = bool.Parse(tourStop.Attributes.GetNamedItem("ShowGalacticGrid").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowGalacticGridText") != null)
            {
                newTourStop.showGalacticGridText = bool.Parse(tourStop.Attributes.GetNamedItem("ShowGalacticGridText").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowEclipticGrid") != null)
            {
                newTourStop.showEclipticGrid = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEclipticGrid").Value);
            }
            if (tourStop.Attributes.GetNamedItem("ShowEclipticGridText") != null)
            {
                newTourStop.showEclipticGridText = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEclipticGridText").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowEclipticOverviewText") != null)
            {
                newTourStop.showEclipticOverviewText = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEclipticOverviewText").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowAltAzGrid") != null)
            {
                newTourStop.showAltAzGrid = bool.Parse(tourStop.Attributes.GetNamedItem("ShowAltAzGrid").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowAltAzGridText") != null)
            {
                newTourStop.showAltAzGridText = bool.Parse(tourStop.Attributes.GetNamedItem("ShowAltAzGridText").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowPrecessionChart") != null)
            {
                newTourStop.showPrecessionChart = bool.Parse(tourStop.Attributes.GetNamedItem("ShowPrecessionChart").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellationPictures") != null)
            {
                newTourStop.showConstellationPictures = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellationPictures").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellationLabels") != null)
            {
                newTourStop.showConstellationLabels = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellationLabels").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemCMB") != null)
            {
                newTourStop.solarSystemCMB = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemCMB").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemMinorPlanets") != null)
            {
                newTourStop.solarSystemMinorPlanets = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemMinorPlanets").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemPlanets") != null)
            {
                newTourStop.solarSystemPlanets = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemPlanets").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowEarthSky") != null)
            {
                newTourStop.showEarthSky = bool.Parse(tourStop.Attributes.GetNamedItem("ShowEarthSky").Value);
            }

            if (tourStop.Attributes.GetNamedItem("SolarSystemMinorOrbits") != null)
            {
                newTourStop.solarSystemMinorOrbits = bool.Parse(tourStop.Attributes.GetNamedItem("SolarSystemMinorOrbits").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ShowSkyOverlays") != null)
            {
                newTourStop.showSkyOverlays = bool.Parse(tourStop.Attributes.GetNamedItem("ShowSkyOverlays").Value);
            }
            else
            {
                newTourStop.showSkyOverlays = true;
            }

            if (tourStop.Attributes.GetNamedItem("ShowConstellations") != null)
            {
                newTourStop.showConstellations = bool.Parse(tourStop.Attributes.GetNamedItem("ShowConstellations").Value);
            }
            else
            {
                newTourStop.showConstellations = true;
            }

            if (tourStop.Attributes.GetNamedItem("ShowSkyNode") != null)
            {
                newTourStop.showSkyNode = bool.Parse(tourStop.Attributes.GetNamedItem("ShowSkyNode").Value);
            }
            else
            {
                newTourStop.showSkyNode = true;
            }

            if (tourStop.Attributes.GetNamedItem("ShowSkyGrids") != null)
            {
                newTourStop.showSkyGrids = bool.Parse(tourStop.Attributes.GetNamedItem("ShowSkyGrids").Value);
            }
            else
            {
                newTourStop.showSkyGrids = true;
            }

            if (tourStop.Attributes.GetNamedItem("ShowSkyOverlaysIn3d") != null)
            {
                newTourStop.showSkyOverlaysIn3d = bool.Parse(tourStop.Attributes.GetNamedItem("ShowSkyOverlaysIn3d").Value);
            }

            if (tourStop.Attributes.GetNamedItem("EarthCutawayView") != null)
            {
                newTourStop.earthCutawayView = bool.Parse(tourStop.Attributes.GetNamedItem("EarthCutawayView").Value);
            }
            if (tourStop.Attributes.GetNamedItem("ShowISSModel") != null)
            {
                newTourStop.showISSModel = bool.Parse(tourStop.Attributes.GetNamedItem("ShowISSModel").Value);
            }

            if (tourStop.Attributes.GetNamedItem("MilkyWayModel") != null)
            {
                newTourStop.milkyWayModel = bool.Parse(tourStop.Attributes.GetNamedItem("MilkyWayModel").Value);
            }

            if (tourStop.Attributes.GetNamedItem("ConstellationBoundariesFilter") != null)
            {
                newTourStop.constellationBoundariesFilter = ConstellationFilter.Parse(tourStop.Attributes.GetNamedItem("ConstellationBoundariesFilter").Value);
            }
            else
            {
                newTourStop.constellationBoundariesFilter = ConstellationFilter.AllConstellation;
            }

            if (tourStop.Attributes.GetNamedItem("ConstellationBoundariesFilter") != null)
            {
                newTourStop.constellationFiguresFilter = ConstellationFilter.Parse(tourStop.Attributes.GetNamedItem("ConstellationBoundariesFilter").Value);
            }
            else
            {
                newTourStop.constellationFiguresFilter = new ConstellationFilter();
            }

            
            if (tourStop.Attributes.GetNamedItem("ConstellationNamesFilter") != null)
            {
                newTourStop.constellationNamesFilter = ConstellationFilter.Parse(tourStop.Attributes.GetNamedItem("ConstellationNamesFilter").Value);
            }
            else
            {
                newTourStop.constellationNamesFilter = new ConstellationFilter();
            }
             
            if (tourStop.Attributes.GetNamedItem("ConstellationArtFilter") != null)
            {
                newTourStop.constellationArtFilter = ConstellationFilter.Parse(tourStop.Attributes.GetNamedItem("ConstellationArtFilter").Value);
            }
            else
            {
                newTourStop.constellationArtFilter = new ConstellationFilter();
            }

            if (tourStop.Attributes.GetNamedItem("MinorPlanetsFilter") != null)
            {
                newTourStop.minorPlanetsFilter = int.Parse(tourStop.Attributes.GetNamedItem("MinorPlanetsFilter").Value);
            }
            if (tourStop.Attributes.GetNamedItem("PlanetOrbitsFilter") != null)
            {
                newTourStop.planetOrbitsFilter = int.Parse(tourStop.Attributes.GetNamedItem("PlanetOrbitsFilter").Value);
            }

            XmlNode place = Util.SelectSingleNode(tourStop, "Place");

            newTourStop.target = Place.FromXml(place);

            XmlNode endTarget = Util.SelectSingleNode(tourStop, "EndTarget");
            if (endTarget != null)
            {
                newTourStop.endTarget = Place.FromXml(endTarget);
            }

            XmlNode overlays = Util.SelectSingleNode(tourStop, "Overlays");

            foreach (XmlNode overlay in overlays.ChildNodes)
            {
                //todo this might have issuse if all the childeren are not good 

                newTourStop.AddOverlay(Overlay.FromXml(newTourStop, overlay));
            }

            XmlNode musicNode = Util.SelectSingleNode(tourStop, "MusicTrack");

            if (musicNode != null)
            {
                newTourStop.musicTrack = (AudioOverlay)Overlay.FromXml(newTourStop, Util.SelectSingleNode(musicNode, "Overlay"));
            }

            XmlNode voiceNode = Util.SelectSingleNode(tourStop, "VoiceTrack");

            if (voiceNode != null)
            {
                newTourStop.voiceTrack = (AudioOverlay)Overlay.FromXml(newTourStop,  Util.SelectSingleNode(voiceNode, "Overlay"));
            }

            //todo fix load thumbnail
            //newTourStop.thumbnail = UiTools.LoadBitmap(string.Format("{0}{1}.thumb.png", newTourStop.owner.WorkingDirectory, newTourStop.id));
            newTourStop.thumbnail = owner.GetCachedTexture(string.Format("{0}.thumb.png", newTourStop.id), delegate { int c=0; });
            return newTourStop;
        }

        public string GetNextDefaultName(string baseName)
        {
            int suffixId = 1;
            foreach (Overlay overlay in overlays)
            {
                if (overlay.Name.StartsWith(baseName))
                {
                    int id = 0;
                    try
                    {
                        id = int.Parse(overlay.Name.Substr(baseName.Length));
                    }
                    catch
                    {
                    }

                    if (id >= suffixId)
                    {
                        suffixId = id + 1;
                    }
                }
            }

            return string.Format("{0} {1}", baseName, suffixId);
        }


        internal void UpdateLayerOpacity()
        {
            if (!KeyFramed)
            {
                //foreach (LayerInfo info in Layers.Values)
                //{
                //    info.FrameOpacity = info.StartOpacity * (1 - tweenPosition) + info.EndOpacity * tweenPosition;
                //    int len = info.StartParams.Length;
                //    info.FrameParams = new double[len];
                //    for (int i = 0; i < len; i++)
                //    {
                //        info.FrameParams[i] = info.StartParams[i] * (1 - tweenPosition) + info.EndParams[i] * tweenPosition;
                //    }

                //}
            }
            else
            {
                UpdateTweenPosition();
            }
        }

       



        // Start of new Settings


        public bool ShowEquatorialGridText
        {
            get { return showEquatorialGridText; }
            set { showEquatorialGridText = value; }
        }

        public bool ShowGalacticGrid
        {
            get { return showGalacticGrid; }
            set { showGalacticGrid = value; }
        }


        public bool ShowGalacticGridText
        {
            get { return showGalacticGridText; }
            set { showGalacticGridText = value; }
        }
        public bool ShowEclipticGrid
        {
            get { return showEclipticGrid; }
            set { showEclipticGrid = value; }
        }


        public bool ShowEclipticGridText
        {
            get { return showEclipticGridText; }
            set { showEclipticGridText = value; }
        }

        public bool ShowEclipticOverviewText
        {
            get { return showEclipticOverviewText; }
            set { showEclipticOverviewText = value; }
        }

        public bool ShowAltAzGrid
        {
            get { return showAltAzGrid; }
            set { showAltAzGrid = value; }
        }

        public bool ShowAltAzGridText
        {
            get { return showAltAzGridText; }
            set { showAltAzGridText = value; }
        }

        public bool ShowPrecessionChart
        {
            get { return showPrecessionChart; }
            set { showPrecessionChart = value; }
        }

        public bool ShowConstellationPictures
        {
            get { return showConstellationPictures; }
            set { showConstellationPictures = value; }
        }

        public bool ShowConstellationLabels
        {
            get { return showConstellationLabels; }
            set { showConstellationLabels = value; }
        }

        //public string ConstellationsEnabled = false;

        public bool SolarSystemCMB
        {
            get { return solarSystemCMB; }
            set { solarSystemCMB = value; }
        }

        public bool SolarSystemMinorPlanets
        {
            get { return solarSystemMinorPlanets; }
            set { solarSystemMinorPlanets = value; }
        }

        public bool SolarSystemPlanets
        {
            get { return solarSystemPlanets; }
            set { solarSystemPlanets = value; }
        }

        public bool ShowEarthSky
        {
            get { return showEarthSky; }
            set { showEarthSky = value; }
        }

        public bool SolarSystemMinorOrbits
        {
            get { return solarSystemMinorOrbits; }
            set { solarSystemMinorOrbits = value; }
        }

        public string ConstellationsEnabled
        {
            get { return constellationsEnabled; }
            set { constellationsEnabled = value; }
        }

        public ConstellationFilter ConstellationFiguresFilter
        {
            get { return constellationFiguresFilter; }
            set { constellationFiguresFilter = value; }
        }

        public ConstellationFilter ConstellationBoundariesFilter
        {
            get { return constellationBoundariesFilter; }
            set { constellationBoundariesFilter = value; }
        }

        public ConstellationFilter ConstellationNamesFilter
        {
            get { return constellationNamesFilter; }
            set { constellationNamesFilter = value; }
        }

        public ConstellationFilter ConstellationArtFilter
        {
            get { return constellationArtFilter; }
            set { constellationArtFilter = value; }
        }

        public bool ShowSkyOverlays
        {
            get { return showSkyOverlays; }
            set { showSkyOverlays = value; }
        }
        public bool ShowConstellations
        {
            get { return showConstellations; }
            set { showConstellations = value; }
        }

        public bool ShowSkyNode
        {
            get { return showSkyNode; }
            set { showSkyNode = value; }
        }

        public bool ShowSkyGrids
        {
            get { return showSkyGrids; }
            set { showSkyGrids = value; }
        }

        public bool ShowSkyOverlaysIn3d
        {
            get { return showSkyOverlaysIn3d; }
            set { showSkyOverlaysIn3d = value; }
        }


        public bool EarthCutawayView
        {
            get { return earthCutawayView; }
            set { earthCutawayView = value; }
        }

        public bool ShowISSModel
        {
            get { return showISSModel; }
            set { showISSModel = value; }
        }

        public bool MilkyWayModel
        {
            get { return milkyWayModel; }
            set { milkyWayModel = value; }
        }

        public int MinorPlanetsFilter
        {
            get { return minorPlanetsFilter; }
            set { minorPlanetsFilter = value; }
        }

        public int PlanetOrbitsFilter
        {
            get { return planetOrbitsFilter; }
            set { planetOrbitsFilter = value; }
        }

        public SettingParameter GetSetting(StockSkyOverlayTypes type)
        {
            return new SettingParameter(false,1,false,null);
        }
    }

    public class LayerInfo
    {
        public Guid ID = Guid.NewGuid();
        public float StartOpacity = 1;
        public float EndOpacity = 1;
        public float FrameOpacity = 1;
        public double[] StartParams = new double[0];
        public double[] EndParams = new double[0];
        public double[] FrameParams = new double[0];
    }

    public interface IUndoStep
    {
        void Redo();
        string ToString();
        void Undo();
    }

    public class UndoTourStopChange : IUndoStep
    {
        string undoXml = "";
        string redoXml = "";
        int currentIndex = 0;
        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        public UndoTourStopChange(string text, TourDocument tour)
        {
            currentIndex = tour.CurrentTourstopIndex;
            actionText = text;
            targetTour = tour;
            undoXml = TourStop.GetXmlText(tour.CurrentTourStop);
            targetTour.TourDirty = true;

        }

        public void Undo()
        {
            TourStop tsRedo = targetTour.TourStops[currentIndex];
            XmlDocumentParser parser = new XmlDocumentParser();

            XmlDocument doc = parser.ParseFromString(undoXml, "text/xml");


            XmlNode node = Util.SelectSingleNode(doc, "TourStop");
            targetTour.TourStops[currentIndex] = TourStop.FromXml(targetTour, node);
            targetTour.CurrentTourstopIndex = currentIndex;


            // Setup redo
            if (string.IsNullOrEmpty(redoXml))
            {
                redoXml = TourStop.GetXmlText(tsRedo);
            }
            targetTour.TourDirty = true;

        }

        public void Redo()
        {
            XmlDocumentParser parser = new XmlDocumentParser();
            XmlDocument doc = parser.ParseFromString(redoXml, "text/xml");
            XmlNode node = Util.SelectSingleNode(doc, "TourStop");
            targetTour.TourStops[currentIndex] = TourStop.FromXml(targetTour, node);
            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;
        }

        override public string ToString()
        {
            return actionText;
        }
    }
}
