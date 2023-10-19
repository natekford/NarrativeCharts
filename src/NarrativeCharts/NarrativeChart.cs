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
		if (!Points.TryGetValue(point.Character, out var set))
		{
			Points[point.Character] = set = new();
		}

		set[point.Point.X] = point;
	}
}