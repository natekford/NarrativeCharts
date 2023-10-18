using NarrativeCharts.Console.P3;

namespace NarrativeCharts.Console;

public static class Program
{
	private static void Main()
	{
		var time = new BookwormTimeTracker();
		var p3v1 = P3V1.Generate(time);
		var p3v2 = P3V2.Generate(time);

		System.Console.WriteLine($"P3V1 total narrative points: {p3v1.NarrativePoints.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3V2 total narrative points: {p3v2.NarrativePoints.Sum(x => x.Value.Count)}");
		System.Console.ReadLine();
	}
}