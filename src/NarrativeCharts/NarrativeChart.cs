﻿using NarrativeCharts.Models;

namespace NarrativeCharts;

public class NarrativeChart
{
	public Dictionary<string, string> Colors { get; } = new();
	public SortedList<int, NarrativeEvent> Events { get; } = new();
	public Dictionary<string, int> Locations { get; } = new();
	public Dictionary<string, SortedList<int, NarrativePoint>> Points { get; } = new();

	public void AddEvent(NarrativeEvent @event)
		=> Events.Add(@event.Point.X, @event);

	public void AddPoint(NarrativePoint point)
	{
		if (!Points.TryGetValue(point.Character, out var points))
		{
			Points[point.Character] = points = new();
		}

		points[point.Point.X] = point;
	}
}