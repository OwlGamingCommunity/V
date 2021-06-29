using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class ActivitySystem
{
	public static WeakReference<CPropertyFurnitureInstance> m_CurrentActivityFurnitureObject = new WeakReference<CPropertyFurnitureInstance>(null);
	private static EActivityType m_CurrentActivityType = EActivityType.None;
	private static Int64 m_CurrentUniqueIdentifier = -1;

	private static bool m_bIsInIntroPrompt = false;

	private int currentManagementChips = 0;

	public static Dictionary<EntityDatabaseID, WeakReference<CWorldPed>> g_dictActivityPeds = new Dictionary<EntityDatabaseID, WeakReference<CWorldPed>>();

	public readonly static Dictionary<EActivityType, ActivityManager> m_dictActivityManagers = new Dictionary<EActivityType, ActivityManager>()
	{
		{ EActivityType.Blackjack, new BlackjackActivityManager() }
	};

	public ActivitySystem()
	{
		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.ChangeCharacterApproved += LeaveActivity;

		NetworkEvents.CreateFurnitureItemFromCache += OnCreateFurnitureItemFromCache;
		NetworkEvents.DestroyFurnitureItemFromCache += OnDestroyFurnitureItemFromCache;

		NetworkEvents.StartActivityApproved += OnStartActivityApproved;
		NetworkEvents.ReplicateActivityState += OnReplicateActivityState;

		CameraManager.RegisterCamera(ECameraID.ACTIVITY, new RAGE.Vector3(), new RAGE.Vector3());

		NetworkEvents.ActivityRequestInteract_Response += OnActivityInteract_Response;

		UIEvents.ActivityInteraction_Dropdown_Done += ActivityInteraction_Dropdown_Done;
		UIEvents.ActivityInteraction_Dropdown_Cancel += ActivityInteraction_Dropdown_Cancel;
		UIEvents.ActivityInteraction_Dropdown_DropdownSelectionChanged += ActivityInteraction_Dropdown_SelectionChanged;

		UIEvents.ChipManagement_Buy_Submit += OnChipManagement_Buy_Submit;
		UIEvents.ChipManagement_Buy_Cancel += OnChipManagement_Buy_Cancel;
		UIEvents.ChipManagement_Sell_Submit += OnChipManagement_Sell_Submit;
		UIEvents.ChipManagement_Sell_Cancel += OnChipManagement_Sell_Cancel;

		UIEvents.CasinoManagement_Add_Submit += OnCasinoManagement_Add_Submit;
		UIEvents.CasinoManagement_Add_Cancel += OnCasinoManagement_Add_Cancel;
		UIEvents.CasinoManagement_Take_Submit += OnCasinoManagement_Take_Submit;
		UIEvents.CasinoManagement_Take_Cancel += OnCasinoManagement_Take_Cancel;

		NetworkEvents.ChipManagement_Buy_GotDetails += OnBuyChips_GotDetails;
		NetworkEvents.ChipManagement_Sell_GotDetails += OnSellChips_GotDetails;
		NetworkEvents.CasinoManagement_GotDetails += OnCasinoManagement_GotDetails;

		UIEvents.CasinoManagement_AddCurrency += OnCasinoManagement_AddCurrency;
		UIEvents.CasinoManagement_TakeCurrency += OnCasinoManagement_TakeCurrency;
		UIEvents.CasinoManagement_Exit += OnCasinoManagement_Exit;

		NetworkEvents.ResetActivityState += OnResetActivityState;

		NetworkEvents.Activity_RoundOutcome += OnRoundOutcome;
	}

	private void OnChipManagement_Buy()
	{
		NetworkEventSender.SendNetworkEvent_ChipManagement_Buy_GetDetails();
	}

	private void OnChipManagement_Sell()
	{
		NetworkEventSender.SendNetworkEvent_ChipManagement_Sell_GetDetails();
	}

	private void GotoManageCasino()
	{
		NetworkEventSender.SendNetworkEvent_CasinoManagement_GetDetails(m_CurrentActivityFurnitureObject.Instance().DBID);
	}

	private void ActivityInteraction_Dropdown_Done(string strName, string strValue)
	{
		EActivityInteractionDropdownActions action = (EActivityInteractionDropdownActions)Enum.Parse(typeof(EActivityInteractionDropdownActions), strValue);
		m_bIsInIntroPrompt = false;

		if (action == EActivityInteractionDropdownActions.BuyChips)
		{
			OnChipManagement_Buy();
		}
		else if (action == EActivityInteractionDropdownActions.SellChips)
		{
			OnChipManagement_Sell();
		}
		else if (action == EActivityInteractionDropdownActions.Play)
		{
			NetworkEventSender.SendNetworkEvent_RequestStartActivity(m_CurrentActivityFurnitureObject.Instance().DBID, m_CurrentActivityType);
			m_bIsInIntroPrompt = false;
		}
		else if (action == EActivityInteractionDropdownActions.Management)
		{
			GotoManageCasino();
		}
	}

	private void ActivityInteraction_Dropdown_SelectionChanged(string strName, string strValue)
	{

	}

	private void ActivityInteraction_Dropdown_Cancel()
	{
		ResetActivityFurnitureAndTypeState();
		m_bIsInIntroPrompt = false;
	}

	public void Init()
	{

	}

	public static void GetPedPosition(RAGE.Elements.MapObject objectInst, out RAGE.Vector3 vecPos, out float fRotZ)
	{
		fRotZ = objectInst.GetRotation(0).Z - 180.0f;
		vecPos = objectInst.Position.CopyVector();
		float fDist = -0.75f;
		float radians = (fRotZ + 90.0f) * (3.14f / 180.0f);
		vecPos.X += (float)Math.Cos(radians) * fDist;
		vecPos.Y += (float)Math.Sin(radians) * fDist;
		vecPos.Z += 1.0f;
	}

	private void OnCreateFurnitureItemFromCache(CFurnitureDefinition furnitureDef, RAGE.Elements.MapObject objectInst, EntityDatabaseID DBID)
	{
		if (furnitureDef.Activity != EActivityType.None)
		{
			if (!g_dictActivityPeds.ContainsKey(DBID))
			{
				// BLACKJACK
				if (furnitureDef.Activity == EActivityType.Blackjack)
				{
					// create instance
					// get the manager
					m_dictActivityManagers[EActivityType.Blackjack].CreateNewInstance(DBID);

					// Request sub
					NetworkEventSender.SendNetworkEvent_RequestSubscribeActivity(DBID, EActivityType.Blackjack);

					GetPedPosition(objectInst, out RAGE.Vector3 vecPos, out float fRotZ);
					var newPed = WorldPedManager.CreatePed(EWorldPedType.Activity, HashHelper.GetHashUnsigned("s_f_y_casino_01"), vecPos, fRotZ, objectInst.Dimension);
					g_dictActivityPeds[DBID] = newPed;

					AsyncAnimLoader.RequestAsyncLoad("anim_casino_b@amb@casino@games@blackjack@dealer", (string strDictionary) =>
					{
						if (newPed != null && newPed.Instance() != null && newPed.Instance().PedInstance != null)
						{
							newPed.Instance().PedInstance.TaskPlayAnim(strDictionary, "idle", 8.0f, 1.0f, -1, (int)(AnimationFlags.Loop), 0.0f, false, false, false);
						}
					});
				}
			}
		}
	}

	private void OnDestroyFurnitureItemFromCache(CFurnitureDefinition furnitureDef, RAGE.Elements.MapObject objectInst, EntityDatabaseID DBID)
	{
		if (g_dictActivityPeds.ContainsKey(DBID))
		{
			// Request unsub
			NetworkEventSender.SendNetworkEvent_RequestUnsubscribeActivity(DBID, EActivityType.Blackjack);

			WorldPedManager.DestroyPed(g_dictActivityPeds[DBID].Instance());
			g_dictActivityPeds.Remove(DBID);
		}
	}

	private void OnRender()
	{
		if (!ItemSystem.GetPlayerInventory().IsVisible() && RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			CPropertyFurnitureInstance furnitureInstance = FurnitureSystem.GetNearestStreamedFurnitureItemWithActivity(out EActivityType activityType);

			if (furnitureInstance != null)
			{
				RAGE.Vector3 vecPos = furnitureInstance.m_Object.Position;
				vecPos.Z += 1.0f;

				if (!m_bIsInIntroPrompt)
				{
					WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), (m_CurrentActivityFurnitureObject.Instance() != null && m_CurrentUniqueIdentifier != -1) ? "Leave Table" : Helpers.FormatString("Play {0}", activityType.ToString()), null, () => { InteractWithActivity(activityType, furnitureInstance); }, vecPos, furnitureInstance.m_Object.Dimension, false, false, ItemConstants.g_fDistFurnitureStorageThreshold, new RAGE.Vector3(0.0f, 0.0f, 0.0f), true, false);
				}
			}
		}

		/*
		if (m_CurrentActivityFurnitureObject.Instance() != null)
		{
			var objectInst = m_CurrentActivityFurnitureObject.Instance().m_Object;

			for (int participantID = 0; participantID < 4; ++participantID)
			{
				float fRotZ = objectInst.GetRotation(0).Z + 10.0f;

				fRotZ += (participantID + 1) * 65.0f;

				RAGE.Vector3 vecPos = objectInst.Position;
				float fDist = 1.0f;
				float radians = (fRotZ + 90.0f) * (3.14f / 180.0f);
				vecPos.X += (float)Math.Cos(radians) * fDist;
				vecPos.Y += (float)Math.Sin(radians) * fDist;
				vecPos.Z += 1.0f;

				int r = 255;
				int g = 255;
				int b = 255;

				if (participantID == 1)
				{
					r = 255;
					g = 0;
					b = 0;
				}
				else if (participantID == 2)
				{
					r = 0;
					g = 255;
					b = 0;
				}
				else if (participantID == 3)
				{
					r = 0;
					g = 0;
					b = 255;
				}

				RAGE.Game.Graphics.DrawBox(vecPos.X, vecPos.Y, vecPos.Z, vecPos.X + 0.3f, vecPos.Y + 0.3f, vecPos.Z + 0.3f, r, g, b, 255);
			}
		}
		*/
	}

	private void LeaveActivity()
	{
		if (m_CurrentActivityFurnitureObject.Instance() != null)
		{
			NetworkEventSender.SendNetworkEvent_RequestStopActivity(m_CurrentActivityFurnitureObject.Instance().DBID, m_CurrentActivityType);

			var objectInst = m_CurrentActivityFurnitureObject.Instance().m_Object;

			float fRotZ = objectInst.GetRotation(0).Z - 180.0f;
			RAGE.Vector3 vecPos = objectInst.Position;
			float fDist = 1.25f;
			float radians = (fRotZ + 90.0f) * (3.14f / 180.0f);
			vecPos.X += (float)Math.Cos(radians) * fDist;
			vecPos.Y += (float)Math.Sin(radians) * fDist;
			vecPos.Z += 1.0f;

			RAGE.Elements.Player.LocalPlayer.SetNoCollisionEntity(objectInst.Handle, false);
			RAGE.Elements.Player.LocalPlayer.Position = vecPos;
		}

		ResetActivityFurnitureAndTypeState();
	}

	private void OnResetActivityState()
	{
		ResetActivityFurnitureAndTypeState();
	}

	private enum EActivityInteractionDropdownActions
	{
		BuyChips,
		SellChips,
		Management,
		Play
	}

	private void OnActivityInteract_Response(bool bIsManager)
	{
		Dictionary<string, string> dictDropdownItems = new Dictionary<string, string>
			{
				{ "Play", EActivityInteractionDropdownActions.Play.ToString() },
				{ "Buy Chips", EActivityInteractionDropdownActions.BuyChips.ToString() },
				{ "Sell Chips", EActivityInteractionDropdownActions.SellChips.ToString() },
			};

		if (bIsManager)
		{
			dictDropdownItems.Add("Management Actions", EActivityInteractionDropdownActions.Management.ToString());
		}

		GenericDropdown.ShowGenericDropdown("Select Action", dictDropdownItems, m_CurrentActivityType.ToString(), "What would you like to do?", "Confirm", "Exit",
			UIEventID.ActivityInteraction_Dropdown_Done, UIEventID.ActivityInteraction_Dropdown_Cancel, UIEventID.ActivityInteraction_Dropdown_DropdownSelectionChanged, EPromptPosition.Center);
	}

	private void InteractWithActivity(EActivityType activityType, CPropertyFurnitureInstance furnitureInstance)
	{
		if (m_CurrentActivityFurnitureObject.Instance() != null)
		{
			LeaveActivity();
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_ActivityRequestInteract();

			// store immediately
			m_bIsInIntroPrompt = true;
			m_CurrentActivityFurnitureObject.SetTarget(furnitureInstance);
			m_CurrentActivityType = activityType;
		}
	}

	private void ResetActivityFurnitureAndTypeState()
	{
		// get the manager
		if (m_CurrentActivityType != EActivityType.None && m_CurrentUniqueIdentifier >= 0)
		{
			m_dictActivityManagers[m_CurrentActivityType].StopActivity(m_CurrentUniqueIdentifier);
		}

		m_CurrentActivityFurnitureObject.SetTarget(null);
		m_CurrentActivityType = EActivityType.None;
		m_bIsInIntroPrompt = false;
	}

	private void OnStartActivityApproved(int participantID, Int64 uniqueIdentifier, EActivityType activityType)
	{
		if (m_CurrentActivityFurnitureObject.Instance() != null)
		{
			// TODO_ACTIVITY_LOW_PRIO: Make generic / part of class
			var objectInst = m_CurrentActivityFurnitureObject.Instance().m_Object;

			float fRotZ = objectInst.GetRotation(0).Z + 20.0f;

			float[] fRotsByParticipant = new float[]
			{
				70.0f,
				125.0f,
				195.0f,
				250.0f
			};

			fRotZ += fRotsByParticipant[participantID];

			float[] fDistsByParticipant = new float[]
			{
				1.05f,
				0.8f,
				0.8f,
				1.0f
			};

			RAGE.Vector3 vecPos = objectInst.Position;
			float fDist = fDistsByParticipant[participantID];
			float radians = (fRotZ + 90.0f) * (3.14f / 180.0f);
			vecPos.X += (float)Math.Cos(radians) * fDist;
			vecPos.Y += (float)Math.Sin(radians) * fDist;
			vecPos.Z += 1.5f;

			// fix rotation
			RAGE.Elements.Player.LocalPlayer.SetNoCollisionEntity(objectInst.Handle, true);
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, fRotZ - 175.0f, 0, true);
			RAGE.Elements.Player.LocalPlayer.Position = vecPos;

			m_CurrentUniqueIdentifier = uniqueIdentifier;

			// get the manager
			m_dictActivityManagers[activityType].OnStartActivity(uniqueIdentifier, out ActivityInstance outInstance, participantID);
		}
	}

	private void OnReplicateActivityState(Int64 uniqueActivityIdentifier, EActivityType activityType, string strState)
	{
		// get the manager
		m_dictActivityManagers[activityType].OnStateReplication(uniqueActivityIdentifier, strState);
	}

	private void OnRoundOutcome(Int64 uniqueActivityIdentifier, EActivityType activityType, string strDealerOutcome, List<string> lstPlayerOutcomes)
	{
		// get the manager
		m_dictActivityManagers[activityType].OnRoundOutcome(uniqueActivityIdentifier, strDealerOutcome, lstPlayerOutcomes);
	}

	private void OnCasinoManagement_GotDetails(int chips)
	{
		currentManagementChips = chips;
		GenericPrompt3Helper.ShowPrompt("Management", Helpers.FormatString("This table has ${0}. What would you like to do?", chips), "Add Currency", "Take Currency", "Exit",
			UIEventID.CasinoManagement_AddCurrency, UIEventID.CasinoManagement_TakeCurrency, UIEventID.CasinoManagement_Exit);
	}

	private void OnCasinoManagement_AddCurrency()
	{
		UserInputHelper.RequestUserInput("Management", Helpers.FormatString("This table has ${0}. How much would you like to add?", currentManagementChips), "0", UIEventID.CasinoManagement_Add_Submit, UIEventID.CasinoManagement_Add_Cancel);
	}

	private void OnCasinoManagement_TakeCurrency()
	{
		UserInputHelper.RequestUserInput("Management", Helpers.FormatString("This table has ${0}. How much would you like to withdraw?", currentManagementChips), "0", UIEventID.CasinoManagement_Take_Submit, UIEventID.CasinoManagement_Take_Cancel);
	}

	private void OnCasinoManagement_Exit()
	{
		ResetActivityFurnitureAndTypeState();
	}

	private void OnBuyChips_GotDetails(int chips)
	{
		UserInputHelper.RequestUserInput("Buy Chips", Helpers.FormatString("You have {0} chips. How many chips would you like to buy? (1 chip = $1)", chips), "0", UIEventID.ChipManagement_Buy_Submit, UIEventID.ChipManagement_Buy_Cancel);
	}

	private void OnSellChips_GotDetails(int chips)
	{
		UserInputHelper.RequestUserInput("Sell Chips", Helpers.FormatString("You have {0} chips. How many chips would you like to sell? (1 chip = $1)", chips), "0", UIEventID.ChipManagement_Sell_Submit, UIEventID.ChipManagement_Sell_Cancel);
	}

	private void OnChipManagement_Buy_Submit(string strInput)
	{
		if (int.TryParse(strInput, out int amount) && amount > 0)
		{
			NetworkEventSender.SendNetworkEvent_ChipManagement_Buy(amount);
		}
		else
		{
			NotificationManager.ShowNotification("Chip Management", "Please enter a valid number.", ENotificationIcon.ExclamationSign);
			OnChipManagement_Buy();
		}

		ResetActivityFurnitureAndTypeState();
	}

	private void OnChipManagement_Buy_Cancel()
	{
		ResetActivityFurnitureAndTypeState();
	}

	private void OnChipManagement_Sell_Submit(string strInput)
	{
		if (int.TryParse(strInput, out int amount) && amount > 0)
		{
			NetworkEventSender.SendNetworkEvent_ChipManagement_Sell(amount);
		}
		else
		{
			NotificationManager.ShowNotification("Chip Management", "Please enter a valid number.", ENotificationIcon.ExclamationSign);
			OnChipManagement_Sell();
		}

		ResetActivityFurnitureAndTypeState();
	}

	private void OnChipManagement_Sell_Cancel()
	{
		ResetActivityFurnitureAndTypeState();
	}

	// management actions
	private void OnCasinoManagement_Add_Submit(string strInput)
	{
		if (int.TryParse(strInput, out int amount) && amount > 0)
		{
			NetworkEventSender.SendNetworkEvent_CasinoManagement_Add(m_CurrentActivityFurnitureObject.Instance().DBID, amount);
		}
		else
		{
			NotificationManager.ShowNotification("Management", "Please enter a valid number.", ENotificationIcon.ExclamationSign);
			OnCasinoManagement_AddCurrency();
		}

		ResetActivityFurnitureAndTypeState();
	}

	private void OnCasinoManagement_Add_Cancel()
	{
		ResetActivityFurnitureAndTypeState();
	}

	private void OnCasinoManagement_Take_Submit(string strInput)
	{
		if (int.TryParse(strInput, out int amount) && amount > 0)
		{
			NetworkEventSender.SendNetworkEvent_CasinoManagement_Take(m_CurrentActivityFurnitureObject.Instance().DBID, amount);
		}
		else
		{
			NotificationManager.ShowNotification("Management", "Please enter a valid number.", ENotificationIcon.ExclamationSign);
			OnCasinoManagement_TakeCurrency();
		}

		ResetActivityFurnitureAndTypeState();
	}

	private void OnCasinoManagement_Take_Cancel()
	{
		ResetActivityFurnitureAndTypeState();
	}
}