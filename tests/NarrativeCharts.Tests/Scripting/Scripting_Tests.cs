using NarrativeCharts.Scripting;
using System.Text.RegularExpressions;
using NarrativeCharts.Tests.Drawing;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Scripting.ScriptingUtils;

namespace NarrativeCharts.Tests.Scripting;

/// <summary>
/// Miscellaneous scripting tests.
/// </summary>
[Trait("Category", "Integration")]
public partial class Scripting_Tests
{
	private bool RedrawUneditedScripts { get; set; } = true;
	private TimeSpan? SaveDelay { get; set; }

	[Fact]
	public void ConvertToCode_Syntax()
	{
		var output = GetDefs().ConvertToCode();

		TestUtils.ValidateSyntax(output);
	}

	[Fact]
	public async Task DrawScripts_MoreScriptsThanParallel()
	{
		SaveDelay = TimeSpan.FromMilliseconds(100);
		var scripts = Enumerable.Repeat(0, 11).Select(_ => new FakeScriptConverter()).ToList();
		var info = await DrawAsync(GetDefs(), scripts).ConfigureAwait(false);

		info.Should().HaveCount(11);
		info.Should().AllSatisfy(x => x.DrawTime.Should().NotBeNull());
	}

	[Fact]
	public async Task DrawScripts_NoRedrawing()
	{
		RedrawUneditedScripts = false;
		var defs = GetDefs();
		var scripts = Enumerable.Repeat(0, 5).Select(_ => new FakeScriptConverter()).ToList();
		foreach (var script in scripts[..2])
		{
			var path = Path.Combine(defs.ScriptDirectory, CHARTS_DIR, $"{script.Name}.png");
			File.Create(path).Dispose();
		}

		var info = await DrawAsync(defs, scripts).ConfigureAwait(false);

		info.Should().HaveCount(5);
		info[..2].Should().AllSatisfy(x => x.DrawTime.Should().BeNull());
		info[2..].Should().AllSatisfy(x => x.DrawTime.Should().NotBeNull());
	}

	[Fact]
	public async Task DrawScripts_RedrawIfFirstEdited()
	{
		RedrawUneditedScripts = false;
		var defs = GetDefs();
		var scripts = Enumerable.Repeat(0, 4)
			.Select(_ => new FakeScriptConverter())
			.Prepend(new FakeScriptConverter(DateTime.UtcNow + TimeSpan.FromHours(1)))
			.ToList();
		foreach (var script in scripts)
		{
			var path = Path.Combine(defs.ScriptDirectory, CHARTS_DIR, $"{script.Name}.png");
			File.Create(path).Dispose();
		}

		var info = await DrawAsync(defs, scripts).ConfigureAwait(false);

		info.Should().HaveCount(5);
		info.Should().AllSatisfy(x => x.DrawTime.Should().NotBeNull());
	}

	[Fact]
	public async Task DrawScripts_Valid()
	{
		var scripts = new[] { new FakeScriptConverter() };
		var info = await DrawAsync(GetDefs(), scripts).ConfigureAwait(false);

		info.Should().HaveCount(1);
		info.Should().AllSatisfy(x => x.DrawTime.Should().NotBeNull());
	}

	[Fact]
	public void LoadScripts_Valid()
	{
		var scripts = TestUtils.Defs.LoadScripts().ToList();

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
	public void SaveConvertedScripts_Disabled()
	{
		var defs = GetDefs();
		defs.ConvertScripts = false;
		var scripts = new[] { new FakeScriptConverter() };
		var saved = defs.SaveConvertedScripts(scripts);

		saved.Should().BeEmpty();
		scripts.Single().WasWritten.Should().Be(false);
	}

	[Fact]
	public void SaveConvertedScripts_NoDefinitions()
	{
		var defs = GetDefs();
		var scripts = new[] { new FakeScriptConverter() };
		var saved = defs.SaveConvertedScripts(scripts, saveDefinitions: false);

		saved.Should().HaveCount(1);
		scripts.Single().WasWritten.Should().Be(true);
	}

	[Fact]
	public void SaveConvertedScripts_YesDefinitions()
	{
		var defs = GetDefs();
		var scripts = new[] { new FakeScriptConverter() };
		var saved = defs.SaveConvertedScripts(scripts);

		saved.Should().HaveCount(2);
		scripts.Single().WasWritten.Should().Be(true);
	}

	[Fact]
	public async Task ScriptDefinitionsSaveAndLoad_Valid()
	{
		var defs = GetDefs();
		defs.OnlyDrawTheseCharacters = [Ferdinand, Myne];

		var path = Path.Combine(defs.ScriptDirectory, "TestDefs.json");
		await defs.SaveAsync(path).ConfigureAwait(false);
		var loadedDefs = await ScriptDefinitions.LoadAsync(path).ConfigureAwait(false);

		loadedDefs.Should().BeEquivalentTo(defs);
	}

	[GeneratedRegex(@"P(\d+)V(\d+)")]
	private static partial Regex BookNumberRegex();

	private async Task<List<DrawInfo>> DrawAsync(
		ScriptDefinitions defs,
		IReadOnlyList<ScriptParser> scripts)
	{
		var drawer = new FakeChartDrawer()
		{
			SaveDelay = SaveDelay,
		};

		var output = new List<DrawInfo>();
		await foreach (var info in defs.DrawScriptsAsync(scripts, drawer))
		{
			output.Add(info);
		}
		return output;
	}

	private ScriptDefinitions GetDefs()
	{
		var defs = TestUtils.Defs;
		defs.RedrawUneditedScripts = RedrawUneditedScripts;
		defs.ScriptDirectory = Path.Combine(defs.ScriptDirectory, nameof(Scripting_Tests));
		return defs;
	}
}