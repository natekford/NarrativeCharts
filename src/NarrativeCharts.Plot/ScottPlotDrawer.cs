using NarrativeCharts.Models;

using ScottPlot;
using ScottPlot.Plottable;

using System.Collections.Concurrent;
using System.Drawing;

namespace NarrativeCharts.Plot;

public sealed class ScottPlotDrawer : ChartDrawer<NarrativeChart, ScottPlot.Plot>
{
	private static readonly ConcurrentDictionary<Hex, Color> _Colors = new();

	public float LabelSize { get; set; } = 10;
	public float LineWidth { get; set; } = 2;
	public float MarkerSize { get; set; } = 6;

	protected override ScottPlot.Plot CreateCanvas(NarrativeChart chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var plot = new ScottPlot.Plot(width, height);

		// To have the top and right axes show with the correct scale
		{
			var hack = plot.AddScatter(
				new double[] { yMap.XMin, yMap.XMax },
				new double[] { yMap.YMin, yMap.YMax },
				color: Color.Transparent
			);
			hack.XAxisIndex = plot.TopAxis.AxisIndex;
			hack.YAxisIndex = plot.RightAxis.AxisIndex;
			plot.TopAxis.Ticks(true);
			plot.RightAxis.Ticks(true);
		}

		// Title
		{
			var titleSize = Math.Max(
				(int)(plot.Height * 0.025),
				plot.TopAxis.AxisLabel.Font.Size
			);
			plot.TopAxis.Label(chart.Name, size: titleSize);
		}

		// All axes
		{
			foreach (var axis in plot.GetSettings().Axes)
			{
				axis.AxisTicks.MajorGridWidth = LineWidth;
				axis.AxisTicks.MajorLineWidth = LineWidth;
			}
		}

		// Top/Bottom Axes
		{
			var (positions, nameLabels) = GetTicks(
				chart.Events,
				x => x.Key.Value,
				x => x.Value.Name
			);
			var numberLabels = Enumerable.Range(1, nameLabels.Length)
				.Select(x => x.ToString())
				.ToArray();

			// Display event numbers at the top of the grid
			plot.TopAxis.ManualTickPositions(positions, numberLabels);

			plot.BottomAxis.TickLabelStyle(rotation: 90);
			plot.BottomAxis.ManualTickPositions(positions, nameLabels);
		}

		// Side axes
		{
			var axisLabelSize = Math.Max(
				(int)(plot.Height * 0.01),
				plot.BottomAxis.AxisTicks.TickLabelFont.Size
			);
			var (positions, labels) = GetTicks(
				chart.Locations.Where(x => yMap.Locations.ContainsKey(x.Key)),
				x => yMap.Locations[x.Key].Value,
				x => x.Key.Value
			);

			foreach (var axis in new[] { plot.LeftAxis, plot.RightAxis })
			{
				axis.TickLabelStyle(fontSize: axisLabelSize);
				axis.ManualTickPositions(positions, labels);
			}
		}

		plot.AxisAuto(0.015, 0.025);
		return plot;
	}

	protected override void DrawMovementSegment(SegmentInfo info)
	{
		var scatter = AddScatter(info);
		scatter.LineStyle = LineStyle.Dot;

		if (info.IsFinalSegment)
		{
			scatter.DataPointLabels = new[]
			{
				info.Character.Value,
				info.Character.Value,
			};
		}
	}

	protected override void DrawStationarySegment(SegmentInfo info)
	{
		var scatter = AddScatter(info);

		scatter.DataPointLabels = new[]
		{
			info.Character.Value,
			// Show the character's name at their last point
			info.IsFinalSegment ? info.Character.Value : string.Empty,
		};
	}

	protected override Task SaveImageAsync(
		NarrativeChart chart,
		YMap yMap,
		ScottPlot.Plot image,
		string path)
	{
		image.SaveFig(path);
		return Task.CompletedTask;
	}

	private static (double[], string[]) GetTicks<TSource>(
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

		return (positions, labels);
	}

	private ScatterPlot AddScatter(SegmentInfo info)
	{
		var xs = new double[] { info.X1.Value, info.X2.Value };
		var ys = new double[] { info.Y1.Value, info.Y2.Value };
		var scatter = info.Canvas.AddScatter(xs, ys);

		var color = _Colors.GetOrAdd(info.Chart.Colors[info.Character], x => ColorTranslator.FromHtml(x.Value));
		scatter.Label = info.Character.Value;
		scatter.Color = color;

		scatter.LineWidth = LineWidth;
		scatter.MarkerSize = MarkerSize;

		scatter.DataPointLabelFont.Size = LabelSize;
		scatter.DataPointLabelFont.Color = color;

		return scatter;
	}
}