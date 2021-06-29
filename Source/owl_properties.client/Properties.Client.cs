#define ENABLE_MAILBOX
using System;
using System.Collections.Generic;
using RAGE;

public class PropertyManager
{
	private Dictionary<int, RAGE.Elements.Blip> m_dictOwnedPropertyBlips = new Dictionary<int, RAGE.Elements.Blip>();

	private const float MAX_MOWER_DISTANCE = 40f;
	private const Int64 MOWING_INCREASE_INCREMENT_SECONDS = 5;
	private const int MOWING_INCREMENT_AMOUNT = 4;
	private const int TOTAL_MOWING_XP = 100;
	private const int ONE_WEEK = 604800;
	private const int ALLOWED_MOWING_AFTER = ONE_WEEK;

	private uint MowingProgress = 0;
	private Int64 LastMowingIncrement = 0;
	private uint AwayFromMowerCounter = 0;
	private RAGE.Elements.Marker MowingProperty;

	public PropertyManager()
	{
		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.ChangeCharacterApproved += OnChangeCharacterApproved;
		NetworkEvents.PurchaseProperty_RequestInfoResponse += OnRequestInfoResponse;

		NetworkEvents.EnterInteriorApproved += OnEnterInteriorApproved;
		NetworkEvents.ExitInteriorApproved += OnExitInteriorApproved;

		NetworkEvents.EnterElevatorApproved += OnEnterElevatorApproved;
		NetworkEvents.ExitElevatorApproved += OnExitElevatorApproved;
		
		NetworkEvents.EnterVehicleReal += OnEnterVehicle;
		NetworkEvents.ExitVehicleReal += OnExitVehicle;

		UIEvents.HidePurchasePropertyUI += HidePurchasePropertyUI;
		UIEvents.PurchaseProperty_OnCheckout += OnCheckout;
		UIEvents.PurchaseProperty_OnPreview += OnPreview;

		UIEvents.PurchaseProperty_SetMonthlyDownpayment += SetDownpayment;
		UIEvents.PurchaseProperty_SetNumMonthlyPayments += SetNumMonthlyPayments;

		m_PurchasePropertyGUI = new CGUIPurchaseProperty(OnUILoaded);

		NetworkEvents.LocalPlayerStreamInNewArea += OnLocalPlayerStreamInNewArea;

		NetworkEvents.LoadCustomMap += OnLoadCustomMap;
		NetworkEvents.UnloadCustomMap += OnUnloadCustomMap;

		NetworkEvents.Property_CreatePlayerBlip += OnCreatePlayerBlip;
		NetworkEvents.Property_DestroyPlayerBlip += OnDestroyPlayerBlip;
		NetworkEvents.Property_DestroyAllPlayerBlips += OnDestroyAllPlayerBlips;

		CreateWorldObjects();
	}

	private void OnEnterVehicle(RAGE.Elements.Vehicle vehicle, int seatId)
	{
		bool isDriver = seatId == (int)EVehicleSeat.Driver;
		bool isMower = vehicle.IsModel(1783355638);
		if (!isMower || !isDriver)
		{
			return;
		}

		Int64 charID = DataHelper.GetLocalPlayerEntityData<Int64>(EDataNames.CHARACTER_ID);
		if (!GetNearestOwnedProperty(charID, out RAGE.Elements.Marker nearestProp))
		{
			return;
		}

		if (nearestProp.Position.DistanceTo(PlayerHelper.GetLocalPlayerPosition()) > MAX_MOWER_DISTANCE)
		{
			return;
		}

		string propertyName = DataHelper.GetEntityData<string>(nearestProp, EDataNames.PROP_NAME);
		Int64 lastMowedAt = DataHelper.GetEntityData<Int64>(nearestProp, EDataNames.PROP_LAST_MOWED_AT);
		Int64 now = Helpers.GetUnixTimestamp(true);
		if (now - lastMowedAt < ALLOWED_MOWING_AFTER)
		{
			NotificationManager.ShowNotification(
				"Cannot Mow Here", 
				Helpers.FormatString("You have mowed the property {0} too recently to mow it again.", propertyName),
				ENotificationIcon.ExclamationSign
			);
			return;
		}

		MowingProperty = nearestProp;
		NotificationManager.ShowNotification(
			"Started Mowing Lawn", 
			Helpers.FormatString(
				"You have started mowing your lawn for property {0}! Keep mowing to increase your properties' XP.",
				propertyName
			),
			ENotificationIcon.InfoSign
		);
	}

	private void OnExitVehicle(RAGE.Elements.Vehicle vehicle)
	{
		if (MowingProperty != null)
		{
			ResetMowerState();
		}
	}

	private void ResetMowerState()
	{
		MowingProgress = 0;
		LastMowingIncrement = 0;
		MowingProperty = null;
		AwayFromMowerCounter = 0;
		NetworkEvents.SendLocalEvent_HideProgressBar();
	}

	private bool GetNearestOwnedProperty(Int64 charID, out RAGE.Elements.Marker nearestMarker)
	{
		RAGE.Elements.Player localPlayer = PlayerHelper.GetLocalPlayer();
		Vector3 playerPos = PlayerHelper.GetLocalPlayerPosition();
		float nearestDistance = 1000.0f;
		nearestMarker = null;
		
		foreach (RAGE.Elements.Marker marker in RAGE.Elements.Entities.Markers.Streamed)
		{
			if (marker.Dimension != localPlayer.Dimension)
			{
				continue;
			}
			
			if (!isOwnerOrRenterOfProperty(charID, marker))
			{
				continue;
			}

			float distance = marker.Position.DistanceTo(playerPos);
			if (distance < nearestDistance)
			{
				nearestDistance = distance;
				nearestMarker = marker;
			}
		}

		return nearestMarker != null;
	}

	private bool isOwnerOrRenterOfProperty(Int64 charID, RAGE.Elements.Marker property)
	{
		EPropertyOwnerType ownerType = DataHelper.GetEntityData<EPropertyOwnerType>(property, EDataNames.PROP_OWNER_TYPE);
		EPropertyOwnerType renterType = DataHelper.GetEntityData<EPropertyOwnerType>(property, EDataNames.PROP_RENTER_TYPE);
		Int64 ownerID = DataHelper.GetEntityData<Int64>(property, EDataNames.PROP_OWNER_ID);
		Int64 renterID = DataHelper.GetEntityData<Int64>(property, EDataNames.PROP_RENTER_ID);
		EPropertyState state = DataHelper.GetEntityData<EPropertyState>(property, EDataNames.PROP_STATE);
		

		if (state == EPropertyState.AvailableToBuy)
		{
			return false;
		}

		if (ownerType == EPropertyOwnerType.Player && charID == ownerID)
		{
			return true;
		}

		if (renterType == (int)EPropertyOwnerType.Player && charID == renterID)
		{
			return true;
		}

		return false;
	}

	private void OnLoadCustomMap(int mapID)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		if (mapID != -1)
		{
			ShowLoadingInteriorMessage(true);

			MapLoader.LoadMap(mapID, (bool bSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) =>
			{
				if (bSuccess)
				{
					ShowLoadingInteriorMessage(false);
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
			});
		}
	}

	private void OnUnloadCustomMap(int mapID)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		if (mapID != -1)
		{

			MapLoader.UnloadMap(mapID, (bool bSuccess) =>
			{
				if (bSuccess)
				{
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
			});
		}
	}

	private void OnCreatePlayerBlip(int PropertyID, string strName, RAGE.Vector3 vecPos)
	{
		OnDestroyPlayerBlip(PropertyID);

		RAGE.Elements.Blip newBlip = new RAGE.Elements.Blip(40, vecPos, strName, 1, 2, 255, 0, true);

		m_dictOwnedPropertyBlips[PropertyID] = newBlip;

	}

	private void OnDestroyPlayerBlip(int PropertyID)
	{
		if (m_dictOwnedPropertyBlips.ContainsKey(PropertyID))
		{
			if (m_dictOwnedPropertyBlips[PropertyID] != null)
			{
				m_dictOwnedPropertyBlips[PropertyID].Destroy();
			}

			m_dictOwnedPropertyBlips.Remove(PropertyID);
		}
	}

	private void OnDestroyAllPlayerBlips()
	{
		foreach (var kvPair in m_dictOwnedPropertyBlips)
		{
			kvPair.Value.Destroy();
		}

		m_dictOwnedPropertyBlips.Clear();
	}

	private void OnLocalPlayerStreamInNewArea(RAGE.Vector3 vecOldArea, RAGE.Vector3 vecNewArea)
	{
		/*
		foreach (RAGE.Elements.Marker marker in RAGE.Elements.Entities.Markers.All)
		{
			RAGE.Vector3 vecPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;
			RAGE.Vector3 vecMarkerPos = marker.Position;
			float fDistance = WorldHelper.GetDistance(vecPlayerPos, vecMarkerPos);

			if (fDistance <= 200.0f)
			{
				bool isProperty = DataHelper.GetEntityData<bool>(marker, EDataNames.PROP_ENTER) || DataHelper.GetEntityData<bool>(marker, EDataNames.PROP_EXIT);
				if (isProperty)
				{
					RAGE.Vector3 vecGroundPos = vecMarkerPos.CopyVector();

					bool bIsExit = DataHelper.GetEntityData<bool>(marker, EDataNames.ELEVATOR_EXIT);
					if (bIsExit)
					{
						vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 5.0f) + 0.05f; // + 0.1f
					}
					else
					{
						vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 1.5f) + 0.05f; // + 0.24f
					}

					marker.Position = vecGroundPos;
				}

				bool isElevator = DataHelper.GetEntityData<bool>(marker, EDataNames.ELEVATOR_ENTRANCE) || DataHelper.GetEntityData<bool>(marker, EDataNames.ELEVATOR_EXIT);
				if (isElevator)
				{
					RAGE.Vector3 vecGroundPos = vecMarkerPos.CopyVector();

					bool bIsExit = DataHelper.GetEntityData<bool>(marker, EDataNames.PROP_EXIT);
					if (bIsExit)
					{
						vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 5.0f) + 0.1f;
					}
					else
					{
						vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 1.5f) + 0.24f;
					}

					marker.Position = vecGroundPos;
				}

				// TODO: Move to fuel system
				bool isFuelPoint = DataHelper.GetEntityData<bool>(marker, EDataNames.FUEL_POINT);
				if (isFuelPoint)
				{
					RAGE.Vector3 vecGroundPos = vecMarkerPos.CopyVector();
					vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 1.5f) + 0.05f;
					marker.Position = vecGroundPos;
				}

				// TODO: Move to carwash system
				bool isCarWashPoint = DataHelper.GetEntityData<bool>(marker, EDataNames.CARWASH_POINT);
				if (isCarWashPoint)
				{
					RAGE.Vector3 vecGroundPos = vecMarkerPos.CopyVector();
					vecGroundPos.Z = WorldHelper.GetGroundPosition(vecGroundPos, true, 0.0f, true, 0.0f, 1.5f) + 0.05f;
					marker.Position = vecGroundPos;
				}
			}
		}
		*/
	}

	class WorldObjectProperty
	{
		public WorldObjectProperty(string a_strObjectName, RAGE.Vector3 a_vecPos, RAGE.Vector3 a_vecRot)
		{
			strObjectName = a_strObjectName;
			vecPos = a_vecPos;
			vecRot = a_vecRot;
		}

		public RAGE.Elements.MapObject Create()
		{
			uint hash = HashHelper.GetHashUnsigned(strObjectName);
			AsyncModelLoader.RequestSyncInstantLoad(hash);

			return new RAGE.Elements.MapObject(hash, vecPos, vecRot, 255, 0);
		}

		private string strObjectName;
		private RAGE.Vector3 vecPos;
		private RAGE.Vector3 vecRot;
	}

	private List<WorldObjectProperty> g_lstWorldObjectProperties = new List<WorldObjectProperty>()
	{
		new WorldObjectProperty("db_apart_08_", new RAGE.Vector3(25.66371f, 6537.821f, 30.2f), new RAGE.Vector3(0.0f, 0.0f, 135.0f)),
		new WorldObjectProperty("db_apart_08_", new RAGE.Vector3(42.42059f, 6554.73f, 30.2f), new RAGE.Vector3(0.0f, 0.0f, 135.0f)),
		new WorldObjectProperty("db_apart_08_", new RAGE.Vector3(59.06924f, 6572.19f, 30.2f), new RAGE.Vector3(0.0f, 0.0f, 135.0f)),
		new WorldObjectProperty("db_apart_08_", new RAGE.Vector3(82.30423f, 6579.691f, 30.2f), new RAGE.Vector3(0.0f, 0.0f, 135.0f - 90.0f)),
	};


	private void CreateWorldObjects()
	{
		foreach (WorldObjectProperty worldObjProp in g_lstWorldObjectProperties)
		{
			worldObjProp.Create();
		}
	}

	public void SetDownpayment(float fDownpayment)
	{
		m_fDownpaymentAmount = fDownpayment;
		UpdatePropertyInfo(true);
	}

	public void SetNumMonthlyPayments(int numMonthlyPayments)
	{
		m_iNumberMonthlyPayments = numMonthlyPayments;
		UpdatePropertyInfo(true);
	}

	private void UpdatePropertyInfo(bool bPlayerActionDriven)
	{
		if (m_PurchaseEntity != null)
		{
			float fPropertyBuyPrice = DataHelper.GetEntityData<float>(m_PurchaseEntity, EDataNames.PROP_BUY_PRICE);

			float fRemainingPrice = fPropertyBuyPrice - m_fDownpaymentAmount;
			float fInterest = fRemainingPrice * Taxation.GetPaymentPlanInterestPercent();
			float fMonthlyPayment = (fRemainingPrice + fInterest) / m_iNumberMonthlyPayments;

			if (!bPlayerActionDriven) // we dont update text box if user changed it... otherwise their typing caret gets screwed
			{
				float fDownpaymentDefault = m_fDownpaymentAmount * fPropertyBuyPrice;
				float fMin = fDownpaymentDefault;
				float fMax = fPropertyBuyPrice;
				m_PurchasePropertyGUI.SetDownpayment(m_fDownpaymentAmount, fMin, fMax);
			}

			m_PurchasePropertyGUI.SetPriceInfo(fPropertyBuyPrice, fInterest, fRemainingPrice, fRemainingPrice + fInterest, fMonthlyPayment);
		}
	}

	private void ShowLoadingInteriorMessage(bool bShow)
	{
		m_bShowLoadingInteriorMessage = bShow;

		// TODO_FIX: Fix and re-enable at some point?
		/*
		if (m_bShowLoadingInteriorMessage)
		{
			RAGE.Game.Graphics.TransitionToBlurred(100);
		}
		else
		{
			RAGE.Game.Graphics.TransitionFromBlurred(100);
		}
		*/
	}

	private void OnEnterInteriorApproved(float x, float y, float z, int mapID, bool bIsCharacterSelect)
	{
		if (!bIsCharacterSelect)
		{
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		}

		if (mapID != -1)
		{
			ShowLoadingInteriorMessage(true);

			MapLoader.LoadMap(mapID, (bool bSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) =>
			{
				if (bSuccess)
				{
					ShowLoadingInteriorMessage(false);
					RAGE.Elements.Player.LocalPlayer.Position = bCustomMap ? new RAGE.Vector3(markerX, markerY, markerZ) : new RAGE.Vector3(x, y, z);
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
				else
				{
					// teleport back
					NetworkEventSender.SendNetworkEvent_RequestExitInteriorForced();
					ChatHelper.ErrorMessage("You were removed from the interior due to the map not loading successfully.");
				}
			});
		}
		else
		{
			if (!bIsCharacterSelect)
			{
				PlayerHelper.OnSafeTeleport(x, y, z);
				ShowLoadingInteriorMessage(false);

				// TODO_FIX: Fix and re-enable at some point?
				/*
				RAGE.Game.Graphics.TransitionToBlurred(100);
				ClientTimerPool.CreateTimer((object[] parameters) =>
				{
					RAGE.Game.Graphics.TransitionFromBlurred(100);
				}, 300, 1);
				*/
			}
		}
	}

	private void OnEnterElevatorApproved(float x, float y, float z, int mapID, bool bIsCharacterSelect)
	{
		if (!bIsCharacterSelect)
		{
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);
		}

		if (mapID != -1)
		{
			MapLoader.LoadMap(mapID, (bool bSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) =>
			{
				if (bSuccess)
				{
					RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
				else
				{
					NetworkEventSender.SendNetworkEvent_RequestExitInteriorForced();
					ChatHelper.ErrorMessage("You were removed from the interior due to the map not loading successfully.");
				}
			});
		}
		else
		{
			if (!bIsCharacterSelect)
			{
				PlayerHelper.OnSafeTeleport(x, y, z);
			}
		}
	}

	private void OnExitElevatorApproved(float x, float y, float z, int mapID, bool hasParentInterior, int parentMapID)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		if (mapID != -1 && mapID != parentMapID)
		{

			MapLoader.UnloadMap(mapID, (bool bSuccess) =>
			{
				if (bSuccess && !hasParentInterior)
				{
					RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
			});
		}

		if (hasParentInterior)
		{
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

			if (parentMapID != -1)
			{
				ShowLoadingInteriorMessage(true);
				MapLoader.LoadMap(parentMapID, (bool bParentSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) =>
				{
					if (bParentSuccess)
					{
						ShowLoadingInteriorMessage(false);
						RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
						RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
					}
					else
					{
						ShowLoadingInteriorMessage(false);
						ChatHelper.ErrorMessage("Something gone wrong while loading the interior, please report it on the bugs tracker.");
					}
				});
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
				RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			}
		}
		else
		{
			PlayerHelper.OnSafeTeleport(x, y, z);
		}
	}

	private void OnExitInteriorApproved(float x, float y, float z, int mapID, bool hasParentInterior, int parentMapID)
	{
		RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

		if (mapID != -1)
		{
			MapLoader.UnloadMap(mapID, (bool bSuccess) =>
			{
				if (bSuccess && !hasParentInterior)
				{
					RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
					RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
				}
			});
		}

		if (hasParentInterior)
		{
			RAGE.Elements.Player.LocalPlayer.FreezePosition(true);

			if (parentMapID != -1)
			{
				ShowLoadingInteriorMessage(true);

				MapLoader.LoadMap(parentMapID, (bool bParentSuccess, bool bCustomMap, float markerX, float markerY, float markerZ) =>
				{
					if (bParentSuccess)
					{
						ShowLoadingInteriorMessage(false);
						RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
						RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
					}
					else
					{
						ShowLoadingInteriorMessage(false);
						ChatHelper.ErrorMessage("Something gone wrong while loading the interior, please report it on the bugs tracker.");
					}
				});
			}
			else
			{
				RAGE.Elements.Player.LocalPlayer.Position = new RAGE.Vector3(x, y, z);
				RAGE.Elements.Player.LocalPlayer.FreezePosition(false);
			}
		}
		else if (mapID == -1)
		{
			PlayerHelper.OnSafeTeleport(x, y, z);
		}
	}

	private void OnCheckout(int purchaserIndex, EPaymentMethod method)
	{
		if (purchaserIndex < 0 || method == EPaymentMethod.None)
		{
			NotificationManager.ShowNotification("Realtor", "You must pick a valid purchaser & payment method.", ENotificationIcon.ExclamationSign);
		}
		else
		{
			Purchaser currentPurchaser = m_lstPurchasers[purchaserIndex];

			// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
			Int64 propertyID = DataHelper.GetEntityData<Int64>(m_PurchaseEntity, EDataNames.PROP_ID);
			NetworkEventSender.SendNetworkEvent_PurchaseProperty_OnCheckout(propertyID, currentPurchaser.Type, currentPurchaser.ID, method, m_fDownpaymentAmount, m_iNumberMonthlyPayments);

			HidePurchasePropertyUI();
		}
	}

	private void OnRequestInfoResponse(List<Purchaser> lstPurchasers, List<string> lstMethods)
	{
		m_lstPurchasers = lstPurchasers;
		m_lstMethods = lstMethods;

		bool bAddedSpacer = false;

		foreach (Purchaser purchaser in lstPurchasers)
		{
			if (purchaser.Type == EPurchaserType.Faction && !bAddedSpacer)
			{
				bAddedSpacer = true;
				m_PurchasePropertyGUI.AddDivider();
			}

			m_PurchasePropertyGUI.AddPurchaser(purchaser.DisplayName);
		}

		foreach (string strPaymentMethod in lstMethods)
		{
			m_PurchasePropertyGUI.AddMethod(strPaymentMethod);
		}

		m_PurchasePropertyGUI.CommitPurchasesAndMethods();
	}

	private void OnPreview()
	{
		if (m_PurchaseEntity != null)
		{
			// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
			Int64 propertyID = DataHelper.GetEntityData<Int64>(m_PurchaseEntity, EDataNames.PROP_ID);
			NetworkEventSender.SendNetworkEvent_PurchaseProperty_OnPreview(propertyID);

			HidePurchasePropertyUI();
		}
	}

	private void OnUILoaded()
	{

	}

	private void HidePurchasePropertyUI()
	{
		if (m_PurchasePropertyGUI != null)
		{
			m_PurchasePropertyGUI.SetVisible(false, false, false);

			m_PurchasePropertyGUI.Reload();

			m_PurchaseEntity = null;
		}
	}

	private void OnChangeCharacterApproved()
	{
		HidePurchasePropertyUI();
	}

	private RAGE.Elements.Marker GetNearestProperty()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.PropertyMarker);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private RAGE.Elements.Marker GetNearestElevator()
	{
		PoolEntry poolEntry = OptimizationCachePool.GetPoolItem(EPoolCacheKey.ElevatorMarker);
		return poolEntry != null ? poolEntry.GetEntity<RAGE.Elements.Marker>() : null;
	}

	private bool CanShowPurchasePropertyUI()
	{
		if (m_PurchasePropertyGUI == null)
		{
			return false;
		}

		return !m_PurchasePropertyGUI.IsVisible();
	}

	private int numClicks = 0;

	private void OnRender()
	{
		RAGE.Elements.Marker nearestPropertyMarker = GetNearestProperty();
		RAGE.Elements.Marker nearestElevatorMarker = GetNearestElevator();
		if (CanShowPurchasePropertyUI())
		{
			if (nearestPropertyMarker != null)
			{
				bool isEntrance = DataHelper.GetEntityData<bool>(nearestPropertyMarker, EDataNames.PROP_ENTER);
				RAGE.Vector3 vecMarkerPos = nearestPropertyMarker.Position;

				if (isEntrance)
				{
					// TODO_POST_LAUNCH: Make use of world hints for this, the 3d to 2d translation meant it wasn't in line with the 2d text
					vecMarkerPos.Z += 0.5f;
					WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "", OnRenderPropertyWorldHint, InteractWithProperty, vecMarkerPos, nearestPropertyMarker.Dimension, false, false, g_fDistThreshold, bCallerWillDraw: true);
				}
				else
				{
					vecMarkerPos.Z += 1.0f;
					WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Exit", OnRenderPropertyWorldHint, InteractWithProperty, vecMarkerPos, nearestPropertyMarker.Dimension, false, false, g_fDistThreshold, bCallerWillDraw: false);
				}
			}
		}

		if (nearestElevatorMarker != null)
		{
			bool isEntrance = DataHelper.GetEntityData<bool>(nearestElevatorMarker, EDataNames.ELEVATOR_ENTRANCE);
			bool isVehicleElevator = DataHelper.GetEntityData<bool>(nearestElevatorMarker, EDataNames.ELEVATOR_VEHICLE);
			RAGE.Vector3 vecMarkerPos = nearestElevatorMarker.Position;

			if (isEntrance)
			{
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "", OnRenderElevatorWorldHint, InteractWithElevator, vecMarkerPos, nearestElevatorMarker.Dimension, false, false, g_fDistThreshold, bAllowInVehicle: isVehicleElevator, bCallerWillDraw: true);
			}
			else
			{
				WorldHintManager.DrawExclusiveWorldHint(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact), "Exit", OnRenderElevatorWorldHint, InteractWithElevator, vecMarkerPos, nearestElevatorMarker.Dimension, false, false, g_fDistThreshold, bAllowInVehicle: isVehicleElevator, bCallerWillDraw: false);
			}
		}

		if (m_bShowLoadingInteriorMessage)
		{
			HUD.SetLoadingMessage("Loading Interior...");
		}

		if (MowingProperty != null)
		{
			Int64 now = Helpers.GetUnixTimestamp();
			if (now - LastMowingIncrement > MOWING_INCREASE_INCREMENT_SECONDS)
			{
				LastMowingIncrement = now;

				if (PlayerHelper.GetLocalPlayerPosition().DistanceTo(MowingProperty.Position) > MAX_MOWER_DISTANCE)
				{
					AwayFromMowerCounter++;
					NetworkEvents.SendLocalEvent_SetProgressBar("TOO FAR FROM PROPERTY", "0%");

					if (AwayFromMowerCounter > 12)
					{
						NotificationManager.ShowNotification("Mower State Reset", "Re-enter the mower near a property you own to start mowing your lawn.", ENotificationIcon.ExclamationSign);
						ResetMowerState();	
					}
					return;
				}

				AwayFromMowerCounter = 0;
				if (PlayerHelper.GetLocalPlayerVehicleMilesPerHour() < 10)
				{
					return;
				}

				MowingProgress += MOWING_INCREMENT_AMOUNT;
				NetworkEvents.SendLocalEvent_SetProgressBar(
					Helpers.FormatString("{0}% Mowed", MowingProgress), 
					Helpers.FormatString("{0}%", MowingProgress)
				);

				if (MowingProgress >= 100)
				{
					long propertyId = DataHelper.GetEntityData<long>(MowingProperty, EDataNames.PROP_ID);
					NetworkEventSender.SendNetworkEvent_Property_MowedLawn(propertyId);
					ResetMowerState();
					NotificationManager.ShowNotification(
						"Mowing Complete!",
						Helpers.FormatString(
							"You mowed your lawn! You have earned {0} XP for your property {1}",
							TOTAL_MOWING_XP,
							DataHelper.GetEntityData<string>(MowingProperty, EDataNames.PROP_NAME)
						),
						ENotificationIcon.Star
					);
				}
			}
		}
	}

	private void OnRenderPropertyWorldHint()
	{
		RAGE.Elements.Marker nearestPropertyMarker = GetNearestProperty();
		if (CanShowPurchasePropertyUI())
		{
			if (nearestPropertyMarker != null)
			{
				const float fScale = 0.6f;
				const float fScaleNormalFont = 0.3f;

				bool isEntrance = DataHelper.GetEntityData<bool>(nearestPropertyMarker, EDataNames.PROP_ENTER);
				RAGE.Vector3 vecMarkerPos = nearestPropertyMarker.Position;

				if (isEntrance)
				{
					string strPropertyName = DataHelper.GetEntityData<string>(nearestPropertyMarker, EDataNames.PROP_NAME);
					EPropertyState propertyState = DataHelper.GetEntityData<EPropertyState>(nearestPropertyMarker, EDataNames.PROP_STATE);
					string strOwnerText = DataHelper.GetEntityData<string>(nearestPropertyMarker, EDataNames.PROP_OWNER_TEXT);

					RAGE.Vector3 vecGameplayCamPos = RAGE.Elements.Player.LocalPlayer.Position;
					float fDistance = WorldHelper.GetDistance(vecGameplayCamPos, vecMarkerPos);

					float fDistanceRatio = 1.0f - (fDistance / g_fDistThreshold);
					int iMainTextAlpha = (int)(fDistanceRatio * 255.0f);

					if (iMainTextAlpha < 0)
					{
						return;
					}

					vecMarkerPos.Z += 1.2f;

					Vector2 vecScreen = GraphicsHelper.GetScreenPositionFromWorldPosition(vecMarkerPos);

					if (vecScreen == null)
					{
						return;
					}

					// BEGIN SPRITE HANDLING
					if (strOwnerText != null && strOwnerText.ToLower().Contains("state patrol"))
					{
						var dict = "shared";
						if (!RAGE.Game.Graphics.HasStreamedTextureDictLoaded(dict))
						{
							RAGE.Game.Graphics.RequestStreamedTextureDict(dict, true);
						}

						if (RAGE.Game.Graphics.HasStreamedTextureDictLoaded(dict))
						{
							// calc scale:
							int screenX = 0;
							int screenY = 0;
							RAGE.Game.Graphics.GetActiveScreenResolution(ref screenX, ref screenY);

							float width = 0.03f;
							float height = 0.05f;
							float scaleX = (32.0f / screenX);
							float scaleY = (32.0f / screenY);

							RAGE.Game.Graphics.DrawSprite(dict, "newstar_32", vecScreen.X - (scaleX * 2), vecScreen.Y - 0.025f, width, height, 0.0f, 255, 255, 255, iMainTextAlpha, 0);
							RAGE.Game.Graphics.DrawSprite(dict, "newstar_32", vecScreen.X - (scaleX), vecScreen.Y - 0.025f, width, height, 0.0f, 255, 255, 255, iMainTextAlpha, 0);
							RAGE.Game.Graphics.DrawSprite(dict, "newstar_32", vecScreen.X, vecScreen.Y - 0.025f, width, height, 0.0f, 255, 255, 255, iMainTextAlpha, 0);
							RAGE.Game.Graphics.DrawSprite(dict, "newstar_32", vecScreen.X + (scaleX), vecScreen.Y - 0.025f, width, height, 0.0f, 255, 255, 255, iMainTextAlpha, 0);
							RAGE.Game.Graphics.DrawSprite(dict, "newstar_32", vecScreen.X + (scaleX * 2), vecScreen.Y - 0.025f, width, height, 0.0f, 255, 255, 255, iMainTextAlpha, 0);
						}
					}
					// END SPRITE HANDLING
					const float fOffsetDelta = 0.05f;

					int XP = DataHelper.GetEntityData<int>(nearestPropertyMarker, EDataNames.PROP_XP);
					TextHelper.Draw2D(Helpers.FormatString("{0} XP", XP), vecScreen.X, vecScreen.Y - 0.03f, 0.35f, new RAGE.RGBA(102, 255, 51, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);

					TextHelper.Draw2D(strPropertyName, vecScreen.X, vecScreen.Y, fScale, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);

					float fOffset = fOffsetDelta;

					if (propertyState == EPropertyState.Rented || propertyState == EPropertyState.Owned || propertyState == EPropertyState.Rented_AlwaysEnterable || propertyState == EPropertyState.Owned_AlwaysEnterable)
					{
						EAdminLevel adminLevel = DataHelper.GetLocalPlayerEntityData<EAdminLevel>(EDataNames.ADMIN_LEVEL);
						bool onDuty = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.ADMIN_DUTY);
						if (adminLevel != EAdminLevel.None && onDuty)
						{
							TextHelper.Draw2D(Helpers.FormatString("Owner: {0}", strOwnerText), vecScreen.X, vecScreen.Y + fOffset, fScaleNormalFont, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
							fOffset += fOffsetDelta;
						}
					}

					// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
					Int64 propertyID = DataHelper.GetEntityData<Int64>(nearestPropertyMarker, EDataNames.PROP_ID);
					TextHelper.Draw2D(Helpers.FormatString("Zip: {0}", propertyID), vecScreen.X, vecScreen.Y + fOffset, fScaleNormalFont, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
					fOffset += fOffsetDelta;


					// ENTRANCE TEXT
					EPropertyEntranceType propertyEntranceType = DataHelper.GetEntityData<EPropertyEntranceType>(nearestPropertyMarker, EDataNames.PROP_ENT_TYPE);

					const float fOffsetX_Large = 0.05f;
					const float fOffsetX_Small = 0.015f;
					float fOffsetX = 0.05f;
					float fOffsetXAlwaysSmall = fOffsetX_Small;
					float fOffsetSecondOption = 0.0f;

					string strMessage = "";
					string strMessageHeld = "";
					if (propertyState == EPropertyState.AvailableToBuy)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Purchase Business" : "Purchase";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;
					}
					else if (propertyState == EPropertyState.AvailableToRent)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Rent Business" : "Rent";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;
					}
					else if (propertyState == EPropertyState.Owned || propertyState == EPropertyState.Owned_AlwaysEnterable)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Walk in to Enter!" : "Enter";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;

						fOffsetSecondOption += 0.03f;

#if ENABLE_MAILBOX
						strMessageHeld = "Access Mailbox";
#endif
					}
					else if (propertyState == EPropertyState.Rented || propertyState == EPropertyState.Rented_AlwaysEnterable)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Walk in to Enter!" : "Enter";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;

						fOffsetSecondOption += 0.03f;
#if ENABLE_MAILBOX
						strMessageHeld = "Access Mailbox";
#endif
					}
					else if (propertyState == EPropertyState.AvailableToBuy_AlwaysEnterable)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Walk in to Enter!" : "Enter";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;

						strMessageHeld = propertyEntranceType == EPropertyEntranceType.World ? "Purchase Business" : "Purchase";
					}
					else if (propertyState == EPropertyState.AvailableToRent_AlwaysEnterable)
					{
						strMessage = propertyEntranceType == EPropertyEntranceType.World ? "Walk in to Enter!" : "Enter";
						fOffsetX = propertyEntranceType == EPropertyEntranceType.World ? fOffsetX_Large : fOffsetX_Small;

						strMessageHeld = propertyEntranceType == EPropertyEntranceType.World ? "Rent Business" : "Rent";
					}

					// Only show E if not world interior, or world interior (not always enterable) and purchasable
					if (propertyEntranceType == EPropertyEntranceType.Normal || (propertyEntranceType == EPropertyEntranceType.World && (propertyState == EPropertyState.AvailableToBuy || propertyState == EPropertyState.AvailableToRent)))
					{
						RAGE.Game.Graphics.DrawRect(vecScreen.X - fOffsetX, vecScreen.Y + fOffset + 0.017f, 0.015f, 0.03f, 0, 0, 0, iMainTextAlpha, 0);

						TextHelper.Draw2D(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact).ToString(), vecScreen.X - fOffsetX, vecScreen.Y + fOffset, 0.5f, new RAGE.RGBA(255, 194, 15, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);
					}

					RAGE.Game.Ui.BeginTextCommandWidth("STRING");
					RAGE.Game.Ui.AddTextComponentSubstringPlayerName("A");
					float fTextWidthSingleChar = RAGE.Game.Ui.EndTextCommandGetWidth((int)RAGE.Game.Font.ChaletComprimeCologne);

					float fTextLeft = (vecScreen.X - fOffsetX) + 0.010f + ((fTextWidthSingleChar * strMessage.Length) / 10.0f);
					TextHelper.Draw2D(strMessage, fTextLeft, vecScreen.Y + fOffset, 0.5f, 209, 209, 209, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);

					// Show a prompt if its a world interior offering to purchase in the 'default action text'
					if (propertyEntranceType == EPropertyEntranceType.World && ((propertyState == EPropertyState.AvailableToBuy || propertyState == EPropertyState.AvailableToRent || propertyState == EPropertyState.AvailableToBuy_AlwaysEnterable || propertyState == EPropertyState.AvailableToRent_AlwaysEnterable)))
					{
						fOffset += fOffsetDelta;
						TextHelper.Draw2D("Walk in to Enter!", vecScreen.X, vecScreen.Y + fOffset, fScaleNormalFont, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
					}

					// render held if available
					if (strMessageHeld.Length > 0)
					{
						fOffset += fOffsetDelta;
						string strKeyMessage = Helpers.FormatString("Double Tap {0}", ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact).ToString());

						float fRectLeft = vecScreen.X + fOffsetXAlwaysSmall + fOffsetXAlwaysSmall - 0.03f - ((fTextWidthSingleChar * strKeyMessage.Length) / 10.0f);
						float fRectWidth = Math.Max(0.012f, 0.012f * (strKeyMessage.Length / 2.0f));
						float fTextLeftInner = vecScreen.X + fOffsetXAlwaysSmall + fOffsetXAlwaysSmall - 0.0309f - ((fTextWidthSingleChar * strKeyMessage.Length) / 10.0f);
						RAGE.Game.Graphics.DrawRect(fRectLeft, vecScreen.Y + fOffset + 0.017f, fRectWidth, 0.03f, 0, 0, 0, iMainTextAlpha, 0);

						TextHelper.Draw2D(strKeyMessage, fTextLeftInner, vecScreen.Y + fOffset, 0.5f, new RAGE.RGBA(255, 194, 15, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);
						TextHelper.Draw2D(strMessageHeld, vecScreen.X + fOffsetX + fOffsetSecondOption, vecScreen.Y + fOffset, 0.5f, 209, 209, 209, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
					}
				}
			}
		}
	}

	private void OnRenderElevatorWorldHint()
	{
		RAGE.Elements.Marker nearestElevatorMarker = GetNearestElevator();
		if (nearestElevatorMarker != null)
		{
			const float fScale = 0.6f;
			const float fScaleNormalFont = 0.3f;

			bool isEntrance = DataHelper.GetEntityData<bool>(nearestElevatorMarker, EDataNames.ELEVATOR_ENTRANCE);
			RAGE.Vector3 vecMarkerPos = nearestElevatorMarker.Position;

			if (isEntrance)
			{
				string strElevatorName = DataHelper.GetEntityData<string>(nearestElevatorMarker, EDataNames.ELEVATOR_NAME);

				RAGE.Vector3 vecGameplayCamPos = RAGE.Elements.Player.LocalPlayer.Position;
				float fDistance = WorldHelper.GetDistance(vecGameplayCamPos, vecMarkerPos);

				float fDistanceRatio = 1.0f - (fDistance / g_fDistThreshold);
				int iMainTextAlpha = (int)(fDistanceRatio * 255.0f);

				if (iMainTextAlpha < 0)
				{
					return;
				}

				vecMarkerPos.Z += 1.2f;

				Vector2 vecScreen = GraphicsHelper.GetScreenPositionFromWorldPosition(vecMarkerPos);

				if (vecScreen == null)
				{
					return;
				}

				TextHelper.Draw2D(strElevatorName, vecScreen.X, vecScreen.Y, fScale, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);

				const float fOffsetDelta = 0.05f;
				float fOffset = fOffsetDelta;

				// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
				Int64 elevatorID = DataHelper.GetEntityData<Int64>(nearestElevatorMarker, EDataNames.ELEVATOR_ID);
				TextHelper.Draw2D(Helpers.FormatString("Elevator ID: {0}", elevatorID), vecScreen.X, vecScreen.Y + fOffset, fScaleNormalFont, new RAGE.RGBA(209, 209, 209, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
				fOffset += fOffsetDelta;

				const float fOffsetX_Small = 0.015f;
				float fOffsetX = 0.05f;
				float fOffsetXAlwaysSmall = fOffsetX_Small;

				string strMessage = "Use";
				string strMessageHeld = "";
				fOffsetX = fOffsetX_Small;

				RAGE.Game.Graphics.DrawRect(vecScreen.X - fOffsetX, vecScreen.Y + fOffset + 0.017f, 0.015f, 0.03f, 0, 0, 0, iMainTextAlpha, 0);
				TextHelper.Draw2D(ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact).ToString(), vecScreen.X - fOffsetX, vecScreen.Y + fOffset, 0.5f, new RAGE.RGBA(255, 194, 15, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);

				RAGE.Game.Ui.BeginTextCommandWidth("STRING");
				RAGE.Game.Ui.AddTextComponentSubstringPlayerName("A");
				float fTextWidthSingleChar = RAGE.Game.Ui.EndTextCommandGetWidth((int)RAGE.Game.Font.ChaletComprimeCologne);

				float fTextLeft = (vecScreen.X - fOffsetX) + 0.010f + ((fTextWidthSingleChar * strMessage.Length) / 10.0f);
				TextHelper.Draw2D(strMessage, fTextLeft, vecScreen.Y + fOffset, 0.5f, 209, 209, 209, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);

				// render held if available
				if (strMessageHeld.Length > 0)
				{
					fOffset += fOffsetDelta;
					string strKeyMessage = Helpers.FormatString("Double Tap {0}", ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact).ToString());

					float fRectLeft = vecScreen.X + fOffsetXAlwaysSmall + fOffsetXAlwaysSmall - 0.03f - ((fTextWidthSingleChar * strKeyMessage.Length) / 10.0f);
					float fRectWidth = Math.Max(0.012f, 0.012f * (strKeyMessage.Length / 2.0f));
					float fTextLeftInner = vecScreen.X + fOffsetXAlwaysSmall + fOffsetXAlwaysSmall - 0.0309f - ((fTextWidthSingleChar * strKeyMessage.Length) / 10.0f);
					RAGE.Game.Graphics.DrawRect(fRectLeft, vecScreen.Y + fOffset + 0.017f, fRectWidth, 0.03f, 0, 0, 0, iMainTextAlpha, 0);

					TextHelper.Draw2D(strKeyMessage, fTextLeftInner, vecScreen.Y + fOffset, 0.5f, new RAGE.RGBA(255, 194, 15, (uint)iMainTextAlpha), RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, false);
					TextHelper.Draw2D(strMessageHeld, vecScreen.X + fOffsetX, vecScreen.Y + fOffset, 0.5f, 209, 209, 209, iMainTextAlpha, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
				}
			}
		}

	}

	private void ShowPurchasePropertyUI(RAGE.Elements.Marker nearestPropertyMarker)
	{
		m_PurchaseEntity = nearestPropertyMarker;

		m_lstPurchasers.Clear();
		m_lstMethods.Clear();

		m_PurchasePropertyGUI.SetVisible(true, true, false);
		m_PurchasePropertyGUI.ResetData();
		UpdatePropertyInfo(false);

		NetworkEventSender.SendNetworkEvent_GetPurchaserAndPaymentMethods(EPurchaseAndPaymentMethodsRequestType.Property);
	}

	private void RequestMailbox(RAGE.Elements.Marker nearestPropertyMarker)
	{
#if ENABLE_MAILBOX
		Int64 propertyID = DataHelper.GetEntityData<Int64>(nearestPropertyMarker, EDataNames.PROP_ID);
		NetworkEventSender.SendNetworkEvent_RequestMailbox(propertyID);
#endif
	}

	private void InteractWithProperty()
	{
		RAGE.Elements.Marker nearestPropertyMarker = GetNearestProperty();
		if (nearestPropertyMarker != null && KeyBinds.CanProcessKeybinds())
		{
			++numClicks;

			if (numClicks >= 2)
			{
				EPropertyState propertyState = DataHelper.GetEntityData<EPropertyState>(nearestPropertyMarker, EDataNames.PROP_STATE);
				if (propertyState == EPropertyState.Owned || propertyState == EPropertyState.Owned_AlwaysEnterable || propertyState == EPropertyState.Rented || propertyState == EPropertyState.Rented_AlwaysEnterable) // mailbox
				{
					RequestMailbox(nearestPropertyMarker);
				}
				else
				{
					ShowPurchasePropertyUI(nearestPropertyMarker);
				}

				numClicks = 0;
			}
			else
			{
				ClientTimerPool.CreateTimer(InterestWithProperty_Delayed, 200, 1);
			}

			/*
			// If it's not a 'enter anywhere' interior, just go straight to normal interact, they can't double click it
			
			if (propertyState != EPropertyState.AvailableToBuy_AlwaysEnterable && propertyState != EPropertyState.AvailableToRent_AlwaysEnterable)
			{
				numClicks = 1;
				InterestWithProperty_Delayed(null);
			}
			else
			{
				
			}
			*/
		}
	}

	private void InteractWithElevator()
	{
		RAGE.Elements.Marker nearestElevatorMarker = GetNearestElevator();
		if (nearestElevatorMarker != null && KeyBinds.CanProcessKeybinds())
		{
			Int64 elevatorID = DataHelper.GetEntityData<Int64>(nearestElevatorMarker, EDataNames.ELEVATOR_ID);
			bool isEntrance = DataHelper.GetEntityData<bool>(nearestElevatorMarker, EDataNames.ELEVATOR_ENTRANCE);
			if (isEntrance)
			{
				NetworkEventSender.SendNetworkEvent_RequestEnterElevator(elevatorID);
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_RequestExitElevator(elevatorID);
			}
		}
	}

	private void InterestWithProperty_Delayed(object[] parameters)
	{
		if (numClicks == 1)
		{
			numClicks = 0;
			RAGE.Elements.Marker nearestPropertyMarker = GetNearestProperty();
			if (nearestPropertyMarker != null && KeyBinds.CanProcessKeybinds())
			{
				bool isEntrance = DataHelper.GetEntityData<bool>(nearestPropertyMarker, EDataNames.PROP_ENTER);

				if (isEntrance)
				{
					EPropertyState propertyState = DataHelper.GetEntityData<EPropertyState>(nearestPropertyMarker, EDataNames.PROP_STATE);

					if (propertyState == EPropertyState.AvailableToBuy)
					{
						ShowPurchasePropertyUI(nearestPropertyMarker);
					}
					else if (propertyState == EPropertyState.AvailableToRent)
					{
						// TODO_POST_LAUNCH: Implement renting
						RAGE.Chat.Output("0 Not implemented");
					}
					else
					{
						EPropertyEntranceType propertyEntranceType = DataHelper.GetEntityData<EPropertyEntranceType>(nearestPropertyMarker, EDataNames.PROP_ENT_TYPE);
						if (propertyEntranceType == EPropertyEntranceType.World)
						{
							NotificationManager.ShowNotification("World Property", "This is a world property. Just walk in to enter!", ENotificationIcon.InfoSign);
						}
						else
						{
							// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
							Int64 propertyID = DataHelper.GetEntityData<Int64>(nearestPropertyMarker, EDataNames.PROP_ID);
							NetworkEventSender.SendNetworkEvent_RequestEnterInterior(propertyID);
						}
					}
				}
				else
				{
					// TODO_CSHARP: Int64 data isnt supported in entity data manager in RAGE... so we actually lose data here
					Int64 propertyID = DataHelper.GetEntityData<Int64>(nearestPropertyMarker, EDataNames.PROP_ID);
					NetworkEventSender.SendNetworkEvent_RequestExitInterior();
				}
			}
		}
	}

	private CGUIPurchaseProperty m_PurchasePropertyGUI = null;
	private const float g_fDistThreshold = 3.0f;
	private RAGE.Elements.Marker m_PurchaseEntity = null;
	private List<Purchaser> m_lstPurchasers = new List<Purchaser>();
	private List<string> m_lstMethods = new List<string>();
	private bool m_bShowLoadingInteriorMessage = false;

	// DOWNPAYMENTS
	private float m_fDownpaymentAmount = 0.0f;
	private int m_iNumberMonthlyPayments = 60;
}