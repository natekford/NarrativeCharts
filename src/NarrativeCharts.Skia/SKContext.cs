using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using SkiaSharp;

using System.Collections.Concurrent;

namespace NarrativeCharts.Skia;

public sealed class SKContext : IDisposable
{
	public SKBitmap Bitmap { get; }
	public SKCanvas Canvas { get; }
	public NarrativeChartData Chart { get; }
	public SKRect Grid { get; }
	public float GridHeight => Grid.Height;
	public float GridWidth => Grid.Width;
	public ConcurrentDictionary<Character, List<SKPoint>> Labels { get; } = [];
	public float PaddingEnd { get; }
	public float PaddingStart { get; }
	// Cache Paint/Text here instead of in SKChartDrawer
	// otherwise AccessViolationExceptions occur if Paralle.ForEachAsync is used
	public ConcurrentDictionary<Hex, SKPaint> Paint { get; } = [];
	public ConcurrentDictionary<string, SKTextBlob> Text { get; } = [];
	public float XMult { get; }
	public float XShift { get; }
	public YMap YMap { get; }
	public float YMult { get; }
	public float YShift { get; }

	public SKContext(
		SKBitmap bitmap,
		SKCanvas canvas,
		NarrativeChartData chart,
		YMap yMap,
		float padding,
		float lineWidth,
		float wMult,
		float hMult)
	{
		Bitmap = bitmap;
		Canvas = canvas;
		Chart = chart;
		YMap = yMap;

		PaddingStart = padding;
		PaddingEnd = padding + lineWidth;

		var bounds = canvas.DeviceClipBounds;
		var x = PaddingStart + (lineWidth / 2);
		var y = PaddingStart + (lineWidth / 2);
		var w = bounds.Width - (x * 2);
		var h = bounds.Height - (y * 2);
		Grid = SKRect.Create(x, y, w, h);

		XMult = wMult * w / bounds.Width;
		YMult = hMult * h / bounds.Height;

		XShift = (w - (yMap.XRange * XMult)) / 2;
		YShift = (h - (yMap.YRange * YMult)) / 2;
	}

	public void Dispose()
	{
		Bitmap.Dispose();
		Canvas.Dispose();

		foreach (var (_, paint) in Paint)
		{
			paint.Dispose();
		}
		foreach (var (_, text) in Text)
		{
			text.Dispose();
		}
	}

	public float X(float x)
		=> ((x - YMap.XMin) * XMult) + XShift;

	public float Y(float y)
		=> GridHeight - (y * YMult) - YShift;
}