using Microsoft.CodeAnalysis.CSharp;

using NarrativeCharts.Bookworm;
using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests;

public static class TestUtils
{
	public static ScriptDefinitions ScriptDefinitions
		=> Program.CreateScriptDefinitions(Directory.GetCurrentDirectory());

	public static void ValidateSyntax(string text)
	{
		var tree = CSharpSyntaxTree.ParseText(text);
		var diagnostics = tree.GetDiagnostics();
		diagnostics.Should().BeEmpty();
	}
}