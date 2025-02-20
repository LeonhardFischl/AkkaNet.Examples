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

	// Dummy data for customer preferences and needs
	// This is the holy grail for a real-world application: Your special knowledge about your own customers
	private readonly Dictionary<string, (List<CustomerPreferences> Preferences, List<CustomerNeeds> Needs)> _customerData;

	/// <summary>
	/// Initializes a new instance of the <see cref="BouncerActor"/> class<br />
	/// and starts the normal behavior.
	/// </summary>
	public BouncerActor(Dictionary<string, (List<CustomerPreferences> Preferences, List<CustomerNeeds> Needs)>? customerData)
	{
		_customerData = customerData ?? CreateDefaultData();
		NormalBehavior();
	}

	// Default data for the customers if no one is provided
	private Dictionary<string, (List<CustomerPreferences> Preferences, List<CustomerNeeds> Needs)> CreateDefaultData()
	{
		return new()
		{
			{
				"Alice",
				(
					new List<CustomerPreferences> { CustomerPreferences.Weinkarte },
					new List<CustomerNeeds> { CustomerNeeds.TomatenUnverträglichkeit }
				)
			},
			{
				"Bob",
				(
					new List<CustomerPreferences> { CustomerPreferences.Espresso },
					new List<CustomerNeeds> { CustomerNeeds.StarkeErdnussallergie}
				)
			},
			{
				"Thomas",
				(
					new List<CustomerPreferences> { CustomerPreferences.Schnaps },
					new List<CustomerNeeds> { CustomerNeeds.Unbekannt }
				)
			}
		};
	}


	private void NormalBehavior()
	{
		//Receive<CustomerEnter>(HandleCustomerEnters);
		Receive<string>(HandleKnownCustomer);

		Receive<CustomerExit>(HandleCustomerExit);
	}

	private void HandleKnownCustomer(string customerName)
	{
		if (_customerData.TryGetValue(customerName, out var data))
		{
			var message = new CustomerEnter(customerName, data.Preferences, data.Needs);
			var hasEntered = HandleCustomerEnters(message);
			if (!hasEntered)
				return;

			Context.System.EventStream.Publish(message);
		}
		else
		{
			HandleCustomerEnters(CustomerEnter.UnknownCustomer);
			Console.WriteLine($"Customer {customerName} is not recognized as a regular customer.");
		}
	}

	private void HandleCustomerExit(CustomerExit customer)
	{
		if (_currentCount <= 0)
			return;

		_currentCount--;
		Console.WriteLine($"Customer '{customer.CustomerName}' exited. Current count: {_currentCount}");

		if (_currentCount >= MaxCapacity)
			return;

		Console.WriteLine("Unstashing customers");
		Stash.UnstashAll();
	}

	private bool HandleCustomerEnters(CustomerEnter customer)
	{
		if (_currentCount < MaxCapacity)
		{
			_currentCount++;
			Console.WriteLine($"Customer '{customer.CustomerName}' entered. Current count: {_currentCount}");
			return true;
		}
		else
		{
			Console.WriteLine($"Restaurant full. '{customer.CustomerName}' is waiting, Stashing customer.");
			Stash.Stash();
			return false;
		}
	}
}