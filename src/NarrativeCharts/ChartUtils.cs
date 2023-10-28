using NarrativeCharts.Models;

namespace NarrativeCharts;

public static class ChartUtils
{
	public static T AddChart<T>(this T chart, RawNarrativeChart other) where T : RawNarrativeChart
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

	public static T AddEvent<T>(this T chart, NarrativeEvent @event) where T : RawNarrativeChart
	{
		chart.Events.Add(@event.Point.Hour, @event);
		return chart;
	}

	public static T AddPoint<T>(this T chart, NarrativePoint point) where T : RawNarrativeChart
	{
		if (!chart.Points.TryGetValue(point.Character, out var points))
		{
			chart.Points[point.Character] = points = new();
		}
		points[point.Point.Hour] = point;
		return chart;
	}

	public static T AddScene<T>(this T chart, NarrativeScene scene) where T : RawNarrativeChart
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(Point: scene.Point, Character: character, IsEnd: false));
		}
		return chart;
	}

	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(this RawNarrativeChart chart)
	{
		foreach (var (_, points) in chart.Points)
		{
			foreach (var point in points)
			{
				yield return point.Value;
			}
		}
	}

	public static T Seed<T>(this T chart, RawNarrativeChart other, int hour) where T : RawNarrativeChart
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
				Point = lastPoint.Point with
				{
					Hour = hour
				},
			});
		}
		return chart;
	}

	public static T Simplify<T>(this T chart) where T : RawNarrativeChart
	{
		foreach (var (_, points) in chart.Points)
		{
			// Don't bother checking the first or last points
			// They will always be valid
			for (var i = points.Count - 2; i > 0; --i)
			{
				var prev = points.Values[i - 1].Point.Location;
				var curr = points.Values[i].Point.Location;
				var next = points.Values[i + 1].Point.Location;
				if (prev == curr && curr == next)
				{
					points.Remove(points.Values[i].Point.Hour);
				}
			}
		}
		return chart;
	}

	public static T UpdatePoints<T>(this T chart, int hour) where T : RawNarrativeChart
		=> chart.UpdatePoints(hour, chart.Points.Keys);

	public static T UpdatePoints<T>(
		this T chart,
		int hour,
		IEnumerable<Character> characters) where T : RawNarrativeChart
	{
		foreach (var character in characters)
		{
			var lastPoint = chart.Points[character].Values[^1];
			// lastPoint already reaches up to where we're trying to update
			if (lastPoint.IsEnd || lastPoint.Point.Hour >= hour)
			{
				continue;
			}

			chart.Points[character].Add(hour, lastPoint with
			{
				Point = lastPoint.Point with
				{
					Hour = hour
				},
			});
		}
		return chart;
	}

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters);
}