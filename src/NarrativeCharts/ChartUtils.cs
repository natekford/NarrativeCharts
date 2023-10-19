using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static void AddChart(this NarrativeChart chart, NarrativeChart other)
	{
		foreach (var @event in other.Events)
		{
			chart.AddEvent(@event.Value);
		}
		foreach (var point in other.GetAllNarrativePoints())
		{
			chart.AddPoint(point);
		}
	}

	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(this NarrativeChart chart)
	{
		foreach (var (_, points) in chart.Points)
		{
			foreach (var point in points)
			{
				yield return point.Value;
			}
		}
	}
}