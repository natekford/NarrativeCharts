using NarrativeCharts.Models;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm.Meta.Locations;

public static class BookwormLocations
{
	[Alias("Ahr_C")]
	public static Location Ahr_Castle { get; } = new("Ahrensbach Castle");
	[Alias("Ahr_Gate")]
	public static Location Ahr_CountryGate { get; } = new("Ahr. Country Gate");
	[Alias("Ahr_LE")]
	public static Location Ahr_LanzEstate { get; } = new("Ahr. Lanzenave Estate");
	[Alias("Ahr_Ships")]
	public static Location Ahr_LanzShips { get; } = new("Ahr. Lanzenave Ships");
	[Alias("Ahr_NQ")]
	public static Location Ahr_NoblesQuarter { get; } = new("Ahr. Noble's Quarter");
	[Alias("Ahr_NP")]
	public static Location Ahr_NorthernProvinces { get; } = new("Ahr. Northern Provinces");
	[Alias("Ahr_T")]
	public static Location Ahr_Temple { get; } = new("Ahr. Temple");
	public static Location Ahrensbach { get; } = new("Ahrensbach");
	public static ImmutableDictionary<string, Location> Aliases { get; }
	public static Location Drewanchel { get; } = new("Drewanchel");
	[Alias("Ditter_Gate")]
	public static Location Dunk_CountryGate { get; } = new("Dunkelfelger Country Gate");
	[Alias("Ditter")]
	public static Location Dunkelfelger { get; } = new("Dunkelfelger");
	[Alias("C")]
	public static Location EhrCastle { get; } = new("Ehrenfest Castle");
	[Alias("EhrFreGate")]
	public static Location EhrFreDuchyGate { get; } = new("Ehr/Fre Duchy Gate");
	[Alias("FE")]
	public static Location FerdinandsHouse { get; } = new("Ferdinand's Estate");
	public static Location Frenbeltag { get; } = new("Frenbeltag");
	[Alias("Gods")]
	public static Location GardenOfBeginnings { get; } = new("Garden of Beginnings");
	public static Location Gilessenmeyer { get; } = new("Gilessenmeyer");
	[Alias("GBath")]
	public static Location GoddessesBath { get; } = new("Goddesses' Bath");
	public static Location Groschel { get; } = new("Groschel");
	public static Location Haldenzel { get; } = new("Haldenzel");
	public static Location Hasse { get; } = new("Hasse");
	public static Location Illgner { get; } = new("Illgner");
	public static Location Immerdink { get; } = new("Immerdink");
	[Alias("IR")]
	public static Location ItalianRestaurant { get; } = new("Italian Restaurant");
	public static Location Jossbrenner { get; } = new("Jossbrenner");
	[Alias("KE")]
	public static Location KarstedtsHouse { get; } = new("Karstedt's Estate");
	public static Location Kirnberger { get; } = new("Kirnberger");
	public static Location Klassenberg { get; } = new("Klassenberg");
	[Alias("KO")]
	public static Location KnightsOrder { get; } = new("Knight's Order");
	public static Location Lanzenave { get; } = new("Lanzenave");
	public static Location Leisegang { get; } = new("Leisegang");
	[Alias("W")]
	public static Location LowerCityWorkshops { get; } = new("Lower City Workshops");
	[Alias("M")]
	public static Location MerchantCompanies { get; } = new("Merchant Companies");
	[Alias("Lohenberg")]
	public static Location MountLohenberg { get; } = new("Mount Lohenberg");
	[Alias("MF")]
	public static Location MynesHouse { get; } = new("Myne's Family's House");
	[Alias("NF")]
	public static Location NoblesForest { get; } = new("Noble's Forest");
	[Alias("NQ")]
	public static Location NoblesQuarter { get; } = new("Noble's Quarter");
	[Alias("RA_AV")]
	public static Location RA_Adalgisa { get; } = new("RA - Adalgisa Villa");
	[Alias("RA_ADC")]
	public static Location RA_ADCClass { get; } = new("RA - ADC Classroom");
	[Alias("RA_A")]
	public static Location RA_Auditorium { get; } = new("RA - Auditorium");
	[Alias("RA_DE")]
	public static Location RA_DormEhr { get; } = new("RA - Ehrenfest Dorm");
	[Alias("RA_DO")]
	public static Location RA_DormOther { get; } = new("RA - Other Duchy Dorms");
	[Alias("RA_DW")]
	public static Location RA_DormWerk { get; } = new("RA - Werkestock Dorm");
	[Alias("RA_FH")]
	public static Location RA_FarthestHall { get; } = new("RA - Farthest Hall");
	[Alias("RA_GE")]
	public static Location RA_GatherEhr { get; } = new("RA - Ehr. Gathering Spot");
	[Alias("RA_G")]
	public static Location RA_Grounds { get; } = new("RA - Grounds");
	[Alias("RA_K")]
	public static Location RA_KnightBuilding { get; } = new("RA - Knight Building");
	[Alias("RA_L")]
	public static Location RA_Library { get; } = new("RA - Library");
	[Alias("RA_RV")]
	public static Location RA_Royals { get; } = new("RA - Royal Villas");
	[Alias("RA_S")]
	public static Location RA_ScholarBuilding { get; } = new("RA - Scholar Building");
	[Alias("RA_SH")]
	public static Location RA_SmallHall { get; } = new("RA - Small Hall");
	[Alias("RA_ST")]
	public static Location RA_Stadium { get; } = new("RA - Stadium");
	[Alias("RA")]
	public static Location RoyalAcademy { get; } = new("Royal Academy");
	[Alias("RTree")]
	public static Location RuelleTree { get; } = new("Ruelle Tree");
	public static Location SmallTowns { get; } = new("Ehrenfest Towns");
	[Alias("SP", "Dahldolf", "Gerlach", "Griebel", "Wiltord")]
	public static Location SouthernProvinces { get; } = new("Southern Provinces");
	public static Location Sovereignty { get; } = new("Sovereignty");
	[Alias("T")]
	public static Location Temple { get; } = new("Temple");
	public static Location WestGate { get; } = new("West Gate");
	public static ImmutableDictionary<Location, int> YIndexes { get; }

