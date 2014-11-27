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

        public string ID;
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



        public Star(string input)
        {

            string[] sa = input.Split(',');


            ID = sa[0];
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

    }
}