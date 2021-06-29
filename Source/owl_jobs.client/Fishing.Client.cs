using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;

public class FishingSystem
{
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> m_FishingObjects = new Dictionary<Player, RAGE.Elements.MapObject>();
	private bool m_bIsInBiteState = false;
	private WeakReference<ClientTimer> m_KeyMoveTimer = new WeakReference<ClientTimer>(null);

	public FishingSystem()
	{
		RageEvents.RAGE_OnPlayerQuit += OnPlayerDisconnected;

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.RAGE_OnEntityStreamIn += OnStreamOut;
		RageEvents.AddDataHandler(EDataNames.FISHING, OnFishingDataChanged);

		NetworkEvents.StartFishing += RequestStartFishing;
		NetworkEvents.Fishing_OnBite += OnBite;
		NetworkEvents.FishingLevelUp += OnFishingLevelUp;

		// fishing uses animation control
		ScriptControls.SubscribeToControl(EScriptControlID.CancelAnimation, StopFishing);

		RageEvents.RAGE_OnRender += OnRender;
	}

	private int m_CurrentLevel = 0;
	private float m_CharPos = 0.0f;
	private int numCorrect = 0;
	private Queue<MissionKeyEntry> m_CurrentCharacters = new Queue<MissionKeyEntry>();

	private Dictionary<int, int> m_dictDifficulties_NumberOfActions_AtOnce = new Dictionary<int, int>()
	{
		{0, 1 },
		{1, 1 },
		{2, 1 },
		{3, 1 },
		{4, 2 },
		{5, 2 },
		{6, 2 },
		{7, 2 },
		{8, 3 },
		{9, 3 },
		{10, 3 },
		{11, 3 },
		{12, 4 },
		{13, 4 },
		{14, 4 },
		{15, 4 },
		{16, 5 },
		{17, 5 },
		{18, 5 },
		{19, 5 }
	};
	private Dictionary<int, int> m_dictDifficulties_NumberOfActions = new Dictionary<int, int>()
	{
		{0, 3 },
		{1, 3 },
		{2, 3 },
		{3, 3 },
		{4, 4 },
		{5, 4 },
		{6, 4 },
		{7, 4 },
		{8, 5 },
		{9, 5 },
		{10, 5 },
		{11, 5 },
		{12, 5 },
		{13, 6 },
		{14, 6 },
		{15, 6 },
		{16, 6 },
		{17, 6 },
		{18, 7 },
		{19, 7 }
	};
	private Dictionary<int, float> m_dictDifficulties_Speed = new Dictionary<int, float>()
	{
		{0, 0.0020f },
		{1, 0.0021f },
		{2, 0.0022f },
		{3, 0.0023f },
		{4, 0.0024f },
		{5, 0.0025f },
		{6, 0.0026f },
		{7, 0.0027f },
		{8, 0.0028f },
		{9, 0.0029f },
		{10, 0.0030f },
		{11, 0.0031f },
		{12, 0.0032f },
		{13, 0.0033f },
		{14, 0.0034f },
		{15, 0.0035f },
		{16, 0.0036f },
		{17, 0.0037f },
		{18, 0.0038f },
		{19, 0.0039f },
	};
	private Dictionary<int, float> m_dictDifficulties_HitArea = new Dictionary<int, float>()
	{
		{0, 0.03f },
		{1, 0.03f },
		{2, 0.04f },
		{3, 0.05f },
		{4, 0.06f },
		{5, 0.07f },
		{6, 0.08f },
		{7, 0.09f },
		{8, 0.10f },
		{9, 0.11f },
		{10, 0.12f },
		{11, 0.13f },
		{12, 0.14f },
		{13, 0.15f },
		{14, 0.16f },
		{15, 0.17f },
		{16, 0.18f },
		{17, 0.19f },
		{18, 0.20f },
		{19, 0.21f },
	};
	private Dictionary<int, float> m_dictDifficulties_GapBetweenSequentialInstructions = new Dictionary<int, float>()
	{
		{0, 0.035f },
		{1, 0.035f },
		{2, 0.035f },
		{3, 0.035f },
		{4, 0.035f },
		{5, 0.035f },
		{6, 0.035f },
		{7, 0.035f },
		{8, 0.035f },
		{9, 0.035f },
		{10, 0.035f },
		{11, 0.035f },
		{12, 0.035f },
		{13, 0.035f },
		{14, 0.035f },
		{15, 0.035f },
		{16, 0.035f },
		{17, 0.035f },
		{18, 0.035f },
		{19, 0.035f },
	};

