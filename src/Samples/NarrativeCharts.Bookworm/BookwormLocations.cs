﻿using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location AhrensbachCastle { get; } = new("Ahrensbach Castle");
	public static Location EhrenfestCastle { get; } = new("Ehrenfest Castle");
	public static Location FerdinandsHouse { get; } = new("Ferdinand's Estate");
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
	public static Location RA_Auditorium { get; } = new("RA - Auditorium");
	public static Location RA_Classroom { get; } = new("RA - Classroom");
	public static Location RA_DormEhrenfest { get; } = new("RA - Ehrenfest Dorm");
	public static Location RA_DormOther { get; } = new("RA - Other Duchy Dorms");
	public static Location RA_FarthestHall { get; } = new("RA - Farthest Hall");
	public static Location RA_Library { get; } = new("RA - Library");
	public static Location RA_Royals { get; } = new("RA - Royal Villas");
	public static Location RA_Scholars { get; } = new("RA - Scholar Building");
	public static Location RoyalAcademy { get; } = new("Royal Academy");
	public static Location RuelleTree { get; } = new("Ruelle Tree");
	public static Location SmallTowns { get; } = new("Ehrenfest Towns");
	// dahldolf, gerlach, wiltord, griebel
	public static Location SouthernProvinces { get; } = new("Southern Provinces");
	public static Location Temple { get; } = new("Temple");
	public static ImmutableDictionary<Location, int> YIndexes { get; }

	static BookwormLocations()
	{
		YIndexes = new[]
		{
			RA_Royals,
			RA_FarthestHall,
			RA_Scholars,
			RA_Library,
			RA_Classroom,
			RA_Auditorium,
			RA_DormOther,
			RA_DormEhrenfest,
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
			SouthernProvinces,
			MountLohenberg,
			AhrensbachCastle,
		}
		.Reverse()
		.Select((x, i) => (Item: x, Index: i))
		.ToImmutableDictionary(x => x.Item, x => x.Index);
	}
}