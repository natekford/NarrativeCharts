namespace NarrativeCharts.Models;

public sealed record NarrativePoint(
	Point Point,
	string Character,
	bool DoNotUpdate
) : INarrativeItem;