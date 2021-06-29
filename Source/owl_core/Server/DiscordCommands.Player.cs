using System;
using System.Collections.Generic;

public class DiscordCommands_Player
{
	public DiscordCommands_Player()
	{
		DiscordCommandManager.RegisterCommand("admins", "Shows admin list", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(AdminList), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.GuestsAllowed, EAdminLevel.None);
		DiscordCommandManager.RegisterCommand("serverinfo", "Shows server info", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(ServerInfo), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.GuestsAllowed, EAdminLevel.None);
		DiscordCommandManager.RegisterCommand("whoami", "Shows linked account status", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(WhoAmI), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.GuestsAllowed, EAdminLevel.None);
		DiscordCommandManager.RegisterCommand("donate", "Shows information about donating and member roles", new Action<DiscordUser, BasicAccountInfo, EDiscordChannelIDs>(Donate), DiscordCommandParsingFlags.Default, EDiscordChannelIDs.DirectMessage, EDiscordAuthRequirements.GuestsAllowed, EAdminLevel.None);
	}

	private void Donate(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		// TODO_GITHUB: You should replace the below with your own website
		DiscordBotIntegration.PushMessage(discordUser, channelToUse,
			"Consider supporting the project by purchasing Gamecoins by visiting our website: https://website.com/account/purchase\n" +
			"You will automatically get a special Discord role by spending a certain amount! Visit the page for more details.");
	}

	private void WhoAmI(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		string strResponseDM;

		if (accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.OK)
		{
			strResponseDM = Helpers.FormatString("You are:\n\n Discord: {0}\n Owl: {1}\n Account Type: {2}", discordUser.Username, accountInfo.Username, accountInfo.AdminLevel);
		}
		else
		{
			strResponseDM = Helpers.FormatString("Your Discord account '{0}' is not linked to any Owl account.", discordUser.Username);
		}

		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponseDM);
	}

	private void ServerInfo(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		// TODO_GITHUB: You should replace the below with your own website/IP address
		string strResponseDM = "Server IP: v.website.com:5000 or direct connect rage://v/connect?ip=v.website.com:5000";
		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponseDM);
	}

	private void AdminList(DiscordUser discordUser, BasicAccountInfo accountInfo, EDiscordChannelIDs channelToUse)
	{
		List<CPlayer> adminsList = new List<CPlayer>();
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			if (player.IsAdmin(EAdminLevel.TrialAdmin, true))
			{
				adminsList.Add(player);
			}
		}

		int count = adminsList.Count;
		string strResponseDM = Helpers.FormatString("OwlV has {0} admins:", count);
		if (count > 0)
		{
			adminsList.Sort((a, b) => a.AdminLevel.CompareTo(b.AdminLevel));
			adminsList.Reverse();

			foreach (CPlayer player in adminsList)
			{
				int latency = player.GetData<int>(player.Client, EDataNames.PING);
				int playerID = player.PlayerID;
				bool bIsLoggedIn = player.IsLoggedIn;
				bool bIsSpawned = player.IsSpawned;

				strResponseDM += Helpers.FormatString("\n\t{0} {1} ({2})", player.AdminTitle, player.Username, player.GetCharacterName(ENameType.CharacterDisplayName));
			}
		}
		else
		{
			strResponseDM += "\n\t No Admins";
		}

		DiscordBotIntegration.PushMessage(discordUser, channelToUse, strResponseDM);
	}
}