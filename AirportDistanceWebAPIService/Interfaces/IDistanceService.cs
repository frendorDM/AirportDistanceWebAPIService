using AirportDistanceWebAPIService.Models;

namespace AirportDistanceWebAPIService.Interfaces
{
  public interface IDistanceService
  {
    /// <summary>
    /// Calculates the distance between two airports.
    /// </summary>
    /// <param name="sourceIata">The IATA code of the first airport.</param>
    /// <param name="destinationIata">The IATA code of the second airport.</param>
    /// <returns>The calculated distance in miles.</returns>
    /// <param name="token"></param>
    Task<double> GetDistanceAsync(string sourceIata, string destinationIata, CancellationToken token);
  }
}
