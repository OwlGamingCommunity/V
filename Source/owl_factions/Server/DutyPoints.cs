using GTANetworkAPI;
using System.Collections.Generic;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public class CDutyPoint
{
	public CDutyPoint(EntityDatabaseID a_DatabaseID, EDutyType a_DutyType, Vector3 a_vecPos, uint a_Dimension)
	{
		DatabaseID = a_DatabaseID;
		DutyType = a_DutyType;
		Position = a_vecPos;
		Dimension = a_Dimension;

		Create();
	}

	public void Destroy()
	{
		if (m_Marker != null)
		{
			NAPI.Entity.DeleteEntity(m_Marker.Handle);
		}

		Database.LegacyFunctions.DestroyDutyPoint(DatabaseID);
	}

	private void Create()
	{
		if (m_Marker != null)
		{
			m_Marker.Delete();
			m_Marker = null;
		}

		m_Marker = NAPI.Marker.CreateMarker(27, Position, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 0.0f, 0.0f), 1.0f, new Color(202, 52, 51, 200), true, Dimension);
		EntityDataManager.SetData(m_Marker, EDataNames.DUTY_POINT, true, EDataType.Synced);
		EntityDataManager.SetData(m_Marker, EDataNames.DUTY_TYPE, DutyType, EDataType.Synced);
	}

	public EntityDatabaseID DatabaseID { get; set; }
	public EDutyType DutyType { get; set; }
	public Vector3 Position { get; set; }

	private Marker m_Marker = null;
	public uint Dimension { get; set; }
}

public class DutyPoints
{
	public DutyPoints()
	{
		LoadAllDutyPoints();
	}

	public async void LoadAllDutyPoints()
	{
		List<CDatabaseStructureDutyPoint> lstPoints = await Database.LegacyFunctions.LoadAllDutyPoints().ConfigureAwait(true);

		NAPI.Task.Run(async () =>
		{
			foreach (var point in lstPoints)
			{
				await CreateDutyPoint(point.dbID, point.dutyType, point.vecPos, point.dimension, false).ConfigureAwait(true);
			}
		});

		NAPI.Util.ConsoleOutput("[DUTY POINTS] Loaded {0} Duty Points!", lstPoints.Count);
	}

	public static async Task<CDutyPoint> CreateDutyPoint(EntityDatabaseID a_DatabaseID, EDutyType a_DutyType, Vector3 a_vecPos, uint a_Dimension, bool bInsertIntoDB)
	{
		if (bInsertIntoDB)
		{
			a_DatabaseID = await Database.LegacyFunctions.CreateDutyPoint(a_DutyType, a_vecPos, a_Dimension).ConfigureAwait(true);
		}

		CDutyPoint newInst = new CDutyPoint(a_DatabaseID, a_DutyType, a_vecPos, a_Dimension);
		m_dictInstances.Add(a_DatabaseID, newInst);
		return newInst;
	}

	public static async void DestroyDutyPoint(CDutyPoint a_Inst, bool bDeleteFromDB)
	{
		if (bDeleteFromDB)
		{
			await Database.LegacyFunctions.DestroyDutyPoint(a_Inst.DatabaseID).ConfigureAwait(true);
		}

		a_Inst.Destroy();
		m_dictInstances.Remove(a_Inst.DatabaseID);
	}

	public static Dictionary<EntityDatabaseID, CDutyPoint> GetDutyPoints()
	{
		return m_dictInstances;
	}

	public static CDutyPoint GetDutyPointByID(EntityDatabaseID ID)
	{
		return m_dictInstances.ContainsKey(ID) ? m_dictInstances[ID] : null;
	}

	private static Dictionary<EntityDatabaseID, CDutyPoint> m_dictInstances = new Dictionary<EntityDatabaseID, CDutyPoint>();
};