using Akka.Actor;

using AkkaNet.Examples.GrpcServiceExample.Processors;
using AkkaNet.Examples.GrpcServiceExample.Services;

using Microsoft.AspNetCore.Server.Kestrel.Core;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.WebHost.ConfigureKestrel(options =>
		{
			// Mixed HTTP/1.1 and HTTP/2 endpoint on port 8082
			options.ListenAnyIP(80, listenOptions =>
			{
				listenOptions.Protocols = HttpProtocols.Http1;
			});

			// HTTP/2 endpoint on port 8080
			options.ListenAnyIP(8080, listenOptions =>
			{
				listenOptions.Protocols = HttpProtocols.Http2;
			});
		});

		// Add services to the container.
		builder.Services.AddGrpc();
		// Add controllers to support REST API
		builder.Services.AddControllers();

		// Create and register Akka system
		ActorSystem actorSystem = ActorSystem.Create("coordinate-system");
		builder.Services.AddSingleton(actorSystem);
		builder.Services.AddSingleton<CoordinateStreamProcessor>();

		builder.Services.AddLogging(loggingBuilder =>
		{
			loggingBuilder.AddConsole();
			loggingBuilder.AddDebug();
			loggingBuilder.SetMinimumLevel(LogLevel.Information);
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.MapGrpcService<GreeterService>();
		app.MapGrpcService<CoordinateGrpcService>();
		
		// For REST API endpoints
		app.MapControllers();

		// Add a simple health check endpoint
		// Health checks have been moved to the HomeController

		// Graceful shutdown
		var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
		lifetime.ApplicationStopping.Register(() =>
		{
			actorSystem.Terminate().Wait(TimeSpan.FromSeconds(10));
		});

		Console.WriteLine("Starting Coordinate Processor Server...");
		Console.WriteLine("HTTP/1.1 endpoint: http://localhost:80 (REST API, Health checks)");
		Console.WriteLine("HTTP/2 endpoint: http://localhost:8080 (gRPC only)");
		Console.WriteLine("Health check: http://localhost:80/health");
		Console.WriteLine("Statistics API: http://localhost:80/api/statistics");

		app.Run();
	}
}

