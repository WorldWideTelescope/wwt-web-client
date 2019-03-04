using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class GridLayer : Layer
    {
        public GridLayer()
        {

        }

        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {

            Grids.DrawPlanetGrid(renderContext, opacity * Opacity, Color);
            Grids.DrawPlanetGridText(renderContext, opacity * Opacity, Color);
            return true;
        }
    }
}
