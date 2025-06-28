using Akka.Actor;

using AkkaNet.Examples.GrpcServiceExample.Messages;

namespace AkkaNet.Examples.GrpcServiceExample.Actors;

public class StatisticsActor : ReceiveActor
{
	private readonly Dictionary<string, BatchStats> _batchStats = new();
	private DateTime? _overallStartTime;
	private DateTime? _overallEndTime;
	private int _totalProcessedPoints;

	public StatisticsActor()
	{
		Receive<StartProcessing>(HandleStartProcessing);
		Receive<PointCompleted>(HandlePointCompleted);
		Receive<GetStatisticSingleton>(HandleGetStatistics);
	}

	private void HandleStartProcessing(StartProcessing message)
	{
		_overallStartTime ??= message.StartTime;

		_batchStats[message.BatchId] = new BatchStats
		{
			BatchId = message.BatchId,
			TotalPoints = message.TotalPoints,
			StartTime = message.StartTime,
			ProcessedPoints = 0
		};

		Console.WriteLine($"Started processing batch {message.BatchId} with {message.TotalPoints} points");
	}

	private void HandlePointCompleted(PointCompleted message)
	{
		if (!_batchStats.TryGetValue(message.BatchId, out var stats))
			return;

		stats.ProcessedPoints++;
		stats.LastProcessedTime = message.CompletedAt;
		_totalProcessedPoints++;

		// Check if batch is already completed
		if (stats.ProcessedPoints < stats.TotalPoints) 
			return;

		stats.EndTime = message.CompletedAt;
		_overallEndTime = message.CompletedAt;

		var batchDuration = (stats.EndTime.Value - stats.StartTime).TotalSeconds;
		var batchThroughput = stats.TotalPoints / batchDuration;

		Console.WriteLine($"Batch {message.BatchId} completed: {stats.TotalPoints} points in {batchDuration:F2}s ({batchThroughput:F2} points/sec)");
	}

	private void HandleGetStatistics(GetStatisticSingleton message)
	{
		var throughput = 0.0;

		if (_overallStartTime.HasValue && _overallEndTime.HasValue)
		{
			var totalDuration = (_overallEndTime.Value - _overallStartTime.Value).TotalSeconds;
			throughput = _totalProcessedPoints / totalDuration;
		}

		var response = new StatisticsResponse(throughput, _overallStartTime, _overallEndTime, _totalProcessedPoints);
		Sender.Tell(response);
	}

	public static Props Props() => Akka.Actor.Props.Create<StatisticsActor>();

	private class BatchStats
	{
		public string BatchId { get; set; }
		public int TotalPoints { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int ProcessedPoints { get; set; }
		public DateTime? LastProcessedTime { get; set; }
	}
}
