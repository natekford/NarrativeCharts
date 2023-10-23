using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts;

public abstract class ChartDrawer<TChart, TImage> where TChart : NarrativeChart
{
	public int ImageHeightMultiplier { get; set; } = 6;
	public int ImageSizeFloor { get; set; } = 100;
	public int ImageWidthMultiplier { get; set; } = 6;
	/// <summary>
	/// The amount of pixels between a Y-tick and the first point.
	/// </summary>
	public int YOffset { get; set; } = 2;
	/// <summary>
	/// The amount of pixels between each point on the same Y-tick.
	/// </summary>
	public int YSpacing { get; set; } = 3;
	/// <summary>
	/// The amount of pixels to put between the highest Y value of a
	/// previous Y-tick and the next Y-tick.
	/// </summary>
	public int YTickSeperation { get; set; } = 25;

	public async Task SaveChartAsync(TChart chart, string path)
	{
		var yMap = GetYMap(chart);
		var image = DrawChart(chart, yMap);
		await SaveImageAsync(chart, yMap, image, path).ConfigureAwait(false);
	}

	protected abstract TImage CreateCanvas(TChart chart, YMap yMap);

	protected virtual TImage DrawChart(TChart chart, YMap yMap)
	{
		var canvas = CreateCanvas(chart, yMap);
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key.Value))
		{
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
					DrawStationarySegment(new(
						Chart: chart,
						Canvas: canvas,
						Character: character,
						X1: xSegmentStart,
						// If we're at the last point don't stop before it
						X2: isFinalSegment ? currX : prevX,
						Y1: yMap.Characters[(character, prevY)],
						Y2: yMap.Characters[(character, prevY)],
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
						Y1: yMap.Characters[(character, prevY)],
						Y2: yMap.Characters[(character, currY)],
						IsFinalSegment: isFinalSegment
					));
				}
			}
		}

		return canvas;
	}

	protected abstract void DrawMovementSegment(SegmentInfo info);

	protected abstract void DrawStationarySegment(SegmentInfo info);

	protected virtual YMap GetYMap(TChart chart)
	{
		int xMax = int.MinValue, xMin = int.MaxValue;
		var timeSpent = new ConcurrentDictionary<Y, ConcurrentDictionary<Character, int>>();
		foreach (var (character, points) in chart.Points)
		{
			if (points.Count > 0)
			{
				xMax = Math.Max(xMax, points.Values[^1].Point.X.Value);
				xMin = Math.Min(xMin, points.Values[0].Point.X.Value);
			}

			for (var p = 0; p < points.Count - 1; ++p)
			{
				var curr = points.Values[p].Point;
				var next = points.Values[p + 1].Point;

				var xDiff = next.X.Value - curr.X.Value;
				timeSpent
					.GetOrAdd(curr.Y, _ => new())
					.AddOrUpdate(character, (_, a) => a, (_, a, b) => a + b, xDiff);
			}
		}

		var y = 0;
		int yMax = int.MinValue, yMin = int.MaxValue;
		var cMap = new Dictionary<(Character, Y), Y>();
		var lMap = new Dictionary<Location, Y>();
		foreach (var (location, time) in timeSpent.OrderBy(x => x.Key.Value))
		{
			lMap[chart.Locations.Single(x => x.Value == location).Key] = new(y);

			// more time spent = closer to the bottom
			// any ties? alphabetical order (A = bottom, Z = top)
			var i = 0;
			foreach (var (character, _) in time.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Value))
			{
				var value = y + (i * YSpacing) + YOffset;
				yMax = Math.Max(yMax, value);
				yMin = Math.Min(yMin, value);

				cMap[new(character, location)] = new(value);
				++i;
			}

			y += (i * YSpacing) + YTickSeperation;
		}

		return new(
			Characters: cMap,
			Locations: lMap,
			XMax: xMax,
			XMin: xMin,
			YMax: yMax,
			YMin: yMin
		);
	}

	protected abstract Task SaveImageAsync(TChart chart, YMap yMap, TImage image, string path);

	protected readonly record struct SegmentInfo(
		TChart Chart, TImage Canvas, Character Character,
		X X1, X X2, Y Y1, Y Y2,
		bool IsFinalSegment
	);

	protected record YMap(
		IReadOnlyDictionary<(Character, Y), Y> Characters,
		IReadOnlyDictionary<Location, Y> Locations,
		int XMax,
		int XMin,
		int YMax,
		int YMin
	)
	{
		public int XRange => XMax - XMin;
		public int YRange => YMax - YMin;
	}
}