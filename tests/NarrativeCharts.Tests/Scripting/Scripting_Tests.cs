using NarrativeCharts.Scripting;

using System.Text.RegularExpressions;

namespace NarrativeCharts.Tests.Scripting;

/// <summary>
/// Miscellaneous scripting tests.
/// </summary>
public partial class Scripting_Tests
{
	[Fact]
	public void ConvertToCode_Syntax()
	{
		var output = TestUtils.ScriptDefinitions.ConvertToCode();

		TestUtils.ValidateSyntax(output);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public void LoadScripts_Valid()
	{
		var defs = TestUtils.ScriptDefinitions;
		var scripts = defs.LoadScripts().ToList();

		var bookNumbers = scripts.ConvertAll(x =>
		{
			var match = BookNumberRegex().Match(x.Name);
			var part = int.Parse(match.Groups[1].ValueSpan);
			var volume = int.Parse(match.Groups[2].ValueSpan);
			return (part, volume);
		});
		bookNumbers.Should().BeInAscendingOrder();
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task ScriptDefinitions_SaveAndLoad()
	{
		var defs = TestUtils.ScriptDefinitions;
		var path = Path.Combine(defs.ScriptDirectory, nameof(Scripting_Tests), "TestDefs.json");
		defs.ScriptDirectory = Path.GetDirectoryName(path)!;

		await defs.SaveAsync(path).ConfigureAwait(false);
		var loadedDefs = await ScriptDefinitions.LoadAsync(path).ConfigureAwait(false);

		loadedDefs.Should().BeEquivalentTo(defs);
	}

	[GeneratedRegex(@"P(\d+)V(\d+)")]
	private static partial Regex BookNumberRegex();
}