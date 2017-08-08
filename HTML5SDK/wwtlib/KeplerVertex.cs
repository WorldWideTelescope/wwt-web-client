using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;



namespace wwtlib
{


    public class KeplerVertex
    {

        public Vector3d ABC = new Vector3d();
        public Vector3d abc1 = new Vector3d();
        public float PointSize;
        public Color Color;

        public float w;
        public float e;
        public float n;
        public float T;
        public float a;
        public float z;
        public float orbitPos;
        public float orbits;
 
        static double sine = .0;
        static double cose = 1;
        static double degrad = Math.PI / 180;

             

        public static int baseDate = (int)SpaceTimeController.UtcToJulian(Date.Now);
        public void Fill(EOE ee)
        {
            double F = Math.Cos(ee.omega * degrad);
            double sinOmega = Math.Sin(ee.omega * degrad);
            double cosi = Math.Cos(ee.i * degrad);
            double sini = Math.Sin(ee.i * degrad);
            double G = sinOmega * cose;
            double H = sinOmega * sine;
            double P = -sinOmega * cosi;
            double Q = (F * cosi * cose) - (sini * sine);
            double R = (F * cosi * sine) + (sini * cose);

            double checkA = (F * F) + (G * G) + (H * H);// Should be 1.0
            double checkB = (P * P) + (Q * Q) + (R * R); // should be 1.0 as well

            ABC.X = (float)Math.Atan2(F, P);
            ABC.Y = (float)Math.Atan2(G, Q);
            ABC.Z = (float)Math.Atan2(H, R);

            abc1.X = (float)Math.Sqrt((F * F) + (P * P));
            abc1.Y = (float)Math.Sqrt((G * G) + (Q * Q));
            abc1.Z = (float)Math.Sqrt((H * H) + (R * R));

            PointSize = .1f;
            if (ee.a < 2.5)
            {
                Color = Colors.White;
            }
            else if (ee.a < 2.83)
            {
                Color = Colors.Red;
            }
            else if (ee.a < 2.96)
            {
                Color = Colors.Green;
            }
            else if (ee.a < 3.3)
            {
                Color = Colors.Magenta;
            }
            else if (ee.a < 5)
            {
                Color = Colors.Cyan;
            }
            else if (ee.a < 10)
            {
                Color = Colors.Yellow;
                PointSize = .9f;
            }
            else
            {
                Color = Colors.White;
                PointSize = 8f;
            }
            w = (float)ee.w;
            e = (float)ee.e;
            if (ee.n == 0)
            {
                n = (float)((0.9856076686 / (ee.a * Math.Sqrt(ee.a))));
            }
            else
            {
                n = (float)ee.n;
            }
            T = (float)(ee.T - baseDate);
            a = (float)ee.a;
            z = 0;

            orbitPos = 0;
            orbits = 0;

        }
    }
}