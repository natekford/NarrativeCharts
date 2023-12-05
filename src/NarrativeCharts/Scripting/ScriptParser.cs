using NarrativeCharts.Models;

namespace NarrativeCharts.Scripting;

/// <summary>
/// Parses a script out of strings.
/// </summary>
public class ScriptParser : NarrativeChartWithUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
			| StringSplitOptions.RemoveEmptyEntries
			| StringSplitOptions.TrimEntries;

	private ScriptSymbols? _Symbols;

	/// <inheritdoc cref="ScriptDefinitions" />
	public ScriptDefinitions Definitions { get; }
	/// <summary>
	/// The last time the script this is sourced from was edited.
	/// </summary>
	public DateTime LastWriteTimeUTC { get; }
	/// <summary>
	/// The lines to parse.
	/// </summary>
	public IEnumerable<string> Lines { get; }
	/// <summary>
	/// Stored groups of characters to easily allow referencing large groups without
	/// mass duplication.
	/// </summary>
	protected Dictionary<string, HashSet<Character>> CharacterGroups { get; } = [];
	/// <summary>
	/// Stored returnable scenes to easily allow returning groups of characters
	/// without having to remember where each individual character came from.
	/// </summary>
	protected Dictionary<string, Dictionary<Character, Location>> StoredScenes { get; } = [];
	/// <summary>
	/// The handlers to use for any line where the start matches the key.
	/// This is sorted in reverse alphabetical order, so something like "##"
	/// shows up before "#", otherwise "#" would always steal "##" items
	/// </summary>
	protected SortedDictionary<string, Action<string>> SymbolHandlers { get; }
		= new(Comparer<string>.Create((a, b) => b.CompareTo(a)));

	/// <summary>
	/// Creates an instance of <see cref="ScriptParser"/>.
	/// </summary>
	/// <param name="definitions"></param>
	/// <param name="lastWriteTimeUtc"></param>
	/// <param name="lines"></param>
	public ScriptParser(
		ScriptDefinitions definitions,
		DateTime lastWriteTimeUtc,
		IEnumerable<string> lines)
		: base(definitions.Time)
	{
		Definitions = definitions;
		LastWriteTimeUTC = lastWriteTimeUtc;
		Lines = lines;

		Colors = new(definitions.CharacterColors);
		YIndexes = new(definitions.LocationYIndexes);
	}

	/// <inheritdoc />
	protected override void AddNarrativeData()
	{
		var i = 0;
		foreach (var line in Lines)
		{
			++i;
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			try
			{
				ProcessLine(line);
			}
			catch (Exception e)
			{
				throw new ArgumentException($"Error occurred while processing line #{i}: {line}", e);
			}
		}
	}

	/// <inheritdoc />
	protected override int Convert(int unit)
		=> unit;

	/// <summary>
	/// Throws an exception if <paramref name="name"/> is in use anywhere.
	/// </summary>
	/// <param name="name"></param>
	protected virtual void EnsureNameNotUsed(string name)
	{
		static ArgumentException AlreadyInUse(string type, string name)
			=> new($"There is already a {type} with the name '{name}'");

		if (CharacterGroups.ContainsKey(name))
		{
			throw AlreadyInUse("character group", name);
		}
		if (StoredScenes.ContainsKey(name))
		{
			throw AlreadyInUse("stored scene", name);
		}
		if (Definitions.CharacterAliases.ContainsKey(name))
		{
			throw AlreadyInUse("character", name);
		}
	}

	/// <summary>
	/// Creates a collection of symbol handlers if any symbols have been changed.
	/// </summary>
	/// <returns></returns>
	protected virtual SortedDictionary<string, Action<string>> GetSymbolHandlers()
	{
		if (_Symbols != Definitions.Symbols)
		{
			_Symbols = Definitions.Symbols;
			SymbolHandlers.Clear();
			SymbolHandlers.Add(_Symbols.Comment, HandleComment);
			SymbolHandlers.Add(_Symbols.Title, HandleTitle);
			SymbolHandlers.Add(_Symbols.Chapter, HandleChapter);
			SymbolHandlers.Add(_Symbols.SkipToCurrentDay, HandleSkipToCurrentDay);
			SymbolHandlers.Add(_Symbols.SkipToNextDay, HandleSkipToNextDay);
			SymbolHandlers.Add(_Symbols.AddUnits, HandleAddUnits);
			SymbolHandlers.Add(_Symbols.AddHours, HandleAddHours);
			SymbolHandlers.Add(_Symbols.TimeSkip, HandleTimeSkip);
			SymbolHandlers.Add(_Symbols.Update, HandleUpdate);
			SymbolHandlers.Add(_Symbols.Freeze, HandleFreeze);
			SymbolHandlers.Add(_Symbols.Kill, HandleKill);
			SymbolHandlers.Add(_Symbols.Scene, HandleScene);
			SymbolHandlers.Add(_Symbols.AddReturnableScene, HandleAddReturnableScene);
			SymbolHandlers.Add(_Symbols.RemoveReturnableScene, HandleRemoveReturnableScene);
			SymbolHandlers.Add(_Symbols.AddCharacterGroup, HandleAddCharacterGroup);
		}
		return SymbolHandlers;
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.AddCharacterGroup"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleAddCharacterGroup(string input)
	{
		var args = SplitAssignment(input);
		switch (args.Length)
		{
			case 2:
				var name = args[0];
				var characters = ParseCharacters(args[1]);
				EnsureNameNotUsed(name);
				CharacterGroups.Add(name, characters);
				return;

			default:
				throw new ArgumentException("Invalid character group assignment.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.AddHours"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleAddHours(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				AddHours(1);
				return;

			case 1:
				AddHours(float.Parse(args[0]));
				return;

			default:
				throw new ArgumentException("Cannot handle more than 1 argument.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.AddReturnableScene"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleAddReturnableScene(string input)
	{
		// split only up to 2 strings
		// since 1 string is the name, and the other is the scene assignment
		// and the scene assignment itself contains the arg splitter
		var returnableScene = SplitArgs(input, 2);
		switch (returnableScene.Length)
		{
			case 2:
				var name = returnableScene[0];
				EnsureNameNotUsed(name);

				var scene = SplitAssignment(returnableScene[1]);
				switch (scene.Length)
				{
					case 2:
						var (location, characters) = SceneAssignment(scene);
						var previousLocations = AddReturnable(location, characters);
						StoredScenes.Add(name, previousLocations);
						return;

					default:
						throw new ArgumentException("Invalid returnable scene assignment.");
				}

			default:
				throw new ArgumentException("Invalid returnable scene, missing a scene assignment or name.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.AddUnits"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleAddUnits(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				Jump();
				return;

			case 1:
				Jump(int.Parse(args[0]));
				return;

			default:
				throw new ArgumentException("Cannot handle more than 1 argument.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Chapter"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleChapter(string input)
		=> Event(input);

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Comment"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleComment(string input)
	{
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Freeze"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleFreeze(string input)
		=> Freeze(ParseCharacters(input));

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Kill"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleKill(string input)
		=> Kill(ParseCharacters(input));

	/// <summary>
	/// Handles <see cref="ScriptSymbols.RemoveReturnableScene"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleRemoveReturnableScene(string input)
	{
		foreach (var scene in SplitArgs(input))
		{
			Return(StoredScenes[scene]);
			StoredScenes.Remove(scene);
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Scene"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleScene(string input)
	{
		var scene = SplitAssignment(input);
		switch (scene.Length)
		{
			case 2:
				var (location, characters) = SceneAssignment(scene);
				Add(location, characters);
				return;

			default:
				throw new ArgumentException("Invalid scene assignment.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.SkipToCurrentDay"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleSkipToCurrentDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			// Does the same thing as AddUnits no args
			case 0:
				Jump();
				return;

			case 1:
				SkipToCurrentDay(Definitions.TimeUnitAliases[args[0]]);
				return;

			default:
				throw new ArgumentException("Cannot handle more than 1 argument.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.SkipToNextDay"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleSkipToNextDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				SkipToNextDay(1);
				return;

			case 1:
				SkipToNextDay(Definitions.TimeUnitAliases[args[0]]);
				return;

			case 2:
				SkipToDaysAhead(int.Parse(args[0]), Definitions.TimeUnitAliases[args[1]]);
				return;

			default:
				throw new ArgumentException("Cannot handle more than 2 arguments.");
		}
	}

	/// <summary>
	/// Handles <see cref="ScriptSymbols.TimeSkip"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleTimeSkip(string input)
		=> TimeSkip(int.Parse(input));

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Title"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleTitle(string input)
		=> Name = input;

	/// <summary>
	/// Handles <see cref="ScriptSymbols.Update"/>.
	/// </summary>
	/// <param name="input"></param>
	protected virtual void HandleUpdate(string input)
		=> Update();

	/// <summary>
	/// Splits <paramref name="input"/> and parses <see cref="Character"/>s.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	protected virtual HashSet<Character> ParseCharacters(string input)
	{
		var args = SplitArgs(input);
		var set = new HashSet<Character>(args.Length);
		foreach (var arg in args)
		{
			if (CharacterGroups.TryGetValue(arg, out var group))
			{
				foreach (var character in group)
				{
					set.Add(character);
				}
			}
			else if (StoredScenes.TryGetValue(arg, out var dict))
			{
				foreach (var character in dict.Keys)
				{
					set.Add(character);
				}
			}
			else
			{
				set.Add(Definitions.CharacterAliases[arg]);
			}
		}
		return set;
	}

	/// <summary>
	/// Parses a location from <paramref name="input"/>.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	protected virtual Location ParseLocation(string input)
		=> Definitions.LocationAliases[input];

	/// <summary>
	/// Attempts to find a symbol handler to deal with <paramref name="line"/>,
	/// throws if one is not found.
	/// </summary>
	/// <param name="line"></param>
	/// <exception cref="ArgumentException"></exception>
	protected virtual void ProcessLine(string line)
	{
		foreach (var (symbol, action) in GetSymbolHandlers())
		{
			if (!line.StartsWith(symbol))
			{
				continue;
			}

			action.Invoke(line[symbol.Length..]);
			return;
		}

		throw new ArgumentException("Line does not match any expected format.");
	}

	/// <summary>
	/// Splits <paramref name="value"/> with <see cref="ScriptSymbols.Args"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="count"></param>
	/// <returns></returns>
	protected virtual string[] SplitArgs(string value, int count = int.MaxValue)
		=> value.Split(Definitions.Symbols.Args, count, SPLIT_OPTIONS);

	/// <summary>
	/// Splits <paramref name="value"/> with <see cref="ScriptSymbols.Assignment"/>.
	/// </summary>
	/// <param name="value"></param>
	/// <param name="count"></param>
	/// <returns></returns>
	protected virtual string[] SplitAssignment(string value, int count = int.MaxValue)
		=> value.Split(Definitions.Symbols.Assignment, count, SPLIT_OPTIONS);

	private (Location, HashSet<Character>) SceneAssignment(string[] input)
	{
		var location = ParseLocation(input[0]);
		var characters = ParseCharacters(input[1]);
		return (location, characters);
	}
}