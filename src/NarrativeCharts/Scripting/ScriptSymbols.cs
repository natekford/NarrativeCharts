namespace NarrativeCharts.Scripting;

/// <summary>
/// Symbols used when parsing a script.
/// </summary>
/// <param name="Comment"></param>
/// <param name="Assignment"></param>
/// <param name="Args"></param>
/// <param name="Title"></param>
/// <param name="Chapter"></param>
/// <param name="SkipToNextDay"></param>
/// <param name="SkipToCurrentDay"></param>
/// <param name="AddUnits"></param>
/// <param name="AddHours"></param>
/// <param name="TimeSkip"></param>
/// <param name="Update"></param>
/// <param name="Freeze"></param>
/// <param name="Kill"></param>
/// <param name="Scene"></param>
/// <param name="AddReturnableScene"></param>
/// <param name="RemoveReturnableScene"></param>
/// <param name="AddCharacterGroup"></param>
public record ScriptSymbols(
	string Comment = "//",
	string Assignment = "=",
	string Args = ",",
	string Title = "##",
	string Chapter = "#",
	string SkipToNextDay = ">>",
	string SkipToCurrentDay = ">",
	string AddUnits = ">U",
	string AddHours = ">H",
	string TimeSkip = ">>>",
	string Update = "@",
	string Freeze = "@?",
	string Kill = "@!",
	string Scene = "$",
	string AddReturnableScene = "+$",
	string RemoveReturnableScene = "-$",
	string AddCharacterGroup = "+%"
);