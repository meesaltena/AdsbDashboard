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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FlightFetcher> _logger;
        private readonly FeederOptions _options;

        public FlightFetcher(IHttpClientFactory httpClientFactory, ILogger<FlightFetcher> logger, IOptions<FeederOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<int> GetCurrentlyTrackedFlightsCount()
        {
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.GetStreamAsync(_options.FeederUrl);

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
            List<Flight> flights = new List<Flight>();
            try
            {
                using HttpClient client = _httpClientFactory.CreateClient();
                var response = await client.GetStreamAsync(_options.FeederUrl);

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
                _logger.LogError(e.ToString());
            }

            return flights;
        }
    }
}
