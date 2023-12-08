namespace NarrativeCharts.Drawing;

/// <summary>
/// The calculated dimensions to use for the image.
/// </summary>
/// <param name="Width">The width to use.</param>
/// <param name="WidthMult">The value to multiply every X value in the grid by.</param>
/// <param name="Height">The height to use.</param>
/// <param name="HeightMult">The value to multiply every Y value in the grid by.</param>
public record ChartDimensions(int Width, float WidthMult, int Height, float HeightMult);