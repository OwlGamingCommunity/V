using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

// NOTE: Must keep in sync
public enum WeaponHash : uint
{
	Sniperrifle = 100416529,
	Fireextinguisher = 101631238,
	Compactlauncher = 125959754,
	Snowball = 126349499,
	Vintagepistol = 137902532,
	Combatpdw = 171789620,
	Heavysniper = 205991906,
	Sweepershotgun = 317205821,
	Microsmg = 324215364,
	Wrench = 419712736,
	Pistol = 453432689,
	Pumpshotgun = 487013001,
	Appistol = 584646201,
	Ball = 600439132,
	Molotov = 615608432,
	Smg = 736523883,
	Stickybomb = 741814745,
	Petrolcan = 883325847,
	Stungun = 911657153,
	Heavyshotgun = 984333226,
	Minigun = 1119849093,
	Golfclub = 1141786504,
	Flaregun = 1198879012,
	Flare = 1233104067,
	Grenadelauncher_smoke = 1305664598,
	Hammer = 1317494643,
	Combatpistol = 1593441988,
	Gusenberg = 1627465347,
	Compactrifle = 1649403952,
	Hominglauncher = 1672152130,
	Nightstick = 1737195953,
	Railgun = 1834241177,
	Sawnoffshotgun = 2017895192,
	Bullpuprifle = 2132975508,
	Firework = 2138347493,
	Combatmg = 2144741730,
	Carbinerifle = 2210333304,
	Crowbar = 2227010557,
	Flashlight = 2343591895,
	Dagger = 2460120199,
	Grenade = 2481070269,
	Poolcue = 2484171525,
	Bat = 2508868239,
	Pistol50 = 2578377531,
	Knife = 2578778090,
	Mg = 2634544996,
	Bullpupshotgun = 2640438543,
	Bzgas = 2694266206,
	Unarmed = 2725352035,
	Grenadelauncher = 2726580491,
	Nightvision = 2803906140,
	Musket = 2828843422,
	Proximine = 2874559379,
	Advancedrifle = 2937143193,
	Rpg = 2982836145,
	Pipebomb = 3125143736,
	Minismg = 3173288789,
	Snspistol = 3218215474,
	Assaultrifle = 3220176749,
	Specialcarbine = 3231910285,
	Revolver = 3249783761,
	Marksmanrifle = 3342088282,
	Battleaxe = 3441901897,
	Heavypistol = 3523564046,
	Knuckle = 3638508604,
	Machinepistol = 3675956304,
	Marksmanpistol = 3696079510,
	Machete = 3713923289,
	Switchblade = 3756226112,
	Assaultshotgun = 3800352039,
	Dbshotgun = 4019527611,
	Assaultsmg = 4024951519,
	Hatchet = 4191993645,
	Bottle = 4192643659,
	Parachute = 4222310262,
	Smokegrenade = 4256991824,
	NavyRevolver = 0X917F6C8C,
	CeramicPistol = 0X2B5Ef5Ec,
	HazardCan = 0Xba536372,
	Stone_hatchet = 0x3813FC08,
	Pistol_mk2 = 0xBFE256D4,
	Snspistol_mk2 = 0x88374054,
	Raypistol = 0xAF3696A1,
	Smg_mk2 = 0x78A97CD0,
	Raycarbine = 0x476BF155,
	Pumpshotgun_mk2 = 0x555AF99A,
	Assaultrifle_mk2 = 0x394F415C,
	Specialcarbine_mk2 = 0x969C3D67,
	Bullpuprifle_mk2 = 0x84D6FAFD,
	Combatmg_mk2 = 0xDBBD7280,
	Heavysniper_mk2 = 0xA914799,
	Marksmanrifle_mk2 = 0x6A6C02E0,
	Rayminigun = 0xB62D1F67,
	Carbinerifle_mk2 = 0xFAD1F1C9,
	Revolver_mk2 = 0xCB96392F,
	Doubleaction = 0x97EA20B8,
	GadgetPistol = 0x57A4368C,
	MilitaryRifle = 0x9D1F17E6,
	CombatShotgun = 0x5A96BA4
}

