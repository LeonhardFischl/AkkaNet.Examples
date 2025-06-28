namespace AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

public sealed class OverallStats
{
	public DateTime? StartTime { get; set; }
	public DateTime? EndTime { get; set; }
	public TimeSpan? TotalProcessingTime { get; set; }
	public int TotalBatches { get; set; }
	public int CompletedBatches { get; set; }
	public int ActiveBatches { get; set; }
	public int TotalPoints { get; set; }
	public int ProcessedPoints { get; set; }
}