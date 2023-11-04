using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChart : NarrativeChartData
{
	protected int Hour => Time.CurrentTotalHours;
	protected bool IsInitialized { get; set; }
	protected TimeTracker Time { get; }

	protected NarrativeChart(TimeTracker time)
	{
		Time = time;
	}

	public virtual void Initialize(NarrativeChartData? seed)
	{
		if (IsInitialized)
		{
			return;
		}
		IsInitialized = true;

		if (seed is not null)
		{
			this.Seed(seed, Hour);
		}

		ProtectedCreate();

		foreach (var points in Points.Values)
		{
			for (var i = points.Count - 1; i >= 0; --i)
			{
				if (points.Values[i].Location == Location.Frozen)
				{
					points.RemoveAt(i);
				}
			}
		}
		this.Simplify();
	}

	protected virtual void Add(Location location, params Character[] characters)
		=> this.AddScene(new(Hour, location, characters));

	protected virtual Dictionary<Character, Location> AddR(Location location, params Character[] characters)
	{
		var locations = this.GetCurrentLocations(characters);
		Add(location, characters);
		return locations;
	}

	protected virtual void Event(string name)
	{
		this.AddEvent(new(Hour, name));
		Update();
	}

	protected virtual void Freeze(params Character[] characters)
		=> Add(Location.Frozen, characters);

	protected virtual void Kill(params Character[] characters)
	{
		Update();
		foreach (var character in characters)
		{
			Points[character].ModifyLastPoint(x => x with { IsEnd = true });
		}
	}

	protected abstract void ProtectedCreate();

	protected virtual void Return(Dictionary<Character, Location> scene)
	{
		foreach (var (character, y) in scene)
		{
			this.AddPoint(new(
				Hour: Hour,
				Location: y,
				Character: character,
				IsEnd: false,
				IsTimeSkip: false
			));
		}
	}

	protected virtual void TimeSkip(int days)
	{
		Update();
		foreach (var points in Points.Values)
		{
			points.ModifyLastPoint(x => x with { IsTimeSkip = true });
		}
		Time.SkipToNextDayStart();
		Time.AddDays(days - 1);
		Update();
	}

	protected virtual void Update()
		=> this.UpdatePoints(Hour);
}