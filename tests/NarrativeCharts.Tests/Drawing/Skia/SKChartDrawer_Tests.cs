using NarrativeCharts.Skia;
using NarrativeCharts.Scripting;
using NarrativeCharts.Tests.Properties;
using SkiaSharp;
using System;

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
		Console.WriteLine(actual.Length);
		actual.Length.Should().NotBe(0);

		// ExpectedP3V1 output is received when this test is run on Linux
		// So we can reencode the bytes on Linux and it should then become the same
		var expected = new byte[actual.Length];
		using (var bm = SKBitmap.Decode(Resources.ExpectedP3V1))
		await using (var ms = new MemoryStream(expected))
		{
			bm.Encode(ms, SKEncodedImageFormat.Png, 100);
		}

		actual.SequenceEqual(expected).Should().Be(true);
	}
}