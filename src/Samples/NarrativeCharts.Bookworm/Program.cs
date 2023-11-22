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
	public const int PARALLEL_CHART_COUNT = 10;
	public const bool REDRAW_ALL_SCRIPTS = false;

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
		//YSpacing = 6,
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
		var count = 0;

		var books = new List<(NarrativeChartData, string)>();
		var shouldRedrawScripts = REDRAW_ALL_SCRIPTS;
		foreach (var book in Books)
		{
			var imagePath = Path.Combine(ChartsDir, $"{book.Name}.png");
			if (!shouldRedrawScripts && book is BookwormScriptConverter scriptConverter)
			{
				var imageTime = File.GetLastWriteTimeUtc(imagePath);
				var scriptTime = scriptConverter.LastWriteTimeUTC;
				if (imageTime >= scriptTime)
				{
					Console.WriteLine($"[{Interlocked.Increment(ref count)}/{Books.Count}] " +
						$"Not redrawing {Path.GetFileName(imagePath)}. " +
						$"Drawn: {imageTime:G}, " +
						$"edited: {scriptTime:G}.");
					continue;
				}
				else
				{
					// redraw subsequent scripts because the editing of a previous script
					// could change character seeding locations
					shouldRedrawScripts = true;
				}
			}

			books.Add(new(book, imagePath));
		}

#if false
		// Parallel.ForEachAsync is a lot simpler to write
		var parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = PARALLEL_CHART_COUNT,
		};
		await Parallel.ForEachAsync(books, parallelOptions, async (book, _) =>
		{
			var start = sw.Elapsed;

			await Drawer.SaveChartAsync(book.Book, book.ImagePath).ConfigureAwait(false);

			Console.WriteLine($"[{Interlocked.Increment(ref count)}/{Books.Count}] " +
				$"Finished drawing {book.ImagePath} " +
				$"in {(sw.Elapsed - start).TotalSeconds:#.##} seconds.");
		}).ConfigureAwait(false);
#else
		// But manually doing the same thing as Parallel.ForEachAsync allows
		// for potentially accounting for RAM usage instead of just task count
		// Which probably is important considering how this program can regularly
		// use up to 10GB+ of RAM
		var queue = new Queue<(NarrativeChartData, string)>(books);
		var active = new Dictionary<Task, (TimeSpan, string)>();
		async Task WhenAnyDrawingAsync()
		{
			var task = await Task.WhenAny(active.Keys).ConfigureAwait(false);
			active.Remove(task, out var item);
			var (start, imagePath) = item;
			Console.WriteLine($"[{Interlocked.Increment(ref count)}/{Books.Count}] " +
				$"Finished drawing {Path.GetFileName(imagePath)} " +
				$"in {(sw.Elapsed - start).TotalSeconds:#.##} seconds.");
		}

		while (queue.TryDequeue(out var item))
		{
			if (active.Count >= PARALLEL_CHART_COUNT)
			{
				await WhenAnyDrawingAsync().ConfigureAwait(false);
			}

			var (book, imagePath) = item;
			active.Add(
				key: Drawer.SaveChartAsync(book, imagePath),
				value: new(sw.Elapsed, imagePath)
			);
		}
		while (active.Any())
		{
			await WhenAnyDrawingAsync().ConfigureAwait(false);
		}
#endif

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
			books.Add(new BookwormScriptConverter(
				definitions: Defs,
				lastWriteTimeUtc: File.GetLastWriteTimeUtc(script),
				lines: File.ReadLines(script)
			));
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
		var properties = new Dictionary<string, string>();

		{
			properties["Character"] = chart.Points.Count.ToString();
		}

		{
			var points = chart.Points.Sum(x => x.Value.Count);
			properties["Points"] = points.ToString();
		}

		{
			float max = float.MinValue, min = float.MaxValue;
			foreach (var point in chart.GetAllNarrativePoints())
			{
				max = Math.Max(max, point.Hour);
				min = Math.Min(min, point.Hour);
			}
			var days = (max - min) / Defs.Time.HoursPerDay;
			properties["Days"] = days.ToString("#.#");
		}

		var joined = string.Join(", ", properties.Select(x => $"{x.Key}={x.Value}"));
		Console.WriteLine($"{chart.Name}: {joined}");
	}
}