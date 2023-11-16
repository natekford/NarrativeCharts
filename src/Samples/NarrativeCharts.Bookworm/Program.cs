using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;
using NarrativeCharts.Time;

using SkiaSharp;

using System.Diagnostics;

using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm;

public class Program
{
	public List<NarrativeChartData> Books { get; } = [];
	public string ChartsDir { get; }
	public ScriptDefinitions Defs { get; private set; } = null!;
	public string Dir { get; }
	public SKChartDrawer Drawer { get; } = new()
	{
		ImageAspectRatio = 32f / 9f,
		// smaller images in debug so they render faster
#if DEBUG
		ImageSizeMult = 3f,
#endif
		IgnoreNonMovingCharacters = false,
		CharacterLabelColorConverter = SKColorConverters.Color(SKColors.Black),
	};
	public string ScriptsDir { get; }

	public Program(string dir)
	{
		Dir = dir;
		ChartsDir = Path.Combine(Dir, "Charts");
		ScriptsDir = Path.Combine(Dir, "Scripts");
	}

	public async Task RunAsync()
	{
		Defs = await GetScriptDefinitionsAsync().ConfigureAwait(false);

		var books = GetBooks();
		for (var i = 0; i < books.Count; ++i)
		{
			books[i].Initialize(i == 0 ? null : books[i - 1]);
			Books.Add(books[i]);
		}

#if true
		foreach (var book in books)
		{
			if (book is not BookwormScriptConverter scriptConverter)
			{
				continue;
			}

			var path = Path.Combine(ScriptsDir, $"{scriptConverter.ClassName}.cs");
			File.WriteAllText(path, scriptConverter.Write());
		}
#endif

#if false
		var combined = books.Combine();
		combined.Name = "Combined";
		Books.Add(combined);
#endif

#if false
		var kept = new HashSet<Models.Character>
		{
			BookwormCharacters.Ella,
			BookwormCharacters.Hugo,
			BookwormCharacters.Rosina,
			BookwormCharacters.Myne
		};
		foreach (var book in books)
		{
			foreach (var key in book.Points.Keys.ToList())
			{
				if (!kept.Contains(key))
				{
					book.Points.Remove(key);
				}
			}
		}
#endif

		await DrawChartsAsync().ConfigureAwait(false);
		await Task.Delay(-1).ConfigureAwait(false);
	}

	private static Task Main()
		=> new Program(Directory.GetCurrentDirectory()).RunAsync();

	private async Task DrawChartsAsync()
	{
		// do this in a separate loop first because the tasks
		// make printing look worse even though we aren't
		// awaiting them sequentially
		foreach (var book in Books)
		{
			PrintBookInfo(book);
		}

		var sw = Stopwatch.StartNew();
		var tasks = new List<Task>();
		foreach (var book in Books)
		{
			var outputPath = Path.Combine(ChartsDir, $"{book.Name}.png");
			tasks.Add(Drawer.SaveChartAsync(book, outputPath));
		}

		await Task.WhenAll(tasks).ConfigureAwait(false);
		Console.WriteLine($"{Books.Count} charts created after {sw.Elapsed.TotalSeconds:#.##} seconds.");
	}

	private List<NarrativeChart> GetBooks()
	{
		var books = new List<NarrativeChart>()
		{
			new P3V1(Defs.Time),
			new P3V2(Defs.Time),
		};
		// use a natural sort so V30 shows up between V29 and V31
		// and not between V3 and V4
		var scripts = Directory.GetFiles(ScriptsDir, "*.txt")
			.OrderBy(x => x, NaturalSortStringComparer.Ordinal);
		foreach (var script in scripts)
		{
			books.Add(new BookwormScriptConverter(Defs, File.ReadLines(script)));
		}
		return books;
	}

	private async Task<ScriptDefinitions> GetScriptDefinitionsAsync()
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

		var defs = new ScriptDefinitions();

		// Characters
		{
			foreach (var (key, value) in BookwormCharacters.Colors)
			{
				defs.CharacterColors.Add(key, value);
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
			}

			AddAliases(defs.LocationAliases, new()
			{
				[RA_Royals] = ["RA_RV"],
				[RA_FarthestHall] = ["RA_FH"],
				[RA_KnightBuilding] = ["RA_K"],
				[RA_ScholarBuilding] = ["RA_S"],
				[RA_Stadium] = ["RA_ST"],
				[RA_Library] = ["RA_L"],
				[RA_ADCClass] = ["RA_ADC"],
				[RA_SmallHall] = ["RA_SH"],
				[RA_Auditorium] = ["RA_A"],
				[RA_DormOther] = ["RA_DO"],
				[RA_DormEhr] = ["RA_DE"],
				[RA_DormWerk] = ["RA_DW"],
				[RA_GatherEhr] = ["RA_GE"],
				[RoyalAcademy] = ["RA"],
				[NoblesForest] = ["NF"],
				[EhrenfestCastle] = ["C"],
				[KnightsOrder] = ["KO"],
				[FerdinandsHouse] = ["FE"],
				[KarstedtsHouse] = ["KE"],
				[NoblesQuarter] = ["NQ"],
				[Temple] = ["T"],
				[ItalianRestaurant] = ["IR"],
				[MerchantCompanies] = ["M"],
				[LowerCityWorkshops] = ["W"],
				[MynesHouse] = ["MF"],
				[SmallTowns] = ["SmallTowns"],
				[RuelleTree] = ["RTree"],
				[GoddessesBath] = ["GBath"],
				[SouthernProvinces] = ["SP"],
				[MountLohenberg] = ["Lohenberg"],
				[Dunkelfelger] = ["Ditter"],
			});
		}

		// Time
		{
			defs.Time = new TimeTrackerWithUnits(BookwormTime.Lengths);

			AddAliases(defs.TimeAliases, new()
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

		var defsPath = Path.Combine(ScriptsDir, "ScriptDefinitions.json");
		await defs.SaveAsync(defsPath).ConfigureAwait(false);
		return await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
	}

	private void PrintBookInfo(NarrativeChartData chart)
	{
		var points = chart.Points.Sum(x => x.Value.Count);
		float max = float.MinValue, min = float.MaxValue;
		foreach (var point in chart.GetAllNarrativePoints())
		{
			max = Math.Max(max, point.Hour);
			min = Math.Min(min, point.Hour);
		}
		var days = (max - min) / Defs.Time.HoursPerDay;
		Console.WriteLine($"{chart.Name}: Points={points}, Days={days:#.#}");
	}
}