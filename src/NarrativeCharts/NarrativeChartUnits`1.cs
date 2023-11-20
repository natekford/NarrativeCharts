﻿using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChartWithUnits<TUnit> : NarrativeChart
{
	protected new TimeTrackerWithUnits Time { get; }

	protected NarrativeChartWithUnits(TimeTrackerWithUnits time) : base(time)
	{
		Time = time;
	}

	protected abstract int Convert(TUnit unit);

	protected virtual void Jump(int amount = 1)
	{
		Update();
		Time.AddUnits(amount);
		Update();
	}

	protected virtual void SkipToCurrentDay(TUnit unit)
		=> SkipToDaysAhead(0, unit);

	protected virtual void SkipToDaysAhead(int days, TUnit unit)
	{
		Update();
		Time.SkipToDaysAheadStart(days);

		var currentUnit = Time.CurrentUnit;
		var desiredUnit = Convert(unit);
		// If the desired unit is less than the current unit
		// we'll want to always throw an exception because
		// we don't allow going back in time
		// If the desired unit is equal to the current unit
		// we don't know if the hour is less than the current one
		// so let it potentially throw
		if (desiredUnit <= currentUnit)
		{
			Time.SetCurrentUnit(desiredUnit);
			Update();
			return;
		}

		// advance to the unit right before the desired one
		// so the chart doesn't have a huge jump, just a 1 unit
		// sized jump
		if (currentUnit < desiredUnit - 1)
		{
			Time.SetCurrentUnit(desiredUnit - 1);
		}
		Jump();
	}

	protected virtual void SkipToNextDay(TUnit unit)
		=> SkipToDaysAhead(1, unit);
}