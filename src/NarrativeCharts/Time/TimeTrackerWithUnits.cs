using System.Collections.Immutable;

namespace NarrativeCharts.Time;

public class TimeTrackerWithUnits : TimeTracker
{
	public int CurrentUnit => HourToUnitMap[CurrentHour];
	public override int HoursPerDay { get; }
	public IReadOnlyDictionary<int, int> HourToUnitMap { get; }
	public int LargestUnit { get; }
	public IReadOnlyDictionary<int, int> UnitToHourMap { get; }

	public TimeTrackerWithUnits(IEnumerable<int> lengths)
	{
		var i = -1;
		var sum = 0;
		var unitToHourMap = new Dictionary<int, int>();
		foreach (var length in lengths)
		{
			unitToHourMap[++i] = sum;
			sum += length;
		}

		var hourToUnitMap = new Dictionary<int, int>();
		for (var hour = 0; hour < sum; ++hour)
		{
			var unit = 0;
			for (; unit <= i; ++unit)
			{
				if (unitToHourMap[unit] > hour)
				{
					break;
				}
			}
			hourToUnitMap[hour] = unit - 1;
		}

		HoursPerDay = sum;
		LargestUnit = i;
		HourToUnitMap = hourToUnitMap.ToImmutableDictionary();
		UnitToHourMap = unitToHourMap.ToImmutableDictionary();
	}
}