using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Castle { get; } = new("Ehrenfest Castle", 175);
	public static ImmutableDictionary<string, Location> Dictionary { get; }
	public static Location GilbertaCompany { get; } = new("Gilberta Company", -150);
	public static Location Hasse { get; } = new("Hasse", 275);
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant", -100);
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate", 100);
	public static Location KnightsOrder { get; } = new("Knight's Order", 150);
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops", -200);
	public static Location MynesHouse { get; } = new("Myne's Family's House", -225);
	public static Location OthmarCompany { get; } = new("Othmar Company", -125);
	public static Location Temple { get; } = new("Temple", 0);

	static BookwormLocations()
	{
		Dictionary = typeof(BookwormLocations)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Location))
			.Select(x => (Location)x.GetValue(null)!)
			.ToImmutableDictionary(x => x.Name, x => x);
	}
}

public readonly record struct Location(string Name, int Y);