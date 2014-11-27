using System;
using System.Collections.Generic;


namespace wwtlib
{
    public class Tessellator
    {
        public static List<int> TesselateSimplePoly(List<Vector3d> inputList)
        {
            List<int> results = new List<int>();

            Tessellator tess = new Tessellator();

            tess.Process(inputList, results);

            return results;
        }


        private bool IsLeftOfHalfSpace(Vector3d pntA, Vector3d pntB, Vector3d pntTest)
        {
            pntA.Normalize();
            pntB.Normalize();
            Vector3d cross = Vector3d.Cross(pntA, pntB);

            double dot = Vector3d.Dot(cross, pntTest);

            return dot > 0;
        }

        private bool InsideTriangle(Vector3d pntA, Vector3d pntB, Vector3d pntC, Vector3d pntTest)
        {
            if (!IsLeftOfHalfSpace(pntA, pntB, pntTest))
            {
                return false;
            }
            if (!IsLeftOfHalfSpace(pntB, pntC, pntTest))
            {
                return false;
            }
            if (!IsLeftOfHalfSpace(pntC, pntA, pntTest))
            {
                return false;
            }

            return true;
        }


        bool CanClipEar(List<Vector3d> poly, int u, int v, int w, int n, int[] verts)
        {
            int p;

            Vector3d a = poly[verts[u]].Copy();
            Vector3d b = poly[verts[v]].Copy();
            Vector3d c = poly[verts[w]].Copy();
            Vector3d P;

            Vector3d d = Vector3d.SubtractVectors(b, a);
            d.Normalize();
            Vector3d e = Vector3d.SubtractVectors(b , c);
            e.Normalize();

            Vector3d g = Vector3d.Cross(d, e);

            Vector3d bn = b.Copy();
            bn.Normalize();

            //Determin if convex edge
            if (Vector3d.Dot(g, bn) > 0)
            {
                return false;
            }


            // Check for any interecting vertexies that would invalidate this ear
            for (p = 0; p < n; p++)
            {
                if ((p == u) || (p == v) || (p == w))
                {
                    continue;
                }

                P = poly[verts[p]].Copy();

                // don;t Clip earth if  other intersecting vertex
                if (InsideTriangle(a, b, c, P))
                {
                    return false;
                }
            }

            return true;
        }


        public bool Process(List<Vector3d> poly, List<int> result)
        {
 
            int n = poly.Count;
            if (poly.Count < 3)
            {
                return false;
            }

            int[] verts = new int[poly.Count];

  
            for (int i = 0; i < n; i++)
            {
                verts[i] = i;
            }
          
            int nv = n;

           
            int count = 2 * nv;   

            for (int m = 0, v = nv - 1; nv > 2; )
            {
                
                if (0 >= (count--))
                {
                    // not enough ears to clip. Non-Simple Polygon
                    return false;
                }

                
                int u = v;
                if (nv <= u)
                {
                    u = 0;     
                }

                v = u + 1;
                if (nv <= v)
                {
                    v = 0;    
                }

                int w = v + 1;
                if (nv <= w)
                {
                    w = 0;     
                }

                if (CanClipEar(poly, u, v, w, nv, verts))
                {
                    int s, t;

                    result.Add(verts[u]);
                    result.Add(verts[v]);
                    result.Add(verts[w]);

                    m++;

                    // remove clipped ear
                    for (s = v, t = v + 1; t < nv; s++, t++)
                    {
                        verts[s] = verts[t];
                    }
                    nv--;

                    count = 2 * nv;
                }
            }
            return true;
        }
    }
}
