using System;
public static partial class GFX
{
    //public static double modf(double orig, ref double intpart)
    //{
    //    return orig - (intpart = Math.Floor(orig));
    //}
	public static VSC[] g_L0EarthCoefficients = { new VSC(175347046, 0, 0), new VSC(3341656, 4.6692568, 6283.0758500), new VSC(34894, 4.62610, 12566.15170), new VSC(3497, 2.7441, 5753.3849), new VSC(3418, 2.8289, 3.5231), new VSC(3136, 3.6277, 77713.7715), new VSC(2676, 4.4181, 7860.4194), new VSC(2343, 6.1352, 3930.2097), new VSC(1324, 0.7425, 11506.7698), new VSC(1273, 2.0371, 529.6910), new VSC(1199, 1.1096, 1577.3435), new VSC(990, 5.233, 5884.927), new VSC(902, 2.045, 26.298), new VSC(857, 3.508, 398.149), new VSC(780, 1.179, 5223.694), new VSC(753, 2.533, 5507.553), new VSC(505, 4.583, 18849.228), new VSC(492, 4.205, 775.523), new VSC(357, 2.920, 0.067), new VSC(317, 5.849, 11790.629), new VSC(284, 1.899, 796.288), new VSC(271, 0.315, 10977.079), new VSC(243, 0.345, 5486.778), new VSC(206, 4.806, 2544.314), new VSC(205, 1.869, 5573.143), new VSC(202, 2.458, 6069.777), new VSC(156, 0.833, 213.299), new VSC(132, 3.411, 2942.463), new VSC(126, 1.083, 20.775), new VSC(115, 0.645, 0.980), new VSC(103, 0.636, 4694.003), new VSC(102, 0.976, 15720.839), new VSC(102, 4.267, 7.114), new VSC(99, 6.21, 2146.17), new VSC(98, 0.68, 155.42), new VSC(86, 5.98, 161000.69), new VSC(85, 1.30, 6275.96), new VSC(85, 3.67, 71430.70), new VSC(80, 1.81, 17260.15), new VSC(79, 3.04, 12036.46), new VSC(75, 1.76, 5088.63), new VSC(74, 3.50, 3154.69), new VSC(74, 4.68, 801.82), new VSC(70, 0.83, 9437.76), new VSC(62, 3.98, 8827.39), new VSC(61, 1.82, 7084.90), new VSC(57, 2.78, 6286.60), new VSC(56, 4.39, 14143.50), new VSC(56, 3.47, 6279.55), new VSC(52, 0.19, 12139.55), new VSC(52, 1.33, 1748.02), new VSC(51, 0.28, 5856.48), new VSC(49, 0.49, 1194.45), new VSC(41, 5.37, 8429.24), new VSC(41, 2.40, 19651.05), new VSC(39, 6.17, 10447.39), new VSC(37, 6.04, 10213.29), new VSC(37, 2.57, 1059.38), new VSC(36, 1.71, 2352.87), new VSC(36, 1.78, 6812.77), new VSC(33, 0.59, 17789.85), new VSC(30, 0.44, 83996.85), new VSC(30, 2.74, 1349.87), new VSC(25, 3.16, 4690.48) };

	public static VSC[] g_L1EarthCoefficients = { new VSC(628331966747.0, 0, 0), new VSC(206059, 2.678235, 6283.075850), new VSC(4303, 2.6351, 12566.1517), new VSC(425, 1.590, 3.523), new VSC(119, 5.796, 26.298), new VSC(109, 2.966, 1577.344), new VSC(93, 2.59, 18849.23), new VSC(72, 1.14, 529.69), new VSC(68, 1.87, 398.15), new VSC(67, 4.41, 5507.55), new VSC(59, 2.89, 5223.69), new VSC(56, 2.17, 155.42), new VSC(45, 0.40, 796.30), new VSC(36, 0.47, 775.52), new VSC(29, 2.65, 7.11), new VSC(21, 5.43, 0.98), new VSC(19, 1.85, 5486.78), new VSC(19, 4.97, 213.30), new VSC(17, 2.99, 6275.96), new VSC(16, 0.03, 2544.31), new VSC(16, 1.43, 2146.17), new VSC(15, 1.21, 10977.08), new VSC(12, 2.83, 1748.02), new VSC(12, 3.26, 5088.63), new VSC(12, 5.27, 1194.45), new VSC(12, 2.08, 4694.00), new VSC(11, 0.77, 553.57), new VSC(10, 1.30, 6286.60), new VSC(10, 4.24, 1349.87), new VSC(9, 2.70, 242.73), new VSC(9, 5.64, 951.72), new VSC(8, 5.30, 2352.87), new VSC(6, 2.65, 9437.76), new VSC(6, 4.67, 4690.48) };

