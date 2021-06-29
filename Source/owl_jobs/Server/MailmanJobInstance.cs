using GTANetworkAPI;

internal class MailmanJobInstance : CollectionCheckpointBasedJob
{
	public MailmanJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.MailmanJob, EAchievementID.MailmanJob, EAchievementID.CompleteMailmanJob, "Mail Man", EDrivingTestType.Truck, EVehicleType.MailmanJob)
	{
		AddLevel(0, 29f);
		AddLevel(20, 38.0f);
		AddLevel(40, 51f);
		AddLevel(80, 56f);
		AddLevel(160, 65f);
		AddLevel(320, 74f);
		AddLevel(640, 83f);
		AddLevel(1280, 88.0f);
		AddLevel(2560, 95f);
		AddLevel(5120, 106f);
		AddLevel(10240, 115f);
		AddLevel(20480, 124f);
		AddLevel(30480, 128f);
		AddLevel(40960, 133f);
		AddLevel(51000, 136f);
		AddLevel(61000, 139f);
		AddLevel(72000, 143f);
		AddLevel(82000, 146f);
		AddLevel(104000, 150f);
		AddLevel(156000, 150f);
		AddLevel(234000, 150f);
		AddLevel(351000, 150f);

		// Paleto
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-5.034743, 6271.901, 31.27281));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2461.537, 4056.878, 37.87043));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2871.638, 4426.412, 48.76958));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-116.434, 6457.086, 31.4517));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-294.684, 6210.465, 31.31779));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2452.83, 4952.807, 45.04192));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-154.8956, 6332.181, 31.45379));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(419.7825, 6520.436, 27.73717));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-147.0803, 6312.454, 31.42136));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2518.208, 4201.771, 39.93089));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2003.17, 3777.395, 32.18077));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(123.3784, 6621.774, 31.82631));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-115.191, 6307.512, 31.35974));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-143.913, 6438.643, 31.42903));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1694.051, 6428.07, 32.62836));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-327.7036, 6196.056, 31.25098));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(910.9073, 3641.078, 32.43159));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-93.18853, 6326.438, 31.49035));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-128.8859, 6395.817, 31.32581));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2504.626, 4098.48, 38.48603));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1589.324, 6446.171, 25.16196));

		// LS
		AddCheckpoint(EScriptLocation.LS, new Vector3(0.6916767, 26.15554, 70.79933));
		AddCheckpoint(EScriptLocation.LS, new Vector3(59.66439, 233.0135, 109.1446));
		AddCheckpoint(EScriptLocation.LS, new Vector3(223.2156, 173.6558, 105.1321));
		AddCheckpoint(EScriptLocation.LS, new Vector3(436.4651, 219.8918, 103.0767));
		AddCheckpoint(EScriptLocation.LS, new Vector3(638.6725, 261.5479, 103.0258));
		AddCheckpoint(EScriptLocation.LS, new Vector3(686.42, 54.99591, 83.43892));
		AddCheckpoint(EScriptLocation.LS, new Vector3(659.0838, -20.51158, 82.44743));
		AddCheckpoint(EScriptLocation.LS, new Vector3(512.7133, -109.5424, 62.38327));
		AddCheckpoint(EScriptLocation.LS, new Vector3(294.6102, -231.3645, 53.78329));
		AddCheckpoint(EScriptLocation.LS, new Vector3(221.1087, -362.3884, 44.01449));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1088.348, -343.1558, 67.1407));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1050.669, -408.8528, 66.65397));
		AddCheckpoint(EScriptLocation.LS, new Vector3(937.6679, -488.4733, 59.99811));
		AddCheckpoint(EScriptLocation.LS, new Vector3(915.6893, -603.2977, 57.26278));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1076.975, -765.073, 57.61903));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1199.004, -668.7979, 61.10488));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1126.509, -508.7554, 63.91337));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1088.664, -413.5257, 66.93353));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1170.437, -321.0658, 69.07857));
		AddCheckpoint(EScriptLocation.LS, new Vector3(943.9801, -138.7228, 74.43201));
		AddCheckpoint(EScriptLocation.LS, new Vector3(933.6298, -2.305403, 78.6655));
		AddCheckpoint(EScriptLocation.LS, new Vector3(417.6685, 131.1733, 100.9625));
		AddCheckpoint(EScriptLocation.LS, new Vector3(272.7484, 181.1746, 104.4389));
		AddCheckpoint(EScriptLocation.LS, new Vector3(55.21828, 263.6976, 109.3086));
		AddCheckpoint(EScriptLocation.LS, new Vector3(19.27844, 372.1127, 112.2464));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-95.21512, 426.0685, 112.9653));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-183.4929, 267.6864, 92.41058));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-306.0833, 310.123, 93.15866));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-484.3245, 260.5401, 82.79254));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-582.1349, 409.9408, 100.5639));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-580.536, 514.6072, 106.0783));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-712.0242, 484.8637, 108.2011));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-861.4983, 515.8369, 89.12727));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-964.0808, 592.4033, 101.3309));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1120.25, 569.4316, 102.3393));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1250.223, 486.9235, 93.95483));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1088.055, 444.9608, 75.46796));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1014.815, 494.5471, 79.20844));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-958.379, 441.5634, 79.56622));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1242.701, 387.8851, 75.49521));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1891.482, 180.3153, 82.02445));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1870.609, 334.8146, 88.4834));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1794.378, 355.7101, 88.48768));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1672.45, 397.955, 88.89799));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1958.885, 266.1207, 85.76591));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1921.182, 193.2204, 84.08028));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1818.28, -329.5199, 43.25099));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1562.237, -523.8068, 35.47458));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1404.477, -420.6049, 36.25903));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1316.989, -368.3723, 36.49401));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1357.236, -533.4367, 30.43738));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1375.933, -577.1558, 29.99508));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1500.818, -623.9254, 29.89079));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1639.323, -989.6493, 12.92035));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1610.79, -971.9968, 12.91989));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1366.096, -700.6088, 24.63196));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1294.8, -649.975, 26.30883));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1153.283, -811.6437, 14.76127));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1265.228, -893.4376, 11.23292));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1322.141, -836.8093, 16.72722));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1380.405, -972.8818, 8.72783));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1278.561, -1251.711, 3.832897));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1132.57, -1524.07, 4.182903));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1043.785, -1644.764, 4.342072));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-944.7007, -1527.706, 4.970713));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-993.373, -1441.216, 4.986144));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-925.5621, -1292.874, 4.921079));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1017.581, -1090.54, 1.771354));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1074.09, -993.6085, 1.950285));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1051.24, -893.1631, 4.620686));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-805.9407, -1070.499, 11.66231));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-659.1486, -1058.585, 16.56959));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-686.8438, -952.3442, 20.17326));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-712.8441, -866.6226, 23.20198));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-572.9143, -963.0203, 22.96419));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-588.6431, -1111.533, 22.07873));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-657.7794, -1379.977, 10.41416));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1127.298, -1988.777, 13.06996));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-925.2755, -2129.002, 8.866995));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-431.4786, -2160.95, 10.11962));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-129.1201, -2172.55, 10.11339));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-105.7794, -1769.907, 29.5365));
		AddCheckpoint(EScriptLocation.LS, new Vector3(47.93812, -1898.468, 21.44238));
		AddCheckpoint(EScriptLocation.LS, new Vector3(163.7496, -1895.536, 23.0473));
		AddCheckpoint(EScriptLocation.LS, new Vector3(257.672, -1907.249, 25.6969));
		AddCheckpoint(EScriptLocation.LS, new Vector3(446.2285, -1860.337, 27.52365));
		AddCheckpoint(EScriptLocation.LS, new Vector3(409.0931, -1774.218, 28.97178));
		AddCheckpoint(EScriptLocation.LS, new Vector3(483.9253, -1667.976, 29.05985));
		AddCheckpoint(EScriptLocation.LS, new Vector3(747.951, -1754.789, 29.13969));
		AddCheckpoint(EScriptLocation.LS, new Vector3(852.266, -1571.704, 30.12912));
		AddCheckpoint(EScriptLocation.LS, new Vector3(771.7255, -1409.957, 26.49928));
		AddCheckpoint(EScriptLocation.LS, new Vector3(326.3823, -1301.445, 31.55111));
		AddCheckpoint(EScriptLocation.LS, new Vector3(162.6701, -1363.85, 29.09326));
		AddCheckpoint(EScriptLocation.LS, new Vector3(104.0535, -1277.24, 28.98015));
		AddCheckpoint(EScriptLocation.LS, new Vector3(15.90392, -1126.066, 28.64783));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-70.42748, -1087.272, 26.53414));
		AddCheckpoint(EScriptLocation.LS, new Vector3(69.61009, -710.3851, 44.009));
		AddCheckpoint(EScriptLocation.LS, new Vector3(147.0354, -593.0154, 43.76188));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-72.80082, -715.6195, 33.82122));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-330.2661, -648.8228, 32.23662));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-555.1802, -648.272, 33.02917));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-619.1868, -395.6979, 34.53981));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-569.176, -383.8764, 34.81974));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-528.3698, -326.968, 34.94193));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-611.4172, -315.9723, 34.61765));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-679.9686, -241.869, 36.51867));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-641.1812, -197.0141, 37.46869));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-667.5629, -99.02015, 37.63866));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-414.8094, -71.83281, 42.87523));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-209.4055, -76.94453, 50.48809));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-31.1249, -137.958, 56.86944));
		AddCheckpoint(EScriptLocation.LS, new Vector3(112.6205, -76.33815, 64.40728));
		AddCheckpoint(EScriptLocation.LS, new Vector3(78.39409, -2.299758, 68.37676));
	}

	public override void OnStartJob(bool b_IsResume)
	{

	}

	public override void OnQuitJob()
	{

	}

	public override int GetXP()
	{
		return m_Owner.Instance().MailmanJobXP;
	}

	public override int OnGainXP(CPlayer a_Player, int a_XPGained)
	{
		a_Player.MailmanJobXP += a_XPGained;
		Database.Functions.Jobs.SaveMailmanJobProgress(a_Player.ActiveCharacterDatabaseID, a_Player.MailmanJobXP);
		return a_Player.MailmanJobXP;
	}
}