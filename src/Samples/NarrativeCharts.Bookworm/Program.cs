using NarrativeCharts.Bookworm.P3;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private static void Main()
	{
		var time = new BookwormTimeTracker();
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

		const string DIR = @"C:\Users\User\Downloads";
		foreach (var book in books)
		{
			book.Export(DIR);
		}

		Console.ReadLine();
	}
}