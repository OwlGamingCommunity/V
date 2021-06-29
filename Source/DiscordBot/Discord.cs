#define USE_DISCORD_IN_DEBUG

using Discord;
using Discord.Rest;
using Discord.WebSocket;
using owl_discord;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class Helpers
{
	public static Int64 GetUnixTimestamp(bool toUTC = false)
	{
		DateTime now = DateTime.Now;

		if (toUTC)
		{
			now = now.ToUniversalTime();
		}

		return (Int64)now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
	}

	public static string FormatString(string strFormat, params object[] strParams)
	{
		return String.Format(new System.Globalization.CultureInfo("en-US"), strFormat, strParams);
	}
}

public class DiscordBot
{
	enum EBotAction
	{
		PushScriptedMessage
	}

	DiscordSocketClient discord = null;

	private Dictionary<EDiscordChannelIDs, ulong> g_dictChannelIDs = new Dictionary<EDiscordChannelIDs, ulong>();
	private Dictionary<EDiscordChannelIDs, ISocketMessageChannel> g_dictChannels = new Dictionary<EDiscordChannelIDs, ISocketMessageChannel>();

	public DiscordBot()
	{
		InitAsync();
	}

	~DiscordBot()
	{
		discord.LogoutAsync();
	}

	public string GetDiscordUsernameFromID(UInt64 discordID)
	{
		var user = discord.GetUser(discordID);
		if (user != null)
		{
			return user.Username;
		}

		return String.Empty;
	}

	public void UpdateBotStatus(string strStatus)
	{
		Game game = new Game(strStatus, ActivityType.Playing);
		discord.SetStatusAsync(UserStatus.Online);
		discord.SetActivityAsync(game);
	}

	private Task OnReady()
	{
		// cache our channels
#if DEBUG
		{
			g_dictChannelIDs[EDiscordChannelIDs.GeneralChat] = 660514719848857647;
			g_dictChannelIDs[EDiscordChannelIDs.Ads] = 660519621409112125;
			g_dictChannelIDs[EDiscordChannelIDs.AdminChat] = 660528753226547220;
			g_dictChannelIDs[EDiscordChannelIDs.AdminCommands] = 662965078164897798;
			g_dictChannelIDs[EDiscordChannelIDs.PeakAlerts] = 696347108898504776;
			g_dictChannelIDs[EDiscordChannelIDs.PlayerCounts] = 696347341648691280;
			g_dictChannelIDs[EDiscordChannelIDs.AnticheatAlerts] = 716206655171461151;
			g_dictChannelIDs[EDiscordChannelIDs.ServerPerfAlerts] = 744440101563793451;
		}
#else
		{
			g_dictChannelIDs[EDiscordChannelIDs.GeneralChat] = 149719196136243200;
			g_dictChannelIDs[EDiscordChannelIDs.Ads] = 627929487069413426;
			g_dictChannelIDs[EDiscordChannelIDs.AdminChat] = 696357119666028605;
			g_dictChannelIDs[EDiscordChannelIDs.AdminCommands] = 662968057727287306;
			g_dictChannelIDs[EDiscordChannelIDs.PeakAlerts] = 696356635882422333;
			g_dictChannelIDs[EDiscordChannelIDs.PlayerCounts] = 696357725944152064;
			g_dictChannelIDs[EDiscordChannelIDs.AnticheatAlerts] = 716206784267944018;
			g_dictChannelIDs[EDiscordChannelIDs.ServerPerfAlerts] = 744440877862223904;
		}
#endif

		Task task = Task.Run(() => { });
		return task;
	}

	private bool IsChannelAnAdminChannel(ulong channelID)
	{
		EDiscordChannelIDs discordChannelID = EDiscordChannelIDs.DirectMessage;
		foreach (var channel in g_dictChannelIDs)
		{
			if (channel.Value == channelID)
			{
				discordChannelID = channel.Key;
				break;
			}
		}

		if (discordChannelID == EDiscordChannelIDs.AdminChat || discordChannelID == EDiscordChannelIDs.AdminCommands)
		{
			return true;
		}

		return false;
	}

	public bool IsReady()
	{
		return discord != null && discord.ConnectionState == ConnectionState.Connected;
	}

	public bool IsChannelIDDefined(ulong channelID, EDiscordChannelIDs discordChannelID)
	{
		if (g_dictChannelIDs.ContainsKey(discordChannelID))
		{
			return g_dictChannelIDs[discordChannelID] == channelID;
		}

		return false;
	}

	private uint g_cooldownLengthSeconds = 10;
	private Dictionary<ulong, double> m_dictCooldowns = new Dictionary<ulong, double>();

	private bool DoesDiscordClientHaveCooldown(ulong channelID, SocketUser user)
	{
		// Never have a cooldown for admin channels
		if (IsChannelAnAdminChannel(channelID))
		{
			return false;
		}

		ExpireCooldowns();
		return m_dictCooldowns.ContainsKey(user.Id);
	}

	private void ExpireCooldowns()
	{
		Int64 unixTimestamp = Helpers.GetUnixTimestamp();

		List<ulong> m_lstToRemove = new List<ulong>();
		foreach (var kvPair in m_dictCooldowns)
		{
			if ((kvPair.Value + g_cooldownLengthSeconds) <= unixTimestamp)
			{
				m_lstToRemove.Add(kvPair.Key);
			}
		}

		foreach (ulong key in m_lstToRemove)
		{
			m_dictCooldowns.Remove(key);
		}
	}

