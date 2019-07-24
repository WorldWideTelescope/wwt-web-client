using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{

    class ColorMap
    {
        public List<Color> colors = new List<Color>();
        public static ColorMap FromNestedLists(List<List<float>> color_list)
        {
            ColorMap temp = new ColorMap();
            foreach (var color in color_list)
            {
                temp.colors.Add(Color.FromArgb(color[3], color[0], color[1], color[2]));
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
