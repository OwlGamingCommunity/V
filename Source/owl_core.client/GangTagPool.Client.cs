using RAGE;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;

public class GangTagStorage
{
	public List<GangTagLayer> Layers { get; set; }
	public float Progress { get; set; }
	public EntityDatabaseID ID { get; set; }
	public EntityDatabaseID OwnerCharacterID { get; set; }
	public Vector3 Position { get; set; }
	public float Rotation { get; set; }
	public uint Dimension { get; set; }

	public GangTagStorage(EntityDatabaseID a_ID, EntityDatabaseID a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress)
	{
		ID = a_ID;
		OwnerCharacterID = a_OwnerCharacterID;
		Position = vecPos;
		Rotation = fRotZ;
		Dimension = a_Dimension;
		Layers = lstLayers;
		Progress = fProgress;
	}

	public void UpdateLayers(List<GangTagLayer> lstLayers)
	{
		Layers = lstLayers;
	}

	public void UpdateProgress(float fProgress)
	{
		Progress = fProgress;
	}
}

public static class GangTagPool
{
	private static Dictionary<EntityDatabaseID, GangTagStorage> m_dictTags = new Dictionary<EntityDatabaseID, GangTagStorage>();

	public static void Init()
	{
		NetworkEvents.CreateGangTag += OnCreateGangTag;
		NetworkEvents.DestroyGangTag += OnDestroyGangTag;
	}

	private static void OnCreateGangTag(EntityDatabaseID a_ID, EntityDatabaseID a_OwnerCharacterID, Vector3 vecPos, float fRotZ, uint a_Dimension, List<GangTagLayer> lstLayers, float fProgress)
	{
		m_dictTags[a_ID] = new GangTagStorage(a_ID, a_OwnerCharacterID, vecPos, fRotZ, a_Dimension, lstLayers, fProgress);
	}

	private static void OnDestroyGangTag(EntityDatabaseID a_ID)
	{
		m_dictTags.Remove(a_ID);
	}

	public static GangTagStorage GetGangTag(EntityDatabaseID a_ID)
	{
		if (m_dictTags.ContainsKey(a_ID))
		{
			return m_dictTags[a_ID];
		}

		return null;
	}

	public static Dictionary<EntityDatabaseID, GangTagStorage> GetAll()
	{
		return m_dictTags;
	}
}