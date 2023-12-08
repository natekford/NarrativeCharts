using Microsoft.CodeAnalysis.CSharp;

using NarrativeCharts.Bookworm;
using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests;

public static class TestUtils
{
	public static NarrativeChartData Chart
	{
		get
		{
			var chart = new NarrativeChartData();
			foreach (var (key, value) in BookwormCharacters.Colors)
			{
				chart.Colors.Add(key, value);
			}
			foreach (var (key, value) in BookwormLocations.YIndexes)
			{
				chart.YIndexes.Add(key, value);
			}
			return chart;
		}
	}

	public static ScriptDefinitions ScriptDefinitions
		=> Program.CreateScriptDefinitions(Directory.GetCurrentDirectory());

	public static void ValidateSyntax(string text)
	{
		var tree = CSharpSyntaxTree.ParseText(text);
		var diagnostics = tree.GetDiagnostics();
		diagnostics.Should().BeEmpty();
	}
}