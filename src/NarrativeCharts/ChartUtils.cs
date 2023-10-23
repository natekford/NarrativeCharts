using NarrativeCharts.Models;

using System.Collections.Concurrent;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static T AddChart<T>(this T chart, NarrativeChart other) where T : NarrativeChart
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

	public static T AddEvent<T>(this T chart, NarrativeEvent @event) where T : NarrativeChart
	{
		chart.Events.Add(@event.Point.X, @event);
		return chart;
	}

	public static T AddPoint<T>(this T chart, NarrativePoint point) where T : NarrativeChart
	{
		if (!chart.Points.TryGetValue(point.Character, out var points))
		{
			chart.Points[point.Character] = points = new();
		}

		points[point.Point.X] = point;
		return chart;
	}

	public static T AddScene<T>(this T chart, NarrativeScene scene) where T : NarrativeChart
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(Point: scene.Point, Character: character, IsEnd: false));
		}
		return chart;
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

	public static Dictionary<CharY, int> GetLocationOrder(this NarrativeChart chart)
	{
		var timeSpent = new ConcurrentDictionary<int, ConcurrentDictionary<string, int>>();
		foreach (var (character, points) in chart.Points)
		{
			for (var p = 0; p < points.Count - 1; ++p)
			{
				var curr = points.Values[p].Point;
				var next = points.Values[p + 1].Point;

				var xDiff = next.X - curr.X;
				timeSpent
					.GetOrAdd(curr.Y, _ => new())
					.AddOrUpdate(character, (_, a) => a, (_, a, b) => a + b, xDiff);
			}
		}

		var locationOrder = new Dictionary<CharY, int>();
		foreach (var (location, time) in timeSpent)
		{
			// more time spent = closer to the bottom
			// any ties? alphabetical order (A = bottom, Z = top)
			var ordered = time
				.OrderByDescending(x => x.Value)
				.ThenBy(x => x.Key);
			var i = 0;
			foreach (var (character, _) in ordered)
			{
				locationOrder[new(character, location)] = i++;
			}
		}

		return locationOrder;
	}

	public static EventRange GetRange(this NarrativeChart chart)
		=> EventRange.GetRange(chart);

	public static T Simplify<T>(this T chart) where T : NarrativeChart
	{
		foreach (var (_, points) in chart.Points)
		{
			// Don't bother checking the first or last points
			// They will always be valid
			for (var i = points.Count - 2; i > 0; --i)
			{
				var prev = points.Values[i - 1].Point.Y;
				var curr = points.Values[i].Point.Y;
				var next = points.Values[i + 1].Point.Y;
				if (prev == curr && curr == next)
				{
					points.Remove(points.Values[i].Point.X);
				}
			}
		}
		return chart;
	}

	public static T UpdatePoints<T>(this T chart, int x) where T : NarrativeChart
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
		return chart;
	}

	public readonly record struct CharY(string Character, int Y);
}