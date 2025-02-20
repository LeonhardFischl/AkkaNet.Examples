namespace AkkaNet.Examples.Restaurant.PoC.Events
{
	/// <summary>
	/// Represents a message that a customer enters the restaurant.
	/// </summary>
	public class CustomerEnter
	{
		public static CustomerEnter UnknownCustomer = new CustomerEnter("Neuer Kunde", new List<CustomerPreferences> { CustomerPreferences.Unbekannt }, new List<CustomerNeeds> { CustomerNeeds.Unbekannt});

		/// <summary>
		/// The name of the customer according to the reservation list
		/// </summary>
		public string CustomerName { get; }

		/// <summary>
		/// The preferences of the customer, e.g. vino, pasta, pizza, coffee
		/// </summary>
		public List<CustomerPreferences> Preferences { get; }

		/// <summary>
		/// The needs of the customer, e.g. vegetarian, tomato allergy, lactose intolerance
		/// </summary>
		public List<CustomerNeeds> Needs { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomerEnter"/> class.
		/// </summary>
		/// <param name="customerName"></param>
		/// <param name="preferences"></param>
		/// <param name="needs"></param>
		public CustomerEnter(string customerName, List<CustomerPreferences> preferences, List<CustomerNeeds> needs)
		{
			CustomerName = customerName;
			Preferences = preferences;
			Needs = needs;
		}
	}

	/// <summary>
	/// Represents a message that a customer exits the restaurant.
	/// </summary>
	/// <param name="CustomerName">Represents the customer`s name</param>
	public record CustomerExit(string CustomerName);
}
