namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class StatisticsOverAllResponse(DateTime? startTime, DateTime? endTime, int processedCount, List<StatisticsResponse> batchStatistics)
{
	public DateTime? StartTime { get; } = startTime;
	public DateTime? EndTime { get; } = endTime;
	public int ProcessedCount { get; } = processedCount;
	public List<StatisticsResponse> BatchStatistics { get; } = batchStatistics;
}
