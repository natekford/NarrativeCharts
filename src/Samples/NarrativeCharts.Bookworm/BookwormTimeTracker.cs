using NarrativeCharts.Time;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public sealed class BookwormTimeTracker : TimeTrackerUnits
{
	// source: https://w.atwiki.jp/booklove/pages/195.html#footnote_body_2
	private static readonly int[] _Lengths = new[]
	{
		4,
		3,
		3,
		2,
		3,
		2,
		3,
		4,
	};

	public BookwormTimeTracker() : base(_Lengths)
	{
	}
}