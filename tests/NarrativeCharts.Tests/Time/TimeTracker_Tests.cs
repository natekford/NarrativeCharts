using FluentAssertions;

using NarrativeCharts.Time;

namespace NarrativeCharts.Tests.Time;

public class TimeTracker_Tests
{
	private TimeTrackerWithUnits Time { get; } = new(new[]
	{
		4,
		3,
		3,
		2,
		3,
		2,
		3,
		4,
	});

	[Fact]
	public void AddHours_Test()
	{
		Time.AddDay();
		Time.CurrentTotalHours.Should().Be(24);

		Time.AddHour();
		Time.CurrentTotalHours.Should().Be(25);

		Time.AddDays(2);
		Time.CurrentTotalHours.Should().Be(73);

		Time.AddHours(3);
		Time.CurrentTotalHours.Should().Be(76);

		Time.AddDay();
		Time.CurrentTotalHours.Should().Be(100);
	}

	[Fact]
	public void AddUnit_Test()
	{
		for (var i = 1; i <= Time.LargestUnit; ++i)
		{
			Time.AddUnit();
			Time.CurrentUnit.Should().Be(i);
		}

		Time.AddUnit();
		Time.CurrentUnit.Should().Be(0);
		Time.CurrentTotalHours.Should().Be(24);
	}

	[Fact]
	public void AddUnits_Test()
	{
		Time.AddUnits(0);
		Time.CurrentTotalHours.Should().Be(0);

		Time.AddUnits(17);
		Time.CurrentDay.Should().Be(3);
		Time.CurrentUnit.Should().Be(1);
	}

	[Fact]
	public void Constructor_Invalid()
	{
		Action action = () => _ = new TimeTracker(0);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void Constructor_Valid()
	{
		Time.CurrentDay.Should().Be(1);
		Time.CurrentHour.Should().Be(0);
		Time.CurrentTotalHours.Should().Be(0);
		Time.HoursPerDay.Should().Be(24);

		Time.LargestUnit.Should().Be(7);
		Time.UnitToHourMap.Should().BeEquivalentTo(new Dictionary<int, int>()
		{
			[0] = 0,
			[1] = 4,
			[2] = 7,
			[3] = 10,
			[4] = 12,
			[5] = 15,
			[6] = 17,
			[7] = 20,
		});
		Time.HourToUnitMap.Should().BeEquivalentTo(new Dictionary<int, int>()
		{
			[0] = 0,
			[1] = 0,
			[2] = 0,
			[3] = 0,
			[4] = 1,
			[5] = 1,
			[6] = 1,
			[7] = 2,
			[8] = 2,
			[9] = 2,
			[10] = 3,
			[11] = 3,
			[12] = 4,
			[13] = 4,
			[14] = 4,
			[15] = 5,
			[16] = 5,
			[17] = 6,
			[18] = 6,
			[19] = 6,
			[20] = 7,
			[21] = 7,
			[22] = 7,
			[23] = 7,
		});
	}

	[Fact]
	public void SetCurrentHour_InPast()
	{
		Time.AddHours(4);

		Action action = () => Time.SetCurrentHour(3);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentHour_TooBig()
	{
		Action action = () => Time.SetCurrentHour(25);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentHour_TooSmall()
	{
		Action action = () => Time.SetCurrentHour(-1);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentHour_Valid()
	{
		Time.AddHours(5);
		Time.SetCurrentHour(10);
		Time.CurrentHour.Should().Be(10);

		Time.AddDays(5);
		Time.CurrentHour.Should().Be(10);
	}

	[Fact]
	public void SetCurrentUnit_InPast()
	{
		Time.AddUnits(4);

		Action action = () => Time.SetCurrentUnit(3);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentUnit_TooBig()
	{
		Action action = () => Time.SetCurrentUnit(10);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentUnit_TooSmall()
	{
		Action action = () => Time.SetCurrentUnit(-1);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetCurrentUnit_Valid()
	{
		Time.AddUnits(2);
		Time.SetCurrentUnit(4);
		Time.CurrentUnit.Should().Be(4);

		Time.AddDays(5);
		Time.CurrentUnit.Should().Be(4);
	}

	[Fact]
	public void SetTotalHours_Invalid()
	{
		Time.SetTotalHours(4);

		Action action = () => Time.SetTotalHours(3);
		action.Should().Throw<ArgumentOutOfRangeException>();
	}

	[Fact]
	public void SetTotalHours_Valid()
	{
		Time.SetTotalHours(25);
		Time.CurrentDay.Should().Be(2);
		Time.CurrentHour.Should().Be(1);
		Time.CurrentTotalHours.Should().Be(25);
		Time.HoursPerDay.Should().Be(24);
	}

	[Fact]
	public void SkipToDaysAheadStart_Test()
	{
		Time.AddHours(3);
		Time.SkipToDayAheadStart();
		Time.CurrentTotalHours.Should().Be(24);

		Time.AddHours(5);
		Time.SkipToDaysAheadStart(3);
		Time.CurrentTotalHours.Should().Be(96);

		Time.SkipToDaysAheadStart(0);
		Time.CurrentTotalHours.Should().Be(96);
	}
}