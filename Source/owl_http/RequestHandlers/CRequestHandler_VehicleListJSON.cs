using GTANetworkAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

internal class CRequestHandler_VehicleListJSON : CRequestHandler
{
	public CRequestHandler_VehicleListJSON() : base("VehicleListJSON")
	{

	}

	private struct SVehicleInfo
	{
		public SVehicleInfo(long dbid, string strName, string strColor)
		{
			DatabaseID = dbid;
			Name = strName;
			Color = strColor;
		}

		public long DatabaseID;
		public string Name;
		public string Color;
	}

	public override string Get(HttpListenerRequest request)
	{
		List<SVehicleInfo> lstVehicles = new List<SVehicleInfo>();

		foreach (Vehicle vehicle in NAPI.Pools.GetAllVehicles())
		{
			CVehicle pVehicle = VehiclePool.GetVehicleFromGTAInstance(vehicle);

			if (pVehicle != null)
			{
				lstVehicles.Add(new SVehicleInfo(pVehicle.m_DatabaseID, pVehicle.GetFullDisplayName(), pVehicle.GetColorsDisplayString()));
			}
		}

		return JsonConvert.SerializeObject(lstVehicles);
	}
}