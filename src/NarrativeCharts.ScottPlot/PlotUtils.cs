using ScottPlot.Renderable;

using System.Collections.Concurrent;
using System.Drawing;

namespace NarrativeCharts.Plot;

public static class PlotUtils
{
	private const float LABEL_ROTATION = 45;

	public static void PlotChart(this NarrativeChart chart, string path)
	{
		var range = chart.GetRange();
		var width = range.RangeX * 4;
		var height = range.RangeY * 4;
		var plot = new ScottPlot.Plot(width, height);

		var locationOrder = GetLocationOrder(chart);
		// process in alphabetical order so the legend is in alphabetical order
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key))
		{
			var xs = new double[points.Count];
			var ys = new double[points.Count];
			for (var p = 0; p < points.Count; ++p)
			{
				var x = points.Values[p].Point.X;
				var y = points.Values[p].Point.Y;
				var shift = locationOrder[(y, character)];
				xs[p] = x;
				ys[p] = y + (shift * 3);
			}

			var color = ColorTranslator.FromHtml(chart.Colors[character]);
			plot.AddScatter(xs, ys, color: color, markerSize: 5, label: character);
		}

		// set up the ability for the top axis to be used for labels
		{
			var hack = plot.AddScatter(
				xs: new double[] { range.MinX, range.MaxX },
				ys: new double[] { range.MinY, range.MaxY },
				color: Color.Transparent
			);
			hack.YAxisIndex = plot.RightAxis.AxisIndex;
			hack.XAxisIndex = plot.TopAxis.AxisIndex;
			plot.TopAxis.Ticks(true);
			plot.TopAxis.Grid(true);
		}

		var evenEvents = chart.Events.Skip(0).Where((_, i) => i % 2 == 0);
		var oddEvents = chart.Events.Skip(1).Where((_, i) => i % 2 == 0);

		plot.TopAxis.Label(chart.Name);
		plot.TopAxis.TickLabelStyle(rotation: LABEL_ROTATION);
		plot.TopAxis.SetTicks(evenEvents, x => x.Key, x => x.Value.Name);

		plot.BottomAxis.TickLabelStyle(rotation: LABEL_ROTATION);
		plot.BottomAxis.SetTicks(oddEvents, x => x.Key, x => x.Value.Name);

		plot.LeftAxis.SetTicks(chart.Locations, x => x.Value, x => x.Key);

		plot.Legend();
		plot.SaveFig(path);
	}

	private static Dictionary<(int, string), int> GetLocationOrder(NarrativeChart chart)
	{
		var timeSpent = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
		foreach (var (character, points) in chart.Points)
		{
			for (var p = 0; p < points.Count - 1; ++p)
			{
				var currPoint = points.Values[p].Point;
				var nextPoint = points.Values[p + 1].Point;

				var timeDiff = nextPoint.X - currPoint.X;
				timeSpent
					.GetOrAdd(currPoint.Y, _ => new())
					.AddOrUpdate(character, (_, a) => a, (_, a, b) => a + b, timeDiff);
			}
		}

		var locationOrder = new Dictionary<(int, string), int>();
		foreach (var (location, time) in timeSpent)
		{
			var i = 0;
			foreach (var (character, _) in time.OrderByDescending(x => x.Value))
			{
				locationOrder[(location, character)] = i++;
			}
		}

		return locationOrder;
	}

	private static void SetTicks<TSource>(
		this Axis axis,
		IEnumerable<TSource> source,
		Func<TSource, double> getPosition,
		Func<TSource, string> getLabel)
	{
		var items = source.ToArray();
		Array.Sort(items, (a, b) => getPosition(a).CompareTo(getPosition(b)));

		var positions = new double[items.Length];
		var labels = new string[items.Length];
		for (var i = 0; i < items.Length; ++i)
		{
			positions[i] = getPosition(items[i]);
			labels[i] = getLabel(items[i]);
		}

		axis.ManualTickPositions(positions, labels);
	}
}