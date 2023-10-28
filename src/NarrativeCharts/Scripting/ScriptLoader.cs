namespace NarrativeCharts.Scripting;

public class ScriptLoader : NarrativeChartUnits<int>
{
	public const char CHAPTER = '#';
	public const string COMMENT = "//";
	public const char GOTO_CURRENT_DAY = '>';
	public const string GOTO_DAYS_AHEAD = ">>";
	public const char SPLIT_ARGS = ',';
	public const char SPLIT_ASSIGNMENT = '=';
	public const string TITLE = "##";
	public const char UPDATE = '@';
	private const StringSplitOptions SPLIT_OPTIONS = 0
		| StringSplitOptions.RemoveEmptyEntries
		| StringSplitOptions.TrimEntries;

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
			if (string.IsNullOrWhiteSpace(line) || line.StartsWith(COMMENT))
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

	private static string[] Args(string value)
		=> value.Split(SPLIT_ARGS, SPLIT_OPTIONS);

	private static string[] Assignment(string value)
		=> value.Split(SPLIT_ASSIGNMENT, SPLIT_OPTIONS);

	private void ProcessLine(string line)
	{
		if (line.StartsWith(TITLE))
		{
			Name = line[2..];
			return;
		}
		else if (line.StartsWith(CHAPTER))
		{
			Event(line[1..]);
			return;
		}
		else if (line.StartsWith(GOTO_DAYS_AHEAD))
		{
			var split = Args(line[2..]);
			switch (split.Length)
			{
				case 0:
					SkipToNextDay(1);
					return;

				case 1:
					SkipToNextDay(Definitions.TimeAliases[split[0]]);
					return;

				case 2:
					SkipToDaysAhead(int.Parse(split[0]), Definitions.TimeAliases[split[1]]);
					return;
			}
		}
		else if (line.StartsWith(GOTO_CURRENT_DAY))
		{
			var split = Args(line[1..]);
			switch (split.Length)
			{
				case 0:
					Jump();
					return;

				case 1:
					SkipToCurrentDay(Definitions.TimeAliases[split[0]]);
					return;
			}
		}
		else if (line.StartsWith(UPDATE) && line.Length == 1)
		{
			Update();
			return;
		}
		else
		{
			var split = Assignment(line);
			if (split.Length == 2)
			{
				var location = Definitions.LocationAliases[split[0]];
				var characters = Args(split[1])
					.Select(x => Definitions.CharacterAliases[x])
					.ToArray();
				Add(Scene(location).With(characters));
				return;
			}
		}

		throw new ArgumentException("Line does not match any expected format.");
	}
}