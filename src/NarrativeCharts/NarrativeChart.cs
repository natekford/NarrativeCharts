using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChart
{
	public Dictionary<Character, Hex> Colors { get; } = new();
	public SortedList<X, NarrativeEvent> Events { get; } = new(ChartUtils.XComparer);
	public Dictionary<Location, Y> Locations { get; } = new();
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<X, NarrativePoint>> Points { get; } = new();
}