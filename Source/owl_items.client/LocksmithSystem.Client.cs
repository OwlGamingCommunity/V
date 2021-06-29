public class LockSmith
{
	private EItemID m_KeyType = EItemID.VEHICLE_KEY;

	public LockSmith()
	{
		var weakRefPed = WorldPedManager.CreatePed(EWorldPedType.LockSmith, 0x99BB00F8, new RAGE.Vector3(248.25761f, 209.79723f, 106.28681f), -19.250969f, 1223);
		weakRefPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Talk to Locksmith", null, () => { OnInteractWithLockSmith(); }, false, false, 3.0f);

		// remove two ugly ass chairs
		RAGE.Game.Entity.CreateModelHide(248.18767f, 212.20592f, 106.28957f, 3.0f, 0xEFA04C50, true);

		UIEvents.OnLocksmithRequest_Submit += OnSubmit;
		UIEvents.OnLocksmithRequest_Cancel += OnCancel;

		UIEvents.OnLocksmithRequestChoose_Vehicle += OnChooseVehicleKey;
		UIEvents.OnLocksmithRequestChoose_Property += OnChoosePropertyKey;
	}

	private void OnInteractWithLockSmith()
	{
		bool bPendingKeyPickup = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.LOCKSMITH_PENDING_PICKUP);

		if (!bPendingKeyPickup)
		{
			GenericPromptHelper.ShowPrompt("Confirm Key Type", "Choose the key type you want to make a copy of.", "Vehicle Key", "Property Key", UIEventID.OnLocksmithRequestChoose_Vehicle, UIEventID.OnLocksmithRequestChoose_Property);
		}
		else
		{
			NetworkEventSender.SendNetworkEvent_LocksmithOnPickupKeys();
		}
	}

	private void OnSubmit(string strInput)
	{
		bool bInputOk = int.TryParse(strInput, out int numInput);

		if (bInputOk)
		{
			NetworkEventSender.SendNetworkEvent_LocksmithRequestDuplication(m_KeyType.ToString(), numInput);
		}
		else
		{
			NotificationManager.ShowNotification("Locksmith", "Input was not in correct format.", ENotificationIcon.ExclamationSign);

			if (m_KeyType == EItemID.VEHICLE_KEY)
			{
				OnChooseVehicleKey();
			}
			else
			{
				OnChoosePropertyKey();
			}
		}
	}

	private void OnChooseVehicleKey()
	{
		m_KeyType = EItemID.VEHICLE_KEY;
		UserInputHelper.RequestUserInput("Locksmith", "Enter a vehicle key ID to duplicate.", "Key ID", UIEventID.OnLocksmithRequest_Submit, UIEventID.OnLocksmithRequest_Cancel);
	}

	private void OnChoosePropertyKey()
	{
		m_KeyType = EItemID.PROPERTY_KEY;
		UserInputHelper.RequestUserInput("Locksmith", "Enter a property key ID to duplicate.", "Key ID", UIEventID.OnLocksmithRequest_Submit, UIEventID.OnLocksmithRequest_Cancel);
	}

	private void OnCancel()
	{

	}
}