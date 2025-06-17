using System.Net;
using System.Text.Json;
using AirportDistanceWebAPIService.Models;
using AirportDistanceWebAPIService.Services;

namespace AirportDistanceWebAPIService.Infrastructure
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<DistanceService> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<DistanceService> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (InvalidInputDataException ex)
      {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new DistanceResponse { Success = false, ErrorMessage = ex.Message });
      }
      catch (NotFoundDataException ex)
      {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new DistanceResponse { Success = false, ErrorMessage = ex.Message });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An unexpected error occurred while fetching airport info.");
        await HandleExceptionAsync(context);
      }
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

      var response = new
      {
        StatusCode = context.Response.StatusCode,
        Message = "An unexpected error occurred."
      };

      var json = JsonSerializer.Serialize(response);

      return context.Response.WriteAsync(json);
    }
  }
}
