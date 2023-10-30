using NarrativeCharts.Models;

using System.Diagnostics.CodeAnalysis;

namespace NarrativeCharts.Scripting;

public class ScriptLoader : NarrativeChartUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
		| StringSplitOptions.RemoveEmptyEntries
		| StringSplitOptions.TrimEntries;

	private readonly Dictionary<string, Dictionary<Character, Location>> _StoredScenes = new();
	// Sort in reverse order so something like "##" shows up before "#"
	// Otherwise "#" would always steal "##" items
	private readonly SortedDictionary<string, Action<string>> _SymbolHandlers
		= new(Comparer<string>.Create((a, b) => b.CompareTo(a)));
	private string? _NextSceneName;
	private ScriptSymbols? _Symbols;

	public ScriptDefinitions Definitions { get; }
	public string ScriptPath { get; }

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

	protected override int ConvertToInt(int unit)
		=> unit;

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

	private string[] Args(string value)
		=> value.Split(Definitions.Symbols.Args, SPLIT_OPTIONS);

	private string[] Assignment(string value)
		=> value.Split(Definitions.Symbols.Assignment, SPLIT_OPTIONS);

	private SortedDictionary<string, Action<string>> GetSymbolHandlers()
	{
		if (_Symbols != Definitions.Symbols)
		{
			_Symbols = Definitions.Symbols;
			_SymbolHandlers.Clear();
			_SymbolHandlers.Add(_Symbols.Comment, HandleComment);
			_SymbolHandlers.Add(_Symbols.Title, HandleTitle);
			_SymbolHandlers.Add(_Symbols.Chapter, HandleChapter);
			_SymbolHandlers.Add(_Symbols.SkipToCurrentDay, HandleSkipToCurrentDay);
			_SymbolHandlers.Add(_Symbols.SkipToNextDay, HandleSkipToNextDay);
			_SymbolHandlers.Add(_Symbols.Update, HandleUpdate);
			_SymbolHandlers.Add(_Symbols.AddScene, HandleAddScene);
			_SymbolHandlers.Add(_Symbols.RemoveScene, HandleRemoveScene);
		}
		return _SymbolHandlers!;
	}

	private void HandleAddScene(string name)
		=> _NextSceneName = name;

	private void HandleChapter(string chapter)
		=> Event(chapter);

	private void HandleComment(string _)
	{
	}

	private void HandleRemoveScene(string name)
	{
		Return(_StoredScenes[name]);
		_StoredScenes.Remove(name);
	}

	private void HandleSkipToCurrentDay(string input)
	{
		var args = Args(input);
		switch (args.Length)
		{
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

	private void HandleSkipToNextDay(string input)
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

	private void HandleTitle(string title)
		=> Name = title;

	private void HandleUpdate(string _)
		=> Update();

	private void ProcessLine(string line)
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

			if (_NextSceneName is null)
			{
				Add(Scene(location).With(characters));
			}
			else
			{
				var dict = AddR(Scene(location).With(characters));
				_StoredScenes.Add(_NextSceneName, dict);
				_NextSceneName = null;
			}
			return;
		}

		throw new ArgumentException("Line does not match any expected format.");
	}
}