	public static VSC[] g_L2EarthCoefficients = { new VSC(52919, 0, 0), new VSC(8720, 1.0721, 6283.0758), new VSC(309, 0.867, 12566.152), new VSC(27, 0.05, 3.52), new VSC(16, 5.19, 26.30), new VSC(16, 3.68, 155.42), new VSC(10, 0.76, 18849.23), new VSC(9, 2.06, 77713.77), new VSC(7, 0.83, 775.52), new VSC(5, 4.66, 1577.34), new VSC(4, 1.03, 7.11), new VSC(4, 3.44, 5573.14), new VSC(3, 5.14, 796.30), new VSC(3, 6.05, 5507.55), new VSC(3, 1.19, 242.73), new VSC(3, 6.12, 529.69), new VSC(3, 0.31, 398.15), new VSC(3, 2.28, 553.57), new VSC(2, 4.38, 5223.69), new VSC(2, 3.75, 0.98) };

	public static VSC[] g_L3EarthCoefficients = { new VSC(289, 5.844, 6283.076), new VSC(35, 0, 0), new VSC(17, 5.49, 12566.15), new VSC(3, 5.20, 155.42), new VSC(1, 4.72, 3.52), new VSC(1, 5.30, 18849.23), new VSC(1, 5.97, 242.73) };

	public static VSC[] g_L4EarthCoefficients = { new VSC(114, 3.142, 0), new VSC(8, 4.13, 6283.08), new VSC(1, 3.84, 12566.15) };

	public static VSC[] g_L5EarthCoefficients = { new VSC(1, 3.14, 0) };


	public static VSC[] g_B0EarthCoefficients = { new VSC(280, 3.199, 84334.662), new VSC(102, 5.422, 5507.553), new VSC(80, 3.88, 5223.69), new VSC(44, 3.70, 2352.87), new VSC(32, 4.00, 1577.34) };

	public static VSC[] g_B1EarthCoefficients = { new VSC(9, 3.90, 5507.55), new VSC(6, 1.73, 5223.69) };

	public static VSC[] g_B2EarthCoefficients = { new VSC(22378, 3.38509, 10213.28555), new VSC(282, 0, 0), new VSC(173, 5.256, 20426.571), new VSC(27, 3.87, 30639.86) };

	public static VSC[] g_B3EarthCoefficients = { new VSC(647, 4.992, 10213.286), new VSC(20, 3.14, 0), new VSC(6, 0.77, 20426.57), new VSC(3, 5.44, 30639.86) };

	public static VSC[] g_B4EarthCoefficients = { new VSC(14, 0.32, 10213.29) };


