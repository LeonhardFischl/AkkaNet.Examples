namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class PointProcessed(Point3D point, string batchId)
{
	public Point3D Point { get; } = point;
	public string BatchId { get; } = batchId;
}
