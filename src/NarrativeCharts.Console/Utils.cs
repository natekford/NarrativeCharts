using NarrativeCharts.Models;
using NarrativeCharts.ScottPlot;

using System.Text;

namespace NarrativeCharts.Console;

public static class Utils
{
	public static void Export(this BookwormNarrativeChart chart, string dir)
	{
		chart.PlotChart(6000, 1000, Path.Combine(dir, $"{chart.Name}_plot.png"));
		chart.ExportFinalCharacterPositions(Path.Combine(dir, $"{chart.Name}_chars.txt"));

		System.Console.WriteLine($"{chart.Name} total points: {chart.Points.Sum(x => x.Value.Count)}");
	}

	public static void ExportFinalCharacterPositions(this NarrativeChart chart, string path)
	{
		// should output something like this:
		/*
			Add(Scene(Temple).With(Ferdinand, Karstedt, Myne, Sylvester));
			Add(Scene(KnightsOrder).With(Bezewanst, Bindewald));
			Add(Scene(Temple).With(Damuel, Delia, Dirk, Egmont, Ella, Gil, Monika, Nicola, Rosina, Wilma, Zahm));
			Add(Scene(LowerCityWorkshops).With(Deid, Ingo, Johann));
			Add(Scene(GilbertaCompany).With(Benno, Leon, Lutz, Mark));
			Add(Scene(MynesHouse).With(Effa, Gunther, Kamil, Tuuli));
			Add(Scene(ItalianRestaurant).With(Hugo, Leise, Todd));
			Add(Scene(OthmarCompany).With(Freida, Gustav));
		*/

		var finalPoints = chart.Points.Select(x => x.Value.Values[^1]);
		var grouped = finalPoints.GroupBy(x => x.Point.Y);

		var sb = new StringBuilder();
		foreach (var group in grouped)
		{
			var location = Locations.PropertyDictionary[group.Key];

			sb.Append("Add(Scene(").Append(location).Append(").With(");
			var first = true;
			foreach (var character in group)
			{
				if (!first)
				{
					sb.Append(", ");
				}
				first = false;
				sb.Append(character.Character);
			}
			sb.AppendLine("));");
		}

		File.WriteAllText(path, sb.ToString());
	}

	public static NarrativeScene With(this Point point, params Character[] characters)
		=> new(point, characters.Select(x => x.Name).ToArray());
}