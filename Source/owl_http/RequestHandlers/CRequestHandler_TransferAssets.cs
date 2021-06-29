using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using EntityDatabaseID = System.Int64;

internal class CRequestHandler_TransferAssets : CRequestHandler
{
	public CRequestHandler_TransferAssets() : base("TransferAssets")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_TransferAssets json = JsonConvert.DeserializeObject<JSONRequest_TransferAssets>(StreamedText());
		if (json != null && json.sourceID != 0)
		{

			PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Processing a stat transfer from character ID#" + json.sourceID);

			// Reload all properties and vehicles owned by the source character
			List<CVehicle> vehs = VehiclePool.GetVehiclesFromPlayerOwner(json.sourceID);
			List<CPropertyInstance> props = PropertyPool.GetPropertyInstancesFromOwner(EPropertyOwnerType.Player, json.sourceID);

			foreach (CVehicle vehicle in vehs)
			{
				PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Reloading Vehicle #" + vehicle.m_DatabaseID);
				VehiclePool.ReloadVehicle(vehicle);
			}

			foreach (CPropertyInstance property in props)
			{
				PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Reloading Property #" + property.Model.Id);
				PropertyPool.ReloadProperty(property);
			}

			JSONResponse_TransferAssets response = new JSONResponse_TransferAssets()
			{
				success = true
			};

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}

internal class JSONRequest_TransferAssets
{
	// Character IDs
	public EntityDatabaseID sourceID = 0;
}

internal class JSONResponse_TransferAssets : JSONResponse
{

}
