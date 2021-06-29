using RAGE;
using RAGE.Elements;
using System;
using System.Collections.Generic;

public class PetSystem
{
	private Int64 m_CurrentPetBeingNamed = -1;
	private Dictionary<RAGE.Elements.Player, PetData> m_Pets = new Dictionary<Player, PetData>();

	public class PetData
	{
		private PedHash m_Hash = PedHash.Pug;

		public PetData(PedHash petHash, RAGE.Vector3 vecPos, float rotZ, uint dimension)
		{
			m_Hash = petHash;
			Create(vecPos, rotZ, dimension);
		}

		private void Create(RAGE.Vector3 vecPos, float rotZ, uint dimension)
		{
			AsyncModelLoader.RequestSyncInstantLoad((uint)m_Hash);
			PedInstance = new RAGE.Elements.Ped((uint)m_Hash, vecPos, rotZ, dimension);
		}

		public void Destroy()
		{
			if (PedInstance != null)
			{
				PedInstance.Destroy();
				PedInstance = null;
			}
		}

		public bool IsSpawned()
		{
			return (PedInstance != null);
		}

		public void Update(RAGE.Elements.Player petOwner)
		{
			// handle vehicle states
			if (PedInstance == null && !petOwner.IsInAnyVehicle(false))
			{
				Create(petOwner.Position, petOwner.GetRotation(0).Z, petOwner.Dimension);
			}
			else if (PedInstance != null && petOwner.IsInAnyVehicle(false))
			{
				Destroy();
				return;
			}

			if (PedInstance != null)
			{
				Vector3 vecOwnerPos = petOwner.Position;
				PedInstance.FreezePosition(false);

				float fDist = 1.0f;
				Vector3 vecPosToSide = vecOwnerPos.CopyVector();
				float radians = (petOwner.GetRotation(0).Z) * (3.14f / 180.0f);
				vecPosToSide.X += (float)Math.Cos(radians) * fDist;
				vecPosToSide.Y += (float)Math.Sin(radians) * fDist;
				vecPosToSide.Z = WorldHelper.GetGroundPosition(vecPosToSide) + 0.45f;

				// and move forward a bit for extrapolation
				fDist = 1.5f;
				radians = (petOwner.GetRotation(0).Z + 90.0f) * (3.14f / 180.0f);
				vecPosToSide.X += (float)Math.Cos(radians) * fDist;
				vecPosToSide.Y += (float)Math.Sin(radians) * fDist;
				vecPosToSide.Z = WorldHelper.GetGroundPosition(vecPosToSide) + 0.45f;

				float fDistFromLastUpdate = (vecPosToSide - Position()).Length();
				if (fDistFromLastUpdate > 100.0f) // Do we need to warp? Could be interior change or TP for example
				{
					RAGE.Game.Entity.SetEntityCoords(PedInstance.Handle, vecPosToSide.X, vecPosToSide.Y, vecPosToSide.Z, false, false, false, false);
				}
				else
				{
					if (fDistFromLastUpdate <= 1.5f)
					{
						RAGE.Game.Ai.ClearPedTasks(PedInstance.Handle);
					}
					else
					{
						RAGE.Game.Ai.TaskGoStraightToCoord(PedInstance.Handle, vecPosToSide.X, vecPosToSide.Y, vecPosToSide.Z, petOwner.GetSpeed() * 2.0f, 1000, petOwner.GetRotation(0).Z, 0.0f);
					}

				}

				// If local player, and inside player specific dimension, set the pet invisible
				if (petOwner == Player.LocalPlayer && petOwner.Dimension == WorldHelper.GetPlayerSpecificDimension())
				{
					PedInstance.Dimension = petOwner.Dimension;
					PedInstance.SetAlpha(0, false);
				}
				else
				{
					PedInstance.Dimension = petOwner.Dimension;
					PedInstance.SetAlpha(255, false);
				}

				LastTargetPosition = vecPosToSide;
			}
		}

		public RAGE.Elements.Ped PedInstance { get; set; }
		public Vector3 LastTargetPosition { get; set; }

		public Vector3 Position()
		{
			if (PedInstance != null)
			{
				RAGE.Vector3 vecRealWorldPos = PedInstance.GetWorldPositionOfBone(PedInstance.GetBoneIndex(12844));

				// doenst have bone
				if (vecRealWorldPos.X == 0.0f && vecRealWorldPos.Y == 0.0f && vecRealWorldPos.Z == 0.0f)
				{
					vecRealWorldPos = PedInstance.GetWorldPositionOfBone(PedInstance.GetBoneIndex(0));
				}

				return vecRealWorldPos;
			}

			return new Vector3(0.0f, 0.0f, 0.0f);
		}
	}


