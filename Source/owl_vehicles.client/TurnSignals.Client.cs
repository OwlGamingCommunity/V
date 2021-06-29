public class TurnSignals
{
	public TurnSignals()
	{
		RageEvents.AddDataHandler(EDataNames.TURNSIGNAL_LEFT, OnTurnSignalDataChanged);
		RageEvents.AddDataHandler(EDataNames.TURNSIGNAL_RIGHT, OnTurnSignalDataChanged);

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;

		ScriptControls.SubscribeToControl(EScriptControlID.LeftTurnSignal, ToggleLeftTurnSignal);
		ScriptControls.SubscribeToControl(EScriptControlID.RightTurnSignal, ToggleRightTurnSignal);
	}

	private void OnTurnSignalDataChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			UpdateTurnSignals(vehicle);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			UpdateTurnSignals(vehicle);
		}
	}

	private void UpdateTurnSignals(RAGE.Elements.Vehicle vehicle)
	{
		bool bTurnSignalStateLeft = DataHelper.GetEntityData<bool>(vehicle, EDataNames.TURNSIGNAL_LEFT);
		vehicle.SetIndicatorLights(1, bTurnSignalStateLeft);

		bool bTurnSignalStateRight = DataHelper.GetEntityData<bool>(vehicle, EDataNames.TURNSIGNAL_RIGHT);
		vehicle.SetIndicatorLights(0, bTurnSignalStateRight);
	}

	// TODO_POST_LAUNCH: Show turn signals, lights, engine etc on hud
	private void ToggleLeftTurnSignal(EControlActionType actionType)
	{
		NetworkEventSender.SendNetworkEvent_ToggleLeftTurnSignal();
	}

	// TODO_RAGE: use entity data changed event (for this, and dirt etc?)
	private void ToggleRightTurnSignal(EControlActionType actionType)
	{
		NetworkEventSender.SendNetworkEvent_ToggleRightTurnSignal();
	}
}