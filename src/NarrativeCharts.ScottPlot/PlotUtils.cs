using ScottPlot.Renderable;

using System.Collections.Concurrent;

namespace NarrativeCharts.Plot;

public static class PlotUtils
{
	public static void PlotChart(this NarrativeChart chart, string path)
	{
		var range = chart.GetRange();
		var width = (int)(range.RangeX * 2.5);
		var height = (int)(range.RangeY * 2.5);
		var plot = new ScottPlot.Plot(width, height);

		var dupes = new ConcurrentDictionary<(double, double), int>();
		foreach (var (character, points) in chart.Points)
		{
			var xs = new double[points.Count];
			var ys = new double[points.Count];
			for (var p = 0; p < points.Count; ++p)
			{
				var x = points.Values[p].Point.X;
				var y = points.Values[p].Point.Y;
				var shift = dupes.AddOrUpdate((x, y), _ => 1, (_, v) => ++v);
				xs[p] = x;
				ys[p] = y + (shift * 3);
			}

			var color = System.Drawing.ColorTranslator.FromHtml(chart.Colors[character]);
			plot.AddScatter(xs, ys, color: color, markerSize: 5, label: character);
		}

		plot.Title(chart.Name);
		plot.Legend();

		plot.LeftAxis.Label("Locations");
		plot.LeftAxis.SetTicks(chart.Locations, x => x.Value, x => x.Key);

		plot.BottomAxis.Label("Chapters");
		plot.BottomAxis.TickLabelStyle(rotation: 15);
		plot.BottomAxis.SetTicks(chart.Events, x => x.Key, x => x.Value.Name);

		plot.SaveFig(path);
	}

	private static void SetTicks<TSource>(
		this Axis axis,
		IReadOnlyCollection<TSource> source,
		Func<TSource, double> getPosition,
		Func<TSource, string> getLabel)
	{
		var positions = new double[source.Count];
		var labels = new string[source.Count];

		var i = 0;
		foreach (var item in source.OrderBy(getPosition))
		{
			positions[i] = getPosition(item);
			labels[i] = getLabel(item);
			++i;
		}

		axis.ManualTickPositions(positions, labels);
	}
}