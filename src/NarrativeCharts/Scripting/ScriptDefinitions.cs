using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System.Collections.Concurrent;
using System.Text.Json;

namespace NarrativeCharts.Scripting;

public class ScriptDefinitions
{
	private static readonly JsonSerializerOptions _JsonOptions = new()
	{
		WriteIndented = true,
	};

	public Dictionary<string, Character> CharacterAliases { get; set; } = new();
	public Dictionary<Character, Hex> CharacterColors { get; set; } = new();
	public Dictionary<string, Location> LocationAliases { get; set; } = new();
	public Dictionary<Location, int> LocationYIndexes { get; set; } = new();
	public TimeTrackerUnits Time { get; set; } = new(Enumerable.Repeat(1, 24));
	public Dictionary<string, int> TimeAliases { get; set; } = new();

	public static async Task<ScriptDefinitions> LoadAsync(string path)
	{
		DefsJson json;
		using (var fs = File.OpenRead(path))
		{
			json = (await JsonSerializer.DeserializeAsync<DefsJson>(fs).ConfigureAwait(false))!;
		}

		var defs = new ScriptDefinitions();
		if (json.Time is not null)
		{
			for (var i = 0; i < json.Time.Count; ++i)
			{
				var aliased = json.Time[i];
				foreach (var alias in aliased.Aliases.Prepend(i.ToString()))
				{
					defs.TimeAliases[alias] = i;
				}
			}

			defs.Time = new(json.Time.Select(x => x.Length));
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

	public async Task SaveAsync(string path)
	{
		var reverseCAliases = new ConcurrentDictionary<Character, List<string>>();
		var reverseLAliases = new ConcurrentDictionary<Location, List<string>>();
		var reverseTAliases = new ConcurrentDictionary<int, List<string>>();
		foreach (var (alias, character) in CharacterAliases)
		{
			if (alias != character.Value)
			{
				reverseCAliases.GetOrAdd(character, _ => new()).Add(alias);
			}
		}
		foreach (var (alias, location) in LocationAliases)
		{
			if (alias != location.Value)
			{
				reverseLAliases.GetOrAdd(location, _ => new()).Add(alias);
			}
		}
		foreach (var (alias, unit) in TimeAliases)
		{
			if (alias != unit.ToString())
			{
				reverseTAliases.GetOrAdd(unit, _ => new()).Add(alias);
			}
		}

		var json = new DefsJson(
			Characters: CharacterColors.OrderBy(x => x.Key.Value).Select(x => new CharacterJson(
				Aliases: reverseCAliases.GetValueOrDefault(x.Key, new()),
				Hex: x.Value == Hex.Unknown ? null : x.Value.Value,
				Name: x.Key.Value
			)).ToList(),
			Locations: LocationYIndexes.OrderBy(x => x.Value).Select(x => new LocationJson(
				Aliases: reverseLAliases.GetValueOrDefault(x.Key, new()),
				Name: x.Key.Value
			)).ToList(),
			Time: Time.UnitToHourMap.OrderBy(x => x.Key).Select(x => new TimeJson(
				Aliases: reverseTAliases.GetValueOrDefault(x.Key, new()),
				Length: (Time.UnitToHourMap.TryGetValue(x.Key + 1, out var end)
					? end : Time.HoursPerDay) - Time.UnitToHourMap[x.Key]
			)).ToList()
		);

		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		using var fs = File.Create(path);
		await JsonSerializer.SerializeAsync(fs, json, _JsonOptions).ConfigureAwait(false);
	}

	public record DefsJson(
		List<CharacterJson> Characters,
		List<LocationJson> Locations,
		List<TimeJson>? Time
	);

	public record TimeJson(
		List<string> Aliases,
		int Length
	);

	public record CharacterJson(
		List<string> Aliases,
		string? Hex,
		string Name
	);

	public record LocationJson(
		List<string> Aliases,
		string Name
	);
}