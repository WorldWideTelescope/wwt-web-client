using System;
using System.Diagnostics;
using System.IO;
using wwtlib;
//using TerraViewer;
//
//Module : AAELLIPTICAL.CPP
//Purpose: Implementation for the algorithms for an elliptical orbit
//Created: PJN / 29-12-2003
//History: PJN / 24-05-2004 1. Fixed a missing break statement in CAAElliptical::Calculate. Thanks to
//                          Carsten A. Arnholm for reporting this bug. 
//                          2. Also fixed an issue with the calculation of the apparent distance to 
//                          the Sun.
//         PJN / 31-12-2004 1. Fix for CAAElliptical::MinorPlanetMagnitude where the phase angle was
//                          being incorrectly converted from Radians to Degress when it was already
//                          in degrees. Thanks to Martin Burri for reporting this problem.
//         PJN / 05-06-2006 1. Fixed a bug in CAAElliptical::Calculate(double JD, EllipticalObject object)
//                          where the correction for nutation was incorrectly using the Mean obliquity of
//                          the ecliptic instead of the true value. The results from the test program now 
//                          agree much more closely with the example Meeus provides which is the position 
//                          of Venus on 1992 Dec. 20 at 0h Dynamical Time. I've also checked the positions
//                          against the JPL Horizons web site and the agreement is much better. Because the
//                          True obliquity of the Ecliptic is defined as the mean obliquity of the ecliptic
//                          plus the nutation in obliquity, it is relatively easy to determine the magnitude
//                          of error this was causing. From the chapter on Nutation in the book, and 
//                          specifically the table which gives the cosine coefficients for nutation in 
//                          obliquity you can see that the absolute worst case error would be the sum of the 
//                          absolute values of all of the coefficients and would have been c. 10 arc seconds 
//                          of degree, which is not a small amount!. This value would be an absolute worst 
//                          case and I would expect the average error value to be much much smaller 
//                          (probably much less than an arc second). Anyway the bug has now been fixed. 
//                          Thanks to Patrick Wong for pointing out this rather significant bug. 
//
//Copyright (c) 2003 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
//
//All rights reserved.
//
//Copyright / Usage Details:
//
//You are allowed to include the source code in any product (commercial, shareware, freeware or otherwise) 
//when your product is released in binary form. You are allowed to modify the source code in any way you want 
//except you cannot modify the copyright details at the top of each module. If you want to distribute source 
//code with your application, then you are only allowed to distribute versions released by the author. This is 
//to maintain a single distribution point for the source code. 
//
//


////////////////////////////// Includes ///////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class EOE // was CAAEllipticalObjectElements
{
    //Constructors / Destructors
    public EOE()
    {
        a = 0;
        e = 0;
        i = 0;
        w = 0;
        omega = 0;
        JDEquinox = 0;
        T = 0;
    }

    internal static EOE Create(BinaryReader br)
    {
        EOE tmp = new EOE();

        tmp.a = br.ReadSingle();
        tmp.e = br.ReadSingle();
        tmp.i = br.ReadSingle();
        tmp.w = br.ReadSingle();
        tmp.omega = br.ReadSingle();
        tmp.JDEquinox = br.ReadSingle();
        tmp.T = br.ReadSingle();
        return tmp;
    }

    //public CAAEllipticalObjectElements(BinaryReader br)
    //{
    //    a = br.ReadSingle();
    //    e = br.ReadSingle();
    //    i = br.ReadSingle();
    //    w = br.ReadSingle();
    //    omega = br.ReadSingle();
    //    JDEquinox = br.ReadSingle();
    //    T = br.ReadSingle();
    //}

    //public CAAEllipticalObjectElements(string line1, string line2, double gravity)
    //{
    //    JDEquinox = SpaceTimeController.TwoLineDateToJulian(line1.Substring(18,14));
    //    e = double.Parse("0." + line2.Substring(26, 7));
    //    i = double.Parse(line2.Substring(8, 8));
    //    omega = double.Parse(line2.Substring(17, 8));
    //    w = double.Parse(line2.Substring(34, 8));
    //    double revs = double.Parse(line2.Substring(52, 11));
    //    double meanAnomoly = double.Parse(line2.Substring(43, 8));
    //    n = revs * 360.0;
    //    double part =(86400.0/revs)/(Math.PI*2.0);
    //    a = Math.Pow((part*part)*gravity,1.0/3.0);
    //    T = JDEquinox - (meanAnomoly / n);


    //}


    //public void WriteBin(BinaryWriter bw)
    //{
    //    bw.Write((float)a);
    //    bw.Write((float)e);
    //    bw.Write((float)i);
    //    bw.Write((float)w);
    //    bw.Write((float)omega);
    //    bw.Write((float)JDEquinox);
    //    bw.Write((float)T);
    //}
    //member variables
    public double a;
    public double e;
    public double i;
    public double w;
    public double omega;
    public double JDEquinox;
    public double T;
    public double n;

    public double meanAnnomolyOut;

   
}

