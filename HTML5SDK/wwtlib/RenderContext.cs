using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;


namespace wwtlib
{
    public class RenderContext
    {
        public static bool UseGl = false;
        public CanvasContext2D Device;
        public GL gl;

        public double Height;
        public double Width;
        public bool Lighting = false;
        public RenderContext()
        {
           

            for (int i = 0; i < 6; i++)
            {
                frustum[i] = new PlaneD(0,0,0,0);
            }
        }
        public static RenderContext Create(CanvasContext2D device)
        {
            RenderContext temp = new RenderContext();
            temp.Device = device;
           
            temp.ViewCamera.Zoom = 700;
            temp.ViewCamera.Target = SolarSystemObjects.Undefined;
            return temp;
        }

        public void Save()
        {
            if (gl != null)
            {
            }
            else
            {
                Device.Save();
            }
        }

        public void Restore()
        {
            if (gl != null)
            {
            }
            else
            {
                Device.Restore();
            }

        }

        public void Clear()
        {
            if (gl != null)
            {
                gl.viewport(0, 0, (int)Width, (int)Height);
                gl.clear(GL.COLOR_BUFFER_BIT | GL.DEPTH_BUFFER_BIT);
                //gl.clearColor(1, 0, 0, 1);

            }
            else
            {
                Device.Save();
                Device.FillStyle = "black";
                Device.FillRect(0, 0, Width, Height);

                Device.Restore();
            }
        }


        Vector3d viewPoint = new Vector3d();
        public Vector3d ViewPoint
        {
            get
            {
                return viewPoint;
            }
        }
        public double RA
        {
            get
            {
                //return ((-((this.currentTheta - 180) / 15) % 24) + 48) % 24;
                return ((((180 - (this.ViewCamera.Lng - 180)) / 15) % 24) + 48) % 24;
            }
        }

        public bool Space = false;

        public double RAtoViewLng(double ra)
        {
            return 180 - ((ra) / 24.0 * 360) - 180; ;
        }
        public double Dec
        {
            get
            {
                return ViewCamera.Lat;
            }
        }
        const double FOVMULT = 343.774f;
        double fovAngle;
        public double FovAngle
        {
            get
            {

                return fovAngle;
            }
        }
        double fovScale = 0;

        public double FovScale
        {
            get { return fovScale; }
            set { fovScale = value; }
        }


        private Matrix3d view;

        public Matrix3d View
        {
            get { return view; }
            set
            {
                view = value;
                //Device.Transform.View = view.Matrix;
                frustumDirty = true;
            }
        }
        private Matrix3d viewBase;

        public Matrix3d ViewBase
        {
            get { return viewBase; }
            set
            {
                viewBase = value;
            }
        } 
        
        private Matrix3d projection;

        public Matrix3d Projection
        {
            get { return projection; }
            set
            {
                projection = value;
                //Device.Transform.Projection = projection.Matrix;
                frustumDirty = true;
            }
        }
        private Matrix3d world;

        public Matrix3d World
        {
            get { return world; }
            set
            {
                world = value;
                //Device.Transform.World = world.Matrix;
                frustumDirty = true;
            }
        }


        private Matrix3d worldBase;

        internal Texture GetScreenTexture()
        {
            //todo add code to capture screen
            Texture tex = null;

            return tex;
            
        }

        public Matrix3d WorldBase
        {
            get { return worldBase; }
            set
            {
                worldBase = value;
            }
        }   
        private Matrix3d worldBaseNonRotating;

        public Matrix3d WorldBaseNonRotating
        {
            get { return worldBaseNonRotating; }
            set
            {
                worldBaseNonRotating = value;
            }
        }

        private double nominalRadius = 6378137.0;

        public double NominalRadius
        {
            get { return nominalRadius; }
            set { nominalRadius = value; }
        }

        private Vector3d sunPosition;
        public Vector3d SunPosition
        {
            get
            {
                return sunPosition;
            }

            set
            {
                sunPosition = value;
            }
        }

        public IViewMover ViewMover = null;

