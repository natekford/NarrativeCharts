using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts.Drawing;

public static class ColorCache<TColor>
{
	public static ConcurrentDictionary<Hex, TColor> Cache { get; } = [];
}