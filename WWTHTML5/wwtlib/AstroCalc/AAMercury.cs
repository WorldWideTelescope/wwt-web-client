using System;
public static partial class GFX
{

	public static VSC[] g_L0MercuryCoefficients = { new VSC(440250710, 0, 0), new VSC(40989415, 1.48302034, 26087.90314157), new VSC(5046294, 4.47785449, 52175.8062831), new VSC(855347, 1.165203, 78263.709425), new VSC(165590, 4.119692, 104351.612566), new VSC(34562, 0.77931, 130439.51571), new VSC(7583, 3.7135, 156527.4188), new VSC(3560, 1.5120, 1109.3786), new VSC(1803, 4.1033, 5661.3320), new VSC(1726, 0.3583, 182615.3220), new VSC(1590, 2.9951, 25028.5212), new VSC(1365, 4.5992, 27197.2817), new VSC(1017, 0.8803, 31749.2352), new VSC(714, 1.541, 24978.525), new VSC(644, 5.303, 21535.950), new VSC(451, 6.050, 51116.424), new VSC(404, 3.282, 208703.225), new VSC(352, 5.242, 20426.571), new VSC(345, 2.792, 15874.618), new VSC(343, 5.765, 955.600), new VSC(339, 5.863, 25558.212), new VSC(325, 1.337, 53285.185), new VSC(273, 2.495, 529.691), new VSC(264, 3.917, 57837.138), new VSC(260, 0.987, 4551.953), new VSC(239, 0.113, 1059.382), new VSC(235, 0.267, 11322.664), new VSC(217, 0.660, 13521.751), new VSC(209, 2.092, 47623.853), new VSC(183, 2.629, 27043.503), new VSC(182, 2.434, 25661.305), new VSC(176, 4.536, 51066.428), new VSC(173, 2.452, 24498.830), new VSC(142, 3.360, 37410.567), new VSC(138, 0.291, 10213.286), new VSC(125, 3.721, 39609.655), new VSC(118, 2.781, 77204.327), new VSC(106, 4.206, 19804.827) };

	public static VSC[] g_L1MercuryCoefficients = { new VSC(2608814706223.0, 0, 0), new VSC(1126008, 6.2170397, 26087.9031416), new VSC(303471, 3.055655, 52175.806283), new VSC(80538, 6.10455, 78263.70942), new VSC(21245, 2.83532, 104351.61257), new VSC(5592, 5.8268, 130439.5157), new VSC(1472, 2.5185, 156527.4188), new VSC(388, 5.480, 182615.322), new VSC(352, 3.052, 1109.379), new VSC(103, 2.149, 208703.225), new VSC(94, 6.12, 27197.28), new VSC(91, 0.00, 24978.52), new VSC(52, 5.62, 5661.33), new VSC(44, 4.57, 25028.52), new VSC(28, 3.04, 51066.43), new VSC(27, 5.09, 234791.13) };

	public static VSC[] g_L2MercuryCoefficients = { new VSC(53050, 0, 0), new VSC(16904, 4.69072, 26087.90314), new VSC(7397, 1.3474, 52175.8063), new VSC(3018, 4.4564, 78263.7094), new VSC(1107, 1.264, 104351.6126), new VSC(378, 4.320, 130439.516), new VSC(123, 1.069, 156527.419), new VSC(39, 4.08, 182615.32), new VSC(15, 4.63, 1109.38), new VSC(12, 0.79, 208703.23) };

	public static VSC[] g_L3MercuryCoefficients = { new VSC(188, 0.035, 52175.806), new VSC(142, 3.125, 26087.903), new VSC(97, 3.00, 78263.71), new VSC(44, 6.02, 104351.61), new VSC(35, 0, 0), new VSC(18, 2.78, 130439.52), new VSC(7, 5.82, 156527.42), new VSC(3, 2.57, 182615.32) };

	public static VSC[] g_L4MercuryCoefficients = { new VSC(114, 3.1416, 0), new VSC(2, 2.03, 26087.90), new VSC(2, 1.42, 78263.71), new VSC(2, 4.50, 52175.81), new VSC(1, 4.50, 104351.61), new VSC(1, 1.27, 130439.52) };

