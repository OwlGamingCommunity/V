using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace Database
{
	namespace Functions
	{
		public static class Characters
		{
			public static void SetTier1PendingLicenseState(EntityDatabaseID DatabaseID, EPendingFirearmLicenseState a_State)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "pending_firearms_lic_state_tier1", (int)a_State }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void SetTier2PendingLicenseState(EntityDatabaseID DatabaseID, EPendingFirearmLicenseState a_State)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "pending_firearms_lic_state_tier2", (int)a_State }

				}, WhereClause.Create("id={0} LIMIT 1", DatabaseID));
			}

			public static void Save(EntityDatabaseID DatabaseID, Vector3 vecPos, float fRotZ, int health, int armor, Dimension dimension)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "x", vecPos.X },
					{ "y", vecPos.Y },
					{ "z", vecPos.Z },
					{ "rz", fRotZ },
					{ "health", health},
					{ "armor", armor },
					{ "last_seen", SqlFieldOperation.Create("CURRENT_TIMESTAMP") },
					{ "dimension", dimension },

				}, WhereClause.Create("id={0}", DatabaseID));
			}

			public static void IsNameUnique(string strName, Action<bool> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Characters, new List<string> { "name" }, WhereClause.Create("LOWER(name)=LOWER('{0}') LIMIT 1", strName), (CMySQLResult mysqlResult) =>
				{
					ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Disallowed_Character_Names, new List<string> { "name" }, WhereClause.Create("LOWER(name)=LOWER('{0}') OR LOWER(name)=LOWER('{1}') LIMIT 1", strName, strName.Replace(' ', '_')), (CMySQLResult mysqlResultDisallowedNames) =>
					{
						CompletionCallback?.Invoke(mysqlResult.NumRows() == 0 && mysqlResultDisallowedNames.NumRows() == 0);
					});
				});
			}

			public static void UpdateMinutesPlayed(EntityDatabaseID CharacterID, uint MinutesToAdd)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "minutes_played", SqlFieldOperation.Create("minutes_played+{0}", MinutesToAdd) }

				}, WhereClause.Create("id={0}", CharacterID));
			}

			public static void SetPaydayProgress(EntityDatabaseID DatabaseID, int a_Progress)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "payday_progress", a_Progress }

				}, WhereClause.Create("id={0}", DatabaseID));
			}

			public static void SaveDrugEffects(EntityDatabaseID DatabaseID, float fImpairment, bool bDrugFX1, bool bDrugFX2, bool bDrugFX3, bool bDrugFX4, bool bDrugFX5,
				Int64 DrugFX1_Duration, Int64 DrugFX2_Duration, Int64 DrugFX3_Duration, Int64 DrugFX4_Duration, Int64 DrugFX5_Duration)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "impairment", fImpairment },
					{ "drug_fx1", bDrugFX1 },
					{ "drug_fx2", bDrugFX2 },
					{ "drug_fx3", bDrugFX3 },
					{ "drug_fx4", bDrugFX4},
					{ "drug_fx5", bDrugFX5 },
					{ "drug_fx1_duration", DrugFX1_Duration },
					{ "drug_fx2_duration", DrugFX2_Duration },
					{ "drug_fx3_duration", DrugFX3_Duration },
					{ "drug_fx4_duration", DrugFX4_Duration },
					{ "drug_fx5_duration", DrugFX5_Duration },

				}, WhereClause.Create("id={0}", DatabaseID));
			}

			public static void GetCharacterNameFromDBID(EntityDatabaseID a_CharacterID, Action<string> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new List<string> { "name" }, WhereClause.Create("id={0} LIMIT 1;", a_CharacterID), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					string strName = "Unknown";
					if (mysqlResult.NumRows() >= 1)
					{
						strName = mysqlResult.GetRow(0)["name"];
					}

					CompletionCallback(strName);
				});
			}

			// TODO_LAUNCH: Check all callers of this function to check that they verify the character exists
			public static void GetCharacterDBIDFromName(string strName, Action<EntityDatabaseID> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new List<string> { "id" }, WhereClause.Create("LOWER(name)=LOWER('{0}') LIMIT 1;", strName), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					EntityDatabaseID databaseID = -1;
					if (mysqlResult.NumRows() >= 1)
					{
						databaseID = (EntityDatabaseID)Convert.ChangeType(mysqlResult.GetRow(0)["id"], typeof(EntityDatabaseID));
					}

					CompletionCallback(databaseID);
				});
			}

			public static void GetAccountIdFromCharacterId(EntityDatabaseID characterID, Action<EntityDatabaseID> CompletionCallback)
			{
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new List<string> { "account" }, WhereClause.Create("id={0} LIMIT 1;", characterID), (CMySQLResult mysqlResult) => // NOTE: on main thread at this point
				{
					EntityDatabaseID databaseID = -1;
					if (mysqlResult.NumRows() >= 1)
					{
						databaseID = (EntityDatabaseID)Convert.ChangeType(mysqlResult.GetRow(0)["account"], typeof(EntityDatabaseID));
					}

					CompletionCallback(databaseID);
				});
			}

			public static void GrantCharacterBankMoney(EntityDatabaseID characterID, float amount)
			{
				ThreadedMySQL.Query_UPDATE(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Characters, new Dictionary<string, object>
				{
					{ "bank_money", SqlFieldOperation.Create("bank_money+{0}", amount) }

				}, WhereClause.Create("id={0}", characterID));
			}

			public static void AddLanguage(EntityDatabaseID characterID, ECharacterLanguage languageID, float fProgress, bool bActive, Action<ulong> CompletionCallback = null)
			{
				ThreadedMySQL.Query_INSERT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Character_Languages, new Dictionary<string, object>
				{
					{"language_id", languageID },
					{"progress", fProgress },
					{"parent", characterID },
					{"active", bActive },
				}, (CMySQLResult result) => { CompletionCallback?.Invoke(result.GetInsertID()); });
			}

			// NOTE: AccountID is -1 when bFullData is false normally, this function is used to retrieve character info for REMOTE players, so you shouldn't load account specific things for remote players
			public static void Get(EntityDatabaseID AccountID, EntityDatabaseID CharacterID, bool bFullData, Action<SGetCharacter> CompletionCallback)
			{
				// NOTE: Full data false will not load the entire inventory, just clothing items
				ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Characters, new List<string> { "id", "name", "type", "x", "y", "z", "rz", "health", "armor", "money", "bank_money", "pending_job_money", "job", "trucker_job_xp", "deliverydriver_job_xp", "busdriver_job_xp", "mailman_job_xp", "trashman_job_xp", "fishing_xp", "dimension", "payday_progress", "gender", "cuffed", "cuffer", "unjail_time", "cell_number", "bail_amount", "jail_reason", "duty", "cked", "impairment", "drug_fx1", "drug_fx2", "drug_fx3", "drug_fx4", "drug_fx5", "drug_fx1_duration", "drug_fx2_duration", "drug_fx3_duration", "drug_fx4_duration", "drug_fx5_duration", "first_use", "minutes_played", "pending_firearms_lic_state_tier1", "pending_firearms_lic_state_tier2", "IFNULL(tag", "'')", "IFNULL(tag_wip", "'')", "show_spawnselector", "creation_version", "current_version", "premade_masked", "age" },
					WhereClause.Create("id={0}", CharacterID), (CMySQLResult characterRows) => // NOTE: on main thread at this point
					{
						// Character data
						if (characterRows.NumRows() == 1)
						{
							CMySQLRow row = characterRows.GetRow(0);

							SGetCharacter returnValue = new SGetCharacter
							{
								id = (EntityDatabaseID)Convert.ChangeType(row["id"], typeof(EntityDatabaseID)),
								pos = new Vector3(float.Parse(row["x"]), float.Parse(row["y"]), float.Parse(row["z"])),
								rz = float.Parse(row["rz"]),
								health = int.Parse(row["health"]),
								armor = int.Parse(row["armor"]),
								money = float.Parse(row["money"]),
								bank_money = float.Parse(row["bank_money"]),
								pending_job_money = float.Parse(row["pending_job_money"]),
								CharacterName = row["name"],
								Job = (EJobID)Convert.ToInt32(row["job"]),
								TruckerJobXP = Convert.ToInt32(row["trucker_job_xp"]),
								DeliveryDriverJobXP = Convert.ToInt32(row["deliverydriver_job_xp"]),
								BusDriverJobXP = Convert.ToInt32(row["busdriver_job_xp"]),
								MailmanJobXP = Convert.ToInt32(row["mailman_job_xp"]),
								TrashmanJobXP = Convert.ToInt32(row["trashman_job_xp"]),
								FishingXP = Convert.ToInt32(row["fishing_xp"]),
								Dimension = (Dimension)Convert.ChangeType(row["dimension"], typeof(Dimension)),
								payday_progress = Convert.ToInt32(row["payday_progress"]),
								Gender = (EGender)Convert.ToInt32(row["gender"]),
								Cuffed = bool.Parse(row["cuffed"]),
								Cuffer = (EntityDatabaseID)Convert.ChangeType(row["cuffer"], typeof(EntityDatabaseID)),
								UnjailTime = Convert.ToInt64(row["unjail_time"]),
								CellNumber = (EPrisonCell)Convert.ToInt32(row["cell_number"]),
								BailAmount = float.Parse(row["bail_amount"]),
								JailReason = row["jail_reason"],
								Duty = (EDutyType)Convert.ToInt32(row["duty"]),
								CKed = bool.Parse(row["cked"]),
								CharacterType = (ECharacterType)Convert.ToInt32(row["type"]),
								Impairment = float.Parse(row["impairment"]),
								DrugFX1 = bool.Parse(row["drug_fx1"]),
								DrugFX2 = bool.Parse(row["drug_fx2"]),
								DrugFX3 = bool.Parse(row["drug_fx3"]),
								DrugFX4 = bool.Parse(row["drug_fx4"]),
								DrugFX5 = bool.Parse(row["drug_fx5"]),
								DrugFX1_Duration = Convert.ToInt64(row["drug_fx1_duration"]),
								DrugFX2_Duration = Convert.ToInt64(row["drug_fx2_duration"]),
								DrugFX3_Duration = Convert.ToInt64(row["drug_fx3_duration"]),
								DrugFX4_Duration = Convert.ToInt64(row["drug_fx4_duration"]),
								DrugFX5_Duration = Convert.ToInt64(row["drug_fx5_duration"]),
								FirstUse = bool.Parse(row["first_use"]),
								minutesPlayed = Convert.ToUInt32(row["minutes_played"]),
								pendingFirearmsLicenseStateTier1 = (EPendingFirearmLicenseState)Convert.ToInt32(row["pending_firearms_lic_state_tier1"]),
								pendingFirearmsLicenseStateTier2 = (EPendingFirearmLicenseState)Convert.ToInt32(row["pending_firearms_lic_state_tier2"]),
								ShowSpawnSelector = bool.Parse(row["show_spawnselector"]),
								CreationVersion = (ECharacterVersions)Convert.ToInt32(row["creation_version"]),
								CurrentVersion = (ECharacterVersions)Convert.ToInt32(row["current_version"]),
								PremadeMasked = bool.Parse(row["premade_masked"]),
								Age = Convert.ToUInt32(row["age"]),
							};

							string strTagData = row["IFNULL(tag, '')"];
							if (strTagData.Length > 0)
							{
								returnValue.gangTags = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GangTagLayer>>(strTagData);
							}

							string strTagDataWIP = row["IFNULL(tag_wip, '')"];
							if (strTagDataWIP.Length > 0)
							{
								returnValue.gangTagsWIP = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GangTagLayer>>(strTagDataWIP);
							}

							if (bFullData)
							{
								// GET LANGUAGES
								ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Character_Languages, new List<string> { "language_id", "progress", "active" },
								WhereClause.Create("parent={0}", CharacterID), (CMySQLResult CharacterLanguageRows) => // NOTE: on main thread at this point
								{
									if (CharacterLanguageRows.GetRows().Count != 0)
									{
										// Get character languages
										// populate return value
										foreach (CMySQLRow characterLanguageRow in CharacterLanguageRows.GetRows())
										{
											returnValue.CharacterLanguages.Add(new CDatabaseStructureCharacterLanguage(characterLanguageRow));
										}
									}
									else
									{
										//If they haven't played since this update they would not have any languages. So we give them english for free :)
										AddLanguage(CharacterID, ECharacterLanguage.English, 100f, true, (ulong insertID) =>
											{
												//TODO_LANGUAGES: Better way to do this? Maybe mass query for everyone instead of having another query here
												ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Character_Languages, new List<string> { "language_id", "progress", "active" },
														WhereClause.Create("parent={0}", CharacterID), (CMySQLResult charLangRows) => // NOTE: on main thread at this point
														{
															returnValue.CharacterLanguages.Add(new CDatabaseStructureCharacterLanguage(charLangRows.GetRows().First()));
														});
											});
									}

									// Get inventory
									Dictionary<EntityDatabaseID, List<CItemInstanceDef>> dictAllContainerItems = new Dictionary<EntityDatabaseID, List<CItemInstanceDef>>();
									ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Inventories, new List<string> { "id", "item_id", "item_value", "current_socket", "parent", "parent_type", "stack_size" }, WhereClause.Create("parent_type={0}", (int)EItemParentTypes.Container), (CMySQLResult inventoryResult) => // NOTE: on main thread at this point
									{
										// store container items
										foreach (CMySQLRow itemRow in inventoryResult.GetRows())
										{
											EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
											EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
											string itemValue = itemRow["item_value"];
											EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
											EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
											EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
											UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

											if (!dictAllContainerItems.ContainsKey(parentDatabaseID))
											{
												dictAllContainerItems[parentDatabaseID] = new List<CItemInstanceDef>();
											}

											dictAllContainerItems[parentDatabaseID].Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
										}

										ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Inventories, new List<string> { "id", "item_id", "item_value", "current_socket", "parent", "parent_type", "stack_size" }, WhereClause.Create("parent_type={0} AND parent={1}", (int)EItemParentTypes.Player, CharacterID), (CMySQLResult inventoryResult) =>
										{
											// Get inventory
											foreach (CMySQLRow itemRow in inventoryResult.GetRows())
											{
												EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
												EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
												string itemValue = itemRow["item_value"];
												EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
												EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
												EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
												UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

												returnValue.Inventory.Add(CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize));
											}

											// now hook up our container items
											List<CItemInstanceDef> lstContainerItems = new List<CItemInstanceDef>();
											foreach (CItemInstanceDef inventoryItem in returnValue.Inventory)
											{
												lstContainerItems.AddRange(Database.Functions.Items.ResolveContainerItemsFromItemListRecursively(inventoryItem.DatabaseID, dictAllContainerItems));
											}
											returnValue.Inventory.AddRange(lstContainerItems);

											// now load factions
											ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnQueryThread, TableNames.Faction_Memberships, new List<string> { "faction_id", "rank_index", "manager" }, WhereClause.Create("character_id={0}", CharacterID), (CMySQLResult factionMembershipsResult) =>
												{
													// Get faction memberships
													// populate return value
													foreach (CMySQLRow factionMembershipRow in factionMembershipsResult.GetRows())
													{
														returnValue.FactionMemberships.Add(new CDatabaseStructureFactionMembership(factionMembershipRow));
													}

													// Load binds
													Database.Functions.Util.GetAccountAndCharacterKeybinds(AccountID, CharacterID, (List<PlayerKeybindObject> lstKeybinds) =>
														{
															returnValue.Keybinds.AddRange(lstKeybinds);

															// we are done now (and back on the main thread )
															CompletionCallback?.Invoke(returnValue);
														});
												});
										});
									});
								});
							}
							else
							{
								// Get clothing inventory
								ThreadedMySQL.Query_SELECT(EDatabase.Game, EThreadContinuationFlag.ContinueOnMainThread, TableNames.Inventories, new List<string> { "id", "item_id", "item_value", "current_socket", "parent", "parent_type", "stack_size" }, WhereClause.Create("parent_type={0} AND parent={1}", (int)EItemParentTypes.Player, CharacterID), (CMySQLResult inventoryResult) =>
									{
										foreach (CMySQLRow itemRow in inventoryResult.GetRows())
										{
											EntityDatabaseID databaseID = itemRow.GetValue<EntityDatabaseID>("id");
											EItemID itemID = (EItemID)Convert.ToInt32(itemRow["item_id"]);
											string itemValue = itemRow["item_value"];
											EItemSocket currentSocket = (EItemSocket)Convert.ToInt32(itemRow["current_socket"]);
											EItemParentTypes parentType = (EItemParentTypes)Convert.ToInt32(itemRow["parent_type"]);
											EntityDatabaseID parentDatabaseID = itemRow.GetValue<EntityDatabaseID>("parent");
											UInt32 StackSize = itemRow.GetValue<UInt32>("stack_size");

											CItemInstanceDef itemDef = CItemInstanceDef.FromJSONString(databaseID, itemID, itemValue, currentSocket, parentDatabaseID, parentType, StackSize);

											if (itemDef.ItemID == EItemID.CLOTHES)
											{
												CItemValueClothingPremade clothingValue = (CItemValueClothingPremade)itemDef.Value;
												if (clothingValue.IsActive)
												{
													returnValue.Inventory.Add(itemDef);
												}
											}
											else if (itemDef.ItemID == EItemID.DUTY_OUTFIT)
											{
												CItemValueDutyOutfit dutyOutfitValue = itemDef.GetValueData<CItemValueDutyOutfit>();
												if (dutyOutfitValue.IsActive)
												{
													returnValue.Inventory.Add(itemDef);
												}
											}
											else if (itemDef.ItemID == EItemID.OUTFIT)
											{
												CItemValueOutfit outfitValue = itemDef.GetValueData<CItemValueOutfit>();
												if (outfitValue.IsActive)
												{
													returnValue.Inventory.Add(itemDef);
												}
											}
											else if ((itemDef.ItemID >= EItemID.CLOTHES_CUSTOM_FACE && itemDef.ItemID <= EItemID.CLOTHES_CUSTOM_TOPS) || ItemHelpers.IsItemIDAProp(itemDef.ItemID))
											{
												CItemValueClothingCustom clothingValue = (CItemValueClothingCustom)itemDef.Value;
												if (clothingValue.IsActive)
												{
													returnValue.Inventory.Add(itemDef);
												}
											}
										}

										// we are done
										CompletionCallback?.Invoke(returnValue);
									});
							}
						}
						else
						{
							// TODO: Error
						}
					});
			}
		}
	}
}
