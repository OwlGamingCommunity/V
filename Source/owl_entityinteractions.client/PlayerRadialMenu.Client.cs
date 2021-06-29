using System;

public class PlayerRadialMenu
{
	private const float INTERACTION_MAX_DISTANCE = 3.0f;

	private CGUIPlayerRadialMenu m_RadialUI = new CGUIPlayerRadialMenu(() => { });
	private RAGE.Elements.GameEntityBase m_EntityInUse = null;
	private EPlayerRadialInteractionID m_LastSelectedItem = EPlayerRadialInteractionID.None;
	private bool m_bIsShowingRadialMenu = false;

	public PlayerRadialMenu()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.ChangeCharacterApproved += () => { Hide(); };

		UIEvents.PlayerRadial_OnEnterItem += OnEnterItem;
		UIEvents.PlayerRadial_OnExitItem += OnExitItem;

		m_RadialUI.SetVisible(true, false, false);
	}

	private void Reset()
	{
		m_EntityInUse = null;
		m_LastSelectedItem = EPlayerRadialInteractionID.None;
	}

	private void OnEnterItem(EPlayerRadialInteractionID selectedItem)
	{
		m_LastSelectedItem = selectedItem;
	}

	private void OnExitItem(EPlayerRadialInteractionID selectedItem)
	{
		if (m_LastSelectedItem == selectedItem)
		{
			m_LastSelectedItem = EPlayerRadialInteractionID.None;
		}
	}

	private void OnTick()
	{
		bool bIsSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		if (!bIsSpawned || !CursorManager.IsCursorVisible() || ItemSystem.GetPlayerInventory().IsVisible())
		{
			return;
		}

		if (RAGE.Game.Pad.IsDisabledControlJustPressed(0, 25))
		{
			RAGE.Vector3 vecClickedWorldPos = GraphicsHelper.GetWorldPositionFromScreenPosition(CursorManager.GetCursorPosition());
			if (vecClickedWorldPos == null)
			{
				return;
			}

			RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);
			CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecClickedWorldPos, RAGE.Elements.Player.LocalPlayer.Handle, -1);
			if (raycast.Hit && raycast.EntityHit != null)
			{
				RAGE.Elements.GameEntityBase element = (RAGE.Elements.GameEntityBase)raycast.EntityHit;
				m_EntityInUse = element;
				Show(element);
			}
		}
		else if (RAGE.Game.Pad.IsDisabledControlJustReleased(0, 25))
		{
			CompleteRadialAction();
		}
	}

	private bool InRangeOfEntity(RAGE.Elements.Entity entity)
	{
		return WorldHelper.GetDistance(RAGE.Elements.Player.LocalPlayer.Position, entity.Position) <
		       INTERACTION_MAX_DISTANCE;
	}

	private void CompleteRadialAction()
	{
		if (m_EntityInUse == null)
		{
			return;
		}
		
		if (!m_RadialUI.IsVisible() || !InRangeOfEntity(m_EntityInUse) || m_LastSelectedItem == EPlayerRadialInteractionID.None)
		{
			Hide();
			return;
		}
		
		switch (m_LastSelectedItem)
		{
			case EPlayerRadialInteractionID.Handcuffs:
				NetworkEventSender.SendNetworkEvent_CuffPlayer((RAGE.Elements.Player)m_EntityInUse);
				break;
			case EPlayerRadialInteractionID.Frisk:
				NetworkEventSender.SendNetworkEvent_FriskPlayer((RAGE.Elements.Player)m_EntityInUse);
				break;
			case EPlayerRadialInteractionID.VehicleTrunk:
			{
				Int64 vehicleID = DataHelper.GetEntityData<Int64>((RAGE.Elements.Vehicle)m_EntityInUse, EDataNames.SCRIPTED_ID);
				NetworkEventSender.SendNetworkEvent_RadialSetDoorState((int)vehicleID, (int)EVehicleDoor.Trunk);
				break;
			}
			case EPlayerRadialInteractionID.VehicleHood:
			{
				Int64 vehicleID = DataHelper.GetEntityData<Int64>((RAGE.Elements.Vehicle)m_EntityInUse, EDataNames.SCRIPTED_ID);
				NetworkEventSender.SendNetworkEvent_RadialSetDoorState((int)vehicleID, (int)EVehicleDoor.Hood);
				break;
			}
			case EPlayerRadialInteractionID.VehicleLock:
			{
				Int64 vehicleID = DataHelper.GetEntityData<Int64>((RAGE.Elements.Vehicle)m_EntityInUse, EDataNames.SCRIPTED_ID);
				NetworkEventSender.SendNetworkEvent_RadialSetLockState((int)vehicleID);
				break;
			}
		}

		Hide();
	}

	private void Show(RAGE.Elements.GameEntityBase entity)
	{
		if (entity == null || !InRangeOfEntity(entity) || !RAGE.Game.Entity.IsEntityOnScreen(entity.Handle))
		{
			return;
		}

		bool bIsPlayer = entity.Type == RAGE.Elements.Type.Player;
		bool bIsVehicle = entity.Type == RAGE.Elements.Type.Vehicle;
		bool bIsPed = entity.Type == RAGE.Elements.Type.Ped;
		if (!bIsPlayer && !bIsVehicle && !bIsPed)
		{
			return;
		}
		
		m_bIsShowingRadialMenu = true;
		m_RadialUI.Show(bIsPlayer, bIsVehicle, bIsPed);
	}

	private void Hide()
	{
		if (m_bIsShowingRadialMenu)
		{
			m_bIsShowingRadialMenu = false;

			Reset();

			// Hide player cursor, kinda hacky, but that's what invoked this menu, not our local script
			CursorManager.ForceHidePlayerRequestedCursor();

			m_RadialUI.Hide();
		}
	}
}