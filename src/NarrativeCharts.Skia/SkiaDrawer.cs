using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SkiaDrawer : ChartDrawer<NarrativeChart, Grid, SKColor>
{
	private static SKPathEffect Dash { get; } = SKPathEffect.CreateDash(new[] { 4f, 6f }, 10f);

	private int RealPadding => ImagePadding + (LineWidth - 1);
	private int XPadding => RealPadding + LineWidth;
	private int YPadding => RealPadding - LineWidth;

	public SkiaDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
		LineWidth = 3;
	}

	protected override Grid CreateCanvas(NarrativeChart chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var surface = SKSurface.Create(new SKImageInfo(width, height));
		var grid = new Grid(surface, yMap, RealPadding, ImageWidthMultiplier, ImageHeightMultiplier);
		var canvas = surface.Canvas;

		canvas.Clear(SKColors.White);

		// Grid lines
		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Intersect);
			using var paint = GetPaint(SKColors.LightGray);

			canvas.Translate(XPadding, YPadding);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Y(yTick);
				canvas.DrawLine(-LineWidth, y, grid.Width + LineWidth, y, paint);
			}
			foreach (var xTick in chart.Events.Keys)
			{
				var x = grid.X(xTick);
				canvas.DrawLine(x, -LineWidth, x, grid.Height + LineWidth, paint);
			}
		}

		// Y-Axes
		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Difference);
			using var paint = GetPaint(SKColors.Black);
			paint.TextSize = 20;

			canvas.Translate(RealPadding - TickLength - 1, YPadding);
			DrawYAxis(grid, paint, isLeftAxis: true);

			canvas.Translate(grid.Width + TickLength + LineWidth, 0);
			DrawYAxis(grid, paint, isLeftAxis: false);
		}

		// X-Axes
		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Difference);
			using var paint = GetPaint(SKColors.Black);
			paint.TextAlign = SKTextAlign.Center;
			paint.TextSize = 20;

			canvas.Translate(XPadding, RealPadding - 1);
			var e = 0;
			var queue = new Queue<(float, float, string)>(chart.Events.Count);
			foreach (var (xTick, label) in chart.Events)
			{
				var x = grid.X(xTick);
				queue.Enqueue((x, paint.MeasureText(label.Name), label.Name));

				canvas.DrawLine(x, 0, x, -TickLength, paint);
				canvas.DrawText((++e).ToString(), x, -(TickLength + 2), paint);
			}

			canvas.Translate(0, grid.Height + LineWidth);
			var prevX = float.MinValue;
			var prevLength = float.MinValue;
			var i = 0;
			while (queue.TryDequeue(out var tuple))
			{
				var (x, length, label) = tuple;
				if (x < prevX)
				{
					paint.PathEffect = Dash;
					++i;
					prevX = float.MinValue;
					prevLength = float.MinValue;
				}

				// checking for any overlap
				if (prevX + (prevLength / 2) >= x - (length / 2))
				{
					queue.Enqueue(tuple);
					continue;
				}

				var offset = (TickLength + paint.TextSize) * i;
				canvas.DrawLine(x, 0, x, offset + TickLength, paint);
				canvas.DrawText(label, x, offset + paint.TextSize + 2, paint);
				prevX = x;
				prevLength = length;
			}
		}

		// Grid Border
		{
			using var paint = GetPaint(SKColors.Black);
			paint.Style = SKPaintStyle.Stroke;

			canvas.DrawRect(grid.Rect, paint);
		}

		return grid;
	}

	protected override void DrawSegment(Segment segment)
	{
		var grid = segment.Canvas;
		var canvas = grid.Surface.Canvas;

		using (new SKAutoCanvasRestore(canvas))
		using (var paint = GetPaint(GetColor(Colors[segment.Character])))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Intersect);
			canvas.Translate(RealPadding + LineWidth, YPadding);

			var p0 = new SKPoint(grid.X(segment.X0), grid.Y(segment.Y0));
			var p1 = new SKPoint(grid.X(segment.X1), grid.Y(segment.Y1));

			var labelOffset = new SKSize(MarkerDiameter / 4f, paint.TextSize);
			if (!segment.IsMovement)
			{
				canvas.DrawText(segment.Character.Value, p0 + labelOffset, paint);
			}
			if (segment.IsFinal)
			{
				canvas.DrawText(segment.Character.Value, p1 + labelOffset, paint);
			}

			paint.IsAntialias = true;
			canvas.DrawCircle(p0, MarkerDiameter / 2f, paint);
			canvas.DrawCircle(p1, MarkerDiameter / 2f, paint);

			if (segment.IsMovement)
			{
				paint.PathEffect = Dash;
			}
			else
			{
				paint.IsAntialias = false;
			}
			canvas.DrawLine(p0, p1, paint);
		}
	}

	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	protected override Task SaveImageAsync(Grid image, string path)
	{
		using (var snapshot = image.Surface.Snapshot())
		using (var data = snapshot.Encode(SKEncodedImageFormat.Png, 100))
		using (var fs = File.OpenWrite(path))
		{
			data.SaveTo(fs);
		}

		image.Surface.Dispose();
		return Task.CompletedTask;
	}

	private void DrawYAxis(Grid grid, SKPaint paint, bool isLeftAxis)
	{
		var canvas = grid.Surface.Canvas;
		foreach (var (label, yTick) in grid.YMap.Locations)
		{
			var y = grid.Y(yTick);
			canvas.DrawLine(0, y, TickLength, y, paint);

			var x1 = isLeftAxis
				? -(paint.MeasureText(label.Value) + (TickLength / 2f))
				: TickLength;
			// at least with the default font, 3/4 is above the Y level
			// so to balance it out we add 1/4 to the other side
			var y1 = y + (paint.TextSize / 4f);
			canvas.DrawText(label.Value, x1, y1, paint);
		}
	}

	private SKPaint GetPaint(SKColor color)
	{
		return new()
		{
			Color = color,
			StrokeWidth = LineWidth,
			IsAntialias = false,
		};
	}
}