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
				if (points.Values[i].Point.Location == Frozen)
				{
					points.RemoveAt(i);
				}
			}
		}
		this.Simplify();
	}

	protected void Add(NarrativeScene scene)
		=> this.AddScene(scene);

	protected void Event(string name)
	{
		this.AddEvent(new(new(X, new()), name));
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
			points[point.Point.Hour] = point with
			{
				IsEnd = true,
			};
		}
	}

	protected abstract void ProtectedCreate();

	protected Point Scene(Location location)
		=> new(X, location);

	protected void Update()
		=> this.UpdatePoints(X);
}