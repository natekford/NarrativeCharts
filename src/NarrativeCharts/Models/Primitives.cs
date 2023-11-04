namespace NarrativeCharts.Models;

public readonly record struct Character(string Value);

public readonly record struct Location(string Value)
{
	public static Location Frozen { get; } = new($"{nameof(Frozen)}-f0b7cb9a-d1ef-4764-b03f-dd697b87849e");
}

public readonly record struct Hex(string Value)
{
	public static Hex Unknown { get; } = new("#000000");
};