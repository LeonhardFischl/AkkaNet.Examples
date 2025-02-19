using Akka.Actor;

using AkkaNet.Examples.Tennis.Actors;
using AkkaNet.Examples.Tennis.Events;

namespace AkkaNet.Examples.Tennis;

class Program
{
	static void Main(string[] args)
	{
		using var system = ActorSystem.Create("TennisMatchSystem");
		var player1 = system.ActorOf(Props.Create(() => new PlayerActor("Player 1")), "player1");
		var player2 = system.ActorOf(Props.Create(() => new PlayerActor("Player 2")), "player2");
		var match = system.ActorOf(Props.Create(() => new MatchActor(player1, player2)), "match");

		match.Tell(new StartMatch());

		Console.ReadLine();
	}
}
