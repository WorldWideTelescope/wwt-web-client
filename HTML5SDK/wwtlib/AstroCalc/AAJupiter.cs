using System;
public static partial class GFX
{

	public static VSC[] g_L0JupiterCoefficients = { new VSC(59954691, 0, 0), new VSC(9695899, 5.0619179, 529.6909651), new VSC(573610, 1.444062, 7.113547), new VSC(306389, 5.417347, 1059.381930), new VSC(97178, 4.14265, 632.78374), new VSC(72903, 3.64043, 522.57742), new VSC(64264, 3.41145, 103.09277), new VSC(39806, 2.29377, 419.48464), new VSC(38858, 1.27232, 316.39187), new VSC(27965, 1.78455, 536.80451), new VSC(13590, 5.77481, 1589.07290), new VSC(8769, 3.6300, 949.1756), new VSC(8246, 3.5823, 206.1855), new VSC(7368, 5.0810, 735.8765), new VSC(6263, 0.0250, 213.2991), new VSC(6114, 4.5132, 1162.4747), new VSC(5305, 4.1863, 1052.2684), new VSC(5305, 1.3067, 14.2271), new VSC(4905, 1.3208, 110.2063), new VSC(4647, 4.6996, 3.9322), new VSC(3045, 4.3168, 426.5982), new VSC(2610, 1.5667, 846.0828), new VSC(2028, 1.0638, 3.1814), new VSC(1921, 0.9717, 639.8973), new VSC(1765, 2.1415, 1066.4955), new VSC(1723, 3.8804, 1265.5675), new VSC(1633, 3.5820, 515.4639), new VSC(1432, 4.2968, 625.6702), new VSC(973, 4.098, 95.979), new VSC(884, 2.437, 412.371), new VSC(733, 6.085, 838.969), new VSC(731, 3.806, 1581.959), new VSC(709, 1.293, 742.990), new VSC(692, 6.134, 2118.764), new VSC(614, 4.109, 1478.867), new VSC(582, 4.540, 309.278), new VSC(495, 3.756, 323.505), new VSC(441, 2.958, 454.909), new VSC(417, 1.036, 2.488), new VSC(390, 4.897, 1692.166), new VSC(376, 4.703, 1368.660), new VSC(341, 5.715, 533.623), new VSC(330, 4.740, 0.048), new VSC(262, 1.877, 0.963), new VSC(261, 0.820, 380.128), new VSC(257, 3.724, 199.072), new VSC(244, 5.220, 728.763), new VSC(235, 1.227, 909.819), new VSC(220, 1.651, 543.918), new VSC(207, 1.855, 525.759), new VSC(202, 1.807, 1375.774), new VSC(197, 5.293, 1155.361), new VSC(175, 3.730, 942.062), new VSC(175, 3.226, 1898.351), new VSC(175, 5.910, 956.289), new VSC(158, 4.365, 1795.258), new VSC(151, 3.906, 74.782), new VSC(149, 4.377, 1685.052), new VSC(141, 3.136, 491.558), new VSC(138, 1.318, 1169.588), new VSC(131, 4.169, 1045.155), new VSC(117, 2.500, 1596.186), new VSC(117, 3.389, 0.521), new VSC(106, 4.554, 526.510) };

