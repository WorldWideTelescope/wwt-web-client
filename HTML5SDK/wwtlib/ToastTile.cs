using System;
using System.Collections.Generic;
using System.Html;
namespace wwtlib
{
    public class ToastTile : Tile
    {
        bool topDown = true;

        protected PositionTexture[] bounds;
        protected bool backslash = false;
        List<PositionTexture> vertexList = null;
        List<Triangle>[] childTriangleList = null;

        protected float[] demArray;
        protected void ComputeBoundingSphere()
        {
            InitializeGrids();

            TopLeft = bounds[0 + 3 * 0].Position.Copy();
            BottomRight = bounds[2 + 3 * 2].Position.Copy();
            TopRight = bounds[2 + 3 * 0].Position.Copy();
            BottomLeft = bounds[0 + 3 * 2].Position.Copy();
            CalcSphere();
        }

        static protected WebGLBuffer[] slashIndexBuffer = new WebGLBuffer[64];
        static protected WebGLBuffer[] backSlashIndexBuffer = new WebGLBuffer[64];
        static protected WebGLBuffer[] rootIndexBuffer = new WebGLBuffer[4];

        public override WebGLBuffer GetIndexBuffer(int index, int accomidation)
        {
            if (Level == 0)
            {
                return rootIndexBuffer[index];
            }

            if (backslash)
            {
                return backSlashIndexBuffer[index * 16 + accomidation];
            }
            else
            {
                return slashIndexBuffer[index * 16 + accomidation];
            }
        }

        private void ProcessIndexBuffer(UInt16[] indexArray, int part)
        {

            if (Level == 0)
            {
                rootIndexBuffer[part] = PrepDevice.createBuffer();
                PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, rootIndexBuffer[part]);
                PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER,(Uint16Array)(object)indexArray, GL.STATIC_DRAW);   
                return;
            }

