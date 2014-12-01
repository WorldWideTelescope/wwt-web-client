using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    class Triangle
    {
        // Vertex Indexies
        public int A;
        public int B;
        public int C;

        public static Triangle Create(int a, int b, int c)
        {
            Triangle temp = new Triangle();
            temp.A = a;
            temp.B = b;
            temp.C = c;
            return temp;
        }

        public Triangle()
        {
            A = -1;
            B = -1;
            C = -1;
        }

        public void SubDivide(List<Triangle> triList, List<PositionTexture> vertexList)
        {
            Vector3d a1 = Vector3d.Lerp(vertexList[B].Position, vertexList[C].Position, .5f);
            Vector3d b1 = Vector3d.Lerp(vertexList[C].Position, vertexList[A].Position, .5f);
            Vector3d c1 = Vector3d.Lerp(vertexList[A].Position, vertexList[B].Position, .5f);

            Vector2d a1uv = Vector2d.Lerp(Vector2d.Create(vertexList[B].Tu, vertexList[B].Tv), Vector2d.Create(vertexList[C].Tu, vertexList[C].Tv), .5f);
            Vector2d b1uv = Vector2d.Lerp(Vector2d.Create(vertexList[C].Tu, vertexList[C].Tv), Vector2d.Create(vertexList[A].Tu, vertexList[A].Tv), .5f);
            Vector2d c1uv = Vector2d.Lerp(Vector2d.Create(vertexList[A].Tu, vertexList[A].Tv), Vector2d.Create(vertexList[B].Tu, vertexList[B].Tv), .5f);

            a1.Normalize();
            b1.Normalize();
            c1.Normalize();

            int aIndex = vertexList.Count;
            int bIndex = vertexList.Count + 1;
            int cIndex = vertexList.Count + 2;

            vertexList.Add(PositionTexture.CreatePosRaw(a1, a1uv.X, a1uv.Y));
            vertexList.Add(PositionTexture.CreatePosRaw(b1, b1uv.X, b1uv.Y));
            vertexList.Add(PositionTexture.CreatePosRaw(c1, c1uv.X, c1uv.Y)); 

            triList.Add(Triangle.Create(A, cIndex, bIndex));
            triList.Add(Triangle.Create(B, aIndex, cIndex));
            triList.Add(Triangle.Create(C, bIndex, aIndex));
            triList.Add(Triangle.Create(aIndex, bIndex, cIndex));
        }

        public void SubDivideNoNormalize(List<Triangle> triList, List<PositionTexture> vertexList)
        {
            Vector3d a1 = Vector3d.Lerp(vertexList[B].Position, vertexList[C].Position, .5f);
            Vector3d b1 = Vector3d.Lerp(vertexList[C].Position, vertexList[A].Position, .5f);
            Vector3d c1 = Vector3d.Lerp(vertexList[A].Position, vertexList[B].Position, .5f);

            Vector2d a1uv = Vector2d.Lerp(Vector2d.Create(vertexList[B].Tu, vertexList[B].Tv), Vector2d.Create(vertexList[C].Tu, vertexList[C].Tv), .5f);
            Vector2d b1uv = Vector2d.Lerp(Vector2d.Create(vertexList[C].Tu, vertexList[C].Tv), Vector2d.Create(vertexList[A].Tu, vertexList[A].Tv), .5f);
            Vector2d c1uv = Vector2d.Lerp(Vector2d.Create(vertexList[A].Tu, vertexList[A].Tv), Vector2d.Create(vertexList[B].Tu, vertexList[B].Tv), .5f);

            //a1.Normalize();
            //b1.Normalize();
            //c1.Normalize();

            int aIndex = vertexList.Count;
            int bIndex = vertexList.Count + 1;
            int cIndex = vertexList.Count + 2;

            vertexList.Add(PositionTexture.CreatePosRaw(a1, a1uv.X, a1uv.Y));
            vertexList.Add(PositionTexture.CreatePosRaw(b1, b1uv.X, b1uv.Y));
            vertexList.Add(PositionTexture.CreatePosRaw(c1, c1uv.X, c1uv.Y));

            triList.Add(Triangle.Create(A, cIndex, bIndex));
            triList.Add(Triangle.Create(B, aIndex, cIndex));
            triList.Add(Triangle.Create(C, bIndex, aIndex));
            triList.Add(Triangle.Create(aIndex, bIndex, cIndex));
        }
    }
}