	public static VSC[] g_L1JupiterCoefficients = { new VSC(52993480757.0, 0, 0), new VSC(489741, 4.220667, 529.690965), new VSC(228919, 6.026475, 7.113547), new VSC(27655, 4.57266, 1059.38193), new VSC(20721, 5.45939, 522.57742), new VSC(12106, 0.16986, 536.80451), new VSC(6068, 4.4242, 103.0928), new VSC(5434, 3.9848, 419.4846), new VSC(4238, 5.8901, 14.2271), new VSC(2212, 5.2677, 206.1855), new VSC(1746, 4.9267, 1589.0729), new VSC(1296, 5.5513, 3.1814), new VSC(1173, 5.8565, 1052.2684), new VSC(1163, 0.5145, 3.9322), new VSC(1099, 5.3070, 515.4639), new VSC(1007, 0.4648, 735.8765), new VSC(1004, 3.1504, 426.5982), new VSC(848, 5.758, 110.206), new VSC(827, 4.803, 213.299), new VSC(816, 0.586, 1066.495), new VSC(725, 5.518, 639.897), new VSC(568, 5.989, 625.670), new VSC(474, 4.132, 412.371), new VSC(413, 5.737, 95.979), new VSC(345, 4.242, 632.784), new VSC(336, 3.732, 1162.475), new VSC(234, 4.035, 949.176), new VSC(234, 6.243, 309.278), new VSC(199, 1.505, 838.969), new VSC(195, 2.219, 323.505), new VSC(187, 6.086, 742.990), new VSC(184, 6.280, 543.918), new VSC(171, 5.417, 199.072), new VSC(131, 0.626, 728.763), new VSC(115, 0.680, 846.083), new VSC(115, 5.286, 2118.764), new VSC(108, 4.493, 956.289), new VSC(80, 5.82, 1045.15), new VSC(72, 5.34, 942.06), new VSC(70, 5.97, 532.87), new VSC(67, 5.73, 21.34), new VSC(66, 0.13, 526.51), new VSC(65, 6.09, 1581.96), new VSC(59, 0.59, 1155.36), new VSC(58, 0.99, 1596.19), new VSC(57, 5.97, 1169.59), new VSC(57, 1.41, 533.62), new VSC(55, 5.43, 10.29), new VSC(52, 5.73, 117.32), new VSC(52, 0.23, 1368.66), new VSC(50, 6.08, 525.76), new VSC(47, 3.63, 1478.87), new VSC(47, 0.51, 1265.57), new VSC(40, 4.16, 1692.17), new VSC(34, 0.10, 302.16), new VSC(33, 5.04, 220.41), new VSC(32, 5.37, 508.35), new VSC(29, 5.42, 1272.68), new VSC(29, 3.36, 4.67), new VSC(29, 0.76, 88.87), new VSC(25, 1.61, 831.86) };

	public static VSC[] g_L2JupiterCoefficients = { new VSC(47234, 4.32148, 7.11355), new VSC(38966, 0, 0), new VSC(30629, 2.93021, 529.69097), new VSC(3189, 1.0550, 522.5774), new VSC(2729, 4.8455, 536.8045), new VSC(2723, 3.4141, 1059.3819), new VSC(1721, 4.1873, 14.2271), new VSC(383, 5.768, 419.485), new VSC(378, 0.760, 515.464), new VSC(367, 6.055, 103.093), new VSC(337, 3.786, 3.181), new VSC(308, 0.694, 206.186), new VSC(218, 3.814, 1589.073), new VSC(199, 5.340, 1066.495), new VSC(197, 2.484, 3.932), new VSC(156, 1.406, 1052.268), new VSC(146, 3.814, 639.897), new VSC(142, 1.634, 426.598), new VSC(130, 5.837, 412.371), new VSC(117, 1.414, 625.670), new VSC(97, 4.03, 110.21), new VSC(91, 1.11, 95.98), new VSC(87, 2.52, 632.78), new VSC(79, 4.64, 543.92), new VSC(72, 2.22, 735.88), new VSC(58, 0.83, 199.07), new VSC(57, 3.12, 213.30), new VSC(49, 1.67, 309.28), new VSC(40, 4.02, 21.34), new VSC(40, 0.62, 323.51), new VSC(36, 2.33, 728.76), new VSC(29, 3.61, 10.29), new VSC(28, 3.24, 838.97), new VSC(26, 4.50, 742.99), new VSC(26, 2.51, 1162.47), new VSC(25, 1.22, 1045.15), new VSC(24, 3.01, 956.29), new VSC(19, 4.29, 532.87), new VSC(18, 0.81, 508.35), new VSC(17, 4.20, 2118.76), new VSC(17, 1.83, 526.51), new VSC(15, 5.81, 1596.19), new VSC(15, 0.68, 942.06), new VSC(15, 4.00, 117.32), new VSC(14, 5.95, 316.39), new VSC(14, 1.80, 302.16), new VSC(13, 2.52, 88.87), new VSC(13, 4.37, 1169.59), new VSC(11, 4.44, 525.76), new VSC(10, 1.72, 1581.96), new VSC(9, 2.18, 1155.36), new VSC(9, 3.29, 220.41), new VSC(9, 3.32, 831.86), new VSC(8, 5.76, 846.08), new VSC(8, 2.71, 533.62), new VSC(7, 2.18, 1265.57), new VSC(6, 0.50, 949.18) };

