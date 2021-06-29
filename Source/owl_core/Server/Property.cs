//#define PROPERTY_ARROW_INSIDE_MARKER
using Database.Models;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

public class CPropertyInstance : CBaseEntity
{
	public CPropertyInstance(Property property)
	{
		Model = property;

		m_Mailbox = new CPropertyMailbox(this);

		foreach (CDatabaseStructureFurnitureItem furnItem in property.FurnitureItems)
		{
			CPropertyFurnitureInstance furnitureInst = new CPropertyFurnitureInstance(furnItem.dbID, property.Id, furnItem.furnitureID, furnItem.vecPos, new Vector3(0.0f, 0.0f, furnItem.fRot), furnItem.droppedByID, furnItem.itemValue);
			m_lstFurniture.Add(furnitureInst);
			NetworkEvents.SendLocalEvent_OnPropertyFurnitureInstanceCreated(furnitureInst);
		}

		foreach (FurnitureRemoval removal in property.FurnitureRemovals)
		{
			m_lstDefaultFurnitureRemovals.Add(new CPropertyDefaultFurnitureRemovalInstance(removal.Id, property.Id, removal.Model, removal.Position, removal.DroppedById));
		}

		CreateMarkers();
	}

	~CPropertyInstance()
	{
		NAPI.Task.Run(() =>
		{
			Destroy(false);
		});
	}

	public int GetMapID()
	{
		int mapId = MapLoader.GetMapIDByInteriorID((int)Model.Id);
		if (mapId != -1)
		{
			return mapId;
		}

		CInteriorDefinition intDef = InteriorDefinitions.GetInteriorDefinition(Model.InteriorId);
		return intDef == null ? mapId : intDef.MapFileName.Length > 0 ? MapLoader.GetMapID(intDef.MapFileName) : -1;
	}

	public void AddDefaultFurnitureRemoval(uint model, Vector3 vecPos, CPlayer a_Player)
	{
		FurnitureRemoval.Create(model, vecPos, a_Player.ActiveCharacterDatabaseID, Model.Id, removal =>
		{
			Model.AddFurnitureRemoval(removal);
			m_lstDefaultFurnitureRemovals.Add(new CPropertyDefaultFurnitureRemovalInstance(removal.Id, removal.PropertyId, removal.Model, removal.Position, removal.DroppedById));

			// transmit furniture + removals to every player in this property
			// TODO_FURNITURE: Only transmit the diff?
			TransmitFurnitureToAllPlayersInside();
		});
	}

	public async void AddNewFurnitureItem(CItemInstanceDef itemDef, uint furnID, Vector3 vecPos, Vector3 vecRot, CPlayer a_PlayerPlacingItem)
	{
		EntityDatabaseID dbid = await Database.LegacyFunctions.CreateFurnitureItemInProperty(itemDef, furnID, vecPos, vecRot, a_PlayerPlacingItem.ActiveCharacterDatabaseID, Model.Id).ConfigureAwait(true);
		CPropertyFurnitureInstance furnitureInst = new CPropertyFurnitureInstance(dbid, Model.Id, furnID, vecPos, vecRot, a_PlayerPlacingItem.ActiveCharacterDatabaseID, itemDef.GetValueDataSerialized());
		m_lstFurniture.Add(furnitureInst);
		NetworkEvents.SendLocalEvent_OnPropertyFurnitureInstanceCreated(furnitureInst);

		a_PlayerPlacingItem.Inventory.RemoveItem(itemDef);

		// transmit furniture to every player in this property
		TransmitFurnitureToAllPlayersInside();
	}

	public void RestoreDefaultFurnitureRemoval(EntityDatabaseID dbid, CPlayer a_PlayerPlacingItem)
	{
		Database.Functions.Items.DeleteDefaultFurnitureRemovalFromProperty(dbid, Model.Id, null);

		// find instance
		CPropertyDefaultFurnitureRemovalInstance removalInst = null;
		foreach (CPropertyDefaultFurnitureRemovalInstance removalIter in m_lstDefaultFurnitureRemovals)
		{
			if (removalIter.DBID == dbid)
			{
				removalInst = removalIter;
				break;
			}
		}

		if (removalInst != null)
		{
			m_lstDefaultFurnitureRemovals.Remove(removalInst);
		}

		// transmit furniture to every player in this property
		TransmitFurnitureToAllPlayersInside();
	}

	public void RemoveCurrentFurnitureItem(EntityDatabaseID dbid, CPlayer a_PlayerPlacingItem, Action<bool> CompletionCallback)
	{
		// Does it have items inside?
		Database.Functions.Items.GetInventoryForFurnitureItemRecursive(dbid, (List<CItemInstanceDef> lstItemsInsideFurniture) =>
		{
			if (lstItemsInsideFurniture.Count == 0)
			{
				Database.Functions.Items.DeleteFurnitureItemFromProperty(dbid, Model.Id);

				// find instance
				CPropertyFurnitureInstance furnInst = null;
				foreach (CPropertyFurnitureInstance furnIter in m_lstFurniture)
				{
					if (furnIter.DBID == dbid)
					{
						furnInst = furnIter;
						break;
					}
				}

				if (furnInst != null)
				{
					// give item
					CItemInstanceDef newFurnItemDef = CItemInstanceDef.FromObjectNoDBID(EItemID.FURNITURE, furnInst.Value, EItemSocket.None, 0, EItemParentTypes.Player);
					a_PlayerPlacingItem.Inventory.AddItemToNextFreeSuitableSlot(newFurnItemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							// remove if given successfully
							NetworkEvents.SendLocalEvent_OnPropertyFurnitureInstanceDestroyed(furnInst);
							m_lstFurniture.Remove(furnInst);
						}

						// transmit furniture to every player in this property
						TransmitFurnitureToAllPlayersInside();
						CompletionCallback(true);
					});
				}
			}
			else
			{
				CompletionCallback(false);
			}
		});
	}

	public CPropertyFurnitureInstance GetFurnitureItemFromDBID(EntityDatabaseID dbid)
	{
		foreach (CPropertyFurnitureInstance furnIter in m_lstFurniture)
		{
			if (furnIter.DBID == dbid)
			{
				return furnIter;
			}
		}

		return null;
	}

	public void CommitFurnitureChange(EntityDatabaseID dbid, Vector3 a_vecPos, Vector3 a_vecRot)
	{
		Database.Functions.Items.CommitFurnitureChange(dbid, Model.Id, a_vecPos, a_vecRot);

		CPropertyFurnitureInstance furnInst = m_lstFurniture.FirstOrDefault(furnIter => furnIter.DBID == dbid);

		if (furnInst != null)
		{
			furnInst.vecPos = a_vecPos;
			furnInst.vecRot = a_vecRot;
		}

		TransmitFurnitureToAllPlayersInside();
	}

	public void TransmitFurnitureToAllPlayersInside()
	{
		// TODO_FURNITURE: Only transmit the diff if its called due to editing furniture rather than interior enter?
		foreach (CPlayer player in PlayerPool.GetAllPlayers())
		{
			if (player.Client.Dimension == Model.Id)
			{
				TransmitFurnitureAndRemovals(player);
			}
		}
	}

	public void TransmitFurnitureAndRemovals(CPlayer a_Player)
	{
		NetworkEventSender.SendNetworkEvent_UpdateFurnitureCache(a_Player, Model.Id, m_lstFurniture, m_lstDefaultFurnitureRemovals);
	}

	public bool CanExpire()
	{
		return Model.OwnerType == EPropertyOwnerType.Player && Model.EntranceType == EPropertyEntranceType.Normal;
	}

	public async Task<bool> CanBeInactive()
	{
		if (!CanExpire())
		{
			return false;
		}

		if (Model.State == EPropertyState.AvailableToBuy || Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable)
		{
			return false;
		}

		// check protection first
		bool bProtected = await Database.LegacyFunctions.IsEntityInactivityProtected(Model.Id, EDonationInactivityPurchasables.PropertyPurchasable).ConfigureAwait(true);

		return !bProtected;
	}

	public async Task<bool> IsInactive()
	{
		if (!await CanBeInactive().ConfigureAwait(true))
		{
			return false;
		}

		bool bIsOwnerCharacterInactive = await Database.LegacyFunctions.IsCharacterInactive(Model.OwnerId).ConfigureAwait(true);

		// Has it not been used in a while?
		DateTime InactiveTime = DateTime.Now.Subtract(TimeSpan.FromDays(InactivityScannerContains.numDaysToConsiderInactiveForUse));
		bool bIsLastUsedConsideredInactive = LastUsed <= InactiveTime;
		return bIsOwnerCharacterInactive || bIsLastUsedConsideredInactive;
	}

	public string GetOwnerText()
	{
		return EntityDataManager.GetData<string>(m_EntranceMarker, EDataNames.PROP_OWNER_TEXT);
	}

	private void UpdateOwnerBlips()
	{
		// just destroy the blip for everyone, if there is a new owner, we'll recreate it
		NetworkEventSender.SendNetworkEvent_Property_DestroyPlayerBlip_ForAll_IncludeEveryone((int)Model.Id);

		if (Model.OwnerType == EPropertyOwnerType.Player)
		{
			WeakReference<CPlayer> ownerPlayerRef = PlayerPool.GetPlayerFromCharacterID(Model.OwnerId);
			if (ownerPlayerRef != null && ownerPlayerRef.Instance() != null)
			{
				CPlayer ownerPlayer = ownerPlayerRef.Instance();

				Vector3 blipPosition = GetBlipPosition();

				NetworkEventSender.SendNetworkEvent_Property_CreatePlayerBlip(ownerPlayer, (int)Model.Id, Model.Name, blipPosition);
			}
		}
	}

	private Vector3 GetBlipPosition()
	{
		if (Model.EntranceDimension == 0)
		{
			return Model.EntrancePosition;
		}

		// Fetch the parent from it's children
		uint childDimension = Model.EntranceDimension;

		while (true)
		{
			CPropertyInstance property = PropertyPool.GetPropertyInstanceFromID(childDimension);

			if (property.Model.EntranceDimension == 0)
			{
				return property.Model.EntrancePosition;
			}
			childDimension = property.Model.EntranceDimension;
		}
	}

	public void UpdateOwnerBlipForPlayer(CPlayer player)
	{
		if (Model.OwnerType == EPropertyOwnerType.Player && player.ActiveCharacterDatabaseID == Model.OwnerId)
		{
			Vector3 blipPosition = GetBlipPosition();
			NetworkEventSender.SendNetworkEvent_Property_CreatePlayerBlip(player, (int)Model.Id, "House", blipPosition);
		}
	}

	private void CreateMarkers()
	{
		NAPI.Task.Run(() =>
		{
			UpdateOwnerBlips();

			if (m_EntranceMarker != null)
			{
				m_EntranceMarker.Delete();
				m_EntranceMarker = null;
			}

#if PROPERTY_ARROW_INSIDE_MARKER
			if (m_EntranceMarkerArrow != null)
			{
				m_EntranceMarkerArrow.Delete();
				m_EntranceMarkerArrow = null;
			}

			if (m_ExitMarkerArrow != null)
			{
				m_ExitMarkerArrow.Delete();
				m_ExitMarkerArrow = null;
			}
#endif

			if (m_ExitMarker != null)
			{
				m_ExitMarker.Delete();
				m_ExitMarker = null;
			}

			if (m_Blip != null)
			{
				NAPI.Entity.DeleteEntity(m_Blip);
				m_Blip = null;
			}

			if (m_SignObject != null)
			{
				NAPI.Entity.DeleteEntity(m_SignObject);
				m_SignObject = null;
			}

			// Create a sign if for rent/buy
			if (Model.IsAvailable())
			{
				string[] strObjectNames =
				{
					"prop_forsale_sign_01",
					"prop_forsale_sign_02",
					"prop_forsale_sign_03",
					"prop_forsale_sign_04",
					"prop_forsalejr1",
					"prop_forsalejr2",
					"prop_forsalejr3",
					"prop_forsalejr4",
					"prop_forsale_sign_05", // needs to be rotated
					"prop_forsale_sign_06", // needs to be rotated
					"prop_forsale_sign_07", // needs to be rotated
					"prop_forsale_sign_fs",  // needs to be rotated
					"prop_forsale_sign_jb",  // needs to be rotated
				};

				int objectIndex = new Random().Next(0, strObjectNames.Length - 1);

				Vector3 vecPosToSide = new Vector3(Model.EntrancePosition.X, Model.EntrancePosition.Y, Model.EntrancePosition.Z - 0.2f);
				float fSignRot = Model.EntranceRotation + 90.0f;
				float fDist = -1.5f;
				float radians = (fSignRot + 90.0f) * (3.14f / 180.0f);
				vecPosToSide.X += (float)Math.Cos(radians) * fDist;
				vecPosToSide.Y += (float)Math.Sin(radians) * fDist;
				m_SignObject = NAPI.Object.CreateObject(NAPI.Util.GetHashKey(strObjectNames[objectIndex]), vecPosToSide, new Vector3(0.0f, 0.0f, fSignRot - ((objectIndex > 7) ? 270.0 : 0.0)), 255, Model.EntranceDimension);
			}

			Color entranceColor = new Color(255, 194, 15, 60);
			Color entranceColorArrow = new Color(255, 194, 15, 60);

			if (Model.IsAvailable())
			{
				entranceColor = new Color(34, 177, 76, 100);
				entranceColorArrow = new Color(34, 177, 76, 100);
			}

			m_EntranceMarker = NAPI.Marker.CreateMarker(23, Model.EntrancePosition.Add(new Vector3(0.0f, 0.0f, 0.4f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0, 00.0f, Model.EntranceRotation), 0.7f, entranceColor, true, Model.EntranceDimension);

			// inner arrow marker
#if PROPERTY_ARROW_INSIDE_MARKER
			m_EntranceMarkerArrow = NAPI.Marker.CreateMarker(2, EntrancePos.Add(new Vector3(0.0f, 0.0f, 0.4f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0, 00.0f, EntranceRot), 0.5f, entranceColorArrow, true, EntranceDimension);
#endif

			if (Model.EntranceType != EPropertyEntranceType.World) // World interiors dont need an exit marker, the marker is purely for buying / seeing owner
			{
				m_ExitMarker = NAPI.Marker.CreateMarker(23, Model.ExitPosition.Add(new Vector3(0.0f, 0.0f, 0.4f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0, 00.0f, Model.ExitRotation), 0.7f, new Color(255, 194, 15, 200), true, Convert.ToUInt32(Model.Id));

				// inner arrow marker
#if PROPERTY_ARROW_INSIDE_MARKER
				m_ExitMarkerArrow = NAPI.Marker.CreateMarker(2, ExitPos.Add(new Vector3(0.0f, 0.0f, 0.4f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(180.0, 00.0f, EntranceRot), 0.5f, new Color(255, 194, 15, 100), true, Convert.ToUInt32(m_DatabaseID));
#endif
			}

			UpdateMarkerEntityData();

			// Calculate prices
			CalculatePaymentInformation();

			// TODO: set below correctly
			if (Model.OwnerType == EPropertyOwnerType.Player)
			{
				Database.Functions.Characters.GetCharacterNameFromDBID(Model.OwnerId, (string strCharacterName) =>
				{
					EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_OWNER_TEXT, strCharacterName, EDataType.Synced);
				});
			}
			else if (Model.OwnerType == EPropertyOwnerType.Faction)
			{
				CFaction factionInst = FactionPool.GetFactionFromID(Model.OwnerId);

				if (factionInst != null)
				{
					EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_OWNER_TEXT, factionInst.Name, EDataType.Synced);

					// Is it a law enforcement / medical faction? Give it a blip
					if (Model.HasScriptedBlip)
					{
						if (factionInst.Type == EFactionType.LawEnforcement)
						{
							m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 60, 63);
						}
						else if (factionInst.Type == EFactionType.Medical)
						{
							if (Model.Name.ToLower().Contains("hospital") || Model.Name.ToLower().Contains("medical center"))
							{
								m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 61, 49);
							}
							else
							{
								m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 88, 49);
							}
						}
						else if (factionInst.Type == EFactionType.ScriptedVehicleStore)
						{
							m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 523);
						}
						else if (factionInst.Type == EFactionType.Government)
						{
							m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 541);
						}
						else if (factionInst.Type == EFactionType.ScriptedFurnitureStore)
						{
							m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 587);
						}
						else if (factionInst.Type == EFactionType.ScriptedPlasticSurgeon)
						{
							m_Blip = HelperFunctions.Blip.Create(Model.EntrancePosition, true, 50.0f, Model.EntranceDimension, Model.Name, 267);
						}
					}
				}
			}
			else
			{
				EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_OWNER_TEXT, "", EDataType.Synced);
			}
		});
	}

	public void UpdateMarkerEntityData()
	{
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_ID, Model.Id, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_ENTER, true, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_STATE, Model.State, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_NAME, Model.Name, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_ENT_TYPE, Model.EntranceType, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_OWNER_ID, Model.OwnerId, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_RENTER_ID, Model.RenterId, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_OWNER_TYPE, Model.OwnerType, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_RENTER_TYPE, Model.RenterType, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_XP, Model.XP, EDataType.Synced);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_LAST_MOWED_AT, Model.LastMowedAt, EDataType.Synced);

		if (Model.EntranceType != EPropertyEntranceType.World)
		{
			EntityDataManager.SetData(m_ExitMarker, EDataNames.PROP_EXIT, true, EDataType.Synced);
			EntityDataManager.SetData(m_ExitMarker, EDataNames.PROP_ID, Model.Id, EDataType.Synced);
		}
	}

	public void OnPurchasedPlayer(CPlayer a_BuyingPlayer, int a_PaymentsRemaining, float a_fCreditAmount, bool a_bTokenUsed = false)
	{
		Model.Purchase(
			Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable ? EPropertyState.Owned_AlwaysEnterable : EPropertyState.Owned,
			Model.State == EPropertyState.Owned,
			EPropertyOwnerType.Player,
			a_PaymentsRemaining,
			0,
			0,
			a_fCreditAmount,
			a_BuyingPlayer.ActiveCharacterDatabaseID,
			a_bTokenUsed
		);

		a_BuyingPlayer.AwardAchievement(EAchievementID.BuyProperty);

		CreateMarkers();
	}

	public void OnPurchasedFaction(CFaction a_Faction, int a_PaymentsRemaining, float a_fCreditAmount)
	{
		Model.Purchase(
			Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable ? EPropertyState.Owned_AlwaysEnterable : EPropertyState.Owned,
			Model.State == EPropertyState.Owned,
			EPropertyOwnerType.Faction,
			a_PaymentsRemaining,
			0,
			0,
			a_fCreditAmount,
			a_Faction.FactionID
		);

		CreateMarkers();
	}

	/// <summary>
	/// Calculates the monthly payment information and assigns the appropriate entity data
	/// </summary>
	private void CalculatePaymentInformation()
	{
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_BUY_PRICE, Model.BuyPrice, EDataType.Synced);
	}

	public int GetPaymentPlanNumberOfMonths()
	{
		return Model.PaymentsRemaining + Model.PaymentsMade;
	}

	public float GetMonthlyPaymentAmount()
	{
		if (Model.PaymentsRemaining == 0)
		{
			return 0.0f;
		}

		float fInterest = (Model.CreditAmount * Taxation.GetPaymentPlanInterestPercent());
		float fMonthlyPaymentAmount = (Model.CreditAmount / GetPaymentPlanNumberOfMonths()) + (fInterest / GetPaymentPlanNumberOfMonths());
		return fMonthlyPaymentAmount;
	}

	public void DecreaseCreditAmount(float fAmount)
	{
		float creditRemaining = GetRemainingCredit(true);

		if (creditRemaining <= 0.0f)
		{
			return;
		}

		// If they are paying off the whole debt, wipe the interest & payments remaining.
		if (Math.Abs(creditRemaining - fAmount) < 0.1f)
		{
			Model.ClearCreditAmount();
		}
		else
		{
			Model.DecreaseCreditAmount(fAmount);
		}
	}

	public float GetRemainingCredit(bool bIgnoreInterest = false)
	{
		if (Model.PaymentsRemaining == 0)
		{
			return 0.0f;
		}

		float fInterest = bIgnoreInterest ? 0.0f : Model.CreditAmount * Taxation.GetPaymentPlanInterestPercent();
		float fAmountPaid = Model.PaymentsMade * GetMonthlyPaymentAmount();
		float fAmountRemaining = (Model.CreditAmount + fInterest) - fAmountPaid;

		return fAmountRemaining;
	}

	public float GetRemainingCreditInterest()
	{
		return Model.PaymentsRemaining == 0 ? 0.0f : Model.CreditAmount * Taxation.GetPaymentPlanInterestPercent();
	}

	public float GetMonthlyTax()
	{
		return Taxation.GetPropertyMonthlyTax() * Model.BuyPrice;
	}

	public async void Repossess(bool byPlayer = false)
	{
		if ((Model.State == EPropertyState.Owned || Model.State == EPropertyState.Rented) && !byPlayer)
		{
			Logging.Log.CreateLog(Model.OwnerId, Logging.EOriginType.Character, Logging.ELogType.PropertyRelated, new List<CBaseEntity>() { this },
				$"INACTIVITY SCANNER FORCESOLD {Model.Name}.");
		}

		Model.Repossess();

		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Model.Id);
		await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);

		CreateMarkers();

		// TODO_LAUNCH: evict everyone inside
	}

	/// <summary>
	/// You should probably call the PropertyPool DestroyProperty
	/// </summary>
	/// <param name="RemoveFromDatabase"></param>
	public void Destroy(bool RemoveFromDatabase)
	{
		if (m_EntranceMarker != null)
		{
			NAPI.Entity.DeleteEntity(m_EntranceMarker.Handle);
			m_EntranceMarker = null;
		}

#if PROPERTY_ARROW_INSIDE_MARKER
		if (m_EntranceMarkerArrow != null)
		{
			NAPI.Entity.DeleteEntity(m_EntranceMarkerArrow.Handle);
			m_EntranceMarkerArrow = null;
		}

		if (m_ExitMarkerArrow != null)
		{
			NAPI.Entity.DeleteEntity(m_ExitMarkerArrow.Handle);
			m_ExitMarkerArrow = null;
		}
#endif

		if (m_ExitMarker != null)
		{
			NAPI.Entity.DeleteEntity(m_ExitMarker.Handle);
			m_ExitMarker = null;
		}

		if (m_Blip != null)
		{
			NAPI.Entity.DeleteEntity(m_Blip);
			m_Blip = null;
		}

		if (m_SignObject != null)
		{
			NAPI.Entity.DeleteEntity(m_SignObject.Handle);
			m_SignObject = null;
		}

		if (RemoveFromDatabase)
		{
			Model.Delete(async () =>
			{
				CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Model.Id);
				await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);
			});
		}
	}

	public bool OwnedBy(CPlayer Player)
	{
		return Model.State != EPropertyState.AvailableToBuy && Model.OwnerType == EPropertyOwnerType.Player && Player.ActiveCharacterDatabaseID == Model.OwnerId;
	}

	public bool OwnedBy(EPropertyOwnerType type, Int64 id)
	{
		return Model.State != EPropertyState.AvailableToBuy && Model.OwnerType == type && id == Model.OwnerId;
	}

	public bool OwnedBy(CFaction Faction)
	{
		return Model.State != EPropertyState.AvailableToBuy && Model.OwnerType == EPropertyOwnerType.Faction && Faction.FactionID == Model.OwnerId;
	}

	public bool HasKeys(CPlayer player)
	{
		CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Model.Id);
		return player.Inventory.HasItem(ItemInstanceDef, true, out CItemInstanceDef outItem);
	}

	public bool RentedBy(CPlayer Player)
	{
		return Model.State == EPropertyState.Rented && Model.RenterType == EPropertyOwnerType.Faction && Player.ActiveCharacterDatabaseID == Model.RenterId;
	}

	public bool RentedBy(CFaction Faction)
	{
		return Model.State == EPropertyState.Rented && Model.RenterType == EPropertyOwnerType.Faction && Faction.FactionID == Model.RenterId;
	}

	public void SetEntrance(Vector3 position, float rot, Dimension dim)
	{
		Model.SetEntrance(position, rot, dim);
		CreateMarkers();
	}

	public void SetExit(Vector3 position, float rot)
	{
		Model.SetExit(position, rot);
		CreateMarkers();
	}

	public void SetBuyPrice(float newPrice)
	{
		if (newPrice < 0)
		{
			return;
		}

		Model.SetBuyPrice(newPrice);
		CalculatePaymentInformation();
	}

	public void SetRentPrice(float newPrice)
	{
		if (newPrice < 0)
		{
			return;
		}

		Model.SetRentPrice(newPrice);
		CalculatePaymentInformation();
	}

	public void SetOwner(EPropertyOwnerType newType, EntityDatabaseID newOwner)
	{
		Model.SetOwner(newType, newOwner);
		CreateMarkers();
	}

	public void SetRenter(EPropertyOwnerType newType, EntityDatabaseID newRenter)
	{
		Model.SetRenter(newType, newRenter);
		CreateMarkers();
	}

	public void SetName(string name)
	{
		Model.SetName(name);
		EntityDataManager.SetData(m_EntranceMarker, EDataNames.PROP_NAME, name, EDataType.Synced);
	}

	public bool IsPropertyForAnyPlayerFaction(CPlayer a_Player, bool a_bManagerOnly = false)
	{
		if (Model.OwnerType != EPropertyOwnerType.Faction)
		{
			return false;
		}

		return a_Player.GetFactionMemberships()
			.Where(factionMembership => Model.OwnerId == factionMembership.Faction.FactionID)
			.Any(factionMembership => !a_bManagerOnly || factionMembership.Manager);
	}

	public DateTime LastUsed
	{
		get
		{
			DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return date.AddSeconds(Model.LastUsed).ToLocalTime();
		}
	}

#pragma warning disable CA1051 // Do not declare visible instance fields
	public readonly Property Model;
#pragma warning restore CA1051 // Do not declare visible instance fields
	public CPropertyMailbox Inventory => m_Mailbox;
	private CPropertyMailbox m_Mailbox;

#if PROPERTY_ARROW_INSIDE_MARKER
	private Marker m_EntranceMarkerArrow = null;
	private Marker m_ExitMarkerArrow = null;
#endif

	private Blip m_Blip = null;
	private Marker m_EntranceMarker = null;
	private Marker m_ExitMarker = null;
	private GTANetworkAPI.Object m_SignObject = null;

	private List<CPropertyFurnitureInstance> m_lstFurniture = new List<CPropertyFurnitureInstance>();
	private List<CPropertyDefaultFurnitureRemovalInstance> m_lstDefaultFurnitureRemovals = new List<CPropertyDefaultFurnitureRemovalInstance>();
}