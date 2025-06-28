using AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class DetailedStatisticsResponse(
	OverallStats overall,
	List<BatchInfo> batches,
	ThroughputStats throughput,
	PerformanceStats performance)
{
	public OverallStats Overall { get; } = overall;
	public List<BatchInfo> Batches { get; } = batches;
	public ThroughputStats Throughput { get; } = throughput;
	public PerformanceStats Performance { get; } = performance;
}