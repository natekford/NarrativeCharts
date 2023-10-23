namespace NarrativeCharts;

public abstract class ChartDrawer<TChart, TImage> where TChart : NarrativeChart
{
	public int ImageHeightMultiplier { get; set; } = 6;
	public int ImageSizeFloor { get; set; } = 100;
	public int ImageWidthMultiplier { get; set; } = 6;

	public async Task SaveChartAsync(TChart chart, string path)
	{
		var (image, range) = DrawChart(chart);
		await SaveImageAsync(chart, range, image, path).ConfigureAwait(false);
	}

	protected abstract TImage CreateCanvas(TChart chart, EventRange range);

	protected virtual (TImage, EventRange) DrawChart(TChart chart)
	{
		var range = chart.GetRange();
		var canvas = CreateCanvas(chart, range);

		var locationOrder = chart.GetLocationOrder();
		int minY = int.MaxValue, maxY = int.MinValue;
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key))
		{
			int ShiftY(int y)
			{
				// if there isn't any location order for this character it's
				// fine to treat it as 0 so we dont care if this fails or not
				locationOrder.TryGetValue(new(character, y), out var shift);
				var shifted = y + (shift * 3) + 2; // add additional offset
				minY = Math.Min(minY, shifted);
				maxY = Math.Max(maxY, shifted);
				return shifted;
			}

			var xSegmentStart = points.Values[0].Point.X;
			for (var p = 1; p < points.Count; ++p)
			{
				var (prevX, prevY) = points.Values[p - 1].Point;
				var (currX, currY) = points.Values[p].Point;
				var hasMovement = prevY != currY;
				var isFinalSegment = p == points.Count - 1;

				// Add the previous stationary segment
				// only do this if 1 bool is true, if a movement segment
				// is the last segment 2 lines will be drawn towards the end
				// The start of this segment shows the character's name
				if (hasMovement ^ isFinalSegment)
				{
					var y = ShiftY(prevY);
					DrawStationarySegment(new(
						Chart: chart,
						Canvas: canvas,
						Character: character,
						X1: xSegmentStart,
						// If we're at the last point don't stop before it
						X2: isFinalSegment ? currX : prevX,
						Y1: y,
						Y2: y,
						IsFinalSegment: isFinalSegment
					));

					xSegmentStart = currX;
				}
				// Add the current movement segment
				if (hasMovement)
				{
					DrawMovementSegment(new(
						Chart: chart,
						Canvas: canvas,
						Character: character,
						X1: prevX,
						X2: currX,
						Y1: ShiftY(prevY),
						Y2: ShiftY(currY),
						IsFinalSegment: isFinalSegment
					));
				}
			}
		}

		return (canvas, range with { MinY = minY, MaxY = maxY });
	}

	protected abstract void DrawMovementSegment(SegmentInfo info);

	protected abstract void DrawStationarySegment(SegmentInfo info);

	protected abstract Task SaveImageAsync(TChart chart, EventRange range, TImage image, string path);

	protected readonly record struct SegmentInfo(
		TChart Chart, TImage Canvas, string Character,
		int X1, int X2, int Y1, int Y2,
		bool IsFinalSegment
	);
}