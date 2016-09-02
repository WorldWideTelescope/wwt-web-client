using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;


namespace wwtlib
{
    public class Settings : ISettings
    {
        static Settings active = null;
        static public ISettings TourSettings = null;
        static public Settings Current
        {
            get
            {
                if (active == null)
                {
                    active = new Settings();
                }
                return active;
            }
        }

        static public Settings GlobalSettings
        {
            get
            {
                if (active == null)
                {
                    active = new Settings();
                }
                return active;
            }
        }

        static public ISettings Active
        {
            get
            {
                if (active == null)
                {
                    active = new Settings();
                }
                if (TourSettings != null)
                {
                    return TourSettings;
                }
                return active;
            }
        }

        public Settings()
        {
        }
        public bool AutoRepeatTour = false;
        private bool localHorizonMode = false;
        private bool galacticMode = false;
        private string constellationBoundryColor = "blue";
        private string constellationSelectionColor = "yellow";
        private string constellationFigureColor = "red";
        private bool showConstellationFigures = true;
        private bool showConstellationBoundries = true;
        private bool showConstellationSelection = true;
        private bool showCrosshairs = true;
        private string crosshairsColor = "white";
        private bool showEcliptic = false;
        private double locationLat = 47.717;
        private double locationLng = -122.08580;
        private double locationAltitude = 100.0;
        private bool showFiledOfView = false;
        bool actualPlanetScale = true;
        int fovCamera = 0;
        int fovEyepiece = 0;
        int fovTelescope = 0;
        bool showClouds = false;


        private bool showGrid = false;
        private bool showHorizon = true;
        private bool showHorizonPanorama = false;
        private bool showMoonsAsPointSource = true;
        private bool showSolarSystem = true;
        private bool solarSystemStars = true;
        private bool solarSystemMilkyWay = true;
        private bool solarSystemCosmos = true;
        private bool solarSystemOrbits = true;
        private bool solarSystemOverlays = true;
        private bool solarSystemLighting = true;
        private bool solarSystemMultiRes = true;
        private int solarSystemScale = 1;
        private bool smoothPan = true;


        public string ConstellationFigureColor
        {
            get { return constellationFigureColor; }
            set { constellationFigureColor = value; }
        }

        public string ConstellationBoundryColor
        {
            get { return constellationBoundryColor; }
            set { constellationBoundryColor = value; }
        }

        public string ConstellationSelectionColor
        {
            get { return constellationSelectionColor; }
            set { constellationSelectionColor = value; }
        }

        public bool ShowCrosshairs
        {
            get { return showCrosshairs; }
            set { showCrosshairs = value; }
        }

        public bool SmoothPan
        {
            get { return smoothPan; }
            set { smoothPan = value; }
        }

        public string CrosshairsColor
        {
            get { return crosshairsColor; }
            set { crosshairsColor = value; }
        }

        public bool ActualPlanetScale
        {
            get { return actualPlanetScale; }
            set { actualPlanetScale = value; }
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
            get { return locationAltitude; }
            set { locationAltitude = value; }
        }

        public double LocationLat
        {
            get { return locationLat; }
            set { locationLat = value; }
        }

        public double LocationLng
        {
            get { return locationLng; }
            set { locationLng = value; }
        }

        public bool ShowClouds
        {
            get { return showClouds; }
        }

        public bool ShowConstellationBoundries
        {
            get { return showConstellationBoundries; }
            set { showConstellationBoundries = value; }
        }

        public bool ShowConstellationFigures
        {
            get { return showConstellationFigures; }
            set { showConstellationFigures = value; }
        }

        public bool ShowConstellationSelection
        {
            get { return showConstellationSelection; }
            set { showConstellationSelection = value; }
        }

        public bool ShowEcliptic
        {
            get { return showEcliptic; }
            set { showEcliptic = value; }
        }

        bool showElevationModel = true;

        public bool ShowElevationModel
        {
            get { return showElevationModel; }
            set { showElevationModel = value; }
        }

