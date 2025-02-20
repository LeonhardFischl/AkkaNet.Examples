using Akka.Actor;
using AkkaNet.Examples.Restaurant.PoC.Events;

namespace AkkaNet.Examples.Restaurant.PoC.Actors
{
	/// <summary>
	/// Represents a staff member of the restaurant who likes to process the information about customers.
	/// </summary>
	public class StaffActor : ReceiveActor
	{
		private readonly string _role;

		/// <summary>
		/// Initializes a new instance of the <see cref="StaffActor"/> class.
		/// </summary>
		/// <param name="role">A barkeeper, cook, waiter or any staff member</param>
		public StaffActor(string role)
		{
			_role = role;
			Receive<CustomerEnter>(HandleCustomerEnter);
		}

		private void HandleCustomerEnter(CustomerEnter info)
		{
			Console.WriteLine($"[{_role}] Processing information for {info.CustomerName}: Preferences - {string.Join(", ", info.Preferences)}, Needs - {string.Join(", ", info.Needs)}.");
		}
	}
}
