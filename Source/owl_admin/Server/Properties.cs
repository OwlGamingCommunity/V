using Database.Models;
using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using Dimension = System.UInt32;
using EntityDatabaseID = System.Int64;

namespace PlayerAdminCommands
{
	public class Properties
	{
		public const uint NEARBY_PROPERTIES_DISTANCE = 20;
		public const uint NEARBY_ELEVATORS_DISTANCE = 20;

		public Properties()
		{
			// COMMANDS
			CommandManager.RegisterCommand("gotoint", "Teleport to a property.", new Action<CPlayer, CVehicle, EntityDatabaseID>(GotoProperty), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "gotoprop", "gotohouse" });
			CommandManager.RegisterCommand("gotointi", "Teleport to a property interior.", new Action<CPlayer, CVehicle, EntityDatabaseID>(GotoPropertyInterior), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "gotopropi", "gotohousei" });
			CommandManager.RegisterCommand("setinttype", "Set the property type.", new Action<CPlayer, CVehicle, EntityDatabaseID, EPropertyType, EPropertyEntranceType>(SetType), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setproptype" });
			CommandManager.RegisterCommand("setintprice", "Set the property price.", new Action<CPlayer, CVehicle, EntityDatabaseID, float>(SetPrice), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setintcost", "setpropprice", "setpropcost" });
			CommandManager.RegisterCommand("setint", "Set the property interior ID.", new Action<CPlayer, CVehicle, EntityDatabaseID, int>(SetInteriorId), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setinteriorid", "setintid" });
			CommandManager.RegisterCommand("setthisint", "Set the property interior ID.", new Action<CPlayer, CVehicle, int>(SetThisInteriorId), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("setintname", "Set the property name.", new Action<CPlayer, CVehicle, EntityDatabaseID, string>(SetName), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setpropname" });
			CommandManager.RegisterCommand("setpropowner", "Set the property owner.", new Action<CPlayer, CVehicle, EntityDatabaseID, EPropertyOwnerType, string>(SetOwner), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setintowner" });
			CommandManager.RegisterCommand("setproprenter", "Set the property renter.", new Action<CPlayer, CVehicle, EntityDatabaseID, EPropertyOwnerType, string>(SetRenter), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setintrenter" });
			CommandManager.RegisterCommand("reloadprop", "Reload the property.", new Action<CPlayer, CVehicle, EntityDatabaseID>(ReloadProperty), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "reloadint" });
			CommandManager.RegisterCommand("setintentrance", "Sets the position of the Property entrance.", new Action<CPlayer, CVehicle, EntityDatabaseID>(MoveEntrance), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setpropentrance" });
			CommandManager.RegisterCommand("setintexit", "Sets the position of the Property exit.", new Action<CPlayer, CVehicle>(MoveExit), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "setpropexit" });
			CommandManager.RegisterCommand("makeint", "Create a new property.", new Action<CPlayer, CVehicle, float, int, bool, string>(CreateProperty), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "makeprop", "addint" });
			CommandManager.RegisterCommand("delint", "Deletes a property.", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteProperty), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "delprop" });
			CommandManager.RegisterCommand("forceremoverenter", "Remove the renter from the property.", new Action<CPlayer, CVehicle, EntityDatabaseID>(RemoveRenter), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "delprop" });
			CommandManager.RegisterCommand("forcesell", "Removes the owner of a property.", new Action<CPlayer, CVehicle, EntityDatabaseID>(ForceSell), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "repossess" });
			CommandManager.RegisterCommand("nearbyproperties", "Shows all nearby properties.", new Action<CPlayer, CVehicle>(NearbyPropertiesCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "nearbyint", "nearbyinteriors", "nearbyints", "nearbyproperty", "nearbyprops" });

			CommandManager.RegisterCommand("thisint", "Gets current interior details", new Action<CPlayer, CVehicle>(ThisInterior), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "thisinterior", "thisprop", "thisproperty" });

