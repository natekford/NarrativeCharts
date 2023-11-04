using NarrativeCharts.Models;
using NarrativeCharts.Time;

namespace NarrativeCharts.Scripting;

public class ScriptLoader : NarrativeChartUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
			| StringSplitOptions.RemoveEmptyEntries
			| StringSplitOptions.TrimEntries;

	private ScriptSymbols? _Symbols;

	public ScriptDefinitions Definitions { get; }
	protected IEnumerable<string> Lines { get; }
	protected string? NextSceneName { get; set; }
	protected Dictionary<string, Dictionary<Character, Location>> StoredScenes { get; }
		= [];
	// Sort in reverse order so something like "##" shows up before "#"
	// Otherwise "#" would always steal "##" items
	protected SortedDictionary<string, Action<string>> SymbolHandlers { get; }
		= new(Comparer<string>.Create((a, b) => b.CompareTo(a)));

	public ScriptLoader(ScriptDefinitions definitions, IEnumerable<string> lines)
		: base(definitions.Time)
	{
		Definitions = definitions;
		Lines = lines;

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
		}
		return SymbolHandlers!;
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
		=> NextSceneName = input;

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
		Return(StoredScenes[input]);
		StoredScenes.Remove(input);
	}

	protected virtual void HandleScene(string input)
	{
		var args = SplitAssignment(input);
		switch (args.Length)
		{
			case 2:
				var location = ParseLocation(args[0]);
				var characters = ParseCharacters(args[1]);
				if (NextSceneName is null)
				{
					Add(location, characters);
				}
				else
				{
					var locations = AddR(location, characters);
					StoredScenes.Add(NextSceneName, locations);
					NextSceneName = null;
				}
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

	protected virtual Character[] ParseCharacters(string input)
	{
		return SplitArgs(input)
			.Select(x => Definitions.CharacterAliases[x])
			.ToArray();
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

	protected virtual string[] SplitArgs(string value)
		=> value.Split(Definitions.Symbols.Args, SPLIT_OPTIONS);

	protected virtual string[] SplitAssignment(string value)
		=> value.Split(Definitions.Symbols.Assignment, SPLIT_OPTIONS);
}