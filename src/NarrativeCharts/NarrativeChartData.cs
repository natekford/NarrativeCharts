using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChartData
{
	public Dictionary<Character, Hex> Colors { get; } = [];
	public SortedList<int, NarrativeEvent> Events { get; } = [];
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<int, NarrativePoint>> Points { get; } = [];
	public Dictionary<Location, int> YIndexes { get; } = [];
}