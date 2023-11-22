using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

public abstract class ChartDrawer<TImage, TColor> : ChartDrawer
{
	public virtual async Task SaveChartAsync(NarrativeChartData chart, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		var yMap = GetYMap(chart);
		var image = DrawChart(chart, yMap);
		await SaveImageAsync(image, path).ConfigureAwait(false);
	}

	protected abstract TImage CreateCanvas(NarrativeChartData chart, YMap yMap);

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
					DrawSegment(new(
						Chart: chart,
						Canvas: canvas,
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
					DrawSegment(new(
						Chart: chart,
						Canvas: canvas,
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

	protected abstract void DrawSegment(Segment segment);

	protected virtual void FinishImage(TImage image)
	{
	}

	protected virtual TColor GetColor(Hex hex)
		=> ColorCache<TColor>.Cache.GetOrAdd(hex, ParseColor);

	protected abstract TColor ParseColor(Hex hex);

	protected abstract Task SaveImageAsync(TImage image, string path);

	protected readonly record struct Segment(
		NarrativeChartData Chart, TImage Canvas, Character Character,
		float X0, float X1, float Y0, float Y1,
		bool IsMovement, bool IsFinal
	);
}