	public static VSC[] g_L3JupiterCoefficients = { new VSC(6502, 2.5986, 7.1135), new VSC(1357, 1.3464, 529.6910), new VSC(471, 2.475, 14.227), new VSC(417, 3.245, 536.805), new VSC(353, 2.974, 522.577), new VSC(155, 2.076, 1059.382), new VSC(87, 2.51, 515.46), new VSC(44, 0, 0), new VSC(34, 3.83, 1066.50), new VSC(28, 2.45, 206.19), new VSC(24, 1.28, 412.37), new VSC(23, 2.98, 543.92), new VSC(20, 2.10, 639.90), new VSC(20, 1.40, 419.48), new VSC(19, 1.59, 103.09), new VSC(17, 2.30, 21.34), new VSC(17, 2.60, 1589.07), new VSC(16, 3.15, 625.67), new VSC(16, 3.36, 1052.27), new VSC(13, 2.76, 95.98), new VSC(13, 2.54, 199.07), new VSC(13, 6.27, 426.60), new VSC(9, 1.76, 10.29), new VSC(9, 2.27, 110.21), new VSC(7, 3.43, 309.28), new VSC(7, 4.04, 728.76), new VSC(6, 2.52, 508.35), new VSC(5, 2.91, 1045.15), new VSC(5, 5.25, 323.51), new VSC(4, 4.30, 88.87), new VSC(4, 3.52, 302.16), new VSC(4, 4.09, 735.88), new VSC(3, 1.43, 956.29), new VSC(3, 4.36, 1596.19), new VSC(3, 1.25, 213.30), new VSC(3, 5.02, 838.97), new VSC(3, 2.24, 117.32), new VSC(2, 2.90, 742.99), new VSC(2, 2.36, 942.06) };

	public static VSC[] g_L4JupiterCoefficients = { new VSC(669, 0.853, 7.114), new VSC(114, 3.142, 0), new VSC(100, 0.743, 14.227), new VSC(50, 1.65, 536.80), new VSC(44, 5.82, 529.69), new VSC(32, 4.86, 522.58), new VSC(15, 4.29, 515.46), new VSC(9, 0.71, 1059.38), new VSC(5, 1.30, 543.92), new VSC(4, 2.32, 1066.50), new VSC(4, 0.48, 21.34), new VSC(3, 3.00, 412.37), new VSC(2, 0.40, 639.90), new VSC(2, 4.26, 199.07), new VSC(2, 4.91, 625.67), new VSC(2, 4.26, 206.19), new VSC(1, 5.26, 1052.27), new VSC(1, 4.72, 95.98), new VSC(1, 1.29, 1589.07) };

	public static VSC[] g_L5JupiterCoefficients = { new VSC(50, 5.26, 7.11), new VSC(16, 5.25, 14.23), new VSC(4, 0.01, 536.80), new VSC(2, 1.10, 522.58), new VSC(1, 3.14, 0) };


	public static VSC[] g_B0JupiterCoefficients = { new VSC(2268616, 3.5585261, 529.6909651), new VSC(110090, 0, 0), new VSC(109972, 3.908093, 1059.381930), new VSC(8101, 3.6051, 522.5774), new VSC(6438, 0.3063, 536.8045), new VSC(6044, 4.2588, 1589.0729), new VSC(1107, 2.9853, 1162.4747), new VSC(944, 1.675, 426.598), new VSC(942, 2.936, 1052.268), new VSC(894, 1.754, 7.114), new VSC(836, 5.179, 103.093), new VSC(767, 2.155, 632.784), new VSC(684, 3.678, 213.299), new VSC(629, 0.643, 1066.495), new VSC(559, 0.014, 846.083), new VSC(532, 2.703, 110.206), new VSC(464, 1.173, 949.176), new VSC(431, 2.608, 419.485), new VSC(351, 4.611, 2118.764), new VSC(132, 4.778, 742.990), new VSC(123, 3.350, 1692.166), new VSC(116, 1.387, 323.505), new VSC(115, 5.049, 316.392), new VSC(104, 3.701, 515.464), new VSC(103, 2.319, 1478.867), new VSC(102, 3.153, 1581.959) };

