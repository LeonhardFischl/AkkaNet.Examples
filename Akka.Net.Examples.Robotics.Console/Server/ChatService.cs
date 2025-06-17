using Grpc.Core;

using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

public class ChatService : ChatServerBase
{
	private readonly ILogger<ChatService> _logger;

	public ChatService(ILogger<ChatService> logger)
	{
		_logger = logger;
	}

	public override Task HandleCommunication(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
	{
		return base.HandleCommunication(requestStream, responseStream, context);
	}
}