namespace NarrativeCharts.Scripting;

public record ScriptSymbols(
	string Comment = "//",
	string Assignment = "=",
	string Args = ",",
	string Title = "##",
	string Chapter = "#",
	string SkipToNextDay = ">>",
	string SkipToCurrentDay = ">",
	string Update = "@",
	string AddScene = "+",
	string RemoveScene = "-"
);