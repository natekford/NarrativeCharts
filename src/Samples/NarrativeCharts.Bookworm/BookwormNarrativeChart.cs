using NarrativeCharts.Time;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChartUnits<BookwormBell>
{
	protected BookwormNarrativeChart(TimeTrackerUnits time) : base(time)
	{
		foreach (var (key, value) in BookwormCharacters.Colors)
		{
			Colors.Add(key, value);
		}
		foreach (var (key, value) in BookwormLocations.YIndexes)
		{
			YIndexes.Add(key, value);
		}
	}

	protected override int ConvertToInt(BookwormBell unit)
		=> (int)unit;
}