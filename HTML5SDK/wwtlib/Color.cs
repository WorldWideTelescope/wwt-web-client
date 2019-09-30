using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class Color
    {
        public float A = 255.0f;
        public float B = 255.0f;
        public float G = 255.0f;
        public float R = 255.0f;
        public string Name = "";
        public static Color FromArgb(float a, float r, float g, float b)
        {
            Color temp = new Color();
            temp.A = a;
            temp.R = r;
            temp.G = g;
            temp.B = b;

            return temp;
        }



        internal static Color FromArgbColor(int a, Color col)
        {
            Color temp = new Color();
            temp.A = a;
            temp.R = col.R;
            temp.G = col.G;
            temp.B = col.B;

            return temp;
        }


        public static Color FromName(string name)
        {
            Color temp = Load(name);
            return temp;
        }
        public string ToFormat()
        {
            if (string.IsNullOrEmpty(Name))
            {
               return String.Format("rgb({0},{1},{2})", R.ToString(), G.ToString(), B.ToString());

                // return String.Format("#{0}{1}{2}{3}", Util.ToHex(A), Util.ToHex(R), Util.ToHex(G), Util.ToHex(B));
            }
            else
            {
                return Name;
            }
        }

        public string Save()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return string.Format("{0}:{1}",
                    0, Name);
            }
            else
            {
                return string.Format("{0}:{1}:{2}:{3}:{4}", 1, A, R, G, B);
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {

                 return String.Format("#{0}{1}{2}", Util.ToHex(R), Util.ToHex(G), Util.ToHex(B));
                // return String.Format("#{0}{1}{2}{3}", Util.ToHex(A), Util.ToHex(R), Util.ToHex(G), Util.ToHex(B));
            }
            else
            {
                return Name;
            }
        }

        public string ToSimpleHex()
        {
            if (string.IsNullOrEmpty(Name))
            {

                return String.Format("{0}{1}{2}{3}", Util.ToHex(A), Util.ToHex(R), Util.ToHex(G), Util.ToHex(B));
            }
            else
            {
                return Name;
            }
        }
        public static Color Load(string color)
        {
            int a=255, r=255, g=255, b=255;

            string[] pieces = color.Split(":");

            if (pieces.Length == 5)
            {

                a = int.Parse(pieces[1]);
                r = int.Parse(pieces[2]);
                g = int.Parse(pieces[3]);
                b = int.Parse(pieces[4]);
            }
            else if (pieces.Length == 2)
            {
                return Color.FromName(pieces[1].ToLowerCase());
            }
            else if (pieces.Length == 1 && pieces[0].StartsWith("#"))
            {
                return FromHex(pieces[0]);
            }
            else if (pieces.Length == 1 && pieces[0].Length == 8)
            {
                return FromSimpleHex(pieces[0]);
            }
            else if (pieces.Length == 1)
            {
                return FromWindowsNamedColor(pieces[0]);
            }
            return Color.FromArgb(a, r, g, b);
        }

        private static Color FromWindowsNamedColor(string color)
        {
            switch (color.ToLowerCase())
            {
                case "activeborder":
                    return Color.FromArgb(255, 180, 180, 180);
                case "activecaption":
                    return Color.FromArgb(255, 153, 180, 209);
                case "activecaptiontext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "appworkspace":
                    return Color.FromArgb(255, 171, 171, 171);
                case "control":
                    return Color.FromArgb(255, 240, 240, 240);
                case "controldark":
                    return Color.FromArgb(255, 160, 160, 160);
                case "controldarkdark":
                    return Color.FromArgb(255, 105, 105, 105);
                case "controllight":
                    return Color.FromArgb(255, 227, 227, 227);
                case "controllightlight":
                    return Color.FromArgb(255, 255, 255, 255);
                case "controltext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "desktop":
                    return Color.FromArgb(255, 255, 255, 255);
                case "graytext":
                    return Color.FromArgb(255, 109, 109, 109);
                case "highlight":
                    return Color.FromArgb(255, 51, 153, 255);
                case "highlighttext":
                    return Color.FromArgb(255, 255, 255, 255);
                case "hottrack":
                    return Color.FromArgb(255, 0, 102, 204);
                case "inactiveborder":
                    return Color.FromArgb(255, 244, 247, 252);
                case "inactivecaption":
                    return Color.FromArgb(255, 191, 205, 219);
                case "inactivecaptiontext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "info":
                    return Color.FromArgb(255, 255, 255, 225);
                case "infotext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "menu":
                    return Color.FromArgb(255, 240, 240, 240);
                case "menutext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "scrollbar":
                    return Color.FromArgb(255, 200, 200, 200);
                case "window":
                    return Color.FromArgb(255, 255, 255, 255);
                case "windowframe":
                    return Color.FromArgb(255, 100, 100, 100);
                case "windowtext":
                    return Color.FromArgb(255, 0, 0, 0);
                case "transparent":
                    return Color.FromArgb(0, 255, 255, 255);
                case "aliceblue":
                    return Color.FromArgb(255, 240, 248, 255);
                case "antiquewhite":
                    return Color.FromArgb(255, 250, 235, 215);
                case "aqua":
                    return Color.FromArgb(255, 0, 255, 255);
                case "aquamarine":
                    return Color.FromArgb(255, 127, 255, 212);
                case "azure":
                    return Color.FromArgb(255, 240, 255, 255);
                case "beige":
                    return Color.FromArgb(255, 245, 245, 220);
                case "bisque":
                    return Color.FromArgb(255, 255, 228, 196);
                case "black":
                    return Color.FromArgb(255, 0, 0, 0);
                case "blanchedalmond":
                    return Color.FromArgb(255, 255, 235, 205);
                case "blue":
                    return Color.FromArgb(255, 0, 0, 255);
                case "blueviolet":
                    return Color.FromArgb(255, 138, 43, 226);
                case "brown":
                    return Color.FromArgb(255, 165, 42, 42);
                case "burlywood":
                    return Color.FromArgb(255, 222, 184, 135);
                case "cadetblue":
                    return Color.FromArgb(255, 95, 158, 160);
                case "chartreuse":
                    return Color.FromArgb(255, 127, 255, 0);
                case "chocolate":
                    return Color.FromArgb(255, 210, 105, 30);
                case "coral":
                    return Color.FromArgb(255, 255, 127, 80);
                case "cornflowerblue":
                    return Color.FromArgb(255, 100, 149, 237);
                case "cornsilk":
                    return Color.FromArgb(255, 255, 248, 220);
                case "crimson":
                    return Color.FromArgb(255, 220, 20, 60);
                case "cyan":
                    return Color.FromArgb(255, 0, 255, 255);
                case "darkblue":
                    return Color.FromArgb(255, 0, 0, 139);
                case "darkcyan":
                    return Color.FromArgb(255, 0, 139, 139);
                case "darkgoldenrod":
                    return Color.FromArgb(255, 184, 134, 11);
                case "darkgray":
                    return Color.FromArgb(255, 169, 169, 169);
                case "darkgreen":
                    return Color.FromArgb(255, 0, 100, 0);
                case "darkkhaki":
                    return Color.FromArgb(255, 189, 183, 107);
                case "darkmagenta":
                    return Color.FromArgb(255, 139, 0, 139);
                case "darkolivegreen":
                    return Color.FromArgb(255, 85, 107, 47);
                case "darkorange":
                    return Color.FromArgb(255, 255, 140, 0);
                case "darkorchid":
                    return Color.FromArgb(255, 153, 50, 204);
                case "darkred":
                    return Color.FromArgb(255, 139, 0, 0);
                case "darksalmon":
                    return Color.FromArgb(255, 233, 150, 122);
                case "darkseagreen":
                    return Color.FromArgb(255, 143, 188, 139);
                case "darkslateblue":
                    return Color.FromArgb(255, 72, 61, 139);
                case "darkslategray":
                    return Color.FromArgb(255, 47, 79, 79);
                case "darkturquoise":
                    return Color.FromArgb(255, 0, 206, 209);
                case "darkviolet":
                    return Color.FromArgb(255, 148, 0, 211);
                case "deeppink":
                    return Color.FromArgb(255, 255, 20, 147);
                case "deepskyblue":
                    return Color.FromArgb(255, 0, 191, 255);
                case "dimgray":
                    return Color.FromArgb(255, 105, 105, 105);
                case "dodgerblue":
                    return Color.FromArgb(255, 30, 144, 255);
                case "firebrick":
                    return Color.FromArgb(255, 178, 34, 34);
                case "floralwhite":
                    return Color.FromArgb(255, 255, 250, 240);
                case "forestgreen":
                    return Color.FromArgb(255, 34, 139, 34);
                case "fuchsia":
                    return Color.FromArgb(255, 255, 0, 255);
                case "gainsboro":
                    return Color.FromArgb(255, 220, 220, 220);
                case "ghostwhite":
                    return Color.FromArgb(255, 248, 248, 255);
                case "gold":
                    return Color.FromArgb(255, 255, 215, 0);
                case "goldenrod":
                    return Color.FromArgb(255, 218, 165, 32);
                case "gray":
                    return Color.FromArgb(255, 128, 128, 128);
                case "green":
                    return Color.FromArgb(255, 0, 128, 0);
                case "greenyellow":
                    return Color.FromArgb(255, 173, 255, 47);
                case "honeydew":
                    return Color.FromArgb(255, 240, 255, 240);
                case "hotpink":
                    return Color.FromArgb(255, 255, 105, 180);
                case "indianred":
                    return Color.FromArgb(255, 205, 92, 92);
                case "indigo":
                    return Color.FromArgb(255, 75, 0, 130);
                case "ivory":
                    return Color.FromArgb(255, 255, 255, 240);
                case "khaki":
                    return Color.FromArgb(255, 240, 230, 140);
                case "lavender":
                    return Color.FromArgb(255, 230, 230, 250);
                case "lavenderblush":
                    return Color.FromArgb(255, 255, 240, 245);
                case "lawngreen":
                    return Color.FromArgb(255, 124, 252, 0);
                case "lemonchiffon":
                    return Color.FromArgb(255, 255, 250, 205);
                case "lightblue":
                    return Color.FromArgb(255, 173, 216, 230);
                case "lightcoral":
                    return Color.FromArgb(255, 240, 128, 128);
                case "lightcyan":
                    return Color.FromArgb(255, 224, 255, 255);
                case "lightgoldenrodyellow":
                    return Color.FromArgb(255, 250, 250, 210);
                case "lightgray":
                    return Color.FromArgb(255, 211, 211, 211);
                case "lightgreen":
                    return Color.FromArgb(255, 144, 238, 144);
                case "lightpink":
                    return Color.FromArgb(255, 255, 182, 193);
                case "lightsalmon":
                    return Color.FromArgb(255, 255, 160, 122);
                case "lightseagreen":
                    return Color.FromArgb(255, 32, 178, 170);
                case "lightskyblue":
                    return Color.FromArgb(255, 135, 206, 250);
                case "lightslategray":
                    return Color.FromArgb(255, 119, 136, 153);
                case "lightsteelblue":
                    return Color.FromArgb(255, 176, 196, 222);
                case "lightyellow":
                    return Color.FromArgb(255, 255, 255, 224);
                case "lime":
                    return Color.FromArgb(255, 0, 255, 0);
                case "limegreen":
                    return Color.FromArgb(255, 50, 205, 50);
                case "linen":
                    return Color.FromArgb(255, 250, 240, 230);
                case "magenta":
                    return Color.FromArgb(255, 255, 0, 255);
                case "maroon":
                    return Color.FromArgb(255, 128, 0, 0);
                case "mediumaquamarine":
                    return Color.FromArgb(255, 102, 205, 170);
                case "mediumblue":
                    return Color.FromArgb(255, 0, 0, 205);
                case "mediumorchid":
                    return Color.FromArgb(255, 186, 85, 211);
                case "mediumpurple":
                    return Color.FromArgb(255, 147, 112, 219);
                case "mediumseagreen":
                    return Color.FromArgb(255, 60, 179, 113);
                case "mediumslateblue":
                    return Color.FromArgb(255, 123, 104, 238);
                case "mediumspringgreen":
                    return Color.FromArgb(255, 0, 250, 154);
                case "mediumturquoise":
                    return Color.FromArgb(255, 72, 209, 204);
                case "mediumvioletred":
                    return Color.FromArgb(255, 199, 21, 133);
                case "midnightblue":
                    return Color.FromArgb(255, 25, 25, 112);
                case "mintcream":
                    return Color.FromArgb(255, 245, 255, 250);
                case "mistyrose":
                    return Color.FromArgb(255, 255, 228, 225);
                case "moccasin":
                    return Color.FromArgb(255, 255, 228, 181);
                case "navajowhite":
                    return Color.FromArgb(255, 255, 222, 173);
                case "navy":
                    return Color.FromArgb(255, 0, 0, 128);
                case "oldlace":
                    return Color.FromArgb(255, 253, 245, 230);
                case "olive":
                    return Color.FromArgb(255, 128, 128, 0);
                case "olivedrab":
                    return Color.FromArgb(255, 107, 142, 35);
                case "orange":
                    return Color.FromArgb(255, 255, 165, 0);
                case "orangered":
                    return Color.FromArgb(255, 255, 69, 0);
                case "orchid":
                    return Color.FromArgb(255, 218, 112, 214);
                case "palegoldenrod":
                    return Color.FromArgb(255, 238, 232, 170);
                case "palegreen":
                    return Color.FromArgb(255, 152, 251, 152);
                case "paleturquoise":
                    return Color.FromArgb(255, 175, 238, 238);
                case "palevioletred":
                    return Color.FromArgb(255, 219, 112, 147);
                case "papayawhip":
                    return Color.FromArgb(255, 255, 239, 213);
                case "peachpuff":
                    return Color.FromArgb(255, 255, 218, 185);
                case "peru":
                    return Color.FromArgb(255, 205, 133, 63);
                case "pink":
                    return Color.FromArgb(255, 255, 192, 203);
                case "plum":
                    return Color.FromArgb(255, 221, 160, 221);
                case "powderblue":
                    return Color.FromArgb(255, 176, 224, 230);
                case "purple":
                    return Color.FromArgb(255, 128, 0, 128);
                case "red":
                    return Color.FromArgb(255, 255, 0, 0);
                case "rosybrown":
                    return Color.FromArgb(255, 188, 143, 143);
                case "royalblue":
                    return Color.FromArgb(255, 65, 105, 225);
                case "saddlebrown":
                    return Color.FromArgb(255, 139, 69, 19);
                case "salmon":
                    return Color.FromArgb(255, 250, 128, 114);
                case "sandybrown":
                    return Color.FromArgb(255, 244, 164, 96);
                case "seagreen":
                    return Color.FromArgb(255, 46, 139, 87);
                case "seashell":
                    return Color.FromArgb(255, 255, 245, 238);
                case "sienna":
                    return Color.FromArgb(255, 160, 82, 45);
                case "silver":
                    return Color.FromArgb(255, 192, 192, 192);
                case "skyblue":
                    return Color.FromArgb(255, 135, 206, 235);
                case "slateblue":
                    return Color.FromArgb(255, 106, 90, 205);
                case "slategray":
                    return Color.FromArgb(255, 112, 128, 144);
                case "snow":
                    return Color.FromArgb(255, 255, 250, 250);
                case "springgreen":
                    return Color.FromArgb(255, 0, 255, 127);
                case "steelblue":
                    return Color.FromArgb(255, 70, 130, 180);
                case "tan":
                    return Color.FromArgb(255, 210, 180, 140);
                case "teal":
                    return Color.FromArgb(255, 0, 128, 128);
                case "thistle":
                    return Color.FromArgb(255, 216, 191, 216);
                case "tomato":
                    return Color.FromArgb(255, 255, 99, 71);
                case "turquoise":
                    return Color.FromArgb(255, 64, 224, 208);
                case "violet":
                    return Color.FromArgb(255, 238, 130, 238);
                case "wheat":
                    return Color.FromArgb(255, 245, 222, 179);
                case "white":
                    return Color.FromArgb(255, 255, 255, 255);
                case "whitesmoke":
                    return Color.FromArgb(255, 245, 245, 245);
                case "yellow":
                    return Color.FromArgb(255, 255, 255, 0);
                case "yellowgreen":
                    return Color.FromArgb(255, 154, 205, 50);
                case "buttonface":
                    return Color.FromArgb(255, 240, 240, 240);
                case "buttonhighlight":
                    return Color.FromArgb(255, 255, 255, 255);
                case "buttonshadow":
                    return Color.FromArgb(255, 160, 160, 160);
                case "gradientactivecaption":
                    return Color.FromArgb(255, 185, 209, 234);
                case "gradientinactivecaption":
                    return Color.FromArgb(255, 215, 228, 242);
                case "menubar":
                    return Color.FromArgb(255, 240, 240, 240);
                case "menuhighlight":
                    return Color.FromArgb(255, 51, 153, 255);
            }
            return Color.FromArgb(255, 255, 255, 255);
        }

     

        public static Color FromHex(string data)
        {
            int r = Util.FromHex(data.Substr(1, 2));
            int g = Util.FromHex(data.Substr(3, 2));
            int b = Util.FromHex(data.Substr(5, 2));
           // int b = Util.FromHex(data.Substr(7, 2));
            int a = 255;
            return Color.FromArgb(a, r, g, b);
        }

        public static Color FromSimpleHex(string data)
        {
            int a = Util.FromHex(data.Substr(0, 2)); 
            int r = Util.FromHex(data.Substr(2, 2));
            int g = Util.FromHex(data.Substr(4, 2));
            int b = Util.FromHex(data.Substr(6, 2));
            return Color.FromArgb(a, r, g, b);
        }

        public static Color FromInt(UInt32 color)
        {
            UInt32 r = (color & 0xFF000000) >> 24;
            UInt32 g = (color & 0x00FF0000) >> 16;
            UInt32 b = (color & 0x0000FF00) >> 8;
            UInt32 a = (color & 0x00000FF);
            return Color.FromArgb(a, r, g, b);
        }



        internal Color Clone()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }

    // Summary:
    //     Implements a set of predefined colors.
    public sealed class Colors
    {
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF000000.
        //
        // Returns:
        //     A System.Windows.Media.Color that has an ARGB value of #FF000000.
        public static Color Black { get { return Color.FromArgb(255, 0, 0, 0); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF0000FF.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FF0000FF.
        public static Color Blue { get { return Color.FromArgb(255, 0, 0, 255); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFA52A2A.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFA52A2A.
        public static Color Brown { get { return Color.FromArgb(255, 165, 42, 42); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF00FFFF.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FF00FFFF.
        public static Color Cyan { get { return Color.FromArgb(255, 0, 255, 255); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFA9A9A9.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFA9A9A9.
        public static Color DarkGray { get { return Color.FromArgb(255, 169, 169, 169); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF808080.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FF808080.
        public static Color Gray { get { return Color.FromArgb(255, 128, 128, 128); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF008000.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FF008000.
        public static Color Green { get { return Color.FromArgb(255, 0, 255, 0); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFD3D3D3.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFD3D3D3.
        public static Color LightGray { get { return Color.FromArgb(255, 211, 211, 211); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFFF00FF.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFFF00FF.
        public static Color Magenta { get { return Color.FromArgb(255, 255, 0, 255); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFFFA500.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFFFA500.
        public static Color Orange { get { return Color.FromArgb(255, 255, 165, 0); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FF800080.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FF800080.
        public static Color Purple { get { return Color.FromArgb(255, 128, 0, 128); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFFF0000.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFFF0000.
        public static Color Red { get { return Color.FromArgb(255, 255, 0, 0); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #00FFFFFF.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #00FFFFFF.
        public static Color Transparent { get { return Color.FromArgb(0, 255, 255, 255); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFFFFFFF.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFFFFFFF.
        public static Color White { get { return Color.FromArgb(255, 255, 255, 255); } }
        //
        // Summary:
        //     Gets the system-defined color that has the ARGB value of #FFFFFF00.
        //
        // Returns:
        //     A System.Windows.Media.Color that has the ARGB value of #FFFFFF00.
        public static Color Yellow { get { return Color.FromArgb(255, 255, 255, 0); } }
    }
}
