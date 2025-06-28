namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class PointCompleted(string batchId)
{
	public string BatchId { get; } = batchId;
	public DateTime CompletedAt { get; } = DateTime.UtcNow;
}
