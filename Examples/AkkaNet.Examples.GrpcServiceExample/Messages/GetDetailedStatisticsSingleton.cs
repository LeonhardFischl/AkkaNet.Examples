namespace AkkaNet.Examples.GrpcServiceExample.Messages;

public sealed class GetDetailedStatisticsSingleton
{
	public static GetDetailedStatisticsSingleton Instance { get; } = new GetDetailedStatisticsSingleton();
}