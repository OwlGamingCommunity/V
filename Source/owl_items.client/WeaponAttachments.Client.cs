using System;
using System.Collections.Generic;

public class WeaponAttachmentsSystem
{
	public WeaponAttachmentsSystem()
	{
		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.AddDataHandler(EDataNames.WEAP_MODS, OnWeaponModsDataChanged);
	}

	private void OnWeaponModsDataChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ProcessWeaponModsForPlayer(player);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			ProcessWeaponModsForPlayer(player);
		}
	}

	private void ProcessWeaponModsForPlayer(RAGE.Elements.Player player)
	{
		if (DataHelper.HasEntityData(player, EDataNames.WEAP_MODS))
		{
			string strAttachmentData = DataHelper.GetEntityData<string>(player, EDataNames.WEAP_MODS);
			Dictionary<WeaponHash, List<EItemID>> dictAttachments = OwlJSON.DeserializeObject<Dictionary<WeaponHash, List<EItemID>>>(strAttachmentData, EJsonTrackableIdentifier.ProcessWeaponMods);

			foreach (var kvPair in dictAttachments)
			{
				WeaponHash weaponHash = kvPair.Key;
				List<EItemID> lstModsForThisWeapon = kvPair.Value;

				foreach (EItemID attachmentID in lstModsForThisWeapon)
				{
					WeaponAttachmentDefinition weaponAttachmentDef = WeaponAttachmentDefinitions.GetWeaponAttachmentDefinitionByID(attachmentID);

					if (weaponAttachmentDef != null)
					{
						weaponAttachmentDef.ApplyToWeapon(player, weaponHash, attachmentID);
					}
				}
			}

			foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
			{
				foreach (var kvPair in WeaponAttachmentDefinitions.g_WeaponAttachmentDefinitions)
				{
					bool bAttemptRemove = true;

					if (dictAttachments.ContainsKey(weapon))
					{
						List<EItemID> lstModsForThisWeapon = dictAttachments[weapon];

						if (lstModsForThisWeapon.Contains(kvPair.Key))
						{
							bAttemptRemove = false;
						}
					}

					if (bAttemptRemove)
					{
						kvPair.Value.RemoveFromWeapon(player, weapon, kvPair.Key);
					}
				}
			}
		}
	}
}