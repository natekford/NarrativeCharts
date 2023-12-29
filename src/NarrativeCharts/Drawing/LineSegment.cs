namespace NarrativeCharts.Drawing;

/// <summary>
/// Information for drawing a character's line segment onto an image.
/// </summary>
/// <param name="X0">The starting X value.</param>
/// <param name="X1">The ending X value.</param>
/// <param name="Y0">The starting Y value.</param>
/// <param name="Y1">The ending Y value.</param>
/// <param name="IsMovement">Whether or not this segment is considered movement.</param>
/// <param name="IsFinal">Whether or not this segment is the final point of this character.</param>
public readonly record struct LineSegment(
	float X0, float X1, float Y0, float Y1,
	bool IsMovement, bool IsFinal
);