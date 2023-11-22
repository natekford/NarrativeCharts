using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts.Drawing;

public abstract class ChartDrawer<TChart, TImage, TColor> where TChart : NarrativeChartData
{
	/// <summary>
	/// The text size to use for axes labels.
	/// </summary>
	public int AxisLabelSize { get; set; } = 10;
	/// <summary>
	/// The pixel count to put on each side for axes.
	/// </summary>
	public int AxisPadding { get; set; } = 250;
	/// <summary>
	/// Whether or not to draw characters that never move.
	/// </summary>
	public bool IgnoreNonMovingCharacters { get; set; }
	/// <summary>
	/// The desired aspect ratio to use. If null, no modifications are made to the
	/// calculated size.
	/// </summary>
	public float? ImageAspectRatio { get; set; }
	/// <summary>
	/// The pixel count to round down to the nearest value for each image dimension.
	/// E.G. a value of 100 would round 1234 to 1200.
	/// </summary>
	public int ImageSizeFloor { get; set; } = 100;
	/// <summary>
	/// Every point in the image is multiplied by this value.
	/// </summary>
	public float ImageSizeMult { get; set; } = 6;
	/// <summary>
	/// The pixel count to use for each segment start/end marker.
	/// </summary>
	public int LineMarkerDiameter { get; set; } = 6;
	/// <summary>
	/// The pixel count to use for each segment width.
	/// </summary>
	public int LineWidth { get; set; } = 2;
	/// <summary>
	/// The pixel length of an axis tick.
	/// </summary>
	public int TickLength { get; set; } = 5;
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

	public async Task SaveChartAsync(TChart chart, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		var yMap = GetYMap(chart);
		var image = DrawChart(chart, yMap);
		await SaveImageAsync(image, path).ConfigureAwait(false);
	}

	protected virtual Dimensions CalculateDimensions(YMap yMap)
	{
		// A default ImageSizeAddition is added because the ScottPlot Render method
		// outputs a blank image if the dimensions are too small
		// There's probably a better way to dynamically make sure the dimensions
		// are big enough, but simply adding several hundred pixels is good enough
		var padding = AxisPadding * 2; // on both sides

		var widthMult = ImageSizeMult;
		var heightMult = ImageSizeMult;
		if (ImageAspectRatio is float ar)
		{
			var arDiff = ar / ((float)yMap.XRange / yMap.YRange);
			if (Math.Abs(arDiff - 1) > 0.01)
			{
				if (arDiff > 1)
				{
					widthMult *= arDiff;
				}
				else
				{
					heightMult *= 1f / arDiff;
				}
			}
		}

		var width = padding + (yMap.XRange * widthMult);
		var widthF = (int)width / ImageSizeFloor * ImageSizeFloor;
		var height = padding + (yMap.YRange * heightMult);
		var heightF = (int)height / ImageSizeFloor * ImageSizeFloor;
		return new(widthF, widthMult, heightF, heightMult);
	}

	protected abstract TImage CreateCanvas(TChart chart, YMap yMap);

	protected virtual TImage DrawChart(TChart chart, YMap yMap)
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

	protected virtual YMap GetYMap(TChart chart)
	{
		float xMax = float.MinValue, xMin = float.MaxValue;
		var timeSpent = new ConcurrentDictionary<Location, ConcurrentDictionary<Character, float>>();
		foreach (var (character, temp) in chart.Points)
		{
			var points = temp.Values;
			if (ShouldIgnore(character, points))
			{
				continue;
			}

			for (var i = 0; i < points.Count - 1; ++i)
			{
				var curr = points[i];
				var next = points[i + 1];

				var xDiff = next.Hour - curr.Hour;
				timeSpent
					.GetOrAdd(curr.Location, _ => [])
					.AddOrUpdate(character, (_, a) => a, (_, a, b) => a + b, xDiff);
			}

			if (points.Count > 0)
			{
				xMax = Math.Max(xMax, points[^1].Hour);
				xMin = Math.Min(xMin, points[0].Hour);

				// prevent issues with ending on a movement segment
				timeSpent
					.GetOrAdd(points[^1].Location, _ => [])
					.TryAdd(character, 0);
			}
		}

		float y = 0, yMax = float.MinValue, yMin = float.MaxValue;
		var cDict = new Dictionary<(Character, Location), float>();
		var lDict = new Dictionary<Location, float>();
		foreach (var (location, time) in timeSpent.OrderBy(x => chart.YIndexes[x.Key]))
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

			y = yMax + YTickSeperation;
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

	protected virtual bool ShouldIgnore(Character character, IList<NarrativePoint> points)
	{
		return IgnoreNonMovingCharacters
			&& points.All(x => x.Location == points[0].Location);
	}

	protected readonly record struct Segment(
		TChart Chart, TImage Canvas, Character Character,
		float X0, float X1, float Y0, float Y1,
		bool IsMovement, bool IsFinal
	);

	protected record Dimensions(int Width, float WidthMult, int Height, float HeightMult);
}