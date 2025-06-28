using AkkaNet.Examples.GrpcServiceExample.Models;
using AkkaNet.Examples.GrpcServiceExample.Processors;

using Microsoft.AspNetCore.Mvc;

namespace AkkaNet.Examples.GrpcServiceExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoordinatesController : ControllerBase
{
	private readonly ILogger<CoordinatesController> _logger;
	private readonly CoordinateStreamProcessor _streamProcessor;

	public CoordinatesController(ILogger<CoordinatesController> logger, CoordinateStreamProcessor streamProcessor)
	{
		_logger = logger;
		_streamProcessor = streamProcessor;
	}

	[HttpPost("process")]
	public async Task<IActionResult> ProcessCoordinates([FromBody] CoordinateArrayDto coordinateArray)
	{
		try
		{
			_logger.LogInformation("Processing {PointCount} coordinates via REST API", coordinateArray.Points.Length);

			// Convert DTO to protobuf message
			var protoArray = new CoordinateArray();
			foreach (var point in coordinateArray.Points)
			{
				protoArray.Points.Add(new Point3D { X = point.X, Y = point.Y, Z = point.Z });
			}

			var response = await _streamProcessor.ProcessCoordinateArray(protoArray);

			_logger.LogInformation("Successfully processed {PointCount} coordinates", response.PointsReceived);

			return Ok(new
			{
				pointsReceived = response.PointsReceived,
				message = response.Message
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error processing coordinates");
			return BadRequest(new { error = ex.Message });
		}
	}
}