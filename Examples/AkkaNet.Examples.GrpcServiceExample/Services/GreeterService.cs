using Grpc.Core;

namespace AkkaNet.Examples.GrpcServiceExample.Services;

public class GreeterService : Greeter.GreeterBase
{
	private readonly ILogger<GreeterService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="GreeterService"/> class.
	/// </summary>
	/// <param name="logger">The logger instance for logging operations within the GreeterService.</param>
	public GreeterService(ILogger<GreeterService> logger)
	{
		this._logger = logger;
	}

	public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
	{
		_logger.LogInformation("Received SayHello request with name: {Name}", request.Name);
		//_remoteRos2MessageReceiveActor.Tell(request.Name);
		return Task.FromResult(new HelloReply
		{
			Message = "Hello " + request.Name
		});
	}
}
