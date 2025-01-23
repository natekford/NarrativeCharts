using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;
using NarrativeCharts.Tests.Properties;

using SkiaSharp;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Xunit.Abstractions;

namespace NarrativeCharts.Tests.Drawing.Skia;

[Trait("Category", "Integration")]
public class SKChartDrawer_Tests
{
	private readonly HashSet<string> _Fonts = SKFontManager.Default.FontFamilies.ToHashSet();
	private readonly ITestOutputHelper _Output;

	public SKChartDrawer_Tests(ITestOutputHelper output)
	{
		_Output = output;
	}

	[Fact]
	public async Task DrawImage_Valid()
	{
		_Output.WriteLine($"Fonts: {string.Join(", ", _Fonts)}");

		var defs = TestUtils.Defs;
		var script = defs.LoadScripts().First();
		using var expected = GetExpectedImage();
		var drawer = new SKChartDrawer(expected.Typeface)
		{
			ImageAspectRatio = null,
			CharacterLabelColorConverter = null,
		};

		var path = Path.Combine(defs.ScriptDirectory, nameof(SKChartDrawer_Tests), "ActualP3V1.png");
		await drawer.SaveChartAsync(script, path).ConfigureAwait(true);

		var actual = File.ReadAllBytes(path);
		actual.SequenceEqual(expected.ExpectedBytes).Should().Be(
			expected: true,
			because: "Drawn does not match expected. Likely character/location name change or font mismatch."
		);
	}

	private ExpectedImage GetExpectedImage()
	{
		// I'm not sure if the same font on a different operating system will
		// produce different images, so that's why I'm checking both OS and font
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			if (TryGetTypeface("Liberation Mono", out var typeface))
			{
				return new(typeface, Resources.ExpectedP3V1LinuxLiberationMono);
			}
			else if (TryGetTypeface("DejaVu Sans Mono", out typeface))
			{
				// TODO: if this ever gets reached by github actions put the
				// created image in here
				return new(typeface, []);
			}
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			if (TryGetTypeface("Segoe UI", out var typeface))
			{
				return new(typeface, Resources.ExpectedP3V1WindowsSegoeUI);
			}
		}

		throw new InvalidOperationException("Unable to find a supported typeface.");
	}

	private bool TryGetTypeface(string familyName, [NotNullWhen(true)] out SKTypeface? typeface)
	{
		var exists = _Fonts.Contains(familyName);
		typeface = exists ? SKTypeface.FromFamilyName(familyName) : null;
		typeface?.FamilyName.Should().Be(familyName);
		return exists;
	}

	private record ExpectedImage(
		SKTypeface Typeface,
		byte[] ExpectedBytes
	) : IDisposable
	{
		public void Dispose() => Typeface.Dispose();
	}
}