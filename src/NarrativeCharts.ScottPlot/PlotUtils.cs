using ScottPlot;

using System.Collections.Concurrent;

namespace NarrativeCharts.ScottPlot;

public static class PlotUtils
{
	public static void PlotChart(this NarrativeChart chart, int width, int height, string path)
	{
		var plot = new Plot(width, height);

		var dupes = new ConcurrentDictionary<(double, double), int>();
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Value.Count))
		{
			var xs1 = new double[points.Count];
			var ys1 = new double[points.Count];
			for (var p = 0; p < points.Count; ++p)
			{
				var x = points.Values[p].Point.X;
				var y = points.Values[p].Point.Y;
				var shift = dupes.AddOrUpdate((x, y), _ => 1, (_, v) => ++v);
				xs1[p] = x;
				ys1[p] = y + (shift * 3);
			}

			var color = System.Drawing.ColorTranslator.FromHtml(chart.Colors[character]);
			plot.AddScatter(xs1, ys1, color: color, markerSize: 5, label: character);
		}

		// Y-Labels (locations)
		{
			var orderedLocs = chart.Locations.OrderBy(x => x.Value);
			var locationPositions = orderedLocs.Select(x => (double)x.Value).ToArray();
			var locations = orderedLocs.Select(x => x.Key).ToArray();

			plot.YAxis.ManualTickPositions(locationPositions, locations);
		}

		// X-Labels (events/chapters)
		{
			var orderedEvents = chart.Events.OrderBy(x => x.Key);
			var eventPositions = orderedEvents.Select(x => (double)x.Key).ToArray();
			var events = orderedEvents.Select(x => x.Value.Name).ToArray();

			plot.XAxis.ManualTickPositions(eventPositions, events);
		}

		plot.Legend();
		plot.SaveFig(path);
	}
}