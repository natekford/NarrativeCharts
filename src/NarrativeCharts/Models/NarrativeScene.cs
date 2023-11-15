namespace NarrativeCharts.Models;

public sealed record NarrativeScene(
	float Hour,
	Location Location,
	IEnumerable<Character> Characters
);