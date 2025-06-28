namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class StatisticsResponse(
	double throughputPerSecond,
	DateTime? startTime,
	DateTime? endTime,
	int processedCount)
{
	public double ThroughputPerSecond { get; } = throughputPerSecond;
	public DateTime? StartTime { get; } = startTime;
	public DateTime? EndTime { get; } = endTime;
	public int ProcessedCount { get; } = processedCount;
}
