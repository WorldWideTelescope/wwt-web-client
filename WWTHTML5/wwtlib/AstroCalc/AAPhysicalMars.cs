using System;
//
//Module : AAPHYSICALMARS.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of Mars
//Created: PJN / 04-01-2004
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


///////////////////////////////// Includes ////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPhysicalMarsDetails
{
  public CAAPhysicalMarsDetails()
  {
	  DE = 0;
	  DS = 0;
	  w = 0;
	  P = 0;
	  X = 0;
	  k = 0;
	  q = 0;
	  d = 0;
  }

//Member variables
  public double DE;
  public double DS;
  public double w;
  public double P;
  public double X;
  public double k;
  public double q;
  public double d;
}

public class  CAAPhysicalMars
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAAPhysicalMarsDetails Calculate(double JD)
  {
	//What will be the return value
	CAAPhysicalMarsDetails details = new CAAPhysicalMarsDetails();
  
	//Step 1
	double T = (JD - 2451545) / 36525;
	double Lambda0 = 352.9065 + 1.17330 *T;
	double Lambda0rad = CT.D2R(Lambda0);
	double Beta0 = 63.2818 - 0.00394 *T;
	double Beta0rad = CT.D2R(Beta0);
  
	//Step 2
	double l0 = CAAEarth.EclipticLongitude(JD);
	double l0rad = CT.D2R(l0);
	double b0 = CAAEarth.EclipticLatitude(JD);
	double b0rad = CT.D2R(b0);
	double R = CAAEarth.RadiusVector(JD);
  
	double PreviousLightTravelTime = 0;
	double LightTravelTime = 0;
	double x = 0;
	double y = 0;
	double z = 0;
	bool bIterate = true;
	double DELTA = 0;
	double l = 0;
	double lrad = 0;
	double b = 0;
	double brad = 0;
	double r = 0;
	while (bIterate)
	{
	  double JD2 = JD - LightTravelTime;
  
	  //Step 3
	  l = CAAMars.EclipticLongitude(JD2);
	  lrad = CT.D2R(l);
	  b = CAAMars.EclipticLatitude(JD2);
	  brad = CT.D2R(b);
	  r = CAAMars.RadiusVector(JD2);
  
	  //Step 4
	  x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	  y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	  z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	  DELTA = Math.Sqrt(x *x + y *y + z *z);
	  LightTravelTime = ELL.DistanceToLightTime(DELTA);
  
	  //Prepare for the next loop around
	  bIterate = (Math.Abs(LightTravelTime - PreviousLightTravelTime) > 2E-6); //2E-6 correponds to 0.17 of a second
	  if (bIterate)
		PreviousLightTravelTime = LightTravelTime;
	}
  
	//Step 5
	double lambdarad = Math.Atan2(y, x);
	double lambda = CT.R2D(lambdarad);
	double betarad = Math.Atan2(z, Math.Sqrt(x *x + y *y));
	double beta = CT.R2D(betarad);
  
	//Step 6
	details.DE = CT.R2D(Math.Asin(-Math.Sin(Beta0rad)*Math.Sin(betarad) - Math.Cos(Beta0rad)*Math.Cos(betarad)*Math.Cos(Lambda0rad - lambdarad)));
  
	//Step 7
	double N = 49.5581 + 0.7721 *T;
	double Nrad = CT.D2R(N);
  
	double ldash = l - 0.00697/r;
	double ldashrad = CT.D2R(ldash);
	double bdash = b - 0.000225*(Math.Cos(lrad - Nrad)/r);
	double bdashrad = CT.D2R(bdash);
  
	//Step 8
	details.DS = CT.R2D(Math.Asin(-Math.Sin(Beta0rad)*Math.Sin(bdashrad) - Math.Cos(Beta0rad)*Math.Cos(bdashrad)*Math.Cos(Lambda0rad - ldashrad)));
  
	//Step 9
	double W = CT.M360(11.504 + 350.89200025*(JD - LightTravelTime - 2433282.5));
  
	//Step 10
	double e0 = CAANutation.MeanObliquityOfEcliptic(JD);
	double e0rad = CT.D2R(e0);
	COR PoleEquatorial = CT.Ec2Eq(Lambda0, Beta0, e0);
	double alpha0rad = CT.H2R(PoleEquatorial.X);
	double delta0rad = CT.D2R(PoleEquatorial.Y);
  
	//Step 11
	double u = y *Math.Cos(e0rad) - z *Math.Sin(e0rad);
	double v = y *Math.Sin(e0rad) + z *Math.Cos(e0rad);
	double alpharad = Math.Atan2(u, x);
	double alpha = CT.R2H(alpharad);
	double deltarad = Math.Atan2(v, Math.Sqrt(x *x + u *u));
	double delta = CT.R2D(deltarad);
	double xi = Math.Atan2(Math.Sin(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad) - Math.Sin(deltarad)*Math.Cos(delta0rad), Math.Cos(deltarad)*Math.Sin(alpha0rad - alpharad));
  
	//Step 12
	details.w = CT.M360(W - CT.R2D(xi));
  
	//Step 13
	double NutationInLongitude = CAANutation.NutationInLongitude(JD);
	double NutationInObliquity = CAANutation.NutationInObliquity(JD);
  
	//Step 14
	lambda += 0.005693 *Math.Cos(l0rad - lambdarad)/Math.Cos(betarad);
	beta += 0.005693 *Math.Sin(l0rad - lambdarad)*Math.Sin(betarad);
  
	//Step 15
	Lambda0 += NutationInLongitude/3600;
	Lambda0rad = CT.D2R(Lambda0);
	lambda += NutationInLongitude/3600;
	lambdarad = CT.D2R(lambda);
	e0 += NutationInObliquity/3600;
	e0rad = CT.D2R(e0rad);
  
	//Step 16
	COR ApparentPoleEquatorial = CT.Ec2Eq(Lambda0, Beta0, e0);
	double alpha0dash = CT.H2R(ApparentPoleEquatorial.X);
	double delta0dash = CT.D2R(ApparentPoleEquatorial.Y);
	COR ApparentMars = CT.Ec2Eq(lambda, beta, e0);
	double alphadash = CT.H2R(ApparentMars.X);
	double deltadash = CT.D2R(ApparentMars.Y);
  
	//Step 17
	details.P = CT.M360(CT.R2D(Math.Atan2(Math.Cos(delta0dash)*Math.Sin(alpha0dash - alphadash), Math.Sin(delta0dash)*Math.Cos(deltadash) - Math.Cos(delta0dash)*Math.Sin(deltadash)*Math.Cos(alpha0dash - alphadash))));
  
	//Step 18
	double SunLambda = CAASun.GeometricEclipticLongitude(JD);
	double SunBeta = CAASun.GeometricEclipticLatitude(JD);
	COR SunEquatorial = CT.Ec2Eq(SunLambda, SunBeta, e0);
	details.X = MIFR.PositionAngle(SunEquatorial.X, SunEquatorial.Y, alpha, delta);
  
	//Step 19
	details.d = 9.36 / DELTA;
	details.k = IFR.IlluminatedFraction2(r, R, DELTA);
	details.q = (1 - details.k)*details.d;
  
	return details;
  }
}
