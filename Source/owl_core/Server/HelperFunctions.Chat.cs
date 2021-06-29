using GTANetworkAPI;
using System;
using System.Collections.Generic;

namespace HelperFunctions
{
	public static class Chat
	{
		public static List<CPlayer> GetEligiblePlayersForChat(CPlayer RequestingPlayer, float fRadius)
		{
			Vector3 vecPlayerPos = RequestingPlayer.Client.Position;
			uint PlayerDimension = RequestingPlayer.Client.Dimension;

			return GetEligiblePlayersForChat(vecPlayerPos, PlayerDimension, fRadius);
		}

		public static List<CPlayer> GetEligiblePlayersForChat(Vector3 vecPos, uint Dimension, float fRadius)
		{
			List<CPlayer> lstPlayers = new List<CPlayer>();

			// Get players within range
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				if (player.Client.Dimension == Dimension) // Are we in the same dimension?
				{
					float fDist = (new Vector3(player.Client.Position.X, player.Client.Position.Y, 0.0f) - new Vector3(vecPos.X, vecPos.Y, 0.0f)).Length();

					// Are we within the 2d plane distance?
					if (fDist < fRadius)
					{
						float fZRadius = fRadius / 3.0f;
						float fLowerZ = vecPos.Z - fZRadius;
						float fUpperZ = vecPos.Z + fZRadius;

						if (player.Client.Position.Z >= fLowerZ && player.Client.Position.Z <= fUpperZ)
						{
							lstPlayers.Add(player);
						}
					}
				}
			}

			return lstPlayers;
		}

		public static void SendMeMessage(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerMe, null, Helpers.FormatString(strMessage));

			foreach (var player in lstPlayers)
			{
				player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 51, 102, "{0} {1}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), strMessage);
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendPedEmote(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerDo, null, Helpers.FormatString(strMessage + " [AUTOMATED]"));

			foreach (var player in lstPlayers)
			{
				player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 51, 102, "{0}", strMessage);
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendPedSpeak(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerSay, null, Helpers.FormatString(strMessage + " [AUTOMATED]"));

			foreach (var player in lstPlayers)
			{
				player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 255, 255, "{0}", strMessage);
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendDistrictMessage(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_District);

			foreach (var player in lstPlayers)
			{
				player.PushChatMessage(EChatChannel.Nearby, "District IC: {0} (({1}))", strMessage, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
			}
		}

		public static void SendDoMessage(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerDo, null, Helpers.FormatString(strMessage));

			foreach (var player in lstPlayers)
			{
				player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 51, 102, "{0} (({1}))", strMessage, SenderPlayer.GetCharacterName(ENameType.StaticCharacterName));
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendObjectDoMessage(Vector3 vecObjectPos, uint objectDimension, string strObjectDisplayName, string strMessage)
		{
			List<CPlayer> lstPlayers = HelperFunctions.Chat.GetEligiblePlayersForChat(vecObjectPos, objectDimension, ChatConstants.g_fDistance_Nearby);

			foreach (var player in lstPlayers)
			{
				player.PushChatMessageWithColor(EChatChannel.Nearby, 255, 51, 102, "{0} (({1}))", strMessage, strObjectDisplayName);
			}
		}

		/// <summary>
		/// Send a message to all admins in game.
		/// </summary>
		/// <remarks>
		/// Exported.
		/// </remarks>
		/// <param name="Message">The message to send</param>
		/// <param name="onDuty">If true, only send to on duty admins</param>
		/// <param name="level">The admin level which should see this message</param>
		public static void SendMessageToAdmins(string message, bool onDuty = false, EAdminLevel level = EAdminLevel.TrialAdmin, int r = 95, int g = 244, int b = 66, EChatChannel chatChannel = EChatChannel.Global)
		{
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				if (player.IsAdmin(level, onDuty))
				{
					player.PushChatMessageWithColor(chatChannel, r, g, b, "[ADMIN] {0}", message);
				}
			}
		}

