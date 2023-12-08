using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts;

/// <summary>
/// Base class for making charts.
/// </summary>
public class NarrativeChart : NarrativeChartData
{
	/// <summary>
	/// Whether or not this chart has been created.
	/// </summary>
	public bool HasInitializeBeenCalled { get; protected set; }
	/// <summary>
	/// The current hour.
	/// </summary>
	public float Hour => Time.CurrentTotalHours;
	/// <summary>
	/// The time of this chart.
	/// </summary>
	protected TimeTracker Time { get; }

	/// <summary>
	/// Creates a <see cref="NarrativeChart"/>.
	/// </summary>
	/// <param name="time"></param>
	public NarrativeChart(TimeTracker time)
	{
		Time = time;
	}

	/// <inheritdoc cref="AddReturnable(Location, IEnumerable{Character})" />
	public void Add(Location location, params Character[] characters)
		=> Add(location, (IEnumerable<Character>)characters);

	/// <summary>
	/// Adds new points at <paramref name="location"/> for every character
	/// in <paramref name="characters"/>.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="characters"></param>
	public virtual void Add(Location location, IEnumerable<Character> characters)
		=> this.AddScene(new(Hour, location, characters));

	/// <summary>
	/// Adds <paramref name="amount"/> to <see cref="Time"/>.
	/// </summary>
	/// <param name="amount"></param>
	public virtual void AddHours(float amount = 1)
	{
		Update();
		Time.AddHours(amount);
		Update();
	}

	/// <inheritdoc cref="AddReturnable(Location, IEnumerable{Character})" />
	public Dictionary<Character, Location> AddReturnable(
		Location location,
		params Character[] characters)
		=> AddReturnable(location, (IEnumerable<Character>)characters);

	/// <summary>
	/// Adds new points at <paramref name="location"/> for every character
	/// in <paramref name="characters"/>, and then returns their previous locations.
	/// </summary>
	/// <param name="location"></param>
	/// <param name="characters"></param>
	/// <returns></returns>
	public virtual Dictionary<Character, Location> AddReturnable(
		Location location,
		IEnumerable<Character> characters)
	{
		var locations = this.GetCurrentLocations(characters);
		Add(location, characters);
		return locations;
	}

	/// <summary>
	/// Add a new <see cref="NarrativeEvent"/>.
	/// </summary>
	/// <param name="name"></param>
	public virtual void Event(string name)
	{
		this.AddEvent(new(Hour, name));
		Update();
	}

	/// <inheritdoc cref="Freeze(IEnumerable{Character})" />
	public void Freeze(params Character[] characters)
		=> Freeze((IEnumerable<Character>)characters);

	/// <summary>
	/// Adds new points at <see cref="Location.Frozen"/> for every character
	/// in <paramref name="characters"/>.
	/// </summary>
	/// <param name="characters"></param>
	public virtual void Freeze(IEnumerable<Character> characters)
		=> Add(Location.Frozen, characters);

	/// <summary>
	/// Seed this chart with <paramref name="seed"/>, handle any internal narrative
	/// chart creation, then simplify the resulting data.
	/// </summary>
	/// <param name="seed"></param>
	public virtual void Initialize(NarrativeChartData? seed)
	{
		if (HasInitializeBeenCalled)
		{
			return;
		}
		HasInitializeBeenCalled = true;

		if (seed is not null)
		{
			this.Seed(seed, Hour);
		}

		AddNarrativeData();
		Simplify();
	}

	/// <inheritdoc cref="Kill(IEnumerable{Character})" />
	public void Kill(params Character[] characters)
		=> Kill((IEnumerable<Character>)characters);

	/// <summary>
	/// Mark each character's last point's <see cref="NarrativePoint.IsEnd"/> as true.
	/// </summary>
	/// <param name="characters"></param>
	public virtual void Kill(IEnumerable<Character> characters)
	{
		Update();
		foreach (var character in characters)
		{
			Points[character].ModifyLastPoint(x => x with { IsEnd = true });
		}
	}

	/// <summary>
	/// Return characters to their original positions after
	/// invoking <see cref="AddReturnable(Location, IEnumerable{Character})"/>.
	/// </summary>
	/// <param name="points"></param>
	public virtual void Return(
		IEnumerable<KeyValuePair<Character, Location>> points)
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

	/// <summary>
	/// Remove unnecessary points and frozen points.
	/// This will leave in the empty list if all points are removed.
	/// </summary>
	public virtual void Simplify()
	{
		foreach (var (_, points) in Points)
		{
			for (var i = points.Count - 1; i >= 0; --i)
			{
				if (points.Values[i].Location == Location.Frozen)
				{
					points.RemoveAt(i);
				}
			}

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

	/// <summary>
	/// Mark each character's last point's <see cref="NarrativePoint.IsTimeSkip"/>
	/// as true and then add <paramref name="days"/> to <see cref="Time"/>.
	/// </summary>
	/// <param name="days"></param>
	public virtual void TimeSkip(int days)
	{
		Update();
		foreach (var points in Points.Values)
		{
			points.ModifyLastPoint(x => x with { IsTimeSkip = true });
		}
		Time.AddDays(days);
		Update();
	}

	/// <summary>
	/// Adds a point for every character at the current hour in their previous location.
	/// </summary>
	public virtual void Update()
		=> this.UpdatePoints(Hour);

	/// <summary>
	/// Adds data to the chart directly after seeding has been done.
	/// This is intended to be called once.
	/// </summary>
	protected virtual void AddNarrativeData()
	{
	}
}