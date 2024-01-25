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
		// Windows
		if (actual.Length == 1085801)
		{
			actual.SequenceEqual(Resources.ExpectedP3V1Windows).Should().Be(true);
		}
		// Linux/Github Actions
		else if (actual.Length == 1098576)
		{
			actual.SequenceEqual(Resources.ExpectedP3V1Linux).Should().Be(true);
		}
		else
		{
			Assert.Fail("Unexpected drawn image length.");
		}
	}
}