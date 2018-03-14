using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;


namespace wwtlib
{

    public class Dates
    {

        public Dates(double start, double end)
        {
            StartDate = start;
            EndDate = end;
        }
        public double StartDate;
        public double EndDate;

        public Dates Copy()
        {
            return new Dates(this.StartDate, this.EndDate);
        }
    }


    public class SimpleLineList
    {
        public SimpleLineList()
        {
        }


        bool zBuffer = true;

        public bool DepthBuffered
        {
            get { return zBuffer; }
            set { zBuffer = value; }
        }

        List<Vector3d> linePoints = new List<Vector3d>();

        public void AddLine(Vector3d v1, Vector3d v2)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            EmptyLineBuffer();

        }

        public void Clear()
        {
            linePoints.Clear();
            EmptyLineBuffer();
        }

        bool usingLocalCenter = false;
        Vector3d localCenter;
        public bool Sky = true;
        public bool aaFix = true;

        public Matrix3d ViewTransform = Matrix3d.Identity;

        public void DrawLines(RenderContext renderContext, float opacity, Color color)
        {
            if (linePoints.Count < 2)
            {
                return;
            }

            InitLineBuffer(renderContext);

            int count = linePoints.Count;

            if (renderContext.gl == null)
            {
                Vector3d viewPoint = Vector3d.TransformCoordinate(renderContext.ViewPoint, ViewTransform);
                


                CanvasContext2D ctx = renderContext.Device;
                ctx.Save();
                
                ctx.StrokeStyle = color.ToString();
                ctx.LineWidth = 2;
                ctx.Alpha = .25;
                Vector3d firstPoint = new Vector3d();
                Vector3d secondPoint = new Vector3d();
                for (int i = 0; i < count; i += 2)
                {
                    firstPoint = renderContext.WVP.Transform(linePoints[i]);
                    secondPoint = renderContext.WVP.Transform(linePoints[i + 1]);
                    if (Vector3d.Dot(linePoints[i], viewPoint) > .60)
                    {
                        ctx.BeginPath();
                        ctx.MoveTo(firstPoint.X, firstPoint.Y);
                        ctx.LineTo(secondPoint.X, secondPoint.Y);


                        ctx.Stroke();
                       
                    }
                } 
                ctx.Restore();
            }
            else
            {
                foreach (PositionVertexBuffer lineBuffer in lineBuffers)
                {
                    SimpleLineShader.Use(renderContext, lineBuffer.VertexBuffer, color, zBuffer);
                    renderContext.gl.drawArrays(GL.LINES, 0, lineBuffer.Count);
                }
            }
        }
           
    

        //List<SharpDX.Direct3D11.VertexBufferBinding> lineBufferBindings = new List<SharpDX.Direct3D11.VertexBufferBinding>();

        List<PositionVertexBuffer> lineBuffers = new List<PositionVertexBuffer>();
        List<int> lineBufferCounts = new List<int>();


        public bool UseLocalCenters = false;
        void InitLineBuffer(RenderContext renderContext)
        {
            if (renderContext.gl != null)
            {
                if (lineBuffers.Count == 0)
                {
                    int count = linePoints.Count;

                    PositionVertexBuffer lineBuffer = null;


                    Vector3d[] linePointList = null;
                    localCenter = new Vector3d();
                    if (DepthBuffered)
                    {
                        // compute the local center..
                        foreach (Vector3d point in linePoints)
                        {
                            localCenter.Add(point);

                        }
                        localCenter.X /= count;
                        localCenter.Y /= count;
                        localCenter.Z /= count;
                    }

                    int countLeft = count;
                    int index = 0;
                    int counter = 0;
                    Vector3d temp;

                    foreach (Vector3d point in linePoints)
                    {
                        if (counter >= 100000 || linePointList == null)
                        {
                            if (lineBuffer != null)
                            {
                                lineBuffer.Unlock();
                            }
                            int thisCount = Math.Min(100000, countLeft);

                            countLeft -= thisCount;
                            lineBuffer = new PositionVertexBuffer(thisCount);

                            linePointList = (Vector3d[])lineBuffer.Lock(); // Lock the buffer (which will return our structs)

                            lineBuffers.Add(lineBuffer);
                            lineBufferCounts.Add(thisCount);
                            counter = 0;
                        }

                        if (UseLocalCenters)
                        {
                            temp = Vector3d.SubtractVectors(point, localCenter);
                            linePointList[counter] = temp;
                        }
                        else
                        {
                            linePointList[counter] = point;
                        }
                        index++;
                        counter++;
                    }

                    if (lineBuffer != null)
                    {
                        lineBuffer.Unlock();
                    }

                }
            }
        }

