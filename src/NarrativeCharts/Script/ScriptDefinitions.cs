using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System.Text.Json;

namespace NarrativeCharts.Script;

public class ScriptDefinitions
{
	public Dictionary<string, Character> CharacterAliases { get; set; } = new();
	public Dictionary<Character, Hex> CharacterColors { get; set; } = new();
	public Dictionary<string, Location> LocationAliases { get; set; } = new();
	public Dictionary<Location, int> LocationYIndexes { get; set; } = new();
	public TimeTrackerUnits Time { get; set; } = new(Enumerable.Repeat(1, 24));
	public Dictionary<string, int> TimeAliases { get; set; } = new();

	public static async Task<ScriptDefinitions> FromFileAsync(string path)
	{
		Json json;
		using (var fs = File.OpenRead(path))
		{
			json = (await JsonSerializer.DeserializeAsync<Json>(fs).ConfigureAwait(false))!;
		}

		var defs = new ScriptDefinitions();
		if (json.Time is not null)
		{
			for (var i = 0; i < json.Time.Count; ++i)
			{
				var aliased = json.Time[i];
				foreach (var alias in aliased.Aliases.Prepend(i.ToString()))
				{
					defs.TimeAliases[alias] = aliased.Start;
				}
			}

			defs.Time = new(json.Time.Select(x => x.Start));
		}
		foreach (var aliased in json.Characters)
		{
			var character = new Character(aliased.Name);
			foreach (var alias in aliased.Aliases.Prepend(aliased.Name))
			{
				defs.CharacterAliases[alias] = character;
			}

			defs.CharacterColors[character] = aliased.Hex is string hex
				? new(hex)
				: Hex.Unknown;
		}
		for (var i = 0; i < json.Locations.Count; ++i)
		{
			var aliased = json.Locations[i];
			var location = new Location(aliased.Name);
			foreach (var alias in aliased.Aliases.Prepend(aliased.Name))
			{
				defs.LocationAliases[alias] = location;
			}

			defs.LocationYIndexes[location] = i;
		}

		return defs;
	}

	private record Json(
		List<AliasedString> Characters,
		List<AliasedString> Locations,
		List<AliasedTime>? Time
	);

	private record AliasedTime(
		int Start,
		List<string> Aliases
	);

	private record AliasedString(
		string Name,
		List<string> Aliases,
		string? Hex
	);
}