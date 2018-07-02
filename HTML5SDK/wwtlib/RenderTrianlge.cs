using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class RenderTriangle
    {
        public PositionTexture A = new PositionTexture();
        public PositionTexture B = new PositionTexture();
        public PositionTexture C = new PositionTexture();

        public Vector3d Normal = new Vector3d();

        public void MakeNormal()
        {
            Vector3d a = A.Position.Copy();
            Vector3d b = B.Position.Copy();
            Vector3d c = C.Position.Copy();

            a.Normalize();
            b.Normalize();
            c.Normalize();


            double x = a.X + b.X + c.X;
            double y = a.Y + b.Y + c.Y;
            double z = a.Z + b.Z + c.Z;

            Normal = Vector3d.Create(x / 3, y / 3, z / 3);
            Normal.Normalize();
        }

        ImageElement texture;
        public double Opacity = 1;
        public static double Width = 1024;
        public static double Height = 768;

        public double ExpansionInPixels = .6;
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
            temp.MakeNormal();
            return temp;

        }

        public static RenderTriangle CreateWithMiter(PositionTexture a, PositionTexture b, PositionTexture c, ImageElement img, int level, double expansion)
        {
            RenderTriangle temp = new RenderTriangle();
            temp.ExpansionInPixels = expansion;
            temp.A = a.Copy();
            temp.B = b.Copy();
            temp.C = c.Copy();
            temp.texture = img;
            temp.TileLevel = level;
            temp.MakeNormal();
            return temp;

        }
      
        static double factor = 1;




        //bool CheckBackface(Vector3d a, Vector3d b, Vector3d c)
       bool CheckBackface()
       {

            Vector3d ab = Vector3d.SubtractVectors(ta, tb);
            Vector3d ac = Vector3d.SubtractVectors(ta, tc);
            Vector3d cp = Vector3d.Cross(ab, ac);
            cp.Normalize();


            return cp.Z >= 0;
        }

        public static bool CullInside = true;

        Vector3d ta = new Vector3d();
        Vector3d tb = new Vector3d();
        Vector3d tc = new Vector3d();


        public void Draw(CanvasContext2D ctx, Matrix3d wvp)
        {
            if (ctx == null)
            {
                return;
            }
            
            wvp.TransformTo(A.Position, ta);
            wvp.TransformTo(B.Position, tb);
            wvp.TransformTo(C.Position, tc);

            if (CheckBackface() == CullInside)
            {
                TrianglesCulled++;
                return;
            }
            
            //TrianglesRendered++;

           

            DrawTriangle(ctx, texture, ta.X, ta.Y, tb.X, tb.Y, tc.X, tc.Y, A.Tu, A.Tv, B.Tu, B.Tv, C.Tu, C.Tv);

            //if (rendered)
            //{
            //    TrianglesRendered++;
            //}
            //else
            //{
            //    TrianglesCulled++;
            //}
        }


        static double hw;
        static double qw;
        static double hh;
        static double qh;

        Vector2d expandedS0 = new Vector2d();
        Vector2d expandedS1 = new Vector2d();
        Vector2d expandedS2 = new Vector2d();

        private bool DrawTriangle(CanvasContext2D ctx, ImageElement im, double x0, double y0, double x1, double y1, double x2, double y2,
                                double sx0, double sy0, double sx1, double sy1, double sx2, double sy2)
        {

            if (!Intersects(0, Width, 0, Height, x0, y0, x1, y1, x2, y2))
            {
                return false;
            }

            //double edgeOffset = isOutlined ? ContractionInPixels : ExpansionInPixels;
            //Vector2d expandedS0 = GetMiterPoint(Vector2d.Create(x0, y0), Vector2d.Create(x1, y1), Vector2d.Create(x2, y2), ExpansionInPixels);
            //Vector2d expandedS1 = GetMiterPoint(Vector2d.Create(x1, y1), Vector2d.Create(x0, y0), Vector2d.Create(x2, y2), ExpansionInPixels);
            //Vector2d expandedS2 = GetMiterPoint(Vector2d.Create(x2, y2), Vector2d.Create(x1, y1), Vector2d.Create(x0, y0), ExpansionInPixels);

            //Vector2d expandedS0 = MiterPoint(x0, y0, x1, y1, x2, y2);
            //Vector2d expandedS1 = MiterPoint(x1, y1, x0, y0, x2, y2);
            //Vector2d expandedS2 = MiterPoint(x2, y2, x1, y1, x0, y0);
            MiterPointOut(expandedS0, x0, y0, x1, y1, x2, y2, ExpansionInPixels);
            MiterPointOut(expandedS1, x1, y1, x0, y0, x2, y2, ExpansionInPixels);
            MiterPointOut(expandedS2, x2, y2, x1, y1, x0, y0, ExpansionInPixels);

            x0 = expandedS0.X;
            y0 = expandedS0.Y;
            x1 = expandedS1.X;
            y1 = expandedS1.Y;
            x2 = expandedS2.X;
            y2 = expandedS2.Y;


            ctx.Save();
            if (RenderingOn)
            {
                
                ctx.BeginPath();
                ctx.MoveTo(x0, y0);
                ctx.LineTo(x1, y1);
                ctx.LineTo(x2, y2);
                ctx.ClosePath();
                ctx.Clip();
            }
            double denom = sx0 * (sy2 - sy1) - sx1 * sy2 + sx2 * sy1 + (sx1 - sx2) * sy0;
            //if (denom == 0)
            //{
            //    ctx.Restore();
            //    return false;
            //}
            double m11 = -(sy0 * (x2 - x1) - sy1 * x2 + sy2 * x1 + (sy1 - sy2) * x0) / denom;
            double m12 = (sy1 * y2 + sy0 * (y1 - y2) - sy2 * y1 + (sy2 - sy1) * y0) / denom;
            double m21 = (sx0 * (x2 - x1) - sx1 * x2 + sx2 * x1 + (sx1 - sx2) * x0) / denom;
            double m22 = -(sx1 * y2 + sx0 * (y1 - y2) - sx2 * y1 + (sx2 - sx1) * y0) / denom;
            double dx = (sx0 * (sy2 * x1 - sy1 * x2) + sy0 * (sx1 * x2 - sx2 * x1) + (sx2 * sy1 - sx1 * sy2) * x0) / denom;
            double dy = (sx0 * (sy2 * y1 - sy1 * y2) + sy0 * (sx1 * y2 - sx2 * y1) + (sx2 * sy1 - sx1 * sy2) * y0) / denom;


            ctx.Transform(m11, m12, m21, m22, dx, dy);

            if (RenderingOn)
            {
                ctx.Alpha = Opacity;

                if (lighting < 1.0)
                {
                    ctx.Alpha = 1;
                    ctx.FillStyle = "Black";
                    ctx.FillRect(0, 0, Width, Height);
                    ctx.Alpha = lighting * Opacity;
                }

                ctx.DrawImage(im, 0, 0);
            }

            
            
            ctx.Restore();
            return true;
        }

        public float lighting = 1;

        private static Vector2d GetMiterPoint(Vector2d p1, Vector2d p2, Vector2d p3, double edgeOffset)
        {
            Vector2d edge1 = Vector2d.Subtract(p2, p1);
            Vector2d edge2 = Vector2d.Subtract(p3, p1);
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

        private static Vector2d MiterPoint(double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double ExpansionInPixels)
        {
            //Vector2d edge1 = Vector2d.SubtractVector(p2, p1);
            double e1x = p2x - p1x;
            double e1y = p2y - p1y;
            
            //Vector2d edge2 = Vector2d.SubtractVector(p3, p1);
            double e2x = p3x - p1x;
            double e2y = p3y - p1y;
           
            //edge1.Normalize();
            double length = Math.Sqrt(e1x * e1x + e1y * e1y);
            if (length != 0)
            {
                e1x /= length;
                e1y /= length;
            }

            //edge2.Normalize();
            length = Math.Sqrt(e2x * e2x + e2y * e2y);
            if (length != 0)
            {
                e2x /= length;
                e2y /= length;
            }
            //Vector2d dir = Vector2d.Create(edge1.X + edge2.X, edge1.Y + edge2.Y);
            double dx = e1x + e2x;
            double dy = e1y + e2y;
           
            //dir.Normalize();
            length = Math.Sqrt(dx * dx + dy * dy);
            if (length != 0)
            {
                dx /= length;
                dy /= length;
            }

            //Vector2d delta = Vector2d.Create(edge1.X - edge2.X, edge1.Y - edge2.Y);
            double deltax = e1x - e2x; 
            double deltay = e1y - e2y;

            //double sineHalfAngle = delta.Length / 2;
            length = Math.Sqrt(deltax * deltax + deltay * deltay);
            double sineHalfAngle = length / 2.0;

            double net = Math.Min(2, ExpansionInPixels / sineHalfAngle);

            //dir.Extend(net);
            dx *= net;
            dy *= net;

            //return Vector2d.Create(p1.X-dir.X,p1.Y-dir.Y);
            return Vector2d.Create(p1x - dx, p1y - dy);
        }

        private static void MiterPointOut(Vector2d pntOut, double p1x, double p1y, double p2x, double p2y, double p3x, double p3y, double ExpansionInPixels)
        {
            //Vector2d edge1 = Vector2d.SubtractVector(p2, p1);
            double e1x = p2x - p1x;
            double e1y = p2y - p1y;

            //Vector2d edge2 = Vector2d.SubtractVector(p3, p1);
            double e2x = p3x - p1x;
            double e2y = p3y - p1y;

            //edge1.Normalize();
            double length = Math.Sqrt(e1x * e1x + e1y * e1y);
            if (length != 0)
            {
                e1x /= length;
                e1y /= length;
            }

            //edge2.Normalize();
            length = Math.Sqrt(e2x * e2x + e2y * e2y);
            if (length != 0)
            {
                e2x /= length;
                e2y /= length;
            }
            //Vector2d dir = Vector2d.Create(edge1.X + edge2.X, edge1.Y + edge2.Y);
            double dx = e1x + e2x;
            double dy = e1y + e2y;

            //dir.Normalize();
            length = Math.Sqrt(dx * dx + dy * dy);
            if (length != 0)
            {
                dx /= length;
                dy /= length;
            }

            //Vector2d delta = Vector2d.Create(edge1.X - edge2.X, edge1.Y - edge2.Y);
            double deltax = e1x - e2x;
            double deltay = e1y - e2y;

            //double sineHalfAngle = delta.Length / 2;
            length = Math.Sqrt(deltax * deltax + deltay * deltay);
            double sineHalfAngle = length / 2.0;

            double net = Math.Min(2, ExpansionInPixels / sineHalfAngle);

            //dir.Extend(net);
            dx *= net;
            dy *= net;

            //return Vector2d.Create(p1.X-dir.X,p1.Y-dir.Y);
            pntOut.X = p1x - dx;
            pntOut.Y = p1y - dy;
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

            double h4 = Height * 4;
            if (TileLevel < 4 && ((Math.Abs(x0 - x1) > h4) || (Math.Abs(y0 - y1) > h4) || (Math.Abs(x2 - x1) > h4) || (Math.Abs(y2 - y1) > h4) || (Math.Abs(x0 - x2) > h4) || (Math.Abs(y0 - y2) > h4)))
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
