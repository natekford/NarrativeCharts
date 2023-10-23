using NarrativeCharts.Models;

using System.Reflection;
using System.Text;

namespace NarrativeCharts.Bookworm;

public static class Utils
{
	private static readonly Dictionary<Character, string> CharacterPropertyNames =
		typeof(BookwormCharacters).MapPropertyNames((Character c) => c);
	private static readonly Dictionary<Y, string> LocationPropertyNames =
		typeof(BookwormLocations).MapPropertyNames((Location l) => BookwormLocations.YValues[l]);

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

		var sb = new StringBuilder();
		var grouped = chart.Points
			.Select(x => x.Value.Values[^1])
			.Where(x => !x.IsEnd)
			.GroupBy(x => x.Point.Y);
		foreach (var group in grouped)
		{
			var property = LocationPropertyNames[group.Key];
			var characters = group
				.Select(x => CharacterPropertyNames[x.Character])
				.OrderBy(x => x);

			sb.Append("Add(Scene(")
				.Append(property)
				.Append(").With(")
				.AppendJoin(", ", characters)
				.AppendLine("));");
		}

		File.WriteAllText(path, sb.ToString());
	}

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters);

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