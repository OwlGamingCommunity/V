using GTANetworkAPI;

internal class TrashmanJobInstance : CollectionCheckpointBasedJob
{
	public TrashmanJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.TrashmanJob, EAchievementID.TrashManJob, EAchievementID.CompleteTrashManJob, "Trash Collector", EDrivingTestType.Truck, EVehicleType.TrashmanJob)
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

		// Paleto
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(54.66718, 2801.625, 57.8783));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1596.879, 6451.174, 25.31715));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-579.8198, 5233.137, 70.47031));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-73.59917, 6500.086, 31.4909));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-140.3387, 6344.219, 31.49082));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-1121.407, 2676.265, 18.33458));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1714.64, 6423.971, 32.81604));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-396.955, 6094.372, 31.47312));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-196.7185, 6535.054, 11.09785));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1383.213, 3601.035, 34.89481));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-409.4032, 6377.749, 13.97398));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-134.6287, 6473.14, 31.46846));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(35.7863, 6292.487, 31.23637));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-839.2763, 5407.418, 34.56978));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2525.571, 4208.993, 39.95332));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-437.1004, 6143.132, 31.47833));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2675.957, 3464.75, 55.65289));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2458.573, 4052.805, 37.97593));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1383.213, 3601.035, 34.89481));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1978.754, 3782.153, 32.18081));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2867.376, 4411.578, 49.15493));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-31.91054, 6526.638, 31.49084));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(96.17446, 6520.92, 31.39542));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-2532.76, 2323.183, 33.05991));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2653.844, 3271.885, 55.24202));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-684.5876, 5785.366, 17.33095));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(925.5278, 3652.575, 32.58303));

		// LS
		AddCheckpoint(EScriptLocation.LS, new Vector3(-664.5407, -1206.6173, 10.062434));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-718.2902, -1142.1907, 10.050781));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-844.6337, -1142.047, 6.2428703));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-914.6196, -1162.9697, 4.2639437));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-997.92535, -1125.2694, 1.5361629));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1059.9624, -1018.27826, 1.4957817));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1040.5724, -887.5175, 4.640918));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1165.1454, -821.72687, 13.684777));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1268.6741, -1026.6438, 8.375735));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1145.3927, -1379.4122, 4.4137926));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1026.3435, -1562.9927, 4.42977));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1194.0416, -1490.0403, 3.8188105));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1254.3733, -1289.6221, 3.2855675));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1331.0372, -1037.8274, 6.9789143));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1477.7125, -687.7086, 26.076548));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1591.3591, -477.7519, 35.40165));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1631.9478, 71.48921, 61.556973));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1499.9305, 49.370716, 53.988518));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-594.55084, 250.70546, 81.55999));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-451.01474, 236.01451, 82.385086));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-404.37292, 207.67174, 82.57615));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-183.06012, 247.30408, 92.02546));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-33.220024, 217.20078, 105.991585));
		AddCheckpoint(EScriptLocation.LS, new Vector3(234.90799, 173.68732, 104.612854));
		AddCheckpoint(EScriptLocation.LS, new Vector3(814.65906, -80.64755, 79.940315));
		AddCheckpoint(EScriptLocation.LS, new Vector3(966.42487, -130.4363, 73.81086));
		AddCheckpoint(EScriptLocation.LS, new Vector3(973.16516, -266.16226, 66.399666));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1053.2886, -407.13226, 66.26443));
		AddCheckpoint(EScriptLocation.LS, new Vector3(960.0872, -477.3484, 60.897835));
		AddCheckpoint(EScriptLocation.LS, new Vector3(913.40826, -600.251, 56.75415));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1081.4553, -764.4095, 57.06647));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1299.4583, -659.91516, 66.29687));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1332.569, -567.74207, 72.8322));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1340.4752, -560.5222, 73.12981));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1250.2072, -342.06808, 68.5205));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1178.9349, -456.59998, 65.75957));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1180.9437, -685.2099, 60.09944));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1152.1316, -1009.5295, 44.03008));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1221.5474, -1229.0433, 34.987747));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1212.4977, -1372.2346, 34.74785));
		AddCheckpoint(EScriptLocation.LS, new Vector3(945.96106, -1423.5208, 30.754522));
		AddCheckpoint(EScriptLocation.LS, new Vector3(825.087, -1632.5409, 30.080227));
		AddCheckpoint(EScriptLocation.LS, new Vector3(787.4226, -1733.3351, 28.875618));
		AddCheckpoint(EScriptLocation.LS, new Vector3(497.40384, -1733.0411, 28.441961));
		AddCheckpoint(EScriptLocation.LS, new Vector3(480.44034, -1803.4254, 27.78492));
		AddCheckpoint(EScriptLocation.LS, new Vector3(411.99625, -1875.9984, 25.673058));
		AddCheckpoint(EScriptLocation.LS, new Vector3(333.27768, -1890.6587, 25.157106));
		AddCheckpoint(EScriptLocation.LS, new Vector3(199.93204, -1858.6572, 26.537867));
		AddCheckpoint(EScriptLocation.LS, new Vector3(38.53329, -1817.8116, 24.234552));
		AddCheckpoint(EScriptLocation.LS, new Vector3(7.428187, -1617.8207, 28.683016));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-97.236885, -1672.9426, 28.613756));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-178.51596, -1668.6407, 32.63476));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-147.87915, -1564.3824, 34.064156));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-50.77997, -1470.552, 31.375952));
		AddCheckpoint(EScriptLocation.LS, new Vector3(117.80273, -1456.914, 28.637308));
		AddCheckpoint(EScriptLocation.LS, new Vector3(233.28857, -1199.8083, 28.653845));
		AddCheckpoint(EScriptLocation.LS, new Vector3(339.3477, -1076.4346, 28.96135));
		AddCheckpoint(EScriptLocation.LS, new Vector3(408.16287, -922.45337, 28.724253));
		AddCheckpoint(EScriptLocation.LS, new Vector3(411.03833, -782.4735, 28.597857));
		AddCheckpoint(EScriptLocation.LS, new Vector3(122.16656, -569.4954, 30.962769));
		AddCheckpoint(EScriptLocation.LS, new Vector3(43.57126, -610.25977, 31.065256));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-85.05279, -1023.56696, 27.60664));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-214.31514, -1117.7849, 22.385292));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-291.48343, -1317.7384, 30.584263));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-315.94275, -1526.7189, 27.010143));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-422.1432, -1710.2848, 18.804445));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-532.7344, -1757.9551, 20.858496));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-640.93726, -1677.4321, 24.46016));
	}

	public override void OnStartJob(bool b_IsResume)
	{

	}

	public override void OnQuitJob()
	{

	}

	public override int GetXP()
	{
		return m_Owner.Instance().TrashmanJobXP;
	}

	public override int OnGainXP(CPlayer a_Player, int a_XPGained)
	{
		a_Player.TrashmanJobXP += a_XPGained;
		Database.Functions.Jobs.SaveTrashmanJobProgress(a_Player.ActiveCharacterDatabaseID, a_Player.TrashmanJobXP);
		return a_Player.TrashmanJobXP;
	}
}