using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;

namespace wwtlib
{

    public class Planets
    {
        static Texture[] planetTextures;
        static double[] planetScales;
        static double[] planetDiameters;
        static double[] planetTilts;
        static Vector3d[][] orbits;
        static Dictionary<double, int> planetDrawOrder;

        public static Texture LoadPlanetTexture(string url)
        {
            Texture texture = new Texture();

            texture.Load(url);
    
            return texture;
        }

        public static Vector3d GetPlanet3dLocation(SolarSystemObjects target)
        {
            try
            {
                if ((int)target < 21)
                {
                    return (Vector3d)planet3dLocations[(int)target].Copy();
                }
            }
            catch
            {
            }
            return Vector3d.Create(0, 0, 0);
        }

        public static double GetPlanet3dSufaceAltitude(SolarSystemObjects target)
        {
            try
            {
                if ((int)target < 21)
                {
                    return GetAdjustedPlanetRadius((int)target);
                }
            }
            catch
            {
            }
            return 0;
        }

        public static Vector3d GetPlanetTargetPoint(SolarSystemObjects target, double lat, double lng, double jNow)
        {
            Vector3d temp;
            if (jNow == 0)
            {
                temp = (Vector3d)Planets.GetPlanet3dLocation(target);
            }
            else
            {
                temp = (Vector3d)Planets.GetPlanet3dLocationJD(target, jNow);
            }
            temp.Add((Vector3d)Coordinates.RADecTo3dAu((lng / 15) + 6, lat, Planets.GetPlanet3dSufaceAltitude(target)));
            return (Vector3d)temp;
        }

