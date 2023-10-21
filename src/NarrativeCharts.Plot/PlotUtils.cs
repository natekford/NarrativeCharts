using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;

using System.Collections.Concurrent;
using System.Drawing;

namespace NarrativeCharts.Plot;

public static class PlotUtils
{
	private const int IMG_FLOOR = 100;
	private const int IMG_HEIGHT_MULT = 6;
	private const int IMG_WIDTH_MULT = 6;
	private const float LABEL_SIZE = LINE_WIDTH * 5;
	private const float LINE_WIDTH = 2;
	private const float MARKER_SIZE = LINE_WIDTH * 3;

	private static readonly ConcurrentDictionary<string, Color> _Colors = new();

	public static void PlotChart(this NarrativeChart chart, string path)
	{
		var (range, plot) = chart.CreatePlot();
		plot.ConfigureAndSavePlot(chart, range, path);
	}

	private static void ConfigureAndSavePlot(
		this ScottPlot.Plot plot,
		NarrativeChart chart,
		EventRange range,
		string path)
	{
		// Used to have the top and right axes show with the correct scale
		{
			var hack = plot.AddScatter(
				new double[] { range.MinX, range.MaxX },
				new double[] { range.MinY, range.MaxY },
				color: Color.Transparent
			);
			hack.XAxisIndex = plot.TopAxis.AxisIndex;
			hack.YAxisIndex = plot.RightAxis.AxisIndex;
			plot.TopAxis.Ticks(true);
			plot.RightAxis.Ticks(true);
		}

		var titleSize = Math.Max(
			(int)(plot.Height * 0.025),
			plot.TopAxis.AxisLabel.Font.Size
		);
		var axisLabelSize = Math.Max(
			(int)(plot.Height * 0.01),
			plot.BottomAxis.AxisTicks.TickLabelFont.Size
		);

		plot.TopAxis.Label(chart.Name, size: titleSize);

		// Display event numbers at the top of the grid
		var c = 0;
		plot.TopAxis.SetTicks(chart.Events, x => x.Key, _ => (++c).ToString());

		plot.BottomAxis.TickLabelStyle(rotation: 90);
		plot.BottomAxis.SetTicks(chart.Events, x => x.Key, x => x.Value.Name);

		foreach (var axis in new[] { plot.LeftAxis, plot.RightAxis })
		{
			axis.TickLabelStyle(fontSize: axisLabelSize);
			axis.SetTicks(chart.Locations, x => x.Value, x => x.Key);
		}

		plot.AxisAuto(0.015, 0.025);
		plot.SaveFig(path);
	}

	private static (EventRange, ScottPlot.Plot) CreatePlot(this NarrativeChart chart)
	{
		var range = chart.GetRange();
		var width = range.RangeX * IMG_WIDTH_MULT / IMG_FLOOR * IMG_FLOOR;
		var height = range.RangeY * IMG_HEIGHT_MULT / IMG_FLOOR * IMG_FLOOR;
		var plot = new ScottPlot.Plot(width, height);

		var locationOrder = GetLocationOrder(chart);
		int minY = int.MaxValue, maxY = int.MinValue;
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key))
		{
			int ShiftY(int y)
			{
				// if there isn't any location order for this character it's
				// fine to treat it as 0 so we dont care if this fails or not
				locationOrder.TryGetValue((y, character), out var shift);
				var shifted = y + (shift * 3) + 2; // add additional offset
				minY = Math.Min(minY, shifted);
				maxY = Math.Max(maxY, shifted);
				return shifted;
			}

			double x1, x2, y1, y2, xSegmentStart = points.Values[0].Point.X;
			string label1 = character, label2 = string.Empty;
			for (var p = 1; p < points.Count; ++p)
			{
				var (prevX, prevY) = points.Values[p - 1].Point;
				var (currX, currY) = points.Values[p].Point;
				var hasMovement = prevY != currY;
				var isEnd = p == points.Count - 1;

				// Add the previous stationary segment
				// only do this if 1 bool is true, if a movement segment
				// is the last segment 2 lines will be drawn towards the end
				// The start of this segment shows the character's name
				if (hasMovement ^ isEnd)
				{
					x1 = xSegmentStart;
					// If we're at the last point don't stop before it
					x2 = isEnd ? currX : prevX;
					y1 = y2 = ShiftY(prevY);
					xSegmentStart = currX;

					var scatter = plot.AddScatter(new[] { x1, x2 }, new[] { y1, y2 });
					scatter.CustomizeForNarrativeChart(chart, character);

					// Show the character's name at their last point
					if (isEnd)
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

					var scatter = plot.AddScatter(new[] { x1, x2 }, new[] { y1, y2 });
					scatter.CustomizeForNarrativeChart(chart, character);
					scatter.LineStyle = LineStyle.Dot;

					// Show the character's name at their last point
					if (isEnd)
					{
						label2 = character;
						scatter.DataPointLabels = new[] { label1, label2 };
					}
				}
			}
		}

		return (range with { MinY = minY, MaxY = maxY }, plot);
	}

	private static void CustomizeForNarrativeChart(
		this ScatterPlot scatter,
		NarrativeChart chart,
		string character)
	{
		var color = _Colors.GetOrAdd(chart.Colors[character], ColorTranslator.FromHtml);
		scatter.Label = character;
		scatter.Color = color;

		scatter.LineWidth = LINE_WIDTH;
		scatter.MarkerSize = MARKER_SIZE;

		scatter.DataPointLabelFont.Size = LABEL_SIZE;
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

		axis.AxisTicks.MajorGridWidth = LINE_WIDTH;
		axis.AxisTicks.MajorLineWidth = LINE_WIDTH;
		axis.ManualTickPositions(positions, labels);
	}
}