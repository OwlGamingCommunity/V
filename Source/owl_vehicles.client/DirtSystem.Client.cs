public class DirtSystem
{
	public DirtSystem()
	{
		// TODO_POST_LAUNCH: Fix this, this could occur during a clean and reset, shouldn't occur when its being cleaned
		RageEvents.RAGE_OnEntityStreamIn += OnStreamIn;

		RageEvents.AddDataHandler(EDataNames.DIRT, OnDirtLevelChanged);
	}

	private void OnDirtLevelChanged(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			UpdateDirtLevel(vehicle);
		}
	}

	private void OnStreamIn(RAGE.Elements.Entity entity)
	{
		if (entity.Type == RAGE.Elements.Type.Vehicle)
		{
			RAGE.Elements.Vehicle vehicle = (RAGE.Elements.Vehicle)entity;
			UpdateDirtLevel(vehicle);
		}
	}

	private void UpdateDirtLevel(RAGE.Elements.Vehicle vehicle)
	{
		float fDirtLevel = DataHelper.GetEntityData<float>(vehicle, EDataNames.DIRT);
		vehicle.SetDirtLevel(fDirtLevel);
	}
}