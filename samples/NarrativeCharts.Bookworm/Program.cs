using NarrativeCharts.Bookworm.Meta.Locations;
using NarrativeCharts.Bookworm.Meta.Time;
using NarrativeCharts.Drawing;
using NarrativeCharts.Scripting;
using NarrativeCharts.Skia;
using NarrativeCharts.Time;

namespace NarrativeCharts.Bookworm;

public sealed class Program
{
	internal static ScriptDefinitions CreateScriptDefinitions(string dir)
	{
		return new ScriptDefinitions
		{
			ScriptDirectory = Path.Combine(dir, "Scripts"),
			ConvertScripts = true,
			RedrawUneditedScripts = false,
			CharacterColors = new(BookwormCharacters.Colors),
			CharacterAliases = new(BookwormCharacters.Aliases),
			LocationYIndexes = new(BookwormLocations.YIndexes),
			LocationAliases = new(BookwormLocations.Aliases),
			Time = new TimeTrackerWithUnits(BookwormTime.Lengths),
			TimeUnitAliases = new(BookwormTime.Aliases),
			OnlyDrawTheseCharacters =
			[
			//BookwormCharacters.Ella,
			//BookwormCharacters.Hugo,
			//BookwormCharacters.Rosina,
			//BookwormCharacters.Myne
			],
		};
	}

	private static async Task Main()
	{
		var defs = CreateScriptDefinitions(Directory.GetCurrentDirectory());
		var scripts = defs.LoadScripts().ToList();
		var drawer = new SKChartDrawer()
		{
#if DEBUG
			// Smaller images in debug so they render faster
			ImageSizeMult = 3f,
#endif
		}.UseRecommendedYSpacing();

		await defs.ProcessAsync(scripts, drawer).ConfigureAwait(false);
		await Task.Delay(-1).ConfigureAwait(false);
	}
}