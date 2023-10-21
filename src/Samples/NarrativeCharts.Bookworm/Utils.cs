using NarrativeCharts.Models;
using NarrativeCharts.Plot;

using System.Reflection;
using System.Text;

namespace NarrativeCharts.Bookworm;

public static class Utils
{
	private static readonly Dictionary<string, string> CharacterPropertyNames =
		typeof(BookwormCharacters).MapPropertyNames(
			(Character c) => c.Name);
	private static readonly Dictionary<int, string> LocationPropertyNames =
		typeof(BookwormLocations).MapPropertyNames(
			(Location l) => BookwormLocations.YValues[l.Name]);

	public static void Export(this BookwormNarrativeChart chart, string dir)
	{
		chart.PlotChart(Path.Combine(dir, $"{chart.Name}_chart.png"));
		chart.ExportFinalCharacterPositions(Path.Combine(dir, $"{chart.Name}_final_positions.txt"));

		var points = chart.Points.Sum(x => x.Value.Count);
		Console.WriteLine($"{chart.Name} total points: {points}");
	}

	public static void ExportFinalCharacterPositions(this NarrativeChart chart, string path)
	{
		// should output something like this:
		/*
			Add(Scene(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
			Add(Scene(KnightsOrder).With(Bezewanst, Bindewald));
			Add(Scene(Temple).With(Damuel, Delia, Dirk, Egmont, Ella, Gil, Monika, Nicola, Rosina, Wilma, Zahm));
			Add(Scene(LowerCityWorkshops).With(Deid, Ingo, Johann));
			Add(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
			Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
			Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
			Add(Scene(OthmarCompany).With(Freida, Gustav));
		*/

		var grouped = chart.Points
			.Select(x => x.Value.Values[^1])
			.Where(x => !x.IsEnd)
			.GroupBy(x => x.Point.Y);

		var sb = new StringBuilder();
		foreach (var group in grouped)
		{
			var property = LocationPropertyNames[group.Key];

			sb.Append("Add(Scene(").Append(property).Append(").With(");
			var first = true;
			foreach (var character in group
				.Select(x => CharacterPropertyNames[x.Character])
				.OrderBy(x => x))
			{
				if (!first)
				{
					sb.Append(", ");
				}
				first = false;
				sb.Append(character);
			}
			sb.AppendLine("));");
		}

		File.WriteAllText(path, sb.ToString());
	}

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters.Select(x => x.Name).ToArray());

	private static Dictionary<TKey, string> MapPropertyNames<TKey, TProperty>(
		this Type type,
		Func<TProperty, TKey> keySelector) where TKey : notnull
	{
		return type
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(TProperty))
			.Select(x => (Value: (TProperty)x.GetValue(null)!, Prop: x.Name))
			.ToDictionary(x => keySelector(x.Value), x => x.Prop);
	}
}