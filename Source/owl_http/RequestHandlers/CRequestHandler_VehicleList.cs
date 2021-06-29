using GTANetworkAPI;
using System.Net;

internal class CRequestHandler_VehicleList : CRequestHandler
{
	public CRequestHandler_VehicleList() : base("VehicleList", true)
	{

	}

	public override string Get(HttpListenerRequest request)
	{
		string strBody = "";

		foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null)
			{
				strBody += Helpers.FormatString("[{0}] {1} {2}<br>", pVehicle.m_DatabaseID, pVehicle.GetColorsDisplayString(), pVehicle.GetFullDisplayName());
			}
		}
		return strBody;
	}
}