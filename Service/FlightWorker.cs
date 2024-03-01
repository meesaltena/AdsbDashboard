using System.Globalization;
using System.Text.Json;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdsbMudBlazor.Service
{
    public class FlightWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDbContextFactory<FlightDbContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly FeederOptions _options;

        public FlightWorker(IDbContextFactory<FlightDbContext> contextFactory,
            IServiceScopeFactory serviceScopeFactory, 
            ILogger<FlightWorker> logger, 
            IOptions<FeederOptions> options)
        {
            _contextFactory = contextFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _options = options.Value;
        }

        private async Task<bool> CreateDb()
        {
            try
            {
                var dbContext = await _contextFactory.CreateDbContextAsync();
                return await dbContext.Database.EnsureCreatedAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("config: {0}", _options);
                _logger.LogError(e.Message);
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await CreateDb()) return;

            while (!stoppingToken.IsCancellationRequested)
            {
                IEnumerable<Flight> newFlights = new List<Flight>();

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var _flightFetcher = scope.ServiceProvider.GetRequiredService<IFlightFetcher>();
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
                    }

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
