namespace NarrativeCharts.Models;

public sealed record NarrativeEvent(
	Point Point,
	string Name
) : INarrativeItem;