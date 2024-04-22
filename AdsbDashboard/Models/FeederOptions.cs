namespace AdsbDashboard.Models
{
    public class FeederOptions
    {
        public const string Position = "FeederOptions";
        public string FeederUrl { get; set; } = string.Empty;
        public string FeederId { get; set; } = string.Empty ;
        public string FeederName { get; set; } = string.Empty;

        public int WorkerInterval { get; set; } = 60;
        public double FeederLat { get; set; }
        public double FeederLong { get; set; }
    }
}
