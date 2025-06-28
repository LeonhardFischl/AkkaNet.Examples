namespace AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

public sealed class BatchInfo
{
	public string BatchId { get; set; }
	public int TotalPoints { get; set; }
	public int ProcessedPoints { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime? EndTime { get; set; }
	public TimeSpan? Duration { get; set; }
	public double? ThroughputPerSecond { get; set; }
	public string Status { get; set; } // "Active", "Completed"
}