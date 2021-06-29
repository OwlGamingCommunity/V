using GTANetworkAPI;
using System;
using System.Collections.Generic;

public static class PlayerPool
{
	static PlayerPool()
	{
		for (int i = 0; i < m_bSlots.Length; ++i)
		{
			m_bSlots[i] = false;
		}

		// EVENTS
		RageEvents.RAGE_OnUpdate += Tick;
		RageEvents.RAGE_OnPlayerConnected += OnPlayerConnected;
		RageEvents.RAGE_OnPlayerDisconnected += OnPlayerDisconnected;

		NetworkEvents.RequestDimensionChange += OnPlayerRequestDimensionChange;
	}

	private static void OnPlayerRequestDimensionChange(CPlayer SenderPlayer, uint dimension)
	{
		SenderPlayer.SetSafeDimension(dimension);
	}

	public static void Tick()
	{
		foreach (CPlayer player in m_LookupTable.Values)
		{
			player.OnUpdate();
		}

		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			player.OnUpdateInGame();
		}
	}

	public static void SaveAll()
	{
		foreach (CPlayer player in m_LookupTable.Values)
		{
			player.Save();
		}
	}


	// NOTE: Do NOT use this event anywhere else, use the Owl event provider system instead
	public static void OnPlayerDisconnected(Player player, GTANetworkAPI.DisconnectionType type, string reason)
	{
		WeakReference<CPlayer> refPlayer = GetPlayerFromClient(player);
		CPlayer pPlayer = refPlayer.Instance();
		if (pPlayer != null)
		{
			// Attempt to autopark
			if (pPlayer.IsInVehicleReal)
			{
				CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(pPlayer.Client.Vehicle);
				if (vehicle != null)
				{
					vehicle.Save();
				}
			}

			pPlayer.Save();

			// Disconnect logging (RAGE DisconnectTypes don't work and don't register)
			Logging.Log log = new Logging.Log(pPlayer, Logging.ELogType.ConnectionEvents, null, "Player Disconnected");
			log.execute();
			// Inform other resources
			NetworkEvents.SendLocalEvent_OnPlayerDisconnected(pPlayer, type, reason);

			// TODO_POST_LAUNCH: Rewrite slot detection not to use CPlayer
			m_bSlots[pPlayer.PlayerID - 1] = false;
		}

		m_LookupTable.Remove(player.Handle);
		m_LookupTableInGame.Remove(player.Handle);

		if (pPlayer != null)
		{
			pPlayer.Cleanup();
			pPlayer.Dispose();
			// TODO_POST_LAUNCH: Other entities aren't disposed properly
		}
	}

	private static int ConsumeNextFreeSlotID()
	{
		for (int i = 0; i < m_bSlots.Length; ++i)
		{
			if (!m_bSlots[i])
			{
				m_bSlots[i] = true;
				return i + 1;
			}
		}

		PrintLogger.LogMessage(ELogSeverity.ERROR, "[PLAYER IDS] Error: Ran out of space!");
		return -1;
	}

	// NOTE: Do NOT use this event anywhere else, use the Owl event provider system instead
	public static async void OnPlayerConnected(Player player)
	{
		int playerID = ConsumeNextFreeSlotID();
		CPlayer newPlayer = new CPlayer(player, playerID);
		m_LookupTable[player.Handle] = newPlayer;

		AccountBanDetails banDetails = await Database.LegacyFunctions.CheckForDeviceBan(player.Serial, player.Address).ConfigureAwait(true);

		if (banDetails.IsBanned)
		{
			newPlayer.KickFromServer(CPlayer.EKickReason.ADMIN_BANNED, Helpers.FormatString("You are banned {0} for '{1}'.", banDetails.Until.Length == 0 ? "permanently" : Helpers.FormatString("until {0}", banDetails.Until), banDetails.GetDisplayReason()));
		}
		else
		{
			// Inform other resources
			NetworkEvents.SendLocalEvent_OnPlayerConnected(newPlayer);
		}
	}

	public static ICollection<CPlayer> GetAllPlayers_IncludeOutOfGame()
	{
		return m_LookupTable.Values;
	}

	public static WeakReference<CPlayer> GetPlayerFromClient_IncludeOutOfGame(Player player)
	{
		if (player == null)
		{
			return new WeakReference<CPlayer>(null);
		}

		CPlayer retVal = null;
		m_LookupTable.TryGetValue(player.Handle, out retVal);
		return new WeakReference<CPlayer>(retVal);
	}

	public static WeakReference<CPlayer> GetPlayerFromNetHandle_IncludeOutOfGame(NetHandle handle)
	{
		return new WeakReference<CPlayer>(m_LookupTable[handle]);
	}

	public static WeakReference<CPlayer> GetPlayerFromPlayerID_IncludeOutOfGame(int a_PlayerID)
	{
		// Early out if we haven't assigned a player to that slot
		if (!m_bSlots[a_PlayerID - 1])
		{
			return new WeakReference<CPlayer>(null);
		}

		foreach (CPlayer player in m_LookupTable.Values)
		{
			if (player.PlayerID == a_PlayerID)
			{
				return new WeakReference<CPlayer>(player);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromCharacterID_IncludeOutOfGame(long a_CharacterID)
	{
		foreach (CPlayer player in m_LookupTable.Values)
		{
			if (player.ActiveCharacterDatabaseID == a_CharacterID)
			{
				return new WeakReference<CPlayer>(player);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromAccountID_IncludeOutOfGame(int a_AccountID)
	{
		foreach (CPlayer player in m_LookupTable.Values)
		{
			if (player.AccountID == a_AccountID)
			{
				return new WeakReference<CPlayer>(player);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromAccountID64_IncludeOutOfGame(long a_AccountID)
	{
		return GetPlayerFromAccountID_IncludeOutOfGame(Convert.ToInt32(a_AccountID));
	}

	public static WeakReference<CPlayer> GetPlayerFromPartialName_IncludeOutOfGame(string a_strPartialName)
	{
		string strPartialNameLower = a_strPartialName.ToLower();

		foreach (CPlayer player in m_LookupTable.Values)
		{
			if (player.DoesAnyCharacterNameMatchPartial(strPartialNameLower))
			{
				return new WeakReference<CPlayer>(player);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromPartialNameOrID_IncludeOutOfGame(string a_strPartialNameOrID)
	{
		// Check for ID first
		bool isNumeric = int.TryParse(a_strPartialNameOrID, out int ID);
		if (isNumeric)
		{
			// Do we have a player with that ID?
			WeakReference<CPlayer> playerWithID = GetPlayerFromPlayerID_IncludeOutOfGame(ID);
			return playerWithID;
		}

		// Check for partial name
		WeakReference<CPlayer> playerWithPartialName = GetPlayerFromPartialName_IncludeOutOfGame(a_strPartialNameOrID.ToLower());
		return playerWithPartialName;
	}

	public static WeakReference<CPlayer> GetPlayerFromName_IncludeOutOfGame(string a_strFullName)
	{
		foreach (CPlayer player in m_LookupTable.Values)
		{
			if (player.DoesAnyCharacterNameMatch(a_strFullName))
			{
				return new WeakReference<CPlayer>(player);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	private static Dictionary<NetHandle, CPlayer> m_LookupTable = new Dictionary<NetHandle, CPlayer>();
	private static Dictionary<NetHandle, CPlayer> m_LookupTableInGame = new Dictionary<NetHandle, CPlayer>();
	private const int g_sMaxSlots = 1000;
	private static bool[] m_bSlots = new bool[g_sMaxSlots];

	public static void SetPlayerAsInGame(CPlayer a_Player, bool a_bInGame)
	{
		if (a_bInGame)
		{
			m_LookupTableInGame[a_Player.Client.Handle] = a_Player;
		}
		else
		{
			m_LookupTableInGame.Remove(a_Player.Client.Handle);
		}
	}

	public static ICollection<CPlayer> GetAllPlayers()
	{
		return m_LookupTableInGame.Values;
	}

	public static WeakReference<CPlayer> GetPlayerFromClient(Player client)
	{
		if (client != null)
		{
			return GetPlayerFromNetHandle(client.Handle);
		}
		else
		{
			return new WeakReference<CPlayer>(null);
		}
	}

	public static WeakReference<CPlayer> GetPlayerFromNetHandle(NetHandle handle)
	{
		if (m_LookupTableInGame.ContainsKey(handle))
		{
			CPlayer pPlayer = m_LookupTableInGame[handle];
			if (pPlayer != null)
			{
				return pPlayer.IsInGame() ? new WeakReference<CPlayer>(pPlayer) : new WeakReference<CPlayer>(null);
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromPlayerID(int a_PlayerID)
	{
		// Early out if we haven't assigned a player to that slot
		int slotIndex = a_PlayerID - 1;
		if (slotIndex < 0 || slotIndex >= m_bSlots.Length || !m_bSlots[slotIndex])
		{
			return new WeakReference<CPlayer>(null);
		}

		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.PlayerID == a_PlayerID)
			{
				if (player.IsInGame())
				{
					return new WeakReference<CPlayer>(player);
				}
				else
				{
					return new WeakReference<CPlayer>(null);
				}
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromCharacterID(long a_CharacterID)
	{
		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.ActiveCharacterDatabaseID == a_CharacterID)
			{
				if (player.IsInGame())
				{
					return new WeakReference<CPlayer>(player);
				}
				else
				{
					return new WeakReference<CPlayer>(null);
				}
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static bool TryGetPlayerFromCharacterId(long a_CharacterID, out WeakReference<CPlayer> playerFound)
	{
		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.ActiveCharacterDatabaseID == a_CharacterID)
			{
				if (player.IsInGame())
				{
					playerFound = new WeakReference<CPlayer>(player);
					return true;
				}
				else
				{
					playerFound = new WeakReference<CPlayer>(null);
					return false;
				}
			}
		}

		playerFound = new WeakReference<CPlayer>(null);
		return false;
	}

	public static WeakReference<CPlayer> GetPlayerFromAccountID(int a_AccountID)
	{
		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.AccountID == a_AccountID)
			{
				if (player.IsInGame())
				{
					return new WeakReference<CPlayer>(player);
				}
				else
				{
					return new WeakReference<CPlayer>(null);
				}
			}
		}

		return new WeakReference<CPlayer>(null);
	}

	public static WeakReference<CPlayer> GetPlayerFromAccountID64(long a_AccountID)
	{
		return GetPlayerFromAccountID(Convert.ToInt32(a_AccountID));
	}

	public static WeakReference<CPlayer> GetPlayerFromPartialName(string a_strPartialName)
	{
		string strPartialNameLower = a_strPartialName.ToLower().Replace('_', ' ');
		CPlayer playerRet = null;

		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.DoesAnyCharacterNameMatchPartial(strPartialNameLower))
			{
				if (player.IsInGame())
				{
					if (playerRet != null)
					{
						// More than one match
						return new WeakReference<CPlayer>(null);
					};
					playerRet = player;
				}
				else
				{
					return new WeakReference<CPlayer>(null);
				}
			}
		}

		return new WeakReference<CPlayer>(playerRet);
	}

	public static WeakReference<CPlayer> GetPlayerFromPartialNameOrID(string a_strPartialNameOrID)
	{
		// Check for ID first
		bool isNumeric = int.TryParse(a_strPartialNameOrID, out int ID);
		if (isNumeric)
		{
			// Do we have a player with that ID?
			WeakReference<CPlayer> playerWithID = GetPlayerFromPlayerID(ID);
			return playerWithID; // func above already checks IsInGame
		}

		// Check for partial name
		WeakReference<CPlayer> playerWithPartialName = GetPlayerFromPartialName(a_strPartialNameOrID.ToLower());
		return playerWithPartialName;
	}

	public static WeakReference<CPlayer> GetPlayerFromName(string a_strFullName)
	{
		foreach (CPlayer player in m_LookupTableInGame.Values)
		{
			if (player.DoesAnyCharacterNameMatch(a_strFullName))
			{
				return new WeakReference<CPlayer>(player); // func above already checks IsInGame
			}
		}

		return new WeakReference<CPlayer>(null);
	}
}