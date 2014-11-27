using System;
public static  partial class GFX
{

	public static VSC[] g_L0UranusCoefficients = { new VSC(548129294, 0, 0), new VSC(9260408, 0.8910642, 74.7815986), new VSC(1504248, 3.6271926, 1.4844727), new VSC(365982, 1.899622, 73.297126), new VSC(272328, 3.358237, 149.563197), new VSC(70328, 5.39254, 63.73590), new VSC(68893, 6.09292, 76.26607), new VSC(61999, 2.26952, 2.96895), new VSC(61951, 2.85099, 11.04570), new VSC(26469, 3.14152, 71.81265), new VSC(25711, 6.11380, 454.90937), new VSC(21079, 4.36059, 148.07872), new VSC(17819, 1.74437, 36.64856), new VSC(14613, 4.73732, 3.93215), new VSC(11163, 5.82682, 224.34480), new VSC(10998, 0.48865, 138.51750), new VSC(9527, 2.9552, 35.1641), new VSC(7546, 5.2363, 109.9457), new VSC(4220, 3.2333, 70.8494), new VSC(4052, 2.2775, 151.0477), new VSC(3490, 5.4831, 146.5943), new VSC(3355, 1.0655, 4.4534), new VSC(3144, 4.7520, 77.7505), new VSC(2927, 4.6290, 9.5612), new VSC(2922, 5.3524, 85.8273), new VSC(2273, 4.3660, 70.3282), new VSC(2149, 0.6075, 38.1330), new VSC(2051, 1.5177, 0.1119), new VSC(1992, 4.9244, 277.0350), new VSC(1667, 3.6274, 380.1278), new VSC(1533, 2.5859, 52.6902), new VSC(1376, 2.0428, 65.2204), new VSC(1372, 4.1964, 111.4302), new VSC(1284, 3.1135, 202.2534), new VSC(1282, 0.5427, 222.8603), new VSC(1244, 0.9161, 2.4477), new VSC(1221, 0.1990, 108.4612), new VSC(1151, 4.1790, 33.6796), new VSC(1150, 0.9334, 3.1814), new VSC(1090, 1.7750, 12.5302), new VSC(1072, 0.2356, 62.2514), new VSC(946, 1.192, 127.472), new VSC(708, 5.183, 213.299), new VSC(653, 0.966, 78.714), new VSC(628, 0.182, 984.600), new VSC(607, 5.432, 529.691), new VSC(559, 3.358, 0.521), new VSC(524, 2.013, 299.126), new VSC(483, 2.106, 0.963), new VSC(471, 1.407, 184.727), new VSC(467, 0.415, 145.110), new VSC(434, 5.521, 183.243), new VSC(405, 5.987, 8.077), new VSC(399, 0.338, 415.552), new VSC(396, 5.870, 351.817), new VSC(379, 2.350, 56.622), new VSC(310, 5.833, 145.631), new VSC(300, 5.644, 22.091), new VSC(294, 5.839, 39.618), new VSC(252, 1.637, 221.376), new VSC(249, 4.746, 225.829), new VSC(239, 2.350, 137.033), new VSC(224, 0.516, 84.343), new VSC(223, 2.843, 0.261), new VSC(220, 1.922, 67.668), new VSC(217, 6.142, 5.938), new VSC(216, 4.778, 340.771), new VSC(208, 5.580, 68.844), new VSC(202, 1.297, 0.048), new VSC(199, 0.956, 152.532), new VSC(194, 1.888, 456.394), new VSC(193, 0.916, 453.425), new VSC(187, 1.319, 0.160), new VSC(182, 3.536, 79.235), new VSC(173, 1.539, 160.609), new VSC(172, 5.680, 219.891), new VSC(170, 3.677, 5.417), new VSC(169, 5.879, 18.159), new VSC(165, 1.424, 106.977), new VSC(163, 3.050, 112.915), new VSC(158, 0.738, 54.175), new VSC(147, 1.263, 59.804), new VSC(143, 1.300, 35.425), new VSC(139, 5.386, 32.195), new VSC(139, 4.260, 909.819), new VSC(124, 1.374, 7.114), new VSC(110, 2.027, 554.070), new VSC(109, 5.706, 77.963), new VSC(104, 5.028, 0.751), new VSC(104, 1.458, 24.379), new VSC(103, 0.681, 14.978) };

