using NarrativeCharts.Models;

using System.Runtime.CompilerServices;
using System.Text;

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
	protected Dictionary<IEnumerable<KeyValuePair<Character, Location>>, string> StoredSceneProperties { get; } = [];

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
		DoThenWriteIfTopMethod(() => base.Add(location, characters), sb => sb
			.Append($"{nameof(Add)}(")
			.Append(ToProperty(location))
			.Append(", ")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	protected override void AddHours(int amount = 1)
	{
		DoThenWriteIfTopMethod(() => base.AddHours(amount), sb => sb
			.Append($"{nameof(AddHours)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	protected override Dictionary<Character, Location> AddR(Location location, IEnumerable<Character> characters)
	{
		// this is probably some of the ugliest code i've ever written
		Dictionary<Character, Location> dict = [];
		var property = $"StoredScene{StoredSceneProperties.Count + 1}";
		DoThenWriteIfTopMethod(() => dict = base.AddR(location, characters), sb => sb
			.Append(property)
			.Append($" = {nameof(AddR)}(")
			.Append(ToProperty(location))
			.Append(", ")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
		StoredSceneProperties.Add(dict, property);
		return dict;
	}

	protected override void Event(string name)
	{
		Chapters.Add(new());
		DoThenWriteIfTopMethod(() => base.Event(name), sb => sb
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
		DoThenWriteIfTopMethod(() => base.Freeze(characters), sb => sb
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	protected override void Jump(int amount = 1)
	{
		DoThenWriteIfTopMethod(() => base.Jump(amount), sb => sb
			.Append($"{nameof(Jump)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	protected override void Kill(IEnumerable<Character> characters)
	{
		DoThenWriteIfTopMethod(() => base.Kill(characters), sb => sb
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	protected override void Return(IEnumerable<KeyValuePair<Character, Location>> points)
	{
		DoThenWriteIfTopMethod(() => base.Return(points), sb => sb
			.Append($"{nameof(Return)}(")
			.Append(StoredSceneProperties[points])
			.AppendLine(");")
		);
	}

	protected override void SkipToCurrentDay(int unit)
	{
		DoThenWriteIfTopMethod(() => base.SkipToCurrentDay(unit), sb => sb
			.Append($"{nameof(SkipToCurrentDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void SkipToDaysAhead(int days, int unit)
	{
		DoThenWriteIfTopMethod(() => base.SkipToDaysAhead(days, unit), sb => sb
			.Append($"{nameof(SkipToDaysAhead)}(")
			.Append(days)
			.Append(", ")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void SkipToNextDay(int unit)
	{
		DoThenWriteIfTopMethod(() => base.SkipToNextDay(unit), sb => sb
			.Append($"{nameof(SkipToNextDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	protected override void TimeSkip(int days)
	{
		DoThenWriteIfTopMethod(() => base.TimeSkip(days), sb => sb
			.Append($"{nameof(TimeSkip)}(")
			.Append(days)
			.AppendLine(");")
		);
	}

	protected override void Update()
	{
		DoThenWriteIfTopMethod(base.Update, sb => sb
			.AppendLine($"{nameof(Update)}();")
		);
	}

	#endregion NarrativeChart methods

	protected virtual void DoThenWriteIfTopMethod(
		Action then,
		Action<StringBuilder> modifyChapter,
		[CallerMemberName] string name = "")
	{
		CallStack.Push(name);
		then.Invoke();
		if (CallStack.Pop() != name)
		{
			throw new InvalidOperationException("Unexpected value in method call stack while converting.");
		}
		if (CallStack.Count == 0)
		{
			modifyChapter.Invoke(Chapter);
		}
	}

	protected abstract IEnumerable<string> ToProperties(IEnumerable<Character> characters);

	protected abstract string ToProperty(Location location);

	protected abstract string ToUnitName(int unit);
}