
using System;
using System.Diagnostics;
//
//Module : AACOORDINATETRANSFORMATION.CPP
//Purpose: Implementation for the algorithms which convert between the various celestial coordinate systems
//Created: PJN / 29-12-2003
//History: PJN / 14-02-2004 1. Fixed a "minus zero" bug in the function CAACoordinateTransformation::DMSToDegrees.
//                          The sign of the value is now taken explicitly from the new bPositive boolean
//                          parameter. Thanks to Patrick Wallace for reporting this problem.
//         PJN / 02-06-2005 1. Most of the angular conversion functions have now been reimplemented as simply
//                          numeric constants. All of the AA+ code has also been updated to use these new constants.
//         PJN / 25-01-2007 1. Fixed a minor compliance issue with GCC in the AACoordinateTransformation.h to do
//                          with the declaration of various methods. Thanks to Mathieu Peyréga for reporting this
//                          issue.
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


//////////////////////// Includes /////////////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////
public class COR
{
    public COR()
    {
        X = 0;
        Y = 0;
    }
    public static COR Create(double x, double y)
    {
        COR item = new COR();
        item.X = x;
        item.Y = y;
        return item;
    }

    //Member variables
    public double X;
    public double Y;
}
public class C3D // was CAA3DCoordinate
{
    public C3D()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }
    public static C3D Create(double x, double y, double z)
    {
        C3D item = new C3D();

        item.X = x;
        item.Y = y;
        item.Z = z;
        return item;
    }

    //member variables
    public double X;
    public double Y;
    public double Z;
}

// 
// CAACoordinateTransformation becomes CT

public class  CT
{
//Conversion functions

  /////////////////////// Implementation ////////////////////////////////////////

    public static COR Eq2Ec(double Alpha, double Delta, double Epsilon) // was Equatorial2Ecliptic
  {
	Alpha = H2R(Alpha);
	Delta = D2R(Delta);
	Epsilon = D2R(Epsilon);
  
	COR Ecliptic = new COR();
	Ecliptic.X = R2D(Math.Atan2(Math.Sin(Alpha)*Math.Cos(Epsilon) + Math.Tan(Delta)*Math.Sin(Epsilon), Math.Cos(Alpha)));
	if (Ecliptic.X < 0)
	  Ecliptic.X += 360;
	Ecliptic.Y = R2D(Math.Asin(Math.Sin(Delta)*Math.Cos(Epsilon) - Math.Cos(Delta)*Math.Sin(Epsilon)*Math.Sin(Alpha)));
  
	return Ecliptic;
  }
  public static COR Ec2Eq(double Lambda, double Beta, double Epsilon) // was Ecliptic2Equatorial
	{
	  Lambda = D2R(Lambda);
	  Beta = D2R(Beta);
	  Epsilon = D2R(Epsilon);
	
	  COR Equatorial = new COR();
	  Equatorial.X = R2H(Math.Atan2(Math.Sin(Lambda)*Math.Cos(Epsilon) - Math.Tan(Beta)*Math.Sin(Epsilon), Math.Cos(Lambda)));
	  if (Equatorial.X < 0)
		Equatorial.X += 24;
	  Equatorial.Y = R2D(Math.Asin(Math.Sin(Beta)*Math.Cos(Epsilon) + Math.Cos(Beta)*Math.Sin(Epsilon)*Math.Sin(Lambda)));
	
		return Equatorial;
	}
    public static COR Eq2H(double LocalHourAngle, double Delta, double Latitude) // was Equatorial2Horizontal
	{
	  LocalHourAngle = H2R(LocalHourAngle);
	  Delta = D2R(Delta);
	  Latitude = D2R(Latitude);
	
	  COR Horizontal = new COR();
	  Horizontal.X = R2D(Math.Atan2(Math.Sin(LocalHourAngle), Math.Cos(LocalHourAngle)*Math.Sin(Latitude) - Math.Tan(Delta)*Math.Cos(Latitude)));
	  if (Horizontal.X < 0)
		Horizontal.X += 360;
	  Horizontal.Y = R2D(Math.Asin(Math.Sin(Latitude)*Math.Sin(Delta) + Math.Cos(Latitude)*Math.Cos(Delta)*Math.Cos(LocalHourAngle)));
	
		return Horizontal;
	}
    public static COR H2Eq(double Azimuth, double Altitude, double Latitude) // was Horizontal2Equatorial
	{
	  //Convert from degress to radians
	  Azimuth = D2R(Azimuth);
	  Altitude = D2R(Altitude);
	  Latitude = D2R(Latitude);
	
	  COR Equatorial = new COR();
	  Equatorial.X = R2H(Math.Atan2(Math.Sin(Azimuth), Math.Cos(Azimuth)*Math.Sin(Latitude) + Math.Tan(Altitude)*Math.Cos(Latitude)));
	  if (Equatorial.X < 0)
		Equatorial.X += 24;
	  Equatorial.Y = R2D(Math.Asin(Math.Sin(Latitude)*Math.Sin(Altitude) - Math.Cos(Latitude)*Math.Cos(Altitude)*Math.Cos(Azimuth)));
	
		return Equatorial;
	}
    public static COR Eq2G(double Alpha, double Delta) // was Equatorial2Galactic
	{
	  Alpha = 192.25 - H2D(Alpha);
	  Alpha = D2R(Alpha);
	  Delta = D2R(Delta);
	
	  COR Galactic = new COR();
	  Galactic.X = R2D(Math.Atan2(Math.Sin(Alpha), Math.Cos(Alpha)*Math.Sin(D2R(27.4)) - Math.Tan(Delta)*Math.Cos(D2R(27.4))));
	  Galactic.X = 303 - Galactic.X;
	  if (Galactic.X >= 360)
		Galactic.X -= 360;
	  Galactic.Y = R2D(Math.Asin(Math.Sin(Delta)*Math.Sin(D2R(27.4)) + Math.Cos(Delta)*Math.Cos(D2R(27.4))*Math.Cos(Alpha)));
	
		return Galactic;
	}
    public static COR G2Eq(double l, double b) // was Galactic2Equatorial
	{
	  l -= 123;
	  l = D2R(l);
	  b = D2R(b);
	
	  COR Equatorial = new COR();
	  Equatorial.X = R2D(Math.Atan2(Math.Sin(l), Math.Cos(l)*Math.Sin(D2R(27.4)) - Math.Tan(b)*Math.Cos(D2R(27.4))));
	  Equatorial.X += 12.25;
	  if (Equatorial.X < 0)
		Equatorial.X += 360;
	  Equatorial.X = D2H(Equatorial.X);
	  Equatorial.Y = R2D(Math.Asin(Math.Sin(b)*Math.Sin(D2R(27.4)) + Math.Cos(b)*Math.Cos(D2R(27.4))*Math.Cos(l)));
	
		return Equatorial;
	}

//Inlined functions
  public static double D2R(double Degrees) // was DegreesToRadians
  {
	return Degrees * 0.017453292519943295769236907684886;
  }

