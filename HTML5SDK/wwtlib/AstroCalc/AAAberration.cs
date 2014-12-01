using System;
public static partial class GFX
{

	public static ACFT[] g_ACft = { new ACFT(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1719914, -2, -25, 0, 25, -13, 1578089, 156, 10, 32, 684185, -358), new ACFT(0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6434, 141, 28007, -107, 25697, -95, -5904, -130, 11141, -48, -2559, -55), new ACFT(0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 715, 0, 0, 0, 6, 0, -657, 0, -15, 0, -282, 0), new ACFT(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 715, 0, 0, 0, 0, 0, -656, 0, 0, 0, -285, 0), new ACFT(0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 486, -5, -236, -4, -216, -4, -446, 5, -94, 0, -193, 0), new ACFT(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 159, 0, 0, 0, 2, 0, -147, 0, -6, 0, -61, 0), new ACFT(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 26, 0, 0, 0, -59, 0), new ACFT(0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 39, 0, 0, 0, 0, 0, -36, 0, 0, 0, -16, 0), new ACFT(0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 33, 0, -10, 0, -9, 0, -30, 0, -5, 0, -13, 0), new ACFT(0, 2, 0, -1, 0, 0, 0, 0, 0, 0, 0, 31, 0, 1, 0, 1, 0, -28, 0, 0, 0, -12, 0), new ACFT(0, 3, -8, 3, 0, 0, 0, 0, 0, 0, 0, 8, 0, -28, 0, 25, 0, 8, 0, 11, 0, 3, 0), new ACFT(0, 5, -8, 3, 0, 0, 0, 0, 0, 0, 0, 8, 0, -28, 0, -25, 0, -8, 0, -11, 0, -3, 0), new ACFT(2, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 21, 0, 0, 0, 0, 0, -19, 0, 0, 0, -8, 0), new ACFT(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -19, 0, 0, 0, 0, 0, 17, 0, 0, 0, 8, 0), new ACFT(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 17, 0, 0, 0, 0, 0, -16, 0, 0, 0, -7, 0), new ACFT(0, 1, 0, -2, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 15, 0, 1, 0, 7, 0), new ACFT(0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 16, 0, 0, 0, 1, 0, -15, 0, -3, 0, -6, 0), new ACFT(0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 11, 0, -1, 0, -1, 0, -10, 0, -1, 0, -5, 0), new ACFT(2, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -11, 0, -10, 0, 0, 0, -4, 0, 0, 0), new ACFT(0, 1, 0, -1, 0, 0, 0, 0, 0, 0, 0, -11, 0, -2, 0, -2, 0, 9, 0, -1, 0, 4, 0), new ACFT(0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, -7, 0, -8, 0, -8, 0, 6, 0, -3, 0, 3, 0), new ACFT(0, 3, 0, -2, 0, 0, 0, 0, 0, 0, 0, -10, 0, 0, 0, 0, 0, 9, 0, 0, 0, 4, 0), new ACFT(1, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, -9, 0, 0, 0, 0, 0, -9, 0, 0, 0, -4, 0), new ACFT(2, -3, 0, 0, 0, 0, 0, 0, 0, 0, 0, -9, 0, 0, 0, 0, 0, -8, 0, 0, 0, -4, 0), new ACFT(0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, -9, 0, -8, 0, 0, 0, -3, 0, 0, 0), new ACFT(2, -4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -9, 0, 8, 0, 0, 0, 3, 0, 0, 0), new ACFT(0, 3, -2, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, -8, 0, 0, 0, -3, 0), new ACFT(0, 0, 0, 0, 0, 0, 0, 1, 2, -1, 0, 8, 0, 0, 0, 0, 0, -7, 0, 0, 0, -3, 0), new ACFT(8, -12, 0, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, -7, 0, -6, 0, 4, 0, -3, 0, 2, 0), new ACFT(8, -14, 0, 0, 0, 0, 0, 0, 0, 0, 0, -4, 0, -7, 0, 6, 0, -4, 0, 3, 0, -2, 0), new ACFT(0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, -6, 0, -5, 0, -4, 0, 5, 0, -2, 0, 2, 0), new ACFT(3, -4, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, -1, 0, -2, 0, -7, 0, 1, 0, -4, 0), new ACFT(0, 2, 0, -2, 0, 0, 0, 0, 0, 0, 0, 4, 0, -6, 0, -5, 0, -4, 0, -2, 0, -2, 0), new ACFT(3, -3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -7, 0, -6, 0, 0, 0, -3, 0, 0, 0), new ACFT(0, 2, -2, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, -5, 0, -4, 0, -5, 0, -2, 0, -2, 0), new ACFT(0, 0, 0, 0, 0, 0, 0, 1, -2, 0, 0, 5, 0, 0, 0, 0, 0, -5, 0, 0, 0, -2, 0) };
}
//
//Module : AAABERRATION.CPP
//Purpose: Implementation for the algorithms for Aberration
//Created: PJN / 29-12-2003
//History: PJN / 21-04-2005 1. Renamed "AAAberation.cpp" to "AAAberration.cpp" so that all source code filenames 
//                          match their corresponding header files. Thanks to Jürgen Schuck for suggesting this 
//                          update.
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


/////////////////////////////// Includes //////////////////////////////////////



