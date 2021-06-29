using GTANetworkAPI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;



internal class CRequestHandler_BiDirectionalAdminChat : CRequestHandler
{
	public CRequestHandler_BiDirectionalAdminChat() : base("BiDirectionalAdminChat")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_BiDirectionalAdminChat json = JsonConvert.DeserializeObject<JSONRequest_BiDirectionalAdminChat>(StreamedText());
		if (json != null)
		{
			JSONResponse_BiDirectionalAdminChat response;

			response = new JSONResponse_BiDirectionalAdminChat()
			{
				success = true
			};

			// marshall to main thread
			NAPI.Task.Run(() =>
			{
				ICollection<CPlayer> players = PlayerPool.GetAllPlayers();
				foreach (var player in players)
				{
					if (player.IsAdmin())
					{
						player.PushChatMessageWithRGBAndPlayerNameAndPrefixWithoutLanguage(EChatChannel.AdminChat, 95, 244, 66, "[ADMIN IN DISCORD]", json.User.Username, "{0}", json.Message);
					}
				}
			});
			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}