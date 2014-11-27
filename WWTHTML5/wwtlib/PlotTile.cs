using System;
using System.Collections.Generic;
using System.Html;

namespace wwtlib
{
    public class PlotTile : Tile
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


        public override void RenderPart(RenderContext renderContext, int part, double opacity, bool combine)
        {
            if (renderContext.gl != null)
            {
                //todo draw in WebGL
            }
            else
            {
                if (part == 0)
                {
                    foreach(Star star in stars)
                    {
                        double radDec = 25 / Math.Pow(1.6, star.Magnitude);

                        Planets.DrawPointPlanet(renderContext, star.Position, radDec, star.Col, false);
                    }
                }
            }
        }

        WebFile webFile;
        public override void RequestImage()
        {
            if (!Downloading && !ReadyToRender)
            {
                Downloading = true;

                webFile = new WebFile(Util.GetProxiedUrl(this.URL));
                webFile.OnStateChange = FileStateChange;
                webFile.Send();
            }
        }

        public void FileStateChange()
        {
            if(webFile.State == StateType.Error)
            {
                Downloading = false;
                ReadyToRender = false;
                errored = true;
                RequestPending = false;
                TileCache.RemoveFromQueue(this.Key, true);
            }
            else if(webFile.State == StateType.Received)
            {

                texReady = true;
                Downloading = false;
                errored = false;
                ReadyToRender = texReady && (DemReady || !demTile);
                RequestPending = false;
                TileCache.RemoveFromQueue(this.Key, true);
                LoadData(webFile.GetText());
            }

        }

        List<Star> stars = new List<Star>();
        private void LoadData(string data)
        {
            string[] rows = data.Replace("\r\n","\n").Split("\n");

            bool firstRow = true;
            PointType type = PointType.Move;
            Star star = null;
            foreach (string row in rows)
            {
                if (firstRow)
                {
                    firstRow = false;
                    continue;
                }

                if (row.Trim().Length > 5)
                {

                    star = new Star(row);
                    star.Position = Coordinates.RADecTo3dAu(star.RA, star.Dec, 1);

                    stars.Add(star);
                }
            }
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

                PlotTile parent = (PlotTile)Parent;

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

            return true;
        }

        



        public PlotTile()
        {
        }

        public static PlotTile Create(int level, int xc, int yc, Imageset dataset, Tile parent)
        {
            PlotTile temp = new PlotTile();
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

    }

}
