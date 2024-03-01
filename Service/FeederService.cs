
using System.Drawing;
using System.Globalization;

namespace AdsbMudBlazor.Service
{
    public class FeederService
    {
        //private readonly IConfiguration _configuration = configuration.;

        public FeederService(IConfiguration configuration)
        {
            FeederUrl = configuration.GetValue<string>("FeederUrl") ?? throw new ArgumentNullException();
            FeederId = configuration["FeederId"] ?? throw new ArgumentNullException(); 
            FeederName = configuration["FeederName"] ?? throw new ArgumentNullException();

            FeederLat = double.Parse(configuration["FeederLat"] ?? throw new ArgumentNullException(), CultureInfo.InvariantCulture);
            FeederLong = double.Parse(configuration["FeederLong"] ?? throw new ArgumentNullException(), CultureInfo.InvariantCulture);
        }

        public string FeederUrl { get; set; }
        public string FeederId { get; set; }
        public string FeederName { get; set; }



        // TODO; when FeederLatLon not set, use browser to get lan lon instead
        public double FeederLat { get; set; }
        public double FeederLong { get; set; }
    }
}
