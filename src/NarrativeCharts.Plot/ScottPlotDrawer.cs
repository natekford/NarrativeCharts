using NarrativeCharts.Models;

using ScottPlot;

using System.Drawing;

namespace NarrativeCharts.Plot;

public sealed class ScottPlotDrawer : ChartDrawer<NarrativeChart, ScottPlot.Plot, Color>
{
	public ScottPlotDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
	}

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
				axis.AxisTicks.MajorTickLength = TickLength;
			}
		}

		// Top/Bottom Axes
		{
			var (positions, nameLabels) = GetTicks(
				chart.Events,
				x => x.Key,
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
				YIndexes.Where(x => yMap.Locations.ContainsKey(x.Key)),
				x => yMap.Locations[x.Key],
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

	protected override void DrawSegment(Segment segment)
	{
		var xs = new double[] { segment.X0, segment.X1 };
		var ys = new double[] { segment.Y0, segment.Y1 };
		var scatter = segment.Canvas.AddScatter(xs, ys);

		var color = GetColor(Colors[segment.Character]);
		scatter.Label = segment.Character.Value;
		scatter.Color = color;

		scatter.LineWidth = LineWidth;
		scatter.MarkerSize = MarkerDiameter;

		scatter.DataPointLabelFont.Size = LabelSize;
		scatter.DataPointLabelFont.Color = color;

		scatter.LineStyle = segment.IsMovement ? LineStyle.Dot : LineStyle.Solid;
		scatter.DataPointLabels = new[]
		{
			segment.IsMovement ? string.Empty : segment.Character.Value,
			// Show the character's name at their last point
			segment.IsFinal ? segment.Character.Value : string.Empty,
		};
	}

	protected override Color ParseColor(Hex hex)
		=> ColorTranslator.FromHtml(hex.Value);

	protected override Task SaveImageAsync(ScottPlot.Plot image, string path)
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
}