using NarrativeCharts.Models;

namespace NarrativeCharts.Console;

public abstract class BookwormNarrativeChart : NarrativeChart
{
	protected bool AlreadyCreaated { get; set; }
	protected BookwormTimeTracker Time { get; }

	protected BookwormNarrativeChart(BookwormTimeTracker time)
	{
		Time = time;
		foreach (var (_, character) in Characters.CharacterDictionary)
		{
			Colors[character.Name] = character.Color;
		}
		foreach (var (_, location) in NarrativeCharts.Console.Locations.LocationDictionary)
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

	protected void Chapter(string _)
		=> this.UpdatePoints(Time.CurrentTotalHours);

	protected void Event(string name)
		=> this!.AddEvent(new(new(Time.CurrentTotalHours, 0), name));

	protected abstract void ProtectedCreate();

	protected Point Scene(Location location)
		=> new(Time.CurrentTotalHours, location.Y);
}