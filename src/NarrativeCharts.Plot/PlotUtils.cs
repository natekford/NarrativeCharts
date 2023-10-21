using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;

using System.Collections.Concurrent;
using System.Drawing;

namespace NarrativeCharts.Plot;

public static class PlotUtils
{
	private const float LABEL_ROTATION = 45;
	private static readonly ConcurrentDictionary<string, Color> _Colors = new();

	public static void PlotChart(this NarrativeChart chart, string path)
	{
		var range = chart.GetRange();
		var width = range.RangeX * 4;
		var height = range.RangeY * 6;
		var plot = new ScottPlot.Plot(width, height);

		var locationOrder = GetLocationOrder(chart);
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key))
		{
			// ScottPlot does NOT make a copy of passed in arrays
			// so use lists that force us to make the copies ourself
			var xs = new List<double>(2) { 0, 0 };
			var ys = new List<double>(2) { 0, 0 };
			var labels = new List<string>(2) { character, string.Empty };

			var segmentStartX = points.Values[0].Point.X;
			for (var p = 1; p < points.Count; ++p)
			{
				var (prevX, prevY) = points.Values[p - 1].Point;
				var (currX, currY) = points.Values[p].Point;
				var broken = prevY != currY;
				var last = p == points.Count - 1;

				// add the previous stationary segment
				// only do this if 1 bool is true
				// otherwise if a movement segment is the last one
				// 2 lines will be drawn towards the end
				// the start of this segment shows the character's name
				if (broken ^ last)
				{
					xs[0] = segmentStartX;
					// if we're at the last point don't stop before it
					xs[1] = last ? currX : prevX;
					ys[0] = ys[1] = locationOrder.ShiftY(character, prevY);
					segmentStartX = currX;

					var scatter = plot.AddScatter(xs.ToArray(), ys.ToArray());
					scatter.CustomizeScatter(chart, character);

					// show the character's name at their last point
					if (last)
					{
						labels[1] = character;
					}
					scatter.DataPointLabels = labels.ToArray();
				}
				// add the current movement segment
				if (broken)
				{
					xs[0] = prevX;
					xs[1] = currX;
					ys[0] = locationOrder.ShiftY(character, prevY);
					ys[1] = locationOrder.ShiftY(character, currY);

					var scatter = plot.AddScatter(xs.ToArray(), ys.ToArray());
					scatter.CustomizeScatter(chart, character);
					scatter.LineStyle = LineStyle.DashDotDot;

					// show the character's name at their last point
					if (last)
					{
						labels[1] = character;
						scatter.DataPointLabels = labels.ToArray();
					}
				}
			}
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
		var titleSize = Math.Max((int)(height * 0.025), plot.TopAxis.AxisLabel.Font.Size);

		plot.TopAxis.Label(chart.Name, size: titleSize);
		plot.TopAxis.TickLabelStyle(rotation: LABEL_ROTATION);
		plot.TopAxis.SetTicks(evenEvents, x => x.Key, x => x.Value.Name);

		plot.BottomAxis.TickLabelStyle(rotation: LABEL_ROTATION);
		plot.BottomAxis.SetTicks(oddEvents, x => x.Key, x => x.Value.Name);

		plot.LeftAxis.SetTicks(chart.Locations, x => x.Value, x => x.Key);

		plot.SaveFig(path);
	}

	private static void CustomizeScatter(this ScatterPlot scatter, NarrativeChart chart, string character)
	{
		var color = _Colors.GetOrAdd(chart.Colors[character], ColorTranslator.FromHtml);
		scatter.Label = character;
		scatter.Color = color;

		scatter.MarkerSize = 6;
		scatter.LineWidth = 2;

		scatter.DataPointLabelFont.Size = 10;
		scatter.DataPointLabelFont.Color = color;
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
			// more time spent = closer to the bottom
			// any ties? alphabetical order (A = bottom, Z = top)
			var ordered = time
				.OrderByDescending(x => x.Value)
				.ThenBy(x => x.Key);
			var i = 0;
			foreach (var (character, _) in ordered)
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

	private static int ShiftY(this Dictionary<(int, string), int> dict, string character, int y)
	{
		// if there isn't any location order for this character it's
		// fine to treat it as 0 so we dont care if this fails or not
		dict.TryGetValue((y, character), out var shift);
		return y + (shift * 3);
	}
}