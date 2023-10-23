namespace NarrativeCharts.Models;

public sealed record NarrativePoint(
	Point Point,
	Character Character,
	bool IsEnd
);