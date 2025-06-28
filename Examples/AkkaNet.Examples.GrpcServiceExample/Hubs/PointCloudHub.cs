namespace AkkaNet.Examples.GrpcServiceExample.Hubs;

using Microsoft.AspNetCore.SignalR;


public class PointCloudHub : Hub
{
	public async Task JoinVisualizationRoom(string roomId = "default")
	{
		await Groups.AddToGroupAsync(Context.ConnectionId, $"pointcloud-{roomId}");
		await Clients.Caller.SendAsync("JoinedRoom", roomId);
	}

	public async Task LeaveVisualizationRoom(string roomId = "default")
	{
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"pointcloud-{roomId}");
	}

	public override async Task OnDisconnectedAsync(Exception exception)
	{
		// Cleanup logic if needed
		await base.OnDisconnectedAsync(exception);
	}
}
