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

        public static int Compare(int l, int r)
        {
            if (l==r)
            {
                return 0;
            }
            if (l > r)
            {
                return 1;
            }

            return -1;

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


            if (url.ToLowerCase().StartsWith("http://worldwidetelescope.org") ||
                url.ToLowerCase().StartsWith("http://www.worldwidetelescope.org"))
            {
                if (url.ToLowerCase().IndexOf("worldwidetelescope.org/wwtweb/") < 12)
                {
                    return url;
                }
                return url.Split("worldwidetelescope.org")[1];
            }
            if (url.ToLowerCase().StartsWith("http://wwtstaging.azurewebsites.net") ||
                url.ToLowerCase().StartsWith("http://wwtstaging.azurewebsites.net"))
            {
                if (url.ToLowerCase().IndexOf("wwtstaging.azurewebsites.net/wwtweb/") < 12)
                {
                    return url;
                }
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

        // convert duration to HH:MM:SS.S
        public static string XMLDuration(int duration)
        {
            double s = duration / 1000.0;

            int hours = Math.Floor(s / 3600);
            int min = Math.Floor(s / 60) - (hours * 60);
            double sec = (s) - ((hours * 3600) + min * 60);

            return string.Format("{0}:{1}:{2}", hours, min, sec);
        }

        public static string GetTourComponent(string url, string name)
        {
            return "http://www.worldwidetelescope.org/GetTourFile.aspx?targeturl=" + url.EncodeUriComponent() + "&filename=" + name;
        }

        public static string XMLDate(Date d)
        {
            int hours = d.GetHours();
            string amPm = "AM";
            if (hours > 12)
            {
                hours -= 12;
                amPm = "PM";
            }

            return (d.GetMonth() + 1).ToString() + '/' +
                d.GetDate().ToString() + '/' +
                d.GetFullYear().ToString() + ' ' +
                hours.ToString() + ":" +
                d.GetMinutes().ToString() + ":" +
                d.GetSeconds().ToString() + " " +
                amPm;
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
            lines.Add(text);
            return lines;


            //string[] words = text.Split(" ");

            //string currentLine = "";

            //for (int i = 0; i < words.Length; i++)
            //{
            //    if (!string.IsNullOrEmpty(words[i]))
            //    {
            //        if (currentLine == "" || ctx.MeasureText(currentLine + " " + words[i]).Width < width)
            //        {
            //            currentLine += " " + words[i];
            //        }
            //        else
            //        {
            //            lines.Add(currentLine);
            //            currentLine = words[i];
            //        }
            //    }
            //}
            //if (currentLine != "")
            //{
            //    lines.Add(currentLine);
            //}


            //return lines;
        }

        public static string ToHex(double number)
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
                return X;
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

        public bool Contains(Vector2d point)
        {
            return (Between(point.X, X, X + Width) && Between(point.Y, Y, Y + Height));

        }

        private bool Between(double n, double n1, double n2)
        {
            if (n1 > n2)
            {
                return !(n > n1) && !(n < n2);
            }
            else
            {
                return !(n < n1) && !(n > n2);
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

    public class Enums
    {
        public static int Parse(string enumType, string value)
        {
            if (value == "Default")
            {
                value = "DefaultV";
            }

            if (value == "0")
            {
                return 0;
            }

            string val = value.Substr(0, 1).ToLowerCase() + value.Substr(1);

            return (int)Script.Literal("wwtlib[{0}][{1}]", enumType, val);

            // Script.Literal(" var x; var p = Object.keys(wwtlib[{0}]); for (var i in p)\n {{ if ( p[i].toLowerCase() == {1}.toLowerCase() ) {{\n x = wwtlib[{0}][p[i]]; break; \n}}\n }}", enumType, value);

            // return (int)Script.Literal(" x");
        }

        public static string ToXml(string enumType, int value)
        {
            Script.Literal(" var x = \"0\"; var p = Object.keys(wwtlib[{0}]); for (var i in p)\n {{ if ( wwtlib[{0}][p[i]] == {1} ) {{\n x = p[i]; break; \n}}\n }}", enumType, value);
            string val = (string)Script.Literal(" x");

            string enumString  = val.Substr(0, 1).ToUpperCase() + val.Substr(1);
            if (enumString == "DefaultV")
            {
                enumString = "Default";
            }

            return enumString;
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
        public void GetCurrentPosition(GeoEventHandler handler, GeoEventHandler error)
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

    public class Language
    {
        public static string GetLocalizedText(int id, string text)
        {
            //todo make this actually use localization file.
            return text;
        }
    }

    public class Cursor
    {
        public static Vector2d Position
        {
            get
            {
                return new Vector2d();
            }
        }

        public static string Current
        {
            set
            {
                Document.Body.Style.Cursor = value;
            }
            get
            {
                return Document.Body.Style.Cursor;
            }
        }
    }

    //
    // Summary:
    //     Provides a collection of System.Windows.Forms.Cursor objects for use by a Windows
    //     Forms application.
    public sealed class Cursors
    {

        //
        // Summary:
        //     Gets the arrow cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the arrow cursor.
        public static string Arrow
        {
            get
            {
                return "default";
            }
        }
        //
        // Summary:
        //     Gets the crosshair cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the crosshair cursor.
        public static string Cross
        {
            get
            {
                return "crosshair";
            }
        }

        //
        // Summary:
        //     Gets the default cursor, which is usually an arrow cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the default cursor.
        public static string DefaultV
        {
            get
            {
                return "default";
            }
        }
        //
        // Summary:
        //     Gets the hand cursor, typically used when hovering over a Web link.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the hand cursor.
        public static string Hand
        {
            get
            {
                return "grab";
            }
        }
        //
        // Summary:
        //     Gets the Help cursor, which is a combination of an arrow and a question mark.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the Help cursor.
        public static string Help
        {
            get
            {
                return "help";
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears when the mouse is positioned over a horizontal splitter
        //     bar.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears when
        //     the mouse is positioned over a horizontal splitter bar.
        public static string HSplit
        {
            get
            {
                return "row-resize";
            }
        }
        //
        // Summary:
        //     Gets the I-beam cursor, which is used to show where the text cursor appears when
        //     the mouse is clicked.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the I-beam cursor.
        public static string IBeam
        {
            get
            {
                return "text";
            }
        }
        //
        // Summary:
        //     Gets the cursor that indicates that a particular region is invalid for the current
        //     operation.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that indicates that
        //     a particular region is invalid for the current operation.
        public static string No
        {
            get
            {
                return "not-allowed";
            }
        }


        //
        // Summary:
        //     Gets the four-headed sizing cursor, which consists of four joined arrows that
        //     point north, south, east, and west.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the four-headed sizing cursor.
        public static string SizeAll
        {
            get
            {
                return "help";
            }
        }
        //
        // Summary:
        //     Gets the two-headed diagonal (northeast/southwest) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents two-headed diagonal (northeast/southwest)
        //     sizing cursor.
        public static string SizeNESW
        {
            get
            {
                return "nwse-resize";
            }
        }
        // Summary:
        //     Gets the two-headed vertical (north/south) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed vertical (north/south)
        //     sizing cursor.
        public static string SizeNS
        {
            get
            {
                return "ns-resize";
            }
        }
        //
        // Summary:
        //     Gets the two-headed diagonal (northwest/southeast) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed diagonal (northwest/southeast)
        //     sizing cursor.
        public static string SizeNWSE
        {
            get
            {
                return "nwse-resize";
            }
        }
        //
        // Summary:
        //     Gets the two-headed horizontal (west/east) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed horizontal (west/east)
        //     sizing cursor.
        public static string SizeWE
        {
            get
            {
                return "ew-resize";
            }
        }
        //
        // Summary:
        //     Gets the up arrow cursor, typically used to identify an insertion point.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the up arrow cursor.
        public static string UpArrow
        {
            get
            {
                return "help";
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears when the mouse is positioned over a vertical splitter
        //     bar.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears when
        //     the mouse is positioned over a vertical splitter bar.
        public static string VSplit
        {
            get
            {
                return "col-resize";
            }
        }
        //
        // Summary:
        //     Gets the wait cursor, typically an hourglass shape.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the wait cursor.
        public static string WaitCursor
        {
            get
            {
                return "wait";
            }
        }
    }

    //
    // Summary:
    //     Specifies key codes and modifiers.

    public enum Keys
    {
        //
        // Summary:
        //     The bitmask to extract modifiers from a key value.
        Modifiers = -65536,
        //
        // Summary:
        //     No key pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        ClearKey = 12,
        //
        // Summary:
        //     The RETURN key.
        ReturnKey = 13,
        //
        // Summary:
        //     The ENTER key.
        Enter = 13,
        //
        // Summary:
        //     The SHIFT key.
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        Menu = 18,
        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,
        //
        // Summary:
        //     The CAPS LOCK key.
        Capital = 20,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        HanguelMode = 21,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //
        // Summary:
        //     The ESC key.
        Escape = 27,
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
        IMEAceept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,
        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        Prior = 33,
        //
        // Summary:
        //     The PAGE UP key.
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        Next = 34,
        //
        // Summary:
        //     The PAGE DOWN key.
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        Snapshot = 44,
        //
        // Summary:
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        InsertKey = 45,
        //
        // Summary:
        //     The DEL key.
        DeleteKey = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,
        //
        // Summary:
        //     The NUM LOCK key.
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        RMenu = 165,
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //
        // Summary:
        //     The OEM 1 key.
        Oem1 = 186,
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        Oemplus = 187,
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        Oemcomma = 188,
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //
        // Summary:
        //     The OEM 2 key.
        Oem2 = 191,
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        Oemtilde = 192,
        //
        // Summary:
        //     The OEM 3 key.
        Oem3 = 192,
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //
        // Summary:
        //     The OEM 4 key.
        Oem4 = 219,
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //
        // Summary:
        //     The OEM 5 key.
        Oem5 = 220,
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //
        // Summary:
        //     The OEM 6 key.
        Oem6 = 221,
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //
        // Summary:
        //     The OEM 7 key.
        Oem7 = 222,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = 223,
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //
        // Summary:
        //     The OEM 102 key.
        Oem102 = 226,
        //
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        Attn = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,
        //
        // Summary:
        //     The bitmask to extract a key code from a key value.
        KeyCode = 65535,
        //
        // Summary:
        //     The SHIFT modifier key.
        Shift = 65536,
        //
        // Summary:
        //     The CTRL modifier key.
        Control = 131072,
        //
        // Summary:
        //     The ALT modifier key.
        Alt = 262144
    }

    public class SimpleInput
    {
        private string url;
        private string v1;
        private string v2;
        private int v3;

        public SimpleInput(string v1, string v2, string url, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.url = url;
            this.v3 = v3;
        }

        public string ResultText = new string();

        public DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }
    }

    public enum DialogResult { OK = 1, };

    public class SelectLink
    {
        public string ID = new string();

        public TourDocument Tour = null;

        internal DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }
    }


    public class PopupVolume
    {
        public int Volume = 0;

        public DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }
    }

    public class PopupColorPicker
    {
        public int Volume = 0;

        public Vector2d Location = new Vector2d();

        public Color Color = new Color();


        public DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }
    }
    public class OverlayProperties
    {
        public int Volume = 0;

        public Vector2d Location = new Vector2d();

        public Overlay Overlay = null;


        public DialogResult ShowDialog()
        {

            return DialogResult.OK;
        }
    }
}
