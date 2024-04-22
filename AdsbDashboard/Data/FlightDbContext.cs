using AdsbDashboard.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdsbDashboard.Data
{
    public class FlightDbContext : DbContext
    {
        private readonly FeederOptions _feederOptions;
        private readonly DbOptions _dbOptions;
        private readonly ILogger<FlightDbContext> _logger;

        public FlightDbContext(DbContextOptions<FlightDbContext> dbContextOptions, IOptions<FeederOptions> feederOptions, IOptions<DbOptions> dbOptions, ILogger<FlightDbContext> logger)
            : base(dbContextOptions)
        {
            _logger = logger;
            _feederOptions = feederOptions.Value;
            _dbOptions = dbOptions.Value;
        }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Plane> Planes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string dbPath = _dbOptions.DatabasePath;
            //if (string.IsNullOrEmpty(dbPath))
            //{
            //    dbPath = "flights.db";
            //    _logger.LogWarning("No DatabasePath path provided! using fallback {0}", dbPath);
            //}
            //optionsBuilder.UseSqlite($"Data Source={dbPath}");

        }
    }
}
