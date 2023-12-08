using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

/// <summary>
/// Information for drawing a segment.
/// </summary>
/// <param name="Chart">The chart that is being drawn.</param>
/// <param name="Character">The character this segment belongs to.</param>
/// <param name="X0">The starting X value.</param>
/// <param name="X1">The ending X value.</param>
/// <param name="Y0">The starting Y value.</param>
/// <param name="Y1">The ending Y value.</param>
/// <param name="IsMovement">Whether or not this segment is considered movement.</param>
/// <param name="IsFinal">Whether or not this segment is the final point of this character.</param>
public readonly record struct LineSegment(
	NarrativeChartData Chart, Character Character,
	float X0, float X1, float Y0, float Y1,
	bool IsMovement, bool IsFinal
);