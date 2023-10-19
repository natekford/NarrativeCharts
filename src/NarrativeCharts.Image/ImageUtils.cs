using ImageMagick;

using NarrativeCharts.Models;

namespace NarrativeCharts.Image;

public static class ImageUtils
{
	public static async Task DrawChartAsync(this NarrativeChart chart, string path)
	{
		var range = ChartRange.GetRange(chart);

		using var image = new MagickImage(MagickColors.White, range.RangeX, range.RangeY);

		foreach (var (x, @event) in chart.Events)
		{
			new Drawables()
				.FontPointSize(10)
				.Font("Comcic Sans")
				.StrokeColor(MagickColors.Transparent)
				.FillColor(MagickColors.Gray)
				.StrokeWidth(1)
				.Text(x + 3, 10, @event.Name)
				.StrokeColor(MagickColors.Black)
				.StrokeWidth(3)
				.Line(x, 0, x, image.Height)
				.Draw(image);
		}

		var duplicateLines = new HashSet<(Point, Point)>();
		// draw the characters with the most points last since they're the most prominent
		foreach (var (character, tempPoints) in chart.Points.OrderBy(x => x.Value.Count))
		{
			// make a copy since we modify values in it
			var points = tempPoints.Values.ToArray();
			for (var i = 0; i < points.Length - 1; ++i)
			{
				var currPoint = points[i];
				var nextPoint = points[i + 1];
				var start = NormalizePoint(range, currPoint.Point);
				var end = NormalizePoint(range, nextPoint.Point);

				var shift = 0;
				// try to not have lines overlap
				while (duplicateLines.Contains((start, end)))
				{
					start = start with
					{
						Y = start.Y - currPoint.LineThickness,
					};
					end = end with
					{
						Y = end.Y - currPoint.LineThickness,
					};
					shift += currPoint.LineThickness;
				}
				// account for the shift by shifting the next point up (down when normalized)
				if (shift != 0)
				{
					points[i + 1] = nextPoint with
					{
						Point = nextPoint.Point with
						{
							Y = nextPoint.Point.Y + shift,
						},
					};
				}
				duplicateLines.Add((start, end));

				new Drawables()
					.StrokeColor(currPoint.LineColor.ToMagickColor())
					.StrokeWidth(currPoint.LineThickness)
					.Line(start.X, start.Y, end.X, end.Y)
					.Draw(image);
			}
		}

		await image.WriteAsync(path).ConfigureAwait(false);
	}

	public static Point NormalizePoint(ChartRange range, Point point)
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
		y = range.RangeY - y;
		return new(x, y);
	}

	public static MagickColor ToMagickColor(this Color color)
		=> new(color.R, color.G, color.B, color.A);

	public static Color ToNarrativeChartColor(this MagickColor color)
		=> new(color.R, color.G, color.B, color.A);
}