        public bool OnTarget(Place place)
        {
            return (
                    (
                        Math.Abs(ViewCamera.Lat - TargetCamera.Lat) < .000000000001
                        && Math.Abs(ViewCamera.Lng - TargetCamera.Lng) < .000000000001
                        && Math.Abs(ViewCamera.Zoom - TargetCamera.Zoom) < .000000000001
                    )
                    && ViewMover == null
                );

        }

      
 
        public void SetTexture(ImageElement texture)
        {
            
        }

        public CameraParameters ViewCamera = new CameraParameters();
        public CameraParameters TargetCamera = new CameraParameters();

        public double alt = 0;
        public double az = 0;
        public double targetAlt = 0;
        public double targetAz = 0;

        Imageset backgroundImageset;

        public Imageset BackgroundImageset
        {
            get { return backgroundImageset; }
            set { backgroundImageset = value; }
        }

        Imageset foregroundImageset;

        public Imageset ForegroundImageset
        {
            get { return foregroundImageset; }
            set { foregroundImageset = value; }
        }


        private static int GetTilesYForLevel(Imageset layer, int level)
        {
            int maxY = 1;

            switch (layer.Projection)
            {
                case ProjectionType.Mercator:
                    maxY = (int)Math.Pow(2, level);
                    break;
                case ProjectionType.Equirectangular:
                    //                    maxY = (int)Math.Pow(2, level-layer.BaseLevel) * (int)(180 / layer.BaseTileDegrees);

                    maxY = (int)(Math.Pow(2, level) * (180 / layer.BaseTileDegrees));
                    break;
                case ProjectionType.Tangent:
                    maxY = (int)Math.Pow(2, level);

                    break;
                case ProjectionType.Spherical:
                    maxY = 1;
                    break;
                default:
                    maxY = (int)Math.Pow(2, level);
                    break;
            }
            if (maxY == Double.PositiveInfinity)
            {
                maxY = 1;
            }
            return maxY;
        }

        private static int GetTilesXForLevel(Imageset layer, int level)
        {
            int maxX = 1;
            switch (layer.Projection)
            {
                case ProjectionType.Plotted:
                case ProjectionType.Toast:
                    maxX = (int)Math.Pow(2, level);
                    break;
                case ProjectionType.Mercator:
                    maxX = (int)Math.Pow(2, level) * (int)(layer.BaseTileDegrees / 360.0);
                    break;
                case ProjectionType.Equirectangular:
                    //maxX = (int)Math.Pow(2, level) * (int)(layer.BaseTileDegrees / 90.0);
                    maxX = (int)Math.Pow(2, level) * (int)(360 / layer.BaseTileDegrees);

                    break;
         
                case ProjectionType.Tangent:
                    if (layer.WidthFactor == 1)
                    {
                        maxX = (int)Math.Pow(2, level) * 2;
                    }
                    else
                    {
                        maxX = (int)Math.Pow(2, level);
                    }
                    break;

                case ProjectionType.SkyImage:
                    maxX = 1;
                    break;
                case ProjectionType.Spherical:
                    maxX = 1;
                    break;
                default:
                    maxX = (int)Math.Pow(2, level) * 2;
                    break;
            }
            

            return maxX;
        }
        public void DrawImageSet(Imageset imageset, double opacity)
        {

            int maxX = GetTilesXForLevel(imageset, imageset.BaseLevel);
            int maxY = GetTilesYForLevel(imageset, imageset.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(imageset.BaseLevel, x, y, imageset, null);
                    if (tile != null)
                    {
                        tile.Draw3D(this, opacity);
                    }
                }
            }
        }

