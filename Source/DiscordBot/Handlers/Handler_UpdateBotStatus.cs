using Newtonsoft.Json;
using owl_discord;
using System.Net;



internal class CRequestHandler_UpdateBotStatus : CRequestHandler
{
	public CRequestHandler_UpdateBotStatus() : base("UpdateBotStatus")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_UpdateBotStatus json = JsonConvert.DeserializeObject<JSONRequest_UpdateBotStatus>(StreamedText());
		if (json != null)
		{
			JSONResponse_UpdateBotStatus response = new JSONResponse_UpdateBotStatus()
			{
				success = true
			};

			Program.GetDiscordBot().UpdateBotStatus(json.Status);

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}