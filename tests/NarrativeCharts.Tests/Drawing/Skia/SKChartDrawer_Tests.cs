using NarrativeCharts.Skia;
using NarrativeCharts.Scripting;
using NarrativeCharts.Tests.Properties;

namespace NarrativeCharts.Tests.Drawing.Skia;

[Trait("Category", "Integration")]
public class SKChartDrawer_Tests
{
	private SKChartDrawer Drawer { get; } = new();

	[Fact]
	public async Task DrawImage_Valid()
	{
		var defs = TestUtils.Defs;
		var script = defs.LoadScripts().First();

		var path = Path.Combine(defs.ScriptDirectory, nameof(SKChartDrawer_Tests), "ActualP3V1.png");
		await Drawer.SaveChartAsync(script, path).ConfigureAwait(false);

		// If the file is marked as a png VS treats it as an image and converts it
		// to a bitmap which we don't want
		File.ReadAllBytes(path).Should().BeEquivalentTo(Resources.ExpectedP3V1,
			x => x.WithStrictOrdering());
	}
}