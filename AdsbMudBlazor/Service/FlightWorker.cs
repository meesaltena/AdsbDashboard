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
                _logger.LogError(_options.ToString(), "config: {0}");
                _logger.LogError(e, "CreateDb Exception: ");
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

                    await UpdateFlightsAndPlanes(stoppingToken);


                    var timeToDelayLeft = endTime.Subtract(startTime);
                    timeToDelayLeft = (timeToDelayLeft < TimeSpan.FromSeconds(5)) ? TimeSpan.FromSeconds(5) : timeToDelayLeft;

                    _logger.LogInformation($"FlightWorker delaying: {timeToDelayLeft}");
                    await Task.Delay(timeToDelayLeft, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FlightWorker UpdateFlightsSimple: An error occurred:");
                throw;
            }

            _logger.LogInformation($"FlightWorker stopping!");
        }

        private async Task UpdateFlightsAndPlanes(CancellationToken token)
        {

            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                IFlightFetcher _flightFetcher = scope.ServiceProvider.GetRequiredService<IFlightFetcher>();
                ICoordUtils coordUtils = scope.ServiceProvider.GetRequiredService<ICoordUtils>();

                var currentFlights = await _flightFetcher.GetFlightsFromFeederAsync(token);


                await InsertOrUpdateFlights(currentFlights, token);
                await InsertOrUpdatePlanes(currentFlights, token);
                await UpdateFlightDistance(currentFlights, token, coordUtils);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "FlightWorker UpdateFlightsSimple: An error occurred:");
                throw;
            }

        }

        private async Task UpdateFlightDistance(IEnumerable<Flight> flights, CancellationToken token, ICoordUtils coordUtils)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync(token))
            {
                foreach (var flight in flights)
                {
                    IQueryable<Flight> match = dbContext.Flights.Where(fl => fl.Equals(flight));
                    
                    Debug.Assert(match.Count() == 1);

                    var f = match.FirstOrDefault();

                    f.Distance = coordUtils.GetDistanceOrZero(f.Lat, f.Long);

                    //await match.ExecuteUpdateAsync(setters => setters
                    //    .SetProperty(f => f.Distance, f => coordUtils.GetDistanceOrZero(f.Lat, f.Long)));
                }

                await dbContext.SaveChangesAsync();
            }
        }
        private async Task InsertOrUpdateFlights(IEnumerable<Flight> flights, CancellationToken token)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync(token))
            {
                IEnumerable<Flight> notAlreadyExistingFlights = flights.Where(flight => !dbContext.Flights.Any(cf => cf.Equals(flight)));

                await dbContext.Flights.AddRangeAsync(notAlreadyExistingFlights, cancellationToken: token);
                var flightsInserted = await dbContext.SaveChangesAsync(token);
                _logger.LogInformation($"Not already existing: {notAlreadyExistingFlights.Count()}. Already existing: {dbContext.Flights.Count() - notAlreadyExistingFlights.Count()}, inserted: {flightsInserted}");
            }
        }

        private async Task InsertOrUpdatePlanes(IEnumerable<Flight> flights, CancellationToken token)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync(token))
            {
                foreach (var flight in flights)
                {
                    var match = await dbContext.Planes.FirstOrDefaultAsync(p => p.ModeS == flight.ModeS, cancellationToken: token);

                    if (match != null)
                    {
                        match.LastSeen = DateTime.UtcNow;
                    }
                    else
                    {
                        dbContext.Planes.Add(
                            new Plane()
                            {
                                ModeS = flight.ModeS,
                                LastSeen = DateTime.UtcNow
                            });
                    }
                }
                await dbContext.SaveChangesAsync(token);
            }
        }
    }
}
