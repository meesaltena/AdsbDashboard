using System.Globalization;
using System.Text.Json;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsbMudBlazor.Service
{
    public class FlightWorker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDbContextFactory<FlightDbContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public FlightWorker(IDbContextFactory<FlightDbContext> contextFactory, IHttpClientFactory httpClientFactory, IServiceScopeFactory serviceScopeFactory, ILogger<FlightWorker> logger, IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _httpClientFactory = httpClientFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration.GetSection("Feeder");
            CreateDb();
        }

        public void CreateDb()
        {
            try
            {
                var dbContext = _contextFactory.CreateDbContext();
                dbContext.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                _logger.LogError("config: {0}", _configuration);
                _logger.LogError(e.Message);
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    List<Flight> newFlights = GetFlightsFromFeeder().ToList();

                    await using (var dbContext = await _contextFactory.CreateDbContextAsync(stoppingToken))
                    {


                        var currFlights = dbContext.Flights.ToList();
                        var currPlanes = dbContext.Planes.ToList();

                        var flightsNotExisting = newFlights
                            .Where(f => !currFlights.Contains(f))
                            .ToList();

                        dbContext.Flights.AddRange(flightsNotExisting);

                        var planesNotExisting = newFlights
                            .Where(f => currPlanes.All(p => p.ModeS != f.ModeS))
                            .Select(fl => new Plane() { ModeS = fl.ModeS }).ToList();


                        dbContext.Planes.AddRange(planesNotExisting);

                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"Added planes {planesNotExisting.Count()}, added flights: {flightsNotExisting.Count()}");
                    }

                    //_logger.LogInformation("Flights saved to the database.");

                    // Sleep for 5 minutes before fetching data again
                    //await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        public IEnumerable<Flight> GetFlightsFromFeeder()
        {
            var client = _httpClientFactory.CreateClient();
            var response = client.GetStringAsync(_configuration["FeederUrl"]).Result;

            var document = JsonDocument.Parse(response);
            var root = document.RootElement;

            List<Flight> flights = new List<Flight>();
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

            return flights;
        }
    }
}
