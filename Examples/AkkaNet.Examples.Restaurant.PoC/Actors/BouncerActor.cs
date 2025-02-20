using Akka.Actor;

using AkkaNet.Examples.Restaurant.PoC.Events;

namespace AkkaNet.Examples.Restaurant.PoC.Actors;

/// <summary>
/// Is the actor responsible for managing the entrance to the restaurant.
/// </summary>
public class BouncerActor : ReceiveActor, IWithUnboundedStash
{
	// Maximale Kapazität des Restaurants
	private const int MaxCapacity = 10;
	// Anzahl der im Restaurant befindlichen Kunden
	private int _currentCount = 0;

	/// <inheritdoc cref="IWithUnboundedStash"/>
	public IStash Stash { get; set; } = null!;

	/// <summary>
	/// Initializes a new instance of the <see cref="BouncerActor"/> class<br />
	/// and starts the normal behavior.
	/// </summary>
	public BouncerActor()
	{
		NormalBehavior();
	}


	private void NormalBehavior()
	{
		Receive<CustomerEnter>(HandleCustomerEnters);

		Receive<CustomerExit>(HandleCustomerExit);
	}

	private void HandleCustomerExit(CustomerExit _)
	{
		if (_currentCount <= 0)
			return;

		_currentCount--;
		Console.WriteLine($"Customer exited. Current count: {_currentCount}");

		if (_currentCount >= MaxCapacity)
			return;

		Console.WriteLine("Unstashing customers");
		Stash.UnstashAll();
	}

	private void HandleCustomerEnters(CustomerEnter _)
	{
		if (_currentCount < MaxCapacity)
		{
			_currentCount++;
			Console.WriteLine($"Customer entered. Current count: {_currentCount}");
		}
		else
		{
			Console.WriteLine($"Restaurant full. Stashing customer.");
			Stash.Stash();
		}
	}
}