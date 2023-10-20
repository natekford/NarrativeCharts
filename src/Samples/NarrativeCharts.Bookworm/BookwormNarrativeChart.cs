﻿using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChart
{
	protected bool AlreadyCreaated { get; set; }
	protected BookwormTimeTracker Time { get; }

	protected BookwormNarrativeChart(BookwormTimeTracker time)
	{
		Time = time;
		foreach (var (character, color) in BookwormCharacters.ColorValues)
		{
			Colors[character] = color.Hex;
		}
		foreach (var (location, y) in BookwormLocations.YValues)
		{
			Locations[location] = y;
		}
	}

	public BookwormNarrativeChart Create()
	{
		if (AlreadyCreaated)
		{
			return this;
		}

		ProtectedCreate();
		AlreadyCreaated = true;
		return this;
	}

	protected void Add(NarrativeScene scene)
	{
		this.AddScene(scene);
		Update();
	}

	protected void Chapter(string name, bool addEvent = true)
	{
		if (addEvent)
		{
			this.AddEvent(new(Now(0), name));
		}
		Update();
	}

	protected void Kill(params Character[] characters)
	{
		Update();
		foreach (var character in characters)
		{
			var points = Points[character.Name];
			var point = points.Values[^1];
			points[point.Point.X] = point with
			{
				IsEnd = true,
			};
		}
	}

	protected Point Now(int y)
		=> new(Time.CurrentTotalHours, y);

	protected abstract void ProtectedCreate();

	protected Point Scene(Location location)
		=> Now(Locations[location.Name]);

	protected void Update()
		=> this.UpdatePoints(Time.CurrentTotalHours);

	protected void UpdateAndAddBell()
	{
		Update();
		Time.AddBell();
	}
}