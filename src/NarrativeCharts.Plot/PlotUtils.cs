using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;

using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace NarrativeCharts.Plot;

public static class PlotUtils
{
	private const int ROUND_TO = 100;
	private static readonly ConcurrentDictionary<string, Color> _Colors = new();

	public static void PlotChart(this NarrativeChart chart, string path)
	{
		var range = chart.GetRange();
		var width = range.RangeX * 4 / ROUND_TO * ROUND_TO;
		var height = range.RangeY * 6 / ROUND_TO * ROUND_TO;
		var plot = new ScottPlot.Plot(width, height);

		var locationOrder = GetLocationOrder(chart);
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key))
		{
			int ShiftY(int y)
			{
				// if there isn't any location order for this character it's
				// fine to treat it as 0 so we dont care if this fails or not
				locationOrder.TryGetValue((y, character), out var shift);
				return y + (shift * 3);
			}

			double x1, x2, y1, y2, xSegmentStart = points.Values[0].Point.X;
			string label1 = character, label2 = string.Empty;
			for (var p = 1; p < points.Count; ++p)
			{
				var (prevX, prevY) = points.Values[p - 1].Point;
				var (currX, currY) = points.Values[p].Point;
				var hasMovement = prevY != currY;
				var isLastSegment = p == points.Count - 1;

				// Add the previous stationary segment
				// only do this if 1 bool is true, otherwise if a movement segment
				// is the last segment 2 lines will be drawn towards the end
				// The start of this segment shows the character's name
				if (hasMovement ^ isLastSegment)
				{
					x1 = xSegmentStart;
					// If we're at the last point don't stop before it
					x2 = isLastSegment ? currX : prevX;
					y1 = y2 = ShiftY(prevY);
					xSegmentStart = currX;

					var scatter = plot
						.AddScatter(new[] { x1, x2 }, new[] { y1, y2 })
						.CustomizeForNarrativeChart(chart, character);

					// Show the character's name at their last point
					if (isLastSegment)
					{
						label2 = character;
					}
					scatter.DataPointLabels = new[] { label1, label2 };
				}
				// Add the current movement segment
				if (hasMovement)
				{
					x1 = prevX;
					x2 = currX;
					y1 = ShiftY(prevY);
					y2 = ShiftY(currY);

					var scatter = plot
						.AddScatter(new[] { x1, x2 }, new[] { y1, y2 })
						.CustomizeForNarrativeChart(chart, character);
					scatter.LineStyle = LineStyle.Dot;

					// Show the character's name at their last point
					if (isLastSegment)
					{
						label2 = character;
						scatter.DataPointLabels = new[] { label1, label2 };
					}
				}
			}
		}

		var titleSize = Math.Max(
			(int)(height * 0.025),
			plot.TopAxis.AxisLabel.Font.Size
		);
		var axisLabelSize = Math.Max(
			(int)(height * 0.01),
			plot.BottomAxis.AxisTicks.TickLabelFont.Size
		);

		plot.TopAxis.Label(chart.Name, size: titleSize);

		plot.BottomAxis.TickLabelStyle(rotation: 90);
		plot.BottomAxis.SetTicks(chart.Events, x => x.Key, x => x.Value.Name);

		plot.LeftAxis.TickLabelStyle(fontSize: axisLabelSize);
		plot.LeftAxis.SetTicks(chart.Locations, x => x.Value, x => x.Key);

		plot.SaveFig(path);
	}

	private static ScatterPlot CustomizeForNarrativeChart(
		this ScatterPlot scatter,
		NarrativeChart chart,
		string character)
	{
		var color = _Colors.GetOrAdd(chart.Colors[character], ColorTranslator.FromHtml);
		scatter.Label = character;
		scatter.Color = color;

		scatter.MarkerSize = 6;
		scatter.LineWidth = 2;

		scatter.DataPointLabelFont.Size = 10;
		scatter.DataPointLabelFont.Color = color;

		return scatter;
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
}