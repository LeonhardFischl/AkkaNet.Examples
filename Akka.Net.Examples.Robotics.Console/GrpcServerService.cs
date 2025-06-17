using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Akka.Net.Examples.Robotics.Console;


public class GrpcServerService : BackgroundService
{
	private readonly ILogger<GrpcServerService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private Server _server;

	public GrpcServerService(ILogger<GrpcServerService> logger, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		// Create Akka.NET system
		var actorSystem = ActorSystem.Create("MessageSystem");
		var cluster = Cluster.Cluster.Get(actorSystem);
		_server = new Server
		{
			Services = { BindService(new MessageService(actorSystem, _serviceProvider.GetRequiredService<ILogger<MessageService>>())) },
			Ports = { new ServerPort("localhost", 8080, ServerCredentials.Insecure) }
		};

		_server.Start();
		_logger.LogInformation("gRPC server started on localhost:8080");

		try
		{
			await Task.Delay(-1, stoppingToken);
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Server shutdown requested");
		}
		finally
		{
			await _server.ShutdownAsync();
			await actorSystem.Terminate();
		}
	}
}
