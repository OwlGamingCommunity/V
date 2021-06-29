using GTANetworkAPI;
using Newtonsoft.Json;
using System.Net;



internal class CRequestHandler_PushCommand : CRequestHandler
{
	public CRequestHandler_PushCommand() : base("PushCommand")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_PushCommand json = JsonConvert.DeserializeObject<JSONRequest_PushCommand>(StreamedText());
		if (json != null)
		{
			JSONResponse_PushCommand response = new JSONResponse_PushCommand()
			{
				success = true
			};

			// marshal to main thread
			NAPI.Task.Run(() =>
			{
				DiscordCommandManager.OnRawCommand(json.User, json.CommandMessage, json.ChannelID);
			});
			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}