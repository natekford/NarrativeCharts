using NarrativeCharts.Console.P3;
using NarrativeCharts.Image;

namespace NarrativeCharts.Console;

public static class Program
{
	private static async Task Main()
	{
		var time = new BookwormTimeTracker();
		var p3v1 = P3V1.Generate(time);
		var p3v2 = P3V2.Generate(time);

		var p3 = new NarrativeChartGenerator();
		p3.Add(p3v1);
		p3.Add(p3v2);

		await p3.DrawChartAsync(@"C:\Users\User\Downloads\chart_test.png").ConfigureAwait(false);

		System.Console.WriteLine($"P3V1 total narrative points: {p3v1.NarrativePoints.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3V2 total narrative points: {p3v2.NarrativePoints.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3 total narrative point: {p3.NarrativePoints.Sum(x => x.Value.Count)}");
		System.Console.ReadLine();
	}
}