	public static VSC[] g_B1JupiterCoefficients = { new VSC(177352, 5.701665, 529.690965), new VSC(3230, 5.7794, 1059.3819), new VSC(3081, 5.4746, 522.5774), new VSC(2212, 4.7348, 536.8045), new VSC(1694, 3.1416, 0), new VSC(346, 4.746, 1052.268), new VSC(234, 5.189, 1066.495), new VSC(196, 6.186, 7.114), new VSC(150, 3.927, 1589.073), new VSC(114, 3.439, 632.784), new VSC(97, 2.91, 949.18), new VSC(82, 5.08, 1162.47), new VSC(77, 2.51, 103.09), new VSC(77, 0.61, 419.48), new VSC(74, 5.50, 515.46), new VSC(61, 5.45, 213.30), new VSC(50, 3.95, 735.88), new VSC(46, 0.54, 110.21), new VSC(45, 1.90, 846.08), new VSC(37, 4.70, 543.92), new VSC(36, 6.11, 316.39), new VSC(32, 4.92, 1581.96) };

	public static VSC[] g_B2JupiterCoefficients = { new VSC(8094, 1.4632, 529.6910), new VSC(813, 3.1416, 0), new VSC(742, 0.957, 522.577), new VSC(399, 2.899, 536.805), new VSC(342, 1.447, 1059.382), new VSC(74, 0.41, 1052.27), new VSC(46, 3.48, 1066.50), new VSC(30, 1.93, 1589.07), new VSC(29, 0.99, 515.46), new VSC(23, 4.27, 7.11), new VSC(14, 2.92, 543.92), new VSC(12, 5.22, 632.78), new VSC(11, 4.88, 949.18), new VSC(6, 6.21, 1045.15) };

	public static VSC[] g_B3JupiterCoefficients = { new VSC(252, 3.381, 529.691), new VSC(122, 2.733, 522.577), new VSC(49, 1.04, 536.80), new VSC(11, 2.31, 1052.27), new VSC(8, 2.77, 515.46), new VSC(7, 4.25, 1059.38), new VSC(6, 1.78, 1066.50), new VSC(4, 1.13, 543.92), new VSC(3, 3.14, 0) };


	public static VSC[] g_B4JupiterCoefficients = { new VSC(15, 4.53, 522.58), new VSC(5, 4.47, 529.69), new VSC(4, 5.44, 536.80), new VSC(3, 0, 0), new VSC(2, 4.52, 515.46), new VSC(1, 4.20, 1052.27) };

	public static VSC[] g_B5JupiterCoefficients = { new VSC(1, 0.09, 522.58) };

