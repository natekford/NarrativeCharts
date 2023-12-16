using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Models.Meta;

/// <summary>
/// Utilities for meta information on models.
/// </summary>
public static class MetaUtils
{
	/// <summary>
	/// Gets aliases that are associated with each item.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="properties"></param>
	/// <param name="convertValue"></param>
	/// <returns></returns>
	public static ImmutableDictionary<string, T> GetAliases<T>(
		this IEnumerable<(MemberInfo Member, T Value)> properties,
		Func<T, string>? convertValue)
	{
		var aliases = new Dictionary<string, T>();
		foreach (var (prop, value) in properties)
		{
			aliases.Add(prop.Name, value);
			if (convertValue is not null)
			{
				aliases.TryAdd(convertValue(value), value);
			}

			var cAliases = prop.GetCustomAttribute<AliasAttribute>()?.Aliases
				?? ImmutableArray<string>.Empty;
			foreach (var alias in cAliases)
			{
				aliases.Add(alias, value);
			}
		}
		return aliases.ToImmutableDictionary();
	}

	/// <summary>
	/// Gets colors that are associated with each character.
	/// </summary>
	/// <param name="properties"></param>
	/// <returns></returns>
	public static ImmutableDictionary<Character, Hex> GetColors(
		this IEnumerable<(MemberInfo Member, Character Value)> properties)
	{
		return properties.ToImmutableDictionary(
			x => x.Value,
			x => x.Member.GetCustomAttribute<ColorAttribute>()?.Hex ?? Hex.Unknown
		);
	}

	/// <summary>
	/// Gets all public+static properties and fields that are <typeparamref name="T"/>
	/// from <paramref name="type"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="type"></param>
	/// <returns></returns>
	public static IEnumerable<(MemberInfo Member, T Value)> GetMembers<T>(Type type)
	{
		const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.Static;
		var properties = type.GetProperties(FLAGS)
			.Where(x => x.PropertyType == typeof(T))
			.Select(x => (Member: (MemberInfo)x, Value: (T)x.GetValue(null)!));
		var fields = type.GetFields(FLAGS)
			.Where(x => x.FieldType == typeof(T))
			.Select(x => (Member: (MemberInfo)x, Value: (T)x.GetValue(null)!));
		return fields.Concat(properties);
	}
}