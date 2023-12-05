namespace NarrativeCharts.Scripting;

/// <summary>
/// Utilities for scripting.
/// </summary>
public static class ScriptingUtils
{
	/// <summary>
	/// Loads all files with the specified extension then parses them.
	/// </summary>
	/// <param name="defs"></param>
	/// <returns></returns>
	public static IEnumerable<ScriptParser> LoadScripts(ScriptDefinitions defs)
	{
		var previous = default(ScriptParser?);
		// use a natural sort so V30 shows up between V29 and V31
		// and not between V3 and V4
		var scripts = Directory.GetFiles(defs.ScriptDirectory, $"*{defs.ScriptExtension}")
			.OrderBy(x => x, NaturalSortStringComparer.Ordinal);
		foreach (var script in scripts)
		{
			var time = File.GetLastWriteTimeUtc(script);
			var lines = File.ReadAllLines(script);
			var chart = defs.ConvertScripts
				? new ScriptConverter(defs, time, lines)
				: new ScriptParser(defs, time, lines);

			chart.Initialize(previous);
			yield return previous = chart;
		}
	}
}