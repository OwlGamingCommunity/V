internal class JSONRequest_BiDirectionalAdminChat : JSONRequest_Base
{
	public DiscordUser User { get; set; }
	public string Message;

	public JSONRequest_BiDirectionalAdminChat(DiscordUser a_User, string a_Message) : base("BiDirectionalAdminChat")
	{
		User = a_User;
		Message = a_Message;
	}
}

internal class JSONResponse_BiDirectionalAdminChat : JSONResponse
{

}
