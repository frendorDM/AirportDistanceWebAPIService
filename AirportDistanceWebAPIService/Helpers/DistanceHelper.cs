using AirportDistanceWebAPIService.Models;

namespace AirportDistanceWebAPIService.Helpers
{
  public static class DistanceHelper
  {
    const double EarthRadius = 3958.8;

    public static double CalculateHaversineDistance(AirportCoordinates point1, AirportCoordinates point2)
    {
      double dLat = DegreesToRadians(point2.Latitude - point1.Latitude);
      double dLon = DegreesToRadians(point2.Longitude - point1.Longitude);

      double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                 Math.Cos(DegreesToRadians(point1.Latitude)) * Math.Cos(DegreesToRadians(point2.Latitude)) *
                 Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

      double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

      return EarthRadius * c;
    }

    public static double DegreesToRadians(double degrees)
    {
      return degrees * Math.PI / 180.0;
    }
  }
}
