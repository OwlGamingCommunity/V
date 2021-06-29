using GTANetworkAPI;

public enum EDataType
{
	Synced,
	Unsynced
}

public static class EntityDataManager
{
	public static void SetData(Entity entity, EDataNames key, object value, EDataType DataType)
	{
		// trap null
		if (value == null)
		{
			try
			{
				throw new System.Exception(Helpers.FormatString("ERROR: Attempting to set data to null for type {0}, key {1}", entity.Type, key.ToString()));
			}
			catch
			{

			}

		}

		// correct values
		if (value.GetType().IsEnum)
		{
			value = (int)value;
		}

		PrintLogger.LogMessage(ELogSeverity.DEBUG, "[EntityData] Set {0} to {1} on {2}. Type was {3}", key.ToString(), value.ToString(), entity.Value.ToString(), DataType.ToString());

		if (DataType == EDataType.Synced)
		{
			NAPI.Data.SetEntitySharedData(entity, ((int)key).ToString(), value);
		}
		else
		{
			NAPI.Data.SetEntityData(entity, ((int)key).ToString(), value);
		}
	}

	public static void ClearData(Entity entity, EDataNames key)
	{
		if (NAPI.Data.HasEntitySharedData(entity, ((int)key).ToString()))
		{
			NAPI.Data.ResetEntitySharedData(entity, ((int)key).ToString());
		}
		else
		{
			NAPI.Data.ResetEntityData(entity, ((int)key).ToString());
		}
	}

	public static T GetData<T>(Entity entity, EDataNames key)
	{
		// TODO: Optimize this by registering it on the first time rather than two attempts
		if (NAPI.Data.HasEntitySharedData(entity, ((int)key).ToString()))
		{
			var data = NAPI.Data.GetEntitySharedData(entity, ((int)key).ToString());
			return data == null ? default(T) : (T)data;
		}
		else if (NAPI.Data.HasEntityData(entity, ((int)key).ToString()))
		{
			var data = NAPI.Data.GetEntityData(entity, ((int)key).ToString());
			return data == null ? default(T) : (T)data;
		}
		else
		{
			return default(T);
		}
	}
}