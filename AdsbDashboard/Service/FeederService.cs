
using AdsbDashboard.Models;
using Microsoft.Extensions.Options;

namespace AdsbDashboard.Service
{
    public class FeederService
    {
        private readonly FeederOptions _options;

        public FeederService(IOptions<FeederOptions> options)
        {
            _options = options.Value;
        }

        public string FeederUrl => _options.FeederUrl;
        public string FeederId => _options.FeederUrl;
        public string FeederName => _options.FeederUrl;

        // TODO; when FeederLatLon not set, use browser to get lan lon instead
        public double FeederLat => _options.FeederLat;
        public double FeederLong => _options.FeederLong;
    }
}
