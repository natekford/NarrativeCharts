using System.Collections.Immutable;

namespace NarrativeCharts.Time;

/// <summary>
/// Deals with arbitrary unit lengths (supports uniform and non-uniform lengths).
/// </summary>
public class TimeTrackerWithUnits : TimeTracker
{
	/// <summary>
	/// The current unit retrieved from <see cref="TimeTracker.CurrentTotalHours"/>.
	/// </summary>
	public int CurrentUnit => HourToUnitMap[CurrentHour];
	/// <summary>
	/// Maps hours to their respective units.
	/// E.G. a unit start at 7h and ends at 10h: the hours 7, 8, and 9 map to that unit.
	/// </summary>
	public IReadOnlyDictionary<int, int> HourToUnitMap { get; }
	/// <summary>
	/// The index of the largest unit.
	/// </summary>
	public int LargestUnit { get; }
	/// <summary>
	/// Maps units to their respective start hour.
	/// </summary>
	public IReadOnlyDictionary<int, int> UnitToHourMap { get; }

	/// <summary>
	/// Creates an instance of <see cref="TimeTrackerWithUnits"/>.
	/// </summary>
	/// <param name="lengths"></param>
	public TimeTrackerWithUnits(IEnumerable<int> lengths) : base(lengths.Sum())
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

		LargestUnit = i;
		HourToUnitMap = hourToUnitMap.ToImmutableDictionary();
		UnitToHourMap = unitToHourMap.ToImmutableDictionary();
	}
}