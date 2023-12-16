using NarrativeCharts.Models;
using NarrativeCharts.Time;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.Meta.Locations.BookwormLocations;

namespace NarrativeCharts.Tests;

public class NarrativeChart_Tests
{
	[Fact]
	public void Initialize_Repeated()
	{
		var seed = new NarrativeChartData();
		seed.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));

		var other = new NarrativeChart(new());
		other.Initialize(seed);

		other.HasInitializeBeenCalled.Should().Be(true);
		other.Points.Should().HaveCount(2);

		other.Points.Clear();
		other.Initialize(seed);

		other.HasInitializeBeenCalled.Should().Be(true);
		other.Points.Should().BeEmpty();
	}

	[Fact]
	public void Initialize_Valid()
	{
		var seed = new NarrativeChartData();
		seed.AddEvent(new(0, "Event 1"));
		seed.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));

		var time = new TimeTracker();
		time.SetCurrentHour(10);
		var other = new NarrativeChart(time);
		other.Initialize(seed);

		other.HasInitializeBeenCalled.Should().Be(true);
		other.Events.Should().BeEmpty();
		other.Points.Keys.Should().BeEquivalentTo(new[]
		{
			Ferdinand,
			Myne
		});
		other.Points.Values.Should().AllSatisfy(x => x.Single().Key.Should().Be(10));
	}

	[Fact]
	public void Simplify_Duplicates()
	{
		var chart = new NarrativeChart(new());
		chart.AddScene(new(0, Temple, new[] { Myne }));
		chart.AddScene(new(5, Temple, new[] { Myne }));
		chart.AddScene(new(10, Temple, new[] { Myne }));

		chart.Simplify();

		chart.Points[Myne].Should().HaveCount(2);
		chart.Points[Myne].Keys.Should().BeEquivalentTo(new[] { 0, 10 });
	}

	[Fact]
	public void Simplify_Frozen()
	{
		var chart = new NarrativeChart(new());
		chart.AddScene(new(0, Location.Frozen, new[] { Myne }));

		chart.Simplify();

		chart.Points.Should().HaveCount(1);
		chart.Points[Myne].Should().BeEmpty();
	}

	[Fact]
	public void Simplify_TimeSkip()
	{
		var chart = new NarrativeChart(new());
		chart.AddPoint(new(0, Temple, Myne, false, false));
		chart.AddPoint(new(1, Temple, Myne, false, false));
		chart.AddPoint(new(2, Temple, Myne, false, false));
		chart.AddPoint(new(3, Temple, Myne, false, true));
		chart.AddPoint(new(4, Temple, Myne, false, false));
		chart.AddPoint(new(5, Temple, Myne, false, false));
		chart.AddPoint(new(6, Temple, Myne, false, false));

		chart.Simplify();

		chart.Points[Myne].Should().HaveCount(4);
		chart.Points[Myne].Keys.Should().BeEquivalentTo(new[] { 0, 3, 4, 6 });
	}
}