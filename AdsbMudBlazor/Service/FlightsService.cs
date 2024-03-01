﻿using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsbMudBlazor.Service
{
    public class FlightsService(IDbContextFactory<FlightDbContext> contextFactory, ILogger<FlightsService> logger)
    {
        private readonly ILogger _logger = logger;

        public async Task<List<Flight>> GetFlightsAsync()
        {
            try
            {
                await using var context = await contextFactory.CreateDbContextAsync();
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
                using var context =  contextFactory.CreateDbContext();
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
                using var context = await contextFactory.CreateDbContextAsync();
                return await context.Flights.CountAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public Task<List<Flight>> GetRecentFlightsAsync(TimeSpan timeSpan)
        {
            try
            {
                using var context =  contextFactory.CreateDbContext();

                DateTime oldest = DateTime.UtcNow.Subtract(timeSpan);

                return context.Flights.Where(f => f.DateTime > oldest).ToListAsync();
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
                await using var context = await contextFactory.CreateDbContextAsync();
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
                await using var context = await contextFactory.CreateDbContextAsync();
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
                using var context = contextFactory.CreateDbContext();
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
                using var context =  contextFactory.CreateDbContext();
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
                using var context = contextFactory.CreateDbContext();
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
                using var context = contextFactory.CreateDbContext();
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
