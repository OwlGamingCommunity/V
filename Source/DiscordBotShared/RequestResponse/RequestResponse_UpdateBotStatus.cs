internal class JSONRequest_UpdateBotStatus : JSONRequest_Base
{
	public string Status;

	public JSONRequest_UpdateBotStatus(string a_Status) : base("UpdateBotStatus")
	{
		Status = a_Status;
	}
}

internal class JSONResponse_UpdateBotStatus : JSONResponse
{

}
