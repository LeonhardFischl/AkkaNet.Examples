using Akka.Actor;
using Akka.Routing;
using Akka.Streams;
using Akka.Streams.Dsl;

using AkkaNet.Examples.GrpcServiceExample.Actors;
using AkkaNet.Examples.GrpcServiceExample.Hubs;
using AkkaNet.Examples.GrpcServiceExample.Messages;
using AkkaNet.Examples.GrpcServiceExample.Models;

using Microsoft.AspNetCore.SignalR;

namespace AkkaNet.Examples.GrpcServiceExample.Processors;

public class CoordinateStreamProcessor
{
	private readonly ActorSystem _actorSystem;
	private readonly IActorRef _statisticsActor;
	private readonly IMaterializer _materializer;
	private readonly IHubContext<PointCloudHub> _hubContext;
	private readonly ILogger<CoordinateStreamProcessor> _logger;

	public CoordinateStreamProcessor(ActorSystem actorSystem,
		IHubContext<PointCloudHub> hubContext,
		ILogger<CoordinateStreamProcessor> logger)
	{
		_actorSystem = actorSystem;
		_statisticsActor = actorSystem.ActorOf(StatisticsActor.Props(), "statistics-actor");
		_materializer = _actorSystem.Materializer();
		_hubContext = hubContext;
		_logger = logger;
	}

	public async Task<CoordinateResponse> ProcessCoordinateArray(CoordinateArray coordinateArray)
	{
		var batchId = Guid.NewGuid().ToString("N")[..8];
		var points = coordinateArray.Points.ToList();

		_logger.LogInformation($"Processing batch {batchId} with {points.Count} points");

		// Start processing tracking
		_statisticsActor.Tell(new StartProcessing(batchId, points.Count));

		// Create processor actor pool with pool size based on CPU cores
		var poolSize = Environment.ProcessorCount *  4;
		// These are the Props for your worker actors (CoordinateProcessorActor)
		var processorProps = CoordinateProcessorActor.Props(_statisticsActor);
		// Attach the RoundRobinPool router to the worker Props
		var processorPool = _actorSystem.ActorOf(processorProps.WithRouter(new RoundRobinPool(poolSize)), $"processor-pool-{batchId}");

		try
		{
			var processedCount = await Source
				.From(points)
				.Select(point => new ProcessPoint(point, batchId))
				.Ask<PointProcessed>(processorPool, TimeSpan.FromSeconds(30))
				.RunWith(Sink.Seq<PointProcessed>(), _materializer);

			// Convert to visualization format and stream to clients
			await StreamPointsToClients(points, batchId);

			return new CoordinateResponse
			{
				PointsReceived = processedCount.Count,
				Message = $"Successfully processed {processedCount.Count} points in batch {batchId}"
			};
		}
		finally
		{
			// Clean up the processor pool
			await processorPool.GracefulStop(TimeSpan.FromSeconds(5));
		}
	}

	private async Task StreamPointsToClients(List<Point3D> points, string batchId)
	{
		try
		{
			// Convert to client format
			var pointData = points.Select(p => new PointData
			{
				X = (float)p.X,
				Y = (float)p.Y,
				Z = (float)p.Z,
				Color = GenerateColorFromPosition(p) // Optional: generate colors based on position
			}).ToList();

			var batch = new PointCloudBatch
			{
				BatchId = batchId,
				Points = pointData,
				Timestamp = DateTime.UtcNow,
				TotalPoints = points.Count,
				IsComplete = true
			};

			// Stream to all connected clients
			await _hubContext.Clients.All.SendAsync("ReceivePointBatch", batch);

			_logger.LogInformation($"Streamed {pointData.Count} points to visualization clients");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error streaming points to clients for batch {batchId}");
		}
	}
	private static uint GenerateColorFromPosition(Point3D point)
	{
		// Generate color based on Z-coordinate (height)
		var normalizedZ = Math.Max(0, Math.Min(1, (point.Z + 100) / 200)); // Normalize Z to 0-1

		// Create a blue-to-red gradient based on height
		var red = (byte)(normalizedZ * 255);
		var green = (byte)(128); // Constant green
		var blue = (byte)((1 - normalizedZ) * 255);

		return (uint)((red << 16) | (green << 8) | blue);
	}

	public async Task<StatisticsResponse> GetStatistics()
	{
		return await _statisticsActor.Ask<StatisticsResponse>(GetStatisticSingleton.Instance, TimeSpan.FromSeconds(5));
	}

	public async Task<DetailedStatisticsResponse> GetDetailedStatistics()
	{
		return await _statisticsActor.Ask<DetailedStatisticsResponse>(GetDetailedStatisticsSingleton.Instance, TimeSpan.FromSeconds(5));
	}
}