public class  EPD // was CAAEllipticalPlanetaryDetails
{
//Constructors / Destructors
  public EPD()
  {
	  ApparentGeocentricLongitude = 0;
	  ApparentGeocentricLatitude = 0;
	  ApparentGeocentricDistance = 0;
	  ApparentLightTime = 0;
	  ApparentGeocentricRA = 0;
	  ApparentGeocentricDeclination = 0;
  }

//Member variables
  public double ApparentGeocentricLongitude;
  public double ApparentGeocentricLatitude;
  public double ApparentGeocentricDistance;
  public double ApparentLightTime;
  public double ApparentGeocentricRA;
  public double ApparentGeocentricDeclination;
}

public class  EOD // was CAAEllipticalObjectDetails
{
//Constructors / Destructors
  public EOD()
  {
	  HeliocentricEclipticLongitude = 0;
	  HeliocentricEclipticLatitude = 0;
	  TrueGeocentricRA = 0;
	  TrueGeocentricDeclination = 0;
	  TrueGeocentricDistance = 0;
	  TrueGeocentricLightTime = 0;
	  AstrometricGeocenticRA = 0;
	  AstrometricGeocentricDeclination = 0;
	  AstrometricGeocentricDistance = 0;
	  AstrometricGeocentricLightTime = 0;
	  Elongation = 0;
	  PhaseAngle = 0;
  }

//Member variables
  public C3D HeliocentricRectangularEquatorial = new C3D();
  public C3D HeliocentricRectangularEcliptical = new C3D();
  public double HeliocentricEclipticLongitude;
  public double HeliocentricEclipticLatitude;
  public double TrueGeocentricRA;
  public double TrueGeocentricDeclination;
  public double TrueGeocentricDistance;
  public double TrueGeocentricLightTime;
  public double AstrometricGeocenticRA;
  public double AstrometricGeocentricDeclination;
  public double AstrometricGeocentricDistance;
  public double AstrometricGeocentricLightTime;
  public double Elongation;
  public double PhaseAngle;
}
public enum EO : int // was EllipticalObject
{
    SUN = 0,
    MERCURY = 1,
    VENUS = 2,
    MARS = 3,
    JUPITER = 4,
    SATURN = 5,
    URANUS = 6,
    NEPTUNE = 7,
    PLUTO = 8
}
public class  ELL // was CAAElliptical
{
//Enums


//Static methods
  //Tangible Process Only End
  
  
  ////////////////////////////// Implementation /////////////////////////////////
  