        void EmptyLineBuffer()
        {
           

        }

    }

    public class OrbitLineList
    {
        public OrbitLineList()
        {
        }


        bool zBuffer = true;

        public bool DepthBuffered
        {
            get { return zBuffer; }
            set { zBuffer = value; }
        }

        List<Vector3d> linePoints = new List<Vector3d>();
        List<Color> lineColors = new List<Color>();
        public void AddLine(Vector3d v1, Vector3d v2, Color c1, Color c2)
        {

            linePoints.Add(v1);
            lineColors.Add(c1);
            linePoints.Add(v2);
            lineColors.Add(c2);
            EmptyLineBuffer();

        }

        public void Clear()
        {
            linePoints.Clear();
            EmptyLineBuffer();
        }

        Vector3d localCenter;
        public bool Sky = true;
        public bool aaFix = true;

        public Matrix3d ViewTransform = Matrix3d.Identity;

        public void DrawLines(RenderContext renderContext, float opacity, Color color)
        {
            if (linePoints.Count < 2)
            {
                return;
            }

            InitLineBuffer(renderContext);

            int count = linePoints.Count;


            foreach (PositionColoredVertexBuffer lineBuffer in lineBuffers)
            {
                OrbitLineShader.Use(renderContext, lineBuffer.VertexBuffer, color);
                renderContext.gl.drawArrays(GL.LINES, 0, lineBuffer.Count);
            }
        }

        List<PositionColoredVertexBuffer> lineBuffers = new List<PositionColoredVertexBuffer>();
        List<int> lineBufferCounts = new List<int>();


        public bool UseLocalCenters = false;
        void InitLineBuffer(RenderContext renderContext)
        {
            if (renderContext.gl != null)
            {
                if (lineBuffers.Count == 0)
                {
                    int count = linePoints.Count;

                    PositionColoredVertexBuffer lineBuffer = null;


                    PositionColored[] linePointList = null;
                    localCenter = new Vector3d();
                    if (DepthBuffered)
                    {
                        // compute the local center..
                        foreach (Vector3d point in linePoints)
                        {
                            localCenter.Add(point);

                        }
                        localCenter.X /= count;
                        localCenter.Y /= count;
                        localCenter.Z /= count;
                    }

                    int countLeft = count;
                    int index = 0;
                    int counter = 0;
                    Vector3d temp;

                    foreach (Vector3d point in linePoints)
                    {
                        if (counter >= 100000 || linePointList == null)
                        {
                            if (lineBuffer != null)
                            {
                                lineBuffer.Unlock();
                            }
                            int thisCount = Math.Min(100000, countLeft);

                            countLeft -= thisCount;
                            lineBuffer = new PositionColoredVertexBuffer(thisCount);

                            linePointList = lineBuffer.Lock(); // Lock the buffer (which will return our structs)

                            lineBuffers.Add(lineBuffer);
                            lineBufferCounts.Add(thisCount);
                            counter = 0;
                        }

                        if (UseLocalCenters)
                        {
                            temp = Vector3d.SubtractVectors(point, localCenter);
                            linePointList[counter] = new PositionColored(temp, lineColors[index]);
                        }
                        else
                        {
                            linePointList[counter] = new PositionColored(point, lineColors[index]);
                        }
                        index++;
                        counter++;
                    }

                    if (lineBuffer != null)
                    {
                        lineBuffer.Unlock();
                    }
                }
            }
        }

        void EmptyLineBuffer()
        {
            foreach (PositionColoredVertexBuffer lineBuffer in lineBuffers)
            {
                lineBuffer.Dispose();
            }
            lineBuffers.Clear();
        }
    }


    public class LineList
    {
        public LineList()
        {
        }
        bool zBuffer = true;

        public bool DepthBuffered
        {
            get { return zBuffer; }
            set { zBuffer = value; }
        }
        public bool TimeSeries = false;
        public bool ShowFarSide = true;
        public bool Sky = false;
        public double Decay = 0;
        public bool UseNonRotatingFrame = false;
        public double JNow = 0;

        List<Vector3d> linePoints = new List<Vector3d>();
        List<Color> lineColors = new List<Color>();
        List<Dates> lineDates = new List<Dates>();
        public void AddLine(Vector3d v1, Vector3d v2, Color color, Dates date)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            lineColors.Add(color);
            lineDates.Add(date);
            EmptyLineBuffer();

        }

        public void AddLineNoDate(Vector3d v1, Vector3d v2, Color color)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            lineColors.Add(color);
            lineDates.Add(new Dates(0,0));
            EmptyLineBuffer();

        }

        public void Clear()
        {
            linePoints.Clear();
            lineColors.Clear();
            lineDates.Clear();
        }
        bool usingLocalCenter = true;
        Vector3d localCenter;
        public void DrawLines(RenderContext renderContext, float opacity)
        {
            if (linePoints.Count < 2 || opacity <= 0)
            {
                return;
            }
            if (renderContext.gl == null)
            {
                //todo draw with HTML5
            }
            else
            {
                InitLineBuffer();
                //Matrix3d savedWorld = renderContext.World;
                //Matrix3d savedView = renderContext.View;
                //if (localCenter != Vector3d.Empty)
                //{
                //    usingLocalCenter = true;
                //    Vector3d temp = localCenter;
                //    if (UseNonRotatingFrame)
                //    {
                //        renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBaseNonRotating * Matrix3d.Translation(-renderContext.CameraPosition);
                //    }
                //    else
                //    {
                //        renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBase * Matrix3d.Translation(-renderContext.CameraPosition);
                //    }
                //    renderContext.View = Matrix3d.Translation(renderContext.CameraPosition) * renderContext.ViewBase;
                //}

                //DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);

                //renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;


                //LineShaderNormalDates11.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
                //LineShaderNormalDates11.Constants.Sky = Sky ? 1 : 0;
                //LineShaderNormalDates11.Constants.ShowFarSide = ShowFarSide ? 1 : 0;
                //if (TimeSeries)
                //{
                //    LineShaderNormalDates11.Constants.Decay = (float)Decay;
                //}
                //else
                //{
                //    LineShaderNormalDates11.Constants.Decay = 0;
                //}

                //LineShaderNormalDates11.Constants.Opacity = opacity;
                //LineShaderNormalDates11.Constants.CameraPosition = new SharpDX.Vector4(Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector311, 1);
                //SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
                //mat.Transpose();

                //LineShaderNormalDates11.Constants.WorldViewProjection = mat;

                //LineShaderNormalDates11.Use(renderContext.devContext);

                //renderContext.DepthStencilMode = DepthBuffered ? DepthStencilMode.ZReadWrite : DepthStencilMode.Off;

                foreach (TimeSeriesLineVertexBuffer lineBuffer in lineBuffers)
                {
                    LineShaderNormalDates.Use(renderContext, lineBuffer.VertexBuffer, Color.FromArgb(255, 255, 255, 255), zBuffer, (float)JNow, TimeSeries ? (float)Decay : 0);
                    renderContext.gl.drawArrays(GL.LINES, 0, lineBuffer.Count);
                }

                //renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;

                //if (usingLocalCenter)
                //{
                //    renderContext.World = savedWorld;
                //    renderContext.View = savedView;
                //}
            }

        }

        List<TimeSeriesLineVertexBuffer> lineBuffers = new List<TimeSeriesLineVertexBuffer>();
        List<int> lineBufferCounts = new List<int>();

        void InitLineBuffer()
        {
            if (lineBuffers.Count == 0)
            {
                int count = linePoints.Count;

                TimeSeriesLineVertexBuffer lineBuffer = null;


                TimeSeriesLineVertex[] linePointList = null;
                //localCenter = new Vector3d();
                //if (DepthBuffered)
                //{
                //    // compute the local center..
                //    foreach (Vector3d point in linePoints)
                //    {
                //        localCenter.Add(point);

                //    }
                //    localCenter.X /= count;
                //    localCenter.Y /= count;
                //    localCenter.Z /= count;
                //}

                int countLeft = count;
                int index = 0;
                int counter = 0;
                Vector3d temp;

                foreach (Vector3d point in linePoints)
                {
                    if (counter >= 100000 || linePointList == null)
                    {
                        if (lineBuffer != null)
                        {
                            lineBuffer.Unlock();
                        }
                        int thisCount = Math.Min(100000, countLeft);

                        countLeft -= thisCount;
                        lineBuffer = new TimeSeriesLineVertexBuffer(thisCount);

                        linePointList = (TimeSeriesLineVertex[])lineBuffer.Lock(); // Lock the buffer (which will return our structs)

                        lineBuffers.Add(lineBuffer);
                        lineBufferCounts.Add(thisCount);
                        counter = 0;
                    }
                    int div2 = (int)(index / 2);

                    temp = point; // -localCenter;
                    linePointList[counter] = new TimeSeriesLineVertex();
                    linePointList[counter].Position = temp;
                    linePointList[counter].Normal = point;
                    linePointList[counter].Tu = (float)lineDates[div2].StartDate;
                    linePointList[counter].Tv = (float)lineDates[div2].EndDate;
                    linePointList[counter].Color = lineColors[div2];
                    index++;
                    counter++;
                }

                if (lineBuffer != null)
                {
                    lineBuffer.Unlock();
                }

            }
        }

        void EmptyLineBuffer()
        {
            //if (lineBuffers != null)
            //{
            //    foreach (TimeSeriesLineVertexBuffer11 lineBuffer in lineBuffers)
            //    {
            //        lineBuffer.Dispose();
            //        GC.SuppressFinalize(lineBuffer);
            //    }
            //    lineBuffers.Clear();
            //    lineBufferCounts.Clear();
            //}

        }
    }


    public enum CullMode { None = 0, CounterClockwise = 2,Clockwise =1  };

    public class TriangleList
    {
        public TriangleList()
        {

        }

        List<Vector3d> trianglePoints = new List<Vector3d>();
        List<Color> triangleColors = new List<Color>();
        List<Dates> triangleDates = new List<Dates>();

        public bool TimeSeries = false;
        public bool ShowFarSide = false;
        public bool Sky = false;
        public bool DepthBuffered = true;
        public bool WriteZbuffer = false;
        public double Decay = 0;

        public bool AutoTime = true;
        public double JNow = 0;
        bool dataToDraw = false;

        public void AddTriangle(Vector3d v1, Vector3d v2, Vector3d v3, Color color, Dates date)
        {
            trianglePoints.Add(v1);
            trianglePoints.Add(v2);
            trianglePoints.Add(v3);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }

        public void AddSubdividedTriangles(Vector3d v1, Vector3d v2, Vector3d v3, Color color, Dates date, int subdivisions)
        {
            subdivisions--;

            if (subdivisions < 0)
            {
                AddTriangle(v1, v2, v3, color, date);
            }
            else
            {
                Vector3d v12;
                Vector3d v23;
                Vector3d v31;

                v12 = Vector3d.MidPointByLength(v1, v2);
                v23 = Vector3d.MidPointByLength(v2, v3);
                v31 = Vector3d.MidPointByLength(v3, v1);

                // Add 1st
                AddSubdividedTriangles(v1, v12, v31, color, date, subdivisions);
                // Add 2nd
                AddSubdividedTriangles(v12, v23, v31, color, date, subdivisions);
                // Add 3rd
                AddSubdividedTriangles(v12, v2, v23, color, date, subdivisions);
                // Add 4th
                AddSubdividedTriangles(v23, v3, v31, color, date, subdivisions);

            }
        }

        public void AddQuad(Vector3d v1, Vector3d v2, Vector3d v3, Vector3d v4, Color color, Dates date)
        {
            trianglePoints.Add(v1);
            trianglePoints.Add(v3);
            trianglePoints.Add(v2);
            trianglePoints.Add(v2);
            trianglePoints.Add(v3);
            trianglePoints.Add(v4);
            triangleColors.Add(color);
            triangleDates.Add(date);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }


        public void Clear()
        {

            triangleColors.Clear();
            trianglePoints.Clear();
            triangleDates.Clear();
            EmptyTriangleBuffer();

        }
        void EmptyTriangleBuffer()
        {
            //if (triangleBuffers != null)
            //{
            //    foreach (TimeSeriesLineVertexBuffer11 buf in triangleBuffers)
            //    {
            //        buf.Dispose();
            //        GC.SuppressFinalize(buf);
            //    }
            //    triangleBuffers.Clear();
            //    triangleBufferCounts.Clear();
            //    dataToDraw = false;
            //}

        }

        List<TimeSeriesLineVertexBuffer> triangleBuffers = new List<TimeSeriesLineVertexBuffer>();
        List<int> triangleBufferCounts = new List<int>();

        void InitTriangleBuffer()
        {

            if (triangleBuffers.Count == 0)
            {
                int count = trianglePoints.Count;

                TimeSeriesLineVertexBuffer triangleBuffer = null;

                TimeSeriesLineVertex[] triPointList = null;
                int countLeft = count;
                int index = 0;
                int counter = 0;
                foreach (Vector3d point in trianglePoints)
                {
                    if (counter >= 90000 || triangleBuffer == null)
                    {
                        if (triangleBuffer != null)
                        {
                            triangleBuffer.Unlock();
                        }
                        int thisCount = Math.Min(90000, countLeft);

                        countLeft -= thisCount;
                        triangleBuffer = new TimeSeriesLineVertexBuffer(thisCount);

                        triangleBuffers.Add(triangleBuffer);
                        triangleBufferCounts.Add(thisCount);
                        triPointList = (TimeSeriesLineVertex[])triangleBuffer.Lock(); // Lock the buffer (which will return our structs)
                        counter = 0;
                    }

                    triPointList[counter] = new TimeSeriesLineVertex();
                    triPointList[counter].Position = point;
                    triPointList[counter].Normal = point;
                    int div3 = (int)(index / 3);

                    triPointList[counter].Color = triangleColors[div3];
                    triPointList[counter].Tu = (float)triangleDates[div3].StartDate;
                    triPointList[counter].Tv = (float)triangleDates[div3].EndDate;
                    index++;
                    counter++;
                }
                if (triangleBuffer != null)
                {
                    triangleBuffer.Unlock();
                }

                triangleColors.Clear();
                triangleDates.Clear();
                trianglePoints.Clear();

                dataToDraw = true;
            }

        }

       

        public void Draw(RenderContext renderContext, float opacity, CullMode cull)
        {
            if (trianglePoints.Count < 1 && !dataToDraw)
            {
                return;
            }

            

            //renderContext.DepthStencilMode = DepthBuffered ? (WriteZbuffer ? DepthStencilMode.ZReadWrite : DepthStencilMode.ZReadOnly) : DepthStencilMode.Off;

            //renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            //switch (cull)
            //{
            //    case CullMode.Clockwise:
            //        renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
            //        break;
            //    case CullMode.CounterClockwise:
            //        renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
            //        break;
            //    case CullMode.None:
            //        renderContext.setRasterizerState(TriangleCullMode.Off);
            //        break;
            //    default:
            //        break;
            //}


            //if (AutoTime)
            //{
            //    DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);
            //    LineShaderNormalDates.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
            //}
            //else
            //{
            //    LineShaderNormalDates.Constants.JNow = (float)JNow;
            //}

            //LineShaderNormalDates.Constants.Sky = 0;
            //LineShaderNormalDates.Constants.ShowFarSide = ShowFarSide ? 1 : 0;
            //if (TimeSeries)
            //{
            //    LineShaderNormalDates.Constants.Decay = (float)Decay;
            //}
            //else
            //{
            //    LineShaderNormalDates.Constants.Decay = 0;
            //}
            //LineShaderNormalDates.Constants.Opacity = opacity;
            //LineShaderNormalDates.Constants.CameraPosition = new SharpDX.Vector4(Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector311, 1);

            //SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            //mat.Transpose();

            //LineShaderNormalDates.Constants.WorldViewProjection = mat;

            //LineShaderNormalDates.Use(renderContext.devContext);

            //foreach (TimeSeriesLineVertexBuffer vertBuffer in triangleBuffers)
            //{
            //    renderContext.SetVertexBuffer(vertBuffer);
            //    renderContext.devContext.Draw(vertBuffer.Count, 0);
            //}

            if (renderContext.gl == null)
            {
               //todo implement HTML5 version
            }
            else
            {

                InitTriangleBuffer();
                foreach (TimeSeriesLineVertexBuffer triBuffer in triangleBuffers)
                {
                    LineShaderNormalDates.Use(renderContext, triBuffer.VertexBuffer, Color.FromArgb(255, 255, 255, 255), DepthBuffered, (float)JNow, TimeSeries ? (float)Decay : 0);
                    renderContext.gl.drawArrays(GL.TRIANGLES, 0, triBuffer.Count);
                }
            }



        }
    }

    public class PointList
    {
        public PointList(RenderContext device)
        {
            this.device = device;
        }

        RenderContext device;
        List<Vector3d> points = new List<Vector3d>();
        List<Color> colors = new List<Color>();
        List<Dates> dates = new List<Dates>();
        List<float> sizes = new List<float>();
        public bool TimeSeries = false;
        public bool ShowFarSide = false;
        public bool Sky = false;
        public bool DepthBuffered = true;
        public double Decay = 0;
        public double scale = 1;
        public bool AutoTime = true;
        public double JNow = 0;
        bool dataToDraw = false;

        public void AddPoint(Vector3d v1, Color color, Dates date, float size)
        {
            points.Add(v1);
            colors.Add(color.Clone());
            dates.Add(date);
            sizes.Add(size);
            EmptyPointBuffer();
        }


        public void Clear()
        {

            colors.Clear();
            points.Clear();
            dates.Clear();
            EmptyPointBuffer();

        }
        Vector3d[] transformedList;
        Vector3d[] worldList;

        void EmptyPointBuffer()
        {
            foreach (TimeSeriesPointVertexBuffer pointBuffer in pointBuffers)
            {
                pointBuffer.Dispose();
            }
            pointBuffers.Clear();
            init = false;
        }

        public List<DataItem> items = new List<DataItem>();

        ImageElement starProfile;

        public static Texture starTexture = null;
        bool imageReady = false;
        bool init = false;
        public float MinSize = 2.0f;
        List<TimeSeriesPointVertexBuffer> pointBuffers = new List<TimeSeriesPointVertexBuffer>();
        List<int> pointBufferCounts = new List<int>();
        //const double jBase = 2455198.0;
        void InitBuffer(RenderContext renderContext)
        {
            if (!init)
            {
                if (renderContext.gl == null)
                {
                    starProfile = (ImageElement)Document.CreateElement("img");
                    starProfile.AddEventListener("load", delegate (ElementEvent e)
                    {
                        imageReady = true;
                    }, false);

                    starProfile.Src = "/images/StarProfileAlpha.png";

                    worldList = new Vector3d[points.Count];
                    transformedList = new Vector3d[points.Count];

                    int index = 0;
                    foreach (Vector3d pnt in points)
                    {
                        // todo filter by date
                        DataItem item = new DataItem();
                        item.Location = pnt;
                        item.Tranformed = new Vector3d();
                        item.Size = sizes[index];
                        item.Color = colors[index];
                        worldList[index] = item.Location;
                        transformedList[index] = item.Tranformed;
                        items.Add(item);
                        index++;
                    }
                }
                else
                {
                    if (pointBuffers.Count == 0)
                    {
                        if (starTexture == null)
                        {
                            starTexture = Planets.LoadPlanetTexture("/images/StarProfileAlpha.png");
                        }

                        int count = this.points.Count;

                        TimeSeriesPointVertexBuffer pointBuffer = null;
                        TimeSeriesPointVertex[] pointList = null;

                        int countLeft = count;
                        int index = 0;
                        int counter = 0;
                        foreach (Vector3d point in points)
                        {
                            if (counter >= 100000 || pointList == null)
                            {
                                if (pointBuffer != null)
                                {
                                    pointBuffer.Unlock();
                                }
                                int thisCount = Math.Min(100000, countLeft);

                                countLeft -= thisCount;
                                pointBuffer = new TimeSeriesPointVertexBuffer(thisCount);

                                pointList = (TimeSeriesPointVertex[])pointBuffer.Lock(); // Lock the buffer (which will return our structs)

                                pointBuffers.Add(pointBuffer);
                                pointBufferCounts.Add(thisCount);
                                counter = 0;
                            }
                            pointList[counter] = new TimeSeriesPointVertex();
                            pointList[counter].Position = point;
                            pointList[counter].PointSize = sizes[index];
                            pointList[counter].Tu = (float)(dates[index].StartDate);
                            pointList[counter].Tv = (float)(dates[index].EndDate);
                            pointList[counter].Color = colors[index];
                            index++;
                            counter++;
                        }

                        if (pointBuffer != null)
                        {
                            pointBuffer.Unlock();
                        }
                    }
                }

                init = true;
            }
        }

        public void Draw(RenderContext renderContext, float opacity, bool cull)
        {
            InitBuffer(renderContext);




            if (renderContext.gl == null)
            {
                if (!imageReady)
                {
                    return;
                }
                renderContext.Device.Save();

                renderContext.WVP.ProjectArrayToScreen(worldList, transformedList);
                CanvasContext2D ctx = renderContext.Device;
                ctx.Alpha = .4;

                double width = renderContext.Width;
                double height = renderContext.Height;

                Vector3d viewPoint = Vector3d.MakeCopy(renderContext.ViewPoint);

                double scaleFactor = renderContext.FovScale / 100;
                foreach (DataItem item in items)
                {
                    // todo filter by date

                    // if (Vector3d.Dot(viewPoint, item.Location) < 0)
                    if (item.Tranformed.Z < 1)
                    {
                        double x = item.Tranformed.X;
                        double y = item.Tranformed.Y;
                        double size = .1 * item.Size / scaleFactor;
                        double half = size / 2;
                        if (x > -half && x < width + half && y > -half && y < height + half)
                        {
                            //ctx.DrawImage(starProfile, x - size / 2, y - size / 2, size, size);

                            ctx.BeginPath();
                            // ctx.FillStyle = "rgb(200,0,0)";
                            ctx.FillStyle = item.Color.ToFormat();
                            ctx.Arc(x, y, size, 0, Math.PI * 2, true);
                            ctx.Fill();
                        }
                    }

                }

                renderContext.Device.Restore();
            }
            else
            {
                foreach (TimeSeriesPointVertexBuffer pointBuffer in pointBuffers)
                {
                    TimeSeriesPointSpriteShader.Use(
                            renderContext, pointBuffer.VertexBuffer, starTexture.Texture2d,
                            Color.FromArgb(255 * opacity, 255, 255, 255), DepthBuffered, (float)(this.JNow),
                            this.TimeSeries ? 0 : (float)Decay, renderContext.CameraPosition, (float)(scale * (renderContext.Height / 960)), MinSize
                        );

                    renderContext.gl.drawArrays(GL.POINTS, 0, pointBuffer.Count);
                }

                // renderContext.gl.disable(0x8642);
            }
        }

        internal void DrawTextured(RenderContext renderContext, Texture texture, float opacity)
        {
            InitBuffer(renderContext);

            foreach (TimeSeriesPointVertexBuffer pointBuffer in pointBuffers)
            {
                TimeSeriesPointSpriteShader.Use(
                        renderContext, pointBuffer.VertexBuffer, texture.Texture2d,
                        Color.FromArgb(255*opacity, 255, 255, 255), DepthBuffered, (float)(this.JNow),
                        (float)Decay, renderContext.CameraPosition, (float)(scale * (renderContext.Height / 960)), MinSize
                    );

                renderContext.gl.drawArrays(GL.POINTS, 0, pointBuffer.Count);
            }     
        }
    }

    public class TimeSeriesLineVertex
    {
        public Vector3d Position = new Vector3d();
        public Vector3d Normal = new Vector3d();
        public Color color;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public float Tu;
        public float Tv;
        public static TimeSeriesLineVertex Create(Vector3d position, Vector3d normal, float time, Color color)
        {
            TimeSeriesLineVertex temp = new TimeSeriesLineVertex();

            temp.Position = position;
            temp.Normal = normal;
            temp.Tu = time;
            temp.Tv = 0;
            temp.color = color;

            return temp;
        }
    }

    public enum PointScaleTypes { Linear=0, Power=1, Log=2, Constant=3, StellarMagnitude=4 };
    public class TimeSeriesPointVertex
    {
        public Vector3d Position;
        public float PointSize;
        public Color color;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        public float Tu;
        public float Tv;
        public static TimeSeriesPointVertex Create(Vector3d position, float size, float time, Color color)
        {
            TimeSeriesPointVertex tmp = new TimeSeriesPointVertex();

            tmp.Position = position;
            tmp.PointSize = size;
            tmp.Tu = time;
            tmp.Tv = 0;
            tmp.color = color;
            return tmp;

        }
    }


}