        public double GetScaledAltitudeForLatLong(double viewLat, double viewLong)
        {
            Imageset layer = BackgroundImageset;

            if (layer == null)
            {
                return 0;
            }

            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                    if (tile != null)
                    {
                        if (tile.IsPointInTile(viewLat, viewLong))
                        {
                            return tile.GetSurfacePointAltitude(viewLat, viewLong, false);
                        }
                    }
                }
            }
            return 0;
        }

        double targetHeight = 1;
        public double targetAltitude = 0;

        public static double back = 0;
        internal void SetupMatricesLand3d()
        {
            Lighting = false;
            Space = false;
            RenderTriangle.CullInside = false;

            // For our world matrix, we will just rotate the Earth and Clouds about the y-axis.
            Matrix3d WorldMatrix = Matrix3d.RotationY(((ViewCamera.Lng - 90f) / 180f * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((-ViewCamera.Lat) / 180f * Math.PI)));
            World = WorldMatrix;
            WorldBase = WorldMatrix.Clone();

            viewPoint = Coordinates.GeoTo3d(ViewCamera.Lat, ViewCamera.Lng);

            double distance = 0;
            if (backgroundImageset.IsMandelbrot)
            {

                distance = (4.0 * (ViewCamera.Zoom / 180)) + 0.00000000000000000000000000000000000000001;
            }
            else
            {

                distance = (4.0 * (ViewCamera.Zoom / 180)) + 0.000001;
            }
            fovAngle = ((this.ViewCamera.Zoom) / FOVMULT) / Math.PI * 180;
            fovScale = (fovAngle / Height) * 3600;

            if (gl != null)
            {
                targetAltitude = GetScaledAltitudeForLatLong(ViewCamera.Lat, ViewCamera.Lng);
                double heightNow = 1 + targetAltitude;
                targetAltitude *= NominalRadius;
                //if ((double.IsNaN(heightNow)))
                //{
                //    heightNow = 0;
                //}

                if (targetHeight < heightNow)
                {
                    targetHeight = (((targetHeight * 2) + heightNow) / 3);
                }
                else
                {
                    targetHeight = (((targetHeight * 9) + heightNow) / 10);
                }
                //if (double.IsNaN(targetHeight))
                //{
                //    targetHeight = 0;
                //}

            }
            else
            {
                targetAltitude = 0;
                targetHeight = 1;
            }

            double rotLocal = ViewCamera.Rotation;

            CameraPosition = Vector3d.Create(
                (Math.Sin(rotLocal) * Math.Sin(ViewCamera.Angle) * distance),
                (Math.Cos(rotLocal) * Math.Sin(ViewCamera.Angle) * distance),
                (-targetHeight - (Math.Cos(ViewCamera.Angle) * distance)));
            Vector3d cameraTarget = Vector3d.Create(0.0f, 0.0f, -targetHeight);


            double camHeight = CameraPosition.Length();


            Vector3d lookUp = Vector3d.Create(Math.Sin(rotLocal) * Math.Cos(ViewCamera.Angle), Math.Cos(rotLocal) * Math.Cos(ViewCamera.Angle), Math.Sin(ViewCamera.Angle));

            View = Matrix3d.LookAtLH(
                          (CameraPosition),
                          (cameraTarget),
                          lookUp);
            // * Matrix3d.RotationX(((-config.DomeTilt) / 180 * Math.PI));

            ViewBase = View;

            double back = Math.Sqrt((distance + 1f) * (distance + 1f) - 1);
            back = Math.Max(.5, back);
            // back = (float)camDist * 40f;
            double m_nearPlane = distance * .05f;

            m_nearPlane = distance * .05f;
            Projection = Matrix3d.PerspectiveFovLH((Math.PI / 4.0), (double)Width / (double)Height, m_nearPlane, back);

            SetMatrixes();

            MakeFrustum();
        }
        bool galactic = true;

        Matrix3d galacticMatrix = Matrix3d.Create(
                -.4838350155, -.0548755604, -.8734370902, 0,
                .7469822445, .4941094279, -.4448296300, 0,
                 .4559837762, -.8676661490, -.1980763734, 0,
                0, 0, 0, 1);
        bool firstTimeInit = false;
        
        public void SetupMatricesSpace3d(double canvasWidth, double canvasHeight)
        {
            Lighting = false;
            if (!firstTimeInit)
            {
                galacticMatrix = Matrix3d.Identity;
                // -28.9361739586894, 17.7603329867975
                galacticMatrix.Multiply(Matrix3d.RotationY(-(270-(17.7603329867975*15 )) / 180.0 * Math.PI));
                galacticMatrix.Multiply(Matrix3d.RotationX(-((-28.9361739586894)) / 180.0 * Math.PI));
                galacticMatrix.Multiply(Matrix3d.RotationZ(((31.422052860102041270114993238783)-90) / 180.0 * Math.PI));
                //galacticMatrix.Transpose();
                //galacticMatrix.Invert();
                firstTimeInit = true;
            }


            Space = true;
            RenderTriangle.CullInside = true;

            Matrix3d WorldMatrix = Matrix3d.Identity;
            if (Settings.Active.GalacticMode)
            {
                WorldMatrix.Multiply(galacticMatrix);
                WorldMatrix.Multiply(Matrix3d.RotationY(((az )) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(-((alt)) / 180.0 * Math.PI));
                double[] gPoint = Coordinates.GalactictoJ2000(az, alt);

                viewPoint = Coordinates.RADecTo3dAu(gPoint[0]/15, gPoint[1], 1.0);
                TargetCamera.Lng = this.RAtoViewLng(gPoint[0] / 15);
                TargetCamera.Lat = gPoint[1];
                ViewCamera.Lat = TargetCamera.Lat;
                ViewCamera.Lng = TargetCamera.Lng;
            }
            else
            {
                WorldMatrix.Multiply(Matrix3d.RotationY(-((ViewCamera.Lng -90 )) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(-((ViewCamera.Lat)) / 180.0 * Math.PI));
                viewPoint = Coordinates.RADecTo3dAu(RA, Dec, 1.0);
            }



            

            double camLocal = ((ViewCamera.Rotation /*+ 90*/));
            fovAngle = ((this.ViewCamera.Zoom) / FOVMULT) / Math.PI * 180;
            fovScale = (fovAngle / canvasHeight) * 3600;

            //Settings.Global.LocalHorizonMode = true;

            // altaz
            if (Settings.Active.LocalHorizonMode && backgroundImageset.DataSetType == ImageSetType.Sky)
            {
                Coordinates zenithAltAz = new Coordinates(0, 0);

                zenithAltAz.Az = 0;

                zenithAltAz.Alt = 0;

                Coordinates zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);
                //Coordinates zenith2 = Coordinates.HorizonToEquitorial(zenithAltAz, Coordinates.FromLatLng(1, 1), SpaceTimeController.Now);
                //Coordinates zenith3 = Coordinates.HorizonToEquitorial(zenithAltAz, Coordinates.FromLatLng(-1, 1), SpaceTimeController.Now);

                double raPart = -((zenith.RA - 6) / 24.0 * (Math.PI * 2));
                double decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
                string raText = Coordinates.FormatDMS(zenith.RA);
                WorldMatrix = Matrix3d.RotationY(-raPart-Math.PI);
                WorldMatrix.Multiply(Matrix3d.RotationX(decPart));

                if (SpaceTimeController.Location.Lat < 0)
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((alt) / 180.0 * Math.PI)));
                    camLocal += Math.PI;
                }
                else
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((-az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((-alt) / 180.0 * Math.PI)));
                }

                Coordinates currentRaDec = Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(alt, az), SpaceTimeController.Location, SpaceTimeController.Now);

                ViewCamera.Lat = TargetCamera.Lat = currentRaDec.Dec;
                ViewCamera.Lng = TargetCamera.Lng = RAtoViewLng(currentRaDec.RA);

            }
            World = WorldMatrix;
            WorldBase = WorldMatrix.Clone();
            // altaz

            

            double localZoomFactor = ViewCamera.Zoom;

            double FovAngle = ((localZoomFactor) / FOVMULT) / Math.PI * 180;
            CameraPosition = Vector3d.Create(0.0, 0.0, 0.0);
            // This is for distance Calculation. For space everything is the same distance, so camera target is key.

            View = Matrix3d.LookAtLH(CameraPosition, Vector3d.Create(0.0, 0.0, -1.0), Vector3d.Create(Math.Sin(camLocal), Math.Cos(camLocal), 0.0));
            ViewBase = View.Clone();

            double m_nearPlane = .1;
            nearPlane = .1f;
            Projection = Matrix3d.PerspectiveFovLH((localZoomFactor) / FOVMULT, (double)canvasWidth / (double)canvasHeight, .1, -2.0);


            SetMatrixes();

            MakeFrustum();

        }

        public SolarSystemObjects SolarSystemTrack
        {
            get
            {
                return ViewCamera.Target;
            }
            set
            {
                ViewCamera.Target = value;
            }
        }


        public double SolarSystemCameraDistance
        {
            get
            {
                return (4.0 * (ViewCamera.Zoom / 9)) + 0.000001;
            }

        }

        public bool SandboxMode
        {
            get
            {
                if (backgroundImageset == null)
                {
                    return false;
                }

                return backgroundImageset.DataSetType == ImageSetType.Sandbox;
            }
        }

        public string TrackingFrame
        {
            get { return ViewCamera.TargetReferenceFrame; }
            set { ViewCamera.TargetReferenceFrame = value; }
        }

        bool useSolarSystemTilt = true;

        public CameraParameters CustomTrackingParams = new CameraParameters();

        Vector3d cameraOffset = new Vector3d();

        double fovLocal = (Math.PI / 4.0);

        public double FovLocal
        {
            get { return fovLocal; }
            set { fovLocal = value; }
        }
        public double PerspectiveFov = Math.PI / 4;


        public void SetupMatricesOverlays()
        {

            World = Matrix3d.Identity;

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            Vector3d lookFrom =  Vector3d.Create(0, 0, 0);
            Vector3d lookAt =  Vector3d.Create(0, 0, 1);
            Vector3d lookUp =  Vector3d.Create(0, 1, 0);

            Matrix3d view;
            view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp);
            view.Multiply( Matrix3d.Scaling(1, -1, 1));

 
            View = view;

            double back = 10000;
            nearPlane = .1f;

            Projection = Matrix3d.PerspectiveFovLH(fovLocal, Width / Height, nearPlane, back);



        }



        public void SetupMatricesSolarSystem(bool forStars)
        {
            Lighting = Settings.Active.SolarSystemLighting;

            Space = false;
            if (SolarSystemTrack != SolarSystemObjects.Custom && SolarSystemTrack != SolarSystemObjects.Undefined)
            {
                ViewCamera.ViewTarget = Planets.GetPlanetTargetPoint(SolarSystemTrack, ViewCamera.Lat, ViewCamera.Lng, 0);
            }
            RenderTriangle.CullInside = false;


            double cameraDistance = SolarSystemCameraDistance;

            Matrix3d trackingMatrix = Matrix3d.Identity;
            cameraDistance -= 0.000001;

            bool activeTrackingFrame = false;
            //if (SolarSystemTrack == SolarSystemObjects.Custom && !string.IsNullOrEmpty(TrackingFrame))
            //{
            //    activeTrackingFrame = true;
            //    viewCamera.ViewTarget = LayerManager.GetFrameTarget(RenderContext, TrackingFrame, out trackingMatrix);
            //}
            //else if (!string.IsNullOrEmpty(TrackingFrame))
            {
                TrackingFrame = "";
            }


            Vector3d center = ViewCamera.ViewTarget;
            //Vector3d lightPosition = -center;

            double localZoom = ViewCamera.Zoom * 20;
            Vector3d lookAt = new Vector3d();

            Matrix3d viewAdjust = Matrix3d.Identity;
            viewAdjust.Multiply(Matrix3d.RotationX(((-ViewCamera.Lat) / 180f * Math.PI)));
            viewAdjust.Multiply(Matrix3d.RotationY(((-ViewCamera.Lng) / 180f * Math.PI)));

            Matrix3d lookAtAdjust = Matrix3d.Identity;


            bool dome = false;

            Vector3d lookUp;


            if (useSolarSystemTilt && !SandboxMode)
            {
                double angle = ViewCamera.Angle;
                if (cameraDistance > 0.0008)
                {
                    angle = 0;
                }
                else if (cameraDistance > 0.00001)
                {
                    double val = Math.Min(1.903089987, Util.Log10(cameraDistance) + 5) / 1.903089987;

                    angle = angle * Math.Max(0, 1 - val);
                }
                


                CameraPosition =  Vector3d.Create(
                (Math.Sin(-ViewCamera.Rotation) * Math.Sin(angle) * cameraDistance),
                (Math.Cos(-ViewCamera.Rotation) * Math.Sin(angle) * cameraDistance),
                ((Math.Cos(angle) * cameraDistance)));
                lookUp = Vector3d.Create(Math.Sin(-ViewCamera.Rotation), Math.Cos(-ViewCamera.Rotation), 0.00001f);
            }
            else
            {
                CameraPosition = Vector3d.Create(0, 0, ((cameraDistance)));

                lookUp = Vector3d.Create(Math.Sin(-ViewCamera.Rotation), Math.Cos(-ViewCamera.Rotation), 0.0001f);
            }


            CameraPosition = viewAdjust.Transform(CameraPosition);

            cameraOffset = CameraPosition.Copy();

            Matrix3d tmp = trackingMatrix.Clone();
            tmp.Invert();
            cameraOffset = Vector3d.TransformCoordinate(cameraOffset, tmp);



            lookUp = viewAdjust.Transform(lookUp);


           // WorldMatrix = Matrix3d.Identity;
            World = Matrix3d.Identity;
            WorldBase = World.Clone();


            View = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(trackingMatrix, Matrix3d.LookAtLH(CameraPosition, lookAt, lookUp)), lookAtAdjust);
                 

            ViewBase = View.Clone();


            Vector3d temp = Vector3d.SubtractVectors(lookAt,CameraPosition);
            temp.Normalize();
            temp = Vector3d.TransformCoordinate(temp, trackingMatrix);
            temp.Normalize();
            viewPoint = temp;



            //if (activeTrackingFrame)
            //{
            //    Vector3d atfCamPos = RenderContext11.CameraPosition;
            //    Vector3d atfLookAt = lookAt;
            //    Vector3d atfLookUp = lookUp;
            //    Matrix3d mat = trackingMatrix;
            //    mat.Invert();

            //    atfCamPos.TransformCoordinate(mat);
            //    atfLookAt.TransformCoordinate(mat);
            //    atfLookUp.TransformCoordinate(mat);
            //    atfLookAt.Normalize();
            //    atfLookUp.Normalize();

            //    CustomTrackingParams.Angle = 0;
            //    CustomTrackingParams.Rotation = 0;
            //    CustomTrackingParams.DomeAlt = viewCamera.DomeAlt;
            //    CustomTrackingParams.DomeAz = viewCamera.DomeAz;
            //    CustomTrackingParams.TargetReferenceFrame = "";
            //    CustomTrackingParams.ViewTarget = viewCamera.ViewTarget;
            //    CustomTrackingParams.Zoom = viewCamera.Zoom;
            //    CustomTrackingParams.Target = SolarSystemObjects.Custom;


            //    Vector3d atfLook = atfCamPos - atfLookAt;
            //    atfLook.Normalize();



            //    Coordinates latlng = Coordinates.CartesianToSpherical2(atfLook);
            //    CustomTrackingParams.Lat = latlng.Lat;
            //    CustomTrackingParams.Lng = latlng.Lng - 90;

            //    Vector3d up = Coordinates.GeoTo3dDouble(latlng.Lat + 90, latlng.Lng - 90);
            //    Vector3d left = Vector3d.Cross(atfLook, up);

            //    double dotU = Math.Acos(Vector3d.Dot(atfLookUp, up));
            //    double dotL = Math.Acos(Vector3d.Dot(atfLookUp, left));

            //    CustomTrackingParams.Rotation = dotU;// -Math.PI / 2;
            //}


            double radius = Planets.GetAdjustedPlanetRadius((int)SolarSystemTrack);
           
           
            if (cameraDistance < radius * 2.0 && !forStars)
            {
                nearPlane = cameraDistance * 0.03;

                //m_nearPlane = Math.Max(m_nearPlane, .000000000030);
                nearPlane = Math.Max(nearPlane, .00000000001);
                back = 1900;
            }
            else
            {
                if (forStars)
                {
                    back = 900056;
                    back = cameraDistance > 900056 ? cameraDistance * 3 : 900056;
                    nearPlane = .00003f;

                    // m_nearPlane = cameraDistance * 0.03;

                    // back = 9937812653;
                    //  back = 21421655730;
                }
                else
                {
                    back = cameraDistance > 1900 ? cameraDistance + 200 : 1900;

                    // m_nearPlane = .0001f;
                    if (Settings.Active.SolarSystemScale < 13)
                    {
                        nearPlane = (float)Math.Min(cameraDistance * 0.03, 0.01);
                    }
                    else
                    {
                        nearPlane = .001f;
                    }
                }
            }


            Projection = Matrix3d.PerspectiveFovLH((fovLocal), (double)Width / (double)Height, nearPlane, back);
            PerspectiveFov = fovLocal;
            fovAngle = ((this.ViewCamera.Zoom) / FOVMULT) / Math.PI * 180;
            fovScale = (fovAngle / Height) * 3600;

            SetMatrixes();
            MakeFrustum();
        }

        private void SetMatrixes()
        {
            //if (gl != null)
            //{
            //    Matrix3d mvMat = Matrix3d.MultiplyMatrix(World, View);

            //    gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
            //    gl.uniformMatrix4fv(projMatLoc, false, Projection.FloatArray());
            //    gl.uniform1i(sampLoc, 0);
            //    if (Space)
            //    {
            //        gl.disable(GL.DEPTH_TEST);
            //    }
            //    else
            //    {
            //        gl.enable(GL.DEPTH_TEST);
            //    }
            //}
        }


        public double nearPlane = 0;
        bool frustumDirty = true;
        PlaneD[] frustum = new PlaneD[6];

        public PlaneD[] Frustum
        {
            get
            {
                return frustum;
            }
        }

        public Vector3d CameraPosition;

        public Matrix3d WVP;
        public Matrix3d WV;
        public void MakeFrustum()
        {
            WV = Matrix3d.MultiplyMatrix(World, View);

            Matrix3d viewProjection =  Matrix3d.MultiplyMatrix(WV, Projection);

            WVP = viewProjection.Clone();

            Matrix3d inverseWorld = World.Clone();
            inverseWorld.Invert();

            // Left plane 
            frustum[0].A = viewProjection.M14 + viewProjection.M11;
            frustum[0].B = viewProjection.M24 + viewProjection.M21;
            frustum[0].C = viewProjection.M34 + viewProjection.M31;
            frustum[0].D = viewProjection.M44 + viewProjection.M41;

            // Right plane 
            frustum[1].A = viewProjection.M14 - viewProjection.M11;
            frustum[1].B = viewProjection.M24 - viewProjection.M21;
            frustum[1].C = viewProjection.M34 - viewProjection.M31;
            frustum[1].D = viewProjection.M44 - viewProjection.M41;

            // Top plane 
            frustum[2].A = viewProjection.M14 - viewProjection.M12;
            frustum[2].B = viewProjection.M24 - viewProjection.M22;
            frustum[2].C = viewProjection.M34 - viewProjection.M32;
            frustum[2].D = viewProjection.M44 - viewProjection.M42;

            // Bottom plane 
            frustum[3].A = viewProjection.M14 + viewProjection.M12;
            frustum[3].B = viewProjection.M24 + viewProjection.M22;
            frustum[3].C = viewProjection.M34 + viewProjection.M32;
            frustum[3].D = viewProjection.M44 + viewProjection.M42;

            // Near plane 
            frustum[4].A = viewProjection.M13;
            frustum[4].B = viewProjection.M23;
            frustum[4].C = viewProjection.M33;
            frustum[4].D = viewProjection.M43;

            // Far plane 
            frustum[5].A = viewProjection.M14 - viewProjection.M13;
            frustum[5].B = viewProjection.M24 - viewProjection.M23;
            frustum[5].C = viewProjection.M34 - viewProjection.M33;
            frustum[5].D = viewProjection.M44 - viewProjection.M43;

            // Normalize planes 
            for (int i = 0; i < 6; i++)
            {
                frustum[i].Normalize();

            }
            frustumDirty = false;
            
            WVP.Scale( Vector3d.Create(Width/2, -Height/2, 1));
            WVP.Translate(Vector3d.Create(Width/2, Height/2, 0));
            SetMatrixes();
        }

        string SkyColor = "Blue";

        //internal WebGLShader frag;
        //internal WebGLShader vert;

        //public int vertLoc;
        //public int textureLoc;
        //public WebGLUniformLocation projMatLoc;
        //public WebGLUniformLocation mvMatLoc;
        //public WebGLUniformLocation sampLoc;
        internal void InitGL()
        {
            if (gl == null)
            {
                return;
            }

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            TileShader.Init(this);
        }

        public void FreezeView()
        {
            targetAlt = alt;
            targetAz = az;

            TargetCamera = ViewCamera.Copy();
        }
    }
}
