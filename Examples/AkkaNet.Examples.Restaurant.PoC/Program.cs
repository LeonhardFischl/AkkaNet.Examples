using Akka.Actor;

using AkkaNet.Examples.Restaurant.PoC.Actors;
using AkkaNet.Examples.Restaurant.PoC.Events;

namespace AkkaNet.Examples.Restaurant.PoC;

class Program
{
	public static void Main(string[] args)
	{
		// Aktorsystem erstellen
		using var system = ActorSystem.Create("RestaurantSystem");

		// Akteur erstellen
		var bouncerActor = system.ActorOf<BouncerActor>("BouncerActor");

		// Nachrichten an den Akteur senden
		for (int i = 0; i < 12; i++)
		{
			bouncerActor.Tell(new CustomerEnter());
		}

		bouncerActor.Tell(new CustomerExit());

		// Warte, damit die Ausgabe sichtbar bleibt
		Console.ReadLine();
	}
}
