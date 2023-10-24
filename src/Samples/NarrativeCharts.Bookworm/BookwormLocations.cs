using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Castle { get; } = new("Ehrenfest Castle");
	public static Location HarvestFestivalTowns { get; } = new("Harvest Festival Towns");
	public static Location Hasse { get; } = new("Hasse");
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant");
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate");
	public static Location KnightsOrder { get; } = new("Knight's Order");
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops");
	public static Location MerchantCompanies { get; } = new("Merchant Companies");
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	public static Location NoblesQuarter { get; } = new("Noble's Quarter");
	public static Location Temple { get; } = new("Temple");
	public static ImmutableDictionary<Location, Y> YValues { get; }

	static BookwormLocations()
	{
		YValues = new Dictionary<Location, int>()
		{
			[HarvestFestivalTowns] = 325,
			[Hasse] = 300,
			[Castle] = 200,
			[KnightsOrder] = 150,
			[KarstedtsHouse] = 100,
			[NoblesQuarter] = 50,
			[Temple] = 0,
			[ItalianRestaurant] = -100,
			[MerchantCompanies] = -150,
			[LowerCityWorkshops] = -200,
			[MynesHouse] = -225,
		}.ToImmutableDictionary(x => x.Key, x => new Y(x.Value));
	}
}