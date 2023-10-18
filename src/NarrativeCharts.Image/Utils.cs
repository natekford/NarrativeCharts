using ImageMagick;

using NarrativeCharts.Models;

namespace NarrativeCharts.Image;

public static class Utils
{
	public static async Task DrawChartAsync(this NarrativeChartGenerator chart, string path)
	{
		var range = ChartRange.GetRange(chart);

		using var image = new MagickImage(new MagickColor("#FFFFFF"), range.RangeX, range.RangeY);

		var duplicateLines = new HashSet<(Point, Point)>();
		foreach (var (character, points) in chart.NarrativePoints.OrderBy(x => x.Value.Count))
		{
			var pointsArr = points.ToArray();
			for (var i = 0; i < pointsArr.Length - 1; ++i)
			{
				var point = pointsArr[i];
				var nextPoint = pointsArr[i + 1];
				var start = range.NormalizePoint(point.Point);
				var end = range.NormalizePoint(nextPoint.Point);

				var shift = 0;
				// try to not have lines overlap
				while (duplicateLines.Contains((start, end)))
				{
					start = start with
					{
						Y = start.Y - point.LineThickness,
					};
					end = end with
					{
						Y = end.Y - point.LineThickness,
					};
					shift += point.LineThickness;
				}
				// account for the shift by shifting the next point up (down when normalized)
				// otherwise the lines dont line up
				if (shift != 0)
				{
					pointsArr[i + 1] = nextPoint with
					{
						Point = nextPoint.Point with
						{
							Y = nextPoint.Point.Y + shift,
						},
					};
				}
				duplicateLines.Add((start, end));

				new Drawables()
					.StrokeColor(point.LineColor.ToMagickColor())
					.StrokeWidth(point.LineThickness)
					.Line(start.X, start.Y, end.X, end.Y)
					.Draw(image);
			}
		}

		await image.WriteAsync(path).ConfigureAwait(false);
	}

	public static Point NormalizePoint(this ChartRange range, Point point)
	{
		var (x, y) = point;
		if (range.MinX < 0)
		{
			x += Math.Abs(range.MinX);
		}
		if (range.MinY < 0)
		{
			y += Math.Abs(range.MinY);
		}
		// 0,0 starts in the top left and not the bottom left
		y = Math.Abs(y - range.RangeY);
		return new(x, y);
	}

	public static MagickColor ToMagickColor(this Color color)
		=> new(color.R, color.G, color.B, color.A);

	public static Color ToNarrativeChartColor(this MagickColor color)
		=> new(color.R, color.G, color.B, color.A);
}