	private class MissionKeyEntry
	{
		public MissionKeyEntry(ConsoleKey a_Key)
		{
			Key = a_Key;
			CurrentState = ECurrentState.None;
		}

		public ConsoleKey Key { get; }
		public ECurrentState CurrentState { get; set; }
	}

	private enum ECurrentState
	{
		None,
		Failed,
		Succeeded
	}

	private bool m_bWaitingOnKeyUp = false;
	private ConsoleKey m_KeyWaitingOn = ConsoleKey.NoName;
	MissionKeyEntry currentEntry = null;
	private void OnRender()
	{
		bool bIsFishing = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.FISHING);

		if (bIsFishing && !m_bIsInBiteState)
		{
			ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Jump);
			TextHelper.Draw2D("Press SPACEBAR to stop fishing", 0.5f, 0.88f, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
		}

		if (m_bIsInBiteState)
		{
			float fSectionWidth = m_dictDifficulties_HitArea[m_CurrentLevel];
			TextHelper.Draw2D("Press the keys as they pass the center", 0.5f, 0.75f, 0.5f, 255, 194, 14, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
			RAGE.Game.Graphics.DrawRect(0.0f, 0.85f, 1.0f - fSectionWidth, 0.05f, 255, 194, 15, 200, 0); // 76, 76, 76
			RAGE.Game.Graphics.DrawRect(0.5f, 0.85f, fSectionWidth, 0.05f, 0, 255, 0, 200, 0);
			RAGE.Game.Graphics.DrawRect(1.0f, 0.85f, 1.0f - fSectionWidth, 0.05f, 255, 194, 15, 200, 0);

			// TODO_FISHING: Later we should refcount dictionary loading, we load a bunch and never unload... especially animations etc
			if (m_CurrentCharacters.Count > 0)
			{
				float fGap = m_dictDifficulties_GapBetweenSequentialInstructions[m_CurrentLevel];
				int numberAtOnce = m_dictDifficulties_NumberOfActions_AtOnce[m_CurrentLevel];

				// If we have less than numberAtOnce remaining, clamp numberAtOnce
				if (m_CurrentCharacters.Count < numberAtOnce)
				{
					numberAtOnce = m_CurrentCharacters.Count;
				}

				float fPosOfFinalEntry = m_CharPos - (numberAtOnce * fGap);
				float fPosOfCurrentActiveEntry = 0.0f;
				for (int i = 0; i < numberAtOnce; ++i)
				{
					MissionKeyEntry thisEntry = m_CurrentCharacters.ToArray()[i];
					float fThisGap = (i * fGap);

					// If its the first one we've encountered that isnt done, set it as current
					if (currentEntry == null && thisEntry.CurrentState == ECurrentState.None)
					{
						currentEntry = thisEntry;
					}

					if (thisEntry == currentEntry)
					{
						fPosOfCurrentActiveEntry = m_CharPos - fThisGap;
					}

					string strDictArrow = "basejumping";
					if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictArrow))
					{
						RAGE.Game.Graphics.RequestStreamedTextureDict(strDictArrow, true);
					}
					string strDictSuccess = "commonmenu";
					if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(strDictSuccess))
					{
						RAGE.Game.Graphics.RequestStreamedTextureDict(strDictSuccess, true);
					}

					if (thisEntry.CurrentState == ECurrentState.None)
					{
						float fRot = 0.0f;
						if (thisEntry.Key == ConsoleKey.UpArrow)
						{
							fRot = 0.0f;
						}
						else if (thisEntry.Key == ConsoleKey.RightArrow)
						{
							fRot = 90.0f;
						}
						else if (thisEntry.Key == ConsoleKey.DownArrow)
						{
							fRot = 180.0f;
						}
						else if (thisEntry.Key == ConsoleKey.LeftArrow)
						{
							fRot = 270.0f;
						}

						RAGE.Game.Graphics.DrawSprite(strDictArrow, "arrow_pointer", m_CharPos - fThisGap, 0.85f, 0.02f, 0.04f, fRot, 255, 255, 255, 255, 0);
					}
					else if (thisEntry.CurrentState == ECurrentState.Failed)
					{
						RAGE.Game.Graphics.DrawSprite(strDictSuccess, "shop_box_cross", m_CharPos - fThisGap, 0.85f, 0.05f, 0.1f, 0.0f, 255, 255, 255, 255, 0);
					}
					else if (thisEntry.CurrentState == ECurrentState.Succeeded)
					{
						RAGE.Game.Graphics.DrawSprite(strDictSuccess, "shop_box_tick", m_CharPos - fThisGap, 0.85f, 0.05f, 0.1f, 0.0f, 255, 255, 255, 255, 0);
					}
				}

