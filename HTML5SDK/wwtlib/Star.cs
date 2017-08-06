using System;
using System.Collections.Generic;
using System.Linq;
using System.Html.Media.Graphics;
using System.Html;
using System.Net;



namespace wwtlib
{
    public class Star
    {
        public double Magnitude;
        public double RA;
        public double Dec;
        public double BMV;
        public string Name
        {
            get
            {
                return "HIP" + ID.ToString();
            }
        }
        public Coordinates Coordinates
        {
            get
            {
                return Coordinates.FromRaDec(RA, Dec);
            }
        }

        public int ID;
        public double AbsoluteMagnitude;
        public double Par;
        public double Distance;
        public Color Col;
        public Vector3d Position;

        public IPlace AsPlace
        {
            get
            {
                Place place = Place.Create(Name, Dec, RA, Classification.Star, Constellations.Containment.FindConstellationForPoint(RA, Dec), ImageSetType.SolarSystem, -1);
                place.Magnitude = Magnitude;
                place.Distance = Distance;
                return place;
            }
        }



        public void Stars(string input, bool newish)
        {

            string[] sa = input.Split('\t');


            ID = Int32.Parse(sa[0]);
            RA = Double.Parse(sa[1])/15;
            Dec = Double.Parse(sa[2]);


            if (sa.Length > 3)
            {
                try
                {
                    Magnitude = Double.Parse(sa[3]);
                }
                catch
                {
                }
            }


            if (sa.Length > 4)
            {
                try
                {
                    Col = Color.Load(sa[4]);
                }
                catch
                {
                }
            }
        }
        public Star(string input)
        {

            string[] sa = input.Split('\t');


            ID = Int32.Parse(sa[0].Replace("HIP", ""));

            Dec = Double.Parse(sa[3]);

            RA = Double.Parse(sa[2]) / 15;


            if (sa.Length > 4)
            {
                try
                {
                    if (sa[4].ToUpperCase() != "NULL" && sa[4] != "")
                    {
                        Magnitude = Double.Parse(sa[4]);
                    }
                }
                catch
                {
                }
            }
            if (sa.Length > 5)
            {
                try
                {
                    BMV = Double.Parse(sa[5]);
                    MakeColor(BMV);

                }
                catch
                {
                }
            }
            if (sa.Length > 6)
            {
                Par = Double.Parse(sa[6]);
                MakeDistanceAndMagnitude();
            }
        }

        private void MakeDistanceAndMagnitude()
        {
            Distance = 1 / (Par / 1000);
            AbsoluteMagnitude = Magnitude - 5 * ((Util.LogN(Distance, 10) - 1));
            //Convert to AU
            Distance *= 206264.806;
        }

        private void MakeColor(double bmv)
        {
            uint c = 0xFFFFFFFF;
            if (bmv <= -0.32) { c = 0xFFA2B8FF; }
            else if (bmv <= -0.31) { c = 0xFFA3B8FF; }
            else if (bmv <= -0.3) { c = 0xFFA4BAFF; }
            else if (bmv <= -0.3) { c = 0xFFA5BAFF; }
            else if (bmv <= -0.28) { c = 0xFFA7BCFF; }
            else if (bmv <= -0.26) { c = 0xFFA9BDFF; }
            else if (bmv <= -0.24) { c = 0xFFABBFFF; }
            else if (bmv <= -0.2) { c = 0xFFAFC2FF; }
            else if (bmv <= -0.16) { c = 0xFFB4C6FF; }
            else if (bmv <= -0.14) { c = 0xFFB6C8FF; }
            else if (bmv <= -0.12) { c = 0xFFB9CAFF; }
            else if (bmv <= -0.09) { c = 0xFFBCCDFF; }
            else if (bmv <= -0.06) { c = 0xFFC1D0FF; }
            else if (bmv <= 0) { c = 0xFFCAD6FF; }
            else if (bmv <= 0.06) { c = 0xFFD2DCFF; }
            else if (bmv <= 0.14) { c = 0xFFDDE4FF; }
            else if (bmv <= 0.19) { c = 0xFFE3E8FF; }
            else if (bmv <= 0.31) { c = 0xFFF2F2FF; }
            else if (bmv <= 0.36) { c = 0xFFF9F6FF; }
            else if (bmv <= 0.43) { c = 0xFFFFF9FC; }
            else if (bmv <= 0.54) { c = 0xFFFFF6F3; }
            else if (bmv <= 0.59) { c = 0xFFFFF3EB; }
            else if (bmv <= 0.63) { c = 0xFFFFF1E7; }
            else if (bmv <= 0.66) { c = 0xFFFFEFE1; }
            else if (bmv <= 0.74) { c = 0xFFFFEEDD; }
            else if (bmv <= 0.82) { c = 0xFFFFEAD5; }
            else if (bmv <= 0.92) { c = 0xFFFFE4C4; }
            else if (bmv <= 1.15) { c = 0xFFFFDFB8; }
            else if (bmv <= 1.3) { c = 0xFFFFDDB4; }
            else if (bmv <= 1.41) { c = 0xFFFFD39D; }
            else if (bmv <= 1.48) { c = 0xFFFFCD91; }
            else if (bmv <= 1.52) { c = 0xFFFFC987; }
            else if (bmv <= 1.55) { c = 0xFFFFC57F; }
            else if (bmv <= 1.56) { c = 0xFFFFC177; }
            else if (bmv <= 1.61) { c = 0xFFFFBD71; }
            else if (bmv <= 1.72) { c = 0xFFFFB866; }
            else if (bmv <= 1.84) { c = 0xFFFFB25B; }
            else if (bmv <= 2) { c = 0xFFFFAD51; }
            Col = Color.FromInt(c);
        }

    }


