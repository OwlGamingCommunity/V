using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class CElevatorInstance : CBaseEntity
{
	public CElevatorInstance(EntityDatabaseID a_ElevatorID, Vector3 entrancePosition, Vector3 exitPosition, uint exitDimension, uint startDimension, bool isCarElevator, float startRotation, float endRotation, string elevatorName)
	{
		m_DatabaseID = a_ElevatorID;
		EntrancePos = entrancePosition;
		ExitPos = exitPosition;
		ExitDim = exitDimension;
		StartDim = startDimension;
		CarElevator = isCarElevator;
		StartRot = startRotation;
		EndRot = endRotation;
		ElevatorName = elevatorName;

		CreateMarkers();
	}

	~CElevatorInstance()
	{
		NAPI.Task.Run(() =>
		{
			Destroy(false);
		});
	}

	public int GetMapID(bool bFromEntrance)
	{
		if (!bFromEntrance)
		{
			int returnValue = MapLoader.GetMapIDByInteriorID((int)ExitDim);

			if (returnValue == -1)
			{
				CInteriorDefinition intDef = InteriorDefinitions.GetInteriorDefinition((int)ExitDim);
				if (intDef != null)
				{
					returnValue = intDef.MapFileName.Length > 0 ? MapLoader.GetMapID(intDef.MapFileName) : -1;
				}
			}

			return returnValue;
		}
		else
		{
			int returnValue = MapLoader.GetMapIDByInteriorID((int)StartDim);

			if (returnValue == -1)
			{
				CInteriorDefinition intDef = InteriorDefinitions.GetInteriorDefinition((int)StartDim);
				if (intDef != null)
				{
					returnValue = intDef.MapFileName.Length > 0 ? MapLoader.GetMapID(intDef.MapFileName) : -1;
				}
			}

			return returnValue;
		}
	}

	private void CreateMarkers()
	{
		NAPI.Task.Run(() =>
		{
			if (m_EntranceMarker != null)
			{
				m_EntranceMarker.Delete();
				m_EntranceMarker = null;
			}

			if (m_ExitMarker != null)
			{
				m_ExitMarker.Delete();
				m_ExitMarker = null;
			}

			if (m_EntranceMarkerInner != null)
			{
				m_EntranceMarkerInner.Delete();
				m_EntranceMarkerInner = null;
			}

			if (m_ExitMarkerInner != null)
			{
				m_ExitMarkerInner.Delete();
				m_ExitMarkerInner = null;
			}

			Color elevatorColor = new Color(38, 107, 235, 200);
			Color elevatorColorInner = new Color(38, 107, 235, 100);

			if (!CarElevator)
			{
				m_EntranceMarker = NAPI.Marker.CreateMarker(21, EntrancePos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, StartRot), 1.25f, elevatorColor, true, StartDim);
				m_ExitMarker = NAPI.Marker.CreateMarker(21, ExitPos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, EndRot), 1.0f, elevatorColor, true, ExitDim);

				//m_EntranceMarkerInner = NAPI.Marker.CreateMarker(21, EntrancePos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, StartRot), 1.25f, elevatorColorInner, true, StartDim);
				//m_ExitMarkerInner = NAPI.Marker.CreateMarker(21, ExitPos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, EndRot), 1.0f, elevatorColorInner, true, ExitDim);
			}
			else
			{
				m_EntranceMarker = NAPI.Marker.CreateMarker(22, EntrancePos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, StartRot), 2.5f, elevatorColor, true, StartDim);
				EntityDataManager.SetData(m_EntranceMarker, EDataNames.ELEVATOR_VEHICLE, true, EDataType.Synced);

				m_ExitMarker = NAPI.Marker.CreateMarker(22, ExitPos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, EndRot), 2.25f, elevatorColor, true, ExitDim);
				EntityDataManager.SetData(m_ExitMarker, EDataNames.ELEVATOR_VEHICLE, true, EDataType.Synced);

				//m_EntranceMarkerInner = NAPI.Marker.CreateMarker(30, EntrancePos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, StartRot), 1.25f, elevatorColorInner, true, StartDim);
				//m_ExitMarkerInner = NAPI.Marker.CreateMarker(30, ExitPos.Add(new Vector3(0.0f, 0.0f, 0.45f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0f, 00.0f, EndRot), 1.0f, elevatorColorInner, true, ExitDim);
			}

			EntityDataManager.SetData(m_EntranceMarker, EDataNames.ELEVATOR_ID, m_DatabaseID, EDataType.Synced);
			EntityDataManager.SetData(m_EntranceMarker, EDataNames.ELEVATOR_ENTRANCE, true, EDataType.Synced);
			EntityDataManager.SetData(m_EntranceMarker, EDataNames.ELEVATOR_NAME, ElevatorName, EDataType.Synced);

			EntityDataManager.SetData(m_ExitMarker, EDataNames.ELEVATOR_EXIT, true, EDataType.Synced);
			EntityDataManager.SetData(m_ExitMarker, EDataNames.ELEVATOR_ID, m_DatabaseID, EDataType.Synced);
		});
	}

	public bool SetElevatorName(string newName)
	{
		if (newName.Length > 0)
		{
			ElevatorName = newName;
			return true;
		}
		return false;
	}

	/// <summary>
	/// You should probably call the PropertyPool DestroyProperty
	/// </summary>
	/// <param name="RemoveFromDatabase"></param>
	public async void Destroy(bool RemoveFromDatabase)
	{
		if (m_EntranceMarker != null)
		{
			NAPI.Entity.DeleteEntity(m_EntranceMarker.Handle);
			m_EntranceMarker = null;
		}

		if (m_ExitMarker != null)
		{
			NAPI.Entity.DeleteEntity(m_ExitMarker.Handle);
			m_ExitMarker = null;
		}

		if (m_EntranceMarkerInner != null)
		{
			NAPI.Entity.DeleteEntity(m_EntranceMarkerInner.Handle);
			m_EntranceMarkerInner = null;
		}

		if (m_ExitMarkerInner != null)
		{
			NAPI.Entity.DeleteEntity(m_ExitMarkerInner.Handle);
			m_ExitMarkerInner = null;
		}

		if (RemoveFromDatabase)
		{
			await Database.LegacyFunctions.DestroyElevator(m_DatabaseID).ConfigureAwait(true);
		}
	}

	public Vector3 EntrancePos { get; private set; }
	public Vector3 ExitPos { get; private set; }
	public uint ExitDim { get; private set; }
	public uint StartDim { get; private set; }
	public bool CarElevator { get; private set; }
	public float StartRot { get; private set; }
	public float EndRot { get; private set; }
	public string ElevatorName { get; private set; }

	private Marker m_EntranceMarker = null;
	private Marker m_EntranceMarkerInner = null;
	private Marker m_ExitMarker = null;
	private Marker m_ExitMarkerInner = null;
}