using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

/// <summary>
/// Base class for drawing a <see cref="NarrativeChartData"/>, dealing with creating
/// canvases, drawing the chart, and saving the image.
/// </summary>
/// <typeparam name="TImage"></typeparam>
/// <typeparam name="TColor"></typeparam>
public abstract class ChartDrawer<TImage, TColor> : ChartDrawer
{
	/// <inheritdoc />
	public override async Task SaveChartAsync(NarrativeChartData chart, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		var yMap = GetYMap(chart);
		var image = DrawChart(chart, yMap);
		await SaveImageAsync(image, path).ConfigureAwait(false);
	}

	/// <summary>
	/// Creates a canvas to draw on.
	/// </summary>
	/// <param name="chart"></param>
	/// <param name="yMap"></param>
	/// <returns></returns>
	protected abstract TImage CreateCanvas(NarrativeChartData chart, YMap yMap);

	/// <summary>
	/// Draws <paramref name="chart"/> as an image.
	/// </summary>
	/// <param name="chart"></param>
	/// <param name="yMap"></param>
	/// <returns></returns>
	protected virtual TImage DrawChart(NarrativeChartData chart, YMap yMap)
	{
		var canvas = CreateCanvas(chart, yMap);
		foreach (var (character, temp) in chart.Points.OrderBy(x => x.Key.Value))
		{
			var points = temp.Values;
			if (ShouldIgnore(character, points))
			{
				continue;
			}

			var stationaryStart = points[0].Hour;
			for (var p = 1; p < points.Count; ++p)
			{
				var prev = points[p - 1];
				var curr = points[p];
				var (prevX, prevY) = (prev.Hour, prev.Location);
				var (currX, currY) = (curr.Hour, curr.Location);
				var hasMovement = prev.IsTimeSkip || prevY != currY;
				var isFinal = p == points.Count - 1;

				// Add the previous stationary segment
				if (hasMovement || isFinal)
				{
					DrawSegment(canvas, new(
						Chart: chart,
						Character: character,
						X0: stationaryStart,
						// If we're at the last point don't stop before it
						X1: isFinal && !hasMovement ? currX : prevX,
						Y0: yMap.Characters[(character, prevY)],
						Y1: yMap.Characters[(character, prevY)],
						IsMovement: false,
						IsFinal: isFinal && !hasMovement
					));
				}
				// Add the current movement segment
				if (hasMovement)
				{
					DrawSegment(canvas, new(
						Chart: chart,
						Character: character,
						X0: prevX,
						X1: currX,
						Y0: yMap.Characters[(character, prevY)],
						Y1: yMap.Characters[(character, currY)],
						IsMovement: true,
						IsFinal: isFinal
					));
					stationaryStart = currX;
				}
			}
		}
		FinishImage(canvas);
		return canvas;
	}

	/// <summary>
	/// Draws <paramref name="segment"/> onto <paramref name="image"/>.
	/// </summary>
	/// <param name="image"></param>
	/// <param name="segment"></param>
	protected abstract void DrawSegment(TImage image, Segment segment);

	/// <summary>
	/// Draws any items after all of the segments have been drawn.
	/// </summary>
	/// <param name="image"></param>
	protected virtual void FinishImage(TImage image)
	{
	}

	/// <summary>
	/// Gets a <typeparamref name="TColor"/> from <paramref name="hex"/>.
	/// </summary>
	/// <param name="hex"></param>
	/// <returns></returns>
	protected virtual TColor GetColor(Hex hex)
		=> ColorCache<TColor>.Cache.GetOrAdd(hex, ParseColor);

	/// <summary>
	/// Creates a <typeparamref name="TColor"/> from <paramref name="hex"/>.
	/// </summary>
	/// <param name="hex"></param>
	/// <returns></returns>
	protected abstract TColor ParseColor(Hex hex);

	/// <summary>
	/// Saves <paramref name="image"/> to <paramref name="path"/>.
	/// </summary>
	/// <param name="image"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	protected abstract Task SaveImageAsync(TImage image, string path);

	/// <summary>
	/// Information for drawing a segment.
	/// </summary>
	/// <param name="Chart">The chart that is being drawn.</param>
	/// <param name="Character">The character this segment belongs to.</param>
	/// <param name="X0">The starting X value.</param>
	/// <param name="X1">The ending X value.</param>
	/// <param name="Y0">The starting Y value.</param>
	/// <param name="Y1">The ending Y value.</param>
	/// <param name="IsMovement">Whether or not this segment is considered movement.</param>
	/// <param name="IsFinal">Whether or not this segment is the final point of this character.</param>
	protected readonly record struct Segment(
		NarrativeChartData Chart, Character Character,
		float X0, float X1, float Y0, float Y1,
		bool IsMovement, bool IsFinal
	);
}