using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class Grid
{
	public float Height => Rect.Height;
	public SKRect Rect { get; }
	public SKSurface Surface { get; }
	public float Width => Rect.Width;
	public float XMult { get; }
	public float XShift { get; }
	public YMap YMap { get; }
	public float YMult { get; }
	public float YShift { get; }

	public Grid(SKSurface surface, YMap yMap, int padding, int wMult, int hMult)
	{
		Surface = surface;
		YMap = yMap;

		var bounds = surface.Canvas.DeviceClipBounds;
		var x = padding;
		var y = padding;
		var w = bounds.Width - (x * 2);
		var h = bounds.Height - (y * 2);
		Rect = SKRect.Create(x, y, w, h);

		XMult = (float)wMult * w / bounds.Width;
		YMult = (float)hMult * h / bounds.Height;

		XShift = (w - (yMap.XRange * XMult)) / 2;
		YShift = (h - (yMap.YRange * YMult)) / 2;
	}

	public float X(float x)
		=> ((x - YMap.XMin) * XMult) + XShift;

	public float Y(float y)
		=> Height - (y * YMult) - YShift;
}