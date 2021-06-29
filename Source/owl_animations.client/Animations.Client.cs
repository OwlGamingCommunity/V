using System.Collections.Generic;

public static class Animations
{
	private static bool m_bCrouchBlocked = false;

	static Animations()
	{

	}

	private static Dictionary<EClipsetID, string> g_dictClipsets = new Dictionary<EClipsetID, string>()
	{
		{ EClipsetID.None, "" }, // none, useful because data gets wiped serverside meaning default(T) is 0
		{ EClipsetID.Crouch, "move_ped_crouched" },
		{ EClipsetID.CrouchStrafe, "move_ped_crouched_strafing" },
		{ EClipsetID.DrunkLow, "move_m@drunk@slightlydrunk" },
		{ EClipsetID.DrunkMed, "move_m@drunk@moderatedrunk" },
		{ EClipsetID.DrunkHigh, "move_m@drunk@verydrunk" },
		{ EClipsetID.Injured, "move_injured_generic" },
		{ EClipsetID.Injured2, "move_characters@lester@std_caneup" }
	};


	public static void Init()
	{
		dictCurrentAnimations = new Dictionary<RAGE.Elements.Player, TransmitAnimation>();

		RageEvents.RAGE_OnRender += OnRender;
		ScriptControls.SubscribeToControl(EScriptControlID.CancelAnimation, CancelAnimation);

		RageEvents.AddDataHandler(EDataNames.ANIM_DATA, UpdateAnimation);
		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;

		RageEvents.RAGE_OnTick_LowFrequency += OnTick;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleCrouch, OnToggleCrouch);

		RageEvents.RAGE_OnTick_MediumFrequency += UpdateAllClipsetsForAll;

		NetworkEvents.CustomAnim_RequestClientLegacyLoad += OnLoadLegacyCustomAnims;

