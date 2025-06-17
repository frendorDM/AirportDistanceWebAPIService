using System.Text.Json;
using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Models;

namespace AirportDistanceWebAPIService.Services
{
  public class AirportService : IAirportService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AirportService> _logger;

    public AirportService(IHttpClientFactory httpClientFactory, ILogger<AirportService> logger)
    {
      _httpClientFactory = httpClientFactory;
      _logger = logger;
    }

    public async Task<AirportModel> GetAirportInfoAsync(string iataCode, CancellationToken token)
    {
      _logger.LogInformation($"Fetching information for airport with IATA code: {iataCode}");

      var client = _httpClientFactory.CreateClient("AirportInfoApi");

      HttpResponseMessage response;

      try
      {
        response = await client.GetAsync($"airports/{iataCode}", token);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        throw;
      }

      if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
      {
        _logger.LogWarning($"Failed to fetch airport data for {iataCode}. Status code: {response.StatusCode}");
        throw new InvalidInputDataException(await response.Content.ReadAsStringAsync(token));
      }
      if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
      {
        _logger.LogWarning($"Airport data not found. Status code: {response.StatusCode}");
        throw new NotFoundDataException(await response.Content.ReadAsStringAsync(token));
      }
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception(await response.Content.ReadAsStringAsync(token));
      }

      var options = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
      };

      var airportModel = await response.Content.ReadFromJsonAsync<AirportModel>(options, token);

      if (airportModel == null)
      {
        throw new Exception($"Received empty data for airport with IATA code: {iataCode}");
      }

      _logger.LogInformation("Successfully fetched airport data for {IataCode}.", iataCode);
      return airportModel;

    }
  }
}
