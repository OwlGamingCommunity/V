using System;
using System.Collections.Generic;
using System.Linq;

// TODO_CSHARP: Show all these commands on help ui... we have enough info to do so or have a /help <cmdname>
[Flags]
public enum CommandRequirementsFlags
{
	Default = 0x0,
	MustBeInVehicle = 0x1, // Setting this means your function will only be called if a valid vehicle is found, otherwise a generic error will be shown
	MustBeInVehicleWithOwnerOrAdminRights = 0x2, // Setting this means your function will only be called if a valid vehicle is found AND the player has owner (or admin) rights to the vehicle, otherwise a generic error will be shown
	MustBeInVehicleWithKeys = 0x4,
	MustBeAdminIgnoreDuty = 0x10,
	MustBeAdminOnDuty = 0x20,
	MustBeScripter = 0x40,
	MustBeUaOrScripter = 0x80,
}

public enum CommandParsingFlags
{
	Default = 0,
	GreedyArgs = 1,
	TargetPlayerAndGreedyArgs = 2,
	TargetPlayer = 3,
	TwoTargetPlayers = 4,
	TargetPlayerAndGreedyArgRemainingStrings = 5,
	TargetFunctionAndGreedyArgsRemainder = 6
}

public static class CommandManager
{
	public static string g_strLastCommand = String.Empty;
	private static List<string> g_strBlockedCommands = new List<string>();

	static CommandManager()
	{
		NetworkEvents.PlayerRawCommand += OnPlayerRawCommand;
		NetworkEvents.HelpRequestCommands += OnRequestCommandList;
	}

