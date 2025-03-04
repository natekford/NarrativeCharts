﻿using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

using SkiaSharp;

using System.Diagnostics;

using static System.Net.Mime.MediaTypeNames;

namespace NarrativeCharts.Skia;

/// <summary>
/// Draws a chart using Skia.
/// </summary>
public sealed class SKChartDrawer : ChartDrawer<SKContext, SKColor>
{
	private static readonly SKPathEffect _MovementEffect
		= SKPathEffect.CreateDash([4f, 6f], 10f);

	/// <summary>
	/// If null, the color used for a character's label is the character's color.
	/// If not null, modifies the passed in hex value and returns a new one.
	/// Default value converts all colors to black since that's the easiest
	/// color to see on a white background.
	/// </summary>
	public Func<Hex, Hex>? CharacterLabelColorConverter { get; set; }
		= SKColorConverters.Color(SKColors.Black);
	/// <summary>
	/// The font to use for axis labels.
	/// </summary>
	public Func<SKFont> CreateAxisLabelFont { get; set; }
	/// <summary>
	/// The font to use for character names in the grid.
	/// </summary>
	public Func<SKFont> CreatePointLabelFont { get; set; }

	/// <summary>
	/// Creates a new instance of <see cref="SKChartDrawer"/>.
	/// </summary>
	public SKChartDrawer(SKTypeface? defaultTypeface = null)
	{
		CreateAxisLabelFont = () => new(defaultTypeface, size: AxisLabelSize);
		CreatePointLabelFont = () => new(defaultTypeface, size: PointLabelSize);

		AxisLabelSize = 20;
		LineWidth = 4;
	}

	/// <inheritdoc />
	protected override SKContext CreateCanvas(NarrativeChartData chart, YMap yMap)
	{
		var dims = GetDimensions(yMap);
		// Default color type is Rgba8888 which is 32 bits, Rgb565 is 16 bits
		// I don't use transparency in any of the drawing and the colors I use for
		// characters don't include any alpha so there's no harm in ignoring alpha
		var info = new SKImageInfo(dims.Width, dims.Height, SKColorType.Rgb565);
		var bitmap = new SKBitmap(info);
		var canvas = new SKCanvas(bitmap);
		var image = new SKContext(
			bitmap: bitmap,
			canvas: canvas,
			pointLabelFont: CreatePointLabelFont(),
			chart: chart,
			yMap: yMap,
			padding: AxisPadding,
			lineWidth: LineWidth,
			wMult: dims.WidthMult,
			hMult: dims.HeightMult
		);

		canvas.Clear(SKColors.White);

		// Title
		using (image.ClipGrid(SKClipOperation.Difference))
		using (var font = CreateAxisLabelFont())
		using (var paint = GetPaint(SKColors.Black))
		{
			font.Size = AxisPadding * 0.50f;
			paint.IsAntialias = true;

			var x = canvas.DeviceClipBounds.Width / 2;
			var y = font.Size;
			canvas.DrawText(chart.Name, x, y, SKTextAlign.Center, font, paint);
		}

		// Y-Axes
		using (image.ClipGrid(SKClipOperation.Difference))
		using (var font = CreateAxisLabelFont())
		using (var tickPaint = GetPaint(SKColors.Black))
		using (var labelPaint = GetPaint(SKColors.DarkGray))
		{
			labelPaint.IsAntialias = true;

			void DrawYAxis(bool isLeftAxis)
			{
				foreach (var (label, yTick) in image.YMap.Locations)
				{
					var y = image.Y(yTick);
					image.Canvas.DrawLine(0, y, TickLength, y, tickPaint);

					var x1 = isLeftAxis
						? -(font.MeasureText(label.Value) + (TickLength / 2f))
						: TickLength;
					// at least with the default font, 3/4 is above the Y level
					// so to balance it out we add 1/4 to the other side
					var y1 = y + (font.Size / 4f);
					image.Canvas.DrawText(label.Value, x1, y1, SKTextAlign.Left, font, labelPaint);
				}
			}

			canvas.Translate(image.PaddingStart - TickLength, image.PaddingEnd);
			DrawYAxis(isLeftAxis: true);

			canvas.Translate(image.Grid.Width + TickLength + LineWidth, 0);
			DrawYAxis(isLeftAxis: false);
		}

		// X-Axes
		using (image.ClipGrid(SKClipOperation.Difference))
		using (var font = CreateAxisLabelFont())
		using (var tickPaint = GetPaint(SKColors.Black))
		using (var labelPaint = GetPaint(SKColors.DarkGray))
		{
			labelPaint.IsAntialias = true;

			canvas.Translate(image.PaddingEnd, image.PaddingStart);
			var e = 0;
			var events = new Queue<(float X, float Length, string Label)>(chart.Events.Count);
			foreach (var (xTick, label) in chart.Events)
			{
				var x = image.X(xTick);
				events.Enqueue((x, font.MeasureText(label.Name), label.Name));

				canvas.DrawLine(x, 0, x, -TickLength, tickPaint);
				canvas.DrawText((++e).ToString(), x, -(TickLength + 2), SKTextAlign.Center, font, labelPaint);
			}

			canvas.Translate(0, image.Grid.Height + LineWidth);
			int iterations = 0, processed = 0, queueCount = events.Count;
			float prevX = float.MinValue, prevLength = float.MinValue;
			var labels = new List<(float X, float Offset, string Label)>(events.Count);
			// figure out where to draw labels
			// x values remain constant
			// ticks will always start at x,0 and go to x,labelY
			// labels are centered at x, and their y value is determined by making sure
			// there is no horizontal overlap between labels
			// e.g.
			//     |                |
			//  1. Short Title      |
			//       2. Longer Title That Would Overlap
			while (events.TryDequeue(out var tuple))
			{
				var (x, length, label) = tuple;
				if (processed == queueCount)
				{
					++iterations;
					processed = 0;
					// add 1 because if we're in the loop 1 has been taken out
					queueCount = events.Count + 1;
					prevX = prevLength = float.MinValue;
				}

				if (iterations > 0 && tickPaint.PathEffect is null)
				{
					tickPaint.PathEffect = SKPathEffect.CreateDash(
						[TickLength, TickLength],
						TickLength * 2
					);
				}

				++processed;
				// checking for any overlap
				if (prevX + (prevLength / 2) + 10 >= x - (length / 2))
				{
					events.Enqueue(tuple);
					continue;
				}

				// draw tick lines, but don't draw labels
				// instead, draw labels after so tick lines will never be on top
				var offset = (TickLength + font.Size) * iterations;
				canvas.DrawLine(x, 0, x, offset + TickLength, tickPaint);
				labels.Add((x, offset, label));
				prevX = x;
				prevLength = length;
			}

			// draw labels now that tick lines can't be drawn on top of them
			foreach (var (x, offset, label) in labels)
			{
				canvas.DrawText(label, x, offset + font.Size + 2, SKTextAlign.Center, font, labelPaint);
			}
		}

		// Grid lines
		using (image.ClipGrid(SKClipOperation.Intersect))
		using (var paint = GetPaint(SKColors.LightGray))
		{
			canvas.Translate(image.PaddingEnd, image.PaddingEnd);
			foreach (var yTick in yMap.Locations.Values)
			{
				var y = image.Y(yTick);
				canvas.DrawLine(-LineWidth, y, image.Grid.Width + LineWidth, y, paint);
			}
			foreach (var xTick in chart.Events.Keys)
			{
				var x = image.X(xTick);
				canvas.DrawLine(x, -LineWidth, x, image.Grid.Height + LineWidth, paint);
			}
		}

		// Grid Border
		using (var paint = GetPaint(SKColors.Black))
		{
			paint.Style = SKPaintStyle.Stroke;

			canvas.DrawRect(image.Grid, paint);
		}

		return image;
	}

