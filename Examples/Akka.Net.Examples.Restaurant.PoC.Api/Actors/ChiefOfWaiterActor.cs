using Akka.Actor;
using Akka.Net.Examples.Restaurant.PoC.Api.Events;

namespace Akka.Net.Examples.Restaurant.PoC.Api.Actors;

public class ChiefOfWaiterActor : ReceiveActor
{
	public ChiefOfWaiterActor()
	{
		Receive<GuideCustomer>(msg =>
		{
			// Logic to guide the customer to a quiet area
			Console.WriteLine($"Guiding {msg.Customer.Name} to a quiet area.");
		});
	}
}