using System;
using System.Collections.Generic;

public class Banking
{
	private List<CreditDetails> g_lstCreditDetails = null;
	private int m_CreditIndex = -1;

	public Banking()
	{
		m_BankingGUI = new CGUIBankingInterface(OnUILoaded);

		RageEvents.RAGE_OnRender += OnRender;

		NetworkEvents.CreateBankPed += OnCreateBankPed;
		NetworkEvents.DestroyBankPed += OnDestroyBankPed;
		NetworkEvents.Banking_RequestInfoResponse += OnRequestInfoResponse;
		NetworkEvents.Banking_GotAccountInfo += OnGotAccountInfo;
		NetworkEvents.ChangeCharacterApproved += HideBankingUI;
		NetworkEvents.ShowMobileBankUI += OnShowMobileBankUI;


		UIEvents.OnSwitchAccount += OnSwitchAccount;
		UIEvents.Banking_OnSwitchCredit += OnSwitchCredit;
		UIEvents.Banking_OnHide += HideBankingUI;

		UIEvents.Banking_OnWithdraw += OnWithdraw;
		UIEvents.Banking_OnDeposit += OnDeposit;
		UIEvents.Banking_OnWireTransfer += OnWireTransfer;
		UIEvents.Banking_PayDownDebt += OnPayDownDebt;

		UIEvents.OnWireTransferKeyboard_Submit += OnWireTransferKeyboard_Submit;
		UIEvents.OnWireTransferKeyboard_Cancel += OnWireTransferKeyboard_Cancel;

		NetworkEvents.Banking_RefreshCreditInfo += OnRefreshCreditInfo;

		NetworkEvents.Banking_OnServerResponse += OnServerResponse;
	}

