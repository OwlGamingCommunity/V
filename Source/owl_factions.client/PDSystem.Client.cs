using RAGE;
using System;

public class PDSystem
{
	private CGUIPDTicket m_PDTicketUI = null;
	private CGUIPDLicensingDevice m_PDLicensingDeviceUI = null;
	private bool m_bLicensingDeviceIsRemoval = false;

	public PDSystem()
	{
		RageEvents.RAGE_OnTick_PerFrame += OnTick;

		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;

		NetworkEvents.RequestTicket += OnRequestTicket;
		NetworkEvents.ChangeCharacterApproved += CleanupTicketUI;
		NetworkEvents.PlayCustomSpeech += OnCustomSpeech;

		NetworkEvents.BlipSiren_Response += OnRemoteBlipSiren;

		NetworkEvents.UseFirearmsLicensingDevice += OnUseFirearmsLicensingDevice;

		ScriptControls.SubscribeToControl(EScriptControlID.ToggleSirensMode, ToggleSirenMode);
		ScriptControls.SubscribeToControl(EScriptControlID.BlipSiren, BlipSiren);
	}

	private static void BlipSiren(EControlActionType actionType)
	{
		NetworkEventSender.SendNetworkEvent_BlipSiren_Request();
	}

	private static void OnRemoteBlipSiren(RAGE.Elements.Vehicle vehicle)
	{
		vehicle.BlipSiren();
	}

	private void OnUseFirearmsLicensingDevice(bool a_bIsRemoval)
	{
		m_bLicensingDeviceIsRemoval = a_bIsRemoval;
		NetworkEvents.SendLocalEvent_HideInventory();

		m_PDLicensingDeviceUI = new CGUIPDLicensingDevice(() => { });
		m_PDLicensingDeviceUI.SetVisible(true, true, false);
	}

	public void OnHideLicenseDevice()
	{
		if (m_PDLicensingDeviceUI != null)
		{
			m_PDLicensingDeviceUI.SetVisible(false, false, false);
		}

		m_PDLicensingDeviceUI = null;
	}

	public void OnFinalizeLicenseDevice(string strTargetName, EWeaponLicenseType weaponLicenseType)
	{
		NetworkEventSender.SendNetworkEvent_FinalizeLicenseDevice(strTargetName, weaponLicenseType, m_bLicensingDeviceIsRemoval);
	}

	private void ToggleSirenMode(EControlActionType actionType)
	{
		NetworkEventSender.SendNetworkEvent_ToggleSirenMode();
	}

	private void OnCustomSpeech(RAGE.Elements.Player player, ESpeechID speechID, ESpeechType speechType)
	{
		RAGE.Game.Audio.PlayAmbientSpeech1(player.Handle, speechID.ToString(), speechType.ToString(), 0);
	}


