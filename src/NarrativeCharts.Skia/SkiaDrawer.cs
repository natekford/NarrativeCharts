using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SkiaDrawer : ChartDrawer<NarrativeChart, SKContext, SKColor>
{
	private static SKPathEffect Movement { get; } = SKPathEffect.CreateDash(new[] { 4f, 6f }, 10f);

	public SkiaDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
		LabelSize = 20;
		LineWidth = 4;
	}

	protected override SKContext CreateCanvas(NarrativeChart chart, YMap yMap)
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
				canvas.DrawText((++e).ToString(), x, -(TickLength + 2), paint);
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

		using (Restrict(context))
		using (var paint = GetPaint(GetColor(Colors[segment.Character])))
		{
			canvas.Translate(context.PaddingEnd, context.PaddingEnd);

			var p0 = new SKPoint(context.X(segment.X0), context.Y(segment.Y0));
			var p1 = new SKPoint(context.X(segment.X1), context.Y(segment.Y1));
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
				paint.PathEffect = Movement;
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

	protected override Task SaveImageAsync(SKContext image, string path)
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

	private static SKAutoCanvasRestore Restrict(
		SKContext context,
		SKClipOperation op = SKClipOperation.Intersect)
	{
		var canvas = context.Surface.Canvas;
		var autoRestore = new SKAutoCanvasRestore(canvas);
		canvas.ClipRect(context.Grid, op);
		return autoRestore;
	}

	private void DrawYAxis(SKContext grid, SKPaint paint, bool isLeftAxis)
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