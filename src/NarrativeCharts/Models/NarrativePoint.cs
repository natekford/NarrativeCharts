namespace NarrativeCharts.Models;

public sealed record NarrativePoint(
	float Hour,
	Location Location,
	Character Character,
	bool IsEnd,
	bool IsTimeSkip
);