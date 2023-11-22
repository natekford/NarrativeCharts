namespace NarrativeCharts.Models;

/// <summary>
/// A scene.
/// </summary>
/// <param name="Hour">The time the scene happened.</param>
/// <param name="Location">The location this scene happened at.</param>
/// <param name="Characters">The characters this scene involved.</param>
public sealed record NarrativeScene(
	float Hour,
	Location Location,
	IEnumerable<Character> Characters
);