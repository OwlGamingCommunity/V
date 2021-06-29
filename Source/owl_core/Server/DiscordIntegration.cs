//#define RUN_DISCORD_BOT_IN_DEBUG

using GTANetworkAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;

public class DiscordBotIntegration 
{
#if !DEBUG || RUN_DISCORD_BOT_IN_DEBUG
	private static CHTTPServer g_HTTPServer = null;
#endif
	private static CRestClient g_RestClient = null;
	private static Process g_BotProcess = null;

#pragma warning disable CA1823 // Remove unread private members
	private readonly DiscordCommands_Player m_discordCommandsPlayer = new DiscordCommands_Player();
	private readonly DiscordCommands_Admin m_discordCommandsAdmin = new DiscordCommands_Admin();
#pragma warning restore CA1823 // Remove unread private members

	public static CRestClient GetRestClient()
	{
		return g_RestClient;
	}

	private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
	{
		Console.WriteLine(outLine.Data);
	}

	public static void KillBotProcess()
	{
		try
		{
			if (IsBotRunning())
			{
				g_BotProcess.Kill();
			}

			// find any stragglers
			foreach (Process proc in Process.GetProcessesByName("DiscordBot"))
			{
				proc.Kill();
			}

			g_BotProcess = null;
		}
		catch
		{
			g_BotProcess = null;
		}
	}

	public static Process GetBotProcess()
	{
		return g_BotProcess;
	}

	public static bool IsBotRunning()
	{
		try
		{
			return g_BotProcess != null && !g_BotProcess.HasExited;
		}
		catch
		{
			return false;
		}
	}

