using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirportDistanceWebAPIService.Controllers
{
  /// <summary>
  /// Controller for calculating distances between airports.
  /// </summary>
  [ApiController]
  [Route("api/[controller]")]
  public class DistanceController : ControllerBase
  {
    private readonly IDistanceService _distanceService;

    public DistanceController(IDistanceService distanceService)
    {
      _distanceService = distanceService;
    }

    /// <summary>
    /// Calculates the distance between two airports based on their IATA codes.
    /// </summary>
    /// <param name="sourceIata">The IATA code of the source airport.</param>
    /// <param name="destinationIata">The IATA code of the destination airport.</param>
    /// <response code="200">The distance was successfully calculated.</response>
    /// <response code="400">One or both of the IATA codes are missing or invalid.</response>
    /// <response code="500">An internal server error occurred while processing the request.</response>

    [HttpGet]
    public async Task<ActionResult<DistanceResponse>> GetDistance([FromQuery] string sourceIata, [FromQuery] string destinationIata, CancellationToken token)
    {
      if (string.IsNullOrWhiteSpace(sourceIata) || string.IsNullOrWhiteSpace(destinationIata))
      {
        throw new InvalidInputDataException("Both IATA codes must be provided.");
      }

      var distance = await _distanceService.GetDistanceAsync(sourceIata, destinationIata, token);

      return Ok(new DistanceResponse
      {
        Distance = distance,
        Success = true
      });
    }
  }
}