            for (int a = 0; a < 16; a++)
            {
                UInt16[] partArray = CloneArray(indexArray);
                ProcessAccomindations(partArray, a);
                if (backslash)
                {
                    backSlashIndexBuffer[part * 16 + a] = PrepDevice.createBuffer();
                    PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, backSlashIndexBuffer[part * 16 + a]);
                    PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, (Uint16Array)(object)partArray, GL.STATIC_DRAW);   
                }
                else
                {
                    slashIndexBuffer[part * 16 + a] = PrepDevice.createBuffer();
                    PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, slashIndexBuffer[part * 16 + a]);
                    PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, (Uint16Array)(object)partArray, GL.STATIC_DRAW);
                }
            }
        }

        private static UInt16[] CloneArray(UInt16[] indexArray)
        {
            int count = indexArray.Length;
            Uint16Array ui16array = new Uint16Array(count);

            UInt16[] indexArrayNew =  (UInt16[])(object)ui16array;
            for (int i = 0; i < count; i++)
            {
                indexArrayNew[i] = indexArray[i];
            }

            return indexArrayNew;
        }

        private void ProcessAccomindations(UInt16[] indexArray, int a)
        {
            Dictionary<UInt16, UInt16> map = new Dictionary<UInt16, UInt16>();
            Dictionary<int, UInt16> gridMap = new Dictionary<int, UInt16>();

            foreach (UInt16 index in indexArray)
            {
                PositionTexture vert = vertexList[index];
                int arrayX = (int)(vert.Tu * 16 + .5);
                int arrayY = (int)(vert.Tv * 16 + .5);
                int ii = (arrayY << 8) + arrayX;

                if (!gridMap.ContainsKey(ii))
                {
                    gridMap[ii] = index;
                }

            }


            int sections = 16;

            if ((a & 1) == 1)
            {
                for (int x = 1; x < sections; x += 2)
                {
                    int y = sections;
                    int key = (y << 8) + x;
                    int val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map[gridMap[key]] = (gridMap[val]);
                    }
                }
            }

            if ((a & 2) == 2)
            {
                for (int y = 1; y < sections; y += 2)
                {
                    int x = sections;
                    int key = (y << 8) + x;
                    int val = ((y + 1) << 8) + x;
                    if (gridMap.ContainsKey(key))
                    {
                        map[gridMap[key]] = (gridMap[val]);
                    }
                }
            }

            if ((a & 4) == 4)
            {
                for (int x = 1; x < sections; x += 2)
                {
                    int y = 0;
                    int key = (y << 8) + x;
                    int val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map[gridMap[key]] = (gridMap[val]);
                    }
                }
            }

            if ((a & 8) == 8)
            {
                for (int y = 1; y < sections; y += 2)
                {
                    int x = 0;
                    int key = (y << 8) + x;
                    int val = ((y + 1) << 8) + x;
                    if (gridMap.ContainsKey(key))
                    {
                        map[gridMap[key]] =  (gridMap[val]);
                    }
                }
            }

            if (map.Count == 0)
            {
                //nothing to process
                return;
            }

            for (int i = 0; i < indexArray.Length; i++)
            {
                if (map.ContainsKey(indexArray[i]))
                {
                    indexArray[i] = map[indexArray[i]];
                }
            }
        }


        protected void CalculateFullSphere(Vector3d[] list)
        {
            SphereHull result = ConvexHull.FindEnclosingSphere(list);

            sphereCenter = result.Center;
            sphereRadius = result.Radius;
        }

        public override bool IsPointInTile(double lat, double lng)
        {
  
            if (Level == 0)
            {
                return true;
            }

            if (Level == 1)
            {
                if ((lng >= 0 && lng <= 90) && (tileX == 0 && tileY == 1))
                {
                    return true;
                }
                if ((lng > 90 && lng <= 180) && (tileX == 1 && tileY == 1))
                {
                    return true;
                }
                if ((lng < 0 && lng >= -90) && (tileX == 0 && tileY == 0))
                {
                    return true;
                }
                if ((lng < -90 && lng >= -180) && (tileX == 1 && tileY == 0))
                {
                    return true;
                }
                return false;
            }

            if (!this.DemReady || this.DemData == null)
            {
                return false;
            }

            Vector3d testPoint = Coordinates.GeoTo3dDouble(-lat, lng);
            bool top = IsLeftOfHalfSpace(TopLeft.Copy(), TopRight.Copy(), testPoint);
            bool right = IsLeftOfHalfSpace(TopRight.Copy(), BottomRight.Copy(), testPoint);
            bool bottom = IsLeftOfHalfSpace(BottomRight.Copy(), BottomLeft.Copy(), testPoint);
            bool left = IsLeftOfHalfSpace(BottomLeft.Copy(), TopLeft.Copy(), testPoint);

            if (top && right && bottom && left)
            {
                // showSelected = true;
                return true;
            }
            return false; ;

        }

        private bool IsLeftOfHalfSpace(Vector3d pntA, Vector3d pntB, Vector3d pntTest)
        {
            pntA.Normalize();
            pntB.Normalize();
            Vector3d cross = Vector3d.Cross(pntA, pntB);

            double dot = Vector3d.Dot(cross, pntTest);

            return dot < 0;
        }

        public override double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (Level < lastDeepestLevel)
            {
                //interate children
                for(int ii=0; ii < 4; ii++)
                {
                    Tile child = children[ii];
                    if (child != null)
                    {
                        if (child.IsPointInTile(lat, lng))
                        {
                            double retVal = child.GetSurfacePointAltitude(lat, lng, meters);
                            if (retVal != 0)
                            {
                                return retVal;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            TileTargetLevel = Level;
            TileTargetX = tileX;
            TileTargetY = tileY;



            Vector3d testPoint = Coordinates.GeoTo3dDouble(-lat, lng);
            testPoint = Vector3d.SubtractVectors(new Vector3d(), testPoint);
            Vector2d uv = DistanceCalc.GetUVFromInnerPoint(TopLeft.Copy(), TopRight.Copy(), BottomLeft.Copy(), BottomRight.Copy(), testPoint.Copy());


            //Document.Title = "u:" + uv.X + ", v:" + uv.Y;
            //uv.X = 1 - uv.X;
            //uv.Y = 1 - uv.Y;
            // Get 4 samples and interpolate
            double uud = Math.Max(0, Math.Min(16, (uv.X * 16)));
            double vvd = Math.Max(0, Math.Min(16, (uv.Y * 16)));



            int uu = Math.Max(0, Math.Min(15, (int)(uv.X * 16)));
            int vv = Math.Max(0, Math.Min(15, (int)(uv.Y * 16)));

            double ha = uud - uu;
            double va = vvd - vv;

            if (demArray != null)
            {
                // 4 nearest neighbors
                double ul = demArray[uu + 17 * vv];
                double ur = demArray[(uu + 1) + 17 * vv];
                double ll = demArray[uu + 17 * (vv + 1)];
                double lr = demArray[(uu + 1) + 17 * (vv + 1)];

                double top = ul * (1 - ha) + ha * ur;
                double bottom = ll * (1 - ha) + ha * lr;
                double val = top * (1 - va) + va * bottom;

                return val / DemScaleFactor;
            }

            return demAverage / DemScaleFactor;
        }
        

        private void InitializeGrids()
        {
            vertexList = new List<PositionTexture>();
            childTriangleList = new List<Triangle>[4];
            childTriangleList[0] = new List<Triangle>();
            childTriangleList[1] = new List<Triangle>();
            childTriangleList[2] = new List<Triangle>();
            childTriangleList[3] = new List<Triangle>();

            bounds = new PositionTexture[9];

            if (Level > 0)
            {
                // Set in constuctor now
                //ToastTile parent = (ToastTile)TileCache.GetTile(level - 1, x / 2, y / 2, dataset, null);
                if (Parent == null)
                {
                    Parent = TileCache.GetTile(Level - 1, tileX / 2, tileY / 2, dataset, null);
                }

                ToastTile parent = (ToastTile)Parent;

                int xIndex = tileX % 2;
                int yIndex = tileY % 2;

                if (Level > 1)
                {
                    backslash = parent.backslash;
                }
                else
                {
                    backslash = xIndex == 1 ^ yIndex == 1;
                }


                bounds[0 + 3 * 0] = parent.bounds[xIndex + 3 * yIndex].Copy();
                bounds[1 + 3 * 0] = Midpoint(parent.bounds[xIndex + 3 * yIndex], parent.bounds[xIndex + 1 + 3 * yIndex]);
                bounds[2 + 3 * 0] = parent.bounds[xIndex + 1 + 3 * yIndex].Copy();
                bounds[0 + 3 * 1] = Midpoint(parent.bounds[xIndex + 3 * yIndex], parent.bounds[xIndex + 3 * (yIndex + 1)]);

                if (backslash)
                {
                    bounds[1 + 3 * 1] = Midpoint(parent.bounds[xIndex + 3 * yIndex], parent.bounds[xIndex + 1 + 3 * (yIndex + 1)]);
                }
                else
                {
                    bounds[1 + 3 * 1] = Midpoint(parent.bounds[xIndex + 1 + 3 * yIndex], parent.bounds[xIndex + 3 * (yIndex + 1)]);
                }

                bounds[2 + 3 * 1] = Midpoint(parent.bounds[xIndex + 1 + 3 * yIndex], parent.bounds[xIndex + 1 + 3 * (yIndex + 1)]);
                bounds[0 + 3 * 2] = parent.bounds[xIndex + 3 * (yIndex + 1)].Copy();
                bounds[1 + 3 * 2] = Midpoint(parent.bounds[xIndex + 3 * (yIndex + 1)], parent.bounds[xIndex + 1 + 3 * (yIndex + 1)]);
                bounds[2 + 3 * 2] = parent.bounds[xIndex + 1 + 3 * (yIndex + 1)].Copy();

                bounds[0 + 3 * 0].Tu = 0*uvMultiple;
                bounds[0 + 3 * 0].Tv = 0 * uvMultiple;
                bounds[1 + 3 * 0].Tu = .5f * uvMultiple;
                bounds[1 + 3 * 0].Tv = 0 * uvMultiple;
                bounds[2 + 3 * 0].Tu = 1 * uvMultiple;
                bounds[2 + 3 * 0].Tv = 0 * uvMultiple;

                bounds[0 + 3 * 1].Tu = 0 * uvMultiple;
                bounds[0 + 3 * 1].Tv = .5f * uvMultiple;
                bounds[1 + 3 * 1].Tu = .5f * uvMultiple;
                bounds[1 + 3 * 1].Tv = .5f * uvMultiple;
                bounds[2 + 3 * 1].Tu = 1 * uvMultiple;
                bounds[2 + 3 * 1].Tv = .5f * uvMultiple;

                bounds[0 + 3 * 2].Tu = 0 * uvMultiple;
                bounds[0 + 3 * 2].Tv = 1 * uvMultiple;
                bounds[1 + 3 * 2].Tu = .5f * uvMultiple;
                bounds[1 + 3 * 2].Tv = 1 * uvMultiple;
                bounds[2 + 3 * 2].Tu = 1 * uvMultiple;
                bounds[2 + 3 * 2].Tv = 1 * uvMultiple;

                vertexList.Add(bounds[0 + 3 * 0]);
                vertexList.Add(bounds[1 + 3 * 0]);
                vertexList.Add(bounds[2 + 3 * 0]);
                vertexList.Add(bounds[0 + 3 * 1]);
                vertexList.Add(bounds[1 + 3 * 1]);
                vertexList.Add(bounds[2 + 3 * 1]);
                vertexList.Add(bounds[0 + 3 * 2]);
                vertexList.Add(bounds[1 + 3 * 2]);
                vertexList.Add(bounds[2 + 3 * 2]);




                if (backslash)
                {
                    childTriangleList[0].Add(Triangle.Create(4, 1, 0));
                    childTriangleList[0].Add(Triangle.Create(3, 4, 0));
                    childTriangleList[1].Add(Triangle.Create(5, 2, 1));
                    childTriangleList[1].Add(Triangle.Create(4, 5, 1));
                    childTriangleList[2].Add(Triangle.Create(7, 4, 3));
                    childTriangleList[2].Add(Triangle.Create(6, 7, 3));
                    childTriangleList[3].Add(Triangle.Create(8, 5, 4));
                    childTriangleList[3].Add(Triangle.Create(7, 8, 4));
                }
                else
                {
                    childTriangleList[0].Add(Triangle.Create(3, 1, 0));
                    childTriangleList[0].Add(Triangle.Create(4, 1, 3));
                    childTriangleList[1].Add(Triangle.Create(4, 2, 1));
                    childTriangleList[1].Add(Triangle.Create(5, 2, 4));
                    childTriangleList[2].Add(Triangle.Create(6, 4, 3));
                    childTriangleList[2].Add(Triangle.Create(7, 4, 6));
                    childTriangleList[3].Add(Triangle.Create(7, 5, 4));
                    childTriangleList[3].Add(Triangle.Create(8, 5, 7));
                }
            }
            else
            {
                bounds[0 + 3 * 0] = PositionTexture.Create(0, -1, 0, 0, 0);
                bounds[1 + 3 * 0] = PositionTexture.Create(0, 0, 1, .5f, 0);
                bounds[2 + 3 * 0] = PositionTexture.Create(0, -1, 0, 1, 0);
                bounds[0 + 3 * 1] = PositionTexture.Create(-1, 0, 0, 0, .5f);
                bounds[1 + 3 * 1] = PositionTexture.Create(0, 1, 0, .5f, .5f);
                bounds[2 + 3 * 1] = PositionTexture.Create(1, 0, 0, 1, .5f);
                bounds[0 + 3 * 2] = PositionTexture.Create(0, -1, 0, 0, 1);
                bounds[1 + 3 * 2] = PositionTexture.Create(0, 0, -1, .5f, 1);
                bounds[2 + 3 * 2] = PositionTexture.Create(0, -1, 0, 1, 1);

                vertexList.Add(bounds[0 + 3 * 0]);
                vertexList.Add(bounds[1 + 3 * 0]);
                vertexList.Add(bounds[2 + 3 * 0]);
                vertexList.Add(bounds[0 + 3 * 1]);
                vertexList.Add(bounds[1 + 3 * 1]);
                vertexList.Add(bounds[2 + 3 * 1]);
                vertexList.Add(bounds[0 + 3 * 2]);
                vertexList.Add(bounds[1 + 3 * 2]);
                vertexList.Add(bounds[2 + 3 * 2]);

                childTriangleList[0].Add(Triangle.Create(3, 1, 0));
                childTriangleList[0].Add(Triangle.Create(4, 1, 3));
                childTriangleList[1].Add(Triangle.Create(5, 2, 1));
                childTriangleList[1].Add(Triangle.Create(4, 5, 1));
                childTriangleList[2].Add(Triangle.Create(7, 4, 3));
                childTriangleList[2].Add(Triangle.Create(6, 7, 3));
                childTriangleList[3].Add(Triangle.Create(7, 5, 4));
                childTriangleList[3].Add(Triangle.Create(8, 5, 7));
                // Setup default matrix of points.
            }
        }

        private PositionTexture Midpoint(PositionTexture positionNormalTextured, PositionTexture positionNormalTextured_2)
        {
            Vector3d a1 = Vector3d.Lerp(positionNormalTextured.Position, positionNormalTextured_2.Position, .5f);
            Vector2d a1uv = Vector2d.Lerp(Vector2d.Create(positionNormalTextured.Tu, positionNormalTextured.Tv), Vector2d.Create(positionNormalTextured_2.Tu, positionNormalTextured_2.Tv), .5f);

            a1.Normalize();
            return PositionTexture.CreatePos(a1, a1uv.X, a1uv.Y);
        }
        int subDivisionLevel = 4;
        bool subDivided = false;

        public override bool CreateGeometry(RenderContext renderContext)
        {
            if (GeometryCreated)
            {
                return true;
            }

            GeometryCreated = true;
            base.CreateGeometry(renderContext);
            if (!subDivided)
            {
             


                if (vertexList == null)
                {
                    InitializeGrids();
                }
           
                if (uvMultiple == 256)
                {
                    if (dataset.DataSetType == ImageSetType.Earth || dataset.DataSetType == ImageSetType.Planet)
                    {
                        subDivisionLevel = Math.Min(5, Math.Max(0, 5 - Level));
                    }
                    else
                    {
                        subDivisionLevel = Math.Min(5, Math.Max(0, 5 - Level));

                    }

                }
                else
                {
                    if (demTile && Level > 1)
                    {
                        demArray = new float[17 * 17];
                        demSize = 17 * 17;
                        if (backslash)
                        {
                            if (backslashYIndex == null)
                            {
                                tempBackslashYIndex = new byte[demSize];
                                tempBackslashXIndex = new byte[demSize];
                            }
                        }
                        else
                        {
                            if (slashYIndex == null)
                            {
                                tempSlashYIndex = new byte[demSize];
                                tempSlashXIndex = new byte[demSize];
                            }
                        }
                    }
                }    

                for (int i = 0; i < 4; i++)
                {
                    int count = subDivisionLevel;
                    while (count-- > 1)
                    {
                        List<Triangle> newList = new List<Triangle>();
                        foreach (Triangle tri in childTriangleList[i])
                        {
                            tri.SubDivide(newList, vertexList);
                        }
                        childTriangleList[i] = newList;
                    }
                }

                if (renderContext.gl == null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        RenderTriangleLists[i] = new List<RenderTriangle>();
                        foreach (Triangle tri in childTriangleList[i])
                        {
                            PositionTexture p1 = vertexList[tri.C];
                            PositionTexture p2 = vertexList[tri.B];
                            PositionTexture p3 = vertexList[tri.A];


                            RenderTriangleLists[i].Add(RenderTriangle.Create(p1, p2, p3, texture, Level));
                        }
                    }
                }
                else
                {
                    //process vertex list
                    VertexBuffer = PrepDevice.createBuffer();
                    PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
                    Float32Array f32array = new Float32Array(vertexList.Count * 5);
                    float[] buffer = (float[])(object)f32array;
                    int index = 0;
                    foreach (PositionTexture pt in vertexList)
                    {
                        if (demTile)
                        {
                            index = AddVertex(buffer, index, GetMappedVertex(pt));
                            demIndex++;
                        }
                        else
                        {
                            index = AddVertex(buffer, index, pt);
                        }
                    }
                    if (demTile)
                    {
                        if (backslash)
                        {
                            if (tempBackslashXIndex != null)
                            {
                                backslashXIndex = tempBackslashXIndex;
                                backslashYIndex = tempBackslashYIndex;
                                tempBackslashXIndex = null;
                                tempBackslashYIndex = null;
                            }
                        }
                        else
                        {
                            if (tempSlashYIndex != null)
                            {
                                slashXIndex = tempSlashXIndex;
                                slashYIndex = tempSlashYIndex;
                                tempSlashYIndex = null;
                                tempSlashXIndex = null;

                            }
                        }
                    }


                    PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);

                    //process index list
                    for (int i = 0; i < 4; i++)
                    {
                        TriangleCount = childTriangleList[i].Count; 
                        if (GetIndexBuffer(i, 0) == null)
                        {

                            Uint16Array ui16array = new Uint16Array(TriangleCount * 3);

                            UInt16[] indexArray = (UInt16[])(object)ui16array;

                            index = 0;
                            foreach (Triangle tri in childTriangleList[i])
                            {
                                indexArray[index++] = (UInt16)tri.C;
                                indexArray[index++] = (UInt16)tri.B;
                                indexArray[index++] = (UInt16)tri.A;
                            }
                            ProcessIndexBuffer(indexArray, i);
                        }
                        //IndexBuffers[i] = PrepDevice.createBuffer();
                        //PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, IndexBuffers[i]);
                        //PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, ui16array, GL.STATIC_DRAW);                       
                    }
                }

                subDivided = true;
            }

            return true;
        }

        private static byte[] slashXIndex;
        private static byte[] slashYIndex;
        private static byte[] backslashXIndex;
        private static byte[] backslashYIndex;

        private byte[] tempSlashXIndex;
        private byte[] tempSlashYIndex;
        private byte[] tempBackslashXIndex;
        private byte[] tempBackslashYIndex;


        internal PositionTexture GetMappedVertex(PositionTexture vert)
        {
            PositionTexture vertOut = new PositionTexture();
            Coordinates latLng = Coordinates.CartesianToSpherical2(vert.Position);
            //      latLng.Lng += 90;
            if (latLng.Lng < -180)
            {
                latLng.Lng += 360;
            }
            if (latLng.Lng > 180)
            {
                latLng.Lng -= 360;
            }


            if (Level > 1)
            {
                byte arrayX = (byte)(int)(vert.Tu * 16 + .5);
                byte arrayY = (byte)(int)(vert.Tv * 16 + .5);
                demArray[arrayX + arrayY * 17] = DemData[demIndex];

                if (backslash)
                {
                    if (tempBackslashYIndex != null)
                    {
                        tempBackslashXIndex[demIndex] = arrayX;
                        tempBackslashYIndex[demIndex] = arrayY;
                    }
                }
                else
                {
                    if (tempSlashYIndex != null)
                    {
                        tempSlashXIndex[demIndex] = arrayX;
                        tempSlashYIndex[demIndex] = arrayY;
                    }
                }
            }

            Vector3d pos = GeoTo3dWithAlt(latLng.Lat, latLng.Lng, false, false);
            vertOut.Tu = (float)vert.Tu;
            vertOut.Tv = (float)vert.Tv;

            //vertOut.Lat = latLng.Lat;
            //vertOut.Lng = latLng.Lng;
            //vertOut.Normal = pos;
            pos.Subtract(localCenter);
            vertOut.Position = pos;


            return vertOut;
        }

        //public override bool IsTileBigEnough(RenderContext renderContext)
        //{
        //  //  return Level < 1;
        //    double arcPixels = 0;
        //    if (dataset.DataSetType == ImageSetType.Earth)
        //    {
        //        arcPixels = (1600 / Math.Pow(2, Level));
        //    }
        //    else
        //    {
        //        arcPixels = (3600 / Math.Pow(2, Level));

        //    }
        //    return (renderContext.FovScale < arcPixels);
        //}
        


        //int quadrant = 0;

        //private void ComputeQuadrant()
        //{
        //    int xQuad = 0;
        //    int yQuad = 0;
        //    int tiles = (int)Math.Pow(2, this.level);

        //    if (x > (tiles / 2) - 1)
        //    {
        //        xQuad = 1;
        //    }

        //    if (y > (tiles / 2) - 1)
        //    {
        //        yQuad = 1;
        //    }
        //    quadrant = yQuad * 2 + xQuad;
        //}

        public ToastTile()
        {
        }

        public static ToastTile Create(int level, int xc, int yc, Imageset dataset, Tile parent)
        {
            ToastTile temp = new ToastTile();
            temp.Parent = parent;
            temp.Level = level;
            temp.tileX = xc;
            temp.tileY = yc;
            temp.dataset = dataset;
            temp.topDown = !dataset.BottomsUp;


            if (temp.tileX != (int)xc)
            {
                Script.Literal("alert('bad')");
            }
            //temp.ComputeQuadrant();

            if (dataset.MeanRadius != 0)
            {
                temp.DemScaleFactor = dataset.MeanRadius;
            }
            else
            {
                if (dataset.DataSetType == ImageSetType.Earth)
                {
                    temp.DemScaleFactor = 6371000;
                }
                else
                {
                    temp.DemScaleFactor = 3396010;
                }
            }


            temp.ComputeBoundingSphere();
            return temp;
        }

        public override void CleanUp(bool removeFromParent)
        {
            base.CleanUp(removeFromParent);
            
            if (vertexList != null)
            {
                vertexList = null;
            }
            if (childTriangleList != null)
            {
                childTriangleList = null;
            }
            
            
            subDivided = false;
            demArray = null;
        }

        private float GetDemSample(int xc, int yc)
        {
            return demArray[(16 - yc) * 17 + xc];
        }

        public override bool CreateDemFromParent()
        {


            ToastTile parent = Parent as ToastTile;
            if (parent == null)
            {
                return false;
            }

            int offsetX = ((tileX % 2) == 1 ? 8 : 0);
            int offsetY = ((tileY % 2) == 0 ? 8 : 0);


            demArray = new float[17 * 17];
            // Interpolate accross 
            for (int yy1 = 0; yy1 < 17; yy1 += 2)
            {
                bool copy = true;
                for (int xx1 = 0; xx1 < 17; xx1++)
                {
                    if (copy)
                    {
                        demArray[(16 - yy1) * 17 + xx1] = parent.GetDemSample((xx1 / 2) + offsetX, (yy1 / 2) + offsetY);
                    }
                    else
                    {
                        demArray[(16 - yy1) * 17 + xx1] =
                            (
                            (
                                parent.GetDemSample((xx1 / 2) + offsetX, (yy1 / 2) + offsetY) +
                                parent.GetDemSample(((xx1 / 2) + offsetX) + 1, (yy1 / 2) + offsetY)
                            ) / 2);
                    }
                    copy = !copy;

                }
            }
            // Interpolate down
            for (int yy2 = 1; yy2 < 17; yy2 += 2)
            {
                for (int xx2 = 0; xx2 < 17; xx2++)
                {

                    demArray[(16 - yy2) * 17 + xx2] =
                        (
                        (
                            GetDemSample(xx2, yy2 - 1) +
                            GetDemSample(xx2, yy2 + 1)
                        ) / 2);

                }
            }

            // Convert the dem array back to the arranged DEM list thu slash/backslash mapping tables


            DemData = new float[demSize];
            for (int i = 0; i < demSize; i++)
            {
                if (backslash)
                {
                    DemData[i] = demArray[backslashXIndex[i] + backslashYIndex[i] * 17];
                }
                else
                {
                    DemData[i] = demArray[slashXIndex[i] + slashYIndex[i] * 17];
                }
                demAverage += DemData[i];

            }

            // Get Average value for new DemData table

            demAverage /= DemData.Length;

            DemReady = true;
            return true;
        }
    }

    class DistanceCalc
    {
        static public double LineToPoint(Vector3d l0, Vector3d l1, Vector3d p)
        {
            Vector3d v = Vector3d.SubtractVectors(l1, l0);
            Vector3d w = Vector3d.SubtractVectors(p, l0);

            double dist = Vector3d.Cross(w, v).Length() / v.Length();

            return dist;
        }

        static public Vector2d GetUVFromInnerPoint(Vector3d ul, Vector3d ur, Vector3d ll, Vector3d lr, Vector3d pnt)
        {
            ul.Normalize();
            ur.Normalize();
            ll.Normalize();
            lr.Normalize();
            pnt.Normalize();

            double dUpper = LineToPoint(ul, ur, pnt);
            double dLower = LineToPoint(ll, lr, pnt);
            double dVert = dUpper + dLower;

            double dRight = LineToPoint(ur, lr, pnt);
            double dLeft = LineToPoint(ul, ll, pnt);
            double dHoriz = dRight + dLeft;

            return Vector2d.Create(dLeft / dHoriz, dUpper / dVert);
        }
    }
}
