using AirportDistanceWebAPIService.Extensions;
using AirportDistanceWebAPIService.Infrastructure;
using AirportDistanceWebAPIService.Interfaces;
using AirportDistanceWebAPIService.Services;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddLogging(config =>
    {
      config.AddConsole();
      config.AddDebug();
    });

    builder.Services.AddControllers();
    builder.Services.AddHealthChecks();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();

    builder.Services.AddHttpClientWithPoliceHandler(builder.Configuration);
    builder.Services.AddScoped<IDistanceService, DistanceService>();
    builder.Services.AddScoped<IAirportService, AirportService>();

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.MapControllers();

    app.UseHealthChecks("/health");

    app.Run();
  }
}