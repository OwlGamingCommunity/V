using GTANetworkAPI;
using Newtonsoft.Json;
using System;
using EntityDatabaseID = System.Int64;

public class FurnitureSystem 
{
	public FurnitureSystem()
	{
		NetworkEvents.EditInterior_PlaceFurniture += OnPlaceFurniture;
		NetworkEvents.EditInterior_PickupFurniture += OnPickupFurniture;
		NetworkEvents.EditInterior_RestoreFurniture += OnRestoreFurniture;
		NetworkEvents.EditInterior_RemoveDefaultFurniture += OnRemoveDefaultFurniture;
		NetworkEvents.EditInterior_CommitChange += OnCommitChange;
		NetworkEvents.RequestEditInterior += OnRequestEditInterior;

		try
		{
			// Populate Furniture Definitions
			PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Deserializing Furniture");

			CFurnitureDefinition[] jsonData = JsonConvert.DeserializeObject<CFurnitureDefinition[]>(System.IO.File.ReadAllText(System.IO.Path.Combine("dotnet", "resources", "owl_gamedata", "FurnitureData.json")));

			foreach (CFurnitureDefinition furnitureDef in jsonData)
			{
				FurnitureDefinitions.g_FurnitureDefinitions.Add(furnitureDef.ID, furnitureDef);
			}

			if (FurnitureDefinitions.g_FurnitureDefinitions.Count != jsonData.Length)
			{
				PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading furniture data: Expected {0} furnitures, got {1} from JSON data.", jsonData.Length, FurnitureDefinitions.g_FurnitureDefinitions.Count);
			}

			PrintLogger.LogMessage(ELogSeverity.MEDIUM, "Deserialized {0}/{1} furnitures.", FurnitureDefinitions.g_FurnitureDefinitions.Count, jsonData.Length);
		}
		catch (Exception ex)
		{
			PrintLogger.LogMessage(ELogSeverity.ERROR, "Fatal error loading furniture data: {0}", ex.ToString());
		}
	}

	private void OnPlaceFurniture(CPlayer a_Player, float x, float y, float z, long dbid)
	{
		CItemInstanceDef itemDef = a_Player.Inventory.GetItemFromDBID(dbid);
		if (itemDef != null)
		{
			if (itemDef.IsFurniture())
			{
				uint furnID = itemDef.GetFurnitureID();
				if (FurnitureDefinitions.g_FurnitureDefinitions.ContainsKey(furnID))
				{
					CFurnitureDefinition furnDef = FurnitureDefinitions.g_FurnitureDefinitions[furnID];

					if (furnDef != null)
					{
						CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);
						if (propInst != null)
						{
							propInst.AddNewFurnitureItem(itemDef, furnID, new Vector3(x, y, z), new Vector3(0.0f, 0.0f, 0.0f), a_Player);

							// resend furniture list
							a_Player.SendFurnitureList();
						}
					}
				}
			}
		}
	}

	private void OnPickupFurniture(CPlayer a_Player, long dbid)
	{
		CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);
		if (propInst != null)
		{
			propInst.RemoveCurrentFurnitureItem(dbid, a_Player, (bool bRemoved) =>
			{
				if (bRemoved)
				{
					// resend furniture list
					a_Player.SendFurnitureList();
				}
				else
				{
					a_Player.SendGenericMessageBox("Pickup Furniture", "This furniture has items stored inside, you cannot pick it up until it is empty.");
				}
			});
		}
	}

	private void OnCommitChange(CPlayer a_Player, float fX, float fY, float fZ, float fRX, float fRY, float fRZ, EntityDatabaseID dbid)
	{
		CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);
		if (propInst != null)
		{
			propInst.CommitFurnitureChange(dbid, new Vector3(fX, fY, fZ), new Vector3(fRX, fRY, fRZ));
		}
	}

	private void OnRemoveDefaultFurniture(CPlayer a_Player, float x, float y, float z, uint model)
	{
		// TODO_FURNITURE: Look for nearby ones already removed (dupes) before transmitting to server
		CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);
		if (propInst != null)
		{
			propInst.AddDefaultFurnitureRemoval(model, new Vector3(x, y, z), a_Player);

			// resend furniture list + removal list
			a_Player.SendFurnitureList();
		}
	}

	private void OnRestoreFurniture(CPlayer a_Player, long dbid)
	{
		CPropertyInstance propInst = PropertyPool.GetPropertyInstanceFromID(a_Player.Client.Dimension);
		if (propInst != null)
		{
			propInst.RestoreDefaultFurnitureRemoval(dbid, a_Player);

			// resend furniture list
			a_Player.SendFurnitureList();
		}
	}

	private void OnRequestEditInterior(CPlayer a_Player)
	{
		a_Player.GotoInteriorEditMode();
	}
}