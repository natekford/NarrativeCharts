using NarrativeCharts.Models;

using System.Diagnostics.CodeAnalysis;

namespace NarrativeCharts.Scripting;

public class ScriptLoader : NarrativeChartUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
			| StringSplitOptions.RemoveEmptyEntries
			| StringSplitOptions.TrimEntries;

	private ScriptSymbols? _Symbols;

	public ScriptDefinitions Definitions { get; }
	public string ScriptPath { get; }
	protected string? NextSceneName { get; set; }
	protected Dictionary<string, Dictionary<Character, Location>> StoredScenes { get; }
		= new();
	// Sort in reverse order so something like "##" shows up before "#"
	// Otherwise "#" would always steal "##" items
	protected SortedDictionary<string, Action<string>> SymbolHandlers { get; }
		= new(Comparer<string>.Create((a, b) => b.CompareTo(a)));

	public ScriptLoader(ScriptDefinitions definitions, string path)
		: base(definitions.Time)
	{
		Definitions = definitions;
		ScriptPath = path;

		foreach (var (key, value) in definitions.CharacterColors)
		{
			Colors.Add(key, value);
		}
		foreach (var (key, value) in definitions.LocationYIndexes)
		{
			YIndexes.Add(key, value);
		}
	}

	protected virtual string[] Args(string value)
		=> value.Split(Definitions.Symbols.Args, SPLIT_OPTIONS);

	protected virtual string[] Assignment(string value)
		=> value.Split(Definitions.Symbols.Assignment, SPLIT_OPTIONS);

	protected override int ConvertToInt(int unit)
				=> unit;

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
			SymbolHandlers.Add(_Symbols.Update, HandleUpdate);
			SymbolHandlers.Add(_Symbols.AddScene, HandleAddScene);
			SymbolHandlers.Add(_Symbols.RemoveScene, HandleRemoveScene);
		}
		return SymbolHandlers!;
	}

	protected virtual void HandleAddScene(string name)
		=> NextSceneName = name;

	protected virtual void HandleAddUnits(string input)
	{
		var args = Args(input);
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

	protected virtual void HandleChapter(string chapter)
		=> Event(chapter);

	protected virtual void HandleComment(string _)
	{
	}

	protected virtual void HandleRemoveScene(string name)
	{
		Return(StoredScenes[name]);
		StoredScenes.Remove(name);
	}

	protected virtual void HandleScene(Location location, Character[] characters)
	{
		if (NextSceneName is null)
		{
			Add(Scene(location).With(characters));
		}
		else
		{
			var scene = AddR(Scene(location).With(characters));
			StoredScenes.Add(NextSceneName, scene);
			NextSceneName = null;
		}
	}

	protected virtual void HandleSkipToCurrentDay(string input)
	{
		var args = Args(input);
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
		var args = Args(input);
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

	protected virtual void HandleTitle(string title)
		=> Name = title;

	protected virtual void HandleUpdate(string _)
		=> Update();

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

		// Scene creation (character movement)
		var assignment = Assignment(line);
		if (assignment.Length == 2)
		{
			var location = Definitions.LocationAliases[assignment[0]];
			var characters = Args(assignment[1])
				.Select(x => Definitions.CharacterAliases[x])
				.ToArray();
			HandleScene(location, characters);
			return;
		}

		throw new ArgumentException("Line does not match any expected format.");
	}

	protected override void ProtectedCreate()
	{
		var i = 0;
		foreach (var line in File.ReadLines(ScriptPath))
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
}