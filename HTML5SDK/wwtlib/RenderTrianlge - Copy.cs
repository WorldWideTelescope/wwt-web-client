using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class RenderTriangle
    {
        PositionTexture A = new PositionTexture();
        PositionTexture B = new PositionTexture();
        PositionTexture C = new PositionTexture();
        ImageElement texture;
        public double Opacity = 1;
        public static double Width = 1024;
        public static double Height = 768;

        private const double ExpansionInPixels = .6;
        private const double ContractionInPixels = -0.5;

        private const bool isOutlined = false;
        private const double halfPixel = 0.001953125;
        private const double almostOne = 0.998;

        public static int TrianglesRendered = 0;
        public static int TrianglesCulled = 0;
        public static bool RenderingOn = true;
        public int TileLevel = 0;
        public static RenderTriangle Create(PositionTexture a, PositionTexture b, PositionTexture c, ImageElement img, int level)
        {
            RenderTriangle temp = new RenderTriangle();

            temp.A = a.Copy();
            temp.B = b.Copy();
            temp.C = c.Copy();
            temp.texture = img;
            temp.TileLevel = level;
            return temp;

        }
        double factor = 1;

        public static bool CullInside = true;
        public void Draw(CanvasContext2D ctx, Matrix3d wvp)
        {
            
            Vector3d a = wvp.Transform(A.Position);
            Vector3d b = wvp.Transform(B.Position);
            Vector3d c = wvp.Transform(C.Position);

            if (CheckBackface(a, b, c) != CullInside)
            {
                TrianglesCulled++;
                return;
            }
            
            TrianglesRendered++;

            bool rendered;
            if (factor == 1.0)
            {
                rendered = DrawTriangle(ctx, texture, (a.X + .5) * Width, (-a.Y + .5) * Height, (b.X + .5) * Width, (-b.Y + .5) * Height, (c.X + .5) * Width, (-c.Y + .5) * Height, A.Tu * 256.0, A.Tv * 256.0, B.Tu * 256.1, B.Tv * 256.0, C.Tu * 256.0, C.Tv * 256.0);
            }
            else
            {
                rendered = DrawTriangle(ctx, texture, (a.X * factor + .5) * Width, (-a.Y * factor + .5) * Height, (b.X * factor + .5) * Width, (-b.Y * factor + .5) * Height, (c.X * factor + .5) * Width, (-c.Y * factor + .5) * Height, A.Tu * 255, A.Tv * 255, B.Tu * 255, B.Tv * 255, C.Tu * 255, C.Tv * 255);
            }
            if (rendered)
            {
                TrianglesRendered++;
            }
            else
            {
                TrianglesCulled++;
            }
        }


        bool CheckBackface(Vector3d a, Vector3d b, Vector3d c)
        {

            Vector3d ab = Vector3d.SubtractVectors(a, b);
            Vector3d ac = Vector3d.SubtractVectors(a, c);
            Vector3d cp = Vector3d.Cross(ab, ac);
            cp.Normalize();


            return cp.Z >= 0;
        }


        static double hw;
        static double qw;
        static double hh;
        static double qh;

        private bool DrawTriangle(CanvasContext2D ctx, ImageElement im, double x0, double y0, double x1, double y1, double x2, double y2,
                                double sx0, double sy0, double sx1, double sy1, double sx2, double sy2)
        {

            bool inside;

            if (factor == 1.0)
            {
                inside = Intersects(0, Width, 0, Height, x0, y0, x1, y1, x2, y2);
            }
            else
            {
                hw = Width / 2;
                qw = hw * factor;
                hh = Height / 2;
                qh = hh * factor;
                inside = Intersects(hw - qw, hw + qw, hh - qh, hh + qh, x0, y0, x1, y1, x2, y2);
            }

            if (!inside)
            {
                return false;
            }

            double edgeOffset = isOutlined ? ContractionInPixels : ExpansionInPixels;
            Vector2d expandedS0 = GetMiterPoint(Vector2d.Create(x0, y0), Vector2d.Create(x1, y1), Vector2d.Create(x2, y2), edgeOffset);
            Vector2d expandedS1 = GetMiterPoint(Vector2d.Create(x1, y1), Vector2d.Create(x0, y0), Vector2d.Create(x2, y2), edgeOffset);
            Vector2d expandedS2 = GetMiterPoint(Vector2d.Create(x2, y2), Vector2d.Create(x1, y1), Vector2d.Create(x0, y0), edgeOffset);

            x0 = expandedS0.X;
            y0 = expandedS0.Y;
            x1 = expandedS1.X;
            y1 = expandedS1.Y;
            x2 = expandedS2.X;
            y2 = expandedS2.Y;


            ctx.Save();
            ctx.BeginPath();
            ctx.MoveTo(x0, y0);
            ctx.LineTo(x1, y1);
            ctx.LineTo(x2, y2);
            ctx.ClosePath();
            ctx.Clip();

            double denom = sx0 * (sy2 - sy1) - sx1 * sy2 + sx2 * sy1 + (sx1 - sx2) * sy0;
            if (denom == 0)
            {
                ctx.Restore();
                return false;
            }
            double m11 = -(sy0 * (x2 - x1) - sy1 * x2 + sy2 * x1 + (sy1 - sy2) * x0) / denom;
            double m12 = (sy1 * y2 + sy0 * (y1 - y2) - sy2 * y1 + (sy2 - sy1) * y0) / denom;
            double m21 = (sx0 * (x2 - x1) - sx1 * x2 + sx2 * x1 + (sx1 - sx2) * x0) / denom;
            double m22 = -(sx1 * y2 + sx0 * (y1 - y2) - sx2 * y1 + (sx2 - sx1) * y0) / denom;
            double dx = (sx0 * (sy2 * x1 - sy1 * x2) + sy0 * (sx1 * x2 - sx2 * x1) + (sx2 * sy1 - sx1 * sy2) * x0) / denom;
            double dy = (sx0 * (sy2 * y1 - sy1 * y2) + sy0 * (sx1 * y2 - sx2 * y1) + (sx2 * sy1 - sx1 * sy2) * y0) / denom;


            ctx.Transform(m11, m12, m21, m22, dx, dy);

            ctx.DrawImage(im, 0, 0);

            ctx.Restore();

            if (factor != 1.0)
            {
                ctx.BeginPath();
                ctx.MoveTo(hw - qw, hh - qh);
                ctx.LineTo(hw + qw, hh - qh);
                ctx.LineTo(hw + qw, hh + qh);
                ctx.LineTo(hw - qw, hh + qh);
                ctx.ClosePath();
                ctx.StrokeStyle = "yellow";
                ctx.Stroke();

            }

            return true;
        }


        private static Vector2d GetMiterPoint(Vector2d p1, Vector2d p2, Vector2d p3, double edgeOffset)
        {
            Vector2d edge1 = Vector2d.SubtractVector(p2, p1);
            Vector2d edge2 = Vector2d.SubtractVector(p3, p1);
            edge1.Normalize();
            edge2.Normalize();
            Vector2d dir = Vector2d.Create(edge1.X + edge2.X, edge1.Y + edge2.Y);
            dir.Normalize();
            Vector2d delta = Vector2d.Create(edge1.X - edge2.X, edge1.Y - edge2.Y);
            double sineHalfAngle = delta.Length / 2;
            double net = Math.Min(2, edgeOffset / sineHalfAngle);

            dir.Extend(net);

            return Vector2d.Create(p1.X-dir.X,p1.Y-dir.Y);
        }

        public bool Intersects(double l, double r, double t, double b, double x0, double y0, double x1, double y1, double x2, double y2)
        {
            if (x0 > l && x0 < r && y0 > t && y0 < b)
            {
                return true;
            }

            if (x1 > l && x1 < r && y1 > t && y1 < b)
            {
                return true;
            }
            
            if (x2 > l && x2 < r && y2 > t && y2 < b)
            {
                return true;
            }

            if ( TileLevel < 4 && ((Math.Abs(x0 - x1) > Height) || (Math.Abs(y0 - y1) > Height) || (Math.Abs(x2 - x1) > Height) || (Math.Abs(y2 - y1) > Height) || (Math.Abs(x0 - x2) > Height) || (Math.Abs(y0 - y2) > Height)))
            {
                return false;
            }

            return lineRectangleIntersect(l, r, t, b, x0, y0, x1, y1)
                  || lineRectangleIntersect(l, r, t, b, x1, y1, x2, y2)
                  || lineRectangleIntersect(l, r, t, b, x2, y2, x0, y0);



        }

        


        public bool lineRectangleIntersect(double l, double r, double t, double b, double x0, double y0, double x1, double y1)
        {
            double top_intersection;
            double bottom_intersection;
            double toptrianglepoint;
            double bottomtrianglepoint;

            double m;
            double c;
            // Calculate m and c for the equation for the line (y = mx+c)
            m = (y1 - y0) / (x1 - x0);
            c = y0 - (m * x0);

            // if the line is going up from right to left then the top intersect point is on the left
            if (m > 0)
            {
                top_intersection = (m * l + c);
                bottom_intersection = (m * r + c);
            }
            // otherwise it's on the right
            else
            {
                top_intersection = (m * r + c);
                bottom_intersection = (m * l + c);
            }

            // work out the top and bottom extents for the triangle
            if (y0 < y1)
            {
                toptrianglepoint = y0;
                bottomtrianglepoint = y1;
            }
            else
            {
                toptrianglepoint = y1;
                bottomtrianglepoint = y0;
            }

            double topoverlap;
            double botoverlap;

            // and calculate the overlap between those two bounds
            topoverlap = top_intersection > toptrianglepoint ? top_intersection : toptrianglepoint;
            botoverlap = bottom_intersection < bottomtrianglepoint ? bottom_intersection : bottomtrianglepoint;

            // (topoverlap<botoverlap) :
            // if the intersection isn't the right way up then we have no overlap

            // (!((botoverlap<t) || (topoverlap>b)) :
            // If the bottom overlap is higher than the top of the rectangle or the top overlap is
            // lower than the bottom of the rectangle we don't have intersection. So return the negative
            // of that. Much faster than checking each of the points is within the bounds of the rectangle.
            return (topoverlap < botoverlap) && (!((botoverlap < t) || (topoverlap > b)));

        }


    }
}
