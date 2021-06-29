using GTANetworkAPI;

public class TurnSignals
{
	public TurnSignals()
	{
		NetworkEvents.ToggleLeftTurnSignal += ToggleLeftTurnSignal;
		NetworkEvents.ToggleRightTurnSignal += ToggleRightTurnSignal;
	}

	public void ToggleLeftTurnSignal(CPlayer player)
	{
		if (player != null && player.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle != null)
			{
				pVehicle.ToggleLeftTurnSignal();
			}
		}
	}

	public void ToggleRightTurnSignal(CPlayer player)
	{
		if (player != null && player.IsInVehicleReal)
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(player.Client.Vehicle);
			if (pVehicle != null)
			{
				pVehicle.ToggleRightTurnSignal();
			}
		}
	}
}