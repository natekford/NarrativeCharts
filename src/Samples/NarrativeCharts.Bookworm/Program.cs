using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Models;
using NarrativeCharts.Skia;

using System.Diagnostics;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private const string DIR = @"C:\Users\User\Downloads\NarrativeCharts";

	private static async Task Main()
	{
		var time = new BookwormTimeTracker();
		var drawer = new SkiaDrawer(
			colors: BookwormCharacters.Colors,
			yIndexes: BookwormLocations.YIndexes
		);
		var books = new BookwormNarrativeChart[]
		{
			new P3V1(time),
			new P3V2(time),
			new P3V3(time),
		};

		books[0].Initialize(null);
		for (var i = 1; i < books.Length; ++i)
		{
			books[i].Initialize(books[i - 1]);
		}

		var scripted = new BookwormScriptLoader(time, @"C:\Users\User\Downloads\NarrativeCharts\P3V3 Script.txt");
		scripted.Initialize(books[1]);

		var kept = new HashSet<Character>
		{
			//BookwormCharacters.Benno,
			BookwormCharacters.Ferdinand,
			BookwormCharacters.Myne,
		};
		foreach (var book in books)
		{
			foreach (var key in book.Points.Keys.ToList())
			{
				if (!kept.Contains(key))
				{
					//book.Points.Remove(key);
				}
			}
		}

		var sw = Stopwatch.StartNew();
		var tasks = new List<Task>();
		foreach (var book in books.Append(scripted))
		{
			Directory.CreateDirectory(DIR);

			var path = Path.Combine(DIR, $"{book.Name}_chart.png");
			tasks.Add(drawer.SaveChartAsync(book, path));

			var points = book.Points.Sum(x => x.Value.Count);
			var myne = book.Points[BookwormCharacters.Myne];
			var start = myne.Values[0].Point.Hour;
			var end = myne.Values[^1].Point.Hour;
			var days = (end - start) / time.HoursPerDay;
			Console.WriteLine($"{book.Name}: Points={points}, Days={days}");
		}

		await Task.WhenAll(tasks).ConfigureAwait(false);
		Console.WriteLine($"{books.Length} charts created after {sw.Elapsed.TotalSeconds:#.##} seconds.");

		Console.ReadLine();
	}
}