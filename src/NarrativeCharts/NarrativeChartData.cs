using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChartData
{
	public Dictionary<Character, Hex> Colors { get; set; } = [];
	public SortedList<int, NarrativeEvent> Events { get; set; } = [];
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<int, NarrativePoint>> Points { get; set; } = [];
	public Dictionary<Location, int> YIndexes { get; set; } = [];

	public virtual void Simplify()
	{
		foreach (var (_, points) in Points)
		{
			// Don't bother checking the first or last points
			// They will always be valid
			for (var i = points.Count - 2; i > 0; --i)
			{
				var prev = points.Values[i - 1];
				var curr = points.Values[i];
				var next = points.Values[i + 1];

				// keep a point at the start and end of each timeskip
				if (prev.IsTimeSkip || curr.IsTimeSkip)
				{
					continue;
				}

				var pLoc = prev.Location;
				var cLoc = curr.Location;
				var nLoc = next.Location;
				if (pLoc == cLoc && cLoc == nLoc)
				{
					points.Remove(points.Values[i].Hour);
				}
			}
		}
	}
}