	public static void StartBotProcess()
	{
		try
		{
			KillBotProcess();

			g_BotProcess = new Process();
			g_BotProcess.StartInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(), "netcoreapp3.1", Helpers.FormatString("DiscordBot{0}", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

#if !DEBUG || RUN_DISCORD_BOT_IN_DEBUG
			g_BotProcess.StartInfo.RedirectStandardOutput = true;
			g_BotProcess.StartInfo.RedirectStandardError = true;
			g_BotProcess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
			g_BotProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
			g_BotProcess.StartInfo.UseShellExecute = false;
#else
			g_BotProcess.StartInfo.UseShellExecute = true;
#endif
			g_BotProcess.Start();

#if !DEBUG || RUN_DISCORD_BOT_IN_DEBUG
			g_BotProcess.BeginOutputReadLine();
			g_BotProcess.BeginErrorReadLine();
#endif
		}
		catch
		{

		}
	}

	public DiscordBotIntegration()
	{
#if !DEBUG || RUN_DISCORD_BOT_IN_DEBUG
		StartBotProcess();

		RageEvents.RAGE_OnUpdate += Tick;

		g_HTTPServer = new CHTTPServer("DiscordBot", 9998);

		// TODO_GITHUB: You should set the environment variable below
		g_HTTPServer.SetAuthMethod_Token(System.Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? String.Empty);
		g_HTTPServer.RegisterHandler<CRequestHandler_BiDirectionalAdminChat>();
		g_HTTPServer.RegisterHandler<CRequestHandler_PushCommand>();

		// TODO_GITHUB: You should set the environment variable below
		g_RestClient = new CRestClient("127.0.0.1", 9999, System.Environment.GetEnvironmentVariable("HTTP_SERVER_PASS") ?? String.Empty);

		NetworkEvents.GotoDiscordLinking += OnGotoDiscordLinking;
		NetworkEvents.DiscordLinkFinalize += OnDiscordLinkingFinalize;
		NetworkEvents.DiscordDeLink += OnDiscordDeLink;

		UpdateBotStatus();
		MainThreadTimerPool.CreateGlobalTimer(UpdateBotStatus, 60000);

		MainThreadTimerPool.CreateGlobalTimer(CheckBotIsRunning, 60000);

		MainThreadTimerPool.CreateGlobalTimer((object[] parameters) =>
		{
			KillBotProcess();

			MainThreadTimerPool.CreateGlobalTimer((object[] parameters) =>
			{
				StartBotProcess();
			}, 5000, 1);

		}, 3600000);

#endif
	}

	private void CheckBotIsRunning(object[] parameters)
	{
		if (!IsBotRunning())
		{
			StartBotProcess();
		}
	}

	private void UpdateBotStatus(object[] parameters = null)
	{
		// TODO_GITHUB: You should replace the below with your own website/IP address
		JSONRequest_UpdateBotStatus request = new JSONRequest_UpdateBotStatus("v.website.com:5000");
		g_RestClient.QueueRequest(request, CRestClient.ERestCallbackThreadingMode.ContinueOnWorkerThread, null);
	}

	private void Tick()
	{
		if (g_RestClient != null)
		{
			g_RestClient.MainThreadTick();
		}
	}

	private void OnDiscordDeLink(CPlayer player)
	{
		player.SetDiscordID(0, true);
	}

	private async void OnDiscordLinkingFinalize(CPlayer player, string strURL)
	{
		string strState = null;
		string strAccessToken = null;
		string[] strComponents = strURL.Split("&");

		foreach (string strComponent in strComponents)
		{
			string[] kvSplit = strComponent.Split("=");
			if (kvSplit.Length == 2)
			{
				string strKey = kvSplit[0];
				string strValue = kvSplit[1];

				if (strKey == "state")
				{
					strState = strValue;
				}

				if (strKey == "access_token")
				{
					strAccessToken = strValue;
				}
			}
		}

		if (strState != null && strAccessToken != null)
		{
			// Does the state match?
			if (strState == player.DiscordOwlToken)
			{
				using (var client = new HttpClient())
				{
					client.DefaultRequestHeaders.Add("Authorization", "Bearer " + strAccessToken);
					string strResponse = await client.GetStringAsync(new Uri("https://discordapp.com/api/v6/users/@me")).ConfigureAwait(true);

					// parse out id
					string strID = String.Empty;
					JObject obj = JObject.FromObject(Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse));

					if (obj != null)
					{
						JToken jTok = obj.GetValue("id", StringComparison.OrdinalIgnoreCase);
						if (jTok != null)
						{
							strID = jTok.ToString();
						}
					}

					if (!String.IsNullOrEmpty(strID))
					{
						UInt64 ui64DiscordID = 0;
						if (UInt64.TryParse(strID, out ui64DiscordID))
						{
							// Check the discord ID isnt bound to another owl account
							bool bIsUnique = await Database.LegacyFunctions.IsDiscordAccountAlreadyLinkedToAnotherAccount(ui64DiscordID).ConfigureAwait(true);

							if (bIsUnique)
							{
								player.SetDiscordID(ui64DiscordID, true);

								player.GetDiscordUsername((bool bSuccess, string strDiscordUsername) =>
								{
									player.SendNotification("Discord", ENotificationIcon.InfoSign, "Your Owl account '{0}' is now linked to Discord account '{1}'.", player.Username, strDiscordUsername);
								});
							}
							else
							{
								player.SendNotification("Discord", ENotificationIcon.ExclamationSign, "Error linking Discord account - That Discord account is already linked to another Owl account");
							}
						}
						else
						{
							player.SendNotification("Discord", ENotificationIcon.ExclamationSign, "Error linking Discord account - Error Code: Alpha");
						}
					}
					else
					{
						player.SendNotification("Discord", ENotificationIcon.ExclamationSign, "Error linking Discord account - Error Code: Bravo");
					}
				}
			}
			else
			{
				player.SendNotification("Discord", ENotificationIcon.ExclamationSign, "Error linking Discord account - Error Code: Charlie");
			}
		}
		else
		{
			player.SendNotification("Discord", ENotificationIcon.InfoSign, "Account linking was canceled.");
		}

		player.DiscordOwlToken = String.Empty;
	}