	private static void OnRequestCommandList(CPlayer a_Player)
	{
		List<CommandHelpInfo> lstTransmitCommands = new List<CommandHelpInfo>();
		foreach (var kvPair in m_dictCommandDescriptors)
		{
			if (!kvPair.Value.IsAnimation)
			{
				string strCmd = kvPair.Key;
				CommandDescriptor descriptor = kvPair.Value;

				List<string> lstRequirements = new List<string>();
				if (a_Player.IsAdmin() || (!descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminIgnoreDuty) && !descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminOnDuty)))
				{
					if (descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicle))
					{
						lstRequirements.Add("In Vehicle");
					}
					else if (descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicleWithOwnerOrAdminRights))
					{
						lstRequirements.Add("In Player Owned Vehicle");
					}
					else if (descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicleWithKeys))
					{
						lstRequirements.Add("In Vehicle with Keys");
					}
					else if (descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminIgnoreDuty))
					{
						lstRequirements.Add("Admin");
					}
					else if (descriptor.RequirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminOnDuty))
					{
						lstRequirements.Add("On Duty Admin");
					}

					if (lstRequirements.Count == 0)
					{
						lstRequirements.Add("N/A");
					}

					string strSyntax = GenerateSyntaxFromDescriptor(descriptor, false);
					lstTransmitCommands.Add(new CommandHelpInfo(strCmd, descriptor.Description, String.Join(", ", lstRequirements), strSyntax));
				}
			}
		}

		NetworkEventSender.SendNetworkEvent_HelpRequestCommandsResponse(a_Player, lstTransmitCommands);
	}

	private class CommandDescriptor
	{
		public CommandDescriptor(string cmd, string desc, CommandParsingFlags parsingFlags, CommandRequirementsFlags requirementFlags, Delegate function, string[] aliases, string overrideSyntax, bool isAnimation, EAnimCategory animCategory)
		{
			CommandName = cmd.ToLower();
			Description = desc;
			RequirementFlags = requirementFlags;
			ParsingFlags = parsingFlags;
			Function = function;
			Aliases = (aliases != null) ? Array.ConvertAll(aliases, d => d.ToLower()) : Array.Empty<string>();
			OverrideSyntax = overrideSyntax;
			IsAnimation = isAnimation;
			AnimCategory = animCategory;
		}

		public string CommandName { get; set; }
		public string Description { get; set; }
		public CommandRequirementsFlags RequirementFlags { get; set; }
		public CommandParsingFlags ParsingFlags { get; set; }
		public Delegate Function { get; set; }
		public string[] Aliases { get; set; }
		public string OverrideSyntax { get; set; }
		public bool IsAnimation { get; set; }
		public EAnimCategory AnimCategory { get; set; }

		internal bool Matches(string cmd)
		{
			return (CommandName == cmd.ToLower() || Aliases.Contains(cmd.ToLower()));
		}
	}

	private static string GenerateSyntaxFromDescriptor(CommandDescriptor descriptor, bool bPrefixWithCommandName)
	{
		string strSyntax = bPrefixWithCommandName ? Helpers.FormatString("/{0} ", descriptor.CommandName) : "";

		if (descriptor.OverrideSyntax == null)
		{
			var funcParams = descriptor.Function.Method.GetParameters();
			for (int i = 2; i < funcParams.Length; ++i)
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

	private static void ShowSyntax(CPlayer a_Player, string cmd)
	{
		CommandDescriptor descriptor = GetCommandDescriptor(cmd);
		string strSyntax = GenerateSyntaxFromDescriptor(descriptor, true);

		a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: {1}", cmd, descriptor.Description);
		a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Syntax: {0}", strSyntax);
	}

	private static CommandDescriptor GetCommandDescriptor(string cmd)
	{
		foreach (CommandDescriptor desc in m_dictCommandDescriptors.Values)
		{
			if (desc.Matches(cmd))
			{
				return desc;
			}
		}

		return null;
	}

	public static void TriggerCommandManual(CPlayer SenderPlayer, string msg)
	{
		if (SenderPlayer != null)
		{
			OnPlayerRawCommand(SenderPlayer, msg);
		}
	}

	public static WeakReference<CPlayer> FindTargetPlayer(CPlayer Source, string target)
	{
		if (target.Equals("*"))
		{
			return new WeakReference<CPlayer>(Source);
		}
		else
		{
			return PlayerPool.GetPlayerFromPartialNameOrID(target);
		}
	}

	public static List<CAnimationCommand> GetAnimationCommands()
	{
		List<CAnimationCommand> lstAnimationCommands = new List<CAnimationCommand>();

		foreach (CommandDescriptor command in m_dictCommandDescriptors.Values)
		{
			if (command.IsAnimation)
			{
				bool hasArguments = command.Function.Method.GetParameters().Length > 2 ? true : false;
				lstAnimationCommands.Add(new CAnimationCommand(command.CommandName, command.Description, command.AnimCategory, hasArguments));
			}
		}

		return lstAnimationCommands;
	}

	public static void OnPlayerRawCommand(CPlayer a_Player, string msg)
	{
		g_strLastCommand = msg;

		if (!a_Player.IsLoggedIn || !a_Player.IsSpawned)
		{
			return;
		}

		string[] exploded = msg.Split(" ");

		if (exploded.Length == 0)
		{
			a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command Error: {0}", msg);
			return;
		}

		string cmd = exploded[0];

		object[] args = exploded.Length > 1 ? exploded.Skip(1).ToArray() : Array.Empty<object>();

		// is it a custom anim?
		Database.Models.CustomAnim customAnim = a_Player.GetCustomAnim(cmd);
		if (customAnim != null)
		{
			AnimationFlags animFlags = 0;

			if (customAnim.Loop)
			{
				animFlags |= AnimationFlags.Loop;
			}

			if (customAnim.StopOnLastFrame)
			{
				animFlags |= AnimationFlags.StopOnLastFrame;
			}
			if (customAnim.OnlyAnimateUpperBody)
			{
				animFlags |= AnimationFlags.OnlyAnimateUpperBody;
			}
			if (customAnim.AllowPlayerMovement)
			{
				animFlags |= AnimationFlags.AllowPlayerControl;
			}

			a_Player.AddAnimationToQueue((int)animFlags, customAnim.AnimDictionary, customAnim.AnimName, true, true, true, customAnim.Duration * 1000, true);
			return;
		}

		CommandDescriptor descriptor = GetCommandDescriptor(cmd);
		if (descriptor != null)
		{
			if (g_strBlockedCommands.Contains(cmd))
			{
				a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "This command is temporarily disabled. If you think this shouldn't happen please report to and Administrator via the F2 report panel");
				return;
			}

			CommandParsingFlags parsingFlag = descriptor.ParsingFlags;
			CommandRequirementsFlags requirementFlags = descriptor.RequirementFlags;

			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle); // Can be null, unless vehicle requirement flags are set

			// BEGIN CHECK FLAGS
			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicle))
			{
				if (!a_Player.IsInVehicleReal)
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: You must be in a vehicle to use this command", cmd);
					return;
				}
				else
				{
					// Already set above
				}
			}

			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicleWithOwnerOrAdminRights))
			{
				if (!a_Player.IsInVehicleReal)
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: You must be in a vehicle to use this command", cmd);
					return;
				}
				else
				{
					if (pVehicle != null)
					{
						bool bHasFactionManagerForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(a_Player, true);

						if (!bHasFactionManagerForVehicle && !pVehicle.OwnedBy(a_Player) && !a_Player.IsAdmin())
						{
							a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: You must be in a vehicle which you have personal or faction ownership of to use this command", cmd);
							return;
						}
					}
				}
			}

			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeInVehicleWithKeys))
			{
				if (!a_Player.IsInVehicleReal)
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: You must be in a vehicle to use this command", cmd);
					return;
				}
				else
				{
					if (pVehicle != null)
					{
						bool bHasFactionManagerForVehicle = pVehicle.IsVehicleForAnyPlayerFaction(a_Player, true);

						if (!bHasFactionManagerForVehicle && !pVehicle.HasKeys(a_Player))
						{
							a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: You must be in a vehicle which you have the keys to in order to use this command", cmd);
							return;
						}
					}
				}
			}

			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminOnDuty) || requirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminIgnoreDuty))
			{
				bool bCheckDuty = requirementFlags.HasFlag(CommandRequirementsFlags.MustBeAdminOnDuty);
				if (!a_Player.IsAdmin(EAdminLevel.TrialAdmin, bCheckDuty) && !a_Player.IsAdmin(EAdminLevel.LeadAdmin))
				{
					// If admin, but not on duty
					if (a_Player.IsAdmin(EAdminLevel.TrialAdmin, false) && !a_Player.AdminDuty)
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "COMMAND: {0} requires you to be on admin duty (/aduty)", cmd);
					}
					else
					{
						// NOTHING, Don't expose our admin commands, just pretend it was an unknown cmd
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "UNKNOWN COMMAND: {0}", cmd);
					}
					return;
				}
			}

			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeScripter))
			{
				if (!a_Player.IsScripter())
				{
					// NOTHING, Don't expose our scripter commands, just pretend it was an unknown cmd
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "UNKNOWN COMMAND: {0}", cmd);
					return;
				}
			}

			if (requirementFlags.HasFlag(CommandRequirementsFlags.MustBeUaOrScripter))
			{
				if (!a_Player.IsAdmin(EAdminLevel.LeadAdmin) && !a_Player.IsScripter())
				{
					// NOTHING, Don't expose our scripter commands, just pretend it was an unknown cmd
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "UNKNOWN COMMAND: {0}", cmd);
					return;
				}
			}

			// END CHECK FLAGS

			// CHECK ARGUMENT PARSING FLAGS
			if (parsingFlag == CommandParsingFlags.TargetPlayerAndGreedyArgs)
			{
				if (descriptor.Function.Method.GetParameters().Length == 4)
				{
					try
					{
						if (args.Length >= 2)
						{
							string IDorPartialNickname = ((string)args[0]).Replace('_', ' ');
							string strRemainder = String.Join(" ", args.Skip(1).ToArray());

							if (strRemainder.Length > 0)
							{
								WeakReference<CPlayer> TargetPlayerRef = FindTargetPlayer(a_Player, IDorPartialNickname);
								CPlayer TargetPlayer = TargetPlayerRef.Instance();

								if (TargetPlayer != null)
								{
									descriptor.Function.DynamicInvoke(a_Player, pVehicle, TargetPlayer, strRemainder);
								}
								else
								{
									// TODO_CHAT: Should be a notification
									a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Player '{1}' not found or found multiple.", cmd, IDorPartialNickname);
								}
							}
							else
							{
								ShowSyntax(a_Player, cmd);
							}
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}

					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #1: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
					throw new Exception(Helpers.FormatString("COMMAND ERROR: {0} is marked as target player & greedy arg, but target method {1} doesn't have correct number of parameters or incorrect types", cmd, descriptor.Function.Method.Name));
				}
			}
			else if (parsingFlag == CommandParsingFlags.GreedyArgs)
			{
				if (descriptor.Function.Method.GetParameters().Length == 3)
				{
					try
					{
						string strMessage = String.Join(" ", args);
						if (strMessage.Length > 0)
						{
							descriptor.Function.DynamicInvoke(a_Player, pVehicle, strMessage);
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}
					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #2: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
					throw new Exception(Helpers.FormatString("COMMAND ERROR: {0} is marked as greedy arg, but target method {1} doesn't have correct number of parameters or incorrect types", cmd, descriptor.Function.Method.Name));
				}
			}
			else if (parsingFlag == CommandParsingFlags.TargetPlayer)
			{
				if (descriptor.Function.Method.GetParameters().Length == (args.Length + 2))
				{
					try
					{
						if (args.Length >= 1)
						{
							string IDorPartialNickname = ((string)args[0]).Replace('_', ' ');

							WeakReference<CPlayer> TargetPlayerRef = FindTargetPlayer(a_Player, IDorPartialNickname);
							CPlayer TargetPlayer = TargetPlayerRef.Instance();
							if (TargetPlayer != null)
							{
								args = args.Skip(1).ToArray().Prepend(TargetPlayer).ToArray().Prepend(pVehicle).ToArray().Prepend(a_Player).ToArray();

								// cast all args
								var funcParams = descriptor.Function.Method.GetParameters();
								for (int i = 2; i < args.Length; ++i)
								{
									if (funcParams[i].ParameterType.IsEnum)
									{
										try
										{
											args[i] = Enum.Parse(funcParams[i].ParameterType, args[i].ToString());
										}
										catch
										{
											ShowSyntax(a_Player, cmd);
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
											ShowSyntax(a_Player, cmd);
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
									a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
									throw new Exception(Helpers.FormatString("COMMAND ERROR #3: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
								}
							}
							else
							{
								// TODO_CHAT: Should be a notification
								a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Player '{1}' not found.", cmd, IDorPartialNickname);
							}
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}

					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured.", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #4: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					ShowSyntax(a_Player, cmd);
				}
			}
			else if (parsingFlag == CommandParsingFlags.TwoTargetPlayers)
			{
				if (descriptor.Function.Method.GetParameters().Length == (args.Length + 2))
				{
					try
					{
						if (args.Length >= 2)
						{
							string IDorPartialNickname = ((string)args[0]).Replace('_', ' ');
							string IDorPartialNicknameSecond = ((string)args[1]).Replace('_', ' ');

							WeakReference<CPlayer> TargetPlayerRef = FindTargetPlayer(a_Player, IDorPartialNickname);
							CPlayer TargetPlayer = TargetPlayerRef.Instance();
							if (TargetPlayer != null)
							{
								WeakReference<CPlayer> TargetPlayerSecondRef = FindTargetPlayer(a_Player, IDorPartialNicknameSecond);
								CPlayer TargetPlayerSecond = TargetPlayerSecondRef.Instance();
								if (TargetPlayerSecond != null)
								{
									args = args.Skip(2).ToArray().Prepend(TargetPlayerSecond).ToArray().Prepend(TargetPlayer).ToArray().Prepend(pVehicle).ToArray().Prepend(a_Player).ToArray();

									// cast all args
									var funcParams = descriptor.Function.Method.GetParameters();
									for (int i = 2; i < args.Length; ++i)
									{
										if (funcParams[i].ParameterType.IsEnum)
										{
											try
											{
												args[i] = Enum.Parse(funcParams[i].ParameterType, args[i].ToString());
											}
											catch
											{
												ShowSyntax(a_Player, cmd);
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
												ShowSyntax(a_Player, cmd);
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
										a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
										throw new Exception(Helpers.FormatString("COMMAND ERROR #5: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
									}
								}
								else
								{
									// TODO_CHAT: Should be a notification
									a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Player '{1}' not found.", cmd, IDorPartialNicknameSecond);
								}
							}
							else
							{
								// TODO_CHAT: Should be a notification
								a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Player '{1}' not found.", cmd, IDorPartialNickname);
							}
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}

					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #6: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					ShowSyntax(a_Player, cmd);
				}
			}
			else if (parsingFlag == CommandParsingFlags.TargetPlayerAndGreedyArgRemainingStrings)
			{
				if (descriptor.Function.Method.GetParameters().Length <= (args.Length + 2))
				{
					try
					{
						if (args.Length >= 1)
						{
							string IDorPartialNickname = ((string)args[0]).Replace('_', ' ');

							WeakReference<CPlayer> TargetPlayerRef = FindTargetPlayer(a_Player, IDorPartialNickname);
							CPlayer TargetPlayer = TargetPlayerRef.Instance();
							if (TargetPlayer != null)
							{
								List<object> trueArguments = new List<object>();
								//args = args.Skip(1).ToArray().Prepend(TargetPlayer).ToArray().Prepend(pVehicle).ToArray().Prepend(a_Player).ToArray();
								trueArguments.Add(a_Player);
								trueArguments.Add(pVehicle);
								trueArguments.Add(TargetPlayer);
								//trueArguments.AddRange(args.Skip(1).ToArray().Prepend().ToArray().Prepend(pVehicle).ToArray().Prepend(a_Player).ToArray());

								// cast all args
								var funcParams = descriptor.Function.Method.GetParameters();

								for (int paramIndex = 3; paramIndex < funcParams.Length; ++paramIndex)
								{
									int i = paramIndex - 2;
									if (funcParams[paramIndex].ParameterType.IsEnum)
									{
										try
										{
											object enumVal = null;
											if (Enum.TryParse(funcParams[paramIndex].ParameterType, args[i].ToString(), out enumVal))
											{
												if (enumVal != null)
												{
													trueArguments.Add(enumVal);
												}
												else
												{
													ShowSyntax(a_Player, cmd);
													return;
												}
											}
											else
											{
												ShowSyntax(a_Player, cmd);
												return;
											}
										}
										catch
										{
											ShowSyntax(a_Player, cmd);
											return;
										}
									}
									else
									{
										// last one? concat all
										if (paramIndex == funcParams.Length - 1)
										{
											trueArguments.Add(String.Join(" ", args.Skip(i).ToArray()));
										}
										else
										{
											// If we can't cast, it's not really an actionable exception for us, it means the user tried to provide an invalid type
											try
											{
												trueArguments.Add(Convert.ChangeType(args[i], funcParams[paramIndex].ParameterType));
											}
											catch
											{
												ShowSyntax(a_Player, cmd);
												return;
											}
										}
									}
								}

								try
								{
									descriptor.Function.DynamicInvoke(trueArguments.ToArray());
								}
								catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
								{
									a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
									throw new Exception(Helpers.FormatString("COMMAND ERROR #7: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
								}
							}
							else
							{
								// TODO_CHAT: Should be a notification
								a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Player '{1}' not found.", cmd, IDorPartialNickname);
							}
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}

					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured.", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #8: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					ShowSyntax(a_Player, cmd);
				}
			}
			else if (parsingFlag == CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder)
			{
				if (descriptor.Function.Method.GetParameters().Length <= (args.Length + 2))
				{
					try
					{
						if (args.Length >= 1)
						{
							List<object> trueArguments = new List<object>();
							trueArguments.Add(a_Player);
							trueArguments.Add(pVehicle);

							// cast all args
							var funcParams = descriptor.Function.Method.GetParameters();

							for (int paramIndex = 2; paramIndex < funcParams.Length; ++paramIndex)
							{
								int i = paramIndex - 2;
								if (funcParams[paramIndex].ParameterType.IsEnum)
								{
									try
									{
										object enumVal = null;
										if (Enum.TryParse(funcParams[paramIndex].ParameterType, args[i].ToString(), out enumVal))
										{
											if (enumVal != null)
											{
												trueArguments.Add(enumVal);
											}
											else
											{
												ShowSyntax(a_Player, cmd);
												return;
											}
										}
										else
										{
											ShowSyntax(a_Player, cmd);
											return;
										}
									}
									catch
									{
										ShowSyntax(a_Player, cmd);
										return;
									}
								}
								else
								{
									// last one? concat all
									if (paramIndex == funcParams.Length - 1)
									{
										trueArguments.Add(String.Join(" ", args.Skip(i).ToArray()));
									}
									else
									{
										// If we can't cast, it's not really an actionable exception for us, it means the user tried to provide an invalid type
										try
										{
											trueArguments.Add(Convert.ChangeType(args[i], funcParams[paramIndex].ParameterType));
										}
										catch
										{
											ShowSyntax(a_Player, cmd);
											return;
										}
									}
								}
							}

							try
							{
								descriptor.Function.DynamicInvoke(trueArguments.ToArray());
							}
							catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
							{
								a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
								throw new Exception(Helpers.FormatString("COMMAND ERROR #9: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
							}
						}
						else
						{
							ShowSyntax(a_Player, cmd);
						}

					}
					catch (Exception ex) // the actual command func threw an exception, let's not bubble this up and show syntax errors...
					{
						a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured.", cmd);
						throw new Exception(Helpers.FormatString("COMMAND ERROR #10: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
					}
				}
				else
				{
					ShowSyntax(a_Player, cmd);
				}
			}
			else
			{
				if (descriptor.Function.Method.GetParameters().Length == (args.Length + 2))
				{
					try
					{
						args = args.Prepend(pVehicle).ToArray().Prepend(a_Player).ToArray();

						// cast all args
						var funcParams = descriptor.Function.Method.GetParameters();
						for (int i = 2; i < args.Length; ++i)
						{
							if (funcParams[i].ParameterType.IsEnum)
							{
								try
								{
									args[i] = Enum.Parse(funcParams[i].ParameterType, args[i].ToString());
								}
								catch
								{
									ShowSyntax(a_Player, cmd);
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
									ShowSyntax(a_Player, cmd);
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
							a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "Command {0}: Error occured. Developers have been informed!", cmd);
							throw new Exception(Helpers.FormatString("COMMAND ERROR #11: {0} got exception: {1} (Command Executed: {2}) (InnerException: {3})", cmd, ex.Message, msg, ex.InnerException == null ? "NONE" : ex.InnerException.Message));
						}
					}
					catch
					{
						ShowSyntax(a_Player, cmd);
					}
				}
				else
				{
					ShowSyntax(a_Player, cmd);
				}
			}
		}
		else
		{
			// unknown cmd
			a_Player.PushChatMessageWithColor(EChatChannel.Syntax, 255, 0, 0, "UNKNOWN COMMAND: {0}", cmd);
		}
	}

	public static void RegisterCommand(string cmd, string desc, Delegate dele, CommandParsingFlags parsingFlags, CommandRequirementsFlags requirementFlags, string[] aliases = null, string overrideSyntax = null, bool isAnimation = false, EAnimCategory animCategory = EAnimCategory.Misc)
	{
		CommandDescriptor descriptor = GetCommandDescriptor(cmd);
		if (descriptor == null)
		{
			m_dictCommandDescriptors.Add(cmd, new CommandDescriptor(cmd, desc, parsingFlags, requirementFlags, (Delegate)dele, aliases, overrideSyntax, isAnimation, animCategory));
		}
		else
		{
			throw new Exception(Helpers.FormatString("COMMAND ERROR: {0} is a duplicate.", cmd));
		}
	}

	public static string BlockCommand(string cmd)
	{
		string strMessageToReturn = Helpers.FormatString("Successfully blocked '/{0}'", cmd);

		if (cmd == "blockcommand" || cmd == "unblockcommand")
		{
			strMessageToReturn = Helpers.FormatString("Can't block this command");
			return strMessageToReturn;
		}

		if (g_strBlockedCommands.Contains(cmd))
		{
			// don't do anything
			strMessageToReturn = Helpers.FormatString("/{0} is already blocked. Use /unblockcommand to unblock it", cmd);
			return strMessageToReturn;
		}

		// Now actually verify it's a command:
		CommandDescriptor descriptor = GetCommandDescriptor(cmd);
		if (descriptor != null)
		{
			g_strBlockedCommands.Add(cmd);
			return strMessageToReturn;
		}
		else
		{
			strMessageToReturn = Helpers.FormatString("/{0} is not a command.", cmd);
		}

		return strMessageToReturn;
	}

	public static string UnBlockCommand(string cmd)
	{
		string strMessageToReturn = Helpers.FormatString("Successfully unblocked '/{0}'", cmd);

		if (g_strBlockedCommands.Contains(cmd))
		{
			g_strBlockedCommands.Remove(cmd);
			return strMessageToReturn;
		}
		else
		{
			strMessageToReturn = Helpers.FormatString("/{0} is not blocked. Use /blockcommand to block it", cmd);
		}

		return strMessageToReturn;
	}

	private static Dictionary<string, CommandDescriptor> m_dictCommandDescriptors = new Dictionary<string, CommandDescriptor>();
}

