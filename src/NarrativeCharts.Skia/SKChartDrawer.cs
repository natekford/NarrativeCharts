using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SKChartDrawer : ChartDrawer<SKContext, SKColor>
{
	public SKFont AxisLabelFont { get; set; } = new();
	public Func<Hex, Hex>? CharacterLabelColorConverter { get; set; }
	public SKFont PointLabelFont { get; set; } = new();
	public override int PointLabelSize
	{
		get => (int)PointLabelFont.Size;
		set => PointLabelFont.Size = value;
	}

	private static SKPathEffect Movement { get; } = SKPathEffect.CreateDash([4f, 6f], 10f);

	public SKChartDrawer()
	{
		AxisLabelSize = 20;
		LineWidth = 4;
	}

	protected override SKContext CreateCanvas(NarrativeChartData chart, YMap yMap)
	{
		var dims = GetDimensions(yMap);
		// Default color type is Rgba8888 which is 32 bits, Rgb565 is 16 bits
		// I don't use transparency in any of the drawing and the colors I use for
		// characters don't include any alpha so there's no harm in ignoring alpha
		var info = new SKImageInfo(dims.Width, dims.Height, SKColorType.Rgb565);
		var bitmap = new SKBitmap(info);
		var canvas = new SKCanvas(bitmap);
		var context = new SKContext(
			bitmap: bitmap,
			canvas: canvas,
			chart: chart,
			yMap: yMap,
			padding: AxisPadding,
			lineWidth: LineWidth,
			wMult: dims.WidthMult,
			hMult: dims.HeightMult
		);

		canvas.Clear(SKColors.White);

		// Title
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black, AxisLabelFont))
		{
			paint.TextSize = AxisPadding * 0.50f;
			paint.TextAlign = SKTextAlign.Center;
			paint.IsAntialias = true;

			var x = canvas.DeviceClipBounds.Width / 2;
			var y = paint.TextSize;
			canvas.DrawText(chart.Name, x, y, paint);
		}

		// Y-Axes
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black, AxisLabelFont))
		{
			paint.TextSize = AxisLabelSize;

			canvas.Translate(context.PaddingStart - TickLength, context.PaddingEnd);
			DrawYAxis(context, paint, isLeftAxis: true);

			canvas.Translate(context.GridWidth + TickLength + LineWidth, 0);
			DrawYAxis(context, paint, isLeftAxis: false);
		}

		// X-Axes
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black, AxisLabelFont))
		{
			paint.TextSize = AxisLabelSize;
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
			int iterations = 0, processed = 0, queueCount = queue.Count;
			float prevX = float.MinValue, prevLength = float.MinValue;
			while (queue.TryDequeue(out var tuple))
			{
				var (x, length, label) = tuple;
				if (processed == queueCount)
				{
					++iterations;
					processed = 0;
					// add 1 because if we're in the loop 1 has been taken out
					queueCount = queue.Count + 1;
					prevX = prevLength = float.MinValue;
				}

				if (iterations > 0 && paint.PathEffect is null)
				{
					paint.PathEffect = SKPathEffect.CreateDash(
						[TickLength, TickLength],
						TickLength * 2
					);
				}

				++processed;
				// checking for any overlap
				if (prevX + (prevLength / 2) + 10 >= x - (length / 2))
				{
					queue.Enqueue(tuple);
					continue;
				}

				var offset = (TickLength + paint.TextSize) * iterations;
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
		using (var paint = GetPaint(SKColors.LightGray, AxisLabelFont))
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
		using (var paint = GetPaint(SKColors.Black, AxisLabelFont))
		{
			paint.Style = SKPaintStyle.Stroke;

			canvas.DrawRect(context.Grid, paint);
		}

		return context;
	}

	protected override void DrawSegment(SKContext image, LineSegment segment)
	{
		var hex = segment.Chart.Colors[segment.Character];
		var paint = image.Paint.GetOrAdd(hex, x => GetPaint(GetColor(x), PointLabelFont));
		var positions = image.Labels.GetOrAdd(segment.Character, _ => []);

		using (Restrict(image))
		{
			image.Canvas.Translate(image.PaddingEnd, image.PaddingEnd);

			var p0 = new SKPoint(image.X(segment.X0), image.Y(segment.Y0));
			var p1 = new SKPoint(image.X(segment.X1), image.Y(segment.Y1));
			var labelOffset = new SKSize(LineMarkerDiameter / 4f, paint.TextSize);

			if (!segment.IsMovement)
			{
				positions.Add(p0 + labelOffset);
			}
			if (segment.IsFinal)
			{
				positions.Add(p1 + labelOffset);
			}

			paint.IsAntialias = true;
			paint.PathEffect = null;
			image.Canvas.DrawCircle(p0, LineMarkerDiameter / 2f, paint);
			image.Canvas.DrawCircle(p1, LineMarkerDiameter / 2f, paint);

			paint.PathEffect = segment.IsMovement ? Movement : null;
			paint.IsAntialias = segment.IsMovement;
			image.Canvas.DrawLine(p0, p1, paint);
		}
	}

	protected override void FinishImage(SKContext image)
	{
		// draw labels after all of the lines have been drawn
		using (Restrict(image))
		{
			image.Canvas.Translate(image.PaddingEnd, image.PaddingEnd);

			foreach (var (character, positions) in image.Labels)
			{
				var name = image.Text.GetOrAdd(
					key: character.Value,
					valueFactory: x => SKTextBlob.Create(x, PointLabelFont)
				);
				var hex = CharacterLabelColorConverter is null
					? image.Chart.Colors[character]
					: CharacterLabelColorConverter(image.Chart.Colors[character]);
				var paint = image.Paint.GetOrAdd(
					key: hex,
					valueFactory: x => GetPaint(GetColor(x), PointLabelFont)
				);

				paint.IsAntialias = true;
				paint.PathEffect = null;
				foreach (var position in positions)
				{
					image.Canvas.DrawText(name, position.X, position.Y, paint);
				}
			}
		}
	}

	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	protected override Task SaveImageAsync(SKContext image, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		var tcs = new TaskCompletionSource();
		_ = Task.Run(() =>
		{
			try
			{
				using var fs = File.Create(path);

				/* Image format summaries:
				 * Png, works for everything so far
				 * Webp, ~25% faster than png but max size is 16k x 16k
				 * Jpeg, ~60% faster than png but looks awful
				 *
				 * The rest don't encode a single image successfully
				 *
				 * Conditionally encoding into webp if the size is under 16k x 16k
				 * is maybe worth it, but I think people would prefer to have a
				 * consistent file type
				 */

				image.Bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
				tcs.SetResult();
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
			finally
			{
				image.Dispose();
			}
		});
		return tcs.Task;
	}

	private static SKAutoCanvasRestore Restrict(
		SKContext context,
		SKClipOperation op = SKClipOperation.Intersect)
	{
		var autoRestore = new SKAutoCanvasRestore(context.Canvas);
		context.Canvas.ClipRect(context.Grid, op);
		return autoRestore;
	}

	private void DrawYAxis(SKContext context, SKPaint paint, bool isLeftAxis)
	{
		foreach (var (label, yTick) in context.YMap.Locations)
		{
			var y = context.Y(yTick);
			context.Canvas.DrawLine(0, y, TickLength, y, paint);

			var x1 = isLeftAxis
				? -(paint.MeasureText(label.Value) + (TickLength / 2f))
				: TickLength;
			// at least with the default font, 3/4 is above the Y level
			// so to balance it out we add 1/4 to the other side
			var y1 = y + (paint.TextSize / 4f);
			paint.IsAntialias = true;
			context.Canvas.DrawText(label.Value, x1, y1, paint);
			paint.IsAntialias = false;
		}
	}

	private SKPaint GetPaint(SKColor color, SKFont font)
	{
		return new(font)
		{
			Color = color,
			StrokeWidth = LineWidth,
			IsAntialias = false,
		};
	}
}