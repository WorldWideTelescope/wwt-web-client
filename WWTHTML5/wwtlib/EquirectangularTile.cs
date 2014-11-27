using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;

namespace wwtlib
{

    public class EquirectangularTile : Tile
    {
        double tileDegrees;
        bool topDown = true;

        
        protected void ComputeBoundingSphere()
        {
            if (!topDown)
            {
                ComputeBoundingSphereBottomsUp();
                return;
            }

            tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.Level));

            double latMin = (90 - (((double)this.tileY) * tileDegrees));
            double latMax = (90 - (((double)(this.tileY + 1)) * tileDegrees));
            double lngMin = (((double)this.tileX * tileDegrees) - 180.0);
            double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = (Vector3d)GeoTo3d(latCenter, lngCenter, false);
            TopLeft = (Vector3d)GeoTo3d(latMin, lngMin, false);
            BottomRight = (Vector3d)GeoTo3d(latMax, lngMax, false);
            TopRight = (Vector3d)GeoTo3d(latMin, lngMax, false);
            BottomLeft = (Vector3d)GeoTo3d(latMax, lngMin, false);

            Vector3d distVect = (Vector3d)GeoTo3d(latMin, lngMin, false);
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
            tileDegrees = lngMax - lngMin;
        }

        //public override bool IsTileBigEnough(RenderContext renderContext)
        //{
        //    double arcPixels = tileDegrees / 256 * 900;
        //    return (renderContext.FovScale < arcPixels);

        //}


        protected void ComputeBoundingSphereBottomsUp()
        {
            double tileDegrees = (double)this.dataset.BaseTileDegrees / ((double)Math.Pow(2, this.Level));


            double latMin = (-90 + (((double)(this.tileY + 1)) * tileDegrees));
            double latMax = (-90 + (((double)this.tileY) * tileDegrees));
            double lngMin = (((double)this.tileX * tileDegrees) - 180.0);
            double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = (Vector3d)GeoTo3d(latCenter, lngCenter, false);

            TopLeft = (Vector3d)GeoTo3d(latMin, lngMin, false);
            BottomRight = (Vector3d)GeoTo3d(latMax, lngMax, false);
            TopRight = (Vector3d)GeoTo3d(latMin, lngMax, false);
            BottomLeft = (Vector3d)GeoTo3d(latMax, lngMin, false);
            Vector3d distVect = TopLeft;
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
            tileDegrees = lngMax - lngMin;
        }
        int subDivisionLevel = 1;

        public override bool CreateGeometry(RenderContext renderContext)
        {
            base.CreateGeometry(renderContext);
            if (renderContext.gl == null)
            {
                if (dataset.DataSetType == ImageSetType.Earth || dataset.DataSetType == ImageSetType.Planet)
                {
                    subDivisionLevel = Math.Max(2, (4 - Level) * 2);
                }
            }
            else
            {
                subDivisionLevel = 32;
            }

            try
            {
                for (int i = 0; i < 4; i++)
                {
                    RenderTriangleLists[i] = new List<RenderTriangle>();
                }

                if (!topDown)
                {
                    return CreateGeometryBottomsUp(renderContext);
                }
                double lat, lng;

                int index = 0;
                double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.Level));

                double latMin = (90 - (((double)this.tileY) * tileDegrees));
                double latMax = (90 - (((double)(this.tileY + 1)) * tileDegrees));
                double lngMin = (((double)this.tileX * tileDegrees) - 180.0);
                double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);
                double tileDegreesX = lngMax - lngMin;
                double tileDegreesY = latMax - latMin;


                TopLeft = (Vector3d)GeoTo3d(latMin, lngMin, false);
                BottomRight = (Vector3d)GeoTo3d(latMax, lngMax, false);
                TopRight = (Vector3d)GeoTo3d(latMin, lngMax, false);
                BottomLeft = (Vector3d)GeoTo3d(latMax, lngMin, false);


                // Create a vertex buffer 
                PositionTexture[] verts = new PositionTexture[(subDivisionLevel + 1) * (subDivisionLevel + 1)]; // Lock the buffer (which will return our structs)
                int x, y;

                double textureStep = 1.0f / subDivisionLevel;
                for (y = 0; y <= subDivisionLevel; y++)
                {
                    if (y != subDivisionLevel)
                    {
                        lat = latMin + (textureStep * tileDegreesY * y);
                    }
                    else
                    {
                        lat = latMax;
                    }
                    for (x = 0; x <= subDivisionLevel; x++)
                    {

                        if (x != subDivisionLevel)
                        {
                            lng = lngMin + (textureStep * tileDegreesX * x);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y * (subDivisionLevel + 1) + x;
                        verts[index] = PositionTexture.CreatePos(GeoTo3d(lat, lng, false), x * textureStep, y * textureStep);
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
                            for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                            {
                                for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                                {
                                    //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                                    // First triangle in quad

                                    PositionTexture p1;
                                    PositionTexture p2;
                                    PositionTexture p3;

                                    p1 = verts[(y1 * (subDivisionLevel + 1) + x1)];
                                    p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                    p3 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                    RenderTriangleLists[part].Add(RenderTriangle.Create(p1, p3, p2, texture, Level));

                                    // Second triangle in quad
                                    p1 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                    p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                    p3 = verts[((y1 + 1) * (subDivisionLevel + 1) + (x1 + 1))];
                                    RenderTriangleLists[part].Add(RenderTriangle.Create(p1, p3, p2, texture, Level));

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
                            for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                            {
                                for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
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
            catch
            {
            }
            return true;
        }

        private bool CreateGeometryBottomsUp(RenderContext renderContext)
        {
        
            double lat, lng;

            int index = 0;
            double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.Level));


            double latMin = (-90 + (((double)(this.tileY+1)) * tileDegrees));
            double latMax = (-90 + (((double)this.tileY) * tileDegrees));
            double lngMin = (((double)this.tileX * tileDegrees) - 180.0);
            double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - 180.0);
            double tileDegreesX = lngMax - lngMin;
            double tileDegreesY = latMax - latMin;
            // Create a vertex buffer 
            PositionTexture[] verts = new PositionTexture[(subDivisionLevel + 1) * (subDivisionLevel + 1)]; // Lock the buffer (which will return our structs)
            int x, y;

            double textureStep = 1.0f / subDivisionLevel;
            for (y = 0; y <= subDivisionLevel; y++)
            {
                if (y != subDivisionLevel)
                {
                    lat = latMin + (textureStep * tileDegreesY * y);
                }
                else
                {
                    lat = latMax;
                }
                for (x = 0; x <= subDivisionLevel; x++)
                {

                    if (x != subDivisionLevel)
                    {
                        lng = lngMin + (textureStep * tileDegreesX * x);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = y * (subDivisionLevel + 1) + x;
                    verts[index] = PositionTexture.CreatePos(GeoTo3d(lat, lng, false), x * textureStep, y * textureStep);
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
                        for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                        {
                            for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                            {
                                //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                                // First triangle in quad

                                PositionTexture p1;
                                PositionTexture p2;
                                PositionTexture p3;

                                p1 = verts[(y1 * (subDivisionLevel + 1) + x1)];
                                p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                p3 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                RenderTriangleLists[part].Add(RenderTriangle.Create(p1, p3, p2, texture, Level));

                                // Second triangle in quad
                                p1 = verts[(y1 * (subDivisionLevel + 1) + (x1 + 1))];
                                p2 = verts[((y1 + 1) * (subDivisionLevel + 1) + x1)];
                                p3 = verts[((y1 + 1) * (subDivisionLevel + 1) + (x1 + 1))];
                                RenderTriangleLists[part].Add(RenderTriangle.Create(p1, p3, p2, texture, Level));

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

                        for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                        {
                            for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
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


            return true;
        }

        public EquirectangularTile()
        {
        }
      
        public static EquirectangularTile Create(int level, int x, int y, Imageset dataset, Tile parent)
        {
            EquirectangularTile temp = new EquirectangularTile();
            temp.Parent = parent;
            temp.Level = level;
            temp.tileX = x;
            temp.tileY = y;
            temp.dataset = dataset;
            temp.topDown = !dataset.BottomsUp;     
            temp.ComputeBoundingSphere();
            return temp;
        }
    }
}