		public static void SendMessageToScripters(string message, EScripterLevel level = EScripterLevel.TrialScripter, int r = 95, int g = 244, int b = 66, EChatChannel chatChannel = EChatChannel.Global)
		{
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				if (player.IsScripter(level))
				{
					player.PushChatMessageWithColor(chatChannel, r, g, b, "[SCRIPTER] {0}", message);
				}
			}
		}

		public static void SendServerMessage(string message, int r = 255, int g = 255, int b = 255)
		{
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				player.PushChatMessageWithColor(EChatChannel.Global, r, g, b, "[SERVER] {0}", message);
			}
		}

		public static Color GetAdminTagColor(CPlayer SenderPlayer)
		{
			Color adminNametagColor = new Color(196, 255, 255);
			if (SenderPlayer.AdminDuty)
			{
				if (SenderPlayer.AdminLevel == EAdminLevel.TrialAdmin || SenderPlayer.AdminLevel == EAdminLevel.Admin || SenderPlayer.AdminLevel == EAdminLevel.SeniorAdmin)
				{
					adminNametagColor = new Color(255, 194, 14);
				}
				else if (SenderPlayer.AdminLevel == EAdminLevel.LeadAdmin || SenderPlayer.AdminLevel == EAdminLevel.HeadAdmin)
				{
					adminNametagColor = new Color(14, 194, 255);
				}
			}

			return adminNametagColor;
		}

		public static void SendAdminAnnouncement(CPlayer SenderPlayer, string strMessage)
		{
			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			foreach (var player in players)
			{
				player.PushChatMessageWithColorAndPlayerName(EChatChannel.Global, 255, 194, 15, "[ADMIN ANNOUNCEMENT] " + SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), "{0}", strMessage);
			}
			new Logging.Log(SenderPlayer, Logging.ELogType.AdminCommand, null, Helpers.FormatString("/ann used by {0} - {1}", SenderPlayer.Username, strMessage)).execute();
		}

		public static void SendAmeMessage(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			SenderPlayer.SetData(SenderPlayer.Client, EDataNames.AME_MESSAGE, strMessage, EDataType.Synced);
			SenderPlayer.SetData(SenderPlayer.Client, EDataNames.MESSAGE_DRAWN, "ame", EDataType.Synced);

			long msgDuration = GetDurationForDrawnActionMessage(strMessage);

			WeakReference<MainThreadTimer> resetAmeTimer = MainThreadTimerPool.CreateEntityTimer(ResetTextDrawTimer_Elapsed, msgDuration, SenderPlayer, 1, new object[] { SenderPlayer, "ame" });
			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerAme, null, Helpers.FormatString("/ame used by {0} ({1}) - {2}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, strMessage));

			foreach (CPlayer player in lstPlayers)
			{
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendAdoMessage(CPlayer SenderPlayer, string strMessage)
		{
			List<CPlayer> lstPlayers = GetEligiblePlayersForChat(SenderPlayer, ChatConstants.g_fDistance_Nearby);

			SenderPlayer.SetData(SenderPlayer.Client, EDataNames.ADO_MESSAGE, strMessage, EDataType.Synced);
			SenderPlayer.SetData(SenderPlayer.Client, EDataNames.MESSAGE_DRAWN, "ado", EDataType.Synced);

			long msgDuration = GetDurationForDrawnActionMessage(strMessage);

			WeakReference<MainThreadTimer> resetAdoTimer = MainThreadTimerPool.CreateEntityTimer(ResetTextDrawTimer_Elapsed, msgDuration, SenderPlayer, 1, new object[] { SenderPlayer, "ado" });
			Logging.Log log = new Logging.Log(SenderPlayer, Logging.ELogType.PlayerAdo, null, Helpers.FormatString("/ado used by {0} ({1}) - {2}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, strMessage));

			foreach (CPlayer player in lstPlayers)
			{
				log.addAffected(player);
			}
			log.execute();
		}

		public static void SendStatusMessage(CPlayer SenderPlayer, string strMessage)
		{
			SenderPlayer.SetData(SenderPlayer.Client, EDataNames.STATUS_MESSAGE, strMessage, EDataType.Synced);
			new Logging.Log(SenderPlayer, Logging.ELogType.PlayerSay, null, Helpers.FormatString("/status used by {0} ({1}) - {2}", SenderPlayer.GetCharacterName(ENameType.StaticCharacterName), SenderPlayer.Username, strMessage)).execute();
		}

		private static long GetDurationForDrawnActionMessage(string strMessage)
		{
			long msgDuration = (strMessage.Length / 4) * 1000; // Get amount of characters. Divide by 4. 0.25 Second per character. * 1000 for time in MS.
			msgDuration = (msgDuration < 5000) ? 5000 : msgDuration; // if it's less than 5 seconds then leave it at 5 seconds

			return msgDuration;
		}

		public static void ClearStatusMessage(CPlayer SenderPlayer)
		{
			SenderPlayer.ClearData(SenderPlayer.Client, EDataNames.STATUS_MESSAGE);
		}

		private static void ResetTextDrawTimer_Elapsed(object[] a_Parameters)
		{
			CPlayer player = (CPlayer)a_Parameters[0];
			string textType = (string)a_Parameters[1];

			if (player != null)
			{
				if (textType.Equals("ame"))
				{
					player.ClearData(player.Client, EDataNames.AME_MESSAGE);
				}
				else
				{
					player.ClearData(player.Client, EDataNames.ADO_MESSAGE);
				}

				player.ClearData(player.Client, EDataNames.MESSAGE_DRAWN);
			}
		}

		public static void SendScriptedAdvertisementWithSender(string phoneNumber, string strMessage, CPlayer SenderPlayer)
		{
			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			foreach (var pPlayer in players)
			{
				if (pPlayer.IsAdmin(EAdminLevel.TrialAdmin, false))
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} ))", strMessage, phoneNumber, $"AUTOMATED - {SenderPlayer.GetCharacterName(ENameType.StaticCharacterName)}");
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}", strMessage, phoneNumber);
				}
			}
		}

		public static void SendScriptedAdvertisement(string phoneNumber, string strMessage)
		{
			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			foreach (var pPlayer in players)
			{
				if (pPlayer.IsAdmin(EAdminLevel.TrialAdmin, false))
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1} (( {2} ))", strMessage, phoneNumber, "AUTOMATED");
				}
				else
				{
					pPlayer.PushChatMessageWithColor(EChatChannel.Global, 50, 255, 50, "[AD] {0} - #{1}", strMessage, phoneNumber);
				}
			}
		}
	}
}