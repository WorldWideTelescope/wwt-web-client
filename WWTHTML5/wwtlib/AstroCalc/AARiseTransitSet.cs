using System;
//
//Module : AARISETRANSITSET.CPP
//Purpose: Implementation for the algorithms which obtain the Rise, Transit and Set times
//Created: PJN / 29-12-2003
//History: PJN / 15-10-2004 1. bValid variable is now correctly set in CAARiseTransitSet::Rise if the
//                          objects does actually rise and sets
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


///////////////////////////// Includes ////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAARiseTransitSetDetails
{
//Constructors / Destructors
  public CAARiseTransitSetDetails()
  {
	  bValid = false;
	  Rise = 0;
	  Transit = 0;
	  Set = 0;
  }

//Member variables
  public bool bValid;
  public double Rise;
  public double Transit;
  public double Set;
}

public class  CAARiseTransitSet
{
//Static methods

  ///////////////////////////// Implementation //////////////////////////////////
  
  public static CAARiseTransitSetDetails Rise(double JD, double Alpha1, double Delta1, double Alpha2, double Delta2, double Alpha3, double Delta3, double Longitude, double Latitude, double h0)
  {
	//What will be the return value
	CAARiseTransitSetDetails details = new CAARiseTransitSetDetails();
	details.bValid = false;
  
	//Calculate the sidereal time
	double theta0 = CAASidereal.ApparentGreenwichSiderealTime(JD);
	theta0 *= 15; //Express it as degrees
  
	//Calculate deltat
	double deltaT = DYT.DeltaT(JD);
  
	//Convert values to radians
	double Delta2Rad = CT.D2R(Delta2);
	double LatitudeRad = CT.D2R(Latitude);
  
	//Convert the standard latitude to radians
	double h0Rad = CT.D2R(h0);
  
	double cosH0 = (Math.Sin(h0Rad) - Math.Sin(LatitudeRad)*Math.Sin(Delta2Rad)) / (Math.Cos(LatitudeRad) * Math.Cos(Delta2Rad));
  
	//Check that the object actually rises
	if ((cosH0 > 1) || (cosH0 < -1))
	  return details;
  
	double H0 = Math.Acos(cosH0);
	H0 = CT.R2D(H0);
  
	double M0 = (Alpha2 *15 + Longitude - theta0) / 360;
	double M1 = M0 - H0/360;
	double M2 = M0 + H0/360;
  
	if (M0 > 1)
	  M0 -= 1;
	else if (M0 < 0)
	  M0 += 1;
  
	if (M1 > 1)
	  M1 -= 1;
	else if (M1 < 0)
	  M1 += 1;
  
	if (M2 > 1)
	  M2 -= 1;
	else if (M2 < 0)
	  M2 += 1;
  
	for (int i =0; i<2; i++)
	{
	  //Calculate the details of rising
  
	  double theta1 = theta0 + 360.985647 *M1;
	  theta1 = CT.M360(theta1);
  
	  double n = M1 + deltaT/86400;
  
	  double Alpha = INTP.Interpolate(n, Alpha1, Alpha2, Alpha3);
	  double Delta = INTP.Interpolate(n, Delta1, Delta2, Delta3);
  
	  double H = theta1 - Longitude - Alpha *15;
	  COR Horizontal = CT.Eq2H(H/15, Delta, Latitude);
  
	  double DeltaM = (Horizontal.Y - h0) / (360 *Math.Cos(CT.D2R(Delta))*Math.Cos(LatitudeRad)*Math.Sin(CT.D2R(H)));
	  M1 += DeltaM;
  
  
	  //Calculate the details of transit
  
	  theta1 = theta0 + 360.985647 *M0;
	  theta1 = CT.M360(theta1);
  
	  n = M0 + deltaT/86400;
  
	  Alpha = INTP.Interpolate(n, Alpha1, Alpha2, Alpha3);
  
	  H = theta1 - Longitude - Alpha *15;
  
	  if (H < -180)
	  {
		  H+=360;
	  }
  
	  DeltaM = -H / 360;
	  M0 += DeltaM;
  
  
	  //Calculate the details of setting
  
	  theta1 = theta0 + 360.985647 *M2;
	  theta1 = CT.M360(theta1);
  
	  n = M2 + deltaT/86400;
  
	  Alpha = INTP.Interpolate(n, Alpha1, Alpha2, Alpha3);
	  Delta = INTP.Interpolate(n, Delta1, Delta2, Delta3);
  
	  H = theta1 - Longitude - Alpha *15;
	  Horizontal = CT.Eq2H(H/15, Delta, Latitude);
  
	  DeltaM = (Horizontal.Y - h0) / (360 *Math.Cos(CT.D2R(Delta))*Math.Cos(LatitudeRad)*Math.Sin(CT.D2R(H)));
	  M2 += DeltaM;
	}
  
	//Finally before we exit, convert to hours
	details.bValid = true;
	details.Rise = M1 * 24;
	details.Set = M2 * 24;
	details.Transit = M0 * 24;
  
	return details;
  }
}
