using RAGE;
using System;
using System.Collections.Generic;
using EntityDatabaseID = System.Int64;
// TODO_CSHARP: Check this all works

public class DutySystem
{
	const float g_fDistThreshold = 3.0f;
	private CGUIDuty m_DutyUI = new CGUIDuty(OnUILoaded);
	private Vector3 m_VecOffset = new Vector3();
	private RAGE.Elements.Vehicle m_Vehicle = null;
	private DutyOutfitEditor m_OutfitEditor = new DutyOutfitEditor();
	private EDutyType m_DutyType = EDutyType.None;
	private List<CItemInstanceDef> m_lstOutfits = new List<CItemInstanceDef>();

	private EntityDatabaseID m_CurrentOutfitDBID = -1;
	private RAGE.Vector3 m_vecRestorePos = null;

	private bool m_bExitIfNoOutfits = false;

	public DutySystem()
	{
		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.Duty_SelectOutfit_DropdownSelectionChanged += OnOutfitSelectionChanged;
		UIEvents.Duty_SelectOutfit_DropdownSelectionHoverChanged += OnOutfitSelectionPreview;

		NetworkEvents.UseDutyPointResult += OnUseDutyPointResult;
		NetworkEvents.DutySystem_GotUpdatedOutfitList += OnGotUpdatedOutfitList;
	}

	~DutySystem()
	{

	}

	public void OnExitEditor()
	{
		m_bExitIfNoOutfits = true;
		NetworkEventSender.SendNetworkEvent_DutySystem_RequestUpdatedOutfitList(m_DutyType);
	}

	private void OnOutfitSelectionChanged(string strName, string strValue)
	{
		CItemInstanceDef outfit = ApplyPreview(strName, strValue);

		if (outfit != null)
		{
			m_CurrentOutfitDBID = outfit.DatabaseID;
		}
	}

	private void OnOutfitSelectionPreview(string strName, string strValue)
	{
		ApplyPreview(strName, strValue);
	}

	private CItemInstanceDef ApplyPreview(string strName, string strValue)
	{
		int outfitIndex = Convert.ToInt32(strValue);
		CItemInstanceDef outfit = m_lstOutfits[outfitIndex];
		m_OutfitEditor.ApplyOutfitPreview(outfit, true);
		return outfit;
	}

	private void OnGotUpdatedOutfitList(List<CItemInstanceDef> lstOutfits)
	{
		// If we have no outfits, just exit fully so we dont get stuck
		if (m_bExitIfNoOutfits && lstOutfits.Count == 0)
		{
			CancelGoingOnDuty();
		}
		else
		{
			SharedInitDuty(lstOutfits);
		}
	}

	private void SharedInitDuty(List<CItemInstanceDef> lstOutfits)
	{
		m_DutyUI.SetVisible(true, true, true);

		HUD.SetVisible(false, false, false);

		m_CurrentOutfitDBID = -1;

		CreateScene();

		m_lstOutfits = lstOutfits;

		// Do we have any outfits made?
		if (lstOutfits.Count > 0)
		{
			// Do we have something active?
			CItemInstanceDef activeOutfit = null;
			bool bHasActiveDutySkin = false;
			foreach (CItemInstanceDef outfit in lstOutfits)
			{
				CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(outfit.GetValueDataSerialized(), EJsonTrackableIdentifier.InitDuty);

				if (itemValue.IsActive)
				{
					m_CurrentOutfitDBID = outfit.DatabaseID;
					activeOutfit = outfit;
					bHasActiveDutySkin = true;
					break;
				}
			}

			// Setup preview (or go to editor if they dont have one)
			if (bHasActiveDutySkin)
			{
				// nothing to apply here, server already did it
				RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.SetAlpha(0, false);
				NotificationManager.ShowNotification("Duty", "You do not have an active outfit. Please select one, or create a new outfit.", ENotificationIcon.InfoSign);
			}

			// dropdown to select outfit
			int index = 0;
			m_DutyUI.ClearDropdown();
			foreach (CItemInstanceDef outfit in lstOutfits)
			{
				CItemValueDutyOutfit itemValue = OwlJSON.DeserializeObject<CItemValueDutyOutfit>(outfit.GetValueDataSerialized(), EJsonTrackableIdentifier.InitDuty2);
				m_DutyUI.AddDropdownItem(itemValue.Name, index.ToString());
				++index;

				// setup default text
				if (itemValue.IsActive)
				{
					m_DutyUI.SetDropdownText(itemValue.Name);
				}
			}
		}
	}


