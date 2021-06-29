using GTANetworkAPI;
using System;

public class VehicleCrusher
{
	private const float g_fCrushPercent = 0.33f;

	public VehicleCrusher()
	{
		NetworkEvents.VehicleCrusher_RequestCrushInformation += OnRequestCrushInformation;
		NetworkEvents.VehicleCrusher_CrushVehicle += OnRequestCrushVehicle;
	}

	public void OnRequestCrushInformation(CPlayer player)
	{
		CVehicle vehicle = GetPlayerVehicle(player);
		if (vehicle == null || !CanSellVehicle(player, vehicle))
		{
			return;
		}

		NetworkEventSender.SendNetworkEvent_VehicleCrusher_ShowCrushInterface(
			player,
			GetCrushAmount(vehicle),
			vehicle.m_bTokenPurchase,
			vehicle.GetFullDisplayName()
		);
	}

	public async void OnRequestCrushVehicle(CPlayer player)
	{
		CVehicle vehicle = GetPlayerVehicle(player);
		if (vehicle == null || !CanSellVehicle(player, vehicle))
		{
			return;
		}

		if (vehicle.m_bTokenPurchase)
		{
			DonationPurchasable vehicleToken = Donations.GetVehicleToken();
			if (vehicleToken == null)
			{
				player.PushChatMessageWithColor(EChatChannel.Notifications, 255, 0, 0, "Error: Could not return vehicle token.");
				return;
			}

			if (!player.DonationInventory.HasActiveDonationOfEffectType(EDonationEffect.VehicleToken))
			{
				await player.DonationInventory.OnPurchaseScripted(vehicleToken, true).ConfigureAwait(true);
			}

			player.SendNotification("Vehicle Crusher", ENotificationIcon.InfoSign, "Your vehicle token has been returned for crushing a token vehicle.");

			VehiclePool.DestroyVehicle(vehicle);

			return;
		}

		string vehicleName = vehicle.GetFullDisplayName();
		float crushAmount = GetCrushAmount(vehicle);

		VehiclePool.DestroyVehicle(vehicle);

		player.AddBankMoney(crushAmount, PlayerMoneyModificationReason.CrushVehicle);

		player.SendNotification(
			"Vehicle Crusher",
			ENotificationIcon.PiggyBank,
			"You have been given ${0:n} for selling a {1}",
			crushAmount,
			vehicleName
		);
	}

	public static float GetCrushAmount(CVehicle vehicle)
	{
		CVehicleDefinition definition = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.GetModelHash());
		//return (float)Math.Ceiling(definition.Price * g_fCrushPercent);
		return definition.Price * g_fCrushPercent;
	}

	private bool CanSellVehicle(CPlayer player, CVehicle vehicle)
	{
		if (vehicle.OwnerID != player.ActiveCharacterDatabaseID)
		{
			Error(player, "You do not own this vehicle.");
			return false;
		}

		if (vehicle.PaymentsRemaining > 0)
		{
			Error(player, "You cannot sell a vehicle you still owe money on.");
			return false;
		}

		if (vehicle.IsRentalCar())
		{
			Error(player, "You cannot sell a rental vehicle.");
			return false;
		}

		return true;
	}

	private void Error(CPlayer player, string error)
	{
		player.SendNotification("Vehicle Crusher", ENotificationIcon.ExclamationSign, error);
	}

	private CVehicle GetPlayerVehicle(CPlayer player)
	{
		Vehicle vehInst = player.Client.Vehicle;
		return vehInst == null ? null : VehiclePool.GetVehicleFromGTAInstance(vehInst);
	}
}
