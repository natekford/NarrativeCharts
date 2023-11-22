namespace NarrativeCharts.Models;

/// <summary>
/// A point.
/// </summary>
/// <param name="Hour">The time the point happened.</param>
/// <param name="Location">The location the point happened at.</param>
/// <param name="Character">The character the point involved.</param>
/// <param name="IsEnd">Whether or not this is the character's final point.</param>
/// <param name="IsTimeSkip">Whether or not this point is before a time skip.</param>
public sealed record NarrativePoint(
	float Hour,
	Location Location,
	Character Character,
	bool IsEnd,
	bool IsTimeSkip
);