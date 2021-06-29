using System;

internal class CGUITowGetVehicle : CEFCore
{
	public CGUITowGetVehicle(OnGUILoadedDelegate callbackOnLoad) : base("owl_vehicles.client/towgetvehicle.html", EGUIID.TowGetVehicle, callbackOnLoad)
	{
		UIEvents.UnimpoundVehicle += (int id) => { VehicleSystem.GetTowingSystem()?.OnUnimpoundVehicle(id); };
		UIEvents.OnHideImpoundedVehicle += () => { VehicleSystem.GetTowingSystem()?.OnHideImpoundedVehicle(); };
	}

	public override void OnLoad()
	{

	}

	public void Reset()
	{
		Execute("Reset");
	}

	public void InitDone()
	{
		Execute("InitDone");
	}

	public void AddVehicle(Int64 id, string strName, string strPlate)
	{
		Execute("AddVehicle", id, strName, strPlate);
	}
}