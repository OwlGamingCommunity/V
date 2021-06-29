using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class CWorldBlipInstance : CBaseEntity
{
	public CWorldBlipInstance(EntityDatabaseID ID, string strName, int Sprite, int Color, Vector3 vecPos)
	{
		m_DatabaseID = ID;
		m_strName = strName;
		m_Sprite = Sprite;
		m_Color = Color;
		m_vecPos = vecPos;

		m_Blip = HelperFunctions.Blip.Create(vecPos, true, 50.0f, 0, strName, m_Sprite, m_Color);
	}

	~CWorldBlipInstance()
	{
		Destroy();
	}

	public void Destroy()
	{
		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}
	}

	public string m_strName { get; set; }
	public int m_Sprite { get; set; }
	public int m_Color { get; set; }
	public Vector3 m_vecPos { get; set; }

	private Blip m_Blip;
}

