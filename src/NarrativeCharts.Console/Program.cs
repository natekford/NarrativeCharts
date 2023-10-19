using NarrativeCharts.Console.P3;
using NarrativeCharts.ScottPlot;

namespace NarrativeCharts.Console;

public static class Program
{
	private static void Main()
	{
		var time = new BookwormTimeTracker();
		var p3v1 = new P3V1(time).Create();
		var p3v2 = new P3V2(time).Create();

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
				//p3v1.Points.Remove(key);
			}
		}

		p3v1.PlotChart(6000, 1000, @"C:\Users\User\Downloads\p3v1.png");
		p3v2.PlotChart(6000, 1000, @"C:\Users\User\Downloads\p3v2.png");

		System.Console.WriteLine($"P3V1 total points: {p3v1.Points.Sum(x => x.Value.Count)}");
		System.Console.WriteLine($"P3V2 total points: {p3v2.Points.Sum(x => x.Value.Count)}");
		System.Console.ReadLine();
	}
}