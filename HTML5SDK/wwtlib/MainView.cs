// MainView.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using System.Html.Media.Graphics;
using System.Net;
using System.Serialization;
namespace wwtlib
{
    
    internal static class MainView
    {
        

        static MainView()
        {
            CanvasElement canvas = (CanvasElement) Document.GetElementById("canvas");

            //Element body = Document.GetElementsByTagName("body")[0];

            //body.AddEventListener("load", delegate(ElementEvent e) { WWTControl.InitControl(); }, false);

            //XmlHttpRequest xhr = new XmlHttpRequest();
            //xhr.Open(HttpVerb.Get, "quakedata.txt?3");
            //xhr.OnReadyStateChange = delegate()
            //{
            //    if (xhr.ReadyState == ReadyState.Loaded)
            //    {
            //        VizLayer layer = new VizLayer();
            //        layer.Load(xhr.ResponseText);
            //        layer.Prepare();
            //        WWTControl.Singleton.Layers.Add(layer);
            //    }
            //};
            //xhr.Send();

            //Wtml.GetWtmlFile("//worldwidetelescope.org/wwtweb/catalog.aspx?W=ExploreRoot");
            //Wtml.GetWtmlFile("imagesets.xml");

        }

        static void DrawTest()
        {
            CanvasElement canvas = (CanvasElement) Document.GetElementById("canvas");

            CanvasContext2D ctx = (CanvasContext2D) canvas.GetContext(Rendering.Render2D);

            ctx.FillStyle = "rgb(80,0,0)";
            ctx.FillRect(120, 120, 165, 160);

            ctx.FillStyle = "rgba(0, 0, 160, 0.5)";
            ctx.FillRect(140, 140, 165, 160);

        }

       



       
    }
}
