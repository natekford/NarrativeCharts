using System.Collections.Immutable;
using System.Reflection;

namespace NarrativeCharts.Bookworm.Meta;

internal static class MetaUtils
{
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

	public static IEnumerable<(MemberInfo Member, T Value)> GetMembers<T>(this Type type)
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

[AttributeUsage(AttributeTargets.Property)]
internal class AliasAttribute : Attribute
{
	public ImmutableArray<string> Aliases { get; }

	public AliasAttribute(params string[] aliases)
	{
		Aliases = aliases.ToImmutableArray();
	}
}