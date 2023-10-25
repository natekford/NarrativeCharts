using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChart
{
	public SortedList<int, NarrativeEvent> Events { get; } = new();
	public string Name { get; set; } = "";
	public Dictionary<Character, SortedList<int, NarrativePoint>> Points { get; } = new();
}