	private void OnRender()
	{
		if (CanShowBankingUI())
		{
			PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.ATM);

			if (poolEntry != null)
			{
				RAGE.Elements.Marker nearestMarker = poolEntry.GetEntity<RAGE.Elements.Marker>();
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Use ATM", OnRenderATMWorldHint, OnInteractWithBankMarker, nearestMarker.Position, nearestMarker.Dimension, false, false, g_fDistThreshold);
			}
		}
	}

	private void OnRenderATMWorldHint()
	{

	}

	private void OnWireTransferKeyboard_Submit(string strInput)
	{
		if (!m_bPendingTransaction)
		{
			if (m_AccountIndex < m_lstAccounts.Count)
			{
				SetPendingTransaction(true);
				m_bPendingShow = true;
				Purchaser account = m_lstAccounts[m_AccountIndex];
				NetworkEventSender.SendNetworkEvent_Banking_OnWireTransfer(strInput, m_fWireTransferAmount, account.Type, account.ID);
				m_fWireTransferAmount = 0.0f;
			}
		}
	}

	private void OnWireTransferKeyboard_Cancel()
	{
		// Nothing actually happened, just reset amount and show the UI again
		SetPendingTransaction(false);
		ShowBankingUI(false);
		m_fWireTransferAmount = 0.0f;
	}

	private void OnRefreshCreditInfo(List<CreditDetails> lstCreditDetails)
	{
		m_BankingGUI.ResetCredits(false);
		HandleCredit(lstCreditDetails);

		// restore previously selected credit info
		OnSwitchCredit(m_CreditIndex);
	}

	private void OnServerResponse(EBankingResponseCode result)
	{
		SetPendingTransaction(false);

		// Force an update of balance etc
		OnSwitchAccount(m_AccountIndex);

		if (result == EBankingResponseCode.Success)
		{
			// Nothing to do, server sent notification, pending transaction flag set above
		}
		else if (result == EBankingResponseCode.Failed_CannotAfford)
		{
			NotificationManager.ShowNotification("Bank", "You do not have enough money to perform that action.", ENotificationIcon.ExclamationSign);
		}
		else if (result == EBankingResponseCode.Failed_CharacterBelongsToSameAccount)
		{
			NotificationManager.ShowNotification("Bank", "You cannot transfer money to a character which belongs to your account.", ENotificationIcon.ExclamationSign);
		}
		else if (result == EBankingResponseCode.Failed_TargetDoesntExist)
		{
			NotificationManager.ShowNotification("Bank", "No character or faction was found with that name.", ENotificationIcon.ExclamationSign);
		}

		if (m_bPendingShow)
		{
			m_bPendingShow = false;
			ShowBankingUI(false);
		}
	}

	private void SetPendingTransaction(bool bPendingTransaction)
	{
		m_bPendingTransaction = bPendingTransaction;
		m_BankingGUI.SetButtonsEnabled(!m_bPendingTransaction);
	}

	private void OnPayDownDebt(float fAmount)
	{
		if (!m_bPendingTransaction)
		{
			if (m_CreditIndex < g_lstCreditDetails.Count && m_AccountIndex < m_lstAccounts.Count)
			{
				CreditDetails details = g_lstCreditDetails[m_CreditIndex];
				Purchaser account = m_lstAccounts[m_AccountIndex];

				NetworkEventSender.SendNetworkEvent_Banking_OnPayDownDebt(account.Type, account.ID, details.CreditType, details.ID, fAmount);
			}
		}
	}

	private void OnWireTransfer(float fAmount)
	{
		if (!m_bPendingTransaction)
		{
			m_fWireTransferAmount = fAmount;
			HideBankingUI();

			UserInputHelper.RequestUserInput("Wire Transfer", "Enter the Full Name of the Recipient", "Forename Surname or organization name.", UIEventID.OnWireTransferKeyboard_Submit, UIEventID.OnWireTransferKeyboard_Cancel);
		}
	}

	private void OnWithdraw(float fAmount)
	{
		if (!m_bPendingTransaction)
		{
			if (m_AccountIndex < m_lstAccounts.Count)
			{
				SetPendingTransaction(true);
				Purchaser account = m_lstAccounts[m_AccountIndex];
				NetworkEventSender.SendNetworkEvent_Banking_OnWithdraw(fAmount, account.Type, account.ID);
			}
		}
	}

	private void OnDeposit(float fAmount)
	{
		if (!m_bPendingTransaction)
		{
			if (m_AccountIndex < m_lstAccounts.Count)
			{
				SetPendingTransaction(true);
				Purchaser account = m_lstAccounts[m_AccountIndex];
				NetworkEventSender.SendNetworkEvent_Banking_OnDeposit(fAmount, account.Type, account.ID);
			}
		}
	}

	private void OnSwitchAccount(int index)
	{
		if (!m_bPendingTransaction)
		{
			if (index < m_lstAccounts.Count)
			{
				m_AccountIndex = index;
				Purchaser account = m_lstAccounts[index];
				NetworkEventSender.SendNetworkEvent_Banking_GetAccountInfo(account.Type, account.ID);
			}
		}
	}

	private void OnSwitchCredit(int index)
	{
		if (!m_bPendingTransaction)
		{
			if (index < g_lstCreditDetails.Count)
			{
				m_CreditIndex = index;
				CreditDetails details = g_lstCreditDetails[index];
				m_BankingGUI.SetCreditInfo(details.strDisplayName, details.numPaymentsRemaining, details.numPaymentsMade, details.fAmount, details.fInterest);

				m_BankingGUI.SetButtonsEnabled(true);
			}
		}
	}

	private void UpdateBankBalance(object[] parameters)
	{
		// Auto refresh for player balance
		if (m_BankingGUI != null && m_AccountIndex == 0)
		{
			float fNewBankMoney = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.BANK_MONEY);
			m_BankingGUI.SetBalance(fNewBankMoney);
		}
	}

	private void HideBankingUI()
	{
		if (m_BankingGUI != null)
		{
			m_BankingGUI.SetVisible(false, false, false);
			SetPendingTransaction(false);
		}

		if (m_UpdateBankBalanceTimer.Instance() != null)
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_UpdateBankBalanceTimer);
		}
	}

	private void OnGotAccountInfo(float fMoney, List<CreditDetails> lstCreditDetails)
	{
		m_BankingGUI.SetButtonsEnabled(true);
		m_BankingGUI.SetBalance(fMoney);

		m_BankingGUI.ResetCredits(true);
		HandleCredit(lstCreditDetails);
	}

	private void HandleCredit(List<CreditDetails> lstCreditDetails)
	{
		g_lstCreditDetails = lstCreditDetails;
		foreach (CreditDetails details in lstCreditDetails)
		{
			m_BankingGUI.AddCredit(details.strDisplayName);
		}
	}

	private void OnCreateBankPed(RAGE.Vector3 vecPos, float fRotZ, uint dimension)
	{
		WeakReference<CWorldPed> refWorldPed = WorldPedManager.CreatePed(EWorldPedType.BankTeller, 2426248831, vecPos, fRotZ, dimension);
		refWorldPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Talk to Bank Teller", null, OnInteractWithBankPed, false, false, 3.0f, null, true);
		m_lstWorldPeds.Add(refWorldPed);
	}

	private void OnDestroyBankPed(RAGE.Vector3 vecPos, float fRotZ, uint dimension)
	{
		foreach (var pedRef in m_lstWorldPeds)
		{
			CWorldPed ped = pedRef.Instance();
			if (ped != null)
			{
				if (ped.Position == vecPos && ped.RotZ == fRotZ && ped.Dimension == dimension)
				{
					WorldPedManager.DestroyPed(ped);
				}
			}
		}
	}

	private void OnRequestInfoResponse(List<Purchaser> lstPurchasers, List<string> lstMethods)
	{
		m_lstAccounts = lstPurchasers;

		bool bAddedSpacer = false;

		foreach (Purchaser purchaser in lstPurchasers)
		{
			if (purchaser.Type == EPurchaserType.Faction && !bAddedSpacer)
			{
				bAddedSpacer = true;
				m_BankingGUI.AddDivider();
			}

			m_BankingGUI.AddAccount(purchaser.DisplayName);
		}

		m_BankingGUI.CommitAccounts();
		m_BankingGUI.SetAccount(0);
	}

	private bool CanShowBankingUI()
	{
		if (m_BankingGUI == null)
		{
			return false;
		}

		return !m_BankingGUI.IsVisible();
	}

	private void OnInteractWithBankPed()
	{
		// TODO: Server verify pos
		if (KeyBinds.CanProcessKeybinds() && CanShowBankingUI())
		{
			ShowBankingUI(false);
		}
	}

	private void OnInteractWithBankMarker()
	{
		// TODO: Server verify pos
		if (KeyBinds.CanProcessKeybinds() && CanShowBankingUI())
		{
			ShowBankingUI(true);
		}
	}

	private void OnShowMobileBankUI()
	{
		ShowBankingUI(false);
		m_BankingGUI.Execute("EnableMobileBanking");

	}

	private void OnUILoaded()
	{

	}

	private void ShowBankingUI(bool bPlaySoundEffect)
	{
		m_BankingGUI.SetVisible(true, true, false);


		m_BankingGUI.Reset();

		NetworkEventSender.SendNetworkEvent_GetPurchaserAndPaymentMethods(EPurchaseAndPaymentMethodsRequestType.Bank);

		m_lstAccounts.Clear();
		m_AccountIndex = -1;

		if (bPlaySoundEffect)
		{
			RAGE.Game.Audio.PlaySoundFrontend(-1, "ATM_WINDOW", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
		}

		if (m_UpdateBankBalanceTimer.Instance() != null)
		{
			ClientTimerPool.MarkTimerForDeletion(ref m_UpdateBankBalanceTimer);
		}

		m_UpdateBankBalanceTimer = ClientTimerPool.CreateTimer(UpdateBankBalance, 2000);

		// Make sure the ATM buttons are there because of mobile banking
		m_BankingGUI.Execute("DisableMobileBanking");
	}

	private CGUIBankingInterface m_BankingGUI = null;
	private List<WeakReference<CWorldPed>> m_lstWorldPeds = new List<WeakReference<CWorldPed>>();

	private List<Purchaser> m_lstAccounts = new List<Purchaser>();
	private int m_AccountIndex = -1;

	private const float g_fDistThreshold = 3.0f;
	private float m_fWireTransferAmount = 0.0f;
	private bool m_bPendingShow = false;
	private bool m_bPendingTransaction = false;
	private WeakReference<ClientTimer> m_UpdateBankBalanceTimer = new WeakReference<ClientTimer>(null);
}