	/// <inheritdoc />
	protected override void DrawSegment(SKContext image, Character character, LineSegment segment)
	{
		var hex = image.Chart.Colors[character];
		var paint = image.Paint.GetOrAdd(hex, GetPaint);
		var positions = image.Labels.GetOrAdd(character, _ => []);

		using (image.ClipGrid(SKClipOperation.Intersect))
		{
			image.Canvas.Translate(image.PaddingEnd, image.PaddingEnd);

			var p0 = new SKPoint(image.X(segment.X0), image.Y(segment.Y0));
			var p1 = new SKPoint(image.X(segment.X1), image.Y(segment.Y1));
			var labelOffset = new SKSize(LineMarkerDiameter / 4f, image.PointLabelFont.Size);

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

			paint.PathEffect = segment.IsMovement ? _MovementEffect : null;
			paint.IsAntialias = segment.IsMovement;
			image.Canvas.DrawLine(p0, p1, paint);
		}
	}

	/// <inheritdoc />
	protected override void FinishImage(SKContext image)
	{
		// draw labels after all of the lines have been drawn
		using (image.ClipGrid(SKClipOperation.Intersect))
		{
			image.Canvas.Translate(image.PaddingEnd, image.PaddingEnd);

			foreach (var (character, positions) in image.Labels)
			{
				var name = image.Text.GetOrAdd(
					key: character.Value,
					valueFactory: (text, font) => SKTextBlob.Create(text, font)!,
					factoryArgument: image.PointLabelFont
				);
				var hex = CharacterLabelColorConverter is null
					? image.Chart.Colors[character]
					: CharacterLabelColorConverter(image.Chart.Colors[character]);
				var paint = image.Paint.GetOrAdd(hex, GetPaint);

				paint.IsAntialias = true;
				paint.PathEffect = null;
				foreach (var position in positions)
				{
					image.Canvas.DrawText(name, position.X, position.Y, paint);
				}
			}
		}
	}

	/// <inheritdoc />
	protected override SKColor ParseColor(Hex hex)
		=> SKColor.Parse(hex.Value);

	/// <inheritdoc />
	protected override Task SaveImageAsync(SKContext image, string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		var tcs = new TaskCompletionSource();
		_ = Task.Run(async () =>
		{
			try
			{
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
				await using (var fs = File.Create(path))
				{
					image.Bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
				}

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

	private SKPaint GetPaint(Hex hex)
		=> GetPaint(GetColor(hex));

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