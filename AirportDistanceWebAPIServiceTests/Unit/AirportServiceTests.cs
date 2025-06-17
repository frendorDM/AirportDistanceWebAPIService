using System.Net;
using System.Text;
using System.Text.Json;
using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Models;
using AirportDistanceWebAPIService.Services;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.CodeQuality.Test.Helpers.Http;

namespace AirportDistanceWebAPIServiceTests.Unit
{
  public class AirportServiceTests
  {
    private readonly Mock<IHttpClientFactory> _mockClientFactory;
    private readonly AirportService _airportService;
    private readonly Mock<ILogger<AirportService>> _mockLogger;
    private readonly IFixture _fixture;

    public AirportServiceTests()
    {
      _mockClientFactory = new Mock<IHttpClientFactory>();
      _mockLogger = new Mock<ILogger<AirportService>>();
      _airportService = new AirportService(_mockClientFactory.Object, _mockLogger.Object);
      _fixture = new Fixture();
    }

    [Fact]
    public async Task ShouldReturnAirportModel()
    {
      // Arrange
      var content = $$"""
        {
            "iata": "EWR",
            "name": "Newark",
            "city": "Newark",
            "city_iata": "NYC",
            "icao": "KEWR",
            "country": "United States",
            "country_iata": "US",
            "location": {
                "lon": -74.178753,
                "lat": 40.689071
            },
            "rating": 3,
            "hubs": 2,
            "timezone_region_name": "America/New_York",
            "type": "airport"
        }
        """;

      SetupHttpClientMock(content, HttpStatusCode.OK);

      var options = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
      };

      var airportModel = JsonSerializer.Deserialize<AirportModel>(content, options);

      // Act
      var result = await _airportService.GetAirportInfoAsync("AAA", CancellationToken.None);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(airportModel);
    }

    [Fact]
    public async Task ShouldThrowInvalidInputDataExceptionWhenHttpClientReturnsBadRequest()
    {
      // Arrange      
      SetupHttpClientMock("Some validation error", HttpStatusCode.BadRequest);

      // Act
      var result = () => _airportService.GetAirportInfoAsync("AAA", CancellationToken.None);

      // Assert
      await result
        .Should().ThrowAsync<InvalidInputDataException>()
        .WithMessage("Some validation error");
    }

    [Fact]
    public async Task ShouldThrowNotFoundDataExceptionWhenHttpClientReturnsNotFound()
    {
      // Arrange      
      SetupHttpClientMock("NotFound error", HttpStatusCode.NotFound);

      // Act
      var result = () => _airportService.GetAirportInfoAsync("AAA", CancellationToken.None);

      // Assert
      await result
        .Should().ThrowAsync<NotFoundDataException>()
        .WithMessage("NotFound error");
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenRequestFaild()
    {
      // Arrange      
      SetupHttpClientMock("Some error", HttpStatusCode.InternalServerError);

      // Act
      var result = () => _airportService.GetAirportInfoAsync("AAA", CancellationToken.None);

      // Assert
      await result
        .Should().ThrowAsync<Exception>()
        .WithMessage("Some error");
    }

    private void SetupHttpClientMock(string content, HttpStatusCode statusCode)
    {
      var builder = new HttpClientMockBuilder();

      var httpClient = builder
          .WithBaseAddress(new Uri("http://host"))
          .WithRequest($"airports/AAA")
          .Responding(request =>
          {
            var responseContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
            responseContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return new HttpResponseMessage
            {
              StatusCode = statusCode,
              Content = responseContent
            };
          })
      .Build();

      _mockClientFactory.Setup(s => s.CreateClient("AirportInfoApi")).Returns(httpClient);
    }
  }
}
