using System;
using System.Collections.Generic;
using System.Linq;

public class BlackjackActivityInstance : ActivityInstance
{
	public BlackjackActivityInstance(Int64 uniqueIdentifier) : base(uniqueIdentifier, EActivityType.Blackjack)
	{
		m_State = null;

		UIEvents.Blackjack_PlaceBet_Submit += OnBlackjack_PlaceBet_Submit;
		UIEvents.Blackjack_PlaceBet_Cancel += OnBlackjack_PlaceBet_Cancel;

		UIEvents.Blackjack_Action_HitMe += OnBlackjack_Action_HitMe;
		UIEvents.Blackjack_Action_Stick += OnBlackjack_Action_Stick;

		RageEvents.RAGE_OnTick_OncePerSecond += () =>
		{
			// set all remote participants to no collision with the object, this is so they sit properly. Locally they have collision set when leaving the table so they won't be able to walk through it
			CPropertyFurnitureInstance rootFurnitureInstance = FurnitureSystem.GetFurnitureItemFromID((uint)m_uniqueIdentifier);
			if (rootFurnitureInstance == null)
			{
				return;
			}

			var activityState = GetActivityState();
			if (activityState != null)
			{
				int index = 0;
				foreach (var kvPair in GetActivityState().PlayerStates)
				{
					BlackjackPlayerState participantState = kvPair.Value;
					if (participantState != null)
					{
						RAGE.Elements.Player participant = GetPlayerFromIndex(index);
						if (participant != null)
						{
							participant.SetNoCollisionEntity(rootFurnitureInstance.m_Object.Handle, true);
						}

						++index;
					}
				}
			}
		};

	}

	private void OnBlackjack_Action_HitMe()
	{
		if (LocalPlayerIsInTableButMayNotBePlaying())
		{
			NetworkEventSender.SendNetworkEvent_BlackJack_Action_HitMe(m_uniqueIdentifier);
		}
	}

	private void OnBlackjack_Action_Stick()
	{
		if (LocalPlayerIsInTableButMayNotBePlaying())
		{
			NetworkEventSender.SendNetworkEvent_BlackJack_Action_Stick(m_uniqueIdentifier);
		}
	}

	private void OnBlackjack_PlaceBet_Submit(string strInput)
	{
		if (LocalPlayerIsInTableButMayNotBePlaying())
		{
			if (int.TryParse(strInput, out int amount) && amount > 0)
			{
				NetworkEventSender.SendNetworkEvent_Blackjack_PlaceBet(m_uniqueIdentifier, amount);
			}
			else
			{
				NotificationManager.ShowNotification("Blackjack", "Please enter a valid number for your bet.", ENotificationIcon.ExclamationSign);

				// sending with zero causes them to go back to the betting screen, with the currency info etc
				NetworkEventSender.SendNetworkEvent_Blackjack_PlaceBet(m_uniqueIdentifier, 0);
			}
		}
	}

	public RAGE.Elements.Player GetPlayerFromIndex(int participantIndex)
	{
		BlackjackActivityState state = GetActivityState();

		if (state != null)
		{
			if (participantIndex < state.PlayerStates.Count)
			{
				var playerID = state.PlayerStates.ElementAt(participantIndex).Key;

				foreach (var player in RAGE.Elements.Entities.Players.All)
				{
					if (DataHelper.GetEntityData<int>(player, EDataNames.PLAYER_ID) == playerID)
					{
						return player;
					}
				}

			}
		}

		return null;
	}

	private void OnBlackjack_PlaceBet_Cancel()
	{
		// TODO_ACTIVITY_LOW_PRIO: Handle cancel, do we really want to? otherwise you just stay as a spectator
	}

	public BlackjackActivityState GetActivityState()
	{
		return (BlackjackActivityState)m_State;
	}

	private class CardOffset
	{
		public CardOffset(float a_fRotOffsetFromParticipantRoot, float a_fPosPerCardOffsetsByParticipant, float a_fRotCardOffsetsByParticipant)
		{
			fRotOffsetFromParticipantRoot = a_fRotOffsetFromParticipantRoot;
			fPosPerCardOffsetsByParticipant = a_fPosPerCardOffsetsByParticipant;
			fRotCardOffsetsByParticipant = a_fRotCardOffsetsByParticipant;
		}

		public float fRotOffsetFromParticipantRoot { get; }
		public float fPosPerCardOffsetsByParticipant { get; }
		public float fRotCardOffsetsByParticipant { get; }
	};

	private void SetCameraToParticipantCards(int participantIndex)
	{
		CameraManager.ActivateCamera(ECameraID.ACTIVITY);
		if (participantIndex == -1) // dealer
		{
			var objectInst = ActivitySystem.m_CurrentActivityFurnitureObject.Instance().m_Object;

			float fRotZ = objectInst.GetRotation(0).Z;
			float fRotZ_Pos = fRotZ;
			float fRotZ_LookAt = fRotZ;

			float fDist = 0.3f;

			// Camera pos
			RAGE.Vector3 vecPos_CameraPos = objectInst.Position;
			float radians_CameraPos = (fRotZ_Pos + 90.0f) * (3.14f / 180.0f);
			vecPos_CameraPos.X += (float)Math.Cos(radians_CameraPos) * fDist;
			vecPos_CameraPos.Y += (float)Math.Sin(radians_CameraPos) * fDist;
			vecPos_CameraPos.Z += 1.5f;

			// Look at
			RAGE.Vector3 vecPos_LookAt = objectInst.Position;
			float radians_LookAt = (fRotZ_LookAt + 90.0f) * (3.14f / 180.0f);
			vecPos_LookAt.X += (float)Math.Cos(radians_LookAt) * fDist;
			vecPos_LookAt.Y += (float)Math.Sin(radians_LookAt) * fDist;
			vecPos_LookAt.Z += 0.5f;

			CameraManager.UpdateCamera(ECameraID.ACTIVITY, vecPos_CameraPos, vecPos_LookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, 1000);
		}
		else
		{
			if (m_dictCardObjects.ContainsKey(participantIndex))
			{
				if (m_dictCardObjects[participantIndex].Count > 0)
				{
					RAGE.Vector3 vecLookAt = m_dictCardObjects[participantIndex][0].Position;
					RAGE.Vector3 vecPos_CameraPos = m_dictCardObjects[participantIndex][0].Position.CopyVector().Add(new RAGE.Vector3(0.0f, 0.0f, 0.5f));

					CameraManager.UpdateCamera(ECameraID.ACTIVITY, vecPos_CameraPos, vecLookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, 1000);
				}
			}
		}
	}

