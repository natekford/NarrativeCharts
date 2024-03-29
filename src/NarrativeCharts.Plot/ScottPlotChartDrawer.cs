﻿using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using ScottPlot;

using System.Drawing;

namespace NarrativeCharts.Plot;

/* this class seems to crash when making images with a side larger than 32k pixels
 * this probably is only an issue in ScottPlot 4 since the crash seems to happen
 * in System.Drawing.Common's Bitmap constructor, and ScottPlot 5 uses SkiaSharp,
 * but at this point SKChartDrawer exists so I don't see a reason to move to
 * ScottPlot 5, and will probably remove this class eventually
 */

public sealed class ScottPlotChartDrawer : ChartDrawer<ScottPlotContext, Color>
{
	protected override ScottPlotContext CreateCanvas(NarrativeChartData chart, YMap yMap)
	{
		var dims = GetDimensions(yMap);
		var plot = new ScottPlot.Plot(dims.Width, dims.Height);

		// To have the top and right axes show with the correct scale
		{
			var hack = plot.AddScatter(
				[yMap.XMin, yMap.XMax],
				[yMap.YMin, yMap.YMax],
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
				chart.YIndexes.Where(x => yMap.Locations.ContainsKey(x.Key)),
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
		return new(plot, chart, yMap);
	}

	protected override void DrawSegment(ScottPlotContext image, Character character, LineSegment segment)
	{
		var xs = new double[] { segment.X0, segment.X1 };
		var ys = new double[] { segment.Y0, segment.Y1 };
		var scatter = image.Plot.AddScatter(xs, ys);

		var color = GetColor(image.Chart.Colors[character]);
		scatter.Label = character.Value;
		scatter.Color = color;

		scatter.LineWidth = LineWidth;
		scatter.MarkerSize = LineMarkerDiameter;

		scatter.DataPointLabelFont.Size = AxisLabelSize;
		scatter.DataPointLabelFont.Color = color;

		scatter.LineStyle = segment.IsMovement ? LineStyle.Dot : LineStyle.Solid;
		scatter.DataPointLabelFont.Size = PointLabelSize;
		scatter.DataPointLabels = new[]
		{
			segment.IsMovement ? string.Empty : character.Value,
			// Show the character's name at their last point
			segment.IsFinal ? character.Value : string.Empty,
		};
	}

	protected override Color ParseColor(Hex hex)
		=> ColorTranslator.FromHtml(hex.Value);

	protected override Task SaveImageAsync(ScottPlotContext image, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		image.Plot.SaveFig(path);
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