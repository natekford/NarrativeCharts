using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System;
using System.Runtime.CompilerServices;
using System.Text;

using static System.Formats.Asn1.AsnWriter;

namespace NarrativeCharts.Scripting;

/* Most of the script->code conversion is done by overriding NarrativeChart
 * methods, and to prevent writing inner methods a stack has to be used.
 * E.G. calling "Jump();" outputs "Jump();" instead of "Update();Jump();"
 * Overriding the HandleX methods from ScriptParser prevents having to make
 * sure we're only at the top most methods but involves parsing strings twice.
 */

public abstract class ScriptConverter : ScriptParser
{
	public string ClassName { get; protected set; } = "";
	// use a stack to ensure we're only writing the top most methods
	protected Stack<string> CallStack { get; } = new();
	protected StringBuilder Chapter => Chapters[^1];
	protected List<StringBuilder> Chapters { get; } = [];

	protected ScriptConverter(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions, lines)
	{
	}

	#region ScriptParser methods

	protected override void HandleComment(string input)
	{
		Chapter.Append("//").AppendLine(input);
		base.HandleComment(input);
	}

	protected override void HandleTitle(string input)
	{
		ClassName = input.Replace(" ", "");
		base.HandleTitle(input);
	}

	#endregion ScriptParser methods

	#region NarrativeChart methods

	protected override void Add(Location location, IEnumerable<Character> characters)
	{
		Write(() => base.Add(location, characters), sb =>
			WriteScene(sb, location, characters)
		);
	}

	protected override void AddHours(int amount = 1)
	{
		Write(() => base.AddHours(amount), sb => sb
			.Append($"{nameof(AddHours)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	protected override void Event(string name)
	{
		Chapters.Add(new());
		Write(() => base.Event(name), sb => sb
			.Append("// ")
			.Append(Hour)
			.AppendLine(" hours")
			.Append($"{nameof(Event)}(\"")
			.Append(name)
			.AppendLine("\");")
		);
	}

	protected override void Freeze(IEnumerable<Character> characters)
	{
		Write(() => base.Freeze(characters), sb => sb
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	protected override void Jump(int amount = 1)
	{
		Write(() => base.Jump(amount), sb => sb
			.Append($"{nameof(Jump)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	protected override void Kill(IEnumerable<Character> characters)
	{
		Write(() => base.Kill(characters), sb => sb
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	protected override void Return(IEnumerable<KeyValuePair<Character, Location>> points)
	{
		Write(() => base.Return(points), sb =>
		{
			foreach (var group in points.GroupBy(x => x.Value))
			{
				WriteScene(sb, group.Key, group.Select(x => x.Key));
			}
		});
	}

	protected override void SkipToCurrentDay(int unit)
	{
		Write(() => base.SkipToCurrentDay(unit), sb => sb
			.Append($"{nameof(SkipToCurrentDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void SkipToDaysAhead(int days, int unit)
	{
		Write(() => base.SkipToDaysAhead(days, unit), sb => sb
			.Append($"{nameof(SkipToDaysAhead)}(")
			.Append(days)
			.Append(", ")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void SkipToNextDay(int unit)
	{
		Write(() => base.SkipToNextDay(unit), sb => sb
			.Append($"{nameof(SkipToNextDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void TimeSkip(int days)
	{
		Write(() => base.TimeSkip(days), sb => sb
			.Append($"{nameof(TimeSkip)}(")
			.Append(days)
			.AppendLine(");")
		);
	}

	protected override void Update()
	{
		Write(base.Update, sb => sb
			.AppendLine($"{nameof(Update)}();")
		);
	}

	#endregion NarrativeChart methods

	protected abstract IEnumerable<string> ToProperties(IEnumerable<Character> characters);

	protected abstract string ToProperty(Location location);

	protected abstract string ToUnitName(int unit);

	protected virtual void Write(
		Action then,
		Action<StringBuilder> modifyChapter,
		[CallerMemberName] string name = "")
	{
		CallStack.Push(name);
		then.Invoke();
		if (CallStack.Pop() != name)
		{
			throw new InvalidOperationException("Unexpected value in the method call stack while converting.");
		}
		if (CallStack.Count == 0)
		{
			modifyChapter.Invoke(Chapter);
		}
	}

	protected virtual void WriteScene(StringBuilder sb, Location location, IEnumerable<Character> characters)
	{
		sb
			.Append($"{nameof(Add)}(")
			.Append(ToProperty(location))
			.Append(", ")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");");
	}
}