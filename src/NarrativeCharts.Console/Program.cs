using NarrativeCharts.Drawing;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;

using System.Collections.Immutable;

namespace NarrativeCharts.Console;

public sealed class Program(ImmutableArray<string> Args)
{
	private const long DEFAULT_CHANGED = 0;
	private const long TICKS_PER_SECOND = 10_000_000;

	private long _Changed = DEFAULT_CHANGED;
	private FileSystemWatcher? _FileSystemWatcher;

	public bool IsCmd => Args.Length != 0;

	public async Task RunAsync()
	{
		var directory = GetDirectory();
		await ProcessAsync(directory, null).ConfigureAwait(false);
		if (IsCmd)
		{
			return;
		}

		while (true)
		{
			var ticks = Interlocked.Exchange(ref _Changed, DEFAULT_CHANGED);
			if (ticks == DEFAULT_CHANGED)
			{
				// No scripts/definitions have been edited, keep waiting and
				// periodically check until something has been edited
				await Task.Delay(250).ConfigureAwait(false);
				continue;
			}

			// There's a minor tick difference between File.LastWriteTimeUtc and
			// the times we get from FSW.Changed (especially because the event
			// doesn't provide any times itself so we have to use DateTime.UtcNow)
			// It's a ~10ms difference, so subtracting 1s is more than enough
			// to get DrawScriptsAsync to redraw recently edited scripts
			ticks -= TICKS_PER_SECOND;
			// Don't reuse any script definitions or parsed scripts because they
			// can be externally edited
			await ProcessAsync(directory, new(ticks)).ConfigureAwait(false);
		}
	}

	private static async Task Main(string[] args)
	{
		try
		{
			await new Program(args.ToImmutableArray()).RunAsync().ConfigureAwait(false);
		}
		catch (Exception e)
		{
			System.Console.WriteLine(e);
		}
	}

	private static async Task ProcessAsync(string directory, DateTime? comparisonTimeUtc)
	{
		try
		{
			var defsPath = Path.Combine(directory, "ScriptDefinitions.json");
			var defs = await ScriptDefinitions.LoadAsync(defsPath).ConfigureAwait(false);
			defs.ComparisonTimeUtc = comparisonTimeUtc;
			var scripts = defs.LoadScripts().ToList();
			// todo: put drawer properties into ScriptDefinitions
			var drawer = new SKChartDrawer().UseRecommendedYSpacing();

			await defs.ProcessAsync(scripts, drawer).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			System.Console.WriteLine(e);
		}
	}

	private string GetDirectory()
	{
		if (IsCmd)
		{
			var argDirectory = Args.SingleOrDefault();
			if (!Directory.Exists(argDirectory))
			{
				throw new ArgumentException("Directory does not exist.");
			}
			return argDirectory;
		}

		string? directory;
		do
		{
			System.Console.WriteLine("Enter a directory to parse scripts from: ");
			directory = System.Console.ReadLine();
		}
		while (!Directory.Exists(directory));

		// just use fsw as an indicator that something was changed, we don't care
		// about what was changed, and trying to do the async processing in
		// the Changed event causes a lot of problems
		_FileSystemWatcher = new FileSystemWatcher()
		{
			Path = directory,
			EnableRaisingEvents = true,
			Filter = "*.*",
		};
		_FileSystemWatcher.Changed += OnChanged;

		System.Console.WriteLine($"Started watching {directory} for changes.");
		System.Console.WriteLine();

		return directory;
	}

	private void OnChanged(object sender, FileSystemEventArgs e)
		=> Interlocked.CompareExchange(ref _Changed, DateTime.UtcNow.Ticks, DEFAULT_CHANGED);
}