	public static VSC[] g_R0EarthCoefficients = { new VSC(100013989, 0, 0), new VSC(1670700, 3.0984635, 6283.0758500), new VSC(13956, 3.05525, 12566.15170), new VSC(3084, 5.1985, 77713.7715), new VSC(1628, 1.1739, 5753.3849), new VSC(1576, 2.8469, 7860.4194), new VSC(925, 5.453, 11506.770), new VSC(542, 4.564, 3930.210), new VSC(472, 3.661, 5884.927), new VSC(346, 0.964, 5507.553), new VSC(329, 5.900, 5223.694), new VSC(307, 0.299, 5573.143), new VSC(243, 4.273, 11790.629), new VSC(212, 5.847, 1577.344), new VSC(186, 5.022, 10977.079), new VSC(175, 3.012, 18849.228), new VSC(110, 5.055, 5486.778), new VSC(98, 0.89, 6069.78), new VSC(86, 5.69, 15720.84), new VSC(86, 1.27, 161000.69), new VSC(65, 0.27, 17260.15), new VSC(63, 0.92, 529.69), new VSC(57, 2.01, 83996.85), new VSC(56, 5.24, 71430.70), new VSC(49, 3.25, 2544.31), new VSC(47, 2.58, 775.52), new VSC(45, 5.54, 9437.76), new VSC(43, 6.01, 6275.96), new VSC(39, 5.36, 4694.00), new VSC(38, 2.39, 8827.39), new VSC(37, 0.83, 19651.05), new VSC(37, 4.90, 12139.55), new VSC(36, 1.67, 12036.46), new VSC(35, 1.84, 2942.46), new VSC(33, 0.24, 7084.90), new VSC(32, 0.18, 5088.63), new VSC(32, 1.78, 398.15), new VSC(28, 1.21, 6286.60), new VSC(28, 1.90, 6279.55), new VSC(26, 4.59, 10447.39) };

	public static VSC[] g_R1EarthCoefficients = { new VSC(103019, 1.107490, 6283.075850), new VSC(1721, 1.0644, 12566.1517), new VSC(702, 3.142, 0), new VSC(32, 1.02, 18849.23), new VSC(31, 2.84, 5507.55), new VSC(25, 1.32, 5223.69), new VSC(18, 1.42, 1577.34), new VSC(10, 5.91, 10977.08), new VSC(9, 1.42, 6275.96), new VSC(9, 0.27, 5486.78) };

	public static VSC[] g_R2EarthCoefficients = { new VSC(4359, 5.7846, 6283.0758), new VSC(124, 5.579, 12566.152), new VSC(12, 3.14, 0), new VSC(9, 3.63, 77713.77), new VSC(6, 1.87, 5573.14), new VSC(3, 5.47, 18849.23) };

	public static VSC[] g_R3EarthCoefficients = { new VSC(145, 4.273, 6283.076), new VSC(7, 3.92, 12566.15) };

	public static VSC[] g_R4EarthCoefficients = { new VSC(4, 2.56, 6283.08) };


	public static VSC[] g_L1EarthCoefficientsJ2000 = { new VSC(628307584999.0, 0, 0), new VSC(206059, 2.678235, 6283.075850), new VSC(4303, 2.6351, 12566.1517), new VSC(425, 1.590, 3.523), new VSC(119, 5.796, 26.298), new VSC(109, 2.966, 1577.344), new VSC(93, 2.59, 18849.23), new VSC(72, 1.14, 529.69), new VSC(68, 1.87, 398.15), new VSC(67, 4.41, 5507.55), new VSC(59, 2.89, 5223.69), new VSC(56, 2.17, 155.42), new VSC(45, 0.40, 796.30), new VSC(36, 0.47, 775.52), new VSC(29, 2.65, 7.11), new VSC(21, 5.43, 0.98), new VSC(19, 1.85, 5486.78), new VSC(19, 4.97, 213.30), new VSC(17, 2.99, 6275.96), new VSC(16, 0.03, 2544.31), new VSC(16, 1.43, 2146.17), new VSC(15, 1.21, 10977.08), new VSC(12, 2.83, 1748.02), new VSC(12, 3.26, 5088.63), new VSC(12, 5.27, 1194.45), new VSC(12, 2.08, 4694.00), new VSC(11, 0.77, 553.57), new VSC(10, 1.30, 6286.60), new VSC(10, 4.24, 1349.87), new VSC(9, 2.70, 242.73), new VSC(9, 5.64, 951.72), new VSC(8, 5.30, 2352.87), new VSC(6, 2.65, 9437.76), new VSC(6, 4.67, 4690.48) };

