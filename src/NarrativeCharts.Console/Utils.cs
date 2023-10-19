using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters.Select(x => x.Name).ToArray());
}