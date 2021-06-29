using GTANetworkAPI;

internal class BusDriverJobInstance : CollectionCheckpointBasedJob
{
	public BusDriverJobInstance(CPlayer a_Owner) : base(a_Owner, EJobID.BusDriverJob, EAchievementID.BusDriverJob, EAchievementID.CompleteBusDriverJob, "Bus Driver", EDrivingTestType.Truck, EVehicleType.BusDriverJob)
	{
		AddLevel(0, 38f);
		AddLevel(20, 55f);
		AddLevel(40, 72f);
		AddLevel(80, 83f);
		AddLevel(160, 95f);
		AddLevel(320, 106f);
		AddLevel(640, 114f);
		AddLevel(1280, 127f);
		AddLevel(2560, 137f);
		AddLevel(5120, 149f);
		AddLevel(10240, 161f);
		AddLevel(20480, 171f);
		AddLevel(30480, 176f);
		AddLevel(40960, 180f);
		AddLevel(51000, 184f);
		AddLevel(61000, 187f);
		AddLevel(72000, 190f);
		AddLevel(82000, 193f);
		AddLevel(104000, 195f);

		// Paleto
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(589.3021, 2722.035, 42.01276));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-216.5897, 6173.23, 31.22741));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2759.391, 3467.123, 55.71585));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(-150.4544, 6212.776, 31.18928));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1985.805, 3069.778, 46.97776));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(119.8821, 6618.032, 31.84575));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1579.545, 6437.806, 24.80836));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1696.162, 6399.793, 32.20768));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(440.2532, 6556.843, 27.07615));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2690.399, 3271.159, 55.24052));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1186.623, 2690.575, 37.76112));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1856.589, 2585.718, 45.67202));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(2918.966, 4392.889, 49.75348));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(1398.669, 3589.084, 34.94542));
		AddCheckpoint(EScriptLocation.Paleto, new Vector3(58.94267, 6417.659, 31.23779));

		// LS
		AddCheckpoint(EScriptLocation.LS, new Vector3(306.1565f, -766.0368f, 28.80099f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(168.3184f, -1356.856f, 28.8022f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-108.42f, -1686.923f, 28.80928f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1048.877930f, -2540.111572f, 12.505210f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1024.295, -2728.884, 13.27309));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-307.695801f, -1844.540430f, 23.845625f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-43.776791f, -1648.477661f, 28.033178f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(50.928295f, -1536.593506f, 28.018265f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(98.416206f, -1055.010620f, 28.118307f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(115.008423f, -784.036377f, 30.126572f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-220.4883, -592.3074, 33.84079));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-256.629272f, -330.119690f, 28.699272f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-490.618530f, 20.407391f, 43.791027f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-693.450562f, -5.167409f, 37.019169f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-931.669128f, -126.633087f, 36.415554f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1280.711, -408.1713, 34.79701));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1047.878540f, -389.459473f, 36.419994f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-679.639771f, -376.881226f, 33.043865f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-652.388367f, -607.065369f, 32.059444f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-558.416321f, -846.186951f, 26.312037f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-250.537796f, -883.167053f, 29.415934f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-338.2216f, -1477.553f, 30.21726f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-91.2199f, -1281.961f, 28.76797f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(355.0814f, -867.5811f, 28.78633f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1113.844f, -754.746f, 57.27937f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(896.4986f, -577.2581f, 56.83137f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(882.4374f, -327.7453f, 62.72161f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(293.9703f, -446.2111f, 43.07431f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(221.6114f, -853.0338f, 29.69249f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(261.568085f, -1217.424927f, 28.424841f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(364.583069f, -1568.484009f, 28.203512f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(574.867859f, -1734.191040f, 28.182583f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(774.118408f, -1752.040039f, 28.409100f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(879.431213f, -1766.265015f, 28.894323f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1303.761475f, -1648.289551f, 50.393467f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(1191.970215f, -1421.047852f, 34.044270f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(806.741638f, -1353.140259f, 25.286720f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(785.882019f, -775.321411f, 25.329002f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(428.806915f, -359.720764f, 46.116802f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(160.195572f, -372.077209f, 41.672024f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(41.333294f, -706.174377f, 43.126556f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-335.592316f, -683.291138f, 31.849171f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-86.832825f, -652.732300f, 35.105892f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(70.441124f, -627.019653f, 30.579842f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(62.284081f, -996.830688f, 28.256859f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-67.13174f, -1129.682f, 25.2938f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-602.4213f, -952.3894f, 21.65525f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1096.855f, -908.3715f, 2.694396f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1345.148f, -1031.592f, 7.368973f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1287.866f, -1224.986f, 4.061595f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1167.743652f, -1472.403931f, 3.280961f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1087.516235f, -1586.963745f, 3.321031f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-944.724731f, -1527.015015f, 4.056648f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-993.357361f, -1441.655884f, 4.072592f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-830.088196f, -1218.166382f, 5.931358f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-800.468201f, -1332.293945f, 3.997653f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-524.743774f, -1198.608643f, 17.541819f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-521.956665f, -1302.051392f, 27.012049f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-322.942200f, -1443.668823f, 29.714195f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-21.008894f, -1378.070068f, 28.275986f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(77.380333f, -1214.512207f, 28.123667f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(95.186195f, -1061.527832f, 28.265455f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(115.118439f, -783.993042f, 30.291632f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-466.381317f, -649.065796f, 31.306229f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-770.386658f, -647.734436f, 28.840813f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-931.685364f, -465.941254f, 36.085449f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1077.113647f, -264.329651f, 36.719830f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1280.407349f, -321.272278f, 35.672688f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1417.084595f, -399.028290f, 35.194302f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1525.448242f, -465.898956f, 34.293213f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1538.372559f, -683.898926f, 27.764669f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1561.036f, -1022.239f, 12.63167f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-1239.073f, -459.2118f, 33.02444f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-759.4496f, -346.2801f, 35.51202f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-316.5447f, -409.123f, 29.54764f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(105.7413f, -320.5405f, 45.27924f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(290.073639f, -588.705994f, 42.999905f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(393.089783f, -198.228836f, 59.249481f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(546.343018f, 154.992188f, 98.938354f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(707.297607f, 661.271912f, 128.741226f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(340.571075f, 473.769196f, 149.536407f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(342.4215f, 867.4611f, 195.1471f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(233.522186f, 1175.608643f, 225.289536f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(134.589f, 959.6533f, 210.7024f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-75.519508f, 893.577209f, 235.406143f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-718.873962f, 960.467590f, 237.414627f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-918.196106f, 793.625732f, 183.889343f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-917.567871f, 697.311035f, 151.466461f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-686.764465f, 604.516663f, 143.481186f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-629.844666f, 685.028870f, 150.348328f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-510.327759f, 569.082642f, 118.609016f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-696.260254f, 489.169250f, 109.245049f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-837.081909f, 292.296692f, 86.154709f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-481.763062f, 224.086395f, 82.916214f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-256.527252f, 12.497521f, 52.772953f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-296.263885f, -271.570099f, 31.390156f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(-286.271729f, -616.369568f, 33.201347f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(119.872200f, -812.574402f, 30.998062f));
		AddCheckpoint(EScriptLocation.LS, new Vector3(466.0186f, -621.4604f, 28.11433f));
	}

	public override void OnStartJob(bool b_IsResume)
	{

	}

	public override void OnQuitJob()
	{

	}

	public override int OnGainXP(CPlayer a_Player, int a_XPGained)
	{
		a_Player.BusDriverJobXP += a_XPGained;
		Database.Functions.Jobs.SaveBusDriverJobProgress(a_Player.ActiveCharacterDatabaseID, a_Player.BusDriverJobXP);
		return a_Player.BusDriverJobXP;
	}

	public override int GetXP()
	{
		return m_Owner.Instance().BusDriverJobXP;
	}
}