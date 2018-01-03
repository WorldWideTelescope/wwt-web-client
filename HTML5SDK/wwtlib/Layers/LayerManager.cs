using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class LayerManager
    {
        static int version = 0;

        public static int Version
        {
            get { return LayerManager.version; }
            set { LayerManager.version = value; }
        }



        static bool tourLayers = false;

        public static bool TourLayers
        {
            get { return LayerManager.tourLayers; }
            set
            {
                if (LayerManager.tourLayers != value && value == false)
                {
                    ClearLayers();
                    LayerManager.tourLayers = value;
                    LoadTree();
                }
                else if (LayerManager.tourLayers != value && value == true)
                {
                    LayerManager.tourLayers = value;
                    InitLayers();
                }

            }
        }

        public static void LoadTree()
        {
            if (WWTControl.scriptInterface != null)
            {
                WWTControl.scriptInterface.RefreshLayerManagerNow();
            }
        }

        static Dictionary<string, LayerMap> layerMaps = new Dictionary<string, LayerMap>();
        static Dictionary<string, LayerMap> layerMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> LayerMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerMapsTours;
                }
                else
                {
                    return LayerManager.layerMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerMapsTours = value;
                }
                else
                {
                    LayerManager.layerMaps = value;
                }
            }
        }

        private static Dictionary<string, LayerMap> allMaps = new Dictionary<string, LayerMap>();
        private static Dictionary<string, LayerMap> allMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> AllMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.allMapsTours;
                }
                else
                {
                    return LayerManager.allMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.allMapsTours = value;
                }
                else
                {
                    LayerManager.allMaps = value;
                }
            }
        }

        static string currentMap = "Earth";

        public static string CurrentMap
        {
            get { return LayerManager.currentMap; }
            set { LayerManager.currentMap = value; }
        }

        private static Dictionary<Guid, Layer> layerList = new Dictionary<Guid, Layer>();
        static Dictionary<Guid, Layer> layerListTours = new Dictionary<Guid, Layer>();

        public static Dictionary<Guid, Layer> LayerList
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerListTours;
                }
                else
                {
                    return LayerManager.layerList;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerListTours = value;
                }
                else
                {
                    LayerManager.layerList = value;
                }
            }
        }


        //static List<Layer> layers = new List<Layer>();
        static LayerManager()
        {
            GetMoonFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=moons");
            //InitLayers();
        }
        static string moonfile = "";
        static public void InitLayers()
        {
            ClearLayers();
            //LayerMap iss = null;
            //if (!TourLayers)
            //{
            //    string[] isstle = new string[0];
            //    try
            //    {
            //        //This is downloaded now on startup
            //        string url = "http://www.worldwidetelescope.org/wwtweb/isstle.aspx";
            //        string filename = string.Format(@"{0}data\isstle.txt", Properties.Settings.Default.CahceDirectory);
            //        DataSetManager.DownloadFile(url, filename, false, false);

            //        isstle = File.ReadAllLines(filename);
            //    }
            //    catch
            //    {
            //    }

            //    iss = new LayerMap("ISS", ReferenceFrames.Custom);
            //    iss.Frame.Epoch = SpaceTimeController.TwoLineDateToJulian("10184.51609218");
            //    iss.Frame.SemiMajorAxis = 6728829.41;
            //    iss.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
            //    iss.Frame.Inclination = 51.6442;
            //    iss.Frame.LongitudeOfAscendingNode = 147.0262;
            //    iss.Frame.Eccentricity = .0009909;
            //    iss.Frame.MeanAnomolyAtEpoch = 325.5563;
            //    iss.Frame.MeanDailyMotion = 360 * 15.72172655;
            //    iss.Frame.ArgumentOfPeriapsis = 286.4623;
            //    iss.Frame.Scale = 1;
            //    iss.Frame.SemiMajorAxisUnits = AltUnits.Meters;
            //    iss.Frame.MeanRadius = 130;
            //    iss.Frame.Oblateness = 0;
            //    iss.Frame.ShowOrbitPath = true;
            //    if (isstle.Length > 1)
            //    {
            //        iss.Frame.FromTLE(isstle[0], isstle[1], 398600441800000);
            //    }
            //}

            LayerMaps["Sun"] = new LayerMap("Sun", ReferenceFrames.Sun);
            LayerMaps["Sun"].AddChild(new LayerMap("Mercury", ReferenceFrames.Mercury));
            LayerMaps["Sun"].AddChild(new LayerMap("Venus", ReferenceFrames.Venus));
            LayerMaps["Sun"].AddChild(new LayerMap("Earth", ReferenceFrames.Earth));
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(new LayerMap("Moon", ReferenceFrames.Moon));

            //if (!TourLayers)
            //{
            //    LayerMaps["Sun"].ChildMaps["Earth"].ChildMaps.Add("ISS", iss);
            //}

            //LayerMaps["Sun"].ChildMaps["Earth"].AddChild(ol);
            //LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l1);
            //LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l2);

            LayerMaps["Sun"].AddChild(new LayerMap("Mars", ReferenceFrames.Mars));
            LayerMaps["Sun"].AddChild(new LayerMap("Jupiter", ReferenceFrames.Jupiter));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Io", ReferenceFrames.Io));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Europa", ReferenceFrames.Europa));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Ganymede", ReferenceFrames.Ganymede));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Callisto", ReferenceFrames.Callisto));
            LayerMaps["Sun"].AddChild(new LayerMap("Saturn", ReferenceFrames.Saturn));
            LayerMaps["Sun"].AddChild(new LayerMap("Uranus", ReferenceFrames.Uranus));
            LayerMaps["Sun"].AddChild(new LayerMap("Neptune", ReferenceFrames.Neptune));
            LayerMaps["Sun"].AddChild(new LayerMap("Pluto", ReferenceFrames.Pluto));


            AddMoons(moonfile);

            LayerMaps["Sky"] = new LayerMap("Sky", ReferenceFrames.Sky);
            LayerMaps["Sun"].Open = true;
            allMaps = new Dictionary<string, LayerMap>();

            AddAllMaps(LayerMaps, null);

            version++;
            LoadTree();

        }

        private static void AddAllMaps(Dictionary<string, LayerMap> maps, String parent)
        {
            foreach (String key in maps.Keys)
            {
                LayerMap map = maps[key];
                map.Frame.Parent = parent;
                AllMaps[map.Name] = map;
                AddAllMaps(map.ChildMaps, map.Name);
            }
        }

        private static void ClearLayers()
        {
            foreach (Guid key in LayerList.Keys)
            {
                Layer layer = LayerList[key];
                layer.CleanUp();
            }

            LayerList.Clear();
            LayerMaps.Clear();
        }


        static WebFile webFileMoons;

        public static void GetMoonFile(string url)
        {
            webFileMoons = new WebFile(url);
            webFileMoons.OnStateChange = MoonFileStateChange;
            webFileMoons.Send();
        }

        public static void MoonFileStateChange()
        {
            if (webFileMoons.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFileMoons.Message);
            }
            else if (webFileMoons.State == StateType.Received)
            {
                moonfile = webFileMoons.GetText();
                InitLayers();
            }

        }

        private static void AddMoons(string file)
        {

            string[] data = file.Split("\r\n");

            bool first = true;
            foreach (string line in data)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                string[] parts = line.Split("\t");
                if (parts.Length > 16)
                {
                    string planet = parts[0];
                    LayerMap frame = new LayerMap(parts[2], ReferenceFrames.Custom);
                    frame.Frame.SystemGenerated = true;
                    frame.Frame.Epoch = double.Parse(parts[1]);
                    frame.Frame.SemiMajorAxis = double.Parse(parts[3]) * 1000;
                    frame.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                    frame.Frame.Inclination = double.Parse(parts[7]);
                    frame.Frame.LongitudeOfAscendingNode = double.Parse(parts[8]);
                    frame.Frame.Eccentricity = double.Parse(parts[4]);
                    frame.Frame.MeanAnomolyAtEpoch = double.Parse(parts[6]);
                    frame.Frame.MeanDailyMotion = double.Parse(parts[9]);
                    frame.Frame.ArgumentOfPeriapsis = double.Parse(parts[5]);
                    frame.Frame.Scale = 1;
                    frame.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                    frame.Frame.MeanRadius = double.Parse(parts[16]) * 1000;
                    frame.Frame.RotationalPeriod = double.Parse(parts[17]);
                    frame.Frame.ShowAsPoint = false;
                    frame.Frame.ShowOrbitPath = true;
                    frame.Frame.RepresentativeColor = Color.FromArgb(255, 144, 238, 144);
                    frame.Frame.Oblateness = 0;

                    LayerMaps["Sun"].ChildMaps[planet].ChildMaps[frame.Name] = frame;
                }
            }
        }

        public static VoTableLayer AddVoTableLayer(VoTable table, string title)
        {

            VoTableLayer layer = VoTableLayer.Create(table);
            layer.Name = title;
            layer.Astronomical = true;
            layer.ReferenceFrame = "Sky";
            LayerList[layer.ID] = layer;
            AllMaps["Sky"].Layers.Add(layer);
            AllMaps["Sky"].Open = true;
            layer.Enabled = true;
            version++;
            LoadTree();

            return layer;
        }

        public static ImageSetLayer AddImageSetLayer(Imageset imageset, string title)
        {
            ImageSetLayer layer = ImageSetLayer.Create(imageset);
            layer.DoneLoading(null);       
            layer.Name = title;
            layer.Astronomical = true;
            layer.ReferenceFrame = "Sky";
            LayerList[layer.ID] = layer;
            AllMaps["Sky"].Layers.Add(layer);
            AllMaps["Sky"].Open = true;
            layer.Enabled = true;
            version++;
            LoadTree();

            return layer;
        }


        internal static void CloseAllTourLoadedLayers()
        {
            List<Guid> purgeTargets = new List<Guid>();
            foreach (Guid key in LayerList.Keys)
            {
                Layer layer = LayerList[key];
                if (layer.LoadedFromTour)
                {
                    purgeTargets.Add(layer.ID);
                }
            }

            foreach (Guid guid in purgeTargets)
            {
                DeleteLayerByID(guid, true, false);
            }

            List<string> purgeMapsNames = new List<string>();

            foreach (String key in AllMaps.Keys)
            {
                LayerMap map = AllMaps[key];

                if (map.LoadedFromTour && map.Layers.Count == 0)
                {
                    purgeMapsNames.Add(map.Name);
                }
            }

            foreach (string name in purgeMapsNames)
            {
                PurgeLayerMapDeep(AllMaps[name], true);
            }



            Version++;
            LoadTree();

        }

        public static void PurgeLayerMapDeep(LayerMap target, bool topLevel)
        {

            foreach (Layer layer in target.Layers)
            {
                LayerManager.DeleteLayerByID(layer.ID, false, false);
            }

            target.Layers.Clear();

            foreach (string key in target.ChildMaps.Keys)
            {
                LayerMap map = target.ChildMaps[key];
                PurgeLayerMapDeep(map, false);
            }

            target.ChildMaps.Clear();
            if (topLevel)
            {
                if (!String.IsNullOrEmpty(target.Frame.Parent))
                {
                    if (AllMaps.ContainsKey(target.Frame.Parent))
                    {
                        AllMaps[target.Frame.Parent].ChildMaps.Remove(target.Name);
                    }
                }
                else
                {
                    if (LayerMaps.ContainsKey(target.Name))
                    {
                        LayerMaps.Remove(target.Name);
                    }
                }
            }
            AllMaps.Remove(target.Name);
            version++;
        }


        internal static void CleanAllTourLoadedLayers()
        {
            foreach (Guid key in LayerList.Keys)
            {
                Layer layer = LayerList[key];
                if (layer.LoadedFromTour)
                {
                    //todo We may want to copy layers into a temp directory later, for now we are just leaving the layer data files in the temp tour directory. 
                    layer.LoadedFromTour = false;
                }
            }
        }

        // Merged layers from Tour Player Alternate universe into the real layer manager layers list
        public static void MergeToursLayers()
        {

            tourLayers = false;
            bool OverWrite = false;
            bool CollisionChecked = false;

            foreach (String key in allMapsTours.Keys)
            {
                LayerMap map = allMapsTours[key];
                if (!allMaps.ContainsKey(map.Name))
                {
                    LayerMap newMap = new LayerMap(map.Name, ReferenceFrames.Custom);
                    newMap.Frame = map.Frame;
                    newMap.LoadedFromTour = true;
                    LayerManager.AllMaps[newMap.Name] = newMap;
                }
            }
            ConnectAllChildren();
            foreach (Guid key in layerListTours.Keys)
            {
                Layer layer = layerListTours[key];

                if (LayerList.ContainsKey(layer.ID))
                {
                    if (!CollisionChecked)
                    {
                        //todo add UI in the future
                        if (true)
                        // if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            OverWrite = true;
                        }
                        else
                        {
                            OverWrite = false;
                        }
                        CollisionChecked = true;
                    }

                    if (OverWrite)
                    {
                        LayerManager.DeleteLayerByID(layer.ID, true, false);
                    }
                }

                if (!LayerList.ContainsKey(layer.ID))
                {
                    if (AllMaps.ContainsKey(layer.ReferenceFrame))
                    {
                        LayerList[layer.ID] = layer;

                        AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    }
                }
                else
                {
                    layer.CleanUp();
                }
            }

            layerListTours.Clear();
            allMapsTours.Clear();
            layerMapsTours.Clear();
            LoadTree();
        }

        public static void ConnectAllChildren()
        {
            foreach (String key in AllMaps.Keys)
            {
                LayerMap map = AllMaps[key];
                if (String.IsNullOrEmpty(map.Frame.Parent) && !LayerMaps.ContainsKey(map.Frame.Name))
                {
                    LayerMaps[map.Name] = map;
                }
                else if (!String.IsNullOrEmpty(map.Frame.Parent) && AllMaps.ContainsKey(map.Frame.Parent))
                {
                    if (!AllMaps[map.Frame.Parent].ChildMaps.ContainsKey(map.Frame.Name))
                    {
                        AllMaps[map.Frame.Parent].ChildMaps[map.Frame.Name] = map;
                    }
                }
            }
        }

        public static bool DeleteLayerByID(Guid ID, bool removeFromParent, bool updateTree)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                layer.CleanUp();
                if (removeFromParent)
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                }
                LayerList.Remove(ID);
                version++;
                if (updateTree)
                {
                    LoadTree();
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        internal static void PrepTourLayers()
        {
            if (TourPlayer.Playing)
            {
                TourPlayer player = (TourPlayer)WWTControl.Singleton.uiController;
                if (player != null)
                {
                    TourDocument tour = player.Tour;

                    if (tour.CurrentTourStop != null)
                    {
                        player.UpdateTweenPosition(-1);


                        if (!tour.CurrentTourStop.KeyFramed)
                        {
                            tour.CurrentTourStop.UpdateLayerOpacity();
                            foreach (Guid key in tour.CurrentTourStop.Layers.Keys)
                            {
                                LayerInfo info = tour.CurrentTourStop.Layers[key];

                                if (LayerList.ContainsKey(info.ID))
                                {
                                    LayerList[info.ID].Opacity = info.FrameOpacity;
                                    LayerList[info.ID].SetParams(info.FrameParams);
                                }
                            }
                        }
                    }
                }
            }
        }


        internal static void Draw(RenderContext renderContext, float opacity, bool astronomical, string referenceFrame, bool nested, bool cosmos)
        {


            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }



            LayerMap thisMap = AllMaps[referenceFrame];

            if (!thisMap.Enabled || (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0 && !(thisMap.Frame.ShowAsPoint || thisMap.Frame.ShowOrbitPath)))
            {
                return;
            }
            if (TourPlayer.Playing)
            {
                TourPlayer player = (TourPlayer)WWTControl.Singleton.uiController;
                if (player != null)
                {
                    TourDocument tour = player.Tour;
                    if (tour.CurrentTourStop != null)
                    {
                        player.UpdateTweenPosition(-1);
                        tour.CurrentTourStop.UpdateLayerOpacity();

                        foreach (Guid key in tour.CurrentTourStop.Layers.Keys)
                        {
                            LayerInfo info = tour.CurrentTourStop.Layers[key];

                            if (LayerList.ContainsKey(info.ID))
                            {
                                LayerList[info.ID].Opacity = info.FrameOpacity;
                                LayerList[info.ID].SetParams(info.FrameParams);
                            }
                        }
                    }
                }
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.ReferenceFrameType != ReferenceFrameTypes.Orbital && thisMap.Frame.ReferenceFrameType != ReferenceFrameTypes.Trajectory)
                //if (true)
                {
                    renderContext.World = Matrix3d.MultiplyMatrix(thisMap.Frame.WorldMatrix, renderContext.World);
                }
                else
                {
                    renderContext.World = Matrix3d.MultiplyMatrix(thisMap.Frame.WorldMatrix, renderContext.WorldBaseNonRotating);

                }
                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }



            if (thisMap.Frame.ShowAsPoint)
            {

                // todo Draw point planet...
                // Planets.DrawPointPlanet(renderContext.Device, new Vector3d(0, 0, 0), (float).2, thisMap.Frame.RepresentativeColor, true);

            }



            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        if (layer.Enabled) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.FadeIn || layer.FadeType == FadeType.Both) ? (layer.FadeSpan / 864000000) : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.FadeOut || layer.FadeType == FadeType.Both) ? (layer.FadeSpan / 864000000) : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / (layer.FadeSpan / 864000000));
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / (layer.FadeSpan / 864000000));
                                }
                                layer.Astronomical = astronomical;
                                //if (thisMap.Frame.Reference == ReferenceFrames.Sky)
                                //{
                                //    layer.Astronomical = true;
                                //}
                                if (layer is SpreadSheetLayer)
                                {
                                    SpreadSheetLayer tsl = layer as SpreadSheetLayer;
                                    tsl.Draw(renderContext, opacity * fadeOpacity, cosmos);
                                }
                                else
                                {
                                    layer.Draw(renderContext, opacity * fadeOpacity, cosmos);
                                }
                            }
                        }
                    }
                }
            }
            if (nested)
            {
                foreach (string key in AllMaps[referenceFrame].ChildMaps.Keys)
                {
                    LayerMap map = AllMaps[referenceFrame].ChildMaps[key];
                    if (map.Frame.ShowOrbitPath && Settings.Active.SolarSystemOrbits)
                    {
                        if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Orbital)
                        {
                            if (map.Frame.Orbit == null)
                            {
                                map.Frame.Orbit = new Orbit(map.Frame.Elements, 360, map.Frame.RepresentativeColor, 1, (float)renderContext.NominalRadius);
                            }
                            Matrix3d matSaved = renderContext.World;
                            renderContext.World = Matrix3d.MultiplyMatrix(thisMap.Frame.WorldMatrix, renderContext.WorldBaseNonRotating);

                            map.Frame.Orbit.Draw3D(renderContext, 1f * .25f, Vector3d.Create(0, 0, 0));
                            renderContext.World = matSaved;
                        }
                        else if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Trajectory)
                        {
                            //todo add trajectories back
                            //if (map.Frame.trajectoryLines == null)
                            //{
                            //    map.Frame.trajectoryLines = new LineList(renderContext.Device);
                            //    map.Frame.trajectoryLines.ShowFarSide = true;
                            //    map.Frame.trajectoryLines.UseNonRotatingFrame = true;

                            //    int count = map.Frame.Trajectory.Count - 1;
                            //    for (int i = 0; i < count; i++)
                            //    {
                            //        Vector3d pos1 = map.Frame.Trajectory[i].Position;
                            //        Vector3d pos2 = map.Frame.Trajectory[i + 1].Position;
                            //        pos1.Multiply(1 / renderContext.NominalRadius);
                            //        pos2.Multiply(1 / renderContext.NominalRadius);
                            //        map.Frame.trajectoryLines.AddLine(pos1, pos2, map.Frame.RepresentativeColor, new Dates());
                            //    }
                            //}
                            //Matrix3D matSaved = renderContext.World;
                            //renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                            //map.Frame.trajectoryLines.DrawLines(renderContext, Earth3d.MainWindow.showMinorOrbits.Opacity * .25f);
                            //renderContext.World = matSaved;
                        }
                    }

                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        Draw(renderContext, opacity, astronomical, map.Name, nested, cosmos);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }

        internal static Dictionary<Guid, LayerInfo> GetVisibleLayerList(Dictionary<Guid, LayerInfo> previous)
        {
            Dictionary<Guid, LayerInfo> list = new Dictionary<Guid, LayerInfo>();

            foreach (Guid key in LayerList.Keys)
            {
                Layer layer = LayerList[key];
                if (layer.Enabled)
                {
                    LayerInfo info = new LayerInfo();
                    info.StartOpacity = info.EndOpacity = layer.Opacity;
                    info.ID = layer.ID;
                    info.StartParams = layer.GetParams();


                    if (previous.ContainsKey(info.ID))
                    {
                        info.EndOpacity = previous[info.ID].EndOpacity;
                        info.EndParams = previous[info.ID].EndParams;
                    }
                    else
                    {
                        info.EndParams = layer.GetParams();
                    }
                    list[layer.ID] = info;
                }
            }
            return list;
        }

        public static void SetVisibleLayerList(Dictionary<Guid, LayerInfo> list)
        {
            foreach (Guid key in LayerList.Keys)
            {
                Layer layer = LayerList[key];
                layer.Enabled = list.ContainsKey(layer.ID);
                try
                {
                    if (layer.Enabled)
                    {
                        layer.Opacity = list[layer.ID].FrameOpacity;
                        layer.SetParams(list[layer.ID].FrameParams);
                    }
                }
                catch
                {
                }
            }
            //SyncLayerState();
        }

        //todo remove the stuff from draw that is redundant once predraw has run
        internal static void PreDraw(RenderContext renderContext, float opacity, bool astronomical, string referenceFrame, bool nested)
        {


            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }



            LayerMap thisMap = AllMaps[referenceFrame];

            if (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0)
            {
                return;
            }
            if (TourPlayer.Playing)
            {
                TourPlayer player = (TourPlayer)WWTControl.Singleton.uiController as TourPlayer;
                if (player != null)
                {
                    TourDocument tour = player.Tour;
                    if (tour.CurrentTourStop != null)
                    {
                        player.UpdateTweenPosition(-1);
                        tour.CurrentTourStop.UpdateLayerOpacity();
                        foreach (Guid key in tour.CurrentTourStop.Layers.Keys)
                        {
                            LayerInfo info = tour.CurrentTourStop.Layers[key];
                            if (LayerList.ContainsKey(info.ID))
                            {
                                LayerList[info.ID].Opacity = info.FrameOpacity;
                                LayerList[info.ID].SetParams(info.FrameParams);
                            }
                        }

                    }
                }
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.ReferenceFrameType != ReferenceFrameTypes.Orbital)
                //if (true)
                {
                    renderContext.World = Matrix3d.MultiplyMatrix(thisMap.Frame.WorldMatrix, renderContext.World);
                }
                else
                {
                    renderContext.World = Matrix3d.MultiplyMatrix(thisMap.Frame.WorldMatrix, renderContext.WorldBaseNonRotating);

                }
                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }



            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        if (layer.Enabled) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.FadeIn || layer.FadeType == FadeType.Both) ? (layer.FadeSpan / 864000000) : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.FadeOut || layer.FadeType == FadeType.Both) ? (layer.FadeSpan / 864000000) : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / (layer.FadeSpan / 864000000));
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / (layer.FadeSpan / 864000000));
                                }
                                if (thisMap.Frame.Reference == ReferenceFrames.Sky)
                                {
                                    layer.Astronomical = true;
                                }
                                layer.PreDraw(renderContext, opacity * fadeOpacity);
                            }
                        }
                    }

                }
            }
            if (nested)
            {
                foreach (string key in AllMaps[referenceFrame].ChildMaps.Keys)
                {
                    LayerMap map = AllMaps[referenceFrame].ChildMaps[key];
                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        PreDraw(renderContext, opacity, astronomical, map.Name, nested);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }



        public static void Add(Layer layer, bool updateTree)
        {
            if (!LayerList.ContainsKey(layer.ID))
            {
                if (AllMaps.ContainsKey(layer.ReferenceFrame))
                {
                    LayerList[layer.ID] = layer;

                    AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    version++;
                    if (updateTree)
                    {
                        LoadTree();
                    }
                }
            }
        }

        static ContextMenuStrip contextMenu;
        static object selectedLayer = null;
        static Vector2d lastMenuClick = new Vector2d();
        static public void showLayerMenu(object selected, int x, int y)
        {
            lastMenuClick = Vector2d.Create(x, y);
            selectedLayer = selected;

            if (selected is LayerMap)
            {
                CurrentMap = ((LayerMap)selected).Name;
            }
            else if (selected is Layer)
            {
                CurrentMap = ((Layer)selected).ReferenceFrame;
            }


            //if (layer is LayerMap)
            //{

            //    contextMenu = new ContextMenuStrip();

            //    ToolStripMenuItem add = ToolStripMenuItem.Create(Language.GetLocalizedText(1291, "Scale/Histogram"));

            //    ToolStripSeparator sep1 = new ToolStripSeparator();

            //    addGirdLayer.Click = addGirdLayer_Click;

            //    contextMenu.Items.Add(scaleMenu);

            //    contextMenu.Show(Vector2d.Create(x, y));
            //}
            //else if (layer is ImageSetLayer)
            //{
            //    contextMenu = new ContextMenuStrip();

            //    ToolStripMenuItem scaleMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(1291, "Scale/Histogram"));

            //    ToolStripSeparator sep1 = new ToolStripSeparator();

            //    scaleMenu.Click = scaleMenu_click;

            //    contextMenu.Items.Add(scaleMenu);

            //    contextMenu.Show(Vector2d.Create(x, y));
            //}

            if (((selected is Layer) && !(selected is SkyOverlays)))
            {
                Layer selectedLayer = (Layer)selected;

                contextMenu = new ContextMenuStrip();
                ToolStripMenuItem renameMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(225, "Rename"));
                ToolStripMenuItem Expand = ToolStripMenuItem.Create(Language.GetLocalizedText(981, "Expand"));
                ToolStripMenuItem Collapse = ToolStripMenuItem.Create(Language.GetLocalizedText(982, "Collapse"));
                ToolStripMenuItem copyMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(428, "Copy"));
                ToolStripMenuItem deleteMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(167, "Delete"));
                ToolStripMenuItem saveMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(960, "Save..."));
                ToolStripMenuItem publishMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(983, "Publish to Community..."));
                ToolStripMenuItem colorMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(458, "Color/Opacity"));
                ToolStripMenuItem opacityMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(305, "Opacity"));

                ToolStripMenuItem popertiesMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(20, "Properties"));
                ToolStripMenuItem scaleMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(1291, "Scale/Histogram"));
                ToolStripMenuItem lifeTimeMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(683, "Lifetime"));
                ToolStripSeparator spacer1 = new ToolStripSeparator();
                ToolStripMenuItem top = ToolStripMenuItem.Create(Language.GetLocalizedText(684, "Move to Top"));
                ToolStripMenuItem up = ToolStripMenuItem.Create(Language.GetLocalizedText(685, "Move Up"));
                ToolStripMenuItem down = ToolStripMenuItem.Create(Language.GetLocalizedText(686, "Move Down"));
                ToolStripMenuItem bottom = ToolStripMenuItem.Create(Language.GetLocalizedText(687, "Move to Bottom"));
                ToolStripMenuItem showViewer = ToolStripMenuItem.Create(Language.GetLocalizedText(957, "VO Table Viewer"));

                ToolStripSeparator spacer2 = new ToolStripSeparator();


                ToolStripMenuItem defaultImageset = ToolStripMenuItem.Create(Language.GetLocalizedText(1294, "Background Image Set"));


                top.Click = top_Click;
                up.Click = up_Click;
                down.Click = down_Click;
                bottom.Click = bottom_Click;
                saveMenu.Click = saveMenu_Click;
                publishMenu.Click = publishMenu_Click;
                Expand.Click = Expand_Click;
                Collapse.Click = Collapse_Click;
                copyMenu.Click = copyMenu_Click;
                colorMenu.Click = colorMenu_Click;
                deleteMenu.Click = deleteMenu_Click;
                renameMenu.Click = renameMenu_Click;
                popertiesMenu.Click = popertiesMenu_Click;
                scaleMenu.Click = scaleMenu_click;


                defaultImageset.Click = defaultImageset_Click;




                opacityMenu.Click = opacityMenu_Click;
                lifeTimeMenu.Click = lifeTimeMenu_Click;
                showViewer.Click = showViewer_Click;
                contextMenu.Items.Add(renameMenu);

                if (!selectedLayer.Opened && selectedLayer.GetPrimaryUI() != null && selectedLayer.GetPrimaryUI().HasTreeViewNodes)
                {
                    contextMenu.Items.Add(Expand);

                }

                if (selectedLayer.Opened)
                {
                    contextMenu.Items.Add(Collapse);
                }


                if (selectedLayer.CanCopyToClipboard())
                {
                    contextMenu.Items.Add(copyMenu);
                }

                contextMenu.Items.Add(deleteMenu);
                contextMenu.Items.Add(saveMenu);

                //if (Earth3d.IsLoggedIn)
                //{
                //    contextMenu.Items.Add(publishMenu);
                //}

                contextMenu.Items.Add(spacer2);
                contextMenu.Items.Add(colorMenu);
                contextMenu.Items.Add(opacityMenu);

                // ToDo Should we have this only show up in layers under Identity Reference Frames?
                contextMenu.Items.Add(lifeTimeMenu);


                if (selected is ImageSetLayer)
                {
                    contextMenu.Items.Add(defaultImageset);

                    ImageSetLayer isl = selected as ImageSetLayer;
                    defaultImageset.Checked = isl.OverrideDefaultLayer;
                }

                if (selected is SpreadSheetLayer || selected is Object3dLayer || selected is GroundOverlayLayer || selected is GreatCirlceRouteLayer || selected is OrbitLayer)
                {
                    contextMenu.Items.Add(popertiesMenu);
                }

                if (selected is VoTableLayer)
                {
                    contextMenu.Items.Add(showViewer);
                }

                if (selected is ImageSetLayer)
                {
                    ImageSetLayer isl = selected as ImageSetLayer;
                    // if (isl.FitsImage != null)
                    {
                        contextMenu.Items.Add(scaleMenu);
                    }
                }

                if (AllMaps[selectedLayer.ReferenceFrame].Layers.Count > 1)
                {
                    contextMenu.Items.Add(spacer1);
                    contextMenu.Items.Add(top);
                    contextMenu.Items.Add(up);
                    contextMenu.Items.Add(down);
                    contextMenu.Items.Add(bottom);
                }


                contextMenu.Show(Vector2d.Create(x, y));
            }
            else if (selected is LayerMap)
            {
                LayerMap map = selected as LayerMap;
                bool sandbox = map.Frame.Reference.ToString() == "Sandbox";
                bool Dome = map.Frame.Name == "Dome";
                bool Sky = map.Frame.Name == "Sky";

                if (Dome)
                {
                    return;
                }
                contextMenu = new ContextMenuStrip();
                ToolStripMenuItem trackFrame = ToolStripMenuItem.Create(Language.GetLocalizedText(1298, "Track this frame"));
                ToolStripMenuItem goTo = ToolStripMenuItem.Create(Language.GetLocalizedText(1299, "Fly Here"));
                ToolStripMenuItem showOrbit = ToolStripMenuItem.Create("Show Orbit");
                ToolStripMenuItem newMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(674, "New Reference Frame"));
                ToolStripMenuItem newLayerGroupMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(675, "New Layer Group"));
                ToolStripMenuItem addMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(166, "Add"));
                ToolStripMenuItem newLight = ToolStripMenuItem.Create("Add Light");
                ToolStripMenuItem addFeedMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(956, "Add OData/table feed as Layer"));
                ToolStripMenuItem addWmsLayer = ToolStripMenuItem.Create(Language.GetLocalizedText(987, "New WMS Layer"));
                ToolStripMenuItem addGirdLayer = ToolStripMenuItem.Create(Language.GetLocalizedText(1300, "New Lat/Lng Grid"));
                ToolStripMenuItem addGreatCircle = ToolStripMenuItem.Create(Language.GetLocalizedText(988, "New Great Circle"));
                ToolStripMenuItem importTLE = ToolStripMenuItem.Create(Language.GetLocalizedText(989, "Import Orbital Elements"));
                ToolStripMenuItem addMpc = ToolStripMenuItem.Create(Language.GetLocalizedText(1301, "Add Minor Planet"));
                ToolStripMenuItem deleteFrameMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(167, "Delete"));
                ToolStripMenuItem pasteMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(425, "Paste"));
                ToolStripMenuItem addToTimeline = ToolStripMenuItem.Create(Language.GetLocalizedText(1290, "Add to Timeline"));
                ToolStripMenuItem addKeyframe = ToolStripMenuItem.Create(Language.GetLocalizedText(1280, "Add Keyframe"));

                ToolStripMenuItem popertiesMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(20, "Properties"));
                ToolStripMenuItem saveMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(990, "Save Layers"));
                ToolStripMenuItem publishLayers = ToolStripMenuItem.Create(Language.GetLocalizedText(991, "Publish Layers to Community"));
                ToolStripSeparator spacer1 = new ToolStripSeparator();
                ToolStripSeparator spacer0 = new ToolStripSeparator();
                ToolStripSeparator spacer2 = new ToolStripSeparator();
                ToolStripMenuItem asReferenceFrame = ToolStripMenuItem.Create("As Reference Frame");
                ToolStripMenuItem asOrbitalLines = ToolStripMenuItem.Create("As Orbital Line");


                trackFrame.Click = trackFrame_Click;
                goTo.Click = goTo_Click;
                asReferenceFrame.Click = addMpc_Click;
                asOrbitalLines.Click = AsOrbitalLines_Click;
                // Ad Sub Menus
                addMpc.DropDownItems.Add(asReferenceFrame);
                addMpc.DropDownItems.Add(asOrbitalLines);




                addMenu.Click = addMenu_Click;
                // newLight.Click = newLight_Click;

                newLayerGroupMenu.Click = newLayerGroupMenu_Click;
                pasteMenu.Click = pasteLayer_Click;
                newMenu.Click = newMenu_Click;
                deleteFrameMenu.Click = deleteFrameMenu_Click;
                popertiesMenu.Click = FramePropertiesMenu_Click;
                //  addWmsLayer.Click = addWmsLayer_Click;
                // importTLE.Click = importTLE_Click;
                addGreatCircle.Click = addGreatCircle_Click;
                //    saveMenu.Click = SaveLayers_Click;
                //   publishLayers.Click = publishLayers_Click;
                addGirdLayer.Click = addGirdLayer_Click;


                ToolStripMenuItem convertToOrbit = ToolStripMenuItem.Create("Extract Orbit Layer");
                //    convertToOrbit.Click = ConvertToOrbit_Click;


                if (map.Frame.Reference != ReferenceFrames.Identity)
                {
                    if (WWTControl.Singleton.SolarSystemMode | WWTControl.Singleton.SandboxMode) //&& Control.ModifierKeys == Keys.Control)
                    {
                        bool spacerNeeded = false;
                        if (map.Frame.Reference != ReferenceFrames.Custom && !WWTControl.Singleton.SandboxMode)
                        {
                            // fly to
                            if (!Sky)
                            {
                                contextMenu.Items.Add(goTo);
                                spacerNeeded = true;
                            }

                            try
                            {
                                string name = map.Frame.Reference.ToString();
                                if (name != "Sandbox")
                                {
                                    SolarSystemObjects ssObj = (SolarSystemObjects)Enums.Parse("SolarSystemObjects", name);
                                    int id = (int)ssObj;

                                    int bit = (int)Math.Pow(2, id);

                                    showOrbit.Checked = (Settings.Active.PlanetOrbitsFilter & bit) != 0;
                                    showOrbit.Click = showOrbitPlanet_Click;
                                    showOrbit.Tag = bit.ToString();
                                }
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            // track
                            if (!sandbox && !Sky)
                            {
                                contextMenu.Items.Add(trackFrame);
                                spacerNeeded = true;
                            }
                            showOrbit.Checked = map.Frame.ShowOrbitPath;
                            showOrbit.Click = showOrbit_Click;
                        }

                        if (spacerNeeded)
                        {
                            contextMenu.Items.Add(spacer2);
                        }

                        if (!Sky && !sandbox)
                        {
                            contextMenu.Items.Add(showOrbit);

                            contextMenu.Items.Add(spacer0);
                        }

                        if (map.Frame.Reference.ToString() == "Sandbox")
                        {
                            contextMenu.Items.Add(newLight);
                        }
                    }

                    if (!Sky)
                    {
                        contextMenu.Items.Add(newMenu);
                    }
                    contextMenu.Items.Add(newLayerGroupMenu);

                }

                contextMenu.Items.Add(addMenu);
                contextMenu.Items.Add(addFeedMenu);
                if (!Sky)
                {
                    contextMenu.Items.Add(addGreatCircle);
                    contextMenu.Items.Add(addGirdLayer);
                }

                if ((map.Frame.Reference != ReferenceFrames.Identity && map.Frame.Name == "Sun") ||
                    (map.Frame.Reference == ReferenceFrames.Identity && map.Parent != null && map.Parent.Frame.Name == "Sun"))
                {
                    contextMenu.Items.Add(addMpc);
                }

                if (map.Frame.Reference == ReferenceFrames.Custom && map.Frame.ReferenceFrameType == ReferenceFrameTypes.Orbital && map.Parent != null && map.Parent.Frame.Name == "Sun")
                {
                    contextMenu.Items.Add(convertToOrbit);
                }


                if (!Sky)
                {
                    contextMenu.Items.Add(addWmsLayer);
                }


                contextMenu.Items.Add(pasteMenu);


                if (map.Frame.Reference == ReferenceFrames.Identity)
                {
                    contextMenu.Items.Add(deleteFrameMenu);
                }

                if (map.Frame.Reference == ReferenceFrames.Custom)
                {
                    contextMenu.Items.Add(deleteFrameMenu);

                    contextMenu.Items.Add(popertiesMenu);

                }

                //if (!Sky)
                {
                    contextMenu.Items.Add(spacer1);
                }
                contextMenu.Items.Add(saveMenu);
                //if (Earth3d.IsLoggedIn)
                //{
                //    contextMenu.Items.Add(publishLayers);
                //}


                contextMenu.Show(Vector2d.Create(x, y));
            }
            //else if (selectedLayer is LayerUITreeNode)
            //{
            //    LayerUITreeNode node = selectedLayer as LayerUITreeNode;
            //    contextMenu = new ContextMenuStrip();

            //    Layer layer = GetParentLayer(layerTree.SelectedNode);

            //    if (layer != null)
            //    {
            //        LayerUI ui = layer.GetPrimaryUI();
            //        List<LayerUIMenuItem> items = ui.GetNodeContextMenu(node);

            //        if (items != null)
            //        {
            //            foreach (LayerUIMenuItem item in items)
            //            {
            //                ToolStripMenuItem menuItem = ToolStripMenuItem.Create(item.Name);
            //                menuItem.Tag = item;
            //                menuItem.Click = menuItem_Click;
            //                contextMenu.Items.Add(menuItem);

            //                if (item.SubMenus != null)
            //                {
            //                    foreach (LayerUIMenuItem subItem in item.SubMenus)
            //                    {
            //                        ToolStripMenuItem subMenuItem = ToolStripMenuItem.Create(subItem.Name);
            //                        subMenuItem.Tag = subItem;
            //                        subMenuItem.Click = menuItem_Click;
            //                        menuItem.DropDownItems.Add(subMenuItem);
            //                    }
            //                }
            //            }
            //            contextMenu.Show(Cursor.Position);
            //        }


            //    }
            //}
        }

        static void publishMenu_Click(object sender, EventArgs e)
        {

            //if (Earth3d.IsLoggedIn)
            //{

            //    Layer target = (Layer)selectedLayer;

            //    string name = target.Name + ".wwtl";
            //    string filename = Path.GetTempFileName();

            //    LayerContainer layers = new LayerContainer();
            //    layers.SoloGuid = target.ID;

            //    layers.SaveToFile(filename);
            //    layers.Dispose();
            //    GC.SuppressFinalize(layers);
            //    EOCalls.InvokePublishFile(filename, name);
            //    File.Delete(filename);

            //    Earth3d.RefreshCommunity();

            //}
        }

        static void addGirdLayer_Click(object sender, EventArgs e)
        {
            GridLayer layer = new GridLayer();

            layer.Enabled = true;
            layer.Name = "Lat-Lng Grid";
            LayerList[layer.ID] = layer;
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();

        }

        static void trackFrame_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)selectedLayer;

            WWTControl.Singleton.RenderContext.SolarSystemTrack = SolarSystemObjects.Custom;
            WWTControl.Singleton.RenderContext.TrackingFrame = target.Name;
            WWTControl.Singleton.RenderContext.ViewCamera.Zoom = WWTControl.Singleton.RenderContext.TargetCamera.Zoom = .000000001;


        }

        static void goTo_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)selectedLayer;

            //IPlace place = Catalogs.FindCatalogObjectExact(target.Frame.Reference.ToString());
            //if (place != null)
            //{
            //    WWTControl.Singleton.GotoTarget(place, false, false, true);
            //}
        }

        static void saveMenu_Click(object sender, EventArgs e)
        {
            //Layer layer = (Layer)selectedLayer;
            //SaveFileDialog saveDialog = new SaveFileDialog();
            //saveDialog.Filter = Language.GetLocalizedText(993, "WorldWide Telescope Layer File(*.wwtl)") + "|*.wwtl";
            //saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //saveDialog.AddExtension = true;
            //saveDialog.DefaultExt = ".wwtl";
            //saveDialog.FileName = layer.Name + ".wwtl";
            //if (saveDialog.ShowDialog() == DialogResult.OK)
            //{
            //    // Todo add dialog for dynamic content options.
            //    LayerContainer layers = new LayerContainer();
            //    layers.SoloGuid = layer.ID;
            //    layers.SaveToFile(saveDialog.FileName);
            //    layers.Dispose();
            //    GC.SuppressFinalize(layers);
            //}
        }
        static void Expand_Click(object sender, EventArgs e)
        {
            //Layer selectedLayer = (Layer)selectedLayer;
            //selectedLayer.Opened = true;
            //LoadLayerChildren(selectedLayer, layerTree.SelectedNode);
            //layerTree.SelectedNode.Expand();
            //version++;
        }

        static void Collapse_Click(object sender, EventArgs e)
        {
            //   selectedLayer.Opened = false;
            //todo update UIO
        }

        static void copyMenu_Click(object sender, EventArgs e)
        {
            if (selectedLayer != null && selectedLayer is Layer)
            {
                Layer node = (Layer)selectedLayer;
                node.CopyToClipboard();
            }
        }

        static void newLayerGroupMenu_Click(object sender, EventArgs e)
        {
            //bool badName = true;
            //string name = Language.GetLocalizedText(676, "Enter Layer Group Name");
            //while (badName)
            //{
            //    SimpleInput input = new SimpleInput(name, Language.GetLocalizedText(238, "Name"), Language.GetLocalizedText(677, "Layer Group"), 100);
            //    if (input.ShowDialog() == DialogResult.OK)
            //    {
            //        name = input.ResultText;
            //        if (!AllMaps.ContainsKey(name))
            //        {
            //            MakeLayerGroup(name);
            //            version++;
            //            badName = false;
            //            LoadTreeLocal();
            //        }
            //        else
            //        {
            //            UiTools.ShowMessageBox(Language.GetLocalizedText(1374, "Choose a unique name"), Language.GetLocalizedText(676, "Enter Layer Group Name"));
            //        }
            //    }
            //    else
            //    {
            //        badName = false;
            //    }
            //}
        }


        static private void ImportTLEFile(string filename)
        {
            //LayerMap target = (LayerMap)selectedLayer;
            //ImportTLEFile(filename, target);
        }

        static private void MakeLayerGroup(string name)
        {
            //LayerMap target = (LayerMap)selectedLayer;
            //MakeLayerGroup(name, target);
        }

        static void lifeTimeMenu_Click(object sender, EventArgs e)
        {
            //if (selectedLayer is Layer)
            //{
            //    LayerLifetimeProperties props = new LayerLifetimeProperties();
            //    props.Target = (Layer)selectedLayer;
            //    if (props.ShowDialog() == DialogResult.OK)
            //    {
            //        // This might be moot
            //        props.Target.CleanUp();
            //    }
            //}

        }

        static void deleteFrameMenu_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)selectedLayer;
            //if (UiTools.ShowMessageBox(Language.GetLocalizedText(678, "This will delete this reference frame and all nested reference frames and layers. Do you want to continue?"), Language.GetLocalizedText(680, "Delete Reference Frame"), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            //{
            //    PurgeLayerMapDeep(target, true);
            //    version++;
            //    LoadTreeLocal();
            //}


        }




        static void FramePropertiesMenu_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)selectedLayer;

            //ReferenceFrame frame = new ReferenceFrame();
            //if (FrameWizard.ShowPropertiesSheet(target.Frame) == DialogResult.OK)
            //{

            //}
        }

        static void newMenu_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)selectedLayer;
            //ReferenceFrame frame = new ReferenceFrame();
            //frame.SystemGenerated = false;
            //if (FrameWizard.ShowWizard(frame) == DialogResult.OK)
            //{
            //    LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Custom);
            //    if (!AllMaps.ContainsKey(frame.Name))
            //    {
            //        newMap.Frame = frame;

            //        target.AddChild(newMap);
            //        newMap.Frame.Parent = target.Name;
            //        AllMaps.Add(frame.Name, newMap);
            //        version++;
            //        LoadTreeLocal();
            //    }
            //}
        }







        static void opacityMenu_Click(object sender, EventArgs e)
        {
            //OpacityPopup popup = new OpacityPopup();
            //popup.Target = (Layer)selectedLayer;
            //popup.Location = Cursor.Position;
            //popup.StartPosition = FormStartPosition.Manual;
            //popup.Show();

        }

        static void defaultImageset_Click(object sender, EventArgs e)
        {
            ImageSetLayer isl = selectedLayer as ImageSetLayer;
            isl.OverrideDefaultLayer = !isl.OverrideDefaultLayer;
        }

        static void popertiesMenu_Click(object sender, EventArgs e)
        {
            //if (selectedLayer is SpreadSheetLayer)
            //{
            //    SpreadSheetLayer target = (SpreadSheetLayer)selectedLayer;
            //    DataWizard.ShowPropertiesSheet(target);

            //    target.CleanUp();
            //    LoadTree();
            //}
            //else if (selectedLayer is SpreadSheetLayer || selectedLayer is Object3dLayer)
            //{
            //    Object3dProperties props = new Object3dProperties();
            //    props.layer = (Object3dLayer)selectedLayer;
            //    //   props.ShowDialog();
            //    props.Owner = Earth3d.MainWindow;
            //    props.Show();
            //}
            //else if (selectedLayer is GroundOverlayLayer)
            //{
            //    GroundOverlayProperties props = new GroundOverlayProperties();
            //    props.Overlay = ((GroundOverlayLayer)selectedLayer).Overlay;
            //    props.OverlayLayer = ((GroundOverlayLayer)selectedLayer);
            //    props.Owner = Earth3d.MainWindow;
            //    props.Show();
            //}
            //else if (selectedLayer is GreatCirlceRouteLayer)
            //{
            //    GreatCircleProperties props = new GreatCircleProperties();
            //    props.Layer = ((GreatCirlceRouteLayer)selectedLayer);
            //    props.Owner = Earth3d.MainWindow;
            //    props.Show();
            //}
        }

        static void renameMenu_Click(object sender, EventArgs e)
        {
            Layer layer = (Layer)selectedLayer;
            SimpleInput input = new SimpleInput(Language.GetLocalizedText(225, "Rename"), Language.GetLocalizedText(228, "New Name"), layer.Name, 32);

            input.Show(lastMenuClick, delegate ()
            {
                if (!string.IsNullOrEmpty(input.Text))
                {
                    layer.Name = input.Text;
                    version++;
                    LoadTree();
                }
            });


        }

        static void colorMenu_Click(object sender, EventArgs e)
        {
            Layer layer = (Layer)selectedLayer;

            ColorPicker picker = new ColorPicker();

            picker.CallBack = delegate
            {
                layer.Color = picker.Color;
            };

            picker.Show(Cursor.Position);
        }

        static void addMenu_Click(object sender, EventArgs e)
        {
            //bool overridable = false;
            //if ( selectedLayer is LayerMap)
            //{
            //    LayerMap map = selectedLayer as LayerMap;
            //    if (map.Frame.reference == ReferenceFrames.Custom)
            //    {
            //        overridable = true;
            //    }
            //}
            //Earth3d.LoadLayerFile(overridable);

        }


        static void deleteMenu_Click(object sender, EventArgs e)
        {
            DeleteSelectedLayer();
        }

        static private void DeleteSelectedLayer()
        {
            if (selectedLayer != null && selectedLayer is Layer)
            {
                Layer node = (Layer)selectedLayer;

                LayerList.Remove(node.ID);
                AllMaps[CurrentMap].Layers.Remove(node);
                LoadTree();
                version++;
            }
        }

        static public void scaleMenu_click(object sender, EventArgs e)
        {
            ImageSetLayer isl = selectedLayer as ImageSetLayer;

            if (isl != null)
            {
                Histogram hist = new Histogram();
                hist.image = isl.GetFitsImage();
                hist.layer = isl;
                hist.Show(Vector2d.Create(200, 200));
            }
        }

        static void showViewer_Click(object sender, EventArgs e)
        {
            if (selectedLayer is VoTableLayer)
            {
                VoTableLayer layer = selectedLayer as VoTableLayer;
                WWTControl.scriptInterface.DisplayVoTableLayer(layer);
            }
        }

        static void bottom_Click(object sender, EventArgs e)
        {
            Layer layer = selectedLayer as Layer;
            if (layer != null)
            {
                AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                AllMaps[layer.ReferenceFrame].Layers.Add(layer);
            }
            version++;
            LoadTree();
        }

        static void down_Click(object sender, EventArgs e)
        {
            Layer layer = selectedLayer as Layer;
            if (layer != null)
            {
                int index = AllMaps[layer.ReferenceFrame].Layers.LastIndexOf(layer);
                if (index < (AllMaps[layer.ReferenceFrame].Layers.Count - 1))
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                    AllMaps[layer.ReferenceFrame].Layers.Insert(index + 1, layer);
                }
            }
            version++;
            LoadTree();
        }

        static void up_Click(object sender, EventArgs e)
        {
            Layer layer = selectedLayer as Layer;
            if (layer != null)
            {
                int index = AllMaps[layer.ReferenceFrame].Layers.LastIndexOf(layer);
                if (index > 0)
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                    AllMaps[layer.ReferenceFrame].Layers.Insert(index - 1, layer);
                }
            }
            version++;
            LoadTree();
        }

        static void top_Click(object sender, EventArgs e)
        {
            Layer layer = selectedLayer as Layer;
            if (layer != null)
            {
                AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                AllMaps[layer.ReferenceFrame].Layers.Insert(0, layer);
            }
            version++;
            LoadTree();
        }


        static void pasteLayer_Click(object sender, EventArgs e)
        {


            //IDataObject dataObject = Clipboard.GetDataObject();
            //if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            //{
            //    string[] formats = dataObject.GetFormats();
            //    object data = dataObject.GetData(DataFormats.UnicodeText);
            //    if (data is String)
            //    {
            //        string layerName = "Pasted Layer";

            //        SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
            //        layer.Enabled = true;
            //        layer.Name = layerName;

            //        if (DataWizard.ShowWizard(layer) == DialogResult.OK)
            //        {
            //            LayerList.Add(layer.ID, layer);
            //            layer.ReferenceFrame = CurrentMap;
            //            AllMaps[CurrentMap].Layers.Add(layer);
            //            AllMaps[CurrentMap].Open = true;
            //            version++;
            //            LoadTree();

            //        }
            //    }
            //}

        }
        static void showOrbitPlanet_Click(object sender, EventArgs e)
        {
            try
            {
                int bit = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());

                // Flip the state
                if ((Settings.GlobalSettings.PlanetOrbitsFilter & bit) == 0)
                {
                    Settings.GlobalSettings.PlanetOrbitsFilter |= bit;
                }
                else
                {
                    Settings.GlobalSettings.PlanetOrbitsFilter &= ~bit;
                }

            }
            catch
            {
            }
        }

        static void showOrbit_Click(object sender, EventArgs e)
        {
            // Flip the state
            LayerMap map = selectedLayer as LayerMap;

            map.Frame.ShowOrbitPath = !map.Frame.ShowOrbitPath;
        }

        static void addGreatCircle_Click(object sender, EventArgs e)
        {
            AddGreatCircleLayer();
        }


        static void addMpc_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)selectedLayer;
            //SimpleInput input = new SimpleInput(Language.GetLocalizedText(1302, "Minor planet name or designation"), Language.GetLocalizedText(238, "Name"), "", 32);
            //bool retry = false;
            //do
            //{
            //    if (input.ShowDialog() == DialogResult.OK)
            //    {
            //        if (target.ChildMaps.ContainsKey(input.ResultText))
            //        {
            //            retry = true;
            //            UiTools.ShowMessageBox("That Name already exists");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                GetMpc(input.ResultText, target);
            //                retry = false;
            //            }
            //            catch
            //            {
            //                retry = true;
            //                UiTools.ShowMessageBox(Language.GetLocalizedText(1303, "The designation was not found or the MPC service was unavailable"));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        retry = false;
            //    }
            //} while (retry);
            //return;
        }

        static private void AsOrbitalLines_Click(object sender, EventArgs e)
        {
            //LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            //SimpleInput input = new SimpleInput(Language.GetLocalizedText(1302, "Minor planet name or designation"), Language.GetLocalizedText(238, "Name"), "", 32);
            //bool retry = false;
            //do
            //{
            //    if (input.ShowDialog() == DialogResult.OK)
            //    {
            //        if (target.ChildMaps.ContainsKey(input.ResultText))
            //        {
            //            retry = true;
            //            UiTools.ShowMessageBox("That Name already exists");
            //        }
            //        else
            //        {
            //            try
            //            {
            //                GetMpcAsTLE(input.ResultText, target);
            //                retry = false;
            //            }
            //            catch
            //            {
            //                retry = true;
            //                UiTools.ShowMessageBox(Language.GetLocalizedText(1303, "The designation was not found or the MPC service was unavailable"));
            //            }
            //        }
            //    }
            //    else
            //    {
            //        retry = false;
            //    }
            //} while (retry);
            //return;
        }

        private static void AddGreatCircleLayer()
        {
            //GreatCirlceRouteLayer layer = new GreatCirlceRouteLayer();
            //layer.LatStart = RenderEngine.Engine.viewCamera.Lat;
            //layer.LatEnd = RenderEngine.Engine.viewCamera.Lat - 5;
            //layer.LngStart = RenderEngine.Engine.viewCamera.Lng;
            //layer.LngEnd = RenderEngine.Engine.viewCamera.Lng + 5;
            //layer.Width = 4;
            //layer.Enabled = true;
            //layer.Name = Language.GetLocalizedText(1144, "Great Circle Route");
            //LayerList.Add(layer.ID, layer);
            //layer.ReferenceFrame = currentMap;
            //AllMaps[currentMap].Layers.Add(layer);
            //AllMaps[currentMap].Open = true;
            //version++;
            //LoadTree();

            //GreatCircleProperties props = new GreatCircleProperties();
            //props.Layer = layer;
            //props.Owner = Earth3d.MainWindow;
            //props.Show();

        }





    }
    public class LayerMap
    {
        public LayerMap(string name, ReferenceFrames reference)
        {
            Name = name;
            Frame.Reference = reference;
            double radius = 6371000;

            switch (reference)
            {
                case ReferenceFrames.Sky:
                    break;
                case ReferenceFrames.Ecliptic:
                    break;
                case ReferenceFrames.Galactic:
                    break;
                case ReferenceFrames.Sun:
                    radius = 696000000;
                    break;
                case ReferenceFrames.Mercury:
                    radius = 2439700;
                    break;
                case ReferenceFrames.Venus:
                    radius = 6051800;
                    break;
                case ReferenceFrames.Earth:
                    radius = 6371000;
                    break;
                case ReferenceFrames.Mars:
                    radius = 3390000;
                    break;
                case ReferenceFrames.Jupiter:
                    radius = 69911000;
                    break;
                case ReferenceFrames.Saturn:
                    radius = 58232000;
                    break;
                case ReferenceFrames.Uranus:
                    radius = 25362000;
                    break;
                case ReferenceFrames.Neptune:
                    radius = 24622000;
                    break;
                case ReferenceFrames.Pluto:
                    radius = 1161000;
                    break;
                case ReferenceFrames.Moon:
                    radius = 1737100;
                    break;
                case ReferenceFrames.Io:
                    radius = 1821500;
                    break;
                case ReferenceFrames.Europa:
                    radius = 1561000;
                    break;
                case ReferenceFrames.Ganymede:
                    radius = 2631200;
                    break;
                case ReferenceFrames.Callisto:
                    radius = 2410300;
                    break;
                case ReferenceFrames.Custom:
                    break;
                case ReferenceFrames.Identity:
                    break;
                default:
                    break;
            }
            Frame.MeanRadius = radius;

        }
        public Dictionary<string, LayerMap> ChildMaps = new Dictionary<string, LayerMap>();
        public void AddChild(LayerMap child)
        {
            child.Parent = this;
            ChildMaps[child.Name] = child;
        }

        public LayerMap Parent = null;
        public List<Layer> Layers = new List<Layer>();
        public bool Open = false;
        public bool Enabled = true;
        public bool LoadedFromTour = false;
        public string Name
        {
            get { return Frame.Name; }
            set { Frame.Name = value; }
        }


        public ReferenceFrame Frame = new ReferenceFrame();
        public void ComputeFrame(RenderContext renderContext)
        {
            if (Frame.Reference == ReferenceFrames.Custom)
            {
                Frame.ComputeFrame(renderContext);

            }

        }

        public override string ToString()
        {
            return Name;
        }


    }
    //public enum ReferenceFrames { Earth = 0, Helocentric = 1, Equatorial = 2, Ecliptic = 3, Galactic = 4, Moon = 5, Mercury = 6, Venus = 7, Mars = 8, Jupiter = 9, Saturn = 10, Uranus = 11, Neptune = 12, Custom = 13 };


    public enum ReferenceFrames
    {
        Sky = 0,
        Ecliptic = 1,
        Galactic = 2,
        Sun = 3,
        Mercury = 4,
        Venus = 5,
        Earth = 6,
        Mars = 7,
        Jupiter = 8,
        Saturn = 9,
        Uranus = 10,
        Neptune = 11,
        Pluto = 12,
        Moon = 13,
        Io = 14,
        Europa = 15,
        Ganymede = 16,
        Callisto = 17,
        Custom = 18,
        Identity = 19
    };

    public class SkyOverlays
    {

    }
    public class Object3dLayer
    {

    }
    public class GroundOverlayLayer
    {
    }
    public class OrbitLayer
    {

    }
}
