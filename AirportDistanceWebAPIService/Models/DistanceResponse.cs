namespace AirportDistanceWebAPIService.Models
{
  public class DistanceResponse
  {
    public double Distance { get; set; }

    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }
  }
}