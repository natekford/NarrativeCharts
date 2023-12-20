using NarrativeCharts.Models;

using SkiaSharp;

using System.Collections.Concurrent;

namespace NarrativeCharts.Skia;

/// <summary>
/// Converts colors to other colors.
/// </summary>
public static class SKColorConverters
{
	private static readonly ConcurrentDictionary<Hex, Hex> _Complementary = [];
	private static readonly ConcurrentDictionary<Hex, Hex> _Darken = [];

	/// <summary>
	/// Converts the passed in color to its complementary color.
	/// </summary>
	public static Func<Hex, Hex> Complementary { get; } = hex =>
	{
		return _Complementary.GetOrAdd(hex, x =>
		{
			SKColor.Parse(x.Value).ToHsv(out var h, out var s, out var v);
			var complementary = SKColor.FromHsv((h + 180) % 360, s, v);
			return new(complementary.ToString());
		});
	};

	/// <summary>
	/// Only returns a specific color.
	/// </summary>
	/// <param name="color"></param>
	/// <returns></returns>
	public static Func<Hex, Hex> Color(SKColor color)
	{
		var hex = new Hex(color.ToString());
		return _ => hex;
	}

	/// <summary>
	/// Darkens the passed in color by <paramref name="brightnessPercentage"/>.
	/// </summary>
	/// <param name="brightnessPercentage"></param>
	/// <returns></returns>
	public static Func<Hex, Hex> Darken(float brightnessPercentage)
	{
		Hex Darken(Hex x)
		{
			SKColor.Parse(x.Value).ToHsv(out var h, out var s, out var v);
			var darkened = SKColor.FromHsv(h, s, v * brightnessPercentage);
			return new(darkened.ToString());
		}

		var valueFactory = Darken;
		return hex => _Darken.GetOrAdd(hex, valueFactory);
	}
}