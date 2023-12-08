using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts.Drawing;

/// <summary>
/// Base class for drawing a <see cref="NarrativeChartData"/>, dealing with image
/// formatting properties and calculating image dimensions.
/// </summary>
public abstract class ChartDrawer
{
	/// <summary>
	/// The font size to use for axes labels.
	/// </summary>
	public virtual int AxisLabelSize { get; set; } = 10;
	/// <summary>
	/// The pixel count to put on each side for axes.
	/// </summary>
	public virtual int AxisPadding { get; set; } = 250;
	/// <summary>
	/// Whether or not to draw characters that never move.
	/// </summary>
	public virtual bool IgnoreNonMovingCharacters { get; set; }
	/// <summary>
	/// The desired aspect ratio to use. If null, no modifications are made to the
	/// calculated size.
	/// </summary>
	public virtual float? ImageAspectRatio { get; set; }
	/// <summary>
	/// The pixel count to round down to the nearest value for each image dimension.
	/// E.G. a value of 100 would round 1234 to 1200.
	/// </summary>
	public virtual int ImageSizeFloor { get; set; } = 100;
	/// <summary>
	/// Every point in the image is multiplied by this value.
	/// </summary>
	public virtual float ImageSizeMult { get; set; } = 6;
	/// <summary>
	/// The pixel count to use for each segment start/end marker.
	/// </summary>
	public virtual int LineMarkerDiameter => LineWidth * 2;
	/// <summary>
	/// The pixel count to use for each segment width.
	/// </summary>
	public virtual int LineWidth { get; set; } = 2;
	/// <summary>
	/// The font size to use for point labels.
	/// </summary>
	public virtual int PointLabelSize { get; set; } = 12;
	/// <summary>
	/// The pixel length of an axis tick.
	/// </summary>
	public virtual int TickLength { get; set; } = 5;
	/// <summary>
	/// The amount of space between a Y-tick and the first point.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public virtual int YOffset { get; set; } = 2;
	/// <summary>
	/// The amount of space between each point on the same Y-tick.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public virtual int YSpacing { get; set; } = 3;
	/// <summary>
	/// The amount of space to put between the highest Y value of a
	/// previous Y-tick and the next Y-tick.
	/// This is NOT an exact amount of pixels, it is dynamically resized.
	/// </summary>
	public virtual int YTickSeperation { get; set; } = 25;

	/// <summary>
	/// Creates an image from <paramref name="chart"/> and saves it to <paramref name="path"/>.
	/// </summary>
	/// <param name="chart"></param>
	/// <param name="path"></param>
	/// <returns></returns>
	public abstract Task SaveChartAsync(NarrativeChartData chart, string path);

	/// <summary>
	/// Gets the width and height to use for the image, along with the values to
	/// multiply each X and Y value by.
	/// </summary>
	/// <param name="yMap"></param>
	/// <returns></returns>
	protected virtual ChartDimensions GetDimensions(YMap yMap)
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

	/// <summary>
	/// Creates a <see cref="YMap"/> from <paramref name="chart"/>.
	/// </summary>
	/// <param name="chart"></param>
	/// <returns></returns>
	protected virtual YMap GetYMap(NarrativeChartData chart)
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

				cDict[(character, location)] = value;
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

	/// <summary>
	/// Whether or not to ignore drawing <paramref name="character"/>.
	/// </summary>
	/// <param name="character"></param>
	/// <param name="points"></param>
	/// <returns></returns>
	protected virtual bool ShouldIgnore(Character character, IList<NarrativePoint> points)
	{
		return IgnoreNonMovingCharacters
			&& points.All(x => x.Location == points[0].Location);
	}
}