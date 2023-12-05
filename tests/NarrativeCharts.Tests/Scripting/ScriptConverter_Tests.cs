using NarrativeCharts.Bookworm;
using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public class ScriptConverter_Tests
{
	[Fact]
	public void HandleAddScene_Invalid()
	{
		Action tooFew = () => ProcessText("$T,Ferdinand,Myne");
		tooFew.Should().Throw<ArgumentException>();

		Action tooMany = () => ProcessText("$T=Ferdinand=Myne");
		tooMany.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddScene_Valid()
	{
		var output = ProcessText("$T=Ferdinand,Myne");
		output.Chapters.Single().Should().Be("Add(Temple, Ferdinand, Myne);");
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			BookwormCharacters.Ferdinand,
			BookwormCharacters.Myne
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			var point = x.Single().Value;
			point.Hour.Should().Be(0);
			point.Location.Should().Be(BookwormLocations.Temple);
		});
	}

	[Fact]
	public void HandleComment_Test()
	{
		var output = ProcessText("// comment a b c");
		output.Chapters.Single().Should().Be("// comment a b c");
	}

	[Fact]
	public void HandleTitle_NoSpaces()
	{
		var output = ProcessText("##ASDF7");
		output.Chapters.Single().Length.Should().Be(0);
		output.ScriptConverter.Name.Should().Be("ASDF7");
		output.ScriptConverter.ClassName.Should().Be("ASDF7");
	}

	[Fact]
	public void HandleTitle_Spaces()
	{
		var output = ProcessText("##A SD F7");
		output.Chapters.Single().Length.Should().Be(0);
		output.ScriptConverter.Name.Should().Be("A SD F7");
		output.ScriptConverter.ClassName.Should().Be("ASDF7");
	}

	[Fact]
	public void ProcessLine_Invalid()
	{
		Action action = () => ProcessText("??not a valid symbol");
		action.Should().Throw<ArgumentException>();
	}

	private static Output ProcessText(params string[] lines)
	{
		var converter = new ScriptConverter(
			definitions: Program.CreateScriptDefinitions(Directory.GetCurrentDirectory()),
			lastWriteTimeUtc: DateTime.UtcNow,
			lines: lines
		);
		converter.Initialize(null);

		var chapters = converter.Chapters.ConvertAll(x => x.TrimEnd().ToString());
		return new(converter, chapters);
	}

	private record Output(ScriptConverter ScriptConverter, List<string> Chapters);
}