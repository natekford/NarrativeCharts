namespace NarrativeCharts.Models;

public sealed record NarrativeScene(
	Point Point,
	IReadOnlyList<string> Characters
);