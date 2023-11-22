namespace NarrativeCharts.Models;

/// <summary>
/// Specifies that a <see cref="string"/> is a <see cref="Character"/>.
/// </summary>
/// <param name="Value"></param>
public readonly record struct Character(string Value);

/// <summary>
/// Specifies that a <see cref="string"/> is a <see cref="Location"/>.
/// </summary>
/// <param name="Value"></param>
public readonly record struct Location(string Value)
{
	/// <summary>
	/// A location to use when keeping a character from moving.
	/// </summary>
	public static Location Frozen { get; } = new($"{nameof(Frozen)}-f0b7cb9a-d1ef-4764-b03f-dd697b87849e");
}

/// <summary>
/// Specifies that a <see cref="string"/> is a <see cref="Hex"/>.
/// </summary>
/// <param name="Value"></param>
public readonly record struct Hex(string Value)
{
	/// <summary>
	/// The default value to use for an unknown value.
	/// </summary>
	public static Hex Unknown { get; } = new("#000000");
};