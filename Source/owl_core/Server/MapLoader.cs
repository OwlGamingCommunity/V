using CodeWalker.GameFiles;
using GTANetworkAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public class MapLoader : IDisposable
{
	private byte[] m_EncryptionKey = new byte[] { 0x50, 0x10, 0x30, 0x14 };

	public MapLoader()
	{
		NetworkEvents.OnPlayerConnected += OnPlayerConnected;

		NetworkEvents.OnPlayerDisconnected += OnPlayerDisconnected;

		RageEvents.RAGE_OnUpdate += OnUpdate;

		NetworkEvents.RequestMap += RequestMap;
		NetworkEvents.CustomInterior_CustomMapTransfer_Start += OnStartCustomMapTransfer;
		NetworkEvents.CustomInterior_CustomMapTransfer_Cancel += OnCancelCustomMapTransfer;
		NetworkEvents.CustomInterior_CustomMapTransfer_SendBytes += OnSendBytesCustomMapTransfer;

		Init();
	}

	private async void Init()
	{
		int index = 0;
		foreach (string strFilename in Directory.GetFiles("cooked_maps"))
		{
			if (strFilename.EndsWith(".owlmap"))
			{
				OwlMapFile mapFile = JsonConvert.DeserializeObject<OwlMapFile>(File.ReadAllText(strFilename));
				m_dictMaps.Add(index, mapFile);

				if (mapFile.MapType == EMapType.Persistent)
				{
					m_dictMaps_PersistentOnly.Add(index, mapFile);
				}

				NAPI.Util.ConsoleOutput("Loaded map {0} with {1} objects", Path.GetFileNameWithoutExtension(strFilename), mapFile.MapData.Count);

				++index;
			}
		}

		List<OwlMapDatabase> customInteriors = await Database.LegacyFunctions.GetAllCustomInteriors().ConfigureAwait(true);

		int addedMapCount = 0;
		foreach (OwlMapDatabase customInt in customInteriors)
		{
			OwlMapFile mapFile = new OwlMapFile
			{
				PropertyID = customInt.PropertyID,
				MarkerX = customInt.MarkerX,
				MarkerY = customInt.MarkerY,
				MarkerZ = customInt.MarkerZ
			};

			foreach (OwlMapObjectDatabase mapObject in customInt.MapObjects)
			{
				OwlMapObject owlMapObject = new OwlMapObject
				{
					model = mapObject.model,
					x = mapObject.x,
					y = mapObject.y,
					z = mapObject.z,
					rx = mapObject.rx,
					ry = mapObject.ry,
					rz = mapObject.rz
				};

				mapFile.MapData.Add(owlMapObject);
			}

			m_dictCustomMaps.Add((int)customInt.PropertyID, mapFile);
			addedMapCount++;
		}

		NAPI.Util.ConsoleOutput("Loaded {0} custom interior(s) from the database!", addedMapCount);

		// Send maps to all
		SendPersistentMapsToAll();
	}

	public void Dispose()
	{
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool a_CleanupNativeAndManaged)
	{
		if (m_CompletedMapBuildTasksSemaphore != null)
		{
			m_CompletedMapBuildTasksSemaphore.Dispose();
			m_CompletedMapBuildTasksSemaphore = null;
		}
	}

	private static Dictionary<int, OwlMapFile> m_dictMaps = new Dictionary<int, OwlMapFile>();
	private static Dictionary<int, OwlMapFile> m_dictCustomMaps = new Dictionary<int, OwlMapFile>();
	private static Dictionary<int, OwlMapFile> m_dictMaps_PersistentOnly = new Dictionary<int, OwlMapFile>();

	public static int GetMapID(string strMapName)
	{
		int index = 0;
		foreach (var map in m_dictMaps.Values)
		{
			if (map.MapName.ToLower() == strMapName.ToLower())
			{
				return index;
			}

			++index;
		}

		return -1;
	}

	public static int GetMapIDByInteriorID(int propertyID)
	{
		return m_dictCustomMaps.ContainsKey(propertyID) ? propertyID : -1;
	}

	public static async void FullyRemoveCustomMap(int propertyID)
	{
		if (m_dictCustomMaps.ContainsKey(propertyID))
		{
			OwlMapDatabase customMap = await Database.LegacyFunctions.GetCustomMap(propertyID).ConfigureAwait(true);

			if (customMap.MapID != 0)
			{
				// If it exists, we delete only the objects and update the last updated from the map
				await Database.LegacyFunctions.DeleteCustomMapObjects(customMap.MapID).ConfigureAwait(true);
				await Database.LegacyFunctions.DeleteCustomMap(customMap.MapID).ConfigureAwait(true);

				m_dictCustomMaps.Remove(propertyID);
			}
		}
	}

	private void RequestMap(CPlayer player, int mapID)
	{
		OwlMapFile mapToSend;
		if (m_dictMaps.ContainsKey(mapID))
		{
			var mapInst = m_dictMaps[mapID];

			// We do not send persistent maps here, fake null if so
			if (mapInst.MapType == EMapType.Persistent)
			{
				mapToSend = null;
			}
			else
			{
				mapToSend = mapInst;
			}
		}
		else if (m_dictCustomMaps.ContainsKey(mapID))
		{
			mapToSend = m_dictCustomMaps[mapID];
		}
		else
		{
			mapToSend = null;
		}

		byte[] mapData = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(mapToSend));
		LargeDataTransferManager.QueueOutgoingTransfer(player, ELargeDataTransferType.MapData_OnDemand, mapID, mapData, null, null, true, m_EncryptionKey);
	}

	private void OnPlayerConnected(CPlayer player)
	{
		SendPersistentMapsToPlayer(player);
	}

	private void SendPersistentMapsToAll()
	{
		// NOTE: We don't just use _ForAll because it only includes signed in players, we need this everywhere
		foreach (CPlayer player in PlayerPool.GetAllPlayers_IncludeOutOfGame())
		{
			SendPersistentMapsToPlayer(player);
		}
	}

	private void SendPersistentMapsToPlayer(CPlayer player)
	{
		byte[] mapData = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(m_dictMaps_PersistentOnly));
		LargeDataTransferManager.QueueOutgoingTransfer(player, ELargeDataTransferType.MapData_Persistent, -1, mapData, null, null, true, m_EncryptionKey);
	}

	public static async Task<int> DetermineGCCostForMapUpload(long propertyID)
	{
		OwlMapDatabase map = await Database.LegacyFunctions.GetCustomMap(propertyID).ConfigureAwait(true);

		DateTime updatedAt = Convert.ToDateTime(map.UpdatedAt);

		int GCCost = -1;

		if (map.MapID != 0)
		{
			if (Math.Abs(updatedAt.Subtract(DateTime.Now).TotalHours) <= 24) // within 24 hours
			{
				GCCost = CustomMapConstants.updateInside24h;
			}
			else if (Math.Abs(updatedAt.Subtract(DateTime.Now).TotalHours) > 24 && Math.Abs(updatedAt.Subtract(DateTime.Now).TotalHours) <= 168) // between 24-1 week
			{
				GCCost = CustomMapConstants.updateInsideWeek;
			}
			else if (Math.Abs(updatedAt.Subtract(DateTime.Now).TotalHours) > 168) // after 1 week
			{
				GCCost = CustomMapConstants.newInteriorGC;
			}

			return GCCost;
		}
		else
		{
			// Interior does not exist yet, so we charge a new interior instead.
			return CustomMapConstants.newInteriorGC;
		}
	}

	private async Task<bool> ChargeGCForMapUpload(CPlayer uploadingPlayer, long propertyID)
	{
		int GCCost = await DetermineGCCostForMapUpload(propertyID).ConfigureAwait(true);
		int donatorCurrency = await uploadingPlayer.GetDonatorCurrency().ConfigureAwait(true);

		if (GCCost >= 0 && donatorCurrency >= GCCost)
		{
			uploadingPlayer.SubtractDonatorCurrency(GCCost);
			return true;
		}

		return false;
	}

	private Queue<OwlMapFile> m_CompletedMapBuildTasks = new Queue<OwlMapFile>();
	private SemaphoreSlim m_CompletedMapBuildTasksSemaphore = new SemaphoreSlim(1, 1);

	private async void OnUpdate()
	{
		// Check which transfers are complete, and move them to the processing stage
		List<CPlayer> lstTransfersToRemove = new List<CPlayer>();
		foreach (var kvPair in m_dictPendingMapTransfers)
		{
			PendingMapTransfer transfer = kvPair.Value;
			if (transfer.IsComplete())
			{
				if (transfer.WasSuccessful())
				{
					kvPair.Key.SendNotification("Map Upload", ENotificationIcon.InfoSign, "Your map has been uploaded and is now being processed.");
					NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(kvPair.Key);

					transfer.GetDetails(out string transferMapType, out long transferPropertyID, out float transferMarkerX, out float transferMarkerY, out float transferMarkerZ, out byte[] transferData);

					// queue processing work on worker thread
					ThreadParams tParams = new ThreadParams
					{
						mapData = transferData,
						mapType = transferMapType,
						propertyID = transferPropertyID,
						markerX = transferMarkerX,
						markerY = transferMarkerY,
						markerZ = transferMarkerZ
					};

					bool purchasedInterior = await ChargeGCForMapUpload(kvPair.Key, transferPropertyID).ConfigureAwait(true);
					if (purchasedInterior)
					{
						ThreadPool.QueueUserWorkItem(new WaitCallback(BuildOwlMap), tParams);
						CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(transferPropertyID);
						if (Property != null)
						{
							Property.SetExit(new Vector3(transferMarkerX, transferMarkerY, transferMarkerZ), 0);
						}
					}
					else
					{
						int GCCost = await DetermineGCCostForMapUpload(transferPropertyID).ConfigureAwait(true);
						kvPair.Key.SendGenericMessageBox("Map Upload Failed", Helpers.FormatString("You need {0} GC to upload a custom map for this property.", GCCost));
						NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(kvPair.Key);
					}
				}
				else
				{
					kvPair.Key.SendGenericMessageBox("Map Upload Failed", "Please try again.");
					NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(kvPair.Key);
				}

				lstTransfersToRemove.Add(kvPair.Key);
			}
		}

		// remove transfers that are marked complete
		foreach (CPlayer player in lstTransfersToRemove)
		{
			m_dictPendingMapTransfers.Remove(player);
		}

		await m_CompletedMapBuildTasksSemaphore.WaitAsync().ConfigureAwait(true);

		// process one map per frame
		if (m_CompletedMapBuildTasks.Count > 0)
		{
			OwlMapFile mapFile = m_CompletedMapBuildTasks.Dequeue();

			Console.WriteLine("Processed 1 map this frame, {0} more map(s) remain.", m_CompletedMapBuildTasks.Count);

			// Check if the map exists in the database
			OwlMapDatabase oldMap = await Database.LegacyFunctions.GetCustomMap(mapFile.PropertyID).ConfigureAwait(true);

			if (oldMap.MapID != 0)
			{
				// If it exists, we delete only the objects and update the last updated from the map
				await Database.LegacyFunctions.DeleteCustomMapObjects(oldMap.MapID).ConfigureAwait(true);

				// Create the new objects
				foreach (OwlMapObject mapObject in mapFile.MapData)
				{
					await Database.LegacyFunctions.CreateCustomMapObject(mapObject.model, RoundCoordinates(mapObject.x), RoundCoordinates(mapObject.y), RoundCoordinates(mapObject.z), RoundCoordinates(mapObject.rx), RoundCoordinates(mapObject.ry), RoundCoordinates(mapObject.rz), oldMap.MapID).ConfigureAwait(true);
				}

				// Update the last updated and marker positions
				await Database.LegacyFunctions.UpdateCustomMapMarker(oldMap.MapID, mapFile.MarkerX, mapFile.MarkerY, mapFile.MarkerZ).ConfigureAwait(true);
				await Database.LegacyFunctions.UpdateCustomMapLastUpdated(oldMap.MapID).ConfigureAwait(true);
			}
			else
			{
				// Create the custom map in the database
				int mapID = await Database.LegacyFunctions.CreateCustomMap(mapFile.PropertyID, mapFile.MarkerX, mapFile.MarkerY, mapFile.MarkerZ).ConfigureAwait(true);

				// Create the objects related to the custom map
				foreach (OwlMapObject mapObject in mapFile.MapData)
				{
					await Database.LegacyFunctions.CreateCustomMapObject(mapObject.model, mapObject.x, mapObject.y, mapObject.z, mapObject.rx, mapObject.ry, mapObject.rz, mapID).ConfigureAwait(true);
				}
			}

			m_dictCustomMaps[(int)mapFile.PropertyID] = mapFile;
		}

		m_CompletedMapBuildTasksSemaphore.Release();
	}

	private float RoundCoordinates(float coordinate)
	{
		return (float)Math.Round(coordinate * 100f) / 100f;
	}
	private class PendingMapTransfer
	{
		public PendingMapTransfer(string a_MapType, long a_PropertyID, float a_MarkerX, float a_MarkerY, float a_MarkerZ, int a_ExpectedBytesLen, int a_crc32)
		{
			MapType = a_MapType;
			PropertyID = a_PropertyID;
			MarkerX = a_MarkerX;
			MarkerY = a_MarkerY;
			MarkerZ = a_MarkerZ;
			ExpectedBytesLen = a_ExpectedBytesLen;

			Data = new byte[ExpectedBytesLen];
			Crc32 = a_crc32;
		}

		public void OnDataReceived(byte[] DataPacket)
		{
			// safety check to avoid network attacks
			if (BytesReceived + DataPacket.Length <= ExpectedBytesLen)
			{
				// copy bytes
				Array.Copy(DataPacket, 0, Data, BytesReceived, DataPacket.Length);
				BytesReceived += DataPacket.Length;

				// did we finish with this packet?
				if (BytesReceived >= ExpectedBytesLen)
				{
					// do the checksums match?
					bool bSuccess = CRC32.ComputeHash(Data) == Crc32;
					OnFinished(bSuccess);
				}
			}
			else
			{
				// Fake our completion, with a failed flag
				OnFinished(false);
			}
		}

		private void OnFinished(bool bSuccess)
		{
			m_bComplete = true;
			m_bSuccess = bSuccess;
		}

		public bool IsComplete()
		{
			return m_bComplete;
		}

		public bool WasSuccessful()
		{
			return m_bSuccess;
		}

		public void GetDetails(out string outMapType, out long outPropertyID, out float outMarkerX, out float outMarkerY, out float outMarkerZ, out byte[] outData)
		{
			outMapType = MapType;
			outPropertyID = PropertyID;
			outMarkerX = MarkerX;
			outMarkerY = MarkerY;
			outMarkerZ = MarkerZ;

			if (IsComplete() && WasSuccessful())
			{
				outData = Data;
			}
			else
			{
				outData = null;
			}
		}

		private string MapType;
		private long PropertyID;
		private float MarkerX;
		private float MarkerY;
		private float MarkerZ;
		private int ExpectedBytesLen;

		private int BytesReceived;
		private byte[] Data;
		private int Crc32;

		private bool m_bComplete = false;
		private bool m_bSuccess = false;
	}
	private Dictionary<CPlayer, PendingMapTransfer> m_dictPendingMapTransfers = new Dictionary<CPlayer, PendingMapTransfer>();

	private void OnPlayerDisconnected(CPlayer player, DisconnectionType type, string reason)
	{
		if (m_dictPendingMapTransfers.ContainsKey(player))
		{
			m_dictPendingMapTransfers.Remove(player);
		}
	}

	private void OnStartCustomMapTransfer(CPlayer player, string mapType, long propertyID, float markerX, float markerY, float markerZ, int expectedBytesLen, int crc32)
	{
		// Don't let them send too much data
		// We don't have to check this per-data chunk transfered because as data comes in, we check totalDataReceived < expectedBytesLen
		if (expectedBytesLen > MapTransferConstants.MaxMapSizeBytes)
		{
			player.SendGenericMessageBox("Map Upload", Helpers.FormatString("That map is too big, the maximum map size is {0} KB", (int)(MapTransferConstants.MaxMapSizeBytes / 1024)));
			NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(player);

		}
		else
		{
			bool hasPendingTransfer = false;
			foreach (PendingMapTransfer transfer in m_dictPendingMapTransfers.Values)
			{
				transfer.GetDetails(out string transferMapType, out long transferPropertyID, out float transferMarkerX, out float transferMarkerY, out float transferMarkerZ, out byte[] transferData);

				if (transferPropertyID == propertyID)
				{
					hasPendingTransfer = true;
				}
			}

			if (!hasPendingTransfer)
			{
				m_dictPendingMapTransfers.Add(player, new PendingMapTransfer(mapType, propertyID, markerX, markerY, markerZ, expectedBytesLen, crc32));
			}
			else
			{
				player.SendGenericMessageBox("Map Upload", "Someone has made a pending upload request for this interior already, please wait.");
				NetworkEventSender.SendNetworkEvent_CustomInterior_CustomMapTransfer_Reset(player);
			}
		}
	}

	private void OnCancelCustomMapTransfer(CPlayer player)
	{
		if (m_dictPendingMapTransfers.ContainsKey(player))
		{
			m_dictPendingMapTransfers.Remove(player);
		}
	}

	private void OnSendBytesCustomMapTransfer(CPlayer player, byte[] dataBytes)
	{
		if (m_dictPendingMapTransfers.ContainsKey(player))
		{
			m_dictPendingMapTransfers[player].OnDataReceived(dataBytes);
		}
	}

	private void BuildOwlMap(object tParams)
	{
		ThreadParams tParamsValues = tParams as ThreadParams;
		byte[] data = tParamsValues.mapData;
		string mapType = tParamsValues.mapType;
		long propertyID = tParamsValues.propertyID;
		float markerX = tParamsValues.markerX;
		float markerY = tParamsValues.markerY;
		float markerZ = tParamsValues.markerZ;

		try
		{
			if (mapType == ("ymap"))
			{
				OwlMapFile outMapFile = new OwlMapFile
				{
					MapSourceType = EMapSourceType.YMAP_RAW,
					MapType = EMapType.Interior,
					PropertyID = propertyID,
					MarkerX = markerX,
					MarkerY = markerY,
					MarkerZ = markerZ
				};

				YmapFile ymapFileInst = new YmapFile();
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

				MarkWorkerThreadTaskAsCompleted(outMapFile);
			}
			else if (mapType == "xml")
			{
				OwlMapFile outMapFile = new OwlMapFile
				{
					MapSourceType = EMapSourceType.MENYOO,
					MapType = EMapType.Interior,
					PropertyID = propertyID,
					MarkerX = markerX,
					MarkerY = markerY,
					MarkerZ = markerZ
				};

				string result = System.Text.Encoding.UTF8.GetString(data);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(result);

				string json = JsonConvert.SerializeXmlNode(doc);
				JObject jsonXml = JObject.Parse(json);

				if (jsonXml.ContainsKey("Map"))
				{
					JObject Map = jsonXml.GetValue("Map").ToObject<JObject>();
					if (Map.ContainsKey("Objects"))
					{
						JObject mapEntities = Map.GetValue("Objects").ToObject<JObject>();
						var resultObjects = AllChildren(JObject.Parse(json)).First(c => c.Type == JTokenType.Array && c.Path.Contains("MapObject")).Children<JObject>();

						foreach (JObject objectResult in resultObjects)
						{
							if (objectResult.Value<string>("Type").Equals("Prop"))
							{
								string objectModel = objectResult.Value<string>("Hash");

								float posX = objectResult.Value<JObject>("Position").Value<float>("X");
								float posY = objectResult.Value<JObject>("Position").Value<float>("Y");
								float posZ = objectResult.Value<JObject>("Position").Value<float>("Z");

								float rotX = objectResult.Value<JObject>("Rotation").Value<float>("X");
								float rotY = objectResult.Value<JObject>("Rotation").Value<float>("Y");
								float rotZ = objectResult.Value<JObject>("Rotation").Value<float>("Z");

								outMapFile.MapData.Add(new OwlMapObject { model = objectModel, x = posX, y = posY, z = posZ, rx = rotX, ry = rotY, rz = rotZ, rw = 0.0f });
							}
						}
					}
				}

				MarkWorkerThreadTaskAsCompleted(outMapFile);
			}
		}
		catch (Exception ex)
		{
			SentryEvent logEvent = new SentryEvent(ex);
			SentrySdk.CaptureEvent(logEvent);
		}
	}

	private async void MarkWorkerThreadTaskAsCompleted(OwlMapFile outputMapFile)
	{
		await m_CompletedMapBuildTasksSemaphore.WaitAsync().ConfigureAwait(true);
		m_CompletedMapBuildTasks.Enqueue(outputMapFile);
		m_CompletedMapBuildTasksSemaphore.Release();
	}

	private static IEnumerable<JToken> AllChildren(JToken json)
	{
		foreach (var c in json.Children())
		{
			yield return c;
			foreach (var cc in AllChildren(c))
			{
				yield return cc;
			}
		}
	}

	// NOTE: If you change this, update cooker too
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
}

internal class ThreadParams
{
	public byte[] mapData { get; set; }
	public string mapType { get; set; }
	public long propertyID { get; set; }
	public float markerX { get; set; }
	public float markerY { get; set; }
	public float markerZ { get; set; }
}