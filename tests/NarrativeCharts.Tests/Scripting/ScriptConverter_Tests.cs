using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public class ScriptConverter_Tests
{
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

	private static Output ProcessText(params string[] lines)
	{
		var converter = new ScriptConverter(
			definitions: new()
			{
				ScriptDirectory = Directory.GetCurrentDirectory()
			},
			lastWriteTimeUtc: DateTime.UtcNow,
			lines: lines
		);
		converter.Initialize(null);

		var chapters = converter.Chapters.ConvertAll(x => x.TrimEnd().ToString());
		return new(converter, chapters);
	}

	private record Output(ScriptConverter ScriptConverter, List<string> Chapters);
}