  public static double DistanceToLightTime(double Distance)
  {
	return Distance * 0.0057755183;
  }
  public static EPD Calculate(double JD, EO @object)
  {
	//What will the the return value
	EPD details = new EPD();
  
	double JD0 = JD;
	double L0 = 0;
	double B0 = 0;
	double R0 = 0;
	double cosB0 = 0;
	if (@object != EO.SUN)
	{
	  L0 = CAAEarth.EclipticLongitude(JD0);
	  B0 = CAAEarth.EclipticLatitude(JD0);
	  R0 = CAAEarth.RadiusVector(JD0);
	  L0 = CT.D2R(L0);
	  B0 = CT.D2R(B0);
	  cosB0 = Math.Cos(B0);
	}
  
  
	//Calculate the initial values
	double L = 0;
	double B = 0;
	double R = 0;

    double Lrad;
    double Brad;
    double cosB;
    double cosL;
    double x;
    double y;
    double z;
	bool bRecalc = true;
	bool bFirstRecalc = true;
	double LPrevious = 0;
	double BPrevious = 0;
	double RPrevious = 0;
	while (bRecalc)
	{
	  switch (@object)
	  {
		case EO.SUN:
		{
		  L = CAASun.GeometricEclipticLongitude(JD0);
		  B = CAASun.GeometricEclipticLatitude(JD0);
		  R = CAAEarth.RadiusVector(JD0);
		  break;
		}
		case EO.MERCURY:
		{
		  L = CAAMercury.EclipticLongitude(JD0);
		  B = CAAMercury.EclipticLatitude(JD0);
		  R = CAAMercury.RadiusVector(JD0);
		  break;
		}
		case EO.VENUS:
		{
		  L = CAAVenus.EclipticLongitude(JD0);
		  B = CAAVenus.EclipticLatitude(JD0);
		  R = CAAVenus.RadiusVector(JD0);
		  break;
		}
		case EO.MARS:
		{
		  L = CAAMars.EclipticLongitude(JD0);
		  B = CAAMars.EclipticLatitude(JD0);
		  R = CAAMars.RadiusVector(JD0);
		  break;
		}
		case EO.JUPITER:
		{
		  L = CAAJupiter.EclipticLongitude(JD0);
		  B = CAAJupiter.EclipticLatitude(JD0);
		  R = CAAJupiter.RadiusVector(JD0);
		  break;
		}
		case EO.SATURN:
		{
		  L = CAASaturn.EclipticLongitude(JD0);
		  B = CAASaturn.EclipticLatitude(JD0);
		  R = CAASaturn.RadiusVector(JD0);
		  break;
		}
		case EO.URANUS:
		{
		  L = CAAUranus.EclipticLongitude(JD0);
		  B = CAAUranus.EclipticLatitude(JD0);
		  R = CAAUranus.RadiusVector(JD0);
		  break;
		}
		case EO.NEPTUNE:
		{
		  L = CAANeptune.EclipticLongitude(JD0);
		  B = CAANeptune.EclipticLatitude(JD0);
		  R = CAANeptune.RadiusVector(JD0);
		  break;
		}
		case EO.PLUTO:
		{
		  L = CAAPluto.EclipticLongitude(JD0);
		  B = CAAPluto.EclipticLatitude(JD0);
		  R = CAAPluto.RadiusVector(JD0);
		  break;
		}
		default:
		{
		  Debug.Assert(false);
		  break;
		}
	  }
  
	  if (!bFirstRecalc)
	  {
		bRecalc = ((Math.Abs(L - LPrevious) > 0.00001) || (Math.Abs(B - BPrevious) > 0.00001) || (Math.Abs(R - RPrevious) > 0.000001));
		LPrevious = L;
		BPrevious = B;
		RPrevious = R;
	  }
	  else
		bFirstRecalc = false;
  


	  //Calculate the new value
	  if (bRecalc)
	  {
		double distance = 0;
		if (@object != EO.SUN)
		{
		  Lrad = CT.D2R(L);
		  Brad = CT.D2R(B);
		  cosB = Math.Cos(Brad);
		  cosL = Math.Cos(Lrad);
		  x = R * cosB * cosL - R0 * cosB0 * Math.Cos(L0);
		  y = R * cosB * Math.Sin(Lrad) - R0 * cosB0 * Math.Sin(L0);
		  z = R * Math.Sin(Brad) - R0 * Math.Sin(B0);
		  distance = Math.Sqrt(x *x + y *y + z *z);
		}
		else
		  distance = R; //Distance to the sun from the earth is in fact the radius vector
  
		//Prepare for the next loop around
		JD0 = JD - ELL.DistanceToLightTime(distance);
	  }
	}
  
	Lrad = CT.D2R(L);
	Brad = CT.D2R(B);
	cosB = Math.Cos(Brad);
	cosL = Math.Cos(Lrad);
	x = R * cosB * cosL - R0 * cosB0 * Math.Cos(L0);
	y = R * cosB * Math.Sin(Lrad) - R0 * cosB0 * Math.Sin(L0);
	z = R * Math.Sin(Brad) - R0 * Math.Sin(B0);
	double x2 = x *x;
	double y2 = y *y;
  
	details.ApparentGeocentricLatitude = CT.R2D(Math.Atan2(z, Math.Sqrt(x2 + y2)));
	details.ApparentGeocentricDistance = Math.Sqrt(x2 + y2 + z *z);
	details.ApparentGeocentricLongitude = CT.M360(CT.R2D(Math.Atan2(y, x)));
	details.ApparentLightTime = ELL.DistanceToLightTime(details.ApparentGeocentricDistance);
  
	//Adjust for Aberration
	COR Aberration = ABR.EclipticAberration(details.ApparentGeocentricLongitude, details.ApparentGeocentricLatitude, JD);
	details.ApparentGeocentricLongitude += Aberration.X;
	details.ApparentGeocentricLatitude += Aberration.Y;
  
	//convert to the FK5 system
	double DeltaLong = CAAFK5.CorrectionInLongitude(details.ApparentGeocentricLongitude, details.ApparentGeocentricLatitude, JD);
	details.ApparentGeocentricLatitude += CAAFK5.CorrectionInLatitude(details.ApparentGeocentricLongitude, JD);
	details.ApparentGeocentricLongitude += DeltaLong;
  
	//Correct for nutation
	double NutationInLongitude = CAANutation.NutationInLongitude(JD);
	double Epsilon = CAANutation.TrueObliquityOfEcliptic(JD);
	details.ApparentGeocentricLongitude += CT.DMS2D(0, 0, NutationInLongitude);
  
	//Convert to RA and Dec
	COR ApparentEqu = CT.Ec2Eq(details.ApparentGeocentricLongitude, details.ApparentGeocentricLatitude, Epsilon);
	details.ApparentGeocentricRA = ApparentEqu.X;
	details.ApparentGeocentricDeclination = ApparentEqu.Y;
  
	return details;
  }


