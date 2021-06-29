internal class JSONRequest_PushRawChannelMessage : JSONRequest_Base
{
	public EDiscordChannelIDs ChannelID;
	public string Message;

	public JSONRequest_PushRawChannelMessage(EDiscordChannelIDs a_ChannelID, string a_Message) : base("PushRawChannelMessage")
	{
		ChannelID = a_ChannelID;
		Message = a_Message;
	}
}

internal class JSONResponse_PushRawChannelMessage : JSONResponse
{

}
