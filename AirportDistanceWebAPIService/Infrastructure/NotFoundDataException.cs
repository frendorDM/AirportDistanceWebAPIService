namespace AirportDistanceWebAPIService.Infrastructure
{
  public class NotFoundDataException : Exception
  {
    public NotFoundDataException(string message)
      : base (message)
    {
      
    }
  }
}
