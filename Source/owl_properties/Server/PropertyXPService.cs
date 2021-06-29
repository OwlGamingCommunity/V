using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PropertyXPService
{
	private const uint MAX_XP_DECREMENT = 80;
	private const uint MIN_XP_DECREMENT = 50;

	private const ulong CACHE_NOT_SET_VALUE = 1;
	private ulong _cachedWeekTimeStamp = CACHE_NOT_SET_VALUE;

	private static Random _random = new Random();

	public async Task<bool> NeedsXPRun()
	{
		// returns whether we need to run the xp decrement
		await UpdateCache().ConfigureAwait(true);

		return CACHE_NOT_SET_VALUE != _cachedWeekTimeStamp && _cachedWeekTimeStamp < (ulong)Helpers.GetUnixTimestamp();
	}
	
	private async Task UpdateCache()
	{
		if(_cachedWeekTimeStamp == CACHE_NOT_SET_VALUE)
		{
			_cachedWeekTimeStamp = await GetWeekTimeStampFromDatabase().ConfigureAwait(true);
		}
	}

	private async Task<ulong> GetWeekTimeStampFromDatabase()
	{
		return await Database.LegacyFunctions.GetNextPropertyXPRunAt().ConfigureAwait(true);
	}

	private async Task TouchWeekTimeStamp()
	{
		await Database.LegacyFunctions.TouchNextPropertyRunAt().ConfigureAwait(true);
		_cachedWeekTimeStamp = (ulong)Helpers.GetUnixTimestamp();
	}

	public async Task RunXPCheck()
	{
		if (!await NeedsXPRun().ConfigureAwait(true))
		{
			return;
		}

		List<CPropertyInstance> properties = PropertyPool.GetAllPropertyInstances();

		Console.WriteLine(Helpers.FormatString("[PROPERTY XP] Running property XP decrements for {0} properties", properties.Count));

		foreach (var property in properties)
		{
			property.Model.DecrementWeeklyXP(GetRandomXP());
		}

		Console.WriteLine("[PROPERTY XP] Completed property XP decrements");

		await TouchWeekTimeStamp().ConfigureAwait(true);
	}

	private static uint GetRandomXP()
	{
		return (uint)(_random.NextDouble() * (MAX_XP_DECREMENT - MIN_XP_DECREMENT) + MIN_XP_DECREMENT);
	}
}

