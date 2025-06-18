using Akka.Actor;

namespace AkkaNet.Examples.GrpcServiceExample.Actors;

/// <summary>
/// Actor that receives remote ROS2 messages and echoes them back to the sender.
/// This actor provides a simple echo service for string messages received from remote systems.
/// </summary>
public class RemoteRos2MessageCounterActor : ReceiveActor
{
	private int _messageCount = 0;

	/// <summary>
	/// Initializes a new instance of the <see cref="RemoteRos2MessageReceiveActor"/> class.
	/// </summary>
	/// <param name="logger">The logger used to log information about received messages.</param>
	public RemoteRos2MessageCounterActor(ILogger<RemoteRos2MessageCounterActor> logger)
	{
		Receive<string>(msg =>
		{
			if (_messageCount == 0)
			{
				logger.LogInformation("RemoteRos2MessageCounterActor started.");
			}

			if (_messageCount % 1000 == 0)
			{
				logger.LogInformation("Received {MessageCount} message: {Msg}", _messageCount, msg);
			}
		});
	}


	protected override void PreStart()
	{
		base.PreStart();
		Console.WriteLine("RemoteRos2MessageCounterActor started.");
	}
}