using Microsoft.EntityFrameworkCore;

namespace AdsbMudBlazor.Models
{
    [Index(nameof(ModeS), nameof(Callsign), nameof(Alt), nameof(Squawk), nameof(Lat), nameof(Long), nameof(DateTime), IsUnique = true)]
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
        public DateTime DateTime { get; set; }
    }
}