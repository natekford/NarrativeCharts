using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts;

public abstract class NarrativeChart : NarrativeChartData
{
	protected bool AlreadyCreated { get; set; }
	protected Location Frozen { get; } = new Location(nameof(Frozen));
	protected TimeTracker Time { get; }
	protected int X => Time.CurrentTotalHours;

	protected NarrativeChart(TimeTracker time)
	{
		Time = time;
	}

	public void Initialize(NarrativeChartData? seed)
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

		foreach (var points in Points.Values)
		{
			for (var i = points.Count - 1; i >= 0; --i)
			{
				if (points.Values[i].Location == Frozen)
				{
					points.RemoveAt(i);
				}
			}
		}
		this.Simplify();
	}

	protected void Add(Location location, params Character[] characters)
		=> this.AddScene(new(X, location, characters));

	protected Dictionary<Character, Location> AddR(Location location, params Character[] characters)
	{
		var dict = characters.ToDictionary(x => x, x => Points[x].Values[^1].Location);
		Add(location, characters);
		return dict;
	}

	protected void Event(string name)
	{
		this.AddEvent(new(X, name));
		Update();
	}

	protected void Freeze(params Character[] characters)
		=> Add(Frozen, characters);

	protected void Kill(params Character[] characters)
	{
		Update();
		foreach (var character in characters)
		{
			var lastPoint = Points[character].Values[^1];
			Points[character][lastPoint.Hour] = lastPoint with
			{
				IsEnd = true,
			};
		}
	}

	protected abstract void ProtectedCreate();

	protected void Return(Dictionary<Character, Location> scene)
	{
		foreach (var (character, y) in scene)
		{
			this.AddPoint(new(
				Hour: X,
				Location: y,
				Character: character,
				IsEnd: false
			));
		}
	}

	protected void Update()
		=> this.UpdatePoints(X);
}