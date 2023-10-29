using NarrativeCharts.Models;

using System.Diagnostics.CodeAnalysis;

namespace NarrativeCharts.Scripting;

public class ScriptLoader : NarrativeChartUnits<int>
{
	private const StringSplitOptions SPLIT_OPTIONS = 0
		| StringSplitOptions.RemoveEmptyEntries
		| StringSplitOptions.TrimEntries;

	private readonly Dictionary<string, Dictionary<Character, Location>> _StoredScenes = new();
	private string? _NextSceneName;

	public ScriptDefinitions Definitions { get; }
	public string ScriptPath { get; }
	private ScriptSymbols Symbols => Definitions.Symbols;

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

	private void ProcessLine(string line)
	{
		if (line.MatchesSymbol(Symbols.Comment, out _))
		{
			return;
		}
		else if (line.MatchesSymbol(Symbols.Title, out var remainder))
		{
			Name = remainder;
			return;
		}
		else if (line.MatchesSymbol(Symbols.Chapter, out remainder))
		{
			Event(remainder);
			return;
		}
		else if (line.MatchesSymbol(Symbols.SkipToNextDay, out remainder))
		{
			var args = Args(remainder);
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
			}
		}
		else if (line.MatchesSymbol(Symbols.SkipToCurrentDay, out remainder))
		{
			var args = Args(remainder);
			switch (args.Length)
			{
				case 0:
					Jump();
					return;

				case 1:
					SkipToCurrentDay(Definitions.TimeAliases[args[0]]);
					return;
			}
		}
		else if (line.MatchesSymbol(Symbols.Update, out remainder) && remainder.Length == 0)
		{
			Update();
			return;
		}
		else if (line.MatchesSymbol(Symbols.AddScene, out remainder))
		{
			_NextSceneName = remainder;
			return;
		}
		else if (line.MatchesSymbol(Symbols.RemoveScene, out remainder))
		{
			Return(_StoredScenes[remainder]);
			_StoredScenes.Remove(remainder);
			return;
		}
		else
		{
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
		}

		throw new ArgumentException("Line does not match any expected format.");
	}
}

internal static class ScriptLoaderUtils
{
	public static bool MatchesSymbol(
		this string line,
		string symbol,
		[NotNullWhen(true)] out string? remainder)
	{
		var matches = line.StartsWith(symbol);
		remainder = matches ? line[symbol.Length..] : null;
		return matches;
	}
}