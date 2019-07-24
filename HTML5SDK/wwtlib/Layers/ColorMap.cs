using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{

    public class ColorMapContainer
    {

        // This class is intended to be used to store colormaps. It does not handle any
        // interpolation and when using FindClosestColor it will simply check which
        // color is closest to the requested value. Therefore, continuous colormaps should
        // be created by providing a sufficient number of colors (ideally 256 or more).

        public List<Color> colors = new List<Color>();

        public static ColorMapContainer FromNestedLists(List<List<float>> color_list)
        {
              
            // Class method to create a new colormap from a list of [r, g, b, a] lists.

            ColorMapContainer temp = new ColorMapContainer();
            foreach (List<float> color in color_list)
            {
                temp.colors.Add(Color.FromArgb(color[3], color[0], color[1], color[2]));
            }
            return temp;
        }

        public static ColorMapContainer FromStringList(List<string> color_list)
        {

            // Class method to create a new colormap from a list of strings.

            ColorMapContainer temp = new ColorMapContainer();
            foreach (string color in color_list)
            {
                temp.colors.Add(Color.Load(color));
            }
            return temp;
        }

        public static ColorMapContainer Grayscale()
        {

            // Example continuous grayscale colormap

            ColorMapContainer temp = new ColorMapContainer();
            for (int i = 0; i < 256; i++)
            {
                temp.colors.Add(Color.FromArgb(255, i, i, i));
            }
            return temp;
        }

        public Color FindClosestColor(float value)
        {
            // Given a floating-point value in the range 0 to 1, find the color that is the
            // closest to it.

            int index;

            if (value <= 0) {
                return colors[0];
            } else if (value >= 1) {
                return colors[colors.Count - 1];
            } else {
                index = (int)(value * colors.Count);
                return colors[index];
            }

        }

    }

}
