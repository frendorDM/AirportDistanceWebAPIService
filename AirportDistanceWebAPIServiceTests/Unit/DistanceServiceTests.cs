using AirportDistanceWebAPIService.Helpers;
using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Models;
using AirportDistanceWebAPIService.Services;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AirportDistanceWebAPIServiceTests.Unit
{
  public class DistanceServiceTests
  {
    private readonly Mock<IAirportService> _mockAirportService;
    private readonly Mock<ILogger<DistanceService>> _mockLogger;
    private readonly Mock<IMemoryCache> _mockInMemoryCache;
    private readonly DistanceService _distanceService;
    private readonly IFixture _fixture;

    public DistanceServiceTests()
    {
      _mockAirportService = new Mock<IAirportService>();
      _mockLogger = new Mock<ILogger<DistanceService>>();
      _mockInMemoryCache = new Mock<IMemoryCache>();
      _distanceService = new DistanceService(_mockAirportService.Object, _mockLogger.Object, _mockInMemoryCache.Object);
      _fixture = new Fixture();
    }

    [Theory]
    [AutoData]
    public async Task ShouldReturnDistance(AirportModel sourceAirport, AirportModel destinationAirport)
    {
      // Arrange
      _mockAirportService.Setup(s => s.GetAirportInfoAsync(sourceAirport.Iata, CancellationToken.None)).ReturnsAsync(sourceAirport);
      _mockAirportService.Setup(s => s.GetAirportInfoAsync(destinationAirport.Iata, CancellationToken.None)).ReturnsAsync(destinationAirport);
      _mockInMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

      // Act
      var result = await _distanceService.GetDistanceAsync(sourceAirport.Iata, destinationAirport.Iata, CancellationToken.None);

      // Assert
      result.Should().NotBe(null);
      result.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ShouldReturnDistanceBetweenTwoAirports()
    {
      // Arrange
      var locationModelSochi = _fixture.Build<LocationModel>().With(x => x.Lon, 39.941106).With(x => x.Lat, 43.44884).Create();
      var locationModelUlyanovsk = _fixture.Build<LocationModel>().With(x => x.Lon, 48.226665).With(x => x.Lat, 54.268333).Create();
      var sourceAirport = _fixture.Build<AirportModel>().With(x => x.Iata, "AER").With(x => x.Location, locationModelSochi).Create();
      var destinationAirport = _fixture.Build<AirportModel>().With(x => x.Iata, "ULV").With(x => x.Location, locationModelUlyanovsk).Create();

      _mockAirportService.Setup(s => s.GetAirportInfoAsync(sourceAirport.Iata, CancellationToken.None)).ReturnsAsync(sourceAirport);
      _mockAirportService.Setup(s => s.GetAirportInfoAsync(destinationAirport.Iata, CancellationToken.None)).ReturnsAsync(destinationAirport);
      _mockInMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

      // Act
      var result = await _distanceService.GetDistanceAsync("AER", "ULV", CancellationToken.None);

      // Assert
      result.Should().NotBe(null);
      result.Should().Be(835.757727115659);
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenAiroportDataIsNull()
    {
      // Arrange
      _mockAirportService
        .Setup(s => s.GetAirportInfoAsync(It.IsAny<string>(), CancellationToken.None))
        .ThrowsAsync(new InvalidInputDataException("message"));

      // Act
      var result = () => _distanceService.GetDistanceAsync("testcode1", "testcode2", CancellationToken.None);

      // Assert
      await result.Should().ThrowAsync<InvalidInputDataException>().WithMessage("message");
    }

    [Fact]
    public async Task ShouldThrowNotFoundDataExceptionWhenAiroportIATAIsEmpty()
    {
      // Arrange
      _mockAirportService
        .Setup(s => s.GetAirportInfoAsync(It.IsAny<string>(), CancellationToken.None))
        .ThrowsAsync(new NotFoundDataException("message"));

      // Act
      var result = () => _distanceService.GetDistanceAsync("testcode1", "testcode2", CancellationToken.None);

      // Assert
      await result.Should().ThrowAsync<NotFoundDataException>().WithMessage("message");
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenSomethingWrong()
    {
      // Arrange
      _mockAirportService
        .Setup(s => s.GetAirportInfoAsync(It.IsAny<string>(), CancellationToken.None))
        .ThrowsAsync(new Exception("message"));

      // Act
      var result = () => _distanceService.GetDistanceAsync("testcode1", "testcode2", CancellationToken.None);

      // Assert
      await result.Should().ThrowAsync<Exception>().WithMessage("message");
    }
  }
}