			CommandManager.RegisterCommand("makepropkey", "Gives a property key to the target player.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID>(MakePropertyKey), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("changeproplock", "Changes a property lock and gives it to the target player.", new Action<CPlayer, CVehicle, CPlayer, EntityDatabaseID>(ChangePropertyLock), CommandParsingFlags.TargetPlayer, CommandRequirementsFlags.MustBeAdminOnDuty);

			CommandManager.RegisterCommand("adde", "Create a new elevator.", new Action<CPlayer, CVehicle, int, string>(CreateElevator), CommandParsingFlags.TargetFunctionAndGreedyArgsRemainder, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("dele", "Deletes an elevator.", new Action<CPlayer, CVehicle, EntityDatabaseID>(DeleteElevator), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty);
			CommandManager.RegisterCommand("nearbye", "Shows all nearby properties.", new Action<CPlayer, CVehicle>(NearbyElevatorsCommand), CommandParsingFlags.Default, CommandRequirementsFlags.MustBeAdminOnDuty, aliases: new string[] { "nearbyelevators" });
			CommandManager.RegisterCommand("toexit", "Teleport to the interior exit.", new Action<CPlayer, CVehicle>(ToExitCommand), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "antifall", "falling", "lifealert" });

			NetworkEvents.AdminDeleteProperty += OnConfirmPropertyDelete;
			NetworkEvents.AdminDeleteElevator += OnConfirmElevatorDelete;
			NetworkEvents.Property_MowedLawn += OnMowedLawn;
		}

