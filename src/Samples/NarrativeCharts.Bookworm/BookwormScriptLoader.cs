using NarrativeCharts.Models;

using static NarrativeCharts.Bookworm.BookwormBell;

namespace NarrativeCharts.Bookworm;

public sealed class BookwormScriptLoader : BookwormNarrativeChart
{
	public const char CHAPTER = '.';
	public const string COMMENT = "//";
	public const char DEF_CHARACTER = 'C';
	public const char DEF_LOCATION = 'L';
	public const char DEF_NAME = 'N';
	public const char GOTO_CURRENT_DAY = '>';
	public const string GOTO_DAYS_AHEAD = ">>";
	public const char SPLIT_ARGS = ',';
	public const char SPLIT_ASSIGNMENT = '=';

	public string ScriptPath { get; }

	public BookwormScriptLoader(BookwormTimeTracker time, string path) : base(time)
	{
		ScriptPath = path;
	}

	protected override void ProtectedCreate()
	{
		var state = new LoaderState();
		foreach (var line in File.ReadLines(ScriptPath))
		{
			if (string.IsNullOrEmpty(line) || line.StartsWith(COMMENT))
			{
				continue;
			}

			if (state.DefinitionsFinished)
			{
				ProcessLine(state, line);
			}
			else
			{
				ProcessDefinition(state, line);
			}
		}
	}

	private static string[] Args(string value)
		=> value.Split(SPLIT_ARGS, StringSplitOptions.RemoveEmptyEntries);

	private static string[] Assignment(string value)
		=> value.Split(SPLIT_ASSIGNMENT, StringSplitOptions.RemoveEmptyEntries);

	private void ProcessDefinition(LoaderState state, string line)
	{
		switch (line[0])
		{
			case DEF_NAME:
				Name = line[2..];
				return;

			case DEF_CHARACTER:
				var character = Assignment(line[2..]);
				state.CharacterAliases.Add(character[0], character[1]);
				return;

			case DEF_LOCATION:
				var location = Assignment(line[2..]);
				state.LocationAliases.Add(location[0], location[1]);
				return;

			case CHAPTER:
				state.DefinitionsFinished = true;
				ProcessLine(state, line);
				return;
		}
	}

	private void ProcessLine(LoaderState state, string line)
	{
		if (line.StartsWith(CHAPTER))
		{
			Chapter(line[1..]);
			return;
		}
		else if (line.StartsWith(GOTO_DAYS_AHEAD))
		{
			var split = Args(line[2..]);
			switch (split.Length)
			{
				case 0:
					SkipToNextDay(EarlyMorning);
					return;

				case 1:
					SkipToNextDay(Enum.Parse<BookwormBell>(split[0]));
					return;

				case 2:
					SkipToDaysAhead(int.Parse(split[0]), Enum.Parse<BookwormBell>(split[1]));
					return;
			}
		}
		else if (line.StartsWith(GOTO_CURRENT_DAY))
		{
			var split = Args(line[1..]);
			switch (split.Length)
			{
				case 0:
					AddBell();
					return;

				case 1:
					SkipToCurrentDay(Enum.Parse<BookwormBell>(split[0]));
					return;
			}
		}
		else
		{
			var split = Assignment(line);
			if (split.Length == 2)
			{
				var location = state.GetLocation(split[0]);
				var characters = Args(split[1]).Select(state.GetCharacter).ToArray();
				Add(Scene(location).With(characters));
				return;
			}
		}

		throw new ArgumentException($"Invalid line: {line}");
	}

	private sealed class LoaderState
	{
		public Dictionary<string, string> CharacterAliases { get; set; } = new();
		public bool DefinitionsFinished { get; set; }
		public Dictionary<string, string> LocationAliases { get; set; } = new();

		public Character GetCharacter(string value)
		{
			if (CharacterAliases.TryGetValue(value, out var aliased))
			{
				value = aliased;
			}
			return BookwormCharacters.Colors.Keys.Single(x => x.Value == value);
		}

		public Location GetLocation(string value)
		{
			if (LocationAliases.TryGetValue(value, out var aliased))
			{
				value = aliased;
			}
			return BookwormLocations.YIndexes.Keys.Single(x => x.Value == value);
		}
	}
}