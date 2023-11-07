using NarrativeCharts.Time;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChartUnits<BookwormBell>
{
	protected BookwormNarrativeChart(TimeTrackerUnits time) : base(time)
	{
		Colors = new(BookwormCharacters.Colors);
		YIndexes = new(BookwormLocations.YIndexes);
	}

	protected override int ConvertToInt(BookwormBell unit)
		=> (int)unit;
}