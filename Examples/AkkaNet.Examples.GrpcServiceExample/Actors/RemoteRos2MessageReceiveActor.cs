using Akka.Actor;

namespace AkkaNet.Examples.GrpcServiceExample.Actors;

/// <summary>
/// Actor that receives remote ROS2 messages and echoes them back to the sender.
/// This actor provides a simple echo service for string messages received from remote systems.
/// </summary>
public class RemoteRos2MessageReceiveActor : ReceiveActor
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RemoteRos2MessageReceiveActor"/> class.
	/// </summary>
	/// <param name="logger">The logger used to log information about received messages.</param>
	public RemoteRos2MessageReceiveActor()
	{
		Receive<string>(msg =>
		{
			Console.WriteLine($"Received message: {msg}");
		});
	}
}