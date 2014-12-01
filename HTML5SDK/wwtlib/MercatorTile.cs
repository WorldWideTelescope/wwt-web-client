using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    public class MercatorTile : Tile
    {


        //public override bool IsTileBigEnough(RenderContext renderContext)
        //{


        //    double arcPixels = tileDegrees / 256 * 900;
        //    return (renderContext.FovScale < arcPixels);

        //}
        double tileDegrees;
        private double latMin = 0;
        private double latMax = 0;
        private double lngMin = 0;
        private double lngMax = 0;

        protected void ComputeBoundingSphere()
        {
            tileDegrees = 360 / (Math.Pow(2, this.Level));

            latMin = AbsoluteMetersToLatAtZoom(tileY * 256, Level);
            latMax = AbsoluteMetersToLatAtZoom((tileY + 1) * 256, Level);
            lngMin = (((double)this.tileX * tileDegrees) - 180.0);
            lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = (Vector3d)GeoTo3d(latCenter, lngCenter, false);


            TopLeft = (Vector3d)GeoTo3d(latMin, lngMin, false);
            BottomRight = (Vector3d)GeoTo3d(latMax, lngMax, false);
            TopRight = (Vector3d)GeoTo3d(latMin, lngMax, false);
            BottomLeft = (Vector3d)GeoTo3d(latMax, lngMin, false);
            if (tileY == 0)
            {
                TopLeft = Vector3d.Create(0, 1, 0);
                TopRight = Vector3d.Create(0, 1, 0);
            }

            if (tileY == Math.Pow(2, Level) - 1)
            {
                BottomRight = Vector3d.Create(0, -1, 0);
                BottomLeft = Vector3d.Create(0, -1, 0);

            }

            Vector3d distVect = TopLeft;
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
            distVect = BottomRight;
            distVect.Subtract(sphereCenter);
            double len = distVect.Length();

            if (sphereRadius < len)
            {
                sphereRadius = len;
            }

            tileDegrees = Math.Abs(latMax - latMin);
        }

        public override bool IsPointInTile(double lat, double lng)
        {
            if (!this.DemReady || this.DemData == null || lat < Math.Min(latMin, latMax) || lat > Math.Max(latMax, latMin) || lng < Math.Min(lngMin, lngMax) || lng > Math.Max(lngMin, lngMax))
            {
                return false;
            }
            return true;

        }
        public override double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (Level < lastDeepestLevel)
            {
                //interate children
                foreach (Tile child in children)
                {
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

            double alt = GetAltitudeAtLatLng(lat, lng, meters ? 1 : DemScaleFactor);

            return alt;

        }

        private double GetAltitudeAtLatLng(double lat, double lng, double scaleFactor)
        {

            double height = Math.Abs(latMax - latMin);
            double width = Math.Abs(lngMax - lngMin);

            double yy = ((lat - Math.Min(latMax, latMin)) / height * 32);
            double xx = ((lng - Math.Min(lngMax, lngMin)) / width * 32);


            int indexY = Math.Min(31, (int)yy);
            int indexX = Math.Min(31, (int)xx);

            double ha = xx - indexX;
            double va = yy - indexY;

            double ul = DemData[indexY * 33 + indexX];
            double ur = DemData[indexY * 33 + (indexX + 1)];
            double ll = DemData[(indexY + 1) * 33 + indexX];
            double lr = DemData[(indexY + 1) * 33 + (indexX + 1)];

            double top = ul * (1 - ha) + ha * ur;
            double bottom = ll * (1 - ha) + ha * lr;
            double val = top * (1 - va) + va * bottom;

            return val / scaleFactor;

        }



        int subDivisionLevel = 32;
        public override bool CreateGeometry(RenderContext renderContext)
        {
            base.CreateGeometry(renderContext);

            if (GeometryCreated)
            {
                return true;
            }
            GeometryCreated = true;

            if (uvMultiple == 256)
            {
                if (dataset.DataSetType == ImageSetType.Earth || dataset.DataSetType == ImageSetType.Planet)
                {
                    subDivisionLevel = Math.Max(2, (6 - Level) * 2);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                RenderTriangleLists[i] = new List<RenderTriangle>();
            }

        //    try
            {
                double lat, lng;

                int index = 0;
                double tileDegrees = 360 / (Math.Pow(2, this.Level));

                latMin = AbsoluteMetersToLatAtZoom(tileY * 256, Level);
                latMax = AbsoluteMetersToLatAtZoom((tileY + 1) * 256, Level);
                lngMin = (((double)this.tileX * tileDegrees) - 180.0);
                lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);

                double latCenter = AbsoluteMetersToLatAtZoom(((tileY * 2) + 1) * 256, Level + 1);

                TopLeft = (Vector3d)GeoTo3d(latMin, lngMin, false);
                BottomRight = (Vector3d)GeoTo3d(latMax, lngMax, false);
                TopRight = (Vector3d)GeoTo3d(latMin, lngMax, false);
                BottomLeft = (Vector3d)GeoTo3d(latMax, lngMin, false);

                PositionTexture[] verts = new PositionTexture[(subDivisionLevel + 1) * (subDivisionLevel + 1)];


                tileDegrees = lngMax - lngMin;
                double dGrid = (tileDegrees / subDivisionLevel);
                int x1, y1;
                double textureStep = 1.0f / subDivisionLevel;

                double latDegrees = latMax - latCenter;

                for (y1 = 0; y1 < subDivisionLevel / 2; y1++)
                {

                    if (y1 != subDivisionLevel / 2)
                    {
                        lat = latMax - (2 * textureStep * latDegrees * (double)y1);
                    }
                    else
                    {
                        lat = latCenter;
                    }

                    for (x1 = 0; x1 <= subDivisionLevel; x1++)
                    {
                        if (x1 != subDivisionLevel)
                        {
                            lng = lngMin + (textureStep * tileDegrees * (double)x1);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y1 * (subDivisionLevel + 1) + x1;
                        verts[index] = new PositionTexture();
                        verts[index].Position = (Vector3d)GeoTo3dWithAlt(lat, lng, false, true);// Add Altitude mapping here
                        verts[index].Tu = (x1 * textureStep)*Tile.uvMultiple;
                        verts[index].Tv = ((AbsoluteLatToMetersAtZoom(lat, Level) - (tileY * 256)) / 256f)*Tile.uvMultiple;
                        demIndex++;
                    }
                }
                latDegrees = latMin - latCenter;

                for (y1 = subDivisionLevel / 2; y1 <= subDivisionLevel; y1++)
                {

                    if (y1 != subDivisionLevel)
                    {
                        lat = latCenter + (2 * textureStep * latDegrees * (double)(y1 - (subDivisionLevel / 2)));
                    }
                    else
                    {
                        lat = latMin;
                    }

                    for (x1 = 0; x1 <= subDivisionLevel; x1++)
                    {
                        if (x1 != subDivisionLevel)
                        {
                            lng = lngMin + (textureStep * tileDegrees * (double)x1);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y1 * (subDivisionLevel + 1) + x1;
                        verts[index] = new PositionTexture();
                        verts[index].Position = (Vector3d)GeoTo3dWithAlt(lat, lng, false, true);// Add Altitude mapping here
                        verts[index].Tu = (x1 * textureStep)*Tile.uvMultiple;
                        verts[index].Tv = ((AbsoluteLatToMetersAtZoom(lat, Level) - (tileY * 256)) / 256f)*Tile.uvMultiple;
                        demIndex++;
                    }
                }
                if (tileY == 0)
                {
                    // Send the tops to the pole to fill in the Bing Hole
                    y1 = subDivisionLevel;
                    for (x1 = 0; x1 <= subDivisionLevel; x1++)
                    {
                        index = y1 * (subDivisionLevel + 1) + x1;
                        verts[index].Position = Vector3d.Create(0, 1, 0);
                        
                    }
                }

                if (tileY == Math.Pow(2, Level) - 1)
                {
                    // Send the tops to the pole to fill in the Bing Hole
                    y1 = 0;
                    for (x1 = 0; x1 <= subDivisionLevel; x1++)
                    {
                        index = y1 * (subDivisionLevel + 1) + x1;
                        verts[index].Position = Vector3d.Create(0, -1, 0);
                        
                    }
                }


                TriangleCount = (subDivisionLevel) * (subDivisionLevel) * 2;

                int quarterDivisions = subDivisionLevel / 2;
                int part = 0;

                if (renderContext.gl == null)
                {
                    for (int y2 = 0; y2 < 2; y2++)
                    {
                        for (int x2 = 0; x2 < 2; x2++)
                        {

                            index = 0;
                            for (y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                            {
                                for (x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                                {
                                    //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                                    // First triangle in quad

                                    PositionTexture p1;
                                    PositionTexture p2;
                                    PositionTexture p3;

                                    p1 = verts[(y1 * (subDivisionLevel + 1) + x1)];
                                    p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                    p3 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                    RenderTriangle tri = RenderTriangle.Create(p1, p2, p3, texture, Level);
                                    RenderTriangleLists[part].Add(tri);

                                    // Second triangle in quad
                                    p1 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                    p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                    p3 = verts[((y1 + 1) * (subDivisionLevel + 1) + (x1 + 1))];
                                    tri = RenderTriangle.Create(p1, p2, p3, texture, Level);
                                    RenderTriangleLists[part].Add(tri);

                                }
                            }

                            part++;
                        }
                    }
                }
                else
                {
                    //process vertex list
                    VertexBuffer = PrepDevice.createBuffer();
                    PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
                    Float32Array f32array = new Float32Array(verts.Length * 5);
                    float[] buffer = (float[])(object)f32array;
                    index = 0;
                    foreach (PositionTexture pt in verts)
                    {
                        index = AddVertex(buffer, index, pt);
                    }

                    PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);

                    for (int y2 = 0; y2 < 2; y2++)
                    {
                        for (int x2 = 0; x2 < 2; x2++)
                        {
                            Uint16Array ui16array = new Uint16Array(TriangleCount * 3);

                            UInt16[] indexArray = (UInt16[])(object)ui16array;
                            
                            index = 0;
                            for (y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                            {
                                for (x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                                {
                                    // First triangle in quad
                                    indexArray[index++] = (UInt16)((y1 * (subDivisionLevel + 1) + x1));
                                    indexArray[index++] = (UInt16)(((y1 + 1) * (subDivisionLevel + 1) + x1));
                                    indexArray[index++] = (UInt16)((y1 * (subDivisionLevel + 1) + (x1 + 1)));
                            
                                    // Second triangle in quad
                                    indexArray[index++] = (UInt16)((y1 * (subDivisionLevel + 1) + (x1 + 1)));
                                    indexArray[index++] = (UInt16)(((y1 + 1) * (subDivisionLevel + 1) + x1));
                                    indexArray[index++] = (UInt16)(((y1 + 1) * (subDivisionLevel + 1) + (x1 + 1)));            
                                }
                            }

                            IndexBuffers[part] = PrepDevice.createBuffer();
                            PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, IndexBuffers[part]);
                            PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, ui16array, GL.STATIC_DRAW);   

                            part++;
                        }
                    }
                }

            }
     //       catch
            {
            }

            return true;
        }


        //public MercatorTile(int Level, int X, int Y, ImageSet dataset)
        //{
        //    this.Level = Level;
        //    this.X = X;
        //    this.Y = Y;
        //    this.tileSize = dataset.TileSize;
        //    rect = new Rectangle(0, 0, tileSize, tileSize);
        //    ComputeBoundingSphere();
        //}

        public MercatorTile()
        {
        }

        public static MercatorTile Create(int level, int X, int Y, Imageset dataset, Tile parent)
        {
            MercatorTile temp = new MercatorTile();
            temp.Parent = parent;
            temp.Level = level;
            temp.tileX = X;
            temp.tileY = Y;
            temp.dataset = dataset;
            temp.ComputeBoundingSphere();

            return temp;
        }

        private float GetDemSample(int x, int y)
        {
            return DemData[(32 - y) * 33 + x];
        }

        public override bool CreateDemFromParent()
        {
            MercatorTile parent = Parent as MercatorTile;

            if (parent == null || parent.DemData == null)
            {
                return false;
            }

           
            int offsetX = ((tileX % 2) == 1 ? 16 : 0);
            int offsetY = ((tileY % 2) == 1 ? 16 : 0);


            DemData = new float[demSize];
            // Interpolate accross 
            for (int y = 0; y < 33; y += 2)
            {
                bool copy = true;
                for (int x = 0; x < 33; x++)
                {
                    if (copy)
                    {
                        DemData[(32 - y) * 33 + x] = parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY);
                    }
                    else
                    {
                        DemData[(32 - y) * 33 + x] =
                            (
                            (
                                parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY) +
                                parent.GetDemSample(((x / 2) + offsetX) + 1, (y / 2) + offsetY)
                            ) / 2);
                    }
                    copy = !copy;

                }
            }
            // Interpolate down
            for (int y = 1; y < 33; y += 2)
            {
                for (int x = 0; x < 33; x++)
                {

                    DemData[(32 - y) * 33 + x] =
                        (
                        (
                            GetDemSample(x, y - 1) +
                            GetDemSample(x, y + 1)
                        ) / 2);

                }
            }

            foreach (float sample in DemData)
            {
                demAverage += sample;
            }

            demAverage /= DemData.Length;
            DemReady = true;
            return true;
        }


        //        string[] tileMap = new string[] { "q", "r", "t", "s" };

        public static Vector2d GetCentrePointOffsetAsTileRatio(double lat, double lon, int zoom)
        {
            double metersX = AbsoluteLonToMetersAtZoom(lon, zoom);

            double relativeXIntoCell = (metersX / GRID_SIZE) -
                Math.Floor(metersX / GRID_SIZE);

            double metersY = AbsoluteLatToMetersAtZoom(lat, zoom);

            double relativeYIntoCell = (metersY / GRID_SIZE) -
                Math.Floor(metersY / GRID_SIZE);

            return (Vector2d.Create(relativeXIntoCell, relativeYIntoCell));
        }

        public static double RelativeMetersToLatAtZoom(double Y,
            int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersY = Y * metersPerPixel;

            return (RadToDeg(Math.PI / 2 - 2 * Math.Atan(Math.Exp(0 - metersY / EARTH_RADIUS))));
        }

        public static double RelativeMetersToLonAtZoom(double X,
            int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = X * metersPerPixel;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }

        public static int AbsoluteLatToMetersAtZoom(double latitude, int zoom)
        {
            double sinLat = Math.Sin(DegToRad(latitude));
            double metersY = EARTH_RADIUS / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            double metersPerPixel = MetersPerPixel2(zoom);

            return ((int)(Math.Round(OFFSET_METERS - metersY) / metersPerPixel));
        }

        public static double AbsoluteMetersToLatAtZoom(int Y, int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersY = (double)OFFSET_METERS - (double)Y * metersPerPixel;

            return (RadToDeg(Math.PI / 2 - 2 * Math.Atan(Math.Exp(0 - metersY / EARTH_RADIUS))));
        }

        public static int AbsoluteLonToMetersAtZoom(double longitude, int zoom)
        {
            double metersX = EARTH_RADIUS * DegToRad(longitude);
            double metersPerPixel = MetersPerPixel2(zoom);

            return (int)(((metersX + OFFSET_METERS) / metersPerPixel));
            //return ((int)Math.Round((metersX + OFFSET_METERS) / metersPerPixel));
        }

        public static double AbsoluteMetersToLonAtZoom(int X, int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = X * metersPerPixel - OFFSET_METERS;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }


        public static int AbsoluteLonToMetersAtZoomTile(double longitude, int zoom, int tileX)
        {
            double metersX = EARTH_RADIUS * DegToRad(longitude);
            double metersPerPixel = MetersPerPixel2(zoom);
            return ((int)((metersX + OFFSET_METERS) / metersPerPixel));

            //return ((int)Math.Round((metersX + OFFSET_METERS) / metersPerPixel));
        }

        public static int AbsoluteLatToMetersAtZoomTile(double latitude, int zoom, int tileX)
        {
            double sinLat = Math.Sin(DegToRad(latitude));
            double metersY = EARTH_RADIUS / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            double metersPerPixel = MetersPerPixel2(zoom);

            return ((int)(Math.Round(OFFSET_METERS - metersY) / metersPerPixel));
        }

        public static double AbsoluteMetersToLonAtZoomByTileY(int X, int zoom, int tileY)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = X * metersPerPixel - OFFSET_METERS;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }


        private static double DegToRad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double MetersPerPixel2(int zoom)
        {
            //return MetersPerPixel(zoom);
            return (BASE_METERS_PER_PIXEL / (double)(1 << zoom));
        }

        private static double RadToDeg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        private const double EARTH_RADIUS = 6378137;
        private const double GRID_SIZE = 256.0d;
        private const double BASE_METERS_PER_PIXEL = 156543;//163840;
        private const double OFFSET_METERS = 20037508;

    }
}