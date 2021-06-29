using RAGE;
using System;
using System.Collections.Generic;


public class MapLoader
{
	public delegate void MapLoadedDelegate(bool bSuccess, bool bCustomMap = false, float markerX = 0.0f, float markerY = 0.0f, float markerZ = 0.0f);
	public delegate void MapUnloadedDelegate(bool bSuccess);

	private static Dictionary<int, OwlMapFile> m_dictMaps = new Dictionary<int, OwlMapFile>();
	private static Dictionary<int, OwlMapFile> m_dictCustomMaps = new Dictionary<int, OwlMapFile>();
	private static Dictionary<int, List<RAGE.Elements.MapObject>> m_dictStreamedInMapData = new Dictionary<int, List<RAGE.Elements.MapObject>>();
	private static Dictionary<int, List<RAGE.Elements.MapObject>> m_dictStreamedInCustomMapData = new Dictionary<int, List<RAGE.Elements.MapObject>>();

	private static bool m_bPendingLoadDone = false;
	private static int m_PendingLoadMapID = -1;
	private static MapLoadedDelegate m_bPendingLoadCallback = null;

	public MapLoader()
	{
		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.MapData_Persistent, OnStartReceivingMap_WorldMaps, OnMapProgress_WorldMaps, OnReceivedMap_WorldMaps);
		LargeDataTransferManager.RegisterIncomingTransferCallbacks(ELargeDataTransferType.MapData_OnDemand, OnStartReceivingMap_OnDemand, OnMapProgress_OnDemand, OnReceivedMap_OnDemand);

		RageEvents.RAGE_OnTick_OncePerSecond += OnTick_OncePerSecond;

