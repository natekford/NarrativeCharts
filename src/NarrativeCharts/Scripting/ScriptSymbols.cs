﻿namespace NarrativeCharts.Scripting;

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
	string Update = "@",
	string Freeze = "@?",
	string Kill = "@!",
	string Scene = "$",
	string AddReturnableScene = "+$",
	string RemoveReturnableScene = "-$"
);