    public class Galaxy
    {
        public float RA;
        public float Dec;
        public float Distance;
        public float Type;
        public int eTypeBucket;
        public float Size = 5;
        public long SdssID = 0;
        //public Galaxy(string line, bool largeSet)
        //{
        //    line = line.Replace("  ", " ");
        //    string[] values = line.Split(new char[] { '\t', ' ', ',' });
        //    SdssID = Convert.ToInt64(values[0]);
        //    RA = Convert.ToSingle(values[1]) / 15;
        //    Dec = Convert.ToSingle(values[2]);
        //    Distance = Convert.ToSingle(values[3]);
        //    if (largeSet)
        //    {
        //        Type = Convert.ToSingle(values[4]);
        //        eTypeBucket = GetEType(Type);
        //    }
        //    else
        //    {
        //        Size = Convert.ToSingle(values[4]) * 30;
        //        Type = Convert.ToSingle(values[6]);
        //        eTypeBucket = Convert.ToInt32(values[7]);
        //    }

        //}

        //public Galaxy(string line)
        //{
        //    line = line.Replace("  ", " ");
        //    string[] values = line.Split(new char[] { '\t', ' ', ',' });
        //    SdssID = Convert.ToInt64(values[0]);
        //    RA = Convert.ToSingle(values[1]) / 15;
        //    Dec = Convert.ToSingle(values[2]);
        //    Distance = Convert.ToSingle(values[3]);
        //    this.Size = 500;
        //    Type = Convert.ToSingle(values[4]);

        //}

        //public IPlace Place
        //{
        //    get
        //    {
        //        TourPlace place = new TourPlace("SDSS " + SdssID.ToString(), Dec, RA, Classification.Galaxy, Earth3d.MainWindow.ConstellationCheck.FindConstellationForPoint(RA, Dec), ImageSetType.SolarSystem, -1);
        //        place.Magnitude = 0;
        //        place.Distance = (this.Distance * UiTools.AuPerParsec * 1000000.0) / .73;
        //        return place;
        //    }
        //}


        public Galaxy(BinaryReader br)
        {
            SdssID = br.ReadInt64();
            RA = br.ReadSingle();
            Dec = br.ReadSingle();
            Distance = br.ReadSingle();
            //Type = br.ReadSingle();
            eTypeBucket = br.ReadByte();
            Size = br.ReadSingle();
        }

        public Vector3d Position;

        static float[] eTypeBuckets = new float[] { -3, -0.1860f, -0.1680f, -0.1580f, -0.1500f, -0.1430f, -0.1370f, -0.1300f, -0.1230f, -0.1150f, -0.1040f, -0.0890f, -0.0680f, -0.0420f, -0.0110f, 0.0240f, 0.0640f, 0.1110f, 0.1690f, 0.2520f, 3 };
        public static int GetEType(float value)
        {
            int a = 0;
            int b = eTypeBuckets.Length - 1;

            while (b - a > 1)
            {
                int m = (a + b) / 2;
                if (value > eTypeBuckets[m])
                {
                    a = m;
                }
                else
                {
                    b = m;
                }
            }
            return a;
        }
    }

}