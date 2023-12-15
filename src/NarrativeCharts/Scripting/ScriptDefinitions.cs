using NarrativeCharts.Models;
using NarrativeCharts.Time;

using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NarrativeCharts.Scripting;

/// <summary>
/// Definitions to use when parsing a script.
/// </summary>
public class ScriptDefinitions
{
	private const string SCRIPT_EXT = ".txt";
	private static readonly JsonSerializerOptions _JsonOptions = new()
	{
		// so characters like > and + don't get escaped
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		WriteIndented = true,
	};

	/// <summary>
	/// The strings that map to a <see cref="Character"/>.
	/// </summary>
	public Dictionary<string, Character> CharacterAliases { get; set; } = [];
	/// <inheritdoc cref="NarrativeChartData.Colors" />
	public Dictionary<Character, Hex> CharacterColors { get; set; } = [];
	/// <summary>
	/// If this is less than the last time the image was drawn this is used when
	/// comparing to the script's last edited time.
	/// If this is null it is not used at all.
	/// </summary>
	public DateTime? ComparisonTimeUtc { get; set; }
	/// <summary>
	///  Whether or not to convert parsed scripts to C#.
	/// </summary>
	public bool ConvertScripts { get; set; }
	/// <summary>
	/// The strings that map to a <see cref="Location"/>.
	/// </summary>
	public Dictionary<string, Location> LocationAliases { get; set; } = [];
	/// <inheritdoc cref="NarrativeChartData.YIndexes" />
	public Dictionary<Location, int> LocationYIndexes { get; set; } = [];
	/// <summary>
	/// If there are items in this collection, only those characters will be drawn.
	/// </summary>
	public HashSet<Character> OnlyDrawTheseCharacters { get; set; } = [];
	/// <summary>
	/// Whether or not to redraw scripts that have not been edited since their chart
	/// has been last drawn.
	/// </summary>
	public bool RedrawUneditedScripts { get; set; }
	/// <summary>
	/// The directory to load scripts from.
	/// </summary>
	public required string ScriptDirectory { get; set; } = null!;
	/// <summary>
	/// The extension that indicates a script.
	/// </summary>
	public string ScriptExtension { get; set; } = SCRIPT_EXT;
	/// <summary>
	/// The symbols to use when parsing each line.
	/// </summary>
	public ScriptSymbols Symbols { get; set; } = new();
	/// <summary>
	/// The time to use for all scripts. This is reused and not reset to 0 each time.
	/// </summary>
	public TimeTrackerWithUnits Time { get; set; } = new(Enumerable.Repeat(1, 24));
	/// <summary>
	/// The strings that map to a time unit for <see cref="Time"/>.
	/// </summary>
	public Dictionary<string, int> TimeUnitAliases { get; set; } = [];

	/// <summary>
	/// Load a <see cref="ScriptDefinitions"/> from <paramref name="path"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public static async Task<ScriptDefinitions> LoadAsync(string path)
	{
		DefsJson json;
		await using (var fs = File.OpenRead(path))
		{
			json = (await JsonSerializer.DeserializeAsync<DefsJson>(fs).ConfigureAwait(false))!;
		}

		var defs = new ScriptDefinitions
		{
			ConvertScripts = json.ConvertScriptsToCSharp,
			RedrawUneditedScripts = json.RedrawUneditedScripts,
			ScriptDirectory = Path.GetDirectoryName(path)!,
			Symbols = json.Symbols ?? new(),
		};
		if (!string.IsNullOrEmpty(json.ScriptExtension))
		{
			defs.ScriptExtension = json.ScriptExtension;
		}
		if (json.Time is not null)
		{
			for (var i = 0; i < json.Time.Count; ++i)
			{
				var aliased = json.Time[i];
				foreach (var alias in aliased.Aliases.Prepend(i.ToString()))
				{
					defs.TimeUnitAliases[alias] = i;
				}
			}

			defs.Time = new(json.Time.Select(x => x.DurationInHours));
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
		foreach (var character in json.OnlyDrawTheseCharacters)
		{
			defs.OnlyDrawTheseCharacters.Add(defs.CharacterAliases[character]);
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

	/// <summary>
	/// Save this to <paramref name="path"/>.
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
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
		foreach (var (alias, unit) in TimeUnitAliases)
		{
			if (alias != unit.ToString())
			{
				reverseTAliases.GetOrAdd(unit, _ => []).Add(alias);
			}
		}

		var json = new DefsJson(
			ConvertScriptsToCSharp: ConvertScripts,
			RedrawUneditedScripts: RedrawUneditedScripts,
			ScriptExtension: ScriptExtension,
			Symbols: Symbols,
			OnlyDrawTheseCharacters: OnlyDrawTheseCharacters.Select(x => x.Value).ToList(),
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
			)).ToList()
		);

		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		await using var fs = File.Create(path);
		await JsonSerializer.SerializeAsync(fs, json, _JsonOptions).ConfigureAwait(false);
	}

	private record DefsJson(
		bool ConvertScriptsToCSharp,
		bool RedrawUneditedScripts,
		string ScriptExtension,
		ScriptSymbols? Symbols,
		List<string> OnlyDrawTheseCharacters,
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