using System;

public class PDSearchlight
{
	public PDSearchlight()
	{
		RageEvents.RAGE_OnRender += OnRender;

		ScriptControls.SubscribeToControl(EScriptControlID.TogglePoliceSearchlight, ToggleSpotlight);
	}

	private void OnRender()
	{
		UpdateLocalSpotlight();
		RenderSpotlights();
	}

	private void ToggleSpotlight(EControlActionType actionType)
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;
		if (localPlayer.Vehicle != null)
		{
			bool bIsPoliceVehicle = DataHelper.GetEntityData<bool>(localPlayer.Vehicle, EDataNames.IS_POLICE_VEHICLE);
			if (bIsPoliceVehicle)
			{
				NetworkEventSender.SendNetworkEvent_ToggleSpotlight();
			}
		}
	}

	private void RenderSpotlights()
	{
		foreach (var vehicle in OptimizationCachePool.StreamedInVehicles())
		{
			bool bSpotlightState = DataHelper.GetEntityData<bool>(vehicle, EDataNames.SEARCHLIGHT);

			if (bSpotlightState)
			{
				float fSpotlightRot = DataHelper.GetEntityData<float>(vehicle, EDataNames.SEARCHLIGHT_ROT);

				RAGE.Vector3 vecSourcePos = new RAGE.Vector3(vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z + 1.0f);
				const float fOffset = 1.0f;
				float fWorldRot = vehicle.GetRotation(0).Z + fSpotlightRot;
				float radians = fWorldRot * (3.14f / 180.0f);
				RAGE.Vector3 vecLightTargetPos = new RAGE.Vector3(vecSourcePos.X, vecSourcePos.Y, vecSourcePos.Z);
				vecLightTargetPos.X += (float)Math.Cos(radians) * fOffset;
				vecLightTargetPos.Y += (float)Math.Sin(radians) * fOffset;

				float fMaxZ = vehicle.Position.Z + 2.0f;
				float fMinZ = vehicle.Position.Z - 2.0f;
				vecLightTargetPos.Z = Math.Clamp(vehicle.Position.Z + 1.0f, fMinZ, fMaxZ);

				RAGE.Vector3 vecDirection = new RAGE.Vector3(vecLightTargetPos.X - vecSourcePos.X, vecLightTargetPos.Y - vecSourcePos.Y, vecLightTargetPos.Z - vecSourcePos.Z);
				RAGE.Game.Graphics.DrawSpotLight(vecSourcePos.X, vecSourcePos.Y, vecSourcePos.Z, vecDirection.X, vecDirection.Y, 0.0f, 255, 255, 255, 20.0f, 10.0f, 0.0f, 13.0f, 1.0f);
			}
		}
	}

	private void UpdateLocalSpotlight()
	{
		RAGE.Elements.Player localPlayer = RAGE.Elements.Player.LocalPlayer;
		if (localPlayer.Vehicle != null)
		{
			// TODO_CSHARP: Way to check mouse buttons
			if (KeyBinds.IsMouseButtonDown(0x04))
			{
				bool bSpotlightState = DataHelper.GetEntityData<bool>(localPlayer.Vehicle, EDataNames.SEARCHLIGHT);

				if (bSpotlightState)
				{
					float fSpotlightRot = DataHelper.GetEntityData<float>(localPlayer.Vehicle, EDataNames.SEARCHLIGHT_ROT);
					float fNewRot = (CameraManager.GetCameraRotation(ECameraID.GAME).Z + 90.0f) - localPlayer.Vehicle.GetRotation(0).Z;

					// must have delta 1 or greater, to save bandwidth
					const float fBandwidthDelta = 1.0f;
					if (fNewRot > (fSpotlightRot + fBandwidthDelta) || fNewRot < (fSpotlightRot - fBandwidthDelta))
					{
						NetworkEventSender.SendNetworkEvent_SetSpotlightRotation(fNewRot);
					}
				}
			}
		}
	}
}