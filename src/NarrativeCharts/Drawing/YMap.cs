using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

/// <summary>
/// Holds Y values to use when drawing a chart.
/// </summary>
/// <param name="Characters">The Y values to use for each character at a location.</param>
/// <param name="Locations">The Y values to use for each location.</param>
/// <param name="XMax">The largest X value in this chart.</param>
/// <param name="XMin">The smallest X value in this chart.</param>
/// <param name="YMax">The largest Y value in this chart.</param>
/// <param name="YMin">The smallest Y value in this chart.</param>
public record YMap(
	Dictionary<(Character, Location), float> Characters,
	Dictionary<Location, float> Locations,
	float XMax,
	float XMin,
	float YMax,
	float YMin
)
{
	/// <summary>
	/// The difference between <see cref="XMax"/> and <see cref="XMin"/>.
	/// </summary>
	public float XRange => XMax - XMin;
	/// <summary>
	/// The difference between <see cref="YMax"/> and <see cref="YMin"/>.
	/// </summary>
	public float YRange => YMax - YMin;
}