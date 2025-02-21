namespace Akka.Net.Examples.Restaurant.PoC.Api.Events;

public class Customer
{
	public string Name { get; set; }
	public bool Recognized { get; set; }  // Simple flag to determine if the customer is recognized
}

public class CustomerArrived
{
	public Customer Customer { get; }

	public CustomerArrived(Customer customer)
	{
		Customer = customer;
	}
}

public class GuideCustomer
{
	public Customer Customer { get; }

	public GuideCustomer(Customer customer)
	{
		Customer = customer;
	}
}