	private void SetCameraToParticipant(int participantIndex)
	{
		CameraManager.ActivateCamera(ECameraID.ACTIVITY);
		if (participantIndex == -1) // dealer
		{
			var objectInst = ActivitySystem.m_CurrentActivityFurnitureObject.Instance().m_Object;

			float fRotZ = objectInst.GetRotation(0).Z;
			float fRotZ_Pos = fRotZ;
			float fRotZ_LookAt = fRotZ;

			float fDist = 0.3f;

			// Camera pos
			RAGE.Vector3 vecPos_CameraPos = objectInst.Position;
			float radians_CameraPos = (fRotZ_Pos + 90.0f) * (3.14f / 180.0f);
			vecPos_CameraPos.X += (float)Math.Cos(radians_CameraPos) * fDist;
			vecPos_CameraPos.Y += (float)Math.Sin(radians_CameraPos) * fDist;
			vecPos_CameraPos.Z += 1.5f;

			// Look at
			RAGE.Vector3 vecPos_LookAt = objectInst.Position;
			float radians_LookAt = (fRotZ_LookAt + 90.0f) * (3.14f / 180.0f);
			vecPos_LookAt.X += (float)Math.Cos(radians_LookAt) * fDist;
			vecPos_LookAt.Y += (float)Math.Sin(radians_LookAt) * fDist;
			vecPos_LookAt.Z += 0.5f;

			CameraManager.UpdateCamera(ECameraID.ACTIVITY, vecPos_CameraPos, vecPos_LookAt, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, 1000);
		}
		else
		{
			RAGE.Elements.Player player = GetPlayerFromIndex(participantIndex);
			if (player != null)
			{
				float fDist = 1.0f;
				float fRot = player.GetRotation(0).Z + 90.0f;
				var vecPlayerPos = player.Position;
				var vecCamPos = new RAGE.Vector3(vecPlayerPos.X, vecPlayerPos.Y, vecPlayerPos.Z);
				var radians = fRot * (3.14 / 180.0);
				vecCamPos.X += (float)Math.Cos(radians) * fDist;
				vecCamPos.Y += (float)Math.Sin(radians) * fDist;
				vecCamPos.Z += 0.5f;

				CameraManager.UpdateCamera(ECameraID.ACTIVITY, vecCamPos, vecPlayerPos, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, 1000);
			}
		}
	}

