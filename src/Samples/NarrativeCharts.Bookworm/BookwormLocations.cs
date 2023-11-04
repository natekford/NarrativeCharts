using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location AhrensbachCastle { get; } = new("Ahrensbach Castle");
	public static Location Dahldolf { get; } = new("Dahldolf");
	public static Location EhrenfestCastle { get; } = new("Ehrenfest Castle");
	public static Location FerdinandsHouse { get; } = new("Ferdinand's Estate");
	public static Location Gerlach { get; } = new("Gerlach");
	public static Location GoddessesBath { get; } = new("Goddesses' Bath");
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
	public static Location MountLohenberg { get; } = new("Mount Lohenberg");
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	public static Location NoblesForest { get; } = new("Noble's Forest");
	public static Location NoblesQuarter { get; } = new("Noble's Quarter");
	public static Location RoyalAcademy { get; } = new("Royal Academy");
	public static Location RuelleTree { get; } = new("Ruelle Tree");
	public static Location SmallTowns { get; } = new("Ehrenfest Towns");
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
			NoblesForest,
			EhrenfestCastle,
			KnightsOrder,
			FerdinandsHouse,
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
			GoddessesBath,
			Illgner,
			Dahldolf,
			Gerlach,
			MountLohenberg,
			AhrensbachCastle,
		}
		.Reverse()
		.Select((x, i) => (Item: x, Index: i))
		.ToImmutableDictionary(x => x.Item, x => x.Index);
	}
}