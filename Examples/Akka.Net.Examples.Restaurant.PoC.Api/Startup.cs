using Akka.Actor;
using Akka.Hosting;
using Akka.Net.Examples.Restaurant.PoC.Api.Actors;

namespace Akka.Net.Examples.Restaurant.PoC.Api;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers();

		services.AddAkka("RestaurantSystem", (configurationBuilder, provider) =>
		{
			// configurationBuilder.AddHoconFile(... ) // Add configuration file
			// configurationBuilder.WithCluster(... ) // Needs Akka.Cluster dependency

			configurationBuilder.WithActors((system, registry, _) =>
			{
				var chiefOfWaiter = system.ActorOf<ChiefOfWaiterActor>("chiefOfWaiter");
				var waiters = new IActorRef[]
				{
					system.ActorOf<WaiterActor>("waiter1"),
					system.ActorOf<WaiterActor>("waiter2"),
					system.ActorOf<WaiterActor>("waiter3")
				};

				var bouncerActor = system.ActorOf(Props.Create(() => new BouncerActor(chiefOfWaiter, waiters)), "bouncer");
				registry.Register<BouncerActor>(bouncerActor);
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}
