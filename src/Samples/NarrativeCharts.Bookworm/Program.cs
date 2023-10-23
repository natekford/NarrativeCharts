using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Models;
using NarrativeCharts.Plot;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private static async Task Main()
	{
		var time = new BookwormTimeTracker();
		var drawer = new PlotDrawer();
		var books = new BookwormNarrativeChart[]
		{
			new P3V1(time),
			new P3V2(time),
		};

		books[0].Initialize();
		for (var i = 1; i < books.Length; ++i)
		{
			books[i].Seed(books[i - 1], new(time.CurrentTotalHours));
			books[i].Initialize();
		}

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
					//book.Points.Remove(key);
				}
			}
		}

		const string DIR = @"C:\Users\User\Downloads\NarrativeCharts";
		foreach (var book in books)
		{
			Directory.CreateDirectory(DIR);

			await drawer.SaveChartAsync(book, Path.Combine(DIR, $"{book.Name}_chart.png")).ConfigureAwait(false);
			book.ExportFinalCharacterPositions(Path.Combine(DIR, $"{book.Name}_final_positions.txt"));

			var points = book.Points.Sum(x => x.Value.Count);
			Console.WriteLine($"{book.Name} marked points: {points}");
		}

		Console.ReadLine();
	}
}