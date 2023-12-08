using NarrativeCharts.Drawing;

using System.Diagnostics;

namespace NarrativeCharts.Scripting;

/// <summary>
/// Handles converting and drawing scripted charts.
/// </summary>
public class ScriptCollection
{
	/// <summary>
	/// The scripted charts to process.
	/// </summary>
	public required IReadOnlyList<NarrativeChartData> Charts { get; init; }
	/// <summary>
	/// The definitions for the scripts.
	/// </summary>
	public required ScriptDefinitions Defs { get; init; }
	/// <summary>
	/// The drawer for drawing charts.
	/// </summary>
	public required ChartDrawer Drawer { get; init; }
	/// <summary>
	/// The amount of charts to draw in parallel.
	/// </summary>
	public int ParallelChartCount { get; init; } = 10;

	/// <summary>
	/// Displays chart information, converts scripts, then draws charts.
	/// </summary>
	/// <returns></returns>
	public virtual async Task ProcessAsync()
	{
		await DisplayChartsAsync().ConfigureAwait(false);
		await ConvertScriptsAsync().ConfigureAwait(false);
		await DrawChartsAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Converts and saves scripts.
	/// </summary>
	/// <returns></returns>
	protected virtual async Task ConvertScriptsAsync()
	{
		if (!Defs.ConvertScripts)
		{
			return;
		}

		var dir = Path.Combine(Defs.ScriptDirectory, "Converted");
		Directory.CreateDirectory(dir);

		foreach (var chart in Charts)
		{
			if (chart is not ScriptConverter scriptConverter)
			{
				continue;
			}

			var path = Path.Combine(dir, $"{scriptConverter.ClassName}.cs");
			await File.WriteAllTextAsync(path, scriptConverter.Write()).ConfigureAwait(false);
		}

		{
			var path = Path.Combine(dir, "ScriptDefinitions.cs");
			await File.WriteAllTextAsync(path, Defs.ConvertToCode()).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Displays chart information.
	/// </summary>
	/// <returns></returns>
	protected virtual async Task DisplayChartsAsync()
	{
		foreach (var chart in Charts)
		{
			var properties = new Dictionary<string, string>();

			{
				properties["Character"] = chart.Points.Count.ToString();
			}

			{
				var points = chart.Points.Sum(x => x.Value.Count);
				properties["Points"] = points.ToString();
			}

			{
				var extrema = chart.GetExtrema();
				var days = extrema.Duration / Defs.Time.HoursPerDay;
				properties["Days"] = days.ToString("#.#");
			}

			var joined = string.Join(", ", properties.Select(x => $"{x.Key}={x.Value}"));
			await PrintAsync($"{chart.Name}: {joined}").ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Draws and saves charts.
	/// </summary>
	/// <returns></returns>
	protected virtual async Task DrawChartsAsync()
	{
		var dir = Path.Combine(Defs.ScriptDirectory, "Charts");
		Directory.CreateDirectory(dir);

		var sw = Stopwatch.StartNew();
		var count = 0;

		var charts = new List<(NarrativeChartData, string)>();
		var shouldRedrawScripts = Defs.RedrawUneditedScripts;
		foreach (var chart in Charts)
		{
			var imagePath = Path.Combine(dir, $"{chart.Name}.png");
			if (!shouldRedrawScripts && chart is ScriptParser scriptParser)
			{
				var imageTime = File.GetLastWriteTimeUtc(imagePath);
				var scriptTime = scriptParser.LastWriteTimeUTC;
				if (imageTime >= scriptTime)
				{
					await PrintAsync($"[{Interlocked.Increment(ref count)}/{Charts.Count}] " +
						$"Not redrawing {Path.GetFileName(imagePath)}. " +
						$"Drawn: {imageTime:G}, " +
						$"edited: {scriptTime:G}."
					).ConfigureAwait(false);
					continue;
				}
				else
				{
					// redraw subsequent scripts because the editing of a previous script
					// could change character seeding locations
					shouldRedrawScripts = true;
				}
			}

			charts.Add(new(chart, imagePath));
		}

		var queue = new Queue<(NarrativeChartData, string)>(charts);
		var active = new Dictionary<Task, (TimeSpan, string)>();
		async Task WhenAnyDrawingAsync()
		{
			var task = await Task.WhenAny(active.Keys).ConfigureAwait(false);
			active.Remove(task, out var item);
			var (start, imagePath) = item;
			await PrintAsync($"[{Interlocked.Increment(ref count)}/{Charts.Count}] " +
				$"Finished drawing {Path.GetFileName(imagePath)} " +
				$"in {(sw.Elapsed - start).TotalSeconds:#.##} seconds."
			).ConfigureAwait(false);
		}

		while (queue.TryDequeue(out var item))
		{
			if (active.Count >= ParallelChartCount)
			{
				await WhenAnyDrawingAsync().ConfigureAwait(false);
			}

			var (chart, imagePath) = item;
			active.Add(
				key: Drawer.SaveChartAsync(chart, imagePath),
				value: new(sw.Elapsed, imagePath)
			);
		}
		while (active.Any())
		{
			await WhenAnyDrawingAsync().ConfigureAwait(false);
		}

		await PrintAsync($"{Charts.Count} charts created " +
			$"after {sw.Elapsed.TotalSeconds:#.##} seconds."
		).ConfigureAwait(false);
	}

	/// <summary>
	/// Prints out text.
	/// </summary>
	/// <param name="text"></param>
	/// <returns></returns>
	protected virtual Task PrintAsync(string text)
	{
		Console.WriteLine(text);
		return Task.CompletedTask;
	}
}