using Polly;
using Polly.Extensions.Http;

namespace AirportDistanceWebAPIService.Extensions
{
  public static class ServiceColllectionExtensions
  {
    public static IServiceCollection AddHttpClientWithPoliceHandler(this IServiceCollection services, ConfigurationManager configuration)
    {
      var httpClientSettings = configuration.GetSection("HttpClientSettings");

      services.AddHttpClient("AirportInfoApi", client =>
      {
        client.BaseAddress = new Uri(uriString: httpClientSettings["AirportInfoApiBaseAddress"]);
        client.DefaultRequestHeaders.Add("Accept", httpClientSettings["AcceptHeader"]);
      })
      .AddPolicyHandler(HttpPolicyExtensions
          .HandleTransientHttpError()
          .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

      return services;
    }
  }
}
