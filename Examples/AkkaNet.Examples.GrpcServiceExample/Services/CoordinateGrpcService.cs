using AkkaNet.Examples.GrpcServiceExample.Processors;

using Grpc.Core;

using System.Threading.Tasks;

namespace AkkaNet.Examples.GrpcServiceExample.Services;

public class CoordinateGrpcService : CoordinateService.CoordinateServiceBase
{
	private readonly CoordinateStreamProcessor _streamProcessor;

	public CoordinateGrpcService(CoordinateStreamProcessor streamProcessor)
	{
		_streamProcessor = streamProcessor;
	}

	public override async Task<CoordinateResponse> SendCoordinates(CoordinateArray request, ServerCallContext context)
	{
		return await _streamProcessor.ProcessCoordinateArray(request);
	}
}
