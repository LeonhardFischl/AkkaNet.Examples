namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class GetStatisticSingleton
{
	public static GetStatisticSingleton Instance { get; } = new();
}