	private void OnUseDutyPointResult(bool bSuccess, EDutyType a_Type, List<CItemInstanceDef> lstOutfits)
	{
		if (bSuccess)
		{
			m_vecRestorePos = RAGE.Elements.Player.LocalPlayer.Position.CopyVector();
			m_DutyType = a_Type;
			// TODO_DUTY: Beards?
			/*
			// cache beard if we have one so we can show it when they set masks to none
			int currentMask = RAGE.Elements.Player.LocalPlayer.GetDrawableVariation((int)ECustomClothingComponent.Masks);
			if (MaskHelpers.MasksFunctioningAsBeards.Contains(currentMask))
			{
				m_FullBeardDrawable = currentMask;
				m_FullBeardTexture = RAGE.Elements.Player.LocalPlayer.GetTextureVariation((int)ECustomClothingComponent.Masks);
			}
			*/
			// Do we have any outfits made?
			if (lstOutfits.Count > 0)
			{
				// TODO_CSHARP: Should really come after we populate, but for ondemand loading currently this has to be first. We need to fix that.
				m_bExitIfNoOutfits = true;
				SharedInitDuty(lstOutfits);
			}
			else // go straight to editor
			{
				m_bExitIfNoOutfits = false;
				GotoOutfits();
			}
		}
	}

	private void CleanupVehicle()
	{
		if (m_Vehicle != null)
		{
			m_Vehicle.Destroy();
			m_Vehicle = null;
		}
	}

	private void CreateScene()
	{
		if (m_DutyType == EDutyType.Law_Enforcement)
		{
			// vehicle
			CleanupVehicle();
			m_Vehicle = new RAGE.Elements.Vehicle(1887487254, new RAGE.Vector3(-1492.1362f, -593.11957f, 22.243276f), 20.0f, " POLICE ", 255, true, 0, 0, RAGE.Elements.Player.LocalPlayer.Dimension);

			// veh mods
			m_Vehicle.SetWindowTint(3);
			for (int i = 0; i < 32; ++i)
			{
				m_Vehicle.SetExtra(i, true);
			}

			RAGE.Vector3 vecCamPos = new RAGE.Vector3(-1495.8264f, -588.6093f, 24.274862f);
			RAGE.Vector3 vecCamLookAt = new RAGE.Vector3(-1493.8164f, -593.82465f, 23.374862f);
			RAGE.Elements.Player.LocalPlayer.Position = vecCamLookAt;
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 20.0f, 0, false);

			CameraManager.RegisterCamera(ECameraID.DUTY_SCENE, vecCamPos, vecCamLookAt);
			CameraManager.ActivateCamera(ECameraID.DUTY_SCENE);
		}
		else if (m_DutyType == EDutyType.EMS)
		{
			RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(-1493.8164f, -593.82465f, 23.243276f);
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 20.0f, 0, false);

			// vehicle
			CleanupVehicle();
			m_Vehicle = new RAGE.Elements.Vehicle(1171614426, new RAGE.Vector3(295.84277f, -584.20685f, 42.921f), -17.617897f, "  EMS  ", 255, true, 0, 0, RAGE.Elements.Player.LocalPlayer.Dimension);


