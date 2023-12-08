using NarrativeCharts.Drawing;

namespace NarrativeCharts.Tests.Drawing;

public record FakeCanvas(List<LineSegment> Segments, YMap YMap, ChartDimensions Dimentions);