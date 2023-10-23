using NarrativeCharts.Models;

using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;

using System.Collections.Concurrent;
using System.Drawing;

namespace NarrativeCharts.Plot;

public sealed class PlotDrawer : ChartDrawer<NarrativeChart, ScottPlot.Plot>
{
	private static readonly ConcurrentDictionary<Hex, Color> _Colors = new();

	public float LabelSize { get; set; } = 10;
	public float LineWidth { get; set; } = 2;
	public float MarkerSize { get; set; } = 6;

	protected override ScottPlot.Plot CreateCanvas(NarrativeChart chart, EventRange range)
	{
		var width = range.RangeX * ImageWidthMultiplier / ImageSizeFloor * ImageSizeFloor;
		var height = range.RangeY * ImageHeightMultiplier / ImageSizeFloor * ImageSizeFloor;
		return new ScottPlot.Plot(width, height);
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
		EventRange range,
		ScottPlot.Plot image,
		string path)
	{
		// Used to have the top and right axes show with the correct scale
		{
			var hack = image.AddScatter(
				new double[] { range.MinX, range.MaxX },
				new double[] { range.MinY, range.MaxY },
				color: Color.Transparent
			);
			hack.XAxisIndex = image.TopAxis.AxisIndex;
			hack.YAxisIndex = image.RightAxis.AxisIndex;
			image.TopAxis.Ticks(true);
			image.RightAxis.Ticks(true);
		}

		var titleSize = Math.Max(
			(int)(image.Height * 0.025),
			image.TopAxis.AxisLabel.Font.Size
		);
		var axisLabelSize = Math.Max(
			(int)(image.Height * 0.01),
			image.BottomAxis.AxisTicks.TickLabelFont.Size
		);

		image.TopAxis.Label(chart.Name, size: titleSize);

		// Display event numbers at the top of the grid
		var c = 0;
		SetTicks(image.TopAxis, chart.Events, x => x.Key.Value, _ => (++c).ToString());

		image.BottomAxis.TickLabelStyle(rotation: 90);
		SetTicks(image.BottomAxis, chart.Events, x => x.Key.Value, x => x.Value.Name);

		foreach (var axis in new[] { image.LeftAxis, image.RightAxis })
		{
			axis.TickLabelStyle(fontSize: axisLabelSize);
			SetTicks(axis, chart.Locations, x => x.Value.Value, x => x.Key.Value);
		}

		image.AxisAuto(0.015, 0.025);
		image.SaveFig(path);
		return Task.CompletedTask;
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

	private void SetTicks<TSource>(
		Axis axis,
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

		axis.AxisTicks.MajorGridWidth = LineWidth;
		axis.AxisTicks.MajorLineWidth = LineWidth;
		axis.ManualTickPositions(positions, labels);
	}
}