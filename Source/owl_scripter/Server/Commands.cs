using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;
using ScripterCommands.RuntimeScripting;


namespace ScripterCommands
{
	public class ScripterCommands : GTANetworkAPI.Script
	{
		public ScripterCommands()
		{
			// Important *dangerous* commands
			CommandManager.RegisterCommand("eval", "Evaluates, Compiles and Runs C# code", new Action<CPlayer, CVehicle, string>(EvalCommand), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeScripter);

			// Misc/Util commands
			CommandManager.RegisterCommand("sc", "Sends a message in scripter chat", new Action<CPlayer, CVehicle, string>(OnScripterChatMessage), CommandParsingFlags.GreedyArgs, CommandRequirementsFlags.MustBeUaOrScripter);
			CommandManager.RegisterCommand("scripterveh", "Creates a temporary vehicle", new Action<CPlayer, CVehicle, int>(ScripterTempVehicleCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeScripter);

			// ClientSide debug commands
			CommandManager.RegisterCommand("tcd", "Toggle a variety of clientside debug options", new Action<CPlayer, CVehicle, EClientsideDebugOption>(ToggleClientSideDebug), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeUaOrScripter);
		}

		private void ToggleClientSideDebug(CPlayer player, CVehicle vehicle, EClientsideDebugOption debugOption)
		{
			// We check this here so we can skip checking on the clientside.
			if (!Enum.IsDefined(typeof(EClientsideDebugOption), debugOption))
			{
				player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Incorrect option provided");
				return;
			}

			NetworkEventSender.SendNetworkEvent_ToggleClientSideDebugOption(player, debugOption);
		}

		private void OnScripterChatMessage(CPlayer sourcePlayer, CVehicle vehicle, string strMessage)
		{
			if (strMessage.Contains("&#"))
			{
				sourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You can't use the &# combination in a message");
				return;
			}

			ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
			Logging.Log log = new Logging.Log(sourcePlayer, Logging.ELogType.LeadAdminChat, null, Helpers.FormatString(strMessage));

			string newMessage = EmojiManager.TryAndParseEmoji(strMessage);

			foreach (var player in players)
			{
				if (player.IsAdmin(EAdminLevel.LeadAdmin) || player.IsScripter())
				{
					player.PushChatMessageWithColorAndPlayerName(EChatChannel.AdminChat, 255, 165, 0, Helpers.FormatString("[SCRIPTER] {0} ({1})", sourcePlayer.GetCharacterName(ENameType.StaticCharacterName), sourcePlayer.Username), "{0}", newMessage);
					log.addAffected(player);
				}
			}
			log.execute();
		}

		private void EvalCommand(CPlayer player, CVehicle vehicle, string strInput)
		{
			if (!player.IsScripter(EScripterLevel.Scripter))
			{
				return;
			}

			if (g_lstBannedTerms.Any(strInput.ToLower().Contains))
			{
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Your script contains an illegal term. Try again.");
				return;
			}

			Globals globals = new Globals { P = player, V = vehicle };
			ScriptOptions options = ScriptOptions.Default.WithImports("GTANetworkAPI")
				.WithReferences(Assembly.GetAssembly(typeof(GTANetworkAPI.Vehicle)), Assembly.GetAssembly(typeof(CPlayer)), Assembly.GetAssembly(typeof(CBaseEntity)));

			player.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Compiling...");

			EvalScript.CreateScriptAsync(strInput.Replace('\'', '\"'), globals, options).ContinueOnMainThread(
				(s) =>
				{
					try
					{
						var result = s.RunAsync(globals);
						player.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, Helpers.FormatString("{0}", result.Result.ReturnValue != null ? result.Result.ReturnValue.ToString() : "Executed!"));
					}
					catch (Exception e)
					{
						player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, e.Message);
					}
				});
		}

		private void ScripterTempVehicleCommand(CPlayer player, CVehicle vehicle, int index)
		{
			PlayerAdminCommands.Vehicles.TemporaryVehicle(player, vehicle, index.ToString());
		}

		// TODO_RUNTIME_SCRIPTING: Get terms from context?
		// WARNING: DON'T REMOVE ANYTHING FROM THIS LIST WITHOUT DISCUSSING ACCORDINGLY
		// START BANNED TERM LIST
		private static readonly List<string> g_lstBannedTerms = new List<string>
		{
			// Any term can be added here. Make sure to add it with lowercase.
			// We ban this term too so the list itself can't be modified.
			"g_lstbannedterms",
			// To prevent directly importing other namespaces (we already can't access any binaries that aren't referenced to this project anyway)
			"using",
			// To prevent using Environment.Exit();
			"environment",
			// To prevent threading shit:
			"task",
			"async",
			"await",
			"thread",
			"worker",
		};
		// END BANNED TERM LIST
	}
}