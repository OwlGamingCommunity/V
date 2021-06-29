using System.Collections.Generic;


public class SmokingSystem
{
	public SmokingSystem()
	{
		RageEvents.RAGE_OnEntityStreamIn += OnEntityStreamIn;
		RageEvents.RAGE_OnEntityStreamOut += OnEntityStreamOut;

		ClientTimerPool.CreateTimer(UpdateAttachments, 200);
	}

	private void OnEntityStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateSmokingAttachments((RAGE.Elements.Player)entity);
		}
	}

	private void OnEntityStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			UpdateSmokingAttachments((RAGE.Elements.Player)entity);
		}
	}

	// TODO_HELPER: Create a helper function for this
	private Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject> g_DictSmokingAttachments = new Dictionary<RAGE.Elements.Player, RAGE.Elements.MapObject>();

	private void UpdateSmokingAttachments(RAGE.Elements.Player player)
	{
		bool IsCurrentlySmoking = DataHelper.GetEntityData<bool>(player, EDataNames.SMOKING);
		ESmokingItemType smokingItemType = DataHelper.GetEntityData<ESmokingItemType>(player, EDataNames.SMOKING_TYPE);
		string unsignedHash = string.Empty;

		if (IsCurrentlySmoking)
		{
			if (smokingItemType == ESmokingItemType.Cigarette)
			{
				unsignedHash = "ng_proc_cigarette01a";
			}
			else if (smokingItemType == ESmokingItemType.CigarBasic || smokingItemType == ESmokingItemType.CigarHighClass)
			{
				unsignedHash = "prop_cigar_02";
			}
			else if (smokingItemType == ESmokingItemType.Joint)
			{
				unsignedHash = "p_amb_joint_01";
			}
			else
			{
				return;
			}

			if (!g_DictSmokingAttachments.ContainsKey(player))
			{
				if (!string.IsNullOrEmpty(unsignedHash))
				{
					uint hash = HashHelper.GetHashUnsigned(unsignedHash);
					AsyncModelLoader.RequestAsyncLoad(hash, (uint modelLoaded) =>
					{
						g_DictSmokingAttachments.Add(player, new RAGE.Elements.MapObject(modelLoaded, player.Position, new RAGE.Vector3(0.0f, 0.0f, 0.0f), dimension: player.Dimension));
					});
				}
			}

			if (g_DictSmokingAttachments.ContainsKey(player))
			{
				if (smokingItemType != ESmokingItemType.CigarBasic && smokingItemType != ESmokingItemType.CigarHighClass)
				{
					RAGE.Game.Entity.AttachEntityToEntity(g_DictSmokingAttachments[player].Handle, player.Handle, player.GetBoneIndex(64017), 0.015f, 0.0100f, 0.0250f, 0.024f, -100.0f, 40.0f, true, true, false, true, 1, true);
				}
				else
				{
					RAGE.Game.Entity.AttachEntityToEntity(g_DictSmokingAttachments[player].Handle, player.Handle, player.GetBoneIndex(64017), 0.015f, -0.0001f, 0.003f, 55f, 0.0f, -85.0f, true, true, false, true, 0, true);
				}
			}
		}
		else
		{
			if (g_DictSmokingAttachments.ContainsKey(player))
			{
				g_DictSmokingAttachments[player].Destroy();
				g_DictSmokingAttachments.Remove(player);
			}
		}
	}

	private void UpdateAttachments(object[] parameters)
	{
		foreach (var player in RAGE.Elements.Entities.Players.All)
		{
			UpdateSmokingAttachments(player);
		}
	}
}