	public static VSC[] g_L5MercuryCoefficients = { new VSC(1, 3.14, 0) };


	public static VSC[] g_B0MercuryCoefficients = { new VSC(11737529, 1.98357499, 26087.90314157), new VSC(2388077, 5.0373896, 52175.8062831), new VSC(1222840, 3.1415927, 0), new VSC(543252, 1.796444, 78263.709425), new VSC(129779, 4.832325, 104351.612566), new VSC(31867, 1.58088, 130439.51571), new VSC(7963, 4.6097, 156527.4188), new VSC(2014, 1.3532, 182615.3220), new VSC(514, 4.378, 208703.325), new VSC(209, 2.020, 24978.525), new VSC(208, 4.918, 27197.282), new VSC(132, 1.119, 234791.128), new VSC(121, 1.813, 53285.185), new VSC(100, 5.657, 20426.571) };

	public static VSC[] g_B1MercuryCoefficients = { new VSC(429151, 3.501698, 26087.903142), new VSC(146234, 3.141593, 0), new VSC(22675, 0.01515, 52175.80628), new VSC(10895, 0.48540, 78263.70942), new VSC(6353, 3.4294, 104351.6126), new VSC(2496, 0.1605, 130439.5157), new VSC(860, 3.185, 156527.419), new VSC(278, 6.210, 182615.322), new VSC(86, 2.95, 208703.23), new VSC(28, 0.29, 27197.28), new VSC(26, 5.98, 234791.13) };

	public static VSC[] g_B2MercuryCoefficients = { new VSC(11831, 4.79066, 26087.90314), new VSC(1914, 0, 0), new VSC(1045, 1.2122, 52175.8063), new VSC(266, 4.434, 78263.709), new VSC(170, 1.623, 104351.613), new VSC(96, 4.80, 130439.52), new VSC(45, 1.61, 156527.42), new VSC(18, 4.67, 182615.32), new VSC(7, 1.43, 208703.23) };

	public static VSC[] g_B3MercuryCoefficients = { new VSC(235, 0.354, 26087.903), new VSC(161, 0, 0), new VSC(19, 4.36, 52175.81), new VSC(6, 2.51, 78263.71), new VSC(5, 6.14, 104351.61), new VSC(3, 3.12, 130439.52), new VSC(2, 6.27, 156527.42) };

	public static VSC[] g_B4MercuryCoefficients = { new VSC(4, 1.75, 26087.90), new VSC(1, 3.14, 0) };


	public static VSC[] g_R0MercuryCoefficients = { new VSC(39528272, 0, 0), new VSC(7834132, 6.1923372, 26087.9031416), new VSC(795526, 2.959897, 52175.806283), new VSC(121282, 6.010642, 78263.709425), new VSC(21922, 2.77820, 104351.61257), new VSC(4354, 5.8289, 130439.5157), new VSC(918, 2.597, 156527.419), new VSC(290, 1.424, 25028.521), new VSC(260, 3.028, 27197.282), new VSC(202, 5.647, 182615.322), new VSC(201, 5.592, 31749.235), new VSC(142, 6.253, 24978.525), new VSC(100, 3.734, 21535.950) };

	public static VSC[] g_R1MercuryCoefficients = { new VSC(217348, 4.656172, 26087.903142), new VSC(44142, 1.42386, 52175.80628), new VSC(10094, 4.47466, 78263.70942), new VSC(2433, 1.2423, 104351.6126), new VSC(1624, 0, 0), new VSC(604, 4.293, 130439.516), new VSC(153, 1.061, 156527.419), new VSC(39, 4.11, 182615.32) };

	public static VSC[] g_R2MercuryCoefficients = { new VSC(3118, 3.0823, 26087.9031), new VSC(1245, 6.1518, 52175.8063), new VSC(425, 2.926, 78263.709), new VSC(136, 5.980, 104351.613), new VSC(42, 2.75, 130439.52), new VSC(22, 3.14, 0), new VSC(13, 5.80, 156527.42) };

