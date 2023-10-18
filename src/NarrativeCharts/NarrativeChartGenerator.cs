using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChartGenerator
{
	private static readonly Comparer<NarrativePoint> _PointComparer = Comparer<NarrativePoint>.Create((a, b) => a.Point.X.CompareTo(b.Point.X));

	public Dictionary<string, SortedSet<NarrativePoint>> NarrativePoints { get; } = new();

	public void AddPoint(NarrativePoint point)
	{
		if (!NarrativePoints.TryGetValue(point.Character, out var set))
		{
			NarrativePoints[point.Character] = set = new(_PointComparer);
		}
		set.Add(point);
	}
}