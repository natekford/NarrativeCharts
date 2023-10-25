using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChart
{
	protected bool AlreadyCreated { get; set; }
	protected Location Frozen { get; } = new Location(nameof(Freeze));
	protected Y FrozenY { get; } = new(int.MinValue);
	protected BookwormTimeTracker Time { get; }
	protected X X => new(Time.CurrentTotalHours);

	protected BookwormNarrativeChart(BookwormTimeTracker time)
	{
		Time = time;
		foreach (var (character, color) in BookwormCharacters.ColorValues)
		{
			Colors[character] = color;
		}
		Locations[Frozen] = FrozenY;
		foreach (var (location, y) in BookwormLocations.YValues)
		{
			Locations[location] = y;
		}
	}

	public void Initialize(BookwormNarrativeChart? seed)
	{
		if (AlreadyCreated)
		{
			return;
		}
		AlreadyCreated = true;

		if (seed is not null)
		{
			this.Seed(seed, X);
		}

		ProtectedCreate();

		Locations.Remove(Frozen);
		foreach (var points in Points.Values)
		{
			for (var i = points.Count - 1; i >= 0; --i)
			{
				if (points.Values[i].Point.Y == FrozenY)
				{
					points.RemoveAt(i);
				}
			}
		}
		this.Simplify();
	}

	protected void Add(NarrativeScene scene)
		=> this.AddScene(scene);

	protected void AddBell(int amount = 1)
	{
		Update();
		Time.AddBells(amount);
	}

	protected Dictionary<Character, Y> AddR(NarrativeScene scene)
	{
		var dict = scene.Characters
			.ToDictionary(x => x, x => Points[x].Values[^1].Point.Y);
		Add(scene);
		return dict;
	}

	protected void Chapter(string name)
	{
		this.AddEvent(new(new(X, new(0)), name));
		Update();
	}

	protected void Freeze(params Character[] characters)
		=> Add(Scene(Frozen).With(characters));

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

	protected void Return(IEnumerable<KeyValuePair<Character, Y>> scene)
	{
		foreach (var (character, y) in scene)
		{
			this.AddPoint(new(
				Point: new(X, y),
				Character: character,
				IsEnd: false
			));
		}
	}

	protected Point Scene(Location location)
		=> new(X, Locations[location]);

	protected void SkipToCurrentDay(BookwormBell bell)
		=> SkipToDaysAhead(0, bell);

	protected void SkipToDaysAhead(int days, BookwormBell bell)
	{
		Time.SkipToDaysAheadStart(days).SetBell(bell - 1);
		AddBell();
	}

	protected void SkipToNextDay(BookwormBell bell)
		=> SkipToDaysAhead(1, bell);

	protected void Update()
		=> this.UpdatePoints(X);
}