	private void OnCuffedTick()
	{
		// must be called every frame
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SpecialAbility);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Sprint);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Jump);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelUpDown);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelLeftRight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelNext);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.WeaponWheelPrev);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectNextWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectPrevWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.PrevWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.NextWeapon);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Attack);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Aim);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.Reload);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.ThrowGrenade);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleAccelerate);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveLeftRight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveLeft);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveLeftOnly);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveRight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveRightOnly);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehicleMoveLeftRight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehiclePassengerAim);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.VehiclePassengerAttack);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackLight);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackHeavy);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttackAlternate);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeBlock);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack1);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.MeleeAttack2);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponUnarmed);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponMelee);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponHandgun);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponShotgun);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponSmg);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponAutoRifle);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponSniper);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectWeaponHeavy);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SpecialAbility);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterMichael);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterFranklin);
		ControlHelper.SetControlDisabledThisFrame(RAGE.Game.Control.SelectCharacterTrevor);
	}


	private int m_CurrentSeatID = -1;
	private void OnRappel()
	{
		// determine seat
		if (m_CurrentSeatID == 2 || m_CurrentSeatID == 3 || m_CurrentSeatID == 4 || m_CurrentSeatID == 5)
		{
			RAGE.Elements.Player.LocalPlayer.ClearTasks();
			RAGE.Elements.Player.LocalPlayer.TaskRappelFromHeli(10);
		}
	}

	private void OnMoveToRappel(int seat)
	{
		NetworkEventSender.SendNetworkEvent_MoveToRappelPosition(seat);
		m_CurrentSeatID = seat;
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		m_CurrentSeatID = seatId;
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicleBeingExited)
	{
		m_CurrentSeatID = -1;
	}

	private void OnTick()
	{
		// are in a vehicle that can rappel?
		RAGE.Elements.Vehicle currentVehicle = RAGE.Elements.Player.LocalPlayer.Vehicle;
		if (currentVehicle != null)
		{
			// can only rappel from a few helicopters:
			bool bVehicleSupportsRappel = currentVehicle.Model == 2634305738 || currentVehicle.Model == 353883353 || currentVehicle.Model == 837858166;
			if (bVehicleSupportsRappel)
			{
				RAGE.Vector3 vecWorldHintPos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector().Add(new Vector3(0.0f, 0.0f, 1.5f));

				// change seat
				/*
				if (m_CurrentSeatID == 4)
				{
					// is the seat in front clear?
					if (currentVehicle.IsSeatFree(2, 0))
					{
						WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Move To Rappel Position", null, () => { OnMoveToRappel(2); }, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
					}
					else
					{
						WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.NoName, "Rappel Position is in use", null, null, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
					}
				}
				else if (m_CurrentSeatID == 5)
				{
					// is the seat in front clear?
					if (currentVehicle.IsSeatFree(3, 0))
					{
						WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Move To Rappel Position", null, () => { OnMoveToRappel(3); }, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
					}
					else
					{
						WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.NoName, "Rappel Position is in use", null, null, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
					}
				}
				*/

				// we show this for the pilot too so they can see the status
				// determine seat
				if (m_CurrentSeatID == 0 || m_CurrentSeatID == 2 || m_CurrentSeatID == 3 || m_CurrentSeatID == 4 || m_CurrentSeatID == 5)
				{
					// can someone actually rappel?
					if (!currentVehicle.IsSeatFree(2, 0) || !currentVehicle.IsSeatFree(3, 0) || !currentVehicle.IsSeatFree(4, 0) || !currentVehicle.IsSeatFree(5, 0))
					{
						// already rapelling?
						bool bIsPilot = m_CurrentSeatID == 0;
						int taskStatus = RAGE.Game.Ai.GetScriptTaskStatus(RAGE.Elements.Player.LocalPlayer.Handle, 4019022656);
						if (taskStatus != 0 && taskStatus != 1)
						{
							const float fMaxSpeed = 10.0f;
							const float fMinHeight = 5.0f;
							const float fMaxHeight = 45.0f;
							const float fMaxAngle = 15.0f;
							string strErrorMessage = String.Empty;

							float fVehicleHeight = currentVehicle.GetHeightAboveGround();
							if (currentVehicle.GetSpeed() > fMaxSpeed)
							{
								strErrorMessage = "Cannot Rappel: Helicopter too fast";
							}
							else if (fVehicleHeight < fMinHeight)
							{
								strErrorMessage = "Cannot Rappel: Helicopter too low";
							}
							else if (fVehicleHeight > fMaxHeight)
							{
								strErrorMessage = "Cannot Rappel: Helicopter too high";
							}
							else if (!currentVehicle.IsUpright(fMaxAngle) || currentVehicle.IsUpsidedown())
							{
								strErrorMessage = "Cannot Rappel: Helicopter too unstable";
							}

							if (string.IsNullOrEmpty(strErrorMessage))
							{
								if (bIsPilot)
								{
									WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.NoName, "Passengers Can Rappel!", null, null, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
								}
								else
								{
									WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.E, "Rappel", null, OnRappel, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
								}
							}
							else
							{
								if (bIsPilot)
								{
									strErrorMessage = Helpers.FormatString("Passengers {0}", strErrorMessage);
								}

								WorldHintManager.DrawExclusiveWorldHint(ConsoleKey.NoName, strErrorMessage, null, null, vecWorldHintPos, RAGE.Elements.Player.LocalPlayer.Dimension, false, false, 10.0f, null, true, bAllowInVehicle: true);
							}
						}
					}
				}
			}
		}

		bool bCuffed = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.CUFFED);
		if (bCuffed)
		{
			OnCuffedTick();
		}

		foreach (var vehicle in OptimizationCachePool.StreamedInVehicles())
		{
			// IS POLICE
			bool bIsPolice = DataHelper.GetEntityData<bool>(vehicle, EDataNames.IS_POLICE_VEHICLE);
			if (bIsPolice) // boost
			{
				if (vehicle.GetSpeed() < 50.0f)
				{
					vehicle.SetEnginePowerMultiplier(1.3f);
					vehicle.SetEngineTorqueMultiplier(1.3f);
				}
				else
				{
					vehicle.SetEnginePowerMultiplier(1.0f);
					vehicle.SetEngineTorqueMultiplier(1.0f);
				}

				vehicle.SetMaxSpeed(1000.0f);
			}
		}
	}

	private void CleanupTicketUI()
	{
		if (m_PDTicketUI != null)
		{
			m_PDTicketUI.SetVisible(false, false, false);
		}

		m_PDTicketUI = null;
	}

	private void OnRequestTicket(string strOfficerName, float fAmount, string strReason)
	{
		CleanupTicketUI();

		// TODO_POST_LAUNCH: What happens if you get two tickets at once? Server might keep it around.
		m_PDTicketUI = new CGUIPDTicket(() => { });
		m_PDTicketUI.SetVisible(true, true, false);

		m_PDTicketUI.ShowTicket(strOfficerName, fAmount, strReason);
	}

	public void OnAcceptPDTicket()
	{
		CleanupTicketUI();
		NetworkEventSender.SendNetworkEvent_TicketResponse(true);
	}

	public void OnDeclinePDTicket()
	{
		CleanupTicketUI();
		NetworkEventSender.SendNetworkEvent_TicketResponse(false);
	}
}