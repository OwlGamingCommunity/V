
using System;
using System.Collections.Generic;

public class ClientSideDebug
{
	private Dictionary<EClientsideDebugOption, DebugAction> g_DebugOptionsMap = new Dictionary<EClientsideDebugOption, DebugAction>();

	#region Constructor
	public ClientSideDebug()
	{
		PopulateDebugOptionsDict();

		NetworkEvents.ToggleClientSideDebugOption += ToggleClientSideDebugOption;
		RageEvents.RAGE_OnTick_PerFrame += RageOnTickPerFrame;
	}
	#endregion


	private void RageOnTickPerFrame()
	{
		foreach (var debugAction in g_DebugOptionsMap)
		{
			if (debugAction.Value.Enabled)
			{
				debugAction.Value.A();
			}
		}
	}

	private void ToggleClientSideDebugOption(EClientsideDebugOption debugOption)
	{
		// Toggle
		g_DebugOptionsMap[debugOption].Enabled = !g_DebugOptionsMap[debugOption].Enabled;
	}

	private void PopulateDebugOptionsDict()
	{
		var options = (EClientsideDebugOption[])Enum.GetValues(typeof(EClientsideDebugOption));
		foreach (var debugOption in options)
		{
			g_DebugOptionsMap.Add(debugOption, new DebugAction(ActionFromDebugOption(debugOption)));
		}
	}

	#region Helpers
	private Action ActionFromDebugOption(EClientsideDebugOption option)
	{
		switch (option)
		{
			case EClientsideDebugOption.DrawStreamedEntitiesCount:
				return Action_DrawStreamedEntitiesCount;
			case EClientsideDebugOption.DrawPlayerBox:
				return Action_DrawPlayerBox;
			// Empty method so we don't have to throw. We check this serverside so we shouldn't worry about it.
			default:
				return () => { };
		}
	}
	#endregion

	#region Actions
	private void Action_DrawPlayerBox()
	{
		var streamedPlayers = RAGE.Elements.Entities.Players.Streamed;
		foreach (var player in streamedPlayers)
		{
			var vec2 = GraphicsHelper.GetScreenPositionFromWorldPosition(player.Position);
			RAGE.Game.Graphics.DrawRect(vec2.X, vec2.Y, 0.03f, 0.30f, 0, 255, 0, 35, 0);
		}
	}

	private void Action_DrawStreamedEntitiesCount()
	{
		var streamedPlayers = RAGE.Elements.Entities.Players.Streamed;
		var streamedVehicles = RAGE.Elements.Entities.Vehicles.Streamed;
		var streamedObjects = RAGE.Elements.Entities.Objects.Streamed;

		string strToDraw = $"Streamed Players: {streamedPlayers.Count}\nStreamed Vehicles: {streamedVehicles.Count}\nStreamed Objects: {streamedObjects.Count}";

		TextHelper.Draw2D(strToDraw, 0.5f, 0.8f, 0.4f, 255, 255, 255, 255, RAGE.Game.Font.ChaletLondon, RAGE.NUI.UIResText.Alignment.Centered, true, true);
	}
	#endregion
}

#region Debug Action Container
public class DebugAction
{
	public DebugAction(Action action)
	{
		A = action;
		Enabled = false;
	}

	public Action A { get; }
	public bool Enabled { get; set; }
}
#endregion
