namespace NarrativeCharts.Time;

public abstract class TimeTracker
{
	public int CurrentDay => (int)CurrentTotalHours / HoursPerDay;
	public int CurrentHour => (int)CurrentTotalHours % HoursPerDay;
	public float CurrentTotalHours { get; protected set; }
	public abstract int HoursPerDay { get; }

	public virtual void SetTotalHours(float totalHours, bool allowBackInTime = false)
	{
		if (totalHours < CurrentTotalHours && !allowBackInTime)
		{
			throw new ArgumentException("Cannot be less than the current time.", nameof(totalHours));
		}
		CurrentTotalHours = totalHours;
	}
}