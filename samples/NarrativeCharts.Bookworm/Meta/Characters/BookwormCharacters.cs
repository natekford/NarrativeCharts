using NarrativeCharts.Bookworm.Meta;
using NarrativeCharts.Models;

using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm;

// This class is split up into 5 separate files because whatever extension I use
// to alphabetize members does not like 150 properties in a single file.
public static partial class BookwormCharacters
{
	public static ImmutableDictionary<string, Character> Aliases { get; }
	public static ImmutableDictionary<Character, Hex> Colors { get; }

	static BookwormCharacters()
	{
		var properties = typeof(BookwormCharacters).GetMembers<Character>();
		Colors = properties.ToImmutableDictionary(
			keySelector: x => x.Value,
			elementSelector: x => x.Member.GetCustomAttribute<ColorAttribute>()!.Hex
		);
		Aliases = properties.GetAliases(x => x.Value);
	}

	[AttributeUsage(AttributeTargets.Property)]
	private class ColorAttribute : Attribute
	{
		public Hex Hex { get; }

		public ColorAttribute(string? hex)
		{
			Hex = hex is string s ? new(s) : Hex.Unknown;
		}
	}
}