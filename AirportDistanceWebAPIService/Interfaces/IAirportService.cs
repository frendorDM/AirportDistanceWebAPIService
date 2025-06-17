using AirportDistanceWebAPIService.Models;

namespace AirportDistanceWebAPIService.Interfaces
{
  public interface IAirportService
  {
    /// <summary>
    /// Asynchronously fetches information about an airport using its IATA code.
    /// </summary>
    /// <param name="iataCode">The IATA code of the airport.</param>
    /// <returns>Returns an instance of the <see cref="AirportModel"/> if the data is successfully retrieved, otherwise returns null.</returns>
    /// <param name="token"></param>
    Task<AirportModel> GetAirportInfoAsync(string iataCode, CancellationToken token);
  }
}
