using Newtonsoft.Json;
using owl_discord;
using System.Net;



internal class CRequestHandler_GetDiscordUsernameFromID : CRequestHandler
{
	public CRequestHandler_GetDiscordUsernameFromID() : base("GetDiscordUsernameFromID")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_GetDiscordUsernameFromID json = JsonConvert.DeserializeObject<JSONRequest_GetDiscordUsernameFromID>(StreamedText());
		if (json != null)
		{
			string strUsername = Program.GetDiscordBot().GetDiscordUsernameFromID(json.DiscordUserID);

			JSONResponse_GetDiscordUsernameFromID response = new JSONResponse_GetDiscordUsernameFromID()
			{
				success = true,
				DiscordUsername = strUsername
			};

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}