namespace NarrativeCharts.Models;

public sealed record NarrativePoint(
	int Hour,
	Location Location,
	Character Character,
	bool IsEnd,
	bool IsTimeSkip
);