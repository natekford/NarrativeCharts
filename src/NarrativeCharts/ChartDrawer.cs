using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts;

public abstract class ChartDrawer<TChart, TImage, TColor> where TChart : NarrativeChart
{
	public IReadOnlyDictionary<Character, Hex> Colors { get; }
	public int ImageHeightMultiplier { get; set; } = 6;
	public int ImagePadding { get; set; } = 250;
	public int ImageSizeFloor { get; set; } = 100;
	public int ImageWidthMultiplier { get; set; } = 6;
	public int LabelSize { get; set; } = 10;
	public int LineWidth { get; set; } = 2;
	public int MarkerDiameter { get; set; } = 6;
	public int TickLength { get; set; } = 5;
	public IReadOnlyDictionary<Location, int> YIndexes { get; }
	/// <summary>
	/// The amount of space between a Y-tick and the first point.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public int YOffset { get; set; } = 2;
	/// <summary>
	/// The amount of space between each point on the same Y-tick.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public int YSpacing { get; set; } = 3;
	/// <summary>
	/// The amount of space to put between the highest Y value of a
	/// previous Y-tick and the next Y-tick.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public int YTickSeperation { get; set; } = 25;

	protected static ConcurrentDictionary<Hex, TColor> ColorCache { get; } = new();

	protected ChartDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
	{
		Colors = colors;
		YIndexes = yIndexes;
	}

	public async Task SaveChartAsync(TChart chart, string path)
	{
		var yMap = GetYMap(chart);
		var image = DrawChart(chart, yMap);
		await SaveImageAsync(image, path).ConfigureAwait(false);
	}

	protected virtual (int, int) CalculateDimensions(YMap yMap)
	{
		// A default ImageSizeAddition is added because the ScottPlot Render method
		// outputs a blank image if the dimensions are too small
		// There's probably a better way to dynamically make sure the dimensions
		// are big enough, but simply adding several hundred pixels is good enough
		var width = ((ImagePadding * 2) + (yMap.XRange * ImageWidthMultiplier)) / ImageSizeFloor * ImageSizeFloor;
		var height = ((ImagePadding * 2) + (yMap.YRange * ImageHeightMultiplier)) / ImageSizeFloor * ImageSizeFloor;
		return (width, height);
	}

	protected abstract TImage CreateCanvas(TChart chart, YMap yMap);

	protected virtual TImage DrawChart(TChart chart, YMap yMap)
	{
		var canvas = CreateCanvas(chart, yMap);
		foreach (var (character, points) in chart.Points.OrderBy(x => x.Key.Value))
		{
			var stationaryStart = points.Values[0].Point.Hour;
			for (var p = 1; p < points.Count; ++p)
			{
				var (prevX, prevY) = points.Values[p - 1].Point;
				var (currX, currY) = points.Values[p].Point;
				var hasMovement = prevY != currY;
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

		return canvas;
	}

	protected abstract void DrawSegment(Segment segment);

	protected virtual TColor GetColor(Hex hex)
		=> ColorCache.GetOrAdd(hex, ParseColor);

	protected virtual YMap GetYMap(TChart chart)
	{
		int xMax = int.MinValue, xMin = int.MaxValue;
		var timeSpent = new ConcurrentDictionary<Location, ConcurrentDictionary<Character, int>>();
		foreach (var (character, points) in chart.Points)
		{
			for (var i = 0; i < points.Count - 1; ++i)
			{
				var curr = points.Values[i].Point;
				var next = points.Values[i + 1].Point;

				var xDiff = next.Hour - curr.Hour;
				timeSpent
					.GetOrAdd(curr.Location, _ => new())
					.AddOrUpdate(character, (_, a) => a, (_, a, b) => a + b, xDiff);
			}

			if (points.Count > 0)
			{
				xMax = Math.Max(xMax, points.Values[^1].Point.Hour);
				xMin = Math.Min(xMin, points.Values[0].Point.Hour);

				// prevent issues with ending on a movement segment
				timeSpent
					.GetOrAdd(points.Values[^1].Point.Location, _ => new())
					.TryAdd(character, 0);
			}
		}

		int y = 0, yMax = int.MinValue, yMin = int.MaxValue;
		var cDict = new Dictionary<(Character, Location), int>();
		var lDict = new Dictionary<Location, int>();
		foreach (var (location, time) in timeSpent.OrderBy(x => YIndexes[x.Key]))
		{
			lDict[location] = y;

			// more time spent = closer to the bottom
			// any ties? alphabetical order (A = bottom, Z = top)
			var i = 0;
			foreach (var (character, _) in time.OrderByDescending(x => x.Value).ThenBy(x => x.Key.Value))
			{
				var value = y + (i * YSpacing) + YOffset;
				yMax = Math.Max(yMax, value);
				yMin = Math.Min(yMin, value);

				cDict[new(character, location)] = value;
				++i;
			}

			y += (i * YSpacing) + YTickSeperation;
		}

		return new(
			Characters: cDict,
			Locations: lDict,
			XMax: xMax,
			XMin: xMin,
			YMax: yMax,
			YMin: yMin
		);
	}

	protected abstract TColor ParseColor(Hex hex);

	protected abstract Task SaveImageAsync(TImage image, string path);

	protected readonly record struct Segment(
		TChart Chart, TImage Canvas, Character Character,
		int X0, int X1, int Y0, int Y1,
		bool IsMovement, bool IsFinal
	);
}

public record YMap(
	Dictionary<(Character, Location), int> Characters,
	Dictionary<Location, int> Locations,
	int XMax,
	int XMin,
	int YMax,
	int YMin
)
{
	public int XRange => XMax - XMin;
	public int YRange => YMax - YMin;
}