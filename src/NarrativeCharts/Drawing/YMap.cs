using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

public record YMap(
	Dictionary<(Character, Location), float> Characters,
	Dictionary<Location, float> Locations,
	float XMax,
	float XMin,
	float YMax,
	float YMin
)
{
	public float XRange => XMax - XMin;
	public float YRange => YMax - YMin;
}