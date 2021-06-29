using Newtonsoft.Json;
using System.Net;

/// <summary>
/// Gets the JSON array of all the VehicleDefs
/// </summary>
internal class CRequestHandler_VehicleDefinitionsJSON : CRequestHandler
{
	public CRequestHandler_VehicleDefinitionsJSON() : base("VehicleDefinitionsJSON")
	{

	}

	public override string Get(HttpListenerRequest request)
	{
		return JsonConvert.SerializeObject(VehicleDefinitions.g_VehicleDefinitions);
	}
}