        public static Vector3d GetPlanet3dLocationJD(SolarSystemObjects target, double jNow)
        {
            try
            {
                Vector3d result = new Vector3d();
                AstroRaDec centerRaDec = AstroCalc.GetPlanet(jNow, 0, 0, 0, -6378149);
                Vector3d center = (Vector3d)Coordinates.RADecTo3dAu(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
                if (target == SolarSystemObjects.Earth)
                {
                    result = Vector3d.Create(-center.X, -center.Y, -center.Z);
                }
                else
                {
                    AstroRaDec planet = AstroCalc.GetPlanet(jNow, (EO)(int)target, 0, 0, -6378149);
                    result = (Vector3d)Coordinates.RADecTo3dAu(planet.RA, planet.Dec, planet.Distance);
                    result.Subtract(center);
                }

                result.RotateX(Coordinates.MeanObliquityOfEcliptic(jNow) * RC);
                if (Settings.Active.SolarSystemScale != 1)
                {
                    switch (target)
                    {
                        case SolarSystemObjects.Moon:
                            {
                                Vector3d parent = (Vector3d)GetPlanet3dLocationJD(SolarSystemObjects.Earth, jNow);
                                // Parent Centric
                                result.Subtract(parent);
                                result.Multiply(Settings.Active.SolarSystemScale / 2);
                                result.Add(parent);
                            }
                            break;
                        case SolarSystemObjects.Io:
                        case SolarSystemObjects.Europa:
                        case SolarSystemObjects.Ganymede:
                        case SolarSystemObjects.Callisto:
                            {
                                Vector3d parent = (Vector3d)GetPlanet3dLocationJD(SolarSystemObjects.Jupiter, jNow);

                                // Parent Centric
                                result.Subtract(parent);
                                result.Multiply(Settings.Active.SolarSystemScale);
                                result.Add(parent);
                            }
                            break;

                        default:
                            break;
                    }
                }
                return (Vector3d)result;
            }
            catch
            {
                return Vector3d.Create(0, 0, 0);
            }
        }
        public static AstroRaDec GetPlanetLocation(string name)
        {
            int id = GetPlanetIDFromName(name);

            if (planetLocations != null)
            {

                return planetLocations[id];
            }
            else
            {
                return AstroCalc.GetPlanet(SpaceTimeController.JNow, (EO)id, SpaceTimeController.Location.Lat, SpaceTimeController.Location.Lng, SpaceTimeController.Altitude);
            }
        }
        public static AstroRaDec GetPlanetLocationJD(string name, double jNow)
        {
            int id = GetPlanetIDFromName(name);


            return AstroCalc.GetPlanet(jNow, (EO)id, SpaceTimeController.Location.Lat, SpaceTimeController.Location.Lng, SpaceTimeController.Altitude);

        }
        public static int GetPlanetIDFromName(string planetName)
        {
            switch (planetName)
            {
                case "Sun":
                    return 0;
                case "Mercury":
                    return 1;
                case "Venus":
                    return 2;
                case "Mars":
                    return 3;
                case "Jupiter":
                    return 4;
                case "Saturn":
                    return 5;
                case "Uranus":
                    return 6;
                case "Neptune":
                    return 7;
                case "Pluto":
                    return 8;
                case "Moon":
                    return 9;
                case "Io":
                    return 10;
                case "Europa":
                    return 11;
                case "Ganymede":
                    return 12;
                case "Callisto":
                    return 13;
                case "Earth":
                    return 19;
                case "IoShadow":
                    return 14;
                     case "EuropaShadow":
                    return 15;
                     case "GanymedeShadow":
                    return 16;
                     case "CallistoShadow":
                    return 17;
                     case "SunEclipsed":
                    return 18;
                     case "Custom":
                    return 20;
                     case "Undefined":
                    return 65536;
                default:
                    return -1;
            }
        }



        public static string GetNameFrom3dId(int id)
        {
            switch (id)
            {
                case 0:
                    return "Sun";
                case 1:
                    return "Mercury";
                case 2:
                    return "Venus";
                case 3:
                    return "Visible Imagery";
                case 4:
                    return "Jupiter";
                case 5:
                    return "Saturn";
                case 6:
                    return "Uranus";
                case 7:
                    return "Neptune";
                case 8:
                    return "Pluto";
                case 9:
                    return "Moon";
                case 10:
                    return "Io (Jupiter)";
                case 11:
                    return "Europa (Jupiter)";
                case 12:
                    return "Ganymede (Jupiter)";
                case 13:
                    return "Callisto (Jupiter)";
                case 19: 
                    return "Bing Maps Aerial";
                default:
                    return "";
            }
        }
        //public static string GetLocalPlanetName(string planetName)
        //{
        //    switch (planetName)
        //    {
        //        case "Sun":
        //            return Language.GetLocalizedText(291, "Sun");
        //        case "Mercury":
        //            return Language.GetLocalizedText(292, "Mercury");
        //        case "Venus":
        //            return Language.GetLocalizedText(293, "Venus");
        //        case "Mars":
        //            return Language.GetLocalizedText(294, "Mars");
        //        case "Jupiter":
        //            return Language.GetLocalizedText(295, "Jupiter");
        //        case "Saturn":
        //            return Language.GetLocalizedText(296, "Saturn");
        //        case "Uranus":
        //            return Language.GetLocalizedText(297, "Uranus");
        //        case "Neptune":
        //            return Language.GetLocalizedText(298, "Neptune");
        //        case "Pluto":
        //            return Language.GetLocalizedText(299, "Pluto");
        //        case "Moon":
        //            return Language.GetLocalizedText(300, "Moon");
        //        case "Io":
        //            return Language.GetLocalizedText(301, "Io");
        //        case "Europa":
        //            return Language.GetLocalizedText(302, "Europa");
        //        case "Ganymede":
        //            return Language.GetLocalizedText(303, "Ganymede");
        //        case "Callisto":
        //            return Language.GetLocalizedText(304, "Callisto");
        //        default:
        //            return planetName;
        //    }
        //}

        public static bool HighPercision = true;
        public static bool ShowActualSize = Settings.Active.ActualPlanetScale;
        protected static double RC = (Math.PI / 180.0);
        public static double[] planetOrbitalYears;
        private static Vector3d[] planet3dLocations;
        public static double[] planetRotations;
        public static Color[] planetColors;
        public static double[] planetRotationPeriod;
        static double jNow = 0;
        public static void UpdatePlanetLocations(bool threeDee)
        {
            jNow = SpaceTimeController.JNow;
            if (threeDee)
            {
                UpdateOrbits(0);
            }

            if (planetDiameters == null)
            {
                planetDiameters = new double[20];
                planetDiameters[0] = 0.009291568;
                planetDiameters[1] = 0.0000325794793734425;
                planetDiameters[2] = 0.0000808669220531394;
                planetDiameters[3] = 0.0000453785605596396;
                planetDiameters[4] = 0.000954501;
                planetDiameters[5] = 0.000802173;
                planetDiameters[6] = 0.000339564;
                planetDiameters[7] = 0.000324825;
                planetDiameters[8] = 0.0000152007379777805;
                planetDiameters[9] = 0.0000232084653538149;
                planetDiameters[10] = 0.0000243519298386342;
                planetDiameters[11] = 0.0000208692629580609;
                planetDiameters[12] = 0.0000351742670356556;
                planetDiameters[13] = 0.0000322263666626559;
                planetDiameters[14] = 0.0000243519298386342;
                planetDiameters[15] = 0.0000208692629580609;
                planetDiameters[16] = 0.0000351742670356556;
                planetDiameters[17] = 0.0000322263666626559;
                planetDiameters[18] = 0.009291568 * 2;
                planetDiameters[19] = 0.00008556264121178090;
            }
            if (planetColors == null)
            {
                Color lightYellow = Color.FromArgb(255, 255, 255, 221);
                Color orangeRed = Color.FromArgb(255, 255, 68, 00);

                planetColors = new Color[20];
                planetColors[0] = Colors.Yellow;
                planetColors[1] = Colors.White;
                planetColors[2] = lightYellow;
                planetColors[3] = orangeRed;
                planetColors[4] = Color.FromArgb(255, 255, 165, 00);
                planetColors[5] = Color.FromArgb(255, 184, 134, 11);
                planetColors[6] = Color.FromArgb(255, 173, 216, 230);
                planetColors[7] = Colors.Blue;
                planetColors[8] = Colors.White;
                planetColors[9] = Colors.White;
                planetColors[10] = Colors.White;
                planetColors[11] = Colors.White;
                planetColors[12] = Colors.White;
                planetColors[13] = Colors.White;
                planetColors[14] = Colors.Black;
                planetColors[15] = Colors.Black;
                planetColors[16] = Colors.Black;
                planetColors[17] = Colors.Black;
                planetColors[18] = Colors.White;
                planetColors[19] = Color.FromArgb(255, 173, 216, 230);

            }

            if (planetTilts == null)
            {
                planetTilts = new double[20];
                planetTilts[0] = 0.0;
                planetTilts[1] = 0.01;
                planetTilts[2] = 177.4;
                planetTilts[3] = 25.19;
                planetTilts[4] = 3.13;
                planetTilts[5] = 26.73;
                planetTilts[6] = 97.77;
                planetTilts[7] = 28.32;
                planetTilts[8] = 119.61;
                planetTilts[9] = 23.439;
                planetTilts[10] = 2.21;
                planetTilts[11] = 0;
                planetTilts[12] = -0.33;
                planetTilts[13] = 0;
                planetTilts[14] = 0;
                planetTilts[15] = 0;
                planetTilts[16] = 0;
                planetTilts[17] = 0;
                planetTilts[18] = 0;
                planetTilts[19] = 23.5;
            }

            planetTilts[19] = obliquity / RC;

            if (planetRotationPeriod == null)
            {
                planetRotationPeriod = new double[20];
                planetRotationPeriod[0] = 25.37995;
                planetRotationPeriod[1] = 58.6462;
                planetRotationPeriod[2] = -243.0187;
                planetRotationPeriod[3] = 1.02595675;
                planetRotationPeriod[4] = 0.41007;
                planetRotationPeriod[5] = 0.426;
                planetRotationPeriod[6] = -0.71833;
                planetRotationPeriod[7] = 0.67125;
                planetRotationPeriod[8] = -6.38718;
                planetRotationPeriod[9] = 27.3;
                planetRotationPeriod[10] = 1.769137786;
                planetRotationPeriod[11] = 3.551;
                planetRotationPeriod[12] = 7.155;
                planetRotationPeriod[13] = 16.69;
                planetRotationPeriod[14] = 0;
                planetRotationPeriod[15] = 0;
                planetRotationPeriod[16] = 0;
                planetRotationPeriod[17] = 0;
                planetRotationPeriod[18] = 0;
                planetRotationPeriod[19] = .99726968;
            }

            if (planetScales == null)
            {
                planetScales = new double[20];
            }



            if (planet3dLocations == null)
            {
                planet3dLocations = new Vector3d[20];
            }

            if (Settings.Active.ActualPlanetScale)
            {
                planetScales[0] = .5f;
                planetScales[1] = .25f;
                planetScales[2] = .25f;
                planetScales[3] = .25f;
                planetScales[4] = .25f;
                planetScales[5] = .5f;
                planetScales[6] = .25f;
                planetScales[7] = .25f;
                planetScales[8] = .25f;
                planetScales[9] = .25f;
                planetScales[10] = .25f;
                planetScales[11] = .25f;
                planetScales[12] = .25f;
                planetScales[13] = .25f;
                planetScales[14] = .25f;
                planetScales[15] = .25f;
                planetScales[16] = .25f;
                planetScales[17] = .25f;
                planetScales[18] = .5f;
                planetScales[19] = .25f;

            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    if (i < 10)
                    {
                        planetScales[i] = .25f;
                    }
                    else
                    {
                        planetScales[i] = .1f;
                    }
                }

                // Make Sun and Saturn bigger
                planetScales[0] = .5f;
                planetScales[5] = .5f;
                planetScales[18] = .5f;

            }


            planetDrawOrder = new Dictionary<double, int>();
            planetLocations = new AstroRaDec[20];
            
            Vector3d center = new Vector3d();
            int planetCenter = 0;
            if (planetCenter > -1)
            {
                AstroRaDec centerRaDec = AstroCalc.GetPlanet(jNow, (EO)planetCenter, threeDee ? 0 : SpaceTimeController.Location.Lat, threeDee ? 0 : SpaceTimeController.Location.Lng, threeDee ? -6378149 : SpaceTimeController.Altitude);
                center = (Vector3d)Coordinates.RADecTo3dAu(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
            }
            planet3dLocations[19] = Vector3d.Create(-center.X, -center.Y, -center.Z);
            planet3dLocations[19].RotateX(obliquity);
            for (int i = 0; i < 18; i++)
            {
                planetLocations[i] = AstroCalc.GetPlanet(jNow, (EO)i, threeDee ? 0 : SpaceTimeController.Location.Lat, threeDee ? 0 : SpaceTimeController.Location.Lng, threeDee ? -6378149 : SpaceTimeController.Altitude);
                planet3dLocations[i] = (Vector3d)Coordinates.RADecTo3dAu(planetLocations[i].RA, planetLocations[i].Dec, planetLocations[i].Distance);

                //planet3dLocations[i] = new Vector3d(planet3dLocations[i].X - center.X,
                //                                        planet3dLocations[i].Y - center.Y,
                //                                        planet3dLocations[i].Z - center.Z);
                planet3dLocations[i].Subtract(center);

                planet3dLocations[i].RotateX(obliquity);

                if (Settings.Active.ActualPlanetScale)
                {
                    planetScales[i] = (2 * Math.Atan(.5 * (planetDiameters[i] / planetLocations[i].Distance))) / Math.PI * 180;
                    //if (i == 5 || i == 0)
                    //{
                    //    planetScales[i] *= 2;
                    //}
                    //if ((i > 9 && i != 18) && Properties.Settings.Default.ShowMoonsAsPointSource) // Jupiters moons should be bigger
                    //{
                    //    planetScales[i] *= 2;
                    //}
                }
                if (Settings.Active.SolarSystemScale != 1)
                {

                    SolarSystemObjects id = (SolarSystemObjects)i;
                    switch (id)
                    {
                        case SolarSystemObjects.Moon:
                            {
                                Vector3d parent = (planet3dLocations[(int)SolarSystemObjects.Earth]);
                                // Parent Centric
                                planet3dLocations[i].Subtract(parent);
                                planet3dLocations[i].Multiply(Settings.Active.SolarSystemScale / 2);
                                planet3dLocations[i].Add(parent);
                            }
                            break;
                        case SolarSystemObjects.Io:
                        case SolarSystemObjects.Europa:
                        case SolarSystemObjects.Ganymede:
                        case SolarSystemObjects.Callisto:
                            {
                                Vector3d parent = (planet3dLocations[(int)SolarSystemObjects.Jupiter]);
                                // Parent Centric
                                planet3dLocations[i].Subtract(parent);
                                planet3dLocations[i].Multiply(Settings.Active.SolarSystemScale);
                                planet3dLocations[i].Add(parent);
                            }
                            break;

                        default:
                            break;
                    }
                }

                double finalDistance = -planetLocations[i].Distance;
                while (planetDrawOrder.ContainsKey(finalDistance))
                {
                    finalDistance += 0.0000000001;
                }
                planetDrawOrder[finalDistance] = i;
            }




            planetLocations[18] = planetLocations[0];
            planetScales[0] *= 2;
            planetScales[18] = planetScales[0];
            planetScales[5] = planetScales[5]*2;

            lastUpdate = SpaceTimeController.Now;

        }
        static Matrix3d eclipticTilt;

        static int lastPlanetCenterID = -2;
        static int orbitalSampleRate = 256;
        static double obliquity = 23.5 * RC;
     //   static Mutex OrbitsMutex = new Mutex();
       // static bool downloadedPlanets = false;

        public static void PlanetsReady()
        {

        }

        public static void UpdateOrbits(int planetCenter)
        {
            try
            {
                //if (!downloadedPlanets)
                //{
                //    Wtml.GetWtmlFile("", PlanetsReady);
                //}

                obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) * RC;
                if (planetCenter != lastPlanetCenterID)
                {
                    orbits = null;
                }
                lastPlanetCenterID = planetCenter;
                if (orbits == null)
                {
                    if (planetCenter < 0)
                    {
                        eclipticTilt = Matrix3d.Identity;
                    }
                    else
                    {
                        eclipticTilt = Matrix3d.Identity;
                        eclipticTilt = Matrix3d.RotationX((float)obliquity);
                    }
                    if (planetOrbitalYears == null)
                    {
                        planetOrbitalYears = new double[20];
                        planetOrbitalYears[0] = 1;
                        planetOrbitalYears[1] = .241;
                        planetOrbitalYears[2] = .615;
                        planetOrbitalYears[3] = 1.881;
                        planetOrbitalYears[4] = 11.87;
                        planetOrbitalYears[5] = 29.45;
                        planetOrbitalYears[6] = 84.07;
                        planetOrbitalYears[7] = 164.9;
                        planetOrbitalYears[8] = 248.1;
                        planetOrbitalYears[9] = 27.3 / 365.25;
                        planetOrbitalYears[10] = 16.6890184 / 365.25;
                        planetOrbitalYears[11] = 3.551181 / 365.25;
                        planetOrbitalYears[12] = 7.15455296 / 365.25;
                        planetOrbitalYears[13] = 16.6890184 / 365.25;
                        planetOrbitalYears[19] = 1;

                    }
                    if (!ReadOrbits())
                    {
                        orbits = new Vector3d[20][];

                        for (int i = 1; i < 20; i++)
                        {
                            orbits[i] = new Vector3d[orbitalSampleRate];

                            if (i < 9 || i == 19)
                            {
                                for (int j = 0; j < orbitalSampleRate; j++)
                                {
                                    int centerId = planetCenter;
                                    double now = jNow + ((planetOrbitalYears[i] * 365.25 / (orbitalSampleRate)) * (double)(j - (orbitalSampleRate / 2)));
                                    Vector3d center = new Vector3d();

                                    if (i == (int)SolarSystemObjects.Moon)
                                    {
                                        centerId = -1;
                                    }
                                    else if (i > 9 && i < 14)
                                    {
                                        centerId = (int)SolarSystemObjects.Jupiter;
                                    }

                                    if (centerId > -1)
                                    {
                                        AstroRaDec centerRaDec = AstroCalc.GetPlanet(now, (EO)centerId, 0, 0, -6378149);
                                        center = Coordinates.RADecTo3dAu(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
                                    }


                                    if (i != 19)
                                    {
                                        AstroRaDec planetRaDec = AstroCalc.GetPlanet(now, (EO)i, 0, 0, -6378149);
                                        // todo move to double precition for less trucation
                                        orbits[i][ j] = (Vector3d)Coordinates.RADecTo3dAu(planetRaDec.RA, planetRaDec.Dec, planetRaDec.Distance);
                                        orbits[i][j].Subtract( (Vector3d)center);
                                    }
                                    else
                                    {
                                        orbits[i][j] = Vector3d.Create(-center.X, -center.Y, -center.Z);
                                    }

                                    // todo is the tilt right?
                                    //if (i != (int)SolarSystemObjects.Moon && !((i > 9 && i < 14)))
                                    {
                                        orbits[i][j].RotateX(obliquity);
                                    }
                                }
                                orbits[i][ orbitalSampleRate - 1] = orbits[i][0];
                            }
                        }
                        DumpOrbitsFile();
                    }
                }
            }
            finally
            {
//                OrbitsMutex.ReleaseMutex();
            }
        }

        public static bool ReadOrbits()
        {
            return false;
            //string filename = Properties.Settings.Default.CahceDirectory + @"data\orbits.bin";
            //DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=orbitsbin", filename, false, true);
            //FileStream fs = null;
            //BinaryReader br = null;
            //long len = 0;
            //try
            //{
            //    fs = new FileStream(filename, FileMode.Open);
            //    len = fs.Length;
            //    br = new BinaryReader(fs);
            //    orbits = new Vector3d[20, orbitalSampleRate];

            //    for (int i = 1; i < 20; i++)
            //    {
            //        if (i < 9 || i == 19)
            //        {
            //            for (int j = 0; j < orbitalSampleRate; j++)
            //            {

            //                orbits[i, j] = new Vector3d(br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
            //            }
            //        }
            //    }
            //}
            //catch
            //{
            //    orbits = null;
            //    return false;
            //}
            //finally
            //{
            //    if (br != null)
            //    {
            //        br.Close();
            //    }
            //    if (fs != null)
            //    {
            //        fs.Close();
            //    }
            //}
            return true;
        }


        public static void DumpOrbitsFile()
        {
            //string filename = Properties.Settings.Default.CahceDirectory + @"data\orbits.bin";

            //if (orbits != null)
            //{
            //    FileStream fs = new FileStream(filename, FileMode.Create);
            //    BinaryWriter bw = new BinaryWriter(fs);
            //    for (int i = 1; i < 20; i++)
            //    {
            //        if (i < 9 || i == 19)
            //        {
            //            for (int j = 0; j < orbitalSampleRate; j++)
            //            {
            //                bw.Write(orbits[i, j].X);
            //                bw.Write(orbits[i, j].Y);
            //                bw.Write(orbits[i, j].Z);
            //            }
            //        }
            //    }
            //    bw.Close();
            //    fs.Close();
            //}
        }

        public static bool DrawPlanets(RenderContext renderContext, float opacity)
        {

            if (planetTextures == null)
            {
                LoadPlanetTextures();
            }


            // Get Moon Phase
            double elong = GeocentricElongation(planetLocations[9].RA, planetLocations[9].Dec, planetLocations[0].RA, planetLocations[0].Dec);
            double raDif = planetLocations[9].RA - planetLocations[0].RA;
            if (planetLocations[9].RA < planetLocations[0].RA)
            {
                raDif += 24;
            }
            double phaseAngle = PhaseAngle(elong, planetLocations[9].Distance, planetLocations[0].Distance);
            double limbAngle = PositionAngle(planetLocations[9].RA, planetLocations[9].Dec, planetLocations[0].RA, planetLocations[0].Dec);

            if (raDif < 12)
            {
                phaseAngle += 180;
            }

            // Check for solar eclipse

            double dista = (Math.Abs(planetLocations[9].RA - planetLocations[0].RA) * 15) * Math.Cos(Coordinates.DegreesToRadians(planetLocations[0].Dec));
            double distb = Math.Abs(planetLocations[9].Dec - planetLocations[0].Dec);
            double sunMoonDist = Math.Sqrt(dista * dista + distb * distb);

            bool eclipse = false;
            double coronaOpacity = 0;


            //if (sunMoonDist < .014 && planetScales[9] > planetScales[0])

            double moonEffect = (planetScales[9] / 2 - sunMoonDist);

            int darkLimb = Math.Min(32, (int)(sunMoonDist * 32));

            if (moonEffect > (planetScales[0] / 4))
            {

                eclipse = true;
                //coronaOpacity = Math.Min(1.0, Math.Abs((sunMoonDist-.014)/.007));
                coronaOpacity = Math.Min(1.0, (moonEffect - (planetScales[0] / 2)) / .001);
                DrawPlanet(renderContext, 18, coronaOpacity);

            }

            //            Earth3d.MainWindow.Text = phaseAngle.ToString() + ", " + raDif.ToString();

//            for (int i = 0; i < planetDrawOrder.Count; i++)

            foreach( double key in planetDrawOrder.Keys)
            {
                int planetId = planetDrawOrder[key];

                //if (planetId == 9)
                //{
                //    DrawPlanetPhase(canvas, planetId, phaseAngle, limbAngle, darkLimb);
                //}
                //else
                {
                    DrawPlanet(renderContext, planetId, 1);
                }

            }
            

            return true;
        }

        private static void LoadPlanetTextures()
        {
            string baseUrl = "http://cdn.worldwidetelescope.org/webclient/images/";

            planetTextures = new Texture[20];
            planetTextures[0] = LoadPlanetTexture(baseUrl + @"sun.png");
            planetTextures[1] = LoadPlanetTexture(baseUrl + @"mercury.png");
            planetTextures[2] = LoadPlanetTexture(baseUrl + @"venus.png");
            planetTextures[3] = LoadPlanetTexture(baseUrl + @"mars.png");
            planetTextures[4] = LoadPlanetTexture(baseUrl + @"jupiter.png");
            planetTextures[5] = LoadPlanetTexture(baseUrl + @"saturn.png");
            planetTextures[6] = LoadPlanetTexture(baseUrl + @"uranus.png");
            planetTextures[7] = LoadPlanetTexture(baseUrl + @"neptune.png");
            planetTextures[8] = LoadPlanetTexture(baseUrl + @"pluto.png");
            planetTextures[9] = LoadPlanetTexture(baseUrl + @"moon.png");
            planetTextures[10] = LoadPlanetTexture(baseUrl + @"io.png");
            planetTextures[11] = LoadPlanetTexture(baseUrl + @"europa.png");
            planetTextures[12] = LoadPlanetTexture(baseUrl + @"ganymede.png");
            planetTextures[13] = LoadPlanetTexture(baseUrl + @"callisto.png");
            planetTextures[14] = LoadPlanetTexture(baseUrl + @"moonshadow.png"); //todo fix these to shadows
            planetTextures[15] = LoadPlanetTexture(baseUrl + @"moonshadow.png");
            planetTextures[16] = LoadPlanetTexture(baseUrl + @"moonshadow.png");
            planetTextures[17] = LoadPlanetTexture(baseUrl + @"moonshadow.png");
            planetTextures[18] = LoadPlanetTexture(baseUrl + @"sunCorona.png");
            planetTextures[19] = LoadPlanetTexture(baseUrl + @"earth.png");
        }


        static Dictionary<double, int> drawOrder = new Dictionary<double,int>();
        public static bool DrawPlanets3D(RenderContext renderContext, float opacity, Vector3d centerPoint)
        {

            InitPlanetResources(renderContext);

            double distss = UiTools.SolarSystemToMeters(renderContext.SolarSystemCameraDistance);

            float moonFade = (float)Math.Min(1, Math.Max(Util.Log10(distss) - 7.3, 0));


            float fade = (float)Math.Min(1, Math.Max(Util.Log10(distss) - 8.6, 0));

            if (Settings.Active.SolarSystemOrbits && fade > 0)
            {


                for (int ii = 1; ii < 10; ii++)
                {
                    int id = ii;

                    if (ii == 9)
                    {
                        id = 19;
                    }

                    double angle = Math.Atan2(planet3dLocations[id].Z, planet3dLocations[id].X);

                    DrawSingleOrbit(renderContext, planetColors[id], id, centerPoint, angle, planet3dLocations[id], fade);
                }

            }

            drawOrder.Clear();

            Vector3d camera = renderContext.CameraPosition.Copy();
            for (int planetId = 0; planetId < 14; planetId++)
            {
                if (!planetLocations[planetId].Eclipsed)
                {
                    Vector3d distVector = Vector3d.SubtractVectors(camera,Vector3d.SubtractVectors(planet3dLocations[planetId],centerPoint));

                    if (!drawOrder.ContainsKey(distVector.Length()))
                    {
                        drawOrder[distVector.Length()] = planetId;
                    }
                }
            }

            Vector3d distVectorEarth = Vector3d.SubtractVectors(camera, Vector3d.SubtractVectors(planet3dLocations[(int)SolarSystemObjects.Earth], centerPoint));
            if (!drawOrder.ContainsKey(distVectorEarth.Length()))
            {
                drawOrder[distVectorEarth.Length()] = (int)SolarSystemObjects.Earth;
            }




            //for (int planetId = 0; planetId < 14; planetId++)
            //{
            //    if (!planetLocations[planetId].Eclipsed)
            //    {
            //        DrawPlanet3d( renderContext, planetId, centerPoint);
            //    }
            //}
         //   DrawPlanet3d(renderContext, (int)SolarSystemObjects.Earth, centerPoint);

            foreach (double key in drawOrder.Keys)
            {
                int planetId = drawOrder[key];
                DrawPlanet3d(renderContext, planetId, centerPoint);
            }

            return true;
        }

        public static void InitPlanetResources(RenderContext renderContext)
        {
//            OrbitsMutex.WaitOne();
//            try
//            {
//                if (planetTexturesMaps == null)
//                {
//                    LoadPlanetTextureMaps(device);
//                }
//                if (sphereIndexBuffers == null)
//                {
//                    InitSphere(device);
//                }

//                if (ringsVertexBuffer == null)
//                {
//                    InitRings(device);
//                }
//            }
//            finally
//            {
//                OrbitsMutex.ReleaseMutex();
//            }
       }

        private static void DrawSingleOrbit(RenderContext renderContext, Color eclipticColor, int id, Vector3d centerPoint, double startAngle, Vector3d planetNow, float opacity)
        {
            if (opacity < .01)
            {
                return;
            }

            if (renderContext.gl == null)
            {
                int count = orbitalSampleRate;
                bool planetDropped = false;

                Vector3d viewPoint = renderContext.ViewPoint;


                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();

                ctx.StrokeStyle = eclipticColor.ToString();
                ctx.LineWidth = 2;
                ctx.Alpha = 1;
                Vector3d point = new Vector3d();
                Vector3d pointTest = new Vector3d();

                Vector3d lastPoint = new Vector3d();
                bool firstPoint = true;
                Matrix3d translate = Matrix3d.Translation(Vector3d.Negate(centerPoint));
                Matrix3d mat = Matrix3d.MultiplyMatrix(translate, renderContext.WVP);
                Matrix3d matWV = Matrix3d.MultiplyMatrix(translate, renderContext.WV);

                for (int i = 0; i < count; i++)
                {
                    Vector3d pnt = orbits[id][i];


                    double angle = (Math.Atan2(orbits[id][i].Z, orbits[id][i].X) + Math.PI * 2 - startAngle) % (Math.PI * 2);
                    int alpha = (int)((angle) / (Math.PI * 2) * 255);

                    double alphaD = (double)alpha / 255.0;

                    if (alpha < 2 && !planetDropped)
                    {
                        pnt = planetNow;
                        alphaD = 1.0;

                    }

                    pointTest = matWV.Transform(pnt);
                    point = mat.Transform(pnt);

                    if (pointTest.Z > 0)
                    {


                        if (firstPoint)
                        {

                            firstPoint = false;
                        }
                        else
                        {
                            // if (Vector3d.Dot(pnt, viewPoint) > .60)
                            {
                                ctx.BeginPath();
                                ctx.Alpha = alphaD * opacity;
                                ctx.MoveTo(lastPoint.X, lastPoint.Y);
                                ctx.LineTo(point.X, point.Y);
                                ctx.Stroke();
                            }
                        }
                    }

                    lastPoint = point;
                }


                ctx.Restore();
            }
            else
            {
                int count = orbitalSampleRate;
                bool planetDropped = false;

                Vector3d viewPoint = renderContext.ViewPoint;
                Vector3d point = new Vector3d();
                Vector3d pointTest = new Vector3d();

                Vector3d lastPoint = new Vector3d();
                Color lastColor = new Color();
                bool firstPoint = true;
                OrbitLineList list = new OrbitLineList();

                for (int i = 0; i < count; i++)
                {
                    Vector3d pnt = orbits[id][i].Copy();

                    double angle = (Math.Atan2(pnt.Z, pnt.X) + Math.PI * 2 - startAngle) % (Math.PI * 2);
                    int alpha = (int)((angle) / (Math.PI * 2) * 255);

                    double alphaD = (double)alpha / 255.0;
                    Color color = Color.FromArgb(alpha, eclipticColor.R, eclipticColor.G, eclipticColor.B);

                    if (alpha < 2 && !planetDropped && !firstPoint)
                    {
                        pnt = Vector3d.SubtractVectors(planetNow, centerPoint);
                        alphaD = 1.0;
                        alpha = 255;

                        color.A = 255;
                        lastColor.A = 255;
                        list.AddLine(lastPoint, pnt.Copy(), lastColor.Clone(), color.Clone());
                        lastColor.A = 0;
                        color.A = 0;
                        pnt = orbits[id][i].Copy();
                        planetDropped = true;
                    }
                  
                    pnt = Vector3d.SubtractVectors(pnt, centerPoint);
                 
  
                    if (firstPoint)
                    {

                        firstPoint = false;
                    }
                    else
                    {
                       list.AddLine(lastPoint, pnt, lastColor, color);
                    }
                    lastPoint = pnt;
                    lastColor = color.Clone();
                }
                list.DrawLines(renderContext, 1.0f, Colors.White);
                list.Clear();
            }
        }


        static Date lastUpdate = new Date();
        const double EarthDiameter = 0.000137224;
//        //private static void DrawPlanet2pnt5d(Device device, int planetID)
//        //{

//        //    Vector3 planetPosition = planet3dLocations[planetID];

//        //    if (planetID < 9 )
//        //    {
//        //        device.SetTexture(0, planetTextures[planetID]);
//        //    }
//        //    else if (planetID == 9 )
//        //    {
//        //        device.SetTexture(0, planetTextures[19]);

//        //    }
//        //    else
//        //    {
//        //        return;
//        //    }

//        //    double diameter = planetDiameters[planetID];

//        //    float radius = (double)(diameter / 2.0 );
//        //    if (!Settings.Active.ActualPlanetScale)
//        //    {
//        //        if (planetID != 0)
//        //        {
//        //            radius *= 1000;
//        //        }
//        //        else
//        //        {
//        //            radius *= 100;
//        //        }
//        //    }

//        //    CustomVertex.PositionTextured[] points = new CustomVertex.PositionTextured[4];

//        //    Vector3 pos = planet3dLocations[planetID];
//        //    points[0].Position = new Vector3(pos.X + radius, pos.Y, pos.Z + radius);

//        //    points[0].Tu = 0;
//        //    points[0].Tv = 0;

//        //    points[1].Position = new Vector3(pos.X - radius, pos.Y, pos.Z + radius);
//        //    points[1].Tu = 1;
//        //    points[1].Tv = 0;

//        //    points[2].Position = new Vector3(pos.X + radius, pos.Y, pos.Z - radius);
//        //    points[2].Tu = 0;
//        //    points[2].Tv = 1;

//        //    points[3].Position = new Vector3(pos.X - radius, pos.Y, pos.Z - radius);
//        //    points[3].Tu = 1;
//        //    points[3].Tv = 1;


//        //    //Matrix mat = Microsoft.DirectX.Matrix.RotationZ(rotationAngle);

//        //    device.DrawUserPrimitives(PrimitiveType.TriangleStrip, points.Length - 2, points);
//        //    // Render Stuff Here


//        //}        public static bool Lighting = true;
//        //private static void DrawPlanet3d(Device device, int planetID, Vector3d centerPoint)
//        //{

//        //    if (planetID == (int)SolarSystemObjects.Sun)
//        //    {
//        //        device.RenderState.Lighting = false;
//        //    }
//        //    else
//        //    {
//        //        device.RenderState.Lighting = Lighting;
//        //    }


//        //    double radius = GetAdjustedPlanetRadius(planetID);
//        //    double rotationCurrent = 0;
//        //    if (planetID == (int)SolarSystemObjects.Earth)
//        //    {
//        //        rotationCurrent = Coordinates.MstFromUTC2(SpaceTimeController.Now, 0)/180.0 * Math.PI;
//        //    }
//        //    else
//        //    {
//        //        rotationCurrent = (((jNow - 2451545.0) / planetRotationPeriod[planetID]) * Math.PI * 2) % (Math.PI * 2);
//        //    }
//        //    Matrix matOld = device.Transform.World;

//        //    matOld = device.Transform.World;
//        //    Matrix matLocal = device.Transform.World;
//        //    Vector3d translation = planet3dLocations[planetID]-centerPoint;

//        //    matLocal.Scale((double)radius, (double)radius, (double)radius);
//        //    matLocal.Multiply(Matrix.RotationY((double)-rotationCurrent));
//        //    matLocal.Multiply(Matrix.RotationX((double)(planetTilts[planetID]*RC)));
//        //    matLocal.Multiply(Matrix.Translation(translation.Vector3));
//        //    device.Transform.World = matLocal;
//        //    Earth3d.MainWindow.MakeFrustum();
//        //    DrawSphere(device, planetTexturesMaps[planetID], GetNameFrom3dId(planetID));
//        //    if (planetID == 5)
//        //    {
//        //        device.RenderState.CullMode = Cull.None;
//        //        Color oldAmbient = device.RenderState.Ambient;
//        //        device.RenderState.Ambient = Color.FromArgb(40, 40, 40);
//        //        DrawRings(device);
//        //        device.RenderState.Ambient = oldAmbient;
//        //        device.RenderState.CullMode = Cull.Clockwise;

//        //    }
//        //    device.Transform.World = matOld;
//        //    Earth3d.MainWindow.MakeFrustum();
//        //    device.RenderState.Lighting = false;
//        //}
//        public static bool Lighting = true;
        public static bool IsPlanetInFrustum(RenderContext renderContext, float rad)
        {
            PlaneD[] frustum = renderContext.Frustum;
            Vector3d center =  Vector3d.Create(0, 0, 0);
            Vector4d centerV4 = new Vector4d(0, 0, 0, 1f);
            for (int i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(centerV4) + rad < 0)
                {
                    return false;
                }
            }
            return true;
        }
//        private static Matrix bias = Matrix.Scaling(new Vector3(.5f, .5f, .5f)) * Matrix.Translation(new Vector3(.5f, .5f, .5f));
        private static void DrawPlanet3d(RenderContext renderContext, int planetID, Vector3d centerPoint)
        {
            
            if (planetID == (int)SolarSystemObjects.Sun)
            {
            //    device.RenderState.Lighting = false;
            }
            else
            {
             //   device.RenderState.Lighting = Settings.Active.SolarSystemLighting;
            }

            double radius = GetAdjustedPlanetRadius(planetID);



            double rotationCurrent = 0;
            if (planetID == (int)SolarSystemObjects.Earth)
            {
                rotationCurrent = Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180.0 * Math.PI;
            }
            else
            {
                rotationCurrent = (((jNow - 2451545.0) / planetRotationPeriod[planetID]) * Math.PI * 2) % (Math.PI * 2);
            }

            //Matrix3d matOldWV = renderContext.WV.Clone();

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldBase = renderContext.WorldBase;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;

            Matrix3d matLocal = renderContext.World.Clone();
            Matrix3d matLocalNR = renderContext.World.Clone();

            Vector3d translation = Vector3d.SubtractVectors(planet3dLocations[planetID], centerPoint);
            
            matLocal.Scale(Vector3d.Create(radius, radius, radius));
            matLocal.Multiply(Matrix3d.RotationY((double)-rotationCurrent));
            matLocal.Multiply(Matrix3d.RotationX((double)(planetTilts[planetID] * RC)));
            matLocal.Multiply(Matrix3d.Translation(translation));
       
            
            matLocalNR.Scale(Vector3d.Create(radius, radius, radius));
            matLocalNR.Multiply(Matrix3d.RotationX((double)(planetTilts[planetID] * RC)));
            matLocalNR.Multiply(Matrix3d.Translation(translation));
             
            renderContext.World = matLocal;
            renderContext.WorldBase = matLocal.Clone();
            renderContext.WorldBaseNonRotating = matLocalNR;
            renderContext.MakeFrustum();

            float planetWidth = 1;

            if (planetID == (int)SolarSystemObjects.Saturn)
            {
                planetWidth = 3;
            }

            if (IsPlanetInFrustum(renderContext,planetWidth))
            {
                //Matrix3d matOld2 = renderContext.World.Clone();
                //Matrix3d matOldBase2 = renderContext.WorldBase.Clone();
                //Matrix3d matOldNonRotating2 = renderContext.WorldBaseNonRotating;

                Vector3d sun = planet3dLocations[0].Copy();
                Vector3d planet = planet3dLocations[planetID].Copy();

                sun = matOld.Transform(sun);
                planet = matOld.Transform(planet);
               

                Vector3d sunPosition = Vector3d.SubtractVectors(sun, planet);
                sunPosition.Normalize();

                renderContext.SunPosition = sunPosition;

                Vector3d loc = Vector3d.SubtractVectors(planet3dLocations[planetID], centerPoint);
                loc.Subtract(renderContext.CameraPosition);
                double dist = loc.Length();
                double sizeIndexParam = (2 * Math.Atan(.5 * (radius / dist))) / Math.PI * 180;



                int sizeIndex = 0;
                if (sizeIndexParam > 10.5)
                {
                    sizeIndex = 0;
                }
                else if (sizeIndexParam > 3.9)
                {
                    sizeIndex = 1;
                }
                else if (sizeIndexParam > .72)
                {
                    sizeIndex = 2;
                }
                else if (sizeIndexParam > 0.05)
                {
                    sizeIndex = 3;
                }
                else
                {
                    sizeIndex = 4;
                }


                //ShadowStuff
                if (planetID == (int)SolarSystemObjects.Earth && sizeIndex < 2)
                {
                    float width = Settings.Active.SolarSystemScale * .00001f;
                  //todo add shadows  centerPoint = SetupShadow(device, centerPoint, width, SolarSystemObjects.Moon);
                }
                //ShadowStuff end

                if (sizeIndex < 3)
                {
                    bool oldLighting = renderContext.Lighting;
                    //double planetDistance = Vector3d.SubtractVectors(planet3dLocations[planetID], renderContext.CameraPosition).Length();
                    if (planetID == 5)
                    {
                        if (renderContext.gl == null)
                        {
                            renderContext.Lighting = false;
                            // DRAW BACK HALF OF RINGS
                            DrawSaturnsRings(renderContext, false, dist);
                            renderContext.Lighting = oldLighting;
                            //if (Settings.Active.SolarSystemLighting)
                            //{
                            //    SetupRingShadow(device, centerPoint, SolarSystemObjects.Saturn, rotationCurrent);
                            //}
                            // todo saturns rings DrawRings(device);
                        }
                        
                    }

                    if (planetID == 0)
                    {
                        renderContext.Lighting = false;
                    }

                    DrawSphere(renderContext, planetID);

                    if (planetID == 5)
                    {
                        if (renderContext.gl == null)
                        {
                            renderContext.Lighting = false;
                            DrawSaturnsRings(renderContext, true, dist);
                            // DRAW FRONT HALF OF RINGS 
                            //if (Settings.Active.SolarSystemLighting)
                            //{
                            //    SetupRingShadow(device, centerPoint, SolarSystemObjects.Saturn, rotationCurrent);
                            //}
                            // todo saturns rings DrawRings(device);
                        }
                        else
                        {
                            DrawRings(renderContext);
                        }
                    }

                    renderContext.Lighting = oldLighting;

                }
                else
                {
                    if (planetID == 0)
                    {
                        DrawPointPlanet(renderContext, new Vector3d(), (double)(10 * planetDiameters[planetID]), planetColors[planetID], true);
                    }
                    else if (planetID < (int)SolarSystemObjects.Moon || planetID == (int)SolarSystemObjects.Earth)
                    {
                        double size = (800 * planetDiameters[planetID]);
                        DrawPointPlanet(renderContext, new Vector3d(), (double)Math.Max(.05, Math.Min(.1f, size)), planetColors[planetID], true);
                    }
                    else if (sizeIndexParam > .002)
                    {
                        double size = (800 * planetDiameters[planetID]);
                        DrawPointPlanet(renderContext, new Vector3d(), (double)Math.Max(.05, Math.Min(.1f, size)), planetColors[planetID], true);
                    }
                }
                //renderContext.World = matOld2;
                //renderContext.WorldBase = matOldBase2;
                //renderContext.WorldBaseNonRotating = matOldNonRotating2;
            }

            LayerManager.Draw(renderContext, 1.0f, false, GetNameFrom3dId(planetID), true, false);

            renderContext.World = matOld;
            renderContext.WorldBase = matOldBase;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }

