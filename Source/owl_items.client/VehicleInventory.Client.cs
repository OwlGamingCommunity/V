using System;
using System.Collections.Generic;

public class VehicleInventory
{
	RAGE.Elements.Vehicle m_CurrentVehicle = null;
	private bool m_bIsInVehicleInventory = false;
	private bool m_bIsInvertedTrunk = false;
	private EVehicleInventoryType m_CurrentVehicleInventoryType = EVehicleInventoryType.NONE;

	public VehicleInventory()
	{
		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.VehicleInventoryDetails += OnGotVehicleInventoryDetails;
	}

	public RAGE.Elements.Vehicle GetCurrentVehicle()
	{
		return m_CurrentVehicle;
	}

	private void OnGotVehicleInventoryDetails(EVehicleInventoryType inventoryType, List<CItemInstanceDef> inventory)
	{
		ItemSystem.GetPlayerInventory().ShowInventory();

		EItemSocket vehicleSocket = EItemSocket.None;
		if (inventoryType == EVehicleInventoryType.TRUNK)
		{
			vehicleSocket = EItemSocket.Vehicle_Trunk;
		}
		else if (inventoryType == EVehicleInventoryType.SEATS_AND_FLOOR)
		{
			vehicleSocket = EItemSocket.Vehicle_Seats;
		}
		else if (inventoryType == EVehicleInventoryType.CENTER_CONSOLE_AND_GLOVEBOX)
		{
			vehicleSocket = EItemSocket.Vehicle_Console_And_Glovebox;
		}

		if (vehicleSocket != EItemSocket.None)
		{
			ItemSystem.GetPlayerInventory().SetCurrentVehicleInventory(inventory, vehicleSocket);
			ItemSystem.GetPlayerInventory().OnExpandContainer(-1, vehicleSocket, false);
		}

		return;
	}

	public void CloseVehicleInventory()
	{
		if (m_bIsInVehicleInventory)
		{
			m_bIsInVehicleInventory = false;
			NetworkEventSender.SendNetworkEvent_CloseVehicleInventory(m_CurrentVehicle, m_CurrentVehicleInventoryType, m_bIsInvertedTrunk);
			m_CurrentVehicle = null;
			m_CurrentVehicleInventoryType = EVehicleInventoryType.NONE;
		}
	}

