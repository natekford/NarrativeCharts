using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;

using SkiaSharp;

namespace NarrativeCharts.Console;

public static class Program
{
	public static string GetDirectory(string? @default)
	{
		// if directory provided as cmd argument, only use that
		if (@default is not null)
		{
			if (!Directory.Exists(@default))
			{
				throw new ArgumentException("Directory does not exist.");
			}
			return @default;
		}

		while (true)
		{
			System.Console.WriteLine("Enter a directory to parse scripts from: ");
			var input = System.Console.ReadLine();
			if (Directory.Exists(input))
			{
				return input;
			}
		}
	}

	public static async Task Main(string[] args)
	{
		var dir = GetDirectory(args.SingleOrDefault());
		var defsPath = Path.Combine(dir, "ScriptDefinitions.json");

		var defs = await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
		var scripts = defs.LoadScripts().ToList();
		// todo: put drawer properties into ScriptDefinitions
		var drawer = new SKChartDrawer()
		{
			ImageAspectRatio = 16f / 9f,
			CharacterLabelColorConverter = SKColorConverters.Color(SKColors.Black),
		};

		await ScriptingUtils.ProcessAsync(scripts, defs, drawer).ConfigureAwait(false);

		// if invoked with no args let the console stay open
		if (args.Length > 0)
		{
			await Task.Delay(-1).ConfigureAwait(false);
		}
	}
}