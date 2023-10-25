using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public static class Utils
{
	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters);
}