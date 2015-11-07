using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Runtime.CompilerServices;
using System.Html.Media.Graphics;
using System.Xml;

namespace wwtlib
{
    class Util
    {
        public static int GetHashCode(string target)
        {
            int hash = 0;

            if (target.Length == 0)
            {
                return hash;
            }

            for (int i = 0; i < target.Length; i++)
            {
                int c = target.CharCodeAt(i);
                hash = ((hash << 5) - hash) + c;  
            }
            return hash;

        }

        public static double LogN(double num, double b)
        {
            return Math.Log(num) / Math.Log(b);
        }

        public static String GetUrlParam(string name)
        {
             //Document.URL

               
        //    return RegularExpression.Parse(name + "=" + "(.+?)(&|$)").Exec(Document.URL.Search)||[,null])[1]
//           function getURLParameter(name) { 
//    return decodeURI( 
//        (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1] 
//    ); 
//} 

//            RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1]

//                "[?&]"+name+"=([^&]*)" 

            return "";
        }

        

        public static string GetProxiedUrl(string url)
        {
            

            if ((url.ToLowerCase().StartsWith("http://worldwidetelescope.org") ||
                url.ToLowerCase().StartsWith("http://www.worldwidetelescope.org")) && url.ToLowerCase().IndexOf("worldwidetelescope.org/wwtweb/") == -1)
            {
                return url.Split("worldwidetelescope.org")[1];
            }
            if ((url.ToLowerCase().StartsWith("http://wwtstaging.azurewebsites.net") ||
                url.ToLowerCase().StartsWith("http://wwtstaging.azurewebsites.net")) && url.ToLowerCase().IndexOf("wwtstaging.azurewebsites.net/wwtweb/") == -1)
            {
                return url.Split("wwtstaging.azurewebsites.net")[1];
            }

            if (url.ToLowerCase().StartsWith("http"))
            {
                return "http://www.worldwidetelescope.org/webserviceproxy.aspx?targeturl=" + url.EncodeUriComponent();
            }
            return url;
        }
        // Parse timespan into int with milliseconds
        public static int ParseTimeSpan(string timespan)
        {
            int val = 0;

            string[] parts = timespan.Split(":");

            if (parts.Length == 3)
            {
                val += int.Parse(parts[0]) * 36000000;
                val += int.Parse(parts[1]) * 600000;
                val += (int)(double.Parse(parts[2]) * 1000);
            }
            return val;
        }

        public static string GetTourComponent(string url, string name)
        {
            if (url.IndexOf("worldwidetelescope.org") != -1 || url.IndexOf("wwtstaging.azurewebsites") != -1)
            {
                return url;
            }

            return "http://www.worldwidetelescope.org/GetTourFile.aspx?targeturl=" + url.EncodeUriComponent() + "&filename=" + name;
        }

        public static XmlNode SelectSingleNode(XmlNode parent, string name)
        {
            //XmlNode
            XmlNode node = null;

            //try
            //{
            //    node = parent.QuerySelector(name);
            //}
            //catch
            //{
                foreach (XmlNode child in parent.ChildNodes)
                {
                    if (child.Name == name)
                    {
                        node = child;
                        break;
                    }
                }
            //}

            return node;
        }

        public static string GetInnerText(XmlNode node)
        {

            if (string.IsNullOrEmpty(node.InnerText))
            {
                ChromeNode cn = (ChromeNode)(object)node;
                return cn.TextContent;
            }
            else
            {
                return node.InnerText;
            }
        }

        public static List<string> GetWrappedText(CanvasContext2D ctx, string text, double width)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(" ");