	static BookwormLocations()
	{
		// attributes don't work for this as well as they do for character colors
		YIndexes = new[]
		{
			GardenOfBeginnings,
			RA_Royals,
			RA_Adalgisa,
			RA_Grounds,
			RA_Stadium,
			RA_Library,
			RA_FarthestHall,
			RA_ADCClass,
			RA_SmallHall,
			RA_Auditorium,
			RA_KnightBuilding,
			RA_ScholarBuilding,
			RA_DormOther,
			RA_DormEhr,
			RA_DormWerk,
			RA_GatherEhr,
			Sovereignty,
			RoyalAcademy,
			Haldenzel,
			Groschel,
			Kirnberger,
			NoblesForest,
			EhrCastle,
			KnightsOrder,
			FerdinandsHouse,
			KarstedtsHouse,
			NoblesQuarter,
			Temple,
			ItalianRestaurant,
			MerchantCompanies,
			WestGate,
			LowerCityWorkshops,
			MynesHouse,
			Hasse,
			SmallTowns,
			EhrFreDuchyGate,
			RuelleTree,
			GoddessesBath,
			Leisegang,
			Illgner,
			SouthernProvinces,
			MountLohenberg,
			Ahr_NorthernProvinces,
			Ahr_Castle,
			Ahr_NoblesQuarter,
			Ahr_Temple,
			Ahr_LanzEstate,
			Ahr_LanzShips,
			Ahr_CountryGate,
			Ahrensbach,
			Drewanchel,
			Dunk_CountryGate,
			Dunkelfelger,
			Frenbeltag,
			Gilessenmeyer,
			Immerdink,
			Jossbrenner,
			Klassenberg,
			Lanzenave
		}
		.Reverse()
		.Select((x, i) => (Item: x, Index: i))
		.ToImmutableDictionary(x => x.Item, x => x.Index);

		Aliases = typeof(BookwormLocations)
			.GetMembers<Location>()
			.GetAliases(x => x.Value);
	}
}