using NarrativeCharts.Models;

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
	public static ImmutableDictionary<Location, Y> YValues { get; }

	static BookwormLocations()
	{
		YValues = new Dictionary<Location, int>()
		{
			[Hasse] = 300,
			[Castle] = 200,
			[KnightsOrder] = 150,
			[KarstedtsHouse] = 100,
			[Temple] = 0,
			[ItalianRestaurant] = -100,
			[OthmarCompany] = -125,
			[GilbertaCompany] = -150,
			[LowerCityWorkshops] = -200,
			[MynesHouse] = -225,
		}.ToImmutableDictionary(x => x.Key, x => new Y(x.Value));
	}
}