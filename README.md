# AirportDistanceWebAPIService

## Overview
This API calculates the distance between two airports based on their IATA codes and returns the result in miles. The service uses Haversine formula to compute the distance and caches airport coordinates for subsequent requests.

## Features
- Distance calculation between airports by IATA code.
- In-memory cache to store airport coordinates for fast retrieval.
- Validation for valid IATA codes.

## API Endpoints

### GET /api/distance
Calculates the distance between two airports.

#### Query Parameters:
- `sourceIata` (string, required): The IATA code of the source airport.
- `destinationIata` (string, required): The IATA code of the destination airport.

#### Response:
Returns a JSON object with the following fields:
- `distance` (double): The calculated distance in miles.
- `success` (bool): Indicates whether the request was successful.
- `errorMessage` (string, optional): An error message, if applicable.

Example response:
```json
{
  "distance": 500,
  "success": true,
  "errorMessage": null
}


## Authors

- [@octokatherine](https://www.github.com/octokatherine)

Denis Martyanov