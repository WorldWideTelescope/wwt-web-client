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
            //todo hook up to LayerManagerUI
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

        static ContextMenuStrip contextMenu;

        static public void showLayerMenu(LayerMap layer, int x, int y)
        {

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem scaleMenu = ToolStripMenuItem.Create(Language.GetLocalizedText(1291, "Scale/Histogram"));
           
            ToolStripSeparator sep1 = new ToolStripSeparator();

            scaleMenu.Click = scaleMenu_click;

            contextMenu.Items.Add(scaleMenu);

            contextMenu.Show(Vector2d.Create(x,y));
        }

        static public void scaleMenu_click(object sender, EventArgs e)
        {
            Histogram hist = new Histogram();
            hist.image = FitsImage.Last;
            hist.Show(Vector2d.Create(200, 200));
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
            LayerMaps["Sun"].ChildMaps["Mercury"] = new LayerMap("Mercury", ReferenceFrames.Mercury);
            LayerMaps["Sun"].ChildMaps["Venus"] = new LayerMap("Venus", ReferenceFrames.Venus);
            LayerMaps["Sun"].ChildMaps["Earth"] = new LayerMap("Earth", ReferenceFrames.Earth);
            LayerMaps["Sun"].ChildMaps["Earth"].ChildMaps["Moon"] = new LayerMap("Moon", ReferenceFrames.Moon);

            //if (!TourLayers)
            //{
            //    LayerMaps["Sun"].ChildMaps["Earth"].ChildMaps.Add("ISS", iss);
            //}

            LayerMaps["Sun"].ChildMaps["Mars"] = new LayerMap("Mars", ReferenceFrames.Mars);
            LayerMaps["Sun"].ChildMaps["Jupiter"] = new LayerMap("Jupiter", ReferenceFrames.Jupiter);
            LayerMaps["Sun"].ChildMaps["Jupiter"].ChildMaps["Io"] = new LayerMap("Io", ReferenceFrames.Io);
            LayerMaps["Sun"].ChildMaps["Jupiter"].ChildMaps["Europa"] = new LayerMap("Europa", ReferenceFrames.Europa);
            LayerMaps["Sun"].ChildMaps["Jupiter"].ChildMaps["Ganymede"] = new LayerMap("Ganymede", ReferenceFrames.Ganymede);
            LayerMaps["Sun"].ChildMaps["Jupiter"].ChildMaps["Callisto"] = new LayerMap("Callisto", ReferenceFrames.Callisto);
            LayerMaps["Sun"].ChildMaps["Saturn"] = new LayerMap("Saturn", ReferenceFrames.Saturn);
            LayerMaps["Sun"].ChildMaps["Uranus"] = new LayerMap("Uranus", ReferenceFrames.Uranus);
            LayerMaps["Sun"].ChildMaps["Neptune"] = new LayerMap("Neptune", ReferenceFrames.Neptune);
            LayerMaps["Sun"].ChildMaps["Pluto"] = new LayerMap("Pluto", ReferenceFrames.Pluto);

            
            AddMoons(moonfile);

            LayerMaps["Sky"] = new LayerMap("Sky", ReferenceFrames.Sky);
            LayerMaps["Sun"].Open = true;
            AllMaps.Clear();

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

                LayerMaps["Sun"].ChildMaps[planet] = frame;

                AllMaps.Clear();

                AddAllMaps(LayerMaps, null);
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
                        player.UpdateTweenPosition( -1);


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
}
