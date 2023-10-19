using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Console;

public static class Locations
{
	public static Location Castle { get; } = new("Ehrenfest Castle", 150);
	public static Location GilbertaCompany { get; } = new("Gilberta Company", -125);
	public static Location Hasse { get; } = new("Hasse", 250);
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant", -75);
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate", 75);
	public static Location KnightsOrder { get; } = new("Knight's Order", 125);
	public static ImmutableDictionary<string, Location> LocationDictionary { get; }
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops", -175);
	public static Location MynesHouse { get; } = new("Myne's Family's House", -200);
	public static Location OthmarCompany { get; } = new("Othmar Company", -100);
	public static ImmutableDictionary<int, string> PropertyDictionary { get; }
	public static Location Temple { get; } = new("Temple", 0);

	static Locations()
	{
		var temp = typeof(Locations)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Location))
			.Select(x => (Value: (Location)x.GetValue(null)!, Prop: x.Name));

		LocationDictionary = temp.ToImmutableDictionary(x => x.Value.Name, x => x.Value);
		PropertyDictionary = temp.ToImmutableDictionary(x => x.Value.Y, x => x.Prop);
	}
}

public readonly record struct Location(string Name, int Y);