	public static VSC[] g_L1UranusCoefficients = { new VSC(7502543122.0, 0, 0), new VSC(154458, 5.242017, 74.781599), new VSC(24456, 1.71256, 1.48447), new VSC(9258, 0.4284, 11.0457), new VSC(8266, 1.5022, 63.7359), new VSC(7842, 1.3198, 149.5632), new VSC(3899, 0.4648, 3.9322), new VSC(2284, 4.1737, 76.2661), new VSC(1927, 0.5301, 2.9689), new VSC(1233, 1.5863, 70.8494), new VSC(791, 5.436, 3.181), new VSC(767, 1.996, 73.297), new VSC(482, 2.984, 85.827), new VSC(450, 4.138, 138.517), new VSC(446, 3.723, 224.345), new VSC(427, 4.731, 71.813), new VSC(354, 2.583, 148.079), new VSC(348, 2.454, 9.561), new VSC(317, 5.579, 52.690), new VSC(206, 2.363, 2.448), new VSC(189, 4.202, 56.622), new VSC(184, 0.284, 151.048), new VSC(180, 5.684, 12.530), new VSC(171, 3.001, 78.714), new VSC(158, 2.909, 0.963), new VSC(155, 5.591, 4.453), new VSC(154, 4.652, 35.164), new VSC(152, 2.942, 77.751), new VSC(143, 2.590, 62.251), new VSC(121, 4.148, 127.472), new VSC(116, 3.732, 65.220), new VSC(102, 4.188, 145.631), new VSC(102, 6.034, 0.112), new VSC(88, 3.99, 18.16), new VSC(88, 6.16, 202.25), new VSC(81, 2.64, 22.09), new VSC(72, 6.05, 70.33), new VSC(69, 4.05, 77.96), new VSC(59, 3.70, 67.67), new VSC(47, 3.54, 351.82), new VSC(44, 5.91, 7.11), new VSC(43, 5.72, 5.42), new VSC(39, 4.92, 222.86), new VSC(36, 5.90, 33.68), new VSC(36, 3.29, 8.08), new VSC(36, 3.33, 71.60), new VSC(35, 5.08, 38.13), new VSC(31, 5.62, 984.60), new VSC(31, 5.50, 59.80), new VSC(31, 5.46, 160.61), new VSC(30, 1.66, 447.80), new VSC(29, 1.15, 462.02), new VSC(29, 4.52, 84.34), new VSC(27, 5.54, 131.40), new VSC(27, 6.15, 299.13), new VSC(26, 4.99, 137.03), new VSC(25, 5.74, 380.13) };

	public static VSC[] g_L2UranusCoefficients = { new VSC(53033, 0, 0), new VSC(2358, 2.2601, 74.7816), new VSC(769, 4.526, 11.046), new VSC(552, 3.258, 63.736), new VSC(542, 2.276, 3.932), new VSC(529, 4.923, 1.484), new VSC(258, 3.691, 3.181), new VSC(239, 5.858, 149.563), new VSC(182, 6.218, 70.849), new VSC(54, 1.44, 76.27), new VSC(49, 6.03, 56.62), new VSC(45, 3.91, 2.45), new VSC(45, 0.81, 85.83), new VSC(38, 1.78, 52.69), new VSC(37, 4.46, 2.97), new VSC(33, 0.86, 9.56), new VSC(29, 5.10, 73.30), new VSC(24, 2.11, 18.16), new VSC(22, 5.99, 138.52), new VSC(22, 4.82, 78.71), new VSC(21, 2.40, 77.96), new VSC(21, 2.17, 224.34), new VSC(17, 2.54, 145.63), new VSC(17, 3.47, 12.53), new VSC(12, 0.02, 22.09), new VSC(11, 0.08, 127.47), new VSC(10, 5.16, 71.60), new VSC(10, 4.46, 62.25), new VSC(9, 4.26, 7.11), new VSC(8, 5.50, 67.67), new VSC(7, 1.25, 5.42), new VSC(6, 3.36, 447.80), new VSC(6, 5.45, 65.22), new VSC(6, 4.52, 151.05), new VSC(6, 5.73, 462.02) };

