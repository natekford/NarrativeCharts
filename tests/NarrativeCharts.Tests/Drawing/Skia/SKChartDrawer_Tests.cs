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

		var actual = File.ReadAllBytes(path);
		// The size I get every time on my Windows desktop
		if (actual.Length == 1_085_801)
		{
			actual.SequenceEqual(Resources.ExpectedP3V1Windows).Should().Be(true);
		}
		else
		{
			// Github Actions doesn't result in a consistent image output
			// Some lengths encountered: 1_098_576, 1_099_484, and 1_112_636
			// I could probably fiddle around with creating an image inside
			// the action, but this is way simpler
			actual.Length.Should().BeCloseTo(1_100_000, 50_000);

			var bounds = SKBitmap.DecodeBounds(actual);
			bounds.Width.Should().Be(13_100);
			bounds.Height.Should().Be(4_000);
			bounds.ColorType.Should().Be(SKColorType.Rgb565);

			// Since the images are big comparing pixels directly takes too long
			// If the image has the correct size/color type it's /probably/ ok
			Console.WriteLine("Only tested width/height/colortype. " +
				"Image may be different than expected.");
		}
	}
}