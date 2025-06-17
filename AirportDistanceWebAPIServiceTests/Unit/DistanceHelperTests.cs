using AirportDistanceWebAPIService.Helpers;
using AirportDistanceWebAPIService.Models;
using FluentAssertions;

namespace AirportDistanceWebAPIServiceTests.Unit
{
  public class DistanceHelperTests
  {
    [Theory]
    [InlineData(43.44884, 39.941106, 54.268333, 48.226665, 835.76)] 
    [InlineData(51.469603, -0.453566, 40.689071, -74.178753, 3456.80)]
    public void ShouldReturnDistance(
      double lat1, double lon1, double lat2, double lon2, double expectedDistance)
    {
      // Arrange
      var point1 = new AirportCoordinates(lat1, lon1);
      var point2 = new AirportCoordinates(lat2, lon2);

      // Act
      var result = DistanceHelper.CalculateHaversineDistance(point1, point2);

      // Assert
      result.Should().BeApproximately(expectedDistance, 0.2);
    }

    [Fact]
    public void ShouldReturnZeroWhenSameCoordinates()
    {
      // Arrange
      var coordinate = new AirportCoordinates(40.7128, -74.0060);

      // Act
      var result = DistanceHelper.CalculateHaversineDistance(coordinate, coordinate);

      // Assert
      result.Should().Be(0);
    }

    [Fact]
    public void ShouldReturnZeroWhenAllCoordinatesAreZero()
    {
      // Arrange
      var point1 = new AirportCoordinates(0, 0);
      var point2 = new AirportCoordinates(0, 0);

      // Act
      var result = DistanceHelper.CalculateHaversineDistance(point1, point2);

      // Assert
      result.Should().Be(0);
    }

    [Fact]
    public void ShouldReturnResultWhenMoreThanZeroWhenHalfOfCoordinatesAreZero()
    {
      // Arrange
      var point1 = new AirportCoordinates(0, 0);
      var point2 = new AirportCoordinates(40.7128, -74.0060);

      // Act
      var result = DistanceHelper.CalculateHaversineDistance(point1, point2);

      // Assert
      result.Should().BeGreaterThan(0);
    }
  }
}
