namespace AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

public sealed class PerformanceStats
{
	public TimeSpan AverageBatchDuration { get; set; }
	public TimeSpan FastestBatchDuration { get; set; }
	public TimeSpan SlowestBatchDuration { get; set; }
	public DateTime? LastActivityTime { get; set; }
	public TimeSpan? TimeSinceLastActivity { get; set; }
	public double ProcessingEfficiency { get; set; } // % of time actively processing
}