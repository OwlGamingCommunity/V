using System;
using System.Collections.Generic;

public class VehicleModShop
{
	private CGUIVehicleModShop m_VehicleModShop = new CGUIVehicleModShop(() => { });

	private List<RAGE.Elements.Marker> m_lstEntranceMarkers = new List<RAGE.Elements.Marker>();
	private List<RAGE.Elements.Blip> m_lstEntranceBlips = new List<RAGE.Elements.Blip>();

	private const float g_fRadius = 3.0f;

	private float m_fCameraRot = 270.0f;
	const float g_fMaxZoom = 4.5f;
	const float g_fMinZoom = 0.5f;
	const float g_fDefaultZoom = g_fMaxZoom;
	float m_fCurrentZoom = g_fDefaultZoom;

	private EVehicleStoreRotationDirection m_RotationDirection = EVehicleStoreRotationDirection.None;
	private EVehicleStoreZoomDirection m_ZoomDirection = EVehicleStoreZoomDirection.None;

	private RAGE.Elements.Vehicle m_Vehicle = null;

	private RAGE.Elements.Vehicle m_DummyVehicle = null;

	private string g_strOriginalPlateText = "";

	public VehicleModShop()
	{
		// Paleto
		CreateMarker(new RAGE.Vector3(-61.44674f, 6443.448f, 30.8f));

		// LS world
		CreateMarker(new RAGE.Vector3(-362.5681f, -132.5629f, 37.9f));

		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.GotoVehicleModShop_Approved += OnGotoVehicleModShop_Approved;
		NetworkEvents.VehicleModShop_GotPrice += OnGotPrice;
		NetworkEvents.VehicleModShop_GotModPrice += OnGotModPrice;
		NetworkEvents.VehicleModShop_OnCheckout_Response += OnCheckoutResponse;

		CameraManager.RegisterCamera(ECameraID.VEHICLE_MOD_SHOP, VehicleModHelpers.VecModShopCarPosition.CopyVector(), VehicleModHelpers.VecModShopCarPosition.CopyVector(), new RAGE.Vector3(-90.0f, -90.0f, m_fCameraRot), 60.0f);
		UpdateCamera();
	}

	~VehicleModShop()
	{

	}

	private void CreateMarker(RAGE.Vector3 vecPos)
	{
		m_lstEntranceMarkers.Add(new RAGE.Elements.Marker(1, vecPos, g_fRadius, new RAGE.Vector3(0.0f, 0.0f, 0.0f), new RAGE.Vector3(g_fRadius, g_fRadius, 1.0f), new RAGE.RGBA(255, 255, 255, 120)));
		m_lstEntranceBlips.Add(new RAGE.Elements.Blip(402, vecPos, "Mod Shop", shortRange: true));
	}

	private void OnGotPrice(float fPrice, int CostGC, Dictionary<EModSlot, string> dictOverviewPrices)
	{
		m_VehicleModShop.SetPrice(fPrice, CostGC);

		GenerateOverview(dictOverviewPrices);
	}

	private void OnGotModPrice(float fPrice, int CostGC)
	{
		m_VehicleModShop.SetModCost(fPrice, CostGC);
	}

	private void OnCheckoutResponse(EVehicleModShopCheckoutResult result)
	{
		if (result == EVehicleModShopCheckoutResult.Success)
		{
			OnExit();
		}
		else
		{
			string strErrorMessage = "Unknown Error";
			switch (result)
			{
				case EVehicleModShopCheckoutResult.CannotAfford:
					{
						strErrorMessage = "You do not have enough money in your bank.";
						break;
					}

				case EVehicleModShopCheckoutResult.CannotAffordGC:
					{
						strErrorMessage = "You do not have enough donator currency.";
						break;
					}

				case EVehicleModShopCheckoutResult.PlateNotUnique:
					{
						strErrorMessage = "Vehicle plate text must be unique.";
						break;
					}

				case EVehicleModShopCheckoutResult.PlateNotValid:
					{
						strErrorMessage = "Vehicle plate text must be between 2 and 8 characters.";
						break;
					}
			}
			m_VehicleModShop.ShowErrorMessage(strErrorMessage);
		}
	}

