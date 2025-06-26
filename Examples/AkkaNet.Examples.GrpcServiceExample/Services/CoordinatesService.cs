using Akka.Actor;
using Akka.Hosting;

using AkkaNet.Examples.GrpcServiceExample.Actors;

using Grpc.Core;

namespace AkkaNet.Examples.GrpcServiceExample.Services;

public class CoordinatesService : CoordinateService.CoordinateServiceBase
{
	private readonly ILogger<CoordinatesService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="CoordinatesService"/> class.
	/// </summary>
	/// <param name="logger">The logger instance for logging operations within the GreeterService.</param>
	public CoordinatesService(ILogger<CoordinatesService> logger)
	{
		this._logger = logger;
	}

	public override Task<CoordinateResponse> SendCoordinates(CoordinateArray request, ServerCallContext context)
	{
		_logger.LogInformation($"Received {nameof(CoordinateArray)} as request with amount of points: {request.Points.Count}");
		
		return Task.FromResult(new CoordinateResponse
		{
			Message = $"Received {request.Points.Count} points",
			PointsReceived = request.Points.Count
		});
	}
}
