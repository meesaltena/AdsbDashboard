
using System.Drawing;
using System.Globalization;

namespace AdsbMudBlazor.Service
{
    public class FeederService(IConfiguration configuration)
    {
        //private readonly IConfiguration _configuration = configuration.;
        public string? FeederUrl => configuration["FeederUrl"];
        public string? FeederId => configuration["FeederId"];
        public string? FeederName => configuration["FeederName"];

        // TODO; when not set, use browser to get lan lon instead

        public double FeederLat => double.Parse(configuration["FeederLat"]!,CultureInfo.InvariantCulture);

        public double FeederLong => double.Parse(configuration["FeederLong"]!,CultureInfo.InvariantCulture);

        public (double, double) FeederLocation => (FeederLat, FeederLong);
    }
}
