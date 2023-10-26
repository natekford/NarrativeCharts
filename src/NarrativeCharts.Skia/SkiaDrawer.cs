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
		TickLength = 25;
	}

	protected override Grid CreateCanvas(NarrativeChart chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var surface = SKSurface.Create(new SKImageInfo(width, height));
		var grid = new Grid(surface, yMap, RealPadding, ImageWidthMultiplier, ImageHeightMultiplier);
		var canvas = surface.Canvas;

		canvas.Clear(SKColors.White);

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Intersect);
			using var paint = GetPaint(SKColors.Red);

			canvas.Translate(XPadding, YPadding);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Y(yTick);
				canvas.DrawLine(-1, y, grid.Width, y, paint);
			}
			foreach (var xTick in chart.Events.Keys)
			{
				var x = grid.X(xTick);
				canvas.DrawLine(x, -1, x, grid.Height, paint);
			}
		}

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid.Rect, SKClipOperation.Difference);
			using var paint = GetPaint(SKColors.HotPink);

			canvas.Translate(RealPadding - TickLength - 1, YPadding);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Y(yTick);
				canvas.DrawLine(0, y, TickLength, y, paint);
			}

			canvas.Translate(grid.Width + TickLength + LineWidth, 0);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Y(yTick);
				canvas.DrawLine(0, y, TickLength, y, paint);
			}
		}

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

			paint.IsAntialias = true;
			if (segment.IsMovement)
			{
				paint.PathEffect = Dash;
			}

			canvas.DrawCircle(p0, MarkerDiameter / 2f, paint);
			canvas.DrawCircle(p1, MarkerDiameter / 2f, paint);
			canvas.DrawLine(p0, p1, paint);

			var labelOffset = new SKSize(MarkerDiameter / 4f, paint.TextSize);
			if (!segment.IsMovement)
			{
				canvas.DrawText(segment.Character.Value, p0 + labelOffset, paint);
			}
			if (segment.IsFinal)
			{
				canvas.DrawText(segment.Character.Value, p1 + labelOffset, paint);
			}
		}
	}

	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	protected override Task SaveImageAsync(
		NarrativeChart chart,
		YMap yMap,
		Grid image,
		string path)
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