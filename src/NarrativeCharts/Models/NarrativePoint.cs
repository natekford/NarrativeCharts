namespace NarrativeCharts.Models;

public sealed record NarrativePoint(
	Point Point,
	string Character,
	LineModifier LineModifier,
	Color LineColor,
	int LineThickness
) : INarrativeItem;