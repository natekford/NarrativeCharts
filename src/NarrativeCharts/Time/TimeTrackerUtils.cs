namespace NarrativeCharts.Time;

/// <summary>
/// Utilities for <see cref="TimeTracker"/>.
/// </summary>
public static class TimeTrackerUtils
{
	/// <summary>
	/// Adds 1 day.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <returns></returns>
	public static T AddDay<T>(this T time) where T : TimeTracker
		=> time.AddDays(1);

	/// <summary>
	/// Adds multiple days.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="days"></param>
	/// <returns></returns>
	public static T AddDays<T>(this T time, int days) where T : TimeTracker
		=> time.AddHours(time.HoursPerDay * days);

	/// <summary>
	/// Adds 1 hour.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <returns></returns>
	public static T AddHour<T>(this T time) where T : TimeTracker
		=> time.AddHours(1);

	/// <summary>
	/// Adds multiple hours.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="hours"></param>
	/// <returns></returns>
	public static T AddHours<T>(this T time, float hours) where T : TimeTracker
	{
		time.SetTotalHours(time.CurrentTotalHours + hours);

		return time;
	}

	/// <summary>
	/// Adds 1 unit.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <returns></returns>
	public static T AddUnit<T>(this T time) where T : TimeTrackerWithUnits
		=> time.SkipToUnit(time.CurrentUnit + 1);

	/// <summary>
	/// Adds multiple units.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="units"></param>
	/// <returns></returns>
	public static T AddUnits<T>(this T time, int units) where T : TimeTrackerWithUnits
	{
		if (units == 0)
		{
			return time;
		}

		var days = units / time.UnitToHourMap.Count;
		if (days != 0)
		{
			time.AddDays(days);
		}

		units %= time.UnitToHourMap.Count;
		if (units != 0)
		{
			time.SkipToUnit(time.CurrentUnit + units);
		}

		return time;
	}

	/// <summary>
	/// Sets <see cref="TimeTracker.CurrentHour"/> to <paramref name="hour"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="hour"></param>
	/// <returns></returns>
	public static T SetCurrentHour<T>(this T time, float hour) where T : TimeTracker
	{
		if (hour < 0 || hour > time.HoursPerDay)
		{
			throw new ArgumentOutOfRangeException(nameof(hour));
		}

		return time.AddHours(hour - time.CurrentHour);
	}

	/// <summary>
	/// Sets <see cref="TimeTrackerWithUnits.CurrentUnit"/> to <paramref name="unit"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="unit"></param>
	/// <returns></returns>
	public static T SetCurrentUnit<T>(this T time, int unit) where T : TimeTrackerWithUnits
	{
		if (unit < 0 || unit > time.LargestUnit)
		{
			throw new ArgumentOutOfRangeException(nameof(unit));
		}

		return time.SetCurrentHour(time.UnitToHourMap[unit]);
	}

	/// <summary>
	/// Skips to the start of the next day.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <returns></returns>
	public static T SkipToDayAheadStart<T>(this T time) where T : TimeTracker
		=> time.SkipToDaysAheadStart(1);

	/// <summary>
	/// Skips to the start of a future day.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="time"></param>
	/// <param name="days"></param>
	/// <returns></returns>
	public static T SkipToDaysAheadStart<T>(this T time, int days) where T : TimeTracker
	{
		if (days == 0)
		{
			return time;
		}

		var dayHours = time.HoursPerDay;
		var eodHours = dayHours - (time.CurrentTotalHours % dayHours);
		if (eodHours != 0)
		{
			--days;
		}

		return time.AddHours((time.HoursPerDay * days) + eodHours);
	}

	private static T SkipToUnit<T>(this T time, int unit) where T : TimeTrackerWithUnits
	{
		if (unit > time.LargestUnit)
		{
			time.SkipToDayAheadStart();
			unit -= time.UnitToHourMap.Count;
		}

		return time.SetCurrentUnit(unit);
	}
}