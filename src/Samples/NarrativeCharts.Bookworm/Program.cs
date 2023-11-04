using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Models;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;

using System.Diagnostics;

using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm;

public class Program
{
	public List<NarrativeChartData> Books { get; } = [];
	public string ChartsDir { get; }
	public string Dir { get; }
	public string ScriptsDir { get; }
	public BookwormTimeTracker Time { get; } = new();

	public Program(string dir)
	{
		Dir = dir;
		ChartsDir = Path.Combine(Dir, "Charts");
		ScriptsDir = Path.Combine(Dir, "Scripts");
	}

	public async Task RunAsync()
	{
		var defs = await GetScriptDefinitionsAsync().ConfigureAwait(false);
		defs.Time = Time;

		var books = new NarrativeChart[]
		{
			new P3V1(Time),
			new P3V2(Time),
			FromScript(defs, "P3V3.txt"),
			FromScript(defs, "P3V4.txt"),
			FromScript(defs, "P3V5.txt"),
			FromScript(defs, "P4V1.txt"),
		};
		for (var i = 0; i < books.Length; ++i)
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
		var kept = new HashSet<Character>
		{
			BookwormCharacters.Myne,
			BookwormCharacters.Georgine,
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
		var sw = Stopwatch.StartNew();
		var tasks = new List<Task>();
		var drawer = new SKChartDrawer();
		foreach (var book in Books)
		{
			var outputPath = Path.Combine(ChartsDir, $"{book.Name}.png");
			tasks.Add(drawer.SaveChartAsync(book, outputPath));

			var points = book.Points.Sum(x => x.Value.Count);
			var days = GetDays(book);
			Console.WriteLine($"{book.Name}: Points={points}, Days={days}");
		}

		await Task.WhenAll(tasks).ConfigureAwait(false);
		Console.WriteLine($"{Books.Count} charts created after {sw.Elapsed.TotalSeconds:#.##} seconds.");
	}

	private BookwormScriptConverter FromScript(ScriptDefinitions defs, string fileName)
		=> new(defs, File.ReadLines(Path.Combine(ScriptsDir, fileName)));

	private int GetDays(NarrativeChartData chart)
	{
		int max = int.MinValue, min = int.MaxValue;
		foreach (var point in chart.GetAllNarrativePoints())
		{
			max = Math.Max(max, point.Hour);
			min = Math.Min(min, point.Hour);
		}
		return (max - min) / Time.HoursPerDay;
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
				[GiebeDahldolf] = [nameof(GiebeDahldolf)],
				[GiebeGroschel] = [nameof(GiebeGroschel)],
				[GiebeJoisontak] = [nameof(GiebeJoisontak)],
				[GiebeKirnberger] = [nameof(GiebeKirnberger)],
				[HasseMayor] = [nameof(HasseMayor)],
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
				[MountLohenberg] = ["Lohenberg"],
				[AhrensbachCastle] = ["AC"],
			});
		}

		// Time
		{
			defs.Time = Time;

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
}