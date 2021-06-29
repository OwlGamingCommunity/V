using System;

public class RecentRobbery
{
	private const int MINIMUM_POLICE_CALL_MILLIS = 15000;
	private const int MAXIMUM_POLICE_CALL_MILLIS = 120000;
	private const int MINIMUM_ROBBERY_MONEY = 400;
	private const int MAXIMUM_ROBBERY_MONEY = 3000;

	public RecentRobbery(Int64 storeID, CPlayer player)
	{
		StoreID = storeID;
		PlayerName = player.GetCharacterName(ENameType.StaticCharacterName);
	}

	public int GetMoney()
	{
		if (MoneyGiven > 0)
		{
			return MoneyGiven;
		}

		return MoneyGiven = RandomBetween(MINIMUM_ROBBERY_MONEY, MAXIMUM_ROBBERY_MONEY);
	}

	public int CallPoliceTimer()
	{
		if (TimeToCallPolice > 0)
		{
			return TimeToCallPolice;
		}

		return TimeToCallPolice = RandomBetween(MINIMUM_POLICE_CALL_MILLIS, MAXIMUM_POLICE_CALL_MILLIS);
	}

	private int RandomBetween(int min, int max)
	{
		return new Random().Next(min, max);
	}

	public int TimeToCallPolice { get; set; }
	public int MoneyGiven { get; set; }
	public Int64 StoreID { get; }
	public string PlayerName { get; }
	public bool Cancelled { get; set; }
}
