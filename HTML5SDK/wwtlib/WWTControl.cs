using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;


namespace wwtlib
{
    public class WWTControl
    {
        public static WWTControl Singleton;

        public RenderContext RenderContext;
        public CanvasElement Canvas;

        public WWTControl()
        {
        }

        public FolderBrowser Explorer;

        static WWTControl()
        {
            Singleton = new WWTControl();
            Singleton.RenderContext = new RenderContext();
            SpaceTimeController.last = Date.Now;
            SpaceTimeController.UpdateClock();
        }

        public static List<Imageset> ImageSets = new List<Imageset>();
        public static Folder ExploreRoot = new Folder();
        public static double StartLat = 0;
        public static double StartLng = 0;
        public static double StartZoom = 360;
        public static string StartMode = "Sky";
        public static string ImageSetName = "";

        public IUiController uiController = null;

        List<Annotation> annotations = new List<Annotation>();
        internal void AddAnnotation(Annotation annotation)
        {
            annotations.Add(annotation);
            
        }

        internal void RemoveAnnotation(Annotation annotation)
        {
            annotations.Remove(annotation);
            
        }

        internal void ClearAnnotations()
        {
            annotations.Clear();
            
        }


        public List<VizLayer> Layers = new List<VizLayer>();
        Date lastUpdate;
        int frameCount = 0;

        double zoomMax = 360;
        double zoomMaxSolarSystem = 10000000000000000;
        double ZoomMax
        {
            get
            {
                if (RenderContext.BackgroundImageset != null && RenderContext.BackgroundImageset.DataSetType == ImageSetType.SolarSystem)
                {
                    return zoomMaxSolarSystem;
                }
                else
                {
                    return zoomMax;
                }
            }
        }
        double zoomMin = 0.001373291015625;
        double zoomMinSolarSystem = 0.0001;

        public double ZoomMin
        {
            get
            {
                if (RenderContext.BackgroundImageset != null && RenderContext.BackgroundImageset.DataSetType == ImageSetType.SolarSystem)
                {
                    return zoomMinSolarSystem / 10000;
                }
                else
                {
                    return zoomMin;
                }
            }
            set { zoomMin = value; }
        }


        public static bool showDataLayers = false;

        static bool renderNeeded = false;
        public static bool RenderNeeded
        {
            get
            {
                return renderNeeded;
            }
            set
            {
                renderNeeded = true;
            }
        }

        public static Constellations constellationsFigures = null;
        public static Constellations constellationsBoundries = null;


        public string Constellation = "UMA";
        private void NotifyMoveComplete()
        {
            //if (!App.NoUI)
            //{
            //    ContextPanel.ContextPanelMaster.RunSearch();
            //}

            //Page.Master.OnArrived();
        }

        PositionColoredTextured[] fadePoints = null;
        public BlendState Fader = BlendState.Create(true, 2000);

        private bool crossFadeFrame = false;

        private Texture crossFadeTexture = null;
        public bool CrossFadeFrame
        {
            set
            {
                if (value && crossFadeFrame != value)
                {
                    if (crossFadeTexture != null)
                    {
                       // crossFadeTexture.Dispose();
                    }
                    crossFadeTexture = RenderContext.GetScreenTexture();

                }
                crossFadeFrame = value;

                if (!value)
                {
                    if (crossFadeTexture != null)
                    {
                       // crossFadeTexture.Dispose();
                        crossFadeTexture = null;
                    }
                }
            }
            get
            {
                return crossFadeFrame;
            }
        }

        private Sprite2d sprite = new Sprite2d();