				if (currentEntry != null)
				{
					if (!m_bWaitingOnKeyUp && currentEntry.CurrentState == ECurrentState.None)
					{
						if (KeyBinds.IsConsoleKeyDown(currentEntry.Key))
						{
							m_bWaitingOnKeyUp = true;
							m_KeyWaitingOn = currentEntry.Key;

							// Is it within the boundary?
							float fDelta = fSectionWidth / 2.0f;
							if (fPosOfCurrentActiveEntry >= (0.5f - fDelta) && fPosOfCurrentActiveEntry <= (0.5f + fDelta))
							{
								RAGE.Game.Audio.PlaySoundFrontend(-1, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET", true);
								++numCorrect;

								currentEntry.CurrentState = ECurrentState.Succeeded;
								currentEntry = null;
							}
							else
							{
								RAGE.Game.Audio.PlaySoundFrontend(-1, "CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET", true);
								currentEntry.CurrentState = ECurrentState.Failed;
								currentEntry = null;
							}
						}
						else if (KeyBinds.IsAnyConsoleKeyDown(currentEntry.Key, out ConsoleKey keyDown))
						{
							// We only fail if its an arrow key, otherwise they might just be chatting or using inventory etc
							if (keyDown == ConsoleKey.UpArrow || keyDown == ConsoleKey.DownArrow || keyDown == ConsoleKey.LeftArrow || keyDown == ConsoleKey.RightArrow)
							{
								m_bWaitingOnKeyUp = true;
								m_KeyWaitingOn = keyDown;

								RAGE.Game.Audio.PlaySoundFrontend(-1, "CHECKPOINT_MISSED", "HUD_MINI_GAME_SOUNDSET", true);
								currentEntry.CurrentState = ECurrentState.Failed;
							}
						}
					}
					else if (m_bWaitingOnKeyUp) // check for key up
					{
						if (!KeyBinds.IsConsoleKeyDown(m_KeyWaitingOn))
						{
							m_KeyWaitingOn = ConsoleKey.NoName;
							m_bWaitingOnKeyUp = false;
						}
					}
				}

				// Do we need to move to the next letter?
				if (fPosOfFinalEntry >= 1.0f)
				{
					fPosOfFinalEntry = 0.0f;
					m_CharPos = 0.0f;

					// dequeue
					for (int i = 0; i < numberAtOnce; ++i)
					{
						m_CurrentCharacters.Dequeue();
					}
					currentEntry = null;
				}
			}
			else
			{
				m_CurrentCharacters.Clear();
				m_bIsInBiteState = false;
				NetworkEventSender.SendNetworkEvent_Fishing_OnBiteOutcome(numCorrect, m_dictDifficulties_NumberOfActions[m_CurrentLevel]);

				ClientTimerPool.MarkTimerForDeletion(ref m_KeyMoveTimer);
			}
		}
	}

	private void ScrollMinigameTimer(object[] parameters)
	{
		float fDist = m_dictDifficulties_Speed[m_CurrentLevel];
		m_CharPos += fDist;
	}

	private void OnBite(int level)
	{
		m_CurrentLevel = level;
		m_CharPos = 0.0f;
		m_CurrentCharacters.Clear();
		m_KeyMoveTimer = ClientTimerPool.CreateTimer(ScrollMinigameTimer, 1, -1);

		m_bIsInBiteState = true;

		// add randomized keys
		int numberOfActions = m_dictDifficulties_NumberOfActions[level];
		Random rnd = new Random();
		for (int i = 0; i < numberOfActions; ++i)
		{
			int rand = rnd.Next(37, 40);
			m_CurrentCharacters.Enqueue(new MissionKeyEntry((ConsoleKey)rand));
		}

		numCorrect = 0;
	}


	private void StopFishing(EControlActionType actionType)
	{
		if (KeyBinds.CanProcessKeybinds() && !m_bIsInBiteState)
		{
			bool bIsFishing = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.FISHING);

			if (bIsFishing)
			{
				NotificationManager.ShowNotification("Fishing", "You have stopped fishing", ENotificationIcon.InfoSign);
				NetworkEventSender.SendNetworkEvent_RequestStopFishing();
			}
		}
	}

	private void OnFishingDataChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			UpdateObjectsForPlayer(player);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			UpdateObjectsForPlayer(player);
		}
	}

	private void OnStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			CleanupObjectsForPlayer(player);
		}
	}

	private void OnPlayerDisconnected(RAGE.Elements.Player player)
	{
		CleanupObjectsForPlayer(player);
	}

	private void CleanupObjectsForPlayer(RAGE.Elements.Player player)
	{
		if (m_FishingObjects.ContainsKey(player))
		{
			if (m_FishingObjects[player] != null)
			{
				m_FishingObjects[player].Destroy();
			}

			m_FishingObjects.Remove(player);
		}
	}

	private void RequestStartFishing(int currentLevel, int XPRequiredForNextLevel)
	{
		m_bIsInBiteState = false;

		float fDist = 5.0f;
		Vector3 vecPosInFront = RAGE.Elements.Player.LocalPlayer.Position;
		float radians = (RAGE.Elements.Player.LocalPlayer.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
		vecPosInFront.X += (float)Math.Cos(radians) * fDist;
		vecPosInFront.Y += (float)Math.Sin(radians) * fDist;

		float fGround = WorldHelper.GetGroundPosition(vecPosInFront, true, 0.0f, false);
		if (fGround > 3.0f)
		{
			if (fGround < 5.0f)
			{
				NotificationManager.ShowNotification("Fishing", "You must be closer to the water to fish", ENotificationIcon.ExclamationSign);
			}
			else
			{
				NotificationManager.ShowNotification("Fishing", "You must be near water to fish", ENotificationIcon.ExclamationSign);
			}
		}
		else if (RAGE.Elements.Player.LocalPlayer.IsInWater())
		{
			NotificationManager.ShowNotification("Fishing", "You cannot be submerged in water. Try getting on land, or if you are on land, taking a few steps back.", ENotificationIcon.ExclamationSign);
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_StartFishing_Approved();

			string strLevelMessage = String.Empty;
			if (XPRequiredForNextLevel >= 0)
			{
				strLevelMessage = Helpers.FormatString("You are level {0}. {1} XP required for level {2}.", currentLevel, XPRequiredForNextLevel, currentLevel + 1);
			}
			else
			{
				strLevelMessage = "You are the maximum Fishing level.";
			}

			ShardManager.ShowShard("Fishing Started!", "Wait for a fish to bite!", strLevelMessage);
		}
	}

	private void OnFishingLevelUp(int newLevel, int XPRequiredForNextLevel)
	{
		string strNextLevelMessage = String.Empty;
		if (XPRequiredForNextLevel >= 0)
		{
			strNextLevelMessage = Helpers.FormatString("{0} XP required for level {1}.", XPRequiredForNextLevel, newLevel + 1);
		}
		else
		{
			strNextLevelMessage = "You are now the maximum Fishing level.";
		}

		ShardManager.ShowShard("Fishing Leveled Up!", Helpers.FormatString("You are now level {0}", newLevel), strNextLevelMessage);
	}

	private void UpdateObjectsForPlayer(RAGE.Elements.Player player)
	{
		bool bIsFishing = DataHelper.GetEntityData<bool>(player, EDataNames.FISHING);
		if (bIsFishing)
		{
			// cleanup existing rod if present
			CleanupObjectsForPlayer(player);

			uint hash = HashHelper.GetHashUnsigned("prop_fishing_rod_01");
			AsyncModelLoader.RequestSyncInstantLoad(hash);
			m_FishingObjects[player] = new RAGE.Elements.MapObject(hash, player.Position, new Vector3(0.0f, 0.0f, 0.0f), 255, player.Dimension);

			// attach
			var vecOffsetPos = new RAGE.Vector3(0.10f, 0.10f, 0.06f);
			var vecOffsetRot = new RAGE.Vector3(90.0f, 180.0f, 135.0f);
			RAGE.Game.Entity.AttachEntityToEntity(m_FishingObjects[player].Handle, player.Handle, player.GetBoneIndexByName("IK_L_Hand"), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, vecOffsetRot.X, vecOffsetRot.Y, vecOffsetRot.Z, true, false, false, false, 0, true);
		}
		else
		{
			CleanupObjectsForPlayer(player);
		}

	}
}