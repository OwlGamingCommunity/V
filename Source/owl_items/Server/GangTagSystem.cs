using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class GangTagSystem 
{
	// This is a source account ID to recently cleaned tags character ID's mapping
	private Dictionary<EntityDatabaseID, List<EntityDatabaseID>> g_dictTagCleaningCooldowns = new Dictionary<EntityDatabaseID, List<EntityDatabaseID>>();

	public GangTagSystem()
	{
		NetworkEvents.GangTags_SaveWIP += OnSaveWIP;
		NetworkEvents.GangTags_SaveActive += OnSaveActive;

		NetworkEvents.RequestStartTagging += OnRequestStartTagging;
		NetworkEvents.UpdateTagging += OnUpdateTagging;

		NetworkEvents.RequestTagCleaning += OnRequestStartCleaning;
		NetworkEvents.UpdateTagCleaning += OnUpdateTagCleaning;

		NetworkEvents.RequestGotoTagMode += OnRequestGotoTagMode;
		NetworkEvents.RequestEditTagMode += OnRequestEditTagMode;

		NetworkEvents.GangTags_ShareTag += OnShareTag;
		NetworkEvents.GangTags_AcceptShare += OnAcceptShare;

		NetworkEvents.AdminClearGangTags += OnAdminRequestClearTag;

		NetworkEvents.OnPlayerConnected += (CPlayer player) =>
		{
			foreach (CGangTag tag in m_lstTags)
			{
				tag.SendToPlayer(player);
			}
		};

		NetworkEvents.CancelTaggingInProgress += OnCancelInProgressTagging;

		// load all sprite data
		try
		{
			PrintLogger.LogMessage(ELogSeverity.HIGH, "GangTagSystem: Deserializing Items");

			CGangTagSpriteDefinition[] jsonData = JsonConvert.DeserializeObject<CGangTagSpriteDefinition[]>(System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata", "GangtagSprites.json")));

			foreach (CGangTagSpriteDefinition gangtagDef in jsonData)
			{
				GangTagSpriteDefinitions.g_GangTagSpriteDefinitions.Add(gangtagDef.ID, gangtagDef);
			}

			PrintLogger.LogMessage(ELogSeverity.HIGH, "Deserialized {0} gangtag sprite definitions.", GangTagSpriteDefinitions.g_GangTagSpriteDefinitions.Count);
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading gangtag sprite data: {0}", ex.ToString());
		}

		// COMMANDS
		CommandManager.RegisterCommand("clearnearbytags", "Clears the tags nearby", new Action<CPlayer, CVehicle>(ClearNearbyTags), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("nearbytags", "Lists the nearby tags", new Action<CPlayer, CVehicle>(NearbyTags), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
		CommandManager.RegisterCommand("deltag", "Deletes a tag by its ID", new Action<CPlayer, CVehicle, EntityDatabaseID>(DelTagCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);

		LoadTags();
	}

	private void OnCancelInProgressTagging(CPlayer player)
	{
		player.Freeze(false);
	}

	public void OnAcceptShare(CPlayer player, List<GangTagLayer> lstLayers)
	{
		player.GangTag = lstLayers;
	}

	public void OnShareTag(CPlayer player, string strTargetName)
	{
		WeakReference<CPlayer> remotePlayerRef = PlayerPool.GetPlayerFromName(strTargetName);
		CPlayer remotePlayer = remotePlayerRef.Instance();

		if (remotePlayer != null)
		{
			if (remotePlayer != player)
			{
				if (player.GangTag == null || player.GangTag.Count == 0)
				{
					player.SendNotification("Gang Tags", ENotificationIcon.ExclamationSign, "You do not have a gang tag yet. Create one or have a friend share one with you!");
				}
				else
				{
					var layers = player.GangTag;
					NetworkEventSender.SendNetworkEvent_GangTags_RequestShareTag(remotePlayer, player.GetCharacterName(ENameType.StaticCharacterName), player.GangTag);

					player.SendNotification("Gang Tags", ENotificationIcon.ExclamationSign, "You have shared your tag with '{0}'", remotePlayer.GetCharacterName(ENameType.StaticCharacterName));
				}
			}
			else
			{
				player.SendNotification("Gang Tags", ENotificationIcon.ExclamationSign, "You cannot share a gang tag with yourself");
			}
		}
		else
		{
			player.SendNotification("Gang Tags", ENotificationIcon.ExclamationSign, "No player was found with that name");
		}
	}

	public async void LoadTags()
	{
		List<CDatabaseStructureGangTag> lstGangTags = await Database.LegacyFunctions.LoadAllGangTags().ConfigureAwait(true);
		NAPI.Task.Run(() =>
		{
			foreach (CDatabaseStructureGangTag dbGangTag in lstGangTags)
			{
				CGangTag tag = new CGangTag(dbGangTag.dbID, dbGangTag.ownerCharacterID, dbGangTag.vecPos, dbGangTag.fRotZ, dbGangTag.dimension, dbGangTag.tagData, dbGangTag.progress);
				m_lstTags.Add(tag);
			}

		});

		NAPI.Util.ConsoleOutput("[GANG TAGS] Loaded {0} Gang Tags!", lstGangTags.Count);
	}

	private void OnRequestEditTagMode(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_GangTags_GotoCreator(player, player.GangTag, player.GangTagWIP);
	}

	private void OnRequestGotoTagMode(CPlayer player)
	{
		if (player.GangTag == null || player.GangTag.Count == 0)
		{
			player.SendNotification("Gang Tags", ENotificationIcon.ExclamationSign, "You do not have a gang tag yet. Create one or have a friend share one with you!");
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_GangTags_GotoTagMode(player, player.GangTag);
		}
	}

	private void OnSaveWIP(CPlayer a_Player, List<GangTagLayer> lstLayers)
	{
		a_Player.GangTagWIP = lstLayers;
	}

	private void OnSaveActive(CPlayer a_Player, List<GangTagLayer> lstLayers)
	{
		a_Player.GangTagWIP.Clear();
		a_Player.GangTag = lstLayers;
	}

	private List<CGangTag> m_lstTags = new List<CGangTag>();

	private async void OnRequestStartTagging(CPlayer a_Player, float x, float y, float z, float fRotZ)
	{
		// Subtract one value from the item
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromItemID(EItemID.SPRAY_CAN);
		if (a_Player.Inventory.HasItem(ItemInstanceDef, false, out CItemInstanceDef sprayCanItem))
		{
			CItemValueBasic itemValue = (CItemValueBasic)sprayCanItem.Value;
			itemValue.value -= 1;

			if (itemValue.value > 0)
			{
				Database.Functions.Items.SaveItemValueAndStackSize(sprayCanItem);
			}
			else
			{
				a_Player.Inventory.RemoveItem(sprayCanItem);
			}

			EntityDatabaseID id = await Database.LegacyFunctions.CreateGangTag(a_Player.ActiveCharacterDatabaseID, x, y, z, fRotZ, a_Player.Client.Dimension, a_Player.GangTag).ConfigureAwait(true);

			CGangTag tag = new CGangTag(id, a_Player.ActiveCharacterDatabaseID, new Vector3(x, y, z), fRotZ, a_Player.Client.Dimension, null, 0.0f);
			tag.SetInProgress(a_Player);

			m_lstTags.Add(tag);

			a_Player.Freeze(true);
			NetworkEventSender.SendNetworkEvent_StartTaggingResponse(a_Player, true, id);
			Logging.Log.CreateLog(a_Player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("{0} tagged the wall at X: {1}, Y: {2}, Z: {3}, Dimension: {4}", a_Player.GetCharacterName(ENameType.StaticCharacterName), x, y, z, a_Player.Client.Dimension));
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_StartTaggingResponse(a_Player, false, -1);
		}
	}

	private void ClearNearbyTags(CPlayer player, CVehicle vehicle)
	{
		player.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Clearing nearby gang tags.");
		NetworkEventSender.SendNetworkEvent_ClearNearbyTags(player);
	}

	private void NearbyTags(CPlayer player, CVehicle vehicle)
	{
		player.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "Nearby tags:");
		NetworkEventSender.SendNetworkEvent_ListNearbyTags(player);
	}

	private void DelTagCommand(CPlayer player, CVehicle vehicle, EntityDatabaseID dbid)
	{
		CGangTag tag = GetTagFromID(dbid);
		if (tag == null)
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "No such tag found with that ID.");
			return;
		}

		tag.DestroyTag();
		player.PushChatMessageWithColor(EChatChannel.AdminActions, 100, 255, 100, "You have deleted a tag with ID: {0}", dbid);
	}


	private CGangTag GetTagFromID(EntityDatabaseID a_ID)
	{
		foreach (CGangTag tag in m_lstTags)
		{
			if (tag.m_DatabaseID == a_ID)
			{
				return tag;
			}
		}

		return null;
	}

	private void OnUpdateTagging(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		CGangTag gangtag = GetTagFromID(a_ID);
		if (gangtag != null)
		{
			gangtag.UpdateProgressLayer(a_Player);
		}
	}

	private void OnUpdateTagCleaning(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		CGangTag gangtag = GetTagFromID(a_ID);
		if (gangtag != null)
		{
			bool bDoneCleaning = gangtag.UpdateCleaningProgressLayer(a_Player);

			if (bDoneCleaning)
			{
				m_lstTags.Remove(gangtag);
				Vector3 tagPos = a_Player.Client.Position;
				Logging.Log.CreateLog(a_Player, Logging.ELogType.ItemMovement, null, Helpers.FormatString("{0} removed a tag from the wall at X: {1}, Y: {2}, Z: {3}, Dimension: {4}", a_Player.GetCharacterName(ENameType.StaticCharacterName), tagPos.X, tagPos.Y, tagPos.Z, a_Player.Client.Dimension));
			}
		}
	}

	private void OnCooldownExpire(EntityDatabaseID AccountID, EntityDatabaseID TargetCharID)
	{
		if (g_dictTagCleaningCooldowns.ContainsKey(AccountID))
		{
			var innerList = g_dictTagCleaningCooldowns[AccountID];

			innerList.Remove(TargetCharID);

			if (innerList.Count == 0)
			{
				// cleanup
				g_dictTagCleaningCooldowns.Remove(AccountID);
			}
		}
	}

	private bool DoesPlayerHaveCooldownForCharacterID(CPlayer a_Player, EntityDatabaseID a_CharacterID)
	{
		bool bHasCooldown = false;

		if (g_dictTagCleaningCooldowns.ContainsKey(a_Player.AccountID))
		{
			if (g_dictTagCleaningCooldowns[a_Player.AccountID].Contains(a_CharacterID))
			{
				bHasCooldown = true;
			}
		}

		return bHasCooldown;
	}

	private void OnAdminRequestClearTag(CPlayer player, EntityDatabaseID a_ID)
	{
		CGangTag gangtag = GetTagFromID(a_ID);
		if (gangtag != null)
		{
			gangtag.DestroyTag();
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted a tag with ID: {0}", a_ID);
		}
	}

	private void OnRequestStartCleaning(CPlayer a_Player, EntityDatabaseID a_ID)
	{
		CGangTag gangtag = GetTagFromID(a_ID);
		if (gangtag != null)
		{
			bool bHasCooldown = DoesPlayerHaveCooldownForCharacterID(a_Player, gangtag.GetOwnerCharacterID());

			if (!bHasCooldown)
			{
				NetworkEventSender.SendNetworkEvent_StartTagCleaningResponse(a_Player, true);

				if (!g_dictTagCleaningCooldowns.ContainsKey(a_Player.AccountID))
				{
					g_dictTagCleaningCooldowns.Add(a_Player.AccountID, new List<EntityDatabaseID>());
				}

				g_dictTagCleaningCooldowns[a_Player.AccountID].Add(gangtag.GetOwnerCharacterID());
				MainThreadTimerPool.CreateGlobalTimer((object[] parameters) => { OnCooldownExpire(a_Player.AccountID, gangtag.GetOwnerCharacterID()); }, 300000, 1);

				a_Player.Freeze(true);
			}
			else
			{
				a_Player.SendNotification("Graffiti Removal", ENotificationIcon.ExclamationSign, "You recently cleaned another tag from this player. Please wait before trying again.");
				NetworkEventSender.SendNetworkEvent_StartTagCleaningResponse(a_Player, false);
			}
		}
		else
		{
			// send not approved
			NetworkEventSender.SendNetworkEvent_StartTagCleaningResponse(a_Player, false);
		}
	}
}

public class CGangTag : CBaseEntity
{
	private List<GangTagLayer> m_lstLayers = null;
	private float m_fProgress = 0.0f;
	private EntityDatabaseID m_OwnerCharacterID = -1;
	private Vector3 m_vecPos = null;
	private float m_fRot = 0.0f;
	private uint m_Dimension = 0;

	public CGangTag(EntityDatabaseID a_ID, EntityDatabaseID a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress)
	{
		m_DatabaseID = a_ID;
		m_OwnerCharacterID = a_OwnerCharacterID;
		m_vecPos = vecPos;
		m_fRot = fRotZ;
		m_Dimension = a_Dimension;

		m_lstLayers = lstLayers;
		m_fProgress = fProgress;

		NetworkEventSender.SendNetworkEvent_CreateGangTag_ForAll_IncludeEveryone(a_ID, a_OwnerCharacterID, vecPos, fRotZ, a_Dimension, lstLayers, fProgress);
	}

	public void SendToPlayer(CPlayer player)
	{
		NetworkEventSender.SendNetworkEvent_CreateGangTag(player, m_DatabaseID, m_OwnerCharacterID, m_vecPos, m_fRot, m_Dimension, m_lstLayers, m_fProgress);
	}

	public EntityDatabaseID GetOwnerCharacterID()
	{
		return m_OwnerCharacterID;
	}

	public void DestroyTag()
	{
		Destroy(true);
	}

	public bool UpdateCleaningProgressLayer(CPlayer a_Player)
	{
		float fPercentPerLayer = 100.0f / m_lstLayers.Count;
		float fPercentPerLayerPerIncrement = fPercentPerLayer / 10.0f;
		SetProgressAmount(m_fProgress - fPercentPerLayerPerIncrement);
		Database.Functions.Items.SetGangTagProgress(m_DatabaseID, m_fProgress);

		if (m_fProgress <= 0.0f)
		{
			TagRemoverJobInstance jobInstance = JobSystem.GetTagRemoverInstance(a_Player);
			if (jobInstance != null)
			{
				jobInstance.OnCompleteRun();
			}

			a_Player.Freeze(false);
			NetworkEventSender.SendNetworkEvent_TagCleaningComplete(a_Player);

			Destroy(true);

			return true;
		}

		return false;
	}

	private async void Destroy(bool bDeleteFromDB)
	{
		NetworkEventSender.SendNetworkEvent_DestroyGangTag_ForAll_IncludeEveryone(m_DatabaseID);

		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyGangTag(m_DatabaseID).ConfigureAwait(true);
		}
	}

	public void UpdateProgressLayer(CPlayer a_Player)
	{
		float fPercentPerLayer = 100.0f / m_lstLayers.Count;
		float fPercentPerLayerPerIncrement = fPercentPerLayer / 10.0f;
		SetProgressAmount(m_fProgress + fPercentPerLayerPerIncrement);
		Database.Functions.Items.SetGangTagProgress(m_DatabaseID, m_fProgress);


		if (m_fProgress >= 100.0f)
		{
			a_Player.Freeze(false);
			NetworkEventSender.SendNetworkEvent_InformClientTaggingComplete(a_Player);
		}
	}

	public void SetProgressAmount(float fProgress)
	{
		if (fProgress < 0.0f)
		{
			fProgress = 0.0f;
		}

		if (fProgress > 100.0f)
		{
			fProgress = 100.0f;
		}

		m_fProgress = fProgress;

		NetworkEventSender.SendNetworkEvent_UpdateGangTagProgress_ForAll_IncludeEveryone(m_DatabaseID, m_fProgress);
	}

	public void SetInProgress(CPlayer a_Player)
	{
		m_lstLayers = a_Player.GangTag;

		SetLayers(a_Player.GangTag);

		SetProgressAmount(0.0f);
	}

	public void SetLayers(List<GangTagLayer> a_lstLayers)
	{
		m_lstLayers = a_lstLayers;

		if (a_lstLayers != null)
		{
			NetworkEventSender.SendNetworkEvent_UpdateGangTagLayers_ForAll_IncludeEveryone(m_DatabaseID, m_lstLayers);
		}
	}
}

