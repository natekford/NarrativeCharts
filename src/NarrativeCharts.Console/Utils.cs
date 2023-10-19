using NarrativeCharts.Image;
using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static void AddScene(this NarrativeChart chart, NarrativeScene scene)
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(scene.Point, character);
		}
	}

	public static void AddScene(this NarrativeChart chart, InterpedNarrativeScene interpedScene)
	{
		var scene = interpedScene.Scene;
		foreach (var character in scene.Characters)
		{
			var previousPoint = chart.Points[character].Values[^1];
			var (pX, pY) = previousPoint.Point;
			var (nX, nY) = scene.Point;

			var diffX = nX - pX;
			var diffY = nY - pY;

			var start = pX + (int)Math.Ceiling(diffX * interpedScene.Start);
			var end = pX + (int)Math.Ceiling(diffX * interpedScene.End);
			var diffI = end - start;

			var i = 1;
			for (var progress = 0d; progress < 1; progress += interpedScene.Step * i, ++i)
			{
				var iX = start + (int)Math.Ceiling(diffI * progress);
				var iY = pY + (int)Math.Ceiling(diffY * progress);
				chart.AddPoint(previousPoint with
				{
					Point = new(iX, iY),
				});
			}

			// add the uninterped point
			chart.AddPoint(scene.Point, character);
		}
	}

	public static InterpedNarrativeScene Interped(this NarrativeScene scene, double? start = null, double? end = null, double? step = null)
		=> new(scene, start ?? .9, end ?? 1, step ?? .09);

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters.Select(x => x.Name).ToArray());

	public readonly record struct InterpedNarrativeScene(NarrativeScene Scene, double Start, double End, double Step);

	private static void AddPoint(this NarrativeChart chart, Point point, string character)
	{
		chart.AddPoint(new(
			Point: point,
			Character: character,
			LineModifier: default,
			LineColor: Characters.CharacterDictionary[character].Color.ToNarrativeChartColor(),
			LineThickness: 2
		));
	}
}