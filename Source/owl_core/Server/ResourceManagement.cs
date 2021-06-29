using GTANetworkAPI;

public class ResourceManagement 
{
	public ResourceManagement()
	{
	}

	public enum EResourceStartResult
	{
		Started,
		AlreadyRunning,
		NotFound
	}

	private EResourceStartResult StartResource(string strName)
	{
		if (NAPI.Resource.DoesResourceExist(strName))
		{
			if (!NAPI.Resource.IsResourceRunning(strName))
			{
				NAPI.Resource.StartResource(strName);
				return EResourceStartResult.Started;
			}
			else
			{
				return EResourceStartResult.AlreadyRunning;
			}
		}

		return EResourceStartResult.NotFound;
	}

	public enum EResourceStopResult
	{
		Stopped,
		NotRunning,
		NotFound
	}

	private EResourceStopResult StopResource(string strName)
	{
		if (NAPI.Resource.DoesResourceExist(strName))
		{
			if (NAPI.Resource.IsResourceRunning(strName))
			{
				NAPI.Resource.StopResource(strName);
				return EResourceStopResult.Stopped;
			}
			else
			{
				return EResourceStopResult.NotRunning;
			}
		}

		return EResourceStopResult.NotFound;
	}

	private bool RestartResource(string strName)
	{
		EResourceStopResult StopResult = StopResource(strName);

		if (StopResult == EResourceStopResult.NotFound)
		{
			return false;
		}

		EResourceStartResult StartResult = StartResource(strName);
		return StartResult == EResourceStartResult.Started;
	}
}