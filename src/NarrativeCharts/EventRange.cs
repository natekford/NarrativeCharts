namespace NarrativeCharts;

public readonly record struct EventRange(int MaxX, int MaxY, int MinX, int MinY)
{
	public int RangeX => MaxX - MinX;
	public int RangeY => MaxY - MinY;

	public static EventRange GetRange(NarrativeChart chart)
	{
		int maxX = int.MinValue,
			maxY = int.MinValue,
			minX = int.MaxValue,
			minY = int.MaxValue;
		foreach (var point in chart.GetAllNarrativePoints())
		{
			maxX = Math.Max(maxX, point.Point.X.Value);
			maxY = Math.Max(maxY, point.Point.Y.Value);
			minX = Math.Min(minX, point.Point.X.Value);
			minY = Math.Min(minY, point.Point.Y.Value);
		}
		return new(MaxX: maxX, MaxY: maxY, MinX: minX, MinY: minY);
	}
}