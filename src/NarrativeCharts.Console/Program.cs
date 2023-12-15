using NarrativeCharts.Drawing;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;

using SkiaSharp;

using System.Collections.Immutable;

namespace NarrativeCharts.Console;

public class Program(ImmutableArray<string> Args)
{
	public bool IsCmd => Args.Length != 0;

	public static Task Main(string[] args)
		=> new Program(args.ToImmutableArray()).RunAsync();

	public async Task RunAsync()
	{
		var directory = GetDirectory();
		await ProcessAsync(directory).ConfigureAwait(false);

		if (!IsCmd)
		{
			System.Console.WriteLine($"Started watching {directory} for changes.");

			// just use fsw as an indicator that something was changed, we don't care
			// about what was changed, and trying to do the async processing in
			// the Changed event causes a lot of problems
			var reprocess = 0;
			var watcher = new FileSystemWatcher()
			{
				Path = directory,
				EnableRaisingEvents = true,
				Filter = "*.*",
			};
			watcher.Changed += (_, _) => Interlocked.Exchange(ref reprocess, 1);

			while (true)
			{
				if (Interlocked.Exchange(ref reprocess, 0) == 1)
				{
					try
					{
						await ProcessAsync(directory!).ConfigureAwait(false);
					}
					catch (Exception e)
					{
						System.Console.WriteLine(e);
					}
				}
				await Task.Delay(500).ConfigureAwait(false);
			}
		}
	}

	private static async Task ProcessAsync(string directory)
	{
		var defsPath = Path.Combine(directory, "ScriptDefinitions.json");
		var defs = await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
		var scripts = defs.LoadScripts().ToList();
		// todo: put drawer properties into ScriptDefinitions
		var drawer = new SKChartDrawer()
		{
			ImageAspectRatio = 16f / 9f,
			CharacterLabelColorConverter = SKColorConverters.Color(SKColors.Black),
		};

		await ScriptingUtils.ProcessAsync(scripts, defs, drawer).ConfigureAwait(false);
	}

	private string GetDirectory()
	{
		if (IsCmd)
		{
			var directory = Args.SingleOrDefault();
			if (!Directory.Exists(directory))
			{
				throw new ArgumentException("Directory does not exist.");
			}
			return directory;
		}
		else
		{
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
	}
}