	public static VSC[] g_L3UranusCoefficients = { new VSC(121, 0.024, 74.782), new VSC(68, 4.12, 3.93), new VSC(53, 2.39, 11.05), new VSC(46, 0, 0), new VSC(45, 2.04, 3.18), new VSC(44, 2.96, 1.48), new VSC(25, 4.89, 63.74), new VSC(21, 4.55, 70.85), new VSC(20, 2.31, 149.56), new VSC(9, 1.58, 56.62), new VSC(4, 0.23, 18.16), new VSC(4, 5.39, 76.27), new VSC(4, 0.95, 77.96), new VSC(3, 4.98, 85.83), new VSC(3, 4.13, 52.69), new VSC(3, 0.37, 78.71), new VSC(2, 0.86, 145.63), new VSC(2, 5.66, 9.56) };

	public static VSC[] g_L4UranusCoefficients = { new VSC(114, 3.142, 0), new VSC(6, 4.58, 74.78), new VSC(3, 0.35, 11.05), new VSC(1, 3.42, 56.62) };


	public static VSC[] g_B0UranusCoefficients = { new VSC(1346278, 2.6187781, 74.7815986), new VSC(62341, 5.08111, 149.56320), new VSC(61601, 3.14159, 0), new VSC(9964, 1.6160, 76.2661), new VSC(9926, 0.5763, 73.2971), new VSC(3259, 1.2612, 224.3448), new VSC(2972, 2.2437, 1.4845), new VSC(2010, 6.0555, 148.0787), new VSC(1522, 0.2796, 63.7359), new VSC(924, 4.038, 151.048), new VSC(761, 6.140, 71.813), new VSC(522, 3.321, 138.517), new VSC(463, 0.743, 85.827), new VSC(437, 3.381, 529.691), new VSC(435, 0.341, 77.751), new VSC(431, 3.554, 213.299), new VSC(420, 5.213, 11.046), new VSC(245, 0.788, 2.969), new VSC(233, 2.257, 222.860), new VSC(216, 1.591, 38.133), new VSC(180, 3.725, 299.126), new VSC(175, 1.236, 146.594), new VSC(174, 1.937, 380.128), new VSC(160, 5.336, 111.430), new VSC(144, 5.962, 35.164), new VSC(116, 5.739, 70.849), new VSC(106, 0.941, 70.328), new VSC(102, 2.619, 78.714) };

	public static VSC[] g_B1UranusCoefficients = { new VSC(206366, 4.123943, 74.781599), new VSC(8563, 0.3382, 149.5632), new VSC(1726, 2.1219, 73.2971), new VSC(1374, 0, 0), new VSC(1369, 3.0686, 76.2661), new VSC(451, 3.777, 1.484), new VSC(400, 2.848, 224.345), new VSC(307, 1.255, 148.079), new VSC(154, 3.786, 63.736), new VSC(112, 5.573, 151.048), new VSC(111, 5.329, 138.517), new VSC(83, 3.59, 71.81), new VSC(56, 3.40, 85.83), new VSC(54, 1.70, 77.75), new VSC(42, 1.21, 11.05), new VSC(41, 4.45, 78.71), new VSC(32, 3.77, 222.86), new VSC(30, 2.56, 2.97), new VSC(27, 5.34, 213.30), new VSC(26, 0.42, 380.13) };

	public static VSC[] g_B2UranusCoefficients = { new VSC(9212, 5.8004, 74.7816), new VSC(557, 0, 0), new VSC(286, 2.177, 149.563), new VSC(95, 3.84, 73.30), new VSC(45, 4.88, 76.27), new VSC(20, 5.46, 1.48), new VSC(15, 0.88, 138.52), new VSC(14, 2.85, 148.08), new VSC(14, 5.07, 63.74), new VSC(10, 5.00, 224.34), new VSC(8, 6.27, 78.71) };

	public static VSC[] g_B3UranusCoefficients = { new VSC(268, 1.251, 74.782), new VSC(11, 3.14, 0), new VSC(6, 4.01, 149.56), new VSC(3, 5.78, 73.30) };


	public static VSC[] g_B4UranusCoefficients = { new VSC(6, 2.85, 74.78) };

