


using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    public class SkyImageTile : Tile
    {

        public SkyImageTile()
        {
        }


        public Matrix3d Matrix;

        public void ComputeMatrix()
        {
            Matrix = Matrix3d.Identity;
            Matrix.Multiply(Matrix3d.RotationX((float)(((Rotation)) / 180f * Math.PI)));
            Matrix.Multiply(Matrix3d.RotationZ((float)((LatCenter) / 180f * Math.PI)));
            Matrix.Multiply(Matrix3d.RotationY((float)(((360 - LngCenter)) / 180f * Math.PI)));
        }

        public double PixelCenterX = 0.0;
        public double PixelCenterY = 0.0;
        public double LatCenter = 0.0;
        public double LngCenter = 0.0;
        public double Rotation = 0.0;
        public double ScaleX = .01;
        public double ScaleY = .01;
        public double Height = 0;
        public double Width = 0;
        public static SkyImageTile Create(int level, int x, int y, Imageset dataset, Tile parent)
        {
            SkyImageTile temp = new SkyImageTile();
            temp.Parent = parent;
            temp.Level = level;
            temp.tileX = x;
            temp.tileY = y;
            temp.dataset = dataset;
            temp.GetParameters();
            temp.ComputeMatrix();
            temp.sphereCenter = temp.GeoTo3dTan(0, 0);
            temp.radius = 1.25f;
            return temp;
        }

        private void GetParameters()
        {
            PixelCenterX = dataset.OffsetX;
            PixelCenterY = dataset.OffsetY;
            LatCenter = dataset.CenterY;
            LngCenter = dataset.CenterX;
            Rotation = dataset.Rotation;
            ScaleX = -(ScaleY = dataset.BaseTileDegrees);
            if (dataset.BottomsUp)
            {
                ScaleX = -ScaleX;
                Rotation = 360 - Rotation;
            }

        }

        protected Vector3d GeoTo3dTan(double lat, double lng)
        {
            lng = -lng;
            double fac1 = (this.dataset.BaseTileDegrees );
            //double fac1 = (this.dataset.BaseTileDegrees ) / 2;
            double factor = Math.Tan(fac1 * RC);

            //return (Vector3d)dataset.Matrix.Transform(Vector3d.Create(1, (lat / fac1 * factor), (lng / fac1 * factor)));
            return (Vector3d)Matrix.Transform(Vector3d.Create(1, (lat / fac1 * factor), (lng / fac1 * factor)));

        }




        //protected void ComputeBoundingSphereBottomsUp()
        //{
        //    double tileDegrees = (double)this.dataset.BaseTileDegrees / ((double)Math.Pow(2, this.Level));


        //    double latMin = ((double)this.dataset.BaseTileDegrees / 2 + (((double)(this.tileY + 1)) * tileDegrees)) + dataset.OffsetY;
        //    double latMax = ((double)this.dataset.BaseTileDegrees / 2 + (((double)this.tileY) * tileDegrees)) + dataset.OffsetY;
        //    double lngMin = (((double)this.tileX * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;
        //    double lngMax = ((((double)(this.tileX + 1)) * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;

        //    double latCenter = (latMin + latMax) / 2.0;
        //    double lngCenter = (lngMin + lngMax) / 2.0;


        //    TopLeft = GeoTo3dTan(latMin, lngMin);
        //    BottomRight = GeoTo3dTan(latMax, lngMax);
        //    TopRight = GeoTo3dTan(latMin, lngMax);
        //    BottomLeft = GeoTo3dTan(latMax, lngMin);
        //    Vector3d distVect = TopLeft;
        //    tileDegrees = lngMax - lngMin;
        //}


        List<PositionTexture> vertexList = null;
        List<Triangle> childTriangleList = null;

        public override bool CreateGeometry(RenderContext renderContext)
        {
            base.CreateGeometry(renderContext);

            if (GeometryCreated)
            {
                return true;
            }

            Bitmap bmp = null;

            if (dataset.WcsImage != null)
            {
                WcsImage wcsImage = dataset.WcsImage as WcsImage;
                bmp = wcsImage.GetBitmap();
                texture2d = bmp.GetTexture();

                if (bmp.Height != wcsImage.SizeY)
                {
                    PixelCenterY += bmp.Height - wcsImage.SizeY;
                }
            }
            
            GeometryCreated = true;

            for (int i = 0; i < 4; i++)
            {
                RenderTriangleLists[i] = new List<RenderTriangle>();
            }


            ComputeMatrix();

            if (bmp != null && renderContext.gl != null)
            {
                Height = bmp.Height;
                Width = bmp.Width;
            }
            else
            {
                Height = texture.NaturalHeight;
                Width = texture.NaturalWidth;
            }
            double latMin = 0 + (ScaleY * (Height - PixelCenterY));
            double latMax = 0 - (ScaleY * PixelCenterY);
            double lngMin = 0 + (ScaleX * PixelCenterX);
            double lngMax = 0 - (ScaleX * (Width - PixelCenterX));


            TopLeft = GeoTo3dTan(latMin, lngMin);
            BottomRight = GeoTo3dTan(latMax, lngMax);
            TopRight = GeoTo3dTan(latMin, lngMax);
            BottomLeft = GeoTo3dTan(latMax, lngMin);



            Vector3d topCenter = Vector3d.Lerp(TopLeft, TopRight, .5f);
            Vector3d bottomCenter = Vector3d.Lerp(BottomLeft, BottomRight, .5f);
            Vector3d center = Vector3d.Lerp(topCenter, bottomCenter, .5f);
            Vector3d rightCenter = Vector3d.Lerp(TopRight, BottomRight, .5f);
            Vector3d leftCenter = Vector3d.Lerp(TopLeft, BottomLeft, .5f);


            if (renderContext.gl == null)
            {
                 vertexList = new List<PositionTexture>();

                vertexList.Add(PositionTexture.CreatePosSize(TopLeft, 0, 0, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(TopRight, 1, 0, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(BottomLeft, 0, 1, Width, Height));
                vertexList.Add(PositionTexture.CreatePosSize(BottomRight, 1, 1, Width, Height));

                childTriangleList = new List<Triangle>();

                if (dataset.BottomsUp)
                {
                    childTriangleList.Add(Triangle.Create(0, 1, 2));
                    childTriangleList.Add(Triangle.Create(2, 1, 3));
                }
                else
                {
                    childTriangleList.Add(Triangle.Create(0, 2, 1));
                    childTriangleList.Add(Triangle.Create(2, 3, 1));

                }

                int count = 3;
                while (count-- > 1)
                {
                    List<Triangle> newList = new List<Triangle>();
                    foreach (Triangle tri in childTriangleList)
                    {
                        tri.SubDivide(newList, vertexList);
                    }
                    childTriangleList = newList;
                }

                double miter = .6 / (Width/256);
                foreach (Triangle tri in childTriangleList)
                {
                    PositionTexture p1 = vertexList[tri.A];
                    PositionTexture p2 = vertexList[tri.B];
                    PositionTexture p3 = vertexList[tri.C];


                    RenderTriangleLists[0].Add(RenderTriangle.CreateWithMiter(p1, p2, p3, texture, Level, miter));
                }

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




    }
}
