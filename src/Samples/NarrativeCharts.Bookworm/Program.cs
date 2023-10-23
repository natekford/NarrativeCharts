using NarrativeCharts.Bookworm.P3;
using NarrativeCharts.Plot;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private static async Task Main()
	{
		var time = new BookwormTimeTracker();
		var drawer = new PlotDrawer();
		var books = new[]
		{
			new P3V1(time).Create(),
			new P3V2(time).Create(),
		};

		var kept = new HashSet<string>
		{
			BookwormCharacters.Ferdinand.Name,
		};
		foreach (var book in books)
		{
			foreach (var key in book.Points.Keys.ToList())
			{
				if (!kept.Contains(key))
				{
					//books[0].Points.Remove(key);
				}
			}
		}

		const string DIR = @"C:\Users\User\Downloads\NarrativeCharts";
		foreach (var book in books)
		{
			book.Simplify();

			Directory.CreateDirectory(DIR);

			await drawer.SaveChartAsync(book, Path.Combine(DIR, $"{book.Name}_chart.png")).ConfigureAwait(false);
			book.ExportFinalCharacterPositions(Path.Combine(DIR, $"{book.Name}_final_positions.txt"));

			var points = book.Points.Sum(x => x.Value.Count);
			Console.WriteLine($"{book.Name} marked points: {points}");
		}

		Console.ReadLine();
	}
}