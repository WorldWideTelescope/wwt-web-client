using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class UiTools
    {
        static public string GetNamesStringFromArray(string[] array)
        {
            string names = "";
            string delim = "";
            foreach (string name in array)
            {
                names += delim;
                names += name;
                delim = ";";
            }
            return names;
        }

        public const double KilometersPerAu = 149598000;
        public const double AuPerParsec = 206264.806;
        public const double AuPerLightYear = 63239.6717;
        public const double SSMUnitConversion = 370; // No idea where this fudge factors comes from
        public static double SolarSystemToMeters(double SolarSystemCameraDistance)
        {
            return SolarSystemCameraDistance * UiTools.KilometersPerAu * SSMUnitConversion;
        }

        public static double MetersToSolarSystemDistance(double meters)
        {
            return meters / SSMUnitConversion * UiTools.KilometersPerAu;
        }


        public static double MetersToZoom(double meters)
        {
            return ((meters / 1000 / SSMUnitConversion) - 0.000001) / 4 * 9;
        }

        // Distance is stored in AU in WWT but is displayed in KM AU, LY, MPC
        public static string FormatDistance(double distance)
        {

            if (distance < .1)
            {
                // Kilometers
                double km = (distance * KilometersPerAu);

                if (km < 10)
                {
                    double m = (int)(km * 1000);
                    return m.Format("N1") + " m";
                }
                else
                {
                    km = (int)km;
                    return km.Format("N1") + " km";
                }
            }
            else if (distance < (10))
            {
                //Units in u
                double au = ((int)(distance * 10)) / 10.0;
                return au.Format("N1") + " au";
            }
            else if (distance < (AuPerLightYear / 10.0))
            {
                //Units in u
                double au = (int)(distance);
                return au.Format("N1") + " au";
            }
            else if (distance < (AuPerLightYear * 10))
            {
                // Units in lightyears
                double ly = ((int)((distance * 10) / AuPerLightYear)) / 10.0;
                return ly.Format("N1") + " ly";
            }
            else if (distance < (AuPerLightYear * 1000000))
            {
                // Units in lightyears
                double ly = ((int)((distance) / AuPerLightYear));
                return ly.Format("N1") + " ly";
            }
            else if (distance < (AuPerParsec * 10000000))
            {
                double mpc = ((int)((distance * 10) / (AuPerParsec * 1000000.0))) / 10.0;
                return mpc.Format("N1") + " Mpc";
            }
            else if (distance < (AuPerParsec * 1000000000))
            {
                double mpc = ((int)((distance) / (AuPerParsec * 1000000.0)));
                return mpc.Format("N1") + " Mpc";
            }
            else
            {
                double mpc = ((int)((distance * 10) / (AuPerParsec * 1000000000.0))) / 10.0;
                return mpc.Format("N1") + " Gpc";
            }
        }
        public static string FormatDecimalHours(double dayFraction)
        {
            Date today = Date.Now;

            double ts = today.GetTimezoneOffset() / 60;
            ts = 0;
            double day = (dayFraction - ts) + 0.0083333334;
            while (day > 24)
            {
                day -= 24;
            }
            while (day < 0)
            {
                day += 24;
            }
            int hours = (int)day;
            int minutes = (int)((day * 60.0) - ((double)hours * 60.0));
            int seconds = (int)((day * 3600) - (((double)hours * 3600) + ((double)minutes * 60.0)));

            return string.Format("{0}:{1}", hours, minutes, seconds);
            //return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        public static List<string> SplitString(string data, string delimiter)
        {

            List<string> output = new List<string>();

            int nestingLevel = 0;

            int current = 0;
            int count = 0;
            int start = 0;

            while (current < data.Length)
            {
                if (data.Substr(current, 1) == "(")
                {
                    nestingLevel++;
                }
                if (data.Substr(current, 1) == ")")
                {
                    nestingLevel--;
                }
                if (current == (data.Length - 1))
                {
                    count++;
                }

                if (current == (data.Length - 1) || (data.Substr(current, 1) == delimiter && delimiter == "\t") || (nestingLevel == 0 && data.Substr(current, 1) == delimiter))
                {
                    output.Add(data.Substr(start, count));
                    start = current + 1;
                    count = 0;
                }
                else
                {
                    count++;
                }
                current++;
            }

            return output;
        }

        public static List<string> Split(string data, string delimiters)
        {

            List<string> output = new List<string>();

            int nestingLevel = 0;

            int current = 0;
            int count = 0;
            int start = 0;

            while (current < data.Length)
            {
         
                if (current == (data.Length - 1))
                {
                    count++;
                }

                if (current == (data.Length - 1) || delimiters.IndexOf(data.Substr(current, 1)) > -1 )
                {
                    output.Add(data.Substr(start, count));
                    start = current + 1;
                    count = 0;
                }
                else
                {
                    count++;
                }
                current++;
            }

            return output;
        }

    }
}
