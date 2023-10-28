using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SkiaDrawer : ChartDrawer<NarrativeChartData, SKContext, SKColor>
{
	private static SKFont Font { get; } = new();
	private static SKPathEffect Movement { get; } = SKPathEffect.CreateDash(new[] { 4f, 6f }, 10f);

	public SkiaDrawer()
	{
		LabelSize = 20;
		LineWidth = 4;
		MarkerDiameter = 8;
	}

	protected override SKContext CreateCanvas(NarrativeChartData chart, YMap yMap)
	{
		var (width, height) = CalculateDimensions(yMap);
		var context = new SKContext(
			surface: SKSurface.Create(new SKImageInfo(width, height)),
			yMap: yMap,
			padding: ImagePadding,
			lineWidth: LineWidth,
			wMult: ImageWidthMultiplier,
			hMult: ImageHeightMultiplier
		);

		var canvas = context.Surface.Canvas;
		canvas.Clear(SKColors.White);

		// Title
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.TextSize = ImagePadding * 0.50f;
			paint.TextAlign = SKTextAlign.Center;
			paint.IsAntialias = true;

			var x = canvas.DeviceClipBounds.Width / 2;
			var y = paint.TextSize;
			canvas.DrawText(chart.Name, x, y, paint);
		}

		// Y-Axes
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.TextSize = LabelSize;

			canvas.Translate(context.PaddingStart - TickLength, context.PaddingEnd);
			DrawYAxis(context, paint, isLeftAxis: true);

			canvas.Translate(context.GridWidth + TickLength + LineWidth, 0);
			DrawYAxis(context, paint, isLeftAxis: false);
		}

		// X-Axes
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.TextSize = LabelSize;
			paint.TextAlign = SKTextAlign.Center;

			canvas.Translate(context.PaddingEnd, context.PaddingStart);
			var e = 0;
			var queue = new Queue<(float, float, string)>(chart.Events.Count);
			foreach (var (xTick, label) in chart.Events)
			{
				var x = context.X(xTick);
				queue.Enqueue((x, paint.MeasureText(label.Name), label.Name));

				canvas.DrawLine(x, 0, x, -TickLength, paint);
				paint.IsAntialias = true;
				canvas.DrawText((++e).ToString(), x, -(TickLength + 2), paint);
				paint.IsAntialias = false;
			}

			canvas.Translate(0, context.GridHeight + LineWidth);
			var i = 0;
			float prevX = float.MinValue, prevLength = float.MinValue;
			while (queue.TryDequeue(out var tuple))
			{
				var (x, length, label) = tuple;
				if (x < prevX)
				{
					++i;
					prevX = prevLength = float.MinValue;
				}

				if (i > 0 && paint.PathEffect is null)
				{
					paint.PathEffect = SKPathEffect.CreateDash(
						new float[] { TickLength, TickLength },
						TickLength * 2
					);
				}

				// checking for any overlap
				if (prevX + (prevLength / 2) + 10 >= x - (length / 2))
				{
					queue.Enqueue(tuple);
					continue;
				}

				var offset = (TickLength + paint.TextSize) * i;
				canvas.DrawLine(x, 0, x, offset + TickLength, paint);
				paint.IsAntialias = true;
				canvas.DrawText(label, x, offset + paint.TextSize + 2, paint);
				paint.IsAntialias = false;
				prevX = x;
				prevLength = length;
			}
		}

		// Grid lines
		using (Restrict(context))
		using (var paint = GetPaint(SKColors.LightGray))
		{
			canvas.Translate(context.PaddingEnd, context.PaddingEnd);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = context.Y(yTick);
				canvas.DrawLine(-LineWidth, y, context.GridWidth + LineWidth, y, paint);
			}
			foreach (var xTick in chart.Events.Keys)
			{
				var x = context.X(xTick);
				canvas.DrawLine(x, -LineWidth, x, context.GridHeight + LineWidth, paint);
			}
		}

		// Grid Border
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.Style = SKPaintStyle.Stroke;

			canvas.DrawRect(context.Grid, paint);
		}

		return context;
	}

	protected override void DrawSegment(Segment segment)
	{
		var context = segment.Canvas;
		var canvas = context.Surface.Canvas;

		if (!context.SegmentCache.TryGetValue(segment.Character, out var items))
		{
			context.SegmentCache[segment.Character] = items = new(
				Paint: GetPaint(GetColor(segment.Chart.Colors[segment.Character])),
				Name: SKTextBlob.Create(segment.Character.Value, Font)
			);
		}
		var (paint, name) = items;

		using (Restrict(context))
		{
			canvas.Translate(context.PaddingEnd, context.PaddingEnd);

			var p0 = new SKPoint(context.X(segment.X0), context.Y(segment.Y0));
			var p1 = new SKPoint(context.X(segment.X1), context.Y(segment.Y1));
			var labelOffset = new SKSize(MarkerDiameter / 4f, paint.TextSize);

			paint.IsAntialias = true;
			paint.PathEffect = null;
			if (!segment.IsMovement)
			{
				var p2 = p0 + labelOffset;
				canvas.DrawText(name, p2.X, p2.Y, paint);
			}
			if (segment.IsFinal)
			{
				var p2 = p1 + labelOffset;
				canvas.DrawText(name, p2.X, p2.Y, paint);
			}

			canvas.DrawCircle(p0, MarkerDiameter / 2f, paint);
			canvas.DrawCircle(p1, MarkerDiameter / 2f, paint);

			paint.PathEffect = segment.IsMovement ? Movement : null;
			paint.IsAntialias = segment.IsMovement;
			canvas.DrawLine(p0, p1, paint);
		}
	}

	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	protected override Task SaveImageAsync(SKContext image, string path)
	{
		var tcs = new TaskCompletionSource();
		_ = Task.Run(() =>
		{
			try
			{
				using (var snapshot = image.Surface.Snapshot())
				using (var data = snapshot.Encode(SKEncodedImageFormat.Png, 100))
				using (var fs = File.OpenWrite(path))
				{
					data.SaveTo(fs);
				}
				tcs.SetResult();
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
			finally
			{
				image.Surface.Dispose();
				foreach (var items in image.SegmentCache.Values)
				{
					items.Paint.Dispose();
					items.Name.Dispose();
				}
			}
		});
		return tcs.Task;
	}

	private static SKAutoCanvasRestore Restrict(
		SKContext context,
		SKClipOperation op = SKClipOperation.Intersect)
	{
		var canvas = context.Surface.Canvas;
		var autoRestore = new SKAutoCanvasRestore(canvas);
		canvas.ClipRect(context.Grid, op);
		return autoRestore;
	}

	private void DrawYAxis(SKContext context, SKPaint paint, bool isLeftAxis)
	{
		var canvas = context.Surface.Canvas;
		foreach (var (label, yTick) in context.YMap.Locations)
		{
			var y = context.Y(yTick);
			canvas.DrawLine(0, y, TickLength, y, paint);

			var x1 = isLeftAxis
				? -(paint.MeasureText(label.Value) + (TickLength / 2f))
				: TickLength;
			// at least with the default font, 3/4 is above the Y level
			// so to balance it out we add 1/4 to the other side
			var y1 = y + (paint.TextSize / 4f);
			paint.IsAntialias = true;
			canvas.DrawText(label.Value, x1, y1, paint);
			paint.IsAntialias = false;
		}
	}

	private SKPaint GetPaint(SKColor color)
	{
		return new(Font)
		{
			Color = color,
			StrokeWidth = LineWidth,
			IsAntialias = false,
		};
	}
}