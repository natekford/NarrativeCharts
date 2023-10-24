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
			Colors[character] = color;
		}
		foreach (var (location, y) in BookwormLocations.YValues)
		{
			Locations[location] = y;
		}
	}

	public void Initialize(BookwormNarrativeChart? seed)
	{
		if (AlreadyCreaated)
		{
			return;
		}

		if (seed is not null)
		{
			this.Seed(seed, new(Time.CurrentTotalHours));
		}

		ProtectedCreate();
		this.Simplify();
		AlreadyCreaated = true;
	}

	protected void Add(NarrativeScene scene)
	{
		this.AddScene(scene);
		Update();
	}

	protected void Chapter(string name)
	{
		this.AddEvent(new(new(new(Time.CurrentTotalHours), new(0)), name));
		Update();
	}

	protected void Kill(params Character[] characters)
	{
		Update();
		foreach (var character in characters)
		{
			var points = Points[character];
			var point = points.Values[^1];
			points[point.Point.X] = point with
			{
				IsEnd = true,
			};
		}
	}

	protected abstract void ProtectedCreate();

	protected Point Scene(Location location)
		=> new(new(Time.CurrentTotalHours), Locations[location]);

	protected void SkipToCurrentDay(BookwormBell bell)
		=> SkipToDaysAhead(0, bell);

	protected void SkipToDaysAhead(int days, BookwormBell bell)
	{
		Time.SkipToDaysAheadStart(days).SetBell(bell - 1);
		Update();
		Time.AddBell();
	}

	protected void SkipToNextDay(BookwormBell bell)
		=> SkipToDaysAhead(1, bell);

	protected void Update()
		=> this.UpdatePoints(new(Time.CurrentTotalHours));
}