	private bool IsRotating()
	{
		return m_RotationDirection != EVehicleStoreRotationDirection.None;
	}

	private bool IsZooming()
	{
		return m_ZoomDirection != EVehicleStoreZoomDirection.None;
	}

	public void OnStartRotation(EVehicleStoreRotationDirection direction)
	{
		m_RotationDirection = direction;
	}

	public void OnStopRotation()
	{
		m_RotationDirection = EVehicleStoreRotationDirection.None;
	}

	public void OnResetCamera()
	{
		m_RotationDirection = EVehicleStoreRotationDirection.None;
		m_ZoomDirection = EVehicleStoreZoomDirection.None;
		m_fCurrentZoom = g_fDefaultZoom;
		m_fCameraRot = 45.0f;
		UpdateCamera();
	}

	public void OnStartZoom(EVehicleStoreZoomDirection direction)
	{
		m_ZoomDirection = direction;
	}

	public void OnStopZoom()
	{
		m_ZoomDirection = EVehicleStoreZoomDirection.None;
	}

	private void UpdateCamera()
	{
		// Calculate cam pos
		var radians = m_fCameraRot * (3.14 / 180.0);
		RAGE.Vector3 vecCamPosNew = VehicleModHelpers.VecModShopCarPosition.CopyVector();
		vecCamPosNew.X += (float)Math.Cos(radians) * m_fCurrentZoom;
		vecCamPosNew.Y += (float)Math.Sin(radians) * m_fCurrentZoom;
		//vecCamPosNew.Z += 2.5f;
		vecCamPosNew.Z += 1.5f;

		CameraManager.UpdateCamera(ECameraID.VEHICLE_MOD_SHOP, vecCamPosNew, VehicleModHelpers.VecModShopCarPosition.CopyVector(), new RAGE.Vector3(-90.0f, -90.0f, 45.0f));
	}

	private void OnGotoVehicleModShop_Approved(RAGE.Elements.Vehicle vehicle)
	{
		m_Vehicle = vehicle;

		g_strOriginalPlateText = vehicle.GetNumberPlateText();
		m_dictPurchasesMods.Clear();

		// populate UI
		m_VehicleModShop.Reset();

		foreach (EModSlot modSlot in Enum.GetValues(typeof(EModSlot)))
		{
			bool bAdd = true;
			if (modSlot >= EModSlot.Spoilers)
			{
				if (modSlot != EModSlot.Armor)
				{
					int numMods = 0;
					if (modSlot == EModSlot.WindowTint)
					{
						numMods = 7;
					}
					else
					{
						numMods = vehicle.GetNumMods((int)modSlot);
					}
					bAdd = numMods > 0;
				}
				else
				{
					bAdd = false;
				}
			}

			if (bAdd)
			{
				m_VehicleModShop.AddModCategory((int)modSlot, System.Text.RegularExpressions.Regex.Replace(modSlot.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim());
			}
		}

		HUD.SetVisible(false, false, false);
		CameraManager.ActivateCamera(ECameraID.VEHICLE_MOD_SHOP);
		m_VehicleModShop.SetVisible(true, true, true);

		// reset overview
		GenerateOverview(null);

		// create dummy vehicle
		RAGE.Vector3 vecPos = vehicle.Position;
		vecPos.X -= 1.0f;
		vecPos.Y -= 1.0f;
		m_DummyVehicle = new RAGE.Elements.Vehicle(vehicle.Model, vecPos, 0, "", 0);
		m_DummyVehicle.Dimension = vehicle.Dimension;
		m_DummyVehicle.SetOnGroundProperly(1);
		m_DummyVehicle.FreezePosition(true);

		vehicle.SetOnGroundProperly(1);
		vehicle.FreezePosition(true);


	}

	private Dictionary<EModSlot, int> m_dictPurchasesMods = new Dictionary<EModSlot, int>();
	public void OnUpdateModIndex(int category, int index)
	{
		EModSlot modSlot = (EModSlot)category;
		// TODO: must set custom wheels and colors too
		if (modSlot >= EModSlot.Spoilers)
		{
			if (modSlot == EModSlot.WindowTint)
			{
				RAGE.Elements.Player.LocalPlayer.Vehicle.SetWindowTint(index);
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.Vehicle.SetMod(category, index, false);
			}

			m_dictPurchasesMods[modSlot] = index;

			if (modSlot == EModSlot.Horns)
			{
				// play horn
				m_DummyVehicle.SetMod(category, index, false);
				m_DummyVehicle.StartHorn(5000, HashHelper.GetHashUnsigned("HELDDOWN"), false);
			}
		}
		else
		{
			if (modSlot == EModSlot.CustomizePlateStyle)
			{
				// block out 4, its exempt
				// TODO: Make it so cops cannot change plate type
				if (index == 4)
				{
					index = 5;
				}

				RAGE.Elements.Player.LocalPlayer.Vehicle.SetNumberPlateTextIndex(index);
				m_dictPurchasesMods[modSlot] = index;
			}
		}

		int r = 0;
		int g = 0;
		int b = 0;
		RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);
		bool bNeonsEnabled = RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0);
		NetworkEventSender.SendNetworkEvent_VehicleModShop_GetModPrice(modSlot, index, RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText(), r, g, b, bNeonsEnabled);

