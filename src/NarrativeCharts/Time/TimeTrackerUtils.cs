namespace NarrativeCharts.Time;

public static class TimeTrackerUtils
{
	public static T AddDay<T>(this T time) where T : TimeTracker
		=> time.AddDays(1);

	public static T AddDays<T>(this T time, int days) where T : TimeTracker
		=> time.AddHours(time.HoursPerDay * days);

	public static T AddHour<T>(this T time) where T : TimeTracker
		=> time.AddHours(1);

	public static T AddHours<T>(this T time, float hours) where T : TimeTracker
	{
		time.SetTotalHours(time.CurrentTotalHours + hours);
		return time;
	}

	public static T AddUnit<T>(this T time) where T : TimeTrackerWithUnits
		=> time.SkipToUnit(time.CurrentUnit + 1);

	public static T AddUnits<T>(this T time, int units) where T : TimeTrackerWithUnits
	{
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

	public static T SetCurrentHour<T>(this T time, float hour) where T : TimeTracker
		=> time.AddHours(hour - time.CurrentHour);

	public static T SetCurrentUnit<T>(this T time, int unit) where T : TimeTrackerWithUnits
		=> time.SetCurrentHour(time.UnitToHourMap[unit]);

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

	public static T SkipToNextDayStart<T>(this T time) where T : TimeTracker
		=> time.SkipToDaysAheadStart(1);

	private static T SkipToUnit<T>(this T time, int unit) where T : TimeTrackerWithUnits
	{
		if (unit > time.LargestUnit)
		{
			time.SkipToNextDayStart();
			unit -= time.UnitToHourMap.Count;
		}
		return time.SetCurrentUnit(unit);
	}
}