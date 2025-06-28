using AkkaNet.Examples.GrpcServiceExample.Models;
using AkkaNet.Examples.GrpcServiceExample.Processors;

using Microsoft.AspNetCore.Mvc;

namespace AkkaNet.Examples.GrpcServiceExample.Controllers;

[ApiController]
[Route("api")]
public class HomeController(ILogger<HomeController> logger, CoordinateStreamProcessor processor) : ControllerBase
{

	[HttpGet("/")]
	public IActionResult Index()
	{
		logger.LogInformation("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
		return Ok("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
	}

	[HttpGet("health")]
	public IActionResult HealthCheck()
	{
		logger.LogDebug("Healthy. Service is running...");
		return Ok("Healthy. Service is running...");
	}

	[HttpGet("version")]
	public IActionResult Version()
	{
		logger.LogInformation($"Healthy. {nameof(BuildInfo.Version)}: {BuildInfo.Instance.Version}, {nameof(BuildInfo.ApplicationStartedAtDateTime)}: {BuildInfo.Instance.ApplicationStartedAtDateTime}");
		return Ok(BuildInfo.Instance);
	}

	[HttpGet("statistics")]
	public async Task<IActionResult> GetStatistics()
	{
		logger.LogInformation("Fetching statistics from the processor...");
		var stats = await processor.GetStatistics();
		return Ok(new
		{
			throughputPerSecond = stats.ThroughputPerSecond,
			startTime = stats.StartTime,
			endTime = stats.EndTime,
			processedCount = stats.ProcessedCount
		});
	}

	[HttpGet("statistics/detailed")]
	public async Task<IActionResult> GetDetailedStatistics()
	{
		logger.LogInformation("Detailed statistics requested");
		var stats = await processor.GetDetailedStatistics();
		return Ok(new
		{
			overall = new
			{
				startTime = stats.Overall.StartTime,
				endTime = stats.Overall.EndTime,
				totalProcessingTime = stats.Overall.TotalProcessingTime?.ToString(@"hh\:mm\:ss\.fff"),
				totalBatches = stats.Overall.TotalBatches,
				completedBatches = stats.Overall.CompletedBatches,
				activeBatches = stats.Overall.ActiveBatches,
				totalPoints = stats.Overall.TotalPoints,
				processedPoints = stats.Overall.ProcessedPoints,
				completionPercentage = stats.Overall.TotalPoints > 0 ?
					(double)stats.Overall.ProcessedPoints / stats.Overall.TotalPoints * 100 : 0
			},
			throughput = new
			{
				currentPointsPerSecond = Math.Round(stats.Throughput.CurrentPointsPerSecond, 2),
				averagePointsPerSecond = Math.Round(stats.Throughput.AveragePointsPerSecond, 2),
				peakPointsPerSecond = Math.Round(stats.Throughput.PeakPointsPerSecond, 2),
				batchesPerMinute = Math.Round(stats.Throughput.BatchesPerMinute, 2),
				averagePointsPerBatch = Math.Round(stats.Throughput.AveragePointsPerBatch, 0)
			},
			performance = new
			{
				averageBatchDuration = stats.Performance.AverageBatchDuration.ToString(@"hh\:mm\:ss\.fff"),
				fastestBatchDuration = stats.Performance.FastestBatchDuration.ToString(@"hh\:mm\:ss\.fff"),
				slowestBatchDuration = stats.Performance.SlowestBatchDuration.ToString(@"hh\:mm\:ss\.fff"),
				lastActivityTime = stats.Performance.LastActivityTime,
				timeSinceLastActivity = stats.Performance.TimeSinceLastActivity?.ToString(@"hh\:mm\:ss\.fff"),
				processingEfficiency = Math.Round(stats.Performance.ProcessingEfficiency, 2)
			},
			batches = stats.Batches.Select(b => new
			{
				batchId = b.BatchId,
				totalPoints = b.TotalPoints,
				processedPoints = b.ProcessedPoints,
				startTime = b.StartTime,
				endTime = b.EndTime,
				duration = b.Duration?.ToString(@"hh\:mm\:ss\.fff"),
				throughputPerSecond = b.ThroughputPerSecond,
				status = b.Status,
				completionPercentage = b.TotalPoints > 0 ?
					(double)b.ProcessedPoints / b.TotalPoints * 100 : 0
			})
		});
	}
}
