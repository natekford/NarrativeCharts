using NarrativeCharts.Drawing;

using System.Diagnostics;

namespace NarrativeCharts.Scripting;

/// <summary>
/// Utilities for scripting.
/// </summary>
public static class ScriptingUtils
{
	/// <summary>
	/// Directory name that charts will be output to.
	/// </summary>
	public const string CHARTS_DIR = "Charts";
	/// <summary>
	/// Directory name that script conversion will be output to.
	/// </summary>
	public const string CONVERTED_DIR = "Converted";

	/// <summary>
	/// Draws and saves all charts.
	/// </summary>
	/// <param name="defs"></param>
	/// <param name="scripts"></param>
	/// <param name="drawer"></param>
	/// <param name="parallelChartCount"></param>
	/// <returns></returns>
	public static async IAsyncEnumerable<DrawInfo> DrawScriptsAsync(
		this ScriptDefinitions defs,
		IReadOnlyList<ScriptParser> scripts,
		ChartDrawer drawer,
		int parallelChartCount = 10)
	{
		var dir = Path.Combine(defs.ScriptDirectory, CHARTS_DIR);
		Directory.CreateDirectory(dir);

		var count = 0;
		var queue = new Queue<(NarrativeChartData, string)>(scripts.Count);
		var shouldRedraw = defs.RedrawUneditedScripts;
		foreach (var chart in scripts)
		{
			var imagePath = Path.Combine(dir, $"{chart.Name}.png");
			if (!shouldRedraw)
			{
				var imageTime = File.GetLastWriteTimeUtc(imagePath);
				var scriptTime = chart.LastWriteTimeUTC;
				if (imageTime >= scriptTime)
				{
					yield return new(
						TotalCount: scripts.Count,
						CurrentCount: Interlocked.Increment(ref count),
						DrawTime: null,
						OutputPath: imagePath,
						Message: $"Not redrawing. Drawn: {imageTime:G}, edited: {scriptTime:G}."
					);
				}
				else
				{
					// redraw subsequent scripts because the editing of a previous script
					// could change character seeding locations
					shouldRedraw = true;
				}
			}

			if (shouldRedraw)
			{
				queue.Enqueue(new(chart, imagePath));
			}
		}

		var sw = Stopwatch.StartNew();
		var active = new HashSet<Task<DrawInfo>>();
		async Task<DrawInfo> WhenAnyDrawingAsync()
		{
			var task = await Task.WhenAny(active).ConfigureAwait(false);
			active.Remove(task);
			return await task;
		}
		async Task<DrawInfo> SaveChartAsync(NarrativeChartData chart, string path)
		{
			var start = sw.Elapsed;
			await drawer.SaveChartAsync(chart, path).ConfigureAwait(false);
			return new(
				TotalCount: scripts.Count,
				CurrentCount: Interlocked.Increment(ref count),
				DrawTime: sw.Elapsed - start,
				OutputPath: path,
				Message: $"Drawn in {(sw.Elapsed - start).TotalSeconds:#.##} seconds."
			);
		}

		while (queue.TryDequeue(out var item))
		{
			if (active.Count >= parallelChartCount)
			{
				yield return await WhenAnyDrawingAsync().ConfigureAwait(false);
			}

			var (chart, path) = item;
			active.Add(SaveChartAsync(chart, path));
		}
		while (active.Any())
		{
			yield return await WhenAnyDrawingAsync().ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Loads and parses all files with the specified extension.
	/// </summary>
	/// <param name="defs"></param>
	/// <returns></returns>
	public static IEnumerable<ScriptParser> LoadScripts(this ScriptDefinitions defs)
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

	/// <summary>
	/// Converts all scripts to C# and saves them to <see cref="ScriptDefinitions.ScriptDirectory"/>.
	/// </summary>
	/// <param name="defs"></param>
	/// <param name="charts"></param>
	/// <param name="saveDefinitions">Whether or not to also save <paramref name="defs"/>.</param>
	/// <returns></returns>
	public static IReadOnlyList<string> SaveConvertedScripts(
		this ScriptDefinitions defs,
		IEnumerable<ScriptConverter> charts,
		bool saveDefinitions = true)
	{
		if (!defs.ConvertScripts)
		{
			return Array.Empty<string>();
		}

		var dir = Path.Combine(defs.ScriptDirectory, CONVERTED_DIR);
		Directory.CreateDirectory(dir);

		var paths = new List<string>();
		foreach (var chart in charts)
		{
			var path = Path.Combine(dir, $"{chart.ClassName}.cs");
			File.WriteAllText(path, chart.Write());
			paths.Add(path);
		}

		if (saveDefinitions)
		{
			var path = Path.Combine(dir, "ScriptDefinitions.cs");
			File.WriteAllText(path, defs.ConvertToCode());
			paths.Add(path);
		}

		return paths;
	}

	/// <summary>
	/// Status updates for drawing charts.
	/// </summary>
	/// <param name="TotalCount"></param>
	/// <param name="CurrentCount"></param>
	/// <param name="DrawTime"></param>
	/// <param name="OutputPath"></param>
	/// <param name="Message"></param>
	public sealed record DrawInfo(
		int TotalCount,
		int CurrentCount,
		TimeSpan? DrawTime,
		string OutputPath,
		string Message
	)
	{
		/// <inheritdoc />
		public override string ToString()
			=> $"[{CurrentCount}/{TotalCount}] {Path.GetFileName(OutputPath)}: {Message}";
	}
}