/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  ABR // was CAAAberration
{
//Static methods
	//L2   L3   L4  L5  L6  L7  L8  Ldash D   Mdash F   xsin      xsint xcos    xcost ysin   ysint ycos     ycost zsin   zsint zcos    zcost
  
  
  //////////////////////////////// Implementation ///////////////////////////////
  
  public static C3D EarthVelocity(double JD)
  {
	double T = (JD - 2451545) / 36525;
	double L2 = 3.1761467 + 1021.3285546 * T;
	double L3 = 1.7534703 + 628.3075849 * T;
	double L4 = 6.2034809 + 334.0612431 * T;
	double L5 = 0.5995465 + 52.9690965 * T;
	double L6 = 0.8740168 + 21.3299095 * T;
	double L7 = 5.4812939 + 7.4781599 * T;
	double L8 = 5.3118863 + 3.8133036 * T;
	double Ldash = 3.8103444 + 8399.6847337 * T;
	double D = 5.1984667 + 7771.3771486 * T;
	double Mdash = 2.3555559 + 8328.6914289 * T;
	double F = 1.6279052 + 8433.4661601 * T;
  
	C3D velocity = new C3D();

    int nAberrationCoefficients = GFX.g_ACft.Length;
	for (int i =0; i<nAberrationCoefficients; i++)
	{
	  double Argument = GFX.g_ACft[i].L2 *L2 + GFX.g_ACft[i].L3 *L3 + GFX.g_ACft[i].L4 *L4 + GFX.g_ACft[i].L5 *L5 + GFX.g_ACft[i].L6 *L6 + GFX.g_ACft[i].L7 *L7 + GFX.g_ACft[i].L8 *L8 + GFX.g_ACft[i].Ldash *Ldash + GFX.g_ACft[i].D *D + GFX.g_ACft[i].Mdash *Mdash + GFX.g_ACft[i].F *F;
	  velocity.X += (GFX.g_ACft[i].xsin + GFX.g_ACft[i].xsint * T) * Math.Sin(Argument);
	  velocity.X += (GFX.g_ACft[i].xcos + GFX.g_ACft[i].xcost * T) * Math.Cos(Argument);
  
	  velocity.Y += (GFX.g_ACft[i].ysin + GFX.g_ACft[i].ysint * T) * Math.Sin(Argument);
	  velocity.Y += (GFX.g_ACft[i].ycos + GFX.g_ACft[i].ycost * T) * Math.Cos(Argument);
  
	  velocity.Z += (GFX.g_ACft[i].zsin + GFX.g_ACft[i].zsint * T) * Math.Sin(Argument);
	  velocity.Z += (GFX.g_ACft[i].zcos + GFX.g_ACft[i].zcost * T) * Math.Cos(Argument);
	}
  
	return velocity;
  }
  public static COR EclipticAberration(double Lambda, double Beta, double JD)
  {
	//What is the return value
	COR aberration = new COR();
  
	double T = (JD - 2451545) / 36525;
	double Tsquared = T *T;
	double e = 0.016708634 - 0.000042037 *T - 0.0000001267 *Tsquared;
	double pi = 102.93735 + 1.71946 *T + 0.00046 *Tsquared;
	double k = 20.49552;
	double SunLongitude = CAASun.GeometricEclipticLongitude(JD);
  
	//Convert to radians
	pi = CT.D2R(pi);
	Lambda = CT.D2R(Lambda);
	Beta = CT.D2R(Beta);
	SunLongitude = CT.D2R(SunLongitude);
  
	aberration.X = (-k *Math.Cos(SunLongitude - Lambda) + e *k *Math.Cos(pi - Lambda)) / Math.Cos(Beta) / 3600;
	aberration.Y = -k *Math.Sin(Beta)*(Math.Sin(SunLongitude - Lambda) - e *Math.Sin(pi - Lambda)) / 3600;
  
	return aberration;
  }
  public static COR EquatorialAberration(double Alpha, double Delta, double JD)
  {
	//Convert to radians
	Alpha = CT.D2R(Alpha *15);
	Delta = CT.D2R(Delta);
  
	double cosAlpha = Math.Cos(Alpha);
	double sinAlpha = Math.Sin(Alpha);
	double cosDelta = Math.Cos(Delta);
	double sinDelta = Math.Sin(Delta);
  
	C3D velocity = EarthVelocity(JD);
  
	//What is the return value
	COR aberration = new COR();
  
	aberration.X = CT.R2H((velocity.Y * cosAlpha - velocity.X * sinAlpha) / (17314463350.0 * cosDelta));
	aberration.Y = CT.R2D(- (((velocity.X * cosAlpha + velocity.Y * sinAlpha) * sinDelta - velocity.Z * cosDelta) / 17314463350.0));
  
	return aberration;
  }
}




////////////////////////////// Macros / Defines ///////////////////////////////

public class ACFT
{
    public ACFT(int L2,int L3,int L4,int L5,int L6,int L7,int L8,int Ldash,int D,int Mdash,int F,int xsin,int xsint,int xcos,int xcost,int ysin,int ysint,int ycos,int ycost,int zsin,int zsint,int zcos,int zcost )
    {

        this.L2 = L2;
        this.L3 = L3;
        this.L4 = L4;
        this.L5 = L5;
        this.L6 = L6;
        this.L7 = L7;
        this.L8 = L8;
        this.Ldash = Ldash;
        this.D = D;
        this.Mdash = Mdash;
        this.F = F;
        this.xsin = xsin;
        this.xsint = xsint;
        this.xcos = xcos;
        this.xcost = xcost;
        this.ysin = ysin;
        this.ysint = ysint;
        this.ycos = ycos;
        this.ycost = ycost;
        this.zsin = zsin;
        this.zsint = zsint;
        this.zcos = zcos;
        this.zcost = zcost;

    }

  public int L2;
  public int L3;
  public int L4;
  public int L5;
  public int L6;
  public int L7;
  public int L8;
  public int Ldash;
  public int D;
  public int Mdash;
  public int F;
  public int xsin;
  public int xsint;
  public int xcos;
  public int xcost;
  public int ysin;
  public int ysint;
  public int ycos;
  public int ycost;
  public int zsin;
  public int zsint;
  public int zcos;
  public int zcost;
}
