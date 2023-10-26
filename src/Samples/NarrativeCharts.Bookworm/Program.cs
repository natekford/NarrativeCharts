using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Models;
using NarrativeCharts.Plot;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private static async Task Main()
	{
		var time = new BookwormTimeTracker();
		var drawer = new ScottPlotDrawer(
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

		const string DIR = @"C:\Users\User\Downloads\NarrativeCharts";
		foreach (var book in books)
		{
			Directory.CreateDirectory(DIR);

			var path = Path.Combine(DIR, $"{book.Name}_chart.png");
			await drawer.SaveChartAsync(book, path).ConfigureAwait(false);

			var points = book.Points.Sum(x => x.Value.Count);
			Console.WriteLine($"{book.Name} marked points: {points}");

			var myne = book.Points[BookwormCharacters.Myne];
			var start = myne.Values[0].Point.Hour;
			var end = myne.Values[^1].Point.Hour;
			var days = (end - start) / time.HoursPerDay;
			Console.WriteLine($"{book.Name} ellapsed days: {days}");
		}

		Console.ReadLine();
	}
}