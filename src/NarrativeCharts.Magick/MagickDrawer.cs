using ImageMagick;

using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts.Magick;

public sealed class MagickDrawer : ChartDrawer<NarrativeChart, MagickImage>
{
	private static readonly ConcurrentDictionary<Hex, MagickColor> _Colors = new();

	public MagickDrawer(
		IReadOnlyDictionary<Character, Hex> colors,
		IReadOnlyDictionary<Location, int> yIndexes)
		: base(colors, yIndexes)
	{
	}

	protected override MagickImage CreateCanvas(NarrativeChart chart, YMap yMap) => throw new NotImplementedException();

	protected override void DrawSegment(SegmentInfo info)
	{
	}

	protected override async Task SaveImageAsync(
		NarrativeChart chart,
		YMap yMap,
		MagickImage image,
		string path)
	{
		await image.WriteAsync(path).ConfigureAwait(false);
		image.Dispose();
	}
}