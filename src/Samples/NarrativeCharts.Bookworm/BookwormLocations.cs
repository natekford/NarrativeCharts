using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Castle { get; } = new("Ehrenfest Castle");
	public static Location Groschel { get; } = new("Groschel");
	public static Location Haldenzel { get; } = new("Haldenzel");
	public static Location Hasse { get; } = new("Hasse");
	public static Location Illgner { get; } = new("Illgner");
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant");
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate");
	public static Location Kirnberger { get; } = new("Kirnberger");
	public static Location KnightsOrder { get; } = new("Knight's Order");
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops");
	public static Location MerchantCompanies { get; } = new("Merchant Companies");
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	public static Location NoblesQuarter { get; } = new("Noble's Quarter");
	public static Location RoyalAcademy { get; } = new("Royal Academy");
	public static Location RuelleTree { get; } = new("Ruelle Tree");
	public static Location SmallTowns { get; } = new("Miscellaneous Towns");
	public static Location Temple { get; } = new("Temple");
	public static ImmutableDictionary<Location, int> YIndexes { get; }

	static BookwormLocations()
	{
		YIndexes = new[]
		{
			RoyalAcademy,
			Haldenzel,
			Kirnberger,
			Groschel,
			Castle,
			KnightsOrder,
			KarstedtsHouse,
			NoblesQuarter,
			Temple,
			ItalianRestaurant,
			MerchantCompanies,
			LowerCityWorkshops,
			MynesHouse,
			Hasse,
			SmallTowns,
			RuelleTree,
			Illgner,
		}
		.Reverse()
		.Select((x, i) => (Item: x, Index: i))
		.ToImmutableDictionary(x => x.Item, x => x.Index);
	}
}