	public PetSystem()
	{
		NetworkEvents.SetPetName += OnSetPetName;

		RageEvents.RAGE_OnPlayerQuit += OnPlayerDisconnected;
		RageEvents.RAGE_OnRender += RenderPetNametags;

		UIEvents.OnNamePet_Submit += OnNamePet_Submit;
		UIEvents.OnNamePet_Cancel += OnNamePet_Cancel;
		RageEvents.RAGE_OnTick_PerFrame += OnUpdatePetPositions;

		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;
		RageEvents.RAGE_OnEntityStreamIn += OnStreamOut;
		RageEvents.AddDataHandler(EDataNames.PET_NAME, OnPetDataChanged);
		RageEvents.AddDataHandler(EDataNames.PET_TYPE, OnPetDataChanged);
	}

	public static bool GetNametagPositionForPet(RAGE.Vector3 vecSourcePositionPlayer, PetData petData, out Vector2 vecNametagScreenPos, out float fDistScale)
	{
		RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);

		float fDistPlayerPos = WorldHelper.GetDistance(vecSourcePositionPlayer, petData.Position());
		float fDistCameraPos = WorldHelper.GetDistance(CameraManager.GetCameraPosition(ECameraID.GAME), petData.Position());

		// Use whichever vector is closer for our render viewport
		float fDist = Math.Max(fDistPlayerPos, fDistCameraPos);

		float fMaxDist = 60.0f;
		if (fDist <= fMaxDist)
		{
			fDistScale = 1.0f - (fDist / fMaxDist);
			float fDistScaleZPos = Math.Min(fDistScale, 0.70f);
			RAGE.Vector3 vecNametagPos = petData.PedInstance.GetWorldPositionOfBone(petData.PedInstance.GetBoneIndex(12844));

			// doenst have bone
			if (vecNametagPos.X == 0.0f && vecNametagPos.Y == 0.0f && vecNametagPos.Z == 0.0f)
			{
				vecNametagPos = petData.PedInstance.GetWorldPositionOfBone(petData.PedInstance.GetBoneIndex(0));
			}

			vecNametagPos.Z += (1.0f * fDistScaleZPos);

			// If we got this far, now do the expensive raycast
			CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecNametagPos, RAGE.Elements.Player.LocalPlayer.Handle, -1);

