using NarrativeCharts.Models;

using SkiaSharp;

namespace NarrativeCharts.Skia;

public static class SKColorConverters
{
	private static readonly Dictionary<SKColor, Hex> _Color = [];
	private static readonly Dictionary<Hex, Hex> _Complementary = [];

	public static Func<Hex, Hex> Complementary { get; } = hex =>
	{
		if (!_Complementary.TryGetValue(hex, out var value))
		{
			SKColor.Parse(hex.Value).ToHsv(out var h, out var s, out var v);
			var complementary = SKColor.FromHsv((h + 180) % 360, s, v);
			_Complementary[hex] = value = new(complementary.ToString());
		}
		return value;
	};

	public static Func<Hex, Hex> Color(SKColor color)
	{
		if (!_Color.TryGetValue(color, out var hex))
		{
			_Color[color] = hex = new(color.ToString());
		}
		return _ => hex;
	}
}