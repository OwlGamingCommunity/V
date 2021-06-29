//#define CREATE_BLIP_FOR_BANKS
using GTANetworkAPI;
using System;

using EntityDatabaseID = System.Int64;

public class CBankInstance : CBaseEntity
{
	public CBankInstance(EntityDatabaseID bankID, Vector3 vecPos, float fRot, EBankSystemType bankType, uint dimension)
	{
		m_DatabaseID = bankID;
		m_vecPos = vecPos;
		m_fRot = fRot;
		m_bankType = bankType;
		m_dimension = dimension;

		if (bankType == EBankSystemType.ObjectATM)
		{
			m_Object = NAPI.Object.CreateObject(-870868698, new Vector3(vecPos.X, vecPos.Y, vecPos.Z - 1.0f), new Vector3(0.0f, 0.0f, 270.0 - fRot), 255, dimension);

			// Calculate pos in front
			// TODO: Helper function for this
			Vector3 vecPosInFront = vecPos;
			float fDist = 1.05f;
			float radians = (fRot + 90.0f) * (3.14f / 180.0f);
			vecPosInFront.X += (float)Math.Cos(radians) * fDist;
			vecPosInFront.Y += (float)Math.Sin(radians) * fDist;

			m_Marker = NAPI.Marker.CreateMarker(29, vecPosInFront, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 270.0 - fRot), 1.0f, new Color(16, 84, 36, 200), true, dimension);
		}
		else if (bankType == EBankSystemType.Teller)
		{
			// TODO: This isn't owned by the resource and wont be destroyed on stop
			SendToAllPlayers();
		}
		else if (bankType == EBankSystemType.WorldATM)
		{
			m_Marker = NAPI.Marker.CreateMarker(29, vecPos, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 270.0 - fRot), 1.0f, new Color(16, 84, 36, 200), true, dimension);
		}

		if (m_Marker != null)
		{
			EntityDataManager.SetData(m_Marker, EDataNames.ATM, true, EDataType.Synced);
		}

		if (m_Ped != null)
		{
			EntityDataManager.SetData(m_Ped, EDataNames.ATM, true, EDataType.Synced);
		}

		// World blip
#if CREATE_BLIP_FOR_BANKS
		m_Blip = HelperFunctions.Blip.Create(vecPos, true, 50.0f, dimension, "Bank", 500);
#endif
	}

	~CBankInstance()
	{
		Destroy();
	}

	public void Destroy()
	{
#if CREATE_BLIP_FOR_BANKS
		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}
#endif

		if (m_Marker != null)
		{
			NAPI.Entity.DeleteEntity(m_Marker);
			m_Marker = null;
		}

		if (m_Ped != null)
		{
			NAPI.Entity.DeleteEntity(m_Ped);
			m_Ped = null;
		}

		if (m_Object != null)
		{
			NAPI.Entity.DeleteEntity(m_Object);
			m_Object = null;
		}

		if (m_bankType == EBankSystemType.Teller)
		{
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				NetworkEventSender.SendNetworkEvent_DestroyBankPed(player, m_vecPos, m_fRot, m_dimension);
			}
		}
	}

	public void SendToAllPlayers()
	{
		if (m_bankType == EBankSystemType.Teller)
		{
			foreach (CPlayer player in PlayerPool.GetAllPlayers())
			{
				SendToPlayer(player);
			}
		}
	}

	public void SendToPlayer(CPlayer a_Player)
	{
		if (m_bankType == EBankSystemType.Teller)
		{
			NetworkEventSender.SendNetworkEvent_CreateBankPed(a_Player, m_vecPos, m_fRot, m_dimension);
		}
	}

	public Vector3 m_vecPos { get; set; }
	private float m_fRot;
	public EBankSystemType m_bankType { get; set; }
	public uint m_dimension { get; set; }
	private Ped m_Ped = null;
	private Marker m_Marker = null;
	private GTANetworkAPI.Object m_Object = null;

#if CREATE_BLIP_FOR_BANKS
	private Blip m_Blip;
#endif
}