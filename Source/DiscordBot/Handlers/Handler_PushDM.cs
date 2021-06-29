using Discord.WebSocket;
using Newtonsoft.Json;
using owl_discord;
using System.Net;



internal class CRequestHandler_PushDM : CRequestHandler
{
	public CRequestHandler_PushDM() : base("PushDM")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_PushDM json = JsonConvert.DeserializeObject<JSONRequest_PushDM>(StreamedText());
		if (json != null)
		{
			JSONResponse_PushDM response = new JSONResponse_PushDM()
			{
				success = true
			};

			SocketUser discordUser = Program.GetDiscordBot().GetDiscordUserFromDiscordID(json.User.ID);
			Program.GetDiscordBot().PushDM(discordUser, json.Message);

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}