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

	public BookwormTimeTracker AddBell()
	{
		if (CurrentBell == Bells.Length - 1)
		{
			SkipToStartOfNextDay();
		}
		else
		{
			SkipToBellOfCurrentDay((BookwormBell)(CurrentBell + 1));
		}
		return this;
	}

	public BookwormTimeTracker AddBells(int bells)
	{
		// cba to think of the logic to do this without a loop
		for (var i = 0; i < bells; ++i)
		{
			AddBell();
		}
		return this;
	}

	public BookwormTimeTracker AddDay()
	{
		CurrentTotalHours += HOURS_PER_DAY;
		return this;
	}

	public BookwormTimeTracker AddDays(int days)
	{
		if (days < 0)
		{
			throw new InvalidOperationException("Not allowed to go back in time.");
		}

		CurrentTotalHours += HOURS_PER_DAY * days;
		return this;
	}

	public BookwormTimeTracker SkipToBellOfCurrentDay(BookwormBell bell)
	{
		var desiredHour = Bells[(int)bell];
		var currentHour = CurrentTotalHours % HOURS_PER_DAY;
		if (currentHour > desiredHour)
		{
			throw new InvalidOperationException("Not allowed to go back in time.");
		}

		CurrentTotalHours += desiredHour - currentHour;
		return this;
	}

	public BookwormTimeTracker SkipToBellOfNextDay(BookwormBell bell)
	{
		SkipToStartOfNextDay();
		CurrentTotalHours += Bells[(int)bell];
		return this;
	}

	public BookwormBellMover SkipToDaysAhead(int days)
		=> new(this, dayDifference: days);

	public BookwormTimeTracker SkipToStartOfNextDay()
	{
		CurrentTotalHours += HOURS_PER_DAY - (CurrentTotalHours % HOURS_PER_DAY);
		return this;
	}
}