using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChart
{
	public abstract string Name { get; }
	protected bool AlreadyCreaated { get; set; }
	protected BookwormTimeTracker Time { get; }

	protected BookwormNarrativeChart(BookwormTimeTracker time)
	{
		Time = time;
		foreach (var (_, character) in BookwormCharacters.Dictionary)
		{
			Colors[character.Name] = character.Color;
		}
		foreach (var (_, location) in NarrativeCharts.Bookworm.BookwormLocations.Dictionary)
		{
			Locations[location.Name] = location.Y;
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

	protected void Chapter(string _)
		=> Update();

	protected void Event(string name)
		=> this!.AddEvent(new(new(Time.CurrentTotalHours, 0), name));

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

	protected abstract void ProtectedCreate();

	protected Point Scene(Location location)
		=> new(Time.CurrentTotalHours, location.Y);

	protected void Update()
		=> this.UpdatePoints(Time.CurrentTotalHours);

	protected void UpdateAndAddBell()
	{
		Update();
		Time.AddBell();
	}
}