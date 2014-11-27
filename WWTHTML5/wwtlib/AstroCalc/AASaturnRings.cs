using System;
//
//Module : AASATURNRINGS.CPP
//Purpose: Implementation for the algorithms which calculate various parameters related to the Rings of Saturn
//Created: PJN / 08-01-2004
//History: None
//
//Copyright (c) 2004 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
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


///////////////////////////////// Includes ////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAASaturnRingDetails
{
//Constructors / Destructors
  public CAASaturnRingDetails()
  {
	  B = 0;
	  Bdash = 0;
	  P = 0;
	  a = 0;
	  b = 0;
	  DeltaU = 0;
  }

//Member variables
  public double B;
  public double Bdash;
  public double P;
  public double a;
  public double b;
  public double DeltaU;
}

public class  CAASaturnRings
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAASaturnRingDetails Calculate(double JD)
  {
	//What will be the return value
	CAASaturnRingDetails details = new CAASaturnRingDetails();
  
	double T = (JD - 2451545) / 36525;
	double T2 = T *T;
  
	//Step 1. Calculate the inclination of the plane of the ring and the longitude of the ascending node referred to the ecliptic and mean equinox of the date
	double i = 28.075216 - 0.012998 *T + 0.000004 *T2;
	double irad = CT.D2R(i);
	double omega = 169.508470 + 1.394681 *T + 0.000412 *T2;
	double omegarad = CT.D2R(omega);
  
	//Step 2. Calculate the heliocentric longitude, latitude and radius vector of the Earth in the FK5 system
	double l0 = CAAEarth.EclipticLongitude(JD);
	double b0 = CAAEarth.EclipticLatitude(JD);
	l0 += CAAFK5.CorrectionInLongitude(l0, b0, JD);
	double l0rad = CT.D2R(l0);
	b0 += CAAFK5.CorrectionInLatitude(l0, JD);
	double b0rad = CT.D2R(b0);
	double R = CAAEarth.RadiusVector(JD);
  
	//Step 3. Calculate the corresponding coordinates l,b,r for Saturn but for the instance t-lightraveltime
	double DELTA = 9;
	double PreviousEarthLightTravelTime = 0;
	double EarthLightTravelTime = ELL.DistanceToLightTime(DELTA);
	double JD1 = JD - EarthLightTravelTime;
	bool bIterate = true;
	double x = 0;
	double y = 0;
	double z = 0;
	double l = 0;
	double b = 0;
	double r = 0;
	while (bIterate)
	{
	  //Calculate the position of Saturn
	  l = CAASaturn.EclipticLongitude(JD1);
	  b = CAASaturn.EclipticLatitude(JD1);
	  l += CAAFK5.CorrectionInLongitude(l, b, JD1);
	  b += CAAFK5.CorrectionInLatitude(l, JD1);
  
	  double lrad = CT.D2R(l);
	  double brad = CT.D2R(b);
	  r = CAASaturn.RadiusVector(JD1);
  
	  //Step 4
	  x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	  y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	  z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	  DELTA = Math.Sqrt(x *x + y *y + z *z);
	  EarthLightTravelTime = ELL.DistanceToLightTime(DELTA);
  
	  //Prepare for the next loop around
	  bIterate = (Math.Abs(EarthLightTravelTime - PreviousEarthLightTravelTime) > 2E-6); //2E-6 corresponds to 0.17 of a second
	  if (bIterate)
	  {
		JD1 = JD - EarthLightTravelTime;
		PreviousEarthLightTravelTime = EarthLightTravelTime;
	  }
	}
  
	//Step 5. Calculate Saturn's geocentric Longitude and Latitude
	double lambda = Math.Atan2(y, x);
	double beta = Math.Atan2(z, Math.Sqrt(x *x + y *y));
  
	//Step 6. Calculate B, a and b
	details.B = Math.Asin(Math.Sin(irad)*Math.Cos(beta)*Math.Sin(lambda - omegarad) - Math.Cos(irad)*Math.Sin(beta));
	details.a = 375.35 / DELTA;
	details.b = details.a * Math.Sin(Math.Abs(details.B));
	details.B = CT.R2D(details.B);
  
	//Step 7. Calculate the longitude of the ascending node of Saturn's orbit
	double N = 113.6655 + 0.8771 *T;
	double Nrad = CT.D2R(N);
	double ldash = l - 0.01759/r;
	double ldashrad = CT.D2R(ldash);
	double bdash = b - 0.000764 *Math.Cos(ldashrad - Nrad)/r;
	double bdashrad = CT.D2R(bdash);
  
	//Step 8. Calculate Bdash
	details.Bdash = CT.R2D(Math.Asin(Math.Sin(irad)*Math.Cos(bdashrad)*Math.Sin(ldashrad - omegarad) - Math.Cos(irad)*Math.Sin(bdashrad)));
  
	//Step 9. Calculate DeltaU
	double U1 = Math.Atan2(Math.Sin(irad)*Math.Sin(bdashrad) + Math.Cos(irad)*Math.Cos(bdashrad)*Math.Sin(ldashrad - omegarad), Math.Cos(bdashrad)*Math.Cos(ldashrad - omegarad));
	double U2 = Math.Atan2(Math.Sin(irad)*Math.Sin(beta) + Math.Cos(irad)*Math.Cos(beta)*Math.Sin(lambda - omegarad), Math.Cos(beta)*Math.Cos(lambda - omegarad));
	details.DeltaU = CT.R2D(Math.Abs(U1 - U2));
  
	//Step 10. Calculate the Nutations
	double Obliquity = CAANutation.TrueObliquityOfEcliptic(JD);
	double NutationInLongitude = CAANutation.NutationInLongitude(JD);
  
	//Step 11. Calculate the Ecliptical longitude and latitude of the northern pole of the ring plane
	double lambda0 = omega - 90;
	double beta0 = 90 - i;
  
	//Step 12. Correct lambda and beta for the aberration of Saturn
	lambda += CT.D2R(0.005693 *Math.Cos(l0rad - lambda)/Math.Cos(beta));
	beta += CT.D2R(0.005693 *Math.Sin(l0rad - lambda)*Math.Sin(beta));
  
	//Step 13. Add nutation in longitude to lambda0 and lambda
	//double NLrad = CAACoordinateTransformation::DegreesToRadians(NutationInLongitude/3600);
	lambda = CT.R2D(lambda);
	lambda += NutationInLongitude/3600;
	lambda = CT.M360(lambda);
	lambda0 += NutationInLongitude/3600;
	lambda0 = CT.M360(lambda0);
  
	//Step 14. Convert to equatorial coordinates
	beta = CT.R2D(beta);
	COR GeocentricEclipticSaturn = CT.Ec2Eq(lambda, beta, Obliquity);
	double alpha = CT.H2R(GeocentricEclipticSaturn.X);
	double delta = CT.D2R(GeocentricEclipticSaturn.Y);
	COR GeocentricEclipticNorthPole = CT.Ec2Eq(lambda0, beta0, Obliquity);
	double alpha0 = CT.H2R(GeocentricEclipticNorthPole.X);
	double delta0 = CT.D2R(GeocentricEclipticNorthPole.Y);
  
	//Step 15. Calculate the Position angle
	details.P = CT.R2D(Math.Atan2(Math.Cos(delta0)*Math.Sin(alpha0 - alpha), Math.Sin(delta0)*Math.Cos(delta) - Math.Cos(delta0)*Math.Sin(delta)*Math.Cos(alpha0 - alpha)));
  
	return details;
  }
}
