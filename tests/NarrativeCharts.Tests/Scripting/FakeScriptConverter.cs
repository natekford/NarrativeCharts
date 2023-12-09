using NarrativeCharts.Scripting;

namespace NarrativeCharts.Tests.Scripting;

public partial class Scripting_Tests
{
	public sealed class FakeScriptConverter : ScriptConverter
	{
		public bool WasWritten { get; private set; }

		public FakeScriptConverter() : this(DateTime.UtcNow)
		{
		}

		public FakeScriptConverter(DateTime time) : base(TestUtils.Defs, time, [])
		{
			Name = ClassName;
		}

		public override string Write()
		{
			WasWritten = true;
			return "";
		}
	}
}