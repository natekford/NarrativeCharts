using NarrativeCharts.Models;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.Meta.Locations.BookwormLocations;

namespace NarrativeCharts.Tests;

public class ChartUtils_Tests
{
	[Fact]
	public static void AddChart_Valid()
	{
		var source = new NarrativeChartData();
		source.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source.AddEvent(new(0, "Prologue"));
		source.Colors.Add(Myne, Hex.Unknown);
		source.YIndexes.Add(Temple, 100);

		var child = new NarrativeChartData().AddChart(source);

		child.Colors.Should().BeEmpty();
		child.YIndexes.Should().BeEmpty();
		child.Events.Single().Value.Should().BeEquivalentTo(new NarrativeEvent(0, "Prologue"));
		child.Points.Values.Should().AllSatisfy(x =>
		{
			x.Should().HaveCount(1);
			x.Values[0].Hour.Should().Be(0);
			x.Values[0].Location.Should().Be(Temple);
		});
	}

	[Fact]
	public static void Combine_Valid()
	{
		var source1 = new NarrativeChartData();
		source1.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source1.AddEvent(new(0, "Prologue"));
		source1.Colors.Add(Myne, Hex.Unknown);
		source1.YIndexes.Add(Temple, 100);

		var source2 = new NarrativeChartData();
		source2.AddScene(new(0, MynesHouse, new[] { Effa, Gunther, Tuuli }));
		source2.AddEvent(new(10, "Chapter 2"));
		source2.Colors.Add(Effa, Hex.Unknown);
		source2.YIndexes.Add(MynesHouse, 0);

		var combined = new[] { source1, source2 }.Combine();

		combined.Points.Should().HaveCount(5);
		combined.Events.Should().HaveCount(2);
		combined.Colors.Should().HaveCount(2);
		combined.YIndexes.Should().HaveCount(2);
	}

	[Fact]
	public static void UpdatePoints_IgnoreEnd()
	{
		var source = new NarrativeChartData();
		source.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source.Points[Myne].ModifyLastPoint(x => x with { IsEnd = true });

		source.UpdatePoints(10);

		source.Points[Myne].Should().HaveCount(1);
		source.Points[Ferdinand].Should().HaveCount(2);
	}

	[Fact]
	public static void UpdatePoints_IgnoreFuture()
	{
		var source = new NarrativeChartData();
		source.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source.Points[Myne].ModifyLastPoint(x => x with { Hour = 20 });

		source.UpdatePoints(10);

		source.Points[Myne].Should().HaveCount(1);
		source.Points[Ferdinand].Should().HaveCount(2);
	}

	[Fact]
	public void Seed_IgnoreEnd()
	{
		var source = new NarrativeChartData();
		source.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source.Points[Myne].ModifyLastPoint(x => x with { IsEnd = true });

		var child = new NarrativeChartData();
		child.Seed(source, 10);

		child.Points.Should().HaveCount(1);
		var point = child.Points[Ferdinand].Values.Single();
		point.Hour.Should().Be(10);
		point.Location.Should().Be(Temple);
	}

	[Fact]
	public void Seed_Valid()
	{
		var source = new NarrativeChartData();
		source.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		source.AddEvent(new(0, "Prologue"));
		source.Colors.Add(Myne, Hex.Unknown);
		source.YIndexes.Add(Temple, 100);

		var child = new NarrativeChartData().Seed(source, 10);

		child.Colors.Should().BeEmpty();
		child.Events.Should().BeEmpty();
		child.YIndexes.Should().BeEmpty();
		child.Points.Values.Should().AllSatisfy(x =>
		{
			x.Should().HaveCount(1);
			x.Values[0].Hour.Should().Be(10);
			x.Values[0].Location.Should().Be(Temple);
		});
	}
}