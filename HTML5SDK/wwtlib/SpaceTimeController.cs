using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class SpaceTimeController
    {
        static public void UpdateClock()
        {
            if (syncToClock)
            {
                Date justNow = Date.Now;

                if (timeRate != 1.0)
                {
                    int ts = justNow.GetTime() - last.GetTime();
                    int ticks = (int)(ts * timeRate);
                    offset += ticks;
                }
                last = justNow;
                try
                {
                    //now = new Date(justNow.GetTime());
                    now = new Date(justNow.GetTime() + offset);

                }
                catch
                {
                    now = new Date(1, 12, 25, 23, 59, 59);
                    offset = now - Date.Now;
                }

                if (now.GetFullYear() > 4000)
                {
                    now = new Date(4000, 12, 31, 23, 59, 59);
                    offset = now - Date.Now;
                }

                if (now.GetFullYear() < 1)
                {
                    now = new Date(0, 12, 25, 23, 59, 59);
                    offset = now - Date.Now;
                }

            }
        }

        static public Date GetTimeForFutureTime(double delta)
        {
            try
            {
                if (syncToClock)
                {
                    Date future = new Date ((int)((int)Now.GetTime() + (delta * 1000) * timeRate));
                    return future;
                }
                else
                {
                    return Now;
                }
            }
            catch
            {
                return Now;
            }
        }
        static public double GetJNowForFutureTime(double delta)
        {
            try
            {
                if (syncToClock)
                {
                    Date future =new Date(Now.GetTime() +(int)(delta * 1000 * timeRate));
                    return UtcToJulian(future);
                }
                else
                {
                    return UtcToJulian(Now);
                }
            }
            catch
            {
                return UtcToJulian(Now);
            }
        }
        static public Date Now
        {
            get
            {
                return now;
            }
            set
            {
                now = value;
                offset = now - Date.Now;
                last = Date.Now;
            }
        }
        public static Date last = Date.Now;


        public static void SyncTime()
        {
            offset = 0;
            now = Date.Now;
            syncToClock = true;
        }

        static int offset = 0;

        static Date now = Date.Now;

        static public double JNow
        {
            get
            {
                return UtcToJulian(Now);
            }
        }

        static bool syncToClock = true;

        public static bool SyncToClock
        {
            get { return SpaceTimeController.syncToClock; }
            set
            {
                if (SpaceTimeController.syncToClock != value)
                {
                    SpaceTimeController.syncToClock = value;
                    if (value)
                    {
                        last = Date.Now;
                        offset = now - Date.Now;
                    }
                    else
                    {
                        now = new Date(Date.Now.GetTime() + offset);
                    }
                }
            }
        }

        static private double timeRate = 1;

        static public double TimeRate
        {
            get { return timeRate; }
            set { timeRate = value; }
        }

        static private Coordinates location;
        static private double altitude;

        public static double Altitude
        {
            get { return SpaceTimeController.altitude; }
            set { SpaceTimeController.altitude = value; }
        }

        static public Coordinates Location
        {
            get
            {
                //if (location == null)
                {
                    location = Coordinates.FromLatLng(Settings.Active.LocationLat, Settings.Active.LocationLng);
                    altitude = Settings.Active.LocationAltitude;
                }
                return location;
            }
            set
            {
                if (Settings.GlobalSettings.LocationLat != value.Lat)
                {
                    Settings.GlobalSettings.LocationLat = value.Lat;
                }

                if (Settings.GlobalSettings.LocationLng != value.Lng)
                {
                    Settings.GlobalSettings.LocationLng = value.Lng;
                }
                location = value;
            }
        }

        public static Date JulianToUtc(double jDate)
        {
            DT date = new DT();
            date.SetJD(jDate, true);

            //date.Get(ref year, ref month, ref day, ref hour, ref minute, ref second);

            
            double ms = (date.Second() - ((int)date.Second())) * 1000;

            return new Date(date.Year(), date.Month()-1, date.Day(), date.Hour(), date.Minute(), (int)date.Second(), (int)ms);
        }

        internal static double TwoLineDateToJulian(string p)
        {
            bool pre1950 = Int32.Parse(p.Substring(0, 1)) < 6;
            int year = Int32.Parse((pre1950 ? " 20" : "19") + p.Substring(0, 2));
            double days = double.Parse(p.Substring(2, 3));
            double fraction = double.Parse(p.Substr(5));

            //TimeSpan ts = TimeSpan.FromDays(days - 1 + fraction);
           
            //DateTime date = new DateTime(year, 1, 1, 0, 0, 0, 0);

            Date date = new Date(year, 0, 1, 0, 0);
            return UtcToJulian(date) + (days-1 + fraction);
        }

        public static string JulianToTwoLineDate(double jDate)
        {
            return DateToTwoLineDate(JulianToUtc(jDate));
        }

        public static string DateToTwoLineDate(Date date)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(date.GetFullYear() % 100);

            Date fullYear = new Date(date.GetFullYear(), 0, 1, 0, 0);

            double dayofyear = Math.Floor((date - fullYear) / (60 * 60 * 24 * 1000))+2;

            double day = dayofyear + date.GetHours() / 24 + date.GetMinutes() / 60 / 24 + date.GetSeconds() / 60 / 60 / 24 + date.GetMilliseconds() / 1000 / 60 / 60 / 24;

            string sDay = TLEDayString(day);

            sb.Append(sDay);

            return sb.ToString();
        }

        public static string TLEDayString(double day)
        {
            string formated = day.ToString();

            int point = formated.IndexOf(".");
            if (point == -1)
            {
                point = formated.Length;
                formated += ".0";
            }
            int len = formated.Length-point-1;

            string fill = "00000000";

            formated = fill.Substr(0, 3 - point) + formated + fill.Substr(0, 8 - len);

            return formated;
        }



        public static double UtcToJulian(Date utc)
        {
            int year = utc.GetUTCFullYear();
            int month = utc.GetUTCMonth()+1;
            double day = utc.GetUTCDate();
            double hour = utc.GetUTCHours();
            double minute = utc.GetUTCMinutes();
            double second = utc.GetUTCSeconds() + utc.GetUTCMilliseconds() / 1000.0;
            double dblDay = day + (hour / 24.0) + (minute / 1440.0) + (second / 86400.0);

            return AstroCalc.GetJulianDay(year, month, dblDay);
            //return DateToJD(year, month, dblDay, true);
            //return julianDays;
        }

        public static double DateToJD(int Year, int Month, double Day, bool bGregorianCalendar)
        {
            int Y = Year;
            int M = Month;
            if (M < 3)
            {
                Y = Y - 1;
                M = M + 12;
            }

            int A = 0;
            int B = 0;
            if (bGregorianCalendar)
            {
                A = (int)(Y / 100.0);
                B = 2 - A + (int)(A / 4.0);
            }

            return (int)(365.25 * (Y + 4716)) + (int)(30.6001 * (M + 1)) + Day + B - 1524.5;
        }
    }
}