	public static VSC[] g_R0UranusCoefficients = { new VSC(1921264848, 0, 0), new VSC(88784984, 5.60377527, 74.78159857), new VSC(3440836, 0.3283610, 73.2971259), new VSC(2055653, 1.7829517, 149.5631971), new VSC(649322, 4.522473, 76.266071), new VSC(602248, 3.860038, 63.735898), new VSC(496404, 1.401399, 454.909367), new VSC(338526, 1.580027, 138.517497), new VSC(243508, 1.570866, 71.812653), new VSC(190522, 1.998094, 1.484473), new VSC(161858, 2.791379, 148.078724), new VSC(143706, 1.383686, 11.045700), new VSC(93192, 0.17437, 36.64856), new VSC(89806, 3.66105, 109.94569), new VSC(71424, 4.24509, 224.34480), new VSC(46677, 1.39977, 35.16409), new VSC(39026, 3.36235, 277.03499), new VSC(39010, 1.66971, 70.84945), new VSC(36755, 3.88649, 146.59425), new VSC(30349, 0.70100, 151.04767), new VSC(29156, 3.18056, 77.75054), new VSC(25786, 3.78538, 85.82730), new VSC(25620, 5.25656, 380.12777), new VSC(22637, 0.72519, 529.69097), new VSC(20473, 2.79640, 70.32818), new VSC(20472, 1.55589, 202.25340), new VSC(17901, 0.55455, 2.96895), new VSC(15503, 5.35405, 38.13304), new VSC(14702, 4.90434, 108.46122), new VSC(12897, 2.62154, 111.43016), new VSC(12328, 5.96039, 127.47180), new VSC(11959, 1.75044, 984.60033), new VSC(11853, 0.99343, 52.69020), new VSC(11696, 3.29826, 3.93215), new VSC(11495, 0.43774, 65.22037), new VSC(10793, 1.42105, 213.29910), new VSC(9111, 4.9964, 62.2514), new VSC(8421, 5.2535, 222.8603), new VSC(8402, 5.0388, 415.5525), new VSC(7449, 0.7949, 351.8166), new VSC(7329, 3.9728, 183.2428), new VSC(6046, 5.6796, 78.7138), new VSC(5524, 3.1150, 9.5612), new VSC(5445, 5.1058, 145.1098), new VSC(5238, 2.6296, 33.6796), new VSC(4079, 3.2206, 340.7709), new VSC(3919, 4.2502, 39.6175), new VSC(3802, 6.1099, 184.7273), new VSC(3781, 3.4584, 456.3938), new VSC(3687, 2.4872, 453.4249), new VSC(3102, 4.1403, 219.8914), new VSC(2963, 0.8298, 56.6224), new VSC(2942, 0.4239, 299.1264), new VSC(2940, 2.1464, 137.0330), new VSC(2938, 3.6766, 140.0020), new VSC(2865, 0.3100, 12.5302), new VSC(2538, 4.8546, 131.4039), new VSC(2364, 0.4425, 554.0700), new VSC(2183, 2.9404, 305.3462) };

	public static VSC[] g_R1UranusCoefficients = { new VSC(1479896, 3.6720571, 74.7815986), new VSC(71212, 6.22601, 63.73590), new VSC(68627, 6.13411, 149.56320), new VSC(24060, 3.14159, 0), new VSC(21468, 2.60177, 76.26607), new VSC(20857, 5.24625, 11.04570), new VSC(11405, 0.01848, 70.84945), new VSC(7497, 0.4236, 73.2971), new VSC(4244, 1.4169, 85.8273), new VSC(3927, 3.1551, 71.8127), new VSC(3578, 2.3116, 224.3448), new VSC(3506, 2.5835, 138.5175), new VSC(3229, 5.2550, 3.9322), new VSC(3060, 0.1532, 1.4845), new VSC(2564, 0.9808, 148.0787), new VSC(2429, 3.9944, 52.6902), new VSC(1645, 2.6535, 127.4718), new VSC(1584, 1.4305, 78.7138), new VSC(1508, 5.0600, 151.0477), new VSC(1490, 2.6756, 56.6224), new VSC(1413, 4.5746, 202.2534), new VSC(1403, 1.3699, 77.7505), new VSC(1228, 1.0470, 62.2514), new VSC(1033, 0.2646, 131.4039), new VSC(992, 2.172, 65.220), new VSC(862, 5.055, 351.817), new VSC(744, 3.076, 35.164), new VSC(687, 2.499, 77.963), new VSC(647, 4.473, 70.328), new VSC(624, 0.863, 9.561), new VSC(604, 0.907, 984.600), new VSC(575, 3.231, 447.796), new VSC(562, 2.718, 462.023), new VSC(530, 5.917, 213.299), new VSC(528, 5.151, 2.969) };

