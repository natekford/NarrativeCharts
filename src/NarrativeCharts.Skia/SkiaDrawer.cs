using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SkiaDrawer : ChartDrawer<NarrativeChart, SKSurface, SKColor>
{
	public SkiaDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
	}

	protected override SKSurface CreateCanvas(NarrativeChart chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var info = new SKImageInfo(width, height);
		var surface = SKSurface.Create(info);
		var canvas = surface.Canvas;

		canvas.Clear(SKColors.White);

		using var paint = new SKPaint
		{
			Color = SKColors.Red,
			StrokeWidth = LineWidth,
			IsAntialias = false,
		};

		foreach (var yTick in yMap.Locations.Values)
		{
			var p1 = Normalize(height, (yMap.XMin, yTick));
			var p2 = Normalize(height, (yMap.XMax, yTick));
			canvas.DrawLine(p1, p2, paint);
		}
		foreach (var xTick in chart.Events.Keys)
		{
			var p1 = Normalize(height, (xTick, yMap.YMin));
			var p2 = Normalize(height, (xTick, yMap.YMax));
			canvas.DrawLine(p1, p2, paint);
		}

		return surface;
	}

	protected override void DrawSegment(SegmentInfo info)
	{
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

	private SKPoint Normalize(int height, (int, int) point)
	{
		var offset = ImageSizeAddition / 2;
		var (x0, y0) = point;
		var x1 = (x0 * ImageWidthMultiplier) + offset;
		// because image coordinates start from the top left
		var y1 = height - ((y0 * ImageHeightMultiplier) + offset);
		return new(x1, y1);
	}
}