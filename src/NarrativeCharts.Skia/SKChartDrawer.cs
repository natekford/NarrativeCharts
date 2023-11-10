using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public sealed class SKChartDrawer
	: ChartDrawer<NarrativeChartData, SKContext, SKColor>, IDisposable
{
	private readonly Dictionary<Hex, SKPaint> _Paint = [];
	private readonly Dictionary<string, SKTextBlob> _Text = [];
	private bool _Disposed;

	public Func<Hex, Hex>? CharacterLabelColorConverter { get; init; }
	private static SKFont Font { get; } = new();
	private static SKPathEffect Movement { get; } = SKPathEffect.CreateDash(new[] { 4f, 6f }, 10f);

	public SKChartDrawer()
	{
		AxisLabelSize = 20;
		LineWidth = 4;
		LineMarkerDiameter = 8;
	}

	public void Dispose()
	{
		if (_Disposed)
		{
			return;
		}

		foreach (var (_, paint) in _Paint)
		{
			paint.Dispose();
		}
		foreach (var (_, text) in _Text)
		{
			text.Dispose();
		}
		_Disposed = true;
	}

	protected override SKContext CreateCanvas(NarrativeChartData chart, YMap yMap)
	{
		var dims = CalculateDimensions(yMap);
		// Default color type is Rgba8888 which is 32 bits, Rgb565 is 16 bits
		// I don't use transparency in any of the drawing and the colors I use for
		// characters don't include any alpha so there's no harm in ignoring alpha
		// The images this program creates are big, the Bookworm sample project
		// (P3V1, P3V2, part P3V3 x2, combined) use the following memory + time:
		// 1600mb when Rgba8888, 800mb when Rgb565
		// 12.5s when Rgba8888, 9.5s when Rgb565
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
		using (var paint = GetPaint(SKColors.Black))
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
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.TextSize = AxisLabelSize;

			canvas.Translate(context.PaddingStart - TickLength, context.PaddingEnd);
			DrawYAxis(context, paint, isLeftAxis: true);

			canvas.Translate(context.GridWidth + TickLength + LineWidth, 0);
			DrawYAxis(context, paint, isLeftAxis: false);
		}

		// X-Axes
		using (Restrict(context, SKClipOperation.Difference))
		using (var paint = GetPaint(SKColors.Black))
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
						new float[] { TickLength, TickLength },
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
		var canvas = context.Canvas;

		var hex = segment.Chart.Colors[segment.Character];
		if (!_Paint.TryGetValue(hex, out var paint))
		{
			_Paint[hex] = paint = GetPaint(GetColor(hex));
		}
		if (!context.Labels.TryGetValue(segment.Character, out var positions))
		{
			context.Labels[segment.Character] = positions = [];
		}

		using (Restrict(context))
		{
			canvas.Translate(context.PaddingEnd, context.PaddingEnd);

			var p0 = new SKPoint(context.X(segment.X0), context.Y(segment.Y0));
			var p1 = new SKPoint(context.X(segment.X1), context.Y(segment.Y1));
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
			canvas.DrawCircle(p0, LineMarkerDiameter / 2f, paint);
			canvas.DrawCircle(p1, LineMarkerDiameter / 2f, paint);

			paint.PathEffect = segment.IsMovement ? Movement : null;
			paint.IsAntialias = segment.IsMovement;
			canvas.DrawLine(p0, p1, paint);
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
				var text = character.Value;
				if (!_Text.TryGetValue(text, out var name))
				{
					_Text[text] = name = SKTextBlob.Create(text, Font);
				}
				var hex = image.Chart.Colors[character];
				if (CharacterLabelColorConverter is not null)
				{
					hex = CharacterLabelColorConverter.Invoke(hex);
				}
				if (!_Paint.TryGetValue(hex, out var paint))
				{
					_Paint[hex] = paint = GetPaint(GetColor(hex));
				}

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

#if true
				image.Bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
#else
				// with 1, 2, or 3 compression: ~33% faster, file ~66% bigger
				// with 0 it's actually slower probably because either my ram or
				// ssd can't keep up with the 250x size increase
				var options = new SKPngEncoderOptions(
					filterFlags: SKPngEncoderFilterFlags.AllFilters,
					zLibLevel: 3
				);

				using var pixmap = image.Bitmap.PeekPixels();
				using var data = pixmap.Encode(options);
				using var ds = data.AsStream();
				ds.CopyTo(fs);
#endif

				tcs.SetResult();
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
			finally
			{
				image.Bitmap.Dispose();
				image.Canvas.Dispose();
			}
		});
		return tcs.Task;
	}

	private static SKAutoCanvasRestore Restrict(
		SKContext context,
		SKClipOperation op = SKClipOperation.Intersect)
	{
		var canvas = context.Canvas;
		var autoRestore = new SKAutoCanvasRestore(canvas);
		canvas.ClipRect(context.Grid, op);
		return autoRestore;
	}

	private void DrawYAxis(SKContext context, SKPaint paint, bool isLeftAxis)
	{
		var canvas = context.Canvas;
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