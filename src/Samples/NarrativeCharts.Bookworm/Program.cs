using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Script;
using NarrativeCharts.Skia;

using System.Diagnostics;

using static NarrativeCharts.Bookworm.BookwormBell;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Bookworm;

public class Program
{
	public List<NarrativeChart> Books { get; } = new();
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
		Books.Add(new P3V1(Time));
		Books.Add(new P3V2(Time));
		Books.Add(new P3V3(Time));
		for (var i = 0; i < Books.Count; ++i)
		{
			Books[i].Initialize(i == 0 ? null : Books[i - 1]);
		}
		await AddScriptedBookAsync().ConfigureAwait(false);

#if false
		var kept = new HashSet<Character>
		{
			BookwormCharacters.Ferdinand,
			BookwormCharacters.Myne,
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

	private async Task AddScriptedBookAsync()
	{
		var p3v2 = Books[1];

		var defsPath = Path.Combine(ScriptsDir, "ScriptDefinitions.json");
		var orgDefs = CreateBookwormScriptDefinitions();
		await orgDefs.SaveAsync(defsPath).ConfigureAwait(false);
		var defs = await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
		defs.Time.SetTotalHours(p3v2.Points[BookwormCharacters.Myne].Keys[^1]);

		var scriptPath = Path.Combine(ScriptsDir, "P3V3.txt");
		var scripted = new ScriptLoader(defs, scriptPath);
		scripted.Initialize(p3v2);

		Books.Add(scripted);
	}

	private ScriptDefinitions CreateBookwormScriptDefinitions()
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
		}

		// Locations
		{
			foreach (var (key, value) in BookwormLocations.YIndexes)
			{
				defs.LocationYIndexes.Add(key, value);
			}

			AddAliases(defs.LocationAliases, new()
			{
				[Temple] = new[] { "T" },
				[MerchantCompanies] = new[] { "M" },
				[LowerCityWorkshops] = new[] { "W" },
				[MynesHouse] = new[] { "MF" },
				[Castle] = new[] { "C" },
				[NoblesQuarter] = new[] { "NQ" }
			});
		}

		// Time
		{
			defs.Time = Time;

			AddAliases(defs.TimeAliases, new()
			{
				[0] = new[] { nameof(Midnight) },
				[1] = new[] { nameof(FirstBell), nameof(EarlyMorning) },
				[2] = new[] { nameof(SecondBell), nameof(Morning) },
				[3] = new[] { nameof(ThirdBell), nameof(Meetings) },
				[4] = new[] { nameof(FourthBell), nameof(Lunch) },
				[5] = new[] { nameof(FifthBell), nameof(MarketClose), nameof(Tea) },
				[6] = new[] { nameof(SixthBell), nameof(Dinner) },
				[7] = new[] { nameof(SeventhBell), nameof(Bed) },
			});
		}

		return defs;
	}

	private async Task DrawChartsAsync()
	{
		var sw = Stopwatch.StartNew();
		var tasks = new List<Task>();
		var drawer = new SkiaDrawer();
		foreach (var book in Books)
		{
			var outputPath = Path.Combine(ChartsDir, $"{book.Name}.png");
			tasks.Add(drawer.SaveChartAsync(book, outputPath));

			var points = book.Points.Sum(x => x.Value.Count);
			var myne = book.Points[BookwormCharacters.Myne].Keys;
			var days = (myne[^1] - myne[0]) / Time.HoursPerDay;
			Console.WriteLine($"{book.Name}: Points={points}, Days={days}");
		}

		await Task.WhenAll(tasks).ConfigureAwait(false);
		Console.WriteLine($"{Books.Count} charts created after {sw.Elapsed.TotalSeconds:#.##} seconds.");
	}
}