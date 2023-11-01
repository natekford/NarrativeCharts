using NarrativeCharts.Models;

using System.Text;

namespace NarrativeCharts.Scripting;

public abstract class ScriptConverter : ScriptLoader
{
	public string ClassName { get; protected set; } = "";
	protected StringBuilder Chapter => Chapters[^1];
	protected List<StringBuilder> Chapters { get; } = [];

	protected ScriptConverter(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions, lines)
	{
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

		base.HandleChapter(input);
	}

	protected override void HandleComment(string input)
	{
		Chapter
			.Append("//")
			.AppendLine(input);

		base.HandleComment(input);
	}

	protected override void HandleFreeze(string input)
	{
		var characters = ToProperties(ParseCharacters(input));
		Chapter
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", characters)
			.AppendLine(");");

		base.HandleFreeze(input);
	}

	protected override void HandleKill(string input)
	{
		var characters = ToProperties(ParseCharacters(input));
		Chapter
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", characters)
			.AppendLine(");");

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

		base.HandleSkipToNextDay(input);
	}

	protected override void HandleTitle(string input)
	{
		ClassName = input.Replace(" ", "");

		base.HandleTitle(input);
	}

	protected override void HandleUpdate(string input)
	{
		Chapter.AppendLine($"{nameof(Update)}();");

		base.HandleUpdate(input);
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