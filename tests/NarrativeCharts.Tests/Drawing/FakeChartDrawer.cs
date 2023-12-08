using NarrativeCharts.Drawing;
using NarrativeCharts.Models;

namespace NarrativeCharts.Tests.Drawing;

public class FakeChartDrawer : ChartDrawer<FakeCanvas, Hex>
{
	public List<FakeCanvas> Images { get; } = [];

	protected override FakeCanvas CreateCanvas(NarrativeChartData chart, YMap yMap)
		=> new([], yMap, GetDimensions(yMap));

	protected override void DrawSegment(FakeCanvas image, LineSegment segment)
		=> image.Segments.Add(segment);

	protected override Hex ParseColor(Hex hex)
		=> hex;

	protected override Task SaveImageAsync(FakeCanvas image, string path)
	{
		Images.Add(image);
		return Task.CompletedTask;
	}
}