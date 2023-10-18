using NarrativeCharts.Image;
using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static void AddScene(this NarrativeChartGenerator chart, NarrativeScene scene)
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(
				Point: scene.Point,
				Character: character,
				LineModifier: default,
				LineColor: Characters.CharacterDictionary[character].Color.ToNarrativeChartColor(),
				LineThickness: 2
			));
		}
	}

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters.Select(x => x.Name).ToArray());
}