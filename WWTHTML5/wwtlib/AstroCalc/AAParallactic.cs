using System;
//
//Module : AAPARALLACTIC.CPP
//Purpose: Implementation for the algorithms which calculate various celestial globe angles
//Created: PJN / 29-12-2003
//History: None
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


////////////////////// Includes ///////////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAParallactic
{
//Static methods

  ////////////////////// Implementation /////////////////////////////////////////
  
  public static double ParallacticAngle(double HourAngle, double Latitude, double delta)
  {
	HourAngle = CT.H2R(HourAngle);
	Latitude = CT.D2R(Latitude);
	delta = CT.D2R(delta);
  
	return CT.R2D(Math.Atan2(Math.Sin(HourAngle), Math.Tan(Latitude)*Math.Cos(delta) - Math.Sin(delta)*Math.Cos(HourAngle)));
  }
  public static double EclipticLongitudeOnHorizon(double LocalSiderealTime, double ObliquityOfEcliptic, double Latitude)
  {
	LocalSiderealTime = CT.H2R(LocalSiderealTime);
	Latitude = CT.D2R(Latitude);
	ObliquityOfEcliptic = CT.D2R(ObliquityOfEcliptic);
  
	double @value = CT.R2D(Math.Atan2(-Math.Cos(LocalSiderealTime), Math.Sin(ObliquityOfEcliptic)*Math.Tan(Latitude) + Math.Cos(ObliquityOfEcliptic)*Math.Sin(LocalSiderealTime)));
	return CT.M360(@value);
  }
  public static double AngleBetweenEclipticAndHorizon(double LocalSiderealTime, double ObliquityOfEcliptic, double Latitude)
  {
	LocalSiderealTime = CT.H2R(LocalSiderealTime);
	Latitude = CT.D2R(Latitude);
	ObliquityOfEcliptic = CT.D2R(ObliquityOfEcliptic);
  
	double @value = CT.R2D(Math.Acos(Math.Cos(ObliquityOfEcliptic)*Math.Sin(Latitude) - Math.Sin(ObliquityOfEcliptic)*Math.Cos(Latitude)*Math.Sin(LocalSiderealTime)));
	return CT.M360(@value);
  }
  public static double AngleBetweenNorthCelestialPoleAndNorthPoleOfEcliptic(double Lambda, double Beta, double ObliquityOfEcliptic)
  {
	Lambda = CT.D2R(Lambda);
	Beta = CT.D2R(Beta);
	ObliquityOfEcliptic = CT.D2R(ObliquityOfEcliptic);
  
	double @value = CT.R2D(Math.Atan2(Math.Cos(Lambda)*Math.Tan(ObliquityOfEcliptic), Math.Sin(Beta)*Math.Sin(Lambda)*Math.Tan(ObliquityOfEcliptic) - Math.Cos(Beta)));
	return CT.M360(@value);
  }
}