        public bool ShowFieldOfView
        {
            get
            {
                return showFiledOfView;
            }

        }

        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value; }
        }

        public bool ShowHorizon
        {
            get { return showHorizon; }
            set { showHorizon = value; }
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
            set { showSolarSystem = value; }
        }

        public bool LocalHorizonMode
        {
            get { return localHorizonMode; }
            set { localHorizonMode = value; }
        }

        public bool GalacticMode
        {
            get { return galacticMode; }
            set
            {
                galacticMode = value;
            }
        }

        public bool SolarSystemStars
        {
            get { return solarSystemStars; }
            set {  solarSystemStars = value; }
        }

        public bool SolarSystemMilkyWay
        {
            get { return solarSystemMilkyWay; }
            set { solarSystemMilkyWay = value; }
       }

        public bool SolarSystemCosmos
        {
            get { return solarSystemCosmos; }
            set { solarSystemCosmos = value; }
        }

        public bool SolarSystemOrbits
        {
            get { return solarSystemOrbits; }
            set { solarSystemOrbits = value; }
        }

        public bool SolarSystemOverlays
        {
            get { return solarSystemOverlays; }
            set { solarSystemOverlays = value; }
        }

        public bool SolarSystemLighting
        {
            get { return solarSystemLighting; }
            set { solarSystemLighting = value; }
       }

        public bool SolarSystemMultiRes
        {
            get { return true; }
            set { solarSystemMultiRes = value; }
       }

        public int SolarSystemScale
        {
            get { return solarSystemScale; }
            set { solarSystemScale = value; }
        }

        bool showEquatorialGridText = false;

        public bool ShowEquatorialGridText
        {
            get { return showEquatorialGridText; }
            set { showEquatorialGridText = value; }
        }

        bool showGalacticGrid = false;
        public bool ShowGalacticGrid
        {
            get { return showGalacticGrid; }
            set { showGalacticGrid = value; }
        }

        bool showGalacticGridText = false;

        public bool ShowGalacticGridText
        {
            get { return showGalacticGridText; }
            set { showGalacticGridText = value; }
        }
        bool showEclipticGrid = false;
        public bool ShowEclipticGrid
        {
            get { return showEclipticGrid; }
            set { showEclipticGrid = value; }
        }

        private bool showEclipticGridText = false;

        public bool ShowEclipticGridText
        {
            get { return showEclipticGridText; }
            set { showEclipticGridText = value; }
        }
        private bool showEclipticOverviewText = false;

        public bool ShowEclipticOverviewText
        {
            get { return showEclipticOverviewText; }
            set { showEclipticOverviewText = value; }
        }
        private bool showAltAzGrid = false;

        public bool ShowAltAzGrid
        {
            get { return showAltAzGrid; }
            set { showAltAzGrid = value; }
        }
        private bool showAltAzGridText = false;

        public bool ShowAltAzGridText
        {
            get { return showAltAzGridText; }
            set { showAltAzGridText = value; }
        }
        private bool showPrecessionChart = false;

        public bool ShowPrecessionChart
        {
            get { return showPrecessionChart; }
            set { showPrecessionChart = value; }
        }
        private bool showConstellationPictures = false;

        public bool ShowConstellationPictures
        {
            get { return showConstellationPictures; }
            set { showConstellationPictures = value; }
        }
        private bool showConstellationLabels = false;

        public bool ShowConstellationLabels
        {
            get { return showConstellationLabels; }
            set { showConstellationLabels = value; }
        }

        //public string ConstellationsEnabled = false;

        bool solarSystemCMB = true;
        public bool SolarSystemCMB
        {
            get { return solarSystemCMB; }
            set {  solarSystemCMB= value;}
        }
        bool solarSystemMinorPlanets = false;
        
        public bool SolarSystemMinorPlanets
        {
            get { return solarSystemMinorPlanets; }
            set { solarSystemMinorPlanets = value;}
        }

        bool solarSystemPlanets = true;
        public bool SolarSystemPlanets
        {
            get { return solarSystemPlanets; }
            set { solarSystemPlanets = value;}
        }

        bool showEarthSky = true;
        public bool ShowEarthSky
        {
            get { return showEarthSky; }
            set { showEarthSky = value;}
        }

        bool solarSystemMinorOrbits = false;
        public bool SolarSystemMinorOrbits
        {
            get { return solarSystemMinorOrbits; }
            set { solarSystemMinorOrbits = value;}
        }

        string constellationsEnabled = "";
        public string ConstellationsEnabled
        {
            get { return constellationsEnabled; }
            set { constellationsEnabled = value;}
        }

        ConstellationFilter constellationFiguresFilter = new ConstellationFilter();
        public ConstellationFilter ConstellationFiguresFilter
        {
            get { return constellationFiguresFilter; }
            set { constellationFiguresFilter = value;}
        }

        ConstellationFilter constellationBoundariesFilter= new ConstellationFilter();
        public ConstellationFilter ConstellationBoundariesFilter
        {
            get { return constellationBoundariesFilter; }
            set { constellationBoundariesFilter = value;}
        }

        ConstellationFilter constellationNamesFilter = new ConstellationFilter();
        public ConstellationFilter ConstellationNamesFilter
        {
            get { return constellationNamesFilter; }
            set { constellationNamesFilter = value;}
        }

        ConstellationFilter constellationArtFilter = new ConstellationFilter();
        public ConstellationFilter ConstellationArtFilter
        {
            get { return constellationArtFilter; }
            set { constellationArtFilter = value;}
        }

        bool showSkyOverlays = true;
        public bool ShowSkyOverlays
        {
            get { return showSkyOverlays; }
            set { showSkyOverlays = value;}
        }
        bool showConstellations = true;
        public bool ShowConstellations
        {
            get { return showConstellations; }
            set { showConstellations = value;}
        }

        bool showSkyNode = true;
        public bool ShowSkyNode
        {
            get { return showSkyNode; }
            set { showSkyNode = value;}
        }

        bool showSkyGrids = true;
        public bool ShowSkyGrids
        {
            get { return showSkyGrids; }
            set { showSkyGrids = value;}
        }

        bool showSkyOverlaysIn3d = true;
        public bool ShowSkyOverlaysIn3d
        {
            get { return showSkyOverlaysIn3d; }
            set { showSkyOverlaysIn3d = value;}
        }


        bool earthCutawayView = false;
        public bool EarthCutawayView
        {
            get { return earthCutawayView; }
            set { earthCutawayView = value;}
        }

        bool showISSModel = false;
        public bool ShowISSModel
        {
            get { return showISSModel; }
            set { showISSModel = value;}
        }

        bool milkyWayModel = false;
        public bool MilkyWayModel
        {
            get { return milkyWayModel; }
            set { milkyWayModel = value;}
        }

        int minorPlanetsFilter = 255;
        public int MinorPlanetsFilter 
        {
            get { return minorPlanetsFilter; }
            set { minorPlanetsFilter = value;}
        }

        int planetOrbitsFilter = 2147483647;
        public int PlanetOrbitsFilter
        {
            get { return planetOrbitsFilter; }
            set { planetOrbitsFilter = value;}
        }
        bool constellations = true;
        public bool Constellations
        {
            get { return constellations; }
            set { constellations = value; }
        }

        public SettingParameter GetSetting(StockSkyOverlayTypes type)
        {
            if (type == StockSkyOverlayTypes.FadeToBlack)
            {
                return new SettingParameter(true, 0, 0 != 0, null);
            }
            return new SettingParameter(false, 1, false, null);
        }
    }
}