	private void GetNearestVehicleInventoryBone(out RAGE.Elements.Vehicle vehicle, out EVehicleInventoryType type, out RAGE.Vector3 pos, out bool bIsInvertedTrunk)
	{
		vehicle = null;
		type = EVehicleInventoryType.NONE;
		pos = null;
		bIsInvertedTrunk = false;

		float fSmallestDistance = 999999.0f;
		RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

		foreach (RAGE.Elements.Vehicle iterVehicle in RAGE.Elements.Entities.Vehicles.Streamed)
		{
			// is the vehicle nearby?
			float fDistance = WorldHelper.GetDistance(vecPlayerPos, iterVehicle.Position);

			if (fDistance <= ItemConstants.g_fDistVehicleTrunkThresholdLarge)
			{
				float distThreshold = ItemConstants.g_fDistVehicleTrunkThresholdSmall;
				float distThresholdTrunk = ItemConstants.g_fDistVehicleTrunkThresholdLarge;

				// get each bone pos
				bool bUsingFallbackTrunkPos = false;
				int boneIDTrunk = iterVehicle.GetBoneIndexByName("boot");
				if (boneIDTrunk == -1)
				{
					boneIDTrunk = iterVehicle.GetBoneIndexByName("platelight");
					bUsingFallbackTrunkPos = true;
				}
				RAGE.Vector3 trunkPos = iterVehicle.GetWorldPositionOfBone(boneIDTrunk);
				RAGE.Vector3 centerConsoleAndGloveboxPos = iterVehicle.GetWorldPositionOfBone(iterVehicle.GetBoneIndexByName("door_pside_f"));
				RAGE.Vector3 rearSeatsPosLeft = iterVehicle.GetWorldPositionOfBone(iterVehicle.GetBoneIndexByName("door_dside_r"));
				RAGE.Vector3 rearSeatsPosRight = iterVehicle.GetWorldPositionOfBone(iterVehicle.GetBoneIndexByName("door_pside_r"));

				trunkPos.Z += bUsingFallbackTrunkPos ? 1.75f : 0.25f;
				centerConsoleAndGloveboxPos.Z += 0.5f;
				rearSeatsPosLeft.Z += 0.5f;
				rearSeatsPosRight.Z += 0.5f;

				// check if the trunk is actually at the front of the vehicle (like on a comet)
				RAGE.Vector3 enginePos = iterVehicle.GetWorldPositionOfBone(iterVehicle.GetBoneIndexByName("engine"));

				fDistance = WorldHelper.GetDistance(enginePos, trunkPos);

				if (fDistance <= 0.9f)
				{
					// engine and trunk are nearby, this means the engine is in the trunk, so lets take the hood as the trunk
					trunkPos = iterVehicle.GetWorldPositionOfBone(iterVehicle.GetBoneIndexByName("bonnet"));

					// shorter distance for front trunk cars because they're typically smaller
					distThresholdTrunk = 2.0f;
					bIsInvertedTrunk = true;
				}

				// trunk
				fDistance = WorldHelper.GetDistance(vecPlayerPos, trunkPos);
				if (fDistance <= fSmallestDistance && fDistance <= distThresholdTrunk)
				{
					vehicle = iterVehicle;
					type = EVehicleInventoryType.TRUNK;
					pos = trunkPos;
					fSmallestDistance = fDistance;
				}

				// center console and glovebox
				fDistance = WorldHelper.GetDistance(vecPlayerPos, centerConsoleAndGloveboxPos);
				if (fDistance <= fSmallestDistance && fDistance <= distThreshold)
				{
					vehicle = iterVehicle;
					type = EVehicleInventoryType.CENTER_CONSOLE_AND_GLOVEBOX;
					pos = centerConsoleAndGloveboxPos;
					fSmallestDistance = fDistance;
				}

				// seats and floor left
				fDistance = WorldHelper.GetDistance(vecPlayerPos, rearSeatsPosLeft);
				if (fDistance <= fSmallestDistance && fDistance <= distThreshold)
				{
					vehicle = iterVehicle;
					type = EVehicleInventoryType.SEATS_AND_FLOOR;
					pos = rearSeatsPosLeft;
					fSmallestDistance = fDistance;
				}

				// seats and floor right
				fDistance = WorldHelper.GetDistance(vecPlayerPos, rearSeatsPosRight);
				if (fDistance <= fSmallestDistance && fDistance <= distThreshold)
				{
					vehicle = iterVehicle;
					type = EVehicleInventoryType.SEATS_AND_FLOOR;
					pos = rearSeatsPosRight;
					fSmallestDistance = fDistance;
				}

			}
		}
	}

	private void OnRender()
	{
		if (!ItemSystem.GetPlayerInventory().IsVisible() && RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			m_CurrentVehicleInventoryType = EVehicleInventoryType.NONE;
			m_CurrentVehicle = null;

			RAGE.Vector3 vecPos;

			GetNearestVehicleInventoryBone(out m_CurrentVehicle, out m_CurrentVehicleInventoryType, out vecPos, out m_bIsInvertedTrunk);
			if (m_CurrentVehicle != null)
			{
				string strName = String.Empty;
				if (m_CurrentVehicleInventoryType == EVehicleInventoryType.TRUNK)
				{
					strName = "Trunk";
				}
				else if (m_CurrentVehicleInventoryType == EVehicleInventoryType.CENTER_CONSOLE_AND_GLOVEBOX)
				{
					strName = "Center Console / Glovebox";
				}
				else if (m_CurrentVehicleInventoryType == EVehicleInventoryType.SEATS_AND_FLOOR)
				{
					strName = "Seats / Flooring Storage";
				}

				RAGE.Vector3 vecRot = m_CurrentVehicle.GetRotation(0);
				vecRot.Z += 270.0f;
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), Helpers.FormatString("Access {0}", strName), null, InteractWithVehicleInventory, vecPos, PlayerHelper.GetLocalPlayerDimension(), false, false, ItemConstants.g_fDistVehicleTrunkThresholdLarge, vecRot, true, true, 1.5f);
			}
		}
	}

	private void InteractWithVehicleInventory()
	{
		m_bIsInVehicleInventory = true;
		NetworkEventSender.SendNetworkEvent_RequestVehicleInventory(m_CurrentVehicle, m_CurrentVehicleInventoryType, m_bIsInvertedTrunk);
	}
}