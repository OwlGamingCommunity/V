using GTANetworkAPI;
using EntityDatabaseID = System.Int64;

public class CMetalDetectorInstance : CBaseEntity
{
	public CMetalDetectorInstance(EntityDatabaseID a_MetalDetectorID, Vector3 detectorPosition, float detectorRotation, uint detectorDimension)
	{
		m_DatabaseID = a_MetalDetectorID;
		DetectorPos = detectorPosition;
		DetectorRot = detectorRotation;
		DetectorDim = detectorDimension;

		CreateDetectors();
	}

	~CMetalDetectorInstance()
	{
		NAPI.Task.Run(() =>
		{
			Destroy(false);
		});
	}

	private void CreateDetectors()
	{
		NAPI.Task.Run(() =>
		{
			if (m_DetectorColshape != null)
			{
				m_DetectorColshape.Delete();
				m_DetectorColshape = null;
			}

			if (m_DetectorObject != null)
			{
				m_DetectorObject.Delete();
				m_DetectorObject = null;
			}

			if (m_DetectorTextLabel != null)
			{
				m_DetectorTextLabel.Delete();
				m_DetectorTextLabel = null;
			}

			m_DetectorObject = NAPI.Object.CreateObject(0x5C514DD3, DetectorPos, new Vector3(0.0, 0.0, DetectorRot), 255, DetectorDim);

			m_DetectorColshape = NAPI.ColShape.CreateSphereColShape(DetectorPos, 1.05f, DetectorDim);

			Color textColor = new Color(255, 255, 255);
			Vector3 textPosition = new Vector3(DetectorPos.X, DetectorPos.Y, DetectorPos.Z + 3.0f);
			m_DetectorTextLabel = NAPI.TextLabel.CreateTextLabel("Detector ID: " + m_DatabaseID, textPosition, 5.0f, 1f, 0, textColor, true, DetectorDim);

			EntityDataManager.SetData(m_DetectorColshape, EDataNames.DETECTOR_ID, m_DatabaseID, EDataType.Synced);
			EntityDataManager.SetData(m_DetectorColshape, EDataNames.IS_DETECTOR, true, EDataType.Synced);
			EntityDataManager.SetData(m_DetectorColshape, EDataNames.DETECTOR_LASTUSED, "0", EDataType.Synced);

			EntityDataManager.SetData(m_DetectorObject, EDataNames.DETECTOR_ID, m_DatabaseID, EDataType.Synced);
		});
	}

	public async void Destroy(bool RemoveFromDatabase)
	{
		if (m_DetectorColshape != null)
		{
			NAPI.Entity.DeleteEntity(m_DetectorColshape.Handle);
			m_DetectorColshape = null;
		}

		if (m_DetectorObject != null)
		{
			NAPI.Entity.DeleteEntity(m_DetectorObject.Handle);
			m_DetectorObject = null;
		}

		if (m_DetectorTextLabel != null)
		{
			NAPI.Entity.DeleteEntity(m_DetectorTextLabel.Handle);
			m_DetectorTextLabel = null;
		}

		if (RemoveFromDatabase)
		{
			await Database.LegacyFunctions.DestroyMetalDetector(m_DatabaseID).ConfigureAwait(true);
		}
	}

	public Vector3 DetectorPos { get; private set; }
	public float DetectorRot { get; private set; }
	public uint DetectorDim { get; private set; }

	private ColShape m_DetectorColshape = null;
	private GTANetworkAPI.Object m_DetectorObject = null;
	private TextLabel m_DetectorTextLabel = null;
}