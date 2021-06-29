using GTANetworkAPI;

internal class TruckerJobInstance : PickupDropoffBasedJob
{
	public TruckerJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.TruckerJob, EAchievementID.TruckerJob, EAchievementID.CompleteTruckerJob, "Truck Driver", EDrivingTestType.Truck, EVehicleType.TruckerJob)
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
		AddPickupLocation(EScriptLocation.Paleto, new Vector3(18.10508, 6330.739, 31.23617));

		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1398.669, 3589.084, 34.94542));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(420.3484, 6614.366, 27.3458));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1906.496, 3699.552, 32.71631));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-140.9002, 6204.274, 31.22508));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(2760.469, 3467.625, 55.69442));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-151.0255, 6493.572, 29.7351));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(2006.482, 3762.943, 32.18078));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1583.273, 6444.374, 25.04703));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1864.238, 2606.13, 45.67202));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-434.4143, 5966.875, 31.56593));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-0.4074481, 6440.832, 31.42522));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1948.312, 3740.459, 32.33942));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(151.1127, 6626.36, 31.76549));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(2661.765, 2900.369, 36.34554));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(2690.139, 3277.664, 55.24052));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1750.315, 6394.609, 36.04994));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-394.3908, 6120.574, 31.27927));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(1887.7, 3699.869, 33.05156));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-341.317, 6072.761, 31.38014));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(424.1226, 6533.236, 27.68181));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(59.78469, 6513.269, 31.55206));
		AddDropoffLocation(EScriptLocation.Paleto, new Vector3(-116.611, 6248.488, 31.17808));

		// LS
		AddPickupLocation(EScriptLocation.LS, new Vector3(164.5886, -3074.742, 5.911865));

		AddDropoffLocation(EScriptLocation.LS, new Vector3(-538.1112, -1765.595, 20.89381));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-333.1519, -1365.677, 30.79421));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-235.996, -1304.942, 30.82359));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-184.4339, -1286.656, 30.79546));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-70.84825, -1376.596, 28.75905));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(93.44965, -1446.61, 28.67877));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-14.34553, -1457.763, 29.96605));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-85.16682, -1481.608, 31.84956));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-175.13, -1465.279, 31.31853));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(306.6277, -1380.634, 31.28445));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(576.6641, -1565.759, 27.95386));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(498.0019, -1728.11, 28.58524));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(473.9225, -1811.626, 27.43387));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(296.5551, -1995.438, 20.18701));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-387.2148, -1868.599, 20.02724));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-145.1793, -1772.65, 29.29529));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-5.011527, -1853.712, 23.91327));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(160.9682, -1815.889, 27.90582));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(194.3929, -1857.04, 26.59789));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(139.2566, -1875.538, 23.34427));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(249.0519, -1710.42, 28.53021));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(498.761, -1757.716, 27.89555));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(467.1305, -1127.553, 28.8179));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(464.683, -1073.094, 28.70823));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(453.2421, -949.6561, 27.83235));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(453.2421, -949.6561, 27.83235));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(409.397, -777.8199, 28.74489));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(450.6659, -533.3129, 27.64263));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(971.0029, 184.6181, 80.32901));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(638.3941, -39.71042, 78.35859));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(514.478, -110.8912, 61.94498));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(401.5565, -382.933, 46.22136));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(247.1897, -585.4383, 42.58165));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(137.0228, -881.1896, 29.9098));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(70.94069, -1054.551, 28.8469));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-118.846, -1219.111, 27.68081));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-300.1945, -1527.532, 27.01683));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-434.0282, -1702.023, 18.47917));
		AddDropoffLocation(EScriptLocation.LS, new Vector3(-674.4516, -1639.856, 24.0678));
	}

	public override void OnStartJob(bool b_IsResume)
	{

	}

	public override void OnQuitJob()
	{

	}

	public override int GetXP()
	{
		return m_Owner.Instance().TruckerJobXP;
	}

	public override int OnGainXP(CPlayer a_Player, int a_XPGained)
	{
		a_Player.TruckerJobXP += a_XPGained;
		Database.Functions.Jobs.SaveTruckerJobProgress(a_Player.ActiveCharacterDatabaseID, a_Player.TruckerJobXP);
		return a_Player.TruckerJobXP;
	}
}