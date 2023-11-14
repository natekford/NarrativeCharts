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
public abstract class ScriptConverter : ScriptParser
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
		base.HandleAddCharacterGroup(input);
		LineConverted = true;
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
					.Append($"{nameof(Time)}.{nameof(TimeTrackerUtils.AddHours)}(")
					.Append(args[0])
					.AppendLine(");")
					.AppendLine($"{nameof(Update)}();");
				break;
		}

		base.HandleAddHours(input);
		LineConverted = true;
	}

	protected override void HandleAddReturnableScene(string input)
	{
		// split only up to 2 strings
		// since 1 string is the name, and the other is the scene assignment
		// and the scene assignment itself contains the arg splitter
		var returnableScene = SplitArgs(input, 2);
		switch (returnableScene.Length)
		{
			case 2:
				// maybe deal with creating properties for these at some point
				var scene = SplitAssignment(returnableScene[1]);
				switch (scene.Length)
				{
					case 2:
						WriteScene(
							ToProperty(ParseLocation(scene[0])),
							ToProperties(ParseCharacters(scene[1]))
						);
						break;
				}
				break;
		}

		base.HandleAddReturnableScene(input);
		LineConverted = true;
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
					.Append($"{nameof(Jump)}(")
					.Append(args[0])
					.AppendLine(");");
				break;
		}

		base.HandleAddUnits(input);
		LineConverted = true;
	}

	protected override void HandleChapter(string input)
	{
		Chapters.Add(new StringBuilder()
			.Append("// ")
			.Append(Hour)
			.AppendLine(" hours")
			.Append($"{nameof(Event)}(\"")
			.Append(input)
			.AppendLine("\");")
		);

		base.HandleChapter(input);
		LineConverted = true;
	}

	protected override void HandleComment(string input)
	{
		Chapter
			.Append("//")
			.AppendLine(input);

		base.HandleComment(input);
		LineConverted = true;
	}

	protected override void HandleFreeze(string input)
	{
		Chapter
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", ToProperties(ParseCharacters(input)))
			.AppendLine(");");

		base.HandleFreeze(input);
		LineConverted = true;
	}

	protected override void HandleKill(string input)
	{
		Chapter
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", ToProperties(ParseCharacters(input)))
			.AppendLine(");");

		base.HandleKill(input);
		LineConverted = true;
	}

	protected override void HandleRemoveReturnableScene(string input)
	{
		foreach (var scene in SplitArgs(input))
		{
			foreach (var group in StoredScenes[scene].GroupBy(x => x.Value))
			{
				WriteScene(
					ToProperty(group.Key),
					ToProperties(group.Select(x => x.Key))
				);
			}
		}

		base.HandleRemoveReturnableScene(input);
		LineConverted = true;
	}

	protected override void HandleScene(string input)
	{
		var scene = SplitAssignment(input);
		switch (scene.Length)
		{
			case 2:
				WriteScene(
					ToProperty(ParseLocation(scene[0])),
					ToProperties(ParseCharacters(scene[1]))
				);
				break;
		}

		base.HandleScene(input);
		LineConverted = true;
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
					.Append($"{nameof(SkipToCurrentDay)}(")
					.Append(args[0])
					.AppendLine(");");
				break;
		}

		base.HandleSkipToCurrentDay(input);
		LineConverted = true;
	}

	protected override void HandleSkipToNextDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Chapter.AppendLine($"{nameof(SkipToNextDay)}(1);");
				break;

			case 1:
				Chapter
					.Append($"{nameof(SkipToNextDay)}(")
					.Append(args[0])
					.AppendLine(");");
				break;

			case 2:
				Chapter
					.Append($"{nameof(SkipToDaysAhead)}(")
					.Append(args[0])
					.Append(", ")
					.Append(args[1])
					.AppendLine(");");
				break;
		}

		base.HandleSkipToNextDay(input);
		LineConverted = true;
	}

	protected override void HandleTimeSkip(string input)
	{
		Chapter
			.Append($"{nameof(TimeSkip)}(")
			.Append(input)
			.AppendLine(");");

		base.HandleTimeSkip(input);
		LineConverted = true;
	}

	protected override void HandleTitle(string input)
	{
		ClassName = input.Replace(" ", "");

		base.HandleTitle(input);
		LineConverted = true;
	}

	protected override void HandleUpdate(string input)
	{
		Chapter.AppendLine($"{nameof(Update)}();");

		base.HandleUpdate(input);
		LineConverted = true;
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