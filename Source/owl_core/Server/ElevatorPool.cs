using GTANetworkAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EntityDatabaseID = System.Int64;

public static class ElevatorPool
{
	public static async Task<CElevatorInstance> CreateElevator(EntityDatabaseID a_ElevatorID, Vector3 entrancePosition, Vector3 exitPosition, uint exitDimension, uint startDimension,
								bool isCarElevator, float startRotation, float endRotation, string elevatorName, bool a_bMakeDatabaseEntry)
	{
		if (a_bMakeDatabaseEntry)
		{
			a_ElevatorID = await Database.LegacyFunctions.CreateElevator(entrancePosition, startRotation, startDimension, exitPosition, endRotation, exitDimension, isCarElevator, elevatorName).ConfigureAwait(true);

		}

		CElevatorInstance newInst = new CElevatorInstance(a_ElevatorID, entrancePosition, exitPosition, exitDimension, startDimension, isCarElevator, startRotation, endRotation, elevatorName);

		m_dictElevatorInstances.Add(a_ElevatorID, newInst);
		return newInst;
	}

	/// <summary>
	/// Removes the elevator from the database, destroys it and removes it from the pool
	/// </summary>
	/// <param name="a_ElevatorInst">The instance to remove</param>
	public static void DestroyElevator(CElevatorInstance a_ElevatorInst)
	{
		try
		{
			NAPI.Task.Run(() =>
			{
				a_ElevatorInst.Destroy(true);
			});

			a_ElevatorInst.Cleanup();
			m_dictElevatorInstances.Remove(a_ElevatorInst.m_DatabaseID);
		}
		catch
		{

		}
	}

	/// <summary>
	/// Deletes the elevator entities, removes it from the pool and queries the database to remake it
	/// </summary>
	/// <param name="a_ElevatorInst">The instance to reload</param>
	public static async Task ReloadElevator(CElevatorInstance a_ElevatorInst)
	{
		EntityDatabaseID elevatorID = a_ElevatorInst.m_DatabaseID;

		NAPI.Task.Run(() =>
		{
			a_ElevatorInst.Destroy(false);
		});
		m_dictElevatorInstances.Remove(elevatorID);
		CDatabaseStructureElevator elevator = await Database.LegacyFunctions.LoadElevator(elevatorID).ConfigureAwait(true);

		NAPI.Task.Run(async () =>
		{
			await CreateElevator(elevator.elevatorID, elevator.entrancePosition, elevator.exitPosition, elevator.exitDimension, elevator.startDimension, elevator.isCarElevator, elevator.startRotation, elevator.endRotation, elevator.elevatorName, false).ConfigureAwait(true);
		});
	}

	public static CElevatorInstance GetElevatorInstanceFromID(EntityDatabaseID a_ElevatorID)
	{
		if (a_ElevatorID == 0)
		{
			return null;
		}

		CElevatorInstance elevatorInst = null;
		if (m_dictElevatorInstances.Keys.Contains(a_ElevatorID))
		{
			elevatorInst = m_dictElevatorInstances[a_ElevatorID];
		}

		return elevatorInst;
	}

	public static List<CElevatorInstance> GetAllElevatorInstances()
	{
		return m_dictElevatorInstances.Values.ToList();
	}

	private static Dictionary<EntityDatabaseID, CElevatorInstance> m_dictElevatorInstances = new Dictionary<EntityDatabaseID, CElevatorInstance>();
}