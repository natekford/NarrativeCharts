using NarrativeCharts.Models;

using System.Runtime.CompilerServices;
using System.Text;

namespace NarrativeCharts.Scripting;

/// <summary>
/// Converts a script to C# code.
/// </summary>
/// <remarks>
/// Most of the script->code conversion is done by overriding NarrativeChart
/// methods, and to prevent writing inner methods a stack has to be used.
/// E.G.calling "Jump();" outputs "Jump();" instead of "Update();Jump();"
/// Overriding the HandleX methods from ScriptParser prevents having to make
/// sure we're only at the top most methods but involves parsing strings twice.
/// </remarks>
public abstract class ScriptConverter : ScriptParser
{
	/// <summary>
	/// The class name to use when outputting.
	/// </summary>
	public string ClassName { get; protected set; } = "";
	/// <summary>
	/// Used to ensure we're only writing top level methods.
	/// </summary>
	protected Stack<string> CallStack { get; } = new();
	/// <summary>
	/// The current chapter.
	/// </summary>
	protected StringBuilder Chapter => Chapters[^1];
	/// <summary>
	/// The chapters of this script.
	/// </summary>
	protected List<StringBuilder> Chapters { get; } = [];
	/// <summary>
	/// Properties to use for stored scenes.
	/// </summary>
	protected Dictionary<IEnumerable<KeyValuePair<Character, Location>>, string> StoredSceneProperties { get; } = [];

	/// <summary>
	/// Creates an instance of <see cref="ScriptConverter" />.
	/// </summary>
	/// <param name="definitions"></param>
	/// <param name="lines"></param>
	protected ScriptConverter(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions, lines)
	{
	}

	#region ScriptParser methods

	/// <inheritdoc />
	protected override void HandleComment(string input)
	{
		Chapter.Append("//").AppendLine(input);
		base.HandleComment(input);
	}

	/// <inheritdoc />
	protected override void HandleTitle(string input)
	{
		ClassName = input.Replace(" ", "");
		base.HandleTitle(input);
	}

	#endregion ScriptParser methods

	#region NarrativeChart methods

	/// <inheritdoc />
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

	/// <inheritdoc />
	protected override void AddHours(float amount = 1)
	{
		DoThenWriteIfTopMethod(() => base.AddHours(amount), sb => sb
			.Append($"{nameof(AddHours)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override Dictionary<Character, Location> AddReturnable(Location location, IEnumerable<Character> characters)
	{
		// this is probably some of the ugliest code i've ever written
		Dictionary<Character, Location> dict = [];
		var property = $"StoredScene{StoredSceneProperties.Count + 1}";
		DoThenWriteIfTopMethod(() => dict = base.AddReturnable(location, characters), sb => sb
			.Append(property)
			.Append($" = {nameof(AddReturnable)}(")
			.Append(ToProperty(location))
			.Append(", ")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
		StoredSceneProperties.Add(dict, property);
		return dict;
	}

	/// <inheritdoc />
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

	/// <inheritdoc />
	protected override void Freeze(IEnumerable<Character> characters)
	{
		DoThenWriteIfTopMethod(() => base.Freeze(characters), sb => sb
			.Append($"{nameof(Freeze)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void Jump(int amount = 1)
	{
		DoThenWriteIfTopMethod(() => base.Jump(amount), sb => sb
			.Append($"{nameof(Jump)}(")
			.Append(amount)
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void Kill(IEnumerable<Character> characters)
	{
		DoThenWriteIfTopMethod(() => base.Kill(characters), sb => sb
			.Append($"{nameof(Kill)}(")
			.AppendJoin(", ", ToProperties(characters))
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void Return(IEnumerable<KeyValuePair<Character, Location>> points)
	{
		DoThenWriteIfTopMethod(() => base.Return(points), sb => sb
			.Append($"{nameof(Return)}(")
			.Append(StoredSceneProperties[points])
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void SkipToCurrentDay(int unit)
	{
		DoThenWriteIfTopMethod(() => base.SkipToCurrentDay(unit), sb => sb
			.Append($"{nameof(SkipToCurrentDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
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

	/// <inheritdoc />
	protected override void SkipToNextDay(int unit)
	{
		DoThenWriteIfTopMethod(() => base.SkipToNextDay(unit), sb => sb
			.Append($"{nameof(SkipToNextDay)}(")
			.Append(ToUnitName(unit))
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void TimeSkip(int days)
	{
		DoThenWriteIfTopMethod(() => base.TimeSkip(days), sb => sb
			.Append($"{nameof(TimeSkip)}(")
			.Append(days)
			.AppendLine(");")
		);
	}

	/// <inheritdoc />
	protected override void Update()
	{
		DoThenWriteIfTopMethod(base.Update, sb => sb
			.AppendLine($"{nameof(Update)}();")
		);
	}

	#endregion NarrativeChart methods

	/// <summary>
	/// Invokes <paramref name="then"/>, and if this is a top level method adds
	/// the invocation to the current output.
	/// </summary>
	/// <param name="then"></param>
	/// <param name="modifyChapter"></param>
	/// <param name="name"></param>
	/// <exception cref="InvalidOperationException"></exception>
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

	/// <summary>
	/// Converts <paramref name="characters"/> to their respective properties.
	/// </summary>
	/// <param name="characters"></param>
	/// <returns></returns>
	protected abstract IEnumerable<string> ToProperties(IEnumerable<Character> characters);

	/// <summary>
	/// Converts <paramref name="location"/> to their respective properties.
	/// </summary>
	/// <param name="location"></param>
	/// <returns></returns>
	protected abstract string ToProperty(Location location);

	/// <summary>
	/// Converts <paramref name="unit"/> to a name.
	/// </summary>
	/// <param name="unit"></param>
	/// <returns></returns>
	protected abstract string ToUnitName(int unit);
}