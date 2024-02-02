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
        private readonly IFlightFetcher _flightFetcher;

        public FlightWorker(IDbContextFactory<FlightDbContext> contextFactory,
            IHttpClientFactory httpClientFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<FlightWorker> logger, 
            IConfiguration configuration,
            IFlightFetcher flightFetcher)
        {
            _contextFactory = contextFactory;
            _httpClientFactory = httpClientFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
            _flightFetcher = flightFetcher;
        }

        private async Task CreateDb()
        {
            try
            {
                var dbContext = await _contextFactory.CreateDbContextAsync();
                await dbContext.Database.EnsureCreatedAsync();
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
            await CreateDb();

            while (!stoppingToken.IsCancellationRequested)
            {
                IEnumerable<Flight> newFlights = new List<Flight>();

                try
                {
                    newFlights = await _flightFetcher.GetFlightsFromFeederAsync();

                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                }

                try
                {

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

                        //_logger.LogInformation($"Added planes {planesNotExisting.Count()}, added flights: {flightsNotExisting.Count()}");
                    }

                    //_logger.LogInformation("Flights saved to the database.");

                    // Sleep for 5 minutes before fetching data again
                    //await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                }
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }


    }
}
