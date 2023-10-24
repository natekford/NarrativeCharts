using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static Comparer<X> XComparer { get; }
		= Comparer<X>.Create((a, b) => a.Value.CompareTo(b.Value));

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
			chart.Points[point.Character] = points = new(XComparer);
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

	public static T Seed<T>(this T chart, NarrativeChart other, X start) where T : NarrativeChart
	{
		foreach (var (_, points) in other.Points)
		{
			var lastPoint = points.Values[^1];
			chart.AddPoint(lastPoint with
			{
				Point = lastPoint.Point with
				{
					X = start
				},
			});
		}
		return chart;
	}

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

	public static T UpdatePoints<T>(this T chart, X x) where T : NarrativeChart
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
}