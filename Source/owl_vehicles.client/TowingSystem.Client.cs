using System;
using System.Collections.Generic;

public class TowingSystem
{
	private WeakReference<CWorldPed> m_refWorldPed_Paleto = new WeakReference<CWorldPed>(null);
	private WeakReference<CWorldPed> m_refWorldPed_LS = new WeakReference<CWorldPed>(null);
	private CGUITowGetVehicle m_TowGetVehicleUI = new CGUITowGetVehicle(() => { });
	private bool m_bPendingRequest = false;
	private EScriptLocation m_LocationInUse = EScriptLocation.Paleto;

	public TowingSystem()
	{
		NetworkEvents.AdminTowGetVehicles += OnAdminRequestTowVehicleList;

		m_refWorldPed_Paleto = WorldPedManager.CreatePed(EWorldPedType.TowClerk, 0x8CCE790F, new RAGE.Vector3(0.4495831f, 6325.446f, 31.41048f), 174.672f, 0);
		m_refWorldPed_Paleto.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Retrieve Towed Vehicles", null, () => { OnInteractWithWorldPed(EScriptLocation.Paleto); }, false, false, 3.0f);
		m_refWorldPed_Paleto.Instance()?.AddBlip(68, true, "Paleto Towing");

		m_refWorldPed_LS = WorldPedManager.CreatePed(EWorldPedType.TowClerk, 0x8CCE790F, new RAGE.Vector3(231.4626f, -1752.708f, 28.98719f), 231.4854f, 0);
		m_refWorldPed_LS.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Retrieve Towed Vehicles", null, () => { OnInteractWithWorldPed(EScriptLocation.LS); }, false, false, 3.0f);
		m_refWorldPed_LS.Instance()?.AddBlip(68, true, "Los Santos Towing");

		NetworkEvents.TowedVehicleList_Response += OnTowedVehicleListResponse;
	}

	private void OnInteractWithWorldPed(EScriptLocation location)
	{
		if (!m_bPendingRequest)
		{
			m_LocationInUse = location;
			m_bPendingRequest = true;
			NetworkEventSender.SendNetworkEvent_TowedVehicleList_Request();
		}
	}

	private void OnAdminRequestTowVehicleList()
	{
		List<Int64> lstEligibleVehicles = new List<Int64>();

		foreach (RAGE.Elements.Vehicle vehicle in RAGE.Elements.Entities.Vehicles.All)
		{
			if (WorldHelper.IsPositionConsideredAbandoned(vehicle.Position))
			{
				Int64 vehicleID = DataHelper.GetEntityData<Int64>(vehicle, EDataNames.SCRIPTED_ID);
				lstEligibleVehicles.Add(vehicleID);
			}
		}

		NetworkEventSender.SendNetworkEvent_AdminTowGotVehicles(lstEligibleVehicles);
	}

	private void OnTowedVehicleListResponse(List<Int64> lstTowedVehicles)
	{
		m_bPendingRequest = false;
		m_TowGetVehicleUI.Reset();

		foreach (var vehicle in RAGE.Elements.Entities.Vehicles.All)
		{
			Int64 vehicleID = DataHelper.GetEntityData<Int64>(vehicle, EDataNames.SCRIPTED_ID);
			if (lstTowedVehicles.Contains(vehicleID))
			{
				CVehicleDefinition vehicleDef = VehicleDefinitions.GetVehicleDefinitionFromHash(vehicle.Model);
				if (vehicleDef != null)
				{
					m_TowGetVehicleUI.AddVehicle(vehicleID, Helpers.FormatString("{0} {1}", vehicleDef.Manufacturer, vehicleDef.Name), vehicle.GetNumberPlateText());
				}
			}
		}

		m_TowGetVehicleUI.InitDone();
		m_TowGetVehicleUI.SetVisible(true, true, false);
	}

	public void OnUnimpoundVehicle(Int64 id)
	{
		foreach (var vehicle in RAGE.Elements.Entities.Vehicles.All)
		{
			Int64 vehicleID = DataHelper.GetEntityData<Int64>(vehicle, EDataNames.SCRIPTED_ID);
			if (vehicleID == id)
			{
				NetworkEventSender.SendNetworkEvent_RequestUnimpoundVehicle(vehicle, m_LocationInUse);
				break;
			}
		}

		OnHideImpoundedVehicle();
	}

	public void OnHideImpoundedVehicle()
	{
		m_TowGetVehicleUI.SetVisible(false, false, false);
	}
}