public enum ERemoteEndpointType
{
	None,
	GameServer,
	UCP
}

public enum EDiscordChannelIDs
{
	Any = -1,
	DirectMessage = 0,
	GeneralChat,
	Ads,
	AdminChat,
	AdminCommands,
	PeakAlerts,
	PlayerCounts,
	AnticheatAlerts,
	ServerPerfAlerts
}

public enum EDiscordAuthRequirements
{
	GuestsAllowed = 0,
	RequiresLinkedAccount = 1
}

public enum DiscordCommandParsingFlags
{
	Default = 0,
	GreedyArgs = 1
}

public class DiscordUser
{
	public ulong ID { get; set; }
	public string Username { get; set; }

	public DiscordUser()
	{

	}

	public DiscordUser(ulong a_ID, string a_Username)
	{
		ID = a_ID;
		Username = a_Username;
	}
}