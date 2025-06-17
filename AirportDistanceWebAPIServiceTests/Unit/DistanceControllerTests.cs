using AirportDistanceWebAPIService.Controllers;
using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirportDistanceWebAPIServiceTests.Unit
{
  public class DistanceControllerTests
  {
    private readonly Mock<IDistanceService> _mockDistanceService;
    private readonly DistanceController _controller;

    private readonly string IATASochi = "AER";
    private readonly string IATAUlyanovsk = "ULV";
    private readonly string IATAFake = "III";
    private readonly double ExpectedDistance = 100;

    public DistanceControllerTests()
    {
      _mockDistanceService = new Mock<IDistanceService>();
      _controller = new DistanceController(_mockDistanceService.Object);
    }

    [Fact]
    public async Task ShouldReturnDistanceBetweenAERandULV()
    {
      // Arrange
      _mockDistanceService.Setup(s => s.GetDistanceAsync(IATASochi, IATAUlyanovsk, It.IsAny<CancellationToken>())).ReturnsAsync(ExpectedDistance);

      // Act
      var actionResult = await _controller.GetDistance(IATASochi, IATAUlyanovsk, CancellationToken.None);

      // Assert
      actionResult.Result.Should().BeOfType<OkObjectResult>()
        .Which.Value.Should().BeOfType<DistanceResponse>()
        .Which.Should().Match<DistanceResponse>(x =>
          x.Success == true
          && x.ErrorMessage == null
          && x.Distance == ExpectedDistance);
    }

    [Fact]
    public async Task ShouldThrowExceptionIfIataCodesAreEmpty()
    {
      // Arrange
      string emptyIata = "";

      // Act
      var actionResult = async () => await _controller.GetDistance(emptyIata, IATAUlyanovsk, CancellationToken.None);

      // Assert
      await actionResult.Should().ThrowAsync<InvalidInputDataException>()
        .WithMessage("Both IATA codes must be provided.");
    }

    [Fact]
    public async Task ThrowInvalidOperationExceptionWhenDistanceServiceReturnInvalidOperationException()
    {
      // Arrange
      _mockDistanceService
        .Setup(s => s.GetDistanceAsync(IATAFake, IATAFake, It.IsAny<CancellationToken>()))
        .ThrowsAsync(new InvalidInputDataException("message"));
      // Act
      var actionResult = async () => await _controller.GetDistance(IATAFake, IATAFake, CancellationToken.None);

      // Assert
      await actionResult.Should().ThrowAsync<InvalidInputDataException>()
        .WithMessage("message");
    }

    [Fact]
    public async Task ThrowNotFoundDataExceptionWhenDistanceServiceReturnsNotFound()
    {
      // Arrange
      _mockDistanceService
        .Setup(s => s.GetDistanceAsync(IATAFake, IATAFake, It.IsAny<CancellationToken>()))
        .ThrowsAsync(new NotFoundDataException("message"));
      // Act
      var actionResult = async () => await _controller.GetDistance(IATAFake, IATAFake, CancellationToken.None);

      // Assert
      await actionResult.Should().ThrowAsync<NotFoundDataException>()
        .WithMessage("message");
    }
  }
}
