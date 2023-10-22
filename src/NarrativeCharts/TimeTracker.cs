namespace NarrativeCharts;

public abstract class TimeTracker
{
	public int CurrentDay => CurrentTotalHours / HoursPerDay;
	public int CurrentHour => CurrentTotalHours % HoursPerDay;
	public int CurrentTotalHours { get; protected set; }
	public abstract int HoursPerDay { get; }

	public virtual void SetTotalHours(int totalHours)
	{
		if (totalHours < CurrentTotalHours)
		{
			throw new ArgumentException("Cannot be less than the current time.", nameof(totalHours));
		}
		CurrentTotalHours = totalHours;
	}
}

public static class TimeTrackerUtils
{
	public static T AddDay<T>(this T time) where T : TimeTracker
		=> time.AddDays(1);

	public static T AddDays<T>(this T time, int days) where T : TimeTracker
		=> time.AddHours(time.HoursPerDay * days);

	public static T AddHour<T>(this T time) where T : TimeTracker
		=> time.AddHours(1);

	public static T AddHours<T>(this T time, int hours) where T : TimeTracker
	{
		time.SetTotalHours(time.CurrentTotalHours + hours);
		return time;
	}

	public static T SetCurrentHour<T>(this T time, int hour) where T : TimeTracker
		=> time.AddHours(hour - time.CurrentHour);

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
}