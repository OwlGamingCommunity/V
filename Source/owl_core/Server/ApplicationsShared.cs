using System;

public static class ApplicationsSharedLogic
{
	private static void HandleLiveApplicationStateChangeForPlayer(int a_RemotePlayerAccountID, EApplicationState a_NewState)
	{
		WeakReference<CPlayer> playerRef = PlayerPool.GetPlayerFromAccountID_IncludeOutOfGame(a_RemotePlayerAccountID);
		CPlayer remotePlayer = playerRef.Instance();

		if (remotePlayer != null) // Player is online
		{
			remotePlayer.SetApplicationState(a_NewState);

			//remotePlayer.ApplicationState = a_NewState;
			//remotePlayer.HandleApplicationStateAndTransmitCharacters(false)
		}
	}

	public static void SetApplicationState(int AccountID, EApplicationState newState)
	{
		// TODO_APPLICATIONS: Re-test all of applications flow when we re-enable them, made lots of changes
		HandleLiveApplicationStateChangeForPlayer(AccountID, newState);
		Database.Functions.Accounts.SetApplicationState(AccountID, newState);
	}
}