        private void FadeFrame()
        {
            if (RenderContext.gl != null)
            {
                SettingParameter sp = Settings.Active.GetSetting(StockSkyOverlayTypes.FadeToBlack);
                if ((sp.Opacity > 0))
                {
                    Color color = Color.FromArgbColor(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f), Colors.Black);

                    if (!(sp.Opacity > 0))
                    {
                        color = Color.FromArgbColor(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f), Colors.Black);
                    }


                    if (crossFadeFrame)
                    {
                        color = Color.FromArgbColor((int)UiTools.Gamma((int)((sp.Opacity) * 255), 1 / 2.2f), Colors.White);
                    }
                    else
                    {
                        if (crossFadeTexture != null)
                        {
                            // crossFadeTexture.Dispose();
                            crossFadeTexture = null;
                        }
                    }
                    if (fadePoints == null)
                    {
                        fadePoints = new PositionColoredTextured[4];
                        for(int i=0; i < 4; i++)
                        {
                            fadePoints[i] = new PositionColoredTextured();
                        }
                    }


                    fadePoints[0].Position.X = -RenderContext.Width/2;
                    fadePoints[0].Position.Y = RenderContext.Height/2;
                    fadePoints[0].Position.Z = 1347;
                    fadePoints[0].Tu = 0;
                    fadePoints[0].Tv = 1;
                    fadePoints[0].Color = color;

                    fadePoints[1].Position.X = -RenderContext.Width/2;
                    fadePoints[1].Position.Y = -RenderContext.Height / 2;
                    fadePoints[1].Position.Z = 1347;
                    fadePoints[1].Tu = 0;
                    fadePoints[1].Tv = 0;
                    fadePoints[1].Color = color;

                    fadePoints[2].Position.X = RenderContext.Width/2;
                    fadePoints[2].Position.Y = RenderContext.Height/2;
                    fadePoints[2].Position.Z = 1347;
                    fadePoints[2].Tu = 1;
                    fadePoints[2].Tv = 1;
                    fadePoints[2].Color = color;

                    fadePoints[3].Position.X = RenderContext.Width / 2;
                    fadePoints[3].Position.Y = -RenderContext.Height / 2;
                    fadePoints[3].Position.Z = 1347;
                    fadePoints[3].Tu = 1;
                    fadePoints[3].Tv = 0;
                    fadePoints[3].Color = color;

                    sprite.Draw(RenderContext, fadePoints, 4, crossFadeTexture, true, 1);
                }
            }
        }

        public ImageSetType RenderType = ImageSetType.Sky;

        private Imageset milkyWayBackground = null;


        public void Render()
        {

            if (RenderContext.BackgroundImageset != null)
            {
                RenderType = RenderContext.BackgroundImageset.DataSetType;
            }
            else
            {
                RenderType = ImageSetType.Sky;
            }

            bool sizeChange = false;
            if (Canvas.Width != int.Parse(Canvas.ParentNode.Style.Width))
            {
                Canvas.Width = int.Parse(Canvas.ParentNode.Style.Width);
                sizeChange = true;
            }

            if (Canvas.Height != int.Parse(Canvas.ParentNode.Style.Height))
            {
                Canvas.Height = int.Parse(Canvas.ParentNode.Style.Height);
                sizeChange = true;
            }

            if (sizeChange)
            {
                if (Explorer != null)
                {
                    Explorer.Refresh();
                }
            }


            Tile.lastDeepestLevel = Tile.deepestLevel;

            RenderTriangle.Width = RenderContext.Width = Canvas.Width;
            RenderTriangle.Height = RenderContext.Height = Canvas.Height;
            Tile.TilesInView = 0;
            Tile.TilesTouched = 0;
            Tile.deepestLevel = 0;

            if (Mover != null)
            {
                SpaceTimeController.Now = Mover.CurrentDateTime;

                Planets.UpdatePlanetLocations(SolarSystemMode);

                if (Mover != null)
                {
                    CameraParameters newCam = Mover.CurrentPosition;

                    RenderContext.TargetCamera = newCam.Copy();
                    RenderContext.ViewCamera = newCam.Copy();
                    if (RenderContext.Space && Settings.Active.GalacticMode)
                    {
                        double[] gPoint = Coordinates.J2000toGalactic(newCam.RA * 15, newCam.Dec);

                        RenderContext.targetAlt = RenderContext.alt = gPoint[1];
                        RenderContext.targetAz = RenderContext.az = gPoint[0];
                    }
                    else if (RenderContext.Space && Settings.Active.LocalHorizonMode)
                    {
                        Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(newCam.RA, newCam.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                        RenderContext.targetAlt = RenderContext.alt = currentAltAz.Alt;
                        RenderContext.targetAz = RenderContext.az = currentAltAz.Az;
                    }

                    if (Mover.Complete)
                    {
                        //Todo Notify interested parties that move is complete
                        scriptInterface.FireArrived(Mover.CurrentPosition.RA, Mover.CurrentPosition.Dec, WWTControl.Singleton.RenderContext.ViewCamera.Zoom);
                        Mover = null;

                        NotifyMoveComplete();
                    }

                }
            }
            else
            {
                SpaceTimeController.UpdateClock();

                Planets.UpdatePlanetLocations(SolarSystemMode);

                UpdateViewParameters();


            }



            RenderContext.Clear();

            if (RenderType == ImageSetType.SolarSystem)
            {
                {
                    if ((int)SolarSystemTrack < (int)SolarSystemObjects.Custom)
                    {
                        double radius = Planets.GetAdjustedPlanetRadius((int)SolarSystemTrack);
                        double distance = RenderContext.SolarSystemCameraDistance;
                        double camAngle = RenderContext.FovLocal;
                        //double distrad = distance / (radius * Math.Tan(.5 * camAngle));

                    }



                    if (trackingObject == null)
                    {
                        //todo fix this       trackingObject = Search.FindCatalogObject("Sun");
                    }

                    RenderContext.SetupMatricesSolarSystem(true);



                    //float skyOpacity = 1.0f - Planets.CalculateSkyBrightnessFactor(RenderContext11.View, viewCamera.ViewTarget);
                    //if (float.IsNaN(skyOpacity))
                    //{
                    //    skyOpacity = 0f;
                    //}

                    double zoom = RenderContext.ViewCamera.Zoom;
                    float milkyWayBlend = (float)Math.Min(1.0, Math.Max(0, (Math.Log(zoom) - 8.4)) / 4.2);
                    float milkyWayBlendIn = (float)Math.Min(1.0, Math.Max(0, (Math.Log(zoom) - 17.9)) / 2.3);

                    Matrix3d matOldMW = RenderContext.World;
                    Matrix3d matLocalMW = RenderContext.World.Clone();
                    matLocalMW.Multiply(Matrix3d.Scaling(100000, 100000, 100000));
                    matLocalMW.Multiply(Matrix3d.RotationX(23.5 / 180 * Math.PI));
                    //                              matLocalMW.Multiply(Matrix3d.RotationY(Math.PI));
                    matLocalMW.Multiply(Matrix3d.Translation(RenderContext.CameraPosition)); //todo change this when tracking is added back
                    RenderContext.World = matLocalMW;
                    RenderContext.WorldBase = matLocalMW;
                    RenderContext.Space = true;
                    RenderContext.MakeFrustum();
                    bool lighting = RenderContext.Lighting;
                    RenderContext.Lighting = false;
                    if (Settings.Active.SolarSystemMilkyWay)
                    {
                        if (milkyWayBlend < 1) // Solar System mode Milky Way background
                        {
                            if (milkyWayBackground == null)
                            {
                                milkyWayBackground = GetImagesetByName("Digitized Sky Survey (Color)");
                            }

                            if (milkyWayBackground != null)
                            {
                                RenderTriangle.CullInside = true;
                                float c = ((1 - milkyWayBlend)) / 2;


                                RenderContext.DrawImageSet(milkyWayBackground, c * 100);

                                RenderTriangle.CullInside = false;
                            }
                        }
                    }

                    DrawSkyOverlays();
                    RenderContext.Lighting = lighting;

                    RenderContext.Space = false;
                    RenderContext.World = matOldMW;
                    RenderContext.WorldBase = matOldMW;
                    RenderContext.MakeFrustum();
                    //// CMB

                    //float cmbBlend = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 33)) / 2.3);


                    //double cmbLog = Math.Log(zoom);

                    //if (Properties.Settings.Default.SolarSystemCMB.State)
                    //{
                    //    if (cmbBlend > 0) // Solar System mode Milky Way background
                    //    {
                    //        if (cmbBackground == null)
                    //        {
                    //            cmbBackground = GetImagesetByName("Planck CMB");
                    //        }

                    //        if (cmbBackground != null)
                    //        {
                    //            float c = ((cmbBlend)) / 16;
                    //            Matrix3d matOldMW = RenderContext11.World;
                    //            Matrix3d matLocalMW = RenderContext11.World;
                    //            //double dist = UiTools.AuPerLightYear*46000000000;
                    //            matLocalMW.Multiply(Matrix3d.Scaling(2.9090248982E+15, 2.9090248982E+15, 2.9090248982E+15));
                    //            matLocalMW.Multiply(Matrix3d.RotationX(-23.5 / 180 * Math.PI));
                    //            matLocalMW.Multiply(Matrix3d.RotationY(Math.PI));
                    //            //  matLocalMW.Multiply(Matrix3d.Translation(cameraOffset));
                    //            RenderContext11.World = matLocalMW;
                    //            RenderContext11.WorldBase = matLocalMW;
                    //            Earth3d.MainWindow.MakeFrustum();

                    //            RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1, Color.White);
                    //            //SetupMatricesSpace11(60, renderType);
                    //            RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                    //            DrawTiledSphere(cmbBackground, c * Properties.Settings.Default.SolarSystemCMB.Opacity, Color.FromArgb(255, 255, 255, 255));
                    //            RenderContext11.World = matOldMW;
                    //            RenderContext11.WorldBase = matOldMW;
                    //            RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                    //        }
                    //    }
                    //}




                    {
                        Vector3d oldCamera = RenderContext.CameraPosition;
                        Matrix3d matOld = RenderContext.World;

                        Matrix3d matLocal = RenderContext.World;
                        matLocal.Multiply(Matrix3d.Translation(RenderContext.ViewCamera.ViewTarget));
                        RenderContext.CameraPosition = Vector3d.SubtractVectors(RenderContext.CameraPosition, RenderContext.ViewCamera.ViewTarget);
                        RenderContext.World = matLocal;
                        RenderContext.MakeFrustum();

                        if (Settings.Active.SolarSystemCosmos)
                        {
                            // RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                            // Grids.DrawCosmos3D(RenderContext, Properties.Settings.Default.SolarSystemCosmos.Opacity * skyOpacity);
                            Grids.DrawCosmos3D(RenderContext, 1.0f);
                            //  RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                        }

                        //    if (true)
                        //    {
                        //        RenderContext11.DepthStencilMode = DepthStencilMode.Off;

                        //        Grids.DrawCustomCosmos3D(RenderContext11, skyOpacity);

                        //        RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                        //    }


                        if (Settings.Active.SolarSystemMilkyWay && milkyWayBlendIn > 0)
                        {
                            //Grids.DrawGalaxy3D(RenderContext11, Properties.Settings.Default.SolarSystemMilkyWay.Opacity * skyOpacity * milkyWayBlendIn);

                            Grids.DrawGalaxyImage(RenderContext, milkyWayBlendIn);
                        }


                        if (Settings.Active.SolarSystemStars)
                        {
                            Grids.DrawStars3D(RenderContext, 1);
                        }


                        matLocal = matOld;
                        Vector3d pnt = RenderContext.ViewCamera.ViewTarget;
                        Vector3d vt = Vector3d.Create(-pnt.X, -pnt.Y, -pnt.Z);
                        RenderContext.CameraPosition = oldCamera;
                        matLocal.Multiply(Matrix3d.Translation(vt));
                        RenderContext.World = matLocal;
                        RenderContext.MakeFrustum();

                        LayerManager.Draw(RenderContext, 1.0f, true, "Sky", true, false);

                        RenderContext.World = matOld;
                        RenderContext.MakeFrustum();
                    }


                    if (RenderContext.SolarSystemCameraDistance < 15000)
                    {
                        RenderContext.SetupMatricesSolarSystem(false);

                        if (Settings.Active.SolarSystemMinorPlanets)
                        {
                            MinorPlanets.DrawMPC3D(RenderContext, 1, RenderContext.ViewCamera.ViewTarget);
                        }

                        if (Settings.Active.SolarSystemPlanets)
                        {
                            Planets.DrawPlanets3D(RenderContext, 1, RenderContext.ViewCamera.ViewTarget);
                        }
                    }

                    //double p = Math.Log(zoom);
                    //double d = (180 / SolarSystemCameraDistance) * 100; // (SolarSystemCameraDistance * SolarSystemCameraDistance) * 10000000;

                    //float sunAtDistance = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 7.5)) / 3);

                    //if (sunAtDistance > 0)
                    //{
                    //    Planets.DrawPointPlanet(RenderContext11, new Vector3d(0, 0, 0), (float)d * sunAtDistance, Color.FromArgb(192, 191, 128), false, 1);
                    //}

                    //if ((SolarSystemMode) && label != null && !TourPlayer.Playing)
                    //{
                    //    label.Draw(RenderContext11, true);
                    //}
                }
            }
            else
            {

                if (RenderType == ImageSetType.Earth || RenderType == ImageSetType.Planet)
                {
                    RenderContext.SetupMatricesLand3d();
                }
                else
                {
                    RenderContext.SetupMatricesSpace3d(RenderContext.Width, RenderContext.Height);
                }



                RenderContext.DrawImageSet(RenderContext.BackgroundImageset, 100);

                if (RenderContext.ForegroundImageset != null)
                {
                    if (RenderContext.ViewCamera.Opacity != 100 && RenderContext.gl == null)
                    {
                        if (foregroundCanvas.Width != RenderContext.Width || foregroundCanvas.Height != RenderContext.Height)
                        {
                            foregroundCanvas.Width = (int)RenderContext.Width;
                            foregroundCanvas.Height = (int)RenderContext.Height;
                        }

                        CanvasContext2D saveDevice = RenderContext.Device;
                        fgDevice.ClearRect(0, 0, RenderContext.Width, RenderContext.Height);
                        RenderContext.Device = fgDevice;
                        RenderContext.DrawImageSet(RenderContext.ForegroundImageset, 100);
                        RenderContext.Device = saveDevice;
                        RenderContext.Device.Save();
                        RenderContext.Device.Alpha = RenderContext.ViewCamera.Opacity / 100;
                        RenderContext.Device.DrawImage(foregroundCanvas, 0, 0);
                        RenderContext.Device.Restore();
                    }
                    else
                    {
                        RenderContext.DrawImageSet(RenderContext.ForegroundImageset, RenderContext.ViewCamera.Opacity);
                    }

                }



                if (RenderType == ImageSetType.Sky)
                {
                    Planets.DrawPlanets(RenderContext, 1);

                    Constellation = Constellations.Containment.FindConstellationForPoint(RenderContext.ViewCamera.RA, RenderContext.ViewCamera.Dec);

                    DrawSkyOverlays();

                    LayerManager.Draw(RenderContext, 1.0f, true, "Sky", true, true);
                }

                if (RenderType == ImageSetType.Earth)
                {

                    LayerManager.Draw(RenderContext, 1.0f, false, "Earth", false, false);
                }


            }

            Matrix3d worldSave = RenderContext.World;
            Matrix3d viewSave = RenderContext.View;
            Matrix3d projSave = RenderContext.Projection;

            Vector2d raDecDownDown = GetCoordinatesForScreenPoint(RenderContext.Width / 2, RenderContext.Height / 2);

            if (Settings.Current.ShowCrosshairs)
            {
                DrawCrosshairs(RenderContext);
            }

            if (uiController != null)
            {
                uiController.Render(RenderContext);
            }
            else
            {
                int index = 0;
                foreach (Annotation item in annotations)
                {
                    item.Draw(RenderContext);
                    index++;
                }

                if ((Date.Now - lastMouseMove) > 400)
                {
                    Vector2d raDecDown = GetCoordinatesForScreenPoint(hoverTextPoint.X, hoverTextPoint.Y);
                    this.AnnotationHover(raDecDown.X, raDecDown.Y, hoverTextPoint.X, hoverTextPoint.Y);
                    lastMouseMove = new Date(2100, 1, 1);
                }

                if (!string.IsNullOrEmpty(hoverText))
                {
                    DrawHoverText(RenderContext);
                }
            }


            RenderContext.SetupMatricesOverlays();
            FadeFrame();
            //RenderContext.Clear();
            //int tilesInView = Tile.TilesInView;
            //int itlesTouched = Tile.TilesTouched;

            frameCount++;

            //TileCache.PurgeLRU();
            TileCache.DecimateQueue();
            TileCache.ProcessQueue(RenderContext);
            Tile.CurrentRenderGeneration++;

            if (!TourPlayer.Playing)
            {
                CrossFadeFrame = false;
            }
            // Restore Matrixies for Finder Scope and such to map points

            RenderContext.World = worldSave;
            RenderContext.View = viewSave;
            RenderContext.Projection = projSave;


            Date now = Date.Now;

            int ms = now - lastUpdate;
            if (ms > 1000)
            {
                lastUpdate = now;
                frameCount = 0;
                RenderTriangle.TrianglesRendered = 0;
                RenderTriangle.TrianglesCulled = 0;
            }

            //  Script.Literal("requestAnimationFrame(this.render);");


            //TileCache.PurgeLRU();
            Script.SetTimeout(delegate () { Render(); }, 10);
        }

        private void DrawSkyOverlays()
        {
            if (Settings.Active.ShowConstellationPictures)
            {
                Constellations.DrawArtwork(RenderContext);
            }

            if (Settings.Active.ShowConstellationFigures)
            {
                if (constellationsFigures == null)
                {
                    constellationsFigures = Constellations.Create("Constellations", "http://www.worldwidetelescope.org/data/figures.txt", false, false, false);
                    //constellationsFigures = Constellations.Create("Constellations", "http://localhost/data/figures.txt", false, false, false);
                }

                constellationsFigures.Draw(RenderContext, false, "UMA", false);
            }

            if (Settings.Active.ShowEclipticGrid)
            {
                Grids.DrawEclipticGrid(RenderContext, 1, Colors.Green);
                if (Settings.Active.ShowEclipticGridText)
                {
                    Grids.DrawEclipticGridText(RenderContext, 1, Colors.Green);
                }
            }

            if (Settings.Active.ShowGalacticGrid)
            {
                Grids.DrawGalacticGrid(RenderContext, 1, Colors.Cyan);
                if (Settings.Active.ShowGalacticGridText)
                {
                    Grids.DrawGalacticGridText(RenderContext, 1, Colors.Cyan);
                }
            }

            if (Settings.Active.ShowAltAzGrid)
            {
                Grids.DrawAltAzGrid(RenderContext, 1, Colors.Magenta);
                if (Settings.Active.ShowAltAzGridText)
                {
                    Grids.DrawAltAzGridText(RenderContext, 1, Colors.Magenta);
                }
            }

            if (Settings.Active.ShowPrecessionChart)
            {
                Grids.DrawPrecessionChart(RenderContext, 1, Colors.Orange);

            }

            if (Settings.Active.ShowEcliptic)
            {
                Grids.DrawEcliptic(RenderContext, 1, Colors.Blue);
                if (Settings.Active.ShowEclipticOverviewText)
                {
                    Grids.DrawEclipticText(RenderContext, 1, Colors.Blue);
                }
            }

            if (Settings.Active.ShowGrid)
            {
                Grids.DrawEquitorialGrid(RenderContext, 1, Colors.White);
                if (Settings.Active.ShowEquatorialGridText)
                {
                    Grids.DrawEquitorialGridText(RenderContext, 1, Colors.White);
                }
            }

            if (Settings.Active.ShowConstellationBoundries)
            {
                if (constellationsBoundries == null)
                {
                    constellationsBoundries = Constellations.Create("Constellations", "http://www.worldwidetelescope.org/data/constellations.txt", true, false, false);
                    //constellationsBoundries = Constellations.Create("Constellations", "http://localhost/data/constellations.txt", true, false, false);
                }
                constellationsBoundries.Draw(RenderContext, Settings.Active.ShowConstellationSelection, Constellation, false);
            }



            if (Settings.Active.ShowConstellationLabels)
            {
                Constellations.DrawConstellationNames(RenderContext, 1, Colors.Yellow);
            }
        }

        private void DrawHoverText(RenderContext RenderContext)
        {
            if (RenderContext.gl == null)
            {
                CanvasContext2D ctx = RenderContext.Device;
                ctx.Save();

                ctx.FillStyle = "White";
                ctx.Font = "15px Arial";
                ctx.FillText(hoverText, hoverTextPoint.X, hoverTextPoint.Y);
                ctx.Restore();
            }
        }


        public double RAtoViewLng(double ra)
        {
            return (((180 - ((ra) / 24.0 * 360) - 180) + 540) % 360) - 180;
        }

        private const double DragCoefficient = 0.8;

       
        private void UpdateViewParameters()
        {
            if (RenderContext.Space && tracking && trackingObject != null)
            {
                if (Settings.Active.GalacticMode && RenderContext.Space)
                {
                    double[] gPoint = Coordinates.J2000toGalactic(trackingObject.RA * 15, trackingObject.Dec);

                    RenderContext.targetAlt = RenderContext.alt = gPoint[1];
                    RenderContext.targetAz = RenderContext.az = gPoint[0];
                }
                else if (RenderContext.Space && Settings.Active.LocalHorizonMode)
                {
                    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(trackingObject.RA, trackingObject.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                    RenderContext.targetAlt = currentAltAz.Alt;
                    RenderContext.targetAz = currentAltAz.Az;
                }
                else
                {
                    RenderContext.ViewCamera.Lng = RenderContext.TargetCamera.Lng = this.RAtoViewLng(trackingObject.RA);
                    RenderContext.ViewCamera.Lat = RenderContext.TargetCamera.Lat = trackingObject.Dec;
                }
            }
            else if (!SolarSystemMode)
            {
                tracking = false;
                trackingObject = null;
            }

            double oneMinusDragCoefficient = 1 - DragCoefficient;
            double dc = DragCoefficient;

            //if (!Settings.Current.SmoothPan)
            //{
            //    oneMinusDragCoefficient = 1;
            //    dc = 0;
            //}
            if (!tracking)
            {
                double minDelta = (RenderContext.ViewCamera.Zoom / 4000.0);
                if (RenderContext.ViewCamera.Zoom > 360)
                {
                    minDelta = (360.0 / 40000.0);
                }
                //if (RenderContext.Space && Settings.Active.LocalHorizonMode)
                //{
                //    //if (!Settings.Current.SmoothPan)
                //    //{
                //    //    this.alt = targetAlt;
                //    //    this.az = targetAz;
                //    //}

                //    if (((Math.Abs(this.TargetAlt - this.alt) >= (minDelta)) |
                //        ((Math.Abs(this.targetAz - this.az) >= (minDelta)))))
                //    {
                //        this.alt += (targetAlt - alt) / 10;

                //        if (Math.Abs(targetAz - az) > 170)
                //        {
                //            if (targetAz > az)
                //            {
                //                this.az += (targetAz - (360 + az)) / 10;
                //            }
                //            else
                //            {
                //                this.az += ((360 + targetAz) - az) / 10;
                //            }
                //        }
                //        else
                //        {
                //            this.az += (targetAz - az) / 10;
                //        }

                //        //this.az = ((az + 540) % 360) - 180;
                //        this.az = ((az + 720) % 360);

                //    }
                //}
                //else
                
               
                    //if (!Settings.Current.SmoothPan)
                    //{
                    //    this.viewCamera.Lat = this.targetCamera.Lat;
                    //    this.viewCamera.Lng = this.targetCamera.Lng;
                    //}
                if (RenderContext.Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
                {
                    if (((Math.Abs(RenderContext.targetAlt - RenderContext.alt) >= (minDelta)) |
                        ((Math.Abs(RenderContext.targetAz - RenderContext.az) >= (minDelta)))))
                    {
                        RenderContext.alt += (RenderContext.targetAlt - RenderContext.alt) / 10;

                        if (Math.Abs(RenderContext.targetAz - RenderContext.az) > 170)
                        {
                            if (RenderContext.targetAz > RenderContext.az)
                            {
                                RenderContext.az += (RenderContext.targetAz - (360 + RenderContext.az)) / 10;
                            }
                            else
                            {
                                RenderContext.az += ((360 + RenderContext.targetAz) - RenderContext.az) / 10;
                            }
                        }
                        else
                        {
                            RenderContext.az += (RenderContext.targetAz - RenderContext.az) / 10;
                        }
                        RenderContext.az = ((RenderContext.az + 720) % 360);
                    }
                }
                else
                {
                    if (((Math.Abs(RenderContext.TargetCamera.Lat - RenderContext.ViewCamera.Lat) >= (minDelta)) |
                        ((Math.Abs(RenderContext.TargetCamera.Lng - RenderContext.ViewCamera.Lng) >= (minDelta)))))
                    {
                        RenderContext.ViewCamera.Lat += (RenderContext.TargetCamera.Lat - RenderContext.ViewCamera.Lat) / 10;

                        if (Math.Abs(RenderContext.TargetCamera.Lng - RenderContext.ViewCamera.Lng) > 170)
                        {
                            if (RenderContext.TargetCamera.Lng > RenderContext.ViewCamera.Lng)
                            {
                                RenderContext.ViewCamera.Lng += (RenderContext.TargetCamera.Lng - (360 + RenderContext.ViewCamera.Lng)) / 10;
                            }
                            else
                            {
                                RenderContext.ViewCamera.Lng += ((360 + RenderContext.TargetCamera.Lng) - RenderContext.ViewCamera.Lng) / 10;
                            }
                        }
                        else
                        {
                            RenderContext.ViewCamera.Lng += (RenderContext.TargetCamera.Lng - RenderContext.ViewCamera.Lng) / 10;
                        }

                        RenderContext.ViewCamera.Lng = ((RenderContext.ViewCamera.Lng + 720) % 360);
                    }
                    else
                    {
                        if (RenderContext.ViewCamera.Lat != RenderContext.TargetCamera.Lat || RenderContext.ViewCamera.Lng != RenderContext.TargetCamera.Lng)
                        {
                            RenderContext.ViewCamera.Lat = RenderContext.TargetCamera.Lat;
                            RenderContext.ViewCamera.Lng = RenderContext.TargetCamera.Lng;
                        }
                    }
                }
            }



            //if (!tracking)
            //{
            //    this.viewCamera.Lng = dc * this.viewCamera.Lng + oneMinusDragCoefficient * this.targetCamera.Lng;
            //    this.viewCamera.Lat = dc * this.viewCamera.Lat + oneMinusDragCoefficient * this.targetCamera.Lat;
            //}
            RenderContext.ViewCamera.Zoom = dc * RenderContext.ViewCamera.Zoom + oneMinusDragCoefficient * RenderContext.TargetCamera.Zoom;
            RenderContext.ViewCamera.Rotation = dc * RenderContext.ViewCamera.Rotation + oneMinusDragCoefficient * RenderContext.TargetCamera.Rotation;
            RenderContext.ViewCamera.Angle = dc * RenderContext.ViewCamera.Angle + oneMinusDragCoefficient * RenderContext.TargetCamera.Angle;


        }

        //public double GetPixelScaleX(bool mouseRelative)
        //{
        //    double lat = RenderContext.ViewCamera.Lat;

        //    if (mouseRelative)
        //    {
        //        //if (Space && Settings.Active.LocalHorizonMode)
        //        //{
        //        //    Point cursor = renderWindow.PointToClient(Cursor.Position);
        //        //    Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);
        //        //    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(GetCoordinatesForScreenPoint(cursor.X, cursor.Y), SpaceTimeController.Location, SpaceTimeController.Now);

        //        //    lat = currentAltAz.Alt;
        //        //}
        //        //else
        //        {
        //            Point cursor = renderWindow.PointToClient(Cursor.Position);
        //            Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);
        //            lat = result.Lat;
        //        }
        //    }

        //    if (CurrentImageSet != null && (CurrentImageSet.DataSetType == ImageSetType.Sky || CurrentImageSet.DataSetType == ImageSetType.Panorama || SolarSystemMode || CurrentImageSet.DataSetType == ImageSetType.Earth || CurrentImageSet.DataSetType == ImageSetType.Planet))
        //    {
        //        double cosLat = 1;
        //        if (ViewLat > 89.9999)
        //        {
        //            cosLat = Math.Cos(89.9999 * RC);
        //        }
        //        else
        //        {
        //            cosLat = Math.Cos(lat * RC);

        //        }

        //        double zz = (90 - ZoomFactor / 6);
        //        double zcos = Math.Cos(zz * RC);

        //        return GetPixelScaleY() / Math.Max(zcos, cosLat);
        //    }
        //    else
        //    {
        //        return (((baseTileDegrees / ((double)Math.Pow(2, viewTileLevel))) / tileSizeX) / 5) / Math.Max(.2, Math.Cos(targetLat));
        //    }

        //}

        //public double GetPixelScaleY()
        //{
        //    if (SolarSystemMode)
        //    {
        //        if ((int)SolarSystemTrack < (int)SolarSystemObjects.Custom)
        //        {
        //            return Math.Min(.06, 545000 * Math.Tan(Math.PI / 4) * ZoomFactor / renderWindow.ClientRectangle.Height);
        //        }
        //        else
        //        {

        //            return .06;
        //        }
        //    }
        //    else if (CurrentImageSet != null && (CurrentImageSet.DataSetType == ImageSetType.Sky || CurrentImageSet.DataSetType == ImageSetType.Panorama))
        //    {
        //        double val = fovAngle / renderWindow.ClientRectangle.Height;
        //        //if (Properties.Settings.Default.DomeView)
        //        //{
        //        //    val = val / 10;
        //        //}
        //        return val;
        //    }
        //    else
        //    {
        //        return ((baseTileDegrees / ((double)Math.Pow(2, viewTileLevel))) / (double)tileSizeY) / 5;
        //    }
        //}


        //public void MoveView(double amountX, double amountY, bool mouseDrag)
        //{
        //    if (CurrentImageSet == null)
        //    {
        //        return;
        //    }
        //    Tracking = false;
        //    double angle = Math.Atan2(amountY, amountX);
        //    double distance = Math.Sqrt(amountY * amountY + amountX * amountX);
        //    if (SolarSystemMode)
        //    {
        //        amountX = Math.Cos(angle - CameraRotate) * distance;
        //        amountY = Math.Sin(angle - CameraRotate) * distance;
        //    }
        //    else if (!PlanetLike)
        //    {
        //        amountX = Math.Cos(angle + CameraRotate) * distance;
        //        amountY = Math.Sin(angle + CameraRotate) * distance;
        //    }
        //    else
        //    {
        //        amountX = Math.Cos(angle - CameraRotate) * distance;
        //        amountY = Math.Sin(angle - CameraRotate) * distance;
        //    }

        //    MoveViewNative(amountX, amountY, mouseDrag);
        //}


        ///// <summary>
        ///// Move the view relative to screen coordinates without account for the view rotation
        ///// </summary>
        ///// <param name="amountX"></param>
        ///// <param name="amountY"></param>
        //public void MoveViewNative(double amountX, double amountY, bool mouseDrag)
        //{
        //    double scaleY = GetPixelScaleY();
        //    double scaleX = GetPixelScaleX(mouseDrag);

        //    //if (CurrentImageSet.DataSetType == ImageSetType.SolarSystem)
        //    //{
        //    //    if (Settings.Active.ActualPlanetScale)
        //    //    {
        //    //        if (ZoomFactor < .0003)
        //    //        {
        //    //            scaleX *= 1210 / 300;
        //    //            scaleY *= 800 / 300;
        //    //        }
        //    //        else
        //    //        {
        //    //            scaleX = .06;
        //    //            scaleY = .06;
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        if (ZoomFactor < .05)
        //    //        {
        //    //            scaleX *= 1210;
        //    //            scaleY *= 800;
        //    //        }
        //    //        else
        //    //        {
        //    //            scaleX = .06;
        //    //            scaleY = .06;
        //    //        }
        //    //    }
        //    //}

        //    if (CurrentImageSet.DataSetType == ImageSetType.SolarSystem)
        //    {
        //        if (scaleY > .05999)
        //        {
        //            scaleX = scaleY;
        //        }
        //    }

        //    if (Space && Settings.Active.LocalHorizonMode)
        //    {
        //        targetAlt += (amountY) * scaleY;
        //        if (targetAlt > Properties.Settings.Default.MaxLatLimit)
        //        {
        //            targetAlt = Properties.Settings.Default.MaxLatLimit;
        //        }
        //        if (targetAlt < -Properties.Settings.Default.MaxLatLimit)
        //        {
        //            targetAlt = -Properties.Settings.Default.MaxLatLimit;
        //        }

        //    }
        //    else
        //    {
        //        TargetLat += (amountY) * scaleY;

        //        if (TargetLat > Properties.Settings.Default.MaxLatLimit)
        //        {
        //            TargetLat = Properties.Settings.Default.MaxLatLimit;
        //        }
        //        if (TargetLat < -Properties.Settings.Default.MaxLatLimit)
        //        {
        //            TargetLat = -Properties.Settings.Default.MaxLatLimit;
        //        }
        //    }
        //    if (Space && Settings.Active.LocalHorizonMode)
        //    {
        //        targetAz = ((targetAz + amountX * scaleX) + 720) % 360;
        //    }
        //    else
        //    {
        //        TargetLong += (amountX) * scaleX;

        //        TargetLong = ((TargetLong + 900.0) % 360.0) - 180.0;
        //    }
        //}


        public void Move(double x, double y)
        {
            double scaleY = RenderContext.FovScale / (3600.0);

            if (RenderContext.BackgroundImageset.DataSetType == ImageSetType.SolarSystem)
            {
                scaleY = .06;
            }


            double scaleX = scaleY / Math.Max(.2, Math.Cos(RenderContext.ViewCamera.Lat / 180.0 * Math.PI));




            if (RenderContext.BackgroundImageset.DataSetType == ImageSetType.Earth || RenderContext.BackgroundImageset.DataSetType == ImageSetType.Planet || RenderContext.BackgroundImageset.DataSetType == ImageSetType.SolarSystem)
            {
                scaleX = scaleX * 6.3;
                scaleY = scaleY * 6.3;
            }



            if (RenderContext.Space && (Settings.Active.GalacticMode || Settings.Active.LocalHorizonMode))
            {
                x = Settings.Active.LocalHorizonMode ? -x : x;
                RenderContext.targetAz += x * scaleX;
                RenderContext.targetAz = ((RenderContext.targetAz + 720) % 360);
                RenderContext.targetAlt += y * scaleY;

                if (RenderContext.targetAlt > 90)
                {
                    RenderContext.targetAlt = 90;
                }

                if (RenderContext.targetAlt < -90)
                {
                    RenderContext.targetAlt = -90;
                }
            }
            else
            {
                RenderContext.TargetCamera.Lng -= x * scaleX;

                RenderContext.TargetCamera.Lng = ((RenderContext.TargetCamera.Lng + 720) % 360);

                RenderContext.TargetCamera.Lat += y * scaleY;

                if (RenderContext.TargetCamera.Lat > 90)
                {
                    RenderContext.TargetCamera.Lat = 90;
                }

                if (RenderContext.TargetCamera.Lat < -90)
                {
                    RenderContext.TargetCamera.Lat = -90;
                }
            }

            if (!Settings.GlobalSettings.SmoothPan)
            {
                RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();
            }


            if (x != 0 && y != 0)
            {
                tracking = false;
                trackingObject = null;
            }
        }

        public void Zoom(double factor)
        {

            RenderContext.TargetCamera.Zoom *= factor;

            if (RenderContext.TargetCamera.Zoom > ZoomMax)
            {
                RenderContext.TargetCamera.Zoom = ZoomMax;
            }

            if (!Settings.GlobalSettings.SmoothPan)
            {
                RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();
            }

        }

        CanvasElement foregroundCanvas = null;
        CanvasContext2D fgDevice = null;
        Folder webFolder;
        public void Setup(CanvasElement canvas)
        {
            Window.AddEventListener("contextmenu", OnContextMenu, false);
            canvas.AddEventListener("dblclick", OnDoubleClick, false);
            //canvas.AddEventListener("mousemove", OnMouseMove, false);
            //canvas.AddEventListener("mouseup", OnMouseUp, false);
            //canvas.AddEventListener("pointerdown", OnPointerDown, false);
            canvas.AddEventListener("mousedown", OnMouseDown, false);
            canvas.AddEventListener("mousewheel", OnMouseWheel, false);
            canvas.AddEventListener("DOMMouseScroll", OnMouseWheel, false);  // this is for firefox as it does not support mousewheel
            canvas.AddEventListener("touchstart", OnTouchStart, false);
            canvas.AddEventListener("touchmove", OnTouchMove, false);
            canvas.AddEventListener("touchend", OnTouchEnd, false);
            canvas.AddEventListener("gesturechange", OnGestureChange, false);
            canvas.AddEventListener("gesturestart", OnGestureStart, false);
            canvas.AddEventListener("gestureend", OnGestureEnd, false);  
            Document.Body.AddEventListener("keydown", OnKeyDown, false); 
            //canvas.AddEventListener("MSGestureChange", OnGestureChange, false);  
            //canvas.AddEventListener("mouseout", OnMouseUp, false);

            // MS Touch code
            canvas.AddEventListener("pointerdown", OnPointerDown, false);

            canvas.AddEventListener("pointermove", OnPointerMove, false);

            canvas.AddEventListener("pointerup", OnPointerUp, false);


            // End MS touch code

            RenderContext.ViewCamera.Lat = StartLat;
            RenderContext.ViewCamera.Lng = StartLng;
            RenderContext.ViewCamera.Zoom = StartZoom;

            RenderContext.TargetCamera = RenderContext.ViewCamera.Copy();

            if (RenderContext.gl == null)
            {

                foregroundCanvas = (CanvasElement)Document.CreateElement("canvas");
                foregroundCanvas.Width = canvas.Width;
                foregroundCanvas.Height = canvas.Height;
                fgDevice = (CanvasContext2D)foregroundCanvas.GetContext(Rendering.Render2D);
            }
            webFolder = new Folder();
            webFolder.LoadFromUrl("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=ImageSets5", SetupComplete);

            WebFile webFile = new WebFile("http://www.worldwidetelescope.org/wwtweb/weblogin.aspx?user=12345678-03D2-4935-8D0F-DCE54C9113E5&Version=HTML5&webkey=AX2011Gqqu&platform=web");
            webFile.Send();
        }

        public void SetupComplete()
        {
            Wtml.LoadImagesets(webFolder);
            scriptInterface.FireReady();
        }

        public static void ShowExplorerUI()
        {
            if (Singleton != null)
            {
                Singleton.CreateExplorerUI();
            }
        }

        public void CreateExplorerUI()
        {
            if (Explorer == null)
            {
                Explorer = FolderBrowser.Create();

                DivElement div = (DivElement)Document.GetElementById("UI");

                div.InsertBefore(Explorer.Canvas);



                ExploreRoot = new Folder();
                ExploreRoot.LoadFromUrl("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=NewExploreRoot",
                    delegate { Explorer.AddItems(WWTControl.ExploreRoot.Children); Explorer.Refresh(); });
            }
        }

        public void OnKeyDown(ElementEvent e)
        {
            if (uiController != null)
            {
                uiController.KeyDown(this, e);
            }
        }

        public void OnDoubleClick(ElementEvent e)
        {
            showDataLayers = true;
        }

        public void OnGestureStart(ElementEvent e)
        {
            mouseDown = false;
            beginZoom = RenderContext.ViewCamera.Zoom;
        }
        double beginZoom = 1;
        public void OnGestureChange(ElementEvent e)
        {
            GestureEvent g = (GestureEvent)e;
            mouseDown = false;
            RenderContext.TargetCamera.Zoom = RenderContext.ViewCamera.Zoom = Math.Min(360, beginZoom * (1.0 / g.Scale));
        }


        public void OnGestureEnd(ElementEvent e)
        {
            GestureEvent g = (GestureEvent)e;
            mouseDown = false;
         
        }

        private bool Annotationclicked(double ra, double dec, double x, double y)
        {
            if (annotations != null && annotations.Count > 0)
            {
                int index = 0;
                foreach (Annotation note in annotations)
                {
                    if (note.HitTest(RenderContext, ra, dec, x, y))
                    {
                        scriptInterface.FireAnnotationclicked(ra, dec, note.ID);
                        return true;
                    }
                    index++;
                }
            }
            return false;
        }

        string hoverText = "";
        Vector2d hoverTextPoint = new Vector2d();
        Date lastMouseMove = new Date(1900, 1, 0, 0, 0, 0, 0);

        private bool AnnotationHover(double ra, double dec, double x, double y)
        {
            if (annotations != null && annotations.Count > 0)
            {
                int index = 0;
                foreach (Annotation note in annotations)
                {
                    if (note.HitTest(RenderContext, ra, dec, x, y))
                    {
                        hoverText = note.Label;
                        hoverTextPoint = Vector2d.Create(x, y);
                        return true;
                    }
                    index++;
                }
            }
            return false;
        }

        //internal void FireAnnotationclicked(double RA, double Dec, string id)
        //{
        //    try
        //    {
        //        Script.Literal("wwtAnotationclicked({0},{1},{2});", RA * 15, Dec, id);
        //    }
        //    catch
        //    {
        //    }
        //}

        //public void OnGestureChange(ElementEvent e)
        //{
        //    GestureEvent g = (GestureEvent)e;
        //    mouseDown = false;
        //    double delta = g.Scale;

        //    if (delta > 1 && Math.Abs(delta - 1) > .05)
        //    {
        //        Zoom(0.95);
        //    }
        //    else
        //    {
        //        Zoom(1.05);
        //    }
        //}

        bool isPintching = false;
        public void OnTouchStart(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)(object)e;
            ev.PreventDefault();
            ev.StopPropagation();

          //  Document.Title = "touched by an event ";

            lastX = ev.TargetTouches[0].PageX;
            lastY = ev.TargetTouches[0].PageY;

            if (ev.TargetTouches.Length == 2)
            {
                isPintching = true;
                return;
            }
            else if (uiController != null)
            {

                WWTElementEvent ee = new WWTElementEvent(lastX, lastY);

                if (uiController.MouseDown(this, (ElementEvent)(object)ee))
                {
                    mouseDown = false;
                    dragging = false;
                    return;
                }
            }


            mouseDown = true;
    
        }

        int[] pointerIds = new int[2];

        public void OnPointerDown(ElementEvent e)
        {
            PointerEvent pe = (PointerEvent)(object)e;
            int index = 0;
            //Canvas..SetPointerCapture(pe.PointerId);


            Script.Literal("var evt = arguments[0], cnv = arguments[0].target; if (cnv.setPointerCapture) {cnv.setPointerCapture(evt.pointerId);} else if (cnv.msSetPointerCapture) { cnv.msSetPointerCapture(evt.pointerId); }");
            if (pointerIds[0] == 0 )
            {
                pointerIds[0] = pe.PointerId;
                index = 0;
            }
            else
            {
                if (pointerIds[1] == 0)
                {
                    pointerIds[1] = pe.PointerId;
                    index = 1;
                }
                else
                {
                    // to many pointers don't track
                    return;
                }
            }
            rect[index] = Vector2d.Create(e.OffsetX, e.OffsetY);

        }

        public void OnPointerMove(ElementEvent e)
        {
            PointerEvent pe = (PointerEvent)(object)e;
            int index = 0;

            if (pointerIds[0] == pe.PointerId)
            {
                index = 0;
            }
            else
            {
                if (pointerIds[1] == pe.PointerId)
                {
                    index = 1;
                }
                else
                {
                    // Not interested in a pointer not on our list
                    return;
                }
            }

            if (pointerIds[0] != 0 && pointerIds[1] != 0)
            {
                // Now we know we are zooming...
                if (rect[0] != null)
                {
                    double oldDist = GetDistance(rect[0], rect[1]);
                    rect[index] = Vector2d.Create(e.OffsetX, e.OffsetY);
                    double newDist = GetDistance(rect[0], rect[1]);
                    double ratio = oldDist / newDist;
                    Zoom(ratio);
                }
                e.StopPropagation();
                e.PreventDefault();
            }

            rect[index] = Vector2d.Create(e.OffsetX, e.OffsetY);

        }

        public void OnPointerUp(ElementEvent e)
        {
            PointerEvent pe = (PointerEvent)(object)e;

            if (pointerIds[0] == pe.PointerId)
            {
                pointerIds[0] = 0;
            }
            else
            {
                if (pointerIds[1] == pe.PointerId)
                {
                    pointerIds[1] = 0;
                }
                else
                {
                    // Not in the list to remove
                    return;
                }
            }


        }


        bool dragging = false;
        public void OnTouchMove(ElementEvent e)
        {
          //  Document.Title = "touched by an event ";
            TouchEvent ev = (TouchEvent)e;

            if (isPintching)
            {
                PinchMove(ev);
                return;
            }


            ev.PreventDefault();
            ev.StopPropagation();
            if (mouseDown)
            {
                dragging = true;
                double curX = ev.TargetTouches[0].PageX - lastX;

                double curY = ev.TargetTouches[0].PageY - lastY;

                Move(curX, curY);

                lastX = ev.TargetTouches[0].PageX;
                lastY = ev.TargetTouches[0].PageY;
            }
            else
            {
                //todo fix this to use syntheszed touch events.
                if (uiController != null)
                {
                    if (uiController.MouseMove(this, e))
                    {
                        e.PreventDefault();
                        e.StopPropagation();
                        return;
                    }
                }
            }
        }

        public void OnTouchEnd(ElementEvent e)
        {
      //      Document.Title = "touched by an event ";
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            ev.StopPropagation();

            rect = new Vector2d[2];

            if (isPintching)
            {
                if (ev.Touches.Length < 2)
                {
                    isPintching = false;
                }
                return;
            }

            if (uiController != null)
            {

                WWTElementEvent ee = new WWTElementEvent(lastX, lastY);

                if (uiController.MouseUp(this, (ElementEvent)(object)ee))
                {
                    mouseDown = false;
                    dragging = false;
                    return;
                }
            }

            mouseDown = false;
            dragging = false;
        }

        Vector2d[] rect = new Vector2d[2];
        public void pinchStart(TouchEvent ev)
        {        
            TouchInfo t0 = ev.Touches[0];
            TouchInfo t1 = ev.Touches[1];
            rect[0] = Vector2d.Create( t0.PageX,  t0.PageY );
            rect[1] = Vector2d.Create( t1.PageX,  t1.PageY );
            ev.StopPropagation();
            ev.PreventDefault();
       //     Document.Title = "pinched by an event ";
        }


        public void PinchMove(TouchEvent ev)
        {
            TouchInfo t0 = ev.Touches[0];
            TouchInfo t1 = ev.Touches[1];
            Vector2d[] newRect = new Vector2d[2];

            newRect[0] = Vector2d.Create(t0.PageX, t0.PageY);
            newRect[1] = Vector2d.Create(t1.PageX, t1.PageY);
            if (rect[0] != null)
            {
                double oldDist = GetDistance(rect[0], rect[1]);
                double newDist = GetDistance(newRect[0], newRect[1]);
                double ratio = oldDist / newDist;
                Zoom(ratio);
            }
            rect = newRect;
            ev.StopPropagation();
            ev.PreventDefault();
        }


        public double GetDistance(Vector2d a, Vector2d b)
        {
            
            double x;
            double y;
            x = a.X - b.X;
            y = a.Y - b.Y;
            return Math.Sqrt(x * x + y * y);
        }


        bool mouseDown = false;
        double lastX;
        double lastY;
        public void OnMouseDown(ElementEvent e)
        {
            // Capture mouse
            

            Document.AddEventListener("mousemove", OnMouseMove, false);
            Document.AddEventListener("mouseup", OnMouseUp, false);

            if (uiController != null)
            {
                if (uiController.MouseDown(this, e))
                {
                    return;
                }
            }

            mouseDown = true;
            lastX = Mouse.OffsetX(Canvas, e);
            lastY = Mouse.OffsetY(Canvas, e);
        }

        public void OnContextMenu(ElementEvent e)
        {
            e.PreventDefault();
            e.StopPropagation();
            
        }


        public void OnMouseMove(ElementEvent e)
        {
           
            lastMouseMove = Date.Now;
            hoverTextPoint = Vector2d.Create( Mouse.OffsetX(Canvas, e), Mouse.OffsetY(Canvas, e));
            hoverText = "";

            if (mouseDown)
            {
                e.PreventDefault();
                e.StopPropagation();

                moved = true;
                if (e.CtrlKey)
                {
                    Tilt(Mouse.OffsetX(Canvas, e) - lastX, Mouse.OffsetY(Canvas, e) - lastY);
                }
                else
                {
                    Move(Mouse.OffsetX(Canvas, e) - lastX, Mouse.OffsetY(Canvas, e) - lastY);
                }

                lastX = Mouse.OffsetX(Canvas, e);
                lastY = Mouse.OffsetY(Canvas, e);
            }
            else
            {
                if (uiController != null)
                {
                    if (uiController.MouseMove(this, e))
                    {
                        e.PreventDefault();
                        e.StopPropagation();
                        return;
                    }
                }
            }
        }

        private void Tilt(double x, double y)
        {


            RenderContext.TargetCamera.Rotation += x * .001;
            RenderContext.TargetCamera.Angle += y * .001;

            if (RenderContext.TargetCamera.Angle < -1.52)
            {
                RenderContext.TargetCamera.Angle = -1.52;
            }

            if (RenderContext.TargetCamera.Angle > 0)
            {
                RenderContext.TargetCamera.Angle = 0;
            }
         
        }
        bool moved = false;
        public void OnMouseUp(ElementEvent e)
        {
            Document.RemoveEventListener("mousemove", OnMouseMove, false);
            Document.RemoveEventListener("mouseup", OnMouseUp, false);
            if (uiController != null)
            {
                if (uiController.MouseUp(this, e))
                {
                    mouseDown = false;
                    e.PreventDefault();
                    return;
                }
            }
            if (mouseDown && !moved)
            {
                Vector2d raDecDown = GetCoordinatesForScreenPoint(Mouse.OffsetX(Canvas, e), Mouse.OffsetY(Canvas, e));
                if (!Annotationclicked(raDecDown.X, raDecDown.Y, Mouse.OffsetX(Canvas, e), Mouse.OffsetY(Canvas, e)))
                {
                    scriptInterface.FireClick(raDecDown.X, raDecDown.Y);
                }
            }
            mouseDown = false;

            moved = false;
            
        }

        public Vector2d GetCoordinatesForScreenPoint(double x, double y)
        {
            Vector2d result;
            Vector3d PickRayOrig;
            Vector3d PickRayDir;
            Vector2d pt = Vector2d.Create(x, y);
            PickRayDir = TransformPickPointToWorldSpace(pt, RenderContext.Width, RenderContext.Height);
            result = Coordinates.CartesianToSphericalSky(PickRayDir);
            
            return result;
        }

        public Vector3d TransformPickPointToWorldSpace(Vector2d ptCursor, double backBufferWidth, double backBufferHeight)
        {

            Vector3d vPickRayOrig;
            Vector3d vPickRayDir;

            Vector3d v = new Vector3d();
            v.X = (((2.0f * ptCursor.X) / backBufferWidth) - 1) / (RenderContext.Projection.M11);// / (backBufferWidth / 2));
            v.Y = (((2.0f * ptCursor.Y) / backBufferHeight) - 1) / (RenderContext.Projection.M22);// / (backBufferHeight / 2));
            v.Z = 1.0f;


            Matrix3d m = Matrix3d.MultiplyMatrix(RenderContext.View, RenderContext.World);

            m.Invert();

            vPickRayDir = new Vector3d();
            vPickRayOrig = new Vector3d();
            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();


            return vPickRayDir;
        }

        //internal void FireClick(double RA, double Dec)
        //{
        //    try
        //    {
        //        Script.Literal("wwtClick({0},{1});", RA * 15, Dec);
        //    }
        //    catch
        //    {
        //    }
        //}

        //internal void FireArrived(double RA, double Dec)
        //{
        //    try
        //    {
        //        Script.Literal("wwtArrived({0},{1});", RA * 15, Dec);
        //    }
        //    catch
        //    {
        //    }
        //}

        public void OnMouseWheel(ElementEvent e)
        {
            WheelEvent ev = (WheelEvent)(object)e;

            // different browsers use different data variable
            double delta;
            if (ev.detail != 0)
                delta = ev.detail * -1;
            else
                delta = ev.WheelDelta / 40;

            //cancelEvent(e);

            if (delta > 0)
            {
                Zoom(0.9);
            }
            else
            {
                Zoom(1.1);
            }

        }
        public static ScriptInterface scriptInterface;
        public static ScriptInterface InitControl(string DivId)
        {
            return InitControlParam(DivId, false);
        }

        public static ScriptInterface InitControlParam(string DivId, bool webGL)
        {
            if (Singleton.RenderContext.Device == null)
            {
                scriptInterface = new ScriptInterface();
                scriptInterface.Settings = Settings.Current;

                CanvasElement canvas = CreateCanvasElement(DivId);


                String webgltext = "webgl";
                GL gl = null;


                //todo remove this line to turn WebGL on...
                webGL = true; 

                if (webGL)
                {
                    gl = (GL)(Object)canvas.GetContext((Rendering)(object)webgltext);
                }
                if (gl == null)
                {
                    webgltext = "experimental-webgl";
                    gl = (GL)(Object)canvas.GetContext((Rendering)(object)webgltext);
                }

                if (gl == null)
                {

                    CanvasContext2D ctx = (CanvasContext2D)canvas.GetContext(Rendering.Render2D);

                    Singleton.RenderContext.Device = ctx;

                }
                else
                {
                    Tile.PrepDevice = gl;
                    Singleton.RenderContext.gl = gl;
                 
                    RenderContext.UseGl = true;
                }

                Singleton.Canvas = canvas;
                Singleton.RenderContext.Width = canvas.Width;
                Singleton.RenderContext.Height = canvas.Height;
                Singleton.Setup(canvas);


                Singleton.RenderContext.BackgroundImageset =
                    Imageset.Create(
                    "DSS",
                    "http://cdn.worldwidetelescope.org/wwtweb/dss.aspx?q={1},{2},{3}",
                    ImageSetType.Sky, BandPass.Visible, ProjectionType.Toast, 100,
                    0, 12, 256, 180, ".png", false, "", 0, 0, 0, false,
                    "http://www.worldwidetelescope.org/thumbnails/DSS.png",
                    true, false, 0, 0, 0, "", "", "", "", 1, "Sky");


                if (StartMode == "earth")
                {
                    Singleton.RenderContext.BackgroundImageset =
                        Imageset.Create(
                        "Blue Marble",
                        "http://www.worldwidetelescope.org/wwtweb/tiles.aspx?q={1},{2},{3},bm200407",
                        ImageSetType.Earth, BandPass.Visible, ProjectionType.Toast, 101,
                        0, 7, 256, 180, ".png", false, "", 0, 0, 0, false,
                        "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=bm200407",
                        true, false, 0, 0, 0, "", "", "", "", 6371000, "Earth");


                }
                if (StartMode == "bing")
                {
                    Singleton.RenderContext.BackgroundImageset =
                     Imageset.Create(
                     "Virtual Earth Aerial",
                     "http://a{0}.ortho.tiles.virtualearth.net/tiles/a{1}.jpeg?g=15",
                     ImageSetType.Earth, BandPass.Visible, ProjectionType.Mercator, 102,
                     1, 20, 256, 360, ".png", false, "0123", 0, 0, 0, false,
                     "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=earth",
                     true, false, 0, 0, 0, "", "", "", "", 6371000, "Earth");
                }
               
                //if (StartMode == "bing")
                //{
                //    Singleton.RenderContext.BackgroundImageset =
                //     Imageset.Create(
                //     "3D Solar System View",
                //     "",
                //     ImageSetType.SolarSystem, BandPass.Visible, ProjectionType.Toast, 10112,
                //     1, 20, 256, 360, ".png", false, "0123", 0, 0, 0, false,
                //     "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=SolarSytem",
                //     true, false, 0, 0, 0, "", "", "", "", 6371000, "3D Solar System View");
                //}

            }

            //UseUserLocation()

            Singleton.RenderContext.ViewCamera.Lng += 0;

            
            Singleton.RenderContext.InitGL();


            Singleton.Render();

            return scriptInterface;
        }

        public static void UseUserLocation()
        {
            Navigator.Geolocation.GetCurrentPosition(GetLocation, GetLocationError);
        }

        private static void GetLocation(Position pos)
        {
            if (pos.Coords.Latitude != 0)
            {
                Settings.GlobalSettings.LocationLat = pos.Coords.Latitude;
            }
            if (pos.Coords.Longitude != 0)
            {
                Settings.GlobalSettings.LocationLng = pos.Coords.Longitude;
            }
            if (pos.Coords.Altitude != 0)
            {
                Settings.GlobalSettings.LocationAltitude = pos.Coords.Altitude;
            }
        }
        private static void GetLocationError (Position pos)
        {
            if (pos != null && pos.Coords != null)
            {
                double lat = pos.Coords.Latitude;
                double lng = pos.Coords.Longitude;

            }
        }
        private static CanvasElement CreateCanvasElement(string DivId)
        {
            CanvasElement canvas = null;

            DivElement div = (DivElement)Document.GetElementById(DivId);
            ElementAttribute style = div.Attributes.GetNamedItem("style");
            canvas = (CanvasElement)Document.CreateElement("canvas");
            canvas.Height = int.Parse(div.Style.Height);
            canvas.Width = int.Parse(div.Style.Width);
            //canvas.Attributes.SetNamedItem(style);
            div.AppendChild(canvas);
            return canvas;
        }

        public static void ShowFolderUI()
        {
            WWTControl.Singleton.CreateExplorerUI();
        }

        //static void Move(double x, double y)
        //{
        //    ViewRenderer.Singleton.Move(x, y);
        //}

        //static void Zoom(double amount)
        //{
        //    ViewRenderer.Singleton.Zoom(amount);
        //}

        public static void Go(string mode, double lat, double lng, double zoom)
        {
            if (mode != null && mode.Length > 0)
            {
                WWTControl.StartMode = mode;
            }

            if (zoom != 0)
            {
                WWTControl.StartLat = lat;
                WWTControl.StartLng = lng;


                WWTControl.StartZoom = zoom * 6;
            }
        }

        public void GotoRADecZoom(double ra, double dec, double zoom, bool instant)
        {
            ra = DoubleUtilities.Clamp(ra, 0, 24);
            dec = DoubleUtilities.Clamp(dec, -90, 90);
            zoom = DoubleUtilities.Clamp(zoom, ZoomMin, ZoomMax);

            tracking = false;
            trackingObject = null;
            GotoTargetFull(false, instant, CameraParameters.Create(dec, WWTControl.Singleton.RenderContext.RAtoViewLng(ra), zoom, WWTControl.Singleton.RenderContext.ViewCamera.Rotation, WWTControl.Singleton.RenderContext.ViewCamera.Angle, (float)WWTControl.Singleton.RenderContext.ViewCamera.Opacity), WWTControl.Singleton.RenderContext.ForegroundImageset, WWTControl.Singleton.RenderContext.BackgroundImageset);
        }


        bool tracking = false;
        Place trackingObject = null;

        public bool SandboxMode = false;

        public bool SolarSystemMode
        {
            get
            {
                if (RenderContext.BackgroundImageset == null)
                {
                    return false;
                }

                return RenderContext.BackgroundImageset.DataSetType == ImageSetType.SolarSystem;
            }
        }

        SolarSystemObjects SolarSystemTrack = SolarSystemObjects.Undefined;
        public void GotoTarget(Place place, bool noZoom, bool instant, bool trackObject)
        {
            if (place == null)
            {
                return;
            }
            if ((trackObject && SolarSystemMode))
            {
                if ((place.Classification == Classification.SolarSystem && place.Type != ImageSetType.SolarSystem) || (place.Classification == Classification.Star) || (place.Classification == Classification.Galaxy) && place.Distance > 0)
                {
                    SolarSystemObjects target = SolarSystemObjects.Undefined;

                    if (place.Classification == Classification.Star || place.Classification == Classification.Galaxy)
                    {
                        target = SolarSystemObjects.Custom;
                    }
                    else
                    {
                        try
                        {
                            if (place.Target != SolarSystemObjects.Undefined)
                            {
                                target = place.Target;
                            }
                            else
                            {
                                target = (SolarSystemObjects)Planets.GetPlanetIDFromName(place.Name);
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (target != SolarSystemObjects.Undefined)
                    {
                        trackingObject = place;
                        if (target == SolarSystemTrack && !(place.Classification == Classification.Star || place.Classification == Classification.Galaxy))
                        {
                            GotoTarget3(place.CamParams, noZoom, instant);
                            return;
                        }
                        double jumpTime = 4;

                        if (target == SolarSystemObjects.Custom)
                        {
                            jumpTime = 17;
                        }
                        else
                        {
                            jumpTime += 13 * (101 - Settings.Active.SolarSystemScale) / 100;
                        }

                        if (instant)
                        {
                            jumpTime = 1;
                        }

                        //SolarSystemTrack = target;
                        CameraParameters camTo = RenderContext.ViewCamera.Copy();
                        camTo.TargetReferenceFrame = "";
                        camTo.Target = target;
                        double zoom = 10;
                        if (target == SolarSystemObjects.Custom)
                        {
                            if (place.Classification == Classification.Galaxy)
                            {
                                zoom = 1404946007758;
                            }
                            else
                            {
                                zoom = 63239.6717 * 100;
                            }
                            // Star or something outside of SS
                            Vector3d vect = Coordinates.RADecTo3dAu(place.RA, place.Dec, place.Distance);
                            double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                            vect.RotateX(ecliptic);
                            camTo.ViewTarget = Vector3d.Negate(camTo.ViewTarget);
                        }
                        else
                        {
                            camTo.ViewTarget = Planets.GetPlanet3dLocationJD(target, SpaceTimeController.GetJNowForFutureTime(jumpTime));
                            switch (target)
                            {
                                case SolarSystemObjects.Sun:
                                    zoom = .6;
                                    break;
                                case SolarSystemObjects.Mercury:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Venus:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Mars:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Jupiter:
                                    zoom = .007;
                                    break;
                                case SolarSystemObjects.Saturn:
                                    zoom = .007;
                                    break;
                                case SolarSystemObjects.Uranus:
                                    zoom = .004;
                                    break;
                                case SolarSystemObjects.Neptune:
                                    zoom = .004;
                                    break;
                                case SolarSystemObjects.Pluto:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Moon:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Io:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Europa:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Ganymede:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Callisto:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Earth:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Custom:
                                    zoom = 10;
                                    break;

                                default:
                                    break;
                            }

                            zoom = zoom * Settings.Active.SolarSystemScale;

                        }

                        CameraParameters fromParams = RenderContext.ViewCamera.Copy();
                        if (SolarSystemTrack == SolarSystemObjects.Custom && !string.IsNullOrEmpty(RenderContext.TrackingFrame))
                        {
                            fromParams =  RenderContext.CustomTrackingParams;
                            RenderContext.TrackingFrame = "";
                        }
                        camTo.Zoom = zoom;
                        Vector3d toVector = camTo.ViewTarget;
                        toVector.Subtract(fromParams.ViewTarget);

                        //Vector3d toVector = camTo.ViewTarget;
                        //toVector.Subtract(new Vector3d(cameraPosition));

                        if (place.Classification == Classification.Star)
                        {
                            toVector = Vector3d.Negate(toVector);
                        }

                        if (toVector.Length() != 0)
                        {

                            Vector2d raDec = toVector.ToRaDec();

                            if (target == SolarSystemObjects.Custom)
                            {
                                camTo.Lat = -raDec.Y;
                            }
                            else
                            {
                                camTo.Lat = raDec.Y;
                            }
                            camTo.Lng = raDec.X * 15 - 90;
                        }
                        else
                        {
                            camTo.Lat = RenderContext.ViewCamera.Lat;
                            camTo.Lng = RenderContext.ViewCamera.Lng;
                        }

                        if (target != SolarSystemObjects.Custom)
                        {
                            // replace with planet surface
                            camTo.ViewTarget = Planets.GetPlanetTargetPoint(target, camTo.Lat, camTo.Lng, SpaceTimeController.GetJNowForFutureTime(jumpTime));

                        }



                        ViewMoverKenBurnsStyle solarMover = new ViewMoverKenBurnsStyle(fromParams, camTo, jumpTime, SpaceTimeController.Now, SpaceTimeController.GetTimeForFutureTime(jumpTime), InterpolationType.EaseInOut);
                        solarMover.FastDirectionMove = true;
                        Mover = solarMover;

                        return;
                    }
                }
            }




            tracking = false;
            trackingObject = null;
            CameraParameters camParams = place.CamParams.Copy();


            // (gonzalo) backgroundimageset could be null... protect onself!
            if (RenderContext.BackgroundImageset != null && place.Type != RenderContext.BackgroundImageset.DataSetType)
            {
                RenderContext.TargetCamera = place.CamParams.Copy();
                RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();
                RenderContext.BackgroundImageset = GetDefaultImageset(place.Type, BandPass.Visible);
                instant = true;
            }
            else if (SolarSystemMode && place.Target != SolarSystemTrack)
            {
                RenderContext.TargetCamera = place.CamParams.Copy();
                RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();
                SolarSystemTrack = place.Target;
                instant = true;
            }


            if (place.Classification == Classification.Constellation)
            {
                camParams.Zoom = ZoomMax;
                GotoTargetFull(false, instant, camParams, null, null);
            }
            else
            {
                SolarSystemTrack = place.Target;
                GotoTargetFull(noZoom, instant, camParams, place.StudyImageset, place.BackgroundImageset);
                //if (place.Classification == Classification.SolarSystem)
                if (trackObject)
                {
                    tracking = true;
                    trackingObject = place;
                }
            }

        }


        public void GotoTarget3(CameraParameters camParams, bool noZoom, bool instant)
        {
            tracking = false;
            trackingObject = null;
            GotoTargetFull(noZoom, instant, camParams, RenderContext.ForegroundImageset, RenderContext.BackgroundImageset);

        }
        public void GotoTargetFull(bool noZoom, bool instant, CameraParameters cameraParams, Imageset studyImageSet, Imageset backgroundImageSet)
        {
            RenderNeeded = true;
            //if (cameraParams == this.viewCamera)
            //{
            //    instant = true;
            //}
            tracking = false;
            trackingObject = null;
            targetStudyImageset = studyImageSet;
            targetBackgroundImageset = backgroundImageSet;


            if (noZoom)
            {
                cameraParams.Zoom = RenderContext.ViewCamera.Zoom;
                cameraParams.Angle = RenderContext.ViewCamera.Angle;
                cameraParams.Rotation = RenderContext.ViewCamera.Rotation;
            }
            else
            {
                if (cameraParams.Zoom == -1 || cameraParams.Zoom == 0)
                {
                    if (RenderContext.Space)
                    {
                        cameraParams.Zoom = 1.40625;
                    }
                    else
                    {
                        cameraParams.Zoom = 0.09F;
                    }
                }
            }

            // if (instant || (Math.Abs(ViewLat - cameraParams.Lat) < .000000000001 && Math.Abs(ViewLong - cameraParams.Lng) < .000000000001 && Math.Abs(ZoomFactor - cameraParams.Zoom) < .000000000001))
            if (instant || (Math.Abs(RenderContext.ViewCamera.Lat - cameraParams.Lat) < .000000000001 && Math.Abs(RenderContext.ViewCamera.Lng - cameraParams.Lng) < .000000000001 && Math.Abs(RenderContext.ViewCamera.Zoom - cameraParams.Zoom) < .000000000001))
            {
                Mover = null;
                RenderContext.TargetCamera = cameraParams.Copy();
                RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();

                //if (Space && Settings.Active.LocalHorizonMode)
                //{
                //    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(viewCamera.RA, viewCamera.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                //    targetAlt = alt = currentAltAz.Alt;
                //    targetAz = az = currentAltAz.Az;
                //}
                mover_Midpoint();
                moving = true;
            }
            else
            {

                Mover = ViewMoverSlew.Create(RenderContext.ViewCamera, cameraParams);
                RenderNeeded = true;
                Mover.Midpoint = mover_Midpoint;
            }
        }

        internal void FreezeView()
        {
            RenderContext.ViewCamera = RenderContext.TargetCamera.Copy();
            Mover = null;
        }
        

        internal IViewMover Mover
        {
            get { return RenderContext.ViewMover; }
            set
            {
                RenderContext.ViewMover = value;
                RenderNeeded = true;
            }
        }
        bool moving = false;
        public void FadeInImageSet(Imageset newImageSet)
        {
            if (RenderContext.BackgroundImageset != null &&
                newImageSet.DataSetType != RenderContext.BackgroundImageset.DataSetType)
            {
                TileCache.PurgeQueue();
                TileCache.ClearCache();
            }
            RenderContext.BackgroundImageset = newImageSet;
        }
        Imageset targetStudyImageset = null;
        Imageset targetBackgroundImageset = null;


        void mover_Midpoint()
        {
            if ((targetStudyImageset != null && RenderContext.ForegroundImageset == null) || (RenderContext.ForegroundImageset != null && !RenderContext.ForegroundImageset.Equals(targetStudyImageset)))
            {
                RenderContext.ForegroundImageset = targetStudyImageset;
            }

            //(gonzalo) protect from backgroundImageset being null ...
            if (RenderContext.BackgroundImageset != null && (targetBackgroundImageset != null && !RenderContext.BackgroundImageset.Equals(targetBackgroundImageset)))
            {
                if (targetBackgroundImageset != null && targetBackgroundImageset.Generic)
                {

                    FadeInImageSet(GetRealImagesetFromGeneric(targetBackgroundImageset));
                }
                else
                {
                    FadeInImageSet(targetBackgroundImageset);
                }
            }
        }

        public Imageset GetDefaultImageset(ImageSetType imageSetType, BandPass bandPass)
        {
            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.DefaultSet && imageset.BandPass == bandPass && imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.BandPass == bandPass && imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            return ImageSets[0];
        }

        private Imageset GetRealImagesetFromGeneric(Imageset generic)
        {
            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.DefaultSet && imageset.BandPass == generic.BandPass && imageset.DataSetType == generic.DataSetType)
                {
                    return imageset;
                }

            }

            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.BandPass == generic.BandPass && imageset.DataSetType == generic.DataSetType)
                {
                    return imageset;
                }

            }
            return ImageSets[0];
        }

        public static void SetBackgroundImageName(string name)
        {
            WWTControl.ImageSetName = name;
        }

        public static void SetForegroundImageName(string name)
        {
            WWTControl.ImageSetName = name;
        }

        public static void ShowLayers(bool show)
        {
            WWTControl.showDataLayers = show;
        }

        internal void HideUI(bool p)
        {
            //todo implement this
        }

        internal void CloseTour()
        {
            //todo implement tour close
        }
        public TourDocument tour = null;

        //public TourEditor TourEditor = null;
        public TourEditTab TourEdit = null;



        public TourDocument CreateTour(string name)
        {
            if (uiController is TourPlayer)
            {
                TourPlayer player = (TourPlayer)uiController;
                player.Stop(false);

            }

            tour = new TourDocument();
            tour.Title = name;
          
            SetupTour();
            tour.EditMode = true;
    
            return tour;
        }

        public void SetupTour()
        {
           TourEdit = new TourEditTab();
           TourEdit.Tour = tour;
           tour.CurrentTourstopIndex = 0;
           tour.EditMode = false;
           uiController = TourEdit.TourEditorUI;
        }

        public void PlayTour(string url)
        {
            if (uiController is TourPlayer)
            {
                TourPlayer player = (TourPlayer)uiController;
                player.Stop(false);
            }

            tour = TourDocument.FromUrl(url, delegate
                    {
                        //TourPlayer player = new TourPlayer();
                        //player.Tour = tour;
                        //tour.CurrentTourstopIndex = -1;
                        //uiController = player;
                        //WWTControl.scriptInterface.FireTourReady();
                        //player.Play();

                        SetupTour();
                        TourEdit.PlayNow(true);
                        WWTControl.scriptInterface.FireTourReady();
                    });
            
        }

        public void PlayCurrentTour()
        {
            if (uiController is TourPlayer)
            {
                TourPlayer player = (TourPlayer)uiController;
                player.Play();
            }

        }

        public void PauseCurrentTour()
        {
            if (uiController is TourPlayer)
            {
                TourPlayer player = (TourPlayer)uiController;
                player.PauseTour();
            }

        }

        public void StopCurrentTour()
        {
            if (uiController is TourPlayer)
            {
                TourPlayer player = (TourPlayer)uiController;
                player.Stop(false);
            }
        }

        public Imageset GetImagesetByName(string name)
        {
            foreach (Imageset imageset in ImageSets)
            {
                if (imageset.Name.ToLowerCase().IndexOf(name.ToLowerCase()) > -1)
                {
                    return imageset;
                }
            }
            return null;
        }

        public void SetBackgroundImageByName(string name)
        {
            Imageset newBackground = GetImagesetByName(name);

            if (newBackground != null)
            {
                RenderContext.BackgroundImageset = newBackground;
            }
        }

        public void SetForegroundImageByName(string name)
        {
            Imageset newForeground = GetImagesetByName(name);

            if (newForeground != null)
            {
                RenderContext.ForegroundImageset = newForeground;
            }
        }

        private void DrawCrosshairs(RenderContext context)
        {
            if (context.gl == null)
            {

                CanvasContext2D ctx = context.Device;
                ctx.Save();
                ctx.BeginPath();
                ctx.StrokeStyle = Settings.Current.CrosshairsColor;
                ctx.LineWidth = 2;

                double x = context.Width / 2, y = context.Height / 2;
                double halfLength = 5;

                ctx.MoveTo(x, y + halfLength);
                ctx.LineTo(x, y - halfLength);
                ctx.MoveTo(x + halfLength, y);
                ctx.LineTo(x - halfLength, y);

                ctx.Stroke();
                ctx.Restore();
            }
        }

 
        public void CaptureThumbnail(BlobReady blobReady)
        {
            Render();
           
            ImageElement image = (ImageElement)Document.CreateElement("img");
            image.AddEventListener("load", delegate (ElementEvent e)
            {
                double imageAspect = ((double)image.Width) / (image.Height);

                double clientAspect = 96 / 45;

                int cw = 96;
                int ch = 45;

                if (imageAspect < clientAspect)
                {
                    ch = (int)((double)cw / imageAspect);
                }
                else
                {
                 cw = (int)((double)ch * imageAspect);
                }

                int cx = (96 - cw) / 2;
                int cy = (45 - ch) / 2;

                CanvasElement temp = (CanvasElement)Document.CreateElement("canvas");
                temp.Height = 45;
                temp.Width = 96;
                CanvasContext2D ctx = (CanvasContext2D)temp.GetContext(Rendering.Render2D);
                ctx.DrawImage(image, cx, cy, cw, ch);
                //Script.Literal("{0}.toBlob({1}, 'image/jpeg')", temp, blobReady);

                Script.Literal("if ( typeof {0}.msToBlob == 'function') {{ var blob = {0}.msToBlob(); {1}(blob); }} else {{ {0}.toBlob({1}, 'image/jpeg'); }}", temp, blobReady);


              //  thumb.Src = temp.GetDataUrl();
            }, false);

            image.Src = Singleton.Canvas.GetDataUrl();

        }
    }
    public delegate void BlobReady(System.Html.Data.Files.Blob blob);


    public class WWTElementEvent
    {
        public double OffsetX;
        public double OffsetY;
        public WWTElementEvent(double x, double y)
        {
            OffsetX = x;
            OffsetY = y;
        }
    }
}
