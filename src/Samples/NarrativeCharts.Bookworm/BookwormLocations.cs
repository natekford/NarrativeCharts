using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Castle { get; } = new("Ehrenfest Castle", 150);
	public static ImmutableDictionary<string, Location> Dictionary { get; }
	public static Location GilbertaCompany { get; } = new("Gilberta Company", -125);
	public static Location Hasse { get; } = new("Hasse", 250);
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant", -75);
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate", 75);
	public static Location KnightsOrder { get; } = new("Knight's Order", 125);
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops", -175);
	public static Location MynesHouse { get; } = new("Myne's Family's House", -200);
	public static Location OthmarCompany { get; } = new("Othmar Company", -100);
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