namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class StartProcessing(string batchId, int totalPoints)
{
	public string BatchId { get; } = batchId;
	public int TotalPoints { get; } = totalPoints;
	public DateTime StartTime { get; } = DateTime.UtcNow;
}
