using NarrativeCharts.Models;

namespace NarrativeCharts.Bookworm;

public abstract class BookwormNarrativeChart : NarrativeChartUnits<BookwormBell>
{
	protected BookwormNarrativeChart(BookwormTimeTracker time) : base(time)
	{
		foreach (var (key, value) in BookwormCharacters.Colors)
		{
			Colors.Add(key, value);
		}
		foreach (var (key, value) in BookwormLocations.YIndexes)
		{
			YIndexes.Add(key, value);
		}
	}

	protected Dictionary<Character, Location> AddR(NarrativeScene scene)
	{
		var dict = scene.Characters
			.ToDictionary(x => x, x => Points[x].Values[^1].Point.Location);
		Add(scene);
		return dict;
	}

	protected override int ConvertToInt(BookwormBell unit)
		=> (int)unit;

	protected void Return(IEnumerable<KeyValuePair<Character, Location>> scene)
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
}