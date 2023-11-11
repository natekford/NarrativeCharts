using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NarrativeCharts.Scripting;

public class ScriptDefinitions
{
	private static readonly JsonSerializerOptions _JsonOptions = new()
	{
		// so characters like > and + don't get escaped
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		WriteIndented = true,
	};

	public Dictionary<string, Character> CharacterAliases { get; set; } = [];
	public Dictionary<Character, Hex> CharacterColors { get; set; } = [];
	public Dictionary<string, Location> LocationAliases { get; set; } = [];
	public Dictionary<Location, int> LocationYIndexes { get; set; } = [];
	public ScriptSymbols Symbols { get; set; } = new();
	public TimeTrackerWithUnits Time { get; set; } = new(Enumerable.Repeat(1, 24));
	public Dictionary<string, int> TimeAliases { get; set; } = [];

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

			defs.Time = new(json.Time.Select(x => x.DurationInHours));
		}
		if (json.Symbols is not null)
		{
			defs.Symbols = json.Symbols;
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
				reverseCAliases.GetOrAdd(character, _ => []).Add(alias);
			}
		}
		foreach (var (alias, location) in LocationAliases)
		{
			if (alias != location.Value)
			{
				reverseLAliases.GetOrAdd(location, _ => []).Add(alias);
			}
		}
		foreach (var (alias, unit) in TimeAliases)
		{
			if (alias != unit.ToString())
			{
				reverseTAliases.GetOrAdd(unit, _ => []).Add(alias);
			}
		}

		var json = new DefsJson(
			Symbols: new(),
			Time: Time.UnitToHourMap.OrderBy(x => x.Key).Select(x => new TimeJson(
				DurationInHours: (Time.UnitToHourMap.TryGetValue(x.Key + 1, out var end)
					? end : Time.HoursPerDay) - Time.UnitToHourMap[x.Key],
				Aliases: reverseTAliases.GetValueOrDefault(x.Key, [])
			)).ToList(),
			Locations: LocationYIndexes.OrderBy(x => x.Value).Select(x => new LocationJson(
				Name: x.Key.Value,
				Aliases: reverseLAliases.GetValueOrDefault(x.Key, [])
			)).ToList(),
			Characters: CharacterColors.OrderBy(x => x.Key.Value).Select(x => new CharacterJson(
				Name: x.Key.Value,
				Hex: x.Value == Hex.Unknown ? null : x.Value.Value,
				Aliases: reverseCAliases.GetValueOrDefault(x.Key, [])
			)).ToList());

		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		using var fs = File.Create(path);
		await JsonSerializer.SerializeAsync(fs, json, _JsonOptions).ConfigureAwait(false);
	}

	private record DefsJson(
		ScriptSymbols? Symbols,
		List<TimeJson>? Time,
		List<LocationJson> Locations,
		List<CharacterJson> Characters
	);

	private record TimeJson(
		int DurationInHours,
		List<string> Aliases
	);

	private record CharacterJson(
		string Name,
		string? Hex,
		List<string> Aliases
	);

	private record LocationJson(
		string Name,
		List<string> Aliases
	);
}