public enum PedHash : uint
{
	Skidrow01AMM = 32417469,
	Hooker01SFY = 42647445,
	Hooker03SFY = 51789996,
	JimmyBostonCutscene = 60192701,
	SalvaGoon03GMY = 62440720,
	Autoshop01SMM = 68070371,
	Eastsa02AFY = 70821038,
	Car3Guy1Cutscene = 71501447,
	Clown01SMY = 71929310,
	TracyDisantoCutscene = 101298480,
	FloydCutscene = 103106535,
	Pigeon = 111281960,
	TigerShark = 113504370,
	Genfat01AMM = 115168927,
	AnitaCutscene = 117698822,
	Eastsa02AMM = 131961260,
	Indian01AFY = 153984193,
	MaryannCutscene = 161007533,
	Zimbor = 188012277,
	Baywatch01SMY = 189425762,
	PestContGunman = 193469166,
	Socenlat01AMM = 193817059,
	DaleCutscene = 216536661,
	Janet = 225287241,
	Michael = 225514697,
	Tanisha = 226559113,
	Corpse02UFY = 228356856,
	StrPunk02GMY = 228715206,
	AmmuCountrySMM = 233415434,
	Soucent03AMO = 238213328,
	Jewelass = 257763003,
	GuadalopeCutscene = 261428209,
	Chef01SMY = 261586155,
	ChiCold01GMM = 275618457,
	MountainLion = 307287994,
	Latino01AMY = 321657486,
	Car3Guy2Cutscene = 327394568,
	Genfat02AMM = 330231874,
	Fitness02AFY = 331645324,
	Hippie01AFY = 343259175,
	Hooker02SFY = 348382215,
	Hipster02AMY = 349505262,
	Dockwork01SMM = 349680864,
	Chop = 351016938,
	Ktown01AMO = 355916122,
	Paige = 357551935,
	Ballas01GFY = 361513884,
	DaveNorton = 365775923,
	Cop01SFY = 368603149,
	FemBarberSFM = 373000027,
	Eastsa02AMY = 377976310,
	PrologueHostage01AFM = 379310561,
	Tramp01AMO = 390939205,
	Crow = 402729631,
	RashkovskyCutscene = 411081129,
	GentransportSMM = 411102470,
	Marnie = 411185872,
	ShopKeep01 = 416176080,
	Hipster04AFY = 429425116,
	HeliStaff01 = 431423238,
	Vinewood01AFY = 435429221,
	Cntrybar01SMM = 436345731,
	Snowcop01SMM = 451459928,
	Ktown01AMY = 452351020,
	FosRepCutscene = 466359675,
	Strpreach01SMM = 469792763,
	Markfost = 479578891,
	MrsThornhill = 503621995,
	PrologueSec02Cutscene = 512955554,
	Tramp01AMM = 516505552,
	OldMan1aCutscene = 518814684,
	Business02AFM = 532905404,
	Vinewood03AMY = 534725268,
	Salton01AMO = 539004493,
	Bevhills03AFY = 549978415,
	RampHipsterCutscene = 569740212,
	Doorman01SMY = 579932932,
	MovPrem01SFY = 587253782,
	Hipster01AMY = 587703123,
	BallaOrig01GMY = 588969535,
	BallaSout01GMY = 599294057,
	Beach02AMY = 600300561,
	Poppymich = 602513566,
	Stwhi01AMY = 605602864,
	CarDesignFemale01 = 606876839,
	Chip = 610290475,
	Agent = 610988552,
	Korean01GMY = 611648169,
	Runner01AMY = 623927022,
	Popov = 645279998,
	Ortega = 648372919,
	AshleyCutscene = 650367097,
	MexGoon01GMY = 653210662,
	FibOffice02SMM = 653289389,
	SalvaGoon01GMY = 663522487,
	Business01AFY = 664399832,
	PrologueSec02 = 666086773,
	Lifeinvad02 = 666718676,
	CrisFormage = 678319271,
	Highsec02SMM = 691061163,
	StripperLite = 695248020,
	Ktown02AMY = 696250687,
	Indian01AMY = 706935758,
	EdToh = 712602007,
	Soucent01AMO = 718836251,
	FilmDirector = 728636342,
	FilmNoir = 732742363,
	Soucent01AFY = 744758650,
	Cyclist01 = 755956971,
	SlodSmallQuadped = 762327283,
	Downtown01AMY = 766375082,
	Jetski01AMY = 767028979,
	ONeil = 768005095,
	Corpse01UMY = 773063444,
	ReporterCutscene = 776079908,
	ChemSec01SMM = 788443093,
	DevinCutscene = 788622594,
	Genhot01AFY = 793439294,
	PornDudesCutscene = 793443893,
	Barry = 797459875,
	Fish = 802685111,
	Malibu01AMM = 803106487,
	JanetCutscene = 808778210,
	Beach01AFM = 808859815,
	MexThug01AMY = 810804565,
	Hiker01AFY = 813893651,
	Grip01SMY = 815693290,
	WeiChengCutscene = 819699067,
	Sweatshop01SFM = 824925120,
	Business02AFY = 826475330,
	MexGoon02GMY = 832784782,
	Vinewood04AMY = 835315305,
	SalvaGoon02GMY = 846439045,
	BoatStaff01F = 848542158,
	Lost03GMY = 850468060,
	Famdd01 = 866411749,
	FibArchitect = 874722259,
	Imporage = 880829941,
	Retriever = 882848737,
	Genstreet02AMY = 891398354,
	KorBoss01GMM = 891945583,
	MovieStar = 894928436,
	Stretch = 915948376,
	Stwhi02AMY = 919005580,
	Bevhills04AFY = 920595805,
	Party01 = 921110016,
	WeaponExpertMale01 = 921328393,
	Vinewood03AFY = 933092024,
	Breakdance01AMY = 933205398,
	AviSchwartzman = 939183526,
	Rashkovsky = 940330470,
	SteveHains = 941695432,
	Marston01 = 943915367,
	MrsPhillips = 946007720,
	LazlowCutscene = 949295643,
	FatWhite01AFM = 951767867,
	TerryCutscene = 978452933,
	FbiSuit01 = 988062523,
	PestContDriver = 994527967,
	Valet01SMY = 999748158,
	Rurmeth01AMM = 1001210244,
	Bodybuild01AFM = 1004114196,
	Maude = 1005070462,
	RsRanger01AMO = 1011059922,
	DreyfussCutscene = 1012965715,
	HammerShark = 1015224100,
	MimeSMY = 1021093698,
	RussianDrunk = 1024089777,
	Lost02GMY = 1032073858,
	Soucent01AFO = 1039800368,
	ExecutivePAMale01 = 1048844220,
	ShopMidSFY = 1055701597,
	SlodHuman = 1057201338,
	Rurmeth01AFY = 1064866854,
	Bevhills02AMM = 1068876755,
	Abigail = 1074457665,
	Beach01AMM = 1077785853,
	Soucent02AMO = 1082572151,
	Ktown02AFM = 1090617681,
	Scientist01SMM = 1092080539,
	Miranda = 1095737979,
	Sheriff01SFY = 1096929346,
	Factory01SMY = 1097048408,
	WeaponWorkerMale01 = 1099321454,
	Hairdress01SMM = 1099825042,
	TanishaCutscene = 1123963760,
	Poodle = 1125994524,
	Shepherd = 1126154828,
	ExecutivePAFemale01 = 1126998116,
	MartinMadrazoCutscene = 1129928304,
	Xmech01SMY = 1142162924,
	JewelassCutscene = 1145088004,
	Bevhills01AFY = 1146800212,
	JackHowitzerCutscene = 1153203121,
	JoshCutscene = 1158606749,
	ExArmy01 = 1161072059,
	LamarDavisCutscene = 1162230285,
	RampHic = 1165307954,
	Fitness01AFY = 1165780219,
	MollyCutscene = 1167167044,
	JosefCutscene = 1167549130,
	Glenstank01 = 1169888870,
	HeadTargets = 1173958009,
	RussianDrunkCutscene = 1179785778,
	Dale = 1182012905,
	Finguru01 = 1189322339,
	FabienCutscene = 1191403201,
	MilitaryBum = 1191548746,
	Humpback = 1193010354,
	DomCutscene = 1198698306,
	Ktown01AFO = 1204772502,
	Andreas = 1206185632,
	PestCont01SMY = 1209091352,
	Tramp01AFM = 1224306523,
	VagosSpeakCutscene = 1224690857,
	MexBoss02GMM = 1226102803,
	Gardener01SMM = 1240094341,
	Chef = 1240128502,
	Baywatch01SFY = 1250841910,
	Vinewood01AMY = 1264851357,
	Musclbeac01AMY = 1264920838,
	CocaineFemale01 = 1264941816,
	Acult02AMO = 1268862154,
	KarenDanielsCutscene = 1269774364,
	MoviePremFemaleCutscene = 1270514905,
	SiemonYetarian = 1283141381,
	PriestCutscene = 1299047806,
	LesterCrest = 1302784073,
	Families01GFY = 1309468115,
	Hipster03AMY = 1312913862,
	Husky = 1318032802,
	NataliaCutscene = 1325314544,
	Salton01AMM = 1328415626,
	PoloGoon01GMY = 1329576454,
	Lost01GMY = 1330042375,
	MrsThornhillCutscene = 1334976110,
	Paparazzi = 1346941736,
	Tourist01AFM = 1347814329,
	FemaleAgent = 1348537411,
	Blackops03SMY = 1349953339,
	Hiker01AMY = 1358380044,
	Eastsa03AFY = 1371553700,
	Baygor = 1380197501,
	Stripper01SFY = 1381498905,
	TylerDixon = 1382414087,
	Ktown01AFM = 1388848350,
	TaosTranslatorCutscene = 1397974313,
	Lifeinvad01 = 1401530684,
	TrampBeac01AMM = 1404403376,
	Acult01AMM = 1413662315,
	Tennis01AMM = 1416254276,
	Bevhills01AMM = 1423699487,
	Tennis01AFY = 1426880966,
	WinClean01SMY = 1426951581,
	Acult01AMO = 1430544400,
	Tourist01AFY = 1446741360,
	Prisguard01SMM = 1456041926,
	CocaineMale01 = 1456705429,
	Cormorant = 1457690978,
	JimmyDisanto = 1459905209,
	TrafficWarden = 1461287021,
	Cat = 1462895032,
	Bestmen = 1464257942,
	MarnieCutscene = 1464721716,
	MexBoss01GMM = 1466037421,
	MagentaCutscene = 1477887514,
	FbiSuit01Cutscene = 1482427218,
	Marine02SMY = 1490458366,
	Trucker01SMM = 1498487404,
	ExecutivePAFemale02 = 1500695792,
	Soucent02AFY = 1519319503,
	Vagos01GFY = 1520708641,
	PaigeCutscene = 1528799427,
	KerryMcintosh = 1530648845,
	HunterCutscene = 1531218220,
	SpyActress = 1535236204,
	StripperLiteSFY = 1544875514,
	TennisCoachCutscene = 1545995274,
	Bevhills02AFY = 1546450936,
	FibSec01 = 1558115333,
	Vinewood02AMY = 1561705728,
	Airhostess01SFY = 1567728751,
	Mistress = 1573528872,
	Cop01SMY = 1581098148,
	PrisMuscl01SMY = 1596003233,
	RivalPaparazzi = 1624626906,
	Omega = 1625728984,
	Salton02AMM = 1626646295,
	MerryWeatherCutscene = 1631478380,
	ForgeryMale01 = 1631482011,
	Bride = 1633872967,
	RampMarineCutscene = 1634506681,
	PopovCutscene = 1635617250,
	Genstreet01AFO = 1640504453,
	Fatlatin01AMM = 1641152947,
	Orleans = 1641334641,
	AirworkerSMY = 1644266841,
	DoaMan = 1646160893,
	Postal01SMM = 1650036788,
	CiaSec01SMM = 1650288984,
	Armymech01SMY = 1657546978,
	TonyaCutscene = 1665391897,
	Armoured02SMM = 1669696074,
	Eastsa02AFM = 1674107025,
	Priest = 1681385341,
	Coyote = 1682622302,
	MovAlien01 = 1684083350,
	Motox01AMY = 1694362237,
	Downtown01AFM = 1699403886,
	Marine01SMY = 1702441027,
	Hao = 1704428387,
	LamarDavis = 1706635382,
	Bevhills02AMY = 1720428295,
	Terry = 1728056212,
	OgBoss01AMM = 1746653202,
	Soucent01AMM = 1750583735,
	Azteca01GMY = 1752208920,
	Epsilon01AFY = 1755064960,
	Xmech02SMYMP = 1762949645,
	BarryCutscene = 1767447799,
	Skater01AFY = 1767892582,
	Methhead01AMY = 1768677545,
	TomCutscene = 1776856003,
	Factory01SFY = 1777626099,
	Tramp01 = 1787764635,
	SbikeAMO = 1794381917,
	Hen = 1794449327,
	PaperCutscene = 1798879480,
	Hasjew01AMM = 1809430156,
	StreetArt01 = 1813637474,
	Hillbilly01AMM = 1822107721,
	MPros01 = 1822283721,
	Clay = 1825562762,
	AmandaTownley = 1830688247,
	Pug = 1832265812,
	Agent14Cutscene = 1841036427,
	Stripper02SFY = 1846523796,
	ShopMaskSMY = 1846684678,
	HughCutscene = 1863555924,
	DeniseCutscene = 1870669624,
	FreemodeMale01 = 1885233650,
	PrologueSec01 = 1888624839,
	MichelleCutscene = 1890499016,
	OldMan1a = 1906124788,
	BradCadaverCutscene = 1915268960,
	Lifeinvad01Cutscene = 1918178165,
	Marine03SMY = 1925237458,
	Postal02SMM = 1936142927,
	Hwaycop01SMY = 1939545845,
	DeadHooker = 1943971979,
	Soucent01AFM = 1951946145,
	Devin = 1952555184,
	Car3Guy2 = 1975732938,
	DwService01SMY = 1976765073,
	Bevhills01AMY = 1982350912,
	BikeHire01 = 1984382277,
	Lsmetro01SMM = 1985653476,
	Motox02AMY = 2007797722,
	Epsilon01AMY = 2010389054,
	Bartender01SFY = 2014052797,
	ForgeryFemale01 = 2014985464,
	Beach02AMM = 2021631368,
	NervousRonCutscene = 2023152276,
	Strperf01SMM = 2035992488,
	Josh = 2040438510,
	Blackops02SMY = 2047212121,
	JayNorris = 2050158196,
	GroomCutscene = 2058033618,
	Hillbilly02AMM = 2064532783,
	FibSec01SMM = 2072724299,
	Prisoner01 = 2073775040,
	TaosTranslator = 2089096292,
	KorLieut01GMY = 2093736314,
	Hippy01AMY = 2097407511,
	Justin = 2109968527,
	Golfer01AFY = 2111372120,
	Beachvesp01AMY = 2114544056,
	ChiGoon01GMM = 2119136831,
	Business01AMM = 2120901815,
	Mariachi01SMM = 2124742566,
	Ashley = 2129936603,
	PrologueSec01Cutscene = 2141384740,
	Acult02AMY = 2162532142,
	Stripper02Cutscene = 2168724337,
	PartyTarget = 2180468199,
	Denise = 2181772221,
	Hipster01AFY = 2185745201,
	BrideCutscene = 2193587873,
	Polynesian01AMY = 2206530719,
	G = 2216405299,
	Beach01AMO = 2217202584,
	Famfor01GMY = 2217749257,
	Runner02AMY = 2218630415,
	ImportExportFemale01 = 2225189146,
	Car3Guy1 = 2230970679,
	Sweatshop01SFY = 2231547570,
	PrologueDriver = 2237544099,
	SlodLargeQuadped = 2238511874,
	DaveNortonCutscene = 2240226444,
	Chef2 = 2240322243,
	RampHicCutscene = 2240582840,
	FibMugger01 = 2243544680,
	Stlat01AMY = 2255803900,
	Dockwork01SMY = 2255894993,
	Solomon = 2260598310,
	Soucent03AFY = 2276611093,
	JohnnyKlebitz = 2278195374,
	TaoChengCutscene = 2288257085,
	StretchCutscene = 2302502917,
	AbigailCutscene = 2306246977,
	Soucent04AMY = 2318861297,
	OmegaCutscene = 2339419141,
	BurgerDrug = 2340239206,
	Dolphin = 2344268885,
	Soucent03AMM = 2346291386,
	ScreenWriterCutscene = 2346790124,
	TomEpsilonCutscene = 2349847778,
	TrampBeac01AFM = 2359345766,
	CarBuyerCutscene = 2362341647,
	BurgerDrugCutscene = 2363277399,
	MoviePremMaleCutscene = 2372398717,
	KillerWhale = 2374682809,
	Swat01SMY = 2374966032,
	Korean02GMY = 2414729609,
	SalvaBoss01GMY = 2422005962,
	WillyFist = 2423691919,
	Bankman = 2426248831,
	Tourist02AFY = 2435054400,
	WeedMale01 = 2441008217,
	Staggrm01AMO = 2442448387,
	Juggalo01AMY = 2445950508,
	Strvend01SMY = 2457805603,
	Wade = 2459507570,
	Farmer01AMM = 2488675799,
	Tattoo01AMO = 2494442380,
	Rottweiler = 2506301981,
	Armoured01SMM = 2512875213,
	AmandaTownleyCutscene = 2515474659,
	Salton04AMM = 2521108919,
	MexGoon03GMY = 2521633500,
	Hotposh01 = 2526768638,
	PrologueHostage01AMM = 2534589327,
	BankmanCutscene = 2539657518,
	Hipster02AFY = 2549481101,
	CounterfeitMale01 = 2555758964,
	Genstreet01AMY = 2557996913,
	AviSchwartzmanCutscene = 2560490906,
	Stbla02AMY = 2563194959,
	OldMan2Cutscene = 2566514544,
	Paper = 2577072326,
	Hacker = 2579169528,
	Taphillbilly = 2585681490,
	CopCutscene = 2595446627,
	Busicas01AMY = 2597531625,
	Franklin = 2602752943,
	Devinsec01SMY = 2606068340,
	Trevor = 2608926626,
	Dom = 2620240008,
	Corpse01UFY = 2624589981,
	FreemodeFemale01 = 2627665880,
	Topless01AFY = 2633130371,
	Claypain = 2634057640,
	Eastsa01AFM = 2638072698,
	Ammucity01SMY = 2651349821,
	Lathandy01SMM = 2659242702,
	Soucent02AMM = 2674735073,
	Ups01SMM = 2680389410,
	Ranger01SFY = 2680682039,
	Bouncer01SMM = 2681481517,
	Bevhills02AFM = 2688103263,
	Business03AMY = 2705543429,
	Stingray = 2705875277,
	PrologueMournFemale01 = 2718472679,
	TennisCoach = 2721800023,
	GCutscene = 2727244247,
	PoloGoon02GMY = 2733138262,
	ChefCutscene = 2739391114,
	MaryAnn = 2741999622,
	DrFriedlanderCutscene = 2745392175,
	Eastsa01AMY = 2756120947,
	CustomerCutscene = 2756669323,
	SteveHainsCutscene = 2766184958,
	Soucent02AFO = 2775443222,
	Gay02AMY = 2775713665,
	Hipster03AFY = 2780469782,
	AntonCutscene = 2781317046,
	Ballasog = 2802535058,
	Chimp = 2825402133,
	ChinGoonCutscene = 2831296918,
	Gaffer01SMM = 2841034142,
	JanitorSMM = 2842417644,
	ShopLowSFY = 2842568196,
	Polynesian01AMM = 2849617566,
	Golfer01AMM = 2850754114,
	RoccoPelosiCutscene = 2858686092,
	Epsilon02AMY = 2860711835,
	ChickenHawk = 2864127842,
	WeiCheng = 2867128955,
	Yoga01AMY = 2869588309,
	Pilot01SMY = 2872052743,
	Scrubs01SFY = 2874755766,
	BallasogCutscene = 2884567044,
	SpyActor = 2886641112,
	Zombie01 = 2890614022,
	Soucent02AMY = 2896414922,
	JewelSec01 = 2899099062,
	OrleansCutscene = 2905870170,
	Waiter01SMY = 2907468364,
	Genstreet01AMO = 2908022696,
	Westy = 2910340283,
	Busker01SMO = 2912874939,
	ShopHighSFM = 2923947184,
	Chef2Cutscene = 2925257274,
	Business03AFY = 2928082356,
	Stripper01Cutscene = 2934601397,
	Molly = 2936266209,
	Skater02AMY = 2952446692,
	Skidrow01AFM = 2962707003,
	Pig = 2971380566,
	Sheriff01SMY = 2974087609,
	Floyd = 2981205682,
	Prisoner01SMY = 2981862233,
	Autopsy01SMY = 2988916046,
	MexLabor01AMM = 2992445106,
	WeedFemale01 = 2992993187,
	Salton03AMM = 2995538501,
	GunVend01 = 3005388626,
	Paramedic01SMM = 3008586398,
	Business02AMY = 3014915558,
	Blackops01SMY = 3019107892,
	BeverlyCutscene = 3027157846,
	Acult01AMY = 3043264555,
	DeniseFriendCutscene = 3045926185,
	LesterCrestCutscene = 3046438339,
	FatCult01AFM = 3050275044,
	ComJane = 3064628686,
	Fireman01SMY = 3065114024,
	Sunbathe01AMY = 3072929548,
	MiltonCutscene = 3077190415,
	CounterfeitFemale01 = 3079205365,
	Business04AFY = 3083210802,
	JimmyDisantoCutscene = 3100414644,
	ChiBoss01GMM = 3118269184,
	Indian01AFO = 3134700416,
	ImportExportMale01 = 3164785898,
	MaudeCutscene = 3166991819,
	NervousRon = 3170921201,
	Beverly = 3181518428,
	Brad = 3183167778,
	MexGang01GMY = 3185399110,
	Bevhills01AFM = 3188223741,
	JoeMinuteman = 3189787803,
	Xmech02SMY = 3189832196,
	Michelle = 3214308084,
	Robber01SMY = 3227390873,
	SiemonYetarianCutscene = 3230888450,
	OrtegaCutscene = 3235579087,
	Claude01 = 3237179831,
	Vindouche01AMY = 3247667175,
	Skater01AMY = 3250873975,
	CrisFormageCutscene = 3253960934,
	JanitorCutscene = 3254803008,
	RampGangCutscene = 3263172030,
	Stlat02AMM = 3265820418,
	Rhesus = 3268439891,
	Soucent04AMM = 3271294718,
	Bankman01 = 3272005365,
	GurkCutscene = 3272931111,
	Rat = 3283429734,
	MrKCutscene = 3284966005,
	Soucent03AMY = 3287349092,
	ClubhouseBar01 = 3287737221,
	Yoga01AFY = 3290105390,
	Griff01 = 3293887675,
	VagosFun01 = 3299219389,
	Benny = 3300333010,
	PrologueHostage01 = 3306347811,
	ArmGoon02GMY = 3310258058,
	Patricia = 3312325004,
	Construct02SMY = 3321821918,
	Guido01 = 3333724719,
	Runner01AFY = 3343476521,
	Beach01AFY = 3349113128,
	BoatStaff01M = 3361671816,
	Tourist01AMM = 3365863812,
	Nigel = 3367442045,
	Mani = 3367706194,
	Musclbeac02AMY = 3374523516,
	Business01AMY = 3382649284,
	CCrew01SMM = 3387290987,
	Uscg01SMY = 3389018345,
	Beachvesp02AMY = 3394697810,
	Tonya = 3402126148,
	CletusCutscene = 3404326357,
	Milton = 3408943538,
	DrFriedlander = 3422293493,
	MrsPhillipsCutscene = 3422397391,
	Salton01AFO = 3439295882,
	SmugMech01 = 3446096293,
	TomEpsilon = 3447159466,
	Soucentmc01AFM = 3454621138,
	Armoured01 = 3455013896,
	Hunter = 3457361118,
	Jesus01 = 3459037009,
	Boar = 3462393972,
	Strvend01SMM = 3465614249,
	PrologueMournMale01 = 3465937675,
	Antonb = 3479321132,
	Stbla01AMY = 3482496489,
	Fabien = 3499148112,
	Ups02SMM = 3502104854,
	Misty01 = 3509125021,
	Ktown01AMM = 3512565361,
	AfriAmer01AMM = 3513928062,
	Gay01AMY = 3519864886,
	Beach01AMY = 3523131524,
	WadeCutscene = 3529955798,
	MethFemale01 = 3534913217,
	Princess = 3538133636,
	Seagull = 3549666813,
	Doctor01SMM = 3564307372,
	Migrant01SFY = 3579522037,
	RoccoPelosi = 3585757951,
	Golfer01AMY = 3609190705,
	Salton01AMY = 3613420592,
	Security01SMM = 3613962792,
	AgentCutscene = 3614493108,
	Construct01SMY = 3621428889,
	Drowned = 3623056905,
	Movprem01SMM = 3630066984,
	Deer = 3630914197,
	Busboy01SMY = 3640249671,
	Skater01AMM = 3654768780,
	Babyd = 3658575486,
	SecuroGuardMale01 = 3660355662,
	Dreyfuss = 3666413874,
	Vinewood02AFY = 3669401835,
	Juggalo01AFY = 3675473203,
	Scdressy01AFY = 3680420864,
	Famdnf01GMY = 3681718840,
	LinecookSMM = 3684436375,
	ClayCutscene = 3687553076,
	Pogo01 = 3696858125,
	TaoCheng = 3697041061,
	MexCntry01AMM = 3716251309,
	Indian01AMM = 3721046572,
	Lifeinvad01SMM = 3724572669,
	Salton01AFM = 3725461865,
	Natalia = 3726105915,
	TrafficWardenCutscene = 3727243251,
	TracyDisanto = 3728026165,
	RampHipster = 3740245870,
	PatriciaCutscene = 3750433537,
	Rabbit = 3753204865,
	Lazlow = 3756278757,
	Maid01SFM = 3767780806,
	Tranvest01AMM = 3773208948,
	Casey = 3774489940,
	Josef = 3776618420,
	NigelCutscene = 3779566603,
	Hasjew01AMY = 3782053633,
	ImranCutscene = 3812756443,
	Dealer01SMY = 3835149295,
	RampGang = 3845001836,
	Barman01SMY = 3852538118,
	Cletus = 3865252245,
	RampMex = 3870061732,
	JewelThief = 3872144604,
	Soucent01AMY = 3877027275,
	AndreasCutscene = 3881194279,
	Pilot01SMM = 3881519900,
	ArmLieut01GMM = 3882958867,
	Talina = 3885222120,
	Beach03AMY = 3886638041,
	Movspace01SMM = 3887273010,
	Famca01GMY = 3896218551,
	GroveStrDlrCutscene = 3898166818,
	CaseyCutscene = 3935738944,
	ZimborCutscene = 3937184496,
	Surfer01AMY = 3938633710,
	KarenDaniels = 3948009817,
	HaoCutscene = 3969814300,
	Paparazzi01AMM = 3972697109,
	DebraCutscene = 3973074921,
	Migrant01SMM = 3977045190,
	JimmyBoston = 3986688045,
	MethMale01 = 3988008767,
	FibOffice01SMM = 3988550982,
	MrK = 3990661997,
	GarbageSMY = 4000686095,
	Niko01 = 4007317449,
	OldMan2 = 4011150407,
	Ranger01SMY = 4017173934,
	UndercoverCopCutscene = 4017642090,
	BradCutscene = 4024807398,
	PrologueDriverCutscene = 4027271643,
	Marine02SMM = 4028996995,
	Hippie01 = 4030826507,
	Autoshop02SMM = 4033578141,
	JoeMinutemanCutscene = 4036845097,
	Abner = 4037813798,
	Jewelass01 = 4040474158,
	AlDiNapoli = 4042020578,
	Highsec01SMM = 4049719826,
	Malc = 4055673113,
	ArmBoss01GMM = 4058522530,
	Marine01SMM = 4074414829,
	Soucent02AFM = 4079145784,
	OscarCutscene = 4095687067,
	BallaEast01GMY = 4096714883,
	Roadcyc01AMY = 4116817094,
	DwService02SMY = 4119890438,
	Eastsa01AFY = 4121954205,
	ChemWork01GMM = 4128603535,
	Pilot02SMM = 4131252449,
	RampMexCutscene = 4132362192,
	SolomonCutscene = 4140949582,
	Tranvest02AMM = 4144940484,
	WareMechMale01 = 4154933561,
	Eastsa01AMM = 4188468543,
	VagosSpeak = 4194109068,
	BikerChic = 4198014287,
	JohnnyKlebitzCutscene = 4203395201,
	FatBla01AFM = 4206136267,
	Vinewood04AFY = 4209271110,
	ManuelCutscene = 4222842058,
	Agent14 = 4227433577,
	Magenta = 4242313482,
	Cow = 4244282910,
	StrPunk01GMY = 4246489531,
	Manuel = 4248931856,
	Lost01GFY = 4250220510,
	ArmGoon01GMM = 4255728232,
	Cyclist01AMY = 4257633223,
	Groom = 4274948997,
	Dhill01AMY = 4282288299,
	ChiGoon02GMM = 4285659174,
	ScreenWriter = 4293277303,
	Panther = 3877461608
}

public static class SkinHelpers
{
	public static int GetTexturesForBeard(int currentDrawable)
	{
		if (MaskHelpers.MasksFunctioningAsBeards.Contains(currentDrawable))
		{
			if (currentDrawable == 121)
			{
				return 0;
			}
			else
			{
				return 2;
			}
		}

		return 0;
	}

