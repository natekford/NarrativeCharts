namespace NarrativeCharts.Models;

public sealed record NarrativeScene(
	Point Point,
	IReadOnlyList<Character> Characters
);