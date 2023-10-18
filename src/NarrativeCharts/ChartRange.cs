namespace NarrativeCharts;

public readonly record struct ChartRange(int MaxX, int MaxY, int MinX, int MinY)
{
	public int RangeX => MaxX - MinX;
	public int RangeY => MaxY - MinY;

	public static ChartRange GetRange(NarrativeChartGenerator chart)
	{
		int maxX = 0, maxY = 0, minX = 0, minY = 0;
		foreach (var point in chart.GetAllNarrativePoints())
		{
			maxX = Math.Max(maxX, point.Point.X);
			maxY = Math.Max(maxY, point.Point.Y);
			minX = Math.Min(minX, point.Point.X);
			minY = Math.Min(minY, point.Point.Y);
		}
		return new(MaxX: maxX, MaxY: maxY, MinX: minX, MinY: minY);
	}
}