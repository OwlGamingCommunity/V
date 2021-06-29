using System;

public class PaydaySystem
{
	public PaydaySystem()
	{
		CommandManager.RegisterCommand("payday", "Shows how much time is remaining before your next paycheck.", new Action<CPlayer, CVehicle>(CheckPayDay), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "paycheck" });
		CommandManager.RegisterCommand("showpayday", "Shows details of your last paycheck.", new Action<CPlayer, CVehicle>(ShowPayDay), CommandParsingFlags.Default, CommandRequirementsFlags.Default, aliases: new string[] { "viewpayday", "paydayoverview" });
	}

	private void CheckPayDay(CPlayer player, CVehicle vehicle)
	{
		int numMinutesTotal = Constants.TimeBetweenPaydays / 60000;
		int currentMinutes = player.PaydayProgress;
		int minutesRemaining = numMinutesTotal - currentMinutes;
		player.SendNotification("Paycheck", ENotificationIcon.USD, "You have to wait {0} minute(s) more for your paycheck.", minutesRemaining == 0 ? "<1" : minutesRemaining.ToString());
	}

	private void ShowPayDay(CPlayer player, CVehicle vehicle)
	{
		var paydayDetails = player.GetPayDayDetails();

		if (paydayDetails != null)
		{
			NetworkEventSender.SendNetworkEvent_ShowPayDayOverview(player, paydayDetails);
		}
		else
		{
			player.SendNotification("Paycheck", ENotificationIcon.USD, "You have not received a paycheck yet.");
		}
	}
}