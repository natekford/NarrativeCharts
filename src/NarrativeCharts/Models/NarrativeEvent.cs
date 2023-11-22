namespace NarrativeCharts.Models;

/// <summary>
/// An event.
/// </summary>
/// <param name="Hour">The time the event happened.</param>
/// <param name="Name">The name of the event.</param>
public sealed record NarrativeEvent(
	float Hour,
	string Name
);