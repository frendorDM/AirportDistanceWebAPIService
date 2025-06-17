using AirportDistanceWebAPIService.Helpers;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AirportDistanceWebAPIService.Services
{
  public class DistanceService : IDistanceService
  {
    private readonly IAirportService _airportService;
    private readonly ILogger<DistanceService> _logger;
    private readonly IMemoryCache _cache;

    public DistanceService(IAirportService airportService, ILogger<DistanceService> logger, IMemoryCache cache)
    {
      _airportService = airportService;
      _logger = logger;
      _cache = cache;
    }

    public async Task<double> GetDistanceAsync(string sourceIata, string destinationIata, CancellationToken token)
    {
      _logger.LogInformation($"Calculating distance between airports: {sourceIata} and {destinationIata}");

      var (coordinate1, coordinate2) = await GetAirportsCoordinates(sourceIata, destinationIata, token); ;

      var distance = DistanceHelper
        .CalculateHaversineDistance(
          coordinate1,
          coordinate2
          );

      _logger.LogInformation($"Calculated distance: {distance} miles");

      return distance;
    }

    private async Task<(AirportCoordinates, AirportCoordinates)> GetAirportsCoordinates(string sourceIata, string destinationIata, CancellationToken token)
    {
      var tasks = new[]
      {
        GetAirportCoordinates(sourceIata, token),
        GetAirportCoordinates(destinationIata, token)
      };

      var results = await Task.WhenAll(tasks);
      return (results[0], results[1]);
    }

    private async Task<AirportCoordinates> GetAirportCoordinates(string iataCode, CancellationToken token)
    {
      if (!_cache.TryGetValue(iataCode, out AirportCoordinates coordinate))
      {
        var airportInfo = await _airportService.GetAirportInfoAsync(iataCode, token);
        coordinate = new AirportCoordinates(airportInfo.Location.Lat, airportInfo.Location.Lon);

        _cache.Set(iataCode, coordinate);
      }

      return coordinate;
    }
  }
}