using Akka.Actor;

using AkkaNet.Examples.GrpcServiceExample.Messages;
using AkkaNet.Examples.GrpcServiceExample.Messages.Statistics;

namespace AkkaNet.Examples.GrpcServiceExample.Actors;

public class StatisticsActor : ReceiveActor
{
	private readonly Dictionary<string, BatchStats> _batchStats = new();
	private DateTime? _overallStartTime;
	private DateTime? _overallEndTime;
	private int _totalProcessedPoints;
	private double _peakThroughput;
	private readonly List<double> _recentThroughputSamples = new();
	private const int MaxThroughputSamples = 10;

	public StatisticsActor()
	{
		Receive<StartProcessing>(HandleStartProcessing);
		Receive<PointCompleted>(HandlePointCompleted);
		Receive<GetStatisticSingleton>(HandleGetStatistics);
		Receive<GetDetailedStatisticsSingleton>(HandleGetDetailedStatistics);
	}

	private void HandleStartProcessing(StartProcessing message)
	{
		_overallStartTime ??= message.StartTime;

		_batchStats[message.BatchId] = new BatchStats
		{
			BatchId = message.BatchId,
			TotalPoints = message.TotalPoints,
			StartTime = message.StartTime,
			ProcessedPoints = 0,
			Status = "Active",
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
		stats.Status = "Completed";
		_overallEndTime = message.CompletedAt;

		var batchDuration = (stats.EndTime.Value - stats.StartTime).TotalSeconds;
		var batchThroughput = stats.TotalPoints / batchDuration;
		stats.ThroughputPerSecond = batchThroughput;

		// Update peak throughput
		if (batchThroughput > _peakThroughput)
			_peakThroughput = batchThroughput;

		// Add to recent throughput samples
		_recentThroughputSamples.Add(batchThroughput);
		if (_recentThroughputSamples.Count > MaxThroughputSamples)
			_recentThroughputSamples.RemoveAt(0);

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

	private void HandleGetDetailedStatistics(GetDetailedStatisticsSingleton message)
	{
		var now = DateTime.UtcNow;
		var completedBatches = _batchStats.Values.Where(b => b.Status == "Completed").ToList();
		var activeBatches = _batchStats.Values.Where(b => b.Status == "Active").ToList();

		// Overall stats
		var overall = new OverallStats
		{
			StartTime = _overallStartTime,
			EndTime = _overallEndTime,
			TotalProcessingTime = _overallStartTime.HasValue ?
				(_overallEndTime ?? now) - _overallStartTime.Value : null,
			TotalBatches = _batchStats.Count,
			CompletedBatches = completedBatches.Count,
			ActiveBatches = activeBatches.Count,
			TotalPoints = _batchStats.Values.Sum(b => b.TotalPoints),
			ProcessedPoints = _totalProcessedPoints
		};

		// Batch details
		var batches = CreateBatchInfos(now);

		// Throughput stats
		var throughput = CreateThroughputStats();

		// Performance stats
		var performance = CreatePerformanceStats(completedBatches);

		if (performance.LastActivityTime.HasValue)
			performance.TimeSinceLastActivity = now - performance.LastActivityTime.Value;

		var response = new DetailedStatisticsResponse(overall, batches, throughput, performance);
		Sender.Tell(response);
	}

	private List<BatchInfo> CreateBatchInfos(DateTime now)
	{
		var batches = _batchStats.Values.Select(b => new BatchInfo
			{
				BatchId = b.BatchId,
				TotalPoints = b.TotalPoints,
				ProcessedPoints = b.ProcessedPoints,
				StartTime = b.StartTime,
				EndTime = b.EndTime,
				Duration = b.EndTime.HasValue ? b.EndTime.Value - b.StartTime : now - b.StartTime,
				ThroughputPerSecond = b.ThroughputPerSecond,
				Status = b.Status
			})
			.OrderBy(b => b.StartTime)
			.ToList();
		return batches;
	}

	private ThroughputStats CreateThroughputStats()
	{
		var throughput = new ThroughputStats
		{
			CurrentPointsPerSecond = _recentThroughputSamples.Count > 0 ? _recentThroughputSamples.TakeLast(3).Average() : 0,
			AveragePointsPerSecond = CalculateOverallThroughput(),
			PeakPointsPerSecond = _peakThroughput,
			BatchesPerMinute = CalculateBatchesPerMinute(),
			AveragePointsPerBatch = _batchStats.Count > 0 ? (double)_batchStats.Values.Sum(b => b.TotalPoints) / _batchStats.Count : 0
		};
		return throughput;
	}

	private PerformanceStats CreatePerformanceStats(List<BatchStats> completedBatches)
	{
		var durations = completedBatches
			.Where(b => b.EndTime.HasValue)
			.Select(b => b.EndTime!.Value - b.StartTime)
			.ToList();

		var performance = new PerformanceStats
		{
			AverageBatchDuration = durations.Count > 0 ? TimeSpan.FromMilliseconds(durations.Average(d => d.TotalMilliseconds)) : TimeSpan.Zero,
			FastestBatchDuration = durations.Count > 0 ? durations.Min() : TimeSpan.Zero,
			SlowestBatchDuration = durations.Count > 0 ? durations.Max() : TimeSpan.Zero,
			LastActivityTime = _batchStats.Values
				.Where(b => b.LastProcessedTime.HasValue)
				.Select(b => b.LastProcessedTime!.Value)
				.DefaultIfEmpty()
				.Max(),
			ProcessingEfficiency = CalculateProcessingEfficiency()
		};
		return performance;
	}

	private double CalculateOverallThroughput()
	{
		if (!_overallStartTime.HasValue || !_overallEndTime.HasValue)
			return 0.0;

		var totalDuration = (_overallEndTime.Value - _overallStartTime.Value).TotalSeconds;
		return totalDuration > 0 ? _totalProcessedPoints / totalDuration : 0.0;
	}

	private double CalculateBatchesPerMinute()
	{
		if (!_overallStartTime.HasValue || _batchStats.Count == 0)
			return 0.0;

		var completedBatches = _batchStats.Values.Count(b => b.Status == "Completed");
		var totalMinutes = ((_overallEndTime ?? DateTime.UtcNow) - _overallStartTime.Value).TotalMinutes;

		return totalMinutes > 0 ? completedBatches / totalMinutes : 0.0;
	}

	private double CalculateProcessingEfficiency()
	{
		if (!_overallStartTime.HasValue)
			return 0.0;

		var totalTime = ((_overallEndTime ?? DateTime.UtcNow) - _overallStartTime.Value).TotalSeconds;
		var processingTime = _batchStats.Values
			.Where(b => b.EndTime.HasValue)
			.Sum(b => (b.EndTime.Value - b.StartTime).TotalSeconds);

		return totalTime > 0 ? (processingTime / totalTime) * 100 : 0.0;
	}

	public static Props Props() => Akka.Actor.Props.Create<StatisticsActor>();

	private class BatchStats
	{
		public required string BatchId { get; init; }
		public required int TotalPoints { get; init; }
		public required DateTime StartTime { get; init; }
		public required string Status { get; set; }

		public DateTime? EndTime { get; set; }
		public int ProcessedPoints { get; set; }
		public DateTime? LastProcessedTime { get; set; }
		public double? ThroughputPerSecond { get; set; }
	}
}
