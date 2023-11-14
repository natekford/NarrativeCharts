using NarrativeCharts.Models;
using NarrativeCharts.Scripting;
using NarrativeCharts.Time;

using System.Reflection;
using System.Text;

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
		var nl = Environment.NewLine;

		var methods = new Dictionary<string, string>();
		for (var i = 0; i < Chapters.Count; ++i)
		{
			var name = $"Chapter_{i + 1:00}";
			var method =
$@"
	private void {name}()
	{{
		{Chapters[i].Replace(nl, $"{nl}\t\t").TrimEnd()}
	}}";
			methods.Add($"{name}();", method);
		}

		var properties = new List<string>();
		foreach (var (_, property) in StoredSceneProperties)
		{
			const string TYPE = $"{nameof(Dictionary<int, int>)}<{nameof(Character)}" +
				$", {nameof(Location)}>";
			properties.Add($"private {TYPE} {property} {{ get; set; }} = null!;");
		}

		// god this is so ugly
		return
$@"using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormBell)};
using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormCharacters)};
using static {nameof(NarrativeCharts)}.{nameof(Bookworm)}.{nameof(BookwormLocations)};

namespace {nameof(NarrativeCharts)}.{nameof(Bookworm)}.*;

public sealed class {ClassName} : {nameof(BookwormNarrativeChart)}
{{
	{string.Join($"{nl}\t", properties)}

	public {ClassName}({nameof(TimeTrackerWithUnits)} time) : base(time)
	{{
		Name = nameof({ClassName});
	}}

	protected override void {nameof(ProtectedCreate)}()
	{{
		{string.Join($"{nl}\t\t", methods.Keys)}
		{nameof(Update)}();
	}}
	{string.Join($"{nl}\t", methods.Values)}
}}";
	}

	protected override IEnumerable<string> ToProperties(IEnumerable<Character> characters)
		=> characters.Select(x => CharacterProperties[x.Value]).OrderBy(x => x);

	protected override string ToProperty(Location location)
		=> LocationProperties[location.Value];

	protected override string ToUnitName(int unit)
		=> ((BookwormBell)unit).ToString();

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

internal static class BookwormScriptConverterUtils
{
	// copied from https://stackoverflow.com/a/24769702
	public static StringBuilder? TrimEnd(this StringBuilder sb)
	{
		if (sb == null || sb.Length == 0)
		{
			return sb;
		}

		var i = sb.Length - 1;

		for (; i >= 0; i--)
		{
			if (!char.IsWhiteSpace(sb[i]))
			{
				break;
			}
		}

		if (i < sb.Length - 1)
		{
			sb.Length = i + 1;
		}

		return sb;
	}
}