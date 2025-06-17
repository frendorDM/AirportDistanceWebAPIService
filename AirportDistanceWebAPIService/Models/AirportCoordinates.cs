namespace AirportDistanceWebAPIService.Models
{
  public struct AirportCoordinates
  {
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public AirportCoordinates(double latitude, double longitude)
    {
      Latitude = latitude;
      Longitude = longitude;
    }
  }
}