  public static double SemiMajorAxisFromPerihelionDistance(double q, double e)
  {
	return q / (1 - e);
  }
  public static double MeanMotionFromSemiMajorAxis(double a)
  {
	return 0.9856076686 / (a * Math.Sqrt(a));
  }


  public static Vector3d CalculateRectangularJD(double JD, EOE elements)
  {
       double JD0 = JD;

      double omega = CT.D2R(elements.omega);
      double w = CT.D2R(elements.w);
      double i = CT.D2R(elements.i);

      double sinEpsilon = 0;
      double cosEpsilon = 1;
      double sinOmega = Math.Sin(omega);
      double cosOmega = Math.Cos(omega);
      double cosi = Math.Cos(i);
      double sini = Math.Sin(i);

      double F = cosOmega;
      double G = sinOmega * cosEpsilon;
      double H = sinOmega * sinEpsilon;
      double P = -sinOmega * cosi;
      double Q = cosOmega * cosi * cosEpsilon - sini * sinEpsilon;
      double R = cosOmega * cosi * sinEpsilon + sini * cosEpsilon;
      double a = Math.Sqrt(F * F + P * P);
      double b = Math.Sqrt(G * G + Q * Q);
      double c = Math.Sqrt(H * H + R * R);
      double A = Math.Atan2(F, P);
      double B = Math.Atan2(G, Q);
      double C = Math.Atan2(H, R);
      //double n = CAAElliptical.MeanMotionFromSemiMajorAxis(elements.a);
     // double n = ;


      double M = elements.n * (JD0 - elements.T);
      elements.meanAnnomolyOut = M;
      double E = CAAKepler.Calculate(M, elements.e);
      E = CT.D2R(E);
      double v = 2 * Math.Atan(Math.Sqrt((1 + elements.e) / (1 - elements.e)) * Math.Tan(E / 2));
      double r = elements.a * (1 - elements.e * Math.Cos(E));
      double x = r * a * Math.Sin(A + w + v);
      double y = r * b * Math.Sin(B + w + v);
      double z = r * c * Math.Sin(C + w + v);

      //elements.meanAnnomolyOut contains the mean annomoly 
      return Vector3d.Create(x, z, y);
  }
  public static Vector3d CalculateRectangular(EOE elements, double meanAnomoly)
  {

//      double JD0 = JD;

      double omega = CT.D2R(elements.omega);
      double w = CT.D2R(elements.w);
      double i = CT.D2R(elements.i);

      double sinEpsilon = 0;
      double cosEpsilon = 1;
      double sinOmega = Math.Sin(omega);
      double cosOmega = Math.Cos(omega);
      double cosi = Math.Cos(i);
      double sini = Math.Sin(i);

      double F = cosOmega;
      double G = sinOmega * cosEpsilon;
      double H = sinOmega * sinEpsilon;
      double P = -sinOmega * cosi;
      double Q = cosOmega * cosi * cosEpsilon - sini * sinEpsilon;
      double R = cosOmega * cosi * sinEpsilon + sini * cosEpsilon;
      double a = Math.Sqrt(F * F + P * P);
      double b = Math.Sqrt(G * G + Q * Q);
      double c = Math.Sqrt(H * H + R * R);
      double A = Math.Atan2(F, P);
      double B = Math.Atan2(G, Q);
      double C = Math.Atan2(H, R);
      double n = elements.n;


      double M = meanAnomoly;
      double E = CAAKepler.Calculate(M, elements.e);
      E = CT.D2R(E);
      double v = 2 * Math.Atan(Math.Sqrt((1 + elements.e) / (1 - elements.e)) * Math.Tan(E / 2));
      double r = elements.a * (1 - elements.e * Math.Cos(E));
      double x = r * a * Math.Sin(A + w + v);
      double y = r * b * Math.Sin(B + w + v);
      double z = r * c * Math.Sin(C + w + v);

      return Vector3d.Create(x, z, y);

  }

