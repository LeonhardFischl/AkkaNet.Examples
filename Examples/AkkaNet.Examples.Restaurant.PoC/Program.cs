using Akka.Actor;

using AkkaNet.Examples.Restaurant.PoC.Actors;
using AkkaNet.Examples.Restaurant.PoC.Events;

namespace AkkaNet.Examples.Restaurant.PoC;

class Program
{
	public static void Main(string[] args)
	{
		// Aktorsystem erstellen
		using var system = ActorSystem.Create("restaurantSystem");
		
		// Dummy-Daten für Kunden laden - in der realen Welt kommt das aus einer Datenbank oder einem anderen System
		var customerData = PrepareSpecialGuestList();

		// Türsteher erstellen
		var bouncerActor = system.ActorOf(Props.Create(() => new BouncerActor(customerData)), "bouncerActor");

		// Staff Members erstellen und abonnieren
		var chef = system.ActorOf(Props.Create(() => new StaffActor("Chef")), "chef");
		var cook1 = system.ActorOf(Props.Create(() => new StaffActor("Cook1")), "cook1");
		var cook2 = system.ActorOf(Props.Create(() => new StaffActor("Cook2")), "cook2");

		// Kellner erstellen
		var waiters = new List<IActorRef>();
		for (int i = 1; i <= 5; i++)
		{
			waiters.Add(system.ActorOf(Props.Create(() => new StaffActor($"Waiter{i}")), $"waiter{i}"));
		}

		// Barkeeper erstellen
		var barkeeper1 = system.ActorOf(Props.Create(() => new StaffActor("Barkeeper1")), "barkeeper1");
		var barkeeper2 = system.ActorOf(Props.Create(() => new StaffActor("Barkeeper2")), "barkeeper2");

		// Abonnieren der Türsteher-Nachrichten
		system.EventStream.Subscribe(chef, typeof(CustomerEnter));
		system.EventStream.Subscribe(cook1, typeof(CustomerEnter));
		system.EventStream.Subscribe(cook2, typeof(CustomerEnter));
		foreach (var waiter in waiters)
		{
			system.EventStream.Subscribe(waiter, typeof(CustomerEnter));
		}
		system.EventStream.Subscribe(barkeeper1, typeof(CustomerEnter));
		system.EventStream.Subscribe(barkeeper2, typeof(CustomerEnter));

		for (int i = 0; i < 12; i++)
		{
			bouncerActor.Tell(customerData.ElementAt(i).Key);
		}
		
		bouncerActor.Tell(new CustomerExit("Michael"));

		bouncerActor.Tell(new CustomerExit("Emma"));

		// Warte, damit die Ausgabe sichtbar bleibt
		Console.ReadLine();
	}

	private static Dictionary<string, (List<CustomerPreferences>, List<CustomerNeeds>)> PrepareSpecialGuestList()
	{
		// Dummy-Daten für Kunden - in der realen Welt kommt das aus einer Datenbank oder einem anderen System
		var customerNames = new[] { "Alice", "John", "Jane", "Michael", "Bob", "Sarah", "David", "Emma", "Thomas", "Chris", "Olivia", "Daniel" };
		var random = new Random();
		var customerPreferences = Enum.GetValues(typeof(CustomerPreferences)).Cast<CustomerPreferences>().ToArray();
		var customerNeeds = Enum.GetValues(typeof(CustomerNeeds)).Cast<CustomerNeeds>().ToArray();

		var dict = new Dictionary<string, (List<CustomerPreferences>, List<CustomerNeeds>)>();
		for (int i = 0; i < 12; i++)
		{
			var randomPreferences = new List<CustomerPreferences>();
			var randomNeeds = new List<CustomerNeeds>();

			randomPreferences.Add(customerPreferences[random.Next(customerPreferences.Length)]);
			randomNeeds.Add(customerNeeds[random.Next(customerNeeds.Length)]);

			dict.Add(customerNames[i], (randomPreferences, randomNeeds));
		}
		return dict;
	}
}
