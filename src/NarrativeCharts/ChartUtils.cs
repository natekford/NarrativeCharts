using NarrativeCharts.Models;

namespace NarrativeCharts;

/// <summary>
/// Utilities for <see cref="NarrativeChartData"/>.
/// </summary>
public static class ChartUtils
{
	/// <summary>
	/// Copies all <see cref="NarrativePoint"/> and <see cref="NarrativeEvent"/>
	/// from <paramref name="other"/> to <paramref name="chart"/>.
	/// This does NOT copy <see cref="NarrativeChartData.Colors"/> or
	/// <see cref="NarrativeChartData.YIndexes"/>,
	/// for that use <see cref="Combine(IEnumerable{NarrativeChartData})"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="other"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Adds <paramref name="event"/> to <paramref name="chart"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="event"></param>
	/// <returns></returns>
	public static T AddEvent<T>(this T chart, NarrativeEvent @event) where T : NarrativeChartData
	{
		chart.Events.Add(@event.Hour, @event);
		return chart;
	}

	/// <summary>
	/// Adds <paramref name="point"/> to <paramref name="chart"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="point"></param>
	/// <returns></returns>
	public static T AddPoint<T>(this T chart, NarrativePoint point) where T : NarrativeChartData
	{
		if (!chart.Points.TryGetValue(point.Character, out var points))
		{
			chart.Points[point.Character] = points = [];
		}
		points[point.Hour] = point;
		return chart;
	}

	/// <summary>
	/// Adds multiple <see cref="NarrativePoint"/> created from <paramref name="scene"/>
	/// to <paramref name="chart"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="scene"></param>
	/// <returns></returns>
	public static T AddScene<T>(this T chart, NarrativeScene scene) where T : NarrativeChartData
	{
		foreach (var character in scene.Characters)
		{
			chart.AddPoint(new(
				Hour: scene.Hour,
				Location: scene.Location,
				Character: character,
				IsEnd: false,
				IsTimeSkip: false
			));
		}
		return chart;
	}

	/// <summary>
	/// Creates an entirely new <see cref="NarrativeChartData"/> and adds everything
	/// to that chart. This includes <see cref="NarrativeChartData.Colors"/> and
	/// <see cref="NarrativeChartData.YIndexes"/>.
	/// </summary>
	/// <param name="charts"></param>
	/// <returns></returns>
	public static NarrativeChartData Combine(
		this IEnumerable<NarrativeChartData> charts)
	{
		var combined = new CombinedNarrativeChart();
		foreach (var chart in charts)
		{
			// Events/Points will throw if duplicate items are added at the same time
			// because charts are unlikely to have duplicate items at a specific time
			combined.AddChart(chart);

			// Colors/YIndexes use TryAdd instead of Add because charts are likely
			// to have the same Colors/Indexes, first come first serve
			foreach (var (character, color) in chart.Colors)
			{
				combined.Colors.TryAdd(character, color);
			}
			foreach (var (location, yIndex) in chart.YIndexes)
			{
				combined.YIndexes.TryAdd(location, yIndex);
			}
		}
		combined.Initialize(null);
		return combined;
	}

	/// <summary>
	/// Gets all narrative points from <paramref name="chart"/>.
	/// </summary>
	/// <param name="chart"></param>
	/// <returns></returns>
	public static IEnumerable<NarrativePoint> GetAllNarrativePoints(
		this NarrativeChartData chart)
	{
		foreach (var (_, points) in chart.Points)
		{
			foreach (var point in points)
			{
				yield return point.Value;
			}
		}
	}

	/// <summary>
	/// Gets the current locations of <paramref name="characters"/>
	/// from <paramref name="chart"/>.
	/// </summary>
	/// <param name="chart"></param>
	/// <param name="characters"></param>
	/// <returns></returns>
	public static Dictionary<Character, Location> GetCurrentLocations(
		this NarrativeChartData chart,
		IEnumerable<Character> characters)
	{
		return characters.ToDictionary(
			x => x,
			x => chart.Points[x].Values[^1].Location
		);
	}

	/// <summary>
	/// Modifies the last points of each character.
	/// </summary>
	/// <param name="points"></param>
	/// <param name="modification"></param>
	public static void ModifyLastPoint(
		this SortedList<float, NarrativePoint> points,
		Func<NarrativePoint, NarrativePoint> modification)
	{
		var lastPoint = points.Values[^1];
		points[lastPoint.Hour] = modification.Invoke(lastPoint);
	}

	/// <summary>
	/// Adds in the last points of each character from <paramref name="other"/>
	/// to <paramref name="chart"/> with <see cref="NarrativePoint.Hour"/>
	/// as <paramref name="hour"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="other"></param>
	/// <param name="hour"></param>
	/// <returns></returns>
	public static T Seed<T>(
		this T chart,
		NarrativeChartData other,
		float hour) where T : NarrativeChartData
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

	/// <summary>
	/// Adds a point for each character with their latest location at <paramref name="hour"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="hour"></param>
	/// <returns></returns>
	public static T UpdatePoints<T>(this T chart, float hour) where T : NarrativeChartData
		=> chart.UpdatePoints(hour, chart.Points.Keys);

	/// <summary>
	/// Adds a point for characters specified within <paramref name="characters"/> with
	/// their latest location at <paramref name="hour"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="chart"></param>
	/// <param name="hour"></param>
	/// <param name="characters"></param>
	/// <returns></returns>
	public static T UpdatePoints<T>(
		this T chart,
		float hour,
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
				Hour = hour,
				IsTimeSkip = false,
			});
		}
		return chart;
	}

	private class CombinedNarrativeChart : NarrativeChart
	{
		public CombinedNarrativeChart() : base(null!)
		{
		}

		protected override void AddNarrativeData()
		{
		}
	}
}