		private async void ChangePropertyLock(CPlayer SourcePlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EntityDatabaseID PropertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(PropertyID);
			if (Property != null)
			{
				// Delete old keys
				CItemInstanceDef oldKeyDefinition = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, PropertyID);
				await HelperFunctions.Items.DeleteAllItems(oldKeyDefinition).ConfigureAwait(true);

				// New key
				CItemInstanceDef newKeyDefinition = CItemInstanceDef.FromDefaultValueWithStackSize(EItemID.PROPERTY_KEY, PropertyID, 1);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;


				if (TargetPlayer.Inventory.CanGiveItem(newKeyDefinition, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(newKeyDefinition, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received a property key for property '{1}' ({2})", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you a property key for property '{2}' ({3})", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive property key '{1}' ({2}): Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
						}
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive property key '{1}' ({2}): {3}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID, strHumanReadableError);
				}

			}
		}

		private void MakePropertyKey(CPlayer SourcePlayer, CVehicle SenderVehicle, CPlayer TargetPlayer, EntityDatabaseID PropertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(PropertyID);
			if (Property != null)
			{
				CItemInstanceDef itemDef = CItemInstanceDef.FromDefaultValueWithStackSize(EItemID.PROPERTY_KEY, PropertyID, 1);

				List<EItemGiveError> lstErrors = new List<EItemGiveError>();
				string strHumanReadableError = String.Empty;

				if (TargetPlayer.Inventory.CanGiveItem(itemDef, out lstErrors, out strHumanReadableError))
				{
					TargetPlayer.Inventory.AddItemToNextFreeSuitableSlot(itemDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0}' has received a property key for property '{1}' ({2})", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
							TargetPlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "'{0} {1}' has granted you a property key for property '{2}' ({3})", SourcePlayer.AdminTitle, SourcePlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive property key '{1}' ({2}): Could not find a suitable socket.", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID);
						}
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "ERROR: '{0}' cannot receive property key '{1}' ({2}): {3}", TargetPlayer.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Name, PropertyID, strHumanReadableError);
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, " No property was found with id {0}", PropertyID);
			}
		}

		private void ThisInterior(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			Dimension propertyID = SourcePlayer.Client.Dimension;
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, " You are not in a property.");
				return;
			}

			ShowInteriorDetails(SourcePlayer, Property);
		}

		private void ShowInteriorDetails(CPlayer player, CPropertyInstance property)
		{
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Current Property Details:");

			string strOwnerText = property.GetOwnerText();
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - Property ID: {0}", property.Model.Id);
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - Interior list ID: {0}", property.Model.InteriorId);
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - State: {0}", property.Model.State.ToString());
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - Owner: {0}", strOwnerText.Length == 0 ? "None" : strOwnerText);
			player.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - Value: ${0:0.00}", property.Model.BuyPrice);
		}

		private void NearbyPropertiesCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Properties:");
			foreach (CPropertyInstance property in PropertyPool.GetAllPropertyInstances())
			{
				if (SourcePlayer.Client.Dimension == property.Model.EntranceDimension)
				{
					if (SourcePlayer.Client.Position.DistanceTo2D(property.Model.EntrancePosition) <= NEARBY_PROPERTIES_DISTANCE || SourcePlayer.Client.Position.DistanceTo2D(property.Model.ExitPosition) <= NEARBY_PROPERTIES_DISTANCE)
					{
						string strOwnerText = property.GetOwnerText();
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - State: {1} - Owned by: {2}", property.Model.Id, property.Model.State.ToString(), strOwnerText.Length == 0 ? "No one" : strOwnerText);
					}
				}
			}
		}

		private void GotoProperty(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				SourcePlayer.SetPositionSafe(Property.Model.EntrancePosition);
				SourcePlayer.SetSafeDimension(Property.Model.EntranceDimension);
				SourcePlayer.PushChatMessage(EChatChannel.AdminActions, "You have teleported to the entrance of {0}.", Property.Model.Name);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		// Teleport to the exit marker, AKA inside the property.
		private void GotoPropertyInterior(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				SourcePlayer.OnEnterProperty(Property);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have teleported to the exit of {0}.", Property.Model.Name);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		private void SetType(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID, EPropertyType type, EPropertyEntranceType entranceType)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				return;
			}

			bool isCurrentlyOwned = Property.Model.State == EPropertyState.Owned || Property.Model.State == EPropertyState.Owned_AlwaysEnterable;
			bool isCurrentlyRented = Property.Model.State == EPropertyState.Rented || Property.Model.State == EPropertyState.Rented_AlwaysEnterable;
			bool isCurrentlyOwnedOrRented = isCurrentlyOwned || isCurrentlyRented;
			EPropertyState newState = Property.Model.State;

			if (type == EPropertyType.Owned)
			{
				if (entranceType == EPropertyEntranceType.Normal)
				{
					newState = isCurrentlyOwnedOrRented ? EPropertyState.Owned : EPropertyState.AvailableToBuy;
				}
				else
				{
					newState = isCurrentlyOwnedOrRented ? EPropertyState.Owned_AlwaysEnterable : EPropertyState.AvailableToBuy_AlwaysEnterable;
				}
			}

			if (type == EPropertyType.Rented)
			{
				if (entranceType == EPropertyEntranceType.Normal)
				{
					newState = isCurrentlyOwnedOrRented ? EPropertyState.Rented : EPropertyState.AvailableToRent;
				}
				else
				{
					newState = isCurrentlyOwnedOrRented ? EPropertyState.Rented_AlwaysEnterable : EPropertyState.AvailableToRent_AlwaysEnterable;

				}
			}

			if (newState == Property.Model.State)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "The property is already in that state!", Property.Model.State);
				return;
			}

			Property.Model.SetState(newState);
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the property type to {0}.", Property.Model.State);
		}

		private void SetPrice(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID, float newPrice)
		{
			if (newPrice >= 0)
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
				if (Property != null)
				{
					if (Property.Model.State == EPropertyState.AvailableToBuy || Property.Model.State == EPropertyState.Owned || Property.Model.State == EPropertyState.Owned_AlwaysEnterable || Property.Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable)
					{
						Property.SetBuyPrice(newPrice);
					}
					else
					{
						Property.SetRentPrice(newPrice);
					}

					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have changed the property {0} {1} price to {2}.",
						Property.Model.Name,
						Property.Model.State == EPropertyState.AvailableToBuy || Property.Model.State == EPropertyState.AvailableToBuy_AlwaysEnterable || Property.Model.State == EPropertyState.Owned || Property.Model.State == EPropertyState.Owned_AlwaysEnterable ? "purchase" : "rent",
						newPrice);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Property ID] [Price]");
			}
		}

		private void SetName(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID PropertyID, string PropertyName)
		{
			if (PropertyID > 0 && PropertyName.Length > 0)
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(PropertyID);
				if (Property != null)
				{
					if (PropertyName.Length > 48)
					{
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "The property name is too long. Please use <= 48 characters.");
					}
					else
					{
						string oldName = Property.Model.Name;
						Property.SetName(PropertyName);
						SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have changed the name of property {0} from '{1}' to '{2}'.", PropertyID, oldName, Property.Model.Name);
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Property ID] [Name]");
			}
		}

		private async void SetOwner(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID, EPropertyOwnerType newOwnerType, string NewOwnerNameOrFactionID)
		{
			if (propertyID != 0 && (newOwnerType == EPropertyOwnerType.Player || newOwnerType == EPropertyOwnerType.Faction))
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
				if (Property != null)
				{
					if (newOwnerType == EPropertyOwnerType.Player)
					{
						// Then we know we want to get the player from the partial information
						WeakReference<CPlayer> newOwnerRef = CommandManager.FindTargetPlayer(SourcePlayer, NewOwnerNameOrFactionID);
						CPlayer newOwner = newOwnerRef.Instance();

						if (newOwner != null)
						{
							CItemInstanceDef ItemInstanceDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Property.Model.Id);
							if (newOwner.Inventory.CanGiveItem(ItemInstanceDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
							{
								await HelperFunctions.Items.DeleteAllItems(ItemInstanceDef).ConfigureAwait(true);

								newOwner.Inventory.AddItemToNextFreeSuitableSlot(ItemInstanceDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
								{
									if (bItemGranted)
									{
										Property.SetOwner(newOwnerType, newOwner.ActiveCharacterDatabaseID);
										SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the owner of {0} (#{2}) to {1}.", Property.Model.Name, newOwner.GetCharacterName(ENameType.StaticCharacterName), Property.Model.Id);
										newOwner.SendNotification("Realtor", ENotificationIcon.InfoSign, "{0} {1} has set you to be the owner of {2}", SourcePlayer.AdminTitle, SourcePlayer.Username, Property.Model.Name);
									}
									else
									{
										SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player does not have enough space for a key!");
									}
								});
							}
							else
							{
								SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "{0} could not receive the key:<br>{1}", newOwner.GetCharacterName(ENameType.StaticCharacterName), strUserFriendlyMessage);
							}
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player not found.");
						}
					}
					else
					{
						// Otherwise we just want to use it as an integer
						EntityDatabaseID newOwner = 0;
						CFaction Faction = null;

						try
						{
							newOwner = EntityDatabaseID.Parse(NewOwnerNameOrFactionID);
							Faction = FactionPool.GetFactionFromID(newOwner);
						}
						catch
						{
						}

						if (Faction != null)
						{
							Property.SetOwner(newOwnerType, Faction.FactionID);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the owner of a {0} to {1}.", Property.Model.Name, Faction.Name);
						}
						else
						{
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Faction ID.");
						}
					}
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				}
			}
		}

		private void SetRenter(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID, EPropertyOwnerType newRenterType, string NewRenterNameOrFactionID)
		{
			if (propertyID < 1)
			{
				return;
			}

			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				return;
			}

			if (!Property.Model.CanBeRented())
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "This property cannot have a renter.");
				return;
			}

			if (newRenterType == EPropertyOwnerType.Player)
			{
				// Then we know we want to get the player from the partial information
				WeakReference<CPlayer> newRenterRef = CommandManager.FindTargetPlayer(SourcePlayer, NewRenterNameOrFactionID);
				CPlayer newRenter = newRenterRef.Instance();
				if (newRenter == null)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Player not found.");
					return;
				}

				CItemInstanceDef itemInstDef = CItemInstanceDef.FromBasicValueNoDBID(EItemID.PROPERTY_KEY, Property.Model.Id);
				if (newRenter.Inventory.CanGiveItem(itemInstDef, out List<EItemGiveError> lstErrors, out string strUserFriendlyMessage))
				{
					SourcePlayer.Inventory.AddItemToNextFreeSuitableSlot(itemInstDef, EShowInventoryAction.DoNothing, EItemID.None, (bool bItemGranted) =>
					{
						if (bItemGranted)
						{
							Property.SetRenter(newRenterType, newRenter.ActiveCharacterDatabaseID);
							SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the renter of {0} to {1}.", Property.Model.Name, newRenter.GetCharacterName(ENameType.StaticCharacterName));
							newRenter.SendNotification("Realtor", ENotificationIcon.InfoSign, "{0} {1} has set you to be the renter of {2}", SourcePlayer.AdminTitle, SourcePlayer.Username, Property.Model.Name);
						}
					});
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "{0} could not receive the key:<br>{1}", newRenter.GetCharacterName(ENameType.StaticCharacterName), strUserFriendlyMessage);
				}
			}
			else
			{
				// Otherwise we just want to use it as an integer
				EntityDatabaseID newRenter = 0;
				CFaction Faction = null;

				try
				{
					newRenter = EntityDatabaseID.Parse(NewRenterNameOrFactionID);
					Faction = FactionPool.GetFactionFromID(newRenter);
				}
				catch
				{
				}

				if (Faction == null)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid Faction ID.");
					return;
				}

				Property.SetRenter(newRenterType, Faction.FactionID);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have set the owner of {0} to {1}.", Property.Model.Name, Faction.Name);
			}
		}

		private void ReloadProperty(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				PropertyPool.ReloadProperty(Property);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have reloaded the property {0}.", Property.Model.Name);
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		private void MoveEntrance(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			Vector3 playerPosition = SourcePlayer.Client.Position;
			playerPosition.Z -= 0.9f;
			if (Property != null)
			{
				Property.SetEntrance(playerPosition, SourcePlayer.Client.Rotation.Z, SourcePlayer.Client.Dimension);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Property Entrance Set.");
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		private void MoveExit(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			Dimension propertyID = SourcePlayer.Client.Dimension;
			Vector3 playerPosition = SourcePlayer.Client.Position;
			playerPosition.Z -= 0.9f;
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				Property.SetExit(playerPosition, SourcePlayer.Client.Rotation.Z);
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Property Exit Set.");
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
			}
		}

		private void CreateProperty(CPlayer SourcePlayer, CVehicle SourceVehicle, float Cost, int InteriorID, bool ShowScriptedBlipIfApplicable, string PropertyName)
		{
			// Output some syntax
			void Syntax()
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Cost] [Interior ID] [Show Scripted Blip If Applicable] [Property Name]");
			}

			CInteriorDefinition InteriorDef = InteriorDefinitions.GetInteriorDefinition(InteriorID);
			if (InteriorDef == null || (Cost < 0 && PropertyName.Length <= 0))
			{
				Syntax();
				return;
			}

			if (PropertyName.Length >= 48)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property name too long.");
				return;
			}

			Vector3 vecEntrancePos = SourcePlayer.Client.Position.Copy();
			vecEntrancePos.Z -= 0.9f;

			Vector3 vecExitPos = InteriorDef.Position.Copy();
			vecExitPos.Z -= 0.9f;
			// TODO: Later we probably want an async way of getting ground positions

			Vector3 rotation = SourcePlayer.Client.Rotation;
			List<FurnitureRemoval> removals = new List<FurnitureRemoval>();
			if (InteriorDef.FurnitureRemovals != null)
			{
				removals = InteriorDef.FurnitureRemovals;
			}

			Property.Create(vecEntrancePos, vecExitPos, Cost, PropertyName, rotation.Z,
				SourcePlayer.Client.Dimension, removals, InteriorID, ShowScriptedBlipIfApplicable,
				property =>
				{
					PropertyPool.Add(property);
				});
		}

		private void DeleteProperty(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			try
			{
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
				if (Property != null)
				{
					NetworkEventSender.SendNetworkEvent_AdminConfirmEntityDelete(SourcePlayer, propertyID, EEntityType.Property);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No property exists with id {0}", propertyID);
				}
			}
			catch
			{

			}
		}

		private void OnConfirmPropertyDelete(CPlayer SourcePlayer, EntityDatabaseID propertyID)
		{
			if (SourcePlayer.IsAdmin())
			{
				// check property still exists, might have been destroyed by another admin while the UI was up
				CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
				if (Property != null)
				{
					string name = Property.Model.Name;

					PropertyPool.DestroyProperty(Property);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted the property {0} (#{1}).", name, propertyID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No property exists with id {0}", propertyID);
				}
			}
		}

		private void RemoveRenter(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				if (Property.Model.State == EPropertyState.Rented || Property.Model.State == EPropertyState.Rented_AlwaysEnterable)
				{
					Property.Repossess();
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have removed the renter from property {0} (#{1}).", Property.Model.Name, propertyID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "This property is not rented.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No property exists with id {0}", propertyID);
			}
		}

		private void ForceSell(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property != null)
			{
				if (Property.Model.State == EPropertyState.Owned || Property.Model.State == EPropertyState.Owned_AlwaysEnterable || Property.Model.State == EPropertyState.AvailableToRent)
				{
					Property.Repossess();
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have repossed the property {0} (#{1}).", Property.Model.Name, propertyID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "This property cannot have it's owner removed.");
				}
			}
			else
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No property exists with id {0}", propertyID);
			}
		}

		private async void CreateElevator(CPlayer SourcePlayer, CVehicle SourceVehicle, int type, string elevatorName)
		{

			Vector3 cachedStartPosition = SourcePlayer.ElevatorStartPosition;
			float cachedElevatorRotation = SourcePlayer.ElevatorStartRotation;
			uint cachedElevatorDimension = SourcePlayer.ElevatorStartDimension;
			string cachedElevatorName = SourcePlayer.ElevatorStartName;

			if (type != 0 && type != 1)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "SYNTAX: [Type - 0: Player | 1: Car] [Elevator Name]");
			}

			if (string.IsNullOrEmpty(elevatorName))
			{
				elevatorName = "Elevator";
			}

			if (cachedStartPosition.IsNull())
			{
				Vector3 startPosition = SourcePlayer.Client.Position.Copy();
				startPosition.Z -= 0.9f;
				Vector3 startRotation = SourcePlayer.Client.Rotation.Copy();
				uint startDimension = SourcePlayer.Client.Dimension;

				SourcePlayer.ElevatorStartPosition = startPosition;
				SourcePlayer.ElevatorStartRotation = startRotation.Z;
				SourcePlayer.ElevatorStartDimension = startDimension;
				SourcePlayer.ElevatorStartName = elevatorName;

				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Entrance of the elevator was created, execute the command again for the exit.");
			}
			else
			{
				Vector3 exitPosition = SourcePlayer.Client.Position.Copy();
				exitPosition.Z -= 0.9f;
				Vector3 exitRotation = SourcePlayer.Client.Rotation.Copy();
				uint exitDimension = SourcePlayer.Client.Dimension;

				await ElevatorPool.CreateElevator(0, cachedStartPosition, exitPosition, exitDimension, cachedElevatorDimension, type == 1 ? true : false, cachedElevatorRotation, exitRotation.Z, cachedElevatorName, true).ConfigureAwait(true);

				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 100, 100, "Elevator was created!");

				// Reset the values again after they're set.
				SourcePlayer.ElevatorStartPosition = new Vector3();
				SourcePlayer.ElevatorStartRotation = 0.0f;
				SourcePlayer.ElevatorStartDimension = 0;
				SourcePlayer.ElevatorStartName = null;
			}
		}

		private void DeleteElevator(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID elevatorID)
		{
			try
			{
				CElevatorInstance elevator = ElevatorPool.GetElevatorInstanceFromID(elevatorID);
				if (elevator != null)
				{
					NetworkEventSender.SendNetworkEvent_AdminConfirmEntityDelete(SourcePlayer, elevatorID, EEntityType.Elevator);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No elevator exists with id {0}", elevatorID);
				}
			}
			catch
			{

			}
		}

		private void OnConfirmElevatorDelete(CPlayer SourcePlayer, EntityDatabaseID elevatorID)
		{
			if (SourcePlayer.IsAdmin())
			{
				// check elevator still exists, might have been destroyed by another admin while the UI was up
				CElevatorInstance elevator = ElevatorPool.GetElevatorInstanceFromID(elevatorID);
				if (elevator != null)
				{
					string name = elevator.ElevatorName;

					ElevatorPool.DestroyElevator(elevator);
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "You have deleted the elevator with name {0} (#{1}).", name, elevatorID);
				}
				else
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 232, 66, "No elevator exists with id {0}", elevatorID);
				}
			}
		}



		private void NearbyElevatorsCommand(CPlayer SourcePlayer, CVehicle SourceVehicle)
		{
			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, "Nearby Elevators:");
			foreach (CElevatorInstance elevator in ElevatorPool.GetAllElevatorInstances())
			{
				if (elevator.StartDim != SourcePlayer.Client.Dimension && elevator.ExitDim != SourcePlayer.Client.Dimension)
				{
					continue;
				}

				if (SourcePlayer.Client.Position.DistanceTo2D(elevator.EntrancePos) <= NEARBY_ELEVATORS_DISTANCE || SourcePlayer.Client.Position.DistanceTo2D(elevator.ExitPos) <= NEARBY_ELEVATORS_DISTANCE)
				{
					SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 244, 185, 66, " - #{0} - Name: {1}", elevator.m_DatabaseID, elevator.ElevatorName);
				}
			}
		}

		private void SetThisInteriorId(CPlayer player, CVehicle vehicle, int interiorId)
		{
			Dimension dimension = player.Client.Dimension;
			if (dimension == 0)
			{
				player.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "You are not in a property.");
			}

			SetInteriorId(player, vehicle, dimension, interiorId);
		}

		private void ToExitCommand(CPlayer player, CVehicle vehicle)
		{
			if (player.Client.Dimension == 0)
			{
				player.SendNotification("Anti-fall", ENotificationIcon.ExclamationSign, "You are not in a property.");
				return;
			}

			if (player.IsJailed())
			{
				player.SendNotification("Anti-fall", ENotificationIcon.ExclamationSign, "You cannot use this command while in jail.");
				return;
			}

			CPropertyInstance property = PropertyPool.GetPropertyInstanceFromID(player.Client.Dimension);
			if (property == null)
			{
				player.SendNotification("Anti-fall", ENotificationIcon.ExclamationSign, "You are not in a property.");
				return;
			}

			if (player.IsInVehicleReal)
			{
				List<CPlayer> lstOccupants = VehiclePool.GetVehicleOccupants(vehicle);
				vehicle.TeleportAndWarpOccupants(lstOccupants, property.Model.ExitPosition.Add(new Vector3(0.0f, 0.0f, 2.0f)), player.Client.Dimension, player.Client.Rotation);
				return;
			}

			player.Client.Position = property.Model.ExitPosition.Add(new Vector3(0.0f, 0.0f, 1.0f));
		}

		private void SetInteriorId(CPlayer SourcePlayer, CVehicle SourceVehicle, EntityDatabaseID propertyID, int interiorId)
		{
			CPropertyInstance Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
			if (Property == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Property not found.");
				return;
			}

			CInteriorDefinition InteriorDef = InteriorDefinitions.GetInteriorDefinition(interiorId);
			if (InteriorDef == null)
			{
				SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 255, 0, 0, "Invalid interior ID.");
				return;
			}

			// remove custom map + objects if present
			MapLoader.FullyRemoveCustomMap((int)propertyID);

			Vector3 vecExitPos = InteriorDef.Position.Copy();
			Property.Model.SetInterior(interiorId, vecExitPos);
			PropertyPool.ReloadProperty(Property);

			NAPI.Task.Run(() =>
			{
				Property = PropertyPool.GetPropertyInstanceFromID(propertyID);
				if (Property == null)
				{
					return;
				}
				foreach (var player in PlayerPool.GetAllPlayers()
					.Where(player => player.Client.Dimension == propertyID))
				{
					player.OnEnterProperty(Property);
				}
			}, delayTime: 1000); // delay by 1sec to give time for the reload to complete.

			SourcePlayer.PushChatMessageWithColor(EChatChannel.AdminActions, 0, 255, 0, "Property's interior was updated to ID {0}.", interiorId);
		}

		private void OnMowedLawn(CPlayer player, long propertyId)
		{
			CPropertyInstance cProperty = PropertyPool.GetPropertyInstanceFromID(propertyId);

			if (cProperty.Model.OwnerType != EPropertyOwnerType.Player || cProperty.Model.OwnerId != player.ActiveCharacterDatabaseID)
			{
				return;
			}

			cProperty.Model.SetMowed();
			cProperty.UpdateMarkerEntityData();
		}
	}
}