  public static EOD CalculateElements(double JD, EOE elements)
  {
	double Epsilon = CAANutation.MeanObliquityOfEcliptic(elements.JDEquinox);
  
	double JD0 = JD;
  
	//What will be the return value
	EOD details = new EOD();
  
	Epsilon = CT.D2R(Epsilon);
	double omega = CT.D2R(elements.omega);
	double w = CT.D2R(elements.w);
	double i = CT.D2R(elements.i);
  
	double sinEpsilon = Math.Sin(Epsilon);
	double cosEpsilon = Math.Cos(Epsilon);
	double sinOmega = Math.Sin(omega);
	double cosOmega = Math.Cos(omega);
	double cosi = Math.Cos(i);
	double sini = Math.Sin(i);
  
	double F = cosOmega;
	double G = sinOmega * cosEpsilon;
	double H = sinOmega * sinEpsilon;
	double P = -sinOmega * cosi;
	double Q = cosOmega *cosi *cosEpsilon - sini *sinEpsilon;
	double R = cosOmega *cosi *sinEpsilon + sini *cosEpsilon;
	double a = Math.Sqrt(F *F + P *P);
	double b = Math.Sqrt(G *G + Q *Q);
	double c = Math.Sqrt(H *H + R *R);
	double A = Math.Atan2(F, P);
	double B = Math.Atan2(G, Q);
	double C = Math.Atan2(H, R);
	double n = ELL.MeanMotionFromSemiMajorAxis(elements.a);
  
	C3D SunCoord = CAASun.EquatorialRectangularCoordinatesAnyEquinox(JD, elements.JDEquinox);
  
	for (int j =0; j<2; j++)
	{
	  double M = n * (JD0 - elements.T);
	  double E = CAAKepler.Calculate(M, elements.e);
	  E = CT.D2R(E);
	  double v = 2 *Math.Atan(Math.Sqrt((1 + elements.e) / (1 - elements.e)) * Math.Tan(E/2));
	  double r = elements.a * (1 - elements.e *Math.Cos(E));
	  double x = r * a * Math.Sin(A + w + v);
	  double y = r * b * Math.Sin(B + w + v);
	  double z = r * c * Math.Sin(C + w + v);
  
	  if (j == 0)
	  {
		details.HeliocentricRectangularEquatorial.X = x;
		details.HeliocentricRectangularEquatorial.Y = y;
		details.HeliocentricRectangularEquatorial.Z = z;
  
		//Calculate the heliocentric ecliptic coordinates also
		double u = omega + v;
		double cosu = Math.Cos(u);
		double sinu = Math.Sin(u);
  
		details.HeliocentricRectangularEcliptical.X = r * (cosOmega *cosu - sinOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Y = r * (sinOmega *cosu + cosOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Z = r *sini *sinu;
  
		details.HeliocentricEclipticLongitude = Math.Atan2(y, x);
		details.HeliocentricEclipticLongitude = CT.M24(CT.R2D(details.HeliocentricEclipticLongitude) / 15);
		details.HeliocentricEclipticLatitude = Math.Asin(z / r);
		details.HeliocentricEclipticLatitude = CT.R2D(details.HeliocentricEclipticLatitude);
	  }
  
	  double psi = SunCoord.X + x;
	  double nu = SunCoord.Y + y;
	  double sigma = SunCoord.Z + z;
  
	  double Alpha = Math.Atan2(nu, psi);
	  Alpha = CT.R2D(Alpha);
	  double Delta = Math.Atan2(sigma, Math.Sqrt(psi *psi + nu *nu));
	  Delta = CT.R2D(Delta);
	  double Distance = Math.Sqrt(psi *psi + nu *nu + sigma *sigma);
  
	  if (j == 0)
	  {
		details.TrueGeocentricRA = CT.M24(Alpha / 15);
		details.TrueGeocentricDeclination = Delta;
		details.TrueGeocentricDistance = Distance;
		details.TrueGeocentricLightTime = DistanceToLightTime(Distance);
	  }
	  else
	  {
		details.AstrometricGeocenticRA = CT.M24(Alpha / 15);
		details.AstrometricGeocentricDeclination = Delta;
		details.AstrometricGeocentricDistance = Distance;
		details.AstrometricGeocentricLightTime = DistanceToLightTime(Distance);
  
		double RES = Math.Sqrt(SunCoord.X *SunCoord.X + SunCoord.Y *SunCoord.Y + SunCoord.Z *SunCoord.Z);
  
		details.Elongation = Math.Acos((RES *RES + Distance *Distance - r *r) / (2 * RES * Distance));
		details.Elongation = CT.R2D(details.Elongation);
  
		details.PhaseAngle = Math.Acos((r *r + Distance *Distance - RES *RES) / (2 * r * Distance));
		details.PhaseAngle = CT.R2D(details.PhaseAngle);
	  }
  
	  if (j == 0) //Prepare for the next loop around
		JD0 = JD - details.TrueGeocentricLightTime;
	}
  
	return details;
  }
  public static double InstantaneousVelocity(double r, double a)
  {
	return 42.1219 * Math.Sqrt((1/r) - (1/(2 *a)));
  }
  public static double VelocityAtPerihelion(double e, double a)
  {
	return 29.7847 / Math.Sqrt(a) * Math.Sqrt((1+e)/(1-e));
  }
  public static double VelocityAtAphelion(double e, double a)
  {
	return 29.7847 / Math.Sqrt(a) * Math.Sqrt((1-e)/(1+e));
  }
  public static double LengthOfEllipse(double e, double a)
  {
	double b = a * Math.Sqrt(1 - e *e);
	return CT.PI() * (3 * (a+b) - Math.Sqrt((a+3 *b)*(3 *a + b)));
  }
  public static double CometMagnitude(double g, double delta, double k, double r)
  {
      return g + 5 * Util.Log10(delta) + k * Util.Log10(r);
  }
  public static double MinorPlanetMagnitude(double H, double delta, double G, double r, double PhaseAngle)
  {
	//Convert from degrees to radians
	PhaseAngle = CT.D2R(PhaseAngle);
  
	double phi1 = Math.Exp(-3.33 *Math.Pow(Math.Tan(PhaseAngle/2), 0.63));
	double phi2 = Math.Exp(-1.87 *Math.Pow(Math.Tan(PhaseAngle/2), 1.22));

    return H + 5 * Util.Log10(r * delta) - 2.5 * Util.Log10((1 - G) * phi1 + G * phi2);
  }
}
