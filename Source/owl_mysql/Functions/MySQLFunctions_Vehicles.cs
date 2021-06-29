using GTANetworkAPI;
using System;
using System.Collections.Generic;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Vehicles
		{
			private static void LoadVehicleMods(EntityDatabaseID a_vehicleID, Action<Dictionary<EModSlot, int>> Callback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicle_Mods, new List<string> { "category", "mod_index" }, WhereClause.Create("vehicle={0}", a_vehicleID), (CMySQLResult result) => // NOTE: on main thread at this point
				{
					Dictionary<EModSlot, int> dictMods = new Dictionary<EModSlot, int>();

					foreach (CMySQLRow row in result.GetRows())
					{
						EModSlot slot = (EModSlot)row.GetValue<int>("category");
						int mod_index = row.GetValue<int>("mod_index");
						dictMods[slot] = mod_index;
					}

					Callback(dictMods);
				});
			}

			public static void IsPlateUnique(string strPlateText, Action<bool> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicles, new List<string> { "plate_text" }, WhereClause.Create("LOWER(plate_text)=LOWER('{0}') LIMIT 1;", strPlateText), (CMySQLResult mysqlResult) =>
				{
					CompletionCallback?.Invoke(mysqlResult.NumRows() == 0);
				});
			}

			public static void Park(EntityDatabaseID DatabaseID, Vector3 vecPos, Vector3 vecRot, Dimension dimension)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicles, new Dictionary<string, object>
				{
					{ "spawn_x", vecPos.X },
					{ "spawn_y", vecPos.Y },
					{ "spawn_z", vecPos.Z },
					{ "spawn_rx", vecRot.X },
					{ "spawn_ry", vecRot.Y },
					{ "spawn_rz", vecRot.Z },
					{ "dimension", dimension }

				}, WhereClause.Create("id={0}", DatabaseID));
			}

			public static void Save(EntityDatabaseID DatabaseID, Vector3 vecPos, Vector3 vecRot, float fFuel, float fDirt, float fHealth, bool a_bLocked, bool a_bEngineOn, float fOdometer, Dimension a_Dimension)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicles, new Dictionary<string, object>
				{
					{ "x", vecPos.X },
					{ "y", vecPos.Y },
					{ "z", vecPos.Z },
					{ "rx", vecRot.X },
					{ "ry", vecRot.Y },
					{ "rz", vecRot.Z },
					{ "fuel", fFuel },
					{ "dirt", fDirt },
					{ "health", fHealth},
					{ "locked", a_bLocked },
					{ "engine", a_bEngineOn },
					{ "odometer", fOdometer },
					{ "dimension", a_Dimension },

				}, WhereClause.Create("id={0}", DatabaseID));
			}

			public static void SetSpecialColor(EntityDatabaseID databaseID, int pearlescentColor)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicles, new Dictionary<string, object>
				{
					{"pearlescent_color", pearlescentColor}
				}, WhereClause.Create("id={0}", databaseID));
			}

			public static void LoadAllVehicles(Action<List<CDatabaseStructureVehicle>> Callback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Vehicles, new List<string> { "*" }, null, (CMySQLResult vehiclesResult) => // NOTE: on main thread at this point
				{
					List<CDatabaseStructureVehicle> lstVehicles = new List<CDatabaseStructureVehicle>();

					int NumVehiclesExpected = vehiclesResult.NumRows();
					int NumVehiclesLoaded = 0;

					Dictionary<EntityDatabaseID, List<CItemInstanceDef>> dictAllContainerItems = new Dictionary<EntityDatabaseID, List<CItemInstanceDef>>();
					Dictionary<EntityDatabaseID, List<CItemInstanceDef>> dictAllVehicleItems = new Dictionary<EntityDatabaseID, List<CItemInstanceDef>>();
					ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Inventories, new List<string> { "id", "item_id", "item_value", "current_socket", "parent", "parent_type", "stack_size" }, WhereClause.Create("parent_type={0} OR parent_type={1}", (int)EItemParentTypes.Container, (int)EItemParentTypes.Vehicle), (CMySQLResult inventoryResult) => // NOTE: on main thread at this point
					{
						// store container items and vehicle inventories
						foreach (CMySQLRow itemRow in inventoryResult.GetRows())
						{
							EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
							EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
							string itemValue = itemRow["item_value"];
							EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
							EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
							EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
							UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

							if (parentType == EItemParentTypes.Container)
							{
								if (!dictAllContainerItems.ContainsKey(parentDatabaseID))
								{
									dictAllContainerItems[parentDatabaseID] = new List<CItemInstanceDef>();
								}

								dictAllContainerItems[parentDatabaseID].Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
							}
							else if (parentType == EItemParentTypes.Vehicle)
							{
								if (!dictAllVehicleItems.ContainsKey(parentDatabaseID))
								{
									dictAllVehicleItems[parentDatabaseID] = new List<CItemInstanceDef>();
								}

								dictAllVehicleItems[parentDatabaseID].Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
							}
						}

						// load all mods (more optimal)
						ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicle_Mods, new List<string> { "vehicle", "category", "mod_index" }, null, (CMySQLResult modsResult) => // NOTE: on main thread at this point
						{
							Dictionary<EntityDatabaseID, Dictionary<EModSlot, int>> dictAllVehicleMods = new Dictionary<EntityDatabaseID, Dictionary<EModSlot, int>>();

							foreach (CMySQLRow row in modsResult.GetRows())
							{
								EntityDatabaseID modVehicleID = row.GetValue<EntityDatabaseID>("vehicle");

								if (!dictAllVehicleMods.ContainsKey(modVehicleID))
								{
									dictAllVehicleMods[modVehicleID] = new Dictionary<EModSlot, int>();
								}

								EModSlot slot = (EModSlot)row.GetValue<int>("category");
								int mod_index = row.GetValue<int>("mod_index");

								dictAllVehicleMods[modVehicleID].Add(slot, mod_index);
							}

							// load all extras (more optimal)
							ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Vehicle_Extras, new List<string> { "vehicle_id", "extra", "enabled" }, null, (CMySQLResult extrasResult) => // NOTE: on main thread at this point
							{
								Dictionary<EntityDatabaseID, Dictionary<int, bool>> dictAllVehicleExtras = new Dictionary<EntityDatabaseID, Dictionary<int, bool>>();

								foreach (CMySQLRow row in extrasResult.GetRows())
								{
									EntityDatabaseID extraVehicleID = row.GetValue<EntityDatabaseID>("vehicle_id");

									if (!dictAllVehicleExtras.ContainsKey(extraVehicleID))
									{
										dictAllVehicleExtras[extraVehicleID] = new Dictionary<int, bool>();
									}

									int extraID = row.GetValue<int>("extra");
									bool bEnabled = row.GetValue<bool>("enabled");

									dictAllVehicleExtras[extraVehicleID].Add(extraID, bEnabled);
								}

								// now load all vehicles
								foreach (CMySQLRow row in vehiclesResult.GetRows())
								{
									CDatabaseStructureVehicle vehicle = new CDatabaseStructureVehicle(row);

									// vehicle mods
									if (dictAllVehicleMods.ContainsKey(vehicle.vehicleID))
									{
										vehicle.VehicleMods = dictAllVehicleMods[vehicle.vehicleID];
									}
									else
									{
										vehicle.VehicleMods = new Dictionary<EModSlot, int>();
									}

									// vehicle extras
									if (dictAllVehicleExtras.ContainsKey(vehicle.vehicleID))
									{
										vehicle.VehicleExtras = dictAllVehicleExtras[vehicle.vehicleID];
									}
									else
									{
										vehicle.VehicleExtras = new Dictionary<int, bool>();
									}

									// hook up inventory
									List<CItemInstanceDef> lstInventory = new List<CItemInstanceDef>();
									if (dictAllVehicleItems.ContainsKey(vehicle.vehicleID))
									{
										lstInventory = dictAllVehicleItems[vehicle.vehicleID];
									}

									// now hook up our container items
									List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
									foreach (CItemInstanceDef inventoryItem in lstInventory)
									{
										lstContainerItems.AddRange(Database.Functions.Items.ResolveContainerItemsFromItemListRecursively(inventoryItem.DatabaseID, dictAllContainerItems));
									}
									lstInventory.AddRange(lstContainerItems);

									vehicle.CopyInventory(lstInventory);

									lstVehicles.Add(vehicle);

									++NumVehiclesLoaded;

									// are we done?
									if (NumVehiclesLoaded == NumVehiclesExpected)
									{
										Callback(lstVehicles);
									}
								}
							});
						});
					});
				});
			}
		}
	}
}