	public static VSC[] g_L2EarthCoefficientsJ2000 = { new VSC(8722, 1.0725, 6283.0758), new VSC(991, 3.1416, 0), new VSC(295, 0.437, 12566.152), new VSC(27, 0.05, 3.52), new VSC(16, 5.19, 26.30), new VSC(16, 3.69, 155.42), new VSC(9, 0.30, 18849.23), new VSC(9, 2.06, 77713.77), new VSC(7, 0.83, 775.52), new VSC(5, 4.66, 1577.34), new VSC(4, 1.03, 7.11), new VSC(4, 3.44, 5573.14), new VSC(3, 5.14, 796.30), new VSC(3, 6.05, 5507.55), new VSC(3, 1.19, 242.73), new VSC(3, 6.12, 529.69), new VSC(3, 0.30, 398.15), new VSC(3, 2.28, 553.57), new VSC(2, 4.38, 5223.69), new VSC(2, 3.75, 0.98) };

	public static VSC[] g_L3EarthCoefficientsJ2000 = { new VSC(289, 5.842, 6283.076), new VSC(21, 6.05, 12566.15), new VSC(3, 5.20, 155.42), new VSC(3, 3.14, 0), new VSC(1, 4.72, 3.52), new VSC(1, 5.97, 242.73), new VSC(1, 5.54, 18849.23) };

	public static VSC[] g_L4EarthCoefficientsJ2000 = { new VSC(8, 4.14, 6283.08), new VSC(1, 3.28, 12566.15) };



	public static VSC[] g_B1EarthCoefficientsJ2000 = { new VSC(227778, 3.413766, 6283.075850), new VSC(3806, 3.3706, 12566.1517), new VSC(3620, 0, 0), new VSC(72, 3.33, 18849.23), new VSC(8, 3.89, 5507.55), new VSC(8, 1.79, 5223.69), new VSC(6, 5.20, 2352.87) };

	public static VSC[] g_B2EarthCoefficientsJ2000 = { new VSC(9721, 5.1519, 6283.07585), new VSC(233, 3.1416, 0), new VSC(134, 0.644, 12566.152), new VSC(7, 1.07, 18849.23) };

	public static VSC[] g_B3EarthCoefficientsJ2000 = { new VSC(276, 0.595, 6283.076), new VSC(17, 3.14, 0), new VSC(4, 0.12, 12566.15) };

	public static VSC[] g_B4EarthCoefficientsJ2000 = { new VSC(6, 2.27, 6283.08), new VSC(1, 0, 0) };
}
//
//Module : AAEARTH.CPP
//Purpose: Implementation for the algorithms which calculate the position of Earth
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


///////////////////////////// Includes ////////////////////////////////////////



//////////////////////// Classes //////////////////////////////////////////////

