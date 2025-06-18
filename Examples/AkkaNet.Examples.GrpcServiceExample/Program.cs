using Akka.Actor;
using Akka.Hosting;

using AkkaNet.Examples.GrpcServiceExample.Actors;
using AkkaNet.Examples.GrpcServiceExample.Services;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		// Add services to the container.
		builder.Services.AddGrpc();
		builder.Services.AddLogging(loggingBuilder =>
		{
			loggingBuilder.AddConsole();
			loggingBuilder.AddDebug();
		});


		//builder.Services.AddAkka("Ros2Python2AkkaNetClusterSystem", (akkaConfigurationBuilder, provider) =>
		//{
		//	// Configure your Akka.NET system here
		//	akkaConfigurationBuilder.WithActors((akkaConfiguration, registry, _) =>
		//	{
		//		// Register your actors here
		//		akkaConfiguration.ActorOf<RemoteRos2MessageReceiveActor>("remoteRos2MessageReceiveActor");
		//	});
		//});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.MapGrpcService<GreeterService>();
		app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

		app.Run();
	}
}