using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using SkiaSharp;

using System.Collections.Concurrent;

namespace NarrativeCharts.Skia;

/// <summary>
/// Holds the bitmap being drawn on and associated information for drawing it.
/// </summary>
public sealed class SKContext : IDisposable
{
	/// <summary>
	/// The bitmap being drawn on.
	/// </summary>
	public SKBitmap Bitmap { get; }
	/// <summary>
	/// The canvas used to drawn on <see cref="Bitmap"/>.
	/// </summary>
	public SKCanvas Canvas { get; }
	/// <summary>
	/// The points/events being drawn.
	/// </summary>
	public NarrativeChartData Chart { get; }
	/// <summary>
	/// The rectangle covering the grid without including the border.
	/// </summary>
	public SKRect Grid { get; }
	/// <summary>
	/// Locations to add labels after all of the lines have been drawn.
	/// </summary>
	public ConcurrentDictionary<Character, List<SKPoint>> Labels { get; } = [];
	/// <summary>
	/// Pixel count for the end of padding + grid border.
	/// </summary>
	public float PaddingEnd { get; }
	/// <summary>
	/// Pixel count for start of padding before grid border.
	/// </summary>
	public float PaddingStart { get; }
	/// <summary>
	/// Cached paints for the current image.
	/// </summary>
	public ConcurrentDictionary<Hex, SKPaint> Paint { get; } = [];
	/// <summary>
	/// Cached text for the current image.
	/// </summary>
	public ConcurrentDictionary<string, SKTextBlob> Text { get; } = [];
	/// <summary>
	/// The value to multiply each X value by so it's in the correct spot relative
	/// to any aspect ratio/image size changes.
	/// </summary>
	public float XMult { get; }
	/// <summary>
	/// How many pixels to shift each X value so they're inside the grid.
	/// </summary>
	public float XShift { get; }
	/// <summary>
	/// Map of location and character Y positions.
	/// </summary>
	public YMap YMap { get; }
	/// <summary>
	/// The value to multiply each Y value by so it's in the correct spot relative
	/// to any aspect ratio/image size changes.
	/// </summary>
	public float YMult { get; }
	/// <summary>
	/// How many pixels to shift each Y value so they're inside the grid.
	/// </summary>
	public float YShift { get; }

	/// <summary>
	/// Creats a new <see cref="SKContext"/>.
	/// </summary>
	/// <param name="bitmap"></param>
	/// <param name="canvas"></param>
	/// <param name="chart"></param>
	/// <param name="yMap"></param>
	/// <param name="padding"></param>
	/// <param name="lineWidth"></param>
	/// <param name="wMult"></param>
	/// <param name="hMult"></param>
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

	/// <inheritdoc />
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

	/// <summary>
	/// Converts <paramref name="x"/> to an X value located inside the grid.
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public float X(float x)
		=> ((x - YMap.XMin) * XMult) + XShift;

	/// <summary>
	/// Converts <paramref name="y"/> to a Y value located inside the grid.
	/// This converts from a bottom-left origin to a top-left origin.
	/// </summary>
	/// <param name="y"></param>
	/// <returns></returns>
	public float Y(float y)
		=> Grid.Height - (y * YMult) - YShift;
}