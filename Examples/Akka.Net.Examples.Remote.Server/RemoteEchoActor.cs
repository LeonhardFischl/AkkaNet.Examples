using Akka.Actor;

namespace Akka.Net.Examples.Remote.Server;

public class RemoteEchoActor : ReceiveActor
{
	public RemoteEchoActor()
	{
		Receive<string>(msg =>
		{
			Console.WriteLine($"Received message: {msg}");
			Sender.Tell(msg);  // Echoes the message back to the sender
		});
	}
}
