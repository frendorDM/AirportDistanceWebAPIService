namespace AirportDistanceWebAPIService.Models
{
  public record AirportModel(
    string Iata,
    string Name,
    string City,
    string CityIata,
    string Icao,
    string Country,
    string CountryIata,
    LocationModel Location,
    int Rating,
    int Hubs,
    string TimezoneRegionName,
    string Type
  );
}
