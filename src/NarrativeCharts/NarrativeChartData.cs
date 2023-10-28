using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChartData
{
	public Dictionary<Character, Hex> Colors { get; } = new();
	public SortedList<int, NarrativeEvent> Events { get; } = new();
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<int, NarrativePoint>> Points { get; } = new();
	public Dictionary<Location, int> YIndexes { get; } = new();
}