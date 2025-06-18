using Akka.Actor;
using Akka.Hosting;

using AkkaNet.Examples.GrpcServiceExample.Actors;

using Grpc.Core;

namespace AkkaNet.Examples.GrpcServiceExample.Services;

public class GreeterService : Greeter.GreeterBase
{
	private readonly ILogger<GreeterService> _logger;
	//private readonly IActorRef _remoteRos2MessageReceiveActor;

	/// <summary>
	/// Initializes a new instance of the <see cref="GreeterService"/> class.
	/// </summary>
	/// <param name="logger">The logger instance for logging operations within the GreeterService.</param>
	/// <param name="remoteRos2MessageReceiveActor">The required actor wrapper containing the ActorRef for handling remote ROS2 message reception.</param>
	public GreeterService(ILogger<GreeterService> logger /*, IRequiredActor<RemoteRos2MessageReceiveActor> remoteRos2MessageReceiveActor*/)
	{
		this._logger = logger;
		// Get the ActorRef from the required actor wrapper
		//this._remoteRos2MessageReceiveActor = remoteRos2MessageReceiveActor.ActorRef; 
		//this._remoteRos2MessageReceiveActor = remoteRos2MessageReceiveActor.GetAsync().Result;
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
