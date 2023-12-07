﻿using NarrativeCharts.Bookworm;
using NarrativeCharts.Models;
using NarrativeCharts.Scripting;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Tests.Scripting;

/// <summary>
/// These tests cover ScriptConverter, ScriptParser, and NarrativeChart because of
/// how tightly coupled they are.
/// </summary>
public class ScriptConverter_Tests
{
	private static string NL { get; } = Environment.NewLine;

	[Fact]
	public void HandleAddCharacterGroup_TooFew()
	{
		Action tooFew = () => ProcessText("+%Group,Ferdinand,Myne,Damuel");
		tooFew.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddCharacterGroup_TooMany()
	{
		Action tooMany = () => ProcessText("+%Group=Ferdinand,Myne=Damuel");
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddCharacterGroup_Valid()
	{
		var output = ProcessText(
			"+%g1=Eckhart,Ferdinand,Justus",
			"+%g2=Angelica,Damuel,Myne",
			"+%g3=g1,g2"
		);
		output.ScriptConverter.CharacterGroups.Should().BeEquivalentTo(new Dictionary<string, HashSet<Character>>
		{
			["g1"] = [Eckhart, Ferdinand, Justus],
			["g2"] = [Angelica, Damuel, Myne],
			["g3"] = [Angelica, Damuel, Eckhart, Ferdinand, Justus, Myne],
		});
	}

	[Fact]
	public void HandleAddHour_NotNumber()
	{
		Action notNumber = () => ProcessText(">H1A");
		notNumber.Should().Throw<ScriptParserException>()
			.WithInnerException<FormatException>();
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
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddHours_Valid()
	{
		var output = ProcessText(">H3");
		output.Chapters.Single().Should().Be("AddHours(3f);");
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(3);
	}

	[Fact]
	public void HandleAddReturnableScene_NoLocation()
	{
		Action noLocation = () => ProcessText(
			"+$1,C=Ferdinand,Myne"
		);
		noLocation.Should().Throw<ScriptParserException>()
			.WithInnerException<KeyNotFoundException>();
	}

	[Fact]
	public void HandleAddReturnableScene_TooFew()
	{
		Action noLocation = () => ProcessText(
			"$T=Ferdinand,Myne",
			"+$1=C=Ferdinand=Myne"
		);
		noLocation.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddScene_TooFew()
	{
		Action tooFew = () => ProcessText("$T,Ferdinand,Myne");
		tooFew.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddScene_TooMany()
	{
		Action tooMany = () => ProcessText("$T=Ferdinand=Myne");
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddScene_Valid()
	{
		var output = ProcessText("$T=Ferdinand,Myne");
		output.Chapters.Single().Should().Be("Add(Temple, Ferdinand, Myne);");
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			Ferdinand,
			Myne
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			var point = x.Single().Value;
			point.Hour.Should().Be(0);
			point.Location.Should().Be(Temple);
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
		notNumber.Should().Throw<ScriptParserException>()
			.WithInnerException<FormatException>();
	}

	[Fact]
	public void HandleAddUnits_TooMany()
	{
		Action tooMany = () => ProcessText(">U1,2");
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
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
		noTimeBetween.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
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
			Ferdinand,
			Myne
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
		noLocation.Should().Throw<ScriptParserException>()
			.WithInnerException<KeyNotFoundException>();
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
			Detlinde,
			Georgine,
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			var point = x.Single().Value;
			point.Hour.Should().Be(0);
			point.Location.Should().Be(Ahrensbach);
			point.IsEnd.Should().Be(true);
		});
	}

	[Fact]
	public void HandleSkipToCurrentDay_InPast()
	{
		Action inPast = () => ProcessText(
			">FirstBell",
			">H1",
			">FirstBell"
		);
		inPast.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void HandleSkipToCurrentDay_NoArgs()
	{
		var output = ProcessText(">");
		output.Chapters.Single().Should().Be("Jump(1);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(1);
	}

	[Fact]
	public void HandleSkipToCurrentDay_SameTime()
	{
		var output = ProcessText(
			">FirstBell",
			">FirstBell"
		);
		output.Chapters.Single().Should().Be(
			$"SkipToCurrentDay(FirstBell);{NL}" +
			"SkipToCurrentDay(FirstBell);"
		);
		output.ScriptConverter.Time.CurrentUnit.Should().Be(1);
	}

	[Fact]
	public void HandleSkipToCurrentDay_TooMany()
	{
		Action tooMany = () => ProcessText(">1,2");
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleSkipToCurrentDay_Valid()
	{
		var output = ProcessText(">SecondBell");
		output.Chapters.Single().Should().Be("SkipToCurrentDay(SecondBell);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(2);
	}

	[Fact]
	public void HandleSkipToNextDay_NoArgs()
	{
		var output = ProcessText(">>");
		output.Chapters.Single().Should().Be("SkipToNextDay(FirstBell);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(1);
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(28);
	}

	[Fact]
	public void HandleSkipToNextDay_OneArg()
	{
		var output = ProcessText(">>SecondBell");
		output.Chapters.Single().Should().Be("SkipToNextDay(SecondBell);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(2);
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(31);
	}

	[Fact]
	public void HandleSkipToNextDay_OneArg_NotUnit()
	{
		var notUnit = () => ProcessText(">>asdf");
		notUnit.Should().Throw<ScriptParserException>()
			.WithInnerException<KeyNotFoundException>();
	}

	[Fact]
	public void HandleSkipToNextDay_TooMany()
	{
		Action tooMany = () => ProcessText(">>1,2,3");
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleSkipToNextDay_TwoArgs()
	{
		var output = ProcessText(">>3,SecondBell");
		output.Chapters.Single().Should().Be("SkipToDaysAhead(3, SecondBell);");
		output.ScriptConverter.Time.CurrentUnit.Should().Be(2);
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(79);
	}

	[Fact]
	public void HandleSkipToNextDay_TwoArgs_NotNumber()
	{
		var notNumber = () => ProcessText(">>A,SecondBell");
		notNumber.Should().Throw<ScriptParserException>()
			.WithInnerException<FormatException>();
	}

	[Fact]
	public void HandleSkipToNextDay_TwoArgs_NotUnit()
	{
		var notUnit = () => ProcessText(">>3,asdf");
		notUnit.Should().Throw<ScriptParserException>()
			.WithInnerException<KeyNotFoundException>();
	}

	[Fact]
	public void HandleTimeSkip_NotNumber()
	{
		Action notNumber = () => ProcessText(">>>1A");
		notNumber.Should().Throw<ScriptParserException>()
			.WithInnerException<FormatException>();
	}

	[Fact]
	public void HandleTimeSkip_Valid()
	{
		var output = ProcessText(
			"$T=Ferdinand,Myne",
			">>>1"
		);
		output.Chapters.Single().Should().Be(
			$"Add(Temple, Ferdinand, Myne);{NL}" +
			"TimeSkip(1);"
		);
		output.ScriptConverter.Time.CurrentTotalHours.Should().Be(24);
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			x.Count.Should().Be(2);
			x.Values[0].IsTimeSkip.Should().Be(true);
			x.Values[1].IsTimeSkip.Should().Be(false);
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
	public void HandleUpdate_Text()
	{
		// text is ignored
		var output = ProcessText("@asdf");
		output.Chapters.Single().Should().Be("Update();");
	}

	[Fact]
	public void HandleUpdate_Valid()
	{
		var output = ProcessText("@");
		output.Chapters.Single().Should().Be("Update();");
	}

	[Fact]
	public void ProcessLine_InvalidSymbol()
	{
		Action action = () => ProcessText("??not a valid symbol");
		action.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
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