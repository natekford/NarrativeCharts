namespace NarrativeCharts.Models;

public readonly record struct Character(string Value);
public readonly record struct Location(string Value);
public readonly record struct Hex(string Value)
{
	public static Hex Unknown { get; } = new("#000000");
};
public readonly record struct Point(int Hour, Location Location);