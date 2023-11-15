using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChart : NarrativeChartData
{
	protected float Hour => Time.CurrentTotalHours;
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
		Simplify();
	}

	public virtual void Simplify()
	{
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

		foreach (var (_, points) in Points)
		{
			// Don't bother checking the first or last points
			// They will always be valid
			for (var i = points.Count - 2; i > 0; --i)
			{
				var prev = points.Values[i - 1];
				var curr = points.Values[i];
				var next = points.Values[i + 1];

				// keep a point at the start and end of each timeskip
				if (prev.IsTimeSkip || curr.IsTimeSkip)
				{
					continue;
				}

				var pLoc = prev.Location;
				var cLoc = curr.Location;
				var nLoc = next.Location;
				if (pLoc == cLoc && cLoc == nLoc)
				{
					points.Remove(points.Values[i].Hour);
				}
			}
		}
	}

	protected void Add(Location location, params Character[] characters)
		=> Add(location, (IEnumerable<Character>)characters);

	protected virtual void Add(Location location, IEnumerable<Character> characters)
		=> this.AddScene(new(Hour, location, characters));

	protected virtual void AddHours(float amount = 1)
	{
		Time.AddHours(amount);
		Update();
	}

	protected Dictionary<Character, Location> AddR(
		Location location,
		params Character[] characters)
		=> AddR(location, (IEnumerable<Character>)characters);

	protected virtual Dictionary<Character, Location> AddR(
		Location location,
		IEnumerable<Character> characters)
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

	protected void Freeze(params Character[] characters)
		=> Freeze((IEnumerable<Character>)characters);

	protected virtual void Freeze(IEnumerable<Character> characters)
		=> Add(Location.Frozen, characters);

	protected void Kill(params Character[] characters)
		=> Kill((IEnumerable<Character>)characters);

	protected virtual void Kill(IEnumerable<Character> characters)
	{
		Update();
		foreach (var character in characters)
		{
			Points[character].ModifyLastPoint(x => x with { IsEnd = true });
		}
	}

	protected abstract void ProtectedCreate();

	protected virtual void Return(IEnumerable<KeyValuePair<Character, Location>> points)
	{
		foreach (var (character, location) in points)
		{
			this.AddPoint(new(
				Hour: Hour,
				Location: location,
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
		Time.AddDays(days);
		Update();
	}

	protected virtual void Update()
		=> this.UpdatePoints(Hour);
}