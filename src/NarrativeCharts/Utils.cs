using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class Utils
{
	public static void Add(this NarrativeChartGenerator chart, NarrativeChartGenerator other)
	{
		foreach (var point in other.GetAllNarrativePoints())
		{
			chart.AddPoint(point);
		}
	}

	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(this NarrativeChartGenerator chart)
	{
		foreach (var (_, points) in chart.NarrativePoints)
		{
			foreach (var point in points)
			{
				yield return point;
			}
		}
	}
}