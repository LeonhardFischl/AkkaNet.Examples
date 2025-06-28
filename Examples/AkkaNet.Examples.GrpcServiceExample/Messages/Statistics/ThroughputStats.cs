namespace AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

public sealed class ThroughputStats
{
	public double CurrentPointsPerSecond { get; set; }
	public double AveragePointsPerSecond { get; set; }
	public double PeakPointsPerSecond { get; set; }
	public double BatchesPerMinute { get; set; }
	public double AveragePointsPerBatch { get; set; }
}