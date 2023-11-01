﻿using NarrativeCharts.Drawing;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SKContext
{
	public SKBitmap Bitmap { get; }
	public SKCanvas Canvas { get; }
	public SKRect Grid { get; }
	public float GridHeight => Grid.Height;
	public float GridWidth => Grid.Width;
	public float PaddingEnd { get; }
	public float PaddingStart { get; }
	public float XMult { get; }
	public float XShift { get; }
	public YMap YMap { get; }
	public float YMult { get; }
	public float YShift { get; }

	public SKContext(
		SKBitmap bitmap,
		SKCanvas canvas,
		YMap yMap,
		float padding,
		float lineWidth,
		float wMult,
		float hMult)
	{
		Bitmap = bitmap;
		Canvas = canvas;
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

	public float X(float x)
		=> ((x - YMap.XMin) * XMult) + XShift;

	public float Y(float y)
		=> GridHeight - (y * YMult) - YShift;
}