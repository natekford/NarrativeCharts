﻿using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChartWithUnits<TUnit> : NarrativeChart
{
	protected new TimeTrackerWithUnits Time { get; }

	protected NarrativeChartWithUnits(TimeTrackerWithUnits time) : base(time)
	{
		Time = time;
	}

	protected abstract int ConvertToInt(TUnit unit);

	protected void Jump(int amount = 1)
	{
		Update();
		Time.AddUnits(amount);
	}

	protected void SkipToCurrentDay(TUnit unit)
		=> SkipToDaysAhead(0, unit);

	protected void SkipToDaysAhead(int days, TUnit unit)
	{
		Time.SkipToDaysAheadStart(days).SetCurrentUnit(ConvertToInt(unit) - 1);
		Jump();
	}

	protected void SkipToNextDay(TUnit unit)
		=> SkipToDaysAhead(1, unit);
}