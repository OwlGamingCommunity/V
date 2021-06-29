using System;
using System.Collections.Generic;

public class AnimationSystem
{
	public AnimationSystem()
	{
		// COMMANDS
		CommandManager.RegisterCommand("stopanim", "Cancels the current animation (unless forced by the script)", new Action<CPlayer, CVehicle>(StopAnimation), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("animlist", "Displays a list of all animations", new Action<CPlayer, CVehicle>(AnimationList), CommandParsingFlags.Default, CommandRequirementsFlags.Default);
		CommandManager.RegisterCommand("piss", "Plays the pissing animation.", new Action<CPlayer, CVehicle>(Animation_Piss), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("bearrested", "Plays a surrendering animation. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_bearrested), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Surrender);
		CommandManager.RegisterCommand("wank", "Plays the wanking animation", new Action<CPlayer, CVehicle>(Animation_Wank), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("handsup", "Puts your hands up in the air. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_HandsUp), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Surrender);
		CommandManager.RegisterCommand("fu", "Raises your right armand displays your middle finger.", new Action<CPlayer, CVehicle>(Animation_FuckYou), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("carchat", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_CarChat), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("bye", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_bye_hard), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("easynow", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_easy_now), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("what", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_what_hard), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("damn", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_damn), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("comehere", "Plays a comehere gestures. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_ComeHere), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Surrender);
		CommandManager.RegisterCommand("hello", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_hello), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("itsme", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_me_hard), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("yesnod", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_pleased), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("itsmine", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_getsure_its_mine), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("shrug", "Plays the requested animation, Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_gesture_shrug_hard), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("noway", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_no_way), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("pointdown", "Plays the requested animation", new Action<CPlayer, CVehicle>(Animation_gesture_hand_down), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("situps", "Makes your character do situps", new Action<CPlayer, CVehicle>(Animation_situps), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("beggar", "Sits like a beggar.", new Action<CPlayer, CVehicle>(Animation_beggar), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("leansit", "Plays an animation of leaning on a railing while sitting.", new Action<CPlayer, CVehicle>(Animation_leansit), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("stairsit", "Sits on stair steps.", new Action<CPlayer, CVehicle>(Animation_stairsit), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("computer", "Types on a computer.", new Action<CPlayer, CVehicle>(Animation_computer), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("mechanic", "Plays an animation as if you were fixing a car as a mechanic.", new Action<CPlayer, CVehicle>(Animation_mechanic), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.General);
		CommandManager.RegisterCommand("tablelean", "Leans on a table.", new Action<CPlayer, CVehicle>(Animation_tablelean), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("salute", "Makes you do a salute.", new Action<CPlayer, CVehicle>(Animation_idle_a), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("crossarms", "Crosses your arms. Type = 0-3", new Action<CPlayer, CVehicle, int>(Animation_hangout), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("canim", "Plays a custom animation", new Action<CPlayer, CVehicle, string, string, bool, bool, bool, bool, int>(PlayCustomAnimation), CommandParsingFlags.Default, CommandRequirementsFlags.Default);

		CommandManager.RegisterCommand("smoke", "Plays a smoking animation. Type = 0-2", new Action<CPlayer, CVehicle, int>(Animation_smoke), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Smoke);
		CommandManager.RegisterCommand("sitf", "Plays a sitting animation for a female character. Type = 0-22", new Action<CPlayer, CVehicle, int>(Animation_sitf), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("sitm", "Plays a sitting animation for a male character. Type = 0-25", new Action<CPlayer, CVehicle, int>(Animation_sitm), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("tablesit", "Plays a sitting on table animation. Type = 0-11", new Action<CPlayer, CVehicle, int>(Animation_tablesit), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("clipboard", "Makes it look like you’re writing on a keyboard.", new Action<CPlayer, CVehicle>(Animation_clipboard), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("drinking", "Makes it look like you’re drinking.", new Action<CPlayer, CVehicle>(Animation_drinking), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Drink);
		CommandManager.RegisterCommand("lean", "Puts you in a leaning position. Type = 0-12", new Action<CPlayer, CVehicle, int>(Animation_leaning), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("flex", "Makes you show off your muscles. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_flexing), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("strip", "Makes you do a strip dance. Type = 0-6", new Action<CPlayer, CVehicle, int>(Animation_strip), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("pushups", "Displays your character doing pushups", new Action<CPlayer, CVehicle>(Animation_pushups), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("weed", "Makes it look like you’re smoking weed. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_weed), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Smoke);
		CommandManager.RegisterCommand("lay", "Places you in a laying position. Type = 0-12", new Action<CPlayer, CVehicle, int>(Animation_lay), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);
		CommandManager.RegisterCommand("crouch", "Places you in a crouching position. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_crouch), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.General);
		CommandManager.RegisterCommand("jog", "Starts jogging. Type = 0-5", new Action<CPlayer, CVehicle, int>(Animation_jogging), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("pullups", "Makes it look you’re pulling yourself up.", new Action<CPlayer, CVehicle>(Animation_pullups), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("dancef", "Plays a dancing animation for female characters. Type = 0-17", new Action<CPlayer, CVehicle, int>(Animation_dancef), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("dancem", "Plays a dancing animation for male characters. Type = 0-18", new Action<CPlayer, CVehicle, int>(Animation_dancem), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("dorkydance", "Plays a funny dance. Type = 0-8", new Action<CPlayer, CVehicle, int>(Animation_dorkydance), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("walkm", "Plays a male walking style. Type = 0-44", new Action<CPlayer, CVehicle, int>(Animation_walkm), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.General);
		CommandManager.RegisterCommand("walkf", "Plays a female walking style. Type = 0-24", new Action<CPlayer, CVehicle, int>(Animation_walkf), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.General);
		CommandManager.RegisterCommand("tapdance", "Plays a tapdance animation. Type = 0-3", new Action<CPlayer, CVehicle, int>(Animation_tapdance), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("grooving", "Plays a grooving animation. Type = 0-4", new Action<CPlayer, CVehicle, int>(Animation_grooving), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("dive", "Make’s your character dive out of the way. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_dive), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("guitar", "Shows you playing the guitar", new Action<CPlayer, CVehicle>(Animation_guitar), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("drums", "Shows you playing the bongos", new Action<CPlayer, CVehicle>(Animation_drums), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("maid", "Makes it look like you’re cleaning something.", new Action<CPlayer, CVehicle>(Animation_maid), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("clap", "Makes you do a clapping animations. Type = 0-4", new Action<CPlayer, CVehicle, int>(Animation_clap), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("blowjob", "Plays Blowjob animation", new Action<CPlayer, CVehicle>(Animation_blowjob), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("celebrate", "Displays your character celebrating. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_celebrate), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("cook", "Displays your character cooking. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_cook), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("dig", "Plays a digging animation", new Action<CPlayer, CVehicle>(Animation_dig), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Misc);
		CommandManager.RegisterCommand("yoga", "Displays your character doing yoga moves. Type = 0-1", new Action<CPlayer, CVehicle, int>(Animation_yoga), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("wait", "Puts your hands in front of you in a waiting position. Type = 0-4", new Action<CPlayer, CVehicle, int>(Animation_wait), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("standing", "Standing animations. Type = 0-3", new Action<CPlayer, CVehicle, int>(Animation_standing), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("leanforward", "Plays an animation where your character would be leaning forward e.g on a railing. Type = 0-9", new Action<CPlayer, CVehicle, int>(Animation_leanforward), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("warmup", "Starts doing some warm-up. Type = 0-5", new Action<CPlayer, CVehicle, int>(Animation_warmup), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Workout);
		CommandManager.RegisterCommand("blowkiss", "Blows a kiss", new Action<CPlayer, CVehicle>(Animation_blowkiss), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("injured", "Plays an injured animation. Type = 0-2", new Action<CPlayer, CVehicle, int>(Animation_injured), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Surrender);
		CommandManager.RegisterCommand("cop", "Plays a cop animation. Type = 0-3", new Action<CPlayer, CVehicle, int>(Animation_cop), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.General);
		CommandManager.RegisterCommand("nonod", "Plays a refusal gesture. Type = 0-2", new Action<CPlayer, CVehicle, int>(Animation_NoNod), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("bartender", "Puts your hands on the bar table", new Action<CPlayer, CVehicle>(Animation_bartender), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Stand);
		CommandManager.RegisterCommand("point", "Points at someone", new Action<CPlayer, CVehicle>(Animation_point), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Gestures);
		CommandManager.RegisterCommand("leandance", "A dance variation for in the nightclub. Type = 0-11", new Action<CPlayer, CVehicle, int>(Animation_leandance), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Dance);
		CommandManager.RegisterCommand("barsit", "A sitting animation for a bar stool. Type = 0-2", new Action<CPlayer, CVehicle, int>(Animation_barsit), CommandParsingFlags.Default, CommandRequirementsFlags.Default, null, null, true, EAnimCategory.Sit);


		// EVENTS
		NetworkEvents.RequestStopAnimation += OnStopAnimationEvent;
		NetworkEvents.RequestCrouch += OnRequestCrouch;
		NetworkEvents.CustomAnim_Create += OnCustomAnim_Create;
		NetworkEvents.CustomAnim_Delete += OnCustomAnim_Delete;
	}

	private void OnCustomAnim_Create(CPlayer player, string strCommandName, string strAnimDictionary, string strAnimName, bool bLoop, bool bStopOnLastFrame, bool bOnlyAnimateUpperBody, bool bAllowPlayerMovement, int duration, bool bIsSilent)
	{
		if (!player.HasSpaceForMoreCustomAnimations())
		{
			if (!bIsSilent)
			{
				player.SendNotification("Custom Animation", ENotificationIcon.ExclamationSign, "You have too many custom animations. Please delete some first!");
			}
			return;
		}

		Database.Models.CustomAnim existingAnim = player.GetCustomAnim(strCommandName);
		if (existingAnim == null)
		{
			Database.Models.CustomAnim.Create(player.AccountID, strCommandName, strAnimDictionary, strAnimName, bLoop, bStopOnLastFrame, bOnlyAnimateUpperBody, bAllowPlayerMovement, duration,
			(Database.Models.CustomAnim newAnim) =>
			{
				player.AddCustomAnimation(newAnim);
			});
		}
		else
		{
			if (!bIsSilent)
			{
				player.SendNotification("Custom Animation", ENotificationIcon.ExclamationSign, "A custom animation with command name '{0}' already exists.", strCommandName);
			}
		}
	}

	private void OnCustomAnim_Delete(CPlayer player, string strCommandName)
	{
		player.DeleteCustomAnimation(strCommandName);
	}

	/*
	Loop = 1 << 0,
	StopOnLastFrame = 1 << 1,
	OnlyAnimateUpperBody = 1 << 4,
	AllowPlayerControl = 1 << 5,
	Cancellable = 1 << 7
	*/
	public void PlayCustomAnimation(CPlayer player, CVehicle vehicle, string AnimDictionary, string AnimName, bool Loop, bool StopOnLastFrame, bool OnlyAnimateUpperBody, bool AllowPlayerMovement, int DurationSeconds)
	{
		AnimationFlags animFlags = 0;

		if (Loop)
		{
			animFlags |= AnimationFlags.Loop;
		}

		if (StopOnLastFrame)
		{
			animFlags |= AnimationFlags.StopOnLastFrame;
		}
		if (OnlyAnimateUpperBody)
		{
			animFlags |= AnimationFlags.OnlyAnimateUpperBody;
		}
		if (AllowPlayerMovement)
		{
			animFlags |= AnimationFlags.AllowPlayerControl;
		}

		player.AddAnimationToQueue((int)animFlags, AnimDictionary, AnimName, true, true, true, DurationSeconds * 1000, true);
	}

	public void OnStopAnimationEvent(CPlayer a_Player)
	{
		CVehicle vehicle = VehiclePool.GetVehicleFromGTAInstance(a_Player.Client.Vehicle);
		StopAnimation(a_Player, vehicle);
	}

	public void OnRequestCrouch(CPlayer player)
	{
		player.IsCrouched = !player.IsCrouched;

		if (player.IsCrouched)
		{
			player.MoveClipset = EClipsetID.Crouch;
			player.StrafeClipset = EClipsetID.CrouchStrafe;
		}
		else
		{
			player.ClearMoveClipset();
			player.ClearStrafeClipset();
		}
	}

	public void StopAnimation(CPlayer player, CVehicle vehicle)
	{
		player.StopCurrentAnimation();
	}

	public void AnimationList(CPlayer player, CVehicle vehicle)
	{
		player.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "To preview an animation, click on the animation name in the table.");
		player.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "To view all the categories available, you can scroll through the tabs with your mouse or arrow keys.");
		player.PushChatMessageWithColor(EChatChannel.Syntax, 244, 232, 66, "To delete a custom animation, click on the animation that has the category Custom.");

		List<CAnimationCommand> lstAnims = CommandManager.GetAnimationCommands();

		foreach (Database.Models.CustomAnim anim in player.GetCustomAnims())
		{
			lstAnims.Add(new CAnimationCommand(anim.CommandName, "A custom command made by yourself", EAnimCategory.Custom, false));
		}

		NetworkEventSender.SendNetworkEvent_ShowAnimationList(player, lstAnims);
	}

	// BEGIN ANIMATIONS
	public void Animation_Piss(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.Loop, "missbigscore1switch_trevor_piss", "piss_loop", true, true, true, 0, true);
	}

	public void Animation_bearrested(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@move_m@prisoner_cuffed", "idle", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "random@arrests@busted", "enter", true, true, true, 0, true);
				break;
		}
	}
	public void Animation_ComeHere(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_come_here_hard", true, true, true, 1000, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_you_hard", true, true, true, 1000, true);
				break;
		}
	}

	public void Animation_NoNod(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_displeased", true, true, true, 1000, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_nod_no_hard", true, true, true, 1000, true);
				break;
		}
	}

	public void Animation_Wank(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_player_intwank", "mp_player_int_wank", true, true, true, 1000, true);
	}

	public void Animation_HandsUp(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "rcmminute2", "kneeling_arrest_idle", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "rcmminute2", "arrest_walk", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_FuckYou(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@code_human_in_car_mp_actions@finger@std@ds@base", "enter", true, true, true, 0, true);
	}

	public void Animation_CarChat(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.Loop, "gestures@m@car@truck@casual@ps", "gesture_chat", true, true, true, 0, true);
	}

	private void Animation_gesture_bye_hard(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_bye_hard", true, true, true, 1000, true);
	}

	private void Animation_gesture_easy_now(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_easy_now", true, true, true, 1000, true);
	}

	private void Animation_gesture_what_hard(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_what_hard", true, true, true, 1000, true);
	}

	private void Animation_gesture_damn(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_damn", true, true, true, 1000, true);
	}

	private void Animation_gesture_hello(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_hello", true, true, true, 1000, true);
	}

	private void Animation_gesture_me_hard(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "gestures@f@standing@casual", "gesture_me_hard", true, true, true, 1000, true);
	}

	private void Animation_gesture_pleased(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_pleased", true, true, true, 1000, true);
	}

	private void Animation_getsure_its_mine(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "getsure_its_mine", true, true, true, 1000, true);
	}

	private void Animation_gesture_shrug_hard(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@mp_celebration@draw@female", "draw_react_female_a", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_shrug_hard", true, true, true, 1000, true);
				break;
		}
	}

	private void Animation_gesture_no_way(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_no_way", true, true, true, 1000, true);
	}

	private void Animation_gesture_hand_down(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "gestures@f@standing@casual", "gesture_hand_down", true, true, true, 1000, true);
	}

	public void Animation_situps(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_sit_ups@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_beggar(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "rcmlastone2leadinout", "sas_idle_sit", true, true, true, 0, true);
	}

	public void Animation_stairsit(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_steps@male@elbows_on_knees@base", "base", true, true, true, 0, true, true, 0.65f);
	}

	public void Animation_tablelean(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@board_room@diagram_blueprints@", "base_amy_skater_01", true, true, true, 0, true);
	}

	public void Animation_mechanic(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.Loop, "amb@world_human_vehicle_mechanic@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_computer(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@board_room@stenographer@computer@", "base_left_amy_skater_01", true, true, true, 0, true);
	}

	public void Animation_leansit(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@seated@female@variant_02@", "enter", true, true, true, 0, true);
	}

	public void Animation_idle_a(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@mp_player_intincarsalutestd@rds@", "idle_a", true, true, true, 0, true);
	}

	public void Animation_hangout(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "anim@amb@casino@hangout@ped_female@stand@01b@enter", "enter", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@casino@hangout@ped_female@stand@01b@enter", "enter", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@casino@hangout@ped_male@stand@01b@enter", "enter", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "oddjobs@bailbond_hobohang_out_street_c", "base", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_barsit(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_bar@male@elbows_on_bar@base", "base", true, true, true, 0, true, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_bar@female@left_elbow_on_bar@base", "base", true, true, true, 0, true, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_bar@female@elbows_on_bar@base", "base", true, true, true, 0, true, true);
				break;
		}
	}

	public void Animation_smoke(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_a@base", "base", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@female@smoke@idle_a", "idle_a", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_aa_smoke@male@idle_a", "idle_c", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_sitm(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@elbows_on_knees@base", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@left_elbow_on_knee@base", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@right_foot_out@base", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_deckchair@male@base", "base", true, true, true, 0, true, true, 0.60f);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@male@var_a@", "enter", true, true, true, 0, true, true, 0.43f);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@male@var_b@", "enter", true, true, true, 0, true, true, 0.43f);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@male@var_c@", "enter", true, true, true, 0, true, true, 0.43f);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@male@var_d@", "enter", true, true, true, 0, true, true, 0.43f);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@male@var_e@", "enter", true, true, true, 0, true, true, 0.43f);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@seating@male@var_a@base@", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@seating@male@var_b@base@", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boss@male@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@seating@male@var_b@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@seating@male@var_c@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "drf_mic_1_cs_2-20", "cs_drfriedlander_dual-20", true, true, true, 0, true, true, 0.45f);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@right_foot_out@enter", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@recline_b@base_b", "base_b", true, true, true, 0, true, true, 0.50f);
				break;
			case 18:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_steps@male@hands_in_lap@base", "base", true, true, true, 0, true, true, 0.65f);
				break;
			case 19:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_wall@male@hands_in_lap@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 20:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_wall@male@hands_by_sides@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 21:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_stupor@male@idle_a", "idle_b", true, true, true, 0, true);
				break;
			case 22:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_idle_nowork", true, true, true, 0, true);
				break;
			case 23:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@seated@male@variation_02@", "enter", true, true, true, 0, true, true, 0.45f);
				break;
			case 24:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@seated@male@variation_04@", "enter", true, true, true, 0, true, true, 0.45f);
				break;
			case 25:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@seated@male@variation_05@", "enter", true, true, true, 0, true, true, 0.45f);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@generic@enter", "enter_forward", true, true, true, 0, true, true, 0.50f);
				break;
		}
	}

	public void Animation_tablesit(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@boss@male@", "base", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@crew@male@var_a@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@crew@male@var_c@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@boardroom@crew@male@var_a@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@boardroom@boss@male@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@boss@female@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@boss@female@base_l@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@boss@female@base_r@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@crew@female@var_b@base@", "base", true, true, true, 0, true, true, 0.40f);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@boardroom@crew@female@var_a@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@boardroom@crew@female@var_b@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@boss@male@base@", "base", true, true, true, 0, true, true, 0.35f);
				break;
		}
	}

	public void Animation_sitf(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@female@var_a@", "enter", true, true, true, 0, true, true, 0.40f);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@female@legs_crossed@base", "base", true, true, true, 0, true, true, 0.45f);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@female@var_b@", "enter", true, true, true, 0, true, true, 0.40f);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@female@var_c@", "enter", true, true, true, 0, true, true, 0.40f);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@female@var_d@", "enter", true, true, true, 0, true, true, 0.40f);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@facility@briefing_room@seating@female@var_e@", "enter", true, true, true, 0, true, true, 0.40f);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@female@proper@base", "base", true, true, true, 0, true, true, 0.45f);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_deckchair@female@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@seating@female@var_a@base@", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@seating@female@var_c@base@", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@office@seating@female@var_d@base@", "enter", true, true, true, 0, true, true, 0.50f);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_picnic@female@enter", "enter", true, true, true, 0, true);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boss@female@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@seating@female@var_a@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_steps@female@hands_by_sides@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_seat_wall@female@hands_by_sides@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@female@arms_folded@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 18:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@boardroom@crew@female@var_c@base@", "base", true, true, true, 0, true, true, 0.50f);
				break;
			case 19:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@seated@female@variation_04@", "enter", true, true, true, 0, true, true, 0.55f);
				break;
			case 20:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@seated@female@variation_05@", "enter", true, true, true, 0, true, true, 0.55f);
				break;
			case 21:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@business@bgen@bgen_no_work@", "sit_phone_phoneputdown_idle_nowork", true, true, true, 0, true);
				break;
			case 22:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_stupor@male@idle_a", "idle_b", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_chair@male@generic@enter", "enter_forward", true, true, true, 0, true, true, 0.50f);
				break;
		}
	}

	public void Animation_clipboard(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_clipboard@male@idle_a", "idle_c", true, true, true, 0, true);
	}

	public void Animation_drinking(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_drinking@beer@male@idle_a", "idle_a", true, true, true, 0, true);
	}

	public void Animation_jogging(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_jog@male@idle_a", "idle_a", true, true, true, 0, true);
	}

	public void Animation_leaning(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@smoking@base", "base", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@texting@base", "base", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@legs_crossed@enter", "enter_front", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@beer@idle_a", "idle_c", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@female@wall@back@hand_up@base", "base", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@foot_up@base", "base", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@lo_res_idles@", "world_human_lean_male_texting_lo_res_base", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@lo_res_idles@", "world_human_lean_male_hands_together_lo_res_base", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@standing@male@variant_01@", "base", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "rcmminute2lean", "idle_c", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@clubhouse@bar@bartender@", "anim@amb@clubhouse@bar@bartender@ base_bartender", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "switch@michael@sitting_on_car_premiere", "sitting_on_car_premiere_loop_player", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_leaning@male@wall@back@mobile@idle_a", "idle_a", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_flexing(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_muscle_flex@arms_at_side@idle_a", "idle_a", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_muscle_flex@arms_in_front@idle_a", "idle_c", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_strip(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "oddjobs@assassinate@multi@yachttarget@lapdance", "yacht_ld_f", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@strip_club@private_dance@part3", "priv_dance_p3", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@strip_club@private_dance@part1", "priv_dance_p1", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@strip_club@private_dance@idle", "priv_dance_idle", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@strip_club@pole_dance@pole_dance1", "pd_dance_01", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mp_am_stripper", "lap_dance_player", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_prostitute@cokehead@base", "base", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_pushups(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_push_ups@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_weed(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking_pot@male@base", "base", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking_pot@male@idle_a", "idle_c", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_crouch(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@medic@standing@kneel@enter", "enter", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@medic@standing@tendtodead@enter", "enter", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_jogging(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@jog@", "run", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@jogger", "run", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@jogger", "idle", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@jogger", "jogging", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@jogger", "run", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@jogger", "idle", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_lay(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_bum_slumped@male@laying_on_left_side@base", "base", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_sit_ups@male@idle_a", "idle_b", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_sunbathe@female@back@enter", "enter", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_sunbathe@female@front@enter", "enter", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_sunbathe@male@back@enter", "enter", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_sunbathe@male@front@enter", "enter", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "switch@michael@sunlounger", "sunlounger_idle", true, true, true, 0, true, true, 0.40f);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_sunlounger@female@base", "base", true, true, true, 0, true, true, 0.52f);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_seat_sunlounger@male@base", "base", true, true, true, 0, true, true, 0.55f);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "switch@trevor@annoys_sunbathers", "trev_annoys_sunbathers_loop_girl", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "switch@trevor@annoys_sunbathers", "trev_annoys_sunbathers_loop_guy", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "timetable@ron@ig_3_couch", "laying", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@world_human_bum_slumped@male@laying_on_right_side@base", "base", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_pullups(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "switch@franklin@gym", "001942_02_gc_fras_ig_5_base", true, true, true, 0, true);
	}

	public void Animation_dancef(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^1", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^3", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^5", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^5", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^6", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^1", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^2", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^3", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^4", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^5", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^1", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^3", true, true, true, 0, true);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^4", true, true, true, 0, true);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^5", true, true, true, 0, true);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_female^6", true, true, true, 0, true);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_female^6", true, true, true, 0, true);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v2_female^1", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_female^2", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_dancem(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@black_madonna_entourage@", "hi_dance_facedj_09_v2_male^5", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^2", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^1", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^3", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^4", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^5", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v1_male^6", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^1", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^2", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^3", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^4", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^5", true, true, true, 0, true);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_09_v2_male^6", true, true, true, 0, true);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^2", true, true, true, 0, true);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^5", true, true, true, 0, true);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^4", true, true, true, 0, true);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@crowddance_facedj@", "hi_dance_facedj_11_v1_male^6", true, true, true, 0, true);
				break;
			case 18:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_li_06_base_laz", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "missfbi3_sniping", "dance_m_default", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_dorkydance(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_11_buttwiggle_b_laz", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_13_crotchgrab_laz", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_13_flyingv_laz", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_15_crazyrobot_laz", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_15_lookaround_laz", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_17_smackthat_laz", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_17_spiderman", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_li_06_base_laz", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@lazlow@hi_podium@", "danceidle_hi_11_takebreath_laz", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_walkm(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@move_m@grooving@slow@", "walk", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@heists@box_carry@", "walk", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@move_lester", "walk", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@bail_bond", "walk", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@brave", "walk", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@brave@b", "walk", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@business@a", "walk", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@business@b", "walk", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@business@c", "walk", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@buzzed", "walk", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@casual@d", "walk", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@casual@f", "walk", true, true, true, 0, true);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@caution", "walk", true, true, true, 0, true);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@chubby@a", "walk", true, true, true, 0, true);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@crazy", "walk", true, true, true, 0, true);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@depressed@a", "walk", true, true, true, 0, true);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@depressed@d", "walk", true, true, true, 0, true);
				break;
			case 18:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@fat@a", "walk", true, true, true, 0, true);
				break;
			case 19:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@favor_right_foot", "walk", true, true, true, 0, true);
				break;
			case 20:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@femme@", "walk", true, true, true, 0, true);
				break;
			case 21:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@fire", "walk", true, true, true, 0, true);
				break;
			case 22:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@gangster@a", "walk", true, true, true, 0, true);
				break;
			case 23:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@gangster@generic", "walk", true, true, true, 0, true);
				break;
			case 24:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@gangster@ng", "walk", true, true, true, 0, true);
				break;
			case 25:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@swagger@b", "walk", true, true, true, 0, true);
				break;
			case 26:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@hiking", "walk", true, true, true, 0, true);
				break;
			case 27:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@hurry@b", "walk", true, true, true, 0, true);
				break;
			case 28:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@hurry_butch@b", "walk", true, true, true, 0, true);
				break;
			case 29:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@injured", "walk", true, true, true, 0, true);
				break;
			case 30:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@strung_out@", "walk", true, true, true, 0, true);
				break;
			case 31:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@tired", "walk", true, true, true, 0, true);
				break;
			case 32:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@joy@a", "walk", true, true, true, 0, true);
				break;
			case 33:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@leaf_blower", "walk", true, true, true, 0, true);
				break;
			case 34:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@melee", "walk", true, true, true, 0, true);
				break;
			case 35:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@money", "walk", true, true, true, 0, true);
				break;
			case 36:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@muscle@a", "walk", true, true, true, 0, true);
				break;
			case 37:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@non_chalant", "walk", true, true, true, 0, true);
				break;
			case 38:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@plodding", "walk", true, true, true, 0, true);
				break;
			case 39:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@posh@", "walk", true, true, true, 0, true);
				break;
			case 40:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@power", "walk", true, true, true, 0, true);
				break;
			case 41:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@powerwalk", "walk", true, true, true, 0, true);
				break;
			case 42:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@sad@c", "walk", true, true, true, 0, true);
				break;
			case 43:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@sassy", "walk", true, true, true, 0, true);
				break;
			case 44:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@scared@a", "walk", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_m@generic", "walk", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_walkf(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@move_f@grooving@slow@", "walk", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@arrogant@a", "walk", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@arrogant@b", "walk", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@arrogant@c", "walk", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@business@a", "walk", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@chichi", "walk", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@chubby@a", "walk", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@fat@a", "walk", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@depressed@a", "walk", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@depressed@b", "walk", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@depressed@c", "walk", true, true, true, 0, true);
				break;
			case 12:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@exhausted", "walk", true, true, true, 0, true);
				break;
			case 13:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@femme@", "walk", true, true, true, 0, true);
				break;
			case 14:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@gangster@ng", "walk", true, true, true, 0, true);
				break;
			case 15:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@heels@c", "walk", true, true, true, 0, true);
				break;
			case 16:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@hiking", "walk", true, true, true, 0, true);
				break;
			case 17:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@hurry@a", "walk", true, true, true, 0, true);
				break;
			case 18:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@hurry@b", "walk", true, true, true, 0, true);
				break;
			case 19:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@injured", "walk", true, true, true, 0, true);
				break;
			case 20:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@posh@", "walk", true, true, true, 0, true);
				break;
			case 21:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@sad@a", "walk", true, true, true, 0, true);
				break;
			case 22:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@sassy", "walk", true, true, true, 0, true);
				break;
			case 23:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@sexy", "walk", true, true, true, 0, true);
				break;
			case 24:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@scared@a", "walk", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "move_f@multiplayer", "walk", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_grooving(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@move_f@grooving@", "idle", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@amb@nightclub@dancers@black_madonna_entourage@", "li_dance_facedj_15_v2_male^2", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@black_madonna_entourage@", "li_dance_facedj_11_v1_male^1", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_strip_watch_stand@male_c@base", "base", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@move_m@grooving@", "idle", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_tapdance(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "special_ped@mountain_dancer@monologue_3@monologue_3a", "mnt_dnc_buttwag", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "special_ped@mountain_dancer@monologue_4@monologue_4a", "mnt_dnc_verse", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "special_ped@mountain_dancer@monologue_2@monologue_2a", "mnt_dnc_angel", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "special_ped@mountain_dancer@base", "base", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_dive(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "move_avoidance@generic_m", "react_front_dive_right", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "move_avoidance@generic_m", "react_front_dive_left", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_guitar(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_musician@guitar@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_drums(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_musician@bongos@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_maid(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_maid_clean@base", "base", true, true, true, 0, true);
	}

	public void Animation_clap(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_cheering@male_b", "base", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_cheering@male_e", "base", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_cheering@male_d", "base", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_cheering@female_d", "base", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_cheering@male_a", "base", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_blowjob(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "misscarsteal2pimpsex", "pimpsex_hooker", true, true, true, 0, true);
	}

	public void Animation_celebrate(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@mp_player_intcelebrationfemale@air_shagging", "air_shagging", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "rcmfanatic1celebrate", "celebrate", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_cook(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_bbq@male@idle_b", "idle_d", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@prop_human_bbq@male@idle_a", "idle_b", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_dig(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_gardener_plant@male@base", "base", true, true, true, 0, true);
	}

	public void Animation_yoga(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_yoga@female@base", "base_b", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_yoga@male@base", "base_a", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_wait(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@casino@hangout@ped_female@stand@02a@enter", "enter", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@casino@hangout@ped_male@stand@01a@enter", "enter", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl | AnimationFlags.OnlyAnimateUpperBody), "amb@world_human_stand_guard@male@idle_a", "idle_a", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "amb@world_human_stand_impatient@female@no_sign@base", "base", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)AnimationFlags.AllowPlayerControl, "amb@world_human_stand_guard@male@enter", "enter", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_standing(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@standing@male@variation_02@", "enter", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@standing@female@variation_01@", "enter", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@standing@female@variation_02@", "enter", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@jacuzzi@standing@male@variation_01@", "enter", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_leanforward(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@bow@male@variation_01@", "enter", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@seated@male@variant_02@", "enter", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@standing@male@variant_01@", "enter", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@standing@male@variant_03@", "enter", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@standing@female@variant_01@", "enter", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@rail@standing@female@variant_03@", "enter", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "anim@amb@yacht@bow@female@variation_01@", "enter", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "abigail_mcs_1_concat-10@", "player_zero_dual-10", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "abigail_mcs_1_concat-10", "csb_abigail_dual-10", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "amb@prop_human_bum_shopping_cart@male@enter", "enter", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_warmup(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "idle_b", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "idle_d", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "idle_e", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "idle_f", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "ig_2_gen_warmup_10", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "mini@triathlon", "idle_a", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_blowkiss(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.Loop | AnimationFlags.AllowPlayerControl), "anim@mp_player_intcelebrationfemale@blow_kiss", "blow_kiss", true, true, true, 0, true);
	}

	public void Animation_cop(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_cop_idles@female@idle_enter", "idle_intro", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_cop_idles@male@idle_enter", "idle_intro", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@code_human_police_investigate@base", "base", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "move_m@intimidation@cop@unarmed", "idle", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_injured(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "missprologueig_6", "lying_dead_player0", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "missprologueig_6", "lying_dead_brad", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.AllowPlayerControl), "missfbi5ig_12", "dead_c", true, true, true, 0, true);
				break;
		}
	}

	public void Animation_bartender(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)AnimationFlags.StopOnLastFrame, "anim@amb@clubhouse@bar@drink@base", "idle_a_bartender", true, true, true, 0, true);
	}

	public void Animation_point(CPlayer player, CVehicle vehicle)
	{
		player.AddAnimationToQueue((int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@mp_point", "intro_0", true, true, true, 0, true);
	}
	public void Animation_leandance(CPlayer player, CVehicle vehicle, int type)
	{
		switch (type)
		{
			case 1:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^2", true, true, true, 0, true);
				break;
			case 2:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^5", true, true, true, 0, true);
				break;
			case 3:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^6", true, true, true, 0, true);
				break;
			case 4:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_09_v1_female^3", true, true, true, 0, true);
				break;
			case 5:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_male^1", true, true, true, 0, true);
				break;
			case 6:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_male^3", true, true, true, 0, true);
				break;
			case 7:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_male^4", true, true, true, 0, true);
				break;
			case 8:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_09_v1_female^4", true, true, true, 0, true);
				break;
			case 9:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^3", true, true, true, 0, true);
				break;
			case 10:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^4", true, true, true, 0, true);
				break;
			case 11:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_male^2", true, true, true, 0, true);
				break;
			default:
				player.AddAnimationToQueue((int)AnimationFlags.Loop, "anim@amb@nightclub@dancers@club_ambientpeds@", "li-mi_amb_club_06_base_female^1", true, true, true, 0, true);
				break;
		}
	}
}