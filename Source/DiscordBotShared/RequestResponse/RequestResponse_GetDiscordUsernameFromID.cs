internal class JSONRequest_GetDiscordUsernameFromID : JSONRequest_Base
{
	public ulong DiscordUserID;

	public JSONRequest_GetDiscordUsernameFromID(ulong a_DiscordUserID) : base("GetDiscordUsernameFromID")
	{
		DiscordUserID = a_DiscordUserID;
	}
}

internal class JSONResponse_GetDiscordUsernameFromID : JSONResponse
{
	public string DiscordUsername { get; set; }
}
