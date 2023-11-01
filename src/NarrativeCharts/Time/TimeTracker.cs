﻿namespace NarrativeCharts.Time;

public abstract class TimeTracker
{
	public int CurrentDay => CurrentTotalHours / HoursPerDay;
	public int CurrentHour => CurrentTotalHours % HoursPerDay;
	public int CurrentTotalHours { get; protected set; }
	public abstract int HoursPerDay { get; }

	public virtual void SetTotalHours(int totalHours, bool allowBackInTime = false)
	{
		if (totalHours < CurrentTotalHours && !allowBackInTime)
		{
			throw new ArgumentException("Cannot be less than the current time.", nameof(totalHours));
		}
		CurrentTotalHours = totalHours;
	}
}