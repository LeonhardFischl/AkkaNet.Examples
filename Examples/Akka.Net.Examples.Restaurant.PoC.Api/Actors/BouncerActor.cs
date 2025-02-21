using Akka.Actor;
using Akka.Net.Examples.Restaurant.PoC.Api.Events;
using static Akka.IO.Tcp;

namespace Akka.Net.Examples.Restaurant.PoC.Api.Actors;

public class BouncerActor : ReceiveActor
{
	private readonly IActorRef _chiefOfWaiter;
	private readonly IActorRef[] _waiters;

	public BouncerActor(IActorRef chiefOfWaiter, IActorRef[] waiters)
	{
		_chiefOfWaiter = chiefOfWaiter;
		_waiters = waiters;

		Receive<CustomerArrived>(msg =>
		{
			if (msg.Customer.Recognized)
			{
				_chiefOfWaiter.Tell(new GuideCustomer(msg.Customer));
			}
			else
			{
				foreach (var waiter in _waiters)
				{
					waiter.Tell(new GuideCustomer(msg.Customer));
				}
			}
		});
	}
}