		// TODO: fake clean the car + restore serverside, also have to restore all the mods etc incase they exit

		/*
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNumberPlateTextIndex(1);
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNumberPlateText("HELL CAT");
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNeonLightsColour(255, 194, 15);
		for (int i = 0; i < 10; ++i)
		{
			RAGE.Elements.Player.LocalPlayer.Vehicle.SetNeonLightEnabled(i, true);
		}
		*/
	}

	public void OnChangeModCategory(int category)
	{
		m_VehicleModShop.SetModCost(0.0f, 0);

		int r = 0;
		int g = 0;
		int b = 0;

		EModSlot modSlot = (EModSlot)category;
		int numMods = 0;
		int currentIndex = 0;
		if (modSlot >= EModSlot.Spoilers)
		{
			if (modSlot == EModSlot.WindowTint)
			{
				numMods = 7;
				currentIndex = RAGE.Elements.Player.LocalPlayer.Vehicle.GetWindowTint();
			}
			else
			{
				numMods = RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumMods(category);
				currentIndex = Math.Max(0, RAGE.Elements.Player.LocalPlayer.Vehicle.GetMod(category));
			}
		}
		else
		{
			if (modSlot == EModSlot.CustomizePlateStyle)
			{
				numMods = 4;
				// block out 4, its exempt
				// TODO: Make it so cops cannot change plate type
				currentIndex = RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateTextIndex();
			}
			else if (modSlot == EModSlot.Neons)
			{
				RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);
				m_VehicleModShop.GotoCustomizeNeons(RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0), r, g, b);
				return;
			}
			else if (modSlot == EModSlot.CustomizePlateText)
			{
				m_VehicleModShop.GotoCustomizePlateText(RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText());
				return;
			}
		}

		m_VehicleModShop.SetNumModsThisCategory(currentIndex, numMods);

		// retrieve price
		RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);
		bool bNeonsEnabled = RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0);
		NetworkEventSender.SendNetworkEvent_VehicleModShop_GetModPrice(modSlot, currentIndex, RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText(), r, g, b, bNeonsEnabled);

		m_VehicleModShop.FinalizeGotoSelectMod(category);
	}

	public void OnUpdatePrice()
	{
		bool bNeonsEnabled = RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0);

		int r = 0;
		int g = 0;
		int b = 0;
		RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);

		NetworkEventSender.SendNetworkEvent_VehicleModShop_GetPrice(m_dictPurchasesMods, RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText(), r, g, b, bNeonsEnabled);
	}

	public void OnSetPlateText(string strPlateText)
	{
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNumberPlateText(strPlateText);
		m_dictPurchasesMods[EModSlot.CustomizePlateText] = 0; // Fake so it triggers serverisde change
		RequestModCost(EModSlot.CustomizePlateText, 0);
	}

	private void RequestModCost(EModSlot modSlot, int index)
	{
		int r = 0;
		int g = 0;
		int b = 0;
		RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);
		bool bNeonsEnabled = RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0);
		NetworkEventSender.SendNetworkEvent_VehicleModShop_GetModPrice(modSlot, index, RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText(), r, g, b, bNeonsEnabled);
	}

	public void GenerateOverview(Dictionary<EModSlot, string> dictOverview)
	{
		if (dictOverview == null || dictOverview.Count == 0)
		{
			m_VehicleModShop.ResetModOverview();
		}
		else
		{
			m_VehicleModShop.ResetModOverviewToBlank();

			foreach (var kvPair in dictOverview)
			{
				string strName = System.Text.RegularExpressions.Regex.Replace(kvPair.Key.ToString(), "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
				string strDisplayPrice = kvPair.Value;
				m_VehicleModShop.AddModToOverview(strName, strDisplayPrice);
			}
		}
	}


	public void OnResetPlateText()
	{
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNumberPlateText(g_strOriginalPlateText);
		m_VehicleModShop.GotoCustomizePlateText(g_strOriginalPlateText);
		RequestModCost(EModSlot.CustomizePlateText, 0);
	}

	public void OnChangeNeonsColor(uint r, uint g, uint b)
	{
		RAGE.Elements.Player.LocalPlayer.Vehicle.SetNeonLightsColour((int)r, (int)g, (int)b);
		m_dictPurchasesMods[EModSlot.Neons] = 0; // Fake so it triggers serverisde change

		// dont request cost because its spammy due to draggy color picker
		VehicleModHelpers.CalculateModChangeCost(EModSlot.Neons, 0, out float fModCost, out int ModCostGC);
		OnGotModPrice(fModCost, ModCostGC);
	}

	public void OnUpdateNeonState(bool bEnabled)
	{
		for (int i = 0; i < 4; ++i)
		{
			RAGE.Elements.Player.LocalPlayer.Vehicle.SetNeonLightEnabled(i, bEnabled);
		}

		m_dictPurchasesMods[EModSlot.Neons] = 0; // Fake so it triggers serverisde change
		RequestModCost(EModSlot.Neons, 0);
	}

	public void OnCheckout()
	{
		bool bNeonsEnabled = RAGE.Elements.Player.LocalPlayer.Vehicle.IsNeonLightEnabled(0);

		int r = 0;
		int g = 0;
		int b = 0;
		RAGE.Elements.Player.LocalPlayer.Vehicle.GetNeonLightsColour(ref r, ref g, ref b);

		NetworkEventSender.SendNetworkEvent_VehicleModShop_OnCheckout(m_dictPurchasesMods, RAGE.Elements.Player.LocalPlayer.Vehicle.GetNumberPlateText(), r, g, b, bNeonsEnabled);
	}

	public void OnExit_Discard()
	{
		NetworkEventSender.SendNetworkEvent_VehicleModShop_OnExit_Discard();

		OnExit();
	}

	private void OnExit()
	{
		if (m_Vehicle != null)
		{
			m_Vehicle.FreezePosition(false);
		}

		CameraManager.DeactivateCamera(ECameraID.VEHICLE_MOD_SHOP);

		m_VehicleModShop.SetVisible(false, false, true);
		HUD.SetVisible(true, false, false);

		RAGE.Game.Ui.DisplayRadar(true);

		m_Vehicle = null;

		if (m_DummyVehicle != null)
		{
			m_DummyVehicle.Destroy();
			m_DummyVehicle = null;
		}
	}

	private RAGE.Elements.Marker GetNearestMarker()
	{
		RAGE.Elements.Marker returnMarker = null;
		float fSmallestDist = 999999.0f;

		foreach (RAGE.Elements.Marker marker in m_lstEntranceMarkers)
		{
			if (marker.Dimension == RAGE.Elements.Player.LocalPlayer.Dimension)
			{
				RAGE.Vector3 vecMarkerPos = marker.Position;
				RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
				float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecMarkerPos);

				if (fDistance <= g_fRadius && fDistance <= fSmallestDist)
				{
					fSmallestDist = fDistance;
					returnMarker = marker;
				}
			}
		}

		return returnMarker;
	}


	private void OnRender()
	{
		if (RAGE.Elements.Player.LocalPlayer.Vehicle == null)
		{
			return;
		}

		if (m_VehicleModShop.IsVisible())
		{
			m_DummyVehicle.SetAlpha(0, false);

			RAGE.Elements.Player.LocalPlayer.Vehicle.SetDirtLevel(0.0f);

			// camera rotation
			if (IsRotating())
			{
				const float fDeltaRot = 4.0f;

				if (m_RotationDirection == EVehicleStoreRotationDirection.Left)
				{
					m_fCameraRot -= fDeltaRot;
				}
				else if (m_RotationDirection == EVehicleStoreRotationDirection.Right)
				{
					m_fCameraRot += fDeltaRot;
				}

				if (m_fCameraRot >= 360.0f)
				{
					m_fCameraRot = 0.0f;
				}
				else if (m_fCameraRot <= 0.0f)
				{
					m_fCameraRot = 360.0f;
				}
			}

			if (IsZooming())
			{
				const float fDeltaZoom = 0.1f;
				m_fCurrentZoom += (m_ZoomDirection == EVehicleStoreZoomDirection.In) ? -fDeltaZoom : fDeltaZoom;
				m_fCurrentZoom = Math.Clamp(m_fCurrentZoom, g_fMinZoom, g_fMaxZoom);
			}

			if (IsRotating() || IsZooming())
			{
				UpdateCamera();
			}

			// TODO: Better way of tracking this
			if (m_VehicleModShop.IsVisible())
			{
				RAGE.Game.Ui.DisplayRadar(false);
			}
		}

		// world hint
		RAGE.Elements.Marker entranceMarker = GetNearestMarker();
		if (entranceMarker != null)
		{
			string strMessage = String.Empty;
			ConsoleKey key = ConsoleKey.NoName;
			if (RAGE.Elements.Player.LocalPlayer.Vehicle == null || !CanInteractWithModShop())
			{
				strMessage = "You must be in a vehicle to use the Vehicle Mod Shop";
				key = ConsoleKey.NoName;
			}
			else
			{
				strMessage = "Enter Vehicle Mod Shop";
				key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
			}

			WorldHintManager.DrawExclusiveWorldHint(key, strMessage, null, InteractWithModShop, entranceMarker.Position, entranceMarker.Dimension, false, false, g_fRadius, bAllowInVehicle: true);
		}
	}

	private bool CanInteractWithModShop()
	{
		EVehicleClass vehicleClass = (EVehicleClass)RAGE.Elements.Player.LocalPlayer.Vehicle.GetClass();
		if (vehicleClass == EVehicleClass.VehicleClass_Cycles
			|| vehicleClass == EVehicleClass.VehicleClass_Boats
			|| vehicleClass == EVehicleClass.VehicleClass_Helicopters
			|| vehicleClass == EVehicleClass.VehicleClass_Planes
			|| vehicleClass == EVehicleClass.VehicleClass_Trains)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	private void InteractWithModShop()
	{
		NetworkEventSender.SendNetworkEvent_GotoVehicleModShop();
	}
}

