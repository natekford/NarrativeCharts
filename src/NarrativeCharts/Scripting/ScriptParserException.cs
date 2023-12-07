namespace NarrativeCharts.Scripting;

/// <summary>
/// Represents an error that occurs during script parsing.
/// </summary>
public class ScriptParserException : Exception
{
	/// <summary>
	/// The text of the line that errored.
	/// </summary>
	public required string Line { get; init; }
	/// <summary>
	/// The line number of the line that errored.
	/// </summary>
	public required int LineNumber { get; init; }

	/// <inheritdoc />
	public ScriptParserException() : base()
	{
	}

	/// <inheritdoc />
	public ScriptParserException(string? message) : base(message)
	{
	}

	/// <inheritdoc />
	public ScriptParserException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}