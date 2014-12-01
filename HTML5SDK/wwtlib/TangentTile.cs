using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    public class TangentTile : Tile
    {
        bool topDown = true;
        protected void ComputeBoundingSphere()
        {
            //insideOut = this.Dataset.DataSetType == ImageSetType.Sky || this.Dataset.DataSetType == ImageSetType.Panorama;
            if (!topDown)
            {
                ComputeBoundingSphereBottomsUp();
                return;
            }
            double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.Level));

            double latMin = (((double)this.dataset.BaseTileDegrees / 2 - (((double)this.tileY) * tileDegrees)) + dataset.OffsetY);
            double latMax = (((double)this.dataset.BaseTileDegrees / 2 - (((double)(this.tileY + 1)) * tileDegrees)) + dataset.OffsetY);
            double lngMin = ((((double)this.tileX * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);
            double lngMax = (((((double)(this.tileX + 1)) * tileDegrees) - (double)this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = GeoTo3dTan(latCenter, lngCenter);

            TopLeft = GeoTo3dTan(latMin, lngMin);
            BottomRight = GeoTo3dTan(latMax, lngMax);
            TopRight = GeoTo3dTan(latMin, lngMax);
            BottomLeft = GeoTo3dTan(latMax, lngMin);

            Vector3d distVect = GeoTo3dTan(latMin, lngMin);
            tileDegrees = lngMax - lngMin;
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
        }

        protected Vector3d GeoTo3dTan(double lat, double lng)
        {
            lng = -lng;
            double fac1 = this.dataset.BaseTileDegrees / 2;
            double factor = Math.Tan(fac1 * RC);

            return (Vector3d)dataset.Matrix.Transform(Vector3d.Create(1, (lat / fac1 * factor), (lng / fac1 * factor)));

        }



        protected void ComputeBoundingSphereBottomsUp()
        {
            double tileDegrees = (double)this.dataset.BaseTileDegrees / ((double)Math.Pow(2, this.Level));


            double latMin = ((double)this.dataset.BaseTileDegrees / 2 + (((double)(this.tileY + 1)) * tileDegrees)) + dataset.OffsetY;
            double latMax = ((double)this.dataset.BaseTileDegrees / 2 + (((double)this.tileY) * tileDegrees)) + dataset.OffsetY;
            double lngMin = (((double)this.tileX * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;
            double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;


            TopLeft = GeoTo3dTan(latMin, lngMin);
            BottomRight = GeoTo3dTan(latMax, lngMax);
            TopRight = GeoTo3dTan(latMin, lngMax);
            BottomLeft = GeoTo3dTan(latMax, lngMin);
            Vector3d distVect = TopLeft;
            tileDegrees = lngMax - lngMin;
        }

        public override bool CreateGeometry(RenderContext renderContext)
        {
            base.CreateGeometry(renderContext);

            if (GeometryCreated)
            {
                return true;
            }
            GeometryCreated = true;

            for (int i = 0; i < 4; i++)
            {
                RenderTriangleLists[i] = new List<RenderTriangle>();
            }


            double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.Level));

            double latMin = (((double)this.dataset.BaseTileDegrees / 2.0 - (((double)this.tileY) * tileDegrees)) + this.dataset.OffsetY);
            double latMax = (((double)this.dataset.BaseTileDegrees / 2.0 - (((double)(this.tileY + 1)) * tileDegrees)) + this.dataset.OffsetY);
            double lngMin = ((((double)this.tileX * tileDegrees) - (double)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
            double lngMax = (((((double)(this.tileX + 1)) * tileDegrees) - (double)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
            double tileDegreesX = lngMax - lngMin;
            double tileDegreesY = latMax - latMin;



            TopLeft = GeoTo3dTan(latMin, lngMin);
            BottomRight = GeoTo3dTan(latMax, lngMax);
            TopRight = GeoTo3dTan(latMin, lngMax);
            BottomLeft = GeoTo3dTan(latMax, lngMin);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            Vector3d center = Vector3d.MidPoint(TopLeft, BottomRight);
            Vector3d leftCenter = Vector3d.MidPoint(TopLeft, BottomLeft);
            Vector3d rightCenter = Vector3d.MidPoint(TopRight, BottomRight);
            Vector3d topCenter = Vector3d.MidPoint(TopLeft, TopRight);
            Vector3d bottomCenter = Vector3d.MidPoint(BottomLeft, BottomRight);

            if (renderContext.gl == null)
            {
                RenderTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePos(TopLeft, 0, 0), PositionTexture.CreatePos(leftCenter, 0, .5), PositionTexture.CreatePos(topCenter, .5, 0), texture, Level));
                RenderTriangleLists[0].Add(RenderTriangle.Create(PositionTexture.CreatePos(leftCenter, 0, 0.5), PositionTexture.CreatePos(center, .5, .5), PositionTexture.CreatePos(topCenter, .5, 0), texture, Level));
                RenderTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePos(topCenter, .5, 0), PositionTexture.CreatePos(rightCenter, 1, .5), PositionTexture.CreatePos(TopRight, 1, 0), texture, Level));
                RenderTriangleLists[1].Add(RenderTriangle.Create(PositionTexture.CreatePos(topCenter, .5, 0), PositionTexture.CreatePos(center, .5, .5), PositionTexture.CreatePos(rightCenter, 1, .5), texture, Level));
                RenderTriangleLists[2].Add(RenderTriangle.Create(PositionTexture.CreatePos(leftCenter, 0, .5), PositionTexture.CreatePos(bottomCenter, .5, 1), PositionTexture.CreatePos(center, .5, .5), texture, Level));
                RenderTriangleLists[2].Add(RenderTriangle.Create(PositionTexture.CreatePos(leftCenter, 0, .5), PositionTexture.CreatePos(BottomLeft, 0, 1), PositionTexture.CreatePos(bottomCenter, .5, 1), texture, Level));
                RenderTriangleLists[3].Add(RenderTriangle.Create(PositionTexture.CreatePos(center, .5, .5), PositionTexture.CreatePos(BottomRight, 1, 1), PositionTexture.CreatePos(rightCenter, 1, .5), texture, Level));
                RenderTriangleLists[3].Add(RenderTriangle.Create(PositionTexture.CreatePos(center, .5, .5), PositionTexture.CreatePos(bottomCenter, .5, 1), PositionTexture.CreatePos(BottomRight, 1, 1), texture, Level));
            }
            else
            {

                //process vertex list
                VertexBuffer = PrepDevice.createBuffer();
                PrepDevice.bindBuffer(GL.ARRAY_BUFFER, VertexBuffer);
                Float32Array f32array = new Float32Array(9 * 5);
                float[] buffer = (float[])(object)f32array;
                int index = 0;

                index = AddVertex(buffer, index, PositionTexture.CreatePos(bottomCenter, .5, 1)); //0
                index = AddVertex(buffer, index, PositionTexture.CreatePos(BottomLeft, 0, 1));    //1
                index = AddVertex(buffer, index, PositionTexture.CreatePos(BottomRight, 1, 1));   //2
                index = AddVertex(buffer, index, PositionTexture.CreatePos(center, .5, .5));      //3
                index = AddVertex(buffer, index, PositionTexture.CreatePos(leftCenter, 0, .5));   //4
                index = AddVertex(buffer, index, PositionTexture.CreatePos(rightCenter, 1, .5));  //5
                index = AddVertex(buffer, index, PositionTexture.CreatePos(topCenter, .5, 0));    //6
                index = AddVertex(buffer, index, PositionTexture.CreatePos(TopLeft, 0, 0));       //7
                index = AddVertex(buffer, index, PositionTexture.CreatePos(TopRight, 1, 0));      //8
                PrepDevice.bufferData(GL.ARRAY_BUFFER, f32array, GL.STATIC_DRAW);

                // process index buffers

                for (int i = 0; i < 4; i++)
                {
                    index = 0;
                    TriangleCount = 2;
                    Uint16Array ui16array = new Uint16Array(TriangleCount * 3);

                    UInt16[] indexArray = (UInt16[])(object)ui16array;
                    switch (i)
                    {
                        case 0:
                            indexArray[index++] = 7;
                            indexArray[index++] = 4;
                            indexArray[index++] = 6;
                            indexArray[index++] = 4;
                            indexArray[index++] = 3;
                            indexArray[index++] = 6;
                            break;
                        case 1:
                            indexArray[index++] = 6;
                            indexArray[index++] = 5;
                            indexArray[index++] = 8;
                            indexArray[index++] = 6;
                            indexArray[index++] = 3;
                            indexArray[index++] = 5;            
                            break;
                        case 2:
                            indexArray[index++] = 4;
                            indexArray[index++] = 0;
                            indexArray[index++] = 3;
                            indexArray[index++] = 4;
                            indexArray[index++] = 1;
                            indexArray[index++] = 0;
                            break;
                        case 3:
                            indexArray[index++] = 3;
                            indexArray[index++] = 2;
                            indexArray[index++] = 5;
                            indexArray[index++] = 3;
                            indexArray[index++] = 0;
                            indexArray[index++] = 2;
                            break;
                    }
                    IndexBuffers[i] = PrepDevice.createBuffer();
                    PrepDevice.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, IndexBuffers[i]);
                    PrepDevice.bufferData(GL.ELEMENT_ARRAY_BUFFER, ui16array, GL.STATIC_DRAW);   

                }
            }
            return true;
        }

        //public override bool IsTileBigEnough(RenderContext renderContext)
        //{   
        //    //to check this
        //    double arcPixels = ((dataset.BaseTileDegrees / 256) / Math.Pow(2, Level)) * 3600;
        //    return (renderContext.FovScale < arcPixels);
        //}

        public TangentTile()
        {
        }

        public static TangentTile Create(int level, int x, int y, Imageset dataset, Tile parent)
        {
            TangentTile temp = new TangentTile();
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
