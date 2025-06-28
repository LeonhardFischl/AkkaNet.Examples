using Akka.Actor;

using AkkaNet.Examples.GrpcServiceExample.Messages;

namespace AkkaNet.Examples.GrpcServiceExample.Actors;

public class CoordinateProcessorActor : ReceiveActor
{
	private readonly IActorRef _statisticsActor;

	public CoordinateProcessorActor(IActorRef statisticsActor)
	{
		_statisticsActor = statisticsActor;

		Receive<ProcessPoint>(HandleProcessPoint);
	}

	private void HandleProcessPoint(ProcessPoint message)
	{
		// Simulate some processing work
		var point = message.Point;

		// Example processing: calculate distance from origin
		var distance = Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);

		// Log processing (optional)
		Console.WriteLine($"Processed point ({point.X:F2}, {point.Y:F2}, {point.Z:F2}) - Distance: {distance:F2}");

		// Notify statistics actor
		_statisticsActor.Tell(new PointCompleted(message.BatchId));

		// Send back confirmation
		Sender.Tell(new PointProcessed(point, message.BatchId));
	}

	public static Props Props(IActorRef statisticsActor) =>
		Akka.Actor.Props.Create(() => new CoordinateProcessorActor(statisticsActor));
}
