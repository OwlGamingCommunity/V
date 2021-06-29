using RAGE;
using System;

public class MarijuanaSystem
{
	private readonly Vector3 PED_POSITION = new Vector3(-1274.11f, -1416.2216f, 4.332273f);
	private const float PED_ROTATION = 128.0f;
	private const int PED_DIMENSION = 0;
	private const uint PED_MODEL = 0x989DFD9A;
	private const int TIMESTAMP_DAY = 86400;
	private WeakReference<CWorldPed> m_SeedPed;

	private CGUIMarijuanaPed m_MarijuanaUI;

	public MarijuanaSystem()
	{
		m_SeedPed = WorldPedManager.CreatePed(EWorldPedType.MarijuanaSales, PED_MODEL, PED_POSITION, PED_ROTATION, PED_DIMENSION);
		m_SeedPed.Instance()?.AddWorldInteraction(EScriptControlID.Interact, "Purchase Socks", null, OnInteractWithWorldPed, false, false, 3.0f);

		NetworkEvents.ChangeCharacterApproved += OnExit;
		NetworkEvents.Marijuana_WaterNearbyPlant += OnWaterPlant;
		NetworkEvents.Marijuana_FertilizeNearbyPlant += OnFertilizePlant;
		NetworkEvents.Marijuana_SheerNearbyPlant += OnSheerPlant;
		NetworkEvents.Marijuana_CloseMenu += OnExit;

		UIEvents.Marijuana_SellDrugs += OnSellDrugs;
		UIEvents.Marijuana_GetSeeds += OnGetSeeds;
		UIEvents.Marijuana_Exit += OnExit;

		RageEvents.RAGE_OnRender += OnRender;
	}

	private RAGE.Elements.MapObject GetNearestMarijuanaPlant()
	{
		RAGE.Elements.MapObject nearestWorldItem = ItemSystem.GetWorldItems()?.GetNearestWorldItem();
		if (nearestWorldItem == null)
		{
			return null;
		}

		EItemID itemID = DataHelper.GetEntityData<EItemID>(nearestWorldItem, EDataNames.ITEM_ID);
		if (itemID != EItemID.MARIJUANA_PLANT)
		{
			return null;
		}

		return nearestWorldItem;
	}

	private void OnWaterPlant()
	{
		RAGE.Elements.MapObject plant = GetNearestMarijuanaPlant();
		if (plant == null)
		{
			return;
		}

		NetworkEventSender.SendNetworkEvent_Marijuana_OnWater(plant);
	}

	private void OnFertilizePlant()
	{
		RAGE.Elements.MapObject plant = GetNearestMarijuanaPlant();
		if (plant == null)
		{
			return;
		}

		NetworkEventSender.SendNetworkEvent_Marijuana_OnFertilize(plant);
	}

	private void OnSheerPlant()
	{
		RAGE.Elements.MapObject plant = GetNearestMarijuanaPlant();
		if (plant == null)
		{
			return;
		}
		NetworkEventSender.SendNetworkEvent_Marijuana_OnSheer(plant);
	}


	private void OnInteractWithWorldPed()
	{
		m_MarijuanaUI = new CGUIMarijuanaPed(() => { });
		m_MarijuanaUI.SetVisible(true, true, false);
	}

	private void OnSellDrugs(uint count)
	{
		NetworkEventSender.SendNetworkEvent_Marijuana_OnSellDrugs(count);
	}

	private void OnGetSeeds()
	{
		NetworkEventSender.SendNetworkEvent_Marijuana_OnGetSeeds();
	}

	private void OnExit()
	{
		if (m_MarijuanaUI != null)
		{
			m_MarijuanaUI.SetVisible(false, false, false);
			m_MarijuanaUI = null;
		}
	}

	private void OnRender()
	{
		RAGE.Elements.MapObject plant = GetNearestMarijuanaPlant();
		if (plant == null)
		{
			return;
		}

		EGrowthState growthState = DataHelper.GetEntityData<EGrowthState>(plant, EDataNames.MARIJUANA_GROWTH_STAGE);
		string wateredStr = DataHelper.GetEntityData<string>(plant, EDataNames.MARIJUANA_WATERED);
		bool trimmed = DataHelper.GetEntityData<bool>(plant, EDataNames.MARIJUANA_TRIMMED);
		bool fertilized = DataHelper.GetEntityData<bool>(plant, EDataNames.MARIJUANA_FERTILIZED);

		Vector3 position = plant.Position.Add(new Vector3(0, 0, 1));
		Vector2 vecScreenPos = GraphicsHelper.GetScreenPositionFromWorldPosition(position);

		Int64 unixTimestamp = Helpers.GetUnixTimestamp(true);
		Int64 watered = Int64.Parse(wateredStr);
		string needsWatering = (watered > (unixTimestamp - TIMESTAMP_DAY)) || growthState == EGrowthState.FullyGrown
			? "Adequately Watered"
			: "Needs Watering";

		string message = Helpers.FormatString(
			"Water Level: {0}\nFertilized: {1}\nTrimmed: {2}\nGrowth Stage: {3}",
			needsWatering,
			fertilized ? "Yes" : "No",
			trimmed ? "Yes" : "No",
			getGrowthStateString(growthState)
		);

		TextHelper.Draw2D(message, vecScreenPos.X, vecScreenPos.Y, 0.5f, 209, 209, 209, 255, RAGE.Game.Font.ChaletComprimeCologne, RAGE.NUI.UIResText.Alignment.Centered, true, true);
	}

	private string getGrowthStateString(EGrowthState growthState)
	{
		switch (growthState)
		{
			case EGrowthState.Seed:
				return "Seed";
			case EGrowthState.Sapling:
				return "Sapling";
			case EGrowthState.Growing:
				return "Growing";
			case EGrowthState.FullyGrown:
				return "Ready for harvest";
		}

		return "Seed";
	}
}