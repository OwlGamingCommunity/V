using Newtonsoft.Json;
using owl_discord;
using System.Net;



internal class CRequestHandler_PushRawChannelMessage : CRequestHandler
{
	public CRequestHandler_PushRawChannelMessage() : base("PushRawChannelMessage")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_PushRawChannelMessage json = JsonConvert.DeserializeObject<JSONRequest_PushRawChannelMessage>(StreamedText());
		if (json != null)
		{
			JSONResponse_PushRawChannelMessage response = new JSONResponse_PushRawChannelMessage()
			{
				success = true
			};

			Program.GetDiscordBot().PushChannelMessage(json.ChannelID, json.Message);

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}