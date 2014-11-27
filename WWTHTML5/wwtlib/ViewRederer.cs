using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;

namespace wwtlib
{
    public class ViewRenderer
    {
        public static ViewRenderer Singleton;

        public RenderContext RenderContext;
        public CanvasElement Canvas;
        public ViewRenderer()
        {
        }
        
        static ViewRenderer()
        {
            Singleton = new ViewRenderer();
            Singleton.RenderContext = new RenderContext();
        }


        public void Render()
        {
            RenderTriangle.Width = RenderContext.Width = Canvas.Width;
            RenderTriangle.Height = RenderContext.Height = Canvas.Height;
            Tile.TilesInView = 0;
            Tile.TilesTouched = 0;
            RenderContext.Device.Save();
            RenderContext.Device.FillStyle = "black";
            RenderContext.Device.FillRect(0, 0, RenderContext.Width, RenderContext.Height);
            //RenderContext.Device.ClearRect(0, 0, RenderContext.Width, RenderContext.Height);
            RenderContext.Device.Restore();
            RenderContext.SetupMatricesSpace3d(RenderContext.Width, RenderContext.Height);
            RenderContext.DrawImageSet(RenderContext.BackgroundImageSet, 1);

            int tilesInView = Tile.TilesInView;
            int itlesTouched = Tile.TilesTouched;
        }

        public void Move(double x, double y)
        {

            RenderContext.ViewCamera.Lng += x * RenderContext.FovScale/6360.0;
            RenderContext.ViewCamera.Lat += y * RenderContext.FovScale/6360.0;
            if (RenderContext.ViewCamera.Lat > 90)
            {
                RenderContext.ViewCamera.Lat = 90;
            }

            if (RenderContext.ViewCamera.Lat < -90)
            {
                RenderContext.ViewCamera.Lat = -90;
            }
        }

        public void Zoom(double factor)
        {

            RenderContext.ViewCamera.Zoom *= factor;

            if (RenderContext.ViewCamera.Zoom > 360)
            {
                RenderContext.ViewCamera.Zoom = 360;
            }

        }

        public void Setup()
        {
            CanvasElement canvas = (CanvasElement)Document.GetElementById("canvas");
            
            canvas.AddEventListener("mousemove", OnMouseMove, false);
            canvas.AddEventListener("mouseup", OnMouseUp, false);
            canvas.AddEventListener("mousedown", OnMouseDown, false);
            canvas.AddEventListener("mousewheel", OnMouseWheel, false);
            Document.Body.AddEventListener("touchstart", OnTouchStart, false);
            Document.Body.AddEventListener("touchmove", OnTouchMove, false);
            Document.Body.AddEventListener("touchend", OnTouchEnd, false);
            Document.Body.AddEventListener("gesturechange", OnGestureChange, false);
            canvas.AddEventListener("mouseout", OnMouseUp, false);

        }

        public void OnGestureChange(ElementEvent e)
        {
            GestureEvent g = (GestureEvent) e;
            mouseDown = false;
            double delta = g.Scale;

            if (delta > 1 && Math.Abs(delta - 1) > .05)
            {
                Zoom(0.95);
            }
            else
            {
                Zoom(1.05);
            }
        }


        public void OnTouchStart(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            mouseDown = true;
            lastX = ev.TargetTouches[0].PageX;
            lastY = ev.TargetTouches[0].PageY;
        }

        public void OnTouchMove(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            if (mouseDown)
            {
                double curX = ev.TargetTouches[0].PageX - lastX;

                double curY = ev.TargetTouches[0].PageY - lastY;

                Move(curX, curY);

                lastX = ev.TargetTouches[0].PageX;
                lastY = ev.TargetTouches[0].PageY;
            }
        }
  
        public void OnTouchEnd(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            mouseDown = false;
        }

        bool mouseDown = false;
        double lastX;
        double lastY;
        public void OnMouseDown(ElementEvent e)
        {
            mouseDown = true;
            lastX = e.OffsetX;
            lastY = e.OffsetY;
        }

        public void OnMouseMove(ElementEvent e)
        {
            if (mouseDown)
            {
                Move(e.OffsetX - lastX, e.OffsetY - lastY);

                lastX = e.OffsetX;
                lastY = e.OffsetY;
            }
        }

        public void OnMouseUp(ElementEvent e)
        {
            mouseDown = false;
         
            
        }

        public void OnMouseWheel(ElementEvent e)
        {
            WheelEvent ev = (WheelEvent)(object)e;
            //firefox
            //    double delta = event.detail ? event.detail * (-120) : event.wheelDelta;
            double delta = ev.WheelDelta;

            if (delta > 0)
            {
                Zoom(0.9);
            }
            else
            {
                Zoom(1.1);
            }

        }

    }

}