			if (!raycast.Hit)
			{
				vecNametagScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(vecNametagPos);

				if (vecNametagScreenPos.SetSuccessfully)
				{
					return true;
				}
			}
		}

		fDistScale = 0.0f;
		vecNametagScreenPos = null;
		return false;
	}

	private void RenderPetNametags()
	{
		bool bLocalPlayerSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		bool bNametagsHidden = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.NAMETAGS);
		float fTextWidthSingleChar = RAGE.Game.Ui.EndTextCommandGetWidth((int)RAGE.Game.Font.ChaletLondon);
		if (bLocalPlayerSpawned && !bNametagsHidden)
		{
			RAGE.Vector3 vecLocalPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

			foreach (var kvPair in m_Pets)
			{
				if (kvPair.Value.IsSpawned())
				{
					if (kvPair.Key == Player.LocalPlayer && kvPair.Key.Dimension == WorldHelper.GetPlayerSpecificDimension())
					{
						continue;
					}

					float fDistScale = 0.0f;
					Vector2 vecScreenPos = null;
					if (GetNametagPositionForPet(vecLocalPlayerPos, kvPair.Value, out vecScreenPos, out fDistScale))
					{
						string strName = DataHelper.GetEntityData<string>(kvPair.Key, EDataNames.PET_NAME);
						string strOwnerName = kvPair.Key.Name;
						string strOwnerDisplayName = strOwnerName.EndsWith('s') ? Helpers.FormatString("<{0}' Pet>", strOwnerName) : Helpers.FormatString("<{0}'s Pet>", strOwnerName);

						TextHelper.Draw2D(strName, vecScreenPos.X, vecScreenPos.Y, 0.35f * fDistScale, new RAGE.RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
						TextHelper.Draw2D(strOwnerDisplayName, vecScreenPos.X, vecScreenPos.Y + 0.03f, 0.35f * fDistScale, new RAGE.RGBA(255, 255, 255, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);

					}
				}
			}
		}
	}

	private void OnPetDataChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			UpdatePetForPlayer(player);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			UpdatePetForPlayer(player);
		}
	}

	private void OnStreamOut(RAGE.Elements.Entity entity)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player)
		{
			RAGE.Elements.Player player = (RAGE.Elements.Player)entity;
			CleanupPetForPlayer(player);
		}
	}

	private void OnPlayerDisconnected(RAGE.Elements.Player player)
	{
		CleanupPetForPlayer(player);
	}

	private void OnPlayerEnterVehicle(RAGE.Elements.Player player, Vehicle vehicle, sbyte seatId)
	{
		CleanupPetForPlayer(player);
	}

	private void OnPlayerExitVehicle(RAGE.Elements.Player player)
	{
		CleanupPetForPlayer(player);
	}

	private void CleanupPetForPlayer(RAGE.Elements.Player player)
	{
		if (m_Pets.ContainsKey(player))
		{
			if (m_Pets[player] != null)
			{
				m_Pets[player].Destroy();
			}

			m_Pets.Remove(player);
		}
	}

	private void OnUpdatePetPositions()
	{
		foreach (var kvPair in m_Pets)
		{
			kvPair.Value.Update(kvPair.Key);
		}
	}

	private PedHash GetPetHash(EPetType a_PetType)
	{
		switch (a_PetType)
		{
			case EPetType.Boar:
				{
					return PedHash.Boar;
				}
			case EPetType.Cat:
				{
					return PedHash.Cat;
				}
			case EPetType.Chickenhawk:
				{
					return PedHash.ChickenHawk;
				}
			case EPetType.Chimp:
				{
					return PedHash.Chimp;
				}
			case EPetType.Chop:
				{
					return PedHash.Chop;
				}
			case EPetType.Cormorant:
				{
					return PedHash.Cormorant;
				}
			case EPetType.Cow:
				{
					return PedHash.Cow;
				}
			case EPetType.Coyote:
				{
					return PedHash.Coyote;
				}
			case EPetType.Crow:
				{
					return PedHash.Crow;
				}
			case EPetType.Deer:
				{
					return PedHash.Deer;
				}
			case EPetType.Dolphin:
				{
					return PedHash.Dolphin;
				}
			case EPetType.Fish:
				{
					return PedHash.Fish;
				}
			case EPetType.HammerheadShark:
				{
					return PedHash.HammerShark;
				}
			case EPetType.Hen:
				{
					return PedHash.Hen;
				}
			case EPetType.Humpback:
				{
					return PedHash.Humpback;
				}
			case EPetType.Husky:
				{
					return PedHash.Husky;
				}
			case EPetType.Killerwhale:
				{
					return PedHash.KillerWhale;
				}
			case EPetType.MountainLion:
				{
					return PedHash.MountainLion;
				}
			case EPetType.Sasquatch:
				{
					return PedHash.Orleans;
				}
			case EPetType.Pig:
				{
					return PedHash.Pig;
				}
			case EPetType.Pigeon:
				{
					return PedHash.Pigeon;
				}
			case EPetType.Poodle:
				{
					return PedHash.Poodle;
				}
			case EPetType.Pug:
				{
					return PedHash.Pug;
				}
			case EPetType.Rabbit:
				{
					return PedHash.Rabbit;
				}
			case EPetType.Rat:
				{
					return PedHash.Rat;
				}
			case EPetType.Retriever:
				{
					return PedHash.Retriever;
				}
			case EPetType.RhesusMonkey:
				{
					return PedHash.Rhesus;
				}
			case EPetType.Rottweiler:
				{
					return PedHash.Rottweiler;
				}
			case EPetType.Seagull:
				{
					return PedHash.Seagull;
				}
			case EPetType.Shepherd:
				{
					return PedHash.Shepherd;
				}
			case EPetType.Stingray:
				{
					return PedHash.Stingray;
				}
			case EPetType.Tigershark:
				{
					return PedHash.TigerShark;
				}
			case EPetType.Westy:
				{
					return PedHash.Westy;
				}
			case EPetType.Panther:
				{
				return PedHash.Panther;
				}
		}

		return PedHash.Pug;
	}

	private void UpdatePetForPlayer(RAGE.Elements.Player player)
	{
		if (DataHelper.HasEntityData(player, EDataNames.PET_NAME) && DataHelper.HasEntityData(player, EDataNames.PET_TYPE))
		{
			PedHash petHash = GetPetHash(DataHelper.GetEntityData<EPetType>(player, EDataNames.PET_TYPE));

			// cleanup existing pet
			CleanupPetForPlayer(player);

			float fDist = 1.0f;
			Vector3 vecPosToSide = player.Position;
			float radians = (player.GetRotation(0).Z) * (3.14f / 180.0f);
			vecPosToSide.X += (float)Math.Cos(radians) * fDist;
			vecPosToSide.Y += (float)Math.Sin(radians) * fDist;
			vecPosToSide.Z = WorldHelper.GetGroundPosition(vecPosToSide) + 0.45f;

			m_Pets[player] = new PetData(petHash, vecPosToSide, player.GetRotation(0).Z, player.Dimension);
		}
		else
		{
			CleanupPetForPlayer(player);
		}

	}

	private void OnSetPetName(EPetType a_Type, Int64 petID)
	{
		ItemSystem.GetPlayerInventory()?.HideInventory();
		m_CurrentPetBeingNamed = petID;
		UserInputHelper.RequestUserInput("Name Your Pet", Helpers.FormatString("Enter the desired name for your pet {0}", a_Type.ToString()), "Fido", UIEventID.OnNamePet_Submit, UIEventID.OnNamePet_Cancel);
	}

	private void OnNamePet_Submit(string strInput)
	{
		NetworkEventSender.SendNetworkEvent_SavePetName(m_CurrentPetBeingNamed, strInput);
	}

	private void OnNamePet_Cancel()
	{
		// Nothing actually happened, do nothing, ui is already gone
	}
}