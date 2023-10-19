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

		var p3 = new NarrativeChart();
		p3.AddChart(p3v1);
		p3.AddChart(p3v2);

		var kept = new HashSet<string>
		{
			Characters.Myne.Name,
			Characters.Benno.Name,
			Characters.Lutz.Name
		};
		foreach (var key in p3v1.Points.Keys.ToList())
		{
			if (!kept.Contains(key))
			{
				p3v1.Points.Remove(key);
			}
		}

		await p3v1.DrawChartAsync(@"C:\Users\User\Downloads\chart_test.png").ConfigureAwait(false);

		System.Console.WriteLine($"P3V1 total narrative points: {p3v1.Points.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3V2 total narrative points: {p3v2.Points.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3 total narrative point: {p3.Points.Sum(x => x.Value.Count)}");
		System.Console.ReadLine();
	}
}