		NetworkEvents.ChangeCharacterApproved += () => { ForceMapsToDimension(WorldHelper.GetPlayerSpecificDimension()); };
		NetworkEvents.CharacterSelectionApproved += () => { ForceMapsToDimension(0); };
	}

	// World maps
	private static void OnStartReceivingMap_WorldMaps(LargeDataTransfer transfer)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		GenericProgressBar.ShowGenericProgressBar("Downloading World Data", "Please hang tight...", 0, false, "", UIEventID.Dummy);
	}

	private static void OnMapProgress_WorldMaps(LargeDataTransfer transfer)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		int bytesRemaining = transfer.GetDataLengthEncrypted() - transfer.GetDataOffset();
		int secondsRemaining = (int)Math.Ceiling((float)bytesRemaining / (float)(LargeTransferConstants.MaxBytesPer100ms * 10));
		int percent = (int)(Math.Ceiling(((float)transfer.GetDataOffset() / (float)transfer.GetDataLengthEncrypted() * 100.0f)));

		string strEstimatedTimeRemaining = Helpers.FormatString("{0} remaining (estimated)", Helpers.ConvertSecondsToTimeString(secondsRemaining));
		GenericProgressBar.UpdateCaption(Helpers.FormatString("Please hang tight... <br>{0}", strEstimatedTimeRemaining));

		GenericProgressBar.UpdateProgress(percent);
	}

	private static void OnReceivedMap_WorldMaps(LargeDataTransfer transfer, bool bTransferSuccess)
	{
		GenericProgressBar.CloseAnyProgressBar();
		RAGE.Chat.Show(DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED));

		m_dictMaps.Clear();
		if (bTransferSuccess)
		{
			int mapID = transfer.m_Identifier;
			try
			{
				Dictionary<int, OwlMapFile> dictMapData = OwlJSON.DeserializeObject<Dictionary<int, OwlMapFile>>(System.Text.Encoding.ASCII.GetString(transfer.GetBytes()), EJsonTrackableIdentifier.WorldMaps);

				foreach (var kvPair in dictMapData)
				{
					m_dictMaps.Add(kvPair.Key, kvPair.Value);

					if (kvPair.Value.MapType == EMapType.Persistent)
					{
						LoadMap(kvPair.Key, (bool bSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) => { });
					}
				}
			}
			catch (Exception e)
			{
				ExceptionHelper.SendException(e);
			}
		}
		RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
	}

	// On demand / interior maps
	private static void OnStartReceivingMap_OnDemand(LargeDataTransfer transfer)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		GenericProgressBar.ShowGenericProgressBar("Loading Custom Interior", "Please hang tight...", 0, false, "", UIEventID.Dummy);
	}

	private static void OnMapProgress_OnDemand(LargeDataTransfer transfer)
	{
		int bytesRemaining = transfer.GetDataLengthEncrypted() - transfer.GetDataOffset();
		int secondsRemaining = (int)Math.Ceiling((float)bytesRemaining / (float)(LargeTransferConstants.MaxBytesPer100ms * 10));
		int percent = (int)(Math.Ceiling(((float)transfer.GetDataOffset() / (float)transfer.GetDataLengthEncrypted() * 100.0f)));

		string strEstimatedTimeRemaining = Helpers.FormatString("{0} remaining (estimated)", Helpers.ConvertSecondsToTimeString(secondsRemaining));
		GenericProgressBar.UpdateCaption(Helpers.FormatString("Please hang tight... <br>{0}<br>You will not see this message again for this interior.", strEstimatedTimeRemaining));

		GenericProgressBar.UpdateProgress(percent);
	}

	private static void OnReceivedMap_OnDemand(LargeDataTransfer transfer, bool bSuccess)
	{
		GenericProgressBar.CloseAnyProgressBar();
		RAGE.Chat.Show(DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED));

		m_bPendingLoadDone = true;
		if (bSuccess)
		{
			int mapID = transfer.m_Identifier;
			try
			{
				OwlMapFile mapFile = OwlJSON.DeserializeObject<OwlMapFile>(System.Text.Encoding.ASCII.GetString(transfer.GetBytes()), EJsonTrackableIdentifier.OnDemandMap);

				if (mapFile != null)
				{
					if (mapFile.PropertyID != 0)
					{
						if (m_dictCustomMaps.ContainsKey(mapID))
						{
							m_dictCustomMaps.Remove(mapID);
						}

						m_dictCustomMaps.Add(mapID, mapFile);
					}
					else
					{
						if (m_dictMaps.ContainsKey(mapID))
						{
							m_dictMaps.Remove(mapID);
						}

						m_dictMaps.Add(mapID, mapFile);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionHelper.SendException(e);
			}
		}
		else
		{
			// TODO_TRANSFER: Kick person out of int?
		}

		RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
	}

	// TODO_POST_LAUNCH: This breaks if we have persistent world maps not in dimension 0
	private static void ForceMapsToDimension(uint dimension)
	{
		foreach (var mapData in m_dictMaps)
		{
			// if its streamed in, its either persistent OR its the interior we're in, so should be fine to set dim, leaving an int unloads it
			if (m_dictStreamedInMapData.ContainsKey(mapData.Key))
			{
				var streamedInMapObjects = m_dictStreamedInMapData[mapData.Key];
				foreach (var obj in streamedInMapObjects)
				{
					obj.Dimension = dimension;
				}
			}
		}
	}

	private static bool IsDoorEntrance(RAGE.Elements.MapObject obj, Vector3 vecEntrance)
	{
		float fDistFromObjectToEntrance = (obj.Position - vecEntrance).Length();
		return fDistFromObjectToEntrance < 2.0f;
	}

	public static void FixDoorStates(bool bIsCustom, int mapIndex, float entranceX, float entranceY, float entranceZ)
	{
		Vector3 vecEntrance = new Vector3(entranceX, entranceY, entranceZ);
		if (bIsCustom)
		{
			if (m_dictStreamedInCustomMapData.ContainsKey(mapIndex))
			{
				foreach (var obj in m_dictStreamedInCustomMapData[mapIndex])
				{
					Vector3 vecRot = obj.GetRotation(0);
					bool bIsDoorRotatedCorrectly = (vecRot.X >= 0.0f && vecRot.X <= 10.0f && vecRot.Y >= 0.0f && vecRot.Y <= 10.0f);
					bool bLocked = IsDoorEntrance(obj, vecEntrance);

					if (bIsDoorRotatedCorrectly)
					{
						RAGE.Game.Object.SetStateOfClosestDoorOfType(obj.Model, obj.Position.X, obj.Position.Y, obj.Position.Z, bLocked, 0.0f, false);
					}
				}
			}
		}
		else
		{
			if (m_dictStreamedInMapData.ContainsKey(mapIndex))
			{
				foreach (var obj in m_dictStreamedInMapData[mapIndex])
				{
					Vector3 vecRot = obj.GetRotation(0);
					bool bIsDoorRotatedCorrectly = (vecRot.X >= 0.0f && vecRot.X <= 10.0f && vecRot.Y >= 0.0f && vecRot.Y <= 10.0f);
					bool bLocked = IsDoorEntrance(obj, vecEntrance);

					if (bIsDoorRotatedCorrectly)
					{
						RAGE.Game.Object.SetStateOfClosestDoorOfType(obj.Model, obj.Position.X, obj.Position.Y, obj.Position.Z, bLocked, 0.0f, false);
					}
				}
			}
		}
	}

	public static void LoadMap(int mapIndex, MapLoadedDelegate callback)
	{
		if (m_dictMaps.ContainsKey(mapIndex))
		{
			// Already streamed in?
			if (m_dictStreamedInMapData.ContainsKey(mapIndex))
			{
				callback(true);
				FixDoorStates(false, mapIndex, m_dictMaps[mapIndex].MarkerX, m_dictMaps[mapIndex].MarkerY, m_dictMaps[mapIndex].MarkerZ);
				return;
			}

			var val = m_dictMaps[mapIndex];

			m_dictStreamedInMapData.Add(mapIndex, new List<RAGE.Elements.MapObject>());

			int expectedObjectCount = val.MapData.Count;
			foreach (OwlMapObject obj in val.MapData)
			{
				// TODO_POST_LAUNCH: Dimension support?
				UInt32 model = 0;

				if (obj.model.StartsWith("0x")) // Custom models (aka mods) are stored as hex hashes, not English strings, no need to hash them
				{
					model = Convert.ToUInt32(obj.model, 16);
				}
				else // vanilla GTA models are stored as model name and must be hashed
				{
					model = HashHelper.GetHashUnsigned(obj.model);
				}

				if (model != 0)
				{
					AsyncModelLoader.RequestAsyncLoad(model, (uint modelLoaded) =>
					{
						RAGE.Vector3 vecRot = new RAGE.Vector3(obj.rx, obj.ry, obj.rz);

						RAGE.Elements.MapObject newObj = new RAGE.Elements.MapObject(model, new RAGE.Vector3(obj.x, obj.y, obj.z), vecRot, 255, RAGE.Elements.Player.LocalPlayer.Dimension);
						m_dictStreamedInMapData[mapIndex].Add(newObj);

						// Are we done loading?
						if (m_dictStreamedInMapData[mapIndex].Count == expectedObjectCount)
						{
							callback(true);
							FixDoorStates(false, mapIndex, m_dictMaps[mapIndex].MarkerX, m_dictMaps[mapIndex].MarkerY, m_dictMaps[mapIndex].MarkerZ);
						}
					}, true);
				}
			}
		}
		else if (m_dictCustomMaps.ContainsKey(mapIndex))
		{
			// Already streamed in?
			if (m_dictStreamedInCustomMapData.ContainsKey(mapIndex))
			{
				if (m_dictCustomMaps[mapIndex].PropertyID != 0)
				{
					callback(true, true, m_dictCustomMaps[mapIndex].MarkerX, m_dictCustomMaps[mapIndex].MarkerY, m_dictCustomMaps[mapIndex].MarkerZ);
					FixDoorStates(true, mapIndex, m_dictCustomMaps[mapIndex].MarkerX, m_dictCustomMaps[mapIndex].MarkerY, m_dictCustomMaps[mapIndex].MarkerZ);
					return;
				}
				else
				{
					callback(true);
					FixDoorStates(true, mapIndex, m_dictCustomMaps[mapIndex].MarkerX, m_dictCustomMaps[mapIndex].MarkerY, m_dictCustomMaps[mapIndex].MarkerZ);
					return;
				}
			}

			var val = m_dictCustomMaps[mapIndex];

			m_dictStreamedInCustomMapData.Add(mapIndex, new List<RAGE.Elements.MapObject>());

			int expectedObjectCount = val.MapData.Count;
			uint dimension = (uint)(val.PropertyID != 0 ? val.PropertyID : RAGE.Elements.Player.LocalPlayer.Dimension);

			foreach (OwlMapObject obj in val.MapData)
			{
				UInt32 model = 0;

				if (obj.model.StartsWith("0x")) // Custom models (aka mods) are stored as hex hashes, not English strings, no need to hash them
				{
					model = Convert.ToUInt32(obj.model, 16);
				}
				else // vanilla GTA models are stored as model name and must be hashed
				{
					model = HashHelper.GetHashUnsigned(obj.model);
				}

				if (model != 0)
				{
					AsyncModelLoader.RequestAsyncLoad(model, (uint modelLoaded) =>
					{
						RAGE.Vector3 vecRot = new RAGE.Vector3(obj.rx, obj.ry, obj.rz);

						RAGE.Elements.MapObject newObj = new RAGE.Elements.MapObject(model, new RAGE.Vector3(obj.x, obj.y, obj.z), vecRot, 255, dimension);
						m_dictStreamedInCustomMapData[mapIndex].Add(newObj);

						// Are we done loading?
						if (m_dictStreamedInCustomMapData[mapIndex].Count == expectedObjectCount)
						{
							callback(true, val.PropertyID != 0, val.MarkerX, val.MarkerY, val.MarkerZ);
							FixDoorStates(true, mapIndex, val.MarkerX, val.MarkerY, val.MarkerZ);
						}
					}, true);
				}
			}
		}
		else
		{
			m_bPendingLoadDone = false;
			m_PendingLoadMapID = mapIndex;
			m_bPendingLoadCallback = callback;

			NetworkEventSender.SendNetworkEvent_RequestMap(mapIndex);
		}
	}

	private void ResetPendingLoadFlags()
	{
		m_PendingLoadMapID = -1;
		m_bPendingLoadDone = false;
		m_bPendingLoadCallback = null;
	}

	private void OnTick_OncePerSecond()
	{
		ForceMapsToDimension(RAGE.Elements.Player.LocalPlayer.Dimension);

		// Do we have a pending load?
		if (m_PendingLoadMapID != -1)
		{
			// Is it done?
			if (m_bPendingLoadDone)
			{
				// Was it successful?
				bool bSuccess = false;

				if (m_dictMaps.ContainsKey(m_PendingLoadMapID) && m_dictMaps[m_PendingLoadMapID].MapData.Count > 0)
				{
					bSuccess = true;
				}
				else if (m_dictCustomMaps.ContainsKey(m_PendingLoadMapID) && m_dictCustomMaps[m_PendingLoadMapID].MapData.Count > 0)
				{
					bSuccess = true;
				}

				// Load map with original callback
				if (bSuccess)
				{
					LoadMap(m_PendingLoadMapID, m_bPendingLoadCallback);
				}
				else
				{
					m_bPendingLoadCallback(false);
				}


				ResetPendingLoadFlags();
			}
		}
	}

	public static void UnloadMap(int mapIndex, MapUnloadedDelegate callback)
	{
		bool bSuccess = false;
		if (m_dictMaps.ContainsKey(mapIndex))
		{
			// not streamed in?
			if (!m_dictStreamedInMapData.ContainsKey(mapIndex))
			{
				return;
			}

			foreach (var obj in m_dictStreamedInMapData[mapIndex])
			{
				obj.Destroy();
			}

			m_dictStreamedInMapData.Remove(mapIndex);
			bSuccess = true;
		}
		else if (m_dictCustomMaps.ContainsKey(mapIndex))
		{
			// not streamed in?
			if (!m_dictStreamedInCustomMapData.ContainsKey(mapIndex))
			{
				return;
			}

			foreach (var obj in m_dictStreamedInCustomMapData[mapIndex])
			{
				obj.Destroy();
			}

			m_dictStreamedInCustomMapData.Remove(mapIndex);
			bSuccess = true;
		}

		callback(bSuccess);
	}

}