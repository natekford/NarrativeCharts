using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public enum BookwormBell
{
	Midnight = 0,
	FirstBell = 1,
	EarlyMorning = FirstBell,
	SecondBell = 2,
	Morning = SecondBell,
	ThirdBell = 3,
	Meetings = ThirdBell,
	FourthBell = 4,
	Lunch = FourthBell,
	FifthBell = 5,
	MarketClose = FifthBell,
	Tea = FifthBell,
	SixthBell = 6,
	Dinner = SixthBell,
	SeventhBell = 7,
	Bed = SeventhBell,
}

public static class BookwormTime
{
	// source: https://w.atwiki.jp/booklove/pages/195.html#footnote_body_2
	public static ImmutableArray<int> Lengths { get; } = new[]
	{
		4,
		3,
		3,
		2,
		3,
		2,
		3,
		4,
	}.ToImmutableArray();
}