	public static VSC[] g_R0JupiterCoefficients = { new VSC(520887429, 0, 0), new VSC(25209327, 3.49108640, 529.69096509), new VSC(610600, 3.841154, 1059.381930), new VSC(282029, 2.574199, 632.783739), new VSC(187647, 2.075904, 522.577418), new VSC(86793, 0.71001, 419.48464), new VSC(72063, 0.21466, 536.80451), new VSC(65517, 5.97996, 316.39187), new VSC(30135, 2.16132, 949.17561), new VSC(29135, 1.67759, 103.09277), new VSC(23947, 0.27458, 7.11355), new VSC(23453, 3.54023, 735.87651), new VSC(22284, 4.19363, 1589.07290), new VSC(13033, 2.96043, 1162.47470), new VSC(12749, 2.71550, 1052.26838), new VSC(9703, 1.9067, 206.1855), new VSC(9161, 4.4135, 213.2991), new VSC(7895, 2.4791, 426.5982), new VSC(7058, 2.1818, 1265.5675), new VSC(6138, 6.2642, 846.0828), new VSC(5477, 5.6573, 639.8973), new VSC(4170, 2.0161, 515.4639), new VSC(4137, 2.7222, 625.6702), new VSC(3503, 0.5653, 1066.4955), new VSC(2617, 2.0099, 1581.9593), new VSC(2500, 4.5518, 838.9693), new VSC(2128, 6.1275, 742.9901), new VSC(1912, 0.8562, 412.3711), new VSC(1611, 3.0887, 1368.6603), new VSC(1479, 2.6803, 1478.8666), new VSC(1231, 1.8904, 323.5054), new VSC(1217, 1.8017, 110.2063), new VSC(1015, 1.3867, 454.9094), new VSC(999, 2.872, 309.278), new VSC(961, 4.549, 2118.764), new VSC(886, 4.148, 533.623), new VSC(821, 1.593, 1898.351), new VSC(812, 5.941, 909.819), new VSC(777, 3.677, 728.763), new VSC(727, 3.988, 1155.361), new VSC(655, 2.791, 1685.052), new VSC(654, 3.382, 1692.166), new VSC(621, 4.823, 956.289), new VSC(615, 2.276, 942.062), new VSC(562, 0.081, 543.918), new VSC(542, 0.284, 525.759) };

	public static VSC[] g_R1JupiterCoefficients = { new VSC(1271802, 2.6493751, 529.6909651), new VSC(61662, 3.00076, 1059.38193), new VSC(53444, 3.89718, 522.57742), new VSC(41390, 0, 0), new VSC(31185, 4.88277, 536.80451), new VSC(11847, 2.41330, 419.48464), new VSC(9166, 4.7598, 7.1135), new VSC(3404, 3.3469, 1589.0729), new VSC(3203, 5.2108, 735.8765), new VSC(3176, 2.7930, 103.0928), new VSC(2806, 3.7422, 515.4639), new VSC(2677, 4.3305, 1052.2684), new VSC(2600, 3.6344, 206.1855), new VSC(2412, 1.4695, 426.5982), new VSC(2101, 3.9276, 639.8973), new VSC(1646, 4.4163, 1066.4955), new VSC(1641, 4.4163, 625.6702), new VSC(1050, 3.1611, 213.2991), new VSC(1025, 2.5543, 412.3711), new VSC(806, 2.678, 632.784), new VSC(741, 2.171, 1162.475), new VSC(677, 6.250, 838.969), new VSC(567, 4.577, 742.990), new VSC(485, 2.469, 949.176), new VSC(469, 4.710, 543.918), new VSC(445, 0.403, 323.505), new VSC(416, 5.368, 728.763), new VSC(402, 4.605, 309.278), new VSC(347, 4.681, 14.227), new VSC(338, 3.168, 956.289), new VSC(261, 5.343, 846.083), new VSC(247, 3.923, 942.062), new VSC(220, 4.842, 1368.660), new VSC(203, 5.600, 1155.361), new VSC(200, 4.439, 1045.155), new VSC(197, 3.706, 2118.764), new VSC(196, 3.759, 199.072), new VSC(184, 4.265, 95.979), new VSC(180, 4.402, 532.872), new VSC(170, 4.846, 526.510), new VSC(146, 6.130, 533.623), new VSC(133, 1.322, 110.206), new VSC(132, 4.512, 525.759) };

