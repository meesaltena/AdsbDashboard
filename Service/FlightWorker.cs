using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using AdsbMudBlazor.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static System.Formats.Asn1.AsnWriter;

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
                _logger.LogInformation("Ensuring database created");
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
            await CreateDb();
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var startTime = DateTime.Now;
                    var endTime = startTime.AddMilliseconds((1000 * _options.WorkerInterval));

                    await UpdateFlightsSimple(stoppingToken);
                    //await UpdateFlights(stoppingToken);

                    var timeToDelayLeft = endTime.Subtract(startTime);
                    timeToDelayLeft = (timeToDelayLeft < TimeSpan.FromSeconds(5)) ? TimeSpan.FromSeconds(5) : timeToDelayLeft;

                    _logger.LogInformation($"FlightWorker delaying: {timeToDelayLeft}");
                    await Task.Delay(timeToDelayLeft, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"FlightWorker: An error occurred: {ex.Message}");
                throw;
            }

            _logger.LogInformation($"FlightWorker stopping!");
        }

        private async Task UpdateFlightsSimple(CancellationToken token)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();

            IFlightFetcher _flightFetcher = scope.ServiceProvider.GetRequiredService<IFlightFetcher>();

            var newFlights = await _flightFetcher.GetFlightsFromFeederAsync(token);

            await using (var dbContext = await _contextFactory.CreateDbContextAsync(token))
            {
                await dbContext.Flights.AddRangeAsync(newFlights, cancellationToken:token);
                var flightsInserted = await dbContext.SaveChangesAsync(token);

                await dbContext.Planes.AddRangeAsync(newFlights.Select(p => new Plane()
                {
                    ModeS = p.ModeS,
                    LastSeen = DateTime.UtcNow,
                }));
                var planesInserted = await dbContext.SaveChangesAsync(token);

                _logger.LogInformation($"--------------- flightsInserted: {flightsInserted} planesInserted: {planesInserted}");
            }

        }

        private async Task UpdateFlights(CancellationToken token)
        {
            IEnumerable<Flight> newFlights = new List<Flight>();
            ICoordUtils _coordUtils;

            using var scope = _serviceScopeFactory.CreateScope();
            var _flightFetcher = scope.ServiceProvider.GetRequiredService<IFlightFetcher>();
            _coordUtils = scope.ServiceProvider.GetRequiredService<ICoordUtils>();
            newFlights = await _flightFetcher.GetFlightsFromFeederAsync(token);


            await using (var dbContext = await _contextFactory.CreateDbContextAsync(token))
            {
                var flightsNotExisting = newFlights.Where(f => !dbContext.Flights.Contains(f));

                _logger.LogInformation($"");
                await dbContext.Flights.AddRangeAsync(flightsNotExisting);

                var planesNotExisting = newFlights
                    .Where(f => dbContext.Planes.All(p => p.ModeS != f.ModeS))
                    .Select(fl => new Plane() { ModeS = fl.ModeS });


                _logger.LogInformation($"-------------------------------------- newFlights: {newFlights.Count()} planesNotExisting: {planesNotExisting.Count()}, flightsNotExisting: {flightsNotExisting.Count()}");

                await dbContext.Planes.AddRangeAsync(planesNotExisting);
                await dbContext.SaveChangesAsync(token);


                // loop through all flights in db whose modeS appears in list of flights currently being trackednewFlights
                var updated = new List<Flight>();
                foreach (var flight in dbContext.Flights.Where(f => newFlights.Select(j => j.ModeS).ToList().Contains(f.ModeS)))
                {
                    //TODO  check if lat long not null?
                    flight.Distance = _coordUtils.GetDistance(_options.FeederLat, _options.FeederLong, flight.Lat, flight.Long);
                    updated.Add(flight);
                }

                dbContext.UpdateRange(updated);
                var c = await dbContext.SaveChangesAsync(token);
                _logger.LogInformation($"Updated : {c}");
            }
        }
    }
}
