using ChatServerApp.Protos;

using Grpc.Core;

using System.Collections.Concurrent;

namespace ChatServer.Services
{
	public class ChatRoomService
	{
		private static readonly ConcurrentDictionary<string, List<ChatClient>> _chatRooms = new ConcurrentDictionary<string, List<ChatClient>>();

		/// <summary>
		/// Read a single message from the client.
		/// </summary>
		/// <exception cref="ConnectionLostException"></exception>
		/// <exception cref="TimeoutException"></exception>
		public async Task<ClientMessage> ReadMessageWithTimeoutAsync(IAsyncStreamReader<ClientMessage> requestStream, TimeSpan timeout)
		{
			CancellationTokenSource cancellationTokenSource = new();

			cancellationTokenSource.CancelAfter(timeout);

			try
			{
				bool moveNext = await requestStream.MoveNext(cancellationTokenSource.Token);

				if (moveNext == false)
				{
					throw new Exception("connection dropped exception");
				}

				return requestStream.Current;
			}
			catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
			{
				throw new TimeoutException();
			}
		}

		/// <summary>
		/// <summary>
		/// </summary>
		/// <param name="chatRoomId"></param>
		/// <param name="user"></param>
		/// <returns></returns>

		public async Task AddClientToChatRoom(string chatRoomId, ChatClient chatClient)
		{
			if (!_chatRooms.ContainsKey(chatRoomId))
			{
				_chatRooms[chatRoomId] = new List<ChatClient> { chatClient };
			}
			else
			{
				var existingUser = _chatRooms[chatRoomId].FirstOrDefault(c => c.UserName == chatClient.UserName);
				if (existingUser != null)
				{
					// A user with the same user name already exists in the chat room
					throw new InvalidOperationException("User with the same name already exists in the chat room");
				}
				_chatRooms[chatRoomId].Add(chatClient);
			}

			await Task.CompletedTask;
		}
		/// <summary>
		/// Broad client joined the room message.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="chatRoomId"></param>
		/// <returns></returns>
		public async Task BroadcastClientJoinedRoomMessage(string userName, string chatRoomId)
		{
			if (_chatRooms.ContainsKey(chatRoomId))
			{
				var message = new ServerMessage { UserJoined = new ServerMessageUserJoined { UserName = userName } };

				var tasks = new List<Task>();

				foreach (var stream in _chatRooms[chatRoomId])
				{
					if (stream != null && stream != default)
					{
						tasks.Add(stream.StreamWriter.WriteAsync(message));
					}
				}

				await Task.WhenAll(tasks);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="chatRoomId"></param>
		/// <param name="senderName"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public async Task BroadcastMessageToChatRoom(string chatRoomId, string senderName, string text)
		{
			if (_chatRooms.ContainsKey(chatRoomId))
			{
				var message = new ServerMessage { Chat = new ServerMessageChat { UserName = senderName, Text = text } };

				var tasks = new List<Task>();
				var streamList = _chatRooms[chatRoomId];
				foreach (var stream in _chatRooms[chatRoomId])
				{
					//This senderName can be something of unique Id for each user.
					if (stream != null && stream != default && stream.UserName != senderName)
					{
						tasks.Add(stream.StreamWriter.WriteAsync(message));
					}
				}

				await Task.WhenAll(tasks);
			}
		}
	}

	public class ChatClient
	{
		public IServerStreamWriter<ServerMessage> StreamWriter { get; set; }
		public string UserName { get; set; }
	}
}