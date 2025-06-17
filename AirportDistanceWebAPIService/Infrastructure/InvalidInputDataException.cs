namespace AirportDistanceWebAPIService.Infrastructure
{
  public class InvalidInputDataException : Exception
  {
    public InvalidInputDataException(string message)
      : base (message)
    {
      
    }
  }
}
