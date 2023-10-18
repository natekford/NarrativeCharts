using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static void Add(this NarrativeChartGenerator chart, NarrativeChartGenerator other)
	{
		foreach (var (_, points) in other.NarrativePoints)
		{
			foreach (var point in points)
			{
				chart.AddPoint(point);
			}
		}
	}

	public static NarrativeScene With(this Point point, params string[] characters)
		=> new(point, characters);
}