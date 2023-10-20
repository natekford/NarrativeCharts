using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChart
{
	public Dictionary<string, Color> Colors { get; } = new();
	public SortedList<int, NarrativeEvent> Events { get; } = new();
	public Dictionary<string, int> Locations { get; } = new();
	public string Name { get; set; } = "";
	public Dictionary<string, SortedList<int, NarrativePoint>> Points { get; } = new();
}