using NarrativeCharts.Models;
using NarrativeCharts.Models.Meta;

using System.Collections.Immutable;

namespace NarrativeCharts.Bookworm;

// This class is split up into 5 separate files because whatever extension I use
// to alphabetize members does not like 150 properties in a single file.
public static partial class BookwormCharacters
{
	public static ImmutableDictionary<string, Character> Aliases { get; }
	public static ImmutableDictionary<Character, Hex> Colors { get; }

	static BookwormCharacters()
	{
		var properties = MetaUtils.GetMembers<Character>(typeof(BookwormCharacters));
		Colors = properties.GetColors();
		Aliases = properties.GetAliases(x => x.Value);
	}
}