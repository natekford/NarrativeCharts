using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Castle { get; } = new("Ehrenfest Castle");
	public static Location GilbertaCompany { get; } = new("Gilberta Company");
	public static Location Hasse { get; } = new("Hasse");
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant");
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate");
	public static Location KnightsOrder { get; } = new("Knight's Order");
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops");
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	public static Location OthmarCompany { get; } = new("Othmar Company");
	public static Location Temple { get; } = new("Temple");
	public static ImmutableDictionary<string, int> YValues { get; }

	static BookwormLocations()
	{
		YValues = new Dictionary<string, int>()
		{
			[Hasse.Name] = 300,
			[Castle.Name] = 200,
			[KnightsOrder.Name] = 150,
			[KarstedtsHouse.Name] = 100,
			[Temple.Name] = 0,
			[ItalianRestaurant.Name] = -100,
			[OthmarCompany.Name] = -125,
			[GilbertaCompany.Name] = -150,
			[LowerCityWorkshops.Name] = -200,
			[MynesHouse.Name] = -225,
		}.ToImmutableDictionary();
	}
}

public readonly record struct Location(string Name);