	public static VSC[] g_R2UranusCoefficients = { new VSC(22440, 0.69953, 74.78160), new VSC(4727, 1.6990, 63.7359), new VSC(1682, 4.6483, 70.8494), new VSC(1650, 3.0966, 11.0457), new VSC(1434, 3.5212, 149.5632), new VSC(770, 0, 0), new VSC(500, 6.172, 76.266), new VSC(461, 0.767, 3.932), new VSC(390, 4.496, 56.622), new VSC(390, 5.527, 85.827), new VSC(292, 0.204, 52.690), new VSC(287, 3.534, 73.297), new VSC(273, 3.847, 138.517), new VSC(220, 1.964, 131.404), new VSC(216, 0.848, 77.963), new VSC(205, 3.248, 78.714), new VSC(149, 4.898, 127.472), new VSC(129, 2.081, 3.181) };

	public static VSC[] g_R3UranusCoefficients = { new VSC(1164, 4.7345, 74.7816), new VSC(212, 3.343, 63.736), new VSC(196, 2.980, 70.849), new VSC(105, 0.958, 11.046), new VSC(73, 1.00, 149.56), new VSC(72, 0.03, 56.62), new VSC(55, 2.59, 3.93), new VSC(36, 5.65, 77.96), new VSC(34, 3.82, 76.27), new VSC(32, 3.60, 131.40) };

	public static VSC[] g_R4UranusCoefficients = { new VSC(53, 3.01, 74.78), new VSC(10, 1.91, 56.62) };
}
//
//Module : AAURANUS.CPP
//Purpose: Implementation for the algorithms which obtain the heliocentric position of Uranus
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


