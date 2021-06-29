using RAGE;
using System;

public class Nametags
{
	// colors
	private RAGE.RGBA rgbaAdmin = new RAGE.RGBA(255, 194, 14, 255);
	private RAGE.RGBA rgbaUAT = new RAGE.RGBA(14, 194, 255, 255);
	private RAGE.RGBA rgbaNormal = new RAGE.RGBA(255, 255, 255, 255);

	public Nametags()
	{
		RageEvents.RAGE_OnRender += OnRender;
		RAGE.Nametags.Enabled = false;
	}

	public static bool GetNametagPositionForPlayer(RAGE.Vector3 vecSourcePositionPlayer, RAGE.Elements.Player player, out Vector2 vecNametagScreenPos, out float fDistScale)
	{
		RAGE.Vector3 vecGameplayCamPos = CameraManager.GetCameraPosition(ECameraID.GAME);

		// Useful for debugging
		//RAGE.Vector3 vecDummyPos = new Vector3(314.3791f, 155.9981f, 103.7807f);
		//float fDistPlayerPos = WorldHelper.GetDistance(vecSourcePositionPlayer, vecDummyPos);
		//float fDistCameraPos = WorldHelper.GetDistance(CameraManager.GetCameraPosition(ECameraID.GAME), vecDummyPos);

		float fDistPlayerPos = WorldHelper.GetDistance(vecSourcePositionPlayer, player.Position);
		float fDistCameraPos = WorldHelper.GetDistance(CameraManager.GetCameraPosition(ECameraID.GAME), player.Position);

		// Use whichever vector is closer for our render viewport
		float fDist = Math.Max(fDistPlayerPos, fDistCameraPos);

		float fMaxDist = 60.0f;
		if (fDist <= fMaxDist)
		{
			fDistScale = 1.0f - (fDist / fMaxDist);
			float fDistScaleZPos = Math.Min(fDistScale, 0.70f);
			RAGE.Vector3 vecNametagPos = player.GetWorldPositionOfBone(player.GetBoneIndex(12844));

			if (player == RAGE.Elements.Player.LocalPlayer)
			{
				vecNametagPos.Z += (0.5f * fDistScaleZPos);
			}
			else
			{
				vecNametagPos.Z += (0.75f * fDistScaleZPos);
			}


			// VEHICLE NAMETAGS
			if (player.Vehicle != null)
			{
				// determine seat
				int currentSeat = -2;
				const int maxPassengerDoors = 4;
				for (int seat = -1; seat < maxPassengerDoors; ++seat)
				{
					if (player.Vehicle.GetPedInSeat(seat, 0) == player.Handle)
					{
						currentSeat = seat;
						break;
					}
				}

				// use true pos / do nothing (we do this for hanging too, no directly relevant bone)
				if (currentSeat == -2 || currentSeat >= maxPassengerDoors - 1) // subtract 1 because -1 is driver
				{
					// nothing to do here, nametag pos is already correct
				}
				else // use seat pos
				{
					string[] strSeatBones = new string[maxPassengerDoors]
					{
						"window_lf1",
						"window_rf1",
						"window_lr1",
						"window_rr1"
					};

					int boneIndex = player.Vehicle.GetBoneIndexByName(strSeatBones[currentSeat + 1]);
					if (boneIndex > 0)
					{
						Vector3 vecNametagPosVehicle = player.Vehicle.GetWorldPositionOfBone(boneIndex);

						if (vecNametagPosVehicle != null)
						{
							vecNametagPosVehicle.Z += 1.5f;
							vecNametagPos = vecNametagPosVehicle;
						}
					}

				}
			}

			// Useful for debugging
			//vecNametagPos = vecDummyPos;

			// If we got this far, now do the expensive raycast
			CRaycastResult raycast = WorldHelper.RaycastFromTo(vecGameplayCamPos, vecNametagPos, RAGE.Elements.Player.LocalPlayer.Handle, -1);

			if (!raycast.Hit || (player.Vehicle != null && raycast.EntityHit == player.Vehicle))
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

	private void OnRender()
	{
		bool bLocalPlayerSpawned = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.IS_SPAWNED);
		float fTextWidthSingleChar = RAGE.Game.Ui.EndTextCommandGetWidth((int)RAGE.Game.Font.ChaletLondon);
		bool bNametagsHidden = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.NAMETAGS);

		if (bLocalPlayerSpawned && !bNametagsHidden)
		{
			RAGE.Vector3 vecLocalPlayerPos = RAGE.Elements.Player.LocalPlayer.Position;

			foreach (var player in RAGE.Elements.Entities.Players.Streamed)
			{
				// We need this to draw the real name for /ame and /ado
				string bPlayerStaticName = DataHelper.GetEntityData<string>(player, EDataNames.CHARACTER_NAME);

				bool bRemotePlayerSpawned = DataHelper.GetEntityData<bool>(player, EDataNames.IS_SPAWNED);
				bool bReconOn = DataHelper.GetEntityData<bool>(player, EDataNames.RECON);
				bool bDisappearOn = DataHelper.GetEntityData<bool>(player, EDataNames.DISAPPEAR);

				// Actions
				string strAmeMessage = DataHelper.GetEntityData<string>(player, EDataNames.AME_MESSAGE);
				string strAdoMessage = DataHelper.GetEntityData<string>(player, EDataNames.ADO_MESSAGE);
				string strStatusMessage = DataHelper.GetEntityData<string>(player, EDataNames.STATUS_MESSAGE);
				string strMessageDrawn = DataHelper.GetEntityData<string>(player, EDataNames.MESSAGE_DRAWN);

				// Admin
				bool bIsAdmin = DataHelper.GetEntityData<EAdminLevel>(player, EDataNames.ADMIN_LEVEL) != EAdminLevel.None;
				bool bAdminDuty = DataHelper.GetEntityData<bool>(player, EDataNames.ADMIN_DUTY);
				bool bIsUAT = DataHelper.GetEntityData<EAdminLevel>(player, EDataNames.ADMIN_LEVEL) >= EAdminLevel.LeadAdmin;

				// Badges
				bool bBadgeEnabled = DataHelper.GetEntityData<bool>(player, EDataNames.BADGE_ENABLED);

				//LOCAL PLAYER
				bool bToggledLocalPlayerNametag = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.LOCALPLAYER_NAMETAG_TOGGLED);

				if (bRemotePlayerSpawned && !bReconOn && !bDisappearOn)
				{
					float fDistScale = 0.0f;
					float textSpacing = 0.03f;
					Vector2 vecScreenPos = null;

					if (GetNametagPositionForPlayer(vecLocalPlayerPos, player, out vecScreenPos, out fDistScale))
					{
						string strNameToRender = KeyBinds.IsKeyDown(0x11) ? Helpers.FormatString("[{0}] {1}", DataHelper.GetEntityData<int>(player, EDataNames.PLAYER_ID), player.Name) : player.Name;

						if (!bToggledLocalPlayerNametag || player != RAGE.Elements.Player.LocalPlayer)
						{
							if (bAdminDuty && bAdminDuty)
							{
								TextHelper.Draw2D(strNameToRender, vecScreenPos.X, vecScreenPos.Y, 0.35f * fDistScale, bIsUAT ? rgbaUAT : rgbaAdmin, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
							}
							else if (bBadgeEnabled)
							{
								string strBadgeFactionName = DataHelper.GetEntityData<string>(player, EDataNames.BADGE_FACTION_NAME);
								string strBadgeText = DataHelper.GetEntityData<string>(player, EDataNames.BADGE_NAME);

								// We have to store the entity data separately because you can't pass the color object
								int badgeColorR = DataHelper.GetEntityData<int>(player, EDataNames.BADGE_COLOR_R);
								int badgeColorG = DataHelper.GetEntityData<int>(player, EDataNames.BADGE_COLOR_G);
								int badgeColorB = DataHelper.GetEntityData<int>(player, EDataNames.BADGE_COLOR_B);

								string strBadgeFullText = Helpers.FormatString("{0} - {1}", strBadgeFactionName, strBadgeText);

								Vector2 vecScreenPosAdminText = new Vector2(vecScreenPos.X, vecScreenPos.Y - 0.03f);

								TextHelper.Draw2D(strBadgeFullText, vecScreenPosAdminText.X, vecScreenPosAdminText.Y, 0.35f * fDistScale, new RAGE.RGBA((uint)badgeColorR, (uint)badgeColorG, (uint)badgeColorB, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false, false);
								TextHelper.Draw2D(strNameToRender, vecScreenPos.X, vecScreenPos.Y, 0.35f * fDistScale, new RAGE.RGBA((uint)badgeColorR, (uint)badgeColorG, (uint)badgeColorB), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
								textSpacing = 0.06f;
							}
							else
							{
								TextHelper.Draw2D(strNameToRender, vecScreenPos.X, vecScreenPos.Y, 0.35f * fDistScale, rgbaNormal, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
							}
						}

						// Statuses must be a bit closer to see
						if (fDistScale >= 0.5f)
						{
							// /ame, /ado and /status
							if (!string.IsNullOrEmpty(strStatusMessage))
							{
								TextHelper.Draw2D(Helpers.FormatString("*{0}*", strStatusMessage), vecScreenPos.X, vecScreenPos.Y - textSpacing, 0.35f * fDistScale, new RAGE.RGBA(136, 87, 201, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false, false);
								textSpacing = bBadgeEnabled ? 0.09f : 0.06f;
							}

							if (!string.IsNullOrEmpty(strMessageDrawn))
							{
								if (strMessageDrawn.Equals("ame"))
								{
									if (!string.IsNullOrEmpty(strAmeMessage))
									{
										string formattedText = Helpers.FormatString("{0} {1}", bPlayerStaticName, strAmeMessage);
										TextHelper.Draw2D(formattedText, vecScreenPos.X, vecScreenPos.Y - textSpacing, 0.35f * fDistScale, new RAGE.RGBA(255, 51, 102, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
									}
								}
								else
								{
									if (!string.IsNullOrEmpty(strAdoMessage))
									{
										string formattedText = Helpers.FormatString("*{0} (({1}))*", strAdoMessage, bPlayerStaticName);
										TextHelper.Draw2D(formattedText, vecScreenPos.X, vecScreenPos.Y - textSpacing, 0.35f * fDistScale, new RAGE.RGBA(255, 51, 102, 255), RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, false);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}