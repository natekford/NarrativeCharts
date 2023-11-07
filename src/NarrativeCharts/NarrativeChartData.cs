using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChartData
{
	public Dictionary<Character, Hex> Colors { get; set; } = [];
	public SortedList<int, NarrativeEvent> Events { get; set; } = [];
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<int, NarrativePoint>> Points { get; set; } = [];
	public Dictionary<Location, int> YIndexes { get; set; } = [];
}