	public static VSC[] g_R3MercuryCoefficients = { new VSC(33, 1.68, 26087.90), new VSC(24, 4.63, 52175.81), new VSC(12, 1.39, 78263.71), new VSC(5, 4.44, 104351.61), new VSC(2, 1.21, 130439.52) };
}
//
//Module : AAMERCURY.CPP
//Purpose: Implementation for the algorithms which obtain the heliocentric position of Mercury
//Created: PJN / 29-12-2003
//History: PJN / 16-11-2005 1. Fixed a transcription error in the second coefficient used to calculate 
//                          the longitude of Mercury. Thanks to "Maurizio" for reporting this bug.
//         PJN / 12-05-2006 1. Fixed a transcription error in the third coefficient used to calculate
//                          the R0 term for the radius vector of Mercury. Thanks to John Kruso for 
//                          reporting this issue.
//                          2. Fixed a transcription error in the third coefficient used to calculate
//                          the R1 term for the radius vector of Mercury. Thanks to John Kruso for 
//                          reporting this issue.  
//                          3. Updated copyright details.
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


//////////////////////////// Includes /////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAMercury
{
//Static methods

  ///////////////////////////// Implementation //////////////////////////////////
  
  public static double EclipticLongitude(double JD)
  {
	double rho = (JD - 2451545) / 365250;
	double rhosquared = rho *rho;
	double rhocubed = rhosquared *rho;
	double rho4 = rhocubed *rho;
	double rho5 = rho4 *rho;
  
	//Calculate L0
    int nL0Coefficients = GFX.g_L0MercuryCoefficients.Length;
	double L0 = 0;
	int i;
	for (i =0; i<nL0Coefficients; i++)
	  L0 += GFX.g_L0MercuryCoefficients[i].A * Math.Cos(GFX.g_L0MercuryCoefficients[i].B + GFX.g_L0MercuryCoefficients[i].C *rho);
  
	//Calculate L1
	int nL1Coefficients = GFX.g_L1MercuryCoefficients.Length;
	double L1 = 0;
	for (i =0; i<nL1Coefficients; i++)
	  L1 += GFX.g_L1MercuryCoefficients[i].A * Math.Cos(GFX.g_L1MercuryCoefficients[i].B + GFX.g_L1MercuryCoefficients[i].C *rho);
  
	//Calculate L2
	int nL2Coefficients = GFX.g_L2MercuryCoefficients.Length;
	double L2 = 0;
	for (i =0; i<nL2Coefficients; i++)
	  L2 += GFX.g_L2MercuryCoefficients[i].A * Math.Cos(GFX.g_L2MercuryCoefficients[i].B + GFX.g_L2MercuryCoefficients[i].C *rho);
  
	//Calculate L3
	int nL3Coefficients = GFX.g_L3MercuryCoefficients.Length;
	double L3 = 0;
	for (i =0; i<nL3Coefficients; i++)
	  L3 += GFX.g_L3MercuryCoefficients[i].A * Math.Cos(GFX.g_L3MercuryCoefficients[i].B + GFX.g_L3MercuryCoefficients[i].C *rho);
  
	//Calculate L4
	int nL4Coefficients = GFX.g_L4MercuryCoefficients.Length;
	double L4 = 0;
	for (i =0; i<nL4Coefficients; i++)
	  L4 += GFX.g_L4MercuryCoefficients[i].A * Math.Cos(GFX.g_L4MercuryCoefficients[i].B + GFX.g_L4MercuryCoefficients[i].C *rho);
  
	//Calculate L5
	int nL5Coefficients = GFX.g_L5MercuryCoefficients.Length;
	double L5 = 0;
	for (i =0; i<nL5Coefficients; i++)
	  L5 += GFX.g_L5MercuryCoefficients[i].A * Math.Cos(GFX.g_L5MercuryCoefficients[i].B + GFX.g_L5MercuryCoefficients[i].C *rho);
  
	double @value = (L0 + L1 *rho + L2 *rhosquared + L3 *rhocubed + L4 *rho4 + L5 *rho5) / 100000000;
  
	//convert results back to degrees
	@value = CT.M360(CT.R2D(@value));
	return @value;
  }
  public static double EclipticLatitude(double JD)
  {
	double rho = (JD - 2451545) / 365250;
	double rhosquared = rho *rho;
	double rhocubed = rhosquared *rho;
	double rho4 = rhocubed *rho;
  
	//Calculate B0
	int nB0Coefficients = GFX.g_B0MercuryCoefficients.Length;
	double B0 = 0;
	int i;
	for (i =0; i<nB0Coefficients; i++)
	  B0 += GFX.g_B0MercuryCoefficients[i].A * Math.Cos(GFX.g_B0MercuryCoefficients[i].B + GFX.g_B0MercuryCoefficients[i].C *rho);
  
	//Calculate B1
	int nB1Coefficients = GFX.g_B1MercuryCoefficients.Length;
	double B1 = 0;
	for (i =0; i<nB1Coefficients; i++)
	  B1 += GFX.g_B1MercuryCoefficients[i].A * Math.Cos(GFX.g_B1MercuryCoefficients[i].B + GFX.g_B1MercuryCoefficients[i].C *rho);
  
	//Calculate B2
	int nB2Coefficients = GFX.g_B2MercuryCoefficients.Length;
	double B2 = 0;
	for (i =0; i<nB2Coefficients; i++)
	  B2 += GFX.g_B2MercuryCoefficients[i].A * Math.Cos(GFX.g_B2MercuryCoefficients[i].B + GFX.g_B2MercuryCoefficients[i].C *rho);
  
	//Calculate B3
	int nB3Coefficients = GFX.g_B3MercuryCoefficients.Length;
	double B3 = 0;
	for (i =0; i<nB3Coefficients; i++)
	  B3 += GFX.g_B3MercuryCoefficients[i].A * Math.Cos(GFX.g_B3MercuryCoefficients[i].B + GFX.g_B3MercuryCoefficients[i].C *rho);
  
	//Calculate B4
    int nB4Coefficients = GFX.g_B4MercuryCoefficients.Length;
	double B4 = 0;
	for (i =0; i<nB4Coefficients; i++)
	  B4 += GFX.g_B4MercuryCoefficients[i].A * Math.Cos(GFX.g_B4MercuryCoefficients[i].B + GFX.g_B4MercuryCoefficients[i].C *rho);
  
	double @value = (B0 + B1 *rho + B2 *rhosquared + B3 *rhocubed + B4 *rho4) / 100000000;
  
	//convert results back to degrees
	@value = CT.R2D(@value);
	return @value;
  }
  public static double RadiusVector(double JD)
  {
	double rho = (JD - 2451545) / 365250;
	double rhosquared = rho *rho;
	double rhocubed = rhosquared *rho;
  
	//Calculate R0
	int nR0Coefficients = GFX.g_R0MercuryCoefficients.Length;
	double R0 = 0;
	int i;
	for (i =0; i<nR0Coefficients; i++)
	  R0 += GFX.g_R0MercuryCoefficients[i].A * Math.Cos(GFX.g_R0MercuryCoefficients[i].B + GFX.g_R0MercuryCoefficients[i].C *rho);
  
	//Calculate R1
	int nR1Coefficients = GFX.g_R1MercuryCoefficients.Length;
	double R1 = 0;
	for (i =0; i<nR1Coefficients; i++)
	  R1 += GFX.g_R1MercuryCoefficients[i].A * Math.Cos(GFX.g_R1MercuryCoefficients[i].B + GFX.g_R1MercuryCoefficients[i].C *rho);
  
	//Calculate R2
	int nR2Coefficients = GFX.g_R2MercuryCoefficients.Length;
	double R2 = 0;
	for (i =0; i<nR2Coefficients; i++)
	  R2 += GFX.g_R2MercuryCoefficients[i].A * Math.Cos(GFX.g_R2MercuryCoefficients[i].B + GFX.g_R2MercuryCoefficients[i].C *rho);
  
	//Calculate R3
    int nR3Coefficients = GFX.g_R3MercuryCoefficients.Length;
	double R3 = 0;
	for (i =0; i<nR3Coefficients; i++)
	  R3 += GFX.g_R3MercuryCoefficients[i].A * Math.Cos(GFX.g_R3MercuryCoefficients[i].B + GFX.g_R3MercuryCoefficients[i].C *rho);
  
	return (R0 + R1 *rho + R2 *rhosquared + R3 *rhocubed) / 100000000;
  }
}



//////////////////////////// Macros / Defines /////////////////////////////////

//public class VSOP87Coefficient
//{
//  public double A;
//  public double B;
//  public double C;
//}