            string currentLine = "";

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    if (currentLine == "" || ctx.MeasureText(currentLine + " " + words[i]).Width < width)
                    {
                        currentLine += " " + words[i];
                    }
                    else
                    {
                        lines.Add(currentLine);
                        currentLine = words[i];
                    }
                }
            }
            if (currentLine != "")
            {
                lines.Add(currentLine);
            }


            return lines;
        }

        public static string ToHex(float number)
        {
            int num = Math.Max(0, Math.Min((int)number, 255));
            return "0123456789ABCDEF".Substr((num - num % 16) / 16, 1) + "0123456789ABCDEF".Substr(num % 16, 1);

        }

        public static int FromHex(string data)
        {
            int val = 0;


            switch (data.Substr(1, 1).ToUpperCase())
            {
                case "A":
                    val += 10;
                    break;
                case "B":

                    val += 11;
                    break;
                case "C":
                    val += 12;
                    break;
                case "D":
                    val += 13;
                    break;
                case "E":
                    val += 14;
                    break;
                case "F":
                    val += 15;
                    break;
                default:
                    val += int.Parse(data.Substr(1, 1));
                    break;
            }
            switch (data.Substr(0, 1).ToUpperCase())
            {
                case "A":
                    val += 10 * 16;
                    break;
                case "B":
                    val += 11 * 16;
                    break;
                case "C":
                    val += 12 * 16;
                    break;
                case "D":
                    val += 13 * 16;
                    break;
                case "E":
                    val += 14 * 16;
                    break;
                case "F":
                    val += 15 * 16;
                    break;
                default:
                    val += int.Parse(data.Substr(0, 1)) * 16;
                    break;
            }

            return val;

        }
        internal static void OpenUrl(string p, bool p_2)
        {
            //todo implement this by opening new window in HTML
        }


        public static double Log10(double num)
        {
            return Math.Log(num) / 2.302585092994046;
        }

        public static double Sign(double num)
        {
            if (num < 0)
            {
                return -1;
            }
            return 1;
        }
    }



    public class Rectangle
    {
        public Rectangle()
        {
        }

        public double X;
        public double Y;
        public double Width;
        public double Height;

        public double Left
        {
            get
            {
                return X ;
            }
        }
        public double Right
        {
            get
            {
                return X + Width;
            }
        }
        public double Top
        {
            get
            {
                return Y;
            }
        }
        public double Bottom
        {
            get
            {
                return Y + Height;
            }
        }


        public Rectangle Copy()
        {
            Rectangle temp = new Rectangle();
            temp.X = X;
            temp.Y = Y;
            temp.Width = Width;
            temp.Height = Height;
            return temp;
        }

        public static Rectangle Create(double x, double y, double width, double height)
        {
            Rectangle temp = new Rectangle();
            temp.X = x;
            temp.Y = y;
            temp.Width = width;
            temp.Height = height;
            return temp;
        }
    }

    public class Guid
    {
        static int nextId = 11232;

        string guid = (nextId++).ToString();

        public Guid()
        {
        }

        public static Guid NewGuid()
        {
            return new Guid();
        }

        public override string ToString()
        {
            return guid;
        }
        public static Guid FromString(string id)
        {
            Guid temp = new Guid();
            temp.guid = id;
            return temp;
        }

    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("navigator")]
    public static class Navigator
    {
        [ScriptField]
        [ScriptName("appVersion")]
        public static string AppVersion { get { return ""; } }

        [ScriptField]
        [ScriptName("geolocation")]
        public static GeoLocation Geolocation { get { return null; } }


        [ScriptField]
        [ScriptName("userAgent")]
        public static string UserAgent { get { return null; } }

    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public delegate void GeoEventHandler(Position pos);

    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("geolocation")]
    public class GeoLocation
    {

        [ScriptName("getCurrentPosition")]
        public void GetCurrentPosition(GeoEventHandler handler)
        {
        }

        [ScriptName("getCurrentPosition")]
        public void GetCurrentPosition(GeoEventHandler handler,GeoEventHandler error)
        {
        }
    }


    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("position")]
    public class Position
    {
        [ScriptField]
        [ScriptName("coords")]
        public GeoCoordinates Coords { get { return null; } }

    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("coordinates")]
    public class GeoCoordinates
    {
        [ScriptField]
        [ScriptName("latitude")]
        public double Latitude { get { return 0; } }
        [ScriptField]
        [ScriptName("longitude")]
        public double Longitude { get { return 0; } }
        [ScriptField]
        [ScriptName("altitude")]
        public double Altitude { get { return 0; } }
    }


    public static class Mouse
    {
        public static int OffsetX(CanvasElement canvas, ElementEvent e)
        {
            int x = 0;
            MouseCanvasElement element = (MouseCanvasElement)(object)canvas;
            MouseEvent me = (MouseEvent)(object)e;
            if (element.offsetParent != null)
            {
                do
                {
                    x += element.offsetLeft;
                } while ((element = element.offsetParent) != null);
            }

            // Add padding and border style widths to offset
            //x += me.stylePaddingLeft;

            //x += me.styleBorderLeft;

            return me.PageX - x;
        }
        public static int OffsetY(CanvasElement canvas, ElementEvent e)
        {
            int y = 0;
            MouseCanvasElement element = (MouseCanvasElement)(object)canvas;
            MouseEvent me = (MouseEvent)(object)e;
            if (element.offsetParent != null)
            {
                do
                {
                    y += element.offsetTop;
                } while ((element = element.offsetParent) != null);
            }

            // Add padding and border style widths to offset
            //y += me.stylePaddingTop;

            //y += me.styleBorderTop;

            return me.PageY - y;
        }
    }
}
