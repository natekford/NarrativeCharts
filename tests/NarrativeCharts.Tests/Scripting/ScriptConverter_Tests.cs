using NarrativeCharts.Models;
using NarrativeCharts.Scripting;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.Meta.Locations.BookwormLocations;

namespace NarrativeCharts.Tests.Scripting;

/// <summary>
/// These tests cover ScriptConverter, ScriptParser, and NarrativeChart because of
/// how tightly coupled they are.
/// </summary>
public class ScriptConverter_Tests
{
	private static string NL { get; } = Environment.NewLine;

	[Fact]
	public void EnsureNameNotUsed_Character()
	{
		var output = ProcessText("@");

		Action inUse = () => output.ScriptConverter.EnsureNameNotUsed("Myne");
		inUse.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void EnsureNameNotUsed_Group()
	{
		var output = ProcessText("@");
		output.ScriptConverter.CharacterGroups.Add("Name1", []);

		Action inUse = () => output.ScriptConverter.EnsureNameNotUsed("Name1");
		inUse.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void EnsureNameNotUsed_Scene()
	{
		var output = ProcessText("@");
		output.ScriptConverter.StoredScenes.Add("Name1", []);

		Action inUse = () => output.ScriptConverter.EnsureNameNotUsed("Name1");
		inUse.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void EnsureNameNotUsed_Valid()
	{
		var output = ProcessText("@");
		output.ScriptConverter.CharacterGroups.Add("Name1", []);
		output.ScriptConverter.StoredScenes.Add("Name2", []);
		output.ScriptConverter.Definitions.CharacterAliases.Add("Name3", new());

		output.ScriptConverter.EnsureNameNotUsed("Name4");
	}

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
		Action tooFew = () => ProcessText(
			"$T=Ferdinand,Myne",
			"+$1=C=Ferdinand=Myne"
		);
		tooFew.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddReturnableScene_TooFew2()
	{
		Action tooFew = () => ProcessText(
			"$T=Ferdinand,Myne",
			"+$1,C,Ferdinand,Myne"
		);
		tooFew.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddReturnableScene_TooMany()
	{
		Action tooMany = () => ProcessText(
			"$T=Ferdinand,Myne",
			"+$1,C=Ferdinand=Myne"
		);
		tooMany.Should().Throw<ScriptParserException>()
			.WithInnerException<ArgumentException>();
	}

	[Fact]
	public void HandleAddReturnableScene_Valid()
	{
		var output = ProcessText(
			"$T=Ferdinand,Myne",
			">H1",
			"+$1,C=Ferdinand,Myne"
		);
		output.Chapters.Single().Should().Be(
			$"Add(Temple, Ferdinand, Myne);{NL}" +
			$"AddHours(1f);{NL}" +
			"StoredScene1 = AddReturnable(EhrenfestCastle, Ferdinand, Myne);"
		);
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			Ferdinand,
			Myne
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			x.Values.Should().HaveCount(2);
			x.Values[0].Hour.Should().Be(0);
			x.Values[0].Location.Should().Be(Temple);
			x.Values[1].Hour.Should().Be(1);
			x.Values[1].Location.Should().Be(EhrCastle);
		});

		var storedScene = output.ScriptConverter.StoredScenes.Single();
		storedScene.Key.Should().Be("1");
		storedScene.Value.Should().BeEquivalentTo(new Dictionary<Character, Location>
		{
			[Ferdinand] = Temple,
			[Myne] = Temple,
		});

		var storedSceneProperty = output.ScriptConverter.StoredSceneProperties.Single();
		storedSceneProperty.Key.Should().BeSameAs(storedScene.Value);
		storedSceneProperty.Value.Should().Be("StoredScene1");
	}

	[Fact]
	public void HandleAddScene_LimitedCharacters()
	{
		var defs = TestUtils.Defs;
		defs.OnlyDrawTheseCharacters.Add(Myne);
		var output = ProcessText(["$T=Ferdinand,Myne"], defs);
		output.Chapters.Single().Should().Be("Add(Temple, Myne);");
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
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
		output.Chapters.Should().HaveCount(2);
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
			x => x.Should().BeEmpty()
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
	public void HandleRemoveRemovableScene_Valid()
	{
		var output = ProcessText(
			"$T=Ferdinand,Myne",
			">H1",
			"+$1,C=Ferdinand,Myne",
			">H1",
			"$KO=1",
			">H1",
			"-$1"
		);
		output.Chapters.Single().Should().Be(
			$"Add(Temple, Ferdinand, Myne);{NL}" +
			$"AddHours(1f);{NL}" +
			$"StoredScene1 = AddReturnable(EhrenfestCastle, Ferdinand, Myne);{NL}" +
			$"AddHours(1f);{NL}" +
			$"Add(KnightsOrder, Ferdinand, Myne);{NL}" +
			$"AddHours(1f);{NL}" +
			"Return(StoredScene1);"
		);
		output.ScriptConverter.Points.Keys.Should().BeEquivalentTo(new[]
		{
			Ferdinand,
			Myne
		});
		output.ScriptConverter.Points.Values.Should().AllSatisfy(x =>
		{
			x.Values.Should().HaveCount(4);
			x.Values[0].Hour.Should().Be(0);
			x.Values[0].Location.Should().Be(Temple);
			x.Values[1].Hour.Should().Be(1);
			x.Values[1].Location.Should().Be(EhrCastle);
			x.Values[2].Hour.Should().Be(2);
			x.Values[2].Location.Should().Be(KnightsOrder);
			x.Values[3].Hour.Should().Be(3);
			x.Values[3].Location.Should().Be(Temple);
		});

		output.ScriptConverter.StoredScenes.Should().BeEmpty();

		var storedSceneProperty = output.ScriptConverter.StoredSceneProperties.Single();
		storedSceneProperty.Key.Should().BeEquivalentTo(new Dictionary<Character, Location>
		{
			[Ferdinand] = Temple,
			[Myne] = Temple,
		});
		storedSceneProperty.Value.Should().Be("StoredScene1");
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
			x.Should().HaveCount(2);
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
		=> ProcessText(lines, TestUtils.Defs);

	private static Output ProcessText(string[] lines, ScriptDefinitions defs)
	{
		var converter = new ScriptConverter(
			definitions: defs,
			lastWriteTimeUtc: DateTime.UtcNow,
			lines: lines
		);
		converter.Initialize(null);

		var chapters = converter.Chapters.ConvertAll(x => x.TrimEnd().ToString());
		foreach (var chapter in chapters)
		{
			TestUtils.ValidateSyntax(chapter);
		}
		TestUtils.ValidateSyntax(converter.Write());

		return new(converter, chapters);
	}

	private record Output(ScriptConverter ScriptConverter, List<string> Chapters);
}