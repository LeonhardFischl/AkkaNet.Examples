using Akka.Actor;

namespace AkkaNet.Examples.PingPong;

class Program
{
	static void Main(string[] args)
	{
		// Create the actor system
		using var system = ActorSystem.Create("PingPongSystem");

		// Create the actors (and automatically start them)
		var pong = system.ActorOf(Props.Create(() => new PongActor()), "Ponger");
		var ping = system.ActorOf(Props.Create(() => new PingActor(pong)), "Pinger");

		Console.ReadLine();
	}
}

/// <summary>
/// First actor for the ping-pong system
/// </summary>
public class PingActor : ReceiveActor
{
	private int _count;
	private readonly IActorRef _pong;

	public PingActor(IActorRef pong)
	{
		this._pong = pong;
		this.Receive<string>(message =>
		{
			if (message == "Pong")
			{
				Console.WriteLine("Received Pong");

				if (this._count < 5)
				{
					this._count++;
					this._pong.Tell("Ping");
				}
			}
		});
	}

	protected override void PreStart()
	{
		// Start interaction by sending initial Ping
		this._pong.Tell("Ping");
	}
}

/// <summary>
/// Second actor for the ping-pong system
/// </summary>
public class PongActor : ReceiveActor
{
	public PongActor()
	{
		this.Receive<string>(message =>
		{
			if (message == "Ping")
			{
				Console.WriteLine("Received Ping");
				this.Sender.Tell("Pong");
			}
		});
	}
}