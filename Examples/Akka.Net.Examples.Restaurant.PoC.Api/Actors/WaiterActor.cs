using Akka.Actor;
using Akka.Net.Examples.Restaurant.PoC.Api.Events;

namespace Akka.Net.Examples.Restaurant.PoC.Api.Actors;

public class WaiterActor : ReceiveActor
{
	public WaiterActor()
	{
		Receive<GuideCustomer>(msg =>
		{
			// Logic to guide the customer to a table
			Console.WriteLine($"Guiding {msg.Customer.Name} to a table.");
		});
	}
}