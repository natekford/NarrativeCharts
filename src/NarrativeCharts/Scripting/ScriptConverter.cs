using NarrativeCharts.Models;
using NarrativeCharts.Time;

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
public class ScriptConverter : ScriptParser
{
	/// <summary>
	/// The class name to use when outputting.
	/// </summary>
	public string ClassName { get; protected set; } = "";
	/// <summary>
	/// Used to ensure we're only writing top level methods.
	/// </summary>
	protected internal Stack<string> CallStack { get; } = new();
	/// <summary>
	/// The current chapter.
	/// </summary>
	protected internal StringBuilder Chapter => Chapters[^1];
	/// <summary>
	/// The chapters of this script.
	/// </summary>
	protected internal List<StringBuilder> Chapters { get; } = [new()];
	/// <summary>
	/// Properties to use for stored scenes.
	/// </summary>
	protected internal Dictionary<IEnumerable<KeyValuePair<Character, Location>>, string> StoredSceneProperties { get; } = [];

	/// <summary>
	/// Creates an instance of <see cref="ScriptConverter" />.
	/// </summary>
	/// <param name="definitions"></param>
	/// <param name="lastWriteTimeUtc"></param>
	/// <param name="lines"></param>
	public ScriptConverter(
		ScriptDefinitions definitions,
		DateTime lastWriteTimeUtc,
		IEnumerable<string> lines)
		: base(definitions, lastWriteTimeUtc, lines)
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
		ClassName = ScriptConverterUtils.ToValidProperty(input);
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
			.AppendLine("f);")
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
		if (Chapter.Length > 0)
		{
			Chapters.Add(new());
		}
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
	/// Creates a string of the autogenerated output.
	/// </summary>
	/// <returns></returns>
	public virtual string Write()
	{
		var nl = Environment.NewLine;

		var c = 1;
		var methods = new Dictionary<string, string>();
		foreach (var chapter in Chapters)
		{
			var name = $"Chapter_{c:00}";
			var method =
$@"
	private void {name}()
	{{
		{chapter.Replace(nl, $"{nl}\t\t").TrimEnd()}
	}}";
			methods.Add($"{name}();", method);
			++c;
		}

		var properties = new StringBuilder();
		foreach (var (_, property) in StoredSceneProperties)
		{
			properties
				.Append("\tprivate ")
				.AppendType<Dictionary<Character, Location>>()
				.Append(' ')
				.Append(property)
				.AppendLine(" { get; set; } = null!;");
		}

		// god this is so ugly
		return
$@"using static {ScriptConverterUtils.NAMESPACE}.{ScriptConverterUtils.CHARACTER_CLASS};
using static {ScriptConverterUtils.NAMESPACE}.{ScriptConverterUtils.LOCATION_CLASS};
using static {ScriptConverterUtils.NAMESPACE}.{ScriptConverterUtils.TIME_CLASS};

namespace {ScriptConverterUtils.NAMESPACE};

public sealed class {ClassName} : {nameof(NarrativeChartWithUnits<int>)}<{ScriptConverterUtils.TIME_CLASS}>
{{
{properties}
	public {ClassName}({nameof(TimeTrackerWithUnits)} time) : base(time)
	{{
		{nameof(Name)} = nameof({ClassName});
		{nameof(Colors)} = new({ScriptConverterUtils.CHARACTER_CLASS}.{ScriptConverterUtils.COLORS});
		{nameof(YIndexes)} = new({ScriptConverterUtils.LOCATION_CLASS}.{ScriptConverterUtils.YINDEXES});
	}}

	protected override void {nameof(AddNarrativeData)}()
	{{
		{string.Join($"{nl}\t\t", methods.Keys)}
		{nameof(Update)}();
	}}
	{string.Join($"{nl}\t", methods.Values)}

	protected override int {nameof(Convert)}({ScriptConverterUtils.TIME_CLASS} unit)
		=> (int)unit;
}}";
	}

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
	protected virtual IEnumerable<string> ToProperties(IEnumerable<Character> characters)
	{
		foreach (var character in characters)
		{
			yield return ScriptConverterUtils.ToValidProperty(character.Value);
		}
	}

	/// <summary>
	/// Converts <paramref name="location"/> to their respective properties.
	/// </summary>
	/// <param name="location"></param>
	/// <returns></returns>
	protected virtual string ToProperty(Location location)
		=> ScriptConverterUtils.ToValidProperty(location.Value);

	/// <summary>
	/// Converts <paramref name="unit"/> to a name.
	/// </summary>
	/// <param name="unit"></param>
	/// <returns></returns>
	protected virtual string ToUnitName(int unit)
	{
		foreach (var (key, value) in Definitions.TimeUnitAliases)
		{
			if (value != unit)
			{
				continue;
			}

			var property = ScriptConverterUtils.ToValidProperty(key);
			if (property.Length != 0)
			{
				return property;
			}
		}
		throw new ArgumentException($"Unable to find a suitable unit name for '{unit}'", nameof(unit));
	}
}