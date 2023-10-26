using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SkiaDrawer : ChartDrawer<NarrativeChart, SKSurface, SKColor>
{
	private static SKPathEffect Dash { get; } = SKPathEffect.CreateDash(new[] { 4f, 6f }, 10f);

	private int GridYOffset => Offset - LineWidth;
	private int Offset => ImagePadding + (LineWidth - 1);

	public SkiaDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
		TickLength = 25;
	}

	protected override SKSurface CreateCanvas(NarrativeChart chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var surface = SKSurface.Create(new SKImageInfo(width, height));
		var canvas = surface.Canvas;
		var (grid, wMult, hMult) = GetGridBounds(canvas.DeviceClipBounds);

		canvas.Clear(SKColors.White);

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid, SKClipOperation.Intersect);
			using var paint = GetPaint(SKColors.Red);

			canvas.Translate(Offset + LineWidth, GridYOffset);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Height - (yTick * hMult);
				canvas.DrawLine(-1, y, grid.Width, y, paint);
			}
			foreach (var xTick in chart.Events.Keys)
			{
				var x = xTick * wMult;
				canvas.DrawLine(x, -1, x, grid.Height, paint);
			}
		}

		using (new SKAutoCanvasRestore(canvas))
		{
			canvas.ClipRect(grid, SKClipOperation.Difference);
			using var paint = GetPaint(SKColors.HotPink);

			canvas.Translate(Offset - TickLength - 1, GridYOffset);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Height - (yTick * hMult);
				canvas.DrawLine(0, y, TickLength, y, paint);
			}

			canvas.Translate(grid.Width + TickLength + LineWidth, 0);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = grid.Height - (yTick * hMult);
				canvas.DrawLine(0, y, TickLength, y, paint);
			}
		}

		{
			using var paint = GetPaint(SKColors.Black);
			paint.Style = SKPaintStyle.Stroke;

			canvas.DrawRect(grid, paint);
		}

		return surface;
	}

	protected override void DrawSegment(SegmentInfo info)
	{
		var canvas = info.Canvas.Canvas;
		var (grid, wMult, hMult) = GetGridBounds(canvas.DeviceClipBounds);

		using (new SKAutoCanvasRestore(canvas))
		using (var paint = GetPaint(GetColor(Colors[info.Character])))
		{
			canvas.ClipRect(grid, SKClipOperation.Intersect);
			canvas.Translate(Offset + LineWidth, GridYOffset);

			paint.IsAntialias = true;
			if (info.IsMovement)
			{
				paint.PathEffect = Dash;
			}

			var p0 = new SKPoint(info.X0 * wMult, grid.Height - (info.Y0 * hMult));
			var p1 = new SKPoint(info.X1 * wMult, grid.Height - (info.Y1 * hMult));

			canvas.DrawCircle(p0, MarkerDiameter / 2f, paint);
			canvas.DrawCircle(p1, MarkerDiameter / 2f, paint);
			canvas.DrawLine(p0, p1, paint);
		}
	}

	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	protected override Task SaveImageAsync(
		NarrativeChart chart,
		YMap yMap,
		SKSurface image,
		string path)
	{
		using (var snapshot = image.Snapshot())
		using (var data = snapshot.Encode(SKEncodedImageFormat.Png, 100))
		using (var fs = File.OpenWrite(path))
		{
			data.SaveTo(fs);
		}

		image.Dispose();
		return Task.CompletedTask;
	}

	private (SKRect Bounds, float wMult, float hMult) GetGridBounds(SKRectI bounds)
	{
		var x = Offset;
		var y = Offset;
		var w = bounds.Width - (x * 2);
		var h = bounds.Height - (y * 2);
		var grid = SKRect.Create(x, y, w, h);

		var wMult = (float)ImageWidthMultiplier * w / bounds.Width;
		var hMult = (float)ImageHeightMultiplier * h / bounds.Height;

		return (grid, wMult, hMult);
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