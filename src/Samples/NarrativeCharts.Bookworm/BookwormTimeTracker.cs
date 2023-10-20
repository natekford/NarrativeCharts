using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public class BookwormTimeTracker
{
	private const int HOURS_PER_DAY = 24;

	public static ImmutableArray<int> Bells { get; }
	public static ImmutableArray<int> HourToBellMap { get; }

	public int CurrentBell => HourToBellMap[CurrentHour];
	public int CurrentDay => CurrentTotalHours / HOURS_PER_DAY;
	public int CurrentHour => CurrentTotalHours % HOURS_PER_DAY;
	public int CurrentTotalHours { get; private set; }
	public BookwormBellMover GoToCurrentDay { get; }
	public BookwormBellMover GoToNextDay { get; }

	static BookwormTimeTracker()
	{
		// source: https://w.atwiki.jp/booklove/pages/195.html#footnote_body_2
		Bells = new int[]
		{
			0,
			4,
			7,
			10,
			12,
			15,
			17,
			20,
		}.ToImmutableArray();

		var hourToBellMap = new int[HOURS_PER_DAY];
		for (var hour = 0; hour < HOURS_PER_DAY; ++hour)
		{
			var bell = 0;
			for (; bell < Bells.Length; ++bell)
			{
				if (Bells[bell] > hour)
				{
					break;
				}
			}
			hourToBellMap[hour] = bell - 1;
		}
		HourToBellMap = hourToBellMap.ToImmutableArray();
	}

	public BookwormTimeTracker()
	{
		GoToCurrentDay = new BookwormBellMover(this, dayDifference: 0);
		GoToNextDay = new BookwormBellMover(this, dayDifference: 1);
	}

	public void AddBell()
	{
		if (CurrentBell == Bells.Length - 1)
		{
			GoToStartOfNextDay();
		}
		else
		{
			GoToBellOfCurrentDay(CurrentBell + 1);
		}
	}

	public void AddBells(int bells)
	{
		// cba to think of the logic to do this without a loop
		for (var i = 0; i < bells; ++i)
		{
			AddBell();
		}
	}

	public void AddDay()
		=> CurrentTotalHours += HOURS_PER_DAY;

	public void AddDays(int days)
	{
		if (days < 0)
		{
			throw new InvalidOperationException("Not allowed to go back in time.");
		}

		CurrentTotalHours += HOURS_PER_DAY * days;
	}

	public void GoToBellOfCurrentDay(int bell)
	{
		var desiredHour = Bells[bell];
		var currentHour = CurrentTotalHours % HOURS_PER_DAY;
		if (currentHour > desiredHour)
		{
			throw new InvalidOperationException("Not allowed to go back in time.");
		}

		CurrentTotalHours += desiredHour - currentHour;
	}

	public void GoToBellOfNextDay(int bell)
	{
		GoToStartOfNextDay();
		CurrentTotalHours += Bells[bell];
	}

	public BookwormBellMover GoToDaysAhead(int days)
		=> new(this, dayDifference: days);

	public void GoToStartOfNextDay()
		=> CurrentTotalHours += HOURS_PER_DAY - (CurrentTotalHours % HOURS_PER_DAY);

	public readonly struct BookwormBellMover
	{
		private readonly int _DayDifference;
		private readonly BookwormTimeTracker _Tracker;

		public BookwormBellMover(BookwormTimeTracker tracker, int dayDifference)
		{
			_Tracker = tracker;
			_DayDifference = dayDifference;
		}

		public void Bed()
			=> Move(7);

		public void Dinner()
			=> Move(6);

		public void EarlyMorning()
			=> Move(1);

		public void Lunch()
			=> Move(4);

		public void MarketClose()
			=> Move(5);

		public void Meetings()
			=> Move(3);

		public void Morning()
			=> Move(2);

		private void Move(int bell)
		{
			if (_DayDifference == 0)
			{
				_Tracker.GoToBellOfCurrentDay(bell);
				return;
			}

			_Tracker.GoToBellOfNextDay(bell);
			if (_DayDifference > 1)
			{
				_Tracker.AddDays(_DayDifference - 1);
			}
		}
	}
}