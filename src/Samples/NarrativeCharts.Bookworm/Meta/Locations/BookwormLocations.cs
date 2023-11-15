using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

public static class BookwormLocations
{
	public static Location Ahrensbach { get; } = new("Ahrensbach");
	public static Location Drewanchel { get; } = new("Drewanchel");
	public static Location Dunkelfelger { get; } = new("Dunkelfelger");
	public static Location EhrenfestCastle { get; } = new("Ehrenfest Castle");
	public static Location FerdinandsHouse { get; } = new("Ferdinand's Estate");
	public static Location Frenbeltag { get; } = new("Frenbeltag");
	public static Location Gilessenmeyer { get; } = new("Gilessenmeyer");
	public static Location GoddessesBath { get; } = new("Goddesses' Bath");
	public static Location Groschel { get; } = new("Groschel");
	public static Location Haldenzel { get; } = new("Haldenzel");
	public static Location Hasse { get; } = new("Hasse");
	public static Location Illgner { get; } = new("Illgner");
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant");
	public static Location Jossbrenner { get; } = new("Jossbrenner");
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate");
	public static Location Kirnberger { get; } = new("Kirnberger");
	public static Location Klassenberg { get; } = new("Klassenberg");
	public static Location KnightsOrder { get; } = new("Knight's Order");
	public static Location Leisegang { get; } = new("Leisegang");
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops");
	public static Location MerchantCompanies { get; } = new("Merchant Companies");
	public static Location MountLohenberg { get; } = new("Mount Lohenberg");
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	public static Location NoblesForest { get; } = new("Noble's Forest");
	public static Location NoblesQuarter { get; } = new("Noble's Quarter");
	public static Location RA_ADCClass { get; } = new("RA - ADC Classroom");
	public static Location RA_Auditorium { get; } = new("RA - Auditorium");
	public static Location RA_AuditoriumShrine { get; } = new("RA - Auditorium Shrine");
	public static Location RA_DormEhr { get; } = new("RA - Ehrenfest Dorm");
	public static Location RA_DormOther { get; } = new("RA - Other Duchy Dorms");
	public static Location RA_DormSov { get; } = new("RA - Scholar Building");
	public static Location RA_DormWerk { get; } = new("RA - Werkestock Dorm");
	public static Location RA_FarthestHall { get; } = new("RA - Farthest Hall");
	public static Location RA_GatherEhr { get; } = new("RA - Ehr. Gathering Spot");
	public static Location RA_Library { get; } = new("RA - Library");
	public static Location RA_Royals { get; } = new("RA - Royal Villas");
	public static Location RA_SmallHall { get; } = new("RA - Small Hall");
	public static Location RA_Stadium { get; } = new("RA - Stadium");
	public static Location RoyalAcademy { get; } = new("Royal Academy");
	public static Location RuelleTree { get; } = new("Ruelle Tree");
	public static Location SmallTowns { get; } = new("Ehrenfest Towns");
	// dahldolf, gerlach, wiltord, griebel
	public static Location SouthernProvinces { get; } = new("Southern Provinces");
	public static Location Sovereignty { get; } = new("Sovereignty");
	public static Location Temple { get; } = new("Temple");
	public static ImmutableDictionary<Location, int> YIndexes { get; }

	static BookwormLocations()
	{
		// attributes don't work for this as well as they do for character colors
		YIndexes = new[]
		{
			RA_Royals,
			RA_FarthestHall,
			RA_DormSov,
			RA_Stadium,
			RA_Library,
			RA_ADCClass,
			RA_SmallHall,
			RA_AuditoriumShrine,
			RA_Auditorium,
			RA_DormOther,
			RA_DormEhr,
			RA_DormWerk,
			RA_GatherEhr,
			Sovereignty,
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
			Leisegang,
			Illgner,
			SouthernProvinces,
			MountLohenberg,
			Klassenberg,
			Dunkelfelger,
			Drewanchel,
			Gilessenmeyer,
			Ahrensbach,
			Jossbrenner,
			Frenbeltag,
		}
		.Reverse()
		.Select((x, i) => (Item: x, Index: i))
		.ToImmutableDictionary(x => x.Item, x => x.Index);
	}
}