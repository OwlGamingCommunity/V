internal class JSONRequest_PushCommand : JSONRequest_Base
{
	public DiscordUser User;
	public string CommandMessage;
	public EDiscordChannelIDs ChannelID;


	public JSONRequest_PushCommand(DiscordUser a_User, string a_CommandMessage, EDiscordChannelIDs a_ChannelID) : base("PushCommand")
	{
		User = a_User;
		CommandMessage = a_CommandMessage;
		ChannelID = a_ChannelID;
	}
}

internal class JSONResponse_PushCommand : JSONResponse
{

}
