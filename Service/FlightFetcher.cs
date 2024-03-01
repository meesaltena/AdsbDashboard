﻿using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using AdsbMudBlazor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdsbMudBlazor.Service
{
    public class FlightFetcher : IFlightFetcher
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<FlightFetcher> _logger;
        private readonly FeederOptions _options;

        public FlightFetcher(HttpClient httpClient, ILogger<FlightFetcher> logger, IOptions<FeederOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<int> GetCurrentlyTrackedFlightsCount()
        {
            try
            {
                var response = await _httpClient.GetStreamAsync(_options.FeederUrl);

                var document = await JsonDocument.ParseAsync(response);
                var root = document.RootElement;

                return root.EnumerateObject().Count();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsFromFeederAsync()
        {
           /* List<Flight> flights = new List<Flight>()*/;
            try
            {
                //var response = await _httpClient.GetStreamAsync(_options.FeederUrl);
                List<Flight>? flights = _httpClient.GetFromJsonAsAsyncEnumerable<Flight>(_options.FeederUrl);

                var document = await JsonDocument.ParseAsync(response);
                var root = document.RootElement;
                

                //foreach (var property in root.EnumerateObject())
                //{
                //    var flightData = property.Value;
                //    var flight = new Flight
                //    {
                //        ModeS = flightData[0].GetString()!,
                //        Callsign = flightData[16].GetString()!,
                //        Lat = flightData[1].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                //        Long = flightData[2].GetDouble().ToString(CultureInfo.InvariantCulture)!,
                //        Alt = flightData[4].GetInt32().ToString(),
                //        Squawk = flightData[6].GetString()!
                //    };
                //    flights.Add(flight);

                //    //_logger.LogInformation($"ModeS: {flight.ModeS}, Callsign: {flight.Callsign}, Lat: {flight.Lat}, Long: {flight.Long}, Alt: {flight.Alt}, SQW: {flight.Squawk}");
                //}
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return flights;
        }
    }
}
