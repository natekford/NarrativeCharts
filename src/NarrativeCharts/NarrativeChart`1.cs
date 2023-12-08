using NarrativeCharts.Time;

namespace NarrativeCharts;

/// <summary>
/// Base class for making charts that deal with arbitrary time.
/// </summary>
/// <typeparam name="TUnit"></typeparam>
public abstract class NarrativeChart<TUnit> : NarrativeChart
{
	/// <inheritdoc cref="NarrativeChart.Time" />
	public new TimeTrackerWithUnits Time { get; }

	/// <summary>
	/// Creates an instance of <see cref="NarrativeChart{TUnit}"/>.
	/// </summary>
	/// <param name="time"></param>
	protected NarrativeChart(TimeTrackerWithUnits time) : base(time)
	{
		Time = time;
	}

	/// <inheritdoc cref="TimeTrackerUtils.AddUnits{T}(T,int)" />
	public virtual void Jump(int amount = 1)
	{
		Update();
		Time.AddUnits(amount);
		Update();
	}

	/// <summary>
	/// Skips to the start of <paramref name="unit"/> during the current day.
	/// </summary>
	/// <param name="unit"></param>
	public virtual void SkipToCurrentDay(TUnit unit)
		=> SkipToDaysAhead(0, unit);

	/// <summary>
	/// Skips to the start of <paramref name="unit"/> during the day that is
	/// <paramref name="days"/> ahead.
	/// </summary>
	/// <param name="days"></param>
	/// <param name="unit"></param>
	public virtual void SkipToDaysAhead(int days, TUnit unit)
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

	/// <summary>
	/// Skips to the start of <paramref name="unit"/> during the next day.
	/// </summary>
	/// <param name="unit"></param>
	public virtual void SkipToNextDay(TUnit unit)
		=> SkipToDaysAhead(1, unit);

	/// <summary>
	/// Converts <paramref name="unit"/> to its <see cref="int"/> equivalent.
	/// </summary>
	/// <param name="unit"></param>
	/// <returns></returns>
	protected abstract int Convert(TUnit unit);
}