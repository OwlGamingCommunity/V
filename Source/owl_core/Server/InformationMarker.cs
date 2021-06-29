using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class CInformationMarkerInstance : CBaseEntity
{
	public CInformationMarkerInstance(EntityDatabaseID a_ID, EntityDatabaseID a_OwnerCharacterID, Vector3 a_vecPosition, uint a_Dimension, string a_strText)
	{
		m_DatabaseID = a_ID;
		OwnerCharacterID = a_OwnerCharacterID;
		Position = a_vecPosition;
		MarkerDimension = a_Dimension;
		strText = a_strText;

		Create();
	}

	~CInformationMarkerInstance()
	{
		NAPI.Task.Run(() =>
		{
			Destroy(false);
		});
	}

	public void Create()
	{
		NAPI.Task.Run(() =>
		{
			if (m_Marker != null)
			{
				m_Marker.Delete();
				m_Marker = null;
			}

			m_Marker = NAPI.Marker.CreateMarker(32, Position.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 00.0f, 0.0f), 0.5f, new Color(255, 195, 15), true, MarkerDimension);
			EntityDataManager.SetData(m_Marker, EDataNames.IS_INFO_MARKER, true, EDataType.Synced);
			EntityDataManager.SetData(m_Marker, EDataNames.INFO_MARKER_ID, m_DatabaseID, EDataType.Synced);
		});
	}

	public void Destroy(bool RemoveFromDatabase)
	{
		if (m_Marker != null)
		{
			NAPI.Entity.DeleteEntity(m_Marker.Handle);
			m_Marker = null;
		}

		if (RemoveFromDatabase)
		{
			Database.Functions.Items.DestroyInfoMarker(m_DatabaseID);
		}
	}

	public EntityDatabaseID OwnerCharacterID { get; private set; }
	public Vector3 Position { get; private set; }
	public uint MarkerDimension { get; private set; }
	public string strText { get; private set; }

	private Marker m_Marker = null;
}