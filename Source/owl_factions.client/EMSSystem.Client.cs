using System;
using System.Collections.Generic;
using System.Linq;

public class EMSSystem
{
	private const int g_ExpirationTimeMS = 300000;

	private class BloodSplatterInstance
	{
		public BloodSplatterInstance(uint dimension, RAGE.Vector3 a_vecPos)
		{
			a_vecPos.Z = WorldHelper.GetGroundPosition(a_vecPos) + 0.05f;
			Object = new RAGE.Elements.MapObject(g_BloodModel, a_vecPos, new RAGE.Vector3(270.0f, 0.0f, 0.0f), 255, dimension);
			Object.SetCollision(false, false);
			vecPos = a_vecPos;
			CreationTime = DateTime.Now;
		}

		public void Destroy()
		{
			if (Object != null)
			{
				Object.Destroy();
				Object = null;
			}
		}

		public bool HasExpired()
		{
			return (int)(DateTime.Now - CreationTime).TotalMilliseconds >= g_ExpirationTimeMS;
		}

		public RAGE.Elements.MapObject Object { get; private set; }
		public RAGE.Vector3 vecPos { get; }
		public DateTime CreationTime { get; }
	}

	private Dictionary<RAGE.Elements.Player, List<BloodSplatterInstance>> g_dictBloodSplatters = new Dictionary<RAGE.Elements.Player, List<BloodSplatterInstance>>();
	private static uint g_BloodModel = HashHelper.GetHashUnsigned("p_bloodsplat_s");

	public EMSSystem()
	{
		// preload blood models
		AsyncModelLoader.RequestSyncInstantLoad(g_BloodModel);

		const int numHospitals = 5;
		for (int i = 0; i < numHospitals; ++i)
		{
			RAGE.Game.Misc.DisableHospitalRestart(i, true);
		}

		NetworkEvents.StartDeathEffect += OnStartDeathEffect;
		RageEvents.RAGE_OnPlayerSpawn += OnPlayerSpawn;

		RageEvents.RAGE_OnTick_MediumFrequency += UpdateBleeding;

		ClientTimerPool.CreateTimer(UpdateLocalBleedOut, 8000);
	}

	private void UpdateLocalBleedOut(object[] parameters)
	{
		bool bIsBleeding = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.BLEEDING);
		if (bIsBleeding)
		{
			RAGE.Elements.Player.LocalPlayer.SetHealth(RAGE.Elements.Player.LocalPlayer.GetHealth() - 1);
		}
	}

	private void UpdateBleeding()
	{
		// check for expiration (so we dont keep them around forever...)
		foreach (var kvPair in g_dictBloodSplatters)
		{
			List<BloodSplatterInstance> lstToRemove = new List<BloodSplatterInstance>();
			foreach (BloodSplatterInstance inst in kvPair.Value)
			{
				if (inst.HasExpired())
				{
					lstToRemove.Add(inst);
					inst.Destroy();
				}
			}

			foreach (BloodSplatterInstance instToRemove in lstToRemove)
			{
				kvPair.Value.Remove(instToRemove);
			}
		}

		foreach (RAGE.Elements.Player player in RAGE.Elements.Entities.Players.Streamed)
		{
			bool bIsBleeding = DataHelper.GetEntityData<bool>(player, EDataNames.BLEEDING);
			if (bIsBleeding)
			{
				// todo remove on stream out
				// todo check player dist
				// todo force limit num objects
				const float fRequiredDeltaDist = 1.0f;
				const int MaxBloodDropsPerPlayer = 30;

				// do we have a last position to compare against?
				float fDistFromLast = 999.0f;
				if (g_dictBloodSplatters.ContainsKey(player) && g_dictBloodSplatters[player].Count >= 1)
				{
					// get the last element added
					BloodSplatterInstance lastSplatter = (BloodSplatterInstance)g_dictBloodSplatters[player].ElementAt(g_dictBloodSplatters[player].Count - 1);
					fDistFromLast = WorldHelper.GetDistance2D(lastSplatter.vecPos, player.Position);
				}

				if (fDistFromLast >= fRequiredDeltaDist)
				{
					// making it here means we will spawn one, so dequeue if needed
					if (g_dictBloodSplatters.ContainsKey(player) && g_dictBloodSplatters[player].Count >= MaxBloodDropsPerPlayer) // pop front
					{
						BloodSplatterInstance inst = g_dictBloodSplatters[player].ElementAt(0);
						inst.Destroy();
						g_dictBloodSplatters[player].Remove(inst);
					}

					if (!g_dictBloodSplatters.ContainsKey(player))
					{
						g_dictBloodSplatters[player] = new List<BloodSplatterInstance>();
					}

					g_dictBloodSplatters[player].Add(new BloodSplatterInstance(player.Dimension, player.Position));
				}
			}
		}
	}

	private void OnStartDeathEffect()
	{
		RAGE.Game.Audio.PlaySoundFrontend(-1, "Bed", "WastedSounds", true);
		RAGE.Game.Graphics.StartScreenEffect("DeathFailMPIn", 0, true);
		RAGE.Game.Cam.SetCamEffect(1);
	}

	public void OnPlayerSpawn(RAGE.Events.CancelEventArgs cancel)
	{
		StopDeathEffect();
	}

	private void StopDeathEffect()
	{
		RAGE.Game.Graphics.StopScreenEffect("DeathFailMPIn");
		RAGE.Game.Cam.SetCamEffect(0);
	}
}