using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public class BookwormTimeTracker : TimeTracker
{
	private const int HOURS_PER_DAY = 24;

	public static ImmutableDictionary<BookwormBell, int> BellToHourMap { get; }
	public static ImmutableDictionary<int, BookwormBell> HourToBellMap { get; }

	public BookwormBell CurrentBell => HourToBellMap[CurrentHour];
	public override int HoursPerDay => HOURS_PER_DAY;

	static BookwormTimeTracker()
	{
		// source: https://w.atwiki.jp/booklove/pages/195.html#footnote_body_2
		BellToHourMap = new Dictionary<BookwormBell, int>
		{
			[BookwormBell.Midnight] = 0,
			[BookwormBell.EarlyMorning] = 4,
			[BookwormBell.Morning] = 7,
			[BookwormBell.Meetings] = 10,
			[BookwormBell.Lunch] = 12,
			[BookwormBell.MarketClose] = 15,
			[BookwormBell.Dinner] = 17,
			[BookwormBell.Bed] = 20,
		}.ToImmutableDictionary();

		var hourToBellMap = new Dictionary<int, BookwormBell>();
		for (var hour = 0; hour < HOURS_PER_DAY; ++hour)
		{
			var bell = BookwormBell.Midnight;
			for (; bell <= BookwormBell.Bed; ++bell)
			{
				if (BellToHourMap[bell] > hour)
				{
					break;
				}
			}
			hourToBellMap[hour] = bell - 1;
		}
		HourToBellMap = hourToBellMap.ToImmutableDictionary();
	}

	public BookwormTimeTracker AddBell()
		=> SkipToBell(CurrentBell + 1);

	public BookwormTimeTracker AddBells(int bells)
	{
		var days = bells / BellToHourMap.Count;
		if (days != 0)
		{
			this.AddDays(days);
		}

		bells %= BellToHourMap.Count;
		if (bells == 0)
		{
			return this;
		}

		return SkipToBell(CurrentBell + bells);
	}

	public BookwormTimeTracker SetBell(BookwormBell bell)
		=> this.SetCurrentHour(BellToHourMap[bell]);

	private BookwormTimeTracker SkipToBell(BookwormBell bell)
	{
		if (bell > BookwormBell.Bed)
		{
			this.SkipToNextDayStart();
			bell -= BellToHourMap.Count;
		}
		return SetBell(bell);
	}
}