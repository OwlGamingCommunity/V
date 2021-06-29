using GTANetworkAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public static class MetalDetectorPool
{
	public static async Task<CMetalDetectorInstance> CreateMetalDetector(EntityDatabaseID a_MetalDetectorID, Vector3 detectorPosition, float detectorRotation, uint dimension, bool a_bMakeDatabaseEntry)
	{
		if (a_bMakeDatabaseEntry)
		{
			a_MetalDetectorID = await Database.LegacyFunctions.CreateMetalDetector(detectorPosition, detectorRotation, dimension).ConfigureAwait(true);

		}

		CMetalDetectorInstance newInst = new CMetalDetectorInstance(a_MetalDetectorID, detectorPosition, detectorRotation, dimension);

		m_dictMetalDetectorInstances.Add(a_MetalDetectorID, newInst);
		return newInst;
	}

	/// <summary>
	/// Removes the metal detector from the database, destroys it and removes it from the pool
	/// </summary>
	/// <param name="a_MetalDetectorInst">The instance to remove</param>
	public static void DestroyMetalDetector(CMetalDetectorInstance a_MetalDetectorInst)
	{
		try
		{
			NAPI.Task.Run(() =>
			{
				a_MetalDetectorInst.Destroy(true);
			});

			a_MetalDetectorInst.Cleanup();
			m_dictMetalDetectorInstances.Remove(a_MetalDetectorInst.m_DatabaseID);
		}
		catch
		{

		}
	}

	public static CMetalDetectorInstance GetMetalDetectorInstanceFromID(EntityDatabaseID a_MetalDetectorID)
	{
		if (a_MetalDetectorID == 0)
		{
			return null;
		}

		CMetalDetectorInstance metalDetectorInst = null;
		if (m_dictMetalDetectorInstances.Keys.Contains(a_MetalDetectorID))
		{
			metalDetectorInst = m_dictMetalDetectorInstances[a_MetalDetectorID];
		}

		return metalDetectorInst;
	}

	public static List<CMetalDetectorInstance> GetAllMetalDetectorInstances()
	{
		return m_dictMetalDetectorInstances.Values.ToList();
	}

	private static Dictionary<EntityDatabaseID, CMetalDetectorInstance> m_dictMetalDetectorInstances = new Dictionary<EntityDatabaseID, CMetalDetectorInstance>();
}