	public static void ApplyHairTattooForPlayer(RAGE.Elements.Player player, bool bUseForceBaseHairID = false, int forceBaseHairID = -1)
	{
		int BASEHAIR = bUseForceBaseHairID ? forceBaseHairID : DataHelper.GetEntityData<int>(player, EDataNames.CC_BASEHAIR);
		CHairTattooDefinition hairTattooDef = TattooDefinitions.GetHairTattooDefinitionFromID(BASEHAIR);
		if (hairTattooDef != null)
		{
			player.SetFacialDecoration((uint)hairTattooDef.HairTattooCollection, (uint)hairTattooDef.HairTattooOverlay);
		}
	}

	// NOTE: we have to apply these at the same time since they are all 'tattoos' to the game
	public static void ApplyTattoosAndHairTattoosForPlayer(RAGE.Elements.Player player, bool bForceHideHair = false, bool bUseForceBaseHairID = false, int forceBaseHairID = -1)
	{
		player.ClearFacialDecorations();

		if (!bForceHideHair)
		{
			ApplyHairTattooForPlayer(player, bUseForceBaseHairID, forceBaseHairID);
		}

		List<int> lstTattoos = OwlJSON.DeserializeObject<List<int>>(DataHelper.GetEntityData<string>(player, EDataNames.CC_TATTOOS), EJsonTrackableIdentifier.HairAndTattoos);
		if (lstTattoos != null)
		{
			EGender Gender = DataHelper.GetEntityData<EGender>(player, EDataNames.GENDER);
			foreach (int tattooID in lstTattoos)
			{
				CTattooDefinition tattooDef = TattooDefinitions.GetTattooDefinitionFromID(tattooID);
				if (tattooDef != null)
				{
					player.SetFacialDecoration(tattooDef.GetHash_Collection(), tattooDef.GetHash_Tattoo(Gender));
				}
			}
		}
	}
}

public enum EPlayerRadialInteractionID
{
	None = -1,
	Handcuffs,
	Frisk,
	VehicleTrunk,
	VehicleHood,
	VehicleLock,
	EnterBuilding,
	AccessMailbox,
	None6,
}

public enum EVehicleStoreRotationDirection
{
	None,
	Left,
	Right
}

public enum EVehicleStoreZoomDirection
{
	None,
	In,
	Out
}

public enum EFurnitureStoreRotationDirection
{
	None,
	Left,
	Right
}

public enum EFurnitureStoreZoomDirection
{
	None,
	In,
	Out
}

public class CItemDetails
{
	public CItemDetails(EItemID a_itemID, string strName, string strDescription, float fWeight)
	{
		itemID = a_itemID;
		itemName = strName;
		itemDesc = strDescription;
		itemWeight = fWeight;
	}

	public EItemID itemID { get; set; }
	public string itemName { get; set; }
	public string itemDesc { get; set; }
	public float itemWeight { get; set; }
}

// NOTE: This is serialized/deserialized to JSON, so if you rename a variable, dont touch the override JsonProperty name
public class DO_NOT_USE_CCustomAnimCommand
{
	[JsonProperty("commandName")]
	public string commandName { get; set; }
	[JsonProperty("animDictionary")]
	public string animDictionary { get; set; }
	[JsonProperty("animName")]
	public string animName { get; set; }
	[JsonProperty("loop")]
	public bool loop { get; set; }
	[JsonProperty("stopOnLastFrame")]
	public bool stopOnLastFrame { get; set; }
	[JsonProperty("onlyAnimateUpperBody")]
	public bool onlyAnimateUpperBody { get; set; }
	[JsonProperty("allowPlayerMovement")]
	public bool allowPlayerMovement { get; set; }
	[JsonProperty("durationSeconds")]
	public int durationSeconds { get; set; }
}

public static class NotificationIconHelpers
{
	public static Dictionary<ENotificationIcon, string> IconMap = new Dictionary<ENotificationIcon, string>()
	{
		{ ENotificationIcon.ExclamationSign, "exclamation" },
		{ ENotificationIcon.InfoSign, "info" },
		{ ENotificationIcon.Flash, "bolt" },
		{ ENotificationIcon.Remove, "minus" },
		{ ENotificationIcon.USD, "dollar-sign" },
		{ ENotificationIcon.Phone, "bullhorn" },
		{ ENotificationIcon.Headphones, "headphones" },
		{ ENotificationIcon.ThumbsUp, "thumbs-up" },
		{ ENotificationIcon.PiggyBank, "piggy-bank" },
		{ ENotificationIcon.Font, "users-cog" },
		{ ENotificationIcon.HeartEmpty, "heart" },
		{ ENotificationIcon.Star, "star" },
		{ ENotificationIcon.VolumeUp, "volume-up" },
	};
}

public enum EWeaponFireTypes
{
	Auto,
	Burst,
	SingleFire
}

// Custom duty presets
public enum EDutyCustomSkinPresets
{
	PD_Male_Deputy,
	PD_Male_Deputy_2,
	PD_Male_LSPD_Officer,
	PD_Male_Sheriff_Detective,
	PD_Male_Sheriff_Detective_1,
	PD_Male_LSPD_SWAT,
	PD_Male_Sheriff_SWAT,
	PD_Female_LSPD_Officer,
	PD_Female_LSPD_Deputy,

	// FD & EMS
	FD_Male_Paramedic,
	FD_Female_Paramedic,
	FD_Male_Firefighter,
	FD_Male_Firefighter_2,
	FD_Female_Firefighter,
	FD_Female_Firefighter_2,
	FD_Male_Ceremonial,
	FD_Female_Ceremonial,
	FD_Male_Pilot,
	FD_Female_Pilot,

	News_Male,
	News_Female
}

public class DutyCustomSkinPresetSlot
{
	public DutyCustomSkinPresetSlot(ECustomClothingComponent a_Component, int a_Drawable, int a_Texture, int a_Palette)
	{
		Component = a_Component;
		Drawable = a_Drawable;
		Texture = a_Texture;
		Palette = a_Palette;
		IsProp = false;
	}

	public DutyCustomSkinPresetSlot(ECustomPropSlot a_Prop, int a_Drawable, int a_Texture)
	{
		Prop = a_Prop;
		Drawable = a_Drawable;
		Texture = a_Texture;
		IsProp = true;
	}

	public ECustomClothingComponent Component { get; }
	public ECustomPropSlot Prop { get; }
	public int Drawable { get; }
	public int Texture { get; }
	public int Palette { get; }
	public bool IsProp { get; }


#if !SERVER
	public void ApplyToLocalPlayer()
	{
		if (IsProp)
		{
			RAGE.Elements.Player.LocalPlayer.SetPropIndex((int)Prop, Drawable, Texture, true);
		}
		else
		{
			RAGE.Elements.Player.LocalPlayer.SetComponentVariation((int)Component, Drawable, Texture, Palette);
		}
	}
#endif
}

public class DutyCustomSkinPreset
{
	public DutyCustomSkinPreset(EDutyCustomSkinPresets a_ID, EGender a_Gender, string a_strDisplayName, List<DutyCustomSkinPresetSlot> lstSlots)
	{
		ID = a_ID;
		Gender = a_Gender;
		strDisplayName = a_strDisplayName;
		Slots = lstSlots;
	}

	public EDutyCustomSkinPresets ID { get; }
	public EGender Gender { get; }
	public string strDisplayName { get; }
	public List<DutyCustomSkinPresetSlot> Slots { get; }
}

public class DutyCustomAvailableClothesItem
{
	public DutyCustomAvailableClothesItem(string a_DisplayName, ECustomClothingComponent a_Component, int a_Model, int a_Texture, bool a_bIsCivilian) // bIsCivilian true means it will still show up in clothing stores etc note though you cant block out individual textures, only entire drawables
	{
		DisplayName = a_DisplayName;
		Component = a_Component;
		Model = a_Model;
		Texture = a_Texture;
		IsCivilian = a_bIsCivilian;
	}

	public string DisplayName { get; }
	public ECustomClothingComponent Component { get; }
	public int Model { get; }
	public int Texture { get; }
	public bool IsCivilian { get; }
}

public class DutyCustomAvailablePropItem
{
	public DutyCustomAvailablePropItem(string a_DisplayName, ECustomPropSlot a_Prop, int a_Model, int a_Texture, bool a_bIsCivilian) // bIsCivilian true means it will still show up in clothing stores etc note though you cant block out individual textures, only entire drawables
	{
		DisplayName = a_DisplayName;
		Prop = a_Prop;
		Model = a_Model;
		Texture = a_Texture;
		IsCivilian = a_bIsCivilian;
	}

	public string DisplayName { get; }
	public ECustomPropSlot Prop { get; }
	public int Model { get; }
	public int Texture { get; }
	public bool IsCivilian { get; }
}

