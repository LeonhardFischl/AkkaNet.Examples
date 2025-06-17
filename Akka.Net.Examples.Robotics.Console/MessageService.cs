using Akka.Actor;

using Grpc.Core;
using Grpc.AspNetCore.Server;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace Akka.Net.Examples.Robotics.Console;
public class MessageService : MessageServiceBase
{
	private readonly ActorSystem _actorSystem;
	private readonly IActorRef _messageActor;
	private readonly ILogger<MessageService> _logger;

	public MessageService(ActorSystem actorSystem, ILogger<MessageService> logger)
	{
		_actorSystem = actorSystem;
		_logger = logger;
		_messageActor = _actorSystem.ActorOf(MessageActor.Props(logger), "message-actor");
	}

	public override Task<MessageResponse> SendMessage(MessageRequest request, ServerCallContext context)
	{
		// Fire and forget - send to actor without waiting for response
		_messageActor.Tell(new MessageReceived(request.Content, request.Sender));
		return Task.FromResult(new MessageResponse { Success = true });
	}
}