        internal static List<RenderTriangle>[] RingsTriangleLists = new List<RenderTriangle>[2];
        internal static ImageElement ringImage = null;
        public static void DrawSaturnsRings(RenderContext renderContext, bool front, double distance)
        {
            if (RingsTriangleLists[0] == null)
            {
                ringImage =  (ImageElement)Document.CreateElement("img");
                CrossDomainImage xdomimg = (CrossDomainImage)(object)ringImage;

                //texture.AddEventListener("load", delegate(ElementEvent e)
                //{
                //    texReady = true;
                //    Downloading = false;
                //    errored = false;
                //    ReadyToRender = texReady && (DemReady || !demTile);
                //    RequestPending = false;
                //    TileCache.RemoveFromQueue(this.Key, true);
                //    MakeTexture();
                //}, false);

                //texture.AddEventListener("error", delegate(ElementEvent e)
                //{
                //    Downloading = false;
                //    ReadyToRender = false;
                //    errored = true;
                //    RequestPending = false;
                //    TileCache.RemoveFromQueue(this.Key, true);
                //}, false);

                xdomimg.crossOrigin = "anonymous";
                ringImage.Src = "/webclient/images/saturnringsshadow.png";

                RingsTriangleLists[0] = new List<RenderTriangle>();
                RingsTriangleLists[1] = new List<RenderTriangle>();

                double ringSize = 2.25;

                Vector3d TopLeft = Vector3d.Create(-ringSize, 0, -ringSize);
                Vector3d TopRight = Vector3d.Create(ringSize, 0, -ringSize);
                Vector3d BottomLeft = Vector3d.Create(-ringSize, 0, ringSize);
                Vector3d BottomRight = Vector3d.Create(ringSize, 0, ringSize);
                Vector3d center = Vector3d.Create(0, 0, 0);
                Vector3d leftCenter = Vector3d.Create(-ringSize, 0, 0);
                Vector3d topCenter = Vector3d.Create(0, 0, -ringSize);
                Vector3d bottomCenter = Vector3d.Create(0, 0, ringSize);
                Vector3d rightCenter = Vector3d.Create(ringSize, 0, 0);

                int level = 6;
                //RingsTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(TopLeft, 0, 0, 1024, 1024), PositionTexture.CreatePosSize(leftCenter, 0, .5, 1024, 1024), PositionTexture.CreatePosSize(topCenter, .5, 0, 1024, 1024), ringImage, level));
                //RingsTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(leftCenter, 0, 0.5, 1024, 1024), PositionTexture.CreatePosSize(center, .5, .5, 1024, 1024), PositionTexture.CreatePosSize(topCenter, .5, 0, 1024, 1024), ringImage, level));
                //RingsTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(topCenter, .5, 0, 1024, 1024), PositionTexture.CreatePosSize(rightCenter, 1, .5, 1024, 1024), PositionTexture.CreatePosSize(TopRight, 1, 0, 1024, 1024), ringImage, level));
                //RingsTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(topCenter, .5, 0, 1024, 1024), PositionTexture.CreatePosSize(center, .5, .5, 1024, 1024), PositionTexture.CreatePosSize(rightCenter, 1, .5, 1024, 1024), ringImage, level));
                //RingsTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(leftCenter, 0, .5, 1024, 1024), PositionTexture.CreatePosSize(bottomCenter, .5, 1, 1024, 1024), PositionTexture.CreatePosSize(center, .5, .5, 1024, 1024), ringImage, level));
                //RingsTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(leftCenter, 0, .5, 1024, 1024), PositionTexture.CreatePosSize(BottomLeft, 0, 1, 1024, 1024), PositionTexture.CreatePosSize(bottomCenter, .5, 1, 1024, 1024), ringImage, level));
                //RingsTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(center, .5, .5, 1024, 1024), PositionTexture.CreatePosSize(BottomRight, 1, 1, 1024, 1024), PositionTexture.CreatePosSize(rightCenter, 1, .5, 1024, 1024), ringImage, level));
                //RingsTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePosSize(center, .5, .5, 1024, 1024), PositionTexture.CreatePosSize(bottomCenter, .5, 1, 1024, 1024), PositionTexture.CreatePosSize(BottomRight, 1, 1, 1024, 1024), ringImage, level));

                List<PositionTexture> vertexList;
                vertexList = new List<PositionTexture>();

                int Width = 1024;
                int Height = 1024;

                vertexList.Add(PositionTexture.CreatePosSize(TopLeft, 0, 0, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(TopRight, 1, 0, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(BottomLeft, 0, 1, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(BottomRight, 1, 1, Width, Height));

                List<Triangle> childTriangleList = new List<Triangle>();

                //if (dataset.BottomsUp)
                //{
                //    childTriangleList.Add(Triangle.Create(0, 1, 2));
                //    childTriangleList.Add(Triangle.Create(2, 1, 3));
                //}
                //else
                {
                    childTriangleList.Add(Triangle.Create(0, 2, 1));
                    childTriangleList.Add(Triangle.Create(2, 3, 1));

                }

                int count = 5;
                while (count-- > 1)
                {
                    List<Triangle> newList = new List<Triangle>();
                    foreach (Triangle tri in childTriangleList)
                    {
                        tri.SubDivideNoNormalize(newList, vertexList);
                    }
                    childTriangleList = newList;
                }

                double miter = .6 / (Width / 256);
                foreach (Triangle tri in childTriangleList)
                {
                    PositionTexture p1 = vertexList[tri.A];
                    PositionTexture p2 = vertexList[tri.B];
                    PositionTexture p3 = vertexList[tri.C];


                    RingsTriangleLists[0].Add(RenderTriangle.CreateWithMiter(p1, p2, p3, ringImage, level, miter));
                }

            }

            if (renderContext.gl == null)
            {
                Vector3d cam = renderContext.CameraPosition;
                Vector3d test = new Vector3d();

                //Matrix3d wv = renderContext.WV;

                Matrix3d worldLocal = Matrix3d.MultiplyMatrix(Matrix3d.RotationY(Math.Atan2(renderContext.SunPosition.X, renderContext.SunPosition.Z)), renderContext.WorldBaseNonRotating);

                Matrix3d wv = Matrix3d.MultiplyMatrix(worldLocal, renderContext.View);
                Matrix3d wvp = Matrix3d.MultiplyMatrix(wv, renderContext.Projection);
                double Width = renderContext.Width;
                double Height = renderContext.Height;

                wvp.Scale(Vector3d.Create(Width / 2, -Height / 2, 1));
                wvp.Translate(Vector3d.Create(Width / 2, Height / 2, 0));
                double td = 0;
               // RenderTriangle.CullInside = !RenderTriangle.CullInside;
                for (int i = 0; i < 2; i++)
                {
                    foreach (RenderTriangle tri in RingsTriangleLists[0])
                    {
                        //test = Vector3d.SubtractVectors(wv.Transform(tri.A.Position), cam);
                        test = wv.Transform(tri.A.Position);
                        td = test.Length();

                        bool draw = td > distance;

                        if (front)
                        {
                            draw = !draw;
                        }

                        if (draw)
                        {
                            tri.Opacity = 1;

                            tri.Draw(renderContext.Device, wvp);
                        }
                    }
                    RenderTriangle.CullInside = !RenderTriangle.CullInside;
                }
            }
            else
            {
                //todo port rings to web gl
                //renderContext.gl.enableVertexAttribArray(renderContext.vertLoc);
                //renderContext.gl.enableVertexAttribArray(renderContext.textureLoc);
                //renderContext.gl.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
                //renderContext.gl.vertexAttribPointer(renderContext.vertLoc, 3, GL.FLOAT, false, 20, 0);
                ////renderContext.gl.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
                //renderContext.gl.vertexAttribPointer(renderContext.textureLoc, 2, GL.FLOAT, false, 20, 12);
                //renderContext.gl.activeTexture(GL.TEXTURE0);
                //renderContext.gl.bindTexture(GL.TEXTURE_2D, texture2d);

                ////if (tileX == TileTargetX && tileY == TileTargetY && Level == TileTargetLevel)
                ////{
                ////    renderContext.gl.bindTexture(GL.TEXTURE_2D, null);
                ////}

                //renderContext.gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, GetIndexBuffer(part, accomidation));
                //renderContext.gl.drawElements(GL.TRIANGLES, TriangleCount * 3, GL.UNSIGNED_SHORT, 0);
            }
        }
        const int subDivisionsRings = 192;

        static int triangleCountRings = subDivisionsRings +1 * 2;
        static PositionTextureVertexBuffer ringsVertexBuffer = null;
        static Texture ringsTexture;

        // Various input layouts used in 3D solar system mode
        // TODO Replace with an input layout cache

        static void DrawRings(RenderContext renderContext)
        {
            InitRings();

            TileShader.Use(renderContext, ringsVertexBuffer.VertexBuffer, null, ringsTexture.Texture2d, 1.0f, false);
            renderContext.gl.drawArrays(GL.TRIANGLE_STRIP, 0, triangleCountRings);
        }



        static void InitRings()
        {
            if (ringsVertexBuffer != null)
            {
                return;
            }
            ringsTexture = Planets.LoadPlanetTexture("http://cdn.worldwidetelescope.org/webclient/images/SaturnRingsStrip.png");
            double inner = 1.113;
            double outer = 2.25;

            ringsVertexBuffer = new PositionTextureVertexBuffer(((subDivisionsRings + 1) * 2));

            triangleCountRings = (subDivisionsRings+1) * 2;
            PositionTexture[] verts = (PositionTexture[])ringsVertexBuffer.Lock(); // Lock the buffer (which will return our structs)

            double radStep = Math.PI * 2.0 / (double)subDivisionsRings;
            int index = 0;
            for (int x = 0; x <= subDivisionsRings; x += 2)
            {
                double rads1 = x * radStep;
                double rads2 = (x + 1) * radStep;
                verts[index] = new PositionTexture();
                verts[index].Position = Vector3d.Create((Math.Cos(rads1) * inner), 0, (Math.Sin(rads1) * inner));
                verts[index].Tu = 1;
                verts[index].Tv = 0;
                index++;
                verts[index] = new PositionTexture();
                verts[index].Position = Vector3d.Create((Math.Cos(rads1) * outer), 0, (Math.Sin(rads1) * outer));
                verts[index].Tu = 0;
                verts[index].Tv = 0;
                index++;
                verts[index] = new PositionTexture();
                verts[index].Position = Vector3d.Create((Math.Cos(rads2) * inner), 0, (Math.Sin(rads2) * inner));
                verts[index].Tu = 1;
                verts[index].Tv = 1;
                index++;
                verts[index] = new PositionTexture();
                verts[index].Position = Vector3d.Create((Math.Cos(rads2) * outer), 0, (Math.Sin(rads2) * outer));
                verts[index].Tu = 0;
                verts[index].Tv = 1;
                index++;

            }
            ringsVertexBuffer.Unlock();
        }
    

        public static void DrawPointPlanet(RenderContext renderContext, Vector3d location, double size, Color color, bool zOrder)
        {
            size = Math.Max(2, size);

            Vector3d center = (Vector3d)location;

            double rad = size / 2;


            if (renderContext.gl != null)
            {
                PointList ppList = new PointList(renderContext);
                ppList.MinSize = 20;
                ppList.AddPoint(location.Copy(), color.Clone(), new Dates(0, 1), (float)size*10);
                // ppList.ShowFarSide = true;
                ppList.DepthBuffered = false;
                ppList.Draw(renderContext, 1, false);
                //  ppList.Clear();

            }
            else
            {
                Vector3d screenSpacePnt = renderContext.WVP.Transform(center);
                if (screenSpacePnt.Z < 0)
                {
                    return;
                }
                if (!zOrder)
                {
                    if (Vector3d.Dot((Vector3d)renderContext.ViewPoint, (Vector3d)center) < .55)
                    {
                        return;
                    }
                }
                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();
                //ctx.Alpha = opacity;
                ctx.BeginPath();
                ctx.Arc(screenSpacePnt.X, screenSpacePnt.Y, rad, 0, Math.PI * 2, true);
                ctx.LineWidth = 1;
                ctx.FillStyle = color.ToString();
                if (true)
                {
                    ctx.Fill();
                }
                ctx.Alpha = 1.0;
                ctx.StrokeStyle = color.ToString();
                ctx.Stroke();

                ctx.Restore();
            }
            //device.RenderState.Lighting = false;
            //StarVertex[] vert = new StarVertex[1];
            //vert[0] = new StarVertex(location.Vector3, size, color.ToArgb());
            //device.RenderState.PointSpriteEnable = true;
            //device.RenderState.PointScaleEnable = true;
            //device.RenderState.PointScaleA = 100;
            //device.RenderState.PointScaleB = 0;
            //device.RenderState.PointScaleC = 0;
            //device.RenderState.ZBufferEnable = zOrder;
            //device.SetTexture(0, Grids.StarProfile);
            ////device.SetTexture(0, null);

            //device.VertexFormat = VertexFormats.Position | VertexFormats.PointSize | VertexFormats.Diffuse;

            ////          device.RenderState.CullMode = Cull.None;
            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
            //device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;

            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
            //device.TextureState[0].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
            //device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

            //device.TextureState[1].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[1].ColorArgument1 = TextureArgument.Current;
            //device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
            //device.TextureState[1].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
            //device.TextureState[1].AlphaArgument2 = TextureArgument.Constant;

            //device.TextureState[1].ConstantColor = Color.FromArgb(255, 255, 255, 255);

            //device.DrawUserPrimitives(PrimitiveType.PointList, 1, vert);

            //device.RenderState.PointSpriteEnable = false;
            //device.RenderState.PointScaleEnable = false;
        }

//        private static Vector3d SetupShadow(Device device, Vector3d centerPoint, float width, SolarSystemObjects shadowCaster)
//        {
//            if (PlanetShadow == null)
//            {
//                PlanetShadow = LoadPlanetTexture(device, Properties.Resources.planetShadow);
//            }

//            // device.SetTexture(2, PlanetShadow);
//            device.SetTexture(2, PlanetShadow);
//            device.SamplerState[2].AddressU = TextureAddress.Clamp;
//            device.SamplerState[2].AddressV = TextureAddress.Clamp;
//            device.TextureState[2].ColorOperation = TextureOperation.Modulate;
//            device.TextureState[2].ColorArgument1 = TextureArgument.TextureColor;
//            device.TextureState[2].ColorArgument2 = TextureArgument.Current;
//            device.TextureState[2].AlphaOperation = TextureOperation.Disable;
//            device.TextureState[2].AlphaArgument1 = TextureArgument.Current;
//            device.TextureState[2].AlphaArgument2 = TextureArgument.Current;
//            device.TextureState[2].TextureTransform = TextureTransform.Count3 | TextureTransform.Projected;
//            device.TextureState[2].TextureCoordinateIndex = (int)TextureCoordinateIndex.CameraSpacePosition;
//            Matrix invViewCam = device.Transform.View;
//            invViewCam.Invert();

//            Vector3d sun = planet3dLocations[0];
//            sun.Subtract(centerPoint);

//            Vector3d moon = planet3dLocations[(int)shadowCaster];
//            moon.Subtract(centerPoint);
//            Matrix mat =
//                invViewCam *
//                Matrix.LookAtLH(sun.Vector3, moon.Vector3, new Vector3(0, 1, 0)) *
//                Matrix.PerspectiveFovLH(width, 1, .001f, 200f) *
//                bias;

//            device.Transform.Texture2 = mat;
//            return centerPoint;
//        }
//        private static Texture ringShadow;
//        private static Vector3d SetupRingShadow(Device device, Vector3d centerPoint, SolarSystemObjects shadowCaster, double rotation)
//        {
//            if (ringShadow == null)
//            {
//                ringShadow = LoadPlanetTexture(device, Properties.Resources.ringShadow);
//            }

//            // device.SetTexture(2, PlanetShadow);
//            device.SetTexture(2, ringShadow);
//            device.SamplerState[2].AddressU = TextureAddress.Wrap;
//            device.SamplerState[2].AddressV = TextureAddress.Wrap;
//            device.TextureState[2].ColorOperation = TextureOperation.Modulate;
//            device.TextureState[2].ColorArgument1 = TextureArgument.TextureColor;
//            device.TextureState[2].ColorArgument2 = TextureArgument.Current;
//            device.TextureState[2].AlphaOperation = TextureOperation.Disable;
//            device.TextureState[2].AlphaArgument1 = TextureArgument.Current;
//            device.TextureState[2].AlphaArgument2 = TextureArgument.Current;
//            device.TextureState[2].TextureTransform = TextureTransform.Count3 | TextureTransform.Projected;
//            device.TextureState[2].TextureCoordinateIndex = (int)TextureCoordinateIndex.CameraSpacePosition;
//            Matrix invViewCam = device.Transform.World * device.Transform.View;
//            invViewCam.Invert();

//            Vector3d sun = planet3dLocations[0];
//            sun.Subtract(centerPoint);

//            Vector3d moon = planet3dLocations[(int)shadowCaster];
//            moon.Subtract(centerPoint);

//            Vector3d sunMoonAngle = moon - sun;
//            Vector2d latLng = sunMoonAngle.ToSpherical();
//            Matrix mat =
//                invViewCam *
//                Matrix.LookAtLH(new Vector3(0, -5, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0)) *
//                Matrix.PerspectiveFovLH(.95f, 1, .00f, 200f) *
//                Matrix.RotationZ((double)(-rotation + latLng.X)) *
//                bias;

//            device.Transform.Texture2 = mat;
//            return centerPoint;
//        }
        public static double GetAdjustedPlanetRadius(int planetID)
        {
            if (planetID > planetDiameters.Length - 1)
            {
                planetID = (int)SolarSystemObjects.Earth;
            }


            double diameter = planetDiameters[planetID];


            double radius = (double)(diameter / 2.0);
            if (planetID != 0)
            {
                radius = radius * (1 + (3 * (Settings.Active.SolarSystemScale - 1)));
            }
            else
            {
                radius = radius * (1 + (.30 * (Settings.Active.SolarSystemScale - 1)));
            }

            return radius;
        }
        static AstroRaDec[] planetLocations;

        static Sprite2d planetSprite = new Sprite2d();

        static PositionColoredTextured[] planetPoints = null;
        private static void DrawPlanet(RenderContext renderContext, int planetID, double opacity)
        {

            AstroRaDec planetPosition = planetLocations[planetID];

            if (((planetID < 14) && planetScales[planetID] < ( renderContext.ViewCamera.Zoom / 6.0) / 400) )
            {
                if (planetID < 10 || ((planetID < 14) && planetScales[planetID] > (renderContext.ViewCamera.Zoom / 6.0) / 6400))
                {
                    Vector3d point = (Vector3d)Coordinates.RADecTo3d(planetPosition.RA, planetPosition.Dec);
                    DrawPointPlanet(renderContext, point, 3, planetColors[planetID], false);
                }
                return;
            }
            //else
            //{
            //    if ((planetID < 10 ) || (planetID < 14 && !planetPosition.Eclipsed) || (planetID > 13 && planetPosition.Shadow) )
            //    {
            //        Vector3d point = (Vector3d)Coordinates.RADecTo3d(planetPosition.RA, planetPosition.Dec);
            //        DrawPointPlanet(canvas, point, planetScales[planetID] / (Viewer.MasterView.FovScale / 3600), planetColors[planetID], false, perspectiveViewMatrix, perspectiveProjectionMatrix);
            //    }
            //}

            Texture brush = null;

            if (planetID < 10 || planetID == 18)
            {
                brush =  planetTextures[planetID];
            }
            else if (planetID < 14)
            {
                if (planetLocations[planetID].Eclipsed)
                {
                    brush = planetTextures[15];

                }
                else
                {
                    if (Settings.Active.ShowMoonsAsPointSource)
                    {
                        brush = planetTextures[14];
                    }
                    else
                    {
                        brush = planetTextures[planetID];
                    }
                }
            }
            else
            {
                if (!planetLocations[planetID].Shadow)
                {
                    return;
                }
                //Shadows of moons
                brush = planetTextures[15];
            }


         

            // Special Case for Saturn and Eclipse 
            //if (planetID == 18 || planetID == 5)
            //{
            //    double Width = rad*2;
            //    double Height = rad*2;
            //    var points = new PointCollection { new Point(screenSpacePnt.X - Width / 2, screenSpacePnt.Y - Height / 2), new Point(screenSpacePnt.X + Width / 2, screenSpacePnt.Y - Height / 2), new Point(screenSpacePnt.X + Width / 2, screenSpacePnt.Y + Height / 2), new Point(screenSpacePnt.X - Width / 2, screenSpacePnt.Y + Height / 2) };
            //    //var bitmapImage = new BitmapImage(this.sourceUri);
            //    //var imageBrush = new ImageBrush { ImageSource = bitmapImage, RelativeTransform = this.imageTransform };


            //    Polygon polygon = new Polygon { Points = points };

            //    polygon.Opacity = opacity;
            //    if (rad < 8)
            //    {
            //        SolidColorBrush fillBrush = new SolidColorBrush(planetColors[planetID]);
            //        polygon.Fill = fillBrush;
            //    }
            //    else
            //    {
            //        polygon.Fill = brush;
            //    }

            //    //RotateTransform rt = new RotateTransform();
            //    //rt.CenterX = screenSpacePnt.X;
            //    //rt.CenterY = screenSpacePnt.Y;
            //    //rt.Angle = this.RotationAngle;
            //    //polygon.RenderTransform = rt;
            //    canvas.Children.Add(polygon);
            //}
            //else
            {
                if (renderContext.gl != null)
                {
                    //todo draw in WebGL
                    if (planetPoints == null)
                    {
                        planetPoints = new PositionColoredTextured[4];
                        for (int i = 0; i < 4; i++)
                        {
                            planetPoints[i] = new PositionColoredTextured();
                        }
                    }
                    float radius = (float)(planetScales[planetID] / 2.0);
                    float raRadius = (float)(radius / Math.Cos(planetPosition.Dec / 180.0 * Math.PI));


                    planetPoints[0].Position = Coordinates.RADecTo3dAu((planetPosition.RA - (raRadius / 15)), planetPosition.Dec + radius, 1);
                    planetPoints[0].Tu = 0;
                    planetPoints[0].Tv = 1;
                    planetPoints[0].Color = Colors.White;

                    planetPoints[1].Position = Coordinates.RADecTo3dAu((planetPosition.RA - (raRadius / 15)), planetPosition.Dec - radius, 1);
                    planetPoints[1].Tu = 0;
                    planetPoints[1].Tv = 0;
                    planetPoints[1].Color = Colors.White;

                 
                    planetPoints[2].Position = Coordinates.RADecTo3dAu((planetPosition.RA + (raRadius / 15)), planetPosition.Dec + radius, 1);
                    planetPoints[2].Tu = 1;
                    planetPoints[2].Tv = 1;
                    planetPoints[2].Color = Colors.White;

                    planetPoints[3].Position = Coordinates.RADecTo3dAu((planetPosition.RA + (raRadius / 15)), planetPosition.Dec - radius, 1);

                    planetPoints[3].Tu = 1;
                    planetPoints[3].Tv = 0;
                    planetPoints[3].Color = Colors.White;

                    planetSprite.Draw(renderContext, planetPoints, 4, brush, true, 1);
                }
                else
                {

                    Vector3d center = Coordinates.RADecTo3d(planetPosition.RA, planetPosition.Dec);

                    double rad = planetScales[planetID] / (renderContext.FovScale / 3600) / 2;

                    Vector3d screenSpacePnt = renderContext.WVP.Transform(center);
                    if (screenSpacePnt.Z < 0)
                    {
                        return;
                    }

                    if (Vector3d.Dot((Vector3d)renderContext.ViewPoint, (Vector3d)center) < .55)
                    {
                        return;
                    }
                    CanvasContext2D ctx = renderContext.Device;
                    ctx.Save();
                    ctx.Alpha = opacity;
                    ctx.BeginPath();
                    ctx.Arc(screenSpacePnt.X, screenSpacePnt.Y, rad, 0, Math.PI * 2, true);
                    ctx.LineWidth = 0;

                    ctx.ClosePath();
                    ctx.Clip();
                    ctx.DrawImage(brush.ImageElement, screenSpacePnt.X - rad, screenSpacePnt.Y - rad, rad * 2, rad * 2);

                    ctx.Alpha = 1.0;
                    ctx.Restore();



                    //Ellipse ellipse = new Ellipse();
                    //ellipse.Width = rad * 2;
                    //ellipse.Height = ellipse.Width;
                    //ellipse.StrokeThickness = 0;
                    //ellipse.Stroke = null;
                    //ellipse.Opacity = opacity;

                    //if (rad < 8)
                    //{
                    //    SolidColorBrush fillBrush = new SolidColorBrush(planetColors[planetID]);
                    //    ellipse.Fill = fillBrush;
                    //}
                    //else
                    //{
                    //    ellipse.Fill = brush;
                    //}
                    //TranslateTransform tt = new TranslateTransform();
                    //tt.X = screenSpacePnt.X - rad;
                    //tt.Y = screenSpacePnt.Y - rad;
                    //ellipse.RenderTransform = tt;
                    //canvas.Children.Add(ellipse);
                }
            }
        }
        private static void DrawPlanetPhase(RenderContext renderContext, int planetID, double phase, double angle, int dark)
        {
        }
            
            //            //AstroRaDec planetPosition = planetLocations[planetID];

//            //if (planetID < 10)
//            //{
//            //    device.SetTexture(0, planetTextures[planetID]);
//            //}
//            //else if (planetID < 14)
//            //{
//            //    if (planetLocations[planetID].Eclipsed)
//            //    {
//            //        device.SetTexture(0, planetTextures[15]);

//            //    }
//            //    else
//            //    {
//            //        if (Properties.Settings.Default.ShowMoonsAsPointSource)
//            //        {
//            //            device.SetTexture(0, planetTextures[14]);

//            //        }
//            //        else
//            //        {
//            //            device.SetTexture(0, planetTextures[planetID]);
//            //        }
//            //    }
//            //}
//            //else
//            //{
//            //    if (!planetLocations[planetID].Shadow)
//            //    {
//            //        return;
//            //    }
//            //    //Shadows of moons
//            //    device.SetTexture(0, planetTextures[15]);
//            //}


//            //float radius = (double)(planetScales[planetID] / 2.0);
//            //float raRadius = (double)(radius / Math.Cos(planetPosition.Dec / 180.0 * Math.PI));
//            //int segments = 128;
//            //CustomVertex.PositionColoredTextured[] points = new CustomVertex.PositionColoredTextured[4 * (segments + 1)];

//            //int index = 0;

//            ////double top = planetPosition.Dec + radius;
//            ////double bottom = planetPosition.Dec - radius;
//            ////double left = (planetPosition.RA + (raRadius / 15)) - 12;
//            ////double right = (planetPosition.RA - (raRadius / 15)) - 12;
//            //double top = radius;
//            //double bottom = -radius;
//            //double left = +(radius / 15);
//            //double right = -(radius / 15);

//            //double width = left - right;
//            //double height = bottom - top;

//            //Color rightColor = Color.FromArgb(dark * 4, dark * 4, dark * 4);
//            //Color leftColor = Color.FromArgb(dark, dark, dark);

//            //double phaseFactor = Math.Sin(Coordinates.DegreesToRadians(phase + 90));
//            //if (phase < 180)
//            //{
//            //    rightColor = leftColor;
//            //    leftColor = Color.FromArgb(dark * 4, dark * 4, dark * 4);
//            //    //  phaseFactor = -phaseFactor;
//            //}

//            //double rotation = -Math.Cos(planetPosition.RA / 12 * Math.PI) * 23.5;

//            //Matrix matrix = Microsoft.DirectX.Matrix.Identity;
//            //matrix.Multiply(Microsoft.DirectX.Matrix.RotationX((double)(((rotation)) / 180f * Math.PI)));
//            //matrix.Multiply(Microsoft.DirectX.Matrix.RotationZ((double)((planetPosition.Dec) / 180f * Math.PI)));
//            //matrix.Multiply(Microsoft.DirectX.Matrix.RotationY((double)(((360 - (planetPosition.RA * 15)) + 180) / 180f * Math.PI)));

//            //double step = 1.0 / segments;
//            //for (int i = 0; i <= segments; i++)
//            //{

//            //    double y = i * (1.0 / segments);
//            //    double yf = (y - .5) * 2;
//            //    double x = Math.Sqrt(1 - ((yf) * (yf)));
//            //    double xt;
//            //    x = x * phaseFactor;
//            //    x = ((width / 2) + (x * width / 2)) - width / 80;
//            //    if (x > width)
//            //    {
//            //        x = width;
//            //    }
//            //    if (x < 0)
//            //    {
//            //        x = 0;
//            //    }
//            //    xt = x / width;
//            //    double x1 = Math.Sqrt(1 - ((yf) * (yf)));
//            //    double x1t;
//            //    x1 = x1 * phaseFactor;
//            //    x1 = ((width / 2) + (x1 * width / 2)) + width / 80;
//            //    if (x1 > width)
//            //    {
//            //        x1 = width;
//            //    }
//            //    if (x1 < 0)
//            //    {
//            //        x1 = 0;
//            //    }
//            //    x1t = x1 / width;

//            //    points[index].Position = Coordinates.RADecTo3d(left, top + y * height, matrix);
//            //    points[index].Tu = 0;
//            //    points[index].Tv = (double)y;
//            //    points[index].Color = leftColor.ToArgb();
//            //    points[index + 1].Position = Coordinates.RADecTo3d(left - x, top + y * height, matrix);
//            //    points[index + 1].Tu = (double)xt;
//            //    points[index + 1].Tv = (double)y;
//            //    points[index + 1].Color = leftColor.ToArgb();
//            //    points[index + 2].Position = Coordinates.RADecTo3d(left - x1, top + y * height, matrix);
//            //    points[index + 2].Tu = (double)x1t;
//            //    points[index + 2].Tv = (double)y;
//            //    points[index + 2].Color = rightColor.ToArgb();
//            //    points[index + 3].Position = Coordinates.RADecTo3d(right, top + y * height, matrix);
//            //    points[index + 3].Tu = 1;
//            //    points[index + 3].Tv = (double)y;
//            //    points[index + 3].Color = rightColor.ToArgb();

//            //    index += 4;
//            //    //points
//            //}

//            //CustomVertex.PositionColoredTextured[] triangles = new CustomVertex.PositionColoredTextured[18 * (segments)];
//            //index = 0;
//            //for (int yy = 0; yy < segments; yy++)
//            //{
//            //    for (int xx = 0; xx < 3; xx++)
//            //    {
//            //        triangles[index] = points[yy * 4 + xx];
//            //        triangles[index + 1] = points[yy * 4 + (xx + 1)];
//            //        triangles[index + 2] = points[((yy + 1) * 4) + (xx)];
//            //        triangles[index + 3] = points[yy * 4 + (xx + 1)];
//            //        triangles[index + 4] = points[((yy + 1) * 4) + (xx + 1)];
//            //        triangles[index + 5] = points[((yy + 1) * 4) + (xx)];
//            //        index += 6;
//            //    }

//            //}
//            ////Matrix mat = Microsoft.DirectX.Matrix.RotationZ(rotationAngle);

//            //device.DrawUserPrimitives(PrimitiveType.TriangleList, triangles.Length / 3, triangles);
//            //// Render Stuff Here
//        }

        static double GeocentricElongation(double ObjectAlpha, double ObjectDelta, double SunAlpha, double SunDelta)
        {
            //Convert the RA's to radians
            ObjectAlpha = Coordinates.DegreesToRadians(ObjectAlpha * 15);
            SunAlpha = Coordinates.DegreesToRadians(SunAlpha * 15);

            //Convert the declinations to radians
            ObjectDelta = Coordinates.DegreesToRadians(ObjectDelta);
            SunDelta = Coordinates.DegreesToRadians(SunDelta);

            //Return the result
            return Coordinates.RadiansToDegrees(Math.Acos(Math.Sin(SunDelta) * Math.Sin(ObjectDelta) + Math.Cos(SunDelta) * Math.Cos(ObjectDelta) * Math.Cos(SunAlpha - ObjectAlpha)));
        }

        static double PhaseAngle(double GeocentricElongation, double EarthObjectDistance, double EarthSunDistance)
        {
            //Convert from degrees to radians
            GeocentricElongation = Coordinates.DegreesToRadians(GeocentricElongation);

            //Return the result
            return Coordinates.MapTo0To360Range(Coordinates.RadiansToDegrees(Math.Atan2(EarthSunDistance * Math.Sin(GeocentricElongation), EarthObjectDistance - EarthSunDistance * Math.Cos(GeocentricElongation))));
        }
        static double PositionAngle(double Alpha0, double Delta0, double Alpha, double Delta)
        {
            //Convert to radians
            Alpha0 = Coordinates.HoursToRadians(Alpha0);
            Alpha = Coordinates.HoursToRadians(Alpha);
            Delta0 = Coordinates.DegreesToRadians(Delta0);
            Delta = Coordinates.DegreesToRadians(Delta);

            return Coordinates.MapTo0To360Range(Coordinates.RadiansToDegrees(Math.Atan2(Math.Cos(Delta0) * Math.Sin(Alpha0 - Alpha), Math.Sin(Delta0) * Math.Cos(Delta) - Math.Cos(Delta0) * Math.Sin(Delta) * Math.Cos(Alpha0 - Alpha))));
        }
//        static BitmapImage PlanetShadow;
        static void DrawSphere(RenderContext renderContext, int planetID)
        {
            string planetName = GetNameFrom3dId(planetID);
            Imageset planet = WWTControl.Singleton.GetImagesetByName(planetName);

            if (planet == null)
            {
                planet = WWTControl.Singleton.GetImagesetByName("Bing Maps Aerial");
            }

            if (planet != null)
            {
                renderContext.DrawImageSet(planet, 100);
                if (planetID == (int)SolarSystemObjects.Earth)
                {
                    // todo clouds   Earth3d.MainWindow.DrawCloudsNoCheck();
                }
                return;

            }
        }

//        static int[] triangleCountSphere = null;
//        static int[] vertexCountSphere = null;
//        static int triangleCountRings = subDivisionsRings * 2;
//        static int maxSubDivisionsX = 96;
//        static int maxSubDivisionsY = 48;
//        const int subDivisionsRings = 192;
//        const int sphereCount = 4;
//        static void InitSphere(Canvas canvas)
//        {
//            //if (sphereIndexBuffers != null)
//            //{
//            //    foreach (IndexBuffer indexBuf in sphereIndexBuffers)
//            //    {
//            //        indexBuf.Dispose();
//            //    }
//            //}
//            //if (sphereVertexBuffers != null)
//            //{
//            //    foreach (VertexBuffer vertBuf in sphereVertexBuffers)
//            //    {
//            //        vertBuf.Dispose();
//            //    }
//            //}
//            //sphereVertexBuffers = new VertexBuffer[sphereCount];
//            //sphereIndexBuffers = new IndexBuffer[sphereCount];

//            //triangleCountSphere = new int[sphereCount];
//            //vertexCountSphere = new int[sphereCount];

//            //int countX = maxSubDivisionsX;
//            //int countY = maxSubDivisionsY;


//            //for (int sphereIndex = 0; sphereIndex < sphereCount; sphereIndex++)
//            //{
//            //    triangleCountSphere[sphereIndex] = countX * countY * 2;
//            //    vertexCountSphere[sphereIndex] = (countX + 1) * (countY + 1);
//            //    sphereVertexBuffers[sphereIndex] = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), ((countX + 1) * (countY + 1)), device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Tile.PoolToUse);

//            //    sphereIndexBuffers[sphereIndex] = new IndexBuffer(typeof(short), countX * countY * 6, device, Usage.WriteOnly, Tile.PoolToUse);

//            //    double lat, lng;

//            //    int index = 0;
//            //    double latMin = 90;
//            //    double latMax = -90;
//            //    double lngMin = -180;
//            //    double lngMax = 180;


//            //    // Create a vertex buffer 
//            //    CustomVertex.PositionNormalTextured[] verts = (CustomVertex.PositionNormalTextured[])sphereVertexBuffers[sphereIndex].Lock(0, 0); // Lock the buffer (which will return our structs)
//            //    int x1, y1;

//            //    double latDegrees = latMax - latMin;
//            //    double lngDegrees = lngMax - lngMin;

//            //    double textureStepX = 1.0f / countX;
//            //    double textureStepY = 1.0f / countY;
//            //    for (y1 = 0; y1 <= countY; y1++)
//            //    {

//            //        if (y1 != countY)
//            //        {
//            //            lat = latMax - (textureStepY * latDegrees * (double)y1);
//            //        }
//            //        else
//            //        {
//            //            lat = latMin;
//            //        }

//            //        for (x1 = 0; x1 <= countX; x1++)
//            //        {
//            //            if (x1 != countX)
//            //            {
//            //                lng = lngMin + (textureStepX * lngDegrees * (double)x1);
//            //            }
//            //            else
//            //            {
//            //                lng = lngMax;
//            //            }
//            //            index = y1 * (countX + 1) + x1;
//            //            verts[index].Position = Coordinates.GeoTo3d(lat, lng);// Add Altitude mapping here
//            //            verts[index].Normal = verts[index].Position;// with altitude will come normal recomputer from adjacent triangles
//            //            verts[index].Tu = (double)(x1 * textureStepX);
//            //            verts[index].Tv = (double)(1f - (y1 * textureStepY));
//            //        }
//            //    }
//            //    sphereVertexBuffers[sphereIndex].Unlock();
//            //    short[] indexArray = (short[])sphereIndexBuffers[sphereIndex].Lock(0, LockFlags.None);

//            //    for (y1 = 0; y1 < countY; y1++)
//            //    {
//            //        for (x1 = 0; x1 < countX; x1++)
//            //        {
//            //            index = (y1 * countX * 6) + 6 * x1;
//            //            // First triangle in quad
//            //            indexArray[index] = (short)(y1 * (countX + 1) + x1);
//            //            indexArray[index + 2] = (short)((y1 + 1) * (countX + 1) + x1);
//            //            indexArray[index + 1] = (short)(y1 * (countX + 1) + (x1 + 1));

//            //            // Second triangle in quad
//            //            indexArray[index + 3] = (short)(y1 * (countX + 1) + (x1 + 1));
//            //            indexArray[index + 5] = (short)((y1 + 1) * (countX + 1) + x1);
//            //            indexArray[index + 4] = (short)((y1 + 1) * (countX + 1) + (x1 + 1));
//            //        }
//            //    }
//            //    sphereIndexBuffers[sphereIndex].Unlock();
//            //    countX /= 2;
//            //    countY /= 2;
//            //}
//        }

//        static BitmapImage ringsMap;
//        static void DrawRings(Canvas canvas)
//        {
//            if (ringsMap == null)
//            {
//                ringsMap = LoadPlanetTexture(device, Properties.Resources.SaturnRings);
//            }
//            device.RenderState.AlphaBlendEnable = true;
//            device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
//            device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.InvSourceAlpha;


//            device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;

//            device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
//            device.TextureState[0].ColorOperation = TextureOperation.Modulate;
//            device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
//            device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
//            device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
//            device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
//            device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

//            // test
//            device.TextureState[0].ColorArgument0 = TextureArgument.Diffuse;

//            //
//            device.TextureState[1].ColorOperation = TextureOperation.SelectArg1;
//            device.TextureState[1].ColorArgument1 = TextureArgument.Current;
//            device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
//            device.TextureState[1].AlphaOperation = TextureOperation.Modulate;
//            device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
//            device.TextureState[1].AlphaArgument2 = TextureArgument.Constant;
//            device.TextureState[1].ConstantColorValue = (int)Color.FromArgb((int)(255), (int)(255), (int)(255), (int)(255)).ToArgb();

//            device.SetTexture(0, ringsMap);
//            device.SetStreamSource(0, ringsVertexBuffer, 0);
//            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
//            //.TextureState[2].TextureCoordinateIndex = TextureCoordinateIndex.CameraSpacePosition;
//            device.Indices = null;
//            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, triangleCountRings);

        }



//        static void InitRings()
//        {
//            //if (ringsVertexBuffer != null)
//            //{
//            //    ringsVertexBuffer.Dispose();
//            //    ringsVertexBuffer = null;
//            //}
//            //double inner = 1.113;
//            //double outer = 2.25;

//            //ringsVertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), ((subDivisionsRings + 1) * 4), device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Tile.PoolToUse);

//            //triangleCountRings = (subDivisionsRings) * 2;
//            //CustomVertex.PositionNormalTextured[] verts = (CustomVertex.PositionNormalTextured[])ringsVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

//            //double radStep = Math.PI * 2.0 / (double)subDivisionsRings;
//            //int index = 0;
//            //for (int x = 0; x <= subDivisionsRings; x += 2)
//            //{
//            //    double rads1 = x * radStep;
//            //    double rads2 = (x + 1) * radStep;

//            //    verts[index].Position = new Vector3((double)(Math.Cos(rads1) * inner), 0, (double)(Math.Sin(rads1) * inner));
//            //    verts[index].Normal = new Vector3(0, 1, 0);
//            //    verts[index].Tu = 1;
//            //    verts[index].Tv = 0;
//            //    index++;

//            //    verts[index].Position = new Vector3((double)(Math.Cos(rads1) * outer), 0, (double)(Math.Sin(rads1) * outer));
//            //    verts[index].Normal = new Vector3(0, 1, 0);
//            //    verts[index].Tu = 0;
//            //    verts[index].Tv = 0;
//            //    index++;
//            //    verts[index].Position = new Vector3((double)(Math.Cos(rads2) * inner), 0, (double)(Math.Sin(rads2) * inner));
//            //    verts[index].Normal = new Vector3(0, 1, 0);
//            //    verts[index].Tu = 1;
//            //    verts[index].Tv = 1;
//            //    index++;
//            //    verts[index].Position = new Vector3((double)(Math.Cos(rads2) * outer), 0, (double)(Math.Sin(rads2) * outer));
//            //    verts[index].Normal = new Vector3(0, 1, 0);
//            //    verts[index].Tu = 0;
//            //    verts[index].Tv = 1;
//            //    index++;

//            //}
//            //ringsVertexBuffer.Unlock();
//        }
//    }
//    //public class SolarSystemObject
//    //{
//    //    public SolarSystemObject(string name, SolarSystemObject parent, double diameter, double orbitalYears,
//    //                             double tilt, double rotation, bool moon, bool has3d, Image disk, Image map)
//    //    {
//    //        this.name = name;
//    //        this.parent = parent;
//    //        this.diameter = diameter;
//    //        this.orbitalYears = orbitalYears;
//    //        this.tilt = tilt;
//    //        this.rotation = rotation;
//    //        this.moon = moon;
//    //        this.has3d = has3d;
//    //        this.disk = disk;
//    //        this.map = map;
//    //    }

//    //    Image disk;
//    //    Image map;
//    //    Texture diskTexture;
//    //    Texture mapTexture;
//    //    Vector3 location3d;
//    //    int indexID = 0;
//    //    double tilt = 0;
//    //    double diameter = 0;
//    //    double distance;
//    //    double ra;

//    //    public double Ra
//    //    {
//    //        get { return ra; }
//    //        set { ra = value; }
//    //    }
//    //    double dec;

//    //    public double Dec
//    //    {
//    //        get { return dec; }
//    //        set { dec = value; }
//    //    }
//    //    double rotation;
//    //    double rotationPeriod;
//    //    string name;

//    //    public string Name
//    //    {
//    //        get { return name; }
//    //        set { name = value; }
//    //    }
//    //    string localname;
//    //    IImageSet sufaceImageset;
//    //    string wikiLink;
//    //    double orbitalYears;
//    //    bool moon;
//    //    bool eclipsed;
//    //    bool shadowing;
//    //    bool has3d;

//    //    public bool Has3d
//    //    {
//    //        get { return has3d; }
//    //        set { has3d = value; }
//    //    }
//    //    double scale;

//    //    public double Scale
//    //    {
//    //        get { return scale; }
//    //        set { scale = value; }
//    //    }

//    //    SolarSystemObject parent;

//    //    public void UpdatePosition(bool UpdateOrbit)
//    //    {
//    //    }
//    //    public void Draw2d(Device device)
//    //    {
//    //    }
//    //    public void Draw3d(Device device)
//    //    {
//    //    }
//    //    public void DrawOrbit(Device device)
//    //    {
//    //    }

//    //}
}
