using System;
using System.Collections.Generic;

public class DancerSystem
{
	private List<WeakReference<CWorldPed>> m_lstWorldPeds = new List<WeakReference<CWorldPed>>();

	public DancerSystem()
	{
		NetworkEvents.CreateDancerPed += CreateDancerPed;
		NetworkEvents.DestroyDancerPed += OnDestroyDancerPed;

		NetworkEvents.CharacterSelectionApproved += UpdateWorldPrompts;
		RageEvents.AddDataHandler(EDataNames.INTERIOR_MANAGER, (RAGE.Elements.Entity entity, object value, object oldValue) => { UpdateWorldPrompts(); });
	}

	private void UpdateWorldPrompts()
	{
		foreach (var pedRef in m_lstWorldPeds)
		{
			CWorldPed ped = pedRef.Instance();
			if (ped != null)
			{
				HandleWorldHint(ped, true);
			}
		}
	}

	private void HandleWorldHint(CWorldPed ped, bool bUpdateOnly)
	{
		if (ped != null)
		{
			if (ped.DancerAllowTip)
			{
				string strText = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.INTERIOR_MANAGER) ? "Collect Tips" : "Tip dancer $10";

				if (bUpdateOnly)
				{
					ped.UpdateWorldHint(EScriptControlID.Interact, strText);
				}
				else
				{
					ped.AddWorldInteraction(EScriptControlID.Interact, Helpers.FormatString("Tip dancer $10"), null, () => { OnInteractWithDancer(ped); }, false, false, 3.0f, null, true);
				}

			}
		}
	}

	private void OnDestroyDancerPed(RAGE.Vector3 vecPos, float fRot, uint dimension)
	{
		foreach (var pedRef in m_lstWorldPeds)
		{
			CWorldPed ped = pedRef.Instance();
			if (ped != null)
			{
				if (ped.Position == vecPos && ped.RotZ == fRot && ped.Dimension == dimension)
				{
					WorldPedManager.DestroyPed(ped);
				}
			}
		}
	}

	private void CreateDancerPed(RAGE.Vector3 vecPos, float fRot, uint dimension, Int64 dancerID, uint DancerSkin, bool bAllowTip, TransmitAnimation transmitAnim)
	{
		WeakReference<CWorldPed> refWorldPed = WorldPedManager.CreatePed(EWorldPedType.Dancer, DancerSkin, vecPos, fRot, dimension, transmitAnim);
		refWorldPed.Instance()?.SetDancerDetails(dancerID, bAllowTip);

		HandleWorldHint(refWorldPed.Instance(), false);

		m_lstWorldPeds.Add(refWorldPed);
	}

	private void OnInteractWithDancer(CWorldPed ped)
	{
		if (ped.DancerAllowTip)
		{
			if (DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.INTERIOR_MANAGER))
			{
				NetworkEventSender.SendNetworkEvent_OnOwnerCollectDancerTips(ped.DancerId);
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_OnInteractWithDancer(ped.DancerId);
			}
		}
	}
}