	public static VSC[] g_R2JupiterCoefficients = { new VSC(79645, 1.35866, 529.69097), new VSC(8252, 5.7777, 522.5774), new VSC(7030, 3.2748, 536.8045), new VSC(5314, 1.8384, 1059.3819), new VSC(1861, 2.9768, 7.1135), new VSC(964, 5.480, 515.464), new VSC(836, 4.199, 419.485), new VSC(498, 3.142, 0), new VSC(427, 2.228, 639.897), new VSC(406, 3.783, 1066.495), new VSC(377, 2.242, 1589.073), new VSC(363, 5.368, 206.186), new VSC(342, 6.099, 1052.268), new VSC(339, 6.127, 625.670), new VSC(333, 0.003, 426.598), new VSC(280, 4.262, 412.371), new VSC(257, 0.963, 632.784), new VSC(230, 0.705, 735.877), new VSC(201, 3.069, 543.918), new VSC(200, 4.429, 103.093), new VSC(139, 2.932, 14.227), new VSC(114, 0.787, 728.763), new VSC(95, 1.70, 838.97), new VSC(86, 5.14, 323.51), new VSC(83, 0.06, 309.28), new VSC(80, 2.98, 742.99), new VSC(75, 1.60, 956.29), new VSC(70, 1.51, 213.30), new VSC(67, 5.47, 199.07), new VSC(62, 6.10, 1045.15), new VSC(56, 0.96, 1162.47), new VSC(52, 5.58, 942.06), new VSC(50, 2.72, 532.87), new VSC(45, 5.52, 508.35), new VSC(44, 0.27, 526.51), new VSC(40, 5.95, 95.98) };

	public static VSC[] g_R3JupiterCoefficients = { new VSC(3519, 6.0580, 529.6910), new VSC(1073, 1.6732, 536.8045), new VSC(916, 1.413, 522.577), new VSC(342, 0.523, 1059.382), new VSC(255, 1.196, 7.114), new VSC(222, 0.952, 515.464), new VSC(90, 3.14, 0), new VSC(69, 2.27, 1066.50), new VSC(58, 1.41, 543.92), new VSC(58, 0.53, 639.90), new VSC(51, 5.98, 412.37), new VSC(47, 1.58, 625.67), new VSC(43, 6.12, 419.48), new VSC(37, 1.18, 14.23), new VSC(34, 1.67, 1052.27), new VSC(34, 0.85, 206.19), new VSC(31, 1.04, 1589.07), new VSC(30, 4.63, 426.60), new VSC(21, 2.50, 728.76), new VSC(15, 0.89, 199.07), new VSC(14, 0.96, 508.35), new VSC(13, 1.50, 1045.15), new VSC(12, 2.61, 735.88), new VSC(12, 3.56, 323.51), new VSC(11, 1.79, 309.28), new VSC(11, 6.28, 956.29), new VSC(10, 6.26, 103.09), new VSC(9, 3.45, 838.97) };

	public static VSC[] g_R4JupiterCoefficients = { new VSC(129, 0.084, 536.805), new VSC(113, 4.249, 529.691), new VSC(83, 3.30, 522.58), new VSC(38, 2.73, 515.46), new VSC(27, 5.69, 7.11), new VSC(18, 5.40, 1059.38), new VSC(13, 6.02, 543.92), new VSC(9, 0.77, 1066.50), new VSC(8, 5.68, 14.23), new VSC(7, 1.43, 412.37), new VSC(6, 5.12, 639.90), new VSC(5, 3.34, 625.67), new VSC(3, 3.40, 1052.27), new VSC(3, 4.16, 728.76), new VSC(3, 2.90, 426.60) };

	public static VSC[] g_R5JupiterCoefficients = { new VSC(11, 4.75, 536.80), new VSC(4, 5.92, 522.58), new VSC(2, 5.57, 515.46), new VSC(2, 4.30, 543.92), new VSC(2, 3.69, 7.11), new VSC(2, 4.13, 1059.38), new VSC(2, 5.49, 1066.50) };
}
//
//Module : AAJUPITER.CPP
//Purpose: Implementation for the algorithms which obtain the heliocentric position of Uranus
//Created: PJN / 29-12-2003
//History: PJN / 31-05-2004 1) Added a missing coefficient to g_L1JupiterCoefficients array as used by 
//                          CAAJupiter::EclipticLongitude. Thanks to Brian Orme for reporting this problem.
//                          2) Added missing g_B5JupiterCoefficients[] in CAAJupiter::EclipticLatitude. Again 
//                          thanks to Brian Orme for reporting this problem.
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


