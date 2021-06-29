using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class Frisk
{
	private CGUIFrisk m_FriskUI = null;

	private List<CItemInstanceDef> m_lstFriskedPlayerInventoryItems = null;

	public Frisk()
	{
		NetworkEvents.ClientFriskPlayer += OnFriskPlayer;
		NetworkEvents.ChangeCharacterApproved += HideFriskUI;
	}

	private void OnFriskPlayer(RAGE.Elements.Player playerBeingFrisked, List<CItemInstanceDef> lstInventory)
	{
		m_lstFriskedPlayerInventoryItems = lstInventory;
		ShowFriskUI(playerBeingFrisked);
	}

	private void ShowFriskUI(RAGE.Elements.Player playerBeingFrisked)
	{
		m_FriskUI = new CGUIFrisk(() => { });

		m_FriskUI.SetVisible(true, true, false);

		// Populate item cache
		int regIndex = 0;
		foreach (var itemKvPair in ItemDefinitions.g_ItemDefinitions)
		{
			CInventoryItemDefinition itemDef = itemKvPair.Value;
			m_FriskUI.RegisterItem(regIndex, itemDef);
			++regIndex;
		}

		m_FriskUI.Initialize(PlayerInventory.g_InventorySlots, PlayerInventory.g_fMaxInventoryWeight);
		m_FriskUI.SetCharacterName(playerBeingFrisked.Name);

		// Populate Inventory
		int itemIndex = 0;
		foreach (var item in m_lstFriskedPlayerInventoryItems)
		{
			string ItemValueString = "N/A";

			// For weapons, show the dbid as the serial
			if (item.IsFirearm())
			{
				ItemValueString = item.DatabaseID.ToString();
			}
			else
			{
				object ItemValue = item.Value;

				// Try to read the key provided by the item def
				string valueKey = ItemDefinitions.g_ItemDefinitions[item.ItemID].ValueKey;
				JObject obj = JObject.FromObject(ItemValue);

				JToken tok;
				if (obj.TryGetValue(valueKey, out tok))
				{
					ItemValueString = tok.ToString();
				}
			}

			m_FriskUI.AddItem(itemIndex, item.ItemID, ItemValueString);
			++itemIndex;
		}
	}

	public void OnFriskTakeItem(int index)
	{
		if (index < m_lstFriskedPlayerInventoryItems.Count)
		{
			CItemInstanceDef itemDef = m_lstFriskedPlayerInventoryItems[index];
			NetworkEventSender.SendNetworkEvent_OnFriskTakeItem(itemDef.DatabaseID);
		}

		HideFriskUI();
	}

	public void OnHideFrisk()
	{
		NetworkEventSender.SendNetworkEvent_OnEndFrisking();
		HideFriskUI();
	}

	private void HideFriskUI()
	{
		if (m_FriskUI != null)
		{
			m_FriskUI.SetVisible(false, false, false);
			m_FriskUI = null;
		}
	}
}