public class CAAEarth
{
    //Static methods
    public static double EclipticLongitude(double JD)
    {
        double rho = (JD - 2451545) / 365250;
        double rhosquared = rho * rho;
        double rhocubed = rhosquared * rho;
        double rho4 = rhocubed * rho;
        double rho5 = rho4 * rho;

        //Calculate L0
        int nL0Coefficients = GFX.g_L0EarthCoefficients.Length;
        double L0 = 0;
        int i;
        for (i = 0; i < nL0Coefficients; i++)
            L0 += GFX.g_L0EarthCoefficients[i].A * Math.Cos(GFX.g_L0EarthCoefficients[i].B + GFX.g_L0EarthCoefficients[i].C * rho);

        //Calculate L1
        int nL1Coefficients = GFX.g_L1EarthCoefficients.Length;
        double L1 = 0;
        for (i = 0; i < nL1Coefficients; i++)
            L1 += GFX.g_L1EarthCoefficients[i].A * Math.Cos(GFX.g_L1EarthCoefficients[i].B + GFX.g_L1EarthCoefficients[i].C * rho);

        //Calculate L2
        int nL2Coefficients = GFX.g_L2EarthCoefficients.Length;
        double L2 = 0;
        for (i = 0; i < nL2Coefficients; i++)
            L2 += GFX.g_L2EarthCoefficients[i].A * Math.Cos(GFX.g_L2EarthCoefficients[i].B + GFX.g_L2EarthCoefficients[i].C * rho);

        //Calculate L3
        int nL3Coefficients = GFX.g_L3EarthCoefficients.Length;
        double L3 = 0;
        for (i = 0; i < nL3Coefficients; i++)
            L3 += GFX.g_L3EarthCoefficients[i].A * Math.Cos(GFX.g_L3EarthCoefficients[i].B + GFX.g_L3EarthCoefficients[i].C * rho);

        //Calculate L4
        int nL4Coefficients = GFX.g_L4EarthCoefficients.Length;
        double L4 = 0;
        for (i = 0; i < nL4Coefficients; i++)
            L4 += GFX.g_L4EarthCoefficients[i].A * Math.Cos(GFX.g_L4EarthCoefficients[i].B + GFX.g_L4EarthCoefficients[i].C * rho);

        //Calculate L5
        int nL5Coefficients = GFX.g_L5EarthCoefficients.Length;
        double L5 = 0;
        for (i = 0; i < nL5Coefficients; i++)
            L5 += GFX.g_L5EarthCoefficients[i].A * Math.Cos(GFX.g_L5EarthCoefficients[i].B + GFX.g_L5EarthCoefficients[i].C * rho);

        double @value = (L0 + L1 * rho + L2 * rhosquared + L3 * rhocubed + L4 * rho4 + L5 * rho5) / 100000000;

        //convert results back to degrees
        @value = CT.M360(CT.R2D(@value));
        return @value;
    }
    public static double EclipticLatitude(double JD)
    {
        double rho = (JD - 2451545) / 365250;
        double rhosquared = rho * rho;
        double rhocubed = rhosquared * rho;
        double rho4 = rhocubed * rho;

        //Calculate B0
        int nB0Coefficients = GFX.g_B0EarthCoefficients.Length;
        double B0 = 0;
        int i;
        for (i = 0; i < nB0Coefficients; i++)
            B0 += GFX.g_B0EarthCoefficients[i].A * Math.Cos(GFX.g_B0EarthCoefficients[i].B + GFX.g_B0EarthCoefficients[i].C * rho);

        //Calculate B1
        int nB1Coefficients = GFX.g_B1EarthCoefficients.Length;
        double B1 = 0;
        for (i = 0; i < nB1Coefficients; i++)
            B1 += GFX.g_B1EarthCoefficients[i].A * Math.Cos(GFX.g_B1EarthCoefficients[i].B + GFX.g_B1EarthCoefficients[i].C * rho);

        //Calculate B2
        int nB2Coefficients = GFX.g_B2EarthCoefficients.Length;
        double B2 = 0;
        for (i = 0; i < nB2Coefficients; i++)
            B2 += GFX.g_B2EarthCoefficients[i].A * Math.Cos(GFX.g_B2EarthCoefficients[i].B + GFX.g_B2EarthCoefficients[i].C * rho);

        //Calculate B3
        int nB3Coefficients = GFX.g_B3EarthCoefficients.Length;
        double B3 = 0;
        for (i = 0; i < nB3Coefficients; i++)
            B3 += GFX.g_B3EarthCoefficients[i].A * Math.Cos(GFX.g_B3EarthCoefficients[i].B + GFX.g_B3EarthCoefficients[i].C * rho);

        //Calculate B4
        int nB4Coefficients = GFX.g_B4EarthCoefficients.Length;
        double B4 = 0;
        for (i = 0; i < nB4Coefficients; i++)
            B4 += GFX.g_B4EarthCoefficients[i].A * Math.Cos(GFX.g_B4EarthCoefficients[i].B + GFX.g_B4EarthCoefficients[i].C * rho);

        double @value = (B0 + B1 * rho + B2 * rhosquared + B3 * rhocubed + B4 * rho4) / 100000000;

        //convert results back to degrees
        @value = CT.R2D(@value);
        return @value;
    }
    public static double RadiusVector(double JD)
    {
        double rho = (JD - 2451545) / 365250;
        double rhosquared = rho * rho;
        double rhocubed = rhosquared * rho;
        double rho4 = rhocubed * rho;

        //Calculate R0
        int nR0Coefficients = GFX.g_R0EarthCoefficients.Length;
        double R0 = 0;
        int i;
        for (i = 0; i < nR0Coefficients; i++)
            R0 += GFX.g_R0EarthCoefficients[i].A * Math.Cos(GFX.g_R0EarthCoefficients[i].B + GFX.g_R0EarthCoefficients[i].C * rho);

        //Calculate R1
        int nR1Coefficients = GFX.g_R1EarthCoefficients.Length;
        double R1 = 0;
        for (i = 0; i < nR1Coefficients; i++)
            R1 += GFX.g_R1EarthCoefficients[i].A * Math.Cos(GFX.g_R1EarthCoefficients[i].B + GFX.g_R1EarthCoefficients[i].C * rho);

        //Calculate R2
        int nR2Coefficients = GFX.g_R2EarthCoefficients.Length;
        double R2 = 0;
        for (i = 0; i < nR2Coefficients; i++)
            R2 += GFX.g_R2EarthCoefficients[i].A * Math.Cos(GFX.g_R2EarthCoefficients[i].B + GFX.g_R2EarthCoefficients[i].C * rho);

        //Calculate R3
        int nR3Coefficients = GFX.g_R3EarthCoefficients.Length;
        double R3 = 0;
        for (i = 0; i < nR3Coefficients; i++)
            R3 += GFX.g_R3EarthCoefficients[i].A * Math.Cos(GFX.g_R3EarthCoefficients[i].B + GFX.g_R3EarthCoefficients[i].C * rho);

        //Calculate R4
        int nR4Coefficients = GFX.g_R4EarthCoefficients.Length;
        double R4 = 0;
        for (i = 0; i < nR4Coefficients; i++)
            R4 += GFX.g_R4EarthCoefficients[i].A * Math.Cos(GFX.g_R4EarthCoefficients[i].B + GFX.g_R4EarthCoefficients[i].C * rho);

        return (R0 + R1 * rho + R2 * rhosquared + R3 * rhocubed + R4 * rho4) / 100000000;
    }
    public static double SunMeanAnomaly(double JD)
    {
        double T = (JD - 2451545) / 36525;
        double Tsquared = T * T;
        double Tcubed = Tsquared * T;
        return CT.M360(357.5291092 + 35999.0502909 * T - 0.0001536 * Tsquared + Tcubed / 24490000);
    }
    public static double Eccentricity(double JD)
    {
        double T = (JD - 2451545) / 36525;
        double Tsquared = T * T;
        return 1 - 0.002516 * T - 0.0000074 * Tsquared;
    }
    public static double EclipticLongitudeJ2000(double JD)
    {
        double rho = (JD - 2451545) / 365250;
        double rhosquared = rho * rho;
        double rhocubed = rhosquared * rho;
        double rho4 = rhocubed * rho;

        //Calculate L0
        int nL0Coefficients = GFX.g_L0EarthCoefficients.Length;
        double L0 = 0;
        int i;
        for (i = 0; i < nL0Coefficients; i++)
            L0 += GFX.g_L0EarthCoefficients[i].A * Math.Cos(GFX.g_L0EarthCoefficients[i].B + GFX.g_L0EarthCoefficients[i].C * rho);

        //Calculate L1
        int nL1Coefficients = GFX.g_L1EarthCoefficientsJ2000.Length;
        double L1 = 0;
        for (i = 0; i < nL1Coefficients; i++)
            L1 += GFX.g_L1EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_L1EarthCoefficientsJ2000[i].B + GFX.g_L1EarthCoefficientsJ2000[i].C * rho);

