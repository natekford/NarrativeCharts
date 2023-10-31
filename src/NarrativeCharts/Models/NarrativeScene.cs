namespace NarrativeCharts.Models;

public sealed record NarrativeScene(
	int Hour,
	Location Location,
	IReadOnlyList<Character> Characters
);