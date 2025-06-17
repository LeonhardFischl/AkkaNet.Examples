using Akka.Actor;

using Microsoft.Extensions.Logging;

namespace Akka.Net.Examples.Robotics.Console;
public class MessageActor : ReceiveActor
{
	private readonly ILogger<MessageActor> _logger;

	public MessageActor(ILogger<MessageActor> logger)
	{
		_logger = logger;
		Receive<MessageReceived>(msg =>
		{
			_logger.LogInformation($"Received message from {msg.Sender}: {msg.Content}");
			// Process message here - no response needed
		});
	}

	public static Props Props(ILogger<MessageActor> logger) =>
		global::Akka.Actor.Props.Create(() => new MessageActor(logger));
}