/////////////////////// Includes //////////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAJupiter
{
//Static methods

  ////////////////////////// Implementation /////////////////////////////////////
  
  public static double EclipticLongitude(double JD)
  {
	double rho = (JD - 2451545) / 365250;
	double rhosquared = rho *rho;
	double rhocubed = rhosquared *rho;
	double rho4 = rhocubed *rho;
	double rho5 = rho4 *rho;
  
	//Calculate L0
	int nL0Coefficients = GFX.g_L0JupiterCoefficients.Length;
	double L0 = 0;
	int i;
	for (i =0; i<nL0Coefficients; i++)
	  L0 += GFX.g_L0JupiterCoefficients[i].A * Math.Cos(GFX.g_L0JupiterCoefficients[i].B + GFX.g_L0JupiterCoefficients[i].C *rho);
  
	//Calculate L1
	int nL1Coefficients = GFX.g_L1JupiterCoefficients.Length;
	double L1 = 0;
	for (i =0; i<nL1Coefficients; i++)
	  L1 += GFX.g_L1JupiterCoefficients[i].A * Math.Cos(GFX.g_L1JupiterCoefficients[i].B + GFX.g_L1JupiterCoefficients[i].C *rho);
  
	//Calculate L2
	int nL2Coefficients = GFX.g_L2JupiterCoefficients.Length;
	double L2 = 0;
	for (i =0; i<nL2Coefficients; i++)
	  L2 += GFX.g_L2JupiterCoefficients[i].A * Math.Cos(GFX.g_L2JupiterCoefficients[i].B + GFX.g_L2JupiterCoefficients[i].C *rho);
  
	//Calculate L3
	int nL3Coefficients = GFX.g_L3JupiterCoefficients.Length;
	double L3 = 0;
	for (i =0; i<nL3Coefficients; i++)
	  L3 += GFX.g_L3JupiterCoefficients[i].A * Math.Cos(GFX.g_L3JupiterCoefficients[i].B + GFX.g_L3JupiterCoefficients[i].C *rho);
  
	//Calculate L4
	int nL4Coefficients = GFX.g_L4JupiterCoefficients.Length;
	double L4 = 0;
	for (i =0; i<nL4Coefficients; i++)
	  L4 += GFX.g_L4JupiterCoefficients[i].A * Math.Cos(GFX.g_L4JupiterCoefficients[i].B + GFX.g_L4JupiterCoefficients[i].C *rho);
  
	//Calculate L5
    int nL5Coefficients = GFX.g_L5JupiterCoefficients.Length;
	double L5 = 0;
	for (i =0; i<nL5Coefficients; i++)
	  L5 += GFX.g_L5JupiterCoefficients[i].A * Math.Cos(GFX.g_L5JupiterCoefficients[i].B + GFX.g_L5JupiterCoefficients[i].C *rho);
  
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
	double rho5 = rho4 *rho;
  
	//Calculate B0
	int nB0Coefficients = GFX.g_B0JupiterCoefficients.Length;
	double B0 = 0;
	int i;
	for (i =0; i<nB0Coefficients; i++)
	  B0 += GFX.g_B0JupiterCoefficients[i].A * Math.Cos(GFX.g_B0JupiterCoefficients[i].B + GFX.g_B0JupiterCoefficients[i].C *rho);
  
	//Calculate B1
	int nB1Coefficients = GFX.g_B1JupiterCoefficients.Length;
	double B1 = 0;
	for (i =0; i<nB1Coefficients; i++)
	  B1 += GFX.g_B1JupiterCoefficients[i].A * Math.Cos(GFX.g_B1JupiterCoefficients[i].B + GFX.g_B1JupiterCoefficients[i].C *rho);
  
	//Calculate B2
	int nB2Coefficients = GFX.g_B2JupiterCoefficients.Length;
	double B2 = 0;
	for (i =0; i<nB2Coefficients; i++)
	  B2 += GFX.g_B2JupiterCoefficients[i].A * Math.Cos(GFX.g_B2JupiterCoefficients[i].B + GFX.g_B2JupiterCoefficients[i].C *rho);
  
	//Calculate B3
	int nB3Coefficients = GFX.g_B3JupiterCoefficients.Length;
	double B3 = 0;
	for (i =0; i<nB3Coefficients; i++)
	  B3 += GFX.g_B3JupiterCoefficients[i].A * Math.Cos(GFX.g_B3JupiterCoefficients[i].B + GFX.g_B3JupiterCoefficients[i].C *rho);
  
	//Calculate B4
	int nB4Coefficients = GFX.g_B4JupiterCoefficients.Length;
	double B4 = 0;
	for (i =0; i<nB4Coefficients; i++)
	  B4 += GFX.g_B4JupiterCoefficients[i].A * Math.Cos(GFX.g_B4JupiterCoefficients[i].B + GFX.g_B4JupiterCoefficients[i].C *rho);
  
	//Calculate B5
    int nB5Coefficients = GFX.g_B5JupiterCoefficients.Length;
	double B5 = 0;
	for (i =0; i<nB5Coefficients; i++)
	  B5 += GFX.g_B5JupiterCoefficients[i].A * Math.Cos(GFX.g_B5JupiterCoefficients[i].B + GFX.g_B5JupiterCoefficients[i].C *rho);
  
	double @value = (B0 + B1 *rho + B2 *rhosquared + B3 *rhocubed + B4 *rho4 + B5 *rho5) / 100000000;
  
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
	double rho5 = rho4 *rho;
  
	//Calculate R0
	int nR0Coefficients = GFX.g_R0JupiterCoefficients.Length;
	double R0 = 0;
	int i;
	for (i =0; i<nR0Coefficients; i++)
	  R0 += GFX.g_R0JupiterCoefficients[i].A * Math.Cos(GFX.g_R0JupiterCoefficients[i].B + GFX.g_R0JupiterCoefficients[i].C *rho);
  
	//Calculate R1
	int nR1Coefficients = GFX.g_R1JupiterCoefficients.Length;
	double R1 = 0;
	for (i =0; i<nR1Coefficients; i++)
	  R1 += GFX.g_R1JupiterCoefficients[i].A * Math.Cos(GFX.g_R1JupiterCoefficients[i].B + GFX.g_R1JupiterCoefficients[i].C *rho);
  
	//Calculate R2
	int nR2Coefficients = GFX.g_R2JupiterCoefficients.Length;
	double R2 = 0;
	for (i =0; i<nR2Coefficients; i++)
	  R2 += GFX.g_R2JupiterCoefficients[i].A * Math.Cos(GFX.g_R2JupiterCoefficients[i].B + GFX.g_R2JupiterCoefficients[i].C *rho);
  
	//Calculate R3
	int nR3Coefficients = GFX.g_R3JupiterCoefficients.Length;
	double R3 = 0;
	for (i =0; i<nR3Coefficients; i++)
	  R3 += GFX.g_R3JupiterCoefficients[i].A * Math.Cos(GFX.g_R3JupiterCoefficients[i].B + GFX.g_R3JupiterCoefficients[i].C *rho);
  
	//Calculate R4
	int nR4Coefficients = GFX.g_R4JupiterCoefficients.Length;
	double R4 = 0;
	for (i =0; i<nR4Coefficients; i++)
	  R4 += GFX.g_R4JupiterCoefficients[i].A * Math.Cos(GFX.g_R4JupiterCoefficients[i].B + GFX.g_R4JupiterCoefficients[i].C *rho);
  
  //Calculate R5
    int nR5Coefficients = GFX.g_R5JupiterCoefficients.Length;
	double R5 = 0;
	for (i =0; i<nR5Coefficients; i++)
	  R5 += GFX.g_R5JupiterCoefficients[i].A * Math.Cos(GFX.g_R5JupiterCoefficients[i].B + GFX.g_R5JupiterCoefficients[i].C *rho);
  
	return (R0 + R1 *rho + R2 *rhosquared + R3 *rhocubed + R4 *rho4 + R5 *rho5) / 100000000;
  }
}



//////////////////////// Macros / Defines ///////////////////////////////////////

//public class VSOP87Coefficient
//{
//  public double A;
//  public double B;
//  public double C;
//}