	private void CreateCooldown(ulong channelID, SocketUser user)
	{
		if (!IsChannelAnAdminChannel(channelID))
		{
			Int64 unixTimestamp = Helpers.GetUnixTimestamp();
			m_dictCooldowns[user.Id] = unixTimestamp;
		}
	}

	Regex g_HtmlRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
	private Task OnMessageReceived(SocketMessage message)
	{
		try
		{
			if (message.Content.Length > 0)
			{
				if (message.Content[0] == '!')
				{
					if (!DoesDiscordClientHaveCooldown(message.Channel.Id, message.Author))
					{
						EDiscordChannelIDs enumChannelID = EDiscordChannelIDs.DirectMessage;
						// Do we have this channel / is it a real channel and not DM?
						foreach (var kvPair in g_dictChannelIDs)
						{
							if (kvPair.Value == message.Channel.Id)
							{
								enumChannelID = kvPair.Key;
								break;
							}
						}

						CreateCooldown(message.Channel.Id, message.Author);
						JSONRequest_PushCommand requestToSend = new JSONRequest_PushCommand(new DiscordUser(message.Author.Id, message.Author.Username), message.Content, enumChannelID);
						Program.GetRestClient().QueueRequest(requestToSend, CRestClient.ERestCallbackThreadingMode.ContinueOnWorkerThread, null);
					}
					else
					{
						PushDM(message.Author, "Too many commands. Please wait.");
					}
				}
				else
				{
					// admin chat bi-directional chat
					if (g_dictChannelIDs.ContainsKey(EDiscordChannelIDs.AdminChat) && message.Channel.Id == g_dictChannelIDs[EDiscordChannelIDs.AdminChat])
					{
						if (!message.Author.IsBot)
						{
							string strMessage = message.Content;
							if (g_HtmlRegex.IsMatch(strMessage))
							{
								strMessage = Helpers.FormatString("{0} is naughty and tried to send HTML!", message.Author.Username);
							}

							JSONRequest_BiDirectionalAdminChat requestToSend = new JSONRequest_BiDirectionalAdminChat(new DiscordUser(message.Author.Id, message.Author.Username), strMessage);
							Program.GetRestClient().QueueRequest(requestToSend, CRestClient.ERestCallbackThreadingMode.ContinueOnWorkerThread, null);
						}
					}
				}
			}
		}
		catch
		{

		}

		Task task = Task.Run(() => { });
		return task;
	}

	private async void InitAsync()
	{
#if !DEBUG || USE_DISCORD_IN_DEBUG
		discord = new DiscordSocketClient();

		// event handlers
		discord.Connected += OnReady;
		discord.MessageReceived += OnMessageReceived;

		// TODO_GITHUB: You should replace the below with your debug key, and also set the environment variable on your server for your release token. These should be different for security purposes.
#if DEBUG
		string Token = "TODO_GITHUB";
#else
		string Token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? "";
#endif

		await discord.LoginAsync(TokenType.Bot, Token).ConfigureAwait(true);
		await discord.StartAsync().ConfigureAwait(true);
#else
		await Task.Delay(1).ConfigureAwait(true);
#endif
	}

	public void PushDM(SocketUser user, string strMessage)
	{
		try
		{
			if (user != null)
			{
				user.SendMessageAsync(strMessage);
			}
		}
		catch
		{
			// User probably has some privacy settings that do not allow us to send DMs
		}
	}

	public SocketUser GetDiscordUserFromDiscordID(ulong DiscordUserID)
	{
		return discord.GetUser(DiscordUserID);
	}

	private ISocketMessageChannel GetChannel(EDiscordChannelIDs channelID)
	{
		ISocketMessageChannel channel = null;
		if (g_dictChannels.ContainsKey(channelID) && g_dictChannels[channelID] != null)
		{
			channel = g_dictChannels[channelID];
		}
		else
		{
			if (g_dictChannelIDs.ContainsKey(channelID))
			{
				channel = (ISocketMessageChannel)discord.GetChannel(g_dictChannelIDs[channelID]);
				g_dictChannels[channelID] = channel;
			}
		}

		return channel;
	}

	public void PushChannelMessage(EDiscordChannelIDs channelID, string strMessage)
	{
		try
		{
			ISocketMessageChannel channel = GetChannel(channelID);
			if (channel != null)
			{
				channel.SendMessageAsync(strMessage);
			}
		}
		catch
		{

		}
	}

	public async Task PushMessage(SocketUser user, ulong channelToUse, string strMessage)
	{
		try
		{
			if (channelToUse == (ulong)EDiscordChannelIDs.DirectMessage)
			{
				PushDM(user, strMessage);
			}
			else
			{
				ISocketMessageChannel channel = (ISocketMessageChannel)discord.GetChannel(channelToUse);
				if (channel != null)
				{
					RestUserMessage msg = await channel.SendMessageAsync(strMessage).ConfigureAwait(true);
				}
			}
		}
		catch
		{

		}
	}
}
