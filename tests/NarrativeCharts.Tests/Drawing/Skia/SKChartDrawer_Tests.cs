using NarrativeCharts.Skia;
using NarrativeCharts.Scripting;
using NarrativeCharts.Tests.Properties;
using SkiaSharp;

namespace NarrativeCharts.Tests.Drawing.Skia;

[Trait("Category", "Integration")]
public class SKChartDrawer_Tests
{
	private SKChartDrawer Drawer { get; } = new()
	{
		ImageAspectRatio = null,
		CharacterLabelColorConverter = null,
	};

	[Fact]
	public async Task DrawImage_Valid()
	{
		var defs = TestUtils.Defs;
		var script = defs.LoadScripts().First();

		var path = Path.Combine(defs.ScriptDirectory, nameof(SKChartDrawer_Tests), "ActualP3V1.png");
		await Drawer.SaveChartAsync(script, path).ConfigureAwait(true);

		using var actual = SKBitmap.Decode(path);
		using var expected = SKBitmap.Decode(Resources.ExpectedP3V1);

		actual.Pixels.SequenceEqual(expected.Pixels).Should().Be(true);
	}
}