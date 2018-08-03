using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public enum SolarSystemObjects
    {
        Sun = 0,
        Mercury = 1,
        Venus = 2,
        Mars = 3,
        Jupiter = 4,
        Saturn = 5,
        Uranus = 6,
        Neptune = 7,
        Pluto = 8,
        Moon = 9,
        Io = 10,
        Europa = 11,
        Ganymede = 12,
        Callisto = 13,
        IoShadow = 14,
        EuropaShadow = 15,
        GanymedeShadow = 16,
        CallistoShadow = 17,
        SunEclipsed = 18,
        Earth = 19,
        Custom = 20,
        Undefined = 65536
    };

    

    public enum InterpolationType { Linear = 0, EaseIn = 1, EaseOut = 2, EaseInOut = 3, Exponential = 4, DefaultV = 5 };
    public class CameraParameters
    {
        public double Lat;
        public double Lng;
        public double Zoom;
        public double Rotation;
        public double Angle;
        public bool RaDec;
        public double Opacity;
        public Vector3d ViewTarget;
        public SolarSystemObjects Target;
        public string TargetReferenceFrame;
        public CameraParameters()
        {
            Zoom = 360;
            ViewTarget = new Vector3d();
        }
        public static CameraParameters Create(double lat, double lng, double zoom, double rotation, double angle, float opactity)
        {
            CameraParameters temp = new CameraParameters();
            temp.Lat = lat;
            temp.Lng = lng;
            temp.Zoom = zoom;
            temp.Rotation = rotation;
            temp.Angle = angle;
            temp.RaDec = false;
            temp.Opacity = opactity;
            temp.ViewTarget = Vector3d.Create(0, 0, 0);
            temp.Target = SolarSystemObjects.Custom;
            temp.TargetReferenceFrame = "";
            return temp;
        }

        public CameraParameters Copy()
        {
            CameraParameters temp = new CameraParameters();
            temp.Lat = Lat;
            temp.Lng = Lng;
            temp.Zoom = Zoom;
            temp.Rotation = Rotation;
            temp.Angle = Angle;
            temp.RaDec = RaDec;
            temp.Opacity = Opacity;
            temp.ViewTarget = ViewTarget.Copy();
            temp.Target = Target;
            temp.TargetReferenceFrame = TargetReferenceFrame;
            return temp;
        }

        public double RA
        {
            get
            {
                return ((((180 - (Lng - 180)) / 360) * 24.0) % 24);
            }
            set
            {
                Lng = 180 - ((value) / 24.0 * 360) - 180;
                RaDec = true;
            }
        }
        public double Dec
        {
            get
            {
                return Lat;
            }
            set
            {
                Lat = value;
            }
        }

        public static double LogN(double num, double b)
        {
            return Math.Log(num) / Math.Log(b);
        }

        public static double Sinh(double v)
        {
            return (Math.Exp(v) - Math.Exp(-v)) / 2;
        }

        public static CameraParameters Interpolate(CameraParameters from, CameraParameters to, double alphaIn, InterpolationType type, bool fastDirectionMove)
        {
            CameraParameters result = new CameraParameters();
            double alpha = EaseCurve(alphaIn, type);
            double alphaBIn = Math.Min(1.0, alphaIn * 2);
            double alphaB = EaseCurve(alphaBIn, type);
            result.Angle = to.Angle * alpha + from.Angle * (1.0 - alpha);
            result.Rotation = to.Rotation * alpha + from.Rotation * (1.0 - alpha);
            if (fastDirectionMove)
            {
                result.Lat = to.Lat * alphaB + from.Lat * (1.0 - alphaB);
                result.Lng = to.Lng * alphaB + from.Lng * (1.0 - alphaB);
            }
            else
            {
                result.Lat = to.Lat * alpha + from.Lat * (1.0 - alpha);
                result.Lng = to.Lng * alpha + from.Lng * (1.0 - alpha);
            }
            result.Zoom = Math.Pow(2, LogN(to.Zoom, 2) * alpha + LogN(from.Zoom, 2) * (1.0 - alpha));
            result.Opacity = (double)(to.Opacity * alpha + from.Opacity * (1.0 - alpha));
            result.ViewTarget = Vector3d.Lerp(from.ViewTarget, to.ViewTarget, alpha);
            result.TargetReferenceFrame = to.TargetReferenceFrame;
            if (to.Target == from.Target)
            {
                result.Target = to.Target;
            }
            else
            {
                result.Target = SolarSystemObjects.Custom;
            }
            return result;
        }

        public static CameraParameters InterpolateGreatCircle(CameraParameters from, CameraParameters to, double alphaIn, InterpolationType type, bool fastDirectionMove)
        {
            CameraParameters result = new CameraParameters();
            double alpha = EaseCurve(alphaIn, type);
            double alphaBIn = Math.Min(1.0, alphaIn * 2);
            double alphaB = EaseCurve(alphaBIn, type);
            result.Angle = to.Angle * alpha + from.Angle * (1.0 - alpha);
            result.Rotation = to.Rotation * alpha + from.Rotation * (1.0 - alpha);

            Vector3d left = Coordinates.GeoTo3dDouble(from.Lat, from.Lng);
            Vector3d right = Coordinates.GeoTo3dDouble(to.Lat, to.Lng);

            Vector3d mid = Vector3d.Slerp(left, right, alpha);

            Vector2d midV2 = Coordinates.CartesianToLatLng(mid);

            result.Lat = midV2.Y;
            result.Lng = midV2.X;


            result.Zoom = Math.Pow(2, LogN(to.Zoom, 2) * alpha + LogN(from.Zoom, 2) * (1.0 - alpha));
            result.Opacity = (double)(to.Opacity * alpha + from.Opacity * (1.0 - alpha));
            result.ViewTarget = Vector3d.Lerp(from.ViewTarget, to.ViewTarget, alpha);

            result.TargetReferenceFrame = to.TargetReferenceFrame;
            if (to.Target == from.Target)
            {
                result.Target = to.Target;
            }
            else
            {
                result.Target = SolarSystemObjects.Custom;
            }
            return result;
        }


        const double factor = 0.1085712344;

        public static double EaseCurve(double alpha, InterpolationType type)
        {
            // =100-SINH(A29/Factor*PI())

            switch (type)
            {
                case InterpolationType.Linear:
                    return alpha;
                case InterpolationType.Exponential:
                    return Math.Pow(alpha, 2);
                case InterpolationType.EaseIn:
                    return ((1 - alpha) * Sinh(alpha / (factor * 2)) / 100.0) + alpha * alpha;
                case InterpolationType.EaseOut:
                    return (alpha * (1 - Sinh((1.0 - alpha) / (factor * 2)) / 100.0)) + (1.0 - alpha) * alpha;
                case InterpolationType.EaseInOut:
                    if (alpha < .5)
                    {
                        return Sinh(alpha / factor) / 100.0;
                    }
                    else
                    {
                        return 1.0 - (Sinh((1.0 - alpha) / factor) / 100.0);
                    }
                default:
                    return alpha;
            }
        }

       
        //public static double EaseCurve(double alpha)
        //{
        //    // =100-SINH(A29/Factor*PI())

        //    if (alpha < .5)
        //    {
        //        return Sinh(alpha / factor) / 100.0;
        //    }
        //    else
        //    {
        //        return 1.0 - (Sinh((1.0 - alpha) / factor) / 100.0);
        //    }
        //}

        //public static bool operator ==(CameraParameters c1, CameraParameters c2)
        //{
        //    if (!(c1 is CameraParameters))
        //    {
        //        return !(c2 is CameraParameters);
        //    }

        //    return c1.Equals(c2);
        //}

        //public static bool operator !=(CameraParameters c1, CameraParameters c2)
        //{
        //    if (!(c1 is CameraParameters))
        //    {
        //        return (c2 is CameraParameters);
        //    }

        //    return !c1.Equals(c2);
        //}


        public bool Equals(object obj)
        {
            if (obj is CameraParameters)
            {
                CameraParameters cam = (CameraParameters)obj;

                if (Math.Abs(cam.Angle - this.Angle) > .01 || Math.Abs(cam.Lat - this.Lat) > (cam.Zoom / 10000) || Math.Abs(cam.RA - this.RA) > (cam.Zoom / 1000) || Math.Abs(cam.Rotation - this.Rotation) > .1 || Math.Abs(cam.Zoom - this.Zoom) > (Math.Abs(cam.Zoom) / 1000))
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