	private void PlayDealerAnim(EBlackJackActivityState oldActivityState, EBlackJackActivityState newActivityState, EBlackJackAnimations animation)
	{
		// only play anim if the state actually changed
		if (oldActivityState == newActivityState)
		{
			return;
		}

		// check we have the required participant etc
		if (animation == EBlackJackAnimations.CheckOwnCard_Dealer)
		{
		}
		else if (animation == EBlackJackAnimations.CheckAndTurnOwnCard_Dealer)
		{

		}
		else if (animation == EBlackJackAnimations.DealCard_Player1)
		{
			if (!HasParticipant(0, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealCard_Player2)
		{
			if (!HasParticipant(1, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealCard_Player3)
		{
			if (!HasParticipant(2, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealCard_Player4)
		{
			if (!HasParticipant(3, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealCard_Dealer)
		{

		}
		else if (animation == EBlackJackAnimations.DealerFocusPlayer1_Idle)
		{
			if (!HasParticipant(0, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealerFocusPlayer2_Idle)
		{
			if (!HasParticipant(1, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealerFocusPlayer3_Idle)
		{
			if (!HasParticipant(2, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealerFocusPlayer4_Idle)
		{
			if (!HasParticipant(3, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.DealerIdle)
		{

		}
		else if (animation == EBlackJackAnimations.HitCardPlayer1)
		{
			if (!HasParticipant(0, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.HitCardPlayer2)
		{
			if (!HasParticipant(1, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.HitCardPlayer3)
		{
			if (!HasParticipant(2, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.HitCardPlayer4)
		{
			if (!HasParticipant(3, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.PlaceBet)
		{

		}
		else if (animation == EBlackJackAnimations.RetrieveCardsPlayer1)
		{
			if (!HasParticipant(0, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.RetrieveCardsPlayer2)
		{
			if (!HasParticipant(1, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.RetrieveCardsPlayer3)
		{
			if (!HasParticipant(2, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.RetrieveCardsPlayer4)
		{
			if (!HasParticipant(3, false))
			{
				return;
			}
		}
		else if (animation == EBlackJackAnimations.RetrieveDealerCards)
		{

		}
		else if (animation == EBlackJackAnimations.TurnCard)
		{

		}

		// Get ped
		var uniqueIdentifier = ActivitySystem.m_CurrentActivityFurnitureObject.Instance().DBID;
		CWorldPed worldPed = ActivitySystem.g_dictActivityPeds[uniqueIdentifier].Instance();

		const int animLen = 3000;
		AsyncAnimLoader.RequestAsyncLoad(BlackjackActivityHelpers.AnimationDictionary, (string strDictionary) =>
		{
			// BASE ANIM
			RAGE.Game.Ai.TaskPlayAnim(worldPed.PedInstance.Handle, BlackjackActivityHelpers.AnimationDictionary, BlackjackActivityHelpers.Animations[animation], 8.0f, 1.0f, animLen, (int)AnimationFlags.Loop, 1.0f, false, false, false);

			// base anim
			// TODO_ACTIVITY_LOW_PRIO: destroy old timer?
			ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				AsyncAnimLoader.RequestAsyncLoad(BlackjackActivityHelpers.AnimationDictionary, (string strDictionaryDefault) =>
				{
					RAGE.Game.Ai.TaskPlayAnim(worldPed.PedInstance.Handle, BlackjackActivityHelpers.AnimationDictionary, BlackjackActivityHelpers.Animations[EBlackJackAnimations.DealerIdle], 8.0f, 1.0f, 4000000, (int)AnimationFlags.Loop, 1.0f, false, false, false);
				});
			}, animLen, 1);

			// ANIM CHAINING WITH SPECIFIC TIMED ACTIONS (E.G. ATTACH CARD)
			if (animation == EBlackJackAnimations.DealCard_Player1 || animation == EBlackJackAnimations.HitCardPlayer1)
			{
				DoChainedAnimation_CardDeal(worldPed, 0);
			}
			else if (animation == EBlackJackAnimations.DealCard_Player2 || animation == EBlackJackAnimations.HitCardPlayer2)
			{
				DoChainedAnimation_CardDeal(worldPed, 1);
			}
			else if (animation == EBlackJackAnimations.DealCard_Player3 || animation == EBlackJackAnimations.HitCardPlayer3)
			{
				DoChainedAnimation_CardDeal(worldPed, 2);
			}
			else if (animation == EBlackJackAnimations.DealCard_Player4 || animation == EBlackJackAnimations.HitCardPlayer4)
			{
				DoChainedAnimation_CardDeal(worldPed, 3);
			}
			else if (animation == EBlackJackAnimations.DealCard_Dealer)
			{
				DoChainedAnimation_CardDeal(worldPed, -1);
			}
			else if (animation == EBlackJackAnimations.RetrieveCardsPlayer1)
			{
				DoChainedAnimation_CardRetrieve(worldPed, 0);
			}
			else if (animation == EBlackJackAnimations.RetrieveCardsPlayer2)
			{
				DoChainedAnimation_CardRetrieve(worldPed, 1);
			}
			else if (animation == EBlackJackAnimations.RetrieveCardsPlayer3)
			{
				DoChainedAnimation_CardRetrieve(worldPed, 2);
			}
			else if (animation == EBlackJackAnimations.RetrieveCardsPlayer4)
			{
				DoChainedAnimation_CardRetrieve(worldPed, 3);
			}
			else if (animation == EBlackJackAnimations.RetrieveDealerCards)
			{
				DoChainedAnimation_CardRetrieve(worldPed, -1);
			}
		});
	}

	private void DoChainedAnimation_CardRetrieve(CWorldPed worldPed, int participantID)
	{
		RAGE.Elements.MapObject objectAttached = null;
		BlackjackActivityState activityState = GetActivityState();
		BlackjackPlayerState playerState = null;

		if (participantID == -1) // dealer
		{
			playerState = activityState.DealerState;
		}
		else
		{
			playerState = activityState.PlayerStates.ElementAt(participantID).Value;

		}

		int animStatelength = participantID == -1 ? 1000 : 500;

		// get most recent card
		var cardObjs = m_dictCardObjects[participantID];

		var vecOffsetPos = new RAGE.Vector3(0.05f, 0.0f, 0.0f);

		int numCards = cardObjs.Count;
		int cardIndex = cardObjs.Count - 1;

		// after 0.5 sec, start taking the cards back, 200ms per card
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			ClientTimerPool.CreateTimer((object[] parametersInner) =>
			{// attach card to dealer right hand
				var cardObj = cardObjs[cardIndex];
				objectAttached = cardObj;

				var boneRight = 64017; // 18905
				RAGE.Game.Entity.AttachEntityToEntity(cardObj.Handle, worldPed.PedInstance.Handle, worldPed.PedInstance.GetBoneIndex(boneRight), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, 0.0f, 180.0f, 90.0f, true, false, false, false, 0, true);

				--cardIndex;

				if (cardIndex < 0)
				{
					ClientTimerPool.CreateTimer((object[] parametersEnding) =>
					{
						// ending
						ApplyCurrentStateFromLastReplication(activityState.State, activityState.State, false);
					}, 500, 1);
				}
			}, 100, numCards);
		}, animStatelength, 1);
	}

	private void DoChainedAnimation_CardDeal(CWorldPed worldPed, int participantID)
	{
		RAGE.Elements.MapObject objectAttached = null;
		BlackjackActivityState activityState = GetActivityState();
		BlackjackPlayerState playerState = null;

		if (participantID == -1) // dealer
		{
			playerState = activityState.DealerState;
		}
		else
		{
			playerState = activityState.PlayerStates.ElementAt(participantID).Value;

		}

		int animStatelength = participantID == -1 ? 700 : 1000;

		// get most recent card
		var cardObjs = m_dictCardObjects[participantID];
		var cardObj = cardObjs[cardObjs.Count - 1];
		objectAttached = cardObj;

		// attach card to dealer left hand
		var bone = 4090; // 18905
		var vecOffsetPos = new RAGE.Vector3(0.05f, 0.0f, 0.0f);
		RAGE.Game.Entity.AttachEntityToEntity(cardObj.Handle, worldPed.PedInstance.Handle, worldPed.PedInstance.GetBoneIndex(bone), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, 0.0f, 0.0f, 90.0f, true, false, false, false, 0, true);
		cardObj.SetAlpha(255, false);
		// TODO_ACTIVITY_LOW_PRIO: Better way of chaining anims?
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			// attach card to dealer right hand
			var boneRight = 64017; // 18905
			RAGE.Game.Entity.AttachEntityToEntity(cardObj.Handle, worldPed.PedInstance.Handle, worldPed.PedInstance.GetBoneIndex(boneRight), vecOffsetPos.X, vecOffsetPos.Y, vecOffsetPos.Z, 0.0f, 180.0f, 90.0f, true, false, false, false, 0, true);

			// detach card
			ClientTimerPool.CreateTimer((object[] parametersInner) =>
			{
				if (objectAttached != null)
				{
					//RAGE.Game.Entity.DetachEntity(objectAttached.Handle, true, true);
					// fake a state replication so the card ends up on the table
					ApplyCurrentStateFromLastReplication(activityState.State, activityState.State, true);
				}
			}, 1000, 1);
		}, 1000, 1);
	}

	private void UpdateStateMachineOnReplication_ParticipantsAndSpectators(EBlackJackActivityState oldActivityState, EBlackJackActivityState newActivityState)
	{
		BlackjackActivityState activityState = GetActivityState();
		BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];

		switch (activityState.State)
		{
			case EBlackJackActivityState.Inactive:
				{
					break;
				}

			case EBlackJackActivityState.PlaceBets_Init:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.PlaceBet);
					break;
				}

			case EBlackJackActivityState.PlaceBets_Wait:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player1);
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player2);
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player3);
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player4);
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Dealer);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player1);
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player2);
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player3);
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Player4);
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_Camera:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Dealer);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.Player1_MakeChoice:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealerFocusPlayer1_Idle);
					break;
				}

			case EBlackJackActivityState.Player1_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player2_MakeChoice:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealerFocusPlayer2_Idle);
					break;
				}

			case EBlackJackActivityState.Player2_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player3_MakeChoice:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealerFocusPlayer3_Idle);
					break;
				}

			case EBlackJackActivityState.Player3_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player4_MakeChoice:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealerFocusPlayer4_Idle);
					break;
				}

			case EBlackJackActivityState.Player4_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.CheckAndTurnOwnCard_Dealer);
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17:
				{
					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17_Wait:
				{
					// this state means we issued a card
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.DealCard_Dealer);
					break;
				}


			case EBlackJackActivityState.DetermineRoundOutcome:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_Wait:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.RetrieveCardsPlayer1);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player2:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.RetrieveCardsPlayer2);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player3:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.RetrieveCardsPlayer3);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player4:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.RetrieveCardsPlayer4);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Dealer:
				{
					PlayDealerAnim(oldActivityState, newActivityState, EBlackJackAnimations.RetrieveDealerCards);
					break;
				}
		}
	}

	private bool HasParticipant(int index, bool bIncludeNotParticipatingInThisRound)
	{
		bool bHasParticipant = false;

		var activityState = GetActivityState();
		if (index < activityState.PlayerStates.Count)
		{
			var playerState = activityState.PlayerStates.ElementAt(index).Value;

			if (playerState != null)
			{
				if (!bIncludeNotParticipatingInThisRound)
				{
					bHasParticipant = playerState.IsParticipatingInCurrentRound();
				}
				else
				{
					bHasParticipant = true;
				}
			}
		}

		return bHasParticipant;
	}

	private void HideAllUserTimeoutableUIs()
	{
		GenericPromptHelper.OnClose();
		UserInputHelper.UserInput_OnClose();
	}

	private Int64 GetTimeoutForState(EBlackJackActivityState state, out string strWaitingText)
	{
		if (state == EBlackJackActivityState.PlaceBets_Wait)
		{
			strWaitingText = "Waiting on everyone to play their bets";
			return BlackjackTimeouts.timeForBets;
		}
		else if (state == EBlackJackActivityState.DealCard_Player1_1_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player2_1_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player3_1_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player4_1_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player1_2_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player2_2_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player3_2_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Player4_2_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenCardDeals;
		}
		else if (state == EBlackJackActivityState.Player1_Wait)
		{
			if (HasParticipant(0, false))
			{
				strWaitingText = GetLocalPlayerParticipantID() == 0 ? "Waiting on you" : Helpers.FormatString("Waiting on {0}", GetPlayerFromIndex(0).Name);
			}
			else
			{
				strWaitingText = string.Empty;
			}
			return BlackjackTimeouts.timeBetweenActionStates;
		}
		else if (state == EBlackJackActivityState.Player2_Wait)
		{
			if (HasParticipant(1, false))
			{
				strWaitingText = GetLocalPlayerParticipantID() == 1 ? "Waiting on you" : Helpers.FormatString("Waiting on {0}", GetPlayerFromIndex(1).Name);
			}
			else
			{
				strWaitingText = string.Empty;
			}
			return BlackjackTimeouts.timeBetweenActionStates;
		}
		else if (state == EBlackJackActivityState.Player3_Wait)
		{
			if (HasParticipant(2, false))
			{
				strWaitingText = GetLocalPlayerParticipantID() == 2 ? "Waiting on you" : Helpers.FormatString("Waiting on {0}", GetPlayerFromIndex(2).Name);
			}
			else
			{
				strWaitingText = string.Empty;
			}
			return BlackjackTimeouts.timeBetweenActionStates;
		}
		else if (state == EBlackJackActivityState.Player4_Wait)
		{
			if (HasParticipant(3, false))
			{
				strWaitingText = GetLocalPlayerParticipantID() == 3 ? "Waiting on you" : Helpers.FormatString("Waiting on {0}", GetPlayerFromIndex(3).Name);
			}
			else
			{
				strWaitingText = string.Empty;
			}
			return BlackjackTimeouts.timeBetweenActionStates;
		}
		else if (state == EBlackJackActivityState.Dealer_TurnSecondCard_Wait)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToDetermineRoundOutcome;
		}
		else if (state == EBlackJackActivityState.Dealer_StandOn17_Wait)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeBetweenStandOn17Deals;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendInRoundOutcome;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_Wait)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player2)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player3)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player4)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Dealer)
		{
			strWaitingText = String.Empty;
			return BlackjackTimeouts.timeToSpendRetrievingCards;
		}
		else if (state == EBlackJackActivityState.DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound)
		{
			strWaitingText = "Waiting on next round to begin";
			return BlackjackTimeouts.timeToSpendInRoundOutcome;
		}

		strWaitingText = String.Empty;
		return -1;
	}

	private WeakReference<ClientTimer> m_refTimerUpdateTimeout = new WeakReference<ClientTimer>(null);
	private Int64 m_TimeoutMSRemaining = 0;
	private void UpdateTimeRemaining(EBlackJackActivityState newActivityState)
	{
		m_TimeoutMSRemaining = GetTimeoutForState(newActivityState, out string strWaitingText);

		if (m_TimeoutMSRemaining < 0 || String.IsNullOrEmpty(strWaitingText))
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_refTimerUpdateTimeout);

			BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];
			activityManager.UpdateOverlayTimeRemaining(String.Empty);
			activityManager.UpdateOverlayWaitingText(String.Empty);
		}
		else
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_refTimerUpdateTimeout);

			BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];
			activityManager.UpdateOverlayTimeRemaining(Helpers.FormatString("{0} seconds remaining", m_TimeoutMSRemaining / 1000));
			activityManager.UpdateOverlayWaitingText(strWaitingText);

			m_refTimerUpdateTimeout = ClientTimerPool.CreateTimer((object[] parameters) =>
			{
				m_TimeoutMSRemaining -= 1000;
				BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];
				activityManager.UpdateOverlayTimeRemaining(Helpers.FormatString("{0} seconds remaining", m_TimeoutMSRemaining / 1000));
				activityManager.UpdateOverlayWaitingText(strWaitingText);
			}, 1000, (int)m_TimeoutMSRemaining / 1000);
		}
	}

	private void UpdateStateMachineOnReplication_ParticiantsOnly(EBlackJackActivityState oldActivityState, EBlackJackActivityState newActivityState)
	{
		// did the state change?
		if (oldActivityState != newActivityState)
		{
			UpdateTimeRemaining(newActivityState);
		}

		BlackjackActivityState activityState = GetActivityState();
		BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];

		switch (activityState.State)
		{
			case EBlackJackActivityState.Inactive:
				{
					break;
				}

			case EBlackJackActivityState.PlaceBets_Init:
				{
					CameraManager.DeactivateCamera(ECameraID.ACTIVITY);
					activityManager.HideOverlay();
					NetworkEventSender.SendNetworkEvent_Blackjack_PlaceBet_GetDetails();
					break;
				}

			case EBlackJackActivityState.PlaceBets_Wait:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_Camera:
				{
					if (HasParticipant(0, false))
					{
						SetCameraToParticipantCards(0);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_Camera:
				{
					if (HasParticipant(1, false))
					{
						SetCameraToParticipantCards(1);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_Camera:
				{
					if (HasParticipant(2, false))
					{
						SetCameraToParticipantCards(2);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_Camera:
				{
					if (HasParticipant(3, false))
					{
						SetCameraToParticipantCards(3);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_Camera:
				{
					SetCameraToParticipantCards(-1);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_Camera:
				{
					if (HasParticipant(0, false))
					{
						SetCameraToParticipantCards(0);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player1_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_Camera:
				{
					if (HasParticipant(1, false))
					{
						SetCameraToParticipantCards(1);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player2_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_Camera:
				{
					if (HasParticipant(2, false))
					{
						SetCameraToParticipantCards(2);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player3_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_Camera:
				{
					if (HasParticipant(3, false))
					{
						SetCameraToParticipantCards(3);
					}
					break;
				}

			case EBlackJackActivityState.DealCard_Player4_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_Camera:
				{
					SetCameraToParticipantCards(-1);
					break;
				}

			case EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue:
				{
					break;
				}

			case EBlackJackActivityState.Player1_MakeChoice:
				{
					if (HasParticipant(0, false))
					{
						HandlePlayerChoiceMakingState(0);
					}
					else
					{
						HideAllUserTimeoutableUIs();
					}

					break;
				}

			case EBlackJackActivityState.Player1_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player2_MakeChoice:
				{
					if (HasParticipant(1, false))
					{
						HandlePlayerChoiceMakingState(1);
					}
					else
					{
						HideAllUserTimeoutableUIs();
					}

					break;
				}

			case EBlackJackActivityState.Player2_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player3_MakeChoice:
				{
					if (HasParticipant(2, false))
					{
						HandlePlayerChoiceMakingState(2);
					}
					else
					{
						HideAllUserTimeoutableUIs();
					}

					break;
				}

			case EBlackJackActivityState.Player3_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Player4_MakeChoice:
				{
					if (HasParticipant(3, false))
					{
						HandlePlayerChoiceMakingState(3);
					}
					else
					{
						HideAllUserTimeoutableUIs();
					}

					break;
				}

			case EBlackJackActivityState.Player4_Wait:
				{
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard:
				{
					HideAllUserTimeoutableUIs();

					SetCameraToParticipantCards(-1);
					break;
				}

			case EBlackJackActivityState.Dealer_TurnSecondCard_Wait:
				{
					activityManager.UpdateOverlayToParticipant(this, -1, false);
					activityManager.ShowOverlay();
					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17:
				{
					activityManager.UpdateOverlayToParticipant(this, -1, false);
					activityManager.ShowOverlay();
					break;
				}

			case EBlackJackActivityState.Dealer_StandOn17_Wait:
				{
					break;
				}


			case EBlackJackActivityState.DetermineRoundOutcome:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_Wait:
				{
					CameraManager.DeactivateCamera(ECameraID.ACTIVITY);
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player2:
				{

					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player3:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player4:
				{
					break;
				}

			case EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Dealer:
				{
					break;
				}
		}
	}

	private void HandlePlayerChoiceMakingState(int participantID)
	{
		BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];

		SetCameraToParticipant(participantID);
		activityManager.UpdateOverlayToParticipant(this, participantID, participantID == m_localPlayerParticipantID);
		activityManager.ShowOverlay();

		if (participantID != m_localPlayerParticipantID) // if not us, cleanup stray UI's
		{
			HideAllUserTimeoutableUIs();
		}

		/*
		// TODO_ACTIVITY_LOW_PRIO: Only allow hitting up to 5 cards
		if (participantID == m_localPlayerParticipantID)
		{
			if (participantID < activityState.PlayerStates.Count)
			{
				var playerState = activityState.PlayerStates.ElementAt(participantID).Value;
				BlackjackActivityHelpers.CalculateValueOfCards(playerState.Cards, out List<string> lstDisplays, out List<int> lstValues, out List<int> lstValuesWithinMaxRange, out string strValidCardCombinations, out int highestValueWithinMaxRange);

				//GenericPromptHelper.ShowPrompt("Blackjack", Helpers.FormatString("{0}<br>What would you like to do?", strValidCardCombinations), "Stick (keep existing cards)", "Hit Me (get another card)", UIEventID.Blackjack_Action_Stick, UIEventID.Blackjack_Action_HitMe, EPromptPosition.Center, false);
			}
		}
		else // if not us, cleanup stray UI's
		{
			
		}
		*/
	}

	public override void OnStateReplication(string strState)
	{
		EBlackJackActivityState oldActivityState = m_State != null ? GetActivityState().State : EBlackJackActivityState.Inactive;
		m_State = OwlJSON.DeserializeObject<BlackjackActivityState>(strState, EJsonTrackableIdentifier.BlackjackStateReplication);
		EBlackJackActivityState newActivityState = GetActivityState().State;

		ApplyCurrentStateFromLastReplication(oldActivityState, newActivityState, false);
	}

	public override void OnRoundOutcome(string strDealerOutcome, List<string> lstPlayerOutcomes)
	{
		BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];
		activityManager.ShowOutcome(strDealerOutcome, lstPlayerOutcomes);
	}

	protected override void CleanupAllUI()
	{
		BlackjackActivityManager activityManager = (BlackjackActivityManager)ActivitySystem.m_dictActivityManagers[EActivityType.Blackjack];

		if (activityManager != null)
		{
			activityManager.HideOverlay();
			UserInputHelper.UserInput_OnClose();
		}
	}

	public void ApplyCurrentStateFromLastReplication(EBlackJackActivityState oldActivityState, EBlackJackActivityState newActivityState, bool bJustFinishedCardAnim)
	{
		// get the root furniture item
		CPropertyFurnitureInstance rootFurnitureInstance = FurnitureSystem.GetFurnitureItemFromID((uint)m_uniqueIdentifier);
		if (rootFurnitureInstance == null)
		{
			return;
		}

		// root offset for all card rotations
		float[] fRotRootOffsetsByParticipant = new float[ActivityConstants.BlackjackMaxParticipants]
		{
									32.0f,
									68.0f,
									100.0f,
									136.0f
		};

		// array of card positions by player
		Dictionary<int, CardOffset[]> dictCardOffsetPositions = new Dictionary<int, CardOffset[]>
		{
			// DEALER
			{ -1, new CardOffset[ActivityConstants.BlackjackMaxCards]
				{
					new CardOffset(0.0f, -0.1f, 245.0f),
					new CardOffset(0.0f, 0.0f, 245.0f),
					new CardOffset(0.0f, 0.1f, 245.0f),
					new CardOffset(0.0f, 0.2f, 245.0f),
					new CardOffset(0.0f, -0.2f, 245.0f)
				}
			},

			// PLAYER 1
			{ 0, new CardOffset[ActivityConstants.BlackjackMaxCards]
				{
					new CardOffset(2.0f, 0.83f, 225.0f),
					new CardOffset(6.0f, 0.86f, 225.0f),
					new CardOffset(10.0f, 0.89f, 225.0f),
					new CardOffset(14.0f, 0.91f, 225.0f),
					new CardOffset(19.0f, 0.93f, 225.0f)
				}
			},

			// PLAYER 2
			{ 1, new CardOffset[ActivityConstants.BlackjackMaxCards]
				{
					new CardOffset(0.0f, 0.98f, 245.0f),
					new CardOffset(4.0f, 0.98f, 245.0f),
					new CardOffset(8.0f, 0.99f, 245.0f),
					new CardOffset(12.0f, 0.98f, 245.0f),
					new CardOffset(16.0f, 0.98f, 245.0f)
				}
			},

			// PLAYER 3
			{ 2, new CardOffset[ActivityConstants.BlackjackMaxCards]
				{
					new CardOffset(0.0f, 0.98f, 245.0f),
					new CardOffset(4.0f, 0.98f, 245.0f),
					new CardOffset(8.0f, 0.99f, 245.0f),
					new CardOffset(12.0f, 0.98f, 245.0f),
					new CardOffset(16.0f, 0.98f, 245.0f)
				}
			},

			// PLAYER 4
			{ 3, new CardOffset[ActivityConstants.BlackjackMaxCards]
				{
					new CardOffset(0.0f, 0.88f, 245.0f),
					new CardOffset(4.0f, 0.88f, 245.0f),
					new CardOffset(8.0f, 0.89f, 245.0f),
					new CardOffset(12.0f, 0.88f, 245.0f),
					new CardOffset(16.0f, 0.882f, 245.0f)
				}
			},
		};

		// TODO_ACTIVITY_LOW_PRIO: Diff don't clear
		foreach (var kvPair in m_dictCardObjects)
		{
			foreach (var obj in kvPair.Value)
			{
				obj.Destroy();
			}
		}
		m_dictCardObjects.Clear();

		foreach (var obj in m_lstChipObjects)
		{
			obj.Destroy();
		}
		m_lstChipObjects.Clear();

		// render dealer state
		int dealerCardIndex = 0;

		// only render dealer cards if not in collected state
		if (newActivityState < EBlackJackActivityState.DetermineRoundOutcome_WaitingToGotoInactiveAndNextRound)
		{
			foreach (var card in GetActivityState().DealerState.Cards)
			{
				if (ActivityConstants.DictCardObjectHashes.ContainsKey(card.Suite))
				{
					if (ActivityConstants.DictCardObjectHashes[card.Suite].ContainsKey(card.Card))
					{
						string strObjectHash = ActivityConstants.DictCardObjectHashes[card.Suite][card.Card];

						// TODO_ACTIVITY_LOW_PRIO: Do we want async? or just load all cards?
						uint model = HashHelper.GetHashUnsigned(strObjectHash);
						AsyncModelLoader.RequestSyncInstantLoad(model);

						// calculate position for player
						var objectInst = rootFurnitureInstance.m_Object;

						ActivitySystem.GetPedPosition(objectInst, out RAGE.Vector3 vecPos, out float fRotZ);

						fRotZ += 90.0f;

						// base pos
						float fDist = 0.5f;
						float radians = (fRotZ) * (3.14f / 180.0f);
						vecPos.X += (float)Math.Cos(radians) * fDist;
						vecPos.Y += (float)Math.Sin(radians) * fDist;
						vecPos.Z -= 0.05f;

						// now calculate straight line card pos
						fDist = dictCardOffsetPositions[-1][dealerCardIndex].fPosPerCardOffsetsByParticipant;
						radians = (fRotZ + 90.0f) * (3.14f / 180.0f);
						vecPos.X += (float)Math.Cos(radians) * fDist;
						vecPos.Y += (float)Math.Sin(radians) * fDist;

						RAGE.Vector3 vecRot = objectInst.GetRotation(0).CopyVector();

						if (!card.FaceUp)
						{
							vecRot.Y -= 180.0f;
						}

						vecRot.Z = fRotZ - 155.0f + dictCardOffsetPositions[-1][dealerCardIndex].fRotCardOffsetsByParticipant;

						int alpha = 255;
						if (DoesCurrentStateHaveCardAnim() && !bJustFinishedCardAnim)
						{
							// is it the final card?
							if (dealerCardIndex == GetActivityState().DealerState.Cards.Count - 1)
							{
								alpha = 0;
							}
						}

						var Object = new RAGE.Elements.MapObject(model, vecPos, vecRot, alpha, RAGE.Elements.Player.LocalPlayer.Dimension);

						if (!m_dictCardObjects.ContainsKey(-1))
						{
							m_dictCardObjects[-1] = new List<RAGE.Elements.MapObject>();
						}

						m_dictCardObjects[-1].Add(Object);
					}
				}

				++dealerCardIndex;
			}
		}

		// render player states
		int participantID = 0;
		foreach (var playerState in GetActivityState().PlayerStates.Values)
		{
			// shared chips variables
			int[] values = new int[] { 10, 50, 100, 500, 1000, 5000, 10000 };
			int[] numPerChipOtherChips = new int[] { 0, 0, 0, 0, 0, 0, 0 };
			int[] numPerChipBet = new int[] { 0, 0, 0, 0, 0, 0, 0 };

			// render current bet
			// work out how much of each denomination
			int betChips = playerState.CurrentBet;

			for (int i = values.Length - 1; i >= 0; --i)
			{
				int val = values[i];

				int remainder = betChips % val;
				int numThisType = betChips / val;

				numPerChipBet[i] = numThisType;
				betChips = remainder;
			}

			// now create
			int row = 0;
			int col = 0;

			for (int i = 0; i < values.Length; ++i)
			{
				int val = values[i];
				int amount = numPerChipBet[i];

				for (int j = 0; j < amount; ++j)
				{
					string strObjectHash = ActivityConstants.DictChipObjectHashesByValue[val];

					// TODO_ACTIVITY_LOW_PRIO: Do we want async? or just load all cards?
					uint model = HashHelper.GetHashUnsigned(strObjectHash);
					AsyncModelLoader.RequestSyncInstantLoad(model);

					// calculate position for player
					var objectInst = rootFurnitureInstance.m_Object;

					ActivitySystem.GetPedPosition(objectInst, out RAGE.Vector3 vecPos, out float fRotZ);

					fRotZ += fRotRootOffsetsByParticipant[participantID] + (dictCardOffsetPositions[participantID][0].fRotOffsetFromParticipantRoot) - (row * 4.0f);

					//RAGE.Vector3 vecPos = objectInst.Position;
					float fDist = dictCardOffsetPositions[participantID][0].fPosPerCardOffsetsByParticipant - 0.1f + (col * 0.04f);
					float radians = (fRotZ) * (3.14f / 180.0f);
					vecPos.X += (float)Math.Cos(radians) * fDist;
					vecPos.Y += (float)Math.Sin(radians) * fDist;
					vecPos.Z -= 0.05f;

					vecPos.Z += (j * 0.008f);

					RAGE.Vector3 vecRot = objectInst.GetRotation(0).CopyVector();

					vecRot.Z = fRotZ - 155.0f + dictCardOffsetPositions[participantID][0].fRotCardOffsetsByParticipant;

					var Object = new RAGE.Elements.MapObject(model, vecPos, vecRot, 255, RAGE.Elements.Player.LocalPlayer.Dimension);
					m_lstChipObjects.Add(Object);
				}

				if (amount > 0)
				{
					++col;

					if (col >= 3)
					{
						++row;
						col = 0;
					}
				}
			}

			// render chips
			// work out how much of each denomination
			int chips = playerState.Chips;

			for (int i = values.Length - 1; i >= 0; --i)
			{
				int val = values[i];

				int remainder = chips % val;
				int numThisType = chips / val;

				numPerChipOtherChips[i] = numThisType;
				chips = remainder;
			}

			// now create
			row = 0;
			col = 0;

			for (int i = 0; i < values.Length; ++i)
			{
				int val = values[i];
				int amount = numPerChipOtherChips[i];

				for (int j = 0; j < amount; ++j)
				{
					string strObjectHash = ActivityConstants.DictChipObjectHashesByValue[val];

					// TODO_ACTIVITY_LOW_PRIO: Do we want async? or just load all cards?
					uint model = HashHelper.GetHashUnsigned(strObjectHash);
					AsyncModelLoader.RequestSyncInstantLoad(model);

					// calculate position for player
					var objectInst = rootFurnitureInstance.m_Object;

					ActivitySystem.GetPedPosition(objectInst, out RAGE.Vector3 vecPos, out float fRotZ);

					fRotZ += fRotRootOffsetsByParticipant[participantID] + (dictCardOffsetPositions[participantID][0].fRotOffsetFromParticipantRoot) - 7.0f - (row * 4.0f);

					//RAGE.Vector3 vecPos = objectInst.Position;
					float fDist = dictCardOffsetPositions[participantID][0].fPosPerCardOffsetsByParticipant + (col * 0.04f);
					float radians = (fRotZ) * (3.14f / 180.0f);
					vecPos.X += (float)Math.Cos(radians) * fDist;
					vecPos.Y += (float)Math.Sin(radians) * fDist;
					vecPos.Z -= 0.05f;

					vecPos.Z += (j * 0.008f);

					RAGE.Vector3 vecRot = objectInst.GetRotation(0).CopyVector();

					vecRot.Z = fRotZ - 155.0f + dictCardOffsetPositions[participantID][0].fRotCardOffsetsByParticipant;

					// TODO_ACTIVITY_LOW_PRIO: Diff rather than recreate all cards on state changes
					var Object = new RAGE.Elements.MapObject(model, vecPos, vecRot, 255, RAGE.Elements.Player.LocalPlayer.Dimension);
					m_lstChipObjects.Add(Object);
				}

				if (amount > 0)
				{
					++col;

					if (col >= 3)
					{
						++row;
						col = 0;
					}
				}
			}

			// render cards
			// only render player cards if not in collected state
			if (newActivityState > (EBlackJackActivityState.DetermineRoundOutcome_RetrieveCards_Player1 + participantID))
			{
				continue;
			}

			int cardIndex = 0;

			foreach (var card in playerState.Cards)
			{
				if (ActivityConstants.DictCardObjectHashes.ContainsKey(card.Suite))
				{
					if (ActivityConstants.DictCardObjectHashes[card.Suite].ContainsKey(card.Card))
					{
						string strObjectHash = ActivityConstants.DictCardObjectHashes[card.Suite][card.Card];

						// TODO_ACTIVITY_LOW_PRIO: Do we want async? or just load all cards?
						uint model = HashHelper.GetHashUnsigned(strObjectHash);
						AsyncModelLoader.RequestSyncInstantLoad(model);

						// calculate position for player
						var objectInst = rootFurnitureInstance.m_Object;

						ActivitySystem.GetPedPosition(objectInst, out RAGE.Vector3 vecPos, out float fRotZ);

						fRotZ += fRotRootOffsetsByParticipant[participantID] + (dictCardOffsetPositions[participantID][cardIndex].fRotOffsetFromParticipantRoot);

						//RAGE.Vector3 vecPos = objectInst.Position;
						float fDist = dictCardOffsetPositions[participantID][cardIndex].fPosPerCardOffsetsByParticipant;
						float radians = (fRotZ) * (3.14f / 180.0f);
						vecPos.X += (float)Math.Cos(radians) * fDist;
						vecPos.Y += (float)Math.Sin(radians) * fDist;
						vecPos.Z -= 0.05f;

						RAGE.Vector3 vecRot = objectInst.GetRotation(0).CopyVector();

						if (!card.FaceUp)
						{
							vecRot.Y -= 180.0f;
						}

						vecRot.Z = fRotZ - 155.0f + dictCardOffsetPositions[participantID][cardIndex].fRotCardOffsetsByParticipant;

						int alpha = 255;
						if (DoesCurrentStateHaveCardAnim() && !bJustFinishedCardAnim)
						{
							// is it the final card?
							if (cardIndex == playerState.Cards.Count - 1)
							{
								alpha = 0;
							}
						}

						// TODO_ACTIVITY_LOW_PRIO: Diff rather than recreate all cards on state changes
						var Object = new RAGE.Elements.MapObject(model, vecPos, vecRot, alpha, RAGE.Elements.Player.LocalPlayer.Dimension);

						if (!m_dictCardObjects.ContainsKey(participantID))
						{
							m_dictCardObjects[participantID] = new List<RAGE.Elements.MapObject>();
						}

						m_dictCardObjects[participantID].Add(Object);
					}
				}

				++cardIndex;
			}

			++participantID;
		}

		// NOTE: Done last to ensure we have the card objects etc created!
		// NOTE: This is logic around cameras, ui, input etc, so its only for participants!
		if (LocalPlayerIsInTableButMayNotBePlaying())
		{
			UpdateStateMachineOnReplication_ParticiantsOnly(oldActivityState, newActivityState);
		}

		UpdateStateMachineOnReplication_ParticipantsAndSpectators(oldActivityState, newActivityState);
	}

	private bool DoesCurrentStateHaveCardAnim()
	{
		var state = GetActivityState();

		return state.State == EBlackJackActivityState.DealCard_Dealer_1_Camera || state.State == EBlackJackActivityState.DealCard_Dealer_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Player1_1_Camera || state.State == EBlackJackActivityState.DealCard_Player1_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Player2_1_Camera || state.State == EBlackJackActivityState.DealCard_Player2_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Player3_1_Camera || state.State == EBlackJackActivityState.DealCard_Player3_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Player4_1_Camera || state.State == EBlackJackActivityState.DealCard_Player4_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Dealer_1_Camera || state.State == EBlackJackActivityState.DealCard_Dealer_2_Camera ||
			state.State == EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue ||
			state.State == EBlackJackActivityState.DealCard_Player1_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Player1_2_WaitForIssue ||
			state.State == EBlackJackActivityState.DealCard_Player2_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Player2_2_WaitForIssue ||
			state.State == EBlackJackActivityState.DealCard_Player3_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Player3_2_WaitForIssue ||
			state.State == EBlackJackActivityState.DealCard_Player4_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Player4_2_WaitForIssue ||
			state.State == EBlackJackActivityState.DealCard_Dealer_1_WaitForIssue || state.State == EBlackJackActivityState.DealCard_Dealer_2_WaitForIssue ||
			state.State == EBlackJackActivityState.Dealer_StandOn17_Wait;
	}
}