////////////////////////////////// Includes ///////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAUranus
{
//Static methods

  /////////////////////////////// Implementation ////////////////////////////////
  
  public static double EclipticLongitude(double JD)
  {
	double rho = (JD - 2451545) / 365250;
	double rhosquared = rho *rho;
	double rhocubed = rhosquared *rho;
	double rho4 = rhocubed *rho;
  
	//Calculate L0
	int nL0Coefficients = GFX.g_L0UranusCoefficients.Length;
	double L0 = 0;
	int i;
	for (i =0; i<nL0Coefficients; i++)
	  L0 += GFX.g_L0UranusCoefficients[i].A * Math.Cos(GFX.g_L0UranusCoefficients[i].B + GFX.g_L0UranusCoefficients[i].C *rho);
  
	//Calculate L1
	int nL1Coefficients = GFX.g_L1UranusCoefficients.Length;
	double L1 = 0;
	for (i =0; i<nL1Coefficients; i++)
	  L1 += GFX.g_L1UranusCoefficients[i].A * Math.Cos(GFX.g_L1UranusCoefficients[i].B + GFX.g_L1UranusCoefficients[i].C *rho);
  
	//Calculate L2
	int nL2Coefficients = GFX.g_L2UranusCoefficients.Length;
	double L2 = 0;
	for (i =0; i<nL2Coefficients; i++)
	  L2 += GFX.g_L2UranusCoefficients[i].A * Math.Cos(GFX.g_L2UranusCoefficients[i].B + GFX.g_L2UranusCoefficients[i].C *rho);
  
	//Calculate L3
	int nL3Coefficients = GFX.g_L3UranusCoefficients.Length;
	double L3 = 0;
	for (i =0; i<nL3Coefficients; i++)
	  L3 += GFX.g_L3UranusCoefficients[i].A * Math.Cos(GFX.g_L3UranusCoefficients[i].B + GFX.g_L3UranusCoefficients[i].C *rho);
  
	//Calculate L4
	int nL4Coefficients = GFX.g_L4UranusCoefficients.Length;
	double L4 = 0;
	for (i =0; i<nL4Coefficients; i++)
	  L4 += GFX.g_L4UranusCoefficients[i].A * Math.Cos(GFX.g_L4UranusCoefficients[i].B + GFX.g_L4UranusCoefficients[i].C *rho);
  
  
	double @value = (L0 + L1 *rho + L2 *rhosquared + L3 *rhocubed + L4 *rho4) / 100000000;
  
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
	int nB0Coefficients = GFX.g_B0UranusCoefficients.Length;
	double B0 = 0;
	int i;
	for (i =0; i<nB0Coefficients; i++)
	  B0 += GFX.g_B0UranusCoefficients[i].A * Math.Cos(GFX.g_B0UranusCoefficients[i].B + GFX.g_B0UranusCoefficients[i].C *rho);
  
	//Calculate B1
	int nB1Coefficients = GFX.g_B1UranusCoefficients.Length;
	double B1 = 0;
	for (i =0; i<nB1Coefficients; i++)
	  B1 += GFX.g_B1UranusCoefficients[i].A * Math.Cos(GFX.g_B1UranusCoefficients[i].B + GFX.g_B1UranusCoefficients[i].C *rho);
  
	//Calculate B2
	int nB2Coefficients = GFX.g_B2UranusCoefficients.Length;
	double B2 = 0;
	for (i =0; i<nB2Coefficients; i++)
	  B2 += GFX.g_B2UranusCoefficients[i].A * Math.Cos(GFX.g_B2UranusCoefficients[i].B + GFX.g_B2UranusCoefficients[i].C *rho);
  
	//Calculate B3
	int nB3Coefficients = GFX.g_B3UranusCoefficients.Length;
	double B3 = 0;
	for (i =0; i<nB3Coefficients; i++)
	  B3 += GFX.g_B3UranusCoefficients[i].A * Math.Cos(GFX.g_B3UranusCoefficients[i].B + GFX.g_B3UranusCoefficients[i].C *rho);
  
	//Calculate B4
    int nB4Coefficients = GFX.g_B4UranusCoefficients.Length;
	double B4 = 0;
	for (i =0; i<nB4Coefficients; i++)
	  B4 += GFX.g_B4UranusCoefficients[i].A * Math.Cos(GFX.g_B4UranusCoefficients[i].B + GFX.g_B4UranusCoefficients[i].C *rho);
  
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
	double rho4 = rhocubed *rho;
  
	//Calculate R0
	int nR0Coefficients = GFX.g_R0UranusCoefficients.Length;
	double R0 = 0;
	int i;
	for (i =0; i<nR0Coefficients; i++)
	  R0 += GFX.g_R0UranusCoefficients[i].A * Math.Cos(GFX.g_R0UranusCoefficients[i].B + GFX.g_R0UranusCoefficients[i].C *rho);
  
	//Calculate R1
	int nR1Coefficients = GFX.g_R1UranusCoefficients.Length;
	double R1 = 0;
	for (i =0; i<nR1Coefficients; i++)
	  R1 += GFX.g_R1UranusCoefficients[i].A * Math.Cos(GFX.g_R1UranusCoefficients[i].B + GFX.g_R1UranusCoefficients[i].C *rho);
  
	//Calculate R2
	int nR2Coefficients = GFX.g_R2UranusCoefficients.Length;
	double R2 = 0;
	for (i =0; i<nR2Coefficients; i++)
	  R2 += GFX.g_R2UranusCoefficients[i].A * Math.Cos(GFX.g_R2UranusCoefficients[i].B + GFX.g_R2UranusCoefficients[i].C *rho);
  
	//Calculate R3
	int nR3Coefficients = GFX.g_R3UranusCoefficients.Length;
	double R3 = 0;
	for (i =0; i<nR3Coefficients; i++)
	  R3 += GFX.g_R3UranusCoefficients[i].A * Math.Cos(GFX.g_R3UranusCoefficients[i].B + GFX.g_R3UranusCoefficients[i].C *rho);
  
  //Calculate R4
    int nR4Coefficients = GFX.g_R4UranusCoefficients.Length;
	double R4 = 0;
	for (i =0; i<nR4Coefficients; i++)
	  R4 += GFX.g_R4UranusCoefficients[i].A * Math.Cos(GFX.g_R4UranusCoefficients[i].B + GFX.g_R4UranusCoefficients[i].C *rho);
  
	return (R0 + R1 *rho + R2 *rhosquared + R3 *rhocubed + R4 *rho4) / 100000000;
  }
}



////////////////////////////////// Macros / Defines ///////////////////////////

//public class VSOP87Coefficient
//{
//  public double A;
//  public double B;
//  public double C;
//}
