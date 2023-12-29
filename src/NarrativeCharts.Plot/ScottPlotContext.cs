using NarrativeCharts.Drawing;

namespace NarrativeCharts.Plot;

public record ScottPlotContext(
	ScottPlot.Plot Plot,
	NarrativeChartData Chart,
	YMap YMap
);