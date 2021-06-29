using GTANetworkAPI;
using System;
using System.Collections.Generic;

using Dimension = System.UInt32;

public class SpikeStripSystem
{
	public SpikeStripSystem()
	{
		NetworkEvents.PickupStrips += OnPickupStrips;
	}

	private Vector3 CalculateStripPos(CPlayer a_Player, float fDist)
	{
		Vector3 vecRot = a_Player.Client.Rotation;
		Vector3 vecPos = a_Player.Client.Position;
		float radians = (vecRot.Z + 90.0f) * (3.14f / 180.0f);
		vecPos.X += (float)Math.Cos(radians) * fDist;
		vecPos.Y += (float)Math.Sin(radians) * fDist;
		vecPos.Z -= 0.92f;
		return vecPos;
	}

	public void DeploySpikeStrip(CPlayer a_DeployingPlayer)
	{
		// TODO_SECURITY: Verify player has item
		Vector3 vecRot = a_DeployingPlayer.Client.Rotation;
		vecRot.X = 0.0f;
		vecRot.Y = 0.0f;
		Vector3 vecPos = CalculateStripPos(a_DeployingPlayer, 3.0f);
		Vector3 vecPos2 = CalculateStripPos(a_DeployingPlayer, 5.0f);
		Vector3 vecPos3 = CalculateStripPos(a_DeployingPlayer, 7.0f);
		Vector3 vecPos4 = CalculateStripPos(a_DeployingPlayer, 9.0f);
		Vector3 vecPos5 = CalculateStripPos(a_DeployingPlayer, 11.0f);
		Vector3 vecPos6 = CalculateStripPos(a_DeployingPlayer, 13.0f);

		NAPI.Task.Run(() =>
		{
			// TODO_POST_LAUNCH: We should save these out, we essentially lose an item if the person doesn't pick them back up / server crashes
			CSpikeStrip newSpike = new CSpikeStrip(vecPos, vecPos2, vecPos3, vecPos4, vecPos5, vecPos6, vecRot, a_DeployingPlayer.Client.Dimension);
			m_lstSpikes.Add(newSpike);

			CItemInstanceDef itemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.SPIKESTRIP, 0.0f);
			a_DeployingPlayer.Inventory.RemoveItemFromBasicDefinition(itemInstanceDef, false);
		});
	}

	public void OnPickupStrips(CPlayer player, GTANetworkAPI.Object spikeStripObject)
	{
		// TODO: Probably a better way of doing this rather than comparing positions...
		foreach (CSpikeStrip strip in m_lstSpikes)
		{
			if (strip.Object.Position == spikeStripObject.Position)
			{
				// Are we in LEO faction?
				if (player.IsInFactionOfType(EFactionType.LawEnforcement))
				{
					CItemInstanceDef itemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.SPIKESTRIP, 0.0f);
					if (player.Inventory.CanGiveItem(itemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
					{
						player.Inventory.AddItemToNextFreeSuitableSlot(itemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, null);
						strip.Destroy();
						m_lstSpikes.Remove(strip);
					}
					else
					{
						player.SendNotification("Spike Strips", ENotificationIcon.ExclamationSign, "You can not pick up spike strips:<br>{0}", strUserFriendlyMessage);
					}
				}
				else
				{
					player.SendNotification("Spike Strips", ENotificationIcon.ExclamationSign, "You must be in a law enforce faction to pickup spike strips.", null);
				}

				return;
			}
		}
	}

	private class CSpikeStrip : CBaseEntity
	{
		public CSpikeStrip(Vector3 vecPos, Vector3 vecPos2, Vector3 vecPos3, Vector3 vecPos4, Vector3 vecPos5, Vector3 vecPos6, Vector3 vecRot, Dimension a_Dimension)
		{
			Object = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos, vecRot, 255, a_Dimension);
			Object2 = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos2, vecRot, 255, a_Dimension);
			Object3 = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos3, vecRot, 255, a_Dimension);
			Object4 = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos4, vecRot, 255, a_Dimension);
			Object5 = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos5, vecRot, 255, a_Dimension);
			Object6 = NAPI.Object.CreateObject(NAPI.Util.GetHashKey("p_stinger_02"), vecPos6, vecRot, 255, a_Dimension);

			// TODO_POST_LAUNCH: Move object creation to client side so we can set each stinger part on the floor correctly
			SetData(Object, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP, EDataType.Synced);
			SetData(Object2, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP_NO_PICKUP, EDataType.Synced);
			SetData(Object3, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP_NO_PICKUP, EDataType.Synced);
			SetData(Object4, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP_NO_PICKUP, EDataType.Synced);
			SetData(Object5, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP_NO_PICKUP, EDataType.Synced);
			SetData(Object6, EDataNames.OBJECT_TYPE, EObjectTypes.SPIKE_STRIP_NO_PICKUP, EDataType.Synced);
		}

		~CSpikeStrip()
		{
			Destroy();
		}

		public void Destroy()
		{
			if (Object != null)
			{
				NAPI.Task.Run(() =>
				{
					NAPI.Entity.DeleteEntity(Object.Handle);
					NAPI.Entity.DeleteEntity(Object2.Handle);
					NAPI.Entity.DeleteEntity(Object3.Handle);
					NAPI.Entity.DeleteEntity(Object4.Handle);
					NAPI.Entity.DeleteEntity(Object5.Handle);
					NAPI.Entity.DeleteEntity(Object6.Handle);
				});
			}
		}

		public GTANetworkAPI.Object Object { get; }
		private GTANetworkAPI.Object Object2 { get; }
		private GTANetworkAPI.Object Object3 { get; }
		private GTANetworkAPI.Object Object4 { get; }
		private GTANetworkAPI.Object Object5 { get; }
		private GTANetworkAPI.Object Object6 { get; }
	}

	private List<CSpikeStrip> m_lstSpikes = new List<CSpikeStrip>();
}