			RAGE.Vector3 vecCamPos = new RAGE.Vector3(294.06f, -573.7115f, 43.576937f);
			RAGE.Vector3 vecCamLookAt = new RAGE.Vector3(293.70493f, -583.4422f, 43.18695f);
			RAGE.Elements.Player.LocalPlayer.Position = vecCamLookAt;
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 20.0f, 0, false);

			CameraManager.RegisterCamera(ECameraID.DUTY_SCENE, vecCamPos, vecCamLookAt);
			CameraManager.ActivateCamera(ECameraID.DUTY_SCENE);
		}
		else if (m_DutyType == EDutyType.Fire)
		{
			RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(-1493.8164f, -593.82465f, 23.243276f);
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 20.0f, 0, false);

			// vehicle
			CleanupVehicle();
			m_Vehicle = new RAGE.Elements.Vehicle(1938952078, new RAGE.Vector3(221.75996f, -1639.5045f, 29.449696f), -38.30886f, "   FD   ", 255, true, 28, 28, RAGE.Elements.Player.LocalPlayer.Dimension);


			RAGE.Vector3 vecCamPos = new RAGE.Vector3(225.14972f, -1627.541f, 29.81564f);
			RAGE.Vector3 vecCamLookAt = new RAGE.Vector3(221.79362f, -1636.0796f, 29.199879f);
			RAGE.Elements.Player.LocalPlayer.Position = vecCamLookAt;
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, -32.526398f, 0, false);

			CameraManager.RegisterCamera(ECameraID.DUTY_SCENE, vecCamPos, vecCamLookAt);
			CameraManager.ActivateCamera(ECameraID.DUTY_SCENE);
		}
		else if (m_DutyType == EDutyType.News)
		{
			RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(-1493.8164f, -593.82465f, 23.243276f);
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 20.0f, 0, false);

			// vehicle
			CleanupVehicle();
			m_Vehicle = new RAGE.Elements.Vehicle(1162065741, new RAGE.Vector3(-617.2728f, -933.5786f, 22.244247f), 114.894775f, "  NEWS  ", 255, true, 24, 24, RAGE.Elements.Player.LocalPlayer.Dimension);
			m_Vehicle.SetLivery(0);

			RAGE.Vector3 vecCamPos = new RAGE.Vector3(-625.07825f, -945.2968f, 21.75708f);
			RAGE.Vector3 vecCamLookAt = new RAGE.Vector3(-617.0102f, -935.5914f, 22.163412f);
			RAGE.Elements.Player.LocalPlayer.Position = vecCamLookAt;
			RAGE.Elements.Player.LocalPlayer.SetRotation(0.0f, 0.0f, 127.06527f, 0, false);

			CameraManager.RegisterCamera(ECameraID.DUTY_SCENE, vecCamPos, vecCamLookAt);
			CameraManager.ActivateCamera(ECameraID.DUTY_SCENE);
		}
	}

	private static void OnUILoaded()
	{

	}

	public bool IsInDutyUI()
	{
		return m_DutyUI.IsVisible();
	}

	public void GoOnDuty()
	{
		NetworkEventSender.SendNetworkEvent_FinalizeGoOnDuty(m_DutyType, m_CurrentOutfitDBID);
		ExitDuty();
	}

	public void GotoOutfits()
	{
		m_DutyUI.SetVisible(false, false, false);
		m_OutfitEditor.Activate(m_DutyType);
	}

	private void ExitDuty()
	{
		m_DutyUI.SetVisible(false, false, true);

		RAGE.Elements.Player.LocalPlayer.Position = m_vecRestorePos;
		m_vecRestorePos = null;
		RAGE.Elements.Player.LocalPlayer.SetAlpha(255, false);

		RAGE.Elements.Player.LocalPlayer.ClearTasksImmediately();

		CleanupVehicle();
		CameraManager.DeactivateCamera(ECameraID.DUTY_SCENE);
		HUD.SetVisible(true, false, false);

		RAGE.Game.Ui.DisplayRadar(true);
	}

	public void CancelGoingOnDuty()
	{
		ExitDuty();
		NetworkEventSender.SendNetworkEvent_CancelGoingOnDuty();
	}

	private RAGE.Elements.Marker GetNearestDutyPoint()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.DutyPoint);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private void OnRender()
	{
		if (m_DutyUI.IsVisible())
		{
			string strText = "";
			int r = 0;
			int g = 0;
			int b = 0;

			if (m_DutyType == EDutyType.Law_Enforcement)
			{
				strText = "Los Santos Police Department";
				r = 0;
				g = 115;
				b = 208;
			}
			else if (m_DutyType == EDutyType.EMS)
			{
				strText = "Los Santos Emergency Medical Services";
				r = 169;
				g = 14;
				b = 21;
			}
			else if (m_DutyType == EDutyType.Fire)
			{
				strText = "Los Santos Fire Department";
				r = 169;
				g = 14;
				b = 21;
			}

			RAGE.Game.Graphics.DrawRect(0.0f, 0.885f, 4.0f, 0.05f, r, g, b, 200, 0);
			TextHelper.Draw2D(strText, 0.5f, 0.865f, 0.5f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);


			RAGE.Game.Ui.DisplayRadar(false);

			string strDict = "amb@world_human_cop_idles@male@idle_enter";
			string strAnim = "idle_intro";
			if (!RAGE.Game.Streaming.HasAnimDictLoaded(strDict))
			{
				RAGE.Game.Streaming.RequestAnimDict(strDict);
			}
			else
			{
				if (!RAGE.Elements.Player.LocalPlayer.IsPlayingAnim(strDict, strAnim, 3))
				{
					RAGE.Game.Ai.TaskPlayAnim(RAGE.Elements.Player.LocalPlayer.Handle, strDict, strAnim, 8.0f, 1.0f, -1, (int)AnimationFlags.StopOnLastFrame, 1.0f, false, false, false);
				}
			}
		}

		RAGE.Elements.Marker nearestDutyPoint = GetNearestDutyPoint();
		EDutyType dutyPointType = DataHelper.GetEntityData<EDutyType>(nearestDutyPoint, EDataNames.DUTY_TYPE);
		if (nearestDutyPoint != null)
		{
			string strDutyType = String.Empty;
			if (dutyPointType == EDutyType.Law_Enforcement)
			{
				strDutyType = "Law Enforcement";
			}
			else if (dutyPointType == EDutyType.EMS)
			{
				strDutyType = "EMS";
			}
			else if (dutyPointType == EDutyType.Fire)
			{
				strDutyType = "Fire Department";
			}
			else if (dutyPointType == EDutyType.News)
			{
				strDutyType = "News Agency";
			}
			else if (dutyPointType == EDutyType.Towing)
			{
				strDutyType = "Tow truck driver";
			}

			// TODO: Should we only show the prompt if they are in this faction?
			EDutyType currentDutyType = DataHelper.GetLocalPlayerEntityData<EDutyType>(EDataNames.DUTY);
			bool bIsOnDuty = currentDutyType != EDutyType.None;
			bool bIsOnOtherDutyType = false;
			if (bIsOnDuty)
			{
				if (currentDutyType != dutyPointType)
				{
					bIsOnOtherDutyType = true;
				}
			}

			m_VecOffset = nearestDutyPoint.Position;
			m_VecOffset.Z += 0.60f;
			if (!bIsOnOtherDutyType)
			{
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), Helpers.FormatString("Go " + (bIsOnDuty ? "off" : "on") + " duty as " + strDutyType), null, OnInteractWithDutyPoint, m_VecOffset, nearestDutyPoint.Dimension, false, false, g_fDistThreshold);
			}
			else
			{
				string strCurrentDutyType = String.Empty;
				if (currentDutyType == EDutyType.Law_Enforcement)
				{
					strCurrentDutyType = "Law Enforcement";
				}
				else if (currentDutyType == EDutyType.EMS)
				{
					strCurrentDutyType = "EMS";
				}
				else if (currentDutyType == EDutyType.Fire)
				{
					strCurrentDutyType = "Fire Department";
				}
				else if (currentDutyType == EDutyType.News)
				{
					strCurrentDutyType = "News Agency";
				}
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.DummyNone), Helpers.FormatString("You are already on duty as {0}", strCurrentDutyType), null, () => { }, m_VecOffset, nearestDutyPoint.Dimension, false, false, g_fDistThreshold);
			}
		}
	}

	private void OnInteractWithDutyPoint()
	{
		RAGE.Elements.Marker nearestDutyPoint = GetNearestDutyPoint();
		EDutyType dutyPointType = DataHelper.GetEntityData<EDutyType>(nearestDutyPoint, EDataNames.DUTY_TYPE);
		if (nearestDutyPoint != null)
		{
			NetworkEventSender.SendNetworkEvent_UseDutyPoint(dutyPointType);
		}
	}
}