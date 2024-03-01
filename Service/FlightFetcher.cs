using System.Globalization;
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

        public async Task<int> GetCurrentlyTrackedFlightsCount(CancellationToken token)
        {
            try
            {
                var response = await _httpClient.GetStreamAsync(_options.FeederUrl, token);

                var document = await JsonDocument.ParseAsync(response, cancellationToken: token);
                var root = document.RootElement;

                if (root.EnumerateObject().TryGetNonEnumeratedCount(out int count)) return count;
                return root.EnumerateObject().Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Flight>> GetFlightsFromFeederAsync(CancellationToken token)
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                var response = await _httpClient.GetStreamAsync(_options.FeederUrl, token);

                //TODO deserialize properly
                var document = await JsonDocument.ParseAsync(response, cancellationToken: token);
                foreach (var property in document.RootElement.EnumerateObject())
                {
                    var flightData = property.Value;
                    var flight = new Flight
                    {
                        ModeS = flightData[0].GetString()!,
                        Callsign = flightData[16].GetString()!,
                        Lat = flightData[1].GetDouble(),
                        Long = flightData[2].GetDouble(),
                        Alt = flightData[4].GetInt32().ToString(),
                        Squawk = flightData[6].GetString()!
                    };
                    flights.Add(flight);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return flights;
        }
    }
}
