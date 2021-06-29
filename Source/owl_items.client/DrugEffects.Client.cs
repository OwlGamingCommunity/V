using System;

// TODO_CSHARP: Check impairment timers still work
public class DrugEffects
{
	public DrugEffects()
	{
		ClientTimerPool.CreateTimer(UpdateLocalDrugEffects, 3000);

		RageEvents.AddDataHandler(EDataNames.IMPAIRMENT, Data_UpdateImpairment);
		RageEvents.AddDataHandler(EDataNames.DRUG_FX_1, Data_DrugFX1);
		RageEvents.AddDataHandler(EDataNames.DRUG_FX_2, Data_DrugFX2);
		RageEvents.AddDataHandler(EDataNames.DRUG_FX_3, Data_DrugFX3);
		RageEvents.AddDataHandler(EDataNames.DRUG_FX_4, Data_DrugFX4);
		RageEvents.AddDataHandler(EDataNames.DRUG_FX_5, Data_DrugFX5);
	}

	// TODO_POST_LAUNCH: Async loader for sprites + anim clipsets

	private void Data_DrugFX1(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity != null && entity.Type == RAGE.Elements.Type.Player && entity == RAGE.Elements.Player.LocalPlayer)
		{
			bool bEnabled = (bool)Convert.ChangeType(newValue, typeof(bool));
			if (bEnabled)
			{
				UpdateLocalDrugEffects();
			}
			else
			{
				ShutdownDrugEffect_FX1();
			}
		}
	}

	private void Data_DrugFX2(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player && entity == RAGE.Elements.Player.LocalPlayer)
		{
			bool bEnabled = (bool)newValue;
			if (bEnabled)
			{
				UpdateLocalDrugEffects();
			}
			else
			{
				ShutdownDrugEffect_FX2();
			}
		}
	}

	private void Data_DrugFX3(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player && entity == RAGE.Elements.Player.LocalPlayer)
		{
			bool bEnabled = (bool)newValue;
			if (bEnabled)
			{
				UpdateLocalDrugEffects();
			}
			else
			{
				ShutdownDrugEffect_FX3();
			}
		}
	}

	private void Data_DrugFX4(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player && entity == RAGE.Elements.Player.LocalPlayer)
		{
			bool bEnabled = (bool)newValue;
			if (bEnabled)
			{
				UpdateLocalDrugEffects();
			}
			else
			{
				ShutdownDrugEffect_FX4();
			}
		}
	}

	private void Data_DrugFX5(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player && entity == RAGE.Elements.Player.LocalPlayer)
		{
			bool bEnabled = (bool)newValue;
			if (bEnabled)
			{
				UpdateLocalDrugEffects();
			}
			else
			{
				ShutdownDrugEffect_FX5();
			}
		}
	}

	private void ShutdownDrugEffect_FX1()
	{
		RAGE.Game.Graphics.StopScreenEffect("DrugsDrivingIn");
	}
	private void ShutdownDrugEffect_FX2()
	{
		RAGE.Game.Graphics.StopScreenEffect("DrugsMichaelAliensFight");
	}

	private void ShutdownDrugEffect_FX3()
	{
		RAGE.Game.Graphics.StopScreenEffect("DrugsTrevorClownsFight");
	}

	private void ShutdownDrugEffect_FX4()
	{
		RAGE.Game.Graphics.StopScreenEffect("DeadlineNeon");
	}

	private void ShutdownDrugEffect_FX5()
	{
		RAGE.Game.Graphics.StopScreenEffect("BeastLaunch");
	}

	private void Data_UpdateImpairment(RAGE.Elements.Entity entity, object newValue, object oldValue)
	{
		if (entity.Type == RAGE.Elements.Type.Player)
		{
			if (entity == RAGE.Elements.Player.LocalPlayer)
			{
				UpdateLocalDrunkAndDrugsEffects();
			}
		}
	}

	private void UpdateLocalDrugEffects(object[] parameters = null)
	{
		bool bDrugFX1 = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.DRUG_FX_1);
		bool bDrugFX2 = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.DRUG_FX_2);
		bool bDrugFX3 = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.DRUG_FX_3);
		bool bDrugFX4 = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.DRUG_FX_4);
		bool bDrugFX5 = DataHelper.GetLocalPlayerEntityData<bool>(EDataNames.DRUG_FX_5);

		if (bDrugFX1)
		{
			RAGE.Game.Graphics.StartScreenEffect("DrugsDrivingIn", 3000, true);
		}

		if (bDrugFX2)
		{
			RAGE.Game.Graphics.StartScreenEffect("DrugsMichaelAliensFight", 3000, true);
		}

		if (bDrugFX3)
		{
			RAGE.Game.Graphics.StartScreenEffect("DrugsTrevorClownsFight", 3000, true);
		}

		if (bDrugFX4)
		{
			RAGE.Game.Graphics.StartScreenEffect("DeadlineNeon", 3000, true);
		}

		if (bDrugFX5)
		{
			RAGE.Game.Graphics.StartScreenEffect("BeastLaunch", 3000, true);
		}
	}

	private void UpdateLocalDrunkAndDrugsEffects(object[] parameters = null)
	{
		float fImpairmentLevel = DataHelper.GetLocalPlayerEntityData<float>(EDataNames.IMPAIRMENT);

		if (fImpairmentLevel <= 0.3f)
		{
			RAGE.Game.Cam.SetCamEffect(0);
		}
		else if (fImpairmentLevel > 0.3f && fImpairmentLevel <= 0.6f)
		{
			RAGE.Game.Cam.SetCamEffect(1);
		}
		else
		{
			RAGE.Game.Cam.SetCamEffect(2);
		}

		if (fImpairmentLevel > 0.0f)
		{
			ClientTimerPool.CreateTimer(UpdateLocalDrunkAndDrugsEffects, 15000, 1);
		}
	}
}