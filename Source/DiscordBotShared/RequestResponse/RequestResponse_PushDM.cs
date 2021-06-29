internal class JSONRequest_PushDM : JSONRequest_Base
{
	public DiscordUser User;
	public string Message;

	public JSONRequest_PushDM(DiscordUser a_User, string a_Message) : base("PushDM")
	{
		User = a_User;
		Message = a_Message;
	}
}

internal class JSONResponse_PushDM : JSONResponse
{

}
