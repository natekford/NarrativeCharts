using NarrativeCharts.Bookworm;
using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public class ScriptConverter_Tests
{
	private static string NL { get; } = Environment.NewLine;

	[Fact]
	public void HandleAddHour_NotNumber()
	{
		Action notNumber = () => ProcessText(">H1A");
		notNumber.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddHours_NoArgs()
	{
		var output = ProcessText(">H");
		output.Chapters.Single().Should().Be("AddHours(1f);");
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(1);
	}

	[Fact]
	public void HandleAddHours_TooMany()
	{
		Action tooMany = () => ProcessText(">H1,2");
		tooMany.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddHours_Valid()
	{
		var output = ProcessText(">H3");
		output.Chapters.Single().Should().Be("AddHours(3f);");
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(3);
	}

	[Fact]
	public void HandleAddScene_TooFew()
	{
		Action tooFew = () => ProcessText("$T,Ferdinand,Myne");
		tooFew.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddScene_TooMany()
	{
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
	public void HandleAddUnits_NoArgs()
	{
		var output = ProcessText(">U");
		output.Chapters.Single().Should().Be("Jump(1);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(1);
	}

	[Fact]
	public void HandleAddUnits_NotNumber()
	{
		Action notNumber = () => ProcessText(">U1A");
		notNumber.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddUnits_TooMany()
	{
		Action tooMany = () => ProcessText(">U1,2");
		tooMany.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleAddUnits_Valid()
	{
		var output = ProcessText(">U3");
		output.Chapters.Single().Should().Be("Jump(3);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(3);
	}

	[Fact]
	public void HandleChapter_NoTimeBetween()
	{
		Action noTimeBetween = () => ProcessText(
			"#Prologue",
			"#Chapter 1"
		);
		noTimeBetween.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleChapter_Valid()
	{
		var output = ProcessText(
			"#Prologue",
			">H1",
			"#Chapter 1",
			"// comment"
		);
		output.Chapters.Count.Should().Be(2);
		output.Chapters[0].Should().Be(
			$"// 0 hours{NL}" +
			$"Event(\"Prologue\");{NL}" +
			"AddHours(1f);"
		);
		output.Chapters[1].Should().Be(
			$"// 1 hours{NL}" +
			$"Event(\"Chapter 1\");{NL}" +
			"// comment"
		);
	}

	[Fact]
	public void HandleComment_Valid()
	{
		var output = ProcessText("// comment a b c");
		output.Chapters.Single().Should().Be("// comment a b c");
	}

	[Fact]
	public void HandleFreeze_Valid()
	{
		var output = ProcessText("@?Ferdinand,Myne");
		output.Chapters.Single().Should().Be("Freeze(Ferdinand, Myne);");
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			BookwormCharacters.Ferdinand,
			BookwormCharacters.Myne
		});
		// Frozen points get removed in NarrativeChart.Simplify
		output.ScriptConverter.Points.Values.Should().AllSatisfy(
			x => x.Count.Should().Be(0)
		);
	}

	[Fact]
	public void HandleKill_NoLocation()
	{
		Action noLocation = () => ProcessText("@!Detlinde,Georgine");
		noLocation.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void HandleKill_Valid()
	{
		// Kill requires a character to already have a location
		var output = ProcessText(
			"$Ahrensbach=Detlinde,Georgine",
			"@!Detlinde,Georgine"
		);
		output.Chapters.Single().Should().Be(
			$"Add(Ahrensbach, Detlinde, Georgine);{NL}" +
			"Kill(Detlinde, Georgine);"
		);
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			BookwormCharacters.Detlinde,
			BookwormCharacters.Georgine,
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			var point = x.Single().Value;
			point.Hour.Should().Be(0);
			point.Location.Should().Be(BookwormLocations.Ahrensbach);
			point.IsEnd.Should().Be(true);
		});
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
	public void ProcessLine_InvalidSymbol()
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