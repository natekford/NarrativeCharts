using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Console;

public static class Locations
{
	public static Location Castle { get; } = new("Ehrenfest Castle", 200);
	public static Location GilbertaCompany { get; } = new("Gilberta Company", -100);
	public static Location Hasse { get; } = new("Hasse", 225);
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant", -25);
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate", 150);
	public static Location KnightsOrder { get; } = new("Knight's Order", 175);
	public static ImmutableDictionary<string, Location> LocationDictionary { get; }
	public static Location LowerCityForest { get; } = new("Lower City Forest", -50);
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops", -225);
	public static Location MynesHouse { get; } = new("Myne's Family's House", -250);
	public static Location OthmarCompany { get; } = new("Othmar Company", -150);
	public static Location RoyalAcademy { get; } = new("Royal Academy", 250);
	public static Location Temple { get; } = new("Temple", 0);

	static Locations()
	{
		LocationDictionary = typeof(Locations)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Location))
			.Select(x => (Location)x.GetValue(null)!)
			.ToImmutableDictionary(x => x.Name, x => x);
	}
}

public readonly record struct Location(string Name, int Y);