using NarrativeCharts.Models;

using static NarrativeCharts.Console.Characters;
using static NarrativeCharts.Console.Locations;

namespace NarrativeCharts.Console.P3;

public static class P3V2
{
	public static NarrativeChartGenerator Generate(BookwormTimeTracker time)
	{
		var p3v2 = new NarrativeChartGenerator();
		Point Scene(double location) => new(time.CurrentTotalHours, location);

		return p3v2;
	}
}