using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class CDancerInstance : CBaseEntity
{
	public CDancerInstance(EntityDatabaseID dancerID, Vector3 vecPos, float fRot, uint dancerSkin, uint dimension, float fTipMoney, string animDict, string animName, bool bAllowTip, EntityDatabaseID parentPropertyID)
	{
		m_DatabaseID = dancerID;
		m_vecPos = vecPos;
		m_fRot = fRot;
		m_dancerSkin = dancerSkin;
		m_dimension = dimension;
		m_parentPropertyID = parentPropertyID;
		m_tipMoney = fTipMoney;
		m_bAllowTip = bAllowTip;

		m_transmitAnim.Dict = animDict;
		m_transmitAnim.Name = animName;
		m_transmitAnim.Flags = (int)(AnimationFlags.Loop);

		SendToAllPlayers();
	}

	public void SendToAllPlayers()
	{
		NetworkEventSender.SendNetworkEvent_CreateDancerPed_ForAll_IncludeEveryone(m_vecPos, m_fRot, m_dimension, m_DatabaseID, m_dancerSkin, m_bAllowTip, m_transmitAnim);
	}

	public void SendToPlayer(CPlayer a_Player)
	{
		NetworkEventSender.SendNetworkEvent_CreateDancerPed(a_Player, m_vecPos, m_fRot, m_dimension, m_DatabaseID, m_dancerSkin, m_bAllowTip, m_transmitAnim);
	}

	~CDancerInstance()
	{
		Destroy();
	}

	public void Destroy()
	{

		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			NetworkEventSender.SendNetworkEvent_DestroyDancerPed(player, m_vecPos, m_fRot, m_dimension);
		}
	}

	public Vector3 m_vecPos { get; set; }
	private readonly float m_fRot;
	public uint m_dancerSkin { get; set; }
	public uint m_dimension { get; set; }
	public EntityDatabaseID m_parentPropertyID { get; set; }
	public float m_tipMoney { get; set; }
	public bool m_bAllowTip { get; set; }
	private TransmitAnimation m_transmitAnim = new TransmitAnimation("", "", 0);
}

