using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public class ScriptDefinitions_Tests
{
	[Fact]
	public async Task SaveAndLoad_Test()
	{
		var defs = TestUtils.ScriptDefinitions;
		var path = Path.Combine(defs.ScriptDirectory, "TestDefs.json");

		await defs.SaveAsync(path).ConfigureAwait(false);
		var loadedDefs = await ScriptDefinitions.LoadAsync(path).ConfigureAwait(false);

		defs.Should().BeEquivalentTo(loadedDefs);
	}
}