using NarrativeCharts.Models;
using NarrativeCharts.Scripting;
using NarrativeCharts.Time;

using System.Reflection;

namespace NarrativeCharts.Bookworm;

public sealed class BookwormScriptConverter : ScriptConverter
{
	private static Dictionary<string, string> CharacterProperties { get; }
		= CreateDict(typeof(BookwormCharacters), (Character x) => x.Value);

	private static Dictionary<string, string> LocationProperties { get; }
		= CreateDict(typeof(BookwormLocations), (Location x) => x.Value);

	public BookwormScriptConverter(ScriptDefinitions definitions, IEnumerable<string> lines) : base(definitions, lines)
	{
	}

	public string Write()
	{
		var methods = new Dictionary<string, string>();
		for (var i = 0; i < Chapters.Count; ++i)
		{
			var name = $"Chapter_{i + 1:00}";
			var method =
$@"
	private void {name}()
	{{
{Chapters[i]}
	}}
";
			methods.Add($"{name}();", method);
		}

		// the string has bad formatting, but the ide will fix that
		return
$@"
using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormBell)};
using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormCharacters)};
using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormLocations)};

namespace {nameof(NarrativeCharts)}.{nameof(Bookworm)}.*;

public sealed class {ClassName} : {nameof(BookwormNarrativeChart)}
{{
	public {ClassName}({nameof(TimeTrackerWithUnits)} time) : base(time)
	{{
		Name = nameof({ClassName});
	}}

	protected override void {nameof(ProtectedCreate)}()
	{{
		{string.Join("\r\n\t\t", methods.Keys)}
		{nameof(Update)}();
	}}
	{string.Join("\t", methods.Values)}
}}
";
	}

	protected override IEnumerable<string> ToProperties(IEnumerable<Character> characters)
		=> characters.Select(x => CharacterProperties[x.Value]).OrderBy(x => x);

	protected override string ToProperty(Location location)
		=> LocationProperties[location.Value];

	private static Dictionary<string, string> CreateDict<T>(
		Type type,
		Func<T, string> getValue)
	{
		return type
			.GetProperties(BindingFlags.Static | BindingFlags.Public)
			.Where(x => x.PropertyType == typeof(T))
			.ToDictionary(x => getValue((T)x.GetValue(null)!), x => x.Name);
	}
}