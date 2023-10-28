using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChartUnits<TUnit> : NarrativeChart
{
	protected new TimeTrackerUnits Time { get; }

	protected NarrativeChartUnits(TimeTrackerUnits time) : base(time)
	{
		Time = time;
	}

	protected void AddUnit(int amount = 1)
	{
		Update();
		Time.AddUnits(amount);
	}

	protected abstract int ConvertToInt(TUnit unit);

	protected void SkipToCurrentDay(TUnit unit)
		=> SkipToDaysAhead(0, unit);

	protected void SkipToDaysAhead(int days, TUnit unit)
	{
		Time.SkipToDaysAheadStart(days).SetUnit(ConvertToInt(unit) - 1);
		AddUnit();
	}

	protected void SkipToNextDay(TUnit unit)
		=> SkipToDaysAhead(1, unit);
}