        //Calculate L2
        int nL2Coefficients = GFX.g_L2EarthCoefficientsJ2000.Length;
        double L2 = 0;
        for (i = 0; i < nL2Coefficients; i++)
            L2 += GFX.g_L2EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_L2EarthCoefficientsJ2000[i].B + GFX.g_L2EarthCoefficientsJ2000[i].C * rho);

        //Calculate L3
        int nL3Coefficients = GFX.g_L3EarthCoefficientsJ2000.Length;
        double L3 = 0;
        for (i = 0; i < nL3Coefficients; i++)
            L3 += GFX.g_L3EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_L3EarthCoefficientsJ2000[i].B + GFX.g_L3EarthCoefficientsJ2000[i].C * rho);

        //Calculate L4
        int nL4Coefficients = GFX.g_L4EarthCoefficientsJ2000.Length;
        double L4 = 0;
        for (i = 0; i < nL4Coefficients; i++)
            L4 += GFX.g_L4EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_L4EarthCoefficientsJ2000[i].B + GFX.g_L4EarthCoefficientsJ2000[i].C * rho);

        double @value = (L0 + L1 * rho + L2 * rhosquared + L3 * rhocubed + L4 * rho4) / 100000000;

        //convert results back to degrees
        @value = CT.M360(CT.R2D(@value));
        return @value;
    }
    public static double EclipticLatitudeJ2000(double JD)
    {
        double rho = (JD - 2451545) / 365250;
        double rhosquared = rho * rho;
        double rhocubed = rhosquared * rho;
        double rho4 = rhocubed * rho;

        //Calculate B0
        int nB0Coefficients = GFX.g_B0EarthCoefficients.Length;
        double B0 = 0;
        int i;
        for (i = 0; i < nB0Coefficients; i++)
            B0 += GFX.g_B0EarthCoefficients[i].A * Math.Cos(GFX.g_B0EarthCoefficients[i].B + GFX.g_B0EarthCoefficients[i].C * rho);

        //Calculate B1
        int nB1Coefficients = GFX.g_B1EarthCoefficientsJ2000.Length;
        double B1 = 0;
        for (i = 0; i < nB1Coefficients; i++)
            B1 += GFX.g_B1EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_B1EarthCoefficientsJ2000[i].B + GFX.g_B1EarthCoefficientsJ2000[i].C * rho);

        //Calculate B2
        int nB2Coefficients = GFX.g_B2EarthCoefficientsJ2000.Length;
        double B2 = 0;
        for (i = 0; i < nB2Coefficients; i++)
            B2 += GFX.g_B2EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_B2EarthCoefficientsJ2000[i].B + GFX.g_B2EarthCoefficientsJ2000[i].C * rho);

        //Calculate B3
        int nB3Coefficients = GFX.g_B3EarthCoefficientsJ2000.Length;
        double B3 = 0;
        for (i = 0; i < nB3Coefficients; i++)
            B3 += GFX.g_B3EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_B3EarthCoefficientsJ2000[i].B + GFX.g_B3EarthCoefficientsJ2000[i].C * rho);

        //Calculate B4
        int nB4Coefficients = GFX.g_B4EarthCoefficientsJ2000.Length;
        double B4 = 0;
        for (i = 0; i < nB4Coefficients; i++)
            B4 += GFX.g_B4EarthCoefficientsJ2000[i].A * Math.Cos(GFX.g_B4EarthCoefficientsJ2000[i].B + GFX.g_B4EarthCoefficientsJ2000[i].C * rho);

        double @value = (B0 + B1 * rho + B2 * rhosquared + B3 * rhocubed + B4 * rho4) / 100000000;

        //convert results back to degrees
        @value = CT.R2D(@value);
        return @value;
    }
}



//////////////////////////// Macros / Defines /////////////////////////////////

public class VSC // VSOP87Coefficient
{
    public VSC(double a, double b, double c)
    {
        A = a;
        B = b;
        C = c;
    }
    public double A;
    public double B;
    public double C;
}