public static class DutyCustomSkins
{
	public static void RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent component, EGender gender, ref List<int> lstItems)
	{
		foreach (EDutyType dutyType in Enum.GetValues(typeof(EDutyType)))
		{
			if (DutyCustomSkins.AvailableClothes.ContainsKey(dutyType))
			{
				var ClothesForDutyType = DutyCustomSkins.AvailableClothes[dutyType];

				if (ClothesForDutyType.ContainsKey(gender))
				{
					var ClothesForDutyTypeAndGender = ClothesForDutyType[gender];

					if (ClothesForDutyTypeAndGender.ContainsKey(component))
					{
						var ClothesForDutyTypeAndGenderAndComponent = ClothesForDutyTypeAndGender[component];

						foreach (DutyCustomAvailableClothesItem clothesItem in ClothesForDutyTypeAndGenderAndComponent)
						{
							if (!clothesItem.IsCivilian)
							{
								lstItems.Remove(clothesItem.Model);
							}
						}
					}
				}
			}
		}
	}

	public static void RemoveAnyDutyPropItemsFromArray(ECustomPropSlot slot, EGender gender, ref List<int> lstItems)
	{
		foreach (EDutyType dutyType in Enum.GetValues(typeof(EDutyType)))
		{
			if (DutyCustomSkins.AvailableProps.ContainsKey(dutyType))
			{
				var PropsForDutyType = DutyCustomSkins.AvailableProps[dutyType];

				if (PropsForDutyType.ContainsKey(gender))
				{
					var PropsForDutyTypeAndGender = PropsForDutyType[gender];

					if (PropsForDutyTypeAndGender.ContainsKey(slot))
					{
						var PropsForDutyTypeAndGenderAndComponent = PropsForDutyTypeAndGender[slot];

						foreach (DutyCustomAvailablePropItem propItem in PropsForDutyTypeAndGenderAndComponent)
						{
							if (!propItem.IsCivilian)
							{
								lstItems.Remove(propItem.Model);
							}
						}
					}
				}
			}
		}
	}


	public static Dictionary<EDutyType, Dictionary<EGender, Dictionary<uint, string>>> PremadeSkins = new Dictionary<EDutyType, Dictionary<EGender, Dictionary<uint, string>>>
	{
		// LAW ENF
		{ EDutyType.Law_Enforcement, new Dictionary<EGender, Dictionary<uint, string>>
			{
				// MALE
				{ EGender.Male, new Dictionary<uint, string>
					{
						{ 2974087609, "Sheriffs Deputy 1"},
						{ 3613962792, "Sheriffs Deputy 2"},
						{ 3455013896, "Cadet 1"},
						{ 2512875213, "Cadet 2"},
						{ 1669696074, "Cadet 3"},
						{ 3660355662, "Hispanic Cop"},
						{ 1657546978, "Off Duty Swat Operator"},
						{ 3774489940, "Cadet 4"},
						{ 1650288984, "Plain Clothes Officer"},
						{ 1581098148, "Police Officer"},
						{ 1939545845, "Highway Patrol"},
						{ 1631478380, "SWAT Operator"},
						{ 2872052743, "Pilot 1"},
						{ 4131252449, "Pilot 2"},
						{ 1456041926, "Sheriffs Deputy 3"},
						{ 666086773, "Sheriffs Deputy 4"},
						{ 2141384740, "Sheriffs Deputy 5"},
						{ 512955554, "Sheriffs Deputy 6"},
						{ 4017173934, "Sheriffs Deputy 7"},
						{ 451459928, "Sheriffs Deputy 8"},
						{ 2374966032, "SWAT Officer"},
						{ 1461287021, "Traffic Cop"},
						{ 3389018345, "Marine Cop"},
					}
				},

				// MALE
				{ EGender.Female, new Dictionary<uint, string>
					{
						{ 368603149, "Police Officer" },
						{ 3948009817, "Plain Clothes Officer (Female)" },
						{ 2680682039, "Sheriffs Deputy 1" },
						{ 1096929346, "Sheriffs Deputy 2" }
					}
				}
			}
		},

		// EMS
		{ EDutyType.EMS, new Dictionary<EGender, Dictionary<uint, string>>
			{
				// MALE
				{ EGender.Male, new Dictionary<uint, string>
					{
						{ 3008586398, "Paramedic" },
						{ 2988916046, "Surgeon" },
						{ 3564307372, "Doctor 1" },
						{ 1092080539, "Doctor 2" }
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<uint, string>
					{
						{ 2874755766, "Surgeon" }
					}
				}
			}
		},

		// FD
		{ EDutyType.Fire, new Dictionary<EGender, Dictionary<uint, string>>
			{
				// MALE
				{ EGender.Male, new Dictionary<uint, string>
					{
						{ 3065114024, "Firefighter" },
						{  4128603535, "HazMat" }
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<uint, string>
					{
						{3065114024, "Firefighter" }
					}
				}
			}
		},

		// News
		{ EDutyType.News, new Dictionary<EGender, Dictionary<uint, string>>
			{
				// MALE
				{ EGender.Male, new Dictionary<uint, string>
					{
						{ 3027157846, "Journalist" },
						{ 3972697109, "News Reporter" },
						{ 776079908, "News Anchor" }
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<uint, string>
					{
						{ 1446741360, "Journalist" },
						{ 3538133636, "News Reporter" },
						{ 1348537411, "News Anchor" }
					}
				}
			}
		},
		
		// Towing
		{ EDutyType.Towing, new Dictionary<EGender, Dictionary<uint, string>>
			{
				// MALE
				{ EGender.Male, new Dictionary<uint, string>
					{
						{ 1644266841, "Tow truck Driver" },
						{ 1657546978, "Mechanic" }
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<uint, string>
					{
						{ 813893651, "Tow truck driver" },
					}
				}
			}
		},
	};

	// NOTE: IF you add things here, you must update DutyServersideGrants above for any related items (e.g. ammo)
	public static Dictionary<EDutyType, Dictionary<EDutyWeaponSlot, List<EItemID>>> DutyLoadouts = new Dictionary<EDutyType, Dictionary<EDutyWeaponSlot, List<EItemID>>>
	{
		// LAW ENF
		{ EDutyType.Law_Enforcement, new Dictionary<EDutyWeaponSlot, List<EItemID>>
			{
				// PursuitAccessory
				{ EDutyWeaponSlot.PursuitAccessory, new List<EItemID>
					{
						EItemID.SPIKESTRIP
					}
				},

				// Melee
				{ EDutyWeaponSlot.Melee, new List<EItemID>
					{
						EItemID.WEAPON_NIGHTSTICK
					}
				},

				// Accessory1
				{ EDutyWeaponSlot.Accessory1, new List<EItemID>
					{
						EItemID.WEAPON_FLASHLIGHT
					}
				},

				// Accessory2
				{ EDutyWeaponSlot.Accessory2, new List<EItemID>
					{
						EItemID.HANDCUFFS
					}
				},

				// Accessory3
				{ EDutyWeaponSlot.Accessory3, new List<EItemID>
					{
						EItemID.MEGAPHONE
					}
				},

				// Handgun1
				{ EDutyWeaponSlot.HandgunHipHolster, new List<EItemID>
					{
						EItemID.WEAPON_PISTOL_MK2,
						EItemID.WEAPON_COMBATPISTOL,
						EItemID.WEAPON_STUNGUN,
						EItemID.WEAPON_FLAREGUN
					}
				},

				// Handgun2
				{ EDutyWeaponSlot.HandgunLegHolster, new List<EItemID>
					{
						EItemID.WEAPON_PISTOL_MK2,
						EItemID.WEAPON_COMBATPISTOL,
						EItemID.WEAPON_STUNGUN,
						EItemID.WEAPON_FLAREGUN
					}
				},

				// LargeWeapon
				{ EDutyWeaponSlot.LargeWeapon, new List<EItemID>
					{
						EItemID.WEAPON_CARBINERIFLE_MK2,
						EItemID.WEAPON_SPECIALCARBINE,
						EItemID.WEAPON_SMG,
						EItemID.WEAPON_PUMPSHOTGUN_MK2,
						EItemID.WEAPON_HEAVYSNIPER_MK2,
						EItemID.WEAPON_MARKSMANRIFLE,
						EItemID.WEAPON_SNIPERRIFLE,
						EItemID.WEAPON_GRENADELAUNCHER_SMOKE
					}
				},

				// Projectile
				{ EDutyWeaponSlot.Projectile, new List<EItemID>
					{
						EItemID.WEAPON_SMOKEGRENADE,
						EItemID.WEAPON_BZGAS
					}
				},

				// Projectile2
				{ EDutyWeaponSlot.Projectile2, new List<EItemID>
					{
						EItemID.WEAPON_FLARE
					}
				},

				// LargeCarriedItem
				{ EDutyWeaponSlot.LargeCarriedItem, new List<EItemID>
					{
						EItemID.RIOT_SHIELD,
						EItemID.SWAT_SHIELD
					}
				},
			}
		},

		// EMS
		{ EDutyType.EMS, new Dictionary<EDutyWeaponSlot, List<EItemID>>
			{

			}
		},

		// FD
		{ EDutyType.Fire, new Dictionary<EDutyWeaponSlot, List<EItemID>>
			{
				// Accessory1
				{ EDutyWeaponSlot.Accessory1, new List<EItemID>
					{
						EItemID.WEAPON_FIREEXTINGUISHER
					}
				},
			}
		},

		// NEWS
		{ EDutyType.News, new Dictionary<EDutyWeaponSlot, List<EItemID>>
			{
				// Accessory1
				{ EDutyWeaponSlot.Accessory1, new List<EItemID>
					{
						EItemID.MICROPHONE
					}
				},

				// Accessory2
				{ EDutyWeaponSlot.Accessory2, new List<EItemID>
					{
						EItemID.BOOM_MIC
					}
				},

				// Accessory3
				{ EDutyWeaponSlot.Accessory3, new List<EItemID>
					{
						EItemID.VIDEO_CAMERA
					}
				},

				// LargeCarriedItem
				{ EDutyWeaponSlot.LargeCarriedItem, new List<EItemID>
					{
						EItemID.NEWS_CAMERA
					}
				},
			}
		}
	};

	public static Dictionary<EDutyType, Dictionary<EGender, Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>>> AvailableClothes = new Dictionary<EDutyType, Dictionary<EGender, Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>>>
	{
		// LAW ENF
		{ EDutyType.Law_Enforcement, new Dictionary<EGender, Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>>
			{
				// MALE
				{ EGender.Male, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
						// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Gas Mask", ECustomClothingComponent.Masks, 46, 0, false),
								new DutyCustomAvailableClothesItem("Balaclava 1", ECustomClothingComponent.Masks, 52, 0, true),
								new DutyCustomAvailableClothesItem("Balaclava 2", ECustomClothingComponent.Masks, 35, 0, true),
							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
								new DutyCustomAvailableClothesItem("Torso 3", ECustomClothingComponent.Torsos, 4, 0, true),
								new DutyCustomAvailableClothesItem("Torso 4", ECustomClothingComponent.Torsos, 96, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Smart Pants", ECustomClothingComponent.Legs, 35, 0, true),
								new DutyCustomAvailableClothesItem("Jeans", ECustomClothingComponent.Legs, 4, 0, true),

								new DutyCustomAvailableClothesItem("Loose Black Pants", ECustomClothingComponent.Legs, 7, 0, true),
								new DutyCustomAvailableClothesItem("Tan Camo Shorts", ECustomClothingComponent.Legs, 15, 11, false),
								new DutyCustomAvailableClothesItem("Black Shorts", ECustomClothingComponent.Legs, 15, 3, true),
								new DutyCustomAvailableClothesItem("Riot Pants", ECustomClothingComponent.Legs, 84, 0, false),
								new DutyCustomAvailableClothesItem("Green Cargo Pants", ECustomClothingComponent.Legs, 133, 0, true),
								new DutyCustomAvailableClothesItem("Black Formal Pants", ECustomClothingComponent.Legs, 35, 0, true),
								new DutyCustomAvailableClothesItem("Tan Cargo Pants", ECustomClothingComponent.Legs, 47, 0, true),
								new DutyCustomAvailableClothesItem("Tactical Pants (SWAT)", ECustomClothingComponent.Legs, 135, 0, false),
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Police Badge on Chest", ECustomClothingComponent.BagsAndParachutes, 85, 0, false),
								new DutyCustomAvailableClothesItem("Badge & Cuffs on Belt", ECustomClothingComponent.BagsAndParachutes, 86, 0, false),
								new DutyCustomAvailableClothesItem("Police Badge on Belt", ECustomClothingComponent.BagsAndParachutes, 87, 0, false),

								new DutyCustomAvailableClothesItem("Badge on Chest, basic belt w/ no guns", ECustomClothingComponent.BagsAndParachutes, 88, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Star on Chest", ECustomClothingComponent.BagsAndParachutes, 89, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Star & Cuffs on belt", ECustomClothingComponent.BagsAndParachutes, 90, 0, false),

								new DutyCustomAvailableClothesItem("Hip Holster", ECustomClothingComponent.BagsAndParachutes, 91, 0, false),
								new DutyCustomAvailableClothesItem("Leg Holster", ECustomClothingComponent.BagsAndParachutes, 92, 0, false),
								new DutyCustomAvailableClothesItem("Earpiece", ECustomClothingComponent.BagsAndParachutes, 93, 0, false),
								new DutyCustomAvailableClothesItem("SWAT Earpiece", ECustomClothingComponent.BagsAndParachutes, 94, 0, false),
							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Formal Duty Shoes", ECustomClothingComponent.Legs, 24, 0, true),
								new DutyCustomAvailableClothesItem("Casual Vans (Detective)", ECustomClothingComponent.Legs, 48, 0, true),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("IAA Badge around neck", ECustomClothingComponent.Accessories, 128, 0, false),
								new DutyCustomAvailableClothesItem("IAA Badge around neck & badge on belt", ECustomClothingComponent.Accessories, 125, 0, false),

								new DutyCustomAvailableClothesItem("Glock in waist holster", ECustomClothingComponent.Accessories, 136, 0, false),
								new DutyCustomAvailableClothesItem("Glock in small waist holster w/ two mag pouches", ECustomClothingComponent.Accessories, 137, 0, false),
								new DutyCustomAvailableClothesItem("Glock in leg holster", ECustomClothingComponent.Accessories, 138, 0, false),
								new DutyCustomAvailableClothesItem("Best Belt w/ Everything", ECustomClothingComponent.Accessories, 151, 0, false),
								new DutyCustomAvailableClothesItem("Badge around neck", ECustomClothingComponent.Accessories, 139, 0, false),
								new DutyCustomAvailableClothesItem("Badge around neck 2", ECustomClothingComponent.Accessories, 140, 0, false),

								new DutyCustomAvailableClothesItem("Underarm holster and mags", ECustomClothingComponent.Accessories, 141, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff star around neck", ECustomClothingComponent.Accessories, 142, 0, false),

								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, No gun", ECustomClothingComponent.Accessories, 143, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, No gun (Metal Buttons)", ECustomClothingComponent.Accessories, 143, 1, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, Taser, No gun", ECustomClothingComponent.Accessories, 144, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pepper spray", ECustomClothingComponent.Accessories, 145, 0, false),

								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pouches", ECustomClothingComponent.Accessories, 146, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 15, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 2", ECustomClothingComponent.Undershirts, 58, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 3", ECustomClothingComponent.Undershirts, 130, 0, true),
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Tan Large Vest", ECustomClothingComponent.BodyArmor, 16, 0, false),
								new DutyCustomAvailableClothesItem("Black Large Vest", ECustomClothingComponent.BodyArmor, 16, 2, false),

								new DutyCustomAvailableClothesItem("FBI Badge", ECustomClothingComponent.BodyArmor, 53, 0, false),
								new DutyCustomAvailableClothesItem("FIB Badge #2", ECustomClothingComponent.BodyArmor, 54, 0, false),
								new DutyCustomAvailableClothesItem("FIB Badge #3", ECustomClothingComponent.BodyArmor, 55, 0, false),

								new DutyCustomAvailableClothesItem("High Visibility Vest (LSPD)", ECustomClothingComponent.BodyArmor, 56, 0, false),
								new DutyCustomAvailableClothesItem("High Visibility Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 56, 1, false),

								new DutyCustomAvailableClothesItem("Vest (Sheriff K9)", ECustomClothingComponent.BodyArmor, 57, 0, false),
								new DutyCustomAvailableClothesItem("Vest (LSPD)", ECustomClothingComponent.BodyArmor, 57, 1, false),
								new DutyCustomAvailableClothesItem("Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 57, 2, false),

								new DutyCustomAvailableClothesItem("Vest (Police w/ Thin Blue Line flag)", ECustomClothingComponent.BodyArmor, 57, 5, false),
								new DutyCustomAvailableClothesItem("Vest (Police K9 w/ Thin Blue Line flag)", ECustomClothingComponent.BodyArmor, 57, 6, false),
								new DutyCustomAvailableClothesItem("Vest (Sheriff w/ Thin Blue Line flag)", ECustomClothingComponent.BodyArmor, 57, 7, false),

								new DutyCustomAvailableClothesItem("Small Vest w/ Thin Blue Line flag & Mags (Police)", ECustomClothingComponent.BodyArmor, 58, 0, false),
								new DutyCustomAvailableClothesItem("Small Vest w/ Thin Blue Line flag & Mags (Sheriff)", ECustomClothingComponent.BodyArmor, 58, 1, false),

								new DutyCustomAvailableClothesItem("Simple Vest (LSPD)", ECustomClothingComponent.BodyArmor, 59, 1, false),
								new DutyCustomAvailableClothesItem("Simple Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 59, 0, false),
								new DutyCustomAvailableClothesItem("Simple Vest (Sheriff Black)", ECustomClothingComponent.BodyArmor, 59, 2, false),

								new DutyCustomAvailableClothesItem("Very Simple Vest (Blank)", ECustomClothingComponent.BodyArmor, 60, 0, false),
								new DutyCustomAvailableClothesItem("Very Simple Vest (LSPD)", ECustomClothingComponent.BodyArmor, 60, 1, false),
								new DutyCustomAvailableClothesItem("Very Simple Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 60, 2, false),
								new DutyCustomAvailableClothesItem("Very Simple Vest (Negotiator)", ECustomClothingComponent.BodyArmor, 60, 3, false),

								new DutyCustomAvailableClothesItem("Advanced Vest (LSPD)", ECustomClothingComponent.BodyArmor, 61, 0, false),
								new DutyCustomAvailableClothesItem("Advanced Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 61, 1, false),

								new DutyCustomAvailableClothesItem("US Flag Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 62, 0, false),
								new DutyCustomAvailableClothesItem("US Flag Vest (LSPD)", ECustomClothingComponent.BodyArmor, 62, 1, false),

								new DutyCustomAvailableClothesItem("Vest w/ Radio & Mic", ECustomClothingComponent.BodyArmor, 63, 0, false),

								new DutyCustomAvailableClothesItem("Full Vest w/ Radio & Belt (LSPD)", ECustomClothingComponent.BodyArmor, 64, 0, false),

								new DutyCustomAvailableClothesItem("Best Vest w/ Tazer & Cuffs etc", ECustomClothingComponent.BodyArmor, 65, 0, false),

								new DutyCustomAvailableClothesItem("Full Vest w/ backpack (LSPD)", ECustomClothingComponent.BodyArmor, 66, 0, false),

								new DutyCustomAvailableClothesItem("Radio clipped to belt", ECustomClothingComponent.BodyArmor, 67, 0, false),
							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Two Chevrons", ECustomClothingComponent.Decals, 8, 1, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Tactical Shirt (Dark Black)", ECustomClothingComponent.Tops, 351, 0, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Green)", ECustomClothingComponent.Tops, 351, 1, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Black)", ECustomClothingComponent.Tops, 351, 2, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Light Black)", ECustomClothingComponent.Tops, 351, 4, false),

								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Blank)", ECustomClothingComponent.Tops, 352, 0, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (LSPD)", ECustomClothingComponent.Tops, 352, 1, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Sheriff)", ECustomClothingComponent.Tops, 352, 2, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Blue)", ECustomClothingComponent.Tops, 352, 3, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (White)", ECustomClothingComponent.Tops, 352, 4, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Red)", ECustomClothingComponent.Tops, 352, 5, false),

								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Blank)", ECustomClothingComponent.Tops, 353, 0, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (LSPD)", ECustomClothingComponent.Tops, 353, 1, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Sheriff)", ECustomClothingComponent.Tops, 353, 2, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Blue)", ECustomClothingComponent.Tops, 353, 3, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (White)", ECustomClothingComponent.Tops, 353, 4, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Red)", ECustomClothingComponent.Tops, 353, 5, false),

								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Blank)", ECustomClothingComponent.Tops, 354, 0, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (LSPD)", ECustomClothingComponent.Tops, 354, 1, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 354, 2, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Blue)", ECustomClothingComponent.Tops, 354, 3, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (White)", ECustomClothingComponent.Tops, 354, 4, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Red)", ECustomClothingComponent.Tops, 354, 5, false),

								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (SWAT)", ECustomClothingComponent.Tops, 355, 0, false),
								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 355, 1, false),
								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (K9)", ECustomClothingComponent.Tops, 355, 2, false),

								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (SWAT)", ECustomClothingComponent.Tops, 356, 0, false),
								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 356, 1, false),
								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (K9)", ECustomClothingComponent.Tops, 356, 2, false),

								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD)", ECustomClothingComponent.Tops, 359, 0, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD SWAT)", ECustomClothingComponent.Tops, 359, 1, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD #2)", ECustomClothingComponent.Tops, 359, 2, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Sheriff Green)", ECustomClothingComponent.Tops, 359, 3, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Sheriff White)", ECustomClothingComponent.Tops, 359, 4, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Firearms Instructor)", ECustomClothingComponent.Tops, 359, 5, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Forensics)", ECustomClothingComponent.Tops, 359, 6, false),

								new DutyCustomAvailableClothesItem("Police Jacket", ECustomClothingComponent.Tops, 357, 0, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 1)", ECustomClothingComponent.Tops, 357, 1, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 2)", ECustomClothingComponent.Tops, 357, 2, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 3)", ECustomClothingComponent.Tops, 357, 3, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 4)", ECustomClothingComponent.Tops, 357, 4, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 5)", ECustomClothingComponent.Tops, 357, 5, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 6)", ECustomClothingComponent.Tops, 357, 6, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 7)", ECustomClothingComponent.Tops, 357, 7, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket", ECustomClothingComponent.Tops, 360, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket", ECustomClothingComponent.Tops, 360, 1, false),
								new DutyCustomAvailableClothesItem("Coroner Jacket", ECustomClothingComponent.Tops, 360, 2, false),
								new DutyCustomAvailableClothesItem("Forensics Jacket", ECustomClothingComponent.Tops, 360, 3, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket (Style 2)", ECustomClothingComponent.Tops, 360, 4, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket w/ holster space", ECustomClothingComponent.Tops, 361, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket w/ holster space", ECustomClothingComponent.Tops, 361, 1, false),
								new DutyCustomAvailableClothesItem("Forensics Jacket w/ holster space", ECustomClothingComponent.Tops, 361, 2, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket (Style 2) w/ holster space", ECustomClothingComponent.Tops, 361, 3, false),
								new DutyCustomAvailableClothesItem("Coroner Jacket w/ holster space", ECustomClothingComponent.Tops, 361, 4, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket Casual", ECustomClothingComponent.Tops, 362, 0, false),

								new DutyCustomAvailableClothesItem("Sheriff Jacket", ECustomClothingComponent.Tops, 363, 0, false),

								new DutyCustomAvailableClothesItem("Sheriff Jacket Brown", ECustomClothingComponent.Tops, 365, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket Green", ECustomClothingComponent.Tops, 365, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Zipped)", ECustomClothingComponent.Tops, 366, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Zipped)", ECustomClothingComponent.Tops, 366, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Unzipped)", ECustomClothingComponent.Tops, 367, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Unzipped)", ECustomClothingComponent.Tops, 367, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Zipped, No hood)", ECustomClothingComponent.Tops, 368, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Zipped, No hood)", ECustomClothingComponent.Tops, 368, 1, false),

								new DutyCustomAvailableClothesItem("Black Tactical Shirt", ECustomClothingComponent.Tops, 53, 0, false),
								new DutyCustomAvailableClothesItem("Tan Tactical Shirt", ECustomClothingComponent.Tops, 53, 2, false),
								new DutyCustomAvailableClothesItem("Grey Tactical Shirt", ECustomClothingComponent.Tops, 53, 1, false),
								new DutyCustomAvailableClothesItem("LSPD Short-sleeved Shirt", ECustomClothingComponent.Tops, 55, 0, false),
								new DutyCustomAvailableClothesItem("Dark Grey Polo (UC)", ECustomClothingComponent.Tops, 242, 0, true),
								new DutyCustomAvailableClothesItem("Black Hoody (UC)", ECustomClothingComponent.Tops, 182, 0, true)
							}
						},
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
					// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Gas Mask", ECustomClothingComponent.Masks, 46, 0, false),
								new DutyCustomAvailableClothesItem("Balaclava 1", ECustomClothingComponent.Masks, 52, 0, true),
								new DutyCustomAvailableClothesItem("Balaclava 2", ECustomClothingComponent.Masks, 35, 0, true),
							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
								new DutyCustomAvailableClothesItem("Torso 3", ECustomClothingComponent.Torsos, 4, 0, true),
								new DutyCustomAvailableClothesItem("Torso 4", ECustomClothingComponent.Torsos, 96, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Smart Pants", ECustomClothingComponent.Legs, 34, 0, true),
								new DutyCustomAvailableClothesItem("Casual Khaki Pants", ECustomClothingComponent.Legs, 49, 0, true),
								new DutyCustomAvailableClothesItem("Jeans", ECustomClothingComponent.Legs, 73, 0, true),

								new DutyCustomAvailableClothesItem("Cargo Pants (Green)", ECustomClothingComponent.Legs, 140, 0, true),
								new DutyCustomAvailableClothesItem("Cargo Pants (Black)", ECustomClothingComponent.Legs, 140, 1, true),
								new DutyCustomAvailableClothesItem("Cargo Pants (Grey)", ECustomClothingComponent.Legs, 140, 2, true),
								new DutyCustomAvailableClothesItem("Cargo Pants (Tan)", ECustomClothingComponent.Legs, 140, 4, true),
								new DutyCustomAvailableClothesItem("Black Formal Pants", ECustomClothingComponent.Legs, 139, 0, true),

								new DutyCustomAvailableClothesItem("Black Tactical Pants (SWAT)", ECustomClothingComponent.Legs, 141, 0, false),
								new DutyCustomAvailableClothesItem("Green Tactical Pants (SWAT)", ECustomClothingComponent.Legs, 141, 1, false),
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Police Badge on Chest", ECustomClothingComponent.BagsAndParachutes, 85, 0, false),
								new DutyCustomAvailableClothesItem("Badge & Cuffs on Belt", ECustomClothingComponent.BagsAndParachutes, 86, 0, false),
								new DutyCustomAvailableClothesItem("Police Badge on Belt", ECustomClothingComponent.BagsAndParachutes, 87, 0, false),

								new DutyCustomAvailableClothesItem("Badge on Chest, basic belt w/ no guns", ECustomClothingComponent.BagsAndParachutes, 88, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Star on Chest", ECustomClothingComponent.BagsAndParachutes, 89, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Star & Cuffs on belt", ECustomClothingComponent.BagsAndParachutes, 90, 0, false),

								new DutyCustomAvailableClothesItem("Hip Holster", ECustomClothingComponent.BagsAndParachutes, 91, 0, false),
								new DutyCustomAvailableClothesItem("Leg Holster", ECustomClothingComponent.BagsAndParachutes, 92, 0, false),
							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Formal Duty Shoes", ECustomClothingComponent.Legs, 24, 0, true),
								new DutyCustomAvailableClothesItem("Casual Vans (Detective)", ECustomClothingComponent.Legs, 49, 0, true),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Glock in waist holster", ECustomClothingComponent.Accessories, 105, 0, false),
								new DutyCustomAvailableClothesItem("Glock in small waist holster w/ two mag pouches", ECustomClothingComponent.Accessories, 106, 0, false),
								new DutyCustomAvailableClothesItem("Glock in leg holster", ECustomClothingComponent.Accessories, 107, 0, false),
								new DutyCustomAvailableClothesItem("Badge around neck", ECustomClothingComponent.Accessories, 108, 0, false),
								new DutyCustomAvailableClothesItem("Badge around neck 2", ECustomClothingComponent.Accessories, 109, 0, false),

								new DutyCustomAvailableClothesItem("Underarm holster and mags", ECustomClothingComponent.Accessories, 110, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff star around neck", ECustomClothingComponent.Accessories, 111, 0, false),

								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, No gun", ECustomClothingComponent.Accessories, 112, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, No gun (Metal Buttons)", ECustomClothingComponent.Accessories, 112, 1, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys, Taser, No gun", ECustomClothingComponent.Accessories, 113, 0, false),

								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pepper spray", ECustomClothingComponent.Accessories, 114, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pouches", ECustomClothingComponent.Accessories, 115, 0, false),

								new DutyCustomAvailableClothesItem("Best Belt w/ Everything", ECustomClothingComponent.Accessories, 121, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 15, 0, true),
								new DutyCustomAvailableClothesItem("Police Undershirt", ECustomClothingComponent.Undershirts, 207, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Undershirt", ECustomClothingComponent.Undershirts, 207, 1, false),

								new DutyCustomAvailableClothesItem("Base Undershirt LSPD", ECustomClothingComponent.Undershirts, 208, 0, false),
								new DutyCustomAvailableClothesItem("Base Undershirt Sheriff", ECustomClothingComponent.Undershirts, 208, 1, false),

								new DutyCustomAvailableClothesItem("Base Undershirt w/ Tie LSPD", ECustomClothingComponent.Undershirts, 209, 0, false),
								new DutyCustomAvailableClothesItem("Base Undershirt w/ Tie Sheriff", ECustomClothingComponent.Undershirts, 209, 1, false),
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Tan Large Vest", ECustomClothingComponent.BodyArmor, 18, 0, false),
								new DutyCustomAvailableClothesItem("Green Large Vest", ECustomClothingComponent.BodyArmor, 18, 1, false),
								new DutyCustomAvailableClothesItem("Black Large Vest", ECustomClothingComponent.BodyArmor, 18, 2, false),

								new DutyCustomAvailableClothesItem("FBI Badge", ECustomClothingComponent.BodyArmor, 53, 0, false),
								new DutyCustomAvailableClothesItem("FIB Badge #2", ECustomClothingComponent.BodyArmor, 54, 0, false),
								new DutyCustomAvailableClothesItem("FIB Badge #3", ECustomClothingComponent.BodyArmor, 55, 0, false),

								new DutyCustomAvailableClothesItem("Basic Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 57, 0, false),
								new DutyCustomAvailableClothesItem("Basic Vest (Police)", ECustomClothingComponent.BodyArmor, 57, 1, false),
								new DutyCustomAvailableClothesItem("Basic Vest (Sheriff K9)", ECustomClothingComponent.BodyArmor, 57, 4, false),
								new DutyCustomAvailableClothesItem("Basic Vest (LSPD Blue Line)", ECustomClothingComponent.BodyArmor, 57, 5, false),
								new DutyCustomAvailableClothesItem("Basic Vest (Sheriff Blue Line)", ECustomClothingComponent.BodyArmor, 57, 6, false),

								new DutyCustomAvailableClothesItem("High Visibility Vest (LSPD)", ECustomClothingComponent.BodyArmor, 57, 0, false),
								new DutyCustomAvailableClothesItem("High Visibility Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 57, 1, false),

								new DutyCustomAvailableClothesItem("Basic Unmarked Vest (Black)", ECustomClothingComponent.BodyArmor, 58, 0, false),
								new DutyCustomAvailableClothesItem("Basic Unmarked Vest (Green)", ECustomClothingComponent.BodyArmor, 58, 1, false),
								new DutyCustomAvailableClothesItem("Basic Unmarked Vest (Tan)", ECustomClothingComponent.BodyArmor, 58, 2, false),
								new DutyCustomAvailableClothesItem("Basic Unmarked Vest (White)", ECustomClothingComponent.BodyArmor, 58, 3, false),

								new DutyCustomAvailableClothesItem("Basic Vest 2 (Sheriff)", ECustomClothingComponent.BodyArmor, 59, 0, false),
								new DutyCustomAvailableClothesItem("Basic Vest 2 (Police)", ECustomClothingComponent.BodyArmor, 59, 1, false),

								new DutyCustomAvailableClothesItem("Basic Vest 3 (Black)", ECustomClothingComponent.BodyArmor, 60, 0, false),
								new DutyCustomAvailableClothesItem("Basic Vest 3 (Sheriff)", ECustomClothingComponent.BodyArmor, 60, 1, false),
								new DutyCustomAvailableClothesItem("Basic Vest 3 (Police)", ECustomClothingComponent.BodyArmor, 60, 2, false),
								new DutyCustomAvailableClothesItem("Basic Vest 3 (Negotiator)", ECustomClothingComponent.BodyArmor, 60, 3, false),

								new DutyCustomAvailableClothesItem("Better Vest (LSPD)", ECustomClothingComponent.BodyArmor, 60, 0, false),
								new DutyCustomAvailableClothesItem("Better Vest (Sheriff Green)", ECustomClothingComponent.BodyArmor, 60, 1, false),
								new DutyCustomAvailableClothesItem("Better Vest (Sheriff Black)", ECustomClothingComponent.BodyArmor, 60, 2, false),

								new DutyCustomAvailableClothesItem("Advanced Vest (LSPD Blue Line)", ECustomClothingComponent.BodyArmor, 62, 0, false),
								new DutyCustomAvailableClothesItem("Advanced Vest (Sheriff Blue Line)", ECustomClothingComponent.BodyArmor, 62, 1, false),
								new DutyCustomAvailableClothesItem("Advanced Vest (Police)", ECustomClothingComponent.BodyArmor, 62, 2, false),
								new DutyCustomAvailableClothesItem("Advanced Vest (LSPD)", ECustomClothingComponent.BodyArmor, 62, 3, false),

								new DutyCustomAvailableClothesItem("US Flag Vest (Sheriff)", ECustomClothingComponent.BodyArmor, 63, 0, false),
								new DutyCustomAvailableClothesItem("US Flag Vest (Police)", ECustomClothingComponent.BodyArmor, 63, 1, false),

								new DutyCustomAvailableClothesItem("Radio (Chest)", ECustomClothingComponent.BodyArmor, 64, 0, false),

								new DutyCustomAvailableClothesItem("Very Advanced Vest w/ Radio on Shoulder (Police Blue Line)", ECustomClothingComponent.BodyArmor, 65, 0, false),

								new DutyCustomAvailableClothesItem("Radio (Belt Clip)", ECustomClothingComponent.BodyArmor, 66, 0, false),

								new DutyCustomAvailableClothesItem("Very Advanced Vest w/ First Aid + Radio (Police)", ECustomClothingComponent.BodyArmor, 67, 0, false),
								new DutyCustomAvailableClothesItem("Very Advanced Vest w/ Mags + Taser on chest (Police)", ECustomClothingComponent.BodyArmor, 68, 0, false),

								new DutyCustomAvailableClothesItem("Simple Vest with Mags, Pen & Bodycam", ECustomClothingComponent.BodyArmor, 69, 0, false),

								new DutyCustomAvailableClothesItem("Bodycam", ECustomClothingComponent.BodyArmor, 70, 0, false),
								new DutyCustomAvailableClothesItem("Bodycam & Radio", ECustomClothingComponent.BodyArmor, 71, 0, false),
							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Two Chevrons", ECustomClothingComponent.Decals, 8, 1, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Hoodie", ECustomClothingComponent.Tops, 78, 0, true),
								new DutyCustomAvailableClothesItem("Black T-Shirt", ECustomClothingComponent.Tops, 79, 0, true),

								new DutyCustomAvailableClothesItem("Tactical Shirt (Dark Black)", ECustomClothingComponent.Tops, 370, 0, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Green)", ECustomClothingComponent.Tops, 370, 1, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Black)", ECustomClothingComponent.Tops, 370, 2, false),
								new DutyCustomAvailableClothesItem("Tactical Shirt (Tan)", ECustomClothingComponent.Tops, 370, 3, false),

								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Blank)", ECustomClothingComponent.Tops, 371, 0, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (LSPD)", ECustomClothingComponent.Tops, 371, 1, false),
								new DutyCustomAvailableClothesItem("Formal Short-sleeved Shirt (Sheriff)", ECustomClothingComponent.Tops, 371, 2, false),

								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Blank)", ECustomClothingComponent.Tops, 372, 0, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (LSPD)", ECustomClothingComponent.Tops, 372, 1, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt (Sheriff)", ECustomClothingComponent.Tops, 372, 2, false),

								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Blank)", ECustomClothingComponent.Tops, 373, 0, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (LSPD)", ECustomClothingComponent.Tops, 373, 1, false),
								new DutyCustomAvailableClothesItem("Formal Long-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 373, 2, false),

								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (SWAT)", ECustomClothingComponent.Tops, 374, 0, false),
								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 374, 1, false),
								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (K9)", ECustomClothingComponent.Tops, 374, 2, false),
								new DutyCustomAvailableClothesItem("Patrol Long-sleeved Shirt w/ Tie (Light Blue)", ECustomClothingComponent.Tops, 374, 3, false),

								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (SWAT)", ECustomClothingComponent.Tops, 375, 0, false),
								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (Sheriff)", ECustomClothingComponent.Tops, 375, 1, false),
								new DutyCustomAvailableClothesItem("Patrol Short-sleeved Shirt w/ Tie (K9)", ECustomClothingComponent.Tops, 375, 2, false),

								new DutyCustomAvailableClothesItem("Police Jacket", ECustomClothingComponent.Tops, 376, 0, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 1)", ECustomClothingComponent.Tops, 376, 1, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 2)", ECustomClothingComponent.Tops, 376, 2, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 3)", ECustomClothingComponent.Tops, 376, 3, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 4)", ECustomClothingComponent.Tops, 376, 4, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 5)", ECustomClothingComponent.Tops, 376, 5, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 6)", ECustomClothingComponent.Tops, 376, 6, false),
								new DutyCustomAvailableClothesItem("Police Jacket (Rank 7)", ECustomClothingComponent.Tops, 376, 7, false),

								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD)", ECustomClothingComponent.Tops, 378, 0, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD SWAT)", ECustomClothingComponent.Tops, 378, 1, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (LSPD #2)", ECustomClothingComponent.Tops, 378, 2, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Sheriff Green)", ECustomClothingComponent.Tops, 378, 3, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Sheriff White)", ECustomClothingComponent.Tops, 378, 4, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Firearms Instructor)", ECustomClothingComponent.Tops, 378, 5, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (Forensics)", ECustomClothingComponent.Tops, 378, 6, false),
								new DutyCustomAvailableClothesItem("Polo Shirt (SWAT)", ECustomClothingComponent.Tops, 378, 8, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket", ECustomClothingComponent.Tops, 379, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket", ECustomClothingComponent.Tops, 379, 1, false),
								new DutyCustomAvailableClothesItem("Coroner Jacket", ECustomClothingComponent.Tops, 379, 2, false),
								new DutyCustomAvailableClothesItem("Forensics Jacket", ECustomClothingComponent.Tops, 379, 3, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket (Style 2)", ECustomClothingComponent.Tops, 379, 4, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket w/ holster space", ECustomClothingComponent.Tops, 380, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket w/ holster space", ECustomClothingComponent.Tops, 380, 1, false),
								new DutyCustomAvailableClothesItem("Forensics Jacket w/ holster space", ECustomClothingComponent.Tops, 380, 2, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket (Style 2) w/ holster space", ECustomClothingComponent.Tops, 380, 3, false),
								new DutyCustomAvailableClothesItem("Coroner Jacket w/ holster space", ECustomClothingComponent.Tops, 380, 4, false),

								new DutyCustomAvailableClothesItem("LSPD Jacket (Police)", ECustomClothingComponent.Tops, 381, 0, false),
								new DutyCustomAvailableClothesItem("LSPD Jacket (Police Alt)", ECustomClothingComponent.Tops, 381, 1, false),
								new DutyCustomAvailableClothesItem("LSPD Jacket (High Vis Police)", ECustomClothingComponent.Tops, 381, 20, false),

								new DutyCustomAvailableClothesItem("Sheriff Jacket", ECustomClothingComponent.Tops, 382, 0, false),

								new DutyCustomAvailableClothesItem("Sheriff Jacket Brown", ECustomClothingComponent.Tops, 384, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff Jacket Green", ECustomClothingComponent.Tops, 384, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Zipped)", ECustomClothingComponent.Tops, 385, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Zipped)", ECustomClothingComponent.Tops, 385, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Unzipped)", ECustomClothingComponent.Tops, 386, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Unzipped)", ECustomClothingComponent.Tops, 386, 1, false),

								new DutyCustomAvailableClothesItem("LSPD High Visibility Jacket (Zipped, No hood)", ECustomClothingComponent.Tops, 387, 0, false),
								new DutyCustomAvailableClothesItem("Sheriff High Visibility Jacket (Zipped, No hood)", ECustomClothingComponent.Tops, 387, 1, false),
							}
						},
					}
				}
			}
		},

		// EMS
		{ EDutyType.EMS, new Dictionary<EGender, Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>>
			{
				// MALE
				{ EGender.Male, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
						// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
								new DutyCustomAvailableClothesItem("Torso 3", ECustomClothingComponent.Torsos, 4, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Firefighter Trousers", ECustomClothingComponent.Legs, 120, 0, false),
								new DutyCustomAvailableClothesItem("Black Cargo Pants", ECustomClothingComponent.Legs, 126, 0, true),
								new DutyCustomAvailableClothesItem("Black Formal Pants", ECustomClothingComponent.Legs, 10, 0, true),
								new DutyCustomAvailableClothesItem("Black Heavy Duty Pants", ECustomClothingComponent.Legs, 34, 0, false),
								new DutyCustomAvailableClothesItem("Pilot Pants", ECustomClothingComponent.Legs, 30, 0, false),
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Shoes", ECustomClothingComponent.Legs, 25, 0, true),
								new DutyCustomAvailableClothesItem("Heavy Duty Shoes", ECustomClothingComponent.Legs, 71, 0, false),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("ID Badge", ECustomClothingComponent.Accessories, 125, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys", ECustomClothingComponent.Accessories, 143, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys (Metal Buttons)", ECustomClothingComponent.Accessories, 143, 1, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pepper spray", ECustomClothingComponent.Accessories, 145, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pouches", ECustomClothingComponent.Accessories, 146, 0, false),
								new DutyCustomAvailableClothesItem("Breathing Apparatus", ECustomClothingComponent.Accessories, 147, 0, false),
								new DutyCustomAvailableClothesItem("Scuba Gear", ECustomClothingComponent.Accessories, 148, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 2, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 2", ECustomClothingComponent.Undershirts, 15, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 3", ECustomClothingComponent.Undershirts, 122, 0, true),
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Red Vest", ECustomClothingComponent.BodyArmor, 57, 3, false),
								new DutyCustomAvailableClothesItem("Yellow Vest", ECustomClothingComponent.BodyArmor, 57, 4, false),
								new DutyCustomAvailableClothesItem("High Visibility Vest (LSFD Public Relations)", ECustomClothingComponent.BodyArmor, 56, 3, false),
								new DutyCustomAvailableClothesItem("Radio Clipped to Belt", ECustomClothingComponent.BodyArmor, 67, 0, false),
							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
							new DutyCustomAvailableClothesItem("Paramedic Decal", ECustomClothingComponent.Decals, 57, 0, false),
							new DutyCustomAvailableClothesItem("Paramedic Decal 2", ECustomClothingComponent.Decals, 58, 0, false),
							new DutyCustomAvailableClothesItem("LSFD Decal", ECustomClothingComponent.Decals, 64, 0, false),
							new DutyCustomAvailableClothesItem("Fire Dept Decal (Front, Back & Badge)", ECustomClothingComponent.Decals, 79, 0, false),
							new DutyCustomAvailableClothesItem("Fire Dept Decal (Front, Back, Side & Badge)", ECustomClothingComponent.Decals, 80, 0, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Firefighter Jacket", ECustomClothingComponent.Tops, 314, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 2", ECustomClothingComponent.Tops, 315, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 3", ECustomClothingComponent.Tops, 358, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 4", ECustomClothingComponent.Tops, 364, 0, false),
								new DutyCustomAvailableClothesItem("Formal Shirt (White)", ECustomClothingComponent.Tops, 353, 4, true),
								new DutyCustomAvailableClothesItem("Formal Shirt (Red)", ECustomClothingComponent.Tops, 353, 5, true),
								new DutyCustomAvailableClothesItem("Formal Shirt w/ Tie (White)", ECustomClothingComponent.Tops, 354, 4, true),
								new DutyCustomAvailableClothesItem("Formal Shirt w/ Tie (Red)", ECustomClothingComponent.Tops, 354, 5, true),
								new DutyCustomAvailableClothesItem("Pilot", ECustomClothingComponent.Tops, 48, 0, false),
							}
						},
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
						// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Paramedic Pants", ECustomClothingComponent.Legs, 99, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Pants", ECustomClothingComponent.Legs, 126, 0, true),
								new DutyCustomAvailableClothesItem("Ceremonial Pants", ECustomClothingComponent.Legs, 34, 0, true),
								new DutyCustomAvailableClothesItem("Pilot Pants", ECustomClothingComponent.Legs, 29, 0, false)
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Shoes", ECustomClothingComponent.Legs, 25, 0, true),
								new DutyCustomAvailableClothesItem("Heavy Duty Shoes", ECustomClothingComponent.Legs, 74, 0, false),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("ID Badge", ECustomClothingComponent.Accessories, 97, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 2, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 2", ECustomClothingComponent.Undershirts, 159, 0, true)
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
							new DutyCustomAvailableClothesItem("Paramedic Decal", ECustomClothingComponent.Decals, 66, 0, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Paramedic Shirt", ECustomClothingComponent.Tops, 258, 0, false),
								new DutyCustomAvailableClothesItem("Fire Dept Jacket", ECustomClothingComponent.Tops, 325, 0, false),
								new DutyCustomAvailableClothesItem("Fire Dept Jacket 2", ECustomClothingComponent.Tops, 326, 0, false),
								new DutyCustomAvailableClothesItem("Ceremonial Shirt", ECustomClothingComponent.Tops, 48, 0, false),
								new DutyCustomAvailableClothesItem("Pilot Jacket", ECustomClothingComponent.Tops, 41, 0, false),
							}
						},
					}
				}
			}
		},

		// FD
		{ EDutyType.Fire, new Dictionary<EGender, Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>>
			{
				// MALE
				{ EGender.Male, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
						// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
								new DutyCustomAvailableClothesItem("Torso 3", ECustomClothingComponent.Torsos, 4, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Firefighter Trousers", ECustomClothingComponent.Legs, 120, 0, false),
								new DutyCustomAvailableClothesItem("Black Cargo Pants", ECustomClothingComponent.Legs, 126, 0, true),
								new DutyCustomAvailableClothesItem("Black Formal Pants", ECustomClothingComponent.Legs, 10, 0, true),
								new DutyCustomAvailableClothesItem("Black Heavy Duty Pants", ECustomClothingComponent.Legs, 34, 0, false),
								new DutyCustomAvailableClothesItem("Pilot Pants", ECustomClothingComponent.Legs, 30, 0, false),
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Shoes", ECustomClothingComponent.Legs, 25, 0, true),
								new DutyCustomAvailableClothesItem("Heavy Duty Shoes", ECustomClothingComponent.Legs, 71, 0, false),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Stethoscope", ECustomClothingComponent.Accessories, 126, 0, false),
								new DutyCustomAvailableClothesItem("ID Badge", ECustomClothingComponent.Accessories, 127, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys", ECustomClothingComponent.Accessories, 143, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio, Flashlight, Keys (Metal Buttons)", ECustomClothingComponent.Accessories, 143, 1, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pepper spray", ECustomClothingComponent.Accessories, 145, 0, false),
								new DutyCustomAvailableClothesItem("Belt w/ Radio & Pouches", ECustomClothingComponent.Accessories, 146, 0, false),
								new DutyCustomAvailableClothesItem("Breathing Apparatus", ECustomClothingComponent.Accessories, 147, 0, false),
								new DutyCustomAvailableClothesItem("Scuba Gear", ECustomClothingComponent.Accessories, 148, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 2, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 2", ECustomClothingComponent.Undershirts, 15, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 3", ECustomClothingComponent.Undershirts, 122, 0, true),
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Red Vest", ECustomClothingComponent.BodyArmor, 57, 3, false),
								new DutyCustomAvailableClothesItem("Yellow Vest", ECustomClothingComponent.BodyArmor, 57, 4, false),
								new DutyCustomAvailableClothesItem("High Visibility Vest (LSFD Public Relations)", ECustomClothingComponent.BodyArmor, 56, 3, false),
								new DutyCustomAvailableClothesItem("Radio Clipped to Belt", ECustomClothingComponent.BodyArmor, 67, 0, false),
							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
							new DutyCustomAvailableClothesItem("Paramedic Decal", ECustomClothingComponent.Decals, 57, 0, false),
							new DutyCustomAvailableClothesItem("Paramedic Decal 2", ECustomClothingComponent.Decals, 58, 0, false),
							new DutyCustomAvailableClothesItem("LSFD Decal", ECustomClothingComponent.Decals, 64, 0, false),
							new DutyCustomAvailableClothesItem("Fire Dept Decal (Front, Back & Badge)", ECustomClothingComponent.Decals, 79, 0, false),
							new DutyCustomAvailableClothesItem("Fire Dept Decal (Front, Back, Side & Badge)", ECustomClothingComponent.Decals, 80, 0, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Firefighter Jacket", ECustomClothingComponent.Tops, 314, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 2", ECustomClothingComponent.Tops, 315, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 3", ECustomClothingComponent.Tops, 358, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Jacket 4", ECustomClothingComponent.Tops, 364, 0, false),
								new DutyCustomAvailableClothesItem("Formal Shirt (White)", ECustomClothingComponent.Tops, 353, 4, true),
								new DutyCustomAvailableClothesItem("Formal Shirt (Red)", ECustomClothingComponent.Tops, 353, 5, true),
								new DutyCustomAvailableClothesItem("Formal Shirt w/ Tie (White)", ECustomClothingComponent.Tops, 354, 4, true),
								new DutyCustomAvailableClothesItem("Formal Shirt w/ Tie (Red)", ECustomClothingComponent.Tops, 354, 5, true),
								new DutyCustomAvailableClothesItem("Pilot", ECustomClothingComponent.Tops, 48, 0, false),
							}
						},
					}
				},

				// FEMALE
				{ EGender.Female, new Dictionary<ECustomClothingComponent, List<DutyCustomAvailableClothesItem>>
					{
						// MASKS
						{ ECustomClothingComponent.Masks, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// HAIR STYLES
						{ ECustomClothingComponent.HairStyles, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// TORSOS
						{ ECustomClothingComponent.Torsos, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Torso 1", ECustomClothingComponent.Torsos, 0, 0, true),
								new DutyCustomAvailableClothesItem("Torso 2", ECustomClothingComponent.Torsos, 3, 0, true),
							}
						},

						// LEGS
						{ ECustomClothingComponent.Legs, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Paramedic Pants", ECustomClothingComponent.Legs, 99, 0, false),
								new DutyCustomAvailableClothesItem("Firefighter Pants", ECustomClothingComponent.Legs, 126, 0, true),
								new DutyCustomAvailableClothesItem("Ceremonial Pants", ECustomClothingComponent.Legs, 34, 0, true),
								new DutyCustomAvailableClothesItem("Pilot Pants", ECustomClothingComponent.Legs, 29, 0, false)
							}
						},

						// BAGS & PARACHUTES
						{ ECustomClothingComponent.BagsAndParachutes, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// SHOES
						{ ECustomClothingComponent.Shoes, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Black Shoes", ECustomClothingComponent.Legs, 25, 0, true),
								new DutyCustomAvailableClothesItem("Heavy Duty Shoes", ECustomClothingComponent.Legs, 74, 0, false),
							}
						},

						// ACCESSORIES
						{ ECustomClothingComponent.Accessories, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("ID Badge", ECustomClothingComponent.Accessories, 97, 0, false),
							}
						},

						// UNDERSHIRTS
						{ ECustomClothingComponent.Undershirts, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Base Undershirt 1", ECustomClothingComponent.Undershirts, 2, 0, true),
								new DutyCustomAvailableClothesItem("Base Undershirt 2", ECustomClothingComponent.Undershirts, 159, 0, true)
							}
						},

						// BODY ARMOR
						{ ECustomClothingComponent.BodyArmor, new List<DutyCustomAvailableClothesItem>
							{

							}
						},

						// DECALS
						{ ECustomClothingComponent.Decals, new List<DutyCustomAvailableClothesItem>
							{
							new DutyCustomAvailableClothesItem("Paramedic Decal", ECustomClothingComponent.Decals, 66, 0, false),
							}
						},

						// TOPS
						{ ECustomClothingComponent.Tops, new List<DutyCustomAvailableClothesItem>
							{
								new DutyCustomAvailableClothesItem("Paramedic Shirt", ECustomClothingComponent.Tops, 258, 0, false),
								new DutyCustomAvailableClothesItem("Fire Dept Jacket", ECustomClothingComponent.Tops, 325, 0, false),
								new DutyCustomAvailableClothesItem("Fire Dept Jacket 2", ECustomClothingComponent.Tops, 326, 0, false),
								new DutyCustomAvailableClothesItem("Ceremonial Shirt", ECustomClothingComponent.Tops, 48, 0, false),
								new DutyCustomAvailableClothesItem("Pilot Jacket", ECustomClothingComponent.Tops, 41, 0, false),
							}
						},
					}
				}
			}
		}
	};

	public static Dictionary<EDutyType, Dictionary<EGender, Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>>> AvailableProps = new Dictionary<EDutyType, Dictionary<EGender, Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>>>
	{
		// LAW ENF
		{ EDutyType.Law_Enforcement, new Dictionary<EGender, Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>>
		{
			// MALE
			{ EGender.Male, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("LSPD Formal Police Hat", ECustomPropSlot.Hats, 150, 0, false),
					new DutyCustomAvailablePropItem("LSPD Formal Police Hat (High Command)", ECustomPropSlot.Hats, 150, 1, false),

					new DutyCustomAvailablePropItem("Sheriff/Trooper Formal Hat", ECustomPropSlot.Hats, 151, 0, false),

					new DutyCustomAvailablePropItem("Bicycle Patrol Hat", ECustomPropSlot.Hats, 153, 0, false),
					new DutyCustomAvailablePropItem("Motorcycle Helmet (Sheriff)", ECustomPropSlot.Hats, 154, 0, false),
					new DutyCustomAvailablePropItem("Motorcycle Helmet (LSPD)", ECustomPropSlot.Hats, 154, 1, false),

					new DutyCustomAvailablePropItem("Beanie (Plain/Undercover)", ECustomPropSlot.Hats, 155, 0, true),

					new DutyCustomAvailablePropItem("Winter Hat (Sheriff)", ECustomPropSlot.Hats, 156, 0, false),
					new DutyCustomAvailablePropItem("Winter Hat (Plain)", ECustomPropSlot.Hats, 157, 0, true),
					new DutyCustomAvailablePropItem("Baseball Cap (Sheriff)", ECustomPropSlot.Hats, 158, 0, false),
					new DutyCustomAvailablePropItem("Baseball Cap (FBI)", ECustomPropSlot.Hats, 158, 1, false),
					new DutyCustomAvailablePropItem("Baseball Cap (LS County)", ECustomPropSlot.Hats, 158, 2, false),

					new DutyCustomAvailablePropItem("Black Baseball Cap", ECustomPropSlot.Hats, 142, 0, true),
					new DutyCustomAvailablePropItem("SWAT Helmet (w/ NVG up)", ECustomPropSlot.Hats, 117, 0, false),
					new DutyCustomAvailablePropItem("SWAT Helmet (Basic)", ECustomPropSlot.Hats, 39, 0, false),
					new DutyCustomAvailablePropItem("Riot Helmet (Visor down)", ECustomPropSlot.Hats, 125, 0, false),
					new DutyCustomAvailablePropItem("LSPD Hat", ECustomPropSlot.Hats, 46, 0, false),
					new DutyCustomAvailablePropItem("Black Beanie (UC)", ECustomPropSlot.Hats, 5, 0, true),

					new DutyCustomAvailablePropItem("SWAT Helmet (Sheriff Green)", ECustomPropSlot.Hats, 159, 0, false),
					new DutyCustomAvailablePropItem("SWAT Helmet (Camo)", ECustomPropSlot.Hats, 159, 1, false),
					new DutyCustomAvailablePropItem("SWAT Helmet (Black)", ECustomPropSlot.Hats, 159, 2, false),

					new DutyCustomAvailablePropItem("Best SWAT Helmet (Police)", ECustomPropSlot.Hats, 165, 0, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet (Sheriff)", ECustomPropSlot.Hats, 165, 1, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet (FBI)", ECustomPropSlot.Hats, 165, 2, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet (Police Tan)", ECustomPropSlot.Hats, 165, 3, false),

					new DutyCustomAvailablePropItem("SWAT Helmet (No Headphones, Black)", ECustomPropSlot.Hats, 160, 0, false),

					new DutyCustomAvailablePropItem("Riot Helmet", ECustomPropSlot.Hats, 161, 0, false),

					new DutyCustomAvailablePropItem("Pilot (Plain White)", ECustomPropSlot.Hats, 162, 0, false),
					new DutyCustomAvailablePropItem("Pilot (LSPD)", ECustomPropSlot.Hats, 162, 1, false),
					new DutyCustomAvailablePropItem("Pilot (Sheriff)", ECustomPropSlot.Hats, 162, 2, false),
					new DutyCustomAvailablePropItem("Pilot (Black)", ECustomPropSlot.Hats, 162, 3, false),

					new DutyCustomAvailablePropItem("Riot Helmet (Blue)", ECustomPropSlot.Hats, 163, 0, false),
					new DutyCustomAvailablePropItem("Riot Helmet (Black)", ECustomPropSlot.Hats, 163, 1, false),
					new DutyCustomAvailablePropItem("Riot Helmet (Green)", ECustomPropSlot.Hats, 163, 2, false),

					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Blue)", ECustomPropSlot.Hats, 164, 0, false),
					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Black)", ECustomPropSlot.Hats, 164, 1, false),
					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Green)", ECustomPropSlot.Hats, 164, 2, false),

					new DutyCustomAvailablePropItem("Bicycle Patrol Hat Heavy (Black)", ECustomPropSlot.Hats, 165, 0, true),
					new DutyCustomAvailablePropItem("Bicycle Patrol Hat Heavy (Green)", ECustomPropSlot.Hats, 165, 1, true),

					new DutyCustomAvailablePropItem("Cowboy Hat (Tan)", ECustomPropSlot.Hats, 166, 0, false),
					new DutyCustomAvailablePropItem("Cowboy Hat (Black)", ECustomPropSlot.Hats, 166, 1, false),

					new DutyCustomAvailablePropItem("Pilot w/ no visor (Plain White)", ECustomPropSlot.Hats, 167, 0, false),
					new DutyCustomAvailablePropItem("Pilot w/ no visor (LSPD)", ECustomPropSlot.Hats, 167, 1, false),
					new DutyCustomAvailablePropItem("Pilot w/ no visor (Sheriff)", ECustomPropSlot.Hats, 167, 2, false),
					new DutyCustomAvailablePropItem("Pilot w/ no visor (Black)", ECustomPropSlot.Hats, 167, 3, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Sunglasses", ECustomPropSlot.Glasses, 9, 0, true),
					new DutyCustomAvailablePropItem("SWAT Goggles (Transparent Visor)", ECustomPropSlot.Glasses, 31, 0, false),
					new DutyCustomAvailablePropItem("SWAT Goggles (Black Visor)", ECustomPropSlot.Glasses, 31, 1, false),
					new DutyCustomAvailablePropItem("SWAT Tactical Glasses", ECustomPropSlot.Glasses, 32, 0, false),
				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Watch", ECustomPropSlot.Watches, 20, 0, true),
				}
				}
			}
			},

			// FEMALE
			{ EGender.Female, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{

					new DutyCustomAvailablePropItem("LSPD Formal Police Hat", ECustomPropSlot.Hats, 149, 0, false),
					new DutyCustomAvailablePropItem("Sheriff/Trooper Formal Hat", ECustomPropSlot.Hats, 150, 0, false),

					new DutyCustomAvailablePropItem("Bicycle Patrol Hat", ECustomPropSlot.Hats, 152, 0, true),
					new DutyCustomAvailablePropItem("Motorcycle Helmet (Sheriff)", ECustomPropSlot.Hats, 153, 0, false),
					new DutyCustomAvailablePropItem("Motorcycle Helmet (LSPD)", ECustomPropSlot.Hats, 153, 1, false),

					new DutyCustomAvailablePropItem("Beanie (Plain/Undercover)", ECustomPropSlot.Hats, 154, 0, true),

					new DutyCustomAvailablePropItem("Winter Hat (Sheriff)", ECustomPropSlot.Hats, 155, 0, false),

					new DutyCustomAvailablePropItem("Baseball Cap (Sheriff)", ECustomPropSlot.Hats, 157, 0, false),
					new DutyCustomAvailablePropItem("Baseball Cap (FBI)", ECustomPropSlot.Hats, 157, 1, false),
					new DutyCustomAvailablePropItem("Baseball Cap (LS County)", ECustomPropSlot.Hats, 157, 2, false),

					new DutyCustomAvailablePropItem("SWAT Helmet (Sheriff Green)", ECustomPropSlot.Hats, 158, 0, false),
					new DutyCustomAvailablePropItem("SWAT Helmet (Tan Camo)", ECustomPropSlot.Hats, 158, 1, false),
					new DutyCustomAvailablePropItem("SWAT Helmet (Black)", ECustomPropSlot.Hats, 158, 2, false),

					new DutyCustomAvailablePropItem("Basic SWAT Helmet", ECustomPropSlot.Hats, 159, 0, false),

					new DutyCustomAvailablePropItem("Tactical SWAT Helmet", ECustomPropSlot.Hats, 160, 0, false),

					new DutyCustomAvailablePropItem("Pilot (Plain White)", ECustomPropSlot.Hats, 161, 0, false),
					new DutyCustomAvailablePropItem("Pilot (LSPD)", ECustomPropSlot.Hats, 161, 1, false),
					new DutyCustomAvailablePropItem("Pilot (Sheriff)", ECustomPropSlot.Hats, 161, 2, false),
					new DutyCustomAvailablePropItem("Pilot (Black)", ECustomPropSlot.Hats, 161, 3, false),

					new DutyCustomAvailablePropItem("Riot Helmet (Blue)", ECustomPropSlot.Hats, 162, 0, false),
					new DutyCustomAvailablePropItem("Riot Helmet (Black)", ECustomPropSlot.Hats, 162, 1, false),
					new DutyCustomAvailablePropItem("Riot Helmet (Green)", ECustomPropSlot.Hats, 162, 2, false),

					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Blue)", ECustomPropSlot.Hats, 163, 0, false),
					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Black)", ECustomPropSlot.Hats, 163, 1, false),
					new DutyCustomAvailablePropItem("Riot Helmet w/ shield up (Green)", ECustomPropSlot.Hats, 163, 2, false),

					new DutyCustomAvailablePropItem("Best SWAT Helmet (Police)", ECustomPropSlot.Hats, 164, 0, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet (Sheriff)", ECustomPropSlot.Hats, 164, 1, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet (FBI)", ECustomPropSlot.Hats, 164, 2, false),
					new DutyCustomAvailablePropItem("Best SWAT Helmet(Police Tan)", ECustomPropSlot.Hats, 164, 3, false),

					new DutyCustomAvailablePropItem("Cowboy Hat", ECustomPropSlot.Hats, 165, 0, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("SWAT Goggles (Transparent Visor)", ECustomPropSlot.Glasses, 33, 0, false),
					new DutyCustomAvailablePropItem("SWAT Goggles (Black Visor)", ECustomPropSlot.Glasses, 33, 1, false),
					new DutyCustomAvailablePropItem("SWAT Tactical Glasses", ECustomPropSlot.Glasses, 34, 0, false),
				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Watch", ECustomPropSlot.Watches, 20, 0, true),
				}
				}
			}
			}
		}
		},

		// EMS
		{ EDutyType.EMS, new Dictionary<EGender, Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>>
		{
			// MALE
			{ EGender.Male, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Paramedic Baseball Cap", ECustomPropSlot.Hats, 122, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet", ECustomPropSlot.Hats, 137, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet with goggles", ECustomPropSlot.Hats, 138, 0, false),
					new DutyCustomAvailablePropItem("Ceremonial Hat", ECustomPropSlot.Hats, 46, 0, false),

					new DutyCustomAvailablePropItem("FF Helmet (Probationary)", ECustomPropSlot.Hats, 152, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet (Firefighter)", ECustomPropSlot.Hats, 152, 1, false),
					new DutyCustomAvailablePropItem("FF Helmet (Captain)", ECustomPropSlot.Hats, 152, 2, false),
					new DutyCustomAvailablePropItem("FF Helmet (Chief)", ECustomPropSlot.Hats, 152, 3, false),
					new DutyCustomAvailablePropItem("FF Helmet (EMT)", ECustomPropSlot.Hats, 152, 4, false),
					new DutyCustomAvailablePropItem("FF Helmet (Public Relations)", ECustomPropSlot.Hats, 152, 5, false),
					new DutyCustomAvailablePropItem("FF Helmet (Fire Marshall)", ECustomPropSlot.Hats, 152, 6, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Sunglasses", ECustomPropSlot.Glasses, 9, 0, true),
					new DutyCustomAvailablePropItem("Goggles (Transparent Visor)", ECustomPropSlot.Glasses, 31, 0, false),
					new DutyCustomAvailablePropItem("Goggles (Black Visor)", ECustomPropSlot.Glasses, 31, 1, false),
				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Watch", ECustomPropSlot.Watches, 20, 0, true),
				}
				}
			}
			},

			// FEMALE
			{ EGender.Female, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Paramedic Baseball Cap", ECustomPropSlot.Hats, 121, 0, false),
					new DutyCustomAvailablePropItem("Firefighter Helmet", ECustomPropSlot.Hats, 136, 0, false),
					new DutyCustomAvailablePropItem("Firefighter Helmet w/ Goggles", ECustomPropSlot.Hats, 137, 0, false),
					new DutyCustomAvailablePropItem("Ceremonial Hat", ECustomPropSlot.Hats, 45, 0, false),

					new DutyCustomAvailablePropItem("FF Helmet (Probationary)", ECustomPropSlot.Hats, 151, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet (Firefighter)", ECustomPropSlot.Hats, 151, 1, false),
					new DutyCustomAvailablePropItem("FF Helmet (Captain)", ECustomPropSlot.Hats, 151, 2, false),
					new DutyCustomAvailablePropItem("FF Helmet (Chief)", ECustomPropSlot.Hats, 151, 3, false),
					new DutyCustomAvailablePropItem("FF Helmet (EMT)", ECustomPropSlot.Hats, 151, 4, false),
					new DutyCustomAvailablePropItem("FF Helmet (Public Relations)", ECustomPropSlot.Hats, 151, 5, false),
					new DutyCustomAvailablePropItem("FF Helmet (Fire Marshall)", ECustomPropSlot.Hats, 151, 6, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{

				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{

				}
				}
			}
			}
		}
		},

		// FD
		{ EDutyType.Fire, new Dictionary<EGender, Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>>
		{
			// MALE
			{ EGender.Male, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Formal Hat", ECustomPropSlot.Hats, 46, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet", ECustomPropSlot.Hats, 137, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet with goggles", ECustomPropSlot.Hats, 138, 0, false),

					new DutyCustomAvailablePropItem("FF Helmet (Probationary)", ECustomPropSlot.Hats, 152, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet (Firefighter)", ECustomPropSlot.Hats, 152, 1, false),
					new DutyCustomAvailablePropItem("FF Helmet (Captain)", ECustomPropSlot.Hats, 152, 2, false),
					new DutyCustomAvailablePropItem("FF Helmet (Chief)", ECustomPropSlot.Hats, 152, 3, false),
					new DutyCustomAvailablePropItem("FF Helmet (EMT)", ECustomPropSlot.Hats, 152, 4, false),
					new DutyCustomAvailablePropItem("FF Helmet (Public Relations)", ECustomPropSlot.Hats, 152, 5, false),
					new DutyCustomAvailablePropItem("FF Helmet (Fire Marshall)", ECustomPropSlot.Hats, 152, 6, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Sunglasses", ECustomPropSlot.Glasses, 9, 0, true),
					new DutyCustomAvailablePropItem("Goggles (Transparent Visor)", ECustomPropSlot.Glasses, 31, 0, false),
					new DutyCustomAvailablePropItem("Goggles (Black Visor)", ECustomPropSlot.Glasses, 31, 1, false),
				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Watch", ECustomPropSlot.Watches, 20, 0, true),
				}
				}
			}
			},

			// FEMALE
			{ EGender.Female, new Dictionary<ECustomPropSlot, List<DutyCustomAvailablePropItem>>
			{
				// HATS
				{ ECustomPropSlot.Hats, new List<DutyCustomAvailablePropItem>
				{
					new DutyCustomAvailablePropItem("Paramedic Baseball Cap", ECustomPropSlot.Hats, 121, 0, false),
					new DutyCustomAvailablePropItem("Firefighter Helmet", ECustomPropSlot.Hats, 136, 0, false),
					new DutyCustomAvailablePropItem("Firefighter Helmet w/ Goggles", ECustomPropSlot.Hats, 137, 0, false),
					new DutyCustomAvailablePropItem("Ceremonial Hat", ECustomPropSlot.Hats, 45, 0, false),

					new DutyCustomAvailablePropItem("FF Helmet (Probationary)", ECustomPropSlot.Hats, 151, 0, false),
					new DutyCustomAvailablePropItem("FF Helmet (Firefighter)", ECustomPropSlot.Hats, 151, 1, false),
					new DutyCustomAvailablePropItem("FF Helmet (Captain)", ECustomPropSlot.Hats, 151, 2, false),
					new DutyCustomAvailablePropItem("FF Helmet (Chief)", ECustomPropSlot.Hats, 151, 3, false),
					new DutyCustomAvailablePropItem("FF Helmet (EMT)", ECustomPropSlot.Hats, 151, 4, false),
					new DutyCustomAvailablePropItem("FF Helmet (Public Relations)", ECustomPropSlot.Hats, 151, 5, false),
					new DutyCustomAvailablePropItem("FF Helmet (Fire Marshall)", ECustomPropSlot.Hats, 151, 6, false),
				}
				},

				// GLASSES
				{ ECustomPropSlot.Glasses, new List<DutyCustomAvailablePropItem>
				{

				}
				},

				// WATCHES
				{ ECustomPropSlot.Watches, new List<DutyCustomAvailablePropItem>
				{

				}
				}
			}
			}
		}
		}
	};

	public static Dictionary<EDutyType, Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>> CustomSkinPresets = new Dictionary<EDutyType, Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>>
	{
		{ EDutyType.Law_Enforcement, new Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>()
			{
				{ EDutyCustomSkinPresets.PD_Female_LSPD_Officer, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Female_LSPD_Officer, EGender.Female, "LSPD Officer", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 3, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 34, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 373, 1, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 149, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 121, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 67, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.PD_Female_LSPD_Deputy, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Female_LSPD_Deputy, EGender.Female, "Sheriffs Deputy", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 140, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 378, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 150, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 121, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 68, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_Deputy, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_Deputy, EGender.Male, "Sheriffs Deputy #1", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 133, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 359, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 158, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 138, 0, 0), // glock in leg holster
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 57, 7, 0), // armor
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 90, 0, 0)
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_Deputy_2, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_Deputy_2, EGender.Male, "Sheriffs Deputy #2", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 133, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 359, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 138, 0, 0), // glock in leg holster
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 58, 1, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 158, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 90, 0, 0)
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_LSPD_Officer, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_LSPD_Officer, EGender.Male, "LSPD Officer", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 35, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 354, 1, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 58, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 150, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Decals, 87, 1, 0),
																//new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 136, 1, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 151, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 65, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 93, 0, 0)
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_Sheriff_Detective, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_Sheriff_Detective, EGender.Male, "Sheriff Detective #1", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 47, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 359, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 158, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Glasses, 9, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Watches, 20, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 137, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 90, 0, 0)
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_Sheriff_Detective_1, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_Sheriff_Detective_1, EGender.Male, "Sheriff Detective #2", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 47, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 359, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 158, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Glasses, 9, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Watches, 20, 0),

																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 142, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 91, 0, 0)
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_LSPD_SWAT, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_LSPD_SWAT, EGender.Male, "LSPD SWAT", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 135, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 355, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 58, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 165, 0),

																new DutyCustomSkinPresetSlot(ECustomPropSlot.Glasses, 31, 1),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 138, 0, 0), // leg holster
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 66, 0, 0), // swat with backpack
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 86, 0, 0) // belt and cuffs on belt
															})
				},

				{ EDutyCustomSkinPresets.PD_Male_Sheriff_SWAT, new DutyCustomSkinPreset(EDutyCustomSkinPresets.PD_Male_Sheriff_SWAT, EGender.Male, "Sheriff SWAT", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 135, 1, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 24, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 359, 3, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 58, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 165, 1),

																new DutyCustomSkinPresetSlot(ECustomPropSlot.Glasses, 32, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 138, 0, 0), // leg holster
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BodyArmor, 66, 2, 0), // swat with backpack
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.BagsAndParachutes, 90, 0, 0) // belt and cuffs on belt
															})
				},
			}
		},

		{ EDutyType.EMS, new Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>()
			{
				{ EDutyCustomSkinPresets.FD_Male_Paramedic, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Paramedic, EGender.Male, "Paramedic", new List<DutyCustomSkinPresetSlot>
												{
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 250, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 129, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 96, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 122, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 127, 0, 0),
													new DutyCustomSkinPresetSlot(ECustomClothingComponent.Decals, 57, 0, 0),
												})
				},

				{ EDutyCustomSkinPresets.FD_Female_Paramedic, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Paramedic, EGender.Female, "Paramedic", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 258, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 159, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 99, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 121, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Accessories, 97, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Decals, 66, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Male_Ceremonial, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Ceremonial, EGender.Male, "Ceremonial", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 55, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 10, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 46, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Ceremonial, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Ceremonial, EGender.Female, "Ceremonial", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 34, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 45, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Male_Pilot, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Pilot, EGender.Male, "Pilot", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 30, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Pilot, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Pilot, EGender.Female, "Pilot", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 41, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 29, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
															})
				},
			}
		},
		

		// FD
		{ EDutyType.Fire, new Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>()
			{
				{ EDutyCustomSkinPresets.FD_Male_Firefighter, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Firefighter, EGender.Male, "Firefighter", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 314, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 120, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 71, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Decals, 64, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 137, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Male_Firefighter_2, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Firefighter_2, EGender.Male, "Firefighter 2", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 315, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 120, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 71, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Decals, 64, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 138, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Firefighter, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Firefighter, EGender.Female, "Firefighter", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 3, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 325, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 126, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 74, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 136, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Firefighter_2, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Firefighter_2, EGender.Female, "Firefighter 2", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 3, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 326, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 126, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 74, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 137, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Male_Ceremonial, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Ceremonial, EGender.Male, "Ceremonial", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 55, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 10, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 46, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Ceremonial, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Ceremonial, EGender.Female, "Ceremonial", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 34, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomPropSlot.Hats, 45, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Male_Pilot, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Male_Pilot, EGender.Male, "Pilot", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 48, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 15, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 30, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.FD_Female_Pilot, new DutyCustomSkinPreset(EDutyCustomSkinPresets.FD_Female_Pilot, EGender.Female, "Pilot", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 0, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 41, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 2, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 29, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 25, 0, 0),
															})
				},
			}
		},

		// News
		{ EDutyType.News, new Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>()
			{
				{ EDutyCustomSkinPresets.News_Male, new DutyCustomSkinPreset(EDutyCustomSkinPresets.News_Male, EGender.Male, "News Reporter", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 1, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 25, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 20, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 4, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Undershirts, 3, 0, 0),
															})
				},

				{ EDutyCustomSkinPresets.News_Female, new DutyCustomSkinPreset(EDutyCustomSkinPresets.News_Female, EGender.Female, "News Reporter", new List<DutyCustomSkinPresetSlot>
															{
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Torsos, 6, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Legs, 7, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Shoes, 7, 0, 0),
																new DutyCustomSkinPresetSlot(ECustomClothingComponent.Tops, 7, 0, 0),
															})
				},
			}
		},

		// Towing
		{ EDutyType.Towing, new Dictionary<EDutyCustomSkinPresets, DutyCustomSkinPreset>()
			{
			}
		}
	};
}

