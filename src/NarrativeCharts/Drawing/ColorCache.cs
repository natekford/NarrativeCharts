using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts.Drawing;

/// <summary>
/// Caches <typeparamref name="TColor"/> parsed from a <see cref="Hex"/>.
/// </summary>
/// <typeparam name="TColor"></typeparam>
public static class ColorCache<TColor>
{
	/// <summary>
	/// The cached <typeparamref name="TColor"/>.
	/// </summary>
	public static ConcurrentDictionary<Hex, TColor> Cache { get; } = [];
}