using CodeWalker.GameFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml;

public class Job_CookMap : JobBase
{
	private CookerTypes.AssetFile m_AssetFile;

	public Job_CookMap(CookerTypes.AssetFile assetFile) : base()
	{
		m_AssetFile = assetFile;
	}

	public override bool Execute()
	{
		bool bWroteMap = false;

		if (m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Interior || m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Persistent)
		{
			OwlMapFile outMapFile = null;
			string rawMapFile = Path.GetFileNameWithoutExtension(m_AssetFile.Name);
			if (rawMapFile.EndsWith(".ymap")) // YMAP RAW
			{
				BufferedWriteLine("\t\tCooking Map: {0}", Path.GetFileName(rawMapFile));
				bWroteMap = true;

				outMapFile = new OwlMapFile();
				outMapFile.MapSourceType = EMapSourceType.YMAP_RAW;
				outMapFile.MapName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(rawMapFile)); // TODO: Path.GetFileNameWithoutExtension is double called because we have .maptype.owlmap, we should fix that perhaps?

				if (m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Interior)
				{
					outMapFile.MapType = EMapType.Interior;
				}
				else if (m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Persistent)
				{
					outMapFile.MapType = EMapType.Persistent;
				}

				YmapFile ymapFileInst = new YmapFile();
				byte[] data = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "..", "RawMaps", rawMapFile));
				ymapFileInst.Load(data);
				foreach (var entity in ymapFileInst.RootEntities)
				{
					string strHash = string.Format("0x{0}", entity.CEntityDef.archetypeName.Hex);

					float posX = entity.Position.X;
					float posY = entity.Position.Y;
					float posZ = entity.Position.Z;

					System.Numerics.Vector3 vecRot = QuaternionToEulerAngle(entity.Orientation);

					outMapFile.MapData.Add(new OwlMapObject { model = strHash, x = posX, y = posY, z = posZ, rx = vecRot.X, ry = vecRot.Y, rz = vecRot.Z, rw = 0.0f });
				}
			}
			else if (rawMapFile.EndsWith(".xml"))
			{
				BufferedWriteLine("\t\tCooking Map: {0}", Path.GetFileName(rawMapFile));
				bWroteMap = true;

				outMapFile = new OwlMapFile();
				outMapFile.MapSourceType = EMapSourceType.MENYOO;
				outMapFile.MapName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(rawMapFile)); // TODO: Path.GetFileNameWithoutExtension is double called because we have .maptype.owlmap, we should fix that perhaps?

				if (m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Interior)
				{
					outMapFile.MapType = EMapType.Interior;
				}
				else if (m_AssetFile.Type == CookerTypes.EAssetType.MapFile_Persistent)
				{
					outMapFile.MapType = EMapType.Persistent;
				}

				string xmlData = File.ReadAllText(rawMapFile);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlData);

				string json = JsonConvert.SerializeXmlNode(doc);

				dynamic obj = JObject.Parse(json);

				for (int i = 0; i < obj.Map.Objects.MapObject.Count; ++i)
				{
					dynamic thisEntity = obj.Map.Objects.MapObject[i];
					string type = thisEntity.Type;

					if (type == "Prop")
					{
						string objHash = thisEntity.Hash;

						float posX = thisEntity.Position.X;
						float posY = thisEntity.Position.Y;
						float posZ = thisEntity.Position.Z;

						float rotX = thisEntity.Rotation.X;
						float rotY = thisEntity.Rotation.Y;
						float rotZ = thisEntity.Rotation.Z;

						outMapFile.MapData.Add(new OwlMapObject { model = objHash, x = posX, y = posY, z = posZ, rx = rotX, ry = rotY, rz = rotZ, rw = 0.0f });
					}
				}
			}

			if (outMapFile != null)
			{
				// Serialize the file
				string outJson = JsonConvert.SerializeObject(outMapFile, Newtonsoft.Json.Formatting.Indented);
				string strOutfilename = String.Concat(rawMapFile, ".owlmap");
				File.WriteAllText(Path.Combine(GetMapDirectory(), strOutfilename), outJson);
			}
		}

		return bWroteMap;
	}

	public override string Describe()
	{
		return String.Format("[{0}] - {1} - {2}", this.GetType().Name, m_AssetFile.Name, m_AssetFile.Type);
	}

	// NOTE: If you change this, update MapLoader.cs too
	private static System.Numerics.Vector3 QuaternionToEulerAngle(SharpDX.Quaternion q)
	{
		System.Numerics.Vector3 retVal = new System.Numerics.Vector3();

		// roll (x-axis rotation)
		double sinr_cosp = +2.0 * (q.W * q.X + q.Y * q.Z);
		double cosr_cosp = +1.0 - 2.0 * (q.X * q.X + q.Y * q.Y);
		retVal.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

		// pitch (y-axis rotation)
		double sinp = +2.0 * (q.W * q.Y - q.Z * q.X);
		double absSinP = Math.Abs(sinp);
		bool bSinPOutOfRage = absSinP >= 1.0;
		if (bSinPOutOfRage)
		{
			retVal.Y = 90.0f; // use 90 degrees if out of range
		}
		else
		{
			retVal.Y = (float)Math.Asin(sinp);
		}

		// yaw (z-axis rotation)
		double siny_cosp = +2.0 * (q.W * q.Z + q.X * q.Y);
		double cosy_cosp = +1.0 - 2.0 * (q.Y * q.Y + q.Z * q.Z);
		retVal.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

		// Rad to Deg
		retVal.X *= (float)(180.0f / Math.PI);

		if (!bSinPOutOfRage) // only mult if within range
		{
			retVal.Y *= (float)(180.0f / Math.PI);
		}
		retVal.Z *= (float)(180.0f / Math.PI);

		return retVal;
	}

	// HELPERS
	public static string GetMapDirectory()
	{
		// NOTE: Hardcoded to release because we cook to release and junction release -> debug
		return Path.Combine(Directory.GetCurrentDirectory(), "..", "Output", "Release", "cooked_maps");
	}

	public static void CreateMapDirectory(bool bDeleteExistingContents)
	{
		string strMapDir = GetMapDirectory();
		if (!Directory.Exists(strMapDir))
		{
			Directory.CreateDirectory(strMapDir);
		}

		if (bDeleteExistingContents)
		{
			DirectoryInfo di = new DirectoryInfo(strMapDir);
			foreach (FileInfo file in di.GetFiles())
			{
				file.Delete();
			}
		}
	}
}
