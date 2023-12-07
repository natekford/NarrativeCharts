using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public class ScriptConverterUtils_Tests
{
	[Fact]
	public void ConvertToCode_Syntax()
	{
		var output = TestUtils.ScriptDefinitions.ConvertToCode();

		TestUtils.ValidateSyntax(output);
	}
}