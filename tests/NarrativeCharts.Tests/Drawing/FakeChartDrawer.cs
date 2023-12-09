using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

namespace NarrativeCharts.Tests.Drawing;

public class FakeChartDrawer : ChartDrawer<FakeCanvas, Hex>
{
	public List<FakeCanvas> Images { get; } = [];
	public TimeSpan? SaveDelay { get; set; } = null;

	protected override FakeCanvas CreateCanvas(NarrativeChartData chart, YMap yMap)
		=> new([], yMap, GetDimensions(yMap));

	protected override void DrawSegment(FakeCanvas image, LineSegment segment)
		=> image.Segments.Add(segment);

	protected override Hex ParseColor(Hex hex)
		=> hex;

	protected override async Task SaveImageAsync(FakeCanvas image, string path)
	{
		if (SaveDelay is TimeSpan delay)
		{
			await Task.Delay(delay).ConfigureAwait(false);
		}

		Images.Add(image);
	}
}