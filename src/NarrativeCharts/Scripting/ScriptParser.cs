﻿using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts.Scripting;

public class ScriptParser : NarrativeChartWithUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
			| StringSplitOptions.RemoveEmptyEntries
			| StringSplitOptions.TrimEntries;

	private ScriptSymbols? _Symbols;

	public ScriptDefinitions Definitions { get; }
	protected Dictionary<string, IEnumerable<Character>> CharacterGroups { get; } = [];
	protected IEnumerable<string> Lines { get; }
	protected Dictionary<string, Dictionary<Character, Location>> StoredScenes { get; } = [];
	// Sort in reverse order so something like "##" shows up before "#"
	// Otherwise "#" would always steal "##" items
	protected SortedDictionary<string, Action<string>> SymbolHandlers { get; }
		= new(Comparer<string>.Create((a, b) => b.CompareTo(a)));

	public ScriptParser(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions.Time)
	{
		Definitions = definitions;
		Lines = lines;

		Colors = new(definitions.CharacterColors);
		YIndexes = new(definitions.LocationYIndexes);
	}

	protected override int ConvertToInt(int unit)
		=> unit;

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
		return SymbolHandlers!;
	}

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

	protected virtual void HandleAddHours(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			// Does the same thing as SkipToCurrentDay no args
			case 0:
				Time.AddHour();
				Update();
				return;

			case 1:
				Time.AddHours(int.Parse(args[0]));
				Update();
				return;

			default:
				throw new ArgumentException("Cannot handle more than 1 argument.");
		}
	}

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
						var previousLocations = AddR(location, characters);
						StoredScenes.Add(name, previousLocations);
						return;

					default:
						throw new ArgumentException("Invalid returnable scene assignment.");
				}

			default:
				throw new ArgumentException("Invalid returnable scene, missing a scene assignment or name.");
		}
	}

	protected virtual void HandleAddUnits(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			// Does the same thing as SkipToCurrentDay no args
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

	protected virtual void HandleChapter(string input)
		=> Event(input);

	protected virtual void HandleComment(string input)
	{
	}

	protected virtual void HandleFreeze(string input)
		=> Freeze(ParseCharacters(input));

	protected virtual void HandleKill(string input)
		=> Kill(ParseCharacters(input));

	protected virtual void HandleRemoveReturnableScene(string input)
	{
		foreach (var scene in SplitArgs(input))
		{
			Return(StoredScenes[scene]);
			StoredScenes.Remove(scene);
		}
	}

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
				SkipToCurrentDay(Definitions.TimeAliases[args[0]]);
				return;

			default:
				throw new ArgumentException("Cannot handle more than 1 argument.");
		}
	}

	protected virtual void HandleSkipToNextDay(string input)
	{
		var args = SplitArgs(input);
		switch (args.Length)
		{
			case 0:
				SkipToNextDay(1);
				return;

			case 1:
				SkipToNextDay(Definitions.TimeAliases[args[0]]);
				return;

			case 2:
				SkipToDaysAhead(int.Parse(args[0]), Definitions.TimeAliases[args[1]]);
				return;

			default:
				throw new ArgumentException("Cannot handle more than 2 arguments.");
		}
	}

	protected virtual void HandleTimeSkip(string input)
		=> TimeSkip(int.Parse(input));

	protected virtual void HandleTitle(string input)
		=> Name = input;

	protected virtual void HandleUpdate(string input)
		=> Update();

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

	protected virtual Location ParseLocation(string input)
		=> Definitions.LocationAliases[input];

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

	protected override void ProtectedCreate()
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

	protected virtual string[] SplitArgs(string value, int count = int.MaxValue)
		=> value.Split(Definitions.Symbols.Args, count, SPLIT_OPTIONS);

	protected virtual string[] SplitAssignment(string value, int count = int.MaxValue)
		=> value.Split(Definitions.Symbols.Assignment, count, SPLIT_OPTIONS);

	private (Location, HashSet<Character>) SceneAssignment(string[] input)
	{
		var location = ParseLocation(input[0]);
		var characters = ParseCharacters(input[1]);
		return (location, characters);
	}
}