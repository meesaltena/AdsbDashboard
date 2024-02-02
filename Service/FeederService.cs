namespace AdsbMudBlazor.Service
{
    public class FeederService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration.GetSection("Feeder");
        public string? FeederUrl => _configuration["FeederUrl"];
        public string? FeederId => _configuration["FeederId"];
        public string? FeederName => _configuration["FeederName"];
        
    }
}
