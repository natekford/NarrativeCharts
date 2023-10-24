using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChart
{
	protected bool AlreadyCreaated { get; set; }
	protected BookwormTimeTracker Time { get; }
	protected X X => new(Time.CurrentTotalHours);

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
			this.Seed(seed, X);
		}

		ProtectedCreate();
		this.Simplify();
		AlreadyCreaated = true;
	}

	protected NarrativeScene Add(NarrativeScene scene)
	{
		this.AddScene(scene);
		Update();
		return scene;
	}

	protected void Chapter(string name)
	{
		this.AddEvent(new(new(X, new(0)), name));
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

	protected void Return(NarrativeScene scene)
	{
		foreach (var character in scene.Characters)
		{
			// should probably binary search instead of
			// just using .Last, but LINQ go brrrr
			var previousLocation = Points[character].Values
				.Last(x => x.Point.X.Value < scene.Point.X.Value);
			this.AddPoint(new(
				Point: new(X, previousLocation.Point.Y),
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
		Update();
		Time.AddBell();
	}

	protected void SkipToNextDay(BookwormBell bell)
		=> SkipToDaysAhead(1, bell);

	protected void Update()
		=> this.UpdatePoints(X);
}