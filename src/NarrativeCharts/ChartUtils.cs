﻿using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static T AddChart<T>(this T chart, NarrativeChartData other) where T : NarrativeChartData
	{
		foreach (var @event in other.Events)
		{
			chart.AddEvent(@event.Value);
		}
		foreach (var point in other.GetAllNarrativePoints())
		{
			chart.AddPoint(point);
		}
		return chart;
	}

	public static T AddEvent<T>(this T chart, NarrativeEvent @event) where T : NarrativeChartData
	{
		chart.Events.Add(@event.Hour, @event);
		return chart;
	}

	public static T AddPoint<T>(this T chart, NarrativePoint point) where T : NarrativeChartData
	{
		if (!chart.Points.TryGetValue(point.Character, out var points))
		{
			chart.Points[point.Character] = points = new();
		}
		points[point.Hour] = point;
		return chart;
	}

	public static T AddScene<T>(this T chart, NarrativeScene scene) where T : NarrativeChartData
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(
				Hour: scene.Hour,
				Location: scene.Location,
				Character: character,
				IsEnd: false
			));
		}
		return chart;
	}

	public static NarrativeChartData Combine(this IEnumerable<NarrativeChartData> charts)
	{
		var combined = new NarrativeChartData();
		foreach (var chart in charts)
		{
			combined.AddChart(chart);

			foreach (var (character, color) in chart.Colors)
			{
				combined.Colors.TryAdd(character, color);
			}
			foreach (var (location, yIndex) in chart.YIndexes)
			{
				combined.YIndexes.TryAdd(location, yIndex);
			}
		}
		combined.Simplify();
		return combined;
	}

	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(this NarrativeChartData chart)
	{
		foreach (var (_, points) in chart.Points)
		{
			foreach (var point in points)
			{
				yield return point.Value;
			}
		}
	}

	public static Dictionary<Character, Location> GetCurrentLocations(
		this NarrativeChartData chart,
		IEnumerable<Character> characters)
	{
		return characters.ToDictionary(
			x => x,
			x => chart.Points[x].Values[^1].Location
		);
	}

	public static T Seed<T>(this T chart, NarrativeChartData other, int hour) where T : NarrativeChartData
	{
		foreach (var (_, points) in other.Points)
		{
			var lastPoint = points.Values[^1];
			if (lastPoint.IsEnd)
			{
				continue;
			}

			chart.AddPoint(lastPoint with
			{
				Hour = hour,
			});
		}
		return chart;
	}

	public static T Simplify<T>(this T chart) where T : NarrativeChartData
	{
		foreach (var (_, points) in chart.Points)
		{
			// Don't bother checking the first or last points
			// They will always be valid
			for (var i = points.Count - 2; i > 0; --i)
			{
				var prev = points.Values[i - 1].Location;
				var curr = points.Values[i].Location;
				var next = points.Values[i + 1].Location;
				if (prev == curr && curr == next)
				{
					points.Remove(points.Values[i].Hour);
				}
			}
		}
		return chart;
	}

	public static T UpdatePoints<T>(this T chart, int hour) where T : NarrativeChartData
		=> chart.UpdatePoints(hour, chart.Points.Keys);

	public static T UpdatePoints<T>(
		this T chart,
		int hour,
		IEnumerable<Character> characters) where T : NarrativeChartData
	{
		foreach (var character in characters)
		{
			var lastPoint = chart.Points[character].Values[^1];
			// lastPoint already reaches up to where we're trying to update
			if (lastPoint.IsEnd || lastPoint.Hour >= hour)
			{
				continue;
			}

			chart.AddPoint(lastPoint with
			{
				Hour = hour
			});
		}
		return chart;
	}
}