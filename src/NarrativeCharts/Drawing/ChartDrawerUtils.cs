namespace NarrativeCharts.Drawing;

/// <summary>
/// Utilities for <see cref="ChartDrawer"/>.
/// </summary>
public static class ChartDrawerUtils
{
	/// <summary>
	/// Sets <see cref="ChartDrawer.YSpacing"/> to a value that should not result
	/// in line/label overlap and should not result in a lot of space between lines.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="drawer"></param>
	/// <returns></returns>
	public static T UseRecommendedYSpacing<T>(this T drawer)
		where T : ChartDrawer
	{
		// The breakdown of the height of each line is:
		// 1/4 diameter
		// 1/2 diameter (usually the Line Width)
		// 1/4 diameter
		//               Character Name in Font Size
		// So we can basically just do Line Marker + Font Size
		var pixels = drawer.LineMarkerDiameter + drawer.PointLabelSize;
		var spacing = pixels / drawer.ImageSizeMult;
		drawer.YSpacing = (int)float.Floor(spacing);
		return drawer;
	}
}