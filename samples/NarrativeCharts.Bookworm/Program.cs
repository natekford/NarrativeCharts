using NarrativeCharts.Drawing;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;
using NarrativeCharts.Time;

using SkiaSharp;

using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	internal static ScriptDefinitions CreateScriptDefinitions(string dir)
	{
		static void AddAliases<TKey, TValue>(
			Dictionary<TKey, TValue> dest,
			Dictionary<TValue, TKey[]> aliases)
			where TKey : notnull
			where TValue : notnull
		{
			foreach (var (key, value) in aliases)
			{
				foreach (var alias in value)
				{
					dest.Add(alias, key);
				}
			}
		}

		var defs = new ScriptDefinitions
		{
			ConvertScripts = true,
			RedrawUneditedScripts = true,
			ScriptDirectory = Path.Combine(dir, "Scripts"),
			OnlyDrawTheseCharacters =
			[
			//BookwormCharacters.Ella,
			//BookwormCharacters.Hugo,
			//BookwormCharacters.Rosina,
			//BookwormCharacters.Myne
			],
		};

		// Characters
		{
			foreach (var (key, value) in BookwormCharacters.Colors)
			{
				defs.CharacterColors.Add(key, value);
				defs.CharacterAliases.Add(key.Value, key);
			}

			AddAliases(defs.CharacterAliases, new()
			{
				[AubDunkelfelger] = ["Werdekraf", "AubDitter"],
				[AubFrenbeltag] = ["AubFre"],
				[AubKlassenberg] = ["AubKla"],
				[Anastasius] = ["Ana"],
				[Eglantine] = ["Eggy"],
				[GiebeDahldolf] = [nameof(GiebeDahldolf)],
				[GiebeGroschel] = [nameof(GiebeGroschel)],
				[GiebeJoisontak] = [nameof(GiebeJoisontak)],
				[GiebeKirnberger] = [nameof(GiebeKirnberger)],
				[GiebeLeisegang] = [nameof(GiebeLeisegang)],
				[GiebeLeisegangSr] = ["OldLeisegang"],
				[HasseMayor] = [nameof(HasseMayor)],
				[Sigiswald] = ["Sigi"],
				[Trauerqual] = ["AnaFather"],
			});
		}

		// Locations
		{
			foreach (var (key, value) in BookwormLocations.YIndexes)
			{
				defs.LocationYIndexes.Add(key, value);
				defs.LocationAliases.Add(key.Value, key);
			}

			AddAliases(defs.LocationAliases, new()
			{
				[GardenOfBeginnings] = ["Gods"],
				[RA_Royals] = ["RA_RV"],
				[RA_Adalgisa] = ["RA_AV"],
				[RA_Stadium] = ["RA_ST"],
				[RA_Grounds] = ["RA_G"],
				[RA_Library] = ["RA_L"],
				[RA_FarthestHall] = ["RA_FH"],
				[RA_ADCClass] = ["RA_ADC"],
				[RA_SmallHall] = ["RA_SH"],
				[RA_Auditorium] = ["RA_A"],
				[RA_KnightBuilding] = ["RA_K"],
				[RA_ScholarBuilding] = ["RA_S"],
				[RA_DormOther] = ["RA_DO"],
				[RA_DormEhr] = ["RA_DE"],
				[RA_DormWerk] = ["RA_DW"],
				[RA_GatherEhr] = ["RA_GE"],
				[RoyalAcademy] = ["RA"],
				[NoblesForest] = ["NF"],
				[EhrCastle] = ["C"],
				[KnightsOrder] = ["KO"],
				[FerdinandsHouse] = ["FE"],
				[KarstedtsHouse] = ["KE"],
				[NoblesQuarter] = ["NQ"],
				[Temple] = ["T"],
				[ItalianRestaurant] = ["IR"],
				[MerchantCompanies] = ["M"],
				[WestGate] = ["WestGate"],
				[LowerCityWorkshops] = ["W"],
				[MynesHouse] = ["MF"],
				[SmallTowns] = ["SmallTowns"],
				[EhrFreDuchyGate] = ["EhrFreGate"],
				[RuelleTree] = ["RTree"],
				[GoddessesBath] = ["GBath"],
				[SouthernProvinces] = ["SP"],
				[MountLohenberg] = ["Lohenberg"],
				[Ahr_NorthernProvinces] = ["Ahr_NP"],
				[Ahr_Castle] = ["Ahr_C"],
				[Ahr_NoblesQuarter] = ["Ahr_NQ"],
				[Ahr_Temple] = ["Ahr_T"],
				[Ahr_LanzEstate] = ["Ahr_LE"],
				[Ahr_LanzShips] = ["Ahr_Ships"],
				[Ahr_CountryGate] = ["Ahr_Gate"],
				[Dunk_CountryGate] = ["Ditter_Gate"],
				[Dunkelfelger] = ["Ditter"],
			});
		}

		// Time
		{
			defs.Time = new TimeTrackerWithUnits(BookwormTime.Lengths);
			foreach (var (key, _) in defs.Time.UnitToHourMap)
			{
				defs.TimeUnitAliases.Add(key.ToString(), key);
			}

			AddAliases(defs.TimeUnitAliases, new()
			{
				[0] = [nameof(Midnight)],
				[1] = [nameof(FirstBell), nameof(EarlyMorning)],
				[2] = [nameof(SecondBell), nameof(Morning)],
				[3] = [nameof(ThirdBell), nameof(Meetings)],
				[4] = [nameof(FourthBell), nameof(Lunch)],
				[5] = [nameof(FifthBell), nameof(MarketClose), nameof(Tea)],
				[6] = [nameof(SixthBell), nameof(Dinner)],
				[7] = [nameof(SeventhBell), nameof(Bed)],
			});
		}

		return defs;
	}

	private static async Task<ScriptDefinitions> GetScriptDefinitionsAsync(string dir)
	{
		var defs = CreateScriptDefinitions(dir);
		var defsPath = Path.Combine(defs.ScriptDirectory, "ScriptDefinitions.json");
		await defs.SaveAsync(defsPath).ConfigureAwait(false);
		return await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
	}

	private static async Task Main()
	{
		var dir = Directory.GetCurrentDirectory();
		var defs = await GetScriptDefinitionsAsync(dir).ConfigureAwait(false);
		var charts = defs.LoadScripts().ToList<NarrativeChart>();
		var drawer = new SKChartDrawer()
		{
			ImageAspectRatio = 16f / 9f,
			// smaller images in debug so they render faster
#if DEBUG
			ImageSizeMult = 3f,
#endif
			IgnoreNonMovingCharacters = false,
			CharacterLabelColorConverter = SKColorConverters.Color(SKColors.Black),
		}.UseRecommendedYSpacing();

#if false
		var combined = charts.Combine();
		combined.Name = "Combined";
		charts.Add(combined);
#endif

		var collection = new ScriptCollection
		{
			Charts = charts,
			Defs = defs,
			Drawer = drawer,
		};

		await collection.ProcessAsync().ConfigureAwait(false);
		await Task.Delay(-1).ConfigureAwait(false);
	}
}