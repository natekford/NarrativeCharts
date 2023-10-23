namespace NarrativeCharts.Models;

public readonly record struct Character(string Value);
public readonly record struct Location(string Value);

public readonly record struct Hex(string Value);

public readonly record struct X(int Value);
public readonly record struct Y(int Value);
public readonly record struct Point(X X, Y Y);