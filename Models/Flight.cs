namespace AdsbMudBlazor.Models
{
    public class Flight
    {   
        public int Id { get; set; } 
        public string ModeS { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;
        public string Squawk { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Long { get; set; }

        public double? Distance { get; set; }
    }
}
