using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{

    public class Annotation
    {
        public virtual void Draw(RenderContext renderContext)
        {

        }


        public static double Separation(double Alpha1, double Delta1, double Alpha2, double Delta2)
        {
            Delta1 = Delta1 / 180.0 * Math.PI;
            Delta2 = Delta2 / 180.0 * Math.PI;

            Alpha1 = Alpha1/ 12.0 * Math.PI;
            Alpha2 = Alpha2/ 12.0 * Math.PI;

            double x = Math.Cos(Delta1) * Math.Sin(Delta2) - Math.Sin(Delta1) * Math.Cos(Delta2) * Math.Cos(Alpha2 - Alpha1);
            double y = Math.Cos(Delta2) * Math.Sin(Alpha2 - Alpha1);
            double z = Math.Sin(Delta1) * Math.Sin(Delta2) + Math.Cos(Delta1) * Math.Cos(Delta2) * Math.Cos(Alpha2 - Alpha1);

            double @value = Math.Atan2(Math.Sqrt(x * x + y * y), z);
            @value = @value/Math.PI * 180.0;
            if (@value < 0)
                @value += 180;

            return @value;
        }

        double opacity = 1;
        
        public double Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }
        string id;
        
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        string tag;
        
        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        string label;
        
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        bool showHoverLabel = false;
        
        public bool ShowHoverLabel
        {
            get { return showHoverLabel; }
            set { showHoverLabel = value; }
        }
        public virtual bool HitTest(RenderContext renderContext, double RA, double dec, double x, double y)
        {
            return false;
        }

        protected Vector3d center;

        public Vector3d Center
        {
            get { return center; }
            set { center = value; }
        }
        public static uint ColorToUint(Color col)
        {
            return ((uint)col.A) << 24 | ((uint)col.R << 16) | ((uint)col.G) << 8 | (uint)col.B;
        }

        public static uint ColorToUintAlpha(Color col, uint opacity)
        {
            return (uint)opacity << 24 | (uint)col.R << 16 | (uint)col.G << 8 | (uint)col.B;
        }
    }

    public class Circle : Annotation
    {
        public Circle()
        {
        }

        bool fill = false;

        
        public bool Fill
        {
            get { return fill; }
            set { fill = value; }
        }


        bool skyRelative = false;

        
        public bool SkyRelative
        {
            get { return skyRelative; }
            set { skyRelative = value; }
        }
        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }
        }

        double radius = 10;
        
        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }


        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                lineColor = Color.FromName(value);
            }
        }
        Color fillColor = Colors.White;

        public string FillColor
        {
            get { return fillColor.ToString(); }
            set
            {
                fillColor = Color.FromName(value);
            }
        }



        double ra = 0;
        double dec = 0;
        
        public void SetCenter(double ra, double dec)
        {

            this.ra = ra / 15;
            this.dec = dec;
            center = Coordinates.RADecTo3d(this.ra, this.dec);
        }


        public override void Draw(RenderContext renderContext)
        {
            double rad = radius;
            if (skyRelative)
            {
                rad /= renderContext.FovScale / 3600;
            }
            Vector3d screenSpacePnt = renderContext.WVP.Transform(center);
            if (screenSpacePnt.Z < 0)
            {
                return;
            }

            if (Vector3d.Dot((Vector3d)renderContext.ViewPoint, (Vector3d)center) < .55)
            {
                return;
            }
            if (renderContext.gl != null)
            {
                //todo draw in WebGL
            }
            else
            {
                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();
                ctx.Alpha = Opacity;
                ctx.BeginPath();
                ctx.Arc(screenSpacePnt.X, screenSpacePnt.Y, rad, 0, Math.PI * 2, true);
                ctx.LineWidth = strokeWidth;
                ctx.FillStyle = fillColor.ToString();
                if (fill)
                {
                    ctx.Fill();
                }
                ctx.Alpha = 1.0;
                ctx.StrokeStyle = lineColor.ToString();
                ctx.Stroke();

                ctx.Restore();
            }
        }

        public override bool HitTest(RenderContext renderContext, double RA, double dec, double x, double y)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return false;
            }

            double rad = radius;
            if (!skyRelative)
            {
                rad *= renderContext.FovScale / 3600;
            }

            return Separation(RA, dec, this.ra, this.dec) < rad;
        }

    }

    public class Poly : Annotation
    {
        List<Vector3d> points = new List<Vector3d>();

        
        public void AddPoint(double x, double y)
        {
            points.Add(Coordinates.RADecTo3d(x / 15, y));
        }

        bool fill = false;
        
        public bool Fill
        {
            get { return fill; }
            set { fill = value; }
        }
        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }
        }
        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                lineColor = Color.FromName(value);
            }
        }
        Color fillColor = Colors.White;

        public string FillColor
        {
            get { return fillColor.ToString(); }
            set
            {
                fillColor = Color.FromName(value);
            }
        }

        //bool hitTestInit = false;

        //void InitHitTest(RenderContext renderContext)
        //{
        //    Vector2d center = new Vector2d();
        //    double radius = 0;
        //    Vector2d[] screenPoints = new Vector2d[points.Count];
        //    int index = 0;
        //    foreach (Vector3d pnt in points)
        //    {
        //        Vector3d screenSpacePnt = renderContext.ViewMatrix.Transform(pnt);
        //        if (screenSpacePnt.Z < 0)
        //        {
        //            return;
        //        }
        //        if (Vector3d.Dot(renderContext.ViewPoint, pnt) < .55)
        //        {
        //            return;
        //        }
        //        screenPoints[index] = new Vector2d(screenSpacePnt.X, screenSpacePnt.Y);
        //        index++;
        //    }

        //    ConvexHull.FindEnclosingCircle(screenPoints, out center, out radius);
        //}

        public override void Draw(RenderContext renderContext)
        {
            if (renderContext.gl != null)
            {
                //todo draw in WebGL
            }
            else
            {
                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();
                ctx.Alpha = Opacity;

                ctx.BeginPath();

                bool first = true;
                foreach (Vector3d pnt in points)
                {
                    Vector3d screenSpacePnt = renderContext.WVP.Transform(pnt);
                    if (screenSpacePnt.Z < 0)
                    {
                        ctx.Restore();

                        return;
                    }

                    if (Vector3d.Dot(renderContext.ViewPoint, pnt) < .75)
                    {
                        ctx.Restore();
                        return;
                    }

                    if (first)
                    {
                        first = false;
                        ctx.MoveTo(screenSpacePnt.X, screenSpacePnt.Y);
                    }
                    else
                    {
                        ctx.LineTo(screenSpacePnt.X, screenSpacePnt.Y);
                    }

                }
                ctx.ClosePath();

                ctx.LineWidth = strokeWidth;
                if (fill)
                {
                    ctx.FillStyle = fillColor.ToString();
                    ctx.Fill();
                }

                ctx.StrokeStyle = lineColor.ToString();
                ctx.Alpha = 1;
                ctx.Stroke();

                ctx.Restore();

            }
        }
    }
    public class PolyLine : Annotation
    {
        List<Vector3d> points = new List<Vector3d>();

        
        public void AddPoint(double x, double y)
        {
            points.Add(Coordinates.RADecTo3d(x / 15, y));
        }

        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }
            set { strokeWidth = value; }
        }
        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                lineColor = Color.FromName(value);
            }
        }
        


        public override void Draw(RenderContext renderContext)
        {
            if (renderContext.gl != null)
            {
                //todo draw in WebGL
            }
            else
            {
                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();
                ctx.Alpha = Opacity;

                bool first = true;
                foreach (Vector3d pnt in points)
                {
                    Vector3d screenSpacePnt = renderContext.WVP.Transform(pnt);
                    if (screenSpacePnt.Z < 0)
                    {
                        ctx.Restore();
                        return;
                    }
                    if (Vector3d.Dot(renderContext.ViewPoint, pnt) < .75)
                    {
                        ctx.Restore();
                        return;
                    }
                    if (first)
                    {
                        first = false;
                        ctx.BeginPath();
                        ctx.MoveTo(screenSpacePnt.X, screenSpacePnt.Y);
                    }
                    else
                    {
                        ctx.LineTo(screenSpacePnt.X, screenSpacePnt.Y);
                    }
                }

                ctx.LineWidth = strokeWidth;

                ctx.StrokeStyle = lineColor.ToString();

                ctx.Stroke();

                ctx.Restore();
            }
        }
    }
}
