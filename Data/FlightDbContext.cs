using AdsbMudBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsbMudBlazor.Data
{
    public class FlightDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FlightDbContext> _logger;
        public FlightDbContext(DbContextOptions<FlightDbContext> options, IConfiguration configuration, ILogger<FlightDbContext> logger)
            : base(options)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Plane> Planes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = _configuration.GetValue<string>("DatabasePath");
            if (dbPath == null)
            {
                dbPath = "flights.db";
                _logger.LogWarning("No DatabasePath path provided! using fallback {0}", dbPath);
            }
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
