using Newtonsoft.Json;
using System;
using System.Net;

/// <summary>
/// Kicks a player by their account ID if they are logged in
/// </summary>
internal class CRequestHandler_KickPlayerByID : CRequestHandler
{
	public CRequestHandler_KickPlayerByID() : base("KickPlayerByID")
	{

	}

	public override string Post(HttpListenerRequest request)
	{
		JSONRequest_KickPlayerByID json = JsonConvert.DeserializeObject<JSONRequest_KickPlayerByID>(StreamedText());
		if (json != null && json.id != 0)
		{

			JSONResponse_KickPlayerByID response;

			WeakReference<CPlayer> playerRef = PlayerPool.GetPlayerFromAccountID(json.id);
			CPlayer TargetPlayer = playerRef.Instance();

			if (TargetPlayer != null)
			{
				TargetPlayer.KickFromServer(CPlayer.EKickReason.ASSET_TRANSFER);
				response = new JSONResponse_KickPlayerByID()
				{
					success = true
				};
			}
			else
			{
				response = new JSONResponse_KickPlayerByID()
				{
					success = false,
					reason = "Player not found in game."
				};
			}

			return JsonConvert.SerializeObject(response);
		}

		return null;
	}
}

internal class JSONRequest_KickPlayerByID
{
	public int id = 0;
}

internal class JSONResponse_KickPlayerByID : JSONResponse
{

}
