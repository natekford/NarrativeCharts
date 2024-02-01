using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;
using NarrativeCharts.Tests.Properties;

using SkiaSharp;

using System.Runtime.InteropServices;

namespace NarrativeCharts.Tests.Drawing.Skia;

[Trait("Category", "Integration")]
public class SKChartDrawer_Tests
{
	private SKChartDrawer Drawer { get; } = new(GetTypeFace())
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
			actual.SequenceEqual(Resources.ExpectedP3V1WindowsSegoeUI).Should().Be(true);
		}
		else if (actual.Length == 1_112_636)
		{
			actual.SequenceEqual(Resources.ExpectedP3V1LinuxLiberationMono).Should().Be(true);
		}
		else
		{
			var bounds = SKBitmap.DecodeBounds(actual);
			bounds.Width.Should().Be(13_100);
			bounds.Height.Should().Be(4_000);
			bounds.ColorType.Should().Be(SKColorType.Rgb565);

			Assert.Fail("Unexpected output length.");
		}
	}

	private static SKTypeface GetTypeFace()
	{
		Console.WriteLine($"Installed fonts: {string.Join(", ", SKFontManager.Default.FontFamilies)}");

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			// Github Actions has this font, idk if other distros have it
			return GetTypeFace("Liberation Mono");
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return GetTypeFace("Segoe UI");
		}
		else
		{
			throw new InvalidOperationException("Only currently supports Ubuntu and Windows.");
		}
	}

	private static SKTypeface GetTypeFace(string familyName)
	{
		var typeface = SKTypeface.FromFamilyName(familyName);
		typeface.FamilyName.Should().Be(familyName);
		return typeface;
	}
}