using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System.Text;

namespace NarrativeCharts.Scripting;

// pretty much all parsing is done twice because the methods in ScriptLoader
// deal with the raw strings. I could make it so there are ScriptLoader methods
// that deal with the already parsed time/characters/locations, but that'd
// add 20+ methods which would make ScriptLoader seem even more bloated
// and make it more confusing when it comes to what methods have to be
// overriden to affect functionality.
// worrying about the efficiency of this class also seems worthless when
// image processing takes 100x longer and has much bigger allocations.
public abstract class ScriptConverter : ScriptLoader
{
	public string ClassName { get; protected set; } = "";
	protected StringBuilder Chapter => Chapters[^1];
	protected List<StringBuilder> Chapters { get; } = [];
	protected bool LineConverted { get; set; }

	protected ScriptConverter(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions, lines)
	{
	}

	protected override void HandleAddCharacterGroup(string input)
	{
		// no code equivalent

		LineConverted = true;
		base.HandleAddCharacterGroup(input);
	}

	protected override void HandleAddHours(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Chapter
					.AppendLine($"{nameof(Time)}.{nameof(TimeTrackerUtils.AddHour)}();")
					.AppendLine($"{nameof(Update)}();");
				break;

			case 1:
				Chapter
					.Append($"{nameof(Time)}.{nameof(TimeTrackerUtils.AddHours)}")
					.Append('(')
					.Append(args[0])
					.AppendLine(");")
					.AppendLine($"{nameof(Update)}();");
				break;
		}

		LineConverted = true;
		base.HandleAddHours(input);
	}

	protected override void HandleAddReturnableScene(string input)
	{
		// no code equivalent

		LineConverted = true;
		base.HandleAddReturnableScene(input);
	}

	protected override void HandleAddUnits(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Chapter.AppendLine($"{nameof(Jump)}();");
				break;

			case 1:
				Chapter
					.Append(nameof(Jump))
					.Append('(')
					.Append(args[0])
					.AppendLine(");");
				break;
		}

		LineConverted = true;
		base.HandleAddUnits(input);
	}

	protected override void HandleChapter(string input)
	{
		var sb = new StringBuilder()
			.Append(nameof(Event))
			.Append("(\"")
			.Append(input)
			.AppendLine("\");");
		Chapters.Add(sb);

		LineConverted = true;
		base.HandleChapter(input);
	}

	protected override void HandleComment(string input)
	{
		Chapter
			.Append("//")
			.AppendLine(input);

		LineConverted = true;
		base.HandleComment(input);
	}

	protected override void HandleFreeze(string input)
	{
		var characters = ToProperties(ParseCharacters(input));
		Chapter
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", characters)
			.AppendLine(");");

		LineConverted = true;
		base.HandleFreeze(input);
	}

	protected override void HandleKill(string input)
	{
		var characters = ToProperties(ParseCharacters(input));
		Chapter
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", characters)
			.AppendLine(");");

		LineConverted = true;
		base.HandleKill(input);
	}

	protected override void HandleRemoveReturnableScene(string input)
	{
		var scene = StoredScenes[input];
		foreach (var group in scene.GroupBy(x => x.Value))
		{
			WriteScene(
				ToProperty(group.Key),
				ToProperties(group.Select(x => x.Key))
			);
		}

		LineConverted = true;
		base.HandleRemoveReturnableScene(input);
	}

	protected override void HandleScene(string input)
	{
		var args = SplitAssignment(input);
		switch (args.Length)
		{
			case 2:
				WriteScene(
					ToProperty(ParseLocation(args[0])),
					ToProperties(ParseCharacters(args[1]))
				);
				break;
		}

		LineConverted = true;
		base.HandleScene(input);
	}

	protected override void HandleSkipToCurrentDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Chapter.AppendLine($"{nameof(Jump)}();");
				break;

			case 1:
				Chapter
					.Append(nameof(SkipToCurrentDay))
					.Append('(')
					.Append(args[0])
					.AppendLine(");");
				break;
		}

		LineConverted = true;
		base.HandleSkipToCurrentDay(input);
	}

	protected override void HandleSkipToNextDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Chapter
					.Append(nameof(SkipToNextDay))
					.Append('(')
					.Append(1)
					.AppendLine(");");
				break;

			case 1:
				Chapter
					.Append(nameof(SkipToNextDay))
					.Append('(')
					.Append(args[0])
					.AppendLine(");");
				break;

			case 2:
				Chapter
					.Append(nameof(SkipToDaysAhead))
					.Append('(')
					.Append(args[0])
					.Append(", ")
					.Append(args[1])
					.AppendLine(");");
				break;
		}

		LineConverted = true;
		base.HandleSkipToNextDay(input);
	}

	protected override void HandleTimeSkip(string input)
	{
		Chapter
			.Append(nameof(TimeSkip))
			.Append('(')
			.Append(input)
			.AppendLine(");");

		LineConverted = true;
		base.HandleTimeSkip(input);
	}

	protected override void HandleTitle(string input)
	{
		ClassName = input.Replace(" ", "");

		LineConverted = true;
		base.HandleTitle(input);
	}

	protected override void HandleUpdate(string input)
	{
		Chapter.AppendLine($"{nameof(Update)}();");

		LineConverted = true;
		base.HandleUpdate(input);
	}

	protected override void ProcessLine(string line)
	{
		LineConverted = false;
		base.ProcessLine(line);

		if (!LineConverted)
		{
			throw new InvalidOperationException("Line not converted from script to class.");
		}
	}

	protected abstract IEnumerable<string> ToProperties(IEnumerable<Character> characters);

	protected abstract string ToProperty(Location location);

	protected virtual void WriteScene(string location, IEnumerable<string> characters)
	{
		Chapter
			.Append(nameof(Add))
			.Append('(')
			.Append(location)
			.Append(", ")
			.AppendJoin(", ", characters)
			.AppendLine(");");
	}
}