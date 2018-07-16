using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{

    public class Annotation
    {
        // Web GL support for annotations
        // Annotations all share a set of supporting primitives, each time any annotation changes the primitives, they must be regenerated if they have been drawn already.
        // It is best to do updates in large batches

        protected static PointList PointList = null;
        protected static LineList LineList = null;
        protected static TriangleList TriangleList = null;

        public static bool BatchDirty = true;
        public static void PrepBatch(RenderContext renderContext)
        {
            if (PointList == null || BatchDirty)
            {
                PointList = new PointList(renderContext);
                LineList = new LineList();
                TriangleList = new TriangleList();
                LineList.DepthBuffered = false;
                TriangleList.DepthBuffered = false;
            }
        }

        public static void DrawBatch(RenderContext renderContext )
        {
            BatchDirty = false;
            if (renderContext.gl == null)
            {
                return;
            }

            if (PointList != null)
            {
                PointList.Draw(renderContext, 1, false);
            }

            if (LineList != null)
            {
                LineList.DrawLines(renderContext, 1);
            }

            if (TriangleList != null)
            {
                TriangleList.Draw(renderContext, 1, CullMode.None);
            }
        }

        protected bool AddedToPrimitives = false;

        protected bool AnnotationDirty = true;

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
            set
            {
                Annotation.BatchDirty = true;
                fill = value;
            }
        }


        bool skyRelative = false;

        
        public bool SkyRelative
        {
            get { return skyRelative; }
            set
            {
                Annotation.BatchDirty = true;
                skyRelative = value;
            }
        }
        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }

            set
            {
                Annotation.BatchDirty = true;
                strokeWidth = value;
            }
        }

        double radius = 10;
        
        public double Radius
        {
            get { return radius; }
            set
            {
                Annotation.BatchDirty = true;
                radius = value;
            }
        }


        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                Annotation.BatchDirty = true;
                lineColor = Color.FromName(value);
            }
        }
        Color fillColor = Colors.White;

        public string FillColor
        {
            get { return fillColor.ToString(); }
            set
            {
                Annotation.BatchDirty = true;
                fillColor = Color.FromName(value);
            }
        }



        double ra = 0;
        double dec = 0;
        
        public void SetCenter(double ra, double dec)
        {
            Annotation.BatchDirty = true;
            this.ra = ra / 15;
            this.dec = dec;
            center = Coordinates.RADecTo3d(this.ra, this.dec);
        }


        public override void Draw(RenderContext renderContext)
        {
            bool onScreen = true;
            double rad = radius;
            if (skyRelative)
            {
                rad /= renderContext.FovScale / 3600;
            }
            Vector3d screenSpacePnt = renderContext.WVP.Transform(center);
            if (screenSpacePnt.Z < 0)
            {
                onScreen = false;
            }

            if (Vector3d.Dot((Vector3d)renderContext.ViewPoint, center) < .55)
            {
                onScreen = false;
            }

            if (renderContext.gl != null)
            {

                if (Annotation.BatchDirty || AnnotationDirty)
                {
                    Vector3d up = Vector3d.Create(0, 1, 0);

                    Vector3d xNormal = Vector3d.Cross(center, up);

                    Vector3d yNormal = Vector3d.Cross(center, xNormal);

                    double r = radius / 44;

                    int segments = 72;

                    double radiansPerSegment = Math.PI * 2 / segments;
                    List<Vector3d> vertexList = new List<Vector3d>();

                    for (int j = 0; j <= segments; j++)
                    {
                        double x = Math.Cos(j * radiansPerSegment) * r;
                        double y = Math.Sin(j * radiansPerSegment) * r;

                        vertexList.Add(Vector3d.Create(center.X + x * xNormal.X + y * yNormal.X, center.Y + x * xNormal.Y + y * yNormal.Y, center.Z + x * xNormal.Z + y * yNormal.Z));

                    }

                    if (strokeWidth > 0 && vertexList.Count > 1)
                    {
                        for (int i = 0; i < (vertexList.Count - 1); i++)
                        {
                            LineList.AddLine(vertexList[i], vertexList[i + 1], lineColor, new Dates(0, 1));
                        }
                        LineList.AddLine(vertexList[vertexList.Count - 1], vertexList[0], lineColor, new Dates(0, 1));
                    }
                    if (fill)
                    {
                        List<int> indexes = Tessellator.TesselateSimplePoly(vertexList);

                        for (int i = 0; i < indexes.Count; i += 3)
                        {
                            TriangleList.AddSubdividedTriangles(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], fillColor, new Dates(0, 1), 2);
                        }
                    }
                    AnnotationDirty = false;
                }
            }
            else
            {
                if (onScreen)
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
            Annotation.BatchDirty = true;
            points.Add(Coordinates.RADecTo3d(x / 15, y));
        }

        bool fill = false;
        
        public bool Fill
        {
            get { return fill; }
            set
            {
                Annotation.BatchDirty = true;
                fill = value;
            }
        }
        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }
            set
            {
                Annotation.BatchDirty = true;
                strokeWidth = value;
            }
        }
        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                Annotation.BatchDirty = true;
                lineColor = Color.FromName(value);
            }
        }
        Color fillColor = Colors.White;

        public string FillColor
        {
            get { return fillColor.ToString(); }
            set
            {
                Annotation.BatchDirty = true;
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
                if (Annotation.BatchDirty || AnnotationDirty)
                {
                    //todo can we save this work for later?
                    List<Vector3d> vertexList = points;


                    if (strokeWidth > 0 && points.Count > 1)
                    {
                        for (int i = 0; i < (points.Count - 1); i++)
                        {
                            LineList.AddLine(vertexList[i], vertexList[i + 1], lineColor, new Dates(0, 1));
                        }
                        LineList.AddLine(vertexList[points.Count - 1], vertexList[0], lineColor, new Dates(0, 1));
                    }
                    if (fill)
                    {
                        List<int> indexes = Tessellator.TesselateSimplePoly(vertexList);

                        for (int i = 0; i < indexes.Count; i += 3)
                        {
                            TriangleList.AddSubdividedTriangles(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], fillColor, new Dates(0, 1), 2);
                        }
                    }
                    AnnotationDirty = false;
                }
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
            Annotation.BatchDirty = true;
            points.Add(Coordinates.RADecTo3d(x / 15, y));
        }

        double strokeWidth = 1;
        
        public double LineWidth
        {
            get { return strokeWidth; }
            set
            {
                Annotation.BatchDirty = true;
                strokeWidth = value;
            }
        }
        Color lineColor = Colors.White;

        public string LineColor
        {
            get { return lineColor.ToString(); }
            set
            {
                Annotation.BatchDirty = true;
                lineColor = Color.FromName(value);
            }
        }
        


        public override void Draw(RenderContext renderContext)
        {
            if (renderContext.gl != null)
            {
                if (Annotation.BatchDirty || AnnotationDirty)
                {
                    //todo can we save this work for later?
                    List<Vector3d> vertexList = points;


                    if (strokeWidth > 0)
                    {
                        for (int i = 0; i < (points.Count - 1); i++)
                        {
                            LineList.AddLine(vertexList[i], vertexList[i + 1], lineColor, new Dates(0, 1));
                        }
                    }
                   
                    AnnotationDirty = false;
                }
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
