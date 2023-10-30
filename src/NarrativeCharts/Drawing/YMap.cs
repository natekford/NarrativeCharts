using NarrativeCharts.Models;

namespace NarrativeCharts.Drawing;

public record YMap(
	Dictionary<(Character, Location), int> Characters,
	Dictionary<Location, int> Locations,
	int XMax,
	int XMin,
	int YMax,
	int YMin
)
{
	public int XRange => XMax - XMin;
	public int YRange => YMax - YMin;
}