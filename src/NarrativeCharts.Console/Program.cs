using NarrativeCharts.Drawing;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;

using SkiaSharp;

using System.Collections.Immutable;

namespace NarrativeCharts.Console;

public class Program(ImmutableArray<string> Args)
{
	private const long TICKS_PER_SECOND = 10_000_000;

	public bool IsCmd => Args.Length != 0;

	public static Task Main(string[] args)
		=> new Program(args.ToImmutableArray()).RunAsync();

	public async Task RunAsync()
	{
		var directory = GetDirectory();
		await ProcessAsync(directory, null).ConfigureAwait(false);

		if (!IsCmd)
		{
			System.Console.WriteLine($"Started watching {directory} for changes.");

			// just use fsw as an indicator that something was changed, we don't care
			// about what was changed, and trying to do the async processing in
			// the Changed event causes a lot of problems
			var changed = 0L;
			var watcher = new FileSystemWatcher()
			{
				Path = directory,
				EnableRaisingEvents = true,
				Filter = "*.*",
			};
			watcher.Changed += (_, _)
				=> Interlocked.CompareExchange(ref changed, DateTime.UtcNow.Ticks, 0);

			while (true)
			{
				var ticks = Interlocked.Exchange(ref changed, 0);
				if (ticks != 0)
				{
					// remove 1 second from the time we're using because of minor
					// tick differences between what FSW event can get and what
					// the file actually has as its timestamp
					// it was only like .01 seconds difference but i dont think
					// making it 1 second difference matters much
					ticks -= TICKS_PER_SECOND;
					try
					{
						// always call this instead of reusing scripts/defs/drawer
						// b/c scripts/defs are things that can be externally changed
						await ProcessAsync(directory, new(ticks)).ConfigureAwait(false);
					}
					catch (Exception e)
					{
						System.Console.WriteLine(e);
					}
				}
				else
				{
					// only need this delay if processasync isn't called because
					// process async takes more than 250ms by itself usually
					await Task.Delay(250).ConfigureAwait(false);
				}
			}
		}
	}

	private static async Task ProcessAsync(string directory, DateTime? comparisonTimeUtc)
	{
		var defsPath = Path.Combine(directory, "ScriptDefinitions.json");
		var defs = await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
		defs.ComparisonTimeUtc = comparisonTimeUtc;
		var scripts = defs.LoadScripts().ToList();
		// todo: put drawer properties into ScriptDefinitions
		var drawer = new SKChartDrawer()
		{
			ImageAspectRatio = 16f / 9f,
			CharacterLabelColorConverter = SKColorConverters.Color(SKColors.Black),
		};

		await defs.ProcessAsync(scripts, drawer).ConfigureAwait(false);
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