using Akka.Actor;
using Akka.Configuration;

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
		builder.Services.AddGrpc(options =>
		{
			options.MaxReceiveMessageSize = 50 * 1024 * 1024; // 50MB
			options.MaxSendMessageSize = 50 * 1024 * 1024;    // 50MB
			options.EnableDetailedErrors = false; // Disable for production
		});
		// Add controllers to support REST API
		builder.Services.AddControllers();

		var config = ConfigurationFactory.ParseString(@"
	    akka {
	        actor {
	            default-dispatcher {
	                type = Dispatcher
	                executor = fork-join-executor
	                fork-join-executor {
	                    parallelism-min = 8
	                    parallelism-factor = 2.0
	                    parallelism-max = 32
	                }
	                throughput = 50
	            }
	        }
	    }");

		// Create and register Akka system
		ActorSystem actorSystem = ActorSystem.Create("coordinate-system", config);
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

		app.MapGet("/health", (ILogger<Program> logger) =>
		{
			logger.LogInformation("Healthy. Service is running...");
			return Results.Ok("Healthy. Service is running...");
		});

		// Graceful shutdown
		var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
		lifetime.ApplicationStopping.Register(() =>
		{
			actorSystem.Terminate().Wait(TimeSpan.FromSeconds(10));
		});

		Console.WriteLine("Starting Coordinate Processor Server...");
		Console.WriteLine("HTTP/1.1 endpoint: http://localhost:80 (REST API, Health checks)");
		Console.WriteLine("HTTP/2 endpoint: http://localhost:8080 (gRPC only)");
		Console.WriteLine("Health check: http://localhost:80/api/health");
		Console.WriteLine("Version: http://localhost:80/api/version");
		Console.WriteLine("Basic statistics: http://localhost:80/api/statistics");
		Console.WriteLine("Detailed statistics: http://localhost:80/api/statistics/detailed");

		app.Run();
	}
}

