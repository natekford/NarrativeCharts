namespace NarrativeCharts.Bookworm;

public readonly struct BookwormBellMover
{
	private readonly int _DayDifference;
	private readonly BookwormTimeTracker _Tracker;

	public BookwormBellMover(BookwormTimeTracker tracker, int dayDifference)
	{
		_Tracker = tracker;
		_DayDifference = dayDifference;
	}

	public BookwormTimeTracker Bed()
		=> Move(BookwormBell.Bed);

	public BookwormTimeTracker Dinner()
		=> Move(BookwormBell.Dinner);

	public BookwormTimeTracker EarlyMorning()
		=> Move(BookwormBell.EarlyMorning);

	public BookwormTimeTracker Lunch()
		=> Move(BookwormBell.Lunch);

	public BookwormTimeTracker MarketClose()
		=> Move(BookwormBell.MarketClose);

	public BookwormTimeTracker Meetings()
		=> Move(BookwormBell.Meetings);

	public BookwormTimeTracker Morning()
		=> Move(BookwormBell.Morning);

	public BookwormTimeTracker Move(BookwormBell bell)
	{
		if (_DayDifference == 0)
		{
			_Tracker.GoToBellOfCurrentDay(bell);
			return _Tracker;
		}

		_Tracker.GoToBellOfNextDay(bell);
		if (_DayDifference > 1)
		{
			_Tracker.AddDays(_DayDifference - 1);
		}
		return _Tracker;
	}
}