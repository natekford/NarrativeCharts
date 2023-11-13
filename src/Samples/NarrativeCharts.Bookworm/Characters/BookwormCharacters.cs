using NarrativeCharts.Models;

using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm;

// This class is split up into 5 separate files because whatever extension I use
// to alphabetize members does not like 150 properties in a single file.
public static partial class BookwormCharacters
{
	public static ImmutableDictionary<Character, Hex> Colors { get; }

	static BookwormCharacters()
	{
		Colors = typeof(BookwormCharacters)
			.GetProperties(BindingFlags.Public | BindingFlags.Static)
			.Where(x => x.PropertyType == typeof(Character))
			.ToImmutableDictionary(
				keySelector: x => (Character)x.GetValue(null)!,
				elementSelector: x => x.GetCustomAttribute<ColorAttribute>()!.Hex
			);
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