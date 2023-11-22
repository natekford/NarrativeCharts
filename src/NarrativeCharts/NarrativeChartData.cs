using NarrativeCharts.Models;

namespace NarrativeCharts;

/// <summary>
/// The information of a chart.
/// </summary>
public class NarrativeChartData
{
	/// <summary>
	/// The colors to use for each character's line.
	/// </summary>
	public Dictionary<Character, Hex> Colors { get; set; } = [];
	/// <summary>
	/// The events to mark on the X axis.
	/// </summary>
	public SortedList<float, NarrativeEvent> Events { get; set; } = [];
	/// <summary>
	/// The name of this chart.
	/// </summary>
	public string Name { get; set; } = "";
	/// <summary>
	/// The points describing each character's path.
	/// </summary>
	public Dictionary<Character, SortedList<float, NarrativePoint>> Points { get; set; } = [];
	/// <summary>
	/// The order of each location. A higher value indicates a higher position in
	/// the grid.
	/// </summary>
	public Dictionary<Location, int> YIndexes { get; set; } = [];
}