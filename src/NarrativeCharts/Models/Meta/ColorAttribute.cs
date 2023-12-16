namespace NarrativeCharts.Models.Meta;

/// <summary>
/// Indicates a color to use for a character.
/// </summary>
/// <param name="hex"></param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ColorAttribute(string? hex) : Attribute
{
	/// <summary>
	/// The color to use.
	/// </summary>
	public Hex Hex { get; } = hex is string s ? new(s) : Hex.Unknown;
}