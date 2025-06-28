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

	[HttpGet("/health")]
	public IActionResult HealthCheck()
	{
		logger.LogInformation("Healthy. Service is running...");
		return Ok("Healthy. Service is running...");
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
}
