using System;
//
//Module : AAMOONILLUMINATEDFRACTION.CPP
//Purpose: Implementation for the algorithms for the Moon's Elongation, Phase Angle and Illuminated Fraction
//Created: PJN / 29-12-2003
//History: PJN / 26-01-2007 1. Changed name of CAAMoonIlluminatedFraction::IluminatedFraction to 
//                          CAAMoonIlluminatedFraction::IlluminatedFraction. Thanks to Ing. Taras Kapuszczak
//                          for reporting this typo!.
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


//////////////////// Includes /////////////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  MIFR // wA S CAAMoonIlluminatedFraction
{
//Static methods

  //////////////////// Implementation ///////////////////////////////////////////
  
  public static double GeocentricElongation(double ObjectAlpha, double ObjectDelta, double SunAlpha, double SunDelta)
  {
	//Convert the RA's to radians
	ObjectAlpha = CT.D2R(ObjectAlpha *15);
	SunAlpha = CT.D2R(SunAlpha *15);
  
	//Convert the declinations to radians
	ObjectDelta = CT.D2R(ObjectDelta);
	SunDelta = CT.D2R(SunDelta);
  
	//Return the result
	return CT.R2D(Math.Acos(Math.Sin(SunDelta)*Math.Sin(ObjectDelta) + Math.Cos(SunDelta)*Math.Cos(ObjectDelta)*Math.Cos(SunAlpha - ObjectAlpha)));
  }
  public static double PhaseAngle(double GeocentricElongation, double EarthObjectDistance, double EarthSunDistance)
  {
	//Convert from degrees to radians
	GeocentricElongation = CT.D2R(GeocentricElongation);
  
	//Return the result
	return CT.M360(CT.R2D(Math.Atan2(EarthSunDistance * Math.Sin(GeocentricElongation), EarthObjectDistance - EarthSunDistance *Math.Cos(GeocentricElongation))));
  }
  public static double IlluminatedFraction(double PhaseAngle)
  {
	//Convert from degrees to radians
	PhaseAngle = CT.D2R(PhaseAngle);
  
	//Return the result
	return (1 + Math.Cos(PhaseAngle)) / 2;
  }
  public static double PositionAngle(double Alpha0, double Delta0, double Alpha, double Delta)
  {
	//Convert to radians
	Alpha0 = CT.H2R(Alpha0);
	Alpha = CT.H2R(Alpha);
	Delta0 = CT.D2R(Delta0);
	Delta = CT.D2R(Delta);
  
	return CT.M360(CT.R2D(Math.Atan2(Math.Cos(Delta0)*Math.Sin(Alpha0 - Alpha), Math.Sin(Delta0)*Math.Cos(Delta) - Math.Cos(Delta0)*Math.Sin(Delta)*Math.Cos(Alpha0 - Alpha))));
  }
}
