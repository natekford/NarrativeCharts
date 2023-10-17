namespace NarrativeCharts.Console;

public class TimeTracker
{
	private const int BELLS_PER_DAY = 8;
	private const int HOURS_PER_BELL = 3;
	private const int HOURS_PER_DAY = BELLS_PER_DAY * HOURS_PER_BELL;

	public int Hour { get; private set; }

	public void AddBell()
		=> Hour += HOURS_PER_BELL;

	public void AddDay()
		=> Hour += HOURS_PER_DAY;

	public void AddHour()
		=> ++Hour;

	public void GoToBellOfNextDay(int bell)
	{
		if (bell >= BELLS_PER_DAY - 1)
		{
			throw new ArgumentOutOfRangeException(nameof(bell));
		}

		GoToStartOfNextDay();
		Hour += bell * HOURS_PER_BELL;
	}

	public void GoToStartOfNextDay()
		=> Hour += Hour % HOURS_PER_DAY;
}