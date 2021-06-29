using RAGE;
using RAGE.Elements;
using System;

public class VehicleCrusher
{
	private static readonly Vector3 g_vecCrusherPos = new Vector3(-45.74776f, -1081.76f, 25.9f);
	private const float g_fRadius = 5.0f;
	private readonly Marker m_CrusherMarker;

	public VehicleCrusher()
	{
		m_CrusherMarker = new Marker(27, g_vecCrusherPos, g_fRadius, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(g_fRadius, g_fRadius, 1.0f), new RGBA(75, 255, 75, 200));

		RageEvents.RAGE_OnRender += OnRender;
		NetworkEvents.VehicleCrusher_ShowCrushInterface += ShowCrusherInterface;
		UIEvents.VehicleCrusher_Crush += CrushVehicle;
	}

	public void OnRender()
	{
		if (m_CrusherMarker == null)
		{
			return;
		}

		string strMessage;
		ConsoleKey key;

		if (Player.LocalPlayer.Vehicle == null)
		{
			strMessage = "You need a vehicle to sell!";
			key = ConsoleKey.NoName;
		}
		else
		{
			strMessage = "Sell vehicle to Premium Deluxe Motorsport";
			key = ScriptControls.GetKeyBoundToControl(EScriptControlID.Interact);
		}

		WorldHintManager.DrawExclusiveWorldHint(key, strMessage, null, InteractWithCrusher, m_CrusherMarker.Position,
			m_CrusherMarker.Dimension, false, false, g_fRadius, bAllowInVehicle: true);
	}

	public void InteractWithCrusher()
	{
		NetworkEventSender.SendNetworkEvent_VehicleCrusher_RequestCrushInformation();
	}

	public void ShowCrusherInterface(float amount, bool tokenVehicle, string vehicleName)
    {
        string message = Helpers.FormatString("Are you sure you want to sell your {0} for ${1:0.00}", vehicleName, amount);
        if (tokenVehicle)
        {
            message = Helpers.FormatString("Are you sure you want to sell your {0}? You will receive your vehicle token in return.", vehicleName);
        }

        GenericPromptHelper.ShowPrompt(
            "Premium Deluxe Motorsport",
            message,
            "Yes, sell it",
            "No, keep it",
            UIEventID.VehicleCrusher_Crush, 
            UIEventID.VehicleCrusher_Exit
        );
    }

	public void CrushVehicle()
	{
		NetworkEventSender.SendNetworkEvent_VehicleCrusher_CrushVehicle();
	}
}
