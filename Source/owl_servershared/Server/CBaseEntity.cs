using GTANetworkAPI;
using System;
using System.Collections.Generic;

public abstract class CBaseEntity
{
	public CBaseEntity()
	{

	}

	public void SetData(Entity entity, EDataNames key, object value, EDataType DataType, bool bForceSet = false)
	{
		string strNewHash = String.Empty;
		if (value.GetType().IsEnum)
		{
			strNewHash = HelperFunctions.Hashing.sha256(((int)value).ToString());
		}
		else
		{
			strNewHash = HelperFunctions.Hashing.sha256(value.ToString());
		}

		// only set if different (RAGE will always sync it :())
		if (bForceSet || !m_dictSafeEntityDataHashes.ContainsKey(key) || m_dictSafeEntityDataHashes[key] != strNewHash)
		{
			EntityDataManager.SetData(entity, key, value, DataType);
		}

		m_dictSafeEntityData[key] = value;
		m_dictSafeEntityDataHashes[key] = strNewHash;
	}

	public void ClearData(Entity entity, EDataNames key)
	{
		m_dictSafeEntityData.Remove(key);
		m_dictSafeEntityDataHashes.Remove(key);
		EntityDataManager.ClearData(entity, key);
	}

	public T GetData<T>(Entity entity, EDataNames key)
	{
		try
		{
			if (m_dictSafeEntityData.ContainsKey(key))
			{
				return (T)m_dictSafeEntityData[key];
			}

			return default(T);
		}
		catch
		{
			return EntityDataManager.GetData<T>(entity, key);
		}
	}

	public bool ValidateEntityDataIntact(Entity entity, out string strFirstDataModified)
	{
		foreach (var kvPair in m_dictSafeEntityData)
		{
			var rageData = NAPI.Data.GetEntitySharedData(entity, kvPair.Key.ToString());
			if (HelperFunctions.Hashing.sha256(rageData.ToString()) != m_dictSafeEntityDataHashes[kvPair.Key])
			{
				strFirstDataModified = kvPair.Key.ToString();
				return false;
			}
		}

		strFirstDataModified = String.Empty;
		return true;
	}


	public void Cleanup()
	{

	}

	public Int64 m_DatabaseID { get; set; }
	public Int64 CreatorID { get; set; }

	private Dictionary<EDataNames, string> m_dictSafeEntityDataHashes = new Dictionary<EDataNames, string>();
	private Dictionary<EDataNames, dynamic> m_dictSafeEntityData = new Dictionary<EDataNames, dynamic>();
}