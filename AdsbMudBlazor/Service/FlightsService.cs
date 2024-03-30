using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsbMudBlazor.Service
{
    public class FlightsService(IDbContextFactory<FlightDbContext> contextFactory, ILogger<FlightsService> logger)
    {
        private readonly ILogger _logger = logger;
        private readonly IDbContextFactory<FlightDbContext> _contextFactory = contextFactory;
        public async Task<List<Flight>> GetFlightsAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Flights.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public IQueryable<Flight> GetFlightsQueryable()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Flights.AsQueryable();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<int> GetFlightsCountAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Flights.CountAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
        public List<Flight> GetRecentFlights(TimeSpan timeSpan)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                DateTime oldest = DateTime.UtcNow.Subtract(timeSpan);

                return context.Flights.Where(f => f.DateTime > oldest).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<List<Flight>> GetRecentFlightsAsync(TimeSpan timeSpan)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                DateTime oldest = DateTime.UtcNow.Subtract(timeSpan);

                return await context.Flights.Where(f => f.DateTime >= oldest).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public List<Plane> GetRecentDistinctPlanes(TimeSpan timeSpan)
        {
            try
            {
                 using var context =  _contextFactory.CreateDbContext();

                DateTime oldest = DateTime.UtcNow.Subtract(timeSpan);

                return context.Planes.Where(p => p.LastSeen >= oldest).Distinct().ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<List<Plane>> GetRecentDistinctPlanesAsync(TimeSpan timeSpan)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                DateTime oldest = DateTime.UtcNow.Subtract(timeSpan);

                return await context.Planes.Where(p => p.LastSeen >= oldest).Distinct().ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<List<Plane>> GetPlanesAsync(Func<Plane, bool> predicate)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return context.Planes.Where(predicate).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
        public async Task<List<Plane>> GetPlanesAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Planes.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<int> GetFlightCountAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Flights.CountAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<int> GetDistinctPlanes()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                if (context.Planes.TryGetNonEnumeratedCount(out int count))
                {
                    return count;
                }
                return await context.Planes.CountAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public int GetNonEnumeratedFlightCount()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                if (context.Flights.TryGetNonEnumeratedCount(out int count))
                {
                    return count;
                }
                return context.Flights.Count();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public List<Flight> GetFlights()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Flights.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public List<Plane> GetPlanes()
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                return context.Planes.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}
