using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public interface IViewMover
    {
        bool Complete { get; }
        CameraParameters CurrentPosition { get; }
        Date CurrentDateTime { get; }
        Action Midpoint { get; set;}
        double MoveTime { get; }
    }
       
    public class ViewMoverKenBurnsStyle : IViewMover
    {
        public InterpolationType InterpolationType = InterpolationType.Linear;
       // public bool Dampened = false;
        public bool FastDirectionMove = false;
        CameraParameters from;
        CameraParameters to;

        Date fromDateTime;
        Date toDateTime;
        Date fromTime;
        double toTargetTime = 0;
        int dateTimeSpan;
        public ViewMoverKenBurnsStyle(CameraParameters from, CameraParameters to, double time, Date fromDateTime, Date toDateTime, InterpolationType type)
        {
            InterpolationType = type;

            if (Math.Abs(from.Lng - to.Lng) > 180)
            {
                if (from.Lng > to.Lng)
                {
                    from.Lng -= 360;
                }
                else
                {
                    from.Lng += 360;
                }
            }

            this.fromDateTime = fromDateTime;
            this.toDateTime = toDateTime;

            dateTimeSpan = toDateTime - fromDateTime;

            this.from = from.Copy();
            this.to = to.Copy();
            fromTime = Date.Now;
            toTargetTime = time;

        }
        bool complete = false;
        bool midpointFired = false;
        public bool Complete
        {
            get
            {
                //Int64 elapsed = HiResTimer.TickCount - fromTime;
                //double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;
                //return (elapsedSeconds > toTargetTime);
                return complete;
            }
        }
        public CameraParameters CurrentPosition
        {
            get
            {
                int elapsed = Date.Now - fromTime;
                double elapsedSeconds = ((double)elapsed) / 1000;

                double alpha = elapsedSeconds / (toTargetTime );

                if (!midpointFired && alpha >= .5)
                {
                    midpointFired = true;

                    if (midpoint != null)
                    {
                        midpoint();
                    }
                }
                if (alpha >= 1.0)
                {
                    alpha = 1.0;
                    complete = true;
                    return to.Copy();
                }
                if (Settings.Active.GalacticMode && WWTControl.Singleton.RenderContext.Space)
                {
                    return CameraParameters.InterpolateGreatCircle(from, to, alpha, InterpolationType, FastDirectionMove);
                }
                return CameraParameters.Interpolate(from, to, alpha, InterpolationType, FastDirectionMove);
            }
        }

        public Date CurrentDateTime
        {
            get
            {
                int elapsed = Date.Now - fromTime;
                double elapsedSeconds = ((double)elapsed) / 1000;

                double alpha = elapsedSeconds / (toTargetTime);

                double delta = (dateTimeSpan) * alpha;

                Date retDate = new Date(fromDateTime.GetTime() + (int)delta);
          
                return retDate;
            }
        }

        Action midpoint;

        public Action Midpoint
        {
            get
            {
                return midpoint;
            }
            set
            {
                midpoint = value;
            }
        }
        

        public double MoveTime
        {
            get { return toTargetTime; }
        }


    }

    class ViewMoverSlew : IViewMover
    {
        CameraParameters from;
        CameraParameters fromTop;
        CameraParameters to;
        CameraParameters toTop;
        Date fromTime;
        double upTargetTime = 0;
        double downTargetTime = 0;
        double toTargetTime = 0;
        double upTimeFactor = .6;
        double downTimeFactor = .6;
        double travelTimeFactor = 7.0;
        public static ViewMoverSlew Create(CameraParameters from, CameraParameters to)
        { 
            ViewMoverSlew temp = new ViewMoverSlew();
            temp.Init(from, to);
            return temp;
        }

        public ViewMoverSlew()
        {
        }

        public static ViewMoverSlew CreateUpDown(CameraParameters from, CameraParameters to, double upDowFactor)
        {
            ViewMoverSlew temp = new ViewMoverSlew();
            temp.upTimeFactor = temp.downTimeFactor = upDowFactor;
            temp.Init(from.Copy(), to.Copy());
            return temp;
        }   
        
        public void Init(CameraParameters from, CameraParameters to)
        {      
            if (Math.Abs(from.Lng - to.Lng) > 180)
            {
                if (from.Lng > to.Lng)
                {
                    from.Lng -= 360;
                }
                else
                {
                    from.Lng += 360;
                }
            }

            if (to.Zoom <= 0)
            {
                to.Zoom = 360;
            }
            if (from.Zoom <= 0)
            {
                from.Zoom = 360;
            }
            this.from = from;
            this.to = to;
            fromTime = Date.Now;
            double zoomUpTarget = 360.0;
            double travelTime;

            double lngDist = Math.Abs(from.Lng - to.Lng);
            double latDist = Math.Abs(from.Lat - to.Lat);
            double distance = Math.Sqrt(latDist * latDist + lngDist * lngDist);


            zoomUpTarget = (distance / 3) * 20;
            if (zoomUpTarget > 360.0)
            {
                zoomUpTarget = 360.0;
            }

            if (zoomUpTarget < from.Zoom)
            {
                zoomUpTarget = from.Zoom;
            }

            travelTime = (distance / 180.0) * (360 / zoomUpTarget) * travelTimeFactor;

            double rotateTime = Math.Max(Math.Abs(from.Angle - to.Angle), Math.Abs(from.Rotation - to.Rotation)) ;


            double logDistUp = Math.Max(Math.Abs(Util.LogN(zoomUpTarget, 2) - Util.LogN(from.Zoom, 2)), rotateTime);
            upTargetTime = upTimeFactor * logDistUp;
            downTargetTime = upTargetTime + travelTime;
            double logDistDown = Math.Abs(Util.LogN(zoomUpTarget, 2) - Util.LogN(to.Zoom, 2));
            toTargetTime = downTargetTime + Math.Max((downTimeFactor * logDistDown),rotateTime);

            fromTop = from.Copy();
            fromTop.Zoom = zoomUpTarget;
            fromTop.Angle = (from.Angle + to.Angle) / 2.0; //todo make short wrap arounds..
            fromTop.Rotation = (from.Rotation + to.Rotation) / 2.0;
            toTop = to.Copy();
            toTop.Zoom = fromTop.Zoom;
            toTop.Angle = fromTop.Angle;
            toTop.Rotation = fromTop.Rotation;

        }

        bool midpointFired = false;
        bool complete = false;
        public bool Complete
        {
            get
            {
                //Int64 elapsed = HiResTimer.TickCount - fromTime;
                //double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;
                //return (elapsedSeconds > toTargetTime);
                return complete;
            }
        }
        public CameraParameters CurrentPosition
        {
            get
            {
                int elapsed = Date.Now - fromTime;
                double elapsedSeconds = ((double)elapsed) / 1000;

                if (elapsedSeconds < upTargetTime)
                {
                    // Log interpolate from from to fromTop
                    return CameraParameters.Interpolate(from, fromTop, elapsedSeconds / upTargetTime, InterpolationType.EaseInOut, false);
                }
                else if (elapsedSeconds < downTargetTime)
                {
                    elapsedSeconds -= upTargetTime;
                    if (Settings.Active.GalacticMode && WWTControl.Singleton.RenderContext.Space)
                    {
                        return CameraParameters.InterpolateGreatCircle(fromTop, toTop, elapsedSeconds / (downTargetTime - upTargetTime), InterpolationType.EaseInOut, false);
                    }
                    // interpolate linear fromTop and toTop
                    return CameraParameters.Interpolate(fromTop, toTop, elapsedSeconds / (downTargetTime - upTargetTime), InterpolationType.EaseInOut, false);
                }
                else
                {
                    if (!midpointFired )
                    {
                        midpointFired = true;

                        if (midpoint != null)
                        {
                            midpoint();
                        }
                  
                    }
                    elapsedSeconds -= downTargetTime;
                    // Interpolate log from toTop and to
                    double alpha = elapsedSeconds / (toTargetTime - downTargetTime);
                    if (alpha > 1.0)
                    {
                        alpha = 1.0;
                        complete = true;
                        return to.Copy();
                    }
                    return CameraParameters.Interpolate(toTop, to, alpha, InterpolationType.EaseInOut, false);
                }
            }
        }

        public Date CurrentDateTime
        {
            get
            {
                SpaceTimeController.UpdateClock();
                return SpaceTimeController.Now;
            }
        }

        Action midpoint;

        public Action Midpoint
        {
            get
            {
                return midpoint;
            }
            set
            {
                midpoint = value;
            }
        }

        public double MoveTime
        {
            get { return toTargetTime; }
        }
    }
}


