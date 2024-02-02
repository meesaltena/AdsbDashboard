﻿using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using AdsbMudBlazor.Models;
using Microsoft.Extensions.Logging;

namespace AdsbMudBlazor.Service
{
    public class FlightFetcher(IHttpClientFactory httpClientFactory, ILogger<FlightFetcher> logger, IConfiguration configuration) : IFlightFetcher
    {
        public IEnumerable<Flight> GetFlightsFromFeeder()
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = client.GetStringAsync(configuration["FeederUrl"]).Result;

                var document = JsonDocument.Parse(response);
                var root = document.RootElement;

                //TODO use proper parsing
                foreach (var property in root.EnumerateObject())
                {
                    var flightData = property.Value;
                    var flight = new Flight
                    {
                        ModeS = flightData[0].GetString()!,
                        Callsign = flightData[16].GetString()!,
                        Lat = flightData[1].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                        Long = flightData[2].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                        Alt = flightData[4].GetInt32().ToString(),
                        Squawk = flightData[6].GetString()!
                    };
                    flights.Add(flight);

                    //_logger.LogInformation($"ModeS: {flight.ModeS}, Callsign: {flight.Callsign}, Lat: {flight.Lat}, Long: {flight.Long}, Alt: {flight.Alt}, SQW: {flight.Squawk}");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.GetBaseException().Message);
                throw;
            }

            return flights;
        }

        public async Task<IEnumerable<Flight>> GetFlightsFromFeederAsync()
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                var client = httpClientFactory.CreateClient();
                var response = await client.GetStreamAsync(configuration["FeederUrl"]);

                var document = await JsonDocument.ParseAsync(response);
                var root = document.RootElement;


                foreach (var property in root.EnumerateObject())
                {
                    var flightData = property.Value;
                    var flight = new Flight
                    {
                        ModeS = flightData[0].GetString()!,
                        Callsign = flightData[16].GetString()!,
                        Lat = flightData[1].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                        Long = flightData[2].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                        Alt = flightData[4].GetInt32().ToString(),
                        Squawk = flightData[6].GetString()!
                    };
                    flights.Add(flight);

                    //_logger.LogInformation($"ModeS: {flight.ModeS}, Callsign: {flight.Callsign}, Lat: {flight.Lat}, Long: {flight.Long}, Alt: {flight.Alt}, SQW: {flight.Squawk}");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.GetBaseException().Message);
                throw;
            }

            return flights;
        }
    }
}
