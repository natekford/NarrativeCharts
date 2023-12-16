using NarrativeCharts.Models;

using SkiaSharp;

using System.Collections.Concurrent;

namespace NarrativeCharts.Skia;

public static class SKColorConverters
{
	private static readonly ConcurrentDictionary<Hex, Hex> _Complementary = [];

	public static Func<Hex, Hex> Complementary { get; } = hex =>
	{
		return _Complementary.GetOrAdd(hex, x =>
		{
			SKColor.Parse(x.Value).ToHsv(out var h, out var s, out var v);
			var complementary = SKColor.FromHsv((h + 180) % 360, s, v);
			return new(complementary.ToString());
		});
	};

	public static Func<Hex, Hex> Color(SKColor color)
	{
		var hex = new Hex(color.ToString());
		return _ => hex;
	}
}