  public static double R2D(double Radians) // was RadiansToDegrees
  {
	return Radians * 57.295779513082320876798154814105;
  }

  public static double R2H(double Radians)// was RadiansToHours
  {
	return Radians * 3.8197186342054880584532103209403;
  }

  public static double H2R(double Hours)// was HoursToRadians
  {
	return Hours * 0.26179938779914943653855361527329;
  }

  public static double H2D(double Hours)// was HoursToDegrees
  {
	return Hours * 15;
  }

  public static double D2H(double Degrees)// was DegreesToHours
  {
	return Degrees / 15;
  }

  public static double PI()
  {
	return 3.1415926535897932384626433832795;
  }

  public static double M360(double Degrees)// was MapTo0To360Range
  {
      return Degrees - Math.Floor(Degrees / 360.0) * 360.0;
  }

  public static double M24(double HourAngle)// was MapTo0To24Range
  {
      return HourAngle - Math.Floor(HourAngle / 24.0) * 24.0;
  }

  public static double DMS2D(double Degrees, double Minutes, double Seconds)// was DMSToDegrees
  {
	  return DMS2Dp(Degrees, Minutes, Seconds, true);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double DMSToDegrees(double Degrees, double Minutes, double Seconds, bool bPositive = true)
  public static double DMS2Dp(double Degrees, double Minutes, double Seconds, bool bPositive)// was DMSToDegreesPos
  {
	//validate our parameters
	if (!bPositive)
	{
	  Debug.Assert(Degrees >= 0); //All parameters should be non negative if the "bPositive" parameter is false
	  Debug.Assert(Minutes >= 0);
	  Debug.Assert(Seconds >= 0);
	}
  
	if (bPositive)
	  return Degrees + Minutes/60 + Seconds/3600;
	else
	  return -Degrees - Minutes/60 - Seconds/3600;
  }
}
