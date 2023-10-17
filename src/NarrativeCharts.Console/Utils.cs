using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static NarrativeScene With(this Point point, params string[] characters)
		=> new(point, characters);
}