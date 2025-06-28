namespace AkkaNet.Examples.GrpcServiceExample.Models;

public class PointData
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public uint Color { get; set; } = 0xFFFFFF; // Default white
}

public class PointCloudBatch
{
	public string BatchId { get; set; }
	public List<PointData> Points { get; set; } = new();
	public DateTime Timestamp { get; set; }
	public int TotalPoints { get; set; }
	public bool IsComplete { get; set; }
}

public class VisualizationStats
{
	public int TotalPointsProcessed { get; set; }
	public int BatchesProcessed { get; set; }
	public double AverageProcessingTime { get; set; }
	public DateTime LastUpdate { get; set; }
}
