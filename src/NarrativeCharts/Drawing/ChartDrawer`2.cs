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
		foreach (var (character, temp) in chart.Points)
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
					// If we're in a movement segment this isn't actually
					// the last segment that gets drawn
					var isActuallyFinal = isFinal && !hasMovement;

					var x0 = stationaryStart;
					// If we're at the last point don't stop before it
					var x1 = isActuallyFinal ? currX : prevX;
					// Only bother attempting to draw if any time has passed
					if (x0 != x1)
					{
						DrawSegment(canvas, character, new(
							X0: x0,
							X1: x1,
							Y0: yMap.Characters[(character, prevY)],
							Y1: yMap.Characters[(character, prevY)],
							IsMovement: false,
							IsFinal: isActuallyFinal
						));
					}
				}
				// Add the current movement segment
				if (hasMovement)
				{
					DrawSegment(canvas, character, new(
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
	/// <param name="character"></param>
	/// <param name="segment"></param>
	protected abstract void DrawSegment(TImage image, Character character, LineSegment segment);

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
}