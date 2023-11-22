namespace NarrativeCharts.Time;

/// <summary>
/// Deals with hours ellapsed and arbitrary day lengths.
/// </summary>
public class TimeTracker
{
	/// <summary>
	/// The current day retrieved from <see cref="CurrentTotalHours"/>.
	/// </summary>
	public int CurrentDay => (int)CurrentTotalHours / HoursPerDay;
	/// <summary>
	/// The current hour retrieved from <see cref="CurrentTotalHours"/>.
	/// </summary>
	public int CurrentHour => (int)CurrentTotalHours % HoursPerDay;
	/// <summary>
	/// The actual time.
	/// </summary>
	public float CurrentTotalHours { get; protected set; }
	/// <summary>
	/// The amount of hours per day.
	/// </summary>
	public virtual int HoursPerDay { get; }

	/// <summary>
	/// Creates an instance of <see cref="TimeTracker"/>.
	/// </summary>
	/// <param name="hoursPerDay"></param>
	public TimeTracker(int hoursPerDay = 24)
	{
		HoursPerDay = hoursPerDay;
	}

	/// <summary>
	/// Sets <see cref="CurrentTotalHours"/> to <paramref name="totalHours"/>.
	/// </summary>
	/// <param name="totalHours"></param>
	/// <param name="allowBackInTime"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public virtual void SetTotalHours(float totalHours, bool allowBackInTime = false)
	{
		if (totalHours < CurrentTotalHours && !allowBackInTime)
		{
			throw new ArgumentOutOfRangeException(nameof(totalHours),
				"Cannot be less than the current time.");
		}
		CurrentTotalHours = totalHours;
	}
}