public static class SkinConstants
{
	public const int NumModels = 12;
	public const int NumProps = 5;

	// NOTE: MASK 71 is reserved, it was given out as a halloween gift
	// NOTE: MASK 8 is reserved, it was given out as a christmas gift
	private static int[] Masks = { 0, 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
		59, 63, 64, 66, 68, 69, 70, 73, 74, 75, 79, 80, 81, 82, 84, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114,
		115, 116, 117, 118, 119, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164,
		166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184};

	public static List<int> GetMasks(EGender gender)
	{
		List<int> lstItems = new List<int>();
		lstItems.AddRange(Masks);
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Masks, gender, ref lstItems);
		return lstItems;
	}

	/*
	The following masks are disabled:

	165 = Easter
	8, 9, 10, 31, 76, 77, 78, 83  = Christmas
	85 = Thanksgiving
	30, 60, 65, 72, 67, 62, 61 = Halloween
	*/

	public static List<int> GetDecalsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		lstItems.AddRange(gender == EGender.Male ? MaleDecals : FemaleDecals);
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Decals, gender, ref lstItems);
		return lstItems;
	}
	private static int[] MaleDecals = { 0, 8, 9, 11, 12, 13, 14, 15, 44, 45, 57, 58, 61, 62, 63, 64, 66, 68, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 };
	private static int[] FemaleDecals = { 0, 7, 8, 10, 11, 12, 13, 14, 52, 53, 65, 66, 70, 71, 72, 75, 76, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97 };

	public static List<int> GetHatsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		lstItems.AddRange(gender == EGender.Male ? MaleHats : FemaleHats);
		DutyCustomSkins.RemoveAnyDutyPropItemsFromArray(ECustomPropSlot.Hats, gender, ref lstItems);
		return lstItems;
	}
	private static int[] MaleHats = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 20, 21, 22, 23, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 40, 41, 42, 43, 44, 45, 54, 55, 56, 58, 60, 61, 63, 64, 65, 66, 76, 77, 83, 94, 95, 96, 97, 98, 99, 100, 102, 103, 104, 105, 106, 107, 108, 109, 110, 114, 120, 121, 122, 130, 131, 132, 135, 136, 139, 140, 142, 143, 145, 146, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167 };
	private static int[] FemaleHats = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 20, 21, 22, 23, 24, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 39, 40, 41, 42, 43, 44, 53, 54, 55, 56, 57, 58, 60, 61, 63, 64, 65, 75, 76, 82, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 113, 119, 120, 121, 129, 130, 131, 134, 135, 138, 139, 141, 142, 144, 145, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168 };

	public static List<int> GetGlassesMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		lstItems.AddRange(gender == EGender.Male ? MaleGlasses : FemaleGlasses);
		DutyCustomSkins.RemoveAnyDutyPropItemsFromArray(ECustomPropSlot.Glasses, gender, ref lstItems);
		return lstItems;
	}
	private static int[] MaleGlasses = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 28, 29, 30, 31, 32, 33 };
	private static int[] FemaleGlasses = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 30, 31, 32, 33, 34, 35 };

	public static List<int> GetEaringsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleEarringsMax : FemaleEarringsMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyPropItemsFromArray(ECustomPropSlot.Ears, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleEarringsMax = 40;
	private static int FemaleEarringsMax = 21;

	public static List<int> GetTorsoMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleTorsoMax : FemaleTorsoMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Torsos, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleTorsoMax = 184;
	private static int FemaleTorsoMax = 229;
	public static List<int> GetTopsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleTopsMax : FemaleTopsMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Tops, gender, ref lstItems);

		return lstItems;
	}

	private static int MaleTopsMax = 403;
	private static int FemaleTopsMax = 424;

	public static List<int> GetUndershirtMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleUndershirtMax : FemaleUndershirtMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Undershirts, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleUndershirtMax = 175;
	private static int FemaleUndershirtMax = 216;

	public static List<int> GetAccessoriesMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleAccessoriesMax : FemaleAccessoriesMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Accessories, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleAccessoriesMax = 151;
	private static int FemaleAccessoriesMax = 122;

	public static List<int> GetWatchesMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleWatchesMax : FemaleWatchesMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyPropItemsFromArray(ECustomPropSlot.Watches, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleWatchesMax = 39;
	private static int FemaleWatchesMax = 28;

	public static List<int> GetBraceletsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleBraceletsMax : FemaleBraceletsMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyPropItemsFromArray(ECustomPropSlot.Bracelets, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleBraceletsMax = 7;
	private static int FemaleBraceletsMax = 14;

	public static List<int> GetLegsMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleLegsMax : FemaleLegsMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Legs, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleLegsMax = 143;
	private static int FemaleLegsMax = 163;

	public static List<int> GetShoesMaxForGender(EGender gender)
	{
		List<int> lstItems = new List<int>();
		for (int i = 0; i < (gender == EGender.Male ? MaleShoesMax : FemaleShoesMax); ++i)
		{
			lstItems.Add(i);
		}
		DutyCustomSkins.RemoveAnyDutyClothingItemsFromArray(ECustomClothingComponent.Shoes, gender, ref lstItems);

		return lstItems;
	}
	private static int MaleShoesMax = 98;
	private static int FemaleShoesMax = 103;

	public static uint[] GetPremadeSkinsForGender(EGender gender) { return gender == EGender.Male ? CharacterConstants.g_PremadeMaleSkins : CharacterConstants.g_PremadeFemaleSkins; }
}

public enum EOverlayTypes
{
	Blemishes = 0,
	FacialHair = 1,
	Eyebrows = 2,
	Ageing = 3,
	Makeup = 4,
	Blush = 5,
	Complexion = 6,
	SunDamage = 7,
	Lipstick = 8,
	MolesFreckles = 9,
	ChestHair = 10,
	BodyBlemishes = 11,
	AddBodyBlemishes = 12
}

public static class CharacterHexCodes
{
	public static string[] HexCodes_Blush = new string[]
	{
		"#36312B",
		"#3D3630",
		"#43362D",
		"#433123",
		"#5B3A2B",
		"#6D4734",
		"#744A32",
		"#76533D",
		"#7A5C44",
		"#7C6249",
		"#8D6F53",
		"#977955",
		"#95825A",
		"#A68856",
		"#B59763",
		"#BB9E72",
		"#8E6C50",
		"#79503C",
		"#65362C",
		"#60302C",
		"#6D302B",
		"#803A30",
		"#9A4433",
		"#B54D32",
		"#8D472E",
		"#A14A2F",
		"#6C6658",
		"#7D7568",
		"#A39D91",
		"#B9B1A4",
		"#64535D",
		"#6B5061",
		"#763D50",
		"#D550BB",
		"#D04679",
		"#CF8093",
		"#38857F",
		"#366B7B",
		"#2E3E5F",
		"#568853",
		"#417A59",
		"#395D51",
		"#9AA336",
		"#7C9F2B",
		"#5C9335",
		"#BB9B50",
		"#C8A429",
		"#C98730",
		"#D06B23",
		"#D96029",
		"#D45D23",
		"#CA4B2B",
		"#C23720",
		"#93221C",
		"#802922",
		"#392A25",
		"#45362F",
		"#423129",
		"#4F392C",
		"#4A3931",
		"#473833",
		"#3E352C",
		"#4B3E35",
		"#66523A"
	};

	public static string[] HexCodes_Eyebrows = new string[]
	{
		"#3C1B16",
		"#5F3D34",
		"#45221C",
		"#421F19",
		"#3F2116",
		"#492517",
		"#4A2A1D",
		"#4A3021",
		"#4E3224",
		"#5B402F",
		"#5C4734",
		"#5C4533",
		"#61563A",
		"#6B552C",
		"#918668",
		"#7E6E4A",
		"#58422D",
		"#5A352C",
		"#482018",
		"#40130E",
		"#3E0F07",
		"#501710",
		"#5C1B15",
		"#672212",
		"#5C2C1E",
		"#642714",
		"#4A3730",
		"#58473D",
		"#636359",
		"#78786E",
		"#3E272D",
		"#52303E",
		"#7A575D",
		"#963386",
		"#8F224B",
		"#864959",
		"#355250",
		"#303C4A",
		"#31293E",
		"#354F28",
		"#223724",
		"#32312D",
		"#556316",
		"#4B610E",
		"#414E18",
		"#79662B",
		"#886F12",
		"#844D0C",
		"#8B3B0A",
		"#95350F",
		"#933309",
		"#A5583A",
		"#862711",
		"#6B1312",
		"#430C09",
		"#39180F",
		"#341512",
		"#321612",
		"#361E12",
		"#321914",
		"#421D17",
		"#2D100A",
		"#3F3222",
		"#594932",
		"#5E4E37",
		"#614F37",
		"#7A644F",
		"#69533B",
		"#5B4D33",
		"#5B4B32",
		"#594D35"
	};

	public static string[] HexCodes_FacialHair = new string[]
	{
		"#3C1B16",
		"#5F3D34",
		"#45221C",
		"#421F19",
		"#3F2116",
		"#492517",
		"#4A2A1D",
		"#4A3021",
		"#4E3224",
		"#5B402F",
		"#5C4734",
		"#5C4533",
		"#61563A",
		"#6B552C",
		"#918668",
		"#7E6E4A",
		"#58422D",
		"#5A352C",
		"#482018",
		"#40130E",
		"#3E0F07",
		"#501710",
		"#5C1B15",
		"#672212",
		"#5C2C1E",
		"#642714",
		"#4A3730",
		"#58473D",
		"#636359",
		"#78786E",
		"#3E272D",
		"#52303E",
		"#7A575D",
		"#963386",
		"#8F224B",
		"#864959",
		"#355250",
		"#303C4A",
		"#31293E",
		"#354F28",
		"#223724",
		"#32312D",
		"#556316",
		"#4B610E",
		"#414E18",
		"#79662B",
		"#886F12",
		"#844D0C",
		"#8B3B0A",
		"#95350F",
		"#933309",
		"#A5583A",
		"#862711",
		"#6B1312",
		"#430C09",
		"#39180F",
		"#341512",
		"#321612",
		"#361E12",
		"#321914",
		"#421D17",
		"#2D100A",
		"#3F3222",
		"#594932"
	};

	public static string[] HexCodes_ChestHair = new string[]
	{
		"#3C1B16",
		"#5F3D34",
		"#45221C",
		"#421F19",
		"#3F2116",
		"#492517",
		"#4A2A1D",
		"#4A3021",
		"#4E3224",
		"#5B402F",
		"#5C4734",
		"#5C4533",
		"#61563A",
		"#6B552C",
		"#918668",
		"#7E6E4A",
		"#58422D",
		"#5A352C",
		"#482018",
		"#40130E",
		"#3E0F07",
		"#501710",
		"#5C1B15",
		"#672212",
		"#5C2C1E",
		"#642714",
		"#4A3730",
		"#58473D",
		"#636359",
		"#78786E",
		"#3E272D",
		"#52303E",
		"#7A575D",
		"#963386",
		"#8F224B",
		"#864959",
		"#355250",
		"#303C4A",
		"#31293E",
		"#354F28",
		"#223724",
		"#32312D",
		"#556316",
		"#4B610E",
		"#414E18",
		"#79662B",
		"#886F12",
		"#844D0C",
		"#8B3B0A",
		"#95350F",
		"#933309",
		"#A5583A",
		"#862711",
		"#6B1312",
		"#430C09",
		"#39180F",
		"#341512",
		"#321612",
		"#361E12",
		"#321914",
		"#421D17",
		"#2D100A",
		"#3F3222",
		"#594932"
	};

	public static string[] HexCodes_Hair = new string[]
	{
		"#3C1B16",
		"#5F3D34",
		"#45221C",
		"#421F19",
		"#3F2116",
		"#492517",
		"#4A2A1D",
		"#4A3021",
		"#4E3224",
		"#5B402F",
		"#5C4734",
		"#5C4533",
		"#61563A",
		"#6B552C",
		"#918668",
		"#7E6E4A",
		"#58422D",
		"#5A352C",
		"#482018",
		"#40130E",
		"#3E0F07",
		"#501710",
		"#5C1B15",
		"#672212",
		"#5C2C1E",
		"#642714",
		"#4A3730",
		"#58473D",
		"#636359",
		"#78786E",
		"#3E272D",
		"#52303E",
		"#7A575D",
		"#963386",
		"#8F224B",
		"#864959",
		"#355250",
		"#303C4A",
		"#31293E",
		"#354F28",
		"#223724",
		"#32312D",
		"#556316",
		"#4B610E",
		"#414E18",
		"#79662B",
		"#886F12",
		"#844D0C",
		"#8B3B0A",
		"#95350F",
		"#933309",
		"#A5583A",
		"#862711",
		"#6B1312",
		"#430C09",
		"#39180F",
		"#341512",
		"#321612",
		"#361E12",
		"#321914",
		"#421D17",
		"#2D100A",
		"#3F3222",
		"#594932"
	};

	public static string[] HexCodes_Lipstick = new string[]
	{
		"#981B29",
		"#B73154",
		"#B34F6B",
		"#A65F71",
		"#A6606B",
		"#A64B52",
		"#863031",
		"#A56856",
		"#B18874",
		"#B89D8C",
		"#BA948B",
		"#A8745F",
		"#AB6448",
		"#A14C30",
		"#AD7371",
		"#B98591",
		"#CB98AB",
		"#CC83A1",
		"#C54880",
		"#AD5472",
		"#722A36",
		"#705053",
		"#A82B31",
		"#C92D42",
		"#C71E25",
		"#BE475B",
		"#C340A8",
		"#BB25AA",
		"#9B17A0",
		"#7D1F75",
		"#813D78",
		"#582462",
		"#6D2499",
		"#20356E",
		"#1E4797",
		"#2176AC",
		"#24A5B8",
		"#21B9BA",
		"#23BF9A",
		"#26B575",
		"#219331",
		"#1C750B",
		"#71C033",
		"#B5CE2A",
		"#CCCE47",
		"#D6CB35",
		"#D3BC32",
		"#D29624",
		"#D0641C",
		"#A0632A",
		"#CFBB7C",
		"#D2CDAF",
		"#C9CCC5",
		"#AEAEA6",
		"#929487",
		"#5A4D45",
		"#251515",
		"#5A9991",
		"#51778A",
		"#1C2956",
		"#997F64",
		"#84644B",
		"#6D5843",
		"#473C36"
	};

	public static string[] HexCodes_Makeup = new string[]
	{
		"#3C1B16",
		"#5F3D34",
		"#45221C",
		"#421F19",
		"#3F2116",
		"#492517",
		"#4A2A1D",
		"#4A3021",
		"#4E3224",
		"#5B402F",
		"#5C4734",
		"#5C4533",
		"#61563A",
		"#6B552C",
		"#918668",
		"#7E6E4A",
		"#58422D",
		"#5A352C",
		"#482018",
		"#40130E",
		"#3E0F07",
		"#501710",
		"#5C1B15",
		"#672212",
		"#5C2C1E",
		"#642714",
		"#4A3730",
		"#58473D",
		"#636359",
		"#78786E",
		"#3E272D",
		"#52303E",
		"#7A575D",
		"#963386",
		"#8F224B",
		"#864959",
		"#355250",
		"#303C4A",
		"#31293E",
		"#354F28",
		"#223724",
		"#32312D",
		"#556316",
		"#4B610E",
		"#414E18",
		"#79662B",
		"#886F12",
		"#844D0C",
		"#8B3B0A",
		"#95350F",
		"#933309",
		"#A5583A",
		"#862711",
		"#6B1312",
		"#430C09",
		"#39180F",
		"#341512",
		"#321612",
		"#361E12",
		"#321914",
		"#421D17",
		"#2D100A",
		"#3F3222",
		"#594932"
	};
}