	private void OnGotoDiscordLinking(CPlayer player)
	{
		ulong discordID;
		if (player.GetDiscordID(out discordID))
		{
			player.GetDiscordUsername((bool bSuccess, string strDiscordUsername) =>
			{
				if (bSuccess && strDiscordUsername.Length > 0)
				{
					NetworkEventSender.SendNetworkEvent_GotoDiscordLinking_Response(player, true, strDiscordUsername, "");
				}
				else
				{
					GotoFirstTimeLink();
				}
			});
		}
		else
		{
			GotoFirstTimeLink();
		}

		void GotoFirstTimeLink()
		{
			// TODO_GITHUB: You should set the environment variable below for your Discord bot client ID in debug and release
			string strClientID = VersionHelpers.IsTestServer() ? "TODO_GITHUB" : (Environment.GetEnvironmentVariable("DISCORD_CLIENT_ID") ?? "");

			string strDiscordOwlToken = HelperFunctions.Hashing.sha256(Helpers.FormatString("{0}_{1}", player.AccountID, DateTime.Now.ToString())).Substring(0, 24);
			player.DiscordOwlToken = strDiscordOwlToken;

			// TODO_GITHUB: You should replace the below with your own website
			string strURL = Helpers.FormatString("https://discordapp.com/api/oauth2/authorize?response_type=token&client_id={0}&state={1}&scope=identify&redirect_uri=https://www.website.com", strClientID, player.DiscordOwlToken);

			// useful if you need to sign out to test
			//strURL = "https://discordapp.com/channels/@me";

			NetworkEventSender.SendNetworkEvent_GotoDiscordLinking_Response(player, false, "", strURL);
		}
	}

	public static void GetDiscordUsernameFromID(ulong DiscordID, Action<bool, string> CompletionCallback)
	{
		DiscordBotIntegration.GetRestClient()?.QueueRequest(new JSONRequest_GetDiscordUsernameFromID(DiscordID), CRestClient.ERestCallbackThreadingMode.ContinueOnMainThread, (string strJsonResponse) =>
		{
			JSONResponse_GetDiscordUsernameFromID resp = Newtonsoft.Json.JsonConvert.DeserializeObject<JSONResponse_GetDiscordUsernameFromID>(strJsonResponse);
			CompletionCallback(resp.success, resp.DiscordUsername);
		});

	}

	public static void PushMessage(DiscordUser user, EDiscordChannelIDs channelToUse, string strFormat, params object[] strParams)
	{
		if (channelToUse == 0)
		{
			PushDM(user, strFormat, strParams);
		}
		else
		{
			PushChannelMessage(channelToUse, strFormat, strParams);
		}
	}

	public static void PushChannelMessage(EDiscordChannelIDs channelToUse, string strFormat, params object[] strParams)
	{
		DiscordBotIntegration.GetRestClient()?.QueueRequest(new JSONRequest_PushRawChannelMessage(channelToUse, Helpers.FormatString(strFormat, strParams)), CRestClient.ERestCallbackThreadingMode.ContinueOnWorkerThread, null);
	}

	public static void PushDM(DiscordUser user, string strFormat, params object[] strParams)
	{
		DiscordBotIntegration.GetRestClient()?.QueueRequest(new JSONRequest_PushDM(user, Helpers.FormatString(strFormat, strParams)), CRestClient.ERestCallbackThreadingMode.ContinueOnWorkerThread, null);
	}
}

// COMMAND SUPPORT
public static class DiscordCommandManager
{
	private static Dictionary<string, DiscordCommandDescriptor> m_dictCommandDescriptors = new Dictionary<string, DiscordCommandDescriptor>();

	private class DiscordCommandDescriptor
	{
		public DiscordCommandDescriptor(string cmd, string desc, DiscordCommandParsingFlags parsingFlags, Delegate function, EDiscordChannelIDs requiredChannel, EDiscordAuthRequirements discordAuthRequirements, EAdminLevel minAdminLevel, string[] aliases, string overrideSyntax)
		{
			CommandName = cmd.ToLower();
			Description = desc;
			ParsingFlags = parsingFlags;
			Function = function;
			Aliases = (aliases != null) ? Array.ConvertAll(aliases, d => d.ToLower()) : Array.Empty<string>();
			OverrideSyntax = overrideSyntax;
			MinAdminLevel = minAdminLevel;
			RequiredChannel = requiredChannel;
			DiscordAuthRequirements = discordAuthRequirements;
		}

