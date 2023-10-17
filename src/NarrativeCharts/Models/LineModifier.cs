namespace NarrativeCharts.Models;

[Flags]
public enum LineModifier : uint
{
	Normal = 0,
	Invisible = 1U << 0,
	Dotted = 1U << 1,
}