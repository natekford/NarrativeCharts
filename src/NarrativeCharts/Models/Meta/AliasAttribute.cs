using System.Collections.Immutable;

namespace NarrativeCharts.Models.Meta;

/// <summary>
/// Indicates other valid names for a character.
/// </summary>
/// <param name="aliases"></param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class AliasAttribute(params string[] aliases) : Attribute
{
	/// <summary>
	/// The aliases to use.
	/// </summary>
	public ImmutableArray<string> Aliases { get; } = [.. aliases];
}