		public string CommandName { get; set; }
		public string Description { get; set; }
		public DiscordCommandParsingFlags ParsingFlags { get; set; }
		public Delegate Function { get; set; }
		public string[] Aliases { get; set; }
		public string OverrideSyntax { get; set; }
		public EDiscordChannelIDs RequiredChannel { get; set; }
		public EDiscordAuthRequirements DiscordAuthRequirements { get; set; }
		public EAdminLevel MinAdminLevel { get; set; }

		internal bool Matches(string cmd)
		{
			return (CommandName == cmd.ToLower() || Aliases.Contains(cmd.ToLower()));
		}
	}

	private static string GenerateSyntaxFromDescriptor(DiscordCommandDescriptor descriptor, bool bPrefixWithCommandName)
	{
		string strSyntax = bPrefixWithCommandName ? Helpers.FormatString("/{0} ", descriptor.CommandName) : "";

		if (descriptor.OverrideSyntax == null)
		{
			var funcParams = descriptor.Function.Method.GetParameters();
			for (int i = 3; i < funcParams.Length; ++i)
			{
				strSyntax += Helpers.FormatString("[{0}] ", System.Text.RegularExpressions.Regex.Replace(funcParams[i].Name, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
			}
		}
		else
		{
			strSyntax += descriptor.OverrideSyntax;
		}

		return strSyntax;
	}

	private static void ShowSyntax(DiscordUser discordUser, string cmd, EDiscordChannelIDs enumChannelID)
	{
		DiscordCommandDescriptor descriptor = GetCommandDescriptor(cmd);
		string strSyntax = GenerateSyntaxFromDescriptor(descriptor, true);

		DiscordBotIntegration.PushMessage(discordUser, enumChannelID, Helpers.FormatString("Command {0}: {1}", cmd, descriptor.Description));
		DiscordBotIntegration.PushMessage(discordUser, enumChannelID, Helpers.FormatString("Syntax: {0}", strSyntax));
	}

	private static DiscordCommandDescriptor GetCommandDescriptor(string cmd)
	{
		foreach (DiscordCommandDescriptor desc in m_dictCommandDescriptors.Values)
		{
			if (desc.Matches(cmd))
			{
				return desc;
			}
		}

		return null;
	}

	public static void RegisterCommand(string cmd, string desc, Delegate dele, DiscordCommandParsingFlags parsingFlags, EDiscordChannelIDs requiredChannel, EDiscordAuthRequirements discordAuthRequirements, EAdminLevel minAdminLevel = EAdminLevel.None, string[] aliases = null, string overrideSyntax = null)
	{
		// SAFETY: Force linked account check if an admin level was defined, incase someone accidentally misdefines
		if (minAdminLevel > EAdminLevel.None)
		{
			discordAuthRequirements = EDiscordAuthRequirements.RequiresLinkedAccount;
		}

		DiscordCommandDescriptor descriptor = GetCommandDescriptor(cmd);
		if (descriptor == null)
		{
			m_dictCommandDescriptors.Add(cmd, new DiscordCommandDescriptor(cmd, desc, parsingFlags, dele, requiredChannel, discordAuthRequirements, minAdminLevel, aliases, overrideSyntax));
		}
		else
		{
			throw new Exception(Helpers.FormatString("COMMAND ERROR: {0} is a duplicate.", cmd));
		}
	}

	public static async void OnRawCommand(DiscordUser discordUser, string msg, EDiscordChannelIDs enumChannelID)
	{
		string[] exploded = msg.Split(" ");

		if (exploded.Length == 0)
		{
			DiscordBotIntegration.PushDM(discordUser, Helpers.FormatString("Command Error: {0}", msg));
			return;
		}

		BasicAccountInfo accountInfo = await Database.LegacyFunctions.GetBasicAccountInfoFromDiscordID(discordUser.ID).ConfigureAwait(true);

		string cmd = exploded[0].Substring(1);

		object[] args = exploded.Length > 1 ? exploded.Skip(1).ToArray() : Array.Empty<object>();

		DiscordCommandDescriptor descriptor = DiscordCommandManager.GetCommandDescriptor(cmd);
		if (descriptor != null)
		{
			bool bAllowGuests = (descriptor.DiscordAuthRequirements == EDiscordAuthRequirements.GuestsAllowed);
			if (bAllowGuests || accountInfo.Result == BasicAccountInfo.EGetAccountInfoResult.OK)
			{
				if (bAllowGuests || accountInfo.AdminLevel >= descriptor.MinAdminLevel)
				{
					// Does the descriptor only allow response on a specific channel (or DM)

					if (descriptor.RequiredChannel != EDiscordChannelIDs.Any)
					{
						// reply in DM if it came in in DM, otherwise use the preferred channel
						if (enumChannelID != EDiscordChannelIDs.DirectMessage)
						{
							enumChannelID = descriptor.RequiredChannel;
						}
					}

					// PARSE PARAMETERS
					DiscordCommandParsingFlags parsingFlag = descriptor.ParsingFlags;
					if (parsingFlag == DiscordCommandParsingFlags.GreedyArgs)
					{
						if (descriptor.Function.Method.GetParameters().Length == 4)
						{
							try
							{
								string strMessage = String.Join(" ", args);
								if (strMessage.Length > 0)
								{
									descriptor.Function.DynamicInvoke(discordUser, accountInfo, enumChannelID, strMessage);
								}
								else
								{
									ShowSyntax(discordUser, cmd, enumChannelID);
								}
							}
							catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
							{
								DiscordBotIntegration.PushDM(discordUser, Helpers.FormatString("Command {0}: Error occured. Developers have been informed!", cmd));
								throw new Exception(Helpers.FormatString("COMMAND ERROR #2: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
							}
						}
						else
						{
							DiscordBotIntegration.PushDM(discordUser, Helpers.FormatString("Command {0}: Error occured. Developers have been informed!", cmd));
							throw new Exception(Helpers.FormatString("COMMAND ERROR: {0} is marked as greedy arg, but target method {1} doesn't have correct number of parameters or incorrect types", cmd, descriptor.Function.Method.Name));
						}
					}
					else if (parsingFlag == DiscordCommandParsingFlags.Default)
					{
						if (descriptor.Function.Method.GetParameters().Length == (args.Length + 3))
						{
							try
							{
								args = args.Prepend(enumChannelID).ToArray().Prepend(accountInfo).ToArray().Prepend(discordUser).ToArray();

								// cast all args
								var funcParams = descriptor.Function.Method.GetParameters();
								for (int i = 3; i < args.Length; ++i)
								{
									if (funcParams[i].ParameterType.IsEnum)
									{
										try
										{
											args[i] = Enum.Parse(funcParams[i].ParameterType, args[i].ToString());
										}
										catch
										{
											ShowSyntax(discordUser, cmd, enumChannelID);
											return;
										}
									}
									else
									{
										// If we can't cast, it's not really an actionable exception for us, it means the user tried to provide an invalid type
										try
										{
											args[i] = Convert.ChangeType(args[i], funcParams[i].ParameterType);
										}
										catch
										{
											ShowSyntax(discordUser, cmd, enumChannelID);
											return;
										}
									}
								}

								try
								{
									descriptor.Function.DynamicInvoke(args);
								}
								catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
								{
									DiscordBotIntegration.PushDM(discordUser, Helpers.FormatString("Command {0}: Error occured. Developers have been informed!", cmd));
									throw new Exception(Helpers.FormatString("COMMAND ERROR #11: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
								}
							}
							catch
							{
								ShowSyntax(discordUser, cmd, enumChannelID);
							}
						}
						else
						{
							ShowSyntax(discordUser, cmd, enumChannelID);
						}
					}
				}
			}
			else if (!bAllowGuests && descriptor.MinAdminLevel == EAdminLevel.None) // only show the 'must link account' message if we failed due to being a guest rather than not having admin perms, we dont expose admin cmds
			{
				DiscordBotIntegration.PushDM(discordUser, "You must link your Discord account to your Owl account to perform this action. When in-game, hit the up arrow on the HUD and select 'Discord Linking'.");
			}
		}
		else
		{
			DiscordBotIntegration.PushDM(discordUser, "Unknown Command");
		}

	}
}