		// Load all clipsets
		// TODO: Later we may want to do this smart and on demand incase we have too many
		foreach (var clipset in g_dictClipsets)
		{
			RAGE.Game.Streaming.RequestClipSet(clipset.Value);
		}
	}

	private static void OnLoadLegacyCustomAnims()
	{
		// Safe to assume rage client storage is loaded by now since we've connected to and logged into the server...
		if (RageClientStorageManager.Container.DO_NOT_USE_CustomAnimations != null)
		{
			foreach (var anim in RageClientStorageManager.Container.DO_NOT_USE_CustomAnimations)
			{
				NetworkEventSender.SendNetworkEvent_CustomAnim_Create(anim.commandName, anim.animDictionary, anim.animName, anim.loop, anim.stopOnLastFrame, anim.onlyAnimateUpperBody, anim.allowPlayerMovement, anim.durationSeconds, true);
			}

			// delete local storage + set to null so its removed totally
			RageClientStorageManager.Container.DO_NOT_USE_CustomAnimations = null;
			RageClientStorageManager.Flush();
		}
	}
	private static void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;

			string strJsonData = DataHelper.GetEntityData<string>(player, EDataNames.ANIM_DATA);
			if (strJsonData != null && strJsonData.Length > 0)
			{
				// Does anim need re-applied?
				TransmitAnimation currentAnim = TransmitAnimation.FromJSON(strJsonData);
				if (!player.IsPlayingAnim(currentAnim.Dict, currentAnim.Name, 3))
				{
					ApplyAnimationInternal(player, currentAnim);
				}
			}
		}
	}

	private static void OnToggleCrouch(EControlActionType actionType)
	{
		if (!m_bCrouchBlocked)
		{
			m_bCrouchBlocked = true;
			ClientTimerPool.CreateTimer((object[] parameters) => { m_bCrouchBlocked = false; }, 1000, 1);

			NetworkEventSender.SendNetworkEvent_RequestCrouch();
		}
	}

	private static void OnTick()
	{
		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			string strJsonData = DataHelper.GetEntityData<string>(player, EDataNames.ANIM_DATA);

			// check for anim else
			if (strJsonData == null)
			{
				if (dictCurrentAnimations.ContainsKey(player))
				{
					TransmitAnimation currentAnim = dictCurrentAnimations[player];

					if (player.IsPlayingAnim(currentAnim.Dict, currentAnim.Name, 3))
					{
						player.StopAnimTask(currentAnim.Dict, currentAnim.Name, 1.0f);
					}
				}
			}
		}
	}

	private static void ApplyAnimationInternal(RAGE.Elements.Player player, TransmitAnimation animation)
	{
		// TODO_POST_LAUNCH: Refcount + unload model and anim dicts?
		// TODO_POST_LAUNCH: This could bug out if the same player is fast enough to change anim, while the previous was loading, it could apply the original if the lambdas dont get called in order
		dictCurrentAnimations[player] = animation;

		AsyncAnimLoader.RequestAsyncLoad(animation.Dict, (string strDictionary) =>
		{
			player.TaskPlayAnim(animation.Dict, animation.Name, 8.0f, 1.0f, -1, animation.Flags, 0.0f, false, false, false);
		});
	}

	private static Dictionary<RAGE.Elements.Player, TransmitAnimation> dictCurrentAnimations = null;
	private static void UpdateAnimation(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;

			if (newValue == null)
			{
				TransmitAnimation oldAnim = dictCurrentAnimations[player];
				player.StopAnimTask(oldAnim.Dict, oldAnim.Name, 1.0f);

				if (!player.IsPlayingAnim(oldAnim.Dict, oldAnim.Name, 3))
				{
					dictCurrentAnimations.Remove(player);
				}
			}
			else
			{
				string strJsonData = newValue.ToString();
				TransmitAnimation currentAnim = TransmitAnimation.FromJSON(strJsonData);

				ApplyAnimationInternal(player, currentAnim);
			}
		}
	}

	private static void CancelAnimation(EControlActionType actionType)
	{
		if (KeyBinds.CanProcessKeybinds())
		{
			bool bHasAnim = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.HAS_ANIM);
			bool bCanCancel = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ANIM_CANCELLABLE);

			if (bHasAnim && bCanCancel)
			{
				NetworkEventSender.SendNetworkEvent_RequestStopAnimation();
			}
		}
	}

	private static void OnRender()
	{
		bool bHasAnim = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.HAS_ANIM);
		bool bCanCancel = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ANIM_CANCELLABLE);

		if (bHasAnim && bCanCancel)
		{
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Jump);
			if (HUD.IsVisible())
			{
				TextHelper.Draw2D("Press SPACEBAR to stop animation", 0.5f, 0.88f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			}
		}
	}

	private static void UpdateAllClipsetsForAll()
	{
		foreach (var player in RAGE.Elements.Entities.Players.Streamed)
		{
			EClipsetID movementClipsetID = DataHelper.GetEntityData<EClipsetID>(player, EDataNames.MOVE_CLIPSET);
			EClipsetID strafeClipsetID = DataHelper.GetEntityData<EClipsetID>(player, EDataNames.STRAFE_CLIPSET);

			// movement
			if (movementClipsetID > 0)
			{
				if (g_dictClipsets.ContainsKey(movementClipsetID))
				{
					string strClipset = g_dictClipsets[movementClipsetID];

					if (RAGE.Game.Streaming.HasClipSetLoaded(strClipset))
					{
						player.SetMovementClipset(strClipset, 1.0f);
					}
				}
			}
			else
			{
				ApplyDefaultMovementClipset(player);
			}

			// strafe
			if (movementClipsetID > 0)
			{
				if (g_dictClipsets.ContainsKey(strafeClipsetID))
				{
					string strClipset = g_dictClipsets[strafeClipsetID];

					if (RAGE.Game.Streaming.HasClipSetLoaded(strClipset))
					{
						player.SetStrafeClipset(strClipset);
					}
				}
			}
			else
			{
				ApplyDefaultStrafeClipset(player);
			}
		}
	}

	private static void ApplyDefaultMovementClipset(RAGE.Elements.Player player)
	{
		if (player.GetHealth() >= 150)
		{
			player.ResetMovementClipset(0.0f);
		}
		else
		{
			player.SetMovementClipset("move_injured_generic", 1.0f);
		}
	}

	private static void ApplyDefaultStrafeClipset(RAGE.Elements.Player player)
	{
		player.ResetStrafeClipset();
	}
}