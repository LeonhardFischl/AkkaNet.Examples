using Akka.Actor;
using Akka.Routing;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaNet.Examples.GrpcServiceExample.Actors;
using AkkaNet.Examples.GrpcServiceExample.Messages;

namespace AkkaNet.Examples.GrpcServiceExample.Processors;

public class CoordinateStreamProcessor
{
	private readonly ActorSystem _actorSystem;
	private readonly IActorRef _statisticsActor;
	private readonly IMaterializer _materializer;

	public CoordinateStreamProcessor(ActorSystem actorSystem)
	{
		_actorSystem = actorSystem;
		_statisticsActor = actorSystem.ActorOf(StatisticsActor.Props(), "statistics-actor");
		_materializer = _actorSystem.Materializer();
	}
	public async Task<CoordinateResponse> ProcessCoordinateArray(CoordinateArray coordinateArray)
	{
		var batchId = Guid.NewGuid().ToString("N")[..8];
		var points = coordinateArray.Points.ToList();

		// Start processing tracking
		_statisticsActor.Tell(new StartProcessing(batchId, points.Count));

		// Create processor actor pool
		// These are the Props for your worker actors (CoordinateProcessorActor)
		var processorProps = CoordinateProcessorActor.Props(_statisticsActor);
		// Attach the RoundRobinPool router to the worker Props
		var processorPool = _actorSystem.ActorOf(processorProps.WithRouter(new RoundRobinPool(Environment.ProcessorCount)), $"processor-pool-{batchId}");

		try
		{
			var processedCount = await Source
				.From(points)
				.Select(point => new ProcessPoint(point, batchId))
				.Ask<PointProcessed>(processorPool, TimeSpan.FromSeconds(30))
				.RunWith(Sink.Seq<PointProcessed>(), _materializer);

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

	public async Task<StatisticsResponse> GetStatistics()
	{
		return await _statisticsActor.Ask<StatisticsResponse>(GetStatisticSingleton.Instance, TimeSpan.FromSeconds(5));
	}
}
