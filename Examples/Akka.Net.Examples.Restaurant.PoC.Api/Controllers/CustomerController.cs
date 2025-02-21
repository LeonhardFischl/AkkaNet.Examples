using Akka.Actor;
using Akka.Hosting;
using Akka.Net.Examples.Restaurant.PoC.Api.Actors;
using Akka.Net.Examples.Restaurant.PoC.Api.Events;

using Microsoft.AspNetCore.Mvc;

namespace Akka.Net.Examples.Restaurant.PoC.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
	private readonly IActorRef _bouncer;

	public CustomerController(IRequiredActor<BouncerActor> bouncerActor)
	{
		_bouncer = bouncerActor.ActorRef;
	}

	[HttpPost("arrive")]
	public IActionResult CustomerArrives([FromBody] Customer customer)
	{
		_bouncer.Tell(new CustomerArrived(customer));
		return Ok($"Customer {customer.Name} processed.");
	}
}
