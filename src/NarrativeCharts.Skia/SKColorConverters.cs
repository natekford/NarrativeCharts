using NarrativeCharts.Models;

using SkiaSharp;

using System.Collections.Concurrent;

namespace NarrativeCharts.Skia;

public static class SKColorConverters
{
	private static readonly ConcurrentDictionary<Hex, Hex> _Complementary = [];
	private static readonly ConcurrentDictionary<Hex, Hex> _Darken = [];

	public static Func<Hex, Hex> Complementary { get; } = hex =>
	{
		return _Complementary.GetOrAdd(hex, x =>
		{
			SKColor.Parse(x.Value).ToHsv(out var h, out var s, out var v);
			var complementary = SKColor.FromHsv((h + 180) % 360, s, v);
			return new(complementary.ToString());
		});
	};

	public static Func<Hex, Hex> Darken { get; } = hex =>
	{
		return _Darken.GetOrAdd(hex, x =>
		{
			SKColor.Parse(x.Value).ToHsv(out var h, out var s, out var v);
			var darkened = SKColor.FromHsv(h, s, v * 0.75f);
			return new(darkened.ToString());
		});
	};

	public static Func<Hex, Hex> Color(SKColor color)
	{
		var hex = new Hex(color.ToString());
		return _ => hex;
	}
}