public class AdminRecon
{
	private RAGE.Elements.Player m_reconTarget = null;
	private uint m_targetStartDimension;
	private uint m_baseWorldDimension = 0;
	private RAGE.Vector3 m_vecOffsetFromTarget = null;

	public AdminRecon()
	{
		NetworkEvents.StartRecon += OnStartRecon;
		NetworkEvents.StopRecon += OnStopRecon;

		RageEvents.RAGE_OnTick_OncePerSecond += Update;

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.AddDataHandler(EDataNames.RECON, OnReconChanged);
		RageEvents.AddDataHandler(EDataNames.VEH_INVISIBLE, OnVehicleInvisibleChanged);
	}

	private void OnVehicleInvisibleChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle) entity;
		
		if ((bool)newValue)
		{
			vehicle.SetAlpha(0, false);
		}
		else
		{
			vehicle.SetAlpha(255, false);
		}
	}

	private void OnReconChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		UpdateReconState(entity);
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		UpdateReconState(entity);
	}

	// TODO_LAUNCH: What happens if player moves to custom map? does this even work for custom maps at all? probably doesn't load the map :(
	private void UpdateReconState(RAGE.Elements.Entity entity)
	{

		if (entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			bool bReconOn = DataHelper.GetEntityData<bool>(player, EDataNames.RECON);
			bool bDisappeared = DataHelper.GetEntityData<bool>(player, EDataNames.DISAPPEAR);
			if (bReconOn)
			{
				player.SetAlpha(0, false);
				player.SetLights(false);
				player.FreezePosition(true);
				player.SetCollision(false, false);
			}
			else if (bDisappeared)
			{
				player.SetAlpha(0, false);
				player.SetLights(false);
			}
			else
			{
				player.SetAlpha(255, false);
				player.SetLights(true);
				player.FreezePosition(false);
				player.SetCollision(true, true);
			}
		}
	}

	private void Update()
	{
		if (m_reconTarget != null)
		{
			RAGE.Vector3 vecPos = m_reconTarget.Position;
			RAGE.Elements.Player.LocalPlayer.Position = vecPos;

			//are we detached? Re-attach to target
			if (!RAGE.Elements.Player.LocalPlayer.IsAttachedToEntity(m_reconTarget.Handle))
			{
				// bone index for skull 
				int boneIndex = m_reconTarget.GetBoneIndex(12844);
				RAGE.Game.Entity.AttachEntityToEntity(RAGE.Elements.Player.LocalPlayer.Handle, m_reconTarget.Handle, boneIndex, m_vecOffsetFromTarget.X, m_vecOffsetFromTarget.Y, m_vecOffsetFromTarget.Z, 180.0f, 180.0f, 90.0f, true, false, false, true, 0, false);
			}
			//Not in the same dimension/enters or exits interior/ ProcessDimensionChange
			if (RAGE.Elements.Player.LocalPlayer.Dimension != m_reconTarget.Dimension)
			{
				ProcessDimensionChange(m_reconTarget.Dimension);
			}
			//In a vehicle? Smooth the heading of our recon to go with the car.
			if (m_reconTarget.Vehicle != null)
			{
				float targetVehHeading = m_reconTarget.Vehicle.GetHeading();
				RAGE.Elements.Player.LocalPlayer.SetHeading(targetVehHeading);
				vecPos.Z += 1.0f;
			}
		}
	}

	RAGE.Vector3 m_vecCachedPos = null;
	private void OnStartRecon(RAGE.Elements.Player reconTarget)
	{
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			m_vecCachedPos = RAGE.Elements.Player.LocalPlayer.Position;
			m_reconTarget = reconTarget;
			m_targetStartDimension = reconTarget.Dimension;
			//not in world dim? ProcessDimensionChange for interior
			if (m_targetStartDimension != m_baseWorldDimension)
			{
				ProcessDimensionChange(m_targetStartDimension);
			}

			RAGE.Vector3 vecPos = reconTarget.Position;
			if (reconTarget.Vehicle != null)
			{
				var targetHeading = reconTarget.Vehicle.GetHeading();
				RAGE.Elements.Player.LocalPlayer.SetHeading(targetHeading);

				vecPos.X += 0.3f;
				vecPos.Y += 0.3f;
				vecPos.Z += 0.3f;
			}
			m_vecOffsetFromTarget = vecPos - reconTarget.Position;
			m_vecOffsetFromTarget.Z += 0.3f;
			// bone index for skull
			int boneIndex = reconTarget.GetBoneIndex(12844);
			RAGE.Game.Entity.AttachEntityToEntity(RAGE.Elements.Player.LocalPlayer.Handle, reconTarget.Handle, boneIndex, m_vecOffsetFromTarget.X, m_vecOffsetFromTarget.Y, m_vecOffsetFromTarget.Z, 180.0f, 180.0f, 90.0f, true, false, false, true, 0, false);
		}, 500, 1);
	}

	private void OnStopRecon()
	{
		// restore pos clientside too so player doesnt see them for a split second
		RAGE.Elements.Player.LocalPlayer.Position = m_vecCachedPos;
		m_reconTarget = null;
		RAGE.Game.Entity.DetachEntity(RAGE.Elements.Player.LocalPlayer.Handle, false, false);
	}

	public void ProcessDimensionChange(uint TargetDimension)
	{
		int boneIndex = m_reconTarget.GetBoneIndex(12844);

		if ((TargetDimension == m_baseWorldDimension) || (RAGE.Elements.Player.LocalPlayer.Dimension != m_baseWorldDimension))
		{
			NetworkEventSender.SendNetworkEvent_RequestExitInterior();
		}
		else if (m_reconTarget.Dimension != 0)
		{
			NetworkEventSender.SendNetworkEvent_RequestEnterInterior((int)m_reconTarget.Dimension);
		}

		//Detach from player while we load the interior and freeze. Then reattach after timer is done (We leave the Freezeposition true due to jumping up and down (no need to reset))
		RAGE.Game.Entity.DetachEntity(RAGE.Elements.Player.LocalPlayer.Handle, false, false);
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		ClientTimerPool.CreateTimer((object[] parameters) =>
		{
			RAGE.Game.Entity.AttachEntityToEntity(RAGE.Elements.Player.LocalPlayer.Handle, m_reconTarget.Handle, boneIndex, m_vecOffsetFromTarget.X, m_vecOffsetFromTarget.Y, m_vecOffsetFromTarget.Z, 180.0f, 180.0f, 90.0f, true, false, false, true, 0, false);
		}, 1250, 1);
	}
}