using RAGE.Game;
using System;
using System.Collections.Generic;
using System.Linq;

public class StoreSystem
{
	private CGUIStore m_StoreUI = new CGUIStore(() => { });

	private bool m_bPendingTransaction = false;
	private List<WeakReference<CWorldPed>> m_lstWorldPeds = new List<WeakReference<CWorldPed>>();
	public static Int64 CurrentStoreID { get; set; } = -1;

	public StoreSystem()
	{
		NetworkEvents.CreateStorePed += CreateStorePed;
		NetworkEvents.DestroyStorePed += OnDestroyStorePed;
		NetworkEvents.ChangeCharacterApproved += HideStoreUI;
		NetworkEvents.GotStoreInfo += OnGotStoreInfo;
		NetworkEvents.OnStoreCheckout_Response += OnStoreCheckout_Response;
		NetworkEvents.Store_OnRobberyFinished += OnRobberyFinished;
		RageEvents.RAGE_OnRender += OnRender;

		UIEvents.ClothingStoreSelector_Exit += OnExitClothingStoreSelector;
		UIEvents.ClothingStoreSelector_GotoClothingStore += OnGotoClothingStore;
		UIEvents.ClothingStoreSelector_GotoOutfitEditor += OnGotoOutfitEditor;
	}

	private void OnExitClothingStoreSelector()
	{
		// nothing to do here, the UI is gone
	}

	private void OnGotoClothingStore()
	{
		ShowClothingStore();
	}

	private void OnGotoOutfitEditor()
	{
		ShowOutfitEditor();
	}

	private void OnGotStoreInfo(List<EItemID> lstItems, float fSalesTaxRate)
	{
		ShowStoreUI();

		m_StoreUI.SetTaxRate(fSalesTaxRate);

		foreach (EItemID itemID in lstItems)
		{
			CInventoryItemDefinition itemDef = ItemDefinitions.g_ItemDefinitions[itemID];

			// TODO_GENERICS: Must support generics for desc
			string strDescription = itemDef.GetDescIgnoreGenericItems();

			if (WeaponHelpers.IsItemAWeaponAttachment(itemID))
			{
				strDescription = Helpers.FormatString("{0}<br><br><b>Fits Weapons: </b>", itemDef.GetDescIgnoreGenericItems());
				int index = 0;

				if (WeaponAttachmentDefinitions.g_WeaponAttachmentIDs.ContainsKey(itemID))
				{
					foreach (var kvPair in WeaponAttachmentDefinitions.g_WeaponAttachmentIDs[itemID])
					{
						// find the item that corresponds to the hash
						foreach (var kvPairItemToWeapon in ItemWeaponDefinitions.g_DictItemIDToWeaponHash)
						{
							if (kvPairItemToWeapon.Value == kvPair.Key)
							{
								if (index > 0)
								{
									strDescription += ", ";
								}

								CInventoryItemDefinition itemDefOfWeapon = ItemDefinitions.g_ItemDefinitions[kvPairItemToWeapon.Key];

								string strColor = RAGE.Elements.Player.LocalPlayer.HasGotWeapon((uint)kvPair.Key, false) ? "green" : "red";
								strDescription += Helpers.FormatString("<font color='{0}'>{1}</font>", strColor, itemDefOfWeapon.GetNameIgnoreGenericItems());

								++index;
								break;
							}
						}
					}

				}
			}

			m_StoreUI.AddItem(itemID, strDescription);
		}
	}

	private void OnStoreCheckout_Response(EStoreCheckoutResult result)
	{
		Store_SetPendingTransaction(false);

		if (result == EStoreCheckoutResult.FailedPartial)
		{
			m_StoreUI.ShowErrorMessage("These items could not be purchased. Please check the notification.");
		}
		if (result == EStoreCheckoutResult.CannotAfford)
		{
			m_StoreUI.ShowErrorMessage("You cannot afford these items.");
		}
		else if (result == EStoreCheckoutResult.Success)
		{
			HideStoreUI();
			NotificationManager.ShowNotification("Store", "Your items have been purchased.", ENotificationIcon.InfoSign);
		}
		else if (result == EStoreCheckoutResult.NoHandgunLicense)
		{
			m_StoreUI.ShowErrorMessage("You must have a Tier 1 Firearm License to purchase from this store.");
		}
		else if (result == EStoreCheckoutResult.NoLonggunLicense)
		{
			m_StoreUI.ShowErrorMessage("You must have a Tier 2 Firearm License to purchase from this store.");
		}
		else if (result == EStoreCheckoutResult.NotInCriminalFaction)
		{
			m_StoreUI.ShowErrorMessage("You must be in an official criminal faction to purchase from this store.");
		}
		else if (result == EStoreCheckoutResult.NeedAnyFirearmLicenseForAmmo)
		{
			m_StoreUI.ShowErrorMessage("You must have a Firearm License (Any Tier) to purchase from this store.");
		}
		else if (result == EStoreCheckoutResult.Failed_MaskForCustomChar)
		{
			m_StoreUI.ShowErrorMessage("This mask is only for premade characters. For custom characters, please visit any clothing store and purchase a mask.");
		}
	}

