﻿using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static void AddChart(this NarrativeChart chart, NarrativeChart other)
	{
		foreach (var @event in other.Events)
		{
			chart.AddEvent(@event.Value);
		}
		foreach (var point in other.GetAllNarrativePoints())
		{
			chart.AddPoint(point);
		}
	}

	public static void AddEvent(this NarrativeChart chart, NarrativeEvent @event)
		=> chart.Events.Add(@event.Point.X, @event);

	public static void AddPoint(this NarrativeChart chart, NarrativePoint point)
	{
		if (!chart.Points.TryGetValue(point.Character, out var points))
		{
			chart.Points[point.Character] = points = new();
		}

		points[point.Point.X] = point;
	}

	public static void AddScene(this NarrativeChart chart, NarrativeScene scene)
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(Point: scene.Point, Character: character, IsEnd: false));
		}
	}

	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(this NarrativeChart chart)
	{
		foreach (var (_, points) in chart.Points)
		{
			foreach (var point in points)
			{
				yield return point.Value;
			}
		}
	}

	public static EventRange GetRange(this NarrativeChart chart)
		=> EventRange.GetRange(chart);

	public static void UpdatePoints(this NarrativeChart chart, int x)
	{
		foreach (var (character, points) in chart.Points)
		{
			var lastPoint = points.Values[^1];
			// lastPoint already reaches up to where we're trying to update
			if (lastPoint.IsEnd || lastPoint.Point.X == x)
			{
				continue;
			}

			chart.Points[character].Add(x, lastPoint with
			{
				Point = lastPoint.Point with
				{
					X = x
				},
			});
		}
	}
}