using NarrativeCharts.Bookworm.P3;

namespace NarrativeCharts.Bookworm;

public static class Program
{
	private static void Main()
	{
		var time = new BookwormTimeTracker();
		var p3v1 = new P3V1(time).Create();
		var p3v2 = new P3V2(time).Create();

		var kept = new HashSet<string>
		{
			BookwormCharacters.Myne.Name,
			BookwormCharacters.Benno.Name,
			BookwormCharacters.Lutz.Name
		};
		foreach (var key in p3v1.Points.Keys.ToList())
		{
			if (!kept.Contains(key))
			{
				//p3v1.Points.Remove(key);
			}
		}

		const string DIR = @"C:\Users\User\Downloads";
		p3v1.Export(DIR);
		p3v2.Export(DIR);

		Console.ReadLine();
	}
}