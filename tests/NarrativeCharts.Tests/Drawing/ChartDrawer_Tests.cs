using NarrativeCharts.Drawing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static NarrativeCharts.Bookworm.BookwormCharacters;
using static NarrativeCharts.Bookworm.BookwormLocations;

namespace NarrativeCharts.Tests.Drawing;

public class ChartDrawer_Tests
{
	private FakeChartDrawer Drawer { get; } = new();

	[Fact]
	public async Task Empty()
	{
		await Drawer.SaveChartAsync(new(), "").ConfigureAwait(false);

		Drawer.Images.Single().Segments.Should().BeEmpty();
	}

	[Fact]
	public async Task EndMovement()
	{
		var chart = TestUtils.Chart;
		chart.AddScene(new(0, Temple, new[] { Myne }));
		chart.AddScene(new(1, Temple, new[] { Myne }));
		chart.AddScene(new(2, MynesHouse, new[] { Myne }));

		await Drawer.SaveChartAsync(chart, "").ConfigureAwait(false);

		var segments = Drawer.Images.Single().Segments;
		segments.Should().HaveCount(2);

		segments[0].X0.Should().Be(0);
		segments[0].X1.Should().Be(1);
		segments[0].Y0.Should().Be(segments[0].Y1);
		segments[0].IsMovement.Should().Be(false);
		segments[0].IsFinal.Should().Be(false);

		segments[1].X0.Should().Be(1);
		segments[1].X1.Should().Be(2);
		segments[1].Y0.Should().NotBe(segments[1].Y1);
		segments[1].IsMovement.Should().Be(true);
		segments[1].IsFinal.Should().Be(true);
	}

	[Fact]
	public async Task EndNoMovement()
	{
		var chart = TestUtils.Chart;
		chart.AddScene(new(0, Temple, new[] { Myne }));
		chart.AddScene(new(1, MynesHouse, new[] { Myne }));
		chart.AddScene(new(2, MynesHouse, new[] { Myne }));

		await Drawer.SaveChartAsync(chart, "").ConfigureAwait(false);

		var segments = Drawer.Images.Single().Segments;
		segments.Should().HaveCount(2);

		segments[0].X0.Should().Be(0);
		segments[0].X1.Should().Be(1);
		segments[0].Y0.Should().NotBe(segments[0].Y1);
		segments[0].IsMovement.Should().Be(true);
		segments[0].IsFinal.Should().Be(false);

		segments[1].X0.Should().Be(1);
		segments[1].X1.Should().Be(2);
		segments[1].Y0.Should().Be(segments[1].Y1);
		segments[1].IsMovement.Should().Be(false);
		segments[1].IsFinal.Should().Be(true);
	}

	[Fact]
	public async Task NoCharacterMovement_Allowed()
	{
		var chart = TestUtils.Chart;
		chart.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		chart.AddScene(new(1, Temple, new[] { Ferdinand, Myne }));
		chart.AddScene(new(2, Temple, new[] { Ferdinand, Myne }));

		Drawer.IgnoreNonMovingCharacters = false;
		await Drawer.SaveChartAsync(chart, "").ConfigureAwait(false);

		Drawer.Images.Single().Segments.Should().HaveCount(2);
	}

	[Fact]
	public async Task NoCharacterMovement_Ignored()
	{
		var chart = TestUtils.Chart;
		chart.AddScene(new(0, Temple, new[] { Ferdinand, Myne }));
		chart.AddScene(new(1, Temple, new[] { Ferdinand, Myne }));
		chart.AddScene(new(2, Temple, new[] { Ferdinand, Myne }));

		Drawer.IgnoreNonMovingCharacters = true;
		await Drawer.SaveChartAsync(chart, "").ConfigureAwait(false);

		Drawer.Images.Single().Segments.Should().BeEmpty();
	}

	[Fact]
	public void RecommendedYSpacing()
	{
		Drawer.LineWidth = 5;
		Drawer.PointLabelSize = 10;
		Drawer.ImageSizeMult = 3;
		Drawer.UseRecommendedYSpacing();
		Drawer.YSpacing.Should().Be(6);

		Drawer.ImageSizeMult = 6;
		Drawer.UseRecommendedYSpacing();
		Drawer.YSpacing.Should().Be(3);

		Drawer.LineWidth = 10;
		Drawer.UseRecommendedYSpacing();
		Drawer.YSpacing.Should().Be(5);
	}
}