	private void OnDestroyStorePed(RAGE.Vector3 vecPos, float fRot, uint dimension)
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

	private void CreateStorePed(RAGE.Vector3 vecPos, float fRot, uint dimension, Int64 storeID, EStoreType storeType)
	{
		uint skinID = 3882958867;

		if (storeType >= EStoreType.Guns_Legal_Handguns && storeType <= EStoreType.Guns_Illegal_SMG)
		{
			skinID = 1161072059;
		}
		if (storeType == EStoreType.Guns_Legal_SMG)
		{
			skinID = 1161072059;
		}
		else if (storeType == EStoreType.Ammo)
		{
			skinID = 1161072059;
		}
		else if (storeType == EStoreType.General)
		{
			skinID = 3882958867;
		}
		else if (storeType == EStoreType.Furniture)
		{
			skinID = 3882958867;
		}
		else if (storeType == EStoreType.Police)
		{
			if (vecPos.Y >= Constants.BorderOfLStoPaleto && dimension == 0) // sheriff PD world interior
			{
				skinID = 1096929346;
			}
			else
			{
				skinID = 0x15F8700D;
			}
		}
		else if (storeType == EStoreType.Hunting)
		{
			skinID = 3457361118;
		}
		else if (storeType == EStoreType.Armor)
		{
			skinID = 3882958867;
		}
		else if (storeType == EStoreType.Clothing)
		{
			skinID = 3882958867;
		}
		else if (storeType == EStoreType.Barber)
		{
			skinID = 373000027;
		}
		else if (storeType == EStoreType.Alcohol)
		{
			skinID = 3882958867;
		}
		else if (storeType == EStoreType.Drugs)
		{
			skinID = 2162532142;
		}
		else if (storeType == EStoreType.Fishing)
		{
			skinID = 3457361118;
		}
		else if (storeType == EStoreType.Fishmonger)
		{
			skinID = 0x85889AC3;
		}
		else if (storeType == EStoreType.TattooArtist)
		{
			skinID = 0x94AE2B8C;
		}
		else if (storeType == EStoreType.PlasticSurgeon)
		{
			skinID = 0xD47303AC;
		}
		else if (storeType == EStoreType.Tobbaco)
		{
			skinID = 3852538118;
		}

		string strStoreTypeName = GetDisplayNameFromStoreType(storeType, vecPos, dimension);
		WeakReference<CWorldPed> refWorldPed = WorldPedManager.CreatePed(EWorldPedType.StoreCashier, skinID, vecPos, fRot, dimension);
		refWorldPed.Instance()?.SetId(storeID);
		refWorldPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, Helpers.FormatString("Talk to {0}", strStoreTypeName), null, () => { OnInteractWithStore(storeType, storeID); }, false, false, 3.0f, null, true);
		m_lstWorldPeds.Add(refWorldPed);
	}

	private void OnInteractWithStore(EStoreType storeType, Int64 storeID)
	{
		CurrentStoreID = storeID;

		// TODO: Server verify pos
		if (!m_StoreUI.IsVisible() && !m_bPendingTransaction)
		{
			if (storeType == EStoreType.Clothing)
			{
				GenericPrompt3Helper.ShowPrompt("Clothing Store", "Which store would you like to access?", "Exit", "Clothing Store", "Outfit Editor", UIEventID.ClothingStoreSelector_Exit, UIEventID.ClothingStoreSelector_GotoClothingStore, UIEventID.ClothingStoreSelector_GotoOutfitEditor, EPromptPosition.Center);
			}
			else if (storeType == EStoreType.Barber)
			{
				ShowBarberShop();
			}
			else if (storeType == EStoreType.TattooArtist)
			{
				ShowTattooArtist();
			}
			else if (storeType == EStoreType.PlasticSurgeon)
			{
				ShowPlasticSurgeon();
			}
			else
			{
				NetworkEventSender.SendNetworkEvent_GetStoreInfo(storeID);
			}
		}
	}

	private void ShowClothingStore()
	{
		StoreSystem_Core.GetClothingStore()?.Show();
	}

	private void ShowOutfitEditor()
	{
		StoreSystem_Core.GetOutfitEditor()?.Show();
	}

	private void ShowBarberShop()
	{
		StoreSystem_Core.GetBarberShop()?.Show();
	}

	public void ShowTattooArtist()
	{
		StoreSystem_Core.GetTattooArtist()?.Show();
	}

	public void ShowPlasticSurgeon()
	{
		StoreSystem_Core.GetPlasticSurgeon()?.Show();
	}

	private void Store_SetPendingTransaction(bool bPendingTransaction)
	{
		m_bPendingTransaction = bPendingTransaction;
		m_StoreUI.SetButtonsEnabled(!bPendingTransaction);
	}

	public void OnStoreCheckout(string strJsonData, int numItems)
	{
		if (numItems > 0)
		{
			Store_SetPendingTransaction(true);
			NetworkEventSender.SendNetworkEvent_OnStoreCheckout(CurrentStoreID, strJsonData);

		}
		else
		{
			Store_SetPendingTransaction(false);
			HideStoreUI();
		}
	}

	public void HideStoreUI()
	{
		CurrentStoreID = -1;
		m_StoreUI.SetVisible(false, false, false);
		Store_SetPendingTransaction(false);
	}

	private void ShowStoreUI()
	{
		m_StoreUI.SetVisible(true, true, false);
		m_StoreUI.Reset();
	}

	private string GetDisplayNameFromStoreType(EStoreType storeType, RAGE.Vector3 vecPos, uint dimension)
	{
		string strRetVal = "Store Cashier";
		if (storeType == EStoreType.Guns_Legal_Handguns)
		{
			strRetVal = "Firearms Vendor (Handguns)";
		}
		else if (storeType == EStoreType.Guns_Legal_Shotguns)
		{
			strRetVal = "Firearms Vendor (Shotguns)";
		}
		else if (storeType == EStoreType.Guns_Legal_Rifles)
		{
			strRetVal = "Firearms Vendor (Rifles)";
		}
		else if (storeType == EStoreType.Guns_Legal_SMG)
		{
			strRetVal = "Firearms Vendor (SMG's)";
		}
		else if (storeType == EStoreType.Guns_Illegal_Handguns)
		{
			strRetVal = "Firearms Vendor (Illegal Handguns)";
		}
		else if (storeType == EStoreType.Guns_Illegal_Shotguns)
		{
			strRetVal = "Firearms Vendor (Illegal Shotguns)";
		}
		else if (storeType == EStoreType.Guns_Illegal_Rifles)
		{
			strRetVal = "Firearms Vendor (Illegal Rifles)";
		}
		else if (storeType == EStoreType.Guns_Illegal_Heavy)
		{
			strRetVal = "Firearms Vendor (Illegal Heavy Weaponry)";
		}
		else if (storeType == EStoreType.Guns_Illegal_Snipers)
		{
			strRetVal = "Firearms Vendor (Illegal Sniper Rifles)";
		}
		else if (storeType == EStoreType.Guns_Illegal_SMG)
		{
			strRetVal = "Firearms Vendor (Illegal SMG's)";
		}
		else if (storeType == EStoreType.Ammo) // ammo
		{
			strRetVal = "Ammo Vendor";
		}
		else if (storeType == EStoreType.General)
		{
			strRetVal = "General Goods Cashier";
		}
		else if (storeType == EStoreType.Furniture)
		{
			strRetVal = "Furniture Store Cashier";
		}
		else if (storeType == EStoreType.Police)
		{
			if (vecPos.Y >= Constants.BorderOfLStoPaleto && dimension == 0) // sheriff PD world interior
			{
				strRetVal = "Deputy";
			}
			else
			{
				strRetVal = "Officer";
			}
		}
		else if (storeType == EStoreType.Hunting)
		{
			strRetVal = "Hunting Merchandiser";
		}
		else if (storeType == EStoreType.Armor)
		{
			strRetVal = "Armor Vendor";
		}
		else if (storeType == EStoreType.Clothing)
		{
			strRetVal = "Clothing Store Cashier";
		}
		else if (storeType == EStoreType.Barber)
		{
			strRetVal = "Barber";
		}
		else if (storeType == EStoreType.Alcohol)
		{
			strRetVal = "Liquor Store Cashier";
		}
		else if (storeType == EStoreType.Drugs)
		{
			strRetVal = "Drug Dealer";
		}
		else if (storeType == EStoreType.Fishing)
		{
			strRetVal = "Fishing Merchandiser";
		}
		else if (storeType == EStoreType.Fishmonger)
		{
			strRetVal = "Fishmonger";
		}
		else if (storeType == EStoreType.TattooArtist)
		{
			strRetVal = "Tattoo Artist";
		}
		else if (storeType == EStoreType.PlasticSurgeon)
		{
			strRetVal = "Plastic Surgeon";
		}
		else if (storeType == EStoreType.Tobbaco)
		{
			strRetVal = "Tobacconist";
		}

		return strRetVal;
	}

	private CWorldPed m_pedAimedAt = null;
	private long m_initiatedRobberyAt = 0;
	private bool m_robberyActive = false;
	private const long SECONDS_BEFORE_ROBBERY = 5;
	private const string HANDSUP_ANIM_DICT = "mp_pol_bust_out";
	private const string HANDSUP_ANIM = "guard_handsup_loop";
	private const int STORE_ROBBERY_MAX_RANGE = 20;

	private void OnRender()
	{
		CWorldPed ped = GetPedBeingAimedAt();

		if (m_robberyActive)
		{
			if (!m_pedAimedAt.LocalPlayerInRange(STORE_ROBBERY_MAX_RANGE))
			{
				NetworkEventSender.SendNetworkEvent_Store_CancelRobbery(m_pedAimedAt.Id);
				ResetRobberyState();
				ResetStorePedAnim();
			}
			return;
		}

		if (ped != null && !Streaming.HasAnimDictLoaded(HANDSUP_ANIM_DICT))
		{
			Streaming.RequestAnimDict(HANDSUP_ANIM_DICT);
			return;
		}

		if (ped == null && m_pedAimedAt != null)
		{
			// If they stop aiming at the ped before the robbery is active, cancel the robbery.
			ResetStorePedAnim();
			ResetRobberyState();
			return;
		}

		if (ped != null && m_pedAimedAt == null && ped.LocalPlayerInRange(STORE_ROBBERY_MAX_RANGE) && HasValidWeaponToRob())
		{
			NotificationManager.ShowNotification("Store Robbery", "Continue aiming at the clerk to initiate a robbery.", ENotificationIcon.Star);
			m_pedAimedAt = ped;
			m_initiatedRobberyAt = Helpers.GetUnixTimestamp();

			StorePedHandsUpLookingAtPlayer();
			return;
		}

		if (m_pedAimedAt != null && m_initiatedRobberyAt < Helpers.GetUnixTimestamp() - SECONDS_BEFORE_ROBBERY && HasValidWeaponToRob())
		{
			m_robberyActive = true;
			NetworkEventSender.SendNetworkEvent_Store_InitiateRobbery(m_pedAimedAt.Id);
		}
	}

	private bool HasValidWeaponToRob()
	{
		uint weapon = RAGE.Elements.Player.LocalPlayer.GetSelectedWeapon();
		EItemID weaponID = (EItemID)WeaponHelpers.GetWeaponItemIDFromHash(weapon);

		return WeaponHelpers.IsItemAFirearm(weaponID);
	}

	private void StorePedHandsUpLookingAtPlayer()
	{
		m_pedAimedAt.PedInstance.FreezePosition(false);
		int pedHandle = m_pedAimedAt.PedInstance.Handle;
		const int flags = (int)(AnimationFlags.StopOnLastFrame | AnimationFlags.OnlyAnimateUpperBody);
		Ai.TaskTurnPedToFaceEntity(pedHandle, RAGE.Elements.Player.LocalPlayer.Handle, 40000);
		Ai.TaskPlayAnim(pedHandle, HANDSUP_ANIM_DICT, HANDSUP_ANIM, 8.0f, 1.0f, 40000, flags, 1.0f, false, false, false);
	}

	private void ResetStorePedAnim()
	{
		if (m_pedAimedAt?.PedInstance == null)
		{
			return;
		}

		m_pedAimedAt.PedInstance.SetHeading(m_pedAimedAt.RotZ);
		m_pedAimedAt.PedInstance.FreezePosition(true);
		Ai.StopAnimTask(m_pedAimedAt.PedInstance.Handle, HANDSUP_ANIM_DICT, HANDSUP_ANIM, 3);
	}

	private void ResetRobberyState()
	{
		m_initiatedRobberyAt = 0;
		m_pedAimedAt = null;
		m_robberyActive = false;
	}

	private void OnRobberyFinished()
	{
		ResetStorePedAnim();
		ResetRobberyState();
	}

	private CWorldPed GetPedBeingAimedAt()
	{
		int entity = 0;
		if (!Player.GetEntityPlayerIsFreeAimingAt(ref entity) || entity == 0)
		{
			return null;
		}

		return m_lstWorldPeds
			.Select(worldPed => worldPed?.Instance())
			.Where(ped => ped?.PedInstance